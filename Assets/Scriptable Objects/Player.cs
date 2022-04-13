using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    // Start is called before the first frame update

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1);
            Destroy(other.gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            Debug.Log("Saved");
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            inventory.Load();
            Debug.Log("Loaded");
        }
    }
    private void OnApplicationQuit()
    {
        inventory.Container.Items = new InventorySlot[20];
    }
}
