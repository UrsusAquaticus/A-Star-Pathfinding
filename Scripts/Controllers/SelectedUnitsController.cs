using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnitsController : MonoBehaviour
{
    WorldController wc;
    MouseController mc;
    public Sprite selectedSprite;
    public Transform parentUI;
    public List<UnitController> selectedUnits;

    void Awake(){
        wc = GetComponent<WorldController>(); 
        mc = GetComponent<MouseController>();
        selectedUnits = new List<UnitController>();
    }
    float t = 0;
    void Update(){
        t+= Time.deltaTime;
        if(t > 0.1f){
            DoSelectUnits();
        }
    }

    void DoSelectUnits(){
        //If either point is null, exit
        if(mc.rectangle == null) return;
        //Remove null
        Rect rect = ((Rect)mc.rectangle);
        t = 0;

        //List of units to be deselected
        UnitController[] units = wc.units.ToArray();
        List<UnitController> oldSelected = selectedUnits;
        List<UnitController> newSelected = new List<UnitController>();

        //Check each instantiated unit
        for (int i = 0; i < units.Length; i++)
        {
            Vector2 pos = units[i].transform.position;
            //Check if their position is within the selection rect
            if(rect.Contains(pos)){
                UnitController unit = units[i];
                //Remove from units to be deselected
                oldSelected.Remove(unit);
                //Add to selected units
                newSelected.Add(unit);
                if(!unit.isSelected){
                    Select(unit);
                }
            }
        }
        //Deselect those that are no longer selected
        for (int i = 0; i < oldSelected.Count; i++)
        {
            Deselect(oldSelected[i]);
        }
        selectedUnits = newSelected;
    }

    void Select(UnitController unit){
        unit.isSelected = true;
        CreateSelectionUI(unit);
    }
    void Deselect(UnitController unit){
        unit.isSelected = false;
        Destroy(unit.transform.Find("Selected").gameObject);
    }

    void CreateSelectionUI (UnitController unit){
        //Create object and attach to unit
        GameObject go = new GameObject();
        go.name = "Selected";
        go.transform.parent = unit.transform;
        go.transform.position = unit.transform.position;
        //Add Sprite
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = selectedSprite;
        sr.sortingLayerName = "Selection";
    }

    public void TryDirectSelected(Vector2 point){
        //Attempt to send each selected object to that location
        foreach(Selectable selected in selectedUnits){
            //Try see if they are a character
            UnitController uc = selected.GetComponent<UnitController>();
            if(uc != null)
            {
                //Send a path  request for any characters selected
                uc.unitMovement.MakePathRequest(point, 10f);
            }
        }
    }
}
