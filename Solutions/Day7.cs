using Aoc2018.Library;
using System.Text;
using System.Text.RegularExpressions;

namespace Aoc2018.Solutions
{
    public class Day7 : DayBase
    {
        public override string Name => "The Sum of Its Parts";
        public override int Day => 7;
        public override bool UseSample => base.UseSample;

        public override object PartOne(string indata)
        {
            // Part 1: In what order should the steps in your instructions be completed?
            var tree = ParseNodeTree(indata);
            var startNode = tree.Where(x => !x.PrevSteps.Any()).OrderBy(x => x.Step).First();
            tree.Remove(startNode);

            return ChainTree(tree, new List<string> { startNode.Step });
        }

        public override object PartTwo(string indata)
        {
            // Part 2: With 5 workers and the 60+ second step durations described above,
            //      how long will it take to complete all of the steps?
            var tree = ParseNodeTree(indata);
            var startNode = tree.Where(x => !x.PrevSteps.Any()).OrderBy(x => x.Step).First();
            tree.Remove(startNode);
            return SimulChainTree(tree, new List<string> { startNode.Step });
        }

        private int SimulChainTree(List<Node> remainingNodes, List<string> visited)
        {
            if (!remainingNodes.Any()) return 0;
            Node next = new();

            if (remainingNodes.Count == 1)
            {
                next = remainingNodes.Single();
            }
            else
            {
                next = remainingNodes.Where(remain => remain.PrevSteps.All(x => visited.Contains(x))).OrderBy(a => a.Step).FirstOrDefault();
            }
            var test = Convert.ToInt32(next.Step[0] - 64);
            var test2 = Convert.ToInt32('A' - 64);
            remainingNodes.Remove(next);
            visited.Add(next.Step);

            return SimulChainTree(remainingNodes, visited);
        }

        private string ChainTree(List<Node> remainingNodes, List<string> visited)
        {
            if (!remainingNodes.Any()) return String.Concat(visited);
            Node next = new();

            if(remainingNodes.Count == 1)
            {
                next = remainingNodes.Single();
            } 
            else
            {
                next = remainingNodes.Where(remain => remain.PrevSteps.All(x => visited.Contains(x))).OrderBy(a => a.Step).FirstOrDefault();
            }

            remainingNodes.Remove(next);
            visited.Add(next.Step);

            return ChainTree(remainingNodes, visited);
        }

        private readonly string prevNodePattern = @"Step (.*?) must";
        private readonly string nodePattern = @"step (.*?) can";
        private List<Node> ParseNodeTree(string indata)
        {
            var data = indata.Split("\r\n").Select(x => new
            {
                Prev = Regex.Matches(x, prevNodePattern).Single().Groups[1].Value,
                Step = Regex.Matches(x, nodePattern).Single().Groups[1].Value
            });
            
            var nodes = data.GroupBy(x => x.Step).Select(x => new Node
            {
                Step = x.Key,
                PrevSteps = x.Select(x => x.Prev).ToList()
            }).ToList();

            // add startnodes on top
            nodes.AddRange(data.SelectMany(x => x.Prev)
                .Distinct()
                .Where(n => !nodes.Select(x => x.Step).Contains(n.ToString()))
                .Select(x => new Node(x.ToString(), new())));

            return nodes;
        }

        record struct Node(string Step, List<string> PrevSteps);
    }
}
