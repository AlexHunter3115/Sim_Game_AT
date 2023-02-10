using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;



// we need a way to disavle the gen

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


    [SerializeField] Material transparent;
    [SerializeField] Material transparentError;
    [SerializeField] GameObject showObj;

    [SerializeField] LayerMask mapLayer;

    private List<GameObject> spawnedShowObj = new List<GameObject>();
    public Vector2Int selectionGridSize = new Vector2Int(0, 0);

    private List<Tile> selectedCoords = new List<Tile>();

    private Tile middleTile;

    [SerializeField] int selectedIndex = 0;

    [Space(30)]
    [SerializeField] GameObject council;
    [SerializeField] GameObject sawMill;
    [SerializeField] GameObject mine;
    [SerializeField] GameObject dock;
    [SerializeField] GameObject house;

    private bool canSpawn = false;


    private void Start()
    {
        GeneralUtil.Ui.SetSelIndexText(buildNames[selectedIndex]);
    }

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
                for (int i = 0; i < GeneralUtil.map.tilesArray.Length; i++)
                {
                    int row = i / GeneralUtil.map.textSize;
                    int col = i % GeneralUtil.map.textSize;

                    if (AABBCol(hit.point, GeneralUtil.map.tilesArray[row, col]))
                    {
                        // Debug.Log("find a collison in the AABB");
                        GeneralUtil.map.ClickedTile = GeneralUtil.map.tilesArray[row, col];

                        var sel = buildingSize[(BuildingData.BUILDING_TYPE)selectedIndex];
                        SpawnShowObj(GeneralUtil.map.ClickedTile, sel.x,sel.y);

                        break;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            if (canSpawn) 
            {
                if (selectedCoords.Count != 0)
                {
                    var success = SpawnBuilding();
                    ClearSection();


                    if (success)
                    {
                    }
                    else
                    {
                     
                    }
                }
            }
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



    //this doesnt work something is not turning them into blocked
    private bool SpawnBuilding() 
    {
        var objRef = Instantiate(council, middleTile.midCoord, Quaternion.identity);
        var BID = objRef.GetComponent<BuildingIdentifier>();
        BID.init(middleTile, buildingSize[(BuildingData.BUILDING_TYPE)selectedIndex],selectedCoords);

        GeneralUtil.dataBank.buildingDict.Add(BID.guid, BID.buildingData);

        foreach (var cord in selectedCoords)
        {
            GeneralUtil.map.tilesArray[cord.coord.x,cord.coord.y].tileType = TileType.BLOCKED;
        }
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

                if (x < 0 || y < 0 || x >= GeneralUtil.map.textSize || y >= GeneralUtil.map.textSize)
                {
                    canInteract = false;
                    break;
                }

                if (tileCounter == midTile)
                {
                    middleTile = GeneralUtil.map.tilesArray[x, y];
                }

                var objRef = Instantiate(showObj, Vector3.Lerp(GeneralUtil.map.tilesArray[x, y].BotRight, GeneralUtil.map.tilesArray[x, y].TopLeft, 0.5f), Quaternion.identity);

                objRef.transform.parent = this.transform;
                spawnedShowObj.Add(objRef);

                bool typeCheck = false;


                foreach (var type in allowedDict[0])
                {
                    if (GeneralUtil.map.tilesArray[x, y].tileType == (TileType)type) 
                    {
                        typeCheck = true;
                        break;
                    }
                }

                selectedCoords.Add(GeneralUtil.map.tilesArray[x,y]);

                if (!typeCheck) 
                {
                    canInteract = false;
                }

                tileCounter++;
            }
        }
        canSpawn = canInteract;
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

        foreach (var tiles in selectedCoords)
        {
            GeneralUtil.map.tilesArray[tiles.coord.x, tiles.coord.y].busy = false;
            if (GeneralUtil.map.tilesArray[tiles.coord.x, tiles.coord.y].tileObject != null)
                Destroy(GeneralUtil.map.tilesArray[tiles.coord.x, tiles.coord.y].tileObject);
        }

        ClearShowObj();
        canSpawn = false;
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
