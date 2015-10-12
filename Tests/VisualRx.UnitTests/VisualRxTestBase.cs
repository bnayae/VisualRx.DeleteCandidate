using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;
using VisualRx.Publishers.Common;

namespace VisualRx.UnitTests
{
    public class VisualRxTestBase
    {
        protected readonly TestScheduler _scheduler = new TestScheduler();

        [TestInitialize]
        public void Setup()
        {
            OnSetup();
        }

        public virtual VisualRxSettings Setting =>
            new VisualRxSettings();

        public virtual void OnSetup() { }
    }
}
