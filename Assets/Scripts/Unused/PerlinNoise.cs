using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise 
{
    [System.Serializable]
    public struct Data
    {
        public int width, height ;
        public float scale, offSetX, offSetY;

        public Data(int width, int height, float scale, float offSetX, float offSetY)
        {
            this.width = width;
            this.height = height;
            this.scale = scale;
            this.offSetX = offSetX;
            this.offSetY = offSetY;
        }
    }

    // there is mirror effect at (0,0), applying this baseOffset will reduce probability meeting this effect
    private static float baseOffset = 1000f;

    private static Texture2D GenerateTextureBase(Data data,Func<float,float> predicate)
    {
        Texture2D texture = new Texture2D(data.width, data.height);
        for (int x = 0; x < data.width; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                float perlinValue = Mathf.PerlinNoise(
                    x / (float)data.width * data.scale + data.offSetX + baseOffset,
                    y / (float)data.height * data.scale + data.offSetY + baseOffset
                    );
                perlinValue = predicate(perlinValue);
                Color c = new Color(perlinValue, perlinValue, perlinValue);
                texture.SetPixel(x, y, c);
            }
        }
        texture.Apply();
        return texture;
    }

    private static Texture2D GenerateTextureBase(Data data, Func<float, Color> predicate)
    {
        Texture2D texture = new Texture2D(data.width, data.height);
        for (int x = 0; x < data.width; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                float perlinValue = Mathf.PerlinNoise(
                    x / (float)data.width * data.scale + data.offSetX + baseOffset,
                    y / (float)data.height * data.scale + data.offSetY + baseOffset
                    );
                Color c = predicate(perlinValue);
                texture.SetPixel(x, y, c);
            }
        }
        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTexture(Data data)
        => GenerateTextureBase(data, (float a) => Mathf.Clamp01(a));

    public static Texture2D GenerateTexture01(Data data,float boundary)
        => GenerateTextureBase(data, (float a) => a > boundary ? 1 : 0);

    public static Texture2D GenerateTextureWorms(Data data, float range)
        => GenerateTextureBase(data, (float a) => Mathf.Abs(a-0.5f) > range/2 ? 1 : 0);

    public static Texture2D GenerateTextureUsingGradient(Data data, Gradient gradient)
        => GenerateTextureBase(data, (float a) => gradient.Evaluate(Mathf.Clamp01(a)));
}
