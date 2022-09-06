using DustySolutions.RCon.Rust.Entities;

namespace DustySolutions.RCon.Rust
{
    public static class CommandsExtensions
    {
        public static void Echo(this ICommands commands, string message)
        {
            commands.Client.SendCommand($"echo {message}");
        }

        public static Task<RconResponseMessage> EchoWithResponseAsync(this ICommands commands, string message)
        {
            return commands.Client.SendCommandWithResponseAsync($"echo {message}");
        }

        public static void Say(this ICommands commands, string message)
        {
            commands.Client.SendCommand($"say {message}");
        }

        public static Task<RconResponseMessage> SayWithResponseAsync(this ICommands commands, string message)
        {
            return commands.Client.SendCommandWithResponseAsync($"say {message}");
        }
    }
}
