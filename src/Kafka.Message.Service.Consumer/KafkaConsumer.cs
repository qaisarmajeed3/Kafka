using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Kafka.Message.Service.Consumer.Helper;

namespace Kafka.Message.Service.Consumer
{
    /// <summary>  
    /// Base class for implementing Kafka Consumer.  
    /// </summary>  
    /// <typeparam name="TKey"></typeparam>  
    /// <typeparam name="TValue"></typeparam>  
    public class KafkaConsumer<TKey, TValue> : IMessageConsumer<TKey, TValue> where TValue : class
    {
        private readonly ConsumerConfig _config;
        private readonly IConsumerHandler<TKey, TValue> _handler;
        private IConsumer<TKey, TValue> _consumer;
        private IEnumerable<string> _topics;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>  
        /// Indicates constructor to initialize the serviceScopeFactory and ConsumerConfig  
        /// </summary>  
        /// <param name="config">Indicates the consumer configuration</param>  
        /// <param name="serviceScopeFactory">Indicates the instance for serviceScopeFactory</param>  
        public KafkaConsumer(ConsumerConfig config, IConsumerHandler<TKey, TValue> consumerHandler, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _config = config;
            _handler = consumerHandler;
        }

        /// <summary>  
        /// Triggered when the service is ready to consume the Kafka topics.  
        /// </summary>  
        /// <param name="topics">Indicates list of Kafka Topics</param>  
        /// <param name="stoppingToken">Indicates stopping token</param>  
        /// <returns></returns>  
        public async Task ConsumeAsync(IEnumerable<string> topics, CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            _consumer = new ConsumerBuilder<TKey, TValue>(_config).SetValueDeserializer(new KafkaValueDeserializer<TValue>()).Build();
            _topics = topics;

            await Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        /// <summary>  
        /// This will close the consumer, commit offsets and leave the group cleanly.  
        /// </summary>  
        public void Close()
        {
            _consumer.Close();
        }

        /// <summary>  
        /// Releases all resources used by the current instance of the consumer  
        /// </summary>  
        public void Dispose()
        {
            _consumer.Dispose();
        }

        /// <summary>
        /// Starts consumer loop.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topics);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(cancellationToken);

                    if (result != null)
                    {
                        await _handler.HandleAsync(result.Topic,result.Message.Key, result.Message.Value);

                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.  
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }

    }
}
