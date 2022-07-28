using System;
using System.Collections.Generic;

public class FibonacciHeap<T>
{

    private static double oneOverLogPhi =
        1.0 / Math.Log((1.0 + Math.Sqrt(5.0)) / 2.0);

    //~ Instance fields --------------------------------------------------------

    /// <summary>
    /// O nodo de menor valor do Heap
    /// </summary>
    private FibonacciHeapNode<T> minNode;

    /// <summary>
    /// Número de nodos no Heap
    /// </summary>
    private int nNodes;

    //~ Constructors -----------------------------------------------------------

    /// <summary>
    /// Cria  um Heap vazio
    /// </summary>
    public FibonacciHeap()
    {
    }

    //~ Methods ----------------------------------------------------------------

    /// <summary>
    /// Verifica se o Heap está vazio
    /// </summary>
    /// <returns>true se vazio, false caso contrário</returns>
    public bool IsEmpty()
    {
        return minNode == null;
    }

    /// <summary>
    /// Remove todos os nodos do Heap
    /// </summary>
    public void Clear()
    {
        minNode = null;
        nNodes = 0;
    }

    /// <summary>
    /// Diminui o valor da key de um nodo do Heap
    /// </summary>
    /// <param name="x"> nodo cuja key será reduzida</param>
    /// <param name="k"> novo valor da key do nodo x</param>
    /// <exception cref="ArgumentException">Lançada caso a nova key seja maior que a antiga</exception>
    public void DecreaseKey(FibonacciHeapNode<T> x, double k)
    {
        long tmp_k = (long)(k * FibonacciHeapNode<T>.BIG_ONE + 0.5);
        long tmp_x = (long)(x.key * FibonacciHeapNode<T>.BIG_ONE + 0.5);
        if (tmp_k > tmp_x)
        {
            throw new ArgumentException(
                "decreaseKey() got larger key value");
        }

        x.key = k;

        FibonacciHeapNode<T> y = x.parent;

        if ((y != null) && x.LessThan(y))
        {
            Cut(x, y);
            CascadingCut(y);
        }

        if (x.LessThan(minNode))
        {
            minNode = x;
        }
    }

    /// <summary>
    /// Diminui o valor da key de um nodo do Heap,
    /// adiciona também uma key secunária para desempate
    /// </summary>
    /// <param name="x"></param>
    /// <param name="newKey"></param>
    /// <param name="newSecondaryKey"> key utilizada ara desempate</param>
    public void DecreaseKey(FibonacciHeapNode<T> x, double newKey, double newSecondaryKey)
    {
        x.secondaryKey = newSecondaryKey;
        DecreaseKey(x, newKey);
    }

    /// <summary>
    /// Remove um nodo do Heap
    /// </summary>
    /// <param name="x">Nodo a ser removido</param>
    public void Delete(FibonacciHeapNode<T> x)
    {
        // make x as small as possible
        DecreaseKey(x, double.NegativeInfinity);

        // remove the smallest, which decreases n also
        RemoveMin();
    }


    /// <summary>
    /// Insere um Nodo no Heap
    /// </summary>
    /// <param name="node">Nodo a ser inserido</param>
    /// <param name="key">key do nodo</param>
    public void Insert(FibonacciHeapNode<T> node, double key)
    {
        node.key = key;

        // concatenate node into min list
        if (minNode != null)
        {
            node.left = minNode;
            node.right = minNode.right;
            minNode.right = node;
            node.right.left = node;

            if (node.LessThan(minNode))
            {
                minNode = node;
            }
        }
        else
        {
            minNode = node;
        }
        nNodes++;
    }

    /// <summary>
    ///  Insere um nodo no Heap,
    ///  adiciona uma key secundária para desempate
    /// </summary>
    /// <param name="node"></param>
    /// <param name="key"></param>
    /// <param name="secondaryKey">key para desempate</param>
    public void Insert(FibonacciHeapNode<T> node, double key, double secondaryKey)
    {
        node.secondaryKey = secondaryKey;
        Insert(node, key);
    }

    /// <summary>
    /// Retorna o menor nodo do Heap, o que tem a menor key
    /// </summary>
    /// <returns></returns>
    public FibonacciHeapNode<T> Min()
    {
        return minNode;
    }

    // min

    /// <summary>
    /// Remove o menor nodo do Heap
    /// </summary>
    /// <returns></returns>
    public FibonacciHeapNode<T> RemoveMin()
    {
        FibonacciHeapNode<T> z = minNode;

        if (z != null)
        {
            int numKids = z.degree;
            FibonacciHeapNode<T> x = z.child;
            FibonacciHeapNode<T> tempRight;

            // for each child of z do...
            while (numKids > 0)
            {
                tempRight = x.right;

                // remove x from child list
                x.left.right = x.right;
                x.right.left = x.left;

                // add x to root list of heap
                x.left = minNode;
                x.right = minNode.right;
                minNode.right = x;
                x.right.left = x;

                // set parent[x] to null
                x.parent = null;
                x = tempRight;
                numKids--;
            }

            // remove z from root list of heap
            z.left.right = z.right;
            z.right.left = z.left;

            if (z == z.right)
            {
                minNode = null;
            }
            else
            {
                minNode = z.right;
                Consolidate();
            }

            // decrement size of heap
            nNodes--;
        }

        return z;
    }

    /// <summary>
    /// Retorna o número de nodos do Heap
    /// </summary>
    /// <returns></returns>
    public int Size()
    {
        return nNodes;
    }

    // size

    /// <summary>
    /// Junta dois Heaps, não realiza ajuste
    /// </summary>
    /// <param name="h1"></param>
    /// <param name="h2"></param>
    /// <returns></returns>
    public static FibonacciHeap<T> Union(FibonacciHeap<T> h1, FibonacciHeap<T> h2)
    {
        FibonacciHeap<T> h = new FibonacciHeap<T>();

        if ((h1 != null) && (h2 != null))
        {
            h.minNode = h1.minNode;

            if (h.minNode != null)
            {
                if (h2.minNode != null)
                {
                    h.minNode.right.left = h2.minNode.left;
                    h2.minNode.left.right = h.minNode.right;
                    h.minNode.right = h2.minNode;
                    h2.minNode.left = h.minNode;

                    if (h2.minNode.LessThan(h1.minNode))
                    {
                        h.minNode = h2.minNode;
                    }
                }
            }
            else
            {
                h.minNode = h2.minNode;
            }

            h.nNodes = h1.nNodes + h2.nNodes;
        }

        return h;
    }


    /**
     * Creates a String representation of this Fibonacci heap.
     *
     * @return String of this.
     */
    /*public override String ToString()
    {
        if (minNode == null)
        {
            return "FibonacciHeap=[]";
        }

        // create a new stack and put root on it
        Stack<FibonacciHeapNode<T>> stack = new Stack<FibonacciHeapNode<T>>();
        stack.Push(minNode);

        StringBuffer buf = new StringBuffer(512);
        buf.append("FibonacciHeap=[");

        // do a simple breadth-first traversal on the tree
        while (!stack.empty())
        {
            FibonacciHeapNode<T> curr = stack.pop();
            buf.append(curr);
            buf.append(", ");

            if (curr.child != null)
            {
                stack.push(curr.child);
            }

            FibonacciHeapNode<T> start = curr;
            curr = curr.right;

            while (curr != start)
            {
                buf.append(curr);
                buf.append(", ");

                if (curr.child != null)
                {
                    stack.push(curr.child);
                }

                curr = curr.right;
            }
        }

        buf.append(']');

        return buf.toString();
    }*/

    /// <summary>
    /// Corta y de seu pai e o mesmo para o pai deste, recursivamente
    /// </summary>
    /// <param name="y"></param>
    protected void CascadingCut(FibonacciHeapNode<T> y)
    {
        FibonacciHeapNode<T> z = y.parent;

        // if there's a parent...
        if (z != null)
        {
            // if y is unmarked, set it marked
            if (!y.mark)
            {
                y.mark = true;
            }
            else
            {
                // it's marked, cut it from parent
                Cut(y, z);

                // cut its parent as well
                CascadingCut(z);
            }
        }
    }

    /// <summary>
    /// Organiza o array paramanter sua estrutura
    /// </summary>
    protected void Consolidate()
    {
        int arraySize =
            ((int)Math.Floor(Math.Log(nNodes) * oneOverLogPhi)) + 1;

        List<FibonacciHeapNode<T>> array =
            new List<FibonacciHeapNode<T>>(arraySize);

        // Initialize degree array
        for (int i = 0; i < arraySize; i++)
        {
            array.Add(null);
        }

        // Find the number of root nodes.
        int numRoots = 0;
        FibonacciHeapNode<T> x = minNode;

        if (x != null)
        {
            numRoots++;
            x = x.right;

            while (x != minNode)
            {
                numRoots++;
                x = x.right;
            }
        }

        // For each node in root list do...
        while (numRoots > 0)
        {
            // Access this node's degree..
            int d = x.degree;
            FibonacciHeapNode<T> next = x.right;

            // ..and see if there's another of the same degree.
            for (; ; )
            {
                FibonacciHeapNode<T> y = array[d];
                if (y == null)
                {
                    // Nope.
                    break;
                }

                // There is, make one of the nodes a child of the other.
                // Do this based on the key value.
                if (y.LessThan(x))
                {
                    (x, y) = (y, x);
                }

                // FibonacciHeapNode<T> y disappears from root list.
                Link(y, x);

                // We've handled this degree, go to next one.
                array[d] = null;
                d++;
            }

            // Save this node for later when we might encounter another
            // of the same degree.
            array[d] = x;

            // Move forward through list.
            x = next;
            numRoots--;
        }

        // Set min to null (effectively losing the root list) and
        // reconstruct the root list from the array entries in array[].
        minNode = null;

        for (int i = 0; i < arraySize; i++)
        {
            FibonacciHeapNode<T> y = array[i];
            if (y == null)
            {
                continue;
            }

            // We've got a live one, add it to root list.
            if (minNode != null)
            {
                // First remove node from root list.
                y.left.right = y.right;
                y.right.left = y.left;

                // Now add to root list, again.
                y.left = minNode;
                y.right = minNode.right;
                minNode.right = y;
                y.right.left = y;

                // Check if this is a new min.
                if (y.LessThan(minNode))
                {
                    minNode = y;
                }
            }
            else
            {
                minNode = y;
            }
        }

    }

    /// <summary>
    /// Remove x da lista de filhos de y
    /// </summary>
    /// <param name="x">Nodo filho que será removido</param>
    /// <param name="y">Nodo pai que terá seu filho removido</param>
    protected void Cut(FibonacciHeapNode<T> x, FibonacciHeapNode<T> y)
    {
        // remove x from childlist of y and decrement degree[y]
        x.left.right = x.right;
        x.right.left = x.left;
        y.degree--;

        // reset y.child if necessary
        if (y.child == x)
        {
            y.child = x.right;
        }

        if (y.degree == 0)
        {
            y.child = null;
        }

        // add x to root list of heap
        x.left = minNode;
        x.right = minNode.right;
        minNode.right = x;
        x.right.left = x;

        // set parent[x] to nil
        x.parent = null;

        // set mark[x] to false
        x.mark = false;
    }

    /// <summary>
    /// Insere y como filho de x
    /// </summary>
    /// <param name="y">Nodo que será filho</param>
    /// <param name="x">Nodo qu eserá pai</param>
    protected void Link(FibonacciHeapNode<T> y, FibonacciHeapNode<T> x)
    {
        // remove y from root list of heap
        y.left.right = y.right;
        y.right.left = y.left;

        // make y a child of x
        y.parent = x;

        if (x.child == null)
        {
            x.child = y;
            y.right = y;
            y.left = y;
        }
        else
        {
            y.left = x.child;
            y.right = x.child.right;
            x.child.right = y;
            y.right.left = y;
        }

        // increase degree[x]
        x.degree++;

        // set mark[y] false
        y.mark = false;
    }

}

