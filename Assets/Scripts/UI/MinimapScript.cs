using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System.Linq;
using System;


/// <summary>
/// Minimap script
/// </summary>
public class MinimapScript : MonoBehaviour
{
    Texture2D _map;
    [SerializeField]
    RawImage _minimapImageObject;
    [SerializeField]
    Image _playerIcon, _bossIcon;

    GridUnit _playerUnit, _bossUnit;
    Vector2Int _playerPos, _bossPos;

    void Start()
    {
        //coroutine to wait for start of game and then create minimap
        StartCoroutine(GameManager.WaitForStart(CreateMap));
    }

    // method to create Sprite for minimap
    private void CreateMap()
    {
        var (width,height) = (GridManager.Instance.Width, GridManager.Instance.Height);
        int spriteSize = math.max(width,height);
        _map = new Texture2D(width,height);
        _map.filterMode = FilterMode.Point;
        _map.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _map.SetPixel(x, y, GridManager.Instance.GetTile(new Vector2Int(x,y)).GetColor());
            }
        }
        _map.Apply();

        _minimapImageObject.texture = _map;
        
        Log.Debug("Minimap created", gameObject);

        _playerUnit = GameManager.Instance.Player;
        _bossUnit = GameManager.Instance.Boss;
    }

    /// <summary>
    /// Update the position of the unit on the minimap
    /// </summary>
    /// <param name="newPos">new position of the unit</param>
    /// <param name="icon">icon that represent the unit</param>
    private void UpdateMinimapUnitPos(Vector2Int newPos, Image icon)
    {
        float ratioX = (float)newPos.x / _map.width;
        float ratioY = (float)newPos.y / _map.height;

        icon.rectTransform.anchoredPosition = new Vector2(
            (_minimapImageObject.rectTransform.rect.width * ratioX) - (_minimapImageObject.rectTransform.rect.width * 0.5f),
            (_minimapImageObject.rectTransform.rect.height * ratioY) - (_minimapImageObject.rectTransform.rect.height * 0.5f));
    }

    private void Update()
    {
        if(_playerUnit!= null)
        {
            var _currentPlayerPos = new Vector2Int(_playerUnit.CurrentTile.x, _playerUnit.CurrentTile.y);
            if (_playerPos != _currentPlayerPos)
            {
                _playerPos = _currentPlayerPos;
                UpdateMinimapUnitPos(_playerPos, _playerIcon);
            }
        }
        if (_bossUnit != null)
        {
            var _currentBossPos = new Vector2Int(_bossUnit.CurrentTile.x, _bossUnit.CurrentTile.y);
            if (_bossPos != _currentBossPos)
            {
                _bossPos = _currentBossPos;
                UpdateMinimapUnitPos(_bossPos, _bossIcon);
            }
        }
    }
}
