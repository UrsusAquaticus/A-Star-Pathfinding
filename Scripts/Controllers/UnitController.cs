using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : Selectable
{
    [HideInInspector]
    public UnitMovement unitMovement;
    [HideInInspector]
    public UnitInventory unitInventory;

    void Awake(){
        unitMovement = GetComponent<UnitMovement>();
        unitInventory = new UnitInventory();
    }
    
    bool DoWork(StaticObject staticObject){
        return staticObject.DoWork();
    }
}
