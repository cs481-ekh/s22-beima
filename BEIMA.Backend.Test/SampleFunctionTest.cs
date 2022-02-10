using NUnit.Framework;

namespace BEIMA.Backend.Test
{
    [TestFixture]
    public class SampleFunctionTest
    {
        [Test]
        public void InitialState_Action_ExpectedResult()
        {
            Assert.That(23, Is.Not.Null);
        }
    }
}
