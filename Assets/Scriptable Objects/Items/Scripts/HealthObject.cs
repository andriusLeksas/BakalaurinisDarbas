using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Health Object", menuName = "Inventory System/Items/Food")]
public class HealthObject : ItemObject
{
    public void Awake()
    {
        type = ItemType.Health;
    }
}
