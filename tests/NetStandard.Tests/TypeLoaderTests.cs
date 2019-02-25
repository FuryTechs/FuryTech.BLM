using System;
using System.Linq;
using System.Threading.Tasks;

using FuryTechs.BLM.NetStandard.Interfaces;

using Xunit;

namespace FuryTechs.BLM.NetStandard.Tests
{
    //class DummyClass
    //{
    //    public int Id { get; set; }
    //}

    //class DummyAuthorizeCollection : AuthorizeCollection<DummyClass>
    //{
    //    public override async Task<IQueryable<DummyClass>> AuthorizeCollectionAsync(IQueryable<DummyClass> entities, IContextInfo ctx)
    //    {
    //        return await Task.Factory.StartNew(() => entities);
    //    }
    //}

    //public class TypeLoaderTests
    //{
    //    [Fact]
    //    public void LoadTypes()
    //    {
    //        var types = Loader.Types;
    //        Assert.NotNull(types);
    //        Assert.True(types.All(t => t.GetInterfaces().Contains(typeof(IBlmEntry))));
    //    }

    //    [Fact]
    //    public void GetInstance()
    //    {
    //        var instance = Loader.GetInstance<DummyAuthorizeCollection>();
    //        Assert.NotNull(instance);

    //        // And... again from cache
    //        var instance2 = Loader.GetInstance<DummyAuthorizeCollection>();
    //        Assert.NotNull(instance2);
    //    }

    //    [Fact]
    //    public void GetEntriesForType()
    //    {
    //        var entryList = Loader.GetEntriesFor<AuthorizeCollection<DummyClass>>();
    //        Assert.NotNull(entryList);

    //        foreach (var instance in entryList)
    //        {
    //            Assert.IsAssignableFrom<AuthorizeCollection<DummyClass>>(instance);
    //        }

    //        // And...again from cache
    //        var entryList2 = Loader.GetEntriesFor<AuthorizeCollection<DummyClass>>();
    //        Assert.NotNull(entryList2);

    //        foreach (var instance in entryList2)
    //        {
    //            Assert.IsAssignableFrom<AuthorizeCollection<DummyClass>>(instance);
    //        }
    //    }
    //}
}
