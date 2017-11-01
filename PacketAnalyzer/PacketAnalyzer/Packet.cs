using System;

namespace PacketAnalyzer
{
    public class Packet
    {
        public double Time { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int SrcPort { get; set; }
        public int DestPort { get; set; }
        public int Length { get; set; }
        public int PreviousLength { get; set; } = 0;
        public string Info { get; set; }

        public double InterArrivalTime { get; set; } = -1;
        public HandshakeType Type { get; set; } = HandshakeType.Null;
        public override string ToString()
        {
            return $"{Time},{Source},{Destination},{SrcPort},{DestPort},{Length},{PreviousLength},{InterArrivalTime},{Type}";
        }

        public string ToMSC(){
            switch(Type){
                case HandshakeType.ClientHello:
                    return "A -> B : ClientHello" + Environment.NewLine;
                case HandshakeType.ServerHelloDone:
                    return "B -> A : ServerHello" + Environment.NewLine
                        + "B -> A : ServerCertificate" + Environment.NewLine
                        + "B -> A : ServerHelloDone" + Environment.NewLine;
                case HandshakeType.ClientKeyExchange:
                    return "A -> B : ClientKeyExchange" + Environment.NewLine
                        + "A -> B : ClientChangeCipherSpec" + Environment.NewLine
                        + "A -> B : ClientEncryptedhandshakeMessage" + Environment.NewLine;
                case HandshakeType.Finished:
                    return "B -> A : ServerChangeCipherSpec" + Environment.NewLine
                        + "B -> A : ServerEncryptedhandshakeMessage" + Environment.NewLine;
            }
            return "NULL";
        }
    }
}
