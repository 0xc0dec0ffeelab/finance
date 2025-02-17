namespace finance_dotnet.Backend.Kakfa
{
    public interface IKafkaProducer
    {
        Task ProduceByPartitionKeyAsync(string topic, string? partitionKey, string value);
    }
}
