using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace Kafka.Message.Service.Producer.Helper
{
    /// <summary>
    /// Class for serializing Kafka value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class KafkaValueSerializer<T> : ISerializer<T>
    {
        /// <summary>
        /// Serialize data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public byte[] Serialize(T data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, typeof(T)));
        }
    }
}
