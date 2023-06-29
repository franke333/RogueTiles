using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TribesManager : SingletonClass<TribesManager>
{
    public List<Sprite> bodies, heads, leftHandItems, rightHandItems,chests,legs;
    
    

    [Serializable]
    class BaseActionWeightEntry
    {
        public NPCActionBase action;
        public int weight;
    }

    [Serializable]
    class ActionCostEntry
    {
        public NPCActionBase action;
        public int cost;
    }
    [Space]
    //action, weight
    [SerializeField]
    private List<BaseActionWeightEntry> _baseActions;

    [Space]
    [SerializeField]
    protected Canvas _healtBarCanvasPrefab;

    //actions that will be added to units at random
    //weights will be randomized between 1 and 3
    //an enemy will have around 5 coins to spend on actions
    //action, cost
    private List<ActionCostEntry> _actions;

    public int baseLayerOrder;

    [SerializeField]
    private int _NPCDetectionRange = 8;
    public int NPCDetectionRange { get { return _NPCDetectionRange; } }

    List<Tribe> tribes;

    private GameObject enemyParentObject;

    private class Tribe
    {
        public string name { get; private set; }
        public Color hue { get; private set; }

        public RoomType assignedRoom { get; private set; }


        List<NPCUnit> units;

        private SpriteRenderer AddSpriteRendererAsChildObject(GameObject parent,string name)
        {
            var obj = new GameObject(name);
            obj.transform.parent = parent.transform;
            return obj.AddComponent<SpriteRenderer>();
        }

        public Tribe(int size,RoomType roomType, Color tribeColor)
        {
            hue = tribeColor;
            name = MyRandom.String(4, 9);
            assignedRoom = roomType;
            units = new List<NPCUnit>();
            TribesManager tribeManager = TribesManager.Instance;
            for (int i = 0; i < size; i++)
            {
                //character creation
                string unitName = MyRandom.String(4, 9);
                GameObject unitObject = new GameObject(name + " " + unitName);
                GameObject spritesHolder = new GameObject("Sprites Holder");
                spritesHolder.transform.parent = unitObject.transform;
                var unit = unitObject.AddComponent<NPCUnit>();
                unit.alias = name + " " + unitName;
                SpriteRenderer AddSpriteBodyPart(string name,Sprite sprite,int sortingOrder)
                {
                    SpriteRenderer sr = AddSpriteRendererAsChildObject(spritesHolder, name);
                    sr.sprite = sprite;
                    sr.sortingOrder = tribeManager.baseLayerOrder + sortingOrder;
                    return sr;
                }

                //body
                var sr = AddSpriteBodyPart("body", MyRandom.Choice(tribeManager.bodies), 0);
                sr.color = hue;
                //chest
                AddSpriteBodyPart("chest piece", MyRandom.Choice(tribeManager.chests), 2);
                //heads
                AddSpriteBodyPart("head piece", MyRandom.Choice(tribeManager.heads), 3);
                //legs
                AddSpriteBodyPart("leg piece", MyRandom.Choice(tribeManager.legs), 1);
                //lefthand
                AddSpriteBodyPart("left hand piece", MyRandom.Choice(tribeManager.leftHandItems), 4);
                //righthand
                AddSpriteBodyPart("right hand piece", MyRandom.Choice(tribeManager.rightHandItems), 5);

                //base actions
                foreach(var actionWeightEntry in tribeManager._baseActions)
                    unit.ActionList.Add(new NPCUnit.ActionEntry() { weight = actionWeightEntry.weight, actionObj = actionWeightEntry.action });




                //buy phase
                int coins = 5;
                int actionsToBuy = MyRandom.Int(1, 4); //1-3
                for (int j = 0; j < actionsToBuy; j++)
                {
                    var actionCostEntry = MyRandom.Choice(tribeManager._actions);
                    if (coins >= actionCostEntry.cost)
                    {
                        coins -= actionCostEntry.cost;
                        unit.ActionList.Add(new NPCUnit.ActionEntry() { weight = MyRandom.Int(3,6), actionObj = actionCostEntry.action });
                    }
                }
                //unused coins are converted to extra health
                unit.Init(3+coins, true);
                units.Add(unit);

                //add helath bar
                var bar = Instantiate(TribesManager.Instance._healtBarCanvasPrefab, spritesHolder.transform);
                bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -0.5f);


                //we need this as prefab. not a real gameobject
                unitObject.SetActive(false);
                GameManager.Instance.UnregisterUnit(unit);
            }
        }

        public NPCUnit GetRandomEnemy() => MyRandom.Choice(units);
    }

    // check that generation happened before trying to fetch enemies
    private bool _ready;

    public void GenerateTribes(List<int> outdoorTribeSizes,int indoorTribeSize)
    {
        List<Color> colors = MyMath.GetDistinctColors(outdoorTribeSizes.Count + 1);
        tribes = new List<Tribe>();
        tribes.Add(new Tribe(indoorTribeSize, RoomType.Hall, colors[colors.Count-1]));
        for(int i = 0; i < outdoorTribeSizes.Count; i++)
            tribes.Add(new Tribe(outdoorTribeSizes[i], RoomType.OutsideEnemyCamp, colors[i]));
        _ready = true;
        enemyParentObject = new GameObject("Enemies");
        enemyParentObject.transform.SetParent(GameObject.Find("Enviroment").transform);
    }

    private void LoadActionsFromResources()
    {
        if(_actions != null && _actions.Count > 0)
        {
            return;
        }
        _actions = new List<ActionCostEntry>();
        var actions = Resources.LoadAll<NPCActionBase>("NPCActions").Where( ab => !ab.isBaseAction).ToList();
        foreach(var action in actions)
        {
            _actions.Add(new ActionCostEntry() { action = action, cost = action.cost });
        }
        Log.Info($"Loaded {_actions.Count} npc actions", this.gameObject);
    }

    private void Start()
    {
        LoadActionsFromResources();
    }

    private void SpawnEnemy(NPCUnit enemy,ITile tile)
    {
        var unit = Instantiate(enemy,enemyParentObject.transform);
        tile.Occupy(unit);
        unit.gameObject.SetActive(true);
        unit.Init(unit.maxHp, true);
    }

    public void ProcessRooms(List<Room> rooms)
    {
        foreach (var room in rooms)
            ProcessRoom(room);
    }

    private void ProcessRoom(Room room)
    {
        if (!_ready)
        {
            Log.Error("Tribes were not generated", gameObject);
            return;
        }
        var appropriateTribes = tribes.Where(t => t.assignedRoom == room.Type).ToList();
        if (appropriateTribes.Count == 0)
            return;
        var assignedTribe = MyRandom.Choice(appropriateTribes);
        foreach (var tile in room.GetRoomTiles)
            tile.TintBaseColor(assignedTribe.hue,0.2f);
        SpawnEnemy(assignedTribe.GetRandomEnemy(), MyRandom.Choice(room.GetRoomTiles));
    }
}
