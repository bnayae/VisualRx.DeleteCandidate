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
using System.Reactive;
using Newtonsoft.Json;

namespace VisualRx.UnitTests
{
    [TestClass]
    public class VisualRx_EtwChannel_Tests : VisualRxTestsBase
    {
        [TestMethod]
        public async Task ETW_Send_Receive_Test()
        {
            const int count = 4;
            // arrange
            using (var listener = new VisualRxEtwListener())
            {
                // listen
                IObservable<Marble> listenStream =
                    await listener.GetStreamAsync();
                listenStream = listenStream.Replay().RefCount();
                ITestableObserver<Marble> listenObserver =
                    _scheduler.CreateObserver<Marble>();
                listenStream
                    .Subscribe(listenObserver);

                // publish
                var channel = new EtwVisualRxChannel();
                await PublisherSetting.TryAddChannels(channel);
                PublisherSetting.AddFilter((key, provider) => true);

                // act
                var xs = Observable.Range(0, count, _scheduler)
                            .Select(i => new Person(i))
                            .Monitor("Test", PublisherSetting);
                xs.Subscribe(v => { });
                _scheduler.AdvanceBy(5);

                await listenStream
                    //.Do(v => { },() => { })
                    .Take(count + 1);
                for (int i = 0; i < count; i++)
                {
                    Assert.AreEqual(NotificationKind.OnNext,
                        listenObserver.Messages[i].Value.Kind);
                    var marble = listenObserver.Messages[i].Value.Value;
                    var p = marble.GetValue<Person>();
                    Assert.AreEqual(i, p.Id);
                }
                Marble lastMarble = listenObserver.Messages[count].Value.Value;
                Assert.AreEqual(NotificationKind.OnCompleted,
                    lastMarble.Kind);
            }
        }

        //[TestMethod]
        public async Task ETW_MultiListeners_Test()
        {
            throw new NotImplementedException();
        }
    }
}
