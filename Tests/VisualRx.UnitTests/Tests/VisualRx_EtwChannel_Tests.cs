using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using VisualRx.ETW.Publishers;
using VisualRx.ETW.Listeners;
using System.Threading;
using VisualRx.Contracts;
using Microsoft.Reactive.Testing;

namespace VisualRx.UnitTests
{
    [TestClass]
    public class VisualRx_EtwChannel_Tests: VisualRxTestsBase
    {
        [TestMethod]
        public async Task ETW_Send_Receive_Test()
        {
            // arrange
            using (var listener = new VisualRxEtwListener())
            {
                // listen
                IObservable<Marble> listenStream = 
                    await listener.Connect(CancellationToken.None);
                ITestableObserver<Marble> listenObserver =
                    _scheduler.CreateObserver<Marble>();
                listenStream.Subscribe(listenObserver);

                // publish
                var channel = new EtwVisualRxChannel();
                await PublisherSetting.TryAddChannels(channel);
                PublisherSetting.AddFilter((key, provider) => true);

                // act
                var xs = Observable.Range(0, 4, _scheduler)
                            .Select(i => new Person(i))
                            .Monitor("Test", PublisherSetting);
                xs.Subscribe(v => { });
                _scheduler.AdvanceBy(5);

                await listenStream;
                //// verify
                //var expected = Enumerable.Range(0, 10);
                //var results = channel.Results.Select(m => m.GetValue<int>());
                //bool succeed = Enumerable.SequenceEqual(expected, results);
                //Assert.IsTrue(succeed);
                //Assert.IsTrue(channel.Completion.IsCompleted);
                await Task.Delay(10000);
            }
        }
    }
}
