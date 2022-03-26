using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    [Space]
    public int spawnSafeArea = 10;

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

            int numberOfDungeons = 1;//MyRandom.Int(1, 3) + MyRandom.Int(1, 2);

            var adjecentCoords = new (int,int)[] { 
                (-1,-1),(-1,0),(-1,1),
                (0,-1),        (0,1),
                (1,-1), (1,0), (1,1),
            };

            for (int i = 0; i < numberOfDungeons; i++)
            {
                Graph g = Graph.WalkToTarget(endPosition, randomMoveChance, numberOfAgents);
                CellMap dungeon = g.GenerateCellMap(roomWidth, roomHeight, corridorWidth, corridorLength, roomMergeChance);

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

                // start room position in nodes
                int startY = 0;
                int startX = g.StartNodeDistanceXInGraph;
                // transform to cell position
                startX = startX * roomWidth + startX * corridorLength + MyRandom.Int(0,roomWidth) + 1 ;

                // make entrance
                dungeonWithWalls.SetCellOfRoom(startX, startY, TileType.Cobblestone, 1);


                //Enumerate all possible positions for dungeon so the entrance is accessable
                map.CalculateDistancesFrom(map.Width / 2, map.Height / 2);
                List<((int, int), int)> possibleDungEntrance = new List<((int, int), int)>();
                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        if (Math.Abs(x - map.Width / 2) < spawnSafeArea || Math.Abs(y - map.Height / 2) < spawnSafeArea)
                            continue;

                        if (!map[x, y].IsWalkable)
                            continue;

                        if (y >= startX && y + dungeonWithWalls.Width - startX < map.Height) {
                            if (x < map.Width / 2)
                            {
                                if (x >= dungeonWithWalls.Height)
                                    if (map[x + 1, y].IsWalkable && map[x, y].distance >= map[x + 1, y].distance)
                                        possibleDungEntrance.Add(((x, y), 3));
                            }
                            else
                            {
                                if (x + dungeonWithWalls.Height <= map.Width)
                                    if (map[x - 1, y].IsWalkable && map[x, y].distance >= map[x - 1, y].distance)
                                        possibleDungEntrance.Add(((x, y), 1));
                            }
                        }
                        if (x >= startX && x + dungeonWithWalls.Width - startX < map.Width) {
                            if (y < map.Height / 2)
                            {
                                if(y >= dungeonWithWalls.Height)
                                    if (map[x, y + 1].IsWalkable && map[x, y].distance >= map[x, y + 1].distance)
                                        possibleDungEntrance.Add(((x, y), 2));
                            }
                            else
                            {
                                if (y + dungeonWithWalls.Height <= map.Height)
                                    if (map[x, y - 1].IsWalkable && map[x, y].distance >= map[x, y - 1].distance)
                                        possibleDungEntrance.Add(((x, y), 0));
                            }
                        }
                    }
                }




                
                var ((dungeonXEntrance, dungeonYEntrance), orientaion) = 
                    MyRandom.Choice(possibleDungEntrance);
                Log.Debug($"orient = {orientaion}", null);
                Log.Debug($"orient 2 = {possibleDungEntrance.Where(x => x.Item2 == 2).Count()}");
                Log.Debug($"trying spawn entrance at {dungeonXEntrance} {dungeonYEntrance} with dungeon of size " +
                    $"{dungeonWithWalls.Width} {dungeonWithWalls.Height} at map {map.Width} {map.Height}",null);
                int dungeonX = 0, dungeonY = 0;
                switch (orientaion)
                {
                    case 0:
                        dungeonX = dungeonXEntrance - startX;
                        dungeonY = dungeonYEntrance;
                        break;
                    case 1:
                        dungeonX = dungeonXEntrance;
                        dungeonY = dungeonYEntrance + startX - dungeonWithWalls.Width;
                        break;
                    case 2:
                        dungeonX = dungeonXEntrance + startX -dungeonWithWalls.Width;
                        dungeonY = dungeonYEntrance - dungeonWithWalls.Height;
                        break;
                    case 3:
                        dungeonX = dungeonXEntrance - dungeonWithWalls.Height;
                        dungeonY = dungeonYEntrance - startX;
                        break;
                }

                map.InsertMap(dungeonX,dungeonY, dungeonWithWalls,orientaion);
            }
        }


        if (map == null)
            return;

        mr.material.mainTexture = map.mapIntoTexture();
        mr.material.mainTexture.filterMode = FilterMode.Point;

        Func<List<Room>> func = () => map.GenerateTileMap();

        GridManager.Instance.GenerateLevel(func, null);

        GameManager.Instance.ChangeState(GameManager.GameState.StartGame);
    }
}
