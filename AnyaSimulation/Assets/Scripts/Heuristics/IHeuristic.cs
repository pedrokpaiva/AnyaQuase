namespace Anya_2d
{
    public interface IHeuristic<V>
    {
        /// <summary>
        /// Valor da heuristica de um nó isolado
        /// </summary>
        /// <param name="n"></param>
        /// <returns>geralmente 0</returns>
        public double GetValue(V n);

        /// <summary>
        /// Valor da heuristica entre dois nós
        /// </summary>
        /// <param name="n"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public double GetValue(V n, V t);
    }
}
