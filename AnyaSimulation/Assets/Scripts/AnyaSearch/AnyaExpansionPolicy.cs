using Anya_2d;
using System.Collections.Generic;
using UnityEngine;

public class AnyaExpansionPolicy : IExpansionPolicy<Node>
{
    private GridGraph grid_;
    private AnyaHeuristic heuristic_;
    private EuclideanDistanceHeuristic euclidean_;

    private Node startNode;
    protected double target_x, target_y;
    private Node currentNode;           
    private Node currentNode_successor;
    private int index_currentNode_successor;      // esse index é o do sucessor atual (sendo testado) do nó atual 
    private List<Node> currentNode_list_successors = new List<Node>();

    // reduces branching by eliminating nodes that cannot have successors
    private bool prune_ = true;

    public AnyaExpansionPolicy(GridGraph grid)
    {
        grid_ = grid;
        currentNode_list_successors = new List<Node>(32);
        heuristic_ = new AnyaHeuristic();
        euclidean_ = new EuclideanDistanceHeuristic();
    }

    /// <summary>
    /// Inicializa as variáveis correspondentes aos nós inicial e final, e retorna verdadeiro caso 
    /// tanto o nó inicial quanto o destino sejam visíveis de algum outro ponto discreto no grid,
    /// ou seja, se eles não são adjacentes a 4 células bloqueadas.
    /// </summary>
    public bool Validate_instance(Node start, Node target)
    {
        this.startNode = start;
        this.target_x = target.root.x;
        this.target_y = target.root.y;

        int startX = (int)start.root.x;
        int startY = (int)start.root.y;
        int targetX = (int)target.root.x;
        int targetY = (int)target.root.y;

        bool startResult = grid_.Get_point_is_visible(startX, startY);
        bool targetResult = grid_.Get_point_is_visible(targetX, targetY);

        return startResult && targetResult;
    }

    /// <summary>
    /// Gera os sucessores do nó e atualiza as variáveis correspondentes.
    /// </summary>
    public void Expand(Node node)
    {
        currentNode = node;
        currentNode_successor = null;
        index_currentNode_successor = 0;
        currentNode_list_successors.Clear();

        if (node.Equals(startNode))
        {
            Generate_start_successors(currentNode, currentNode_list_successors);
        }
        else
        {
            Generate_successors(currentNode, currentNode_list_successors);
        }
    }

    /// <summary>
    /// Vai para o próximo sucessor a ser testado do nó atual, caso ainda contenha algum, senão retorna null.
    /// </summary>
    public Node Next()
    {
        currentNode_successor = null;
        if (index_currentNode_successor < currentNode_list_successors.Count)
        {
            currentNode_successor = currentNode_list_successors[index_currentNode_successor++];
        }
        return currentNode_successor;
    }

    /// <summary>
    /// Retorna verdadeiro caso o nó atual ainda contenha algum sucessor não testado, senão retorna falso.
    /// </summary>
    public bool HasNext()
    {
        return index_currentNode_successor < currentNode_list_successors.Count;
    }

    /// <summary>
    /// Retorna o custo para ir do nó atual para o seu atual sucessor sendo testado.
    /// (custo = distância euclidiana)
    /// </summary>
    public double Step_cost()
    {
        Debug.Assert(currentNode != null && currentNode_successor != null);
        double retval = euclidean_.H(currentNode.root.x, currentNode.root.y, currentNode_successor.root.x, currentNode_successor.root.y);
        return retval;
    }

    /// <summary>
    /// Retorna uma lista com os sucessores do nó passado, este que deve ser o nó inicial.
    /// </summary>
    protected void Generate_start_successors(Node node, List<Node> retval)
    {
        int rootx = (int)node.root.x;
        int rooty = (int)node.root.y;
        IntervalProjection projection = new IntervalProjection();
        
        bool startIsDoubleCorner = grid_.Get_point_is_double_corner((int)node.root.x, (int)node.root.y);
        if (!startIsDoubleCorner)
        {
            // gera a projeção flat pra esquerda do ponto inicial
            projection.Project(rootx, rootx, rooty, rootx + 1, rooty, grid_);  // usa uma fake root (rooty, rootx + 1)
            // gera os sucessores observáveis a partir dessa projeção
            Generate_observable_flat__(projection, rootx, rooty, node, retval);
        }
        // gera os observadores flat pra direita do ponto inicial
        projection.Project(rootx, rootx, rooty, rootx - 1, rooty, grid_);      // usa uma fake root (rooty, rootx - 1)
        // gera os sucessores observáveis a partir dessa projeção
        Generate_observable_flat__(projection, rootx, rooty, node, retval);


        // gera os sucessores observáveis na linha abaixo do ponto inicial
        int max_left = grid_.Scan_cells_left(rootx - 1, rooty);
        int max_right = grid_.Scan_cells_right(rootx, rooty);
        if (max_left != rootx && !startIsDoubleCorner)
        {
            Split_interval_make_successors(max_left, rootx, rooty + 1, rootx, rooty, rooty + 1, node, retval);
        }
        if (max_right != rootx)
        {
            Split_interval_make_successors(rootx, max_right, rooty + 1, rootx, rooty, rooty + 1, node, retval);
        }

        // gera os sucessores observáveis na linha acima do ponto inicial
        max_left = grid_.Scan_cells_left(rootx - 1, rooty - 1);
        max_right = grid_.Scan_cells_right(rootx, rooty - 1);
        if (max_left != rootx && !startIsDoubleCorner)
        {
            Split_interval_make_successors(max_left, rootx, rooty - 1, rootx, rooty, rooty - 2, node, retval);
        }
        if (max_right != rootx)
        {
            Split_interval_make_successors(rootx, max_right, rooty - 1, rootx, rooty, rooty - 2, node, retval);
        }
    }

    /// <summary>
    /// Gera a lista de sucessores observáveis a partir da projeção flat.
    /// </summary>
    private void Generate_observable_flat__(IntervalProjection projection, int rootx, int rooty, Node parent, List<Node> retval)
    {
        Debug.Assert(projection.row == rooty);
        if (!projection.valid) { return; }

        // Checa se a projeção contém o target
        bool goal_interval = Contains_target(projection.left, projection.right, projection.row);

        if (projection.intermediate && prune_ && !goal_interval)
        {
            // ignore intermediate nodes and project further along the row
            projection.Project(projection.left, projection.right, projection.row, rootx, rooty, grid_);
            // check if the projection contains the goal
            goal_interval = Contains_target(projection.left, projection.right, projection.row);
        }

        if (!projection.deadend || !prune_ || goal_interval)
        {
            retval.Add(new Node(parent, new Interval(projection.left, projection.right, projection.row), rootx, rooty));
        }
    }

    /// <summary>
    /// Gera a lista de sucessores do nó.
    /// </summary>
    protected void Generate_successors(Node node, List<Node> retval)
    {
        IntervalProjection projection = new IntervalProjection();

        if (node.root.y == node.interval.GetRow())    // se a linha do nó é igual a linha do intervalo
        {
            projection.Project(node, grid_);
            Flat_node_obs(node, retval, projection);
            projection.Project_f2c(node, grid_);
            Flat_node_nobs(node, retval, projection);
        }
        else
        {
            projection.Project(node, grid_);
            Cone_node_obs(node, retval, projection);
            Cone_node_nobs(node, retval, projection);
        }
    }

    private void Split_interval_make_successors(double max_left, double max_right, int irow, int rootx, int rooty, int sterile_check_row, Node parent, List<Node> retval)
    {
        if (max_left == max_right) { return; }

        double succ_left = max_right;
        double succ_right;
        int num_successors = retval.Count;
        bool target_node = Contains_target(max_left, max_right, irow);
        bool forced_succ = !prune_ || target_node;

        Node successor = null;
        do
        {
            succ_right = succ_left;   
            succ_left = grid_.Scan_left(succ_right, irow);
            if (forced_succ || !Sterile(succ_left, succ_right, sterile_check_row))
            {
                successor = new Node(parent, new Interval(succ_left, succ_right, irow), rootx, rooty);
                successor.interval.SetLeft(succ_left < max_left ? max_left : succ_left);
                retval.Add(successor);
            }
        }while ((succ_left != succ_right) && (succ_left > max_left));


        // TODO: recurse over every node (NB: intermediate check includes goal check) 
        // TODO: recurse until we start heading e.g. up instead of down (flat is ok)
        if (!forced_succ && retval.Count == (num_successors + 1) && Intermediate(successor.GetInterval(), rootx, rooty))
        {
            retval.RemoveAt(retval.Count - 1);
            // TODO: optimise this new call out?
            IntervalProjection proj = new IntervalProjection();
            proj.Project_cone(successor.interval.GetLeft(), successor.interval.GetRight(), successor.interval.GetRow(), rootx, rooty, grid_);
            if (proj.valid && proj.observable)
            {
                Split_interval_make_successors(proj.left, proj.right, proj.row, rootx, rooty, proj.sterile_check_row, parent, retval);
            }
        }
    }

    // return true if the non-discrete points of the interval 
    // [@param left, @param right] on @param row sit adjacent to
    // any obstacle cells.
    //
    // TODO: can we do this better/faster by keeping track of whether the
    // interval endpoints are open or closed? i.e. only generate
    // intervals with two closed endpoints and ignore the rest unless
    // they contain the start or goal. Should work because for every semi-open 
    // interval (a, b] we can create two new intervals (a, b) and [b, c]
    private bool Sterile(double left, double right, int row)
    {
        int discrete_right = (int)(right - GridGraph.epsilon);
        int discrete_left = (int)(left + GridGraph.epsilon);
        bool result = !((grid_.Get_cell_is_traversable(discrete_right, row) && grid_.Get_cell_is_traversable(discrete_left, row)));
        return result;
    }

    // return true if the interval [@param left, @param right] on
    // has no adjacent successors on @param row.
    // NB: This code is not inside IntervalProjection because the
    // area inside a projection needs to be split into individual 
    // intervals.
    private bool Intermediate(Interval interval, int rootx, int rooty)
    {
        // intermediate nodes have intervals that are not taut; i.e.
        // their endpoints are not adjacent to any location that cannot be 
        // directly observed from the root.

        double left = interval.GetLeft();
        double right = interval.GetRight();
        int row = interval.GetRow();

        int tmp_left = (int)left;
        int tmp_right = (int)right;
        bool discrete_left = interval.discrete_left;
        bool discrete_right = interval.discrete_right;

        bool rightroot = (tmp_right < rootx);  // verdadeiro se rootx > tmp_right, ou seja, se a raiz estiver pra direita do intervalo
        bool leftroot = (rootx < tmp_left);    // verdadeiro se tmp_left > rootx, ou seja, se a raiz estiver pra esquerda do intervalo
        bool right_turning_point; 
        bool left_turning_point;
        if (rooty < row)
        {
            left_turning_point = discrete_left && grid_.Get_point_is_corner(tmp_left, row) && (!grid_.Get_cell_is_traversable(tmp_left - 1, row - 1) || leftroot);
            right_turning_point = discrete_right && grid_.Get_point_is_corner(tmp_right, row) && (!grid_.Get_cell_is_traversable(tmp_right, row - 1) || rightroot);
        }
        else
        {
            left_turning_point = discrete_left && grid_.Get_point_is_corner(tmp_left, row) && (!grid_.Get_cell_is_traversable(tmp_left - 1, row) || leftroot);
            right_turning_point = discrete_right && grid_.Get_point_is_corner(tmp_right, row) && (!grid_.Get_cell_is_traversable(tmp_right, row) || rightroot);
        }

        return !(left_turning_point || right_turning_point);
    }

    /// <summary>
    /// Retorna verdadeiro caso o intervalo contenha o target, senão retorna falso.
    /// </summary>
    private bool Contains_target(double left, double right, int row)
    {
        return (row == target_y) && (target_x >= left - GridGraph.epsilon) && (target_x <= right + GridGraph.epsilon);
    }

    // TODO: assumes vertical move to the next row is always valid.
    // there is an inductive argument here: if the move is not valid
    // the node should have been pruned. check this is always true.
    protected void Cone_node_obs(Node node, List<Node> retval, IntervalProjection projection)
    {
        Debug.Assert(node.root.y != node.interval.GetRow());

        Vector2 root = node.root;
        Generate_observable_cone__(projection, (int)root.x, (int)root.y, node, retval);
    }

    private void Generate_observable_cone__(IntervalProjection projection, int rootx, int rooty, Node parent, List<Node> retval)
    {
        if (!(projection.valid && projection.observable)) { return; }
        Split_interval_make_successors(projection.left, projection.right, projection.row, rootx, rooty, projection.sterile_check_row, parent, retval);
    }

    // there are two kinds of non-observable successors
    // (i) conical successors that are adjacent to an observable projection
    // (ii) flat successors that are adjacent to the current interval
    // (iii) conical successors that are not ajdacent to to any observable
    // projection or the current interval (i.e the angle from the root
    // to the interval is too low to observe any point from the next row)
    // TODO: seems like too many branching statements in this function. consolidate?
    protected void Cone_node_nobs(Node node,
            List<Node> retval, IntervalProjection projection)
    {
        if (!projection.valid) { return; }

        double ileft = node.interval.GetLeft();
        double iright = node.interval.GetRight();
        int irow = node.interval.GetRow();

        // non-observable successor type (iii)
        if (!projection.observable)
        {
            if (node.root.x > iright && node.interval.discrete_right && grid_.Get_point_is_corner((int)iright, irow))
            {
                Split_interval_make_successors(projection.max_left, iright, projection.row, (int)iright, irow, projection.sterile_check_row, node, retval);
            }
            else if (node.root.x < ileft && node.interval.discrete_left && grid_.Get_point_is_corner((int)ileft, irow))
            {
                Split_interval_make_successors(ileft, projection.max_right, projection.row, (int)ileft, irow, projection.sterile_check_row, node, retval);
            }
            // non-observable successors to the left of the current interval
            if (node.interval.discrete_left && !grid_.Get_cell_is_traversable((int)ileft - 1, projection.type_iii_check_row) &&
                                                grid_.Get_cell_is_traversable((int)ileft - 1, projection.check_vis_row))
            {
                projection.Project_flat(ileft - grid_.smallest_step, ileft, (int)ileft, irow, grid_);
                Generate_observable_flat__(projection, (int)ileft, irow, node, retval);
            }
            // non-observable successors to the right of the current interval
            if (node.interval.discrete_right && !grid_.Get_cell_is_traversable((int)iright, projection.type_iii_check_row) &&
                                                 grid_.Get_cell_is_traversable((int)iright, projection.check_vis_row))
            {
                projection.Project_flat(iright, iright + grid_.smallest_step, (int)iright, irow, grid_); // NB: dummy root
                Generate_observable_flat__(projection, (int)iright, irow, node, retval);
            }
            return;
        }

        // non-observable successors type (i) and (ii)
        IntervalProjection flatprj = new IntervalProjection();
        int corner_row = irow - (int)((uint)((int)node.root.y - irow) >> 31);

        // non-observable successors to the left of the current interval
        if (node.interval.discrete_left && grid_.Get_point_is_corner((int)ileft, irow))
        {
            // flat successors from the interval row
            if (!grid_.Get_cell_is_traversable((int)(ileft - 1), corner_row))
            {
                flatprj.Project(ileft - GridGraph.epsilon, iright, irow, (int)ileft, irow, grid_);
                Generate_observable_flat__(flatprj, (int)ileft, irow, node, retval);
            }

            // conical successors from the projected row
            Split_interval_make_successors(projection.max_left, projection.left, projection.row, (int)ileft, irow, projection.sterile_check_row, node, retval);
        }

        // non-observable successors to the right of the current interval
        if (node.interval.discrete_right && grid_.Get_point_is_corner((int)iright, irow))
        {
            // flat successors from the interval row
            if (!grid_.Get_cell_is_traversable((int)(iright), corner_row))
            {
                flatprj.Project(ileft, iright + GridGraph.epsilon, irow, (int)ileft, irow, grid_);
                Generate_observable_flat__(flatprj, (int)iright, irow, node, retval);
            }

            // conical successors from the projected row
            Split_interval_make_successors(projection.right, projection.max_right, projection.row, (int)iright, irow, projection.sterile_check_row, node, retval);
        }
    }

    protected void Flat_node_obs(Node node, List<Node> retval, IntervalProjection projection)
    {
        Vector2 root = node.root;
        Generate_observable_flat__(projection, (int)root.x, (int)root.y, node, retval);
    }

    protected void Flat_node_nobs(Node node, List<Node> retval, IntervalProjection projection)
    {
        if (!projection.valid) { return; }
        // conical successors from the projected row
        int new_rootx;
        int new_rooty = node.interval.GetRow();
        if (node.root.x <= node.interval.GetLeft())
        {
            new_rootx = (int)node.interval.GetRight();
        }
        else
        {
            new_rootx = (int)node.interval.GetLeft();
        }
        Split_interval_make_successors(projection.left, projection.right, projection.row, new_rootx, new_rooty, projection.sterile_check_row, node, retval);
    }

    public int Hash(Node n)
    {
        int x = (int)n.root.x;
        int y = (int)n.root.y;
        return y * grid_.Get_width() + x; // AQUI ERA PADDED WIDTH
    }

    public int GetHashCode(Node v)
    {
        return v.GetHashCode();
    }

    public IHeuristic<Node> Heuristic()
    {
        return heuristic_;
    }
}
