#region Using

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
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
    public abstract class MarbleBase
    {
        #region Private / Protected Fields

        //private static NetDataContractSerializer _ser = new NetDataContractSerializer();
        //private static BinaryFormatter _formater = new BinaryFormatter();

        #endregion Private / Protected Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MarbleBase" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="kind">The kind.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        internal MarbleBase(
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
        public JsonToken Value { get; private set; }

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

        public T GetValue<T>()
        {
            throw new NotImplementedException();
        }

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
        public static MarbleBase CreateNext<T>(
                        string name,
                        T item,
                        TimeSpan elapsed,
                        string machineName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the error.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static MarbleBase CreateError(string name,
                        Exception ex,
                        TimeSpan elapsed,
                        string machineName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the completed.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="elapsed">The elapsed.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static MarbleBase CreateCompleted(string name,
            TimeSpan elapsed, string machineName)
        {
            throw new NotImplementedException();
        }
        #endregion // Methods
    }
}