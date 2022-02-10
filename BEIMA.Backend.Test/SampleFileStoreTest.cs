using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.Test
{
    [TestFixture]
    public class SampleFileStoreTest
    {
        [Test]
        public void InitialState_Action_ExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => { throw new InvalidOperationException(); });
        }
    }
}
