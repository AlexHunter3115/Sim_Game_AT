using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;



// we need a way to disavle the gen

public class MouseInteraction : MonoBehaviour
{
    [SerializeField] DataHolder dataHolder;



    private string[] buildNames = new string[6] { "Council", "Farm", "Mine", "House", "Sawmill","Dock" };


    [SerializeField] Material transparent;
    [SerializeField] Material transparentError;
    [SerializeField] GameObject showObj;

    public LayerMask mapLayer;

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

    private string guid;
    private int typeSelected = 0;
    private bool showToolTip = false;


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


                switch (hit.transform.gameObject.layer)
                {
                    case 6:  //map
                        for (int i = 0; i < GeneralUtil.map.tilesArray.Length; i++)
                        {
                            int row = i / GeneralUtil.map.textSize;
                            int col = i % GeneralUtil.map.textSize;

                            if (GeneralUtil.AABBCol(hit.point, GeneralUtil.map.tilesArray[row, col]))
                            {
                                GeneralUtil.map.ClickedTile = GeneralUtil.map.tilesArray[row, col];

                                var sel = GeneralUtil.buildingSize[(BuildingData.BUILDING_TYPE)selectedIndex];
                                SpawnShowObj(GeneralUtil.map.ClickedTile, sel.x, sel.y);

                                break;
                            }
                        }
                        typeSelected = 0;
                        break;
                    case 8:  //building

                        showToolTip = true;
                        var compBuilding = hit.transform.GetComponent<BuildingIdentifier>();
                        guid = compBuilding.buildingData.guid;

                        typeSelected = 2;

                        break;
                    case 9:  //citizen

                        showToolTip = true;
                        var compAgent = hit.transform.GetComponent<Agent>();
                        guid = compAgent.data.guid;
                        typeSelected = 1;
                        break;



                    default:
                        break;
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            if (canSpawn) 
            {
                if (selectedCoords.Count != 0)
                {
                    //if (!CheckEnoughResources((BuildingData.BUILDING_TYPE)selectedIndex))
                    //    return;

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



        if (Input.GetKeyDown(KeyCode.Escape))
            showToolTip = false;



        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            if (selectedIndex + 1>= GeneralUtil.allowedDict.Count) { selectedIndex = 0; }
            else { selectedIndex += 1; }

            ClearSection();
            GeneralUtil.Ui.SetSelIndexText(buildNames[selectedIndex]);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedIndex -1  < 0) { selectedIndex = GeneralUtil.allowedDict.Count -1; }
            else { selectedIndex -= 1; }

            ClearSection();

            GeneralUtil.Ui.SetSelIndexText(buildNames[selectedIndex]);
        }


        if (Input.GetMouseButtonDown(1))
        {
            //deselect everything
            if (spawnedShowObj.Count > 0)
                ClearShowObj();
            showToolTip = false;

            typeSelected = 0;
        }
    }






    private bool CheckEnoughResources(BuildingData.BUILDING_TYPE type) 
    {
        var list = GeneralUtil.ResourcesWSFSStart[type];

        int wood = list[0];
        int stone = list[1];
        int food = list[2];
        int sand = list[3];

        var bank = GeneralUtil.bank;

        if (bank.woodAmount < wood) 
        {
            return false;
        }
        if (bank.stoneAmount < stone)
        {
            return false;
        }
        if (bank.foodAmount < food)
        {
            return false;
        }
        if (bank.sandAmount < sand)
        {
            return false;
        }


        bank.ChangeFoodAmount(-food);
        bank.ChangeWoodAmount(-wood);
        bank.ChangeStoneAmount(-stone);
        bank.ChangeSandAmount(-sand);

        return true;
    }







    //this doesnt work something is not turning them into blocked
    private bool SpawnBuilding() 
    {

        foreach (var cord in selectedCoords)
        {
            GeneralUtil.map.tilesArray[cord.coord.x, cord.coord.y].tileType = TileType.BLOCKED;
        }

        var objRef = Instantiate(council, middleTile.midCoord, Quaternion.identity);
        var BID = objRef.GetComponent<BuildingIdentifier>();
        BID.init(middleTile, GeneralUtil.buildingSize[(BuildingData.BUILDING_TYPE)selectedIndex],selectedCoords);

        GeneralUtil.dataBank.buildingDict.Add(BID.guid, BID.buildingData);

       
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


                foreach (var type in GeneralUtil.allowedDict[0])
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




    private void OnGUI()
    {
        if (showToolTip == true) 
        {
            switch (typeSelected)
            {
                case 1:
                    GuiAgent();
                    break;

                case 2:
                    GuiBuilidng();
                    break;

                default:
                    break;
            }
        }
    }




    private void GuiAgent() 
    {
        var npcData = GeneralUtil.dataBank.npcDict[guid];



        GUI.Box(new Rect(5, 5, 160, 120), "");
        GUI.Label(new Rect(10, 10, 100, 20), "name");


        GUI.Label(new Rect(10, 20, 100, 20), "stamina");
        GUI.Label(new Rect(10, 30, 100, 20), "health");
        GUI.Label(new Rect(10, 40, 100, 20), "hunger");
        GUI.Label(new Rect(10, 50, 100, 20), "gender");
        GUI.Label(new Rect(10, 60, 100, 20), "speed");


        GUI.Label(new Rect(10, 70, 100, 20), "age state");

        if (npcData.refToWorkPlace != null)
            GUI.Label(new Rect(10, 80, 100, 20), "job guid");

        if (npcData.refToHouse != null)
            GUI.Label(new Rect(10, 90, 100, 20), "house guid");


        GUI.Label(new Rect(10, 100, 100, 20), "mother");
        GUI.Label(new Rect(10, 100, 100, 20), "father");

        GUI.Label(new Rect(10, 110, 100, 20), "list of children");

        if (npcData.children.Count > 0) 
        {
            for (int i = 0; i < npcData.children.Count; i++)
            {
                GUI.Label(new Rect(10, 120 + (i * 10), 100, 20), "child");
            }
        }
       

    }


    private void GuiBuilidng()
    {
        BuildingData buildingData = null;

        if (GeneralUtil.dataBank.buildingDict.ContainsKey(guid))
            buildingData = GeneralUtil.dataBank.buildingDict[guid];
        else 
        {
            showToolTip = false;
            return;
        }



        GUI.Box(new Rect(5, 5, 160, 250), "");
        GUI.Label(new Rect(10, 10, 100, 20), "building type");

        GUI.Label(new Rect(10, 20, 100, 20), "building status");


        GUI.Label(new Rect(10, 30, 100, 20), "list of workers");


        for (int i = 0; i < buildingData.workers.Count; i++)
        {
            if (GUI.Button(new Rect(10, 60 + (i * 30), 100, 20), $"{buildingData.workers[i].name}"))
            {
                typeSelected = 1;
                guid = buildingData.workers[i].guid;
            }
        }
        
        


        GUI.Label(new Rect(10, 180 , 100, 20), "building age");


        if (GUI.Button(new Rect(10, 220, 100, 20), "Delete Me"))
        {
            buildingData.buildingID.DeleteBuilding();
        }

    }

}
