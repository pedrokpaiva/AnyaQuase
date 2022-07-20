using System.Numerics;

namespace Anya_2d
{
    public class AnyaVertex : BaseVertex
    {
        public enum CellDirections { CD_LEFTDOWN, CD_LEFTUP, CD_RIGHTDOWN, CD_RIGHTUP };

        public enum VertexDirections { VD_LEFT, VD_RIGHT, VD_DOWN, VD_UP };

        ///<summary>
        /// Coordenadas do vértice
        ///</summary>
        public GridPosition gridPos;

        public override string ToString()
        {
            return "GV[" + gridPos + "]";
        }

        public AnyaVertex(long id, Vector2 pos, GridPosition gridPos) : base(id, pos)
        {
            this.gridPos = gridPos;
        }

    }
}
