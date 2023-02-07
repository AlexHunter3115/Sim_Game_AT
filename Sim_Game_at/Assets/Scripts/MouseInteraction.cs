using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class MouseInteraction : MonoBehaviour
{

    [SerializeField] MapCreation map;
    [SerializeField] Material transparent;
    [SerializeField] Material transparentError;
    [SerializeField] GameObject showObj;

    [SerializeField] LayerMask mapLayer;

    private List<GameObject> spawnedShowObj = new List<GameObject>();
    public Vector2Int selectionGridSize = new Vector2Int(0, 0);
    //public bool canInteract;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("pressed the right mous but");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("called the raycast");
                for (int i = 0; i < map.tilesArray.Length; i++)
                {
                    int row = i / map.textSize;
                    int col = i % map.textSize;

                    if (AABBCol(hit.point, map.tilesArray[row, col]))
                    {
                        Debug.Log("find a collison in the AABB");
                        map.ClickedTile = map.tilesArray[row, col];
                        SpawnShowObj(map.ClickedTile, selectionGridSize.x, selectionGridSize.y);
                        break;
                    }
                }
            }
        }



        if (Input.GetMouseButtonDown(1))
        {
           
        }
    }


    private void SpawnShowObj(Tile startingTile, int width, int height) 
    {
        if (spawnedShowObj.Count > 0)
            ClearShowObj();

        bool canInteract = true;

        int halfWidth = width / 2;
        int halfHeight = height / 2;


        for (int y = startingTile.coord.y  - (height - halfHeight); y < startingTile.coord.y + halfHeight; y++)
        {
            for (int x = startingTile.coord.x - (width - halfWidth); x < startingTile.coord.x + halfWidth; x++)
            {

                if (x < 0 || y < 0 || x >= map.textSize || y >= map.textSize) 
                {
                    canInteract = false;
                    break;
                }

                var objRef =Instantiate(showObj, Vector3.Lerp(map.tilesArray[x, y].BotRight, map.tilesArray[x, y].TopLeft, 0.5f), Quaternion.identity);


                objRef.transform.parent = this.transform;
                spawnedShowObj.Add(objRef);

                if (map.tilesArray[x, y].tileType == TileType.WATER)
                    canInteract = false;

            }
        }

        CheckInteractionAllowance(canInteract);
    }
    private void ClearShowObj() 
    {
        for (int i = spawnedShowObj.Count; i-- > 0;)
        {
            Destroy(spawnedShowObj[i]);
            spawnedShowObj.RemoveAt(i);
        }
    }
    private void CheckInteractionAllowance(bool canInteract) 
    {
        foreach (var showObj in spawnedShowObj)
        {
            showObj.GetComponent<Renderer>().material = canInteract == true ? transparent : transparentError;
        }
    }


    private bool AABBCol(Vector3 player, Tile tile)
    {
        if (player.x >= tile.BotLeft.x && player.x < tile.TopRight.x)
        {
            if (player.z >= tile.BotLeft.z && player.z < tile.TopRight.z)
            {
                return true;
            }
        }

        return false;
    }
}
