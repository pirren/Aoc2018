using Aoc2018.Library;
using System.Text.RegularExpressions;

namespace Aoc2018.Solutions
{
    public class Day4 : DayBase
    {
        public override string Name => "Repose Record";
        public override int Day => 4;
        public override bool UseSample => base.UseSample;

        public override object PartOne(string indata)
        {
            // Strategy 1: Find the guard that has the most minutes asleep. What minute does that guard spend asleep the most?
            var logs = GetOrderedLogs(indata);
            var guardId = SleepiestGuardId(new(logs));

            return guardId * MostAsleepAt(GetSleepWindowsForGuard(new(logs), guardId));
        }

        public override object PartTwo(string indata)
        {
            // Strategy 2: Of all guards, which guard is most frequently asleep on the same minute?
            var logs = GetOrderedLogs(indata);
            Dictionary<int, List<(DateTime, DateTime)>> sleepWindowsForGuards = new();
            foreach(var guardId in logs.Select(x => x.Id).Where(x => x != 0).Distinct())
            {
                sleepWindowsForGuards.Add(guardId, GetSleepWindowsForGuard(new(logs), guardId));
            }

            Dictionary<int, int> guardsMostAsleepAt = new();
            foreach (var kvp in sleepWindowsForGuards.Where(x => x.Value.Count != 0))
            {
                guardsMostAsleepAt.Add(kvp.Key, MostFrequentSleepingMinute(kvp.Value));
            }
            var guardWithMostFrequentMinute = guardsMostAsleepAt.OrderByDescending(x => x.Value).First();

            var mostAsleepAt = MostAsleepAt(sleepWindowsForGuards[guardWithMostFrequentMinute.Key]);
            return guardWithMostFrequentMinute.Key * mostAsleepAt;
        }

        private int MostFrequentSleepingMinute(List<(DateTime fallAsleep, DateTime wakeUp)> windows)
            => windows.SelectMany(x => Enumerable.Range(x.fallAsleep.Minute, x.wakeUp.Minute - x.fallAsleep.Minute))
                .GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Count()).First();

        private int MostAsleepAt(List<(DateTime fallAsleep, DateTime wakeUp)> windows)
            => windows.SelectMany(x => Enumerable.Range(x.fallAsleep.Minute, x.wakeUp.Minute - x.fallAsleep.Minute))
                .GroupBy(x => x).OrderByDescending(x => x.Count()).Select(grp => grp.Key).First();

        private List<(DateTime, DateTime)> GetSleepWindowsForGuard(Queue<LogEntry> logs, int guardId)
        {
            List<(DateTime, DateTime)> sleepWindowsForGuard = new();
            var guardIdLogs = new Queue<LogEntry>((logs.Where(x => x.Id.Equals(guardId))));
            while (guardIdLogs.Any())
            {
                var shiftStartlog = guardIdLogs.Dequeue(); // discard the shift start log
                if(shiftStartlog.Type != LogType.BeginShift)
                {
                    throw new Exception($"Log was of unexpected type: {shiftStartlog.Type}. Expected type was: {LogType.BeginShift}");
                }
                var shiftLogs = guardIdLogs.DequeueGuardLogs(guardId);
                while (shiftLogs.Any())
                {
                    var (sleepLog, wakeLog) = shiftLogs.NextSleepLogs();
                    sleepWindowsForGuard.Add((sleepLog.TimeStamp, wakeLog.TimeStamp));
                }
            }
            return sleepWindowsForGuard;
        }

        private int SleepiestGuardId(Queue<LogEntry> logs)
        {
            Dictionary<int, int> guardSumSleepTime = new();
            while (logs.Any())
            {
                var guardId = logs.Dequeue().Id;
                var shiftLogs = logs.DequeueGuardLogs(guardId);
                while (shiftLogs.Any())
                    guardSumSleepTime.AddIncrement(guardId, Math.Abs(shiftLogs.SumNextSleepWindow()));
            }
            return guardSumSleepTime.OrderByDescending(x => x.Value).First().Key;
        }

        private Queue<LogEntry> GetOrderedLogs(string indata)
        {
            var logs = indata.Split("\r\n").Select(x =>
            {
                var (timestamp, id, type) = x.GetLogInfo();
                return new LogEntry
                {
                    TimeStamp = timestamp,
                    Type = type,
                    Id = id
                };
            }).OrderBy(x => x.TimeStamp).ToList();
            int currentId = logs.First().Id;
            for(int i = 1; i < logs.Count; i++)
            {
                if (logs[i].Id != 0) 
                    currentId = logs[i].Id;
                logs[i].Id = currentId;
            }
            return new Queue<LogEntry>(logs);
        }

        public enum LogType
        {
            BeginShift = 0,
            FallsAsleep = 1,
            WakesUp = 2
        }

        public class LogEntry
        {
            public LogType Type { get; set; }
            public int Id { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }

    public static class Ext4
    {
        public static (Day4.LogEntry sleepLog, Day4.LogEntry wakeLog) NextSleepLogs(this Queue<Day4.LogEntry> logs)
            => (logs.Dequeue(), logs.Dequeue());

        public static int SumNextSleepWindow(this Queue<Day4.LogEntry> logs)
        {
            if (!logs.Any()) return 0;
            var (sleepLog, wakeLog) = logs.NextSleepLogs();
            return (int)(sleepLog.TimeStamp - wakeLog.TimeStamp).TotalMinutes;
        }

        public static Queue<Day4.LogEntry> DequeueGuardLogs(this Queue<Day4.LogEntry> logs, int guardId)
        {
            Queue<Day4.LogEntry> guardLogs = new();
            while(logs.Any() && logs.Peek().Id.Equals(guardId) && !logs.Peek().Type.Equals(Day4.LogType.BeginShift))
                guardLogs.Enqueue(logs.Dequeue());
            return guardLogs;
        }

        public static void AddIncrement<TKey>(this Dictionary<TKey, int> dict, TKey key, int value)
        {
            if (dict.ContainsKey(key))
                dict[key] += value;
            else 
                dict.TryAdd(key, value);
        }

        private static string pattern = @"\[(.*?)\]";
        public static (DateTime timestamp, int id, Day4.LogType type) GetLogInfo(this string data)
        {
            var timestamp = Convert.ToDateTime(Regex.Matches(data, pattern).Single().Groups[1].Value);
            if (data.Contains("begins shift"))
            {
                var id = data.Split(' ').Where(x => x.StartsWith('#')).Single().TrimStart('#').ToInt();
                return (timestamp, id, Day4.LogType.BeginShift);
            }
            if (data.Contains("falls asleep")) return (timestamp, 0, Day4.LogType.FallsAsleep);
            if (data.Contains("wakes up")) return (timestamp, 0, Day4.LogType.WakesUp);

            throw new ArgumentException($"Unhandled log type. Data: {data}");
        }
    }
}

