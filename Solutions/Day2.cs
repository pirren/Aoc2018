using Aoc2018.Library;

namespace Aoc2018.Solutions
{
    public class Day2 : DayBase
    {
        public override string Name => "Inventory Management System";
        public override int Day => 2;
        public override bool UseSample => base.UseSample;

        public override object PartOne(string indata)
        {
            var letterCounts = GetBoxInformation(indata).Select(x => x.count);
            int checksum = letterCounts.Count(x => x == 2) * letterCounts.Count(x => x == 3);
            return checksum;
        }

        public override object PartTwo(string indata)
        {
            var allBoxIds = GetBoxInformation(indata).Select(x => x.boxId);

            foreach(var boxId in allBoxIds)
            {
                foreach(var otherboxId in allBoxIds.Where(x => x != boxId))
                {
                    string lettersInCommon = "";
                    for(int i = 0; i < boxId.Length; i++)
                        if (boxId[i] == otherboxId[i]) lettersInCommon += boxId[i];

                    if(lettersInCommon.Length == boxId.Length - 1)
                        return lettersInCommon;
                }
            }

            return 0;
        }

        private IEnumerable<(int count, string boxId)> GetBoxInformation(string indata)
        {
            var data = ParseIdsFromIndata(indata);
            return data.SelectMany(
                    (str, internalId) =>
                        str.GroupBy(ch => ch)
                        .Where(x => new[] { 2, 3 }.Contains(x.Count()))
                        .Select(x => new { Count = x.Count(), BoxId = data[internalId] })
                    ).DistinctBy(x => new { x.BoxId, x.Count })
                    .Select(x => (x.Count, x.BoxId));
        }

        private List<string> ParseIdsFromIndata(string indata) => indata.Split("\r\n").ToList();
    }
}
