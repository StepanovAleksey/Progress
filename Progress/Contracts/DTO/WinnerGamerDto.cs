namespace Contracts.DTO
{
    /// <summary>
    /// игроки могут быть не авторизованны поэтому их ID мы можем не знать.
    /// На уровне сервиса GameSession идёт проверка, если все игроки анонимны то сообщение не отправиться
    /// </summary>
    public class WinnerGamerDto
    {
        /// <summary>
        /// ID игрока который победил
        /// </summary>
        public long? WinnerGamerId { get; set; }

        /// <summary>
        /// ID игрока который проиграл
        /// </summary>
        public long? LossGamerId { get; set; }
    }
}
