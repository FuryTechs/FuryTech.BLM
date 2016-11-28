using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.Tests
{
    [TestClass]
    public class AuthorizerTests
    {
        private MockEntity valid = new MockEntity()
        {
            Id = 1,
            IsValid = true,
            IsVisible = true,
            IsVisible2 = true

        };

        private MockEntity invalid = new MockEntity()
        {
            Id = 2,
            IsValid = false,
            IsVisible = true,
            IsVisible2 = false
        };

        private MockEntity invisible = new MockEntity()
        {
            Id = 3,
            IsValid = true,
            IsVisible = false,
            IsVisible2 = true
        };

        IContextInfo ctx = new GenericContextInfo(Thread.CurrentPrincipal.Identity);

        [TestMethod]
        public void CreateSuccess()
        {
            var createResult = Authorize.Create(valid, ctx).CreateAggregateResult();
            Assert.IsTrue(createResult.HasSucceed);
        }

        [TestMethod]
        public void CreateFail()
        {
            var createResult = Authorize.Create(invalid, ctx).CreateAggregateResult();
            Assert.IsFalse(createResult.HasSucceed);
        }

        [TestMethod]
        public void CreateSuccessElevated()
        {
            using (new ElevatedContext())
            {
                var creationResult = Authorize.Create(invalid, ctx).CreateAggregateResult();
                Assert.IsTrue(creationResult.HasSucceed);
            }
        }

        [TestMethod]
        public void Modify()
        {
            var modResult = Authorize.Modify(valid, valid, ctx).CreateAggregateResult();
            Assert.IsTrue(modResult.HasSucceed);

        }

        [TestMethod]
        public void ModifyFails()
        {
            var modResult = Authorize.Modify(invalid, invalid, ctx).CreateAggregateResult();
            Assert.IsFalse(modResult.HasSucceed);
        }

        public void ModifyElevated()
        {
            using (new ElevatedContext())
            {
                var modResult = Authorize.Modify(invalid, invalid, ctx).CreateAggregateResult();
                Assert.IsTrue(modResult.HasSucceed);
            }
        }

        [TestMethod]
        public void Remove()
        {
            var result = Authorize.Remove(valid, ctx).CreateAggregateResult();
            Assert.IsTrue(result.HasSucceed);

        }

        [TestMethod]
        public void RemoveFails()
        {
            var result = Authorize.Remove(invalid, ctx).CreateAggregateResult();
            Assert.IsFalse(result.HasSucceed);
        }

        [TestMethod]
        public void RemoveElevated()
        {
            using (new ElevatedContext())
            {
                var result = Authorize.Remove(invalid, ctx).CreateAggregateResult();
                Assert.IsTrue(result.HasSucceed);
            }
        }

        [TestMethod]
        public void AuthorizeCollection()
        {
            var list = new List<MockEntity>()
            {
                valid, invalid, invisible
            }.AsQueryable();

            var authorizedCollection = Authorize.Collection(list, ctx);

            Assert.IsTrue(authorizedCollection.All(a=>a.IsVisible && a.IsVisible2));
        }

        [TestMethod]
        public void AuthorizeCollectionElevated()
        {
            using (new ElevatedContext())
            {
                var list = new List<MockEntity>()
            {
                valid, invalid, invisible
            }.AsQueryable();

                var authorizedCollection = Authorize.Collection(list, ctx);

                Assert.IsTrue(authorizedCollection.Any(a => a.IsVisible || a.IsVisible2));
            }

        }
    }
}
