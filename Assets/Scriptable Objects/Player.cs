//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Player : MonoBehaviour
//{
//    //public MouseItem mouseItem = new MouseItem();

//    public InventoryObject inventory;
//    public InventoryObject equipement;

//    public Attribute[] attributes;

//    private void Start()
//    {
        
//    }

//    public void OnTriggerEnter(Collider other)
//    {
//        var item = other.GetComponent<GroundItem>();
//        if (item)
//        {
//            Item _item = new Item(item.item);
//            if (inventory.AddItem(_item, 1))
//            {
//                Destroy(other.gameObject);
//            }
                    
//        }
//    }
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            inventory.Save();
//            equipement.Save();
//            Debug.Log("Saved");
//        }
//        if (Input.GetKeyDown(KeyCode.Backspace))
//        {
//            inventory.Load();
//            equipement.Load();
//            Debug.Log("Loaded");
//        }
//    }

//    public void AttributeModified(Attribute attribute)
//    {
//        Debug.Log(string.Concat(attribute.type, "was updated! Value is now", attribute.value.ModifiedValue));
//    }
//    private void OnApplicationQuit()
//    {
//        inventory.Clear();
//        equipement.Clear();
//    }
//}

//[System.Serializable]
//public class Attribute
//{
//    [System.NonSerialized]
//    public Player parent;
//    public Attributes type;
//    public ModifiableInt value;

//    public void SetParent(Player _parent)
//    {
//        parent = _parent;
//        value = new ModifiableInt(AttributeModified);
//    }

//    public void AttributeModified()
//    {
//        parent.AttributeModified(this);
//    }
//}
