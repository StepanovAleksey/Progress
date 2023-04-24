using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLibrary.RabbitMQ.DTO
{
    /// <summary>
    /// Статус игрока On - вошел на старницу но не нажал на кнопку, Ready - нажал на кнопку и ожидает соперника, Play - играет
    /// </summary>
    public enum UserStatus { On, Ready, Play }
    /// <summary>
    /// Класс для передачи в запросе GameSession
    /// </summary>
    public class ChatUserDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? GroupName { get; set; } = null;
        public UserStatus Status { get; set; } = UserStatus.On;
    }
}
