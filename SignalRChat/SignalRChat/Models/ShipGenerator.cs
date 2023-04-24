using System;
using System.Text;

namespace SignalRChat.Models
{
    public class ShipGenerator
    {
        private Tuple<bool, int>[,] _shipField;
        private Ship[]? _ships;
        public Ship[]? Generate()
        {
            //генерируем массив из 10 кораблей
            _ships = new Ship[10];
            //генерируем первый корабль
            var temp = OneShipGenerator(1);
            //записываем его в массив
            _ships[0] = temp;
            //генерируем остальные 9 кораблей
            for (int i = 1; i < 10; i++)
            {   
                //определяем длинну корабля в зависимсти от его номера
                var shipLenght = 1;
                if ((i > 3) && (i < 7)) shipLenght = 2;
                else if ((i > 6) && (i < 9)) shipLenght = 3;
                else if (i==9) shipLenght = 4;
                //геннерируем следующий корабль
                temp = OneShipGenerator(shipLenght);
                //проверяем его на пересечение с сгенерированными кораблями
                for (var j = 0; j < i; j++)
                {   
                    //если есть пересечение то генерируем новый и сбрасываем счётчик проверяемых кораблей в 0
                    while (_ships[j].CheckIntersection(temp))
                    { 
                        temp = OneShipGenerator(shipLenght);
                        j = 0;
                    }
                }
                _ships[i] = temp;
            }
             _shipField = GetField();
            return _ships;//_ships_shipField;
        }

        /// <summary>
        /// Преобразуем корабли в массив булевы значений игрового поля
        /// </summary>
        /// <returns>массив значений игрового поля</returns>
        private Tuple<bool,int>[,] GetField()
        {
            var field = new Tuple<bool, int>[10, 10];
            for(int i = 0; i< _ships.Length; i++)
           // foreach (var ship in _ships)
            {
                if (_ships[i] != null)
                {
                    foreach (var point in _ships[i].Points)
                        field[point.X, point.Y] = new Tuple<bool, int> (true, i);
                }
            }
            return field;
        }

        /// <summary>
        /// Генерируем один корабль определенной длины
        /// </summary>
        /// <param name="lenght">длина</param>
        /// <returns></returns>
        private Ship OneShipGenerator(int lenght)
        {
            Point start = Point.Generate();
            bool isHorizontal = RandomBoolGenerator.Generate();
            while (!ShipLenghtChecker(start, lenght, isHorizontal))
                {
                start = Point.Generate();
                }
            return new Ship(start,lenght, isHorizontal);
        }

        /// <summary>
        /// Проверяем выход корабля за границы игрового поля
        /// </summary>
        /// <param name="start">точка начала корабля</param>
        /// <param name="lenght">его длина в палубах</param>
        /// <param name="isHorizontal">горизонтальный он или вертикальный</param>
        /// <returns></returns>
        private bool ShipLenghtChecker(Point start, int lenght, bool isHorizontal)
        {
            if((isHorizontal) && ((start.X+lenght-1)>9)) return false;
            else if ((start.Y + lenght - 1) > 9) return false;
            return true;
        }
    }

}
