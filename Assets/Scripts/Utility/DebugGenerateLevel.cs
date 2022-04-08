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
    public MeshRenderer voronoiMr, voronoiMr2;
    public int voronoiPointsCount;
    public float enemyCampProb;
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

            int numberOfDungeons = MyRandom.Int(1, 3);

            var adjecentCoords = new (int,int)[] { 
                (-1,-1),(-1,0),(-1,1),
                (0,-1),        (0,1),
                (1,-1), (1,0), (1,1),
            };
            int maxUnsucessfulTries = 15;
            List<(Vector2,Vector2)> listOfDungeonPlacements = new List<(Vector2, Vector2)>();

            map.CalculateDistancesFrom(map.Width / 2, map.Height / 2);

            // place dungeons
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



                //Enumerate all possible positions for dungeon placing so the entrance is accessable
                List<((int, int), int)> possibleDungEntrance = new List<((int, int), int)>();

                //check for possible entrances
                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        if (Math.Abs(x - map.Width / 2) < spawnSafeArea || Math.Abs(y - map.Height / 2) < spawnSafeArea)
                            continue;

                        if (map[x, y].type != (byte)TileType.Dirt)
                            continue;

                        if (x < map.Width / 2)
                        {
                            if (y >= startX && y + dungeonWithWalls.Width - startX  < map.Height)
                                if (x >= dungeonWithWalls.Height)
                                    if (map[x + 1, y].IsWalkable && map[x, y].distance >= map[x + 1, y].distance)
                                        possibleDungEntrance.Add(((x, y), 3));
                        }
                        else
                        {
                            if (y >= dungeonWithWalls.Width - startX && y + startX  < map.Height)
                                if (x + dungeonWithWalls.Height <= map.Width)
                                    if (map[x - 1, y].IsWalkable && map[x, y].distance >= map[x - 1, y].distance)
                                        possibleDungEntrance.Add(((x, y), 1));
                        }
        
                        if (y < map.Height / 2)
                        {
                            if (x >= dungeonWithWalls.Width - startX && x + startX  < map.Width)
                                if (y >= dungeonWithWalls.Height)
                                    if (map[x, y + 1].IsWalkable && map[x, y].distance >= map[x, y + 1].distance)
                                        possibleDungEntrance.Add(((x, y), 2));
                        }
                        else
                        {
                            if (x >= startX && x + dungeonWithWalls.Width - startX < map.Width)
                                if (y + dungeonWithWalls.Height <= map.Height)
                                    if (map[x, y - 1].IsWalkable && map[x, y].distance >= map[x, y - 1].distance)
                                        possibleDungEntrance.Add(((x, y), 0));
                        }
                    }
                }


                if (possibleDungEntrance.Count == 0)
                {
                    Log.Error("No possible dungeon entrance", null);
                    if (maxUnsucessfulTries > 0)
                    {
                        maxUnsucessfulTries--;
                        i--;
                    }
                    continue;
                }

                for (int entrancyTry = 0; entrancyTry < 100; entrancyTry++)
                {
                    var ((dungeonXEntrance, dungeonYEntrance), orientaion) = MyRandom.Choice(possibleDungEntrance);
                    int dungeonX = 0, dungeonY = 0;
                    switch (orientaion)
                    {
                        case 0:
                            dungeonX = dungeonXEntrance - startX;
                            dungeonY = dungeonYEntrance;
                            break;
                        case 1:
                            dungeonX = dungeonXEntrance;
                            dungeonY = dungeonYEntrance + startX - dungeonWithWalls.Width + 1;
                            break;
                        case 2:
                            dungeonX = dungeonXEntrance + startX - dungeonWithWalls.Width + 1;
                            dungeonY = dungeonYEntrance - dungeonWithWalls.Height;
                            break;
                        case 3:
                            dungeonX = dungeonXEntrance - dungeonWithWalls.Height;
                            dungeonY = dungeonYEntrance - startX;
                            break;
                    }

                    Vector2 leftTopCorner = new Vector2(
                        dungeonX,
                        dungeonY + ((orientaion%2)==0 ? dungeon.Height + 2 : dungeon.Width +2));
                    Vector2 rightDownCorner = new Vector2(
                         dungeonX + ((orientaion % 2) == 0 ? dungeon.Width + 2 : dungeon.Height + 2),
                         dungeonY);

                    if (listOfDungeonPlacements.Any(
                        d => MyMath.RectanglesOverlap(d.Item1,d.Item2,leftTopCorner,rightDownCorner)))
                    {
                        // there is an overlap, check if this is last option
                        if (entrancyTry == listOfDungeonPlacements.Count - 1)
                        {
                            Log.Error("No possible dungeon entrance due to overlaps", null);
                            if (maxUnsucessfulTries > 0)
                            {
                                maxUnsucessfulTries--;
                                i--;
                            }
                        }
                        continue;
                    }
                    Log.Debug($"{orientaion}");
                    map.InsertMap(dungeonX, dungeonY, dungeonWithWalls, orientaion);
                    listOfDungeonPlacements.Add((leftTopCorner, rightDownCorner));
                    break;
                }
            }

            map.ClearUnreachableTilesFrom(mapWidth / 2, mapHeight / 2);

            //here we should check that all important entrances are avaliable and if not, reset generation

            // divide outside into rooms using voronoi graph, then fill them up
            List<Vector2> voronoiPList = new List<Vector2>();
            List<Color> voronoiColors = new List<Color>();
            Texture2D voronoiTexture = new Texture2D(mapWidth,mapHeight);
            Texture2D voronoiTextureMap = new Texture2D(mapWidth, mapHeight);
            List<List<(int,int)>> tilesOfVoronoiCellList = new List<List<(int, int)>>();
            for (int i = 0; i < voronoiPointsCount; i++)
            {
                voronoiPList.Add(new Vector2(MyRandom.Int(0, mapWidth), MyRandom.Int(0, mapHeight)));
                voronoiColors.Add(Color.HSVToRGB(MyRandom.Float(), MyRandom.Float(0.5f,1f), MyRandom.Float(0.5f, 1f)));
                tilesOfVoronoiCellList.Add(new List<(int, int)>());
            }
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    //find closest voronoi point
                    Vector2 v = new Vector2(x, y);
                    int minPointIndex = 0;
                    float minDist = voronoiPList[0].ManhattanDistance(v);
                    for (int i = 1; i < voronoiPointsCount; i++)
                    {
                        var dist = voronoiPList[i].ManhattanDistance(v);
                        if (minDist > dist)
                        {
                            minPointIndex = i;
                            minDist = dist;
                        }
                    }

                    //add to some list of lists?
                    if(map[x,y].type == (byte)TileType.Dirt)
                        tilesOfVoronoiCellList[minPointIndex].Add((x,y));


                    if(minDist==0)
                        voronoiTexture.SetPixel(x, y,Color.black);
                    else
                        voronoiTexture.SetPixel(x, y, voronoiColors[minPointIndex]);
                        
                }
            }
            //check for too small Cells (lets say 30 for now)
            for (int i = 0; i < tilesOfVoronoiCellList.Count; i++)
            {
                var voronoiCell = tilesOfVoronoiCellList[i];
                if (voronoiCell.Count < 30)
                {
                    foreach(var (x,y) in voronoiCell)
                        voronoiTextureMap.SetPixel(x, y, Color.black);
                    continue;
                }
                // new outside room types here 
                if (MyRandom.Float() <= enemyCampProb)
                    map.AddNewRoom(RoomType.OutsideEnemyCamp);
                else
                    map.AddNewRoom(RoomType.Outside);

                foreach (var (x, y) in voronoiCell)
                {
                    map.SetCell(x, y, TileType.Dirt);
                    voronoiTextureMap.SetPixel(x, y, voronoiColors[i]);
                }
            }
            voronoiTexture.Apply();
            voronoiMr.material.mainTexture = voronoiTexture;
            voronoiMr.material.mainTexture.filterMode = FilterMode.Point;
            voronoiTextureMap.Apply();
            voronoiMr2.material.mainTexture = voronoiTextureMap;
            voronoiMr2.material.mainTexture.filterMode = FilterMode.Point;
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
