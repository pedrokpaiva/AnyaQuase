<<<<<<< HEAD
public class FibonacciHeapNode<T>
=======
using UnityEngine;

/// <summary>
/// Nodo de um Heap
/// </summary>
/// <typeparam name="T"></typeparam>
public class FibonacciHeapNode<T> : MonoBehaviour
>>>>>>> 74655071d545d51f763dfc0be900942a3e77e066
{
    /// <summary>
    /// Dados do nodo
    /// </summary>
    public T data;

    /// <summary>
    /// Primeiro filho
    /// </summary>
    public FibonacciHeapNode<T> child;

    /// <summary>
    /// Nodo a esquerda
    /// </summary>
    public FibonacciHeapNode<T> left;

    /// <summary>
    /// Nodo pai
    /// </summary>
    public FibonacciHeapNode<T> parent;

    /// <summary>
    /// Nodo a direita
    /// </summary>
    public FibonacciHeapNode<T> right;

    /// <summary>
    /// True se esse nodo teve um filho removido desde sua inserção ao seu pai
    /// </summary>
    public bool mark;

    public double key;
    public double secondaryKey;
    public static long BIG_ONE = 100000;
    public static double epsilon = 1 / BIG_ONE;

    /// <summary>
    /// Número de filhos do nodo, apenas filhos diretos
    /// </summary>
    public int degree;

    //~ Constructors -----------------------------------------------------------

    /// <summary>
    /// Cria um Nodo a partir de seus dados
    /// </summary>
    /// <param name="data"></param>
    public FibonacciHeapNode(T data)
    {
        this.data = data;
        ResetNode();
    }

    /// <summary>
    /// Inicializa um nodo e o torna double-linked circularmente
    /// </summary>
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

    /// <summary>
    /// Getter de key
    /// </summary>
    /// <returns>key</returns>
    public double GetKey()
    {
        return key;
    }

    /// <summary>
    /// Getter de secondaryKey
    /// </summary>
    /// <returns>key secundária</returns>
    public double GetSecondaryKey()
    {
        return secondaryKey;
    }

    /// <summary>
    /// Getter de data
    /// </summary>
    /// <returns>dados do nodo</returns>
    public T GetData()
    {
        return data;
    }

    /// <summary>
    /// Transforma o nodo em uma string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return key.ToString();
    }

    /// <summary>
    /// Compara dois nodos
    /// </summary>
    /// <param name="other"></param>
    /// <returns>true se possui menos prioridade que other</returns>
    public bool LessThan(FibonacciHeapNode<T> other)
    {
        return FibonacciHeapNode<T>.LessThan(
                key, secondaryKey,
                other.key, other.secondaryKey);
    }
    /// <summary>
    /// Compara as keys (primárias e secundárias) de dois nodos
    /// </summary>
    /// <returns>true se key menor que otherKey ou , caso iguais, secKey maior que otherSecKey </returns>
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
