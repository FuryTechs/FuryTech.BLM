using BLM.EventListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Principal;
using System;

namespace BLM.Tests
{
    [TestClass]
    public class EventListenerManagerTests
    {
        class MockClass { }
        class MockListener : IEventListener<MockClass>
        {
            public MockListener()
            {
                Reset();
            }

            public void Reset()
            {
                OnCreatedTriggered = false;
                OnCreationValidationFailedTriggered = false;
                OnRemovedTriggered = false;
                OnRemoveFailedTriggered = false;
                OnModifiedTriggered = false;
                OnModificationFailedTriggered = false;
            }

            public bool OnCreatedTriggered;
            public void OnCreated(MockClass entity, IIdentity user)
            {
                OnCreatedTriggered = true;
            }

            public bool OnCreationValidationFailedTriggered;
            public void OnCreationValidationFailed(MockClass entity, IIdentity user)
            {
                OnCreationValidationFailedTriggered = true;
            }

            public bool OnRemovedTriggered;
            public void OnRemoved(MockClass entity, IIdentity user)
            {
                OnRemovedTriggered = true;
            }


            public bool OnRemoveFailedTriggered;
            public void OnRemoveFailed(MockClass entity, IIdentity user)
            {
                OnRemoveFailedTriggered = true;
            }

            public bool OnModificationFailedTriggered;

            public void OnModificationFailed(MockClass originalEntity, MockClass modifiedEntity, IIdentity user)
            {
                OnModificationFailedTriggered = true;
            }

            public bool OnModifiedTriggered;
            public void OnModified(MockClass originalEntity, MockClass modifiedEntity, IIdentity user)
            {
                OnModifiedTriggered = true;
            }
        }

        interface IMockInterface { }
        class IMockInterfacedClass : IMockInterface { }

        class MockInterfacedListener : IEventListener<IMockInterface>
        {
            public bool OnCreatedTriggered = false;
            public void OnCreated(IMockInterface entity, IIdentity user)
            {
                OnCreatedTriggered = true;
            }

            public void OnCreationValidationFailed(IMockInterface entity, IIdentity user)
            {
                throw new NotImplementedException();
            }

            public void OnRemoved(IMockInterface entity, IIdentity user)
            {
                throw new NotImplementedException();
            }

            public void OnRemoveFailed(IMockInterface entity, IIdentity user)
            {
                throw new NotImplementedException();
            }

            public void OnModificationFailed(IMockInterface originalEntity, IMockInterface modifiedEntity, IIdentity user)
            {
                throw new NotImplementedException();
            }

            public void OnModified(IMockInterface originalEntity, IMockInterface modifiedEntity, IIdentity user)
            {
                throw new NotImplementedException();
            }
        }

        private EventListenerManager _manager;
        private MockListener _testMockListener;
        private MockClass _mockObject;

        [TestInitialize]
        public void Init()
        {
            _manager = EventListenerManager.Current;
            _testMockListener = _manager.GetListener<MockListener>() as MockListener;
            _mockObject = new MockClass();
        }

        [TestMethod]
        public void TriggerCreateShouldFire()
        {
            _testMockListener.Reset();
            _manager.TriggerOnCreated(_mockObject, null);
            Assert.IsTrue(_testMockListener.OnCreatedTriggered);
        }

        [TestMethod]
        public void TriggerCreateFailShouldFire()
        {
            _testMockListener.Reset();
            _manager.TriggerOnCreationFailed(_mockObject, null);
            Assert.IsTrue(_testMockListener.OnCreationValidationFailedTriggered);
        }

        [TestMethod]
        public void TriggerModifyShouldFire()
        {
            _testMockListener.Reset();
            _manager.TriggerOnModified(_mockObject, _mockObject, null);
            Assert.IsTrue(_testMockListener.OnModifiedTriggered);
        }


        [TestMethod]
        public void TriggerModificationFailedShouldFire()
        {
            _testMockListener.Reset();
            _manager.TriggerOnModificationFailed(_mockObject, _mockObject, null);
            Assert.IsTrue(_testMockListener.OnModificationFailedTriggered);
        }


        [TestMethod]
        public void TriggerRemoveShouldFire()
        {
            _testMockListener.Reset();
            _manager.TriggerOnRemoved(_mockObject, null);
            Assert.IsTrue(_testMockListener.OnRemovedTriggered);
        }

        [TestMethod]
        public void TriggerRemoveFailedShouldFire()
        {
            _testMockListener.Reset();
            _manager.TriggerOnRemoveFailed(_mockObject, null);
            Assert.IsTrue(_testMockListener.OnRemoveFailedTriggered);
        }

        [TestMethod]
        public void TriggerOnCreatedShouldTriggerOnInterfaces()
        {
            _manager.TriggerOnCreated(new IMockInterfacedClass(), null);
            var l = _manager.GetListener<MockInterfacedListener>() as MockInterfacedListener;
            Assert.IsTrue(l.OnCreatedTriggered);
        }
    }
}
