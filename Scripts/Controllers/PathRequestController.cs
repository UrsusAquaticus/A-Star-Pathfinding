using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathRequestController : MonoBehaviour
{
    Queue<PathResult> results = new Queue<PathResult>();
    static PathRequestController instance;
    Pathfinding pathfinding;

    void Awake(){
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    void Update(){
        if(results.Count > 0){
            int itemsInQueue = results.Count;
            lock(results){
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    static System.Object _lock = new System.Object();
    public static void RequestPath(PathRequest request){
        //Target is not null
        lock(_lock){
            if(request.targetTile.isReserved)
            {
                //Debug.Log($"RequestPath :: Requested Tile is Reserved, Finding new Tile");
                request.targetTile = instance.pathfinding.GetClosestEmptyTile(request.startTile, request.targetTile);
            }
            //Reserve tile
            request.targetTile.isReserved = true;
        }
        ThreadStart threadStart = delegate {
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }

    public void FinishedProcessingPath(PathResult result){
        lock(results){
            results.Enqueue(result);
        }
    }
}


public struct PathResult {
    public Tile[] path;
    public bool success;
    public Action<Tile[], bool> callback;

    public PathResult(Tile[] path, bool success, Action<Tile[], bool> callback){
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}
public struct PathRequest{
    public Tile startTile;
    public Tile targetTile;
    public Action<Tile[], bool> callback;

    public PathRequest(Tile startTile, Tile targetTile, Action<Tile[], bool> callback){
        this.startTile = startTile;
        this.targetTile = targetTile;
        this.callback = callback;
    }
}
