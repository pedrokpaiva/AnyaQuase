using Anya_2d;
using System;
using System.Collections;
using UnityEngine;

public class AnyaSearch
{
    private AnyaExpansionPolicy expander;
    private IHeuristic<Node> heuristic;
    private FibonacciHeap<Node> open;               // all the yet-to-be-expanded search nodes ordered by f value
    private Hashtable roots_ = new Hashtable();     // hash table of all visited roots with their best g-values


    private SearchNode lastNodeParent;

    public bool verbose = false;
    public bool isRecording = false;

    public int expanded;
    public int insertions;
    public int generated;
    public int heap_ops;

    public Node startNode;
    public Node targetNode;
    public double mb_cost_;

    private static int search_id_counter = 0;

    public AnyaSearch(AnyaExpansionPolicy expander)
    {
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
                if (!PointsEqual(path.GetNode().root, node.GetData().root))
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
            return cost;      // se ou start ou target forem obstáculos retorna -1
        }

        SearchNode startNode = Generate(start);
        startNode.ResetNode(search_id_counter);
        open.Insert(startNode, heuristic.GetValue(start, target), 0);   // insere o nó inicial em open

        while (!open.IsEmpty())                                         // enquanto open não é vazio
        {
            SearchNode current = (SearchNode)open.RemoveMin();          // remove o search node com menor custo de open
            expander.Expand(current.GetData());

            if (current.GetData().interval.Contains(target.root))
            {
                // found the goal
                cost = current.GetKey();
                lastNodeParent = current;

                if (verbose)
                {
                    //print_path(current, System.err);
                    Debug.Log(target.ToString() + "; f=" + current.GetKey());
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

    private SearchNode Generate(Node v)
    {
        SearchNode retval = new SearchNode(v, search_id_counter);
        return retval;
    }
}
