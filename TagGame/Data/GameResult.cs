using System;

namespace TagGame.Data
{
    public class GameResult
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int Steps { get; set; }

        public GameResult(DateTime date, string time, int steps)
        {
            Date = date;
            Time = time;
            Steps = steps;
        }
    }
}
