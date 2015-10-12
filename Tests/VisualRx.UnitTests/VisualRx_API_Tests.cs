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
            _scheduler.AdvanceBy(11);

            // verify
            var expected = Enumerable.Range(0, 10);
            var results = testChannel.Results.Select(m => m.GetValue<int>());
            bool succeed = Enumerable.SequenceEqual(expected, results);
            Assert.IsTrue(succeed);
            Assert.IsTrue(testChannel.Completion.IsCompleted);
        }

        [TestMethod]
        public async Task Filter_Test()
        {
            // arrange
            var testChannel = new TestVisualRxChannel();
            await Setting.TryAddChannels(testChannel);

            // act
            var xs = Observable.Range(0, 10, _scheduler)
                        .Monitor("Test", Setting)
                        .Monitor("Test not filtered", Setting);
            xs.Subscribe(v => { });
            _scheduler.AdvanceBy(5);
            Setting.AddFilter((key, provider) => key == "Test");
            _scheduler.AdvanceBy(6);

            // verify
            var expected = Enumerable.Range(5, 5);
            var results = testChannel.Results.Select(m => m.GetValue<int>());
            bool succeed = Enumerable.SequenceEqual(expected, results);
            Assert.IsTrue(succeed);
            Assert.IsTrue(testChannel.Completion.IsCompleted);
        }

        [TestMethod]
        public async Task MultiChannel_Filter_Test()
        {
            // arrange
            var testChannelA = new TestVisualRxChannel(Guid.NewGuid());
            var testChannelB = new TestVisualRxChannel(Guid.NewGuid());
            await Setting.TryAddChannels(testChannelA, testChannelB);

            // act
            var xs = Observable.Range(0, 10, _scheduler)
                        .Monitor("Test", Setting);
            xs.Subscribe(v => { });
            Setting.AddFilter((key, channel) => channel == testChannelA);
            _scheduler.AdvanceBy(11);

            // verify
            var expected = Enumerable.Range(0, 10);
            var results = testChannelA.Results.Select(m => m.GetValue<int>());
            bool succeed = Enumerable.SequenceEqual(expected, results);
            Assert.IsTrue(succeed);
            Assert.IsTrue(testChannelA.Completion.IsCompleted);
            Assert.AreEqual(0, testChannelB.Results.Length);
        }
    }
}
