using System.Text.Json.Serialization;
namespace RaceDay.Models.Events
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }   // JSON date text converts to DateTime automatically
        [JsonPropertyName("event Address")]
        public string Address { get; set; } = string.Empty;
        public string? Description { get; set; }   // "?" = may be null (your seed data has NULL descriptions)

        public string? BannerUrl { get; set; }   // "?" = may be null (your seed data has NULL banner URLs)

    }
}
