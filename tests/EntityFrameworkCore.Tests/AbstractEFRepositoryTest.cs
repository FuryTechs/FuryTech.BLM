using System;
using System.Security.Principal;

using Microsoft.EntityFrameworkCore;

using Xunit;

using FuryTechs.BLM.NetStandard.Tests;
using Xunit.Abstractions;
using System.Reflection;
using System.Collections.Generic;
using FuryTechs.BLM.NetStandard.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    public class AbstractEfRepositoryTest: IDisposable
    {
        protected IIdentity _identity;

        protected IServiceCollection _serviceCollection;
        protected IServiceProvider _serviceProvider;

        static IServiceCollection InitializeServiceCollection(ITestOutputHelper output)
        {
            var type = output.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            var test = (ITest)testMember.GetValue(output);
            var name = $"{test.DisplayName}.{test.TestCase}-{Guid.NewGuid()}";

            var coll = new ServiceCollection();

            coll.AddDbContext<FakeDbContext>(o =>
            {
                o.UseInMemoryDatabase(name);
            });

            coll.AddBlmEfCore();
            coll.AddSingleton<IBlmEntry, MockCollectionAuthorizer>();
            coll.AddSingleton<IBlmEntry, MockCollectionAuthorizer2>();
            coll.AddSingleton<IBlmEntry, MockCreateAuthorizer>();
            coll.AddSingleton<IBlmEntry, MockInterfaceAuthorizer>();
            coll.AddSingleton<IBlmEntry, MockModifyAuthorizer>();
            coll.AddSingleton<IBlmEntry, MockRemoveAuthorizer>();
            coll.AddSingleton<IBlmEntry, MockInterpretedEntityCreateInterpreter>();
            coll.AddSingleton<IBlmEntry, MockInterpretedEntityModifyInterpreter>();
            coll.AddScoped<IBlmEntry, EfChangeListener>();
            return coll;
        }

        public void Dispose()
        {
            var dbContext = _serviceProvider.GetService<FakeDbContext>();
            dbContext.MockEntities.RemoveRange(dbContext.MockEntities);
            dbContext.MockInterpretedEntities.RemoveRange(dbContext.MockInterpretedEntities);
            dbContext.MockNestedEntities.RemoveRange(dbContext.MockNestedEntities);

        }

        public AbstractEfRepositoryTest(ITestOutputHelper output)
        {
            /// In EFCore 1.x there is no transient InMemory db, so we'll need to generate spearated db-s for testing.
            /// In EFCore 2.x there will be a TransientInMemoryDatabase, so we'll have to use that later.

            _serviceCollection = InitializeServiceCollection(output);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            _identity = new GenericIdentity("gallayb");

            EfChangeListener.Reset();
            Entity1 = new MockEntity()
            {
                Id = 1,
                IsValid = true,
                IsVisible = true,
                IsVisible2 = true
            };

            Entity2 = new LogicalDeleteEntity()
            {
                Id = 2,
                IsValid = true,
                IsVisible = true,
                IsVisible2 = true
            };

            Entity3 = new LogicalDeleteEntity()
            {
                Id = 3,
                IsValid = true,
                IsVisible = true,
                IsVisible2 = true
            };

            Entity4 = new InheritedLogicalDeleteEntity()
            {
                Id = 2,
                IsValid = true,
                IsVisible = true,
                IsVisible2 = true
            };

            Entity5 = new InheritedLogicalDeleteEntity()
            {
                Id = 3,
                IsValid = true,
                IsVisible = true,
                IsVisible2 = true
            };
        }

        protected MockEntity Entity1 { get; set; }
        protected LogicalDeleteEntity Entity2 { get; set; }
        protected LogicalDeleteEntity Entity3 { get; set; }

        protected InheritedLogicalDeleteEntity Entity4 { get; set; }
        protected InheritedLogicalDeleteEntity Entity5 { get; set; }
    }
}
