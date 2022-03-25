using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lucky.JsonDb;

namespace Lucky.DataSchema {
    [TestClass]
    public class DataSchemaTests {
        [TestMethod]
        public void ServerJsonDbTest() {
            DataSchemaLogger logger = new DataSchemaLogger(typeof(ServerJsonDb));
            Assert.IsTrue(logger.Log());
        }

        [TestMethod]
        public void LocalJsonDbTest() {
            DataSchemaLogger logger = new DataSchemaLogger(typeof(LocalJsonDb));
            Assert.IsTrue(logger.Log());
        }

        [TestMethod]
        public void GpuProfilesJsonDbTest() {
            DataSchemaLogger logger = new DataSchemaLogger(typeof(GpuProfilesJsonDb));
            Assert.IsTrue(logger.Log());
        }
    }
}
