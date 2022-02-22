using System;
using MongoDB.Bson;
using NUnit.Framework;

namespace BEIMA.Backend.MongoService.Test
{
    [TestFixture]
    public class BaseDeviceTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void InitialState_Action_ExpectedResult()
        {
            var objId = new ObjectId("61f4882f42dab933544849d3");
            var device = new BaseDevice(objId, "asd", "asd", "xcsd", "cvliokj", "asd", 1, "sadloifk");
            device.AddField("asdfwefsdf", "test-value");
            device.AddField("abc", "test2");
            Console.WriteLine(device.GetBsonDocument().ToString());
        }
    }
}
