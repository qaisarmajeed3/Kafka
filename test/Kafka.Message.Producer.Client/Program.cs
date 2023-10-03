// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Kafka.Message.Service.Producer;
using Kafka.Message.Service.Producer.Model;
using System.Net;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
            services.AddScoped<IMessageProducer<string,MessageModel>,KafkaProducer<string,MessageModel>>(x =>
        ActivatorUtilities.CreateInstance<KafkaProducer<string, MessageModel>>(x, GetProducerConfig())))
    .Build();

var message = new MessageModel();
message.Key = Guid.NewGuid().ToString();
message.Version = new Version(1, 0).ToString();
message.Data = @"{""patient"":{""name"",""Smith"",""dob"":""02-10-1978""}}";

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

var producer = provider.GetRequiredService<IMessageProducer<string, MessageModel>>();
await producer.ProduceAsync("event-patient", message.Key, message);
ProducerConfig GetProducerConfig()
{
    return new ProducerConfig
    {
        BootstrapServers = "pkc-mz3gw.westus3.azure.confluent.cloud:9092",
        SecurityProtocol = SecurityProtocol.SaslSsl,
        SaslMechanism = SaslMechanism.Plain,
        SaslUsername = "TOMB3OYUQGAWBDSV",
        SaslPassword = "QWNwg0pWzusrfADU8cegOpKAFv2nP5EFvQGYozcTkoHygXlWP9DcUL6A0e/8emdl",
        ClientId = Dns.GetHostName()
    };
}
Console.WriteLine("Hello, World!");
await host.RunAsync();

