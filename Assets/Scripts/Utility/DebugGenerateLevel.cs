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

            int outsideRoomIndex = 1;
            if(map.GetTypeOfRoom(outsideRoomIndex) != RoomType.Outside)
            {
                Log.Error("Failed assert on outside room of generate");
            }

            map.ClearUnreachableTilesFrom(mapWidth / 2, mapHeight / 2);

            // split outside into regions
            LevelDesignManager.SplitRoomVoronoi(map,(Cell c) => c.roomIndex==outsideRoomIndex, map.Width * map.Height / 200);
        }

       

        if (map == null)
            return;

        GameManager.Instance.ClearUnits();


        mr.material.mainTexture = map.mapIntoTexture();
        mr.material.mainTexture.filterMode = FilterMode.Point;

        Func<List<Room>> func = () => map.GenerateTileMap();

        TribesManager.Instance.GenerateTribes(new List<int>() {2,2}, 2);

        GridManager.Instance.GenerateLevel(func, TribesManager.Instance.ProcessRooms);

        GameManager.Instance.ChangeState(GameManager.GameState.StartGame);
    }
}
