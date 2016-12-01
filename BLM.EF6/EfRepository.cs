using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;

using BLM.Extensions;
using Microsoft.Win32;

namespace BLM.EF6
{
    public class EfRepository<T> : IRepository<T> where T : class, new()
    {

        private readonly DbContext _dbcontext;
        private readonly DbSet<T> _dbset;

        private readonly PropertyInfo _logicalDelete;

        public EfRepository(DbContext db)
        {
            _dbcontext = db;
            _dbset = db.Set<T>();
            // TODO: What about the inherited classes? 
            _logicalDelete = typeof(T).GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(LogicalDeleteAttribute), true).Any());
        }

        private EfContextInfo GetContextInfo(IIdentity user)
        {
            return new EfContextInfo(user, _dbcontext);
        }

        private async Task<AuthorizationResult> AuthorizeAddAsync(IIdentity usr, T newEntity)
        {
            _dbset.Attach(newEntity);
            var authResult = (await Authorize.CreateAsync(newEntity, GetContextInfo(usr))).CreateAggregateResult();
            if (!authResult.HasSucceed)
            {
                await Listen.CreateFailedAsync(newEntity, GetContextInfo(usr));
                _dbcontext.Entry(newEntity).State = EntityState.Detached;
            }

            return authResult;
        }

        public void Add(IIdentity user, T newItem)
        {
            AddAsync(user, newItem).Wait();
        }

        public async Task AddAsync(IIdentity user, T newItem)
        {
            var result = await AuthorizeAddAsync(user, newItem);

            if (!result.HasSucceed)
            {
                throw new AuthorizationFailedException(result);
            }
            _dbset.Add(newItem);
        }

        public void AddRange(IIdentity user, IEnumerable<T> newItems)
        {
            AddRangeAsync(user, newItems).Wait();
        }

        public async Task AddRangeAsync(IIdentity user, IEnumerable<T> newItems)
        {
            var fails = new List<AuthorizationResult>();

            var newItemlist = newItems.ToList();
            foreach (var item in newItemlist)
            {
                var result = await AuthorizeAddAsync(user, item);
                if (!result.HasSucceed)
                {
                    fails.Add(result);
                }
            }

            if (fails.Any())
            {
                throw new AuthorizationFailedException(fails.CreateAggregateResult());
            }

            _dbset.AddRange(newItemlist);
        }


        public void Dispose()
        {
            _dbcontext?.Dispose();
        }

        public IQueryable<T> Entities(IIdentity user)
        {
            return Authorize.Collection(_dbset, GetContextInfo(user));
        }


        public void Remove(IIdentity usr, T item)
        {
            RemoveAsync(usr, item).Wait();
        }

        public async Task RemoveAsync(IIdentity user, T item)
        {
            var result = (await Authorize.RemoveAsync(item, GetContextInfo(user))).CreateAggregateResult();
            if (!result.HasSucceed)
            {
                throw new AuthorizationFailedException(result);
            }
            _dbset.Remove(item);
        }

        public void RemoveRange(IIdentity usr, IEnumerable<T> items)
        {
            RemoveRangeAsync(usr, items).Wait();
        }

        public async Task RemoveRangeAsync(IIdentity user, IEnumerable<T> items)
        {
            var fails = new List<AuthorizationResult>();
            var entityList = items.ToList();
            foreach (var entity in entityList)
            {
                var result = (await Authorize.RemoveAsync(entity, GetContextInfo(user))).CreateAggregateResult();
                if (!result.HasSucceed)
                {
                    fails.Add(result);
                }
            }

            if (fails.Any())
            {
                var aggregated = fails.CreateAggregateResult();
                throw new AuthorizationFailedException(aggregated);
            }

            _dbset.RemoveRange(entityList);
        }

        private AuthorizationResult AuthorizeEntityChange(IIdentity user, DbEntityEntry ent)
        {
            return AuthorizeEntityChangeAsync(user, ent).Result;
        }

        private async Task<AuthorizationResult> AuthorizeEntityChangeAsync(IIdentity user, DbEntityEntry ent)
        {

            if (ent.State == EntityState.Unchanged || ent.State == EntityState.Detached)
                return AuthorizationResult.Success();

            if (ent.Entity is T)
            {
                var casted = ent.Cast<T>();
                switch (ent.State)
                {
                    case EntityState.Added:
                        T interpreted = Interpret.BeforeCreate(casted.Entity, GetContextInfo(user));
                        return (await Authorize.CreateAsync(interpreted, GetContextInfo(user))).CreateAggregateResult();

                    case EntityState.Modified:
                        var original = await CreateWithValuesAsync(casted.OriginalValues);
                        var modified = await CreateWithValuesAsync(casted.CurrentValues);
                        var modifiedInterpreted = Interpret.BeforeModify(original, modified, GetContextInfo(user));
                        foreach (var field in ent.CurrentValues.PropertyNames)
                        {
                            ent.CurrentValues[field] = modifiedInterpreted.GetType().GetProperty(field).GetValue(modifiedInterpreted, null);
                        }
                        return (await Authorize.ModifyAsync(original, modifiedInterpreted, GetContextInfo(user))).CreateAggregateResult();
                    case EntityState.Deleted:
                        return (await Authorize.RemoveAsync(await CreateWithValuesAsync(casted.OriginalValues, casted.Entity.GetType()), GetContextInfo(user))).CreateAggregateResult();
                    default:
                        return AuthorizationResult.Fail("The entity state is invalid", casted.Entity);
                }
            }
            else
            {
                return AuthorizationResult.Fail($"Changes for entity type '{ent.Entity.GetType().FullName}' is not supported in a context of a repository with type '{typeof(T).FullName}'", ent.Entity);
            }
        }
        private static async Task<T> CreateWithValuesAsync(DbPropertyValues values, Type type = null)
        {
            return await Task.Factory.StartNew(() =>
            {

                if (type == null)
                {
                    type = typeof(T);
                }

                T entity = (T)Activator.CreateInstance(type);

                foreach (var name in values.PropertyNames)
                {
                    var value = values.GetValue<object>(name);
                    var property = type.GetProperty(name);

                    if (value != null)
                    {
                        property.SetValue(entity, Convert.ChangeType(value, property.PropertyType), null);
                    }
                }

                return entity;
            });
        }



        public void SaveChanges(IIdentity user)
        {
            SaveChangesAsync(user).Wait();
        }

        public async Task SaveChangesAsync(IIdentity user)
        {
            var contextInfo = GetContextInfo(user);

            _dbcontext.ChangeTracker.DetectChanges();
            List<DbEntityEntry> entries = _dbcontext.ChangeTracker.Entries().ToList();

            foreach (var entityChange in _dbcontext.ChangeTracker.Entries())
            {
                var authResult = await AuthorizeEntityChangeAsync(user, entityChange);
                if (!authResult.HasSucceed)
                {
                    if (entityChange.State == EntityState.Modified)
                    {
                        await Listen.ModificationFailedAsync(await CreateWithValuesAsync(entityChange.OriginalValues), entityChange.Entity as T, GetContextInfo(user));
                    }
                    else if (entityChange.State == EntityState.Deleted)
                    {
                        await Listen.RemoveFailedAsync(await CreateWithValuesAsync(entityChange.OriginalValues), contextInfo);
                    }
                    throw new AuthorizationFailedException(authResult);
                }
            }

            List<T> removed = null;

            List<T> added =
                entries.Where(a => a.State == EntityState.Added).Select(a => SelectCurrentAsync(a).Result).ToList();
            List<dynamic> modified =
                entries.Where(a => a.State == EntityState.Modified).Select(a => SelectBothAsync(a).Result).ToList();

            if (_logicalDelete == null)
            {
                // Just add the Deleted entities to the removed list;
                removed = entries.Where(a => a.State == EntityState.Deleted)
                            .Select(a => SelectOriginalAsync(a).Result)
                            .ToList();
            }
            else
            {
                List<DbEntityEntry> logicalRemoved = entries.Where(a => a.State == EntityState.Deleted).ToList();
                logicalRemoved.ForEach(async entry =>
                {
                    await entry.ReloadAsync();
                    entry.State = EntityState.Modified;
                    entry.Property(_logicalDelete.Name).CurrentValue = true;
                });
                modified.AddRange(logicalRemoved.Select(a => SelectBothAsync(a).Result));
            }

            _dbcontext.SaveChanges();

            added.ForEach(async a => await Listen.CreatedAsync(a, contextInfo));
            modified.ForEach(async a => await Listen.ModifiedAsync(a.OriginalValues, a.CurrentValues, contextInfo));

            if (this._logicalDelete == null)
            {
                removed?.ForEach(async a => await Listen.RemovedAsync(a, contextInfo));
            }
        }

        public void SetEntityState(T entity, EntityState newState)
        {
            _dbcontext.Entry(entity).State = newState;
        }

        private static async Task<T> SelectCurrentAsync(DbEntityEntry a, Type type = null)
        {
            if (type == null)
            {
                type = a.Entity.GetType();
            }
            return await CreateWithValuesAsync(a.CurrentValues.Clone(), type);
        }
        private static async Task<T> SelectOriginalAsync(DbEntityEntry a, Type type = null)
        {
            if (type == null)
            {
                type = a.Entity.GetType();
            }
            return await CreateWithValuesAsync(a.OriginalValues.Clone(), type);
        }

        private static async Task<dynamic> SelectBothAsync(DbEntityEntry a)
        {
            var type = a.Entity.GetType();
            return new
            {
                OriginalValues = await SelectOriginalAsync(a, type),
                CurrentValues = await SelectCurrentAsync(a, type)
            };
        }

        public EntityState GetEntityState(T entity)
        {
            return _dbcontext.Entry(entity).State;
        }
    }
}
