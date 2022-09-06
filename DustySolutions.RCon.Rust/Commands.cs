namespace DustySolutions.RCon.Rust
{
    public interface ICommands
    {
        IRustRconClient Client { get; }
    }

    public class Commands : ICommands
    {
        public IRustRconClient Client { get; }

        public Commands(IRustRconClient client)
        {
            Client = client;
        }
    }
}
