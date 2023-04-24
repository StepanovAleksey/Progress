using Contracts.Enums;

namespace Contracts.DTO
{
    public class ShipDto
    {
        public EShipStatus Status = EShipStatus.Healthy;

        public IEnumerable<CoordinateSimple> Coordinates { get; set; }

        public bool IsKilling()
        {
            return Status == EShipStatus.Killing;
        }

        public (EShootStatus, ShipDto) ShootValidate(CoordinateSimple coordinate)
        {
            var targetCoordinate = Coordinates.SingleOrDefault(c => c.Equals(coordinate));

            // попали мимо
            if (targetCoordinate == null)
            {
                return (EShootStatus.Miss, this);
            }

            // если повторный выстрел
            if (targetCoordinate.IsShooted)
            {
                return (EShootStatus.Error, this);
            }

            /// выставляем что попали
            targetCoordinate.IsShooted = true;

            // если убили корабль
            if (Coordinates.All(c => c.IsShooted))
            {
                Status = EShipStatus.Killing;
                return (EShootStatus.Killing, this);
            }
            Status = EShipStatus.Wounded;

            /// попали, но не убили
            return (EShootStatus.Hit, this);
        }
    }


}
