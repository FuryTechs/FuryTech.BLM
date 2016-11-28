using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLM.Tests
{
    [TestClass]
    public class ElevatedContextTests
    {
        [TestMethod]
        public void NotElevatedByDefault()
        {
            Assert.IsFalse(ElevatedContext.IsElevated());
        }

        [TestMethod]
        public void ElevatedValues()
        {
            Assert.IsFalse(ElevatedContext.IsElevated());
            using (new ElevatedContext())
            {
                Assert.IsTrue(ElevatedContext.IsElevated());
            }
            Assert.IsFalse(ElevatedContext.IsElevated());
        }
    }
}
