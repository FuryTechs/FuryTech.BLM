using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Principal;

using Xunit;

using FuryTech.BLM.NetStandard.Interfaces;
using FuryTech.BLM.NetStandard.Interfaces.Listen;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable 1998
#pragma warning disable IDE1006 // Naming Styles
// ReSharper disable UnusedMember.Global
// ReSharper disable StaticMemberInGenericType

namespace FuryTech.BLM.NetStandard.Tests
{
    #region GENERIC Listener
    public class Listener<T> : IListenCreated<T>, IListenCreateFailed<T>,
        IListenModified<T>, IListenModificationFailed<T>,
        IListenRemoved<T>, IListenRemoveFailed<T>
    {
        public static bool WasOnCreatedCalled;
        public static bool WasOnCreationValidationFailedCalled;
        public static bool WasOnModifiedCalled;
        public static bool? EntitesWereNotTheSameOnCalling;
        public static bool WasOnModificationFailedCalled;
        public static bool WasOnDeletedCalled;
        public static bool WasOnDeletionFailedCalled;

        public virtual async Task OnCreatedAsync(T entity, IContextInfo user)
        {
            WasOnCreatedCalled = true;
        }

        public virtual async Task OnCreateFailedAsync(T entity, IContextInfo user)
        {
            WasOnCreationValidationFailedCalled = true;
        }

        public virtual async Task OnModifiedAsync(T original, T modified, IContextInfo user)
        {
            if (original != null)
            {
                EntitesWereNotTheSameOnCalling = !original.Equals(modified);
            }
            else if (modified != null)
            {
                EntitesWereNotTheSameOnCalling = true;
            }
            else
            {
                EntitesWereNotTheSameOnCalling = false;
            }
            WasOnModifiedCalled = true;
        }
        public virtual async Task OnModificationFailedAsync(T original, T modified, IContextInfo user)
        {
            if (original != null)
            {
                EntitesWereNotTheSameOnCalling = !original.Equals(modified);
            }
            else if (modified != null)
            {
                EntitesWereNotTheSameOnCalling = !modified.Equals(original);
            }
            else
            {
                EntitesWereNotTheSameOnCalling = false;
            }
            WasOnModificationFailedCalled = true;
        }


        public virtual async Task OnRemovedAsync(T entity, IContextInfo user)
        {
            WasOnDeletedCalled = true;
        }


        public virtual async Task OnRemoveFailedAsync(T entity, IContextInfo user)
        {
            WasOnDeletionFailedCalled = true;
        }

        public static void Reset()
        {
            WasOnCreatedCalled = false;
            WasOnCreationValidationFailedCalled = false;
            WasOnModifiedCalled = false;
            WasOnModificationFailedCalled = false;
            WasOnDeletedCalled = false;
            WasOnDeletionFailedCalled = false;

            EntitesWereNotTheSameOnCalling = null;
        }
    }
    #endregion

    public class MockListener : Listener<MockEntity> { }

    public class MockListener2 : Listener<MockEntity> { }

    public class ObjectListener : Listener<object> { }


    public class ListenerTests
    {

        MockEntity ent = new MockEntity();
        MockEntity ent2 = new MockEntity()
        {
            Guid = Guid.NewGuid().ToString()
        };
        GenericContextInfo ctx = new GenericContextInfo(new GenericIdentity("gallayb"));

        protected readonly IServiceProvider _serviceProvider;

        public ListenerTests()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IBlmEntry, MockListener>();
            serviceCollection.AddSingleton<IBlmEntry, MockListener2>();
            serviceCollection.AddSingleton<IBlmEntry, ObjectListener>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task Created()
        {
            MockListener.Reset();
            await Listen.CreatedAsync(ent, ctx, _serviceProvider);

            Assert.True(MockListener.WasOnCreatedCalled);
            Assert.False(MockListener.WasOnCreationValidationFailedCalled);
            Assert.False(MockListener.WasOnModifiedCalled);
            Assert.False(MockListener.WasOnModificationFailedCalled);
            Assert.False(MockListener.WasOnDeletedCalled);
            Assert.False(MockListener.WasOnDeletionFailedCalled);
        }

        [Fact]
        public async Task CreateFailed()
        {
            MockListener.Reset();
            await Listen.CreateFailedAsync(ent, ctx, _serviceProvider);

            Assert.False(MockListener.WasOnCreatedCalled);
            Assert.True(MockListener.WasOnCreationValidationFailedCalled);
            Assert.False(MockListener.WasOnModifiedCalled);
            Assert.False(MockListener.WasOnModificationFailedCalled);
            Assert.False(MockListener.WasOnDeletedCalled);
            Assert.False(MockListener.WasOnDeletionFailedCalled);
        }

        [Fact]
        public async Task Modified()
        {
            MockListener.Reset();
            ObjectListener.Reset();

            await Listen.ModifiedAsync(ent, ent2, ctx, _serviceProvider);

            Assert.False(MockListener.WasOnCreatedCalled);
            Assert.False(MockListener.WasOnCreationValidationFailedCalled);
            Assert.True(MockListener.WasOnModifiedCalled);
            Assert.False(MockListener.WasOnModificationFailedCalled);
            Assert.False(MockListener.WasOnDeletedCalled);
            Assert.False(MockListener.WasOnDeletionFailedCalled);

            Assert.True(MockListener.EntitesWereNotTheSameOnCalling.HasValue && MockListener.EntitesWereNotTheSameOnCalling.Value);
        }

        /// <summary>
        /// Test the 3 different modify listeners, all of them need to run, and we pass two different entities into them
        /// </summary>
        [Fact]
        public async Task MultipleModifiedListeners()
        {
            MockListener.Reset();
            MockListener2.Reset();
            ObjectListener.Reset();

            await Listen.ModifiedAsync(ent, ent2, ctx, _serviceProvider);

            Assert.False(MockListener.WasOnCreatedCalled);
            Assert.False(MockListener.WasOnCreationValidationFailedCalled);
            Assert.True(MockListener.WasOnModifiedCalled);
            Assert.False(MockListener.WasOnModificationFailedCalled);
            Assert.False(MockListener.WasOnDeletedCalled);
            Assert.False(MockListener.WasOnDeletionFailedCalled);

            Assert.False(MockListener2.WasOnCreatedCalled);
            Assert.False(MockListener2.WasOnCreationValidationFailedCalled);
            Assert.True(MockListener2.WasOnModifiedCalled);
            Assert.False(MockListener2.WasOnModificationFailedCalled);
            Assert.False(MockListener2.WasOnDeletedCalled);
            Assert.False(MockListener2.WasOnDeletionFailedCalled);

            Assert.False(ObjectListener.WasOnCreatedCalled);
            Assert.False(ObjectListener.WasOnCreationValidationFailedCalled);
            Assert.True(ObjectListener.WasOnModifiedCalled);
            Assert.False(ObjectListener.WasOnModificationFailedCalled);
            Assert.False(ObjectListener.WasOnDeletedCalled);
            Assert.False(ObjectListener.WasOnDeletionFailedCalled);

            Assert.True(MockListener.EntitesWereNotTheSameOnCalling.HasValue && MockListener.EntitesWereNotTheSameOnCalling.Value);
            Assert.True(MockListener2.EntitesWereNotTheSameOnCalling.HasValue && MockListener2.EntitesWereNotTheSameOnCalling.Value);
            Assert.True(ObjectListener.EntitesWereNotTheSameOnCalling.HasValue && ObjectListener.EntitesWereNotTheSameOnCalling.Value);
        }

        /// <summary>
        /// Only the object modify listener should run, because we pass two (different) objects into the listener
        /// </summary>
        [Fact]
        public async Task MultipleModifiedListeners_TestInheritance()
        {
            MockListener.Reset();
            MockListener2.Reset();
            ObjectListener.Reset();

            await Listen.ModifiedAsync(new object(), new { sajt = true }, ctx, _serviceProvider);

            Assert.False(MockListener.WasOnCreatedCalled);
            Assert.False(MockListener.WasOnCreationValidationFailedCalled);
            Assert.False(MockListener.WasOnModifiedCalled);
            Assert.False(MockListener.WasOnModificationFailedCalled);
            Assert.False(MockListener.WasOnDeletedCalled);
            Assert.False(MockListener.WasOnDeletionFailedCalled);

            Assert.False(MockListener2.WasOnCreatedCalled);
            Assert.False(MockListener2.WasOnCreationValidationFailedCalled);
            Assert.False(MockListener2.WasOnModifiedCalled);
            Assert.False(MockListener2.WasOnModificationFailedCalled);
            Assert.False(MockListener2.WasOnDeletedCalled);
            Assert.False(MockListener2.WasOnDeletionFailedCalled);

            Assert.False(ObjectListener.WasOnCreatedCalled);
            Assert.False(ObjectListener.WasOnCreationValidationFailedCalled);
            Assert.True(ObjectListener.WasOnModifiedCalled);
            Assert.False(ObjectListener.WasOnModificationFailedCalled);
            Assert.False(ObjectListener.WasOnDeletedCalled);
            Assert.False(ObjectListener.WasOnDeletionFailedCalled);
            
            Assert.Null(MockListener.EntitesWereNotTheSameOnCalling);
            Assert.Null(MockListener2.EntitesWereNotTheSameOnCalling);
            Assert.True(ObjectListener.EntitesWereNotTheSameOnCalling.HasValue && ObjectListener.EntitesWereNotTheSameOnCalling.Value);
        }

        /// <summary>
        /// A little loadtest like something for check the performance is OK
        /// </summary>
        [Fact]
        public async Task LoadTest()
        {
            MockListener.Reset();
            MockListener2.Reset();
            ObjectListener.Reset();

            var start = DateTime.Now;

            var tasks = new List<Task>();

            for (var i = 0; i < 500000; i++)
            {
                tasks.Add(Listen.ModifiedAsync(ent, ent, ctx, _serviceProvider));
            }

            Task.WaitAll(tasks.ToArray());

            var time = DateTime.Now.Subtract(start).TotalMilliseconds;

            Assert.True(time < 1500, "time < 1500");

            Assert.False(MockListener.WasOnCreatedCalled);
            Assert.False(MockListener.WasOnCreationValidationFailedCalled);
            Assert.True(MockListener.WasOnModifiedCalled);
            Assert.False(MockListener.WasOnModificationFailedCalled);
            Assert.False(MockListener.WasOnDeletedCalled);
            Assert.False(MockListener.WasOnDeletionFailedCalled);

            Assert.False(MockListener2.WasOnCreatedCalled);
            Assert.False(MockListener2.WasOnCreationValidationFailedCalled);
            Assert.True(MockListener2.WasOnModifiedCalled);
            Assert.False(MockListener2.WasOnModificationFailedCalled);
            Assert.False(MockListener2.WasOnDeletedCalled);
            Assert.False(MockListener2.WasOnDeletionFailedCalled);


            Assert.False(ObjectListener.WasOnCreatedCalled);
            Assert.False(ObjectListener.WasOnCreationValidationFailedCalled);
            Assert.True(ObjectListener.WasOnModifiedCalled);
            Assert.False(ObjectListener.WasOnModificationFailedCalled);
            Assert.False(ObjectListener.WasOnDeletedCalled);
            Assert.False(ObjectListener.WasOnDeletionFailedCalled);
        }

        [Fact]
        public async Task ModificationFailed()
        {
            MockListener.Reset();
            await Listen.ModificationFailedAsync(ent, ent, ctx, _serviceProvider);

            Assert.False(MockListener.WasOnCreatedCalled);
            Assert.False(MockListener.WasOnCreationValidationFailedCalled);
            Assert.False(MockListener.WasOnModifiedCalled);
            Assert.True(MockListener.WasOnModificationFailedCalled);
            Assert.False(MockListener.WasOnDeletedCalled);
            Assert.False(MockListener.WasOnDeletionFailedCalled);
        }

        [Fact]
        public async Task Removed()
        {
            MockListener.Reset();
            await Listen.RemovedAsync(ent, ctx, _serviceProvider);

            Assert.False(MockListener.WasOnCreatedCalled);
            Assert.False(MockListener.WasOnCreationValidationFailedCalled);
            Assert.False(MockListener.WasOnModifiedCalled);
            Assert.False(MockListener.WasOnModificationFailedCalled);
            Assert.True(MockListener.WasOnDeletedCalled);
            Assert.False(MockListener.WasOnDeletionFailedCalled);
        }

        [Fact]
        public async Task RemoveFailed()
        {
            MockListener.Reset();
            await Listen.RemoveFailedAsync(ent, ctx, _serviceProvider);

            Assert.False(MockListener.WasOnCreatedCalled);
            Assert.False(MockListener.WasOnCreationValidationFailedCalled);
            Assert.False(MockListener.WasOnModifiedCalled);
            Assert.False(MockListener.WasOnModificationFailedCalled);
            Assert.False(MockListener.WasOnDeletedCalled);
            Assert.True(MockListener.WasOnDeletionFailedCalled);
        }
    }
}
// ReSharper restore UnusedMember.Global
// ReSharper restore StaticMemberInGenericType
