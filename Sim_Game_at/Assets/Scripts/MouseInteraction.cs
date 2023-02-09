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
    [SerializeField] DataHolder dataHolder;

    Dictionary<BuildingData.BUILDING_TYPE, List<int>> allowedDict = new Dictionary<BuildingData.BUILDING_TYPE, List<int>>()
    {
        {BuildingData.BUILDING_TYPE.COUNCIL, new List<int>() {0,1 }  },
        {BuildingData.BUILDING_TYPE.FARM, new List<int>() {0 }  },
        {BuildingData.BUILDING_TYPE.MINE, new List<int>() {1,2 }  },
        {BuildingData.BUILDING_TYPE.HOUSE, new List<int>() {0,1 }  },
        {BuildingData.BUILDING_TYPE.SAWMILL, new List<int>() {0 }  },
        {BuildingData.BUILDING_TYPE.DOCK, new List<int>() {0,3 }  }
    };

    Dictionary<BuildingData.BUILDING_TYPE, Vector2Int> buildingSize = new Dictionary<BuildingData.BUILDING_TYPE, Vector2Int>()
    {
        {BuildingData.BUILDING_TYPE.COUNCIL, new Vector2Int(5,5)  },
        {BuildingData.BUILDING_TYPE.FARM,  new Vector2Int(5,10)  },
        {BuildingData.BUILDING_TYPE.MINE,  new Vector2Int(3,5)  },
        {BuildingData.BUILDING_TYPE.HOUSE,  new Vector2Int(5,5)  },
        {BuildingData.BUILDING_TYPE.SAWMILL,  new Vector2Int(3,5)  },
        {BuildingData.BUILDING_TYPE.DOCK,  new Vector2Int(3,7)  }
    };

    private string[] buildNames = new string[6] { "Council", "Farm", "Mine", "House", "Sawmill","Dock" };





    [SerializeField] MapCreation map;
    [SerializeField] Material transparent;
    [SerializeField] Material transparentError;
    [SerializeField] GameObject showObj;

    [SerializeField] LayerMask mapLayer;

    private List<GameObject> spawnedShowObj = new List<GameObject>();
    public Vector2Int selectionGridSize = new Vector2Int(0, 0);

    private List<Vector2Int> selectedCoords = new List<Vector2Int>();

    private Tile middleTile;

    [SerializeField] int selectedIndex = 0;

    [Space(30)]
    [SerializeField] GameObject council;
    [SerializeField] GameObject sawMill;
    [SerializeField] GameObject mine;
    [SerializeField] GameObject dock;
    [SerializeField] GameObject house;

    //public bool canInteract;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("pressed the right mous but");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //  Debug.Log("called the raycast");
                for (int i = 0; i < map.tilesArray.Length; i++)
                {
                    int row = i / map.textSize;
                    int col = i % map.textSize;

                    if (AABBCol(hit.point, map.tilesArray[row, col]))
                    {
                        // Debug.Log("find a collison in the AABB");
                        map.ClickedTile = map.tilesArray[row, col];

                        var sel = buildingSize[(BuildingData.BUILDING_TYPE)selectedIndex];
                        SpawnShowObj(map.ClickedTile, sel.x,sel.y);
                        break;
                    }
                }
            }
        }




        if (Input.GetKeyDown(KeyCode.Return)) 
        {

            if (selectedCoords.Count != 0) 
            {
                ClearSection();
                var success = SpawnBuilding();


                if (success)
                {
                    Debug.Log("Did spawn fine");
                }
                else
                {
                    Debug.Log("There is an issue");
                }
            }
            // need to check if i am allowed to spawn
            
        }

          


        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            if (selectedIndex + 1>= allowedDict.Count) { selectedIndex = 0; }
            else { selectedIndex += 1; }

            ClearSection();
            GeneralUtil.Ui.SetSelIndexText(buildNames[selectedIndex]);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedIndex -1  < 0) { selectedIndex = allowedDict.Count -1; }
            else { selectedIndex -= 1; }

            ClearSection();

            GeneralUtil.Ui.SetSelIndexText(buildNames[selectedIndex]);
        }


        if (Input.GetMouseButtonDown(1))
        {

        }
    }




    private bool SpawnBuilding() 
    {
        var objRef = Instantiate(council, middleTile.midCoord, Quaternion.identity);
        var BID = objRef.GetComponent<BuildingIdentifier>();
        BID.init(middleTile, buildingSize[(BuildingData.BUILDING_TYPE)selectedIndex]);

        dataHolder.buildingDict.Add(BID.guid, BID.buildingData);

        return true;
    }



    private void SpawnShowObj(Tile startingTile, int width, int height)
    {
        if (spawnedShowObj.Count > 0)
            ClearShowObj();

        bool canInteract = true;

        int halfWidth = width / 2;
        int halfHeight = height / 2;

        int midTile = (width * height) / 2;
        int tileCounter = 0;

        for (int y = startingTile.coord.y - (height - halfHeight); y < startingTile.coord.y + halfHeight; y++)
        {
            for (int x = startingTile.coord.x - (width - halfWidth); x < startingTile.coord.x + halfWidth; x++)
            {

                if (x < 0 || y < 0 || x >= map.textSize || y >= map.textSize)
                {
                    canInteract = false;
                    break;
                }

                if (tileCounter == midTile)
                {
                    middleTile = map.tilesArray[x, y];
                }

                var objRef = Instantiate(showObj, Vector3.Lerp(map.tilesArray[x, y].BotRight, map.tilesArray[x, y].TopLeft, 0.5f), Quaternion.identity);

                objRef.transform.parent = this.transform;
                spawnedShowObj.Add(objRef);

                bool typeCheck = false;


                foreach (var type in allowedDict[0])
                {
                    if (map.tilesArray[x, y].tileType == (TileType)type) 
                    {
                        typeCheck = true;
                        break;
                    }
                }

                selectedCoords.Add(new Vector2Int(x, y));

                if (!typeCheck) 
                {
                    canInteract = false;
                }

                tileCounter++;
            }
        }

        CheckInteractionAllowance(canInteract);
    }
    private void ClearShowObj()
    {
        selectedCoords.Clear();

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
    private void ClearSection() 
    {

        ClearShowObj();

        foreach (var tiles in selectedCoords)
        {
            map.tilesArray[tiles.x, tiles.y].busy = false;
            Destroy(map.tilesArray[tiles.x, tiles.y].tileObject);
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
