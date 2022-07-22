using Anya_2d;
using UnityEngine;

public class GridGeneration : MonoBehaviour
{

    [SerializeField] private LayerMask unwalkableMask;        // a masc�ra da layer de obst�culos 
    [SerializeField] private int numNodesXGrid;
    [SerializeField] private int numNodesYGrid;
    [SerializeField] private float distanceNodes;              // o espa�amento de cada n� no grid
    private GridGraph grid;

    private void Awake()
    {
        CreateGrid();
        // start node = (0, 0) ; target node = (numNodesXGrid, numNodesYGrid)
        Anya pathFinding = new Anya(grid, numNodesXGrid, numNodesYGrid, 0, 0, numNodesXGrid, numNodesYGrid);
        pathFinding.ComputePath();
        printa_path(pathFinding.GetPath());
        ;
    }

    /// <summary>
    /// Cria um grafo de 'numNodesXGrid' pontos no espa�o no eixo X e 'numNodesYGrid' pontos no espa�o no eixo Y, equidistantes,
    /// espa�ados em 'nodeDiameter' unidades.
    /// </summary>
    private void CreateGrid()
    {
        grid = new GridGraph(numNodesXGrid, numNodesYGrid);               // inicializa o grid

        // pega a posi��o no mundo do canto esquerdo do grid 
        Vector3 worldBottomLeft = transform.position - (Vector3.right * (numNodesXGrid / 2) * distanceNodes) - (Vector3.forward * (numNodesYGrid / 2) * distanceNodes);

        for (int x = 0; x < numNodesXGrid; x++)        // pra cada linha da matriz
        {
            for (int y = 0; y < numNodesYGrid; y++)    // pra cada linha da matriz
            {
                // cria a posi��o no mundo correspondente a este �ndice da matriz
                Vector3 worldPoint = worldBottomLeft + Vector3.right * ((x * distanceNodes) + (distanceNodes / 2)) + Vector3.forward * ((y * distanceNodes) + (distanceNodes / 2));
                // checa se nesta posi��o do mundo est� um obst�culo
                bool walkable = !(Physics.CheckSphere(worldPoint, distanceNodes / 2, unwalkableMask));
                // seta o bloco correspondente no grid
                grid.SetBlocked(x, y, walkable);
            }
        }
    }

    private void OnDrawGizmosSelected()         // fun��o pra criar o grid visual na tela de jogo
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(numNodesXGrid * distanceNodes, distanceNodes, numNodesYGrid * distanceNodes));    // desenha o cubo 

        if (grid != null)
        {
            // pega a posi��o no mundo do canto esquerdo do grid 
            Vector3 worldBottomLeft = transform.position - (Vector3.right * (numNodesXGrid / 2) * distanceNodes) - (Vector3.forward * (numNodesYGrid / 2) * distanceNodes);

            for (int x = 0; x < numNodesXGrid; x++)        // pra cada linha da matriz
            {
                for (int y = 0; y < numNodesYGrid; y++)    // pra cada coluna da matriz
                {
                    Gizmos.color = grid.IsBlocked(x, y) ? Color.white : Color.red;
                    // cria a posi��o no mundo correspondente a este �ndice da matriz
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * ((x * distanceNodes) + (distanceNodes / 2)) + Vector3.forward * ((y * distanceNodes) + (distanceNodes / 2));
                    Gizmos.DrawSphere(worldPoint, distanceNodes / 8);    // desenha o cubo representando o n�
                }
            }
        }
    }

    /// <summary>
    /// Printa um caminho.
    /// </summary>
    private void printa_path(int[][] path)
    {
        int dimensaoX = path.GetLength(0);

        for (int x = 0; x < dimensaoX; x++)
        {
            Debug.Log("(" + path[x][0] + ", " + path[x][1] + ") ");
        }
    }

}