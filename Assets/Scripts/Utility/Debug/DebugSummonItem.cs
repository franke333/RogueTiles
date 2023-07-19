using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSummonItem : MonoBehaviour
{
    public Item item;
    // Start is called before the first frame update
    void Start()
    {
        item = Item.GenerateEquip(3);
    }

}
