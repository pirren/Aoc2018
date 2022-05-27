using Aoc2018.Library;
using System.Text.RegularExpressions;

namespace Aoc2018.Solutions
{
    public class Day7 : DayBase
    {
        public override string Name => "The Sum of Its Parts";
        public override int Day => 7;
        public override bool UseSample => base.UseSample;

        private const int amountWorkers = 5;

        public override object PartOne(string indata)
        {
            // Part 1: In what order should the steps in your instructions be completed?
            var tree = new Tree(ParseNodes(indata));
            return tree.ChainSequence();
        }

        public override object PartTwo(string indata)
        {
            // Part 2: With 5 workers and the 60+ second step durations described above,
            //      how long will it take to complete all of the steps?
            var tree = new Tree(ParseNodes(indata));
            return tree.ChainTime();
        }

        public List<WorkStep> ParseNodes(string indata)
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

        public class Tree
        {
            public Tree(List<WorkStep> nodes)
            {
                Nodes = nodes;
                Workers = Enumerable.Range(0, amountWorkers).Select(x => new Worker()).ToList();
            }

            protected List<Worker> Workers = new();
            protected List<WorkStep> Nodes = new();
            protected List<string> CompletedSteps = new();

            private IEnumerable<Worker> AvailableWorkers => Workers.Where(x => string.IsNullOrEmpty(x.Step));
            private IEnumerable<Worker> ActiveWorkers => Workers.Except(AvailableWorkers);

            public int ChainTime(int time = 0)
            {
                if (!Nodes.Any() && !ActiveWorkers.Any()) return time;

                while (true)
                {
                    var work = Nodes.AvailableNodes(CompletedSteps);
                    if (!work.Any()) break;

                    var worker = AvailableWorkers.FirstOrDefault();
                    if (worker == null) break;

                    worker.Step = Nodes.Pop(work.First()).Step;
                }

                foreach (var worker in ActiveWorkers)
                {
                    var (success, step) = worker.Work();
                    if (!success) continue;

                    CompletedSteps.Add(step);
                }

                time++;
                return ChainTime(time);
            }

            public string ChainSequence()
            {
                if (!Nodes.Any()) return String.Concat(CompletedSteps);
                WorkStep next;

                if (Nodes.Count == 1)
                {
                    next = Nodes.Single();
                }
                else
                {
                    next = Nodes.AvailableNodes(CompletedSteps).First();
                }

                CompletedSteps.Add(Nodes.Pop(next).Step);

                return ChainSequence();
            }
        }

        public record struct WorkStep(string Step, List<string> PrevSteps) { }

        public class Worker
        {
            public bool Available => string.IsNullOrEmpty(Step);
            public int Time { get; set; } = 0;
            public string Step { get; set; } = string.Empty;

            public (bool success, string task) Work()
            {
                Time++;

                if(Time < Step.TaskTime()) return (false, string.Empty);

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
            => Convert.ToInt32(char.Parse(str) - 64) + 60; // letter position in alphabet + 60 (1 minute)

        public static IEnumerable<Day7.WorkStep> AvailableNodes(this List<Day7.WorkStep> remaining, List<string> visited)
            => remaining.Where(remain => !remain.PrevSteps.Any() || remain.PrevSteps.All(x => visited.Contains(x)))
                .OrderBy(a => a.Step);
    }
}
