namespace Contracts.Enums
{
    /// <summary>
    /// Статус корабля
    /// </summary>
    public enum EShipStatus
    {
        Unknown = 0,

        /// <summary>
        /// Здоровый
        /// </summary>
        Healthy = 1,

        /// <summary>
        /// Раненый
        /// </summary>
        Wounded = 2,

        /// <summary>
        /// Убитый
        /// </summary>
        Killing = 3,
    }
}
