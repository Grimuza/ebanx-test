using System.Text.Json.Serialization;

namespace EbanxApi.Models
{
    public class Event
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventType Type { get; set; } 
        public int? Destination { get; set; }
        public int? Origin { get; set; }
        public decimal Amount { get; set; }
    }
}