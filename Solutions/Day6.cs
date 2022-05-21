using Aoc2018.Library;
using System.Threading.Tasks;

namespace Aoc2018.Solutions
{
    public class Day6 : DayBase
    {
        public override string Name => "Chronal Coordinates";
        public override int Day => 6;
        public override bool UseSample => base.UseSample;

        public record struct BoundingGrid(int MinX, int MaxX, int MinY, int MaxY);

        public override object PartOne(string indata)
        {
            // Part 1: What is the size of the largest area that isn't infinite?
            var points = GetPoints(indata);
            var grid = GetBoundingGrid(points);

            return LargestFiniteArea(points, grid);
        }

        public override object PartTwo(string indata)
        {
            // What is the size of the region containing all locations which have a total
            // distance to all given coordinates of less than 10000?
            var points = GetPoints(indata);
            var grid = GetBoundingGrid(points);
            return points.SafestRegionSize(grid);
        }

        private int LargestFiniteArea(List<(int x, int y)> points, BoundingGrid grid)
            => GetPointAreas(points, grid).Max(x => x.Value);

        private Dictionary<(int, int), int> GetPointAreas(List<(int x, int y)> points, BoundingGrid grid)
        {
            Dictionary<(int, int), int> pointAreas = new();
            foreach (var point in points.RemoveInfinitePoints(grid))
            {
                // all non infinite points
                pointAreas.Add(point, 0);
                var otherPoints = points.Where(x => x != point);
                for (int x = grid.MinX + 1; x < grid.MaxX; x++)
                {
                    for (int y = grid.MinY + 1; y < grid.MaxY; y++)
                    {
                        var manhattan = point.Manhattan((x, y));
                        if (!otherPoints.Any(p => p.Manhattan((x, y)) <= manhattan))
                            pointAreas[point]++;
                    }
                }
            }
            return pointAreas;
        }

        private BoundingGrid GetBoundingGrid(List<(int x, int y)> points)
            => new()
            {
                MinX = points.Min(p => p.x),
                MaxX = points.Max(p => p.x),
                MinY = points.Min(p => p.y),
                MaxY = points.Max(p => p.y)
            };

        private List<(int x, int y)> GetPoints(string indata)
            => indata.Split("\r\n").Select(x => x.Split(", ")).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToList();
    }

    public static class Ext6
    {
        //public static int AbsValue(this (int x, int y) point, (int x, int y) otherPoint)
        //{
        //    return Math.Abs(point.x - otherPoint.x) + Math.Abs(point.y - otherPoint.y);
        //}

        public static int SafestRegionSize(this List<(int x, int y)> points, Day6.BoundingGrid grid, int distance = 10000)
        {
            int size = 0;

            for (int x = grid.MinX + 1; x < grid.MaxX; x++)
            {
                for (int y = grid.MinX + 1; y < grid.MaxX; y++)
                {
                    if (points.Sum(p => p.Manhattan((x, y))) < distance)
                        size++;
                }
            }

            return size;
        }

        public static List<(int x, int y)> RemoveInfinitePoints(this List<(int x, int y)> points, Day6.BoundingGrid grid)
        {
            List<(int x, int y)> result = new(points);
            foreach(var point in points)
            {
                // do for every x on y axis
                bool removed = false;
                var otherPoints = points.Where(x => x != point);
                foreach (int y in new[] { grid.MaxY, grid.MinY })
                {
                    for (int x = grid.MinX + 1; x < grid.MaxX; x++)
                    {
                        var manhattan = point.Manhattan((x, y));
                        if (!otherPoints.Any(p => p.Manhattan((x, y)) <= manhattan))
                        {
                            result.Remove(point);
                            removed = true;
                            break;
                        }
                    }
                    if (removed) break;
                }

                // do for every y on x axis
                if (!removed)
                {
                    foreach (int x in new[] { grid.MaxX, grid.MinX })
                    {
                        for (int y = grid.MinX + 1; y < grid.MaxX; y++)
                        {
                            var manhattan = point.Manhattan((x, y));
                            if (!otherPoints.Any(p => p.Manhattan((x, y)) <= manhattan))
                            {
                                result.Remove(point);
                                removed = true;
                                break;
                            }
                        }
                        if (removed) break;
                    }
                }
            }

            return result;
        }
    }
}
