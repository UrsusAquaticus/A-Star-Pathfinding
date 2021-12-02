using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Tile : IHeapItem<Tile>
{
    public string name;

    World world;
    public World World
    {
        get { return world; }
    }
    //Pathfinding
    public Tile parent;
    public bool isWalkable = false;
    public bool isReserved = false;
    public int gCost;
    public int hCost;
    public int fCost{
        get{ return gCost + hCost; }
    }
    int heapIndex;
    public int HeapIndex{
        get{
            return heapIndex;
        }
        set{
            heapIndex = value;
        }
    }

    public int CompareTo(Tile tileToCompare){
        int compare = fCost.CompareTo(tileToCompare.fCost);
        if(compare == 0){
            compare = hCost.CompareTo(tileToCompare.hCost);
        }
        return -compare;
    }

    //Coordinates
    protected int x, y;
    public int X
    {
        get { return x; }
    }
    public int Y
    {
        get { return y; }
    }
    public Vector2 Position
    {
        get { return new Vector2(x, y); }
    }

    //Tiletypes
    public enum TileType { Null, Wall = 1000, Grass = 5, Dirt = 1};
    TileType type = TileType.Wall;
    Action<Tile> cbTileTypeChanged;
    public TileType Type
    {
        get { return type; }
        set
        {
            TileType oldType = type;
            type = value;
            //Call the callback
            if (cbTileTypeChanged != null && oldType != type)
                cbTileTypeChanged(this);
        }
    }

    public static TileType GetTileType(int type){
        if(type == 1){
            return TileType.Wall;
        } else if(type == 2){
            return TileType.Grass;
        } else if(type == 3){
            return TileType.Dirt;
        } else{
            return TileType.Null;
        }
    }
    
    //Occupancy
    public List<Selectable> occupants;

    public void AddOccupant(Selectable occupant){
        occupants.Add(occupant);
        CheckIfWalkable();
    }

    public void RemoveOccupant(Selectable occupant){
        occupants.Remove(occupant);
        CheckIfWalkable();
    }

    void CheckIfWalkable(){
        bool nowWalkable = true;
        foreach(Selectable occ in occupants){
            if(occ.transform.GetComponent<StaticObject>() != null){
                nowWalkable = false;
            }
        }
        isWalkable = nowWalkable;
    }

    //Constructor
    public Tile(World world, int x, int y)
    {
        occupants = new List<Selectable>();
        this.world = world;
        this.x = x;
        this.y = y;
        this.name = $"(Tile: {x}, {y})";
    }

    public Tile(World world, int x, int y, int type)
    {
        occupants = new List<Selectable>();
        this.world = world;
        this.x = x;
        this.y = y;
        this.name = $"(Tile: {x}, {y})";
        this.type = GetTileType(type);
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }
    public void DeregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }
}
