namespace View.Models.In
{
    public class InUserStatistics
    {
        public long UserId { get; set; }
        public int GameCount { get; set; }

        public int WinGames { get; set; }
        public int LossGames { get; set; }
    }
}
