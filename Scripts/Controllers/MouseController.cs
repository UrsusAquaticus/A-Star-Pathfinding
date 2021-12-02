using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public World world;
    SelectedUnitsController selectedUnitsController;

    //Map Drag
    public float scrollSpeed = 10;
    Vector2 lastFramePosition;

    //Mouse mode
    public enum Mode { Select, Edit };
    public Mode curMode = Mode.Select;

    //Assets
    public Sprite hoverSprite;
    GameObject hoverUI;

    //Rect
    public LineRenderer rectangleRenderer;
    public Vector2? pointA, pointB;
    public Rect? rectangle;

    //World edit
    Tile.TileType tileType = Tile.TileType.Wall;

    // Start is called before the first frame update
    void Start()
    {
        //Hover
        selectedUnitsController = GetComponent<SelectedUnitsController>();
        //
        hoverUI = CreateUI("Hover UI", hoverSprite, transform, Vector2.zero);
    }

    GameObject CreateUI(string name, Sprite sprite, Transform parent, Vector2 position){
        //Selection
        var tempGO = new GameObject();
        tempGO.name = name;
        tempGO.transform.parent = parent;
        tempGO.transform.position = position;
        SpriteRenderer sr = tempGO.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Selection";
        return tempGO;
    }

    // Update is called once per frame
    void Update()
    {
        var point = GetWorldPointFromMousePosition();
        var tile = world.GetTileFromWorldPoint(point);

        if(Input.GetKeyDown(KeyCode.Space)){
            curMode = curMode == Mode.Select ? Mode.Edit : Mode.Select;
        }

        CameraMovement(point);
        TileHover(point);

        if(curMode == Mode.Select)
        {
            RectangleSelect(point);
            //Rightclick
            if(Input.GetMouseButtonDown(1)){
                selectedUnitsController.TryDirectSelected(point);
            }
        }
        
        if(curMode == Mode.Edit){
            ToggleTile(tile);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Camera.main.orthographicSize -= scroll * scrollSpeed;
        }
    }

    void ToggleTile(Tile tile)
    {
        if(Input.GetMouseButtonDown(0)){
            if(tile.Type == Tile.TileType.Wall){
                tileType = Tile.TileType.Grass;
            } else if(tile.Type == Tile.TileType.Grass){
                tileType = Tile.TileType.Dirt;
            } else if(tile.Type == Tile.TileType.Dirt){
                tileType = Tile.TileType.Wall;
            }
        }
        if(Input.GetMouseButton(0)){
            if(tile.occupants.Count == 0 && !tile.isReserved){
                tile.Type = tileType;
                tile.isWalkable = (int)tileType < 10;
            }
        }
    }

    void CameraMovement(Vector2 point)
    {
        if (Input.GetMouseButton(2))
        {
            Vector2 diff = lastFramePosition - point;
            Camera.main.transform.Translate(diff);
        }
        lastFramePosition = point;
    }

    //Move the hover object
    void TileHover(Vector2 point)
    {
        hoverUI.transform.position = new Vector2(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
    }

    Vector2 GetWorldPointFromMousePosition(){
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void RectangleSelect(Vector2 point)
    {
        //Initial click
        if(Input.GetMouseButtonDown(0)){
            pointA = point;
        }
        //While held down
        if(Input.GetMouseButton(0)){
            pointB = point;
        }
        //when released
        if(Input.GetMouseButtonUp(0)){
            //Clear the rect
            pointA = null;
            pointB = null;
        }
        //Create and assign rectangle
        SetRectangle(pointA, pointB);
        //Draw the rectangle
        DrawRectangle(pointA, pointB);
    }

    void SetRectangle(Vector2? pointA, Vector2? pointB){
        if(pointA != null && pointB != null){

            Vector2 pA = (Vector2)pointA;
            Vector2 pB = (Vector2)pointB;

            Rect rect = new Rect();
            rect.xMin = pA.x < pB.x ? pA.x : pB.x;
            rect.xMax = pA.x > pB.x ? pA.x : pB.x;
            rect.yMin = pA.y < pB.y ? pA.y : pB.y;
            rect.yMax = pA.y > pB.y ? pA.y : pB.y;

            rectangle = rect;
        }else{
            rectangle = null;
        }
    }

    void DrawRectangle(Vector2? firstPoint, Vector2? secondPoint){
        if(firstPoint != null && secondPoint != null)
        {
            rectangleRenderer.enabled = true;
            rectangleRenderer.positionCount = 5;
            Vector3 pointA = (Vector3)firstPoint;
            Vector3 pointC = (Vector3)secondPoint;
            //Derive from the two points
            Vector3 pointB = new Vector3(pointA.x, pointC.y);
            Vector3 pointD = new Vector3(pointC.x, pointA.y);
            rectangleRenderer.SetPosition(0, pointA);
            rectangleRenderer.SetPosition(1, pointB);
            rectangleRenderer.SetPosition(2, pointC);
            rectangleRenderer.SetPosition(3, pointD);
            rectangleRenderer.SetPosition(4, pointA);
        }
        else{
            rectangleRenderer.enabled = false;
        }
    }
}
