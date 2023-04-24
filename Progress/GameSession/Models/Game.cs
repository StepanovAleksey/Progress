using Contracts.DTO;
using Contracts.Enums;
using GameSession.Hubs;
using GameSession.Models.Gamers;
using Microsoft.AspNetCore.SignalR;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace GameSession.Models
{
    public class Game
    {
        public IHubContext<GameHub> HubContext { get; }

        /// <summary>
        /// идетнификатор текущей игры
        /// </summary>
        public Guid Uid { get; set; } = Guid.NewGuid();


        /// <summary>
        /// событие о конце игры
        /// </summary>
        public Subject<WinnerPayload> EndGameSubj = new();

        /// <summary>
        /// все игроки
        /// </summary>
        public IGamer[] Gamers { get; set; }

        /// <summary>
        /// имя группы WS-Hub-а
        /// </summary>
        private string HubGroupName { get => this.Uid.ToString(); }


        private IDictionary<EShootStatus, Func<CoordinateSimple, ShipDto, IGamer, IGamer, Task>> strategy = new Dictionary<EShootStatus, Func<CoordinateSimple, ShipDto, IGamer, IGamer, Task>>();

        public Game(IHubContext<GameHub> hubContext, params IGamer[] gamers)
        {
            HubContext = hubContext;
            InitGamers(gamers);
            InitGameHub();

            this.strategy.Add(EShootStatus.Error, (coordiante, ship, shootGamer, otherGamer) =>
            {
                return Task.CompletedTask;
            });

            this.strategy.Add(EShootStatus.Miss, async (coordiante, ship, shootGamer, otherGamer) =>
            {
                foreach (var gamer in Gamers)
                {
                    gamer.SwitchShoot();
                }
                await SendShoot(EShootStatus.Miss, coordiante, shootGamer, otherGamer);
            });

            this.strategy.Add(EShootStatus.Hit, async (coordiante, ship, shootGamer, otherGamer) =>
            {
                foreach (var gamer in Gamers)
                {
                    gamer.ContinueShoot();
                }
                await SendShoot(EShootStatus.Hit, coordiante, shootGamer, otherGamer);
            });

            this.strategy.Add(EShootStatus.Killing, async (coordiante, ship, shootGamer, otherGamer) =>
            {
                await ExecuteStrategy(EShootStatus.Hit, ship, shootGamer, otherGamer, coordiante);
                await SendGamerMsg("KillingShip", new KillingShipDto
                {
                    SourceGamerConnectionId = shootGamer.ConnectionId,
                    TargetGamerConnectionId = otherGamer.ConnectionId,
                    GameUid = HubGroupName,
                    Coordinates = ship.Coordinates.ToArray(),
                }); ;
            });

            this.strategy.Add(EShootStatus.KillingAll, async (coordiante, ship, shootGamer, otherGamer) =>
            {
                await EndGame(shootGamer.ConnectionId);
            });
        }

        /// <summary>
        /// обработка выстрела
        /// </summary>
        public async Task EvolveShoot(string shootGamerConnectionId, CoordinateSimple coordinateShoot)
        {
            var otherGamer = GetOtherGamer(shootGamerConnectionId);
            var shootGamer = GetGamerById(shootGamerConnectionId).AddHistory(coordinateShoot);

            var (status, ship) = otherGamer.EvolveShoot(coordinateShoot);

            await ExecuteStrategy(status, ship, shootGamer, otherGamer, coordinateShoot);
        }

        public static void BotShoot(string shootGamerConnectionId, CoordinateSimple coordinateShoot)
        {
            //EvolveShoot(shootGamerConnectionId, coordinateShoot);
        }

        private void InitGamers(IGamer[] gamers)
        {
            Gamers = gamers;
            var shootGamer = Gamers[new Random().Next(1)];
            shootGamer.ChangeStatus(EGamerStatus.Shooted);
            GetOtherGamer(shootGamer.ConnectionId).ChangeStatus(EGamerStatus.Wait);
            Observable.Merge(Gamers.Select(gamer => gamer.DisconnectedSub)).Subscribe(async (connectionId) =>
            {
                await EndGame(GetOtherGamer(connectionId).ConnectionId);
            });

        }

        private async Task EndGame(string winnerGamerId)
        {
            await SendGamerMsg("EndGame", new EndGameDto
            {
                GameUid = HubGroupName,
                WinnerGamerId = winnerGamerId
            });
            this.GetGamerById(winnerGamerId).ChangeStatus(EGamerStatus.Winner);
            this.EndGameSubj.OnNext(new WinnerPayload(this.Uid, this.GetGamerById(winnerGamerId)));
            this.EndGameSubj.OnCompleted();
        }

        private async Task SendShoot(EShootStatus eShootStatus, CoordinateSimple coordinate, IGamer shootGamer, IGamer otherGamer)
        {
            await SendGamerMsg("ResultShoot", new ShootResultDto
            {
                ShootStatus = eShootStatus,
                SourceGamerConnectionId = shootGamer.ConnectionId,
                TargetGamerConnectionId = otherGamer.ConnectionId,
                GameUid = HubGroupName,
                NextGamerShooterConnectionId = GetShooterGamer().ConnectionId,
                Coordinate = coordinate
            });
        }

        private async Task ExecuteStrategy(EShootStatus status, ShipDto ship, IGamer shootGamer, IGamer otherGamer, CoordinateSimple coordinateShoot)
        {
            if (!strategy.ContainsKey(status))
            {
                throw new KeyNotFoundException(status.ToString());
            }
            await strategy[status](coordinateShoot, ship, shootGamer, otherGamer);
        }

        /// <summary>
        /// проверка на игрока
        /// </summary>
        /// <param name="gamerConnectionId"></param>
        /// <returns></returns>
        public bool IsExistGamer(string gamerConnectionId)
        {
            return Gamers.Any(g => g.ConnectionId.Equals(gamerConnectionId));
        }

        /// <summary>
        /// отправить сообщение группе
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task SendGamerMsg<T>(string cmd, T payload)
        {
            await HubContext.Clients.Group(HubGroupName).SendAsync(cmd, payload);
        }

        /// <summary>
        /// создание игры
        /// </summary>
        private async void InitGameHub()
        {
            foreach (var gamer in Gamers)
            {
                await HubContext.Groups.AddToGroupAsync(gamer.ConnectionId, HubGroupName);
            }
            await SendGamerMsg("StartGame", new InitGameDto()
            {
                GameUid = HubGroupName,
                AllGamerIds = Gamers.Select(g => g.ConnectionId).ToArray(),
                ShootGamerId = GetShooterGamer().ConnectionId
            });
        }


        /// <summary>
        /// получить другого игрока
        /// </summary>
        /// <param name="shootGamerConnectionId"></param>
        /// <returns></returns>
        private IGamer GetOtherGamer(string shootGamerConnectionId)
        {
            return Gamers.Single(g => !g.ConnectionId.Equals(shootGamerConnectionId));
        }

        /// <summary>
        /// получить игрока по Id
        /// </summary>
        private IGamer GetGamerById(string gamerId)
        {
            var gamer = Gamers.Single(g => g.ConnectionId.Equals(gamerId));
            return gamer;
        }

        /// <summary>
        /// стреляющий игрок
        /// </summary>
        /// <returns></returns>
        private IGamer GetShooterGamer()
        {
            return Gamers.Single(g => g.IsShooted);
        }
    }

}
