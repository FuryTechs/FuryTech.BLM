using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using BLM.Authorization;
using BLM.EventListeners;
using BLM.Repository;

namespace BLM.EF6
{
    public class EfRepository<T> : IRepository<T> where T: class, new()
    {
        public IAuthorizer<T> Authorizer { get; private set; }

        private readonly DbContext _dbcontext;
        private readonly DbSet<T> _dbset;
        private readonly EventListenerManager _listenerManager;

        public EfRepository(DbContext db)
        {
            Authorizer = AuthorizerManager.GetAuthorizer<T>();

            _dbcontext = db;
            _dbset = db.Set<T>();
            _listenerManager = EventListenerManager.Current;

            if (_dbset == null)
            {
                throw new NullReferenceException($"DbSet not found for type '{typeof(T).FullName}'");
            }

            var authorizer = Authorizer as BaseEfAuthorizer<T>;
            if (authorizer != null)
            {
                authorizer.DbContext = _dbcontext;
            }
        }

        private void AuthorizeAdd(IIdentity usr, T newEntity)
        {
            _dbset.Attach(newEntity);
            if (!Authorizer.CanInsert(usr, newEntity))
            {
                _listenerManager.TriggerOnCreationFailed(newEntity, usr);
                
                _dbcontext.Entry(newEntity).State = EntityState.Detached;
                throw new UnauthorizedAccessException();
            }
        }

        public void Add(IIdentity user, T newItem)
        {
            AuthorizeAdd(user, newItem);
            _dbset.Add(newItem);
        }

        public void AddRange(IIdentity user, IEnumerable<T> newItems)
        {
            var newItemlist = newItems.ToList();
            foreach (var item in newItemlist)
            {
                AuthorizeAdd(user, item);
            }
            _dbset.AddRange(newItemlist);
        }

        public void Dispose()
        {
            _dbcontext?.Dispose();
        }

        public IQueryable<T> Entities(IIdentity user)
        {
            return Authorizer.AuthorizeCollection(user, _dbset);
        }

        private void AuthorizeRemove(IIdentity user, T item)
        {
            if (!Authorizer.CanRemove(user, item))
            {
                _listenerManager.TriggerOnRemoveFailed(item, user);
                throw new UnauthorizedAccessException();
            }
        }

        public void Remove(IIdentity user, T item)
        {
            AuthorizeRemove(user, item);
            _dbset.Remove(item);
        }

        public void RemoveRange(IIdentity user, IEnumerable<T> items)
        {
            var entityList = items as IList<T> ?? items.ToList();
            foreach (var entity in entityList)
            {
                AuthorizeRemove(user, entity);

            }
            _dbset.RemoveRange(entityList);
        }

        private bool AuthorizeEntityChange(IIdentity user, DbEntityEntry ent)
        {

            if (ent.State == EntityState.Unchanged || ent.State == EntityState.Detached)
                return true;

            if (ent.Entity is T)
            {
                var casted = ent.Cast<T>();
                switch (ent.State)
                {
                    case EntityState.Added:
                        var createdInterpreted = _listenerManager.TriggerOnBeforeCreate(casted.Entity, user) as T;
                        return Authorizer.CanInsert(user, createdInterpreted);
                    case EntityState.Modified:
                        var original = CreateWithValues(casted.OriginalValues);
                        var modified = CreateWithValues(casted.CurrentValues);
                        var modifiedInterpreted = _listenerManager.TriggerOnBeforeModify(original, modified, user) as T;
                        foreach (var field in ent.CurrentValues.PropertyNames)
                        {
                            ent.CurrentValues[field] = modifiedInterpreted.GetType().GetProperty(field).GetValue(modifiedInterpreted, null);
                        }
                        return Authorizer.CanUpdate(user, original, modifiedInterpreted);
                    case EntityState.Deleted:
                        return Authorizer.CanRemove(user, casted.Entity);
                    default:
                        throw new InvalidOperationException();
                }
            } else
            {
                throw new InvalidOperationException($"Changes for entity type '{ent.Entity.GetType().FullName}' is not supported in a context of a repository with type '{typeof(T).FullName}'");
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
            _dbcontext.ChangeTracker.DetectChanges();
            var entries = _dbcontext.ChangeTracker.Entries().ToList();
            foreach (var entityChange in _dbcontext.ChangeTracker.Entries())
            {
                if (!AuthorizeEntityChange(user, entityChange))
                {
                    if (entityChange.State == EntityState.Modified)
                    {
                        _listenerManager.TriggerOnModificationFailed(CreateWithValues(entityChange.OriginalValues), entityChange.Entity as T, user);
                    }
                    else if (entityChange.State == EntityState.Deleted)
                    {
                        _listenerManager.TriggerOnRemoveFailed(CreateWithValues(entityChange.OriginalValues), user);
                    }
                    throw new UnauthorizedAccessException();
                }
            }

            var added = entries.Where(a => a.State == EntityState.Added).ToList();
            var modified = entries.Where(a => a.State == EntityState.Modified).ToList();
            var removed = entries.Where(a => a.State == EntityState.Deleted).Select(a=>CreateWithValues(a.OriginalValues)).ToList();

            _dbcontext.SaveChanges();

            added.ForEach(a => _listenerManager.TriggerOnCreated(a.Entity, user));
            modified.ForEach(a => _listenerManager.TriggerOnModified( CreateWithValues(a.OriginalValues), a.Entity, user));
            removed.ForEach(a => _listenerManager.TriggerOnRemoved(a, user));
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
