
namespace Anya_2d
{
    public class GridPosition
    {
        int X;
        int Y;

        public GridPosition()
        {
            this.X = 0;
            this.Y = 0;
        }

        public GridPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int GetX()
        {
            return X;
        }

        public void SetX(int x)
        {
            this.X = x;
        }

        public int GetY()
        {
            return Y;
        }

        public void SetY(int y)
        {
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !typeof(GridPosition).IsInstanceOfType(obj))
            {
                return false;
            }
            GridPosition other = (GridPosition)obj;
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return "[" + X + "," + Y + "]";
        }

        public override int GetHashCode()
        {
            int result = X;
            result = 31 * result + Y;
            return result;
        }
    }
}

