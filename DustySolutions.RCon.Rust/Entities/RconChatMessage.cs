using System.Text.Json.Serialization;

namespace DustySolutions.RCon.Rust.Entities
{
    public class RconChatMessage
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RconChatChannel Channel { get; }
        public string Message { get; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public ulong UserId { get; }
        public string Username { get; }
        public string Color { get; }
        public int Time { get; }
        [JsonIgnore]
        public DateTimeOffset SentAt { get; }

        public RconChatMessage(RconChatChannel channel, string message, ulong userId, string username, string color, int time)
        {
            Channel = channel;
            Message = message;
            UserId = userId;
            Username = username;
            Color = color;
            Time = time;

            SentAt = DateTimeOffset.FromUnixTimeSeconds(time);
        }
    }

    public enum RconChatChannel
    {
        Global,
        Team,
        Server,
        Cards,
        Local,
    }
}
