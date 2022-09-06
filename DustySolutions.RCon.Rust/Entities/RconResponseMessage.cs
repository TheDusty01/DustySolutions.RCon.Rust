using System.Text.Json.Serialization;

namespace DustySolutions.RCon.Rust.Entities
{
    public class RconResponseMessage
    {
        public int Identifier { get; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RconMessageType Type { get; }
        public string Message { get; } = "";
        public string Stacktrace { get; } = "";

        public RconResponseMessage(int identifier, RconMessageType type, string message, string stacktrace)
        {
            Identifier = identifier;
            Type = type;
            Message = message;
            Stacktrace = stacktrace;
        }
    }
}
