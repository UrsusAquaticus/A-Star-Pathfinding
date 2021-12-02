using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[RequireComponent(typeof(PathRequestController))]
public class Pathfinding : MonoBehaviour
{
    public World world;

    public void FindPath(PathRequest request, Action<PathResult> callback){

        Tile[] waypoints = new Tile[0];
        bool pathSuccess = false;

        Tile startTile = request.startTile;
        Tile targetTile = request.targetTile;

        if(startTile.isWalkable && targetTile.isWalkable)
        {
            Heap<Tile> openSet = new Heap<Tile>(world.MaxSize);
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add(startTile);
            
            while(openSet.Count > 0){
                Tile currentTile = openSet.RemoveFirst();

                closedSet.Add(currentTile);
                if(currentTile == targetTile){
                    pathSuccess = true;
                    break;
                }

                foreach (Tile neighbour in world.GetNeighbours(currentTile))
                {
                    if(!neighbour.isWalkable || closedSet.Contains(neighbour)){
                        continue;
                    }

                    //Cost, where weights can be added
                    int newCostToNeighbour = currentTile.gCost + GetDistance(currentTile, neighbour) + (int)neighbour.Type;
                    if(newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)){
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetTile);
                        neighbour.parent = currentTile;

                        if(!openSet.Contains(neighbour)){
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        
        if(pathSuccess){
            waypoints = RetracePath(startTile, targetTile);
            pathSuccess = waypoints.Length > 0;
        }
        if(!pathSuccess){
            //Path failure
            waypoints = new Tile[]{request.targetTile};
        }
        callback (new PathResult(waypoints, pathSuccess, request.callback));
    }

    Tile[] RetracePath(Tile startTile, Tile targetTile){
        List<Tile> path = new List<Tile>();
        Tile currentTile = targetTile;

        while(currentTile != startTile){
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        // Tile[] waypoints = SimplifyPath(path);
        // Array.Reverse(waypoints);
        // return waypoints;
        path.Reverse();
        return path.ToArray();
    }

    Tile[] SimplifyPath(List<Tile> path){
        List<Tile> waypoints = new List<Tile>();
        Vector2 dirOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++){
            Vector2 dirNew = new Vector2(path[i-1].X - path[i].X, path[i-1].Y - path[i].Y);
            if(dirNew != dirOld){
                waypoints.Add(path[i]);
            }
            dirOld = dirNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Tile tileA, Tile tileB){
        int dstX = Mathf.Abs(tileA.X - tileB.X);
        int dstY = Mathf.Abs(tileA.Y - tileB.Y);

        if(dstX > dstY){
            return 14*dstY + 10 * (dstX-dstY);
        }else{
            return 14*dstX + 10 * (dstY-dstX);
        }
    }

    public Tile GetClosestEmptyTile(Tile startTile, Tile targetTile){
        Tile closestTile = null;
        float dist = float.MaxValue;

        int i = 0;
        while(closestTile == null && i < world.MaxSize/2){
            i++;
            for (int x = -i; x <= i; x++) {
			    for (int y = -i; y <= i; y++) {
                    if (x == 0 && y == 0)
                        continue;
                    if(x == -i || x == i || y == -i || y == i){
                        int checkX = targetTile.X + x;
                        int checkY = targetTile.Y + y;
                        //within world
                        if (checkX >= 0 && checkX < world.Width && checkY >= 0 && checkY < world.Height) {
                            Tile tile = world.GetTile(checkX, checkY);
                            //If available
                            if(!tile.isReserved && tile.isWalkable){
                                //If closest to start Tile
                                if(Vector2.Distance(startTile.Position, tile.Position) < dist){
                                    closestTile = tile;
                                }
                            }
                        }
                    }
                }
            }
        }
        if(closestTile == null){
            Debug.Log("GetClosestEmptyTile :: Tile Is Null");
        }
        return closestTile;
    }
}
