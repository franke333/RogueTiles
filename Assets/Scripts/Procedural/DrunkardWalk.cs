using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkardWalk
{
    private static int maxStartingPositionTries = 3;

    public static CellMap Generate(int width,int height, RoomType roomType,TileType tileType, int numOfAgents, int maxStepsOfAgent)
    {
        CellMap cm = new CellMap(width, height);
        var center = (width / 2, height / 2);

        cm.AddNewRoom(roomType);

        cm.SetCell(center.Item1, center.Item2, tileType);

        List<(int,int)> possibleMoves = new List<(int, int)>(){ (0,1),(0,-1),(1,0),(-1,0) };

        for (int i = 0; i < numOfAgents; i++)
        {
            var (x,y) = center;

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
                cm.SetCell(x, y, tileType);
            }

        }

        return cm;
    }
}
