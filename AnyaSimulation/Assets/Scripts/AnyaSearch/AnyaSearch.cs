using Anya_2d;
using System;
using System.Collections;
using UnityEngine;

public class AnyaSearch : MonoBehaviour, IMBRunnable
{
    private static int search_id_counter = 0;
    private AnyaExpansionPolicy expander;
    private IHeuristic<Node> heuristic;

    //	private Object[] pool;
    //	private double[] roots;
    private Hashtable roots_ = new Hashtable();
    private SearchNode lastNodeParent;

    public bool verbose = false;
    public bool isRecording = false;

    public int expanded;
    public int insertions;
    public int generated;
    public int heap_ops;
    private FibonacciHeap<Node> open;

    // these can be set apriori; only used in conjunction with the
    // run method.
    public Node mb_start_;
    public Node mb_target_;
    public double mb_cost_;

    public Action<Node> snapshotInsert;
    public Action<Node> snapshotExpand;
    public AnyaSearch(AnyaExpansionPolicy expander)
    {
        //		this.pool = new Object[search_space_size];
        //		this.roots = new double[search_space_size];
        roots_ = new Hashtable(65535);
        open = new FibonacciHeap<Node>();
        heuristic = expander.Heuristic();
        this.expander = expander;
    }

    private void Init()
    {
        search_id_counter++;
        expanded = 0;
        insertions = 0;
        generated = 0;
        heap_ops = 0;
        open.Clear();
        roots_.Clear();
    }

    /*private void print_path(SearchNode current, java.io.PrintStream stream)
	{
		if (current.parent != null)
		{
			print_path(current.parent, stream);
		}
		stream.println(current.getData().hashCode() + "; "
				+ current.getData().root.toString()
				+ "; g=" + current.getSecondaryKey());
	}*/

    private bool PointsEqual(Vector2 p1, Vector2 p2)
    {
        return (int)p1.x == (int)p2.x && (int)p1.y == (int)p2.y;
    }

    public Path<Node> Search(Node start, Node target)
    {
        double cost = Search_costonly(start, target);
        // generate the path
        Path<Node> path = null;
        if (cost != -1)
        {
            //SearchNode node = generate(target);
            path = new Path<Node>(target, path, 0);
            SearchNode node = lastNodeParent;

            while (node != null)
            {
                if (!PointsEqual(path.GetVertex().root, node.GetData().root))
                {
                    path = new Path<Node>(node.GetData(), path, node.GetSecondaryKey());
                }
                node = node.parent;

            }//while(!(node.parent == null));
        }
        return path;
    }

    public double Search_costonly(Node start, Node target)
    {
        Init();
        double cost = -1;
        if (!expander.Validate_instance(start, target))
        {
            return cost;
        }

        SearchNode startNode = Generate(start);
        startNode.ResetNode(search_id_counter);
        open.Insert(startNode, heuristic.GetValue(start, target), 0);

        while (!open.IsEmpty())
        {
            SearchNode current = (SearchNode)open.RemoveMin();
            //if(verbose) { System.out.println("expanding (f="+current.getKey()+") "+current.toString()); }
            /*if (isRecording)
            {
                snapshotExpand.Invoke(current.GetData());
            }*/

            expander.Expand(current.GetData());
            expanded++;
            heap_ops++;
            if (current.GetData().interval.Contains(target.root))
            {
                // found the goal
                cost = current.GetKey();
                lastNodeParent = current;

                if (verbose)
                {
                    //print_path(current, System.err);
                    Console.WriteLine(target.ToString() + "; f=" + current.GetKey());
                }
                break;
            }

            // unique id for the root of the parent node
            int p_hash = expander.Hash(current.GetData());

            // iterate over all neighbours			
            while (expander.HasNext())
            {
                Node succ = expander.Next();
                SearchNode neighbour = Generate(succ);

                bool insert = true;
                int root_hash = expander.Hash(succ);
                SearchNode root_rep = (SearchNode)roots_[root_hash];
                double new_g_value = current.GetSecondaryKey() +
                        expander.Step_cost();


                // Root level pruning:
                // We prune a node if its g-value is larger than the best 
                // distance to its root point. In the case that the g-value
                // is equal to the best known distance, we prune only if the
                // node isn't a sibling of the node with the best distance or
                // if the node with the best distance isn't the immediate parent
                if (root_rep != null)
                {
                    double root_best_g = root_rep.GetSecondaryKey();
                    insert = (new_g_value - root_best_g)
                                   <= GridGraph.epsilon;
                    bool eq = (new_g_value - root_best_g)
                            >= -GridGraph.epsilon;
                    if (insert && eq)
                    {
                        int p_rep_hash = expander.Hash(root_rep.parent.GetData());
                        insert = (root_hash == p_hash) || (p_rep_hash == p_hash);
                    }
                }

                if (insert)
                {
                    neighbour.ResetNode(search_id_counter);
                    neighbour.parent = current;


                    open.Insert(neighbour,
                            new_g_value +
                            heuristic.GetValue(neighbour.GetData(), target),
                            new_g_value);
                    roots_.Add(root_hash, neighbour);

                    //if(verbose) {System.out.println("\tinserting with f=" + neighbour.getKey() +" (g= "+new_g_value+");" + neighbour.toString());}
                    if (isRecording)
                    {
                        snapshotInsert.Invoke(neighbour.GetData());
                    }

                    heap_ops++;
                    insertions++;
                }
                else
                {
                    if (verbose)
                    {
                        Console.WriteLine("\told rootg: " + root_rep.GetSecondaryKey());
                        Console.WriteLine("\tNOT inserting with f=" + neighbour.GetKey() + " (g= " + new_g_value + ");" + neighbour.ToString());
                    }

                }
            }
        }
        if (verbose)
        {
            Console.WriteLine("finishing search;");
        }
        return cost;

    }

    private SearchNode
    Generate(Node v)
    {
        SearchNode retval = new SearchNode(v, search_id_counter);
        generated++;
        return retval;
    }

    public int GetExpanded()
    {
        return expanded;
    }

    public void SetExpanded(int expanded)
    {
        this.expanded = expanded;
    }

    public int GetGenerated()
    {
        return insertions;
    }

    public void SetGenerated(int generated)
    {
        insertions = generated;
    }

    public int GetTouched()
    {
        return generated;
    }

    public void SetTouched(int touched)
    {
        generated = touched;
    }

    public int GetHeap_ops()
    {
        return heap_ops;
    }

    public void SetHeap_ops(int heap_ops)
    {
        this.heap_ops = heap_ops;
    }

    public void Run()
    {
        mb_cost_ = Search_costonly(mb_start_, mb_target_);
    }

    public void CleanUp()
    {
        // TODO Auto-generated method stub

    }
}
