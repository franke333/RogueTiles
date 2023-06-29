using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMath 
{
    public static bool RectanglesOverlap(Vector2 a_tl,Vector2 a_dr, Vector2 b_tl, Vector2 b_dr)
    {
        // If one rectangle is on left side of other
        if (a_tl.x >= b_dr.x || b_tl.x >= a_dr.x)
        {
            return false;
        }

        // If one rectangle is above other
        if (a_dr.y >= b_tl.y || b_dr.y >= a_tl.y)
        {
            return false;
        }
        return true;
    }

    public static List<Color> GetDistinctColors(int count)
    {
        List<Color> colors = new List<Color>();
        for (int i = 0; i < count; i++)
        {
            colors.Add(Color.HSVToRGB(((float)i)/count,1,1));
        }
        return colors;
    }
}

public static class Vector2Extension
{
    public static float ManhattanDistance(this Vector2 a, Vector2 b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    public static int ManhattanDistance(this Vector2Int a, Vector2Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
}
