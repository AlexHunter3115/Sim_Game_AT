using System.Collections.Generic;
using System.Linq;
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

                                GeneralUtil.map.clickedTile = GeneralUtil.WorldPosToTile(hit.point);

                                SpawnShowObj(GeneralUtil.map.clickedTile, sel.x, sel.y);
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

                        if (!CheckEnoughResources(selectedIndex))
                            return;

                        var success = SpawnBuilding(selectedIndex);

                        ClearSection(true);
                        ClearShowObj();

                        if (success)
                        {
                            var building =  GeneralUtil.buildingScritpable.buildingStats[selectedIndex];

                            GeneralUtil.resourceBank.woodMaxAmount += building.BankAmountWSFS[0];
                            GeneralUtil.resourceBank.stoneMaxAmount += building.BankAmountWSFS[1];
                            GeneralUtil.resourceBank.foodMaxAmount += building.BankAmountWSFS[2];
                            GeneralUtil.resourceBank.sandMaxAmount += building.BankAmountWSFS[3];
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
                    SpawnShowObj(GeneralUtil.map.clickedTile, sel.x, sel.y);


                    GeneralUtil.Ui.SetSelIndexText(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].name);


                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedIndex - 1 < (spawnedCouncil == false ? 0 : 1)) { selectedIndex = GeneralUtil.buildingScritpable.buildingStats.Count - 1; }
                    else { selectedIndex -= 1; }

                    ClearSection(false);

                    var sel = GeneralUtil.buildingScritpable.buildingStats[selectedIndex].size;
                    SpawnShowObj(GeneralUtil.map.clickedTile, sel.x, sel.y);

                    GeneralUtil.Ui.SetSelIndexText(GeneralUtil.buildingScritpable.buildingStats[selectedIndex].name);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (graphChoiceTwo + 1 >= GeneralUtil.dataBank.ArrayOfLists.Length) { graphChoiceTwo = 0; }
                    else { graphChoiceTwo += 1; }

                    if (graphChoiceTwo == graphChoiceOne)
                    {
                        graphChoiceTwo = 1;
                    }

                    CallDrawGraph();

                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (graphChoiceTwo - 1 < 0) { graphChoiceTwo = GeneralUtil.dataBank.ArrayOfLists.Length - 1; }
                    else { graphChoiceTwo -= 1; }

                    if (graphChoiceTwo == graphChoiceOne)
                    {
                        graphChoiceTwo = GeneralUtil.dataBank.ArrayOfLists.Length - 2;
                    }

                    CallDrawGraph();
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (graphChoiceOne - 1 < 0)
                    {
                        graphChoiceOne = GeneralUtil.dataBank.ArrayOfLists.Length - 1;
                    }
                    else
                    {
                        graphChoiceOne -= 1;
                    }

                    if (graphChoiceOne == graphChoiceTwo)
                    {
                        graphChoiceOne = GeneralUtil.dataBank.ArrayOfLists.Length - 2;
                    }

                    CallDrawGraph();
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (graphChoiceOne + 1 >= GeneralUtil.dataBank.ArrayOfLists.Length) { graphChoiceOne = 0; }
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
                    GeneralUtil.map.plane.GetComponent<Renderer>().material.mainTexture = GeneralUtil.map.textMap;
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                GeneratePoissantPoints(selectedIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            GeneralUtil.Ui.showingMenu = !GeneralUtil.Ui.showingMenu;
            Time.timeScale = GeneralUtil.Ui.showingMenu == true ? 0 : 1;
        }
    }


    #region poissant

    public List<PoissantPoints> GeneratePoissantPoints(int buildingIndex) 
    {
        var grid = GeneralUtil.map.tilesArray;
         List<PoissantPoints> listOfPoissantsPoints = new List<PoissantPoints>();

        //get the boundary of the current square

        Tile firstTileTop = null;
        Tile lastTileBottom = null;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y  = 0; y < grid.GetLength(1); y++)
            {
                if (firstTileTop == null && GeneralUtil.dataBank.allowedBuildingLocations.Contains(grid[x,y].coord)) 
                {
                    firstTileTop = grid[x,y];
                }

                if (GeneralUtil.dataBank.allowedBuildingLocations.Contains(grid[x, y].coord))
                {
                    lastTileBottom = grid[x, y];
                }
            }
        }

        Tile firstTileLeft = null;
        Tile lastTileRight = null;

        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (firstTileLeft == null && GeneralUtil.dataBank.allowedBuildingLocations.Contains(grid[x, y].coord))
                {
                    firstTileLeft = grid[x, y];
                }

                if (GeneralUtil.dataBank.allowedBuildingLocations.Contains(grid[x, y].coord))
                {
                    lastTileRight = grid[x, y];
                }
            }
        }

        var pos1 = new Vector3(firstTileTop.TopLeft.x,0,firstTileLeft.BotLeft.z);
        var pos2 = new Vector3(lastTileBottom.BotRight.x,0,lastTileRight.TopRight.z);

        listOfPoissantsPoints = new List<PoissantPoints>();

        foreach (var building in GeneralUtil.dataBank.buildingDict.Values)
        {
            listOfPoissantsPoints.Add(new PoissantPoints(building.buildingID.transform.position, building.stats.poissantRadius, true));
        }

        float width = pos2.x - pos1.x;
        float height = pos1.z - pos2.z;

        int tries = 0;

        for (int i = 0; i < GeneralUtil.dataBank.allowedBuildingLocations.Count/2; i++)
        {
            float x = Random.Range(pos1.x, pos1.x + width);
            float y = Random.Range(pos2.z, pos2.z + height);

            bool add = true;

            var newPossiblePoints = new PoissantPoints(new Vector3(x, 0, y), GeneralUtil.buildingScritpable.buildingStats[buildingIndex].poissantRadius, false);

            foreach (var alreadyTherePoint in listOfPoissantsPoints)
            {
                if (!FarEnoughApart(newPossiblePoints, alreadyTherePoint)) 
                {
                    tries++;
                    add = false;
                    break;
                }
            }

            if (add == true) 
            {
                listOfPoissantsPoints.Add(newPossiblePoints);
            }

            if (tries >= GeneralUtil.dataBank.allowedBuildingLocations.Count /4) 
            {
                break;
            }
        }

        //deletes the point if in a non allowed area just for sanity reasons  and gets rid of points from other buidlings
        for (int i = listOfPoissantsPoints.Count; i-- > 0;)
        {
            var tile = GeneralUtil.WorldPosToTile(listOfPoissantsPoints[i].position);

            if (!GeneralUtil.dataBank.allowedBuildingLocations.Contains(tile.coord) || listOfPoissantsPoints[i].buildingHere) 
            {
                listOfPoissantsPoints.RemoveAt(i);
            }
            else 
            {
                SpawnShowObj(tile, GeneralUtil.buildingScritpable.buildingStats[buildingIndex].size.x, GeneralUtil.buildingScritpable.buildingStats[buildingIndex].size.y);

                if (!canSpawn) 
                {
                    listOfPoissantsPoints.RemoveAt(i);
                }
            }
        }

        return listOfPoissantsPoints;
    }


    /// <summary>
    /// returns true if it does not interfere
    /// </summary>
    /// <param name="proposedPoint"></param>
    /// <param name="establishedPoint"></param>
    /// <returns></returns>
    private bool FarEnoughApart(PoissantPoints proposedPoint, PoissantPoints establishedPoint) 
    {
        float distance = Vector3.Distance(proposedPoint.position, establishedPoint.position);

        float sumOfRadii = proposedPoint.radius + establishedPoint.radius;

        if (distance > sumOfRadii)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    #endregion


    public void CallDrawGraph()
    {
        GeneralUtil.graphRef.DrawGraph(GeneralUtil.dataBank.ArrayOfLists[graphChoiceOne], GeneralUtil.dataBank.ArrayOfLists[graphChoiceTwo]);

        GeneralUtil.graphRef.graphOneString.text = GeneralUtil.dataBank.ArrayOfListsNames[graphChoiceTwo];
        GeneralUtil.graphRef.graphTwoString.text = GeneralUtil.dataBank.ArrayOfListsNames[graphChoiceOne];
    }

    /// <summary>
    /// returns true if there are enough resrouces avaialble to build 
    /// </summary>
    /// <returns></returns>
    private bool CheckEnoughResources(int index)
    {
        var list = GeneralUtil.buildingScritpable.buildingStats[index].startCostWSFS;

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
        var obj = Instantiate(floatingText, parent.position + new Vector3(0,4,0), Quaternion.identity, parent);
        obj.GetComponent<TMP_Text>().text = text;
        obj.GetComponent<TMP_Text>().color = color;
    }

    public bool SpawnBuildingAuto(Tile centerTile, int indexBuilding) 
    {
        var sel = GeneralUtil.buildingScritpable.buildingStats[indexBuilding].size;

        SpawnShowObj(centerTile, sel.x, sel.y);
        var success =SpawnBuilding(indexBuilding);

        if (success) 
        {
            GeneralUtil.Ui.SetMessage($"The AI has created a {GeneralUtil.buildingScritpable.buildingStats[indexBuilding].name}", Color.green);
        }
        else 
        {
            GeneralUtil.Ui.SetMessage($"The AI tried to place a {GeneralUtil.buildingScritpable.buildingStats[indexBuilding].name} but there was an issue", Color.red);
        }

        return true;
    }

    private bool SpawnBuilding(int buildingIndex)
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

        var objRef = Instantiate(GeneralUtil.buildingScritpable.buildingStats[buildingIndex].building, new Vector3(middleTile.midCoord.x + GeneralUtil.buildingScritpable.buildingStats[buildingIndex].centerOffset.x, middleTile.midCoord.y, middleTile.midCoord.z + GeneralUtil.buildingScritpable.buildingStats[selectedIndex].centerOffset.y), Quaternion.identity);
        var BID = objRef.GetComponent<BuildingIdentifier>();
        BID.init(middleTile, GeneralUtil.buildingScritpable.buildingStats[buildingIndex].size, selectedCoords, buildingIndex);

        GeneralUtil.dataBank.buildingDict.Add(BID.guid, BID.buildingData);

        //GeneralUtil.map.UpdateMapTexture();
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

        //if (!GeneralUtil.dataBank.allowedBuildingLocations.Contains(middleTile.coord) && selectedIndex !=0)
        //    canInteract = false;

        canSpawn = canInteract;
        SetTextureDependingOnInteractionAllowance(canInteract);
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
    private void SetTextureDependingOnInteractionAllowance(bool canInteract)
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
        GUILayout.Label( $"Speed: {npcData.speed}");
        GUILayout.Space(-5);
        GUILayout.Label( $"Age: {npcData.name}");

        GUILayout.Space(10);
        if (npcData.refToWorkPlace != null)
        {
            GUILayout.Label($"Works at: {npcData.refToWorkPlace.typeOfBuilding}");
        }
        if (npcData.refToHouse != null)
        {
            GUILayout.Label( $"House at: {npcData.refToHouse.centerCoord}");
        }

        GUILayout.Space(10);
        if (npcData.parentsArr[0] != null)
            GUILayout.Label($"parent 1: {npcData.parentsArr[0].name}");

        if (npcData.parentsArr[0] != null)
            GUILayout.Label($"parent 2: {npcData.parentsArr[1].name}");

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

        GUILayout.Label($"building type: {buildingData.stats.type}");
        GUILayout.Space(-5);
        GUILayout.Label(new GUIContent() {text = $"building Health: {buildingData.health}" } );
        GUILayout.Space(-5);


        GUILayout.Label("list of workers");
        for (int i = 0; i < buildingData.workers.Count; i++)
        {
            if (GUILayout.Button( $"{buildingData.workers[i].name}"))
            {
                typeSelected = 1;
                guid = buildingData.workers[i].guid;
            }
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Delete Me"))
        {
            if (buildingData.buildingID.buildingIndex != 0)
                buildingData.buildingID.DeleteBuilding();
            else
                SpawnFloatingText("Cant delete this building", Color.yellow, buildingData.buildingID.transform);
        }


        if (buildingData.tilesWithResourcesInRange.Count > 0)
        {
            buildingData.buildingID.drawResources = GUILayout.Toggle(buildingData.buildingID.drawResources, "draw resources");
        }
        else 
        {
            GUILayout.Label(new GUIContent() { text = "This buidling has no resources close to it", tooltip = "this is a tooltip" });
        }

  
        if (buildingData.stats.type == BuildingData.BUILDING_TYPE.HOUSE) 
        {
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent() { text = "The children in this house", tooltip = "this is a tooltip" });

            var comp = buildingData.buildingID.transform.GetComponent<HouseBuilding>();

            for (int i = 0; i < comp.childrenHabitantsGUID.Count; i++)
            {
                GUILayout.Label(new GUIContent() { text = $"{GeneralUtil.dataBank.npcDict[comp.childrenHabitantsGUID[i]].name} and its age is {GeneralUtil.dataBank.npcDict[comp.childrenHabitantsGUID[i]].daysAlive}", tooltip = "this is a tooltip" });
            }
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


public class PoissantPoints 
{
    public Vector3 position;
    public float radius;
    public bool buildingHere;

    public PoissantPoints(Vector3 pos, float radius, bool buidling) 
    {
        position = pos;
        this.radius = radius; 
        buildingHere = buidling;
    }
}
