using Kafka.Message.Service.Consumer;

namespace Kafka.Message.Consumer.Client
{
    /// <summary>
    /// Consumer handler
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class ConsumerHandler<TKey,TValue> : IConsumerHandler<TKey, TValue>
    {
        /// <summary>
        /// Method to handle the Kafka message.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task HandleAsync(string topic,TKey key, TValue value)
        {
            Console.WriteLine($"{key.ToString()} {value.ToString()}");
            
        }
    }
}
