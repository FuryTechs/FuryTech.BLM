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
            var creationErrors = Authorize.Create(valid, ctx).Where(a=>!a.HasSucceed);
            Assert.AreEqual(0, creationErrors.Count());
        }

        [TestMethod]
        public void CreateFail()
        {
            var creationErrors = Authorize.Create(invalid, ctx).Where(a=>!a.HasSucceed);
            Assert.AreEqual(1, creationErrors.Count());
        }

        [TestMethod]
        public void Modify()
        {
            var modErrors = Authorize.Modify(valid, valid, ctx).Where(a=>!a.HasSucceed);
            Assert.AreEqual(0, modErrors.Count());

        }

        [TestMethod]
        public void ModifyFails()
        {
            var modErrors = Authorize.Modify(valid, invalid, ctx);
            Assert.AreEqual(1, modErrors.Count());
        }

        [TestMethod]
        public void Remove()
        {
            var errors = Authorize.Remove(valid, ctx).Where(a=>!a.HasSucceed);
            Assert.AreEqual(0, errors.Count());

        }

        [TestMethod]
        public void RemoveFails()
        {
            var errors = Authorize.Remove(invalid, ctx);
            Assert.AreEqual(1, errors.Count());
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
    }
}
