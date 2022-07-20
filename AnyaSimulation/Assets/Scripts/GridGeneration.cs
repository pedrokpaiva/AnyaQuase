using UnityEngine;
using System.Collections.Generic;
using Anya_2d;

public class GridGeneration : MonoBehaviour
{

	[SerializeField] LayerMask unwalkableMask;        // a masc�ra da layer de obst�culos 
	[SerializeField] int numNodesXGrid;               
	[SerializeField] int numNodesYGrid;
	[SerializeField] float nodeDiameter;              // o diametro  de cada n� no grid
	GridGraph grid;

	void Awake()
	{
		CreateGrid();
	}

	void CreateGrid()
	{
		grid = new GridGraph(numNodesXGrid, numNodesYGrid);               // inicializa o grid

		// pega a posi��o no mundo do canto esquerdo do grid 
		Vector3 worldBottomLeft = transform.position - (Vector3.right * (numNodesXGrid / 2) * nodeDiameter) - (Vector3.forward * (numNodesYGrid / 2) * nodeDiameter);

		for (int x = 0; x < numNodesXGrid; x++)        // pra cada linha do array
		{
			for (int y = 0; y < numNodesYGrid; y++)    // pra cada coluna do array
			{
				// cria a posi��o no mundo correspondente a este �ndice do array
				Vector3 worldPoint = worldBottomLeft + Vector3.right * ((x * nodeDiameter) + (nodeDiameter / 2)) + Vector3.forward * ((y * nodeDiameter) + (nodeDiameter / 2));
				// checa se nesta posi��o do mundo est� um obst�culo
				bool walkable = !(Physics.CheckBox(worldPoint, new Vector3(nodeDiameter/2,nodeDiameter/2,nodeDiameter/2), Quaternion.identity, unwalkableMask));
				// seta o bloco correspondente no grid
				grid.SetBlocked(x, y, walkable);
			}
		}	
	}

	
	void OnDrawGizmosSelected()         // fun��o pra criar o grid visual na tela de jogo
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(numNodesXGrid * nodeDiameter, nodeDiameter, numNodesYGrid * nodeDiameter));    // desenha o cubo 

		if (grid != null)
		{
			// pega a posi��o no mundo do canto esquerdo do grid 
			Vector3 worldBottomLeft = transform.position - (Vector3.right * (numNodesXGrid / 2) * nodeDiameter) - (Vector3.forward * (numNodesYGrid / 2) * nodeDiameter);

			for (int x = 0; x < numNodesXGrid; x++)        // pra cada linha do array
			{
				for (int y = 0; y < numNodesYGrid; y++)    // pra cada coluna do array
				{
					Gizmos.color = grid.IsBlocked(x, y) ? Color.white : Color.red;
					// cria a posi��o no mundo correspondente a este �ndice do array
					Vector3 worldPoint = worldBottomLeft + Vector3.right * ((x * nodeDiameter) + (nodeDiameter / 2)) + Vector3.forward * ((y * nodeDiameter) + (nodeDiameter / 2));
					Gizmos.DrawCube(worldPoint, Vector3.one * (nodeDiameter - .1f));    // desenha o cubo representando o n�
				}
			}
		}
	}

}