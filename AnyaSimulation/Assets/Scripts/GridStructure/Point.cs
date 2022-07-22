namespace Anya_2d
{
    /// <summary>
    /// Um ponto 2D
    /// </summary>
    public class Point
    {
        public int x;
        public int y;

        /// <summary>
        /// Cria um ponto 2D com duas coordenadas
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// Verifica se dois pontos são iguais
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !typeof(Point).IsInstanceOfType(obj))
            {
                return false;
            }
            Point other = (Point)obj;
            return x == other.x && y == other.y;
        }

        /// <summary>
        /// Conversão de um ponto para uma string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }

        /// <summary>
        /// Gera um código para uma tabela Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + x;
            result = prime * result + y;
            return result;
        }
    }
}

