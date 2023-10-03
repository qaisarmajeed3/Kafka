namespace Kafka.Message.Service.Consumer
{
    /// <summary>
    /// Class for hadling message.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IConsumerHandler<TKey, TValue>
    {
        /// <summary>
        /// Handle kafka message.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task HandleAsync(string topic, TKey key, TValue value);
    }
}
