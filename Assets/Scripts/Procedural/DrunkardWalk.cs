using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Random Walk algorithm that generates a CellMap
/// </summary>
public class DrunkardWalk
{
    private static int maxStartingPositionTries = 6;

    /// <summary>
    /// Generate a CellMap using the Drunkard Walk algorithm
    /// </summary>
    /// <param name="width">Width of the map</param>
    /// <param name="height">Height of the map</param>
    /// <param name="roomType">roomType of the created room</param>
    /// <param name="tileType">tileType the drunkard applies when walking</param>
    /// <param name="ratioWalkableTiles">How many tiles should the drunkard visit until the algorithm finishes</param>
    /// <param name="maxStepsOfAgent">How many steps before creating new drunkard</param>
    /// <returns></returns>
    public static CellMap Generate(int width,int height, RoomType roomType,TileType tileType, float ratioWalkableTiles, int maxStepsOfAgent)
    {
        CellMap cm = new CellMap(width, height);
        var center = (width / 2, height / 2);

        int tilesInTotal = width * height;
        int walkableTiles = 0;

        cm.AddNewRoom(roomType);

        cm.SetCell(center.Item1, center.Item2, tileType);

        List<(int,int)> possibleMoves = new List<(int, int)>(){ (0,1),(0,-1),(1,0),(-1,0) };

        while (walkableTiles < ratioWalkableTiles*tilesInTotal)
        {
            var (x,y) = center;

            //try new placement for agent
            for (int j = 0; j < maxStartingPositionTries; j++)
            {
                var (newx, newy) = (MyRandom.Int(0, width), MyRandom.Int(0, height));
                if (cm[newx, newy].roomIndex == 1)
                    (x, y) = (newx, newy);
            }

            for (int step = 0; step < maxStepsOfAgent; step++)
            {
                var (dx,dy) = MyRandom.Choice(possibleMoves);
                (x, y) = (x + dx, y + dy);
                //check if agent left the map, if yes -> summon new agent instead
                if (x < 0 || y < 0 || x == width || y == height)
                    break;
                //check if tile wasn't already visited
                if (cm[x, y].roomIndex != 1)
                {
                    cm.SetCell(x, y, tileType);
                    walkableTiles++;
                }
            }

        }

        return cm;
    }
}
