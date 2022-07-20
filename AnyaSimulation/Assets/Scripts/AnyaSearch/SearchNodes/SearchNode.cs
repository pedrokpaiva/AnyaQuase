using Anya_2d;

public class SearchNode : FibonacciHeapNode<Node>
{
    // parent node
    public SearchNode parent;

    // tracks if the node has been added to open
    public int search_id;

    // tracks if the node has been expanded
    public bool closed;

    public SearchNode(Node vertex, int search_id_counter) : base(vertex)
    {
        data = vertex;
        ResetNode(search_id_counter);
        search_id = -1;
    }

    public void ResetNode(int search_id_counter)
    {
        parent = null;
        search_id = search_id_counter;
        closed = false;

    }

    public override string ToString()
    {
        return "searchnode " + GetData().GetHashCode() + ";" + GetData().ToString();
    }
}
