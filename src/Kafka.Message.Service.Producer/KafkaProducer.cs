using Confluent.Kafka;
using Kafka.Message.Service.Producer.Helper;

namespace Kafka.Message.Service.Producer
{
    /// <summary>
    /// Class for hndling Kafka message produce.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KafkaProducer<TKey, TValue> : IDisposable, IMessageProducer<TKey, TValue> where TValue : class
    {
        private readonly IProducer<TKey, TValue> _producer;

        /// <summary>  
        /// Initializes the producer  
        /// </summary>  
        /// <param name="config"></param>  
        public KafkaProducer(ProducerConfig config)
        {
            _producer = new ProducerBuilder<TKey, TValue>(config).SetValueSerializer(new KafkaValueSerializer<TValue>()).Build();
        }

        /// <summary>  
        /// Triggered when the service creates Kafka topic.  
        /// </summary>  
        /// <param name="topic">Indicates topic name</param>  
        /// <param name="key">Indicates message's key in Kafka topic</param>  
        /// <param name="value">Indicates message's value in Kafka topic</param>  
        /// <returns></returns>  
        public async Task ProduceAsync(string topic, TKey key, TValue value)
        {
            await _producer.ProduceAsync(topic, new Message<TKey, TValue> { Key = key, Value = value });
        }

        /// <summary>  
        /// Releases all resources used by the current instance of the producer.  
        /// </summary>  
        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}
