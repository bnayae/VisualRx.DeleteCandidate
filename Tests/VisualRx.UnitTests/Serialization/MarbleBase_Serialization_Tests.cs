using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualRx.Contracts;
using Newtonsoft.Json;

namespace VisualRx.UnitTests
{
    [TestClass]
    public class MarbleBase_Serialization_Tests
    {
        [TestMethod]
        public void SerializeNext_Test()
        {
            // arrange
            var msg = Marble.CreateNext("Test Marble",
                    7,
                    TimeSpan.FromMilliseconds(10),
                    "Test Machine");

            // act
            string json = JsonConvert.SerializeObject(msg);
            Marble result = JsonConvert.DeserializeObject<Marble>(json);

            // verify
            Assert.AreEqual(msg.StreamKey, result.StreamKey);
            Assert.AreEqual(msg.Kind, result.Kind);
            Assert.AreEqual(msg.IndexOrder, result.IndexOrder);
            Assert.AreEqual(msg.DateCreatedUtc, result.DateCreatedUtc);
            Assert.AreEqual(msg.MachineName, result.MachineName);
            Assert.AreEqual(msg.Offset, result.Offset);
            Assert.AreEqual(msg.GetValue<int>(), result.GetValue<int>());
            Assert.IsTrue(json.Contains($"\"Kind\":\"{MarbleKind.OnNext}\""));
        }

        [TestMethod]
        public void SerializeError_Test()
        {
            // arrange
            var ex = new ArgumentException("Bad");
            var msg = Marble.CreateError("Test Marble",
                    ex,
                    TimeSpan.FromMilliseconds(10),
                    "Test Machine");

            // act
            string json = JsonConvert.SerializeObject(msg);
            Marble result = JsonConvert.DeserializeObject<Marble>(json);

            // verify
            Assert.AreEqual(msg.StreamKey, result.StreamKey);
            Assert.AreEqual(msg.Kind, result.Kind);
            Assert.AreEqual(msg.IndexOrder, result.IndexOrder);
            Assert.AreEqual(msg.DateCreatedUtc, result.DateCreatedUtc);
            Assert.AreEqual(msg.MachineName, result.MachineName);
            Assert.AreEqual(msg.Offset, result.Offset);
            Assert.AreEqual(msg.GetValue<ArgumentException>().Message, result.GetValue<ArgumentException>().Message);
            Assert.IsTrue(json.Contains($"\"Kind\":\"{MarbleKind.OnError}\""));
        }

        [TestMethod]
        public void SerializeCompleted_Test()
        {
            // arrange
            var msg = Marble.CreateCompleted("Test Marble",
                    TimeSpan.FromMilliseconds(10),
                    "Test Machine");

            // act
            string json = JsonConvert.SerializeObject(msg);
            Marble result = JsonConvert.DeserializeObject<Marble>(json);

            // verify
            Assert.AreEqual(msg.StreamKey, result.StreamKey);
            Assert.AreEqual(msg.Kind, result.Kind);
            Assert.AreEqual(msg.IndexOrder, result.IndexOrder);
            Assert.AreEqual(msg.DateCreatedUtc, result.DateCreatedUtc);
            Assert.AreEqual(msg.MachineName, result.MachineName);
            Assert.AreEqual(msg.Offset, result.Offset);
            Assert.IsTrue(json.Contains($"\"Kind\":\"{MarbleKind.OnCompleted}\""));
        }
    }
}
