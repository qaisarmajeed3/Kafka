using Confluent.Kafka;
using System.Text.Json;

namespace Kafka.Message.Service.Consumer.Helper
{
    /// <summary>
    /// Class for deserializing Kafka value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class KafkaValueDeserializer<T> : IDeserializer<T>
    {
        /// <summary>
        /// Deserialize data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isNull"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

    }
}
