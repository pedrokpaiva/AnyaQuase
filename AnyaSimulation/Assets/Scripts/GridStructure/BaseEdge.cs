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

        /// <summary>
        /// Cria uma aresta a partir de uma aresta, vértices de início e fim e um custo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="weight"></param>
        public BaseEdge(long id, BaseVertex start, BaseVertex end, double weight)
        {

            this.id = id;
            this.start = start;
            this.end = end;
            this.weight = weight;
        }

        /// <summary>
        /// Verifica se duas arestas são iguais baseado no id
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool Equals(BaseEdge o)
        {
            return id == o.id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>comprimento da aresta</returns>
        public abstract double GetLength();


        /// <summary>
        /// Getter de weight
        /// </summary>
        /// <returns>custo</returns>
        public double GetEdgeWeight()
        {
            return weight;
        }

        /// <summary>
        /// Compara os ids de duas arestas
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(BaseEdge o)
        {
            return id.CompareTo(o.id);
        }
    }
}
