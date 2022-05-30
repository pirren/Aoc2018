using Aoc2018.Library;
using System.Text.RegularExpressions;

namespace Aoc2018.Solutions
{
    public class Day10 : DayBase
    {
        public override string Name => "The Stars Align";
        public override int Day => 10;
        public override bool UseSample => base.UseSample;

        public override object PartOne(string indata)
        {
            // Part 1: What message will eventually appear in the sky?
            var points = ParsePoints(indata);
            var debug = points.Select(x => x.PosX).Min();
            var debug2 = points.Select(x => x.PosX).Max();
            var test = Math.Max(debug, debug2) - Math.Min(debug, debug2);
            while(true)
            {

            }
            return 0; 
        }

        public override object PartTwo(string indata)
        {
            // Part 2: What would the new winning Elf's score be if the number of the last marble were 100 times larger?
            var points = ParsePoints(indata);
            return 0;
        }

        IEnumerable<Point> ParsePoints(string indata)
        {
            string pattern = @"\<(.*?)\>";

            return indata.Split("\r\n").Select(x => {
                var data = Regex.Matches(x, pattern);
                var pos = data.First().Groups[1].Value.Split(", ").Select(int.Parse);
                var vel = data.Last().Groups[1].Value.Split(", ").Select(int.Parse);
                return new Point(pos.First(), pos.Last(), vel.First(), vel.Last());
            });
        }

        record struct Point(int PosX, int PosY, int VelX, int VelY) { }
    }

    public static class Ext10
    {

    }
}
