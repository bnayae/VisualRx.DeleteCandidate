using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;

namespace VisualRx.UnitTests
{
    [TestClass]
    public class VisualRx_API_Tests: VisualRxTestBase
    {
        [TestMethod]
        public void Send_Receive_Test()
        {
            var testChannel = new TestVisualRxChannel(_scheduler);
            Setting.TryAddProxies(testChannel);
            var xs = Observable.Range(0, 10)
                        .Monitor("Test");

        }
    }
}
