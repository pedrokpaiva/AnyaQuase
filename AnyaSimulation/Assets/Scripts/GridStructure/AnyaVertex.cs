using System.Numerics;

namespace Anya_2d
{
    /// <summary>
    /// Vértice em uma grid
    /// </summary>
    public class AnyaVertex : BaseVertex
    {
        public enum CellDirections { CD_LEFTDOWN, CD_LEFTUP, CD_RIGHTDOWN, CD_RIGHTUP };

        public enum VertexDirections { VD_LEFT, VD_RIGHT, VD_DOWN, VD_UP };

        ///<summary>
        /// Coordenadas do vértice na grid
        ///</summary>
        public GridPosition gridPos;

        /// <summary>
        /// Cria um vértice a partir de um id, um ponto 2D e uma posição na grid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="gridPos"></param>
        public AnyaVertex(long id, Vector2 pos, GridPosition gridPos) : base(id, pos)
        {
            this.gridPos = gridPos;
        }

        /// <summary>
        /// Converte a posição do vértice na grid em string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "GV[" + gridPos + "]";
        }

    }
}
