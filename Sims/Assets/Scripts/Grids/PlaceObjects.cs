using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjects : MonoBehaviour
{
    [SerializeField] GameObject placeAbleObject;
    private GridManager gridManager;
    private GameObject obj;
    private Vector2 size;
    private int sizeX;
    private int sizeY;

    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        obj = Instantiate(placeAbleObject);
        size = obj.GetComponent<ObjectSize>().Size();

        sizeX = obj.GetComponent<ObjectSize>().GetObjectWidth();
        sizeY = obj.GetComponent<ObjectSize>().GetObjectHeight();
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 snappedPosition = gridManager.SnapToGrid(mousePosition);
        transform.position = snappedPosition;

        var (x, y) = gridManager.GetGridPosition(snappedPosition);

        if(Input.GetMouseButton(0) && !gridManager.IsTileOccupied(x,y) && (sizeX == 1 || sizeY == 1))
        {
            PlaceObjectSingle(snappedPosition, x, y);
        }
        else if(Input.GetMouseButton(0) && !gridManager.IsAreaOccupied(x, y, sizeX, sizeY) && (sizeX > 1 || sizeY > 1))
        {
            PlaceObjectMultiple(snappedPosition, x, y);
        }
        else if(Input.GetMouseButton(2) && gridManager.IsTileOccupied(x,y))
        {
            // Implement later a way to delete an object
        }

        HighlightTileCell(snappedPosition, x, y);
    }

    private void PlaceObjectSingle(Vector3 position, int x, int y)
    {
        transform.position = position;
        gridManager.SetTileOccupied(x, y, true);

        Instantiate(placeAbleObject, transform.position, Quaternion.identity);
    }

    private void PlaceObjectMultiple(Vector3 position, int x, int y)
    {
        transform.position = position;
        gridManager.SetAreaOccupied(x, y, sizeX, sizeY, true);

        Instantiate(placeAbleObject, transform.position, Quaternion.identity);
    }

    private void RemoveObject(Vector3 position, int x, int y)
    {
        transform.position = position;
        gridManager.SetTileOccupied(x, y, false);

    }

    private void HighlightTileCell(Vector3 position, int startX, int startY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                if (x >= 0 && x < gridManager.gridWidth && y >= 0 && y < gridManager.gridWidth)
                {
                    GameObject tile = gridManager.GetTileAt(x, y);
                    if (tile != null)
                    {
                        obj.transform.position = position;

                        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();

                        foreach (SpriteRenderer renderer in renderers)
                        {
                            renderer.sortingOrder = 5;

                            if (gridManager.IsAreaOccupied(startX, startY, sizeX, sizeY))
                                renderer.color = Color.red;
                            else
                                renderer.color = Color.green;
                        }
                    }
                }
            }
        }
    }
}
