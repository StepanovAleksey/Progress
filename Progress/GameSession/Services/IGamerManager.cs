using Contracts.DTO;
using GameSession.Hubs;
using GameSession.Models.Gamers;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace GameSession.Services
{
    public interface IGamerManager
    {


        /// <summary>
        /// регистрация готового игрока
        /// </summary>
        public void RegisterGamer(IGamer gamer);

        public void AddNewGame(params IGamer[] gamers);

        public Task EvolveShoot(string shootGamerConnectionId, CoordinateSimple coordinateShoot);

        /// <summary>
        /// удаляем игрока (обрыв связи, и т.д.)
        /// </summary>
        /// <param name="connectionId"></param>
        public void RemoveGamer(string connectionId);

        public void SetGamerEntityId(string connectionId, long entityId);

        /// <summary>
        /// Отправка сообщения на очередь в брокер
        /// </summary>
        /// <param name="gamers"></param>
        public void SendBrokerWinnerMsg(IEnumerable<IGamer> gamers);

        /// <summary>
        /// в цикле создаёт игры
        /// </summary>
        /// <param name="stateInfo"></param>
        public void CreateGames(Object stateInfo);

        public Gamer? GetRanadomGamer();
    }
}

