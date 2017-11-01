﻿using PcapngUtils.Common;
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
            Console.WriteLine(@"1. Parse raw packets for hidden features.
2. Generate ssl sessions, MSC and CSP from packets.");
            var choice = Console.ReadLine();
            PacketParser pp = new PacketParser();
            if (choice.Equals("1"))
            {
                var result = pp.Parse("sample");
                File.WriteAllLines(@"train", result.Select(x => x.ToString()));
                result = pp.Parse("test");
                File.WriteAllLines(@"test", result.Select(x => x.ToString()));
            }
            else if (choice.Equals("2"))
            {
                var result = pp.DecodeSessions("result_svm.csv");
                File.WriteAllLines(@"sessions", result.Select(x => "=====Session start=====" + Environment.NewLine
                + x.ToString() + Environment.NewLine 
                + "=====Session end=====" + Environment.NewLine));
                var session = result.First();
                var msc = pp.SessionToMSC(session);
                File.WriteAllText($"msc-{session.ClientHello.Source}-{session.ClientHello.Destination}.txt", msc);
            }
            Console.WriteLine("Done");
            Console.Read();
        }

    }
}
