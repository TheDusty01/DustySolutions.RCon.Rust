using System.Text.Json.Serialization;

namespace DustySolutions.RCon.Rust.Entities
{
    public class RconPlayerReport
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public ulong PlayerId { get; }
        public string PlayerName { get; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public ulong TargetId { get; }
        public string TargetName { get; }
        public string Subject { get; }
        public string Message { get; }
        public string Type { get; }

        public RconPlayerReport(ulong playerId, string playerName, ulong targetId, string targetName, string subject, string message, string type)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            TargetId = targetId;
            TargetName = targetName;
            Subject = subject;
            Message = message;
            Type = type;
        }
    }
}
