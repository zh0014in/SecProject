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
        private static double InterArrivalTimeout = 6;
        public List<Packet> Parse(string fileName)
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
                    if (packet.Type != HandshakeType.Null)
                    {
                        result.Add(packet);
                    }
                }
            }
            var count = result.Count(x => x.Type == HandshakeType.ServerHelloDone);
            DecodeInterArrivalTime(result);
            return result;
        }

        private void DecodeInterArrivalTime(List<Packet> packets)
        {
            List<Packet> tempClientHelloPackets = new List<Packet>(); // temperory clieng hello packets for server hello done search
            List<Packet> tempServerHelloDonePackets = new List<Packet>(); // temperory server hello done packets for client key exchange search
            List<Packet> tempClientKeyExchangePackets = new List<Packet>(); // temperory client key exchange packets for finish search
            Packet packet;
            for (var i = 0; i < packets.Count; i++)
            {
                packet = packets[i];
                if(packet.Type == HandshakeType.ClientHello)
                {
                    packet.InterArrivalTime = 0;
                    tempClientHelloPackets.Add(packet);
                }
                else if (packet.Type == HandshakeType.ServerHelloDone)
                {
                    // look for client hello
                    var clientHello = tempClientHelloPackets.Where(x => x.Source == packet.Destination
                    && x.Destination == packet.Source
                    && packet.Time - x.Time <= InterArrivalTimeout)
                    .OrderByDescending(x => x.Time)
                    .FirstOrDefault();
                    if (clientHello != null)
                    {
                        packet.InterArrivalTime = packet.Time - clientHello.Time;
                        tempServerHelloDonePackets.Add(packet);
                    }
                }
                else if (packet.Type == HandshakeType.ClientKeyExchange)
                {
                    // look for server hello done
                    var serverHelloDone = tempServerHelloDonePackets.Where(x => x.Source == packet.Destination
                    && x.Destination == packet.Source
                    && packet.Time - x.Time <= InterArrivalTimeout)
                    .OrderByDescending(x => x.Time)
                    .FirstOrDefault();
                    if(serverHelloDone != null)
                    {
                        packet.InterArrivalTime = packet.Time - serverHelloDone.Time;
                        tempClientKeyExchangePackets.Add(packet);
                    }
                }
                else if(packet.Type == HandshakeType.Finished)
                {
                    // look for client key exchange
                    var clientKeyExchange = tempClientKeyExchangePackets.Where(x => x.Source == packet.Destination
                    && x.Destination == packet.Source
                    && packet.Time - x.Time <= InterArrivalTimeout)
                    .OrderByDescending(x => x.Time)
                    .FirstOrDefault();
                    if(clientKeyExchange != null)
                    {
                        packet.InterArrivalTime = packet.Time - clientKeyExchange.Time;
                    }
                }
            }
        }
    }
}