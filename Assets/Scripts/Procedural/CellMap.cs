using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    public byte type;
    public int roomIndex;
    public int distance;
}

public class CellMap
{
    // byte map. bool could be enough, but in the matter of mem size there is no difference
    // and we can more easily expand byte map

    //[x,y]
    Cell[,] _map;

    int _maxRoomIndex;
    
    
    

    List<RoomType> _roomTypes;

    public int Height { get; private set; }
    public int Width { get; private set; }

    public (int, int) DistanceStartPoint { get; private set; }

    public int WalkableReachableTilesCount { get; private set; }

    public Cell this[int x,int y]
    {
        get => _map[x, y];
        set => _map[x, y] = value;
    }

    public Cell this[(int,int) pos] => this[pos.Item1,pos.Item2];

    public void SetCell(int x,int y,TileType tileType)
    {
        if(_maxRoomIndex == 0)
        {
            Log.Error("No room set up for CellMap", null);
            return;
        }
        _map[x, y] = new Cell() { type = (byte)tileType, roomIndex = _maxRoomIndex };
    }

    public void SetCellOfRoom(int x, int y, TileType tileType,int roomIndex)
    {
        if(roomIndex > _maxRoomIndex || roomIndex < 0)
        {
            Log.Error("Index must be in range of rooms in CellMap", null);
            return;
        }
        _map[x, y] = new Cell() { type = (byte)tileType, roomIndex = roomIndex };
    }

    /// <summary>
    /// Add new room to Cell Map
    /// </summary>
    /// <returns> index of added room </returns>
    public int AddNewRoom(RoomType roomType)
    {
        _roomTypes.Add(roomType);
        return ++_maxRoomIndex;
    }

    public RoomType GetTypeOfRoom(int index) => _roomTypes[index];

    public CellMap(int width,int height)
    {
        Width = width;
        Height = height;

        _map = new Cell[width, height];
        _maxRoomIndex = 0;
        _roomTypes = new List<RoomType>();
    }


    // BFS
    public void CalculateDistancesFrom(int x,int y)
    {
        if(!((TileType)_map[x,y].type).IsWalkable())
        {
            Log.Error("Starting point is unwalkable!!!");
            return;
        }

        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                _map[i, j].distance = -1;

        _map[x, y].distance = 0;
        WalkableReachableTilesCount = 0;

        List<(int, int)> listCurrentPositions, listNextPositions = new List<(int, int)>();
        listNextPositions.Add((x, y));
        while(listNextPositions.Count > 0)
        {
            listCurrentPositions = listNextPositions;
            listNextPositions = new List<(int, int)>();
            foreach(var (cx,cy) in listCurrentPositions)
            {
                (int, int)[] neighbours = new (int, int)[] { (cx+1,cy), (cx-1, cy), (cx, cy+1), (cx, cy-1) };
                foreach(var (nx,ny) in neighbours)
                {
                    if (nx < 0 || ny < 0 || nx == Width || ny == Height)
                        continue;
                    if (_map[nx,ny].distance == -1 && ((TileType)_map[nx, ny].type).IsWalkable())
                    {
                        WalkableReachableTilesCount++;
                        _map[nx, ny].distance = _map[cx, cy].distance + 1;
                        listNextPositions.Add((nx, ny));
                    }
                }
            }
        }

        Log.Info($"Recalculated distances from {x} {y}, coverage is {(float)WalkableReachableTilesCount / (Width * Height)}", null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rotate"> rotate clockwise 90° n times (4=0)</param>
    /// <returns> return the index that would correspond to the 0 index of insertedMap </returns>
    public int InsertMap(int x,int y,CellMap insertedMap,int rotate = 0)
    {
        Func<int, int, (int, int)> rotCoords;

        // anit-clockwise rotation as we will be asking cell where it wes before rotation
        switch (rotate%4)
        {
            case 1:
                rotCoords = (int x,int y) => (y, insertedMap.Height - x - 1);
                break;
            case 2:
                rotCoords = (int x, int y) => (insertedMap.Width - y - 1, insertedMap.Height - x - 1);
                break;
            case 3:
                rotCoords = (int x, int y) => (insertedMap.Width - y - 1, x);
                break;
            case 0:
            default:
                rotCoords = (int x, int y) => (x, y);
                break;
        }

        if (rotate % 2 == 0)
        {
            if (x + insertedMap.Width > Width || y + insertedMap.Height > Height)
            {
                Log.Error($"Inserting map out of bounds\n{Width}:{Height} and inserted map was from" +
                    $"{x}:{y} to {x + insertedMap.Width}:{y + insertedMap.Height} upper bounds excluded", null);
                return -1;
            }
        }
        else
        {
            if (x + insertedMap.Height > Width || y + insertedMap.Width > Height)
            {
                Log.Error($"Inserting map out of bounds\n{Width}:{Height} and inserted map was from" +
                    $"{x}:{y} to {x + insertedMap.Width}:{y + insertedMap.Height} upper bounds excluded", null);
                return -1;
            }
        }

        for (int i = 0; i < (rotate % 2 == 0 ? insertedMap.Width : insertedMap.Height); i++)
            for (int j = 0; j < (rotate % 2 == 0 ? insertedMap.Height : insertedMap.Width ); j++)
                if (insertedMap[rotCoords(i, j)].type != 0)
                {
                    _map[i + x, j + y] = insertedMap[rotCoords(i,j)];
                    _map[i + x, j + y].roomIndex += _maxRoomIndex;
                }
        int returnValue = _maxRoomIndex;
        _maxRoomIndex += insertedMap._maxRoomIndex;
        _roomTypes.AddRange(insertedMap._roomTypes);

        return returnValue;
    }

    public int MergeRooms(List<int> indexes,RoomType newRoomType)
    {
        int count = 0;
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (indexes.Contains(_map[x,y].roomIndex))
                {
                    _map[x, y].roomIndex = indexes[0];
                    count++;
                }

        _roomTypes[indexes[0]] = newRoomType;
        return count;
    }

    /// <summary>
    /// Generates tiles and rooms from cell map
    /// </summary>
    public List<Room> GenerateTileMap()
    {

        var rooms = new List<Room>();
        for (int i = 0; i < _maxRoomIndex; i++) 
        {
            var go = new GameObject($"Room {_roomTypes[i]}");
            var room = go.AddComponent<Room>();
            room.Type = _roomTypes[i];

            rooms.Add(room);
        }

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if ( _map[x, y].type != 0)
                {
                    Cell c = _map[x, y];

                    int roomIndex = c.roomIndex - 1;
                    Tile tilePrefab = LevelDesignManager.Instance.GetTilePrefab(c.type);
                    var tile = GameObject.Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, rooms[roomIndex].transform);
                    tile.Init((x + y)% 2 == 0, x, y, rooms[roomIndex]);
                    rooms[roomIndex].GetRoomTiles.Add(tile);
                }
        rooms.RemoveAll(r => r.GetRoomTiles.Count == 0);
        return rooms;
    }




    //Debug purposes
    public Texture2D mapIntoTexture()
    {
        Dictionary<byte, Color> typeToColor = new Dictionary<byte, Color>()
        {
            {(byte)TileType.Empty,Color.black },
            {(byte)TileType.Dirt,new Color(.7f,.4f,0)},
            {(byte)TileType.Cobblestone, new Color(.7f,.7f,.7f)},
            {(byte)TileType.Wall, new Color(.4f,.4f,.4f)}
        };

        Texture2D texture = new Texture2D(Width, Height);
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                texture.SetPixel(x, y, typeToColor[_map[x, y].type]);
            }
        }

        texture.Apply();
        return texture;
    }
}
