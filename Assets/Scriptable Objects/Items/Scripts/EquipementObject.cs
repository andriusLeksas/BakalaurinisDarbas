using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory System/Items/Equipement")]
public class EquipementObject : ItemObject
{
    public void Awake()
    {
        type = ItemType.Equipment;
    }
}
