using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AStarTile : MonoBehaviour, IPointerClickHandler
{
    public enum TileType
	{
        Searchable,
        Start,
        End,
        Wall
	}

    public AStarTile parent;
    public Vector2Int location;
    public float GScore;
    public float HScore;
    public float FScore;

    public AStarController controller;
    public TileType tileType;

    public Text FN;
    public Text G;
    public Text H;

    public GameObject searchTile;
    public GameObject startTile;
    public GameObject endTile;
    public GameObject wallTile;
    public GameObject pathTile;
    public GameObject currentBorder;
    public GameObject inspectedBorder;

    void Awake()
    {
        tileType = TileType.Searchable;
        FN.gameObject.SetActive(false);
        G.gameObject.SetActive(false);
        H.gameObject.SetActive(false);
        searchTile.gameObject.SetActive(true);
        startTile.gameObject.SetActive(false);
        endTile.gameObject.SetActive(false);
        wallTile.gameObject.SetActive(false);
        pathTile.gameObject.SetActive(false);
        currentBorder.gameObject.SetActive(false);
        inspectedBorder.gameObject.SetActive(false);
    }
    public void SetParent(AStarTile _parent)
    {
        parent = _parent;
    }

    public void SetAsPathTile()
    {
        pathTile.gameObject.SetActive(true);
    }
    public void ResetTile(bool clearWalls = false)
	{
        FN.gameObject.SetActive(false);
        G.gameObject.SetActive(false);
        H.gameObject.SetActive(false);
        pathTile.gameObject.SetActive(false);
        currentBorder.gameObject.SetActive(false);
        inspectedBorder.gameObject.SetActive(false);

        if (clearWalls && tileType == AStarTile.TileType.Wall)
        {
            SetTile(AStarTile.TileType.Searchable);
        }
    }

    public void SetScore(float g_score, float h_score)
	{
        GScore = g_score;
        HScore = h_score;
        FScore = GScore + HScore;

        FN.gameObject.SetActive(true);
        G.gameObject.SetActive(true);
        H.gameObject.SetActive(true);
        FN.text = string.Format("{0:0.00}", FScore);
        G.text = string.Format("{0:0.00}", GScore);
        H.text = string.Format("{0:0.00}", HScore);
    }

    public void SetTile(TileType type)
	{
        tileType = type;

        searchTile.gameObject.SetActive(type == TileType.Searchable);
        startTile.gameObject.SetActive(type == TileType.Start);
        endTile.gameObject.SetActive(type == TileType.End);
        wallTile.gameObject.SetActive(type == TileType.Wall);
    }

	public void OnPointerClick(PointerEventData eventData)
	{
        if (tileType != AStarTile.TileType.Start || tileType != AStarTile.TileType.End)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Input.GetKey(KeyCode.S))
				{
                    controller.startTile.SetTile(TileType.Searchable);
                    controller.startTile = this;
                    SetTile(AStarTile.TileType.Start);
                }
                else if (Input.GetKey(KeyCode.E))
				{
                    controller.endTile.SetTile(TileType.Searchable);
                    controller.endTile = this;
                    SetTile(AStarTile.TileType.End);
                }
                else
				{
                    SetTile(AStarTile.TileType.Wall);
                }
            }
            else
			{
                SetTile(AStarTile.TileType.Searchable);
            }
        }
    }
}
