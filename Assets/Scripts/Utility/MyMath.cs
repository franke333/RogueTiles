using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMath 
{
    /// <summary>
    /// Returns true if the two rectangles overlap
    /// </summary>
    /// <param name="a_tl">Rectangle A TopLeft</param>
    /// <param name="a_dr">Recatngle A DownRight</param>
    /// <param name="b_tl">Rectangle B TopLeft</param>
    /// <param name="b_dr">Recatngle B DownRight</param>
    /// <returns>True if the two recatngles overlap</returns>
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

    /// <summary>
    /// Get "count" number of distinct colors using HSV color space
    /// </summary>
    /// <param name="count">number of colors</param>
    /// <returns>List of distinct colors</returns>
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
