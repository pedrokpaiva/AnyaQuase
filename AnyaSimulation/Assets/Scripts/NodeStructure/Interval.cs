using System;
using UnityEngine;

namespace Anya_2d
{
    /// <summary>
    /// Um intervalo horizontal entre 2 pontos em 2D
    /// </summary>
    public class Interval
    {
        public double left;
        public double right;
        public int row;

        public bool discrete_left;
        public bool discrete_right;
        public bool left_is_root;
        public bool right_is_root;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left">Coordenada X mais a esquerda</param>
        /// <param name="right">Coordenada X mais a direita</param>
        /// <param name="row">Coordenada Y dos dois pontos</param>
        public Interval(double left, double right, int row)
        {
            Init(left, right, row);
        }

        ///<summary>
        ///Inicializa um intervalo a partir de duas coordenadas X e uma Y
        ///</summary>
        public void Init(double left, double right, int row)
        {
            SetLeft(left);
            SetRight(right);
            SetRow(row);
        }

        ///<summary>
        ///Determina a diferença máxima entre dois pontos para serem considerados iguais
        ///</summary>
        public static readonly double DOUBLE_INEQUALITY_THRESHOLD = 0.0000001;

        /// <summary>
        /// Verifica se dois intervalos são iguais
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !typeof(Interval).IsInstanceOfType(obj))
            {
                return false;
            }
            Interval p = (Interval)obj;
            return Math.Abs(p.left - left) < DOUBLE_INEQUALITY_THRESHOLD && Math.Abs(p.right - right) < DOUBLE_INEQUALITY_THRESHOLD && p.row == row;
        }

        /// <summary>
        /// Verifica se o intervalo i está contido nesse
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool Covers(Interval i)
        {
            if (Math.Abs(i.left - left) < DOUBLE_INEQUALITY_THRESHOLD && Math.Abs(i.right - right) < DOUBLE_INEQUALITY_THRESHOLD && i.row == row)
            {
                return true;
            }

            return left <= i.left && right >= i.right && row == i.row;

        }

        /// <summary>
        /// Verifica se o intervalo contem o ponto p
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Contains(Vector2 p)
        {
            return ((int)p.y) == row &&
                    (p.x + GridGraph.epsilon) >= left &&
                    p.x <= (right + GridGraph.epsilon);
        }

        /// <summary>
        /// Gera um código para uma tabela Hash
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Getter de left
        /// </summary>
        /// <returns>Coordenada X mais a esquerda</returns>
        public double GetLeft()
        {
            return left;
        }

        /// <summary>
        /// Setter de left
        /// </summary>
        /// <param name="left"></param>
        public void SetLeft(double left)
        {
            this.left = left;
            discrete_left = Math.Abs((int)(left + GridGraph.epsilon) - left) <
                    GridGraph.epsilon;
            if (discrete_left)
            {
                this.left = (int)(this.left + GridGraph.epsilon);
            }
        }

        // <summary>
        /// Getter de right
        /// </summary>
        /// <returns>Coordenada X mais a direita</returns>
        public double GetRight()
        {
            return right;
        }

        /// <summary>
        /// Setter de right
        /// </summary>
        /// <param name="right"></param>
        public void SetRight(double right)
        {
            this.right = right;
            discrete_right = Math.Abs((int)(right + GridGraph.epsilon) - right) <
                    GridGraph.epsilon;
            if (discrete_right)
            {
                this.right = (int)(this.right + GridGraph.epsilon);
            }
        }

        /// <summary>
        /// Getter de row
        /// </summary>
        /// <returns>Coordenada Y do intervalo</returns>
        public int GetRow()
        {
            return row;
        }

        /// <summary>
        /// Setter de row
        /// </summary>
        /// <param name="row"></param>
        public void SetRow(int row)
        {
            this.row = row;
        }

        /// <summary>
        /// Distancia entre ponto mais a direita e ponto mais a esquerda
        /// </summary>
        /// <returns></returns>
        public double RangeSize()
        {
            return right - left;
        }

        /// <summary>
        /// Conversão de um intervalo para uma string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Interval (" + left + ", " + right + ", " + row + ")";
        }
    }
}
