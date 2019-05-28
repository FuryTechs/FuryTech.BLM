using FuryTechs.BLM.EntityFrameworkCore.Identity;
using FuryTechs.BLM.NetStandard;
using FuryTechs.BLM.NetStandard.Attributes;
using FuryTechs.BLM.NetStandard.Exceptions;
using FuryTechs.BLM.NetStandard.Extensions;
using FuryTechs.BLM.NetStandard.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FuryTechs.BLM.EntityFrameworkCore
{
    public class EfRepository<T, TDbContext> : EfRepository<T>
        where T : class, new()
        where TDbContext : DbContext
    {
        public EfRepository(IServiceProvider serviceProvider) : base(typeof(TDbContext), serviceProvider)
        {
        }
    }

    public class EfRepository<T> : IRepository<T>, IEfRepository
        where T : class, new()
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private readonly Type _type;
        private readonly bool _disposeDbContextOnDispose;

        private readonly Dictionary<string, IRepository> _childRepositories = new Dictionary<string, IRepository>();

        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// If there are inherited objets, which have LogicalDeleteAttribue-s on it, we throw an exception
        /// </summary>
        public bool IgnoreLogicalDeleteError { get; set; } = false;

        /// <summary>
        /// Initializes a new instance from the EfRepository with the given type parameter
        /// </summary>
        /// <param name="dbContextType">Type of the database context</param>
        /// <param name="serviceProvider">Service provider</param>
        public EfRepository(Type dbContextType, IServiceProvider serviceProvider)
            : this((DbContext)serviceProvider.GetService(dbContextType), serviceProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance from the EfRepository with the given type parameter
        /// </summary>
        /// <param name="dbContext">DbContext instance</param>
        /// <param name="serviceProvider">Service provider</param>
        public EfRepository(DbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _type = typeof(T);
            _serviceProvider = serviceProvider;
        }

        private EfContextInfo<T> GetContextInfo(IIdentity user)
        {
            return new EfContextInfo<T>(user, _dbContext, _serviceProvider);
        }

        #region Get child repositories
        private IRepository GetChildRepositoryFor(EntityEntry entry)
        {
            var repoType = entry.Entity.GetType();
            var mi = this.GetType().GetMethods().FirstOrDefault(x => x.Name == "GetChildRepositoryFor" && x.IsGenericMethod);
            var genericMethod = mi.MakeGenericMethod(repoType);

            return (IRepository)genericMethod.Invoke(this, null); ;
        }

        private IRepository GetChildRepositoryFor(Type type)
        {
            var repoKey = type.FullName;
            if (_childRepositories.ContainsKey(repoKey))
            {
                return (IRepository)_childRepositories[repoKey];
            }

            var childRepositoryType = typeof(IRepository<,>).MakeGenericType(type, _dbContext.GetType());
            return (IRepository)_serviceProvider.GetService(childRepositoryType);
        }


        /// <inheritdoc />
        public IRepository<T2> GetChildRepositoryFor<T2>() where T2 : class
        {
            return (IRepository<T2>)GetChildRepositoryFor(typeof(T2));
        }
        #endregion

        #region Static things

        /// <summary>
        /// Check the given type if it has an LogicalDeleteAttribute on any property, and returns with the first property it founds (or null)
        /// </summary>
        /// <param name="type">Checked type</param>
        /// <returns></returns>
        public static PropertyInfo GetLogicalDeleteProperty(Type type)
        {
            if (!LogicalDeleteCache.ContainsKey(type.FullName ?? throw new InvalidOperationException()))
            {
                var logicalDeleteProperty = type.GetProperties()
                    .FirstOrDefault(x => x.GetCustomAttributes<LogicalDeleteAttribute>().Any());
                if (logicalDeleteProperty == null)
                {
                    logicalDeleteProperty =
                        type.GetInterfaces()
                            .Select(x =>
                                x.GetProperties().FirstOrDefault(y =>
                                    y.GetCustomAttributes<LogicalDeleteAttribute>().Any()))
                            ?.FirstOrDefault(x => x != null);
                }

                LogicalDeleteCache.Add(type.FullName, logicalDeleteProperty);
            }

            return LogicalDeleteCache[type.FullName];
        }

        private static readonly Dictionary<string, PropertyInfo> LogicalDeleteCache;

        static EfRepository()
        {
            LogicalDeleteCache = new Dictionary<string, PropertyInfo>();
        }

        #endregion

        private async Task<AuthorizationResult> AuthorizeAddAsync(IIdentity usr, T newEntity)
        {
            var authResult = (await Authorize.CreateAsync(newEntity, GetContextInfo(usr), _serviceProvider))
                .CreateAggregateResult();
            if (!authResult.HasSucceed)
            {
                await Listen.CreateFailedAsync(newEntity, GetContextInfo(usr), _serviceProvider);
                _dbContext.Entry(newEntity).State = EntityState.Detached;
            }

            return authResult;
        }

        /// <inheritdoc />
        public void Add(T newItem, IIdentity user = null)
        {
            if (user == null)
            {
                user = _serviceProvider.GetService<IIdentityResolver>().GetIdentity();
            }

            AddAsync(newItem, user).Wait();
        }

        /// <inheritdoc />
        public async Task AddAsync(T newItem, IIdentity user = null)
        {
            if (user == null)
            {
                user = _serviceProvider.GetService<IIdentityResolver>().GetIdentity();
            }

            await Task.Factory.StartNew(() => { _dbSet.Add(newItem); });
        }

        /// <inheritdoc />
        public void AddRange(IEnumerable<T> newItems, IIdentity user = null)
        {
            if (user == null)
            {
                user = _serviceProvider.GetService<IIdentityResolver>().GetIdentity();
            }

            AddRangeAsync(newItems, user).Wait();
        }

        /// <inheritdoc />
        public async Task AddRangeAsync(IEnumerable<T> newItems, IIdentity user)
        {
            if (user == null)
            {
                user = _serviceProvider.GetService<IIdentityResolver>().GetIdentity();
            }

            await Task.Factory.StartNew(() => { _dbSet.AddRange(newItems); });
        }


        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var childRepo in _childRepositories)
            {
                childRepo.Value?.Dispose();
            }

            if (_disposeDbContextOnDispose)
            {
                _dbContext?.Dispose();
            }
        }

        /// <inheritdoc />

        public IQueryable<T> Entities(IIdentity user = null)
        {
            if (user == null)
            {
                user = _serviceProvider.GetService<IIdentityResolver>().GetIdentity();
            }

            return Authorize.Collection(_dbSet, GetContextInfo(user), _serviceProvider);
        }

        /// <inheritdoc />
        public async Task<IQueryable<T>> EntitiesAsync(IIdentity user = null)
        {
            if (user == null)
            {
                user = _serviceProvider.GetService<IIdentityResolver>().GetIdentity();
            }

            var result = await Authorize.CollectionAsync(_dbSet, GetContextInfo(user), _serviceProvider);
            return result as IQueryable<T>;
        }

        /// <inheritdoc />
        public void Remove(T item, IIdentity usr = null)
        {
            RemoveAsync(item, usr ?? _serviceProvider.GetService<IIdentityResolver>().GetIdentity()).Wait();
        }

        /// <inheritdoc />
        public async Task RemoveAsync(T item, IIdentity user = null)
        {
            await Task.Factory.StartNew(() => { _dbSet.Remove(item); });
        }

        /// <inheritdoc />
        public void RemoveRange(IEnumerable<T> items, IIdentity usr = null)
        {
            RemoveRangeAsync(items, usr ?? _serviceProvider.GetService<IIdentityResolver>().GetIdentity()).Wait();
        }

        /// <inheritdoc />
        public async Task RemoveRangeAsync(IEnumerable<T> items, IIdentity usr)
        {
            await Task.Factory.StartNew(() => { _dbSet.RemoveRange(items); });
        }

        /// <inheritdoc />
        public async Task<AuthorizationResult> AuthorizeEntityChangeAsync(EntityEntry ent, IIdentity usr = null)
        {
            if (usr == null)
            {
                usr = _serviceProvider.GetService<IIdentityResolver>().GetIdentity();
            }

            if (ent.State == EntityState.Unchanged || ent.State == EntityState.Detached)
            {
                return AuthorizationResult.Success();
            }

            if (ent.Entity is T variable)
            {
                switch (ent.State)
                {
                    case EntityState.Added:
                        var interpreted =
                            Interpret.BeforeCreate(variable, GetContextInfo(usr), _serviceProvider);
                        return (await Authorize.CreateAsync(interpreted, GetContextInfo(usr), _serviceProvider))
                            .CreateAggregateResult();

                    case EntityState.Modified:
                        var original = CreateWithValues(ent.OriginalValues);
                        var modified = CreateWithValues(ent.CurrentValues);
                        var modifiedInterpreted =
                            Interpret.BeforeModify((T)original, (T)modified, GetContextInfo(usr), _serviceProvider);
                        foreach (var property in ent.CurrentValues.Properties)
                        {
                            ent.CurrentValues[property.Name] = modifiedInterpreted.GetType().GetProperty(property.Name)
                                ?.GetValue(modifiedInterpreted, null);
                        }

                        return (await Authorize.ModifyAsync((T)original, modifiedInterpreted, GetContextInfo(usr),
                                _serviceProvider))
                            .CreateAggregateResult();
                    case EntityState.Deleted:
                        return (await Authorize.RemoveAsync(
                                (T)CreateWithValues(ent.OriginalValues, variable.GetType()), GetContextInfo(usr),
                                _serviceProvider))
                            .CreateAggregateResult();
                    default:
                        return AuthorizationResult.Fail("The entity state is invalid", ent.Entity);
                }
            }
            else
            {
                return await ((IEfRepository)GetChildRepositoryFor(ent)).AuthorizeEntityChangeAsync(ent, usr);
            }
        }

        private static object CreateWithValues(PropertyValues values, Type type = null)
        {
            if (type == null)
            {
                type = typeof(T);
            }

            try
            {
                return values.ToObject();
            }
            catch
            {
                var entity = Activator.CreateInstance(type);

                //Debug.WriteLine(values.ToObject());
                foreach (var p in values.Properties)
                {
                    var name = p.Name;
                    var value = values.GetValue<object>(name);
                    var property = type.GetProperty(name);

                    if (value == null)
                    {
                        continue;
                    }

                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    property.SetValue(entity, Convert.ChangeType(value, propertyType), null);
                }

                return entity;
            }
        }

        /// <inheritdoc />
        public void SaveChanges(IIdentity user = null)
        {
            SaveChangesAsync(user).Wait();
        }

        private async Task<List<EntityEntry>> AuthorizeChanges(IIdentity user, IContextInfo contextInfo)
        {
            _dbContext.ChangeTracker.DetectChanges();
            var entries = _dbContext.ChangeTracker.Entries().ToList();

            for (var i = 0; i < entries.Count; i++)
            {
                var entityChange = entries[i];
                var authResult = await AuthorizeEntityChangeAsync(entityChange, user);
                if (authResult.HasSucceed) continue;
                switch (entityChange.State)
                {
                    case EntityState.Modified:
                        await Listen.ModificationFailedAsync(
                            CreateWithValues(entityChange.OriginalValues),
                            entityChange.Entity as T,
                            GetContextInfo(user),
                            _serviceProvider);
                        break;
                    case EntityState.Deleted:
                        await Listen.RemoveFailedAsync(
                            CreateWithValues(entityChange.OriginalValues),
                            contextInfo,
                            _serviceProvider);
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Added:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                throw new AuthorizationFailedException(authResult);
            }

            _dbContext.ChangeTracker.DetectChanges();

            var compareEntriesCount = _dbContext.ChangeTracker.Entries().Count();
            if (compareEntriesCount != entries.Count)
            {
                return await AuthorizeChanges(user, contextInfo);
            }

            return entries;
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync(IIdentity user = null)
        {
            if (user == null)
            {
                user = _serviceProvider.GetService<IIdentityResolver>().GetIdentity();
            }
            var contextInfo = GetContextInfo(user);
            var entries = await AuthorizeChanges(user, contextInfo);

            // Added should be updated after saving changes for get the ID of the newly created entity
            var added = entries.Where(a => a.State == EntityState.Added).Select(a => a.Entity).ToList();
            var modified = entries.Where(a => a.State == EntityState.Modified).Select(SelectBoth).ToList();
            var removed = entries.Where(a => a.State == EntityState.Deleted).Select(a => SelectOriginal(a)).ToList();

            if (removed.Any())
            {
                if (GetLogicalDeleteProperty(_type) == null)
                {
                    if (!IgnoreLogicalDeleteError &&
                        removed.Any(entry => GetLogicalDeleteProperty(entry.GetType()) != null))
                    {
                        throw new LogicalSecurityRiskException(
                            $"There are derived types in the deleted entries which have LogicalDeleteAttribute, but the base type does not use logical delete.");
                    }
                }
                else
                {
                    var logicalRemoved = entries.Where(a => a.State == EntityState.Deleted).ToList();
                    logicalRemoved.ForEach(entry =>
                    {
                        entry.Reload();
                        entry.State = EntityState.Modified;
                        entry.Property(GetLogicalDeleteProperty(_type).Name).CurrentValue = true;
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
            await DistributeToListenersAsync(added, contextInfo, modified, removed);
        }

        /// <inheritdoc />
        public async Task DistributeToListenersAsync(List<object> added, IContextInfo contextInfo,
            List<Tuple<object, object>> modified, List<object> removed, bool isChildRepository = false)
        {
            if (!isChildRepository)
            {
                var otherTypes = added.Where(a => !(a is T)).Select(a => a.GetType()).ToList();
                otherTypes.AddRange(modified.Where(a => !(a.Item1 is T)).Select(a => a.Item1.GetType()));
                otherTypes.AddRange(removed.Where(a => !(a is T)).Select(a => a.GetType()));
                foreach (var otherType in otherTypes.Distinct())
                {
                    var repo = GetChildRepositoryFor(otherType);
                    await repo.DistributeToListenersAsync(added, contextInfo, modified, removed, true);
                }
            }


            /* from the same type */
            //added.Where(a=>(a) is T).Cast<T>().Select(async a => await Listen.CreatedAsync(a, contextInfo));
            foreach (var addedEntry in added.Where(a => (a) is T).Cast<T>())
            {
                await Listen.CreatedAsync(
                    addedEntry,
                    contextInfo,
                    _serviceProvider);
            }

            //var t2 = modified.Where(a => a is Tuple<T,T>).Cast<Tuple<T,T>>().Select(async a =>await Listen.ModifiedAsync((a).Item1, (a).Item2, contextInfo));
            foreach (var modifiedEntry in modified.Where(a => a.Item1 is T && a.Item2 is T)
                .Cast<Tuple<object, object>>())
            {
                await Listen.ModifiedAsync(
                    modifiedEntry.Item1 as T,
                    modifiedEntry.Item2 as T,
                    contextInfo,
                    _serviceProvider);
            }

            //var t3 = removed.Where(a => a is T).Cast<T>().Select(async a => await Listen.RemovedAsync((a), contextInfo));
            foreach (var removedEntry in removed.Where(a => a is T).Cast<T>())
            {
                await Listen.RemovedAsync(
                    removedEntry,
                    contextInfo,
                    _serviceProvider);
            }
        }

        /// <inheritdoc />
        public void SetEntityState(T entity, EntityState newState)
        {
            _dbContext.Entry(entity).State = newState;
        }

        private static object SelectCurrent(EntityEntry a, Type type = null)
        {
            if (type == null)
            {
                type = a.Entity.GetType();
            }

            return CreateWithValues(a.CurrentValues.Clone(), type);
        }

        private static object SelectOriginal(EntityEntry a, Type type = null)
        {
            if (type == null)
            {
                type = a.Entity.GetType();
            }

            return CreateWithValues(a.OriginalValues.Clone(), type);
        }

        private static Tuple<object, object> SelectBoth(EntityEntry a)
        {
            var type = a.Entity.GetType();
            return (new Tuple<object, object>(SelectOriginal(a, type), SelectCurrent(a, type)));
        }

        /// <inheritdoc />
        public EntityState GetEntityState(T entity)
        {
            return _dbContext.Entry(entity).State;
        }

    }
}
