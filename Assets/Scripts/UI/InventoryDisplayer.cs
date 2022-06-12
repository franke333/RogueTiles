using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayer : MonoBehaviour
{
    public GameObject inventory;
    private void Start()
    {
        inventory.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            inventory.gameObject.SetActive(!inventory.activeSelf);
    }
}
