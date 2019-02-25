using FuryTechs.BLM.NetStandard.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Xunit;

namespace FuryTechs.BLM.NetStandard.Tests.Generic
{
    public class GenericTests
    {
        private readonly GenericEntity<int> _valid = new GenericEntity<int>()
        {
            Id = 1,
            IsValid = true,
            IsVisible = true,
            IsVisible2 = true

        };

        private readonly GenericEntity<int> _invalid = new GenericEntity<int>()
        {
            Id = 2,
            IsValid = false,
            IsVisible = true,
            IsVisible2 = false
        };

        private readonly GenericEntity<int> _invisible = new GenericEntity<int>()
        {
            Id = 3,
            IsValid = true,
            IsVisible = false,
            IsVisible2 = true
        };

        private readonly IServiceProvider serviceProvider;
        readonly IContextInfo _ctx = new GenericContextInfo(new GenericIdentity("gallayb"));

        public GenericTests()
        {
            var srvCollection = new ServiceCollection();
            srvCollection.AddSingleton<IBlmEntry, MockCollectionAuthorizerGeneric<int>>();
            srvCollection.AddSingleton<IBlmEntry, MockCollectionAuthorizerIGeneric<int>>();
            serviceProvider = srvCollection.BuildServiceProvider();

        }

        [Fact]
        public void TestGenericAuthorizeCollection()
        {
            var list = new List<GenericEntity<int>>()
            {
                _valid, _invalid, _invisible
            }.AsQueryable();

            var authorizedCollection = Authorize.Collection(list, _ctx, serviceProvider);

            Assert.True(authorizedCollection.All(a => a.IsVisible && a.IsVisible2));
        }

    }
}
