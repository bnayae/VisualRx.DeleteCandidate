using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualRx.ETW.Common
{
    public class EtwConstants
    {
        /// <summary>
        /// The session name
        /// </summary>
        public const string SessionName = "VisualRx.Session.V1";
        /// <summary>
        /// The provider name
        /// </summary>
        public const string ProviderName = "VisualRx.V1";
        /// <summary>
        /// The provider unique identifier
        /// </summary>
        public const string ProviderGuidString = "2C62A9B8-6082-43F8-BEBD-99EFFF418C4B";
        /// <summary>
        /// The provider unique identifier
        /// </summary>
        public static readonly Guid ProviderGuid = new Guid(ProviderGuidString);
    }
}
