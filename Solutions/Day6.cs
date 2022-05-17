using Aoc2018.Library;

namespace Aoc2018.Solutions
{
    public class Day6 : DayBase
    {
        public override string Name => "Chronal Coordinates";
        public override int Day => 6;
        public override bool UseSample => !base.UseSample;

        public record struct BoundingGrid(int MinX, int MaxX, int MinY, int MaxY);

        public override object PartOne(string indata)
        {
            // Part 1: What is the size of the largest area that isn't infinite?
            var points = GetPoints(indata);
            var grid = GetBoundingGrid(points);
            points = points.RemoveInfinitePoints(grid);


            return 0;
        }

        public override object PartTwo(string indata)
        {
            // ?
            var coords = GetPoints(indata);


            return 0;
        }

        private BoundingGrid GetBoundingGrid(List<(int x, int y)> points)
        {
            return new BoundingGrid
            {
                MinX = points.Min(p => p.x),
                MaxX = points.Max(p => p.x),
                MinY = points.Min(p => p.y),
                MaxY = points.Max(p => p.y)
            };
        }

        private List<(int x, int y)> GetPoints(string indata)
            => indata.Split("\r\n").Select(x => x.Split(", ")).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToList();
    }

    public static class Ext6
    {
        public static List<(int x, int y)> RemoveInfinitePoints(this List<(int x, int y)> points, Day6.BoundingGrid grid)
            => points.Where(p => p.x != grid.MinX 
                && p.x != grid.MaxX
                && p.y != grid.MinY 
                && p.y != grid.MaxY)
            .ToList();

        public static int Manhattan(this (int x, int y) p, (int x, int y) other) 
            => (other.x - p.x) + (other.y - p.y);

        public static bool IsInfinite(this (int x, int y) p, Day6.BoundingGrid grid)
            => p.x > grid.MinX && p.x < grid.MaxX && p.y > grid.MinY && p.y < grid.MaxY;
    }
}
