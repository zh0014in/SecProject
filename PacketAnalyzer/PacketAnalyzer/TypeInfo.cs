using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyzer
{
    public class TypeInfo
    {
        public HandshakeType Type { get; set; } = HandshakeType.Null;
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
    }
}
