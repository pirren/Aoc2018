using Aoc2018.Library;

namespace Aoc2018.Solutions
{
    public class Day5 : DayBase
    {
        public override string Name => "Alchemical Reduction";
        public override int Day => 5;
        public override bool UseSample => base.UseSample;

        protected static int MaxBatchSize => 2000; // seems to be fastest size for this solution
        record struct ActiveWork(List<string> Rules, int BatchId, List<(string before, string after)> Seen) { }
        enum PolyState
        {
            FinishedBatch,
            MergedBatches,
            FinishedWork
        }

        public override object PartOne(string indata)
        {
            // Part 1: How many units remain after fully reacting the polymer you scanned?
            var rules = ParseRules();
            var polymer = ParsePolymerBatches(indata);

            return GetPolyLength(polymer, rules);
        }

        public override object PartTwo(string indata)
        {
            // Part 2: What is the length of the shortest polymer you can produce
            var rules = ParseRules();

            List<int> polyLengths = new();
            Parallel.ForEach(rules, rule =>
            {
                polyLengths.Add(GetPolyLength(ParsePolymerBatches(indata, rule), rules));
            });

            return polyLengths.Min();
        }

        private int GetPolyLength(List<(int, string)> polymer, List<string> rules)
        {
            var (newPolymer, newState) = React(polymer, rules);
            
            if (newState.Equals(PolyState.FinishedWork))
                return newPolymer.Single().sequence.Length;

            return GetPolyLength(newPolymer, rules);
        }

        private (List<(int, string sequence)>, PolyState) React(List<(int batchId, string sequence)> polymer, List<string> rules)
        {
            // we find out what rules applies on which batch to avoid uneccessary work
            var activeWork = (from poly in polymer
                              from rule in rules
                              where poly.sequence.Contains(rule)
                              group rule by poly.batchId into g
                              select new ActiveWork
                              {
                                  Rules = g.Select(x => x).ToList(),
                                  BatchId = g.Key
                              }).ToList();

            if (activeWork.Any())
                return (FinishWorkBatch(polymer, activeWork), PolyState.FinishedBatch); // finished batch

            if (polymer.ShouldMerge(activeWork.Any()))
                return (MergeBatches(polymer), PolyState.MergedBatches); // merge batches

            return (polymer, PolyState.FinishedWork); // finished work
        }

        private List<(int, string sequence)> FinishWorkBatch(List<(int batchId, string sequence)> polymer, List<ActiveWork> activeWork)
        {
            var activeBatches = polymer
                .Where(batch => activeWork.Any(work => work.BatchId.Equals(batch.batchId)));

            var staleBatches = polymer.Where(batch => !activeBatches.Any(x => x.batchId.Equals(batch.batchId)));

            List<(int batchId, string sequence)> newPolymer = new(staleBatches);
            foreach (var polyBatch in activeBatches)
            {
                var newBatch = string.Empty;
                int batchId = polyBatch.batchId;

                foreach (var rule in activeWork.Where(x => x.BatchId.Equals(batchId)).SelectMany(x => x.Rules))
                {
                    newBatch = polyBatch.sequence.Replace(rule, string.Empty);
                }
                newPolymer.Add((batchId, newBatch));
            }
            newPolymer = newPolymer.OrderBy(x => x.batchId).ToList();
            return newPolymer;
        }

        private List<(int, string)> MergeBatches(List<(int, string sequence)> polymer)
        {
            List<(int, string)> newPolymer = new();
            int batchId = 1;
            while (polymer.Any())
            {
                newPolymer.Add((batchId, string.Concat(polymer.Take(2).Select(x => x.sequence))));

                if (polymer.Count == 1) polymer.RemoveAt(0);
                else polymer.RemoveRange(0, 2);
                batchId++;
            }
            return newPolymer;
        }

        private List<string> ParseRules() 
            => new(Enumerable.Range(65, 26)
                .Select(x => {
                    var rule = new string(new[] { (char)x, (char)(x + 32) });
                    return new[] { rule, new string(rule.Reverse().ToArray()) };
                }).SelectMany(x => x));

        private List<(int batchId, string sequence)> ParsePolymerBatches(string indata, string skippedLetters = "")
        {
            List<(int, string)> poly = new();

            if(!string.IsNullOrEmpty(skippedLetters))
            {
                var skip = new[] { skippedLetters[0].ToString(), skippedLetters[1].ToString() };
                foreach (var c in new[] { skip[0].ToString(), skip[1].ToString() })
                {
                    indata = indata.Replace(c.ToString(), "");
                }
            }

            int sumBatch = 0;
            int batchId = 1;
            while (sumBatch < indata.Length)
            {
                int currentBatchSize = Math.Min(indata.Length - sumBatch, MaxBatchSize);
                poly.Add((batchId, indata.Substring(sumBatch, currentBatchSize)));
                sumBatch += currentBatchSize;
                batchId++;
            }

            return poly;
        }
    }

    public static class Ext5
    {
        public static bool ShouldMerge(this List<(int, string)> poly, bool workToBeDone)
            => !workToBeDone && poly.Count != 1;
    }
}
