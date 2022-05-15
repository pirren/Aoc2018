using Aoc2018.Library;
using System.Text.RegularExpressions;

namespace Aoc2018.Solutions
{
    public class Day5 : DayBase
    {
        public override string Name => "Alchemical Reduction";
        public override int Day => 5;
        public override bool UseSample => !base.UseSample;

        public override object PartOne(string indata)
        {
            // Strategy 1: Find the guard that has the most minutes asleep. What minute does that guard spend asleep the most?
            var rules = ReactingPairRules();
            var polymer = GetPolymer(indata);
            //var product = Trigger(polymer, rules);
            var result = Trigger(polymer, rules).Where(x => x.Value > 0).Select(x => x.Key).Count();
            return result;
        }

        public override object PartTwo(string indata)
        {
            // Strategy 2: Of all guards, which guard is most frequently asleep on the same minute?
            var polymer = GetPolymer(indata);

            return 0;
        }

        private Dictionary<(char, char), int> Trigger(Dictionary<(char,char), int> polymer, List<string> rules)
        {
            // no more reacting pairs, we are done
            //if (!polymer.Where(x => x.Value > 0).Any(x => rules.Contains(x.Key)))
            //    return polymer;

            return Trigger(React(polymer, rules), rules);
        }

        private Dictionary<(char, char), int> React(Dictionary<(char, char), int> polymer, List<string> rules)
        {
            Dictionary<(char, char), int> newPolymer = new(polymer);
            // do something
            return newPolymer;
        }

        // get pairs Aa, aA, Bb etc
        private List<string> ReactingPairRules() 
            => new(Enumerable.Range(65, 26)
                .Select(x => {
                    var rule = new string(new[] { (char)x, (char)(x + 32) });
                    return new[] { rule, new string(rule.Reverse().ToArray()) };
                }).SelectMany(x => x));

        private Dictionary<(char, char), int> GetPolymer(string indata)
        {
            Dictionary<(char, char), int> poly = new();
            indata.SkipLast(1).Select((ch, idx) => (ch, indata[idx+1]))
                .ForEach(x => poly.AddIncrement(x));
            return poly;
        }
    }

    public static class Ext5
    {
        public static void AddIncrement<TKey>(this Dictionary<TKey, int> poly, TKey key)
        {
            if (poly.ContainsKey(key))
                poly[key]++;
            else
                poly.Add(key, 1);
        }
    }
}
