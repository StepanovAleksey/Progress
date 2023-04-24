namespace GameSession.Models
{
    /// <summary>
    /// статусы игрока
    /// </summary>
    public enum EGamerStatus
    {
        /// <summary>
        /// обрыв связи
        /// </summary>
        Disconnectd = -10,

        /// <summary>
        /// неизвестный
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// игрок стреляет
        /// </summary>
        Shooted = 10,

        /// <summary>
        /// игрок ожидает
        /// </summary>
        Wait = 20,

        /// <summary>
        /// игрок выиграл
        /// </summary>
        Winner = 30,

        /// <summary>
        /// игрок проиграл
        /// </summary>
        Loose = 40
    }
}
