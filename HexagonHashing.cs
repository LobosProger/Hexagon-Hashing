using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MyBox;
[RequireComponent(typeof(Grid))]
public class HexagonHashing : MonoBehaviour
{
	private Grid grid;

	//* All optimizing objects, which need for 
	public static HashSet<HexagonHashingOptimization> allHexagonOptimizationObjects = new HashSet<HexagonHashingOptimization>();

	public static bool isInitialized = false;
	public static HexagonHashing singletonHexagon;

	//* The players in grid
	////* This needs to rebuild ALL optimizable objects
	[SerializeField]
	public Dictionary<Vector2Int, HashSet<NetworkIdentity>> ObserversOfCells = new Dictionary<Vector2Int, HashSet<NetworkIdentity>>();

	//* Just keep last time of rebuilding 
	//* => Because, each operation of rebuilding in every frame is expensive!
	//* => So, we just waiting a moment for rebuilding
	[SerializeField]
	[Range(0f, 1f)]
	private float timeRebuilding;
	private float lastTime;
	[SerializeField]
	private bool optimizedHexagonHashing;


	//* Resolution for every cell
	//* => We can represent resolution as one cell of table
	private float resolution;

	//* In start we waiting for starting player, get a grid, make it more sensitive and assign value in varibale resolution!
	//* => We need to our script works correctly!
	private IEnumerator Start()
	{
		yield return (NetworkClient.ready && NetworkTime.time > 0 && !NetworkServer.active && NetworkClient.localPlayer != null) || NetworkServer.active && NetworkClient.localPlayer != null;
		singletonHexagon = this;
		isInitialized = true;

		grid = GetComponent<Grid>();
		SetCorrectPositionOfGrid();

		if (!optimizedHexagonHashing)
			grid.cellSize = new Vector3(grid.cellSize.x / 2, grid.cellSize.x / 2, 1);

		resolution = grid.cellSize.x;

		ClearGridNonAlloc();

		if (NetworkServer.active)
			RebuildOnHost();
		else
			RebuildOnClient();
	}

	//* Just waiting next rebuilding operation, clear a grid to avoid allocations and rebuild everything
	private void Update()
	{
		if (NetworkClient.localPlayer == null)
			return;

		if (Time.fixedTime >= lastTime + timeRebuilding)
		{
			lastTime = Time.fixedTime;
			ClearGridNonAlloc();

			if (NetworkServer.active)
				RebuildOnHost();
			else
				RebuildOnClient();
		}
	}

	//* This needs for rebuilding network objects (for other players) and components (for local player)
	//* => Because by this, we can reach better perfomance by switching off simulation of entities (bots, ships and etc)
	private void RebuildOnHost()
	{
		foreach (NetworkConnection everyPlayer in NetworkServer.connections.Values)
			SetObserversCells(everyPlayer.identity);

		foreach (HexagonHashingOptimization every in allHexagonOptimizationObjects)
			RebuildEntity(every);
	}

	//* This needs for rebuilding ONLY for client, which connected to server
	//* => By this, we reach better perfomance by turning off simple components as animators and etc
	//* => We don't need in simulating components, which player can't see
	private void RebuildOnClient()
	{
		SetObserversCells(NetworkClient.localPlayer);

		foreach (HexagonHashingOptimization every in allHexagonOptimizationObjects)
			RebuildEntity(every);
	}

	//* Just use a function to present it more simplier
	//* And by this function we create a key by position of object, rebuild its simple and network components if cell contains our local player
	//* And rebuild for other player, that is if cell contains other players, but we rebuild only network components
	//* => We can use this function for rebuilding on server and host,
	//* => because functionality is same
	//* => don't worry about using VisibleByNetworkedPlayer and VisibleByLocalPlayer together 
	//* => script HexagonHashingOptimization is checking, from where were called functions (from server/client)
	public void RebuildEntity(HexagonHashingOptimization entity)
	{
		Vector2Int key = entity.transform.hasChanged ? ProjectToGrid(entity.transform.position) : entity.key;
		entity.transform.hasChanged = false;
		entity.key = key;

		if (ObserversOfCells.ContainsKey(key) && ObserversOfCells[key].Count != 0)
		{
			if (ObserversOfCells[key].Contains(NetworkClient.localPlayer))
			{
				entity.Rebuild(true, true);
			}
			else
			{
				entity.Rebuild(false, true);
			}
		}
		else
		{
			entity.Rebuild(false, false);
		}
	}

	//* There is we just creating keys in dictionary, on which our player will affect by observing
	//* => So, we just write in cells our player as observer
	private void SetObserversCells(NetworkIdentity conn)
	{
		if (conn != null)
		{
			Vector2Int[] neighbours = optimizedHexagonHashing ? OptimizedNeighbours : Neighbours;
			Vector2Int projectedPositionOfPlayer = ProjectToGrid(conn.transform.position);

			foreach (Vector2Int offset in neighbours)
			{
				Vector2Int key = new Vector2Int(offset.x + projectedPositionOfPlayer.x, offset.y + projectedPositionOfPlayer.y);
				if (!ObserversOfCells.ContainsKey(key))
					ObserversOfCells.Add(key, new HashSet<NetworkIdentity>());

				ObserversOfCells[key].Add(conn);
			}
		}
	}

	//* Just get key by representing in our grid
	Vector2Int ProjectToGrid(Vector3 position) =>
		Vector2Int.RoundToInt(new Vector2(position.x, position.y) / resolution);

	//* Clear all players in grid
	//* => Because we need to avoid allocations
	private void ClearGridNonAlloc()
	{
		foreach (HashSet<NetworkIdentity> every in ObserversOfCells.Values)
			every.Clear();
	}

	Vector2Int[] Neighbours =
	{
		//* Simple grid
		new Vector2Int(0, 0),
		new Vector2Int(0, 1),
		new Vector2Int(1, 1),
		new Vector2Int(-1, 1),
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(0, -1),
		new Vector2Int(1, -1),
		new Vector2Int(-1, -1),

		//* Up side
		new Vector2Int(0, 2),
		new Vector2Int(1, 2),
		new Vector2Int(2, 2),
		new Vector2Int(-1, 2),
		new Vector2Int(-2, 2),

		//* Down side
		new Vector2Int(0, -2),
		new Vector2Int(1, -2),
		new Vector2Int(2, -2),
		new Vector2Int(-1, -2),
		new Vector2Int(-2, -2),

		//* Right side
		new Vector2Int(2, 0),
		new Vector2Int(2, 1),
		new Vector2Int(2, -1),

		//* Left side
		new Vector2Int(-2, 0),
		new Vector2Int(-2, 1),
		new Vector2Int(-2, -1)
	};

	Vector2Int[] OptimizedNeighbours =
	{
		//* Simple grid
		new Vector2Int(0, 0),
		new Vector2Int(0, 1),
		new Vector2Int(1, 1),
		new Vector2Int(-1, 1),
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(0, -1),
		new Vector2Int(1, -1),
		new Vector2Int(-1, -1),
	};

	[ButtonMethod]
	private void SetCorrectPositionOfGrid()
	{
		if (!optimizedHexagonHashing)
			transform.position = new Vector3(-(GetComponent<Grid>().cellSize.x / 4), -(GetComponent<Grid>().cellSize.y / 4), 0);
		else
			transform.position = new Vector3(-(GetComponent<Grid>().cellSize.x / 2), -(GetComponent<Grid>().cellSize.y / 2), 0);
	}
}
