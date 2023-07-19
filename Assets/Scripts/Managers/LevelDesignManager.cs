using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.Mathematics;

public enum WorldType
{
    Island=0,
    DungeonRandomWalk=1,
    DungeonBSP=2,
}

[Serializable]
public struct DungeonSettings
{
    public Vector2 endPosition;
    public float randomMoveChance;
    public int numberOfAgents;
    public int roomWidth, roomHeight, corridorWidth, corridorLength;
    public float roomMergeChance;
}

public enum TileType
{
    Empty,
    Dirt,
    Wall,
    Cobblestone
}

public static class TileTypeExtensions
{
    private static HashSet<TileType> _isWalkable =
        new HashSet<TileType>() { TileType.Dirt, TileType.Cobblestone };
    public static bool IsWalkable(this TileType tileType) => _isWalkable.Contains(tileType);
}

public class LevelDesignManager : PersistentSingletonClass<LevelDesignManager>
{
    // world settings
    [SerializeField]
    private PlayerUnit hero;
    [SerializeField]
    private NPCUnit boss;
    [SerializeField]
    private DungeonSettings dungeonSettings;
    [SerializeField]
    private int numberOfDungeons;
    [SerializeField]
    private int mapWidth;
    [SerializeField]
    private int mapHeight;
    [SerializeField]
    private float minimalWalkableTileRatio;
    [SerializeField]
    private int drunkardsMaxPath;
    [SerializeField]
    private List<int> outsideTribesSizes;
    [SerializeField]
    private int insideTribeSize;
    [SerializeField]
    private WorldType worldType;
    [Space]
    public TileStandalone DirtTilePrefab;
    public TileStandalone WallTilePrefab;
    public TileStandalone CobblestoneTilePrefab;
    public TileStandalone WaterTilePrefab;
    [Space]
    public Tile2DTM DirtTile2DTMPrefab;
    public Tile2DTM WallTile2DTMPrefab;
    public Tile2DTM CobblestoneTile2DTMPrefab;
    public Tile2DTM WaterTile2DTMPrefab;


    public PlayerUnit Hero { get => hero; set => hero = value; }
    public DungeonSettings DungeonSettings { get => dungeonSettings; set => dungeonSettings = value; }
    public int NumberOfDungeons { get => numberOfDungeons; set => numberOfDungeons = value; }
    public int MapWidth { get => mapWidth; set => mapWidth = value; }

    public float MapWidthF { get => mapWidth; set => mapWidth = (int)value; }
    public int MapHeight { get => mapHeight; set => mapHeight = value; }

    public float MapHeightF { get => mapHeight; set => mapHeight = (int)value; }
    public float MinimalWalkableTileRatio { get => minimalWalkableTileRatio; set => minimalWalkableTileRatio = value; }
    public int DrunkardsMaxPath { get => drunkardsMaxPath; set => drunkardsMaxPath = value; }
    public List<int> OutsideTribesSizes { get => outsideTribesSizes; set => outsideTribesSizes = value; }
    public int InsideTribeSize { get => insideTribeSize; set => insideTribeSize = value; }
    public WorldType WorldType { get => worldType; set => worldType = value; }

    public int WorldTypeInt { get => (int)worldType; set => worldType = (WorldType)value; }

    public TileStandalone GetTileStandalonePrefab(TileType type)
    {
        return type switch
        {
            TileType.Dirt => DirtTilePrefab,
            TileType.Wall => WallTilePrefab,
            TileType.Cobblestone => CobblestoneTilePrefab,
            _ => WaterTilePrefab,
        };
    }

    public Tile2DTM GetTile2DTM(TileType type)
    {
        return type switch
        {
            TileType.Dirt => DirtTile2DTMPrefab,
            TileType.Wall => WallTile2DTMPrefab,
            TileType.Cobblestone => CobblestoneTile2DTMPrefab,
            _ => WaterTile2DTMPrefab,
        };
    }

    public ITile GetTilePrefab(byte index) => GetTileStandalonePrefab((TileType)index);

    /// <summary>
    /// Tries to place a dungeon onto the map that is reachable from certain position
    /// </summary>
    /// <param name="map"> Cell Map onto the dungeon is placed</param>
    /// <param name="dungeonSettings">Settings that will generate dungeon to be placed</param>
    /// <param name="spawnPoint"> a position from which must exist a path tu dungeon entrance </param>
    /// <returns>true if dungeon was placed succesfully</returns>
    public static bool PlaceWalledDungeon(CellMap map, DungeonSettings dungeonSettings,Vector2 spawnPoint, int spawnSafeArea = 10)
    {
        DungeonSettings dg = dungeonSettings;
        map.CalculateDistancesFrom((int)spawnPoint.x, (int)spawnPoint.y);
        Graph g = Graph.WalkToTarget(dg.endPosition, dg.randomMoveChance, dg.numberOfAgents);
        CellMap dungeon = g.GenerateCellMap(dg.roomWidth, dg.roomHeight, dg.corridorWidth, dg.corridorLength, dg.roomMergeChance);

        var adjecentCoords = new (int, int)[] {
                (-1,-1),(-1,0),(-1,1),
                (0,-1),        (0,1),
                (1,-1), (1,0), (1,1),
            };

        //pack dungeon in walls
        CellMap dungeonWithWalls = new CellMap(dungeon.Width + 2, dungeon.Height + 2);
        dungeonWithWalls.InsertMap(1, 1, dungeon);
        dungeonWithWalls.AddNewRoom(RoomType.Wall);

        for (int x = 0; x < dungeonWithWalls.Width; x++)
            for (int y = 0; y < dungeonWithWalls.Height; y++)
                if (dungeonWithWalls[x, y].type == 0)
                {
                    bool adjecentWalkableTile = false;
                    foreach (var (ax, ay) in adjecentCoords)
                        if (ax + x >= 0 && ax + x < dungeonWithWalls.Width &&
                           ay + y >= 0 && ay + y < dungeonWithWalls.Height &&
                           dungeonWithWalls[ax + x, ay + y].type == (byte)TileType.Cobblestone)
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
        // transform to cell position of entrance
        startX = startX * dg.roomWidth + startX * dg.corridorLength + MyRandom.Int(0, dg.roomWidth) + 1;

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
                    if (y >= startX && y + dungeonWithWalls.Width - startX < map.Height)
                        if (x >= dungeonWithWalls.Height)
                            if (map[x + 1, y].IsWalkable && map[x, y].distance >= map[x + 1, y].distance)
                                possibleDungEntrance.Add(((x, y), 3));
                }
                else
                {
                    if (y >= dungeonWithWalls.Width - startX && y + startX < map.Height)
                        if (x + dungeonWithWalls.Height <= map.Width)
                            if (map[x - 1, y].IsWalkable && map[x, y].distance >= map[x - 1, y].distance)
                                possibleDungEntrance.Add(((x, y), 1));
                }

                if (y < map.Height / 2)
                {
                    if (x >= dungeonWithWalls.Width - startX && x + startX < map.Width)
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
            return false;

        for (int entrancePlacementTry = 0; entrancePlacementTry < 100; entrancePlacementTry++)
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
                dungeonY + ((orientaion % 2) == 0 ? dungeon.Height + 2 : dungeon.Width + 2));
            Vector2 rightDownCorner = new Vector2(
                 dungeonX + ((orientaion % 2) == 0 ? dungeon.Width + 2 : dungeon.Height + 2),
                 dungeonY);

            if (map.protectedAreas.Any(
                d => MyMath.RectanglesOverlap(d.Item1, d.Item2, leftTopCorner, rightDownCorner))
                )
            {
                continue;
            }
            map.InsertMap(dungeonX, dungeonY, dungeonWithWalls, orientaion);
            map.protectedAreas.Add((leftTopCorner, rightDownCorner));
            return true;
        }
        return false;
    }

    public static void SplitRoomVoronoi(CellMap map,Predicate<Cell> predicate,int numberOfVoronoiCells)
    {
        // divide outside into rooms using voronoi graph, then fill them up
        List<Vector2> voronoiPList = new List<Vector2>();
        List<Color> voronoiColors = new List<Color>();
        List<List<(int, int)>> tilesOfVoronoiCellList = new List<List<(int, int)>>();
        for (int i = 0; i < numberOfVoronoiCells; i++)
        {
            voronoiPList.Add(new Vector2(MyRandom.Int(0, map.Width), MyRandom.Int(0, map.Height)));
            voronoiColors.Add(Color.HSVToRGB(MyRandom.Float(), MyRandom.Float(0.5f, 1f), MyRandom.Float(0.5f, 1f)));
            tilesOfVoronoiCellList.Add(new List<(int, int)>());
        }
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                //find closest voronoi point
                if (!predicate(map[x,y]))
                    continue;
                Vector2 v = new Vector2(x, y);
                int minPointIndex = 0;
                float minDist = voronoiPList[0].ManhattanDistance(v);
                for (int i = 1; i < numberOfVoronoiCells; i++)
                {
                    var dist = voronoiPList[i].ManhattanDistance(v);
                    if (minDist > dist)
                    {
                        minPointIndex = i;
                        minDist = dist;
                    }
                }
                tilesOfVoronoiCellList[minPointIndex].Add((x, y));

            }
        }
        //check for too small Cells (<=20)
        for (int i = 0; i < tilesOfVoronoiCellList.Count; i++)
        {
            var voronoiCell = tilesOfVoronoiCellList[i];
            if (voronoiCell.Count <= 20)
            {
                continue;
            }
            map.AddNewRoom(RoomType.OutsideEnemyCamp);

            foreach (var (x, y) in voronoiCell)
            {
                map.SetCell(x, y, (TileType)map[x,y].type);
            }
        }
    }

    private Vector2Int ChooseBossSpawnCell(Vector2Int heroStart,CellMap map) {
        List<Vector2Int> possibleBossSpawns = new List<Vector2Int>();
        for (int x = 0; x < map.Width; x++)
            for (int y = 0; y < map.Height; y++)
            {
                Cell c = map[x, y];
                if (map.RoomTypes.Count <= c.roomIndex)
                    continue;
                if (GetTilePrefab(c.type).IsWalkable && map.GetTypeOfRoom(c.roomIndex) == RoomType.End)
                    possibleBossSpawns.Add(new Vector2Int(x, y));
            }
        if (possibleBossSpawns.Count == 0)
        {
            for (int x = 0; x < map.Width; x++)
                for (int y = 0; y < map.Height; y++)
                {
                    Cell c = map[x, y];
                    if (map.RoomTypes.Count <= c.roomIndex)
                        continue;
                    if (GetTilePrefab(c.type).IsWalkable && map.GetTypeOfRoom(c.roomIndex) == RoomType.OutsideEnemyCamp)
                        possibleBossSpawns.Add(new Vector2Int(x, y));
                }
        }
        int minDistanceFromHero = math.min(15, possibleBossSpawns.Max(c => c.ManhattanDistance(heroStart)));
        Log.Debug($"Dist:{minDistanceFromHero}, possible:{possibleBossSpawns}");
        return MyRandom.Choice(possibleBossSpawns.Where(c => c.ManhattanDistance(heroStart) >= minDistanceFromHero).ToList());
    }

    public bool GenerateWorld()
    {
        CellMap map = null;
        Vector2Int heroStart = new Vector2Int(0,0);
        switch (WorldType)
        {
            case WorldType.Island:
                map = DrunkardWalk.Generate(mapWidth, mapHeight, RoomType.Outside, TileType.Dirt, minimalWalkableTileRatio, drunkardsMaxPath);
                heroStart = new Vector2Int(map.Width / 2, map.Height / 2);
                // place dungeons
                for (int i = 0; i < numberOfDungeons; i++)
                    PlaceWalledDungeon(map, dungeonSettings, heroStart);

                int outsideRoomIndex = 1;
                if (map.GetTypeOfRoom(outsideRoomIndex) != RoomType.Outside)
                {
                    Log.Error("Failed assert on outside room of generate");
                    return false;
                }

                map.ClearUnreachableTilesFrom((int)heroStart.x, (int)heroStart.y);

                // split outside into regions
                SplitRoomVoronoi(map, (Cell c) => c.roomIndex == outsideRoomIndex, map.Width * map.Height / 200);
                break;
            case WorldType.DungeonRandomWalk:
                var dg = dungeonSettings;
                var g = Graph.WalkToTarget(dg.endPosition, dg.randomMoveChance, dg.numberOfAgents, false);
                map = g.GenerateCellMap(dg.roomWidth, dg.roomHeight, dg.corridorWidth, dg.corridorLength, dg.roomMergeChance);
                heroStart = new Vector2Int(g.StartNodeDistanceXInGraph*(dg.roomWidth+dg.corridorWidth) + dg.roomWidth/2, dg.roomHeight/2);
                break;
            case WorldType.DungeonBSP:
                var settings = dungeonSettings;
                map = BSP.Generate(MapWidth, mapHeight,5, settings.corridorWidth,4);
                var walkableTiles = map.GetCells().Where(c => ((TileType)c.Item3.type).IsWalkable()).Select(c => (c.Item1,c.Item2));
                var heroStartentry = MyRandom.Choice(walkableTiles.ToList());
                heroStart = new Vector2Int(heroStartentry.Item1, heroStartentry.Item2);
                break;
            default:
                break;
        }

        
        Func<List<Room>> func = map.GenerateTileMap;

        TribesManager.Instance.GenerateTribes(outsideTribesSizes, insideTribeSize);

        GridManager.Instance.GenerateLevel(func, TribesManager.Instance.ProcessRooms);

        //summon hero
        Log.Debug($"Debug summon of {hero} at {heroStart}", gameObject);
        var unit = Instantiate(hero);
        GridManager.Instance.GetTile(heroStart)?.Occupy(unit);
        if (unit.CurrentTile == null)
        {
            GameManager.Instance.UnregisterUnit(unit);
            Destroy(unit.gameObject);
            Log.Error("! FAILED TO SUMMON HERO", gameObject);
            return false;
        }

        //summon boss
        var bossSpawnCell = ChooseBossSpawnCell(heroStart, map);
        var boss = Instantiate(this.boss);
        GameManager.Instance.Boss = (BossUnit)boss;
        GridManager.Instance.GetTile(new Vector2Int(bossSpawnCell.x, bossSpawnCell.y))?.Occupy(boss);
        if (boss.CurrentTile == null)
        {
            GameManager.Instance.UnregisterUnit(unit);
            Destroy(unit.gameObject);
            Log.Error("! FAILED TO SUMMON BOSS", gameObject);
            return false;
        }


        GameManager.Instance.ChangeState(GameManager.GameState.StartGame);
        return true;
    }

    /// <summary>
    /// This is a hack.
    /// We need to remove this manager when its not needed anymore
    /// so it does not mess up settings in main menu. (new one is created when return back to main menu)
    /// </summary>
    public void RemoveManagerInstance()
    {
        //sets instance to null and destroys gameObject
        OnApplicationQuit();
    }
}
