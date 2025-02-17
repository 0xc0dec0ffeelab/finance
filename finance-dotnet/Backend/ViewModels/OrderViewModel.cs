using System.Text.Json.Serialization;

namespace finance_dotnet.Backend.Models
{
    public class OrderViewModel
    {
        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }
    }
}
