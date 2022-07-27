using Anya_2d;

public class SearchNode : FibonacciHeapNode<Node>
{

    /// <summary>
    /// Verifica se o nod ofoi adicionado a lista open
    /// </summary>
    public int search_id;

    /// <summary>
    /// Verifica se o nodo foi expandido
    /// </summary>
    public bool closed;

    /// <summary>
    /// Cria um SearchNode com base em um nodo e seu id_search
    /// </summary>
    /// <param name="vertex"></param>
    /// <param name="search_id_counter"></param>
    public SearchNode(Node vertex, int search_id_counter) : base(vertex)
    {
        data = vertex;
        ResetNode(search_id_counter);
        search_id = -1;
    }

    public void ResetNode(int search_id_counter)
    {
        base.ResetNode();
        search_id = search_id_counter;
        closed = false;

    }

    /// <summary>
    /// Converte o SearchNode em string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "searchnode " + GetData().GetHashCode() + ";" + GetData().ToString();
    }
}
