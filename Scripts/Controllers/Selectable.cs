using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public World world;
    public bool isSelected;
    public Tile[] occupies;
    public void SetOccupancy(Tile[] tiles){
        //Remove from old occupied tiles
        ClearOccupancy();
        //Add to new occupied tiles
        if (tiles.Length != 0)
        {
            foreach (Tile tile in tiles)
            {
                tile.AddOccupant(this);
            }
            occupies = tiles;
        }
        else
        {
            Debug.LogError($"{name}: New tile array Empty");
        }
    }
    public void SetOccupancy(Tile tile){
        //Remove from old occupied tiles
        ClearOccupancy();
        //Add to new occupied tiles
        if (tile != null)
        {
            tile.AddOccupant(this);
            occupies = new Tile[]{tile};
        }
        else
        {
            Debug.LogError($"{name}: New tile null");
        }
    }
    public void ClearOccupancy()
    {
        if(occupies != null){
            foreach (Tile tile in occupies)
            {
                tile.RemoveOccupant(this);
            }
            occupies = new Tile[0];
        }
    }

    public Vector2Int GetCoord(){
        return world.GetCoordFromWorldPoint(transform.position);
    }
    
    public static Vector2Int GetCoordFromWorldPoint(World world, Vector3 worldPoint){
        return world.GetCoordFromWorldPoint(worldPoint);
    }

    public static Selectable[] GetAll(){
        List<Selectable> selectables = new List<Selectable>();
        Object[] objs = FindObjectsOfType(typeof(Selectable));
        for (int i = 0; i < objs.Length; i++)
        {
            selectables.Add((Selectable)objs[i]);
        }
        return selectables.ToArray();
    }

    public static Selectable[,] GetSelectablesByCoords(World world, Selectable[] selectables){
        Selectable[,] byCoords = new Selectable[world.Width, world.Height];
        for (int i = 0; i < selectables.Length-1; i++)
        {
            Vector2Int coord = selectables[i].GetCoord();
            byCoords[coord.x, coord.y] = selectables[i];
        }
        return byCoords;
    }
    public static Selectable[] GetSelectablesFromWorldRect(World world, Selectable[,] selectables, Vector2 worldRectA, Vector2 worldRectB)
    {
        List<Selectable> tempSelectable = new List<Selectable>();

        Vector2Int coA = GetCoordFromWorldPoint(world, worldRectA);
        Vector2Int coB = GetCoordFromWorldPoint(world, worldRectB);

        int minX = coA.x < coB.x ? coA.x : coB.x;
        int minY = coA.y < coB.y ? coA.y : coB.y;

        int maxX = coA.x > coB.x ? coA.x : coB.x;
        int maxY = coA.y > coB.y ? coA.y : coB.y;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                tempSelectable.Add(selectables[Mathf.Clamp(x, 0, world.Width-1), Mathf.Clamp(y, 0, world.Height-1)]);
            }
        }
        return tempSelectable.ToArray();
    }
}
