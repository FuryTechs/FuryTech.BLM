using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLM.Interfaces.Listen;

#pragma warning disable 1998
#pragma warning disable IDE1006 // Naming Styles
// ReSharper disable UnusedMember.Global
// ReSharper disable StaticMemberInGenericType

namespace BLM.Tests
{
    #region GENERIC Listener
    public class Listener<T> : IListenCreated<T>, IListenCreateFailed<T>,
        IListenModified<T>, IListenModificationFailed<T>,
        IListenRemoved<T>, IListenRemoveFailed<T>
    {
        public static bool WasOnCreatedCalled;
        public static bool WasOnCreationValidationFailedCalled;
        public static bool WasOnModifiedCalled;
        public static bool WasOnModificationFailedCalled;
        public static bool WasOnDeletedCalled;
        public static bool WasOnDeletionFailedCalled;

        public async Task OnCreatedAsync(T entity, IContextInfo user)
        {
            WasOnCreatedCalled = true;
        }

        public async Task OnCreateFailedAsync(T entity, IContextInfo user)
        {
            WasOnCreationValidationFailedCalled = true;
        }

        public async Task OnModifiedAsync(T original, T modified, IContextInfo user)
        {
            WasOnModifiedCalled = true;
        }
        public async Task OnModificationFailedAsync(T original, T modified, IContextInfo user)
        {
            WasOnModificationFailedCalled = true;
        }


        public async Task OnRemovedAsync(T entity, IContextInfo user)
        {
            WasOnDeletedCalled = true;
        }


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
            await Listen.CreatedAsync(ent, ctx);

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
            await Listen.CreateFailedAsync(ent, ctx);

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

            await Listen.ModifiedAsync(ent, ent, ctx);

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

            await Listen.ModifiedAsync(ent, ent, ctx);

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

            await Listen.ModifiedAsync(ent, ent, ctx);

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
        public async Task LoadTest()
        {
            MockListener.Reset();
            MockListener2.Reset();
            ObjectListener.Reset();

            var start = DateTime.Now;

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 5000000; i++)
            {
                tasks.Add(Listen.ModifiedAsync(ent, ent, ctx));
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
            await Listen.ModificationFailedAsync(ent, ent, ctx);

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
            await Listen.RemovedAsync(ent, ctx);

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
            await Listen.RemoveFailedAsync(ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsTrue(MockListener.WasOnDeletionFailedCalled);
        }
    }
}
// ReSharper restore UnusedMember.Global
// ReSharper restore StaticMemberInGenericType
