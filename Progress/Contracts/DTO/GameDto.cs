using Contracts.Enums;

namespace Contracts.DTO
{
    /// <summary>
    /// базовый класс для ДТО моделей по игре
    /// </summary>
    public abstract class AGameDto
    {

        /// <summary>
        /// идентификатор игры
        /// </summary>
        public string GameUid { get; set; }

    }

    /// <summary>
    /// базовый класс для ДТО моделей по обработке выстрела
    /// </summary>
    public abstract class AGameShootDto : AGameDto
    {

        /// <summary>
        /// ID игрока который стерлял
        /// </summary>
        public string SourceGamerConnectionId { get; set; }

        /// <summary>
        /// ID игрока по кому стерляли
        /// </summary>
        public string TargetGamerConnectionId { get; set; }
    }

    /// <summary>
    /// Результаты вытсрела
    /// </summary>
    public class ShootResultDto : AGameShootDto
    {

        /// <summary>
        /// статус выстрела
        /// </summary>
        public EShootStatus ShootStatus { get; set; }

        /// <summary>
        /// ID игрока кто делает следующий ход
        /// </summary>
        public string NextGamerShooterConnectionId { get; set; }

        /// <summary>
        /// координаты выстрела
        /// </summary>
        public CoordinateSimple Coordinate { get; set; }
    }

    /// <summary>
    /// корабль убит
    /// </summary>
    public class KillingShipDto : AGameShootDto
    {
        /// <summary>
        /// координаты корабля который убили
        /// </summary>
        public CoordinateSimple[] Coordinates { get; set; }
    }

    /// <summary>
    /// Инициализация игры
    /// </summary>
    public class InitGameDto : AGameDto
    {
        /// <summary>
        /// ID всех игроков
        /// </summary>
        public string[] AllGamerIds { get; set; }

        /// <summary>
        ///  ID игрока чей выстрел
        /// </summary>
        public string ShootGamerId { get; set; }
    }


    /// <summary>
    /// Завершение игры
    /// </summary>
    public class EndGameDto : AGameDto
    {
        /// <summary>
        /// ID игрока который виграл
        /// </summary>
        public string WinnerGamerId { get; set; }
    }
}
