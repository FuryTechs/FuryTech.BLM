using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FuryTechs.BLM.EntityFrameworkCore.Identity;
using FuryTechs.BLM.NetStandard.Interfaces;
using FuryTechs.BLM.NetStandard.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    public class DependencyInjectionTessts
    {
        public DependencyInjectionTessts(ITestOutputHelper output)
        {
            _serviceCollection = InitializeServiceCollection(output);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        protected readonly IServiceCollection _serviceCollection;
        protected readonly IServiceProvider _serviceProvider;

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

            coll.AddBlmEfCoreDefaultDbContext<FakeDbContext>();
            return coll;
        }

        [Fact]
        public void TypeofIRepositoryWithDbContext()
        {
            var repository = (IRepository<MockEntity>)_serviceProvider.GetService(typeof(IRepository<MockEntity, FakeDbContext>));
            Assert.NotNull(repository);
            var childRepository = repository.GetChildRepositoryFor<MockGenericIntEntity>();
            Assert.NotNull(childRepository);
        }

        [Fact]
        public void TypeofIRepositoryWithoutDbContext()
        {
            var repository = (IRepository<MockEntity>)_serviceProvider.GetService(typeof(IRepository<MockEntity>));
            Assert.NotNull(repository);
            var childRepository = repository.GetChildRepositoryFor<MockGenericIntEntity>();
            Assert.NotNull(childRepository);
        }

        [Fact]
        public void GenericIRepositoryWithoutDbContext()
        {
            var repository = _serviceProvider.GetService<IRepository<MockEntity>>();
            Assert.NotNull(repository);
            var childRepository = repository.GetChildRepositoryFor<MockGenericIntEntity>();
            Assert.NotNull(childRepository);
        }


        [Fact]
        public void TypeOfEfRepositoryWithDbContext()
        {
            var repository = (IRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity, FakeDbContext>));
            Assert.NotNull(repository);
            var childRepository = repository.GetChildRepositoryFor<MockGenericIntEntity>();
            Assert.NotNull(childRepository);
        }

        [Fact]
        public void GenericEfRepositoryWithDbContext()
        {
            var repository = _serviceProvider.GetService<EfRepository<MockEntity, FakeDbContext>>();
            Assert.NotNull(repository);
            var childRepository = repository.GetChildRepositoryFor<MockGenericIntEntity>();
            Assert.NotNull(childRepository);
        }
        [Fact]
        public void TypeOfEfRepositoryWithoutDbContext()
        {
            var repository = (IRepository<MockEntity>)_serviceProvider.GetService(typeof(EfRepository<MockEntity>));
            Assert.NotNull(repository);
            var childRepository = repository.GetChildRepositoryFor<MockGenericIntEntity>();
            Assert.NotNull(childRepository);
        }

        [Fact]
        public void GenericEfRepositoryWithoutDbContext()
        {
            var repository = _serviceProvider.GetService<EfRepository<MockEntity>>();
            Assert.NotNull(repository);
            var childRepository = repository.GetChildRepositoryFor<MockGenericIntEntity>();
            Assert.NotNull(childRepository);
        }

        [Fact]
        public void ResolveChildRepository()
        {
            var repository = _serviceProvider.GetService<EfRepository<MockEntity>>();
            Assert.NotNull(repository);
            
        }
    }
}