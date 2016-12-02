using System.Linq;
using System.Threading.Tasks;
using BLM.Interfaces.Authorize;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.Tests
{
    class DummyClass
    {
        public int Id { get; set; }
    }

    class DummyAuthorizeCollection : IAuthorizeCollection<DummyClass, DummyClass>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IQueryable<DummyClass>> AuthorizeCollection(IQueryable<DummyClass> entities, IContextInfo ctx)
        {
            return entities;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }

    [TestClass]
    public class TypeLoaderTests
    {
        [TestMethod]
        public void LoadTypes()
        {
            var types = Loader.Types;
            Assert.IsNotNull(types);
            Assert.IsTrue(types.All(t => t.GetInterfaces().Contains(typeof(IBlmEntry))));
        }

        [TestMethod]
        public void GetInstance()
        {
            var instance = Loader.GetInstance<DummyAuthorizeCollection>();
            Assert.IsNotNull(instance);

            // And... again from cache
            var instance2 = Loader.GetInstance<DummyAuthorizeCollection>();
            Assert.IsNotNull(instance2);
        }

        [TestMethod]
        public void GetEntriesForType()
        {
            var entryList = Loader.GetEntriesFor<IAuthorizeCollection<DummyClass>>();
            Assert.IsNotNull(entryList);

            foreach (var instance in entryList)
            {
                Assert.IsInstanceOfType(instance, typeof(IAuthorizeCollection<DummyClass>));
            }

            // And...again from cache
            var entryList2 = Loader.GetEntriesFor<IAuthorizeCollection<DummyClass>>();
            Assert.IsNotNull(entryList2);

            foreach (var instance in entryList2)
            {
                Assert.IsInstanceOfType(instance, typeof(IAuthorizeCollection<DummyClass>));

            }
        }
    }
}
