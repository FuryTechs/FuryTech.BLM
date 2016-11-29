using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLM.Interfaces.Listen;

namespace BLM.Tests
{
    #region GENERIC Listener
    public class Listener<T> : IListenCreated<T>, IListenCreateFailed<T>,
        IListenModified<T>, IListenModificationFailed<T>,
        IListenRemoved<T>, IListenRemoveFailed<T>
    {
        public static bool WasOnCreatedCalled;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task OnCreatedAsync(T entity, IContextInfo user)
        {
            WasOnCreatedCalled = true;
        }

        public static bool WasOnCreationValidationFailedCalled;

        public async Task OnCreateFailedAsync(T entity, IContextInfo user)
        {
            WasOnCreationValidationFailedCalled = true;
        }

        public static bool WasOnModifiedCalled;

        public async Task OnModifiedAsync(T original, T modified, IContextInfo user)
        {
            WasOnModifiedCalled = true;
        }

        public static bool WasOnModificationFailedCalled;

        public async Task OnModificationFailedAsync(T original, T modified, IContextInfo user)
        {
            WasOnModificationFailedCalled = true;
        }

        public static bool WasOnDeletedCalled;

        public async Task OnRemovedAsync(T entity, IContextInfo user)
        {
            WasOnDeletedCalled = true;
        }

        public static bool WasOnDeletionFailedCalled;

        public async Task OnRemoveFailedAsync(T entity, IContextInfo user)
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
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously


    }
    #endregion


    public class MockListener : Listener<MockEntity> { }

    public class MockListener2 : Listener<MockEntity> { }

    public class ObjectListener : Listener<object> { }


    [TestClass]
    public class ListenerTests
    {

        MockEntity ent = new MockEntity();
        GenericContextInfo ctx = new GenericContextInfo(Thread.CurrentPrincipal.Identity);

        [TestMethod]
        public async Task Created()
        {
            MockListener.Reset();
            await Listen.Created(ent, ctx);

            Assert.IsTrue(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public async Task CreateFailed()
        {
            MockListener.Reset();
            await Listen.CreateFailed(ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsTrue(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public async Task Modified()
        {
            MockListener.Reset();
            ObjectListener.Reset();

            await Listen.Modified(ent, ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public async Task MultipleModifiedListeners()
        {
            MockListener.Reset();
            MockListener2.Reset();

            await Listen.Modified(ent, ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);

            Assert.IsFalse(MockListener2.WasOnCreatedCalled);
            Assert.IsFalse(MockListener2.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(MockListener2.WasOnModifiedCalled);
            Assert.IsFalse(MockListener2.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener2.WasOnDeletedCalled);
            Assert.IsFalse(MockListener2.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public async Task MultipleModifiedListeners_Inheritance()
        {
            MockListener.Reset();
            MockListener2.Reset();
            ObjectListener.Reset();

            await Listen.Modified(ent, ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);

            Assert.IsFalse(MockListener2.WasOnCreatedCalled);
            Assert.IsFalse(MockListener2.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(MockListener2.WasOnModifiedCalled);
            Assert.IsFalse(MockListener2.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener2.WasOnDeletedCalled);
            Assert.IsFalse(MockListener2.WasOnDeletionFailedCalled);


            Assert.IsFalse(ObjectListener.WasOnCreatedCalled);
            Assert.IsFalse(ObjectListener.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(ObjectListener.WasOnModifiedCalled);
            Assert.IsFalse(ObjectListener.WasOnModificationFailedCalled);
            Assert.IsFalse(ObjectListener.WasOnDeletedCalled);
            Assert.IsFalse(ObjectListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public void LoadTest()
        {
            MockListener.Reset();
            MockListener2.Reset();
            ObjectListener.Reset();

            var start = DateTime.Now;

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Listen.Modified(ent, ent, ctx));
            }

            Task.WaitAll(tasks.ToArray());

            var time = DateTime.Now.Subtract(start).TotalMilliseconds;

            Assert.IsTrue(time < 10000);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);

            Assert.IsFalse(MockListener2.WasOnCreatedCalled);
            Assert.IsFalse(MockListener2.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(MockListener2.WasOnModifiedCalled);
            Assert.IsFalse(MockListener2.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener2.WasOnDeletedCalled);
            Assert.IsFalse(MockListener2.WasOnDeletionFailedCalled);


            Assert.IsFalse(ObjectListener.WasOnCreatedCalled);
            Assert.IsFalse(ObjectListener.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(ObjectListener.WasOnModifiedCalled);
            Assert.IsFalse(ObjectListener.WasOnModificationFailedCalled);
            Assert.IsFalse(ObjectListener.WasOnDeletedCalled);
            Assert.IsFalse(ObjectListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public async Task ModificationFailed()
        {
            MockListener.Reset();
            await Listen.ModificationFailed(ent, ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsTrue(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public async Task Removed()
        {
            MockListener.Reset();
            await Listen.Removed(ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsTrue(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public async Task RemoveFailed()
        {
            MockListener.Reset();
            await Listen.RemoveFailed(ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsTrue(MockListener.WasOnDeletionFailedCalled);
        }
    }
}
