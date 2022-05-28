using Aoc2018.Library;

namespace Aoc2018.Solutions
{
    public class Day8 : DayBase
    {
        public override string Name => "Memory Maneuver";
        public override int Day => 8;
        public override bool UseSample => base.UseSample;

        public override object PartOne(string indata)
        {
            // Part 1: What is the sum of all metadata entries?
            var data = ParseIndata(indata);
            var package = DequeueTree(data);
            return Metadata(package).Sum();
        }

        public override object PartTwo(string indata)
        {
            // Part 2: What is the value of the root node?
            var data = ParseIndata(indata);
            var package = DequeueTree(data);
            return RootNodeValue(package);
        }

        public Package DequeueTree(Queue<int> rawData)
        {
            Package packet = new();

            while (rawData.Any())
            {
                packet = DequeueNextPacket(rawData);
            }

            return packet;
        }

        public Package DequeueNextPacket(Queue<int> rawData)
        {
            Package packet = rawData.PackageHeader();

            // ---------------------------
            // first parse subpackets
            for (int i = 0; i < packet.HeaderSubPackets; i++)
            {
                packet.SubPackets.Add(DequeueNextPacket(rawData));
            }

            // ---------------------------
            // get the metadata
            for (int i = 0; i < packet.HeaderMetaEntries; i++)
            {
                packet.MetaData.Add(rawData.Dequeue());
            }

            return packet;
        }

        public int RootNodeValue(Package packet)
        {
            List<int> metaData = new();

            if(!packet.SubPackets.Any())
            {
                metaData.AddRange(packet.MetaData);
            }
            else
            {
                foreach(var data in packet.MetaData)
                {
                    if (packet.SubPackets.ElementAtOrDefault(data - 1) == null) 
                        continue;
                    metaData.Add(RootNodeValue(packet.SubPackets[data - 1]));
                }
            }

            return metaData.Sum();
        }

        public List<int> Metadata(Package packet)
        {
            List<int> metaData = new();
            metaData.AddRange(packet.MetaData);
            
            foreach (var subpack in packet.SubPackets)
            {
                metaData.AddRange(Metadata(subpack));
            }

            return metaData;
        }

        public class Package
        {
            public int HeaderSubPackets { get; set; }
            public int HeaderMetaEntries { get; set; }

            public List<Package> SubPackets = new();
            public List<int> MetaData = new();
        }


        public Queue<int> ParseIndata(string indata)
        {
            Queue<int> queue = new();
            var data = indata.Split(' ').Select(int.Parse).ToList();
            data.ForEach(x => queue.Enqueue(x));
            return queue;
        }
    }

    public static class Ext8
    {
        public static Day8.Package PackageHeader(this Queue<int> rawData)
        {
            return new Day8.Package
            {
                HeaderSubPackets = rawData.Dequeue(),
                HeaderMetaEntries = rawData.Dequeue()
            };
        }
    }
}
