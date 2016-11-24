using System;
using System.Linq;
using System.Security.Principal;
using BLM.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.Tests
{
    [TestClass]
    public class AuthorizerManagerTests
    {
        class ClassWithoutAuthorizer { }

        class ClassWithOneAuthorizer { }
        class MockAuth1 : IAuthorizer<ClassWithOneAuthorizer>
        {
            public IQueryable<ClassWithOneAuthorizer> AuthorizeCollection(IIdentity usr, IQueryable<ClassWithOneAuthorizer> entities)
            {
                throw new NotImplementedException();
            }

            public bool CanRemove(IIdentity usr, ClassWithOneAuthorizer entry)
            {
                throw new NotImplementedException();
            }

            public bool CanInsert(IIdentity usr, ClassWithOneAuthorizer entry)
            {
                throw new NotImplementedException();
            }

            public bool CanUpdate(IIdentity usr, ClassWithOneAuthorizer original, ClassWithOneAuthorizer modified)
            {
                throw new NotImplementedException();
            }
        }

        class ClassWithTwoAuthorizers { }

        class MockAuthMultiple1 : IAuthorizer<ClassWithTwoAuthorizers>
        {
            public IQueryable<ClassWithTwoAuthorizers> AuthorizeCollection(IIdentity usr, IQueryable<ClassWithTwoAuthorizers> entities)
            {
                throw new NotImplementedException();
            }

            public bool CanRemove(IIdentity usr, ClassWithTwoAuthorizers entry)
            {
                throw new NotImplementedException();
            }

            public bool CanInsert(IIdentity usr, ClassWithTwoAuthorizers entry)
            {
                throw new NotImplementedException();
            }

            public bool CanUpdate(IIdentity usr, ClassWithTwoAuthorizers original, ClassWithTwoAuthorizers modified)
            {
                throw new NotImplementedException();
            }
        }
        class MockAuthMultiple2 : IAuthorizer<ClassWithTwoAuthorizers>
        {
            public IQueryable<ClassWithTwoAuthorizers> AuthorizeCollection(IIdentity usr, IQueryable<ClassWithTwoAuthorizers> entities)
            {
                throw new NotImplementedException();
            }

            public bool CanRemove(IIdentity usr, ClassWithTwoAuthorizers entry)
            {
                throw new NotImplementedException();
            }

            public bool CanInsert(IIdentity usr, ClassWithTwoAuthorizers entry)
            {
                throw new NotImplementedException();
            }

            public bool CanUpdate(IIdentity usr, ClassWithTwoAuthorizers original, ClassWithTwoAuthorizers modified)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizerNotFoundException))]
        public void ShouldFailWithoutAuthorizer(){
            AuthorizerManager.GetAuthorizer<ClassWithoutAuthorizer>();
        }

        [TestMethod]
        public void ShouldGetAuthorizerWhenAvailable()
        {
            var auth = AuthorizerManager.GetAuthorizer<ClassWithOneAuthorizer>();
            Assert.IsInstanceOfType(auth, typeof(IAuthorizer<ClassWithOneAuthorizer>));
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizerMismatchException))]
        public void ShouldThrowExceptionOnMismatch()
        {
            var auth = AuthorizerManager.GetAuthorizer<ClassWithTwoAuthorizers>();
        }

    }
}
