#region Using

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Threading;

#endregion Using

namespace VisualRx.Contracts
{
    /// <summary>
    /// base class for the marble item
    /// </summary>
    //[KnownType(typeof(MarbleError))]
    //[KnownType(typeof(MarbleComplete))]
    //[KnownType(typeof(MarbleNext))]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [DebuggerDisplay("{StreamKey}: {Kind}, {Value}, {Offset}")]
    public class Marble
    {
        private static readonly Func<Exception, string> DEFAULT_ERROR_FORMATTER = ex => ex.ToString();

        #region Constructors

        [Obsolete("Don't use, this constructor is for serialization only", true)]
        internal Marble()
        {
                
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Marble" /> class.
        /// </summary>
        /// <param name="streamKey"></param>
        /// <param name="kind">The kind.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        internal Marble(
            string streamKey,
            NotificationKind kind,
            TimeSpan elapsed,
            string machineName)
        {
            StreamKey = streamKey;
            Kind = kind;

            Offset = elapsed;

            DateCreatedUtc = DateTime.UtcNow;

            MachineName = machineName;
        }

        #endregion Constructors

        #region Properties

        #region Value

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty]
        public string Value { get; private set; }

        #endregion // Value

        #region DateCreatedUtc

        /// <summary>
        /// Gets the date created.
        /// </summary>
        [JsonProperty]
        public DateTime DateCreatedUtc { get; private set; }

        #endregion DateCreatedUtc

        #region StreamKey

        /// <summary>
        /// stream friendly key 
        /// </summary>
        [JsonProperty]
        public string StreamKey { get; private set; }

        #endregion StreamKey

        #region Kind

        /// <summary>
        /// type of the marble
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationKind Kind { get; private set; }

        #endregion Kind

        #region Offset

        /// <summary>
        /// Occurrence offset
        /// </summary>
        [JsonProperty]
        public TimeSpan Offset { get; private set; }

        #endregion Offset

        #region IndexOrder

        /// <summary>
        /// the order index
        /// </summary>
        [JsonProperty]
        public double IndexOrder { get; set; }

        #endregion IndexOrder

        #region MachineName

        /// <summary>
        /// diagram name (sue as a key)
        /// </summary>
        [JsonProperty]
        public string MachineName { get; private set; }

        #endregion MachineName

        #endregion Properties

        #region Methods

        // TODO: Bnaya, 2015-10: #3, Enable custom serialization
        //                           consider using extension methods
        #region GetValue

        /// <summary>
        /// Get the internal value cast to generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>(JsonSerializerSettings setting = null)
        {
            setting = setting ?? Constants.JsonDefaultSetting;
            T instance = JsonConvert.DeserializeObject<T>(Value, setting);
            return instance;
        }

        #endregion // GetValue

        // TODO: Bnaya, 2015-10: #3, Enable custom serialization
        //                           consider using extension methods
        #region CreateNext

        /// <summary>
        /// Creates the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="streamKey">The stream key.</param>
        /// <param name="item">The item.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <param name="setting">The setting.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Marble CreateNext<T>(
                        string streamKey,
                        T item,
                        TimeSpan elapsed,
                        string machineName,
                        JsonSerializerSettings setting = null)
        {
            setting = setting ?? Constants.JsonDefaultSetting;
            var msg = new Marble(streamKey, NotificationKind.OnNext, elapsed, machineName);
            msg.Value = JsonConvert.SerializeObject(item, setting);
            return msg;
        }

        #endregion // CreateNext

        #region CreateError

        /// <summary>
        /// Creates the error.
        /// </summary>
        /// <param name="streamKey">The stream key.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Marble CreateError(
                        string streamKey,
                        Exception ex,
                        TimeSpan elapsed,
                        string machineName,
                        Func<Exception, string> formatter = null)
        {
            formatter = formatter ?? DEFAULT_ERROR_FORMATTER;
            var msg = new Marble(streamKey, NotificationKind.OnError, elapsed, machineName);
            msg.Value = formatter(ex);
            return msg;
        }
        #endregion // CreateError

        #region CreateCompleted

        /// <summary>
        /// Creates the completed.
        /// </summary>
        /// <param name="streamKey"></param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Marble CreateCompleted(
            string streamKey,
            TimeSpan elapsed, string machineName)
        {
            var msg = new Marble(streamKey, NotificationKind.OnCompleted, elapsed, machineName);
            return msg;
        } 

        #endregion // CreateCompleted

        #endregion // Methods
    }
}