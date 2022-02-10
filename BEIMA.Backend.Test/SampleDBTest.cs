using NUnit.Framework;

namespace BEIMA.Backend.Test
{
    [TestFixture]
    public class SampleDBTest
    {
        [Test]
        public void InitialState_Action_ExpectedResult()
        {
            Assert.That("Test", Is.EqualTo("Test"));
        }
    }
}
