using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugGenerateLevel : MonoBehaviour
{
    public Vector2 endPosition;
    public float randomMoveChance;
    public int numberOfAgents;
    public int roomWidth, roomHeight, corridorWidth, corridorLength;
    public float roomMergeChance;
    [Space]
    public int mapWidth;
    public int mapHeight;
    public int drunkards;
    public int drunkardsMaxPath;

    public MeshRenderer mr;

    

    void Update()
    {
        CellMap map = null;
        if (Input.GetKeyDown(KeyCode.M))
        {
             map = DrunkardWalk.Generate(mapWidth, mapHeight, RoomType.Outside, TileType.Dirt, drunkards, drunkardsMaxPath);

        }
        if(Input.GetKeyDown(KeyCode.N))
        {
             map = Graph.WalkToTarget(endPosition, randomMoveChance, numberOfAgents)
                .GenerateCellMap(roomWidth, roomHeight, corridorWidth, corridorLength, roomMergeChance);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            map = DrunkardWalk.Generate(mapWidth, mapHeight, RoomType.Outside, TileType.Dirt, drunkards, drunkardsMaxPath);

            int numberOfDungeons = MyRandom.Int(1, 3) + MyRandom.Int(1, 2);

            var adjecentCoords = new (int,int)[] { 
                (-1,-1),(-1,0),(-1,1),
                (0,-1),        (0,1),
                (1,-1), (1,0), (1,1),
            };

            for (int i = 0; i < numberOfDungeons; i++)
            {
                CellMap dungeon = Graph.WalkToTarget(endPosition, randomMoveChance, numberOfAgents)
                .GenerateCellMap(roomWidth, roomHeight, corridorWidth, corridorLength, roomMergeChance);

                //pack dungeon in walls
                CellMap dungeonWithWalls = new CellMap(dungeon.Width+2, dungeon.Height+2);
                dungeonWithWalls.InsertMap(1, 1, dungeon);
                dungeonWithWalls.AddNewRoom(RoomType.Wall);
                for (int x = 0; x < dungeonWithWalls.Width; x++)
                    for (int y = 0; y < dungeonWithWalls.Height; y++)
                        if (dungeonWithWalls[x, y].type == 0) 
                        {
                            bool adjecentWalkableTile = false;
                            foreach (var (ax,ay) in adjecentCoords)
                                if(ax + x >= 0 && ax + x < dungeonWithWalls.Width &&
                                   ay + y >= 0 && ay + y < dungeonWithWalls.Height &&
                                   dungeonWithWalls[ax+x,ay+y].type == (byte)TileType.Cobblestone)
                                {
                                    adjecentWalkableTile = true;
                                    break;
                                }
                            if (adjecentWalkableTile)
                                dungeonWithWalls.SetCell(x, y, TileType.Wall);

                        }

                var (dungeonX, dungeonY) = (MyRandom.Int(0, mapWidth - dungeon.Width -2), MyRandom.Int(0, mapHeight - dungeon.Height -2));

                map.InsertMap(dungeonX,dungeonY, dungeonWithWalls,1);
            }
        }


        if (map == null)
            return;


        map.CalculateDistancesFrom(map.Width / 2, map.Height / 2);
        mr.material.mainTexture = map.mapIntoTexture();
        mr.material.mainTexture.filterMode = FilterMode.Point;

        Func<List<Room>> func = () => map.GenerateTileMap();

        GridManager.Instance.GenerateLevel(func, null);

        GameManager.Instance.ChangeState(GameManager.GameState.StartGame);
    }
}
