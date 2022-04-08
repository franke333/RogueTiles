using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DebugGenerateLevel : MonoBehaviour
{
    public DungeonSettings dungeonSettings;
    public int numberOfDungeons;
    [Space]
    public int mapWidth;
    public int mapHeight;
    public int drunkards;
    public int drunkardsMaxPath;

    public MeshRenderer mr;
    public MeshRenderer voronoiMr, voronoiMr2;
    public int voronoiPointsCount;
    public float enemyCampProb;
    

    void Update()
    {
        

        CellMap map = null;

        if (Input.GetKeyDown(KeyCode.M))
        {
             map = DrunkardWalk.Generate(mapWidth, mapHeight, RoomType.Outside, TileType.Dirt, drunkards, drunkardsMaxPath);

        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            var dg = dungeonSettings;
             map = Graph.WalkToTarget(dg.endPosition, dg.randomMoveChance, dg.numberOfAgents)
                .GenerateCellMap(dg.roomWidth, dg.roomHeight, dg.corridorWidth, dg.corridorLength, dg.roomMergeChance);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            map = DrunkardWalk.Generate(mapWidth, mapHeight, RoomType.Outside, TileType.Dirt, drunkards, drunkardsMaxPath);
           
            // place dungeons
            for (int i = 0; i < numberOfDungeons; i++)
                LevelDesignManager.PlaceWalledDungeon(map, dungeonSettings, new Vector2(map.Width / 2, map.Height / 2));


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
        map.ClearUnreachableTilesFrom(mapWidth / 2, mapHeight / 2);
        mr.material.mainTexture = map.mapIntoTexture();
        mr.material.mainTexture.filterMode = FilterMode.Point;

        Func<List<Room>> func = () => map.GenerateTileMap();

        GridManager.Instance.GenerateLevel(func, null);

        GameManager.Instance.ChangeState(GameManager.GameState.StartGame);
    }
}
