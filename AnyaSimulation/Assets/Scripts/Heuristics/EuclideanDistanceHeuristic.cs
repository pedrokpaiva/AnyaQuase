using System;

namespace Anya_2d
{
    /// <summary>
    /// Heuristica de distância euclideana
    /// </summary>
    public class EuclideanDistanceHeuristic : IHeuristic<BaseVertex>
    {

        public double GetValue(BaseVertex n)
        {
            return 0;
        }

        public double GetValue(BaseVertex n, BaseVertex t)
        {
            if (n == null || t == null) { return 0; }
            return H(n.pos.X, n.pos.Y, t.pos.X, t.pos.Y);
        }

        /// <summary>
        /// Calcula a distância euclidenana entre dois pontos no espaço 2D
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public double H(double x1, double y1, double x2, double y2)
        {
            double delta_x = x1 - x2;
            double delta_y = y1 - y2;
            return Math.Sqrt(delta_x * delta_x + delta_y * delta_y);
        }
    }
}
