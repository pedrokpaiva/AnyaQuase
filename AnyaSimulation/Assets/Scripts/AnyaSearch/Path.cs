namespace Anya_2d
{
    public class Path<V>
    {
        private readonly double pathCost;
        private readonly V node;
        private readonly Path<V> next;
        private Path<V> prev;

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

        public double GetPathCost()
        {
            return pathCost;
        }
        public Path<V> GetNext()
        {
            return next;
        }
        public Path<V> GetPrev()
        {
            return prev;
        }
        public V GetNode()
        {
            return node;
        }
    }
}
