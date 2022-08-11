using Anya_2d;
using System;
using UnityEngine;

public class Anya
{
    private static AnyaSearch anya = null;
    protected GridGraph storedGraph;
    private Path<Node> pathStartNode = null;

    protected int[] parent;

    protected int sizeX;         // dimensões do gridgraph
    protected int sizeY;
    protected int sizeXplusOne;

    protected int startX;        // coordenadas x e y do ponto inicial e destino da busca
    protected int startY;
    protected int targetX;
    protected int targetY;


    public Anya(GridGraph graph, int sizeX, int sizeY, int indexSX, int indexSY, int indexTX, int indexTY)
    {
        storedGraph = graph;
        this.sizeX = sizeX;
        sizeXplusOne = sizeX + 1;
        this.sizeY = sizeY;
        startX = indexSX;
        startY = indexSY;
        targetX = indexTX;
        targetY = indexTY;

        Initialise(graph, indexSX, indexSY, indexTX, indexTY);
    }


    /// <summary>
    /// Inicializa as informações para começar a busca.
    /// </summary>
    private static void Initialise(GridGraph grid, int indexSX, int indexSY, int indexTX, int indexTY)
    {
        try
        {
            anya = new AnyaSearch(new AnyaExpansionPolicy(grid));

            Node start = new Node(null, new Interval(0, 0, 0), 0, 0);
            Node target = new Node(null, new Interval(0, 0, 0), 0, 0);
            start.root.Set(indexSX, indexSY);
            start.interval.Init(indexSX, indexSX, indexSY);
            target.root.Set(indexTX, indexTY);
            target.interval.Init(indexTX, indexTX, indexTY);

            anya.startNode = start;
            anya.targetNode = target;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Computa o caminho através do algoritmo Anya, e ajusta os parentescos dos nós que o compõem
    /// (para que o backtracking seja possível).
    /// </summary>
    public void ComputePath()
    {
        pathStartNode = anya.Search(anya.startNode, anya.targetNode);
    }

    /// <summary>
    /// Retorna uma matriz com os pontos do caminho computado.
    /// Cada linha descreve um ponto e as colunas representam as coordenadas.
    /// p = (x, y) <=> p = (matriz[p][0], matriz[p][1])
    /// </summary>
    public int[][] GetPath()
    {
        int length = 0;
        Path<Node> current = pathStartNode;
        while (current != null)
        {
            current = current.GetNext();
            ++length;                            // conta quantos nós tem no caminho
        }
        int[][] path = new int[length][];        // inicializa as linhas da matriz com o número de nós do caminho

        current = pathStartNode;
        int i = 0;
        while (current != null)
        {
            Vector2 point = current.GetNode().root;
            path[i] = new int[] { (int)point.x, (int)point.y };  // seta as colunas com as coordenadas do ponto da linha correspondente,
            current = current.GetNext();                         // de acordo com a ordem no trajeto
            ++i;
        }

        return path;
    }
}
