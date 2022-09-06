using System.Text.Json.Serialization;

namespace DustySolutions.RCon.Rust.Entities
{
    public class RconFeedbackReport
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public ulong PlayerId { get; }
        public string PlayerName { get; }
        public string Subject { get; }
        public string Message { get; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RconFeedbackReportType Type { get; }

        public RconFeedbackReport(ulong playerId, string playerName, string subject, string message, RconFeedbackReportType type)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            Subject = subject;
            Message = message;
            Type = type;
        }
    }

    public enum RconFeedbackReportType
    {
        FIRST = 0,
        General = 0,
        Bug = 1,
        Cheat = 2,
        Abuse = 3,
        Idea = 4,
        LAST = 5,
        OffensiveContent = 5,
    }
}
