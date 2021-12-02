using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    Tile[,] tiles;
    public Tile[,] Tiles{
        get{ return tiles; }
    }

    int width;
    public int Width //property
    {
        get { return width; }
    }
    int height;
    public int Height //property
    {
        get { return height; }
    }

    public int MaxSize {
        get{ return width*height; }
    }

    public World(int width = 100, int height = 100)
    {
        this.width = width;
        this.height = height;
        tiles = new Tile[width, height];
        
        MapGenerator mg = new MapGenerator(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y, mg.map[x, y]);
            }
        }

    }
    public World(int[,] map)
    {
        this.width = map.GetLength(0);
        this.height = map.GetLength(1);
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
    }

    public void RandomiseTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int rand = Random.Range(0, 10);
                Tile tile = tiles[x, y];
                if (rand == 0)
                {
                    tile.Type = Tile.TileType.Grass;
                    tile.isWalkable = true;
                } else if( rand > 0 && rand < 5)
                {
                    tile.Type = Tile.TileType.Dirt;
                    tile.isWalkable = true;
                } else{
                    tile.Type = Tile.TileType.Wall;
                    tile.isWalkable = false;
                }
            }
        }
    }

    public void LoadTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y].Type = Tile.TileType.Grass;
                tiles[x, y].isWalkable = true;
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (x > width || x < 0 || y > height || y < 0)
            return null;

        return tiles[x, y];
    }

    public Tile GetTileFromWorldPoint(Vector3 worldPosition)
    {
        var co = GetCoordFromWorldPoint(worldPosition);
        return tiles[co.x, co.y];
    }

    public Tile[] GetTilesFromWorldRect(Vector3 worldRectA, Vector3 worldRectB)
    {
        List<Tile> tempTiles = new List<Tile>();

        Vector2Int coA = GetCoordFromWorldPoint(worldRectA);
        Vector2Int coB = GetCoordFromWorldPoint(worldRectB);

        int minX = coA.x < coB.x ? coA.x : coB.x;
        int minY = coA.y < coB.y ? coA.y : coB.y;

        int maxX = coA.x > coB.x ? coA.x : coB.x;
        int maxY = coA.y > coB.y ? coA.y : coB.y;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                tempTiles.Add(tiles[Mathf.Clamp(x, 0, width-1), Mathf.Clamp(y, 0, height-1)]);
            }
        }
        return tempTiles.ToArray();
    }

    public Tile[,] GetTilesFromWorldRectByCoord(Vector3 worldRectA, Vector3 worldRectB)
    {
        Tile[,] tempTiles = new Tile[Width, Height];

        Vector2Int coA = GetCoordFromWorldPoint(worldRectA);
        Vector2Int coB = GetCoordFromWorldPoint(worldRectB);

        int minX = coA.x < coB.x ? coA.x : coB.x;
        int minY = coA.y < coB.y ? coA.y : coB.y;

        int maxX = coA.x > coB.x ? coA.x : coB.x;
        int maxY = coA.y > coB.y ? coA.y : coB.y;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                tempTiles[x, y] = tiles[Mathf.Clamp(x, 0, width-1), Mathf.Clamp(y, 0, height-1)];
            }
        }
        return tempTiles;
    }

    public Vector2Int GetCoordFromWorldPoint(Vector3 worldPoint)
    {
        int x = Mathf.Clamp(Mathf.RoundToInt(worldPoint.x), 0, width);
        int y = Mathf.Clamp(Mathf.RoundToInt(worldPoint.y), 0, height);

        return new Vector2Int(x, y);
    }

    public List<Tile> GetNeighbours(Tile tile)
    {
		List<Tile> neighbours = new List<Tile>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = tile.X + x;
				int checkY = tile.Y + y;

				if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height) {
					neighbours.Add(tiles[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}

    public Vector2 GetRandomWorldPosition(){
        int randX = Random.Range(Mathf.RoundToInt(0), Mathf.RoundToInt(Width));
        int randY = Random.Range(Mathf.RoundToInt(0), Mathf.RoundToInt(Height));
        return new Vector2(randX, randY);
    }

    public Tile GetRandomEmptyTile(){
        for(int i = 0; i < MaxSize; i++){
            Tile tile = GetTileFromWorldPoint(GetRandomWorldPosition());
            if(tile.isWalkable && !tile.isReserved) {
                return tile;
            }
        }
        return null;
    }

}
