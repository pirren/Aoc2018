namespace Aoc2018.Library
{
    public abstract class DayBase : ISolver
    {
        public enum SolutionPart
        {
            PartOne,
            PartTwo
        }

        private const string indataFolder = "Indata";
        private const string sampleFolder = "Sample";
        protected string Folder => UseSample ? sampleFolder : indataFolder;

        public virtual string Name => throw new NotImplementedException();
        public virtual int Day => throw new NotImplementedException();
        public virtual int Order => Day;
        public virtual string Indata => File.ReadAllText(Path.Combine(Folder, $"{Day}.in"));
        public virtual bool UseSample => false;

        public abstract object PartOne(string Indata);
        public abstract object PartTwo(string Indata);
    }
}
