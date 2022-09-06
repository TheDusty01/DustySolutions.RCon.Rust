using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DustySolutions.RCon.Rust.Entities
{
    class RconRequestMessage
    {
        public int Identifier { get; }
        public string Message { get; }
        public string Name { get; }

        internal RconRequestMessage(int identifier, string message, string name)
        {
            Identifier = identifier;
            Message = message;
            Name = name;
        }
    }
}
