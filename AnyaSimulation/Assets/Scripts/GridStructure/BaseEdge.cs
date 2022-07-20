namespace Anya_2d
{
    public abstract class BaseEdge
    {
        public long id = -1;

        ///<summary>
        ///Vértice de início da aresta
        ///</summary>
        public BaseVertex start;
        ///<summary>
        ///Vértice do fim da aresta
        ///</summary>
        public BaseVertex end;
        public double weight;
        public double otherCost = 0;
        public double zoneCost = 0;

        public BaseEdge(long id, BaseVertex start, BaseVertex end, double weight)
        {

            this.id = id;
            this.start = start;
            this.end = end;
            this.weight = weight;
        }

        ///<summary>
        ///Verifica se o id de duas arestas são iguais
        ///</summary>
        public bool Equals(BaseEdge o)
        {
            return id == o.id;
        }

        ///<summary>
        ///Retorna o comprimento da aresta
        ///</summary>
        public abstract double GetLength();


        ///<summary>
        ///Retorna o custo de atravessar a aresta
        ///</summary>
        public double GetEdgeWeight()
        {
            return weight;
        }

        ///<summary>
        ///Compara o id de duas arestas
        ///</summary>
        public int CompareTo(BaseEdge o)
        {
            return id.CompareTo(o.id);
        }
    }
}
