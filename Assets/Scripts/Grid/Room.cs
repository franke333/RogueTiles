using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    public RoomType Type { get; set; }
    public string roomName;

    public List<Tile> GetRoomTiles { get => _roomTiles; }
    List<Tile> _roomTiles;

    public Room() {
        _roomTiles = new List<Tile>();
    }

    public static Room GenerateRectangleRoom(int x, int y,int width,int height, string roomName, RoomType roomType)
    {
        var go = new GameObject($"Room {roomName}");
        var room = go.AddComponent<Room>();
        room.Type = roomType;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var tile = Instantiate(GridManager.Instance.TilePrefab, new Vector3(x + i, y + j), Quaternion.identity, room.transform);
                tile.Init((x + y + i + j) % 2 == 0, x + i, y + j,room);
                room._roomTiles.Add(tile);
            }
        }
        return room;
    }
}

public enum RoomType
{
    Outside,


    // Graph Rooms
    Start,
    End,
    Hall,
    Treasure,
    Corridor,
    Wall
}


