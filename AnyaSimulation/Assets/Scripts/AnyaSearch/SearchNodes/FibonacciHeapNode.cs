using UnityEngine;

public class FibonacciHeapNode<T> : MonoBehaviour
{
    /**
    * Node data.
    */
    public T data;

    /**
     * first child node
     */
    public FibonacciHeapNode<T> child;

    /**
     * left sibling node
     */
    public FibonacciHeapNode<T> left;

    /**
     * parent node
     */
    public FibonacciHeapNode<T> parent;

    /**
     * right sibling node
     */
    public FibonacciHeapNode<T> right;

    /**
     * true if this node has had a child removed since this node was added to
     * its parent
     */
    public bool mark;

    /**
     * key value for this node
     */
    public double key;
    public double secondaryKey;
    public static long BIG_ONE = 100000;
    public static double epsilon = 1 / BIG_ONE;

    /**
     * number of children of this node (does not count grandchildren)
     */
    public int degree;

    //~ Constructors -----------------------------------------------------------

    /**
     * Default constructor. Initializes the right and left pointers, making this
     * a circular doubly-linked list.
     *
     * @param data data for this node
     */
    public FibonacciHeapNode(T data)
    {
        this.data = data;
        ResetNode();
    }

    protected void ResetNode()
    {
        parent = null;
        child = null;
        right = this;
        left = this;
        key = 0;
        secondaryKey = 0;
        degree = 0;
        mark = false;
    }

    //~ Methods ----------------------------------------------------------------

    /**
     * Obtain the key for this node.
     *
     * @return the key
     */
    public double GetKey()
    {
        return key;
    }

    public double GetSecondaryKey()
    {
        return secondaryKey;
    }

    /**
     * Obtain the data for this node.
     */
    public T GetData()
    {
        return data;
    }

    /**
     * Return the string representation of this object.
     *
     * @return string representing this object
     */
    public override string ToString()
    {
        return key.ToString();
    }

    /*
     * @return true if this node has a lower priority
     * than @parameter other
     */
    public bool LessThan(FibonacciHeapNode<T> other)
    {
        return FibonacciHeapNode<T>.LessThan(
                key, secondaryKey,
                other.key, other.secondaryKey);
    }

    public static bool LessThan(double pk_a, double sk_a,
            double pk_b, double sk_b)
    {
        long tmpKey = (long)(pk_a * BIG_ONE + 0.5);
        long tmpOther = (long)(pk_b * BIG_ONE + 0.5);
        if (tmpKey < tmpOther)
        {
            return true;
        }

        // tie-break in favour of nodes with higher 
        // secondaryKey values
        if (tmpKey == tmpOther)
        {
            tmpKey = (long)(sk_a * BIG_ONE + 0.5);
            tmpOther = (long)(sk_b * BIG_ONE + 0.5);
            if (tmpKey > tmpOther)
            {
                return true;
            }
        }
        return false;
    }
}
