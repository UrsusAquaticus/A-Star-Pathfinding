using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class UnitMovement : MonoBehaviour
{
    public float speed = 10;
    public bool isWaitingForResponse;
    [HideInInspector]
    public UnitController uc;
    [HideInInspector]
    public Pathfinding pathfinding;
    public List<Tile> path;
    Tile targetTile;
    LineRenderer lr;
    private void Awake()
    {
        uc = GetComponent<UnitController>();
        lr = GetComponent<LineRenderer>();
    }

    float t = 0;
    private void Update()
    {
        DrawPath();
        
        if(t > 0){
            t-=Time.deltaTime;
        }else{
            if(!isWaitingForResponse){
                //Get a random place
                if(path == null || path.Count == 0){
                    int randX = Random.Range(0, uc.world.Width);
                    int randY = Random.Range(0, uc.world.Height);
                    MakePathRequest(new Vector2(randX, randY), 0.1f);
                }
            }
        }
    }

    public void MakePathRequest(Vector2 pos, float waitTime){
        t = waitTime;
        isWaitingForResponse = true;
        Tile tile = uc.world.GetTileFromWorldPoint(pos);
        PathRequestController.RequestPath(new PathRequest(uc.occupies[0], tile, OnPathFound));
    }

    public void OnPathFound(Tile[] newPath, bool pathSuccessful){
        isWaitingForResponse = false;
        //Check if latest path
        if(pathSuccessful){
            //Stop reserving previous tile
            if(targetTile != null){
                targetTile.isReserved = false;
            }
            //Set new path
            path = new List<Tile>(newPath);
            targetTile = newPath[newPath.Length-1];
            //Follow Path
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        } else{
            //DiscardReserved tile
            newPath[newPath.Length-1].isReserved = false;
        }
    }

    IEnumerator FollowPath()
    {
        while(true){
            //Exit if path = null
            //Exit if path has no tiles
            if (path == null || path.Count == 0){
                transform.position = uc.occupies[0].Position;
                yield break;
            }

            Tile targetTile = path[0];
            //If tile now blocked, get new path
            if(!targetTile.isWalkable){
                //Try find new path around
                MakePathRequest(path[path.Count-1].Position, 0.1f);
                path = null;
                yield break;
            }

            if (Vector2.Distance(transform.position, targetTile.Position) > 0.2)
            {
                //Move toward next tile
                Vector3 dir = ((Vector3)targetTile.Position - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
                //transform.position = Vector2.Lerp(transform.position, targetTile.Position, Time.deltaTime* speed);
            }
            else
            {
                //Occupy tile 
                if(path != null){
                    uc.SetOccupancy(path[0]);
                    path.Remove(path[0]);
                }
            }
            yield return null;
        }
    }

    
    void DrawPath()
    {
        //Exit if not selected
        //Exit if path = null
        //Exit if path has no tiles
        if(!uc.isSelected ||
            path == null ||
            path.Count == 0)
        {
            lr.enabled = false;
            return;
        }

        lr.enabled = true;
        lr.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            lr.SetPosition(i, path[i].Position);
        }
    }
}
