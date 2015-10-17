using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;
using VisualRx.Publishers.Common;
using System.Diagnostics;

namespace VisualRx.UnitTests
{
    public class VisualRxTestsBase
    {
        protected readonly TestScheduler _scheduler = new TestScheduler();

        [TestInitialize]
        public void Setup()
        {
            OnSetup();
        }

        public virtual void OnSetup()
        {
            PublisherSetting.Log = (level, message, ex) => Trace.WriteLine($"VisualRx LOG [{message}]: {message}\r\n\t{ex?.ToString()?.Replace(Environment.NewLine, Environment.NewLine + "\t")}");
        }
        public virtual VisualRxPublishersSettings PublisherSetting { get; } =
            new VisualRxPublishersSettings();
    }
}
