using System;
using System.Linq;
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
            public bool CanInsert(ClassWithOneAuthorizer entity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public bool CanUpdate(ClassWithOneAuthorizer originalEntity, ClassWithOneAuthorizer modifiedEntity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public bool CanRemove(ClassWithOneAuthorizer entity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public IQueryable<ClassWithOneAuthorizer> AuthorizeCollection(IQueryable<ClassWithOneAuthorizer> entities, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }
        }

        class ClassWithTwoAuthorizers { }

        class MockAuthMultiple1 : IAuthorizer<ClassWithTwoAuthorizers>
        {
            public bool CanInsert(ClassWithTwoAuthorizers entity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public bool CanUpdate(ClassWithTwoAuthorizers originalEntity, ClassWithTwoAuthorizers modifiedEntity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public bool CanRemove(ClassWithTwoAuthorizers entity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public IQueryable<ClassWithTwoAuthorizers> AuthorizeCollection(IQueryable<ClassWithTwoAuthorizers> entities, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }
        }
        class MockAuthMultiple2 : IAuthorizer<ClassWithTwoAuthorizers>
        {
            public bool CanInsert(ClassWithTwoAuthorizers entity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public bool CanUpdate(ClassWithTwoAuthorizers originalEntity, ClassWithTwoAuthorizers modifiedEntity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public bool CanRemove(ClassWithTwoAuthorizers entity, IContextInfo ctx)
            {
                throw new NotImplementedException();
            }

            public IQueryable<ClassWithTwoAuthorizers> AuthorizeCollection(IQueryable<ClassWithTwoAuthorizers> entities, IContextInfo ctx)
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
