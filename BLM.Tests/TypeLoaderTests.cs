using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.Tests
{
    [TestClass]
    public class TypeLoaderTests
    {
        [TestMethod]
        public void LoadTypes()
        {
            var types = BlmTypeLoader.GetLoadedTypes();
            Assert.IsNotNull(types);
        }
    }
}
