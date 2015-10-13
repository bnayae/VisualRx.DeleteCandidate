using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;
using VisualRx.Publishers.Common;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using VisualRx.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Newtonsoft.Json;

namespace VisualRx.UnitTests
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Person
    {
        public Person(int id)
        {
            Id = id;
            Name = $"John {id}";
        }

        [JsonProperty]
        public int Id { get; private set; }

        [JsonProperty]
        public string Name { get; private set; }
    }
}
