#region Using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VisualRx.Contracts;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;
using System.Linq.Expressions;

#endregion Using


namespace VisualRx.Contracts
{
    #region Type's Overloads

    #region JsonKnownType<TBase, TDerive1-9>

    public class JsonKnownType<TBase, TDerive1, TDerive2, TDerive3, TDerive4, TDerive5, TDerive6, TDerive7, TDerive8, TDerive9> :
        JsonKnownType<TBase>
        where TDerive1 : new()
        where TDerive2 : new()
        where TDerive3 : new()
        where TDerive4 : new()
        where TDerive5 : new()
        where TDerive6 : new()
        where TDerive7 : new()
        where TDerive8 : new()
        where TDerive9 : new()
    {
        public JsonKnownType()
            : base(
            () => typeof(TDerive1),
            () => typeof(TDerive2),
            () => typeof(TDerive3),
            () => typeof(TDerive4),
            () => typeof(TDerive5),
            () => typeof(TDerive6),
            () => typeof(TDerive7),
            () => typeof(TDerive8),
            () => typeof(TDerive9))
        {
        }
    }

    #endregion // JsonKnownType<TBase, TDerive1-9>

    #region JsonKnownType<TBase, TDerive1-8>

    public class JsonKnownType<TBase, TDerive1, TDerive2, TDerive3, TDerive4, TDerive5, TDerive6, TDerive7, TDerive8> :
    JsonKnownType<TBase>
        where TDerive1 : new()
        where TDerive2 : new()
        where TDerive3 : new()
        where TDerive4 : new()
        where TDerive5 : new()
        where TDerive6 : new()
        where TDerive7 : new()
        where TDerive8 : new()
    {
        public JsonKnownType()
            : base(
            () => typeof(TDerive1),
            () => typeof(TDerive2),
            () => typeof(TDerive3),
            () => typeof(TDerive4),
            () => typeof(TDerive5),
            () => typeof(TDerive6),
            () => typeof(TDerive7),
            () => typeof(TDerive8))
        {
        }
    }

    #endregion // JsonKnownType<TBase, TDerive1-8>

    #region JsonKnownType<TBase, TDerive1-7>

    public class JsonKnownType<TBase, TDerive1, TDerive2, TDerive3, TDerive4, TDerive5, TDerive6, TDerive7> :
    JsonKnownType<TBase>
        where TDerive1 : new()
        where TDerive2 : new()
        where TDerive3 : new()
        where TDerive4 : new()
        where TDerive5 : new()
        where TDerive6 : new()
        where TDerive7 : new()
    {
        public JsonKnownType()
            : base(
            () => typeof(TDerive1),
            () => typeof(TDerive2),
            () => typeof(TDerive3),
            () => typeof(TDerive4),
            () => typeof(TDerive5),
            () => typeof(TDerive6),
            () => typeof(TDerive7))
        {
        }
    }

    #endregion // JsonKnownType<TBase, TDerive1-7>

    #region JsonKnownType<TBase, TDerive1-6>

    public class JsonKnownType<TBase, TDerive1, TDerive2, TDerive3, TDerive4, TDerive5, TDerive6> :
    JsonKnownType<TBase>
        where TDerive1 : new()
        where TDerive2 : new()
        where TDerive3 : new()
        where TDerive4 : new()
        where TDerive5 : new()
        where TDerive6 : new()
    {
        public JsonKnownType()
            : base(
            () => typeof(TDerive1),
            () => typeof(TDerive2),
            () => typeof(TDerive3),
            () => typeof(TDerive4),
            () => typeof(TDerive5),
            () => typeof(TDerive6))
        {
        }
    }

    #endregion // JsonKnownType<TBase, TDerive1-6>

    #region JsonKnownType<TBase, TDerive1-5>

    public class JsonKnownType<TBase, TDerive1, TDerive2, TDerive3, TDerive4, TDerive5> :
    JsonKnownType<TBase>
        where TDerive1 : new()
        where TDerive2 : new()
        where TDerive3 : new()
        where TDerive4 : new()
        where TDerive5 : new()
    {
        public JsonKnownType()
            : base(
            () => typeof(TDerive1),
            () => typeof(TDerive2),
            () => typeof(TDerive3),
            () => typeof(TDerive4),
            () => typeof(TDerive5))
        {
        }
    }

    #endregion // JsonKnownType<TBase, TDerive1-5>

    #region JsonKnownType<TBase, TDerive1-4>

    public class JsonKnownType<TBase, TDerive1, TDerive2, TDerive3, TDerive4> :
    JsonKnownType<TBase>
        where TDerive1 : new()
        where TDerive2 : new()
        where TDerive3 : new()
        where TDerive4 : new()
    {
        public JsonKnownType()
            : base(
            () => typeof(TDerive1),
            () => typeof(TDerive2),
            () => typeof(TDerive3),
            () => typeof(TDerive4))
        {
        }
    }

    #endregion // JsonKnownType<TBase, TDerive1-4>

    #region JsonKnownType<TBase, TDerive1-3>

    public class JsonKnownType<TBase, TDerive1, TDerive2, TDerive3> :
    JsonKnownType<TBase>
        where TDerive1 : new()
        where TDerive2 : new()
        where TDerive3 : new()
    {
        public JsonKnownType()
            : base(
            () => typeof(TDerive1),
            () => typeof(TDerive2),
            () => typeof(TDerive3))
        {
        }
    }

    #endregion // JsonKnownType<TBase, TDerive1-3>

    #region JsonKnownType<TBase, TDerive1-2>

    public class JsonKnownType<TBase, TDerive1, TDerive2> :
    JsonKnownType<TBase>
        where TDerive1 : new()
        where TDerive2 : new()
    {
        public JsonKnownType()
            : base(
            () => typeof(TDerive1),
            () => typeof(TDerive2))
        {
        }
    }

    #endregion // JsonKnownType<TBase, TDerive1-2>


    #endregion // Type's Overloads

    /// <summary>
    /// Handle serialize and deserialize type of array's items
    /// </summary>
    public class JsonKnownType<TBase> : JsonConverter
    {
        // strategy for converting property name into the deserialize typeName.
        private Func<string, Type> _deserialzeTypeFromPropertyName;
        // strategy for converting the derive typeName into property name.
        private Func<Type, string> _toPropertyName;

        #region Ctor

        #region Overloads

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonKnownType{T, TDerive}"/> class.
        /// </summary>
        /// <param name="deserialzeTypeStrategies"></param>
        public JsonKnownType(
            params Expression<Func<Type>>[] deserialzeTypeStrategies)
            : this(ToDictionary(deserialzeTypeStrategies))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonKnownType{T, TDerive}"/> class.
        /// </summary>
        /// <param name="canConvert"></param>
        /// <param name="deserialzeTypeFromPropertyName">
        /// strategy for converting property name into the deserialize typeName.
        /// </param>
        /// <param name="deserialzeTypeMap">Mapping of deserialization type handling</param>
        public JsonKnownType(
            IDictionary<string, Func<Type>> deserialzeTypeMap)
            : this(name => deserialzeTypeMap[name].Invoke(),
                    t => t.Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonKnownType{T, TDerive}"/> class.
        /// </summary>
        /// <param name="deserialzeTypeFromPropertyName">
        /// strategy for converting property name into the deserialize typeName.
        /// </param>
        public JsonKnownType(
            Func<string, Type> deserialzeTypeFromPropertyName)
            : this(deserialzeTypeFromPropertyName, t => t.Name)
        {
        }

        #endregion // Overloads

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonKnownType{T, TDerive}"/> class.
        /// </summary>
        /// <param name="deserialzeTypeFromPropertyName">
        /// strategy for converting property name into the deserialize typeName.
        /// </param>
        /// <param name="toPropertyName">
        /// strategy for converting the derive typeName into property name.
        /// </param>
        public JsonKnownType(
            Func<string, Type> deserialzeTypeFromPropertyName,
            Func<Type, string> toPropertyName)
        {
            _deserialzeTypeFromPropertyName = deserialzeTypeFromPropertyName;
            _toPropertyName = toPropertyName;
        }

        #endregion // Ctor

        #region ToDictionary

        /// <summary>
        /// To the dictionary.
        /// </summary>
        /// <param name="deserialzeTypeStrategies">The deserialize type strategies.</param>
        /// <returns></returns>
        private static IDictionary<string, Func<Type>> ToDictionary(
            Expression<Func<Type>>[] deserialzeTypeStrategies)
        {
            var paires = from e in deserialzeTypeStrategies
                         let c = (ConstantExpression)e.Body
                         let t = (Type)c.Value
                         select new { t.Name, Factory = e.Compile() };
            IDictionary<string, Func<Type>> result =
                paires.ToDictionary(m => m.Name, m => m.Factory);

            return result;
        }

        #endregion // ToDictionary

        #region CanConvert

        /// <summary>
        /// Determines whether this instance can convert the specified object typeName.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified object typeName; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            var r = objectType == typeof(TBase[]) ||
                objectType == typeof(IEnumerable<TBase>) ||
                objectType == typeof(IList<TBase>) ||
                objectType == typeof(List<TBase>);
            return r;
        }

        #endregion // CanConvert

        #region ReadJson

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(
         JsonReader reader,
         Type objectType,
         object existingValue,
         JsonSerializer serializer)
        {
            List<TBase> items = new List<TBase>();
            reader.Read(); // to start object
            reader.Read(); // to property (of typeName)
            while (reader.TokenType == JsonToken.PropertyName)
            {
                string typeName = (string)reader.Value;
                reader.Read(); // step into (the item information)

                Type t = _deserialzeTypeFromPropertyName(typeName); // get the deserialized type
                TBase item = (TBase)serializer.Deserialize(reader, t);

                reader.Read(); // step over (End of object)
                reader.Read(); // next step (End of property)
                if (reader.TokenType != JsonToken.EndArray)
                    reader.Read(); // step into (property start of array end)
                items.Add(item);
            }

            if (objectType == typeof(TBase[]))
                return items.ToArray();
            return items;
        }

        #endregion // ReadJson

        #region WriteJson

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            var arr = value as IEnumerable<TBase>;
            writer.WriteStartArray();

            foreach (var item in arr)
            {
                writer.WriteStartObject();
                string propName = _toPropertyName(item.GetType());
                writer.WritePropertyName(propName); // wrap the item with property that represent it's name
                serializer.Serialize(writer, item); // serialize the content of the item
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        #endregion // WriteJson
    }
}