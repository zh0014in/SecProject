using PcapngUtils.Common;
using PcapngUtils.PcapNG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PacketAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            PacketParser pp = new PacketParser();
            var result = pp.Parse("sample");
            //File.WriteAllLines(@"train", new string[] { "Time,Source,Destination,SrcPort,DestPort,Length,Info" });
            File.WriteAllLines(@"train", result.Select(x => x.ToString()));
            result = pp.Parse("test");
            //File.WriteAllLines(@"test", new string[] { "Time,Source,Destination,SrcPort,DestPort,Length,Info" });
            File.WriteAllLines(@"test", result.Select(x => x.ToString()));
            Console.WriteLine("Done");
            Console.Read();
        }

    }
}
