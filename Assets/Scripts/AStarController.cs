using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AStarController : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public GameObject searchTile;
    public Vector2Int StartLocation = new Vector2Int(4, 4);
    public Vector2Int EndLocation = new Vector2Int(4, 11);

    [HideInInspector] public List<List<AStarTile>> tiles = new List<List<AStarTile>>();
    [HideInInspector] public AStarTile startTile;
    [HideInInspector] public AStarTile endTile;

    public List<AStarTile> openList = new List<AStarTile>();
    public List<AStarTile> closedList = new List<AStarTile>();


    void Start()
    {
        for (int i = 0; i < 9; i++)
		{
            tiles.Add(new List<AStarTile>());
            for(int j = 0; j < 16; j++)
			{
                GameObject tile = GameObject.Instantiate(searchTile);
                tile.transform.SetParent(canvasScaler.transform);

                RectTransform rectTransform = tile.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(-450 + j * 60, 240 - i * 60);
                tile.transform.localScale = Vector2.one;
                tile.GetComponent<AStarTile>().controller = this;

                tiles[i].Add(tile.GetComponent<AStarTile>());
            }
		}

        startTile = tiles[StartLocation.x][StartLocation.y];
        endTile = tiles[EndLocation.x][EndLocation.y];

        startTile.SetTile(AStarTile.TileType.Start);
        endTile.SetTile(AStarTile.TileType.End);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ResetGrid(true);
        }
        else if (Input.GetKeyUp(KeyCode.Return))
		{
            ResetGrid();
            GeneratePath();
		}
    }

    public void ResetGrid(bool clearWalls = false)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = 0; j < tiles[i].Count; j++)
            {
                tiles[i][j].ResetTile(clearWalls);
                tiles[i][j].location = new Vector2Int(j, i);
            }
        }
    }

    public void GeneratePath()
    {
        openList.Clear();
        closedList.Clear();

        openList.Add(startTile);

        while (openList.Count > 0)
        {
            AStarTile currentTile = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FScore < currentTile.FScore || openList[i].FScore == currentTile.FScore && openList[i].HScore < currentTile.HScore)
                {
                    currentTile = openList[i];
                }
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == endTile)
            {
                RetracePath();
                return;
            }

            foreach (AStarTile neighbour in GetNeighbours(currentTile))
            {
                if (neighbour.tileType == AStarTile.TileType.Wall || closedList.Contains(neighbour))
                {
                    continue;
                }

                float newGScore = currentTile.GScore + GetDistance(currentTile, neighbour);
                if (newGScore < neighbour.GScore || !openList.Contains(neighbour))
                {
                    neighbour.GScore = newGScore;
                    neighbour.HScore = GetDistance(neighbour, endTile);
                    neighbour.FScore = neighbour.GScore + neighbour.HScore;
                    neighbour.SetScore(neighbour.FScore, neighbour.GScore);
                    neighbour.parent = currentTile;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

    }

    private void RetracePath()
    {
        AStarTile currentTile = endTile;
        while (currentTile != startTile)
        {
            currentTile.SetAsPathTile();
            currentTile=currentTile.parent;
            
            
        }
    }

    private float GetDistance(AStarTile tileA, AStarTile tileB)
    {
        return Vector2Int.Distance(tileA.location, tileB.location);
    }

    private List<AStarTile> GetNeighbours(AStarTile currentTile)
    {
        List<AStarTile> neighbours = new List<AStarTile>();

        int x = currentTile.location.x;
        int y = currentTile.location.y;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Skip the current tile itself
                if (i == 0 && j == 0)
                    continue;

                int checkX = x + i;
                int checkY = y + j;

                // Check if the neighbor is within grid bounds
                if (checkX >= 0 && checkX < tiles[0].Count && checkY >= 0 && checkY < tiles.Count)
                {
                    neighbours.Add(tiles[checkY][checkX]);
                }
            }
        }
        return neighbours;

    }
}
