using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.Tests
{

    class MockEntity
    {
        public int Id { get; set; }
        public bool IsValid { get; set; }
        public bool IsVisible { get; set; }

        public bool IsVisible2 { get; set; }

        public string Guid { get; set; }
    }

    class MockCreateAuthorizer : IAuthorizeCreate<MockEntity> {
        public bool CanCreate(MockEntity entity, IContextInfo ctx)
        {
            return entity.IsValid;
        }
    }

    class MockModifyAuthorizer : IAuthorizeModify<MockEntity> {
        public bool CanModify(MockEntity originalEntity, MockEntity modifiedEntity, IContextInfo ctx)
        {
            return modifiedEntity.IsValid;
        }
    }

    class MockRemoveAuthorizer : IAuthorizeRemove<MockEntity>
    {
        public bool CanRemove(MockEntity entity, IContextInfo ctx)
        {
            return entity.IsValid;
        }
    }

    class MockCollectionAuthorizer : IAuthorizeCollection<MockEntity>
    {
        public IQueryable<MockEntity> AuthorizeCollection(IQueryable<MockEntity> entities, IContextInfo ctx)
        {
            return entities.Where(a=>a.IsVisible);
        }
    }

    class MockCollectionAuthorizer2 : IAuthorizeCollection<MockEntity>
    {
        public IQueryable<MockEntity> AuthorizeCollection(IQueryable<MockEntity> entities, IContextInfo ctx)
        {
            return entities.Where(a => a.IsVisible2);
        }
    }


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
            var creationErrors = Authorize.Create(valid, ctx);
            Assert.AreEqual(0, creationErrors.Count());
        }

        [TestMethod]
        public void CreateFail()
        {
            var creationErrors = Authorize.Create(invalid, ctx);
            Assert.AreEqual(1, creationErrors.Count());
        }

        [TestMethod]
        public void Modify()
        {
            var modErrors = Authorize.Modify(valid, valid, ctx);
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
            var errors = Authorize.Remove(valid, ctx);
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
