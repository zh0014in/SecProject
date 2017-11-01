using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyzer
{
    public class PacketParser
    {
        private static string ClientHello = "Client Hello";
        private static string ServerHelloDone = "Server Hello Done";
        private static string ClientKeyExchange = "Client Key Exchange";
        private static string ServerChangeCipherSpec = "Change Cipher Spec";
        private static double InterArrivalTimeout = 2;
        private const string ACTIONSPLITTER = ":";
        private const string PARTYSPLITTER = "->";

        public List<Packet> Parse(string fileName)
        {
            var result = DecodeRawPackets(fileName);
            DecodeInterArrivalTime(result);
            return result;
        }

        private List<Packet> DecodeRawPackets(string fileName)
        {
            var result = new List<Packet>();
            using (var reader = new StreamReader(fileName))
            {
                reader.ReadLine(); // skip header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.Replace("\",\"", ";");
                    var values = line.Split(';');
                    var packet = new Packet
                    {
                        Time = double.Parse(values[1].Trim('\"')),
                        Source = values[2].Trim('\"'),
                        Destination = values[3].Trim('\"'),
                        SrcPort = int.Parse(values[5].Trim('\"')),
                        DestPort = int.Parse(values[6].Trim('\"')),
                        Length = int.Parse(values[7].Trim('\"')),
                        Info = values[8].Trim('\"')
                    };

                    if (packet.Info.Contains(ClientHello))
                    {
                        packet.Type = HandshakeType.ClientHello;
                    }
                    else if (packet.Info.Contains(ServerHelloDone))
                    {
                        packet.Type = HandshakeType.ServerHelloDone;
                    }
                    else if (packet.Info.Contains(ClientKeyExchange))
                    {
                        packet.Type = HandshakeType.ClientKeyExchange;
                    }
                    else if (packet.Info.Contains(ServerChangeCipherSpec)
                       && !packet.Info.Contains(ClientKeyExchange))
                    {
                        packet.Type = HandshakeType.Finished;
                    }
                    result.Add(packet);
                }
            }
            return result;
        }
        private void DecodeInterArrivalTime(List<Packet> packets)
        {
            Packet packet, previousPacket;
            for (var i = 0; i < packets.Count; i++)
            {
                packet = packets[i];
                previousPacket = packets.Take(i).Where(x => x.Source == packet.Destination
                && x.Destination == packet.Source
                && (x.DestPort == packet.SrcPort || x.SrcPort == packet.DestPort)
                 && packet.Time - x.Time <= InterArrivalTimeout)
                .OrderByDescending(x => x.Time)
                .FirstOrDefault();
                if(previousPacket != null)
                {
                    packet.InterArrivalTime = packet.Time - previousPacket.Time;
                    packet.PreviousLength = previousPacket.Length;
                }
            }
        }

        public List<Session> DecodeSessions(string fileName)
        {
            var packets = DecodeSessionPackets(fileName);
            var result = new List<Session>();
            for(var i = 0; i < packets.Count; i++)
            {
                var packet = packets[i];
                if(packet.Type == HandshakeType.Finished)
                {
                    var clientKeyExchange = packets.Take(i).Where(x => x.Source == packet.Destination
                    && x.Destination == packet.Source
                    && x.Type == HandshakeType.ClientKeyExchange
                    && packet.Time - x.Time < InterArrivalTimeout)
                    .OrderByDescending(x => x.Time)
                    .FirstOrDefault();
                    if(clientKeyExchange != null)
                    {
                        var serverHelloDone = packets.Take(i).Where(x => x.Time < clientKeyExchange.Time
                        && x.Source == clientKeyExchange.Destination
                        && x.Destination == clientKeyExchange.Source
                        && x.Type == HandshakeType.ServerHelloDone
                        && clientKeyExchange.Time - x.Time < InterArrivalTimeout)
                        .OrderByDescending(x => x.Time)
                        .FirstOrDefault();
                        if(serverHelloDone != null)
                        {
                            var clientHello = packets.Take(i).Where(x => x.Time < serverHelloDone.Time
                            && x.Source == serverHelloDone.Destination
                            && x.Destination == serverHelloDone.Source
                            && x.Type == HandshakeType.ClientHello
                            && serverHelloDone.Time - x.Time < InterArrivalTimeout)
                            .OrderByDescending(x => x.Time)
                            .FirstOrDefault();
                            if(clientHello != null)
                            {
                                // a session is found
                                result.Add(new Session
                                {
                                    ClientHello = clientHello,
                                    ServerHello = serverHelloDone,
                                    ClientKeyExchange = clientKeyExchange,
                                    Finished = packet
                                });
                            }
                        }
                    }
                }
            }
            return result;
        }

        private List<Packet> DecodeSessionPackets(string fileName)
        {
            var result = new List<Packet>();
            using (var reader = new StreamReader(fileName))
            {
                reader.ReadLine(); // skip header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    var packet = new Packet
                    {
                        Time = double.Parse(values[0].Trim('\"')),
                        Source = values[1].Trim('\"'),
                        Destination = values[2].Trim('\"'),
                        SrcPort = int.Parse(values[3].Trim('\"')),
                        DestPort = int.Parse(values[4].Trim('\"')),
                        Length = int.Parse(values[5].Trim('\"')),
                        Info = values[9].Trim('\"')
                    };
                    packet.Type = (HandshakeType)Enum.Parse(typeof(HandshakeType), packet.Info);
                    result.Add(packet);
                }
            }
            return result;
        }

        public string SessionToMSC(Session session){
            var result = session.ClientHello.ToMSC()
                                + session.ServerHello.ToMSC()
                                + session.ClientKeyExchange.ToMSC()
                                + session.Finished.ToMSC();
            return result;
        }

        public string MSCToCSP(string input){
            StringReader strReader = new StringReader(input);
            var lines = new List<string>();
            while (true)
            {
                var aLine = strReader.ReadLine();
                if (aLine != null && !string.IsNullOrWhiteSpace(aLine) 
                    && aLine.Contains(ACTIONSPLITTER)
                    && aLine.Contains(PARTYSPLITTER))
                {
                    lines.Add(aLine);
                }
                else
                {
                    break;
                }
            }
            var parties = DecodeParties(lines);
            var actions = DecodeActions(lines);
            var result = "";
            result += GenerateEnums(parties, actions);
            result += "channel network 0;" + Environment.NewLine;
            result += GenerateProcesses(lines, parties);
            return result;
        }

        private List<string> DecodeParties(List<string> lines){
            var result = new List<string>();
            foreach(var line in lines){
                var parties = line.Split(new string[]{ACTIONSPLITTER}, StringSplitOptions.None)
                                  .First().Split(new string[]{PARTYSPLITTER}, StringSplitOptions.None);
                foreach(var party in parties){
                    if(result.Contains(party.Trim())){
                        continue;
                    }
                    result.Add(party.Trim());
                }
            }
            return result;
        }

        private List<string> DecodeActions(List<string> lines){
            var result = new List<string>();
            foreach(var line in lines){
                var action = line.Split(new string[] { ACTIONSPLITTER }, StringSplitOptions.None)
                                  .Skip(1)
                                  .First().Trim();
                result.Add(action);
            }
            return result;
        }

        private string GenerateEnums(List<string> parties, List<string> actions){
            var result = "enum{";
            foreach(var party in parties){
                result += party + ",";
            }
            foreach(var action in actions){
                result += action + ",";
            }
            result = result.Substring(0, result.Length - 1);
            result += "};" + Environment.NewLine;
            return result;
        }

        private string GenerateProcesses(List<string> lines, List<string> parties){
            Dictionary<string, string> processes = new Dictionary<string, string>();
            foreach(var party in parties){
                processes[party] = $"Process{party}() = {Environment.NewLine}";
            }
            foreach(var line in lines){
                var p = line.Split(new string[] { ACTIONSPLITTER }, StringSplitOptions.None)
                                  .First().Split(new string[] { PARTYSPLITTER }, StringSplitOptions.None);
                var action = line.Split(new string[] { ACTIONSPLITTER }, StringSplitOptions.None)
                                  .Skip(1)
                                  .First().Trim();
                var from = p[0].Trim();
                var to = p[1].Trim();
                foreach(var party in parties){
                    if (party != from){
                        processes[party] += $"network?{action} -> {Environment.NewLine}";
                    }else{
                        processes[party] += $"network!{action} -> {Environment.NewLine}";
                    }
                }
            }
            var result = "";
            foreach(var process in processes){
                result += process.Value + "Skip;" + Environment.NewLine;
            }
            result += "TLS = ";
            foreach(var party in parties){
                result += party + "() ||| ";
            }
            result = result.Substring(0, result.Length - 5) + ";" + Environment.NewLine;
            result += "#assert TLS deadlockfree";
            return result;
        }
    }
}