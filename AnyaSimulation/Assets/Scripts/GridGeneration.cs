using Anya_2d;
using UnityEngine;

public class GridGeneration : MonoBehaviour
{

    [SerializeField] private LayerMask unwalkableMask;        // a mascára da layer de obstáculos 
    [SerializeField] private int numNodesXGrid;
    [SerializeField] private int numNodesYGrid;
    [SerializeField] private float distanceNodes;             // o espaçamento de cada nó no grid
    [SerializeField] int startNodeX;
    [SerializeField] int startNodeY;
    [SerializeField] int targetNodeX;
    [SerializeField] int targetNodeY;
    private GridGraph grid;
    private Anya pathFinding;

    [SerializeField] Transform seeker;                 // transform do drone
    [SerializeField] Rigidbody seekerRB;               // rigidbody do drone
    int destinoX;                                     // destino

    private void Awake()
    {
        CreateGrid();
        pathFinding = new Anya(grid, numNodesXGrid, numNodesYGrid, startNodeX, startNodeY, targetNodeX, targetNodeY);
        pathFinding.ComputePath();
        Printa_path(pathFinding.GetPath());

        // pega a posição no mundo do canto esquerdo do grid 
        Vector3 worldUpLeft = transform.position - (Vector3.right * (numNodesXGrid / 2) * distanceNodes) + (Vector3.forward * (numNodesYGrid / 2) * distanceNodes);
        seeker.transform.position = worldUpLeft + Vector3.right * ((startNodeX * distanceNodes) + (distanceNodes / 2)) - Vector3.forward * ((startNodeY * distanceNodes) + (distanceNodes / 2)); //bota o drone na altura inicial
        destinoX = 1;
    }

    private void Update()
    {
        Follow_Path(pathFinding.GetPath());
    }

    /// <summary>
    /// Cria um grafo de 'numNodesXGrid' pontos no espaço no eixo X e 'numNodesYGrid' pontos no espaço no eixo Y, equidistantes,
    /// espaçados em 'nodeDiameter' unidades.
    /// </summary>
    private void CreateGrid()
    {
        grid = new GridGraph(numNodesXGrid, numNodesYGrid);               // inicializa o grid

        // pega a posição no mundo do canto esquerdo do grid 
        Vector3 worldUpLeft = transform.position - (Vector3.right * (numNodesXGrid / 2) * distanceNodes) + (Vector3.forward * (numNodesYGrid / 2) * distanceNodes);

        for (int x = 0; x < numNodesXGrid; x++)        // pra cada linha da matriz
        {
            for (int y = 0; y < numNodesYGrid; y++)    // pra cada linha da matriz
            {
                // cria a posição no mundo correspondente a este índice da matriz
                Vector3 worldPoint = worldUpLeft + Vector3.right * ((x * distanceNodes) + (distanceNodes / 2)) - Vector3.forward * ((y * distanceNodes) + (distanceNodes / 2));
                // checa se nesta posição do mundo está um obstáculo
                bool blocked = Physics.CheckSphere(worldPoint, distanceNodes / 2, unwalkableMask);
                // seta o bloco correspondente no grid
                grid.SetBlocked(x, y, blocked);
            }
        }
        // setando as bordas do grid como bloqueadas
        for (int x = 0; x < numNodesXGrid; x++)
        {
            grid.SetBlocked(x, 0, true);
            grid.SetBlocked(x, numNodesYGrid - 1, true);
        }
        for (int y = 0; y < numNodesYGrid; y++)
        {
            grid.SetBlocked(0, y, true);
            grid.SetBlocked(numNodesXGrid - 1, y, true);
        }
    }

    private void OnDrawGizmosSelected()         // função pra criar o grid visual na tela de jogo
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(numNodesXGrid * distanceNodes, distanceNodes, numNodesYGrid * distanceNodes));    // desenha o cubo 

        if (grid != null)
        {
            // pega a posição no mundo do canto esquerdo do grid 
            Vector3 worldUpLeft = transform.position - (Vector3.right * (numNodesXGrid / 2) * distanceNodes) + (Vector3.forward * (numNodesYGrid / 2) * distanceNodes);

            for (int x = 0; x < numNodesXGrid; x++)        // pra cada linha da matriz
            {
                for (int y = 0; y < numNodesYGrid; y++)    // pra cada coluna da matriz
                {
                    if (x == startNodeX && y == startNodeY) Gizmos.color = Color.green;
                    else if (x == targetNodeX && y == targetNodeY) Gizmos.color = Color.black;
                    else Gizmos.color = grid.IsBlocked(x, y) ? Color.red : Color.white;
                    
                    // cria a posição no mundo correspondente a este índice da matriz
                    Vector3 worldPoint = worldUpLeft + Vector3.right * ((x * distanceNodes) + (distanceNodes / 2)) - Vector3.forward * ((y * distanceNodes) + (distanceNodes / 2));
                    Gizmos.DrawSphere(worldPoint, distanceNodes / 8);    // desenha o cubo representando o nó
                }
            }
        }
    }

    /// <summary>
    /// Printa um caminho.
    /// </summary>
    private void Printa_path(int[][] path)
    {
        int dimensaoX = path.GetLength(0);

        if (dimensaoX != 0)
        {
            for (int x = 0; x < dimensaoX; x++)
            {
                Debug.Log("(" + path[x][0] + ", " + path[x][1] + ") ");
            }
        }
        else Debug.Log("Nenhum caminho encontrado.");

    }
        
    void Follow_Path(int[][] path)      // função que um objeto segue um caminho de nodos
    {
        int dimensaoX = path.GetLength(0);

        if (dimensaoX != 0 && destinoX != dimensaoX)                // se este nodo é o final, chegou ao destino
        {
            // pega a posição no mundo do canto esquerdo do grid 
            Vector3 worldUpLeft = transform.position - (Vector3.right * (numNodesXGrid / 2) * distanceNodes) + (Vector3.forward * (numNodesYGrid / 2) * distanceNodes);
            // cria a posição no mundo correspondente ao destino atual na matriz
            Vector3 destino = worldUpLeft + Vector3.right * ((path[destinoX][0] * distanceNodes) + (distanceNodes / 2)) - Vector3.forward * ((path[destinoX][1] * distanceNodes) + (distanceNodes / 2));


            if (seeker.transform.position != destino)          // enquanto a posição do objeto não é o destino
            {
                seeker.transform.LookAt(destino);              // direciona o objeto ao destino
                seeker.transform.position = Vector3.MoveTowards(seeker.transform.position, destino, 10 * Time.deltaTime);

                // quando a distância é quase igual, zera a velocidade e seta a posição
                if (Vector3.Distance(seeker.transform.position, destino) < 0.1f)
                {
                    seeker.transform.position = new Vector3(destino.x, destino.y, destino.z);
                }
            }
            // se chegou ao nodo destino, vai para o próximo 
            else destinoX++;

            if (destinoX == dimensaoX)                // se este nodo é o final, chegou ao destino
            {
                seekerRB.velocity = Vector3.zero;     // zera a velocidade 
            }
        }
    }
}

