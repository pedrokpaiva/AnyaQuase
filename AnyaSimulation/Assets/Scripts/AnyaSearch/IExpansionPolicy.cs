namespace Anya_2d
{
    public interface IExpansionPolicy<V>
    {
        /// <summary>
        /// return true if the start and target are valid location
        /// and false otherwise. this method is intended to be invoked
        /// once; at the commencement of search.
        /// </summary>
        public bool Validate_instance(V start, V target);

        /// <summary>
        /// generate all the immediate neighbours of a node 
        /// </summary>
        public void Expand(V vertex);
        /// <summary>
        /// return the next neighbour of the node currently being
        /// expanded; and null if there are no neighbours or if all
        /// neighbours have been exhausted
        /// </summary>
        public V Next();

        /// <summary>
        /// return true until all neighbours have been iterated over
        /// </summary>
        public bool HasNext();

        /// <summary>
        /// return the distance (g-value) from the node being expanded
        /// to the current neighbour
        /// </summary>
        public double Step_cost();

        /// <summary>
        /// A heuristic for evaluating cost-to-go
        /// </summary>
        int GetHashCode(V v);
    }
}