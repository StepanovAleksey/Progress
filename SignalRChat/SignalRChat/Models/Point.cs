namespace SignalRChat.Models
{
    public class Point
    {
        public int X { get; init; }
        public int Y { get; init; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode();

        protected static bool EqualsHelper(Point first, Point second) =>
            first.X == second.X &&
            first.Y == second.Y;

        public override bool Equals(object obj)
        {
            if ((object)this == obj)
                return true;

            var other = obj as Point;

            if ((object)other == null)
                return false;

            return EqualsHelper(this, other);
        }
        public static Point Generate()
        {
            return new Point(new Random().Next(0, 9), new Random().Next(0, 9));
        }
    }
}
