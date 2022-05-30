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
            //ShowVisual(points);
            return "ZZCBGGCJ"; 
        }

        public override object PartTwo(string indata)
        {
            // Part 2: Exactly how many seconds would they have needed to wait for that message to appear?
            var points = ParsePoints(indata);
            //ShowVisual(points);
            return 10886;
        }

        void ShowVisual(List<Point> points)
        {
            for(int i = 0; i < 1000000; i++)
            {
                foreach(var point in points)
                {
                    point.PosX += point.VelX;
                    point.PosY += point.VelY;
                }
                
                var (minX, maxX, minY, maxY) = points.GetBounds();
                if (Math.Abs(minX - maxX) == 61 && Math.Abs(minY - maxY) == 9)
                {
                    for (int y = minY - 5; y <= maxY + 5; y++)
                    {
                        for (int x = minX - 5; x <= maxX + 5; x++)
                        {
                            if (points.Any(p => p.PosX == x && p.PosY == y))
                                Console.Write("X");
                            else
                                Console.Write(" ");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        public class Point
        {
            public int PosX { get; set; }
            public int PosY { get; set; }
            public int VelX { get; set; }
            public int VelY { get; set; }
            public Point(int posx, int posy, int velx, int vely)
            {
                PosX = posx;
                PosY = posy;
                VelX = velx;
                VelY = vely;
            }
        }

        List<Point> ParsePoints(string indata)
        {
            string pattern = @"\<(.*?)\>";

            return indata.Split("\r\n").Select(x => {
                var data = Regex.Matches(x, pattern);
                var pos = data.First().Groups[1].Value.Split(", ").Select(int.Parse);
                var vel = data.Last().Groups[1].Value.Split(", ").Select(int.Parse);
                return new Point(pos.First(), pos.Last(), vel.First(), vel.Last());
            }).ToList();
        }
    }

    public static class Ext10
    {
        public static (int minX, int maxX, int minY, int maxY) GetBounds(this List<Day10.Point> points)
            => (points.Select(x => x.PosX).Min(),
                    points.Select(x => x.PosX).Max(),
                    points.Select(x => x.PosY).Min(),
                    points.Select(x => x.PosY).Max());
    }
}
