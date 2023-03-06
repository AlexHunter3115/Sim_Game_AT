using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class MapInteraction : MonoBehaviour
{
    [SerializeField] DataHolder dataHolder;

    public GameObject floatingText;

    [SerializeField] Material transparent;
    [SerializeField] Material transparentError;
    [SerializeField] GameObject showObj;

    [SerializeField] GameObject ghostObjHolder;
 
    private List<GameObject> spawnedShowObj = new List<GameObject>();
    public Vector2Int selectionGridSize = new Vector2Int(0, 0);

    private List<Tile> selectedCoords = new List<Tile>();

    private Tile middleTile;

    [SerializeField] int selectedIndex = 0;

    private bool canSpawn = false;

    private string guid;
    private int typeSelected = 0;
    private bool showToolTip = false;

    private bool arrowKeyLock = false;

    private int graphChoiceOne = 0;
    private int graphChoiceTwo = 1;

    public bool spawnedCouncil = false;

    private bool showingAllowedTiles = false;


    private void Start()
    {
        GeneralUtil.Ui.SetSelIndexText(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].name);
        GeneralUtil.mapInteraction = this;
    }

    void Update()
    {
        if (!GeneralUtil.Ui.showingMenu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.GetMask("Resources")))
                {
                    switch (hit.transform.gameObject.layer)
                    {
                        case 6:  //map
                            if (!showToolTip)
                            {
                                var sel = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size;

                                GeneralUtil.map.ClickedTile = GeneralUtil.WorldPosToTile(hit.point);

                                SpawnShowObj(GeneralUtil.map.ClickedTile, sel.x, sel.y);
                                typeSelected = 0;
                            }

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
                        if (spawnedCouncil && selectedIndex == 0)
                            return;

                        //if (!CheckEnoughResources())
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

            if (arrowKeyLock == false)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (selectedIndex + 1 >= GeneralUtil.buildingScritpable.buildingStats.Count) { selectedIndex = spawnedCouncil == false ? 0 : 1; }
                    else { selectedIndex += 1; }

                    ClearSection(false);


                    var sel = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size;
                    SpawnShowObj(GeneralUtil.map.ClickedTile, sel.x, sel.y);


                    GeneralUtil.Ui.SetSelIndexText(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].name);


                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedIndex - 1 < (spawnedCouncil == false ? 0 : 1)) { selectedIndex = GeneralUtil.buildingScritpable.buildingStats.Count - 1; }
                    else { selectedIndex -= 1; }

                    ClearSection(false);

                    var sel = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size;
                    SpawnShowObj(GeneralUtil.map.ClickedTile, sel.x, sel.y);

                    GeneralUtil.Ui.SetSelIndexText(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].name);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (graphChoiceTwo + 1 >= GeneralUtil.dataBank.arrayOfLists.Length) { graphChoiceTwo = 0; }
                    else { graphChoiceTwo += 1; }

                    if (graphChoiceTwo == graphChoiceOne)
                    {
                        graphChoiceTwo = 1;
                    }

                    CallDrawGraph();

                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (graphChoiceTwo - 1 < 0) { graphChoiceTwo = GeneralUtil.dataBank.arrayOfLists.Length - 1; }
                    else { graphChoiceTwo -= 1; }

                    if (graphChoiceTwo == graphChoiceOne)
                    {
                        graphChoiceTwo = GeneralUtil.dataBank.arrayOfLists.Length - 2;
                    }

                    CallDrawGraph();

                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (graphChoiceOne - 1 < 0)
                    {
                        graphChoiceOne = GeneralUtil.dataBank.arrayOfLists.Length - 1;
                    }
                    else
                    {
                        graphChoiceOne -= 1;
                    }

                    if (graphChoiceOne == graphChoiceTwo)
                    {
                        graphChoiceOne = GeneralUtil.dataBank.arrayOfLists.Length - 2;
                    }

                    CallDrawGraph();
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (graphChoiceOne + 1 >= GeneralUtil.dataBank.arrayOfLists.Length) { graphChoiceOne = 0; }
                    else { graphChoiceOne += 1; }

                    if (graphChoiceTwo == graphChoiceOne)
                    {
                        graphChoiceOne = 1;
                    }

                    CallDrawGraph();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                //deselect everything
                if (spawnedShowObj.Count > 0)
                    ClearShowObj();
                showToolTip = false;

                typeSelected = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (GeneralUtil.graphRef.MainGraph.gameObject.activeSelf)
                {
                    arrowKeyLock = false;
                    GeneralUtil.graphRef.MainGraph.gameObject.SetActive(false);
                }
                else
                {
                    arrowKeyLock = true;

                    GeneralUtil.graphRef.MainGraph.gameObject.SetActive(true);


                    CallDrawGraph();
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                showingAllowedTiles = !showingAllowedTiles;

                if (showingAllowedTiles) 
                {
                    GeneralUtil.dataBank.RecalcAllAllowedTiles();
                    GeneralUtil.map.DrawAllowedTiles();
                }
                else
                    GeneralUtil.map.UpdateMapTexture();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            GeneralUtil.Ui.showingMenu = !GeneralUtil.Ui.showingMenu;
            Time.timeScale = GeneralUtil.Ui.showingMenu == true ? 0 : 1;
        }
    }

    public void CallDrawGraph()
    {
        GeneralUtil.graphRef.DrawGraph(GeneralUtil.dataBank.arrayOfLists[graphChoiceOne], GeneralUtil.dataBank.arrayOfLists[graphChoiceTwo]);

        GeneralUtil.graphRef.graphOneString.text = GeneralUtil.dataBank.arrayOfListsNames[graphChoiceTwo];
        GeneralUtil.graphRef.graphTwoString.text = GeneralUtil.dataBank.arrayOfListsNames[graphChoiceOne];
    }

    /// <summary>
    /// returns true if there are enough resrouces avaialble to build 
    /// </summary>
    /// <returns></returns>
    private bool CheckEnoughResources()
    {
        var list = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].startCostWSFS;

        int wood = list[0];
        int stone = list[1];
        int food = list[2];
        int sand = list[3];

        var bank = GeneralUtil.resourceBank;

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

    public void SpawnFloatingText(string text, Color color, Transform parent) 
    {
        var obj = Instantiate(floatingText, parent.position, Quaternion.identity, parent);
        obj.GetComponent<TMP_Text>().text = text;
        obj.GetComponent<TMP_Text>().color = color;
    }

    private bool SpawnBuilding()
    {
        foreach (var cord in selectedCoords)
        {
            GeneralUtil.map.tilesArray[cord.coord.x, cord.coord.y].tileType = TileType.BLOCKED;

            if (GeneralUtil.map.tilesArray[cord.coord.x, cord.coord.y].tileObject != null)
            {
                Destroy(GeneralUtil.map.tilesArray[cord.coord.x, cord.coord.y].tileObject);
                GeneralUtil.map.tilesArray[cord.coord.x, cord.coord.y].tileObject = null;
            }
        }

        var objRef = Instantiate(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].building, new Vector3(middleTile.midCoord.x + GeneralUtil.buildingScritpable.buildingStats[selectedIndex].centerOffset.x, middleTile.midCoord.y, middleTile.midCoord.z + GeneralUtil.buildingScritpable.buildingStats[selectedIndex].centerOffset.y), Quaternion.identity);
        var BID = objRef.GetComponent<BuildingIdentifier>();
        BID.init(middleTile, GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size, selectedCoords, selectedIndex);

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

                objRef.transform.parent = ghostObjHolder.transform;
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

                selectedCoords.Add(GeneralUtil.map.tilesArray[x, y]);

                if (!typeCheck)
                {
                    canInteract = false;
                }

                tileCounter++;
            }
        }

        if (!GeneralUtil.dataBank.allowedBuildingLocations.Contains(middleTile.coord) && selectedIndex !=0)
            canInteract = false;

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


    #region GUi region

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

    public float lastX = 0;
    public float lastY = 0;

    private void GuiAgent()
    {

        var npcData = GeneralUtil.dataBank.npcDict[guid];

        if (npcData.agentObj == null)
        {
            showToolTip = false;
            return;
        }


        var npcObjComp = npcData.agentObj.GetComponent<Agent>();



        GUI.Box(new Rect(5, 5, 140, lastY), "");

        GUILayout.BeginVertical();
        GUILayout.BeginArea(new Rect(10, 10, 100, Screen.height));

        GUILayout.Label( $"Name: {npcData.name}");
        GUILayout.Space(-5);
        GUILayout.Label( $"Health: {npcData.health}");
        GUILayout.Space(-5);
        GUILayout.Label($"Stamina: {npcData.stamina}");
        GUILayout.Space(-5);
        GUILayout.Label( $"Hunger: {npcData.hunger}");
        GUILayout.Space(-5);
        GUILayout.Label( $"Gender: {npcData.gender}");
        GUILayout.Space(-5);
        GUILayout.Label( $"Speed: {npcData.speed}");
        GUILayout.Space(-5);
        GUILayout.Label( $"Age: {npcData.name}");

        GUILayout.Space(10);
        if (npcData.refToWorkPlace != null)
        {
            GUILayout.Label($"Work: {npcData.refToWorkPlace.guid}");
        }

        if (npcData.refToHouse != null)
        {
            GUILayout.Label( $"House: {npcData.refToHouse.guid}");
        }


        GUILayout.Space(10);
        GUILayout.Label( $"mother");
        
        GUILayout.Label( $"father");


        if (npcData.children.Count > 0)
        {
            GUILayout.Label("list of children");
            for (int i = 0; i < npcData.children.Count; i++)
            {
                GUILayout.Label("child");
            }
        }

        GUILayout.Space(10);
        npcObjComp.showPathToggle = GUILayout.Toggle( npcObjComp.showPathToggle, "Show path Mode");

        if (npcObjComp.showPathToggle)
            npcObjComp.switchPathMode = GUILayout.Toggle( npcObjComp.switchPathMode, npcObjComp.switchPathMode == true ? "path mode" : "all tiles checked mode");

        Rect lastRect = GUILayoutUtility.GetLastRect();

        float valToCopy = lastRect.y + lastRect.height + 20;

        if (valToCopy > 40)
            lastY = valToCopy;


        GUILayout.EndArea();
        GUILayout.EndVertical();
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

        GUI.Box(new Rect(5, 5, 140, lastY), "");

        GUILayout.BeginVertical();
        GUILayout.BeginArea(new Rect(10, 10, 100, Screen.height));        

        GUILayout.Label("building type");
        GUILayout.Space(-5);
        GUILayout.Label(new GUIContent() {text = "building Layout", tooltip = "this is a tooltip" } );
        GUILayout.Space(-5);
        GUILayout.Label( "list of workers");

        for (int i = 0; i < buildingData.workers.Count; i++)
        {
            if (GUILayout.Button( $"{buildingData.workers[i].name}"))
            {
                typeSelected = 1;
                guid = buildingData.workers[i].guid;
            }
        }

        GUILayout.Label("Building age");

        if (GUILayout.Button("Delete Me"))
        {
            if (buildingData.buildingID.buildingIndex != 0)
                buildingData.buildingID.DeleteBuilding();
            else
                SpawnFloatingText("Cant delete this building", Color.yellow, buildingData.buildingID.transform);
        }

        Rect lastRect = GUILayoutUtility.GetLastRect();

        float valToCopy = lastRect.y + lastRect.height + 20;

        if (valToCopy > 40)
            lastY = valToCopy;

        GUILayout.EndArea();
        GUILayout.EndVertical();

    }

    #endregion


    
}
