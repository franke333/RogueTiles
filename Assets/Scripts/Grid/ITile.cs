using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface ITile
{
    public Room Room { get; set; }

    public int x { get; set; }
    public int y { get; set; }

    public GridObject GetObject { get; }
    public ITile Init(bool isOdd, int x, int y, Room room);
    public void TintBaseColor(Color color, float by);

    public bool Visible { get; set; }

    public bool IsWall { get; }

    public bool IsWalkable { get; }

    public bool InRange { get; set; }

    public bool Selected { get; set; }

    public void Occupy(GridObject obj);

    public void Refresh();

}

public static class ITileExtensions
{
    public static int ManhattanDistance(this ITile a, ITile b)
    {
        return math.abs(a.x - b.x) + math.abs(a.y - b.y);
    }
}
