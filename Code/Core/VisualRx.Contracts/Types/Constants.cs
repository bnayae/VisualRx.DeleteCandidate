using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualRx.Contracts
{
    public class Constants
    {
        public readonly static JsonSerializerSettings JsonDefaultSetting = 
            new JsonSerializerSettings();

        static Constants()
        {
            JsonDefaultSetting.Converters.Add(new StringEnumConverter());
            JsonDefaultSetting.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
        }

    }
}
