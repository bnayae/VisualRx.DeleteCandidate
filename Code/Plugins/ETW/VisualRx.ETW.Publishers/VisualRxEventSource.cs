using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.ETW.Publishers
{
    [EventSource(Name = "VisualRx",
        Guid = "2C62A9B8-6082-43F8-BEBD-99EFFF418C4B")]
    public class VisualRxEventSource : EventSource
    {
        public void Send(Marble marble, JsonSerializerSettings setting)
        {
            if (base.IsEnabled())
            {
                string json = JsonConvert.SerializeObject(marble, setting);
                WriteEvent(1, json);
            }
        }
    }
}
