using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
[RequireComponent(typeof(MouseController))]
[RequireComponent(typeof(Pathfinding))]
public class WorldController : MonoBehaviour
{
    MouseController mouseCon;
    Pathfinding pathfinding;
    World world;
    public List<UnitController> units;
    public Sprite[] floorSprites;
    public Camera cam;
    public GameObject[] character;
    public GameObject[] staticObjects;

    // Start is called before the first frame update
    void Start()
    {
        mouseCon = GetComponent<MouseController>();
        pathfinding = GetComponent<Pathfinding>();
        //Create default world with empty tile data
        world = new World();
        units = new List<UnitController>();
        //Setup camera
        cam.transform.position = new Vector3(world.Width * 0.5f, world.Height * 0.5f, -1);
        mouseCon.world = world;
        pathfinding.world = world;
        //Create tile objects
        CreateTileObjects(world);
        //world.RandomiseTiles();
        //Create Characters
        for (int i = 0; i < 100; i++)
        {
            Tile randomTile = world.GetRandomEmptyTile();
            CreateCharacter(world, character[0], randomTile, $"Unit {i}");
        }
        for (int i = 0; i < 100; i++)
        {
            Tile randomTile = world.GetRandomEmptyTile();
            CreateCharacter(world, character[1], randomTile, $"Unit {i}");
        }
        for (int i = 0; i < world.MaxSize/2; i++)
        {
            Tile randomTile = world.GetRandomEmptyTile();
            CreateStaticObject(world, staticObjects[0], randomTile);
        }
    }

    void CreateTileObjects(World world)
    {
        GameObject tileContainer = new GameObject();
        tileContainer.transform.parent = this.transform;
        tileContainer.name = "Tile Container";
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile tile_data = world.GetTile(x, y);

                GameObject tile_go = new GameObject();
                tile_go.name = tile_data.name;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.AddComponent<SpriteRenderer>();
                tile_go.transform.parent = tileContainer.transform;

                //lambda callback function
                tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
            }
        }
        //world.RandomiseTiles();
        world.LoadTiles();
    }

    void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        int tileIndex = 0;
        if(tile_data.Type == Tile.TileType.Wall)
        {
            tileIndex = 0;
        } else if(tile_data.Type == Tile.TileType.Grass)
        {
            tileIndex = 1;
        } else if(tile_data.Type == Tile.TileType.Dirt)
        {
            tileIndex = 2;
        }
        tile_go.GetComponent<SpriteRenderer>().sprite = floorSprites[tileIndex];
    }

    void CreateCharacter(World world, GameObject characterObject, Tile tile, string name)
    {
        //Create character object
        GameObject go = Instantiate(characterObject);
        go.name = name;
        UnitController uc = go.GetComponent<UnitController>();
        uc.world = world;
        uc.unitMovement.pathfinding = pathfinding;
        //Attach the character to a tile
        go.transform.position = tile.Position;
        uc.SetOccupancy(tile);
        uc.unitMovement.MakePathRequest(tile.Position, 0.1f);
        units.Add(uc);
    }

    void CreateStaticObject(World world, GameObject gameObject, Tile tile)
    {
        //Create character object
        GameObject go = Instantiate(gameObject);
        //Attach the character to a tile
        go.transform.position = tile.Position;
        go.GetComponent<Selectable>().SetOccupancy(tile);
    }

    void OnDrawGizmosSelected()
    {  
        if(world!=null){
            Selectable[,] selectables = Selectable.GetSelectablesByCoords(world, units.ToArray());
            Tile[,] tiles = world.Tiles;
            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    Tile tile = tiles[x,y];
                    if(tile.isReserved){
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(tile.Position, new Vector3(1, 1, 1));
                    }
                    if(selectables[x,y] != null){
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(tile.Position, new Vector3(1, 1, 1));
                    }
                }
            }
        }
    }
}
