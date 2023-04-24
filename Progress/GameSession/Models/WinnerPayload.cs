using GameSession.Models.Gamers;

namespace GameSession.Models
{
    public class WinnerPayload
    {
        public Guid GameUid { get; set; }
        public IGamer WinnerGamer { get; set; }
        public WinnerPayload(Guid gameUid, IGamer winnerGamer)
        {
            this.GameUid = gameUid;
            this.WinnerGamer = winnerGamer;
        }
    }
}
