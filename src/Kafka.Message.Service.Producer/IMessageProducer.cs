namespace Kafka.Message.Service.Producer
{
    /// <summary>
    /// Interface for producing message.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMessageProducer<TKey,TValue>
    {
        /// <summary>
        /// Produce message to Kafka.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task ProduceAsync(string topic, TKey key, TValue value);
    }
}
