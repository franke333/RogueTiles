using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Room represents a collection of tiles.
/// Map is divided into rooms.
/// </summary>
public class Room : MonoBehaviour
{
    /// <summary>
    /// Room type
    /// </summary>
    public RoomType Type { get; set; }
    public string RoomName;

    /// <summary>
    /// Index of the tribe that occupies this room
    /// </summary>
    public int TribeOccupation { get; set; }

    public List<ITile> GetRoomTiles { get => _roomTiles; }
    List<ITile> _roomTiles;

    public Room() {
        _roomTiles = new List<ITile>();
    }
}

public enum RoomType
{
    //overworld rooms
    Water,
    Outside,
    OutsideEnemyCamp,


    // Graph Rooms
    BossRoom,
    Start,
    End,
    Hall,
    Treasure,
    Corridor,
    Wall,
}


