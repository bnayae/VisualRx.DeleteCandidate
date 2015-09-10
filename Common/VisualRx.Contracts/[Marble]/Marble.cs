#region Using

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
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
    [DebuggerDisplay("{Name}: {Kind}, {Value}, {Offset}")]
    public class Marble
    {
        #region Private / Protected Fields

        //private static NetDataContractSerializer _ser = new NetDataContractSerializer();
        //private static BinaryFormatter _formater = new BinaryFormatter();

        #endregion Private / Protected Fields

        #region Constructors

        [Obsolete("Don't use, this constructor is for serialization only", true)]
        internal Marble()
        {
                
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Marble" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="kind">The kind.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        internal Marble(
            string name,
            MarbleKind kind,
            TimeSpan elapsed,
            string machineName)
        {
            Name = name;
            Kind = kind;

            Offset = elapsed;

            DateCreatedUtc = DateTime.UtcNow;
            Keywords = new string[0];

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
        public JToken Value { get; private set; }

        #endregion // Value

        #region DateCreatedUtc

        /// <summary>
        /// Gets the date created.
        /// </summary>
        [JsonProperty]
        public DateTime DateCreatedUtc { get; private set; }

        #endregion DateCreatedUtc

        #region Name

        /// <summary>
        /// diagram name (sue as a key)
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }

        #endregion Name

        #region Kind

        /// <summary>
        /// type of the marble
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarbleKind Kind { get; private set; }

        #endregion Kind

        #region Offset

        /// <summary>
        /// Occurrence offset
        /// </summary>
        [JsonProperty]
        public TimeSpan Offset { get; private set; }

        #endregion Offset

        #region Keywords

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>
        /// The keywords.
        /// </value>
        [JsonProperty]
        public string[] Keywords { get; set; }

        #endregion Keywords

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

        #region GetValue

        /// <summary>
        /// Get the internal value cast to generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>() => Value.ToObject<T>();

        #endregion // GetValue

        #region CreateNext

        /// <summary>
        /// Creates the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="item">The item.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Marble CreateNext<T>(
                        string name,
                        T item,
                        TimeSpan elapsed,
                        string machineName)
        {
            var msg = new Marble(name, MarbleKind.OnNext, elapsed, machineName);
            msg.Value = JToken.FromObject(item);
            return msg;
        }

        #endregion // CreateNext

        #region CreateError

        /// <summary>
        /// Creates the error.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Marble CreateError(string name,
                        Exception ex,
                        TimeSpan elapsed,
                        string machineName)
        {
            var msg = new Marble(name, MarbleKind.OnError, elapsed, machineName);
            msg.Value = JToken.FromObject(ex);
            return msg;
        }
        #endregion // CreateError

        #region CreateCompleted

        /// <summary>
        /// Creates the completed.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Marble CreateCompleted(string name,
            TimeSpan elapsed, string machineName)
        {
            var msg = new Marble(name, MarbleKind.OnCompleted, elapsed, machineName);
            return msg;
        } 

        #endregion // CreateCompleted

        #endregion // Methods
    }
}