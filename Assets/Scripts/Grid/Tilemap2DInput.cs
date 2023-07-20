using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A compliment class to Tile2DTM that handles input for the tilemap
/// </summary>
public class Tilemap2DInput : MonoBehaviour
{
    Tilemap tilemap;
    Tile2DTM hoveringOver;
    void Start()
    {
        tilemap = GridManager.Instance.Tilemap;
    }

    // Update is called once per frame
    void Update()
    {
        // get the tile that the mouse is hovering over
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var tilePos = tilemap.WorldToCell(mousePos);
        var tile = Tile2DTM.GetTile(tilePos.x, tilePos.y);

        // process input
        if (Input.GetMouseButtonDown(0) && hoveringOver != null)
            ButtonPress();
        if (tile == hoveringOver)
            return;
        if (tile != null)
        {
            if (hoveringOver != null)
                hoveringOver.Selected = false;
            hoveringOver = tile;
            tile.Selected = true;
        }
        
    }

    private void ButtonPress()
    {
        if (!hoveringOver.CheckSelectability())
            return;
        if (hoveringOver.InRange)
            GridManager.Instance.SelectedTileInRange = hoveringOver;
        GridManager.Instance.SetSelectedTile(hoveringOver);
    }
}
