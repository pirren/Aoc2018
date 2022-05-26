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

        private const int amountWorkers = 5;
        protected List<Worker> Workers = new();

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
            SetupWorkers();
            return SimulChainTree(tree, new());
        }

        private int SimulChainTree(List<WorkStep> remaining, List<string> visited, int time = 0)
        {
            if (!remaining.Any()) return time - 1;

            var workersAvailable = Workers.Where(w => string.IsNullOrEmpty(w.Step));
            foreach (var worker in Workers.Except(workersAvailable))
            {
                var (success, task) = worker.Work();
                if (!success) continue;

                visited.Add(task);
                time += task.TaskTime();
            }

            while(true)
            {
                var work = remaining.AvailableNodes(visited);
                if (!work.Any()) break;

                var worker = workersAvailable.FirstOrDefault();
                if (worker == null) break;

                worker.Step = remaining.Pop(work.First()).Step;
            }

            return SimulChainTree(remaining, visited, time);
        }

        private string ChainTree(List<WorkStep> remaining, List<string> visited)
        {
            if (!remaining.Any()) return String.Concat(visited);
            WorkStep next;

            if (remaining.Count == 1)
            {
                next = remaining.Single();
            }
            else
            {
                next = remaining.AvailableNodes(visited).First();
            }

            visited.Add(remaining.Pop(next).Step);

            return ChainTree(remaining, visited);
        }

        private void SetupWorkers()
        {
            Workers = Enumerable.Range(0, amountWorkers).Select(x => new Worker()).ToList();
        }


        private List<WorkStep> ParseNodeTree(string indata)
        {
            string prevNodePattern = @"Step (.*?) must";
            string nodePattern = @"step (.*?) can";

            var data = indata.Split("\r\n").Select(x => new
            {
                Prev = Regex.Matches(x, prevNodePattern).Single().Groups[1].Value,
                Step = Regex.Matches(x, nodePattern).Single().Groups[1].Value
            });
            
            var nodes = data.GroupBy(x => x.Step).Select(x => new WorkStep
            {
                Step = x.Key,
                PrevSteps = x.Select(x => x.Prev).ToList()
            }).ToList();

            // add startnodes on top
            nodes.AddRange(data.SelectMany(x => x.Prev)
                .Distinct()
                .Where(n => !nodes.Select(x => x.Step).Contains(n.ToString()))
                .Select(x => new WorkStep(x.ToString(), new())));

            return nodes;
        }

        public record struct WorkStep(string Step, List<string> PrevSteps);

        public class Worker
        {
            public bool Available => string.IsNullOrEmpty(Step);
            public int Time { get; set; } = 0;
            public string Step { get; set; } = string.Empty;

            public (bool success, string task) Work()
            {
                var debug = Step.TaskTime();
                Time++;
                if(Time != Step.TaskTime())
                    return (false, string.Empty);

                var returnTask = Step;
                Reset();
                return (true, returnTask);
            }

            private void Reset()
            {
                Time = 0;
                Step = string.Empty;
            }
        }
    }

    public static class Ext7 
    {
        public static int TaskTime(this string str)
            => Convert.ToInt32(char.Parse(str) - 64);

        public static IEnumerable<Day7.WorkStep> AvailableNodes(this List<Day7.WorkStep> remaining, List<string> visited)
            => remaining.Where(remain => !remain.PrevSteps.Any() || remain.PrevSteps.All(x => visited.Contains(x))).OrderBy(a => a.Step).ToList();
    }
}
