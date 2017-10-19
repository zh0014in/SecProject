using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyzer
{
    public class Session
    {
        public Packet ClientHello { get; set; }
        public Packet ServerHello { get; set; }
        public Packet ClientKeyExchange { get; set; }
        public Packet Finished { get; set; }
        public override string ToString()
        {
            return ClientHello.ToString() + Environment.NewLine +
                ServerHello.ToString() + Environment.NewLine +
                ClientKeyExchange.ToString() + Environment.NewLine +
                Finished.ToString();
        }
    }
}
