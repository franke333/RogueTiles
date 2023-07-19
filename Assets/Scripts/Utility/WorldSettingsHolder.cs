using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettingsHolder : MonoBehaviour
{
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

    public void LoadSettingsAndPlay()
    {
        var levelDesignManager = LevelDesignManager.Instance;
        levelDesignManager.DungeonSettings = dungeonSettings;
        levelDesignManager.NumberOfDungeons = numberOfDungeons;
        levelDesignManager.MapWidth = mapWidth;
        levelDesignManager.MapHeight = mapHeight;
        levelDesignManager.MinimalWalkableTileRatio = minimalWalkableTileRatio;
        levelDesignManager.DrunkardsMaxPath = drunkardsMaxPath;
        levelDesignManager.OutsideTribesSizes = outsideTribesSizes;
        levelDesignManager.InsideTribeSize = insideTribeSize;
        levelDesignManager.WorldType = worldType;
        MenuManager.Instance.Play();
    }
}
