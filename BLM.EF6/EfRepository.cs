using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Principal;

using BLM.Repository;

namespace BLM.EF6
{
    public class EfRepository<T> : IRepository<T> where T: class, new()
    {

        private readonly DbContext _dbcontext;
        private readonly DbSet<T> _dbset;

        public EfRepository(DbContext db)
        {
            _dbcontext = db;
            _dbset = db.Set<T>();

        }

        private EfContextInfo GetContextInfo(IIdentity user)
        {
            return new EfContextInfo(user, _dbcontext);
        }

        private AuthorizationResult AuthorizeAdd(IIdentity usr, T newEntity)
        {
            _dbset.Attach(newEntity);
            var authResult = Authorize.Create(newEntity, GetContextInfo(usr)).CreateAggregateResult();
            if (!authResult.HasSucceed)
            {
                Listen.CreateFailed(newEntity, GetContextInfo(usr));
                _dbcontext.Entry(newEntity).State = EntityState.Detached;
            }

            return authResult;
        }

        public void Add(IIdentity user, T newItem)
        {
            var result = AuthorizeAdd(user, newItem);

            if (!result.HasSucceed)
            {
                throw new AuthorizationFailedException(result); 
            }
            _dbset.Add(newItem);
        }

        public void AddRange(IIdentity user, IEnumerable<T> newItems)
        {
            var fails = new List<AuthorizationResult>();

            var newItemlist = newItems.ToList();
            foreach (var item in newItemlist)
            {
                var result = AuthorizeAdd(user, item);
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


        public void Remove(IIdentity user, T item)
        {
            var result = Authorize.Remove(item, GetContextInfo(user)).CreateAggregateResult();
            if (!result.HasSucceed)
            {
                throw new AuthorizationFailedException(result);
            }
            _dbset.Remove(item);
        }

        public void RemoveRange(IIdentity user, IEnumerable<T> items)
        {
            var fails = new List<AuthorizationResult>();
            var entityList = items.ToList();
            foreach (var entity in entityList)
            {
                var result = Authorize.Remove(entity, GetContextInfo(user)).CreateAggregateResult();
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

            if (ent.State == EntityState.Unchanged || ent.State == EntityState.Detached)
                return AuthorizationResult.Success();

            if (ent.Entity is T)
            {
                var casted = ent.Cast<T>();
                switch (ent.State)
                {
                    case EntityState.Added:
                        T interpreted = Interpret.BeforeCreate(casted.Entity, GetContextInfo(user));
                        return Authorize.Create(interpreted, GetContextInfo(user)).CreateAggregateResult();

                    case EntityState.Modified:
                        var original = CreateWithValues(casted.OriginalValues);
                        var modified = CreateWithValues(casted.CurrentValues);
                        var modifiedInterpreted = Interpret.BeforeModify(original, modified, GetContextInfo(user));
                        foreach (var field in ent.CurrentValues.PropertyNames)
                        {
                            ent.CurrentValues[field] = modifiedInterpreted.GetType().GetProperty(field).GetValue(modifiedInterpreted, null);
                        }
                        return Authorize.Modify(original, modifiedInterpreted, GetContextInfo(user)).CreateAggregateResult();
                    case EntityState.Deleted:
                        return Authorize.Remove(CreateWithValues(casted.OriginalValues), GetContextInfo(user)).CreateAggregateResult();
                    default:
                        return AuthorizationResult.Fail("The entity state is invalid", casted.Entity);
                }
            } else
            {
                return AuthorizationResult.Fail($"Changes for entity type '{ent.Entity.GetType().FullName}' is not supported in a context of a repository with type '{typeof(T).FullName}'", ent.Entity);
            }
        }
        private T CreateWithValues(DbPropertyValues values)
        {
            T entity = new T();
            Type type = typeof(T);

            foreach (var name in values.PropertyNames)
            {
                var property = type.GetProperty(name);
                property.SetValue(entity, values.GetValue<object>(name));
            }

            return entity;
        }

        public void SaveChanges(IIdentity user)
        {

            var contextInfo = GetContextInfo(user);

            _dbcontext.ChangeTracker.DetectChanges();
            var entries = _dbcontext.ChangeTracker.Entries().ToList();
            foreach (var entityChange in _dbcontext.ChangeTracker.Entries())
            {
                var authResult = AuthorizeEntityChange(user, entityChange);
                if (!authResult.HasSucceed)
                {
                    if (entityChange.State == EntityState.Modified)
                    {
                        Listen.ModificationFailed(CreateWithValues(entityChange.OriginalValues), entityChange.Entity as T, GetContextInfo(user));
                    }
                    else if (entityChange.State == EntityState.Deleted)
                    {
                        Listen.RemoveFailed(CreateWithValues(entityChange.OriginalValues), contextInfo);
                    }
                    throw new AuthorizationFailedException(authResult);
                }
            }

            var added = entries.Where(a => a.State == EntityState.Added).ToList();
            var modified = entries.Where(a => a.State == EntityState.Modified).ToList();
            var removed = entries.Where(a => a.State == EntityState.Deleted).Select(a=>CreateWithValues(a.OriginalValues)).ToList();

            _dbcontext.SaveChanges();

            added.ForEach(a => Listen.Created(CreateWithValues(a.OriginalValues), contextInfo));
            modified.ForEach(a => Listen.Modified( CreateWithValues(a.OriginalValues), CreateWithValues(a.CurrentValues), contextInfo));
            removed.ForEach(a => Listen.Removed(a, contextInfo));
        }

        public void SetEntityState(T entity, EntityState newState)
        {
            _dbcontext.Entry(entity).State = newState;
        }

        public EntityState GetEntityState(T entity)
        {
            return _dbcontext.Entry(entity).State;
        }
    }
}
