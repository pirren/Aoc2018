using Aoc2018.Library;

namespace Aoc2018.Solutions
{
    public class Day9 : DayBase
    {
        public override string Name => "Marble Mania";
        public override int Day => 9;
        public override bool UseSample => base.UseSample;

        public override object PartOne(string indata)
        {
            // Part 1: What is the winning Elf's score?
            var settings = GetSettings(indata, SolutionPart.PartOne);
            return PlayGame(new GameBuffer(settings.LastMarble), settings).Select(x => x.Value).Max(); // 8317
        }

        public override object PartTwo(string indata)
        {
            // Part 2: What is the value of the root node?
            var settings = GetSettings(indata, SolutionPart.PartTwo);
            return PlayGame(new GameBuffer(settings.LastMarble), settings).Select(x => x.Value).Max();
        }

        /// <summary>
        /// Returns highscore
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private Dictionary<int, int> PlayGame(GameBuffer game, GameSettings settings) 
        {
            int currentPlayer = 0;

            var playerScores = GetPlayers(settings);

            while(true)
            {

                var turnResult = game.PlaceNext();
                playerScores[currentPlayer] += turnResult.Score;

                if (turnResult.CurrentMarble.Equals(settings.LastMarble)) break; // game is over
                currentPlayer = NextPlayerTurn(settings.Players, currentPlayer);
            }

            return playerScores;
        }

        int NextPlayerTurn(int totalPlayers, int currentPlayer)
        {
            currentPlayer++;
            return currentPlayer > totalPlayers - 1 ? 0 : currentPlayer;
        }

        Dictionary<int, int> GetPlayers(GameSettings settings)
        {
            Dictionary<int, int> playerScores = new();
            Enumerable.Range(0, settings.Players).ForEach(player => playerScores.Add(player, 0));
            return playerScores;
        }

        GameSettings GetSettings(string indata, SolutionPart part)
        {
            var data = indata.Split(' ').SkipLast(1);
            
            var playerCount = int.Parse(data.First());
            var lastMarble = int.Parse(data.Last());
            if (part.Equals(SolutionPart.PartTwo))
                lastMarble *= 100;

            return new(playerCount, lastMarble);
        }

        public class GameBuffer
        {
            private List<int> Buffer = new();
            private int currentMarble = -1;
            private int _pointer = 0;

            public GameBuffer(int lastMarble)
            {
                Buffer = new();
            }

            public TurnResult PlaceNext()
            {
                currentMarble++;
                if (currentMarble < 2)
                {
                    _pointer = currentMarble;
                    Buffer.Add(currentMarble);
                    return new(0, currentMarble);
                }

                if (currentMarble > 0 && currentMarble % 23 == 0) // player scores
                {
                    _pointer = _pointer.GetScoreMarble(Buffer.Count);
                    var score = Buffer.PopAt(_pointer) + currentMarble; // score is currentMarble plus marble 7 positions back

                    return new(score, currentMarble);
                }

                _pointer = _pointer.GetPointerPosition(Buffer.Count);
                Buffer.Insert(_pointer, currentMarble);
                return new (0, currentMarble);
            }
        }

        public record struct GameSettings(int Players, int LastMarble) { }
        public record struct TurnResult(int Score, int CurrentMarble) { }
    }

    public static class Ext9
    {
        public static int GetPointerPosition(this int pointer, int size)
        {
            var newPointer = pointer + 2;
            return newPointer > size ? newPointer - size : newPointer;
        }
        public static int GetScoreMarble(this int pointer, int size)
        {
            var newPointer = pointer - 7;
            if (newPointer < 0) return size - Math.Abs(newPointer);
            return newPointer > size ? newPointer - size : newPointer;
        }
    }
}
