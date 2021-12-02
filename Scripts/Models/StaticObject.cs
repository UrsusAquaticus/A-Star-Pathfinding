using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : Selectable
{
    public int maxHp = 5;
    public int curHp;

    public StaticObject(string name, int maxHp){
        this.name = name;
        this.maxHp = maxHp;
        curHp = maxHp;
    }

    public bool DoWork(){
        maxHp--;
        return maxHp == 0;
    }

    public InventoryObject Deconstruct(){
        return new InventoryObject(this);
    }
}
