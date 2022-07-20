namespace Anya_2d
{
    public interface IHeuristic<V>
    {
        public double GetValue(V n);
        public double GetValue(V n, V t);
    }
}
