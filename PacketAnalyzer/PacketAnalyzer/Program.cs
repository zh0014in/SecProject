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
            File.WriteAllLines(@"test", result.Select(x => x.ToString()));
            Console.Read();
        }

    }
}
