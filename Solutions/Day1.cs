using Aoc2018.Library;

namespace Aoc2018.Solutions
{
    public class Day1 : DayBase
    {
        public override string Name => "Chronal Calibration";
        public override int Day => 1;

        public override object PartOne(string indata)
        {
            return ParseData(indata).Sum();
        }

        public override object PartTwo(string indata)
        {
            int currentFrequency = 0;
            HashSet<int> frequencies = new()
            {
                currentFrequency
            };
            while (true)
            {
                foreach(var change in ParseData(indata))
                {
                    currentFrequency += change;

                    if (frequencies.Contains(currentFrequency)) 
                        return currentFrequency;
                    else frequencies.Add(currentFrequency);
                }
            }
        }

        private IEnumerable<int> ParseData(string indata)
            => indata.Split("\r\n")
                .Select(x => x[0] == '+' ? x[1..].ToInt() : x[1..].ToInt() * -1);
    }
}
