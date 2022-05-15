namespace Aoc2018.Library
{
    public interface ISolver
    {
        int Order { get; }
        string Name { get; }
        int Day { get; }
        string Indata { get; }
        object PartOne(string Indata);
        object PartTwo(string Indata);
    }
}
