using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TribesManager : SingletonClass<TribesManager>
{
    public List<Sprite> bodies, heads, leftHandItems, rightHandItems,chests,legs;

    public List<NPCActionBase> actions;

    public int baseLayerOrder;

    List<Tribe> tribes;

    private class Tribe
    {
        public string name { get; private set; }
        public Color hue { get; private set; }

        public RoomType assignedRoom { get; private set; }


        List<GridUnit> units;

        private SpriteRenderer AddSpriteRendererAsChildObject(GameObject parent,string name)
        {
            var obj = new GameObject(name);
            obj.transform.parent = parent.transform;
            return obj.AddComponent<SpriteRenderer>();
        }

        public Tribe(int size,RoomType roomType)
        {
            hue = MyRandom.Color();
            name = MyRandom.String(4, 9);
            assignedRoom = roomType;
            units = new List<GridUnit>();
            TribesManager tribeManager = TribesManager.Instance;
            for (int i = 0; i < size; i++)
            {
                string unitName = MyRandom.String(4, 9);
                GameObject unitObject = new GameObject(unitName);
                GameObject spritesHolder = new GameObject("Sprites Holder");
                spritesHolder.transform.parent = unitObject.transform;
                var unit = unitObject.AddComponent<NPCUnit>();
                unit.alias = name + " " + unitName;
                SpriteRenderer sr;
                //body
                sr = AddSpriteRendererAsChildObject(spritesHolder, "body");
                sr.color = hue;
                sr.sprite = MyRandom.Choice(tribeManager.bodies);
                sr.sortingOrder = tribeManager.baseLayerOrder;
                //chest
                sr = AddSpriteRendererAsChildObject(spritesHolder, "chest piece");
                sr.sprite = MyRandom.Choice(tribeManager.chests);
                sr.sortingOrder = tribeManager.baseLayerOrder + 1;
                //heads
                sr = AddSpriteRendererAsChildObject(spritesHolder, "head piece");
                sr.sprite = MyRandom.Choice(tribeManager.heads);
                sr.sortingOrder = tribeManager.baseLayerOrder + 2;
                //legs
                sr = AddSpriteRendererAsChildObject(spritesHolder, "leg piece");
                sr.sprite = MyRandom.Choice(tribeManager.legs);
                sr.sortingOrder = tribeManager.baseLayerOrder + 2;
                //lefthand
                sr = AddSpriteRendererAsChildObject(spritesHolder, "left hand");
                sr.sprite = MyRandom.Choice(tribeManager.leftHandItems);
                sr.sortingOrder = tribeManager.baseLayerOrder + 4;
                //righthand
                sr = AddSpriteRendererAsChildObject(spritesHolder, "right hand");
                sr.sprite = MyRandom.Choice(tribeManager.rightHandItems);
                sr.sortingOrder = tribeManager.baseLayerOrder + 5;

                //TODO add movements
                unit.ActionList.Add(new NPCUnit.ActionEntry() { weight = 1, actionObj = TribesManager.Instance.actions[0] });

                unit.Init(5, true);
                units.Add(unit);

                //we need this as prefab. not a real gameobject
                unitObject.SetActive(false);
                GameManager.Instance.UnregisterUnit(unit);
            }
        }

        public GridUnit GetRandomEnemy() => MyRandom.Choice(units);
    }

    // check that generation happened before trying to fetch enemies
    private bool _ready;

    public void GenerateTribes(List<int> outdoorTribeSizes,int indoorTribeSize)
    {
        
        tribes = new List<Tribe>();
        tribes.Add(new Tribe(indoorTribeSize, RoomType.Hall));
        foreach(var size in outdoorTribeSizes)
            tribes.Add(new Tribe(size, RoomType.OutsideEnemyCamp));
        _ready = true;
    }

    private void SpawnEnemy(GridUnit enemy,Tile tile)
    {
        var unit = Instantiate(enemy);
        tile.Occupy(unit);
        unit.gameObject.SetActive(true);
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
