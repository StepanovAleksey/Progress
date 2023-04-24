using System.Text;

namespace SignalRChat.Models
{
    public class Ship
    {
        public Point[] Points { get; init; }
        public Point Start { get; init; }
        public int Lenght { get; init; }
        public bool IsHorizontal { get; init; }
        public Ship(Point start, int lenght, bool isHorizontal)
        {
            (Start, Lenght, IsHorizontal) = (start, lenght, isHorizontal);
            //Генерируем массив координат
            Points = new Point[lenght];
            for (int i = 0; i < lenght; i++)
            {
                if (IsHorizontal)
                    Points[i] = new Point(start.X + i, start.Y);
                else
                    Points[i] = new Point(start.X, start.Y + i);
            }
        }
        public bool CheckIntersection(Ship ship)
        {          
            foreach (var point in Points)
            {   
                //область в 1 клетку вокруг точки
                Point[] areaOfPoint =
                        {
                        new Point(point.X-1,point.Y-1),
                        new Point(point.X-1,point.Y),
                        new Point(point.X-1,point.Y+1),
                        new Point(point.X,point.Y-1),
                        new Point(point.X,point.Y),
                        new Point(point.X,point.Y+1),
                        new Point(point.X+1,point.Y-1),
                        new Point(point.X+1,point.Y),
                        new Point(point.X+1,point.Y+1),
                        };
                //перебираем все точки проверяемого корабля на попадание в область
                foreach (var checkPoint in ship.Points)
                {
                    foreach (var area in areaOfPoint)
                    {
                        if (checkPoint.Equals(area)) return true;
                    }
                }
             }
            return false;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine($"Ship Lenght: {this.Lenght}, IsHorizontal: {IsHorizontal.ToString()}");
            foreach (var point in Points)
            {
                str.AppendLine($"X: {point.X}; Y: {point.Y} ");
            }
            str.AppendLine("");
            return str.ToString();
        }
    }
}
