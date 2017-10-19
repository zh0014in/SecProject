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
    }
}
