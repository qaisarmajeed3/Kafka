// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Kafka.Message.Consumer.Client;
using Kafka.Message.Service.Consumer;
using Kafka.Message.Service.Consumer.Model;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
            services.AddScoped<IConsumerHandler<string, string>, ConsumerHandler<string, string>>()
            .AddScoped<IMessageConsumer<string, MessageModel>, KafkaConsumer<string, MessageModel>>(x =>
        ActivatorUtilities.CreateInstance<KafkaConsumer<string,MessageModel>>(x, GetConsumerConfig()))
            )
    .Build();


using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

ConsumerConfig GetConsumerConfig()
{
    return new ConsumerConfig
    {
        BootstrapServers = $"test.westus3.azure.confluent.cloud:9092",
        SecurityProtocol = SecurityProtocol.SaslSsl,
        SaslMechanism = SaslMechanism.Plain,
        SaslUsername = "test",
        SaslPassword = "test",
        GroupId = "test",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
}
var consumer = provider.GetRequiredService<IMessageConsumer<string, MessageModel>>();
List<string> topics = new List<string> { "event-task" };
await consumer.ConsumeAsync(topics, CancellationToken.None);

Console.WriteLine("Subscriber!");
await host.RunAsync();
