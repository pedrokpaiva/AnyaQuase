namespace Anya_2d
{
    public interface IExpansionPolicy<V>
    {
        /// <summary>
        /// Verifica se os nodos inicial e final sestão em posições válidas
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <returns>true se ambos os nodos estãp em posições válidas</returns>
        public bool Validate_instance(V start, V target);

        /// <summary>
        /// Gera todos os vizinhos imediatos de um nodo
        /// </summary>
        /// <param name="vertex"></param>
        public void Expand(V node);

        /// <summary>
        /// Retorna o próximo vizinho do nodo sendo expandido
        /// </summary>
        /// <returns></returns>
        public V Next();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true se ainda há vizinhos a serem iterados</returns>
        public bool HasNext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Retorna a distância (g-value) do nodo sendo expandido
        /// até o vizinho atual</returns>
        public double Step_cost();

        /// <summary>
        /// Heuristica utilizada para determinar o custo
        /// </summary>
        /// <returns></returns>
        IHeuristic<V> Heuristic();

        /// <summary>
        /// Gera um código Hash do nodo sendo expandido
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        int GetHashCode(V v);
    }
}