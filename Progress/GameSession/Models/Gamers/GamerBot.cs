using System.Reactive.Subjects;
using Contracts.DTO;
using Contracts.Enums;

namespace GameSession.Models.Gamers
{
    public class GamerBot : IGamer
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();

        private IList<CoordinateSimple> HistoryShoot = new List<CoordinateSimple>();

        /// <summary>
        /// ID игрока в БД
        /// </summary>
        public long? UserEntityId { get; set; } = null;

        /// <summary>
        /// Ид соединения,он же и главный идентификатор
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// обрыв соединения, передаётся ConnectionId
        /// </summary>
        public Subject<string> DisconnectedSub { get; set; } = new Subject<string>();

        private EGamerStatus Status { get; set; } = EGamerStatus.Unknown;

        private CoordinateSimple previousShootCoordinateSimple = new CoordinateSimple();

        private IList<CoordinateSimple> listPreviousShootCoordinateSimple = new List<CoordinateSimple>();

        /// <summary>
        /// игрок стреляет
        /// </summary>
        public bool IsShooted
        {
            get
            {
                return Status == EGamerStatus.Shooted;
            }
        }

        /// <summary>
        ///  С игроком потеряна связь
        /// </summary>
        public bool IsDisconnected
        {
            get
            {
                return Status == EGamerStatus.Disconnectd;
            }
        }

        private readonly IEnumerable<ShipDto> Ships;

        public delegate Task ShootBot(string shootGamerConnectionId, CoordinateSimple coordinateShoot);
        public event ShootBot ShootBotEvent;

        public GamerBot(IEnumerable<ShipDto> ships)
        {
            ConnectionId = "BOT_" + new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
            Ships = ships;

            Ships = GenerateShips();
        }

        public static IEnumerable<ShipDto> GenerateShips()
        {
            var random = new Random();
            var ships = new List<ShipDto>();

            // генерация 4-х палубного корабля
            var ship = new ShipDto();
            var shipCoordinates = new List<CoordinateSimple>();
            var x = random.Next(0, 7);
            var y = random.Next(0, 10);
            for (var i = 0; i < 4; i++)
            {
                var coordinate = new CoordinateSimple { X = x + i, Y = y };
                shipCoordinates.Add(coordinate);
            }
            ship.Coordinates = shipCoordinates;
            ships.Add(ship);

            // генерация двух трехпалубных кораблей
            for (var j = 0; j < 2; j++)
            {
                ship = new ShipDto();
                shipCoordinates = new List<CoordinateSimple>();
                var isIntersect = true;
                while (isIntersect)
                {
                    x = random.Next(0, 8);
                    y = random.Next(0, 10);
                    for (var i = 0; i < 3; i++)
                    {
                        var coordinate = new CoordinateSimple { X = x + i, Y = y };
                        if (ships.Any(s => s.Coordinates.Any(c => Math.Abs(c.X - coordinate.X) <= 1 && Math.Abs(c.Y - coordinate.Y) <= 1)))
                        {
                            isIntersect = true;
                            break;
                        }
                        shipCoordinates.Add(coordinate);
                        isIntersect = false;
                    }
                }
                ship.Coordinates = shipCoordinates;
                ships.Add(ship);
            }

            // генерация трех двухпалубных кораблей
            for (var j = 0; j < 3; j++)
            {
                ship = new ShipDto();
                shipCoordinates = new List<CoordinateSimple>();
                var isIntersect = true;
                while (isIntersect)
                {
                    x = random.Next(0, 9);
                    y = random.Next(0, 10);
                    for (var i = 0; i < 2; i++)
                    {
                        var coordinate = new CoordinateSimple { X = x + i, Y = y };
                        if (ships.Any(s => s.Coordinates.Any(c => Math.Abs(c.X - coordinate.X) <= 1 && Math.Abs(c.Y - coordinate.Y) <= 1)))
                        {
                            isIntersect = true;
                            break;
                        }
                        shipCoordinates.Add(coordinate);
                        isIntersect = false;
                    }
                }
                ship.Coordinates = shipCoordinates;
                ships.Add(ship);
            }

            // генерация четырех однопалубных кораблей
            for (var j = 0; j < 4; j++)
            {
                ship = new ShipDto();
                shipCoordinates = new List<CoordinateSimple>();
                var isIntersect = true;
                while (isIntersect)
                {
                    x = random.Next(0, 10);
                    y = random.Next(0, 10);
                    var coordinate = new CoordinateSimple { X = x, Y = y };
                    if (ships.Any(s => s.Coordinates.Any(c => Math.Abs(c.X - coordinate.X) <= 1 && Math.Abs(c.Y - coordinate.Y) <= 1)))
                    {
                        isIntersect = true;
                        continue;
                    }
                    shipCoordinates.Add(coordinate);
                    isIntersect = false;
                }
                ship.Coordinates = shipCoordinates;
                ships.Add(ship);
            }

            return ships;
        }

        /// <summary>
        /// обработка выстрела по игроку
        /// </summary>
        /// <param name="coordinateShoot"></param>
        /// <returns></returns>
        public (EShootStatus, ShipDto?) EvolveShoot(CoordinateSimple coordinateShoot)
        {
            var shootedShipsStatus = Ships.Select(s => s.ShootValidate(coordinateShoot)).ToList();
            if (Ships.All(s => s.IsKilling()))
            {
                return (EShootStatus.KillingAll, null);
            }

            var status = shootedShipsStatus.SingleOrDefault((status) => !status.Item1.IsMissing());
            return status;
        }

        public void ContinueShoot()
        {
            if (Status == EGamerStatus.Shooted)
            {
                CoordinateSimple newCoordinates = CalculateShootCoordinate(true);
                ShootBotEvent?.Invoke(this.ConnectionId, newCoordinates);
            }
        }

        /// <summary>
        /// добавить выстрел в историю 
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public IGamer AddHistory(CoordinateSimple history)
        {
            HistoryShoot.Add(history);
            return this;
        }

        /// <summary>
        /// переключить статус выстрела
        /// </summary>
        public void SwitchShoot()
        {
            ChangeStatus(IsShooted ? EGamerStatus.Wait : EGamerStatus.Shooted);
        }

        private CoordinateSimple CalculateShootCoordinate(bool isHitPrevious)
        {
            Random random = new Random();
            CoordinateSimple newCoordinates = new CoordinateSimple() { X = 5, Y = 5 };
            int X = 0;
            int Y = 0;

            if (isHitPrevious)
            {
                while (listPreviousShootCoordinateSimple
                           .Any(c => c.X == X && c.Y == Y))
                {
                    X = previousShootCoordinateSimple.X + random.Next(-2, 2);
                    Y = previousShootCoordinateSimple.Y + random.Next(-2, 2);
                }
            }
            else
            {
                while (listPreviousShootCoordinateSimple
                       .Any(c => c.X == X && c.Y == Y))
                {
                    X = random.Next(0, 10);
                    Y = random.Next(0, 10);
                }
            }
            newCoordinates = new CoordinateSimple()
            {
                X = X,
                Y = Y
            };

            listPreviousShootCoordinateSimple.Add(newCoordinates);
            previousShootCoordinateSimple = newCoordinates;

            return newCoordinates;
        }

        public bool EqualsConnectionId(string connectionId) { return ConnectionId.Equals(connectionId); }

        public void SetDisconnected()
        {
            ChangeStatus(EGamerStatus.Disconnectd);
            DisconnectedSub.OnNext(ConnectionId);
            DisconnectedSub.OnCompleted();
        }

        public void ChangeStatus(EGamerStatus newStatus)
        {
            Status = newStatus;

            if (Status == EGamerStatus.Shooted)
            {
                CoordinateSimple newCoordinates = CalculateShootCoordinate(false);
                ShootBotEvent?.Invoke(this.ConnectionId, newCoordinates);
            }
        }

        public bool IsWinner()
        {
            return Status == EGamerStatus.Winner;
        }

        public void SetUserEntityId(long id)
        {
            UserEntityId = id;
        }
    }
}
