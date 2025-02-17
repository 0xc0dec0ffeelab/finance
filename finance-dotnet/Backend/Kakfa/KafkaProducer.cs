using Confluent.Kafka;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace finance_dotnet.Backend.Kakfa
{
    public class KafkaProducer : IKafkaProducer
    {

        private readonly IProducer<string?, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(ILogger<KafkaProducer> logger, IConfiguration configuration)
        {
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers")!,
                TransactionalId = "per-producer-1", // per producer id (generated from dotnet-app)
                EnableIdempotence = true, // Idempotence
                Acks = Acks.All,
                Debug = "all"
                //MaxInFlight = 5,
                //LingerMs = 5
            };
            _producer = new ProducerBuilder<string?, string>(config).Build();
            _producer.InitTransactions(TimeSpan.FromSeconds(10));
        }

        public async Task ProduceByPartitionKeyAsync(string topic, string? partitionKey, string value)
        {
            try
            {
                _producer.BeginTransaction();
                await _producer.ProduceAsync(topic, new Message<string?, string> { Key = partitionKey, Value = value });
                _producer.CommitTransaction();
                //_logger.LogInformation($"Message sent: {partitionKey} -> {value}");
            }
            catch (ProduceException<string, string> e)
            {
                //_logger.LogError($"Kafka error: {e.Error.Reason}");
                _producer.AbortTransaction();
            }
        }
    }
}
