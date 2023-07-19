using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPerlin : MonoBehaviour
{
    MeshRenderer mr;

    public PerlinNoise.Data perlinData;
    public float divider = .5f;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        mr.material.mainTexture = PerlinNoise.GenerateTextureWorms(perlinData, divider);
    }
}
