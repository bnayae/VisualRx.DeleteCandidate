using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using VisualRx.Publishers.ETW;

namespace VisualRx.UnitTests
{
    [TestClass]
    public class VisualRx_EtwChannel_Tests: VisualRxTestBase
    {
        [TestMethod]
        public async Task Send_Test()
        {
            // arrange
            var channel = new EtwVisualRxChannel();
            await Setting.TryAddChannels(channel);
            Setting.AddFilter((key, provider) => true);

            // act
            var xs = Observable.Range(0, 4, _scheduler)
                        .Select(i => new Person(i))
                        .Monitor("Test", Setting);
            xs.Subscribe(v => { });
            _scheduler.AdvanceBy(5);

            // verify
            var expected = Enumerable.Range(0, 10);
            var results = channel.Results.Select(m => m.GetValue<int>());
            bool succeed = Enumerable.SequenceEqual(expected, results);
            Assert.IsTrue(succeed);
            Assert.IsTrue(channel.Completion.IsCompleted);
        }
    }
}
