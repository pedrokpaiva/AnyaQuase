namespace Anya_2d
{
    /// <summary>
    /// Representa um caminho de nodos
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class Path<V>
    {
        private readonly double pathCost;
        private readonly V node;
        private readonly Path<V> next;
        private Path<V> prev;

<<<<<<< HEAD
=======
        /// <summary>
        /// Cria um caminho a partir de um nodo, o nodo em sequênciaa e um custo
        /// </summary>
        /// <param name="node"></param>
        /// <param name="next"></param>
        /// <param name="pathCost"></param>
>>>>>>> 74655071d545d51f763dfc0be900942a3e77e066
        public Path(V node, Path<V> next, double pathCost)
        {
            this.node = node;
            this.pathCost = pathCost;
            this.next = next;
            if (next != null)
            {
                next.prev = this;
            }
        }

<<<<<<< HEAD
=======
        /// <summary>
        /// Getter de pathCost
        /// </summary>
        /// <returns>custo do path</returns>
>>>>>>> 74655071d545d51f763dfc0be900942a3e77e066
        public double GetPathCost()
        {
            return pathCost;
        }
        /// <summary>
        /// Getter de next
        /// </summary>
        /// <returns>próximo nodo</returns>
        public Path<V> GetNext()
        {
            return next;
        }
        /// <summary>
        /// Getter de prev
        /// </summary>
        /// <returns>nodo anterior</returns>
        public Path<V> GetPrev()
        {
            return prev;
        }
<<<<<<< HEAD
=======
        /// <summary>
        /// Getter de node
        /// </summary>
        /// <returns>nodo</returns>
>>>>>>> 74655071d545d51f763dfc0be900942a3e77e066
        public V GetNode()
        {
            return node;
        }
    }
}
