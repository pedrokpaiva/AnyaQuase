
namespace Anya_2d
{
    /// <summary>
    /// Posição numa grid 2D
    /// </summary>
    public class GridPosition
    {
        private int x;
        private int y;

        /// <summary>
        /// Cria uma posição na grid padrão x = y = 0
        /// </summary>
        public GridPosition()
        {
            x = 0;
            y = 0;
        }

        /// <summary>
        /// Cria uma posição na grid a partir de uma coordenada x e uma y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public GridPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Getter de x
        /// </summary>
        /// <returns>x</returns>
        public int GetX()
        {
            return x;
        }

        /// <summary>
        /// Setter de x
        /// </summary>
        /// <param name="x"></param>
        public void SetX(int x)
        {
            this.x = x;
        }

        /// <summary>
        /// Getter de y
        /// </summary>
        /// <returns>y</returns>
        public int GetY()
        {
            return y;
        }

        /// <summary>
        /// Setter de y
        /// </summary>
        /// <param name="y"></param>
        public void SetY(int y)
        {
            this.y = y;
        }

        /// <summary>
        /// Verifica se duas posições são iguais
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !typeof(GridPosition).IsInstanceOfType(obj))
            {
                return false;
            }
            GridPosition other = (GridPosition)obj;
            return x == other.x && y == other.y;
        }

        /// <summary>
        /// Converte a posição na grid em string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + x + "," + y + "]";
        }

        /// <summary>
        /// Gera um código para uma tabela Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int result = x;
            result = 31 * result + y;
            return result;
        }
    }
}

