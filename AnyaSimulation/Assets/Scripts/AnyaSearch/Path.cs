namespace Anya_2d
{
    public class Path<V>
    {
        private readonly double pathCost;
        private readonly V vertex;
        private readonly Path<V>? next;
        private Path<V>? prev;

        public Path(V vertex, Path<V> next, double pathCost)
        {
            this.vertex = vertex;
            this.pathCost = pathCost;
            this.next = next;
            if (next != null)
            {
                next.prev = this;
            }
        }

        public double getPathCost()
        {
            return pathCost;
        }
        public Path<V>? GetNext()
        {
            return next;
        }
        public Path<V>? GetPrev()
        {
            return prev;
        }
        public V GetVertex()
        {
            return vertex;
        }
    }
}
