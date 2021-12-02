using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryObject
{
    Selectable selectable;
    public InventoryObject(Selectable selectable){
        this.selectable = selectable;
    }
}
