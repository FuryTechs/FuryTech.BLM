using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.Tests
{
    public class MockListener : IListenCreated<MockEntity>, IListenCreateFailed<MockEntity>,
        IListenModified<MockEntity>, IListenModificationFailed<MockEntity>,
        IListenRemoved<MockEntity>, IListenRemoveFailed<MockEntity>
    {
        public static bool WasOnCreatedCalled;

        public void OnCreated(MockEntity entity, IContextInfo user)
        {
            WasOnCreatedCalled = true;
        }

        public static bool WasOnCreationValidationFailedCalled;

        public void OnCreateFailed(MockEntity entity, IContextInfo user)
        {
            WasOnCreationValidationFailedCalled = true;
        }

        public static bool WasOnModifiedCalled;

        public void OnModified(MockEntity original, MockEntity modified, IContextInfo user)
        {
            WasOnModifiedCalled = true;
        }

        public static bool WasOnModificationFailedCalled;

        public void OnModificationFailed(MockEntity original, MockEntity modified, IContextInfo user)
        {
            WasOnModificationFailedCalled = true;
        }

        public static bool WasOnDeletedCalled;

        public void OnRemoved(MockEntity entity, IContextInfo user)
        {
            WasOnDeletedCalled = true;
        }

        public static bool WasOnDeletionFailedCalled;

        public void OnRemoveFailed(MockEntity entity, IContextInfo user)
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

    [TestClass]
    public class ListenerTests
    {

        MockEntity ent = new MockEntity();
        GenericContextInfo ctx = new GenericContextInfo(Thread.CurrentPrincipal.Identity);

        [TestMethod]
        public void Created()
        {
            MockListener.Reset();
            Listen.Created(ent, ctx);

            Assert.IsTrue(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public void CreateFailed()
        {
            MockListener.Reset();
            Listen.CreateFailed(ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsTrue(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public void Modified()
        {
            MockListener.Reset();
            Listen.Modified(ent, ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsTrue(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public void ModificationFailed()
        {
            MockListener.Reset();
            Listen.ModificationFailed(ent, ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsTrue(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public void Removed()
        {
            MockListener.Reset();
            Listen.Removed(ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsTrue(MockListener.WasOnDeletedCalled);
            Assert.IsFalse(MockListener.WasOnDeletionFailedCalled);
        }

        [TestMethod]
        public void RemoveFailed()
        {
            MockListener.Reset();
            Listen.RemoveFailed(ent, ctx);

            Assert.IsFalse(MockListener.WasOnCreatedCalled);
            Assert.IsFalse(MockListener.WasOnCreationValidationFailedCalled);
            Assert.IsFalse(MockListener.WasOnModifiedCalled);
            Assert.IsFalse(MockListener.WasOnModificationFailedCalled);
            Assert.IsFalse(MockListener.WasOnDeletedCalled);
            Assert.IsTrue(MockListener.WasOnDeletionFailedCalled);
        }
    }
}
