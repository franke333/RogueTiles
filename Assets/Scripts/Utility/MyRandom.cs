using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class MyRandom
{


    public static void SetState(int seed)
    {
        Random.InitState(seed);
    }

    public static int Int(int minValueInc = int.MinValue,int maxValueExc = int.MaxValue)
        => Random.Range(minValueInc, maxValueExc);

    public static float Float(float maxValue = 1.0f)
        => Random.Range(0, maxValue);

    public static float Float(float minValue, float maxValue)
        => Random.Range(minValue, maxValue);

    public static T Choice<T>(IList<T> list)
    {
        if (list.Count == 0)
            throw new System.Exception("Cannot choose from an empty list.");
        return list[Int(0, list.Count)];
    }

    public static T Choice<T>(T[] array)
    {
        return array[Int(0, array.Length)];
    }

    public static Color Color()
    {
        return Random.ColorHSV(0,1,1,1,1,1,1,1);
    }

    private static char[] _pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray();

    public static string String() => String(1, 10);

    public static string String(int length) => String(length, length);

    public static string String(int minSize,int maxSize, char[] pool = null)
    {
        if (maxSize < minSize || minSize < 0 || maxSize < 0)
        {
            Log.Error("Invalid values in MyRandom.String() call, they must be non negative and maxSize >= minSize");
            return null;
        }
        if (pool == null)
            pool = _pool;
        int length = MyRandom.Int(minSize, maxSize);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
            sb.Append(Choice(pool));
        return sb.ToString();
    }
}
