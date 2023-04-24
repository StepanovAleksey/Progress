using Contracts.DTO;
using Contracts.Enums;
using System.Reactive.Subjects;

namespace GameSession.Models.Gamers
{
    public interface IGamer
    {

        /// <summary>
        /// игрок стреляет
        /// </summary>
        public bool IsShooted { get; }

        /// <summary>
        /// Ид соединения,он же и главный идентификатор
        /// </summary>
        public string ConnectionId { get; set; }

        public long? UserEntityId { get; set; }

        /// <summary>
        /// обрыв соединения, передаётся ConnectionId
        /// </summary>
        public Subject<string> DisconnectedSub { get; set; }

        /// <summary>
        /// обработка выстрела по игроку
        /// </summary>
        /// <param name="coordinateShoot"></param>
        /// <returns></returns>
        public (EShootStatus, ShipDto?) EvolveShoot(CoordinateSimple coordinateShoot);

        public void ContinueShoot();

        /// <summary>
        /// добавить выстрел в историю 
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public IGamer AddHistory(CoordinateSimple history);

        /// <summary>
        /// переключить статус выстрела
        /// </summary>
        public void SwitchShoot();

        public void SetDisconnected();

        public bool EqualsConnectionId(string connectionId);

        public void ChangeStatus(EGamerStatus newStatus);

        public bool IsWinner();

        public void SetUserEntityId(long id);


    }
}
