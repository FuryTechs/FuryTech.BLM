using FuryTechs.BLM.NetStandard.Extensions;
using FuryTechs.BLM.NetStandard.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace FuryTechs.BLM.NetStandard.Tests
{
    public class AuthorizerTests
    {
        private readonly MockEntity _valid = new MockEntity()
        {
            Id = 1,
            IsValid = true,
            IsVisible = true,
            IsVisible2 = true

        };

        private readonly MockEntity _invalid = new MockEntity()
        {
            Id = 2,
            IsValid = false,
            IsVisible = true,
            IsVisible2 = false
        };

        private readonly MockEntity _invisible = new MockEntity()
        {
            Id = 3,
            IsValid = true,
            IsVisible = false,
            IsVisible2 = true
        };

        private readonly MockGenericIntEntity _validGenericInt = new MockGenericIntEntity()
        {
            Id = 1,
            IsValid = true,
            IsVisible = true,
            IsVisible2 = true

        };

        private readonly MockGenericIntEntity _invalidGenericInt = new MockGenericIntEntity()
        {
            Id = 2,
            IsValid = false,
            IsVisible = true,
            IsVisible2 = false
        };

        private readonly MockGenericIntEntity _invisibleGenericInt = new MockGenericIntEntity()
        {
            Id = 3,
            IsValid = true,
            IsVisible = false,
            IsVisible2 = true
        };



        private readonly MockImplementedEntity _validMockImplementedEntity = new MockImplementedEntity()
        {
            IsValid = true,
            IsVisible = true
        };

        private readonly MockImplementedEntity _invalidMockImplementedEntity = new MockImplementedEntity()
        {
            IsValid = false,
            IsVisible = true
        };

        private readonly MockImplementedEntity _invisibleMockImplementedEntity = new MockImplementedEntity()
        {
            IsVisible = false
        };

        private readonly IServiceProvider serviceProvider;

        public AuthorizerTests()
        {
            var srvCollection = new ServiceCollection();
            srvCollection.AddSingleton<IBlmEntry, MockGenericIntAuthorizer1>();
            srvCollection.AddSingleton<IBlmEntry, MockGenericGuidAuthorizer1>();
            srvCollection.AddSingleton<IBlmEntry, MockGenericIntAuthorizer2>();
            srvCollection.AddSingleton<IBlmEntry, MockGenericGuidAuthorizer2>();
            srvCollection.AddSingleton<IBlmEntry, MockCollectionAuthorizer>();
            srvCollection.AddSingleton<IBlmEntry, MockCollectionAuthorizer2>();
            srvCollection.AddSingleton<IBlmEntry, MockCreateAuthorizer>();
            srvCollection.AddSingleton<IBlmEntry, MockInterfaceAuthorizer>();
            srvCollection.AddSingleton<IBlmEntry, MockModifyAuthorizer>();
            srvCollection.AddSingleton<IBlmEntry, MockRemoveAuthorizer>();
            serviceProvider = srvCollection.BuildServiceProvider();

        }
        
        private readonly IContextInfo _ctx = new GenericContextInfo(new GenericIdentity("carathorys"));

        [Fact]
        public async Task CreateSuccess()
        {
            Assert.True((await Authorize.CreateAsync(_valid, _ctx, serviceProvider)).HasSucceeded());
        }

        [Fact]
        public async Task CreateFail()
        {
            Assert.False((await Authorize.CreateAsync(_invalid, _ctx, serviceProvider)).HasSucceeded());
        }

        [Fact]
        public async Task Modify()
        {
            Assert.True((await Authorize.ModifyAsync(_valid, _valid, _ctx, serviceProvider)).HasSucceeded());

        }

        [Fact]
        public async Task ModifyFails()
        {
            Assert.False((await Authorize.ModifyAsync(_invalid, _invalid, _ctx, serviceProvider)).HasSucceeded());
        }

        [Fact]
        public async Task Remove()
        {
            Assert.True((await Authorize.RemoveAsync(_valid, _ctx, serviceProvider)).HasSucceeded());

        }

        [Fact]
        public async Task RemoveFails()
        {
            Assert.False((await Authorize.RemoveAsync(_invalid, _ctx, serviceProvider)).HasSucceeded());
        }

        [Fact]
        public void Collection()
        {
            var list = new List<MockEntity>()
            {
                _valid, _invalid, _invisible
            }.AsQueryable();

            var authorizedCollection = Authorize.Collection(list, _ctx, serviceProvider);

            Assert.True(authorizedCollection.All(a => a.IsVisible && a.IsVisible2));
        }


        [Fact]
        public void GenericCollection()
        {
            var list = new List<MockGenericEntity<int>>()
            {
                _validGenericInt, _invalidGenericInt, _invisibleGenericInt
            }.AsQueryable();

            var authorizedCollection = Authorize.Collection(list, _ctx, serviceProvider);

            Assert.True(authorizedCollection.All(a => a.IsVisible && a.IsVisible2));
        }


        [Fact]
        public async Task InterfacedCreate()
        {
            var result = await Authorize.CreateAsync(_validMockImplementedEntity, _ctx, serviceProvider);
            Assert.True(result.HasSucceeded());
        }

        [Fact]
        public async Task InterfacedCreateFail()
        {
            var result = await Authorize.CreateAsync(_invalidMockImplementedEntity, _ctx, serviceProvider);
            Assert.False(result.HasSucceeded());
        }


        [Fact]
        public async Task InterfacedModify()
        {
            var result = await Authorize.ModifyAsync(_validMockImplementedEntity, _validMockImplementedEntity, _ctx, serviceProvider);
            Assert.True(result.HasSucceeded());
        }

        [Fact]
        public async Task InterfacedModifyFail()
        {
            var result = await Authorize.ModifyAsync(_validMockImplementedEntity, _invalidMockImplementedEntity, _ctx, serviceProvider);
            Assert.False(result.HasSucceeded());
        }


        [Fact]
        public async Task InterfacedRemove()
        {
            var result = await Authorize.RemoveAsync(_validMockImplementedEntity, _ctx, serviceProvider);
            Assert.True(result.HasSucceeded());
        }

        [Fact]
        public async Task InterfacedRemovefail()
        {
            var result = await Authorize.RemoveAsync(_invalidMockImplementedEntity, _ctx, serviceProvider);
            Assert.False(result.HasSucceeded());
        }

        [Fact]
        public async Task InterfacedCollection()
        {
            var collection = new List<MockImplementedEntity>()
            {
                _validMockImplementedEntity,
                _invalidMockImplementedEntity,
                _invisibleMockImplementedEntity
            };

            var authorized = await Authorize.CollectionAsync(collection.AsQueryable(), _ctx, serviceProvider);
            Assert.True(authorized.Any(a => a.IsVisible));

        }

    }
}
