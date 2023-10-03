namespace Kafka.Message.Service.Consumer
{
    /// <summary>
    /// Interface for handling message.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMessageConsumer<TKey, TValue>
    {
        /// <summary>
        /// Method for consuming message
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        Task ConsumeAsync(IEnumerable<string> topics, CancellationToken stoppingToken);
    }
}