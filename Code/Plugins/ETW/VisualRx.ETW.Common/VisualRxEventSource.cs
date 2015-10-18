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
    [EventSource(Name = ProviderName,
        Guid = ProviderGuidString)]
    public class VisualRxEventSource : EventSource
    {
        /// <summary>
        /// The provider name
        /// </summary>
        public const string ProviderName = "VisualRx.V1";
        /// <summary>
        /// The provider unique identifier
        /// </summary>
        private const string ProviderGuidString = "2C62A9B8-6082-43F8-BEBD-99EFFF418C4B";
        /// <summary>
        /// The provider unique identifier
        /// </summary>
        public static readonly Guid ProviderGuid = new Guid(ProviderGuidString);

        private static VisualRxEventSource _log = new VisualRxEventSource();

        #region Send

        [Event(1, Message = "{0}", Level = EventLevel.Informational)]
        public void Send(
            string json)
        {
            if (IsEnabled())
            {
                WriteEvent(1, json);
            }
        }

        public static void Send(
            Marble marble,
            JsonSerializerSettings setting = null)
        {
            setting = setting ?? Constants.JsonDefaultSetting;

            string json = JsonConvert.SerializeObject(marble, setting);
            _log.Send(json);
        }

        #endregion // Send
    }
}
