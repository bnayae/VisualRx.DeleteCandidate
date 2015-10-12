using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace VisualRx.UnitTests
{
    [TestClass]
    public class VisualRx_API_Tests: VisualRxTestBase
    {
        [TestMethod]
        public async Task Send_Receive_Test()
        {
            // arrange
            var testChannel = new TestVisualRxChannel();
            await Setting.TryAddChannels(testChannel);
            Setting.AddFilter((key, provider) => true);

            // act
            var xs = Observable.Range(0, 10, _scheduler)
                        .Monitor("Test", Setting);
            xs.Subscribe(v => { });
            _scheduler.AdvanceBy(1000);

            // verify
            var expected = Enumerable.Range(0, 10);
            var results = testChannel.Results.Select(m => m.GetValue<int>());
            bool succeed = Enumerable.SequenceEqual(expected, results);
            Assert.IsTrue(succeed);
            Assert.IsTrue(testChannel.Completion.IsCompleted);
        }
    }
}
