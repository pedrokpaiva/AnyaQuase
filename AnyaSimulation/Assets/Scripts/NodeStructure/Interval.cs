using System;
using UnityEngine;

namespace Anya_2d
{
    public class Interval
    {
        public double left;
        public double right;
        public int row;

        //Serão implementadas depois que tivermos Graph.epsilon
        public bool discrete_left;
        public bool discrete_right;
        public bool left_is_root;
        public bool right_is_root;

        public Interval(double left, double right, int row)
        {
            Init(left, right, row);
        }

        ///<summary>
        ///Inicializa um intervalo
        ///</summary>
        public void Init(double left, double right, int row)
        {
            SetLeft(left);
            SetRight(right);
            SetRow(row);
        }

        ///<summary>
        ///Variável que determina a diferença máxima entre dois pontos para serem considerados iguais
        ///</summary>
        public static readonly double DOUBLE_INEQUALITY_THRESHOLD = 0.0000001;

        public override bool Equals(object obj)
        {
            if (obj == null || !typeof(Interval).IsInstanceOfType(obj))
            {
                return false;
            }
            Interval p = (Interval)obj;
            return Math.Abs(p.left - left) < DOUBLE_INEQUALITY_THRESHOLD && Math.Abs(p.right - right) < DOUBLE_INEQUALITY_THRESHOLD && p.row == row;
        }

        ///<summary>
        ///Verifica se o intervalo do parâmetro está inserido no intervalo que o chamou
        ///</summary>
        public bool Covers(Interval i)
        {
            if (Math.Abs(i.left - left) < DOUBLE_INEQUALITY_THRESHOLD && Math.Abs(i.right - right) < DOUBLE_INEQUALITY_THRESHOLD && i.row == row)
            {
                return true;
            }

            return left <= i.left && right >= i.right && row == i.row;

        }
        ///<summary>
        ///Verifica se o ponto está contido no intervalo
        ///</summary>
        public bool Contains(Vector2 p)
        {
            throw new NotImplementedException();
            /*return ((int)p.Y) == row &&
                    (p.X + GridGraph.epsilon) >= left &&
                    p.X <= (right + GridGraph.epsilon);*/
        }


        public override int GetHashCode()
        {

            int result;
            long temp;
            temp = BitConverter.DoubleToInt64Bits(left);
            result = (int)((ulong)(temp ^ temp) >> 32);
            temp = BitConverter.DoubleToInt64Bits(right);
            result = 31 * result + (int)((ulong)(temp ^ temp) >> 32);
            result = 31 * result + row;
            return result;

        }

        public double GetLeft()
        {
            return left;
        }

        public void SetLeft(double left)
        {
            this.left = left;
            throw new NotImplementedException();
            /*discrete_left = Math.Abs((int)(left + GridGraph.epsilon) - left) <
                    GridGraph.epsilon;
            if (discrete_left)
            {
                this.left = (int)(this.left + GridGraph.epsilon);
            }*/
        }

        public double GetRight()
        {
            return right;
        }

        public void SetRight(double right)
        {
            this.right = right;
            throw new NotImplementedException();
            /*discrete_right = Math.Abs((int)(right + GridGraph.epsilon) - right) <
                    GridGraph.epsilon;
            if (discrete_right)
            {
                this.right = (int)(this.right + GridGraph.epsilon);
            }*/
        }

        public int GetRow()
        {
            return row;
        }

        public void SetRow(int row)
        {
            this.row = row;
        }

        public double RangeSize()
        {
            return right - left;
        }

        public override string ToString()
        {
            return "Interval (" + left + ", " + right + ", " + row + ")";
        }
    }
}
