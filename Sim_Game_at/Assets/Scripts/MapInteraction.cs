using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class MapInteraction : MonoBehaviour
{
    [SerializeField] DataHolder dataHolder;

    [SerializeField] Material transparent;
    [SerializeField] Material transparentError;
    [SerializeField] GameObject showObj;

    public LayerMask mapLayer;

    private List<GameObject> spawnedShowObj = new List<GameObject>();
    public Vector2Int selectionGridSize = new Vector2Int(0, 0);

    private List<Tile> selectedCoords = new List<Tile>();

    private Tile middleTile;

    [SerializeField] int selectedIndex = 0;

    private bool canSpawn = false;

    private string guid;
    private int typeSelected = 0;
    private bool showToolTip = false;

    public Vector2Int testCoord = new Vector2Int(0, 0);

    private void Start()
    {
        GeneralUtil.Ui.SetSelIndexText(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].name);
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
                        var sel = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size;

                        GeneralUtil.map.ClickedTile = GeneralUtil.WorldTileCoord(hit.point);

                        Debug.Log(GeneralUtil.map.ClickedTile.coord);

                        SpawnShowObj(GeneralUtil.map.ClickedTile, sel.x, sel.y);
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
                    ClearSection(true);


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
            if (selectedIndex + 1>= GeneralUtil.buildingScritpable.buildingStats.Count) { selectedIndex = 0; }
            else { selectedIndex += 1; }

            ClearSection(false);


            var sel = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size;
            SpawnShowObj(GeneralUtil.map.ClickedTile, sel.x, sel.y);


            GeneralUtil.Ui.SetSelIndexText(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].name);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedIndex -1  < 0) { selectedIndex = GeneralUtil.buildingScritpable.buildingStats.Count - 1; }
            else { selectedIndex -= 1; }

            ClearSection(false);



            var sel = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size;
            SpawnShowObj(GeneralUtil.map.ClickedTile, sel.x, sel.y);

            GeneralUtil.Ui.SetSelIndexText(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].name);

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






    private bool CheckEnoughResources() 
    {
        var list = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].startCostWSFS;

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



    private bool SpawnBuilding() 
    {
        foreach (var cord in selectedCoords)
        {
            GeneralUtil.map.tilesArray[cord.coord.x, cord.coord.y].tileType = TileType.BLOCKED;
        }

        var objRef = Instantiate(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].building, new Vector3(middleTile.midCoord.x  + GeneralUtil.buildingScritpable.buildingStats[selectedIndex].centerOffset.x, middleTile.midCoord.y, middleTile.midCoord.z + GeneralUtil.buildingScritpable.buildingStats[selectedIndex].centerOffset.y)   , Quaternion.identity);
        var BID = objRef.GetComponent<BuildingIdentifier>();
        BID.init(middleTile, GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size, selectedCoords,selectedIndex);

        GeneralUtil.dataBank.buildingDict.Add(BID.guid, BID.buildingData);

        GeneralUtil.map.UpdateMapTexture();
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


                foreach (var type in GeneralUtil.buildingScritpable.buildingStats[selectedIndex].allowedTileTypes)
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


    private void ClearSection(bool delResources) 
    {
        if (delResources)
        {
            foreach (var tiles in selectedCoords)
            {
                GeneralUtil.map.tilesArray[tiles.coord.x, tiles.coord.y].busy = false;
                if (GeneralUtil.map.tilesArray[tiles.coord.x, tiles.coord.y].tileObject != null)
                    Destroy(GeneralUtil.map.tilesArray[tiles.coord.x, tiles.coord.y].tileObject);
            }
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
        GUI.Label(new Rect(10, 10, 100, 20), $"Name: {npcData.name}");


        GUI.Label(new Rect(10, 20, 100, 20), $"Health: {npcData.health}");
        GUI.Label(new Rect(10, 30, 100, 20), $"Stamina: {npcData.stamina}");
        GUI.Label(new Rect(10, 40, 100, 20), $"Hunger: {npcData.hunger}");
        GUI.Label(new Rect(10, 50, 100, 20), $"Gender: {npcData.gender}");
        GUI.Label(new Rect(10, 60, 100, 20), $"Speed: {npcData.speed}");


        GUI.Label(new Rect(10, 70, 100, 20), $"Age: {npcData.name}");

        if (npcData.refToWorkPlace != null) 
        {
            GUI.Label(new Rect(10, 80, 100, 20), $"Work: {npcData.refToWorkPlace.guid}");

        }

        if (npcData.refToHouse != null) 
        {

            GUI.Label(new Rect(10, 90, 100, 20), $"House: {npcData.refToHouse.guid}");

        }


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






    private void OnDrawGizmos()
    {
        if (dataHolder != null)
            Gizmos.DrawSphere(GeneralUtil.map.tilesArray[testCoord.x, testCoord.y].midCoord, 0.5f);
    }
}
