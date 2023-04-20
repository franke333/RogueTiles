using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    public RoomType Type { get; set; }
    public string roomName;
    public int TribeOccupation { get; set; }

    public List<ITile> GetRoomTiles { get => _roomTiles; }
    List<ITile> _roomTiles;

    public Room() {
        _roomTiles = new List<ITile>();
    }
}

public enum RoomType
{
    Water,

    Outside,
    OutsideEnemyCamp,
    BossRoom,

    // Graph Rooms
    Start,
    End,
    Hall,
    Treasure,
    Corridor,
    Wall
}


