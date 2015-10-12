using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;
using VisualRx.Publishers.Common;
using System.Diagnostics;

namespace VisualRx.UnitTests
{
    public class VisualRxTestBase
    {
        protected readonly TestScheduler _scheduler = new TestScheduler();

        [TestInitialize]
        public void Setup()
        {
            Setting.Log = (level, message, ex) => Trace.WriteLine($"VisualRx LOG [{message}]: {message}\r\n\t{ex?.ToString()?.Replace(Environment.NewLine, Environment.NewLine + "\t")}");

            OnSetup();
        }

        public virtual VisualRxSettings Setting { get; } =
            new VisualRxSettings();

        public virtual void OnSetup() { }
    }
}
