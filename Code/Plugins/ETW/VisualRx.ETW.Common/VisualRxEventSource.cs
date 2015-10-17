using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualRx.Contracts;
using VisualRx.ETW.Common;

namespace VisualRx.ETW.Common
{
    [EventSource(Name = EtwConstants.ProviderName,
        Guid = EtwConstants.ProviderGuidString)]
    public class VisualRxEventSource : EventSource
    {
        //[Event(1, Level = EventLevel.Informational)]
        public void Send(
            Marble marble, 
            JsonSerializerSettings setting = null)
        {
            setting = setting ?? Constants.JsonDefaultSetting;

            if (IsEnabled())
            {
                string json = JsonConvert.SerializeObject(marble, setting);
                byte[] payload = Encoding.UTF8.GetBytes(json);
                WriteEvent(1, payload);
                WriteEvent(1, json);
            }
        }
    }
}
