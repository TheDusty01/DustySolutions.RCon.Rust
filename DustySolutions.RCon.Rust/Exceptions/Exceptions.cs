namespace DustySolutions.RCon.Rust.Exceptions
{

    public class NotConnectedException : Exception
    {
        public NotConnectedException() : base("Client is not connected.")
        {
        }
    }

    public class SameIdentifierException : Exception
    {
        public SameIdentifierException() : base("The same identifier is already in use.")
        {
        }
    }
}
