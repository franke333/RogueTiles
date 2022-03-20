using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyRandom
{
    //use unity Random to preserve randomness states across program runs thorugh saves


    //to get consistent results across runs
    static int _seed = 1234;
    

    static MyRandom()
    {
        Log.Warning($"MyRandom is not consistent");
        //Random.InitState(_seed);
    }

    public static Random.State State
    {
        get => Random.state;
        set => Random.state = value;
    }

    public static int Int(int minValueInc = int.MinValue,int maxValueExc = int.MaxValue)
        => Random.Range(minValueInc, maxValueExc);

    public static float Float(float maxValue = 1.0f)
        => Random.Range(0, maxValue);

    public static float Float(float minValue, float maxValue)
        => Random.Range(minValue, maxValue);

    public static T Choice<T>(IList<T> list)
    {
        return list[Int(0, list.Count)];
    }
}
