using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class MapCreation : MonoBehaviour
{
    public int textSize = 512;

    [Space(30)]
    [Range(1, 10000)]
    public int offsetX = 100;
    [Range(1, 10000)]
    public int offsetY = 100;

    [Space(20)]
    public float scale = 18;
    [Range(0.1f, 0.5f)]
    public float lacu = 5;
    [Range(0.1f, 0.5f)]
    public float pers = 0.5f;
    [Range(2, 6)]
    public int octaves = 3;

    [Space(30)]
    [Range(0.05f,0.5f)]
    public float threasholdGrass = 0.2f;
    [Range(0.5f, 0.75f)]
    public float threasholdHill = 0.65f;
    [Range(0.75f, 0.9f)]
    public float threasholdSnow = 0.89f;

    [Space(30)]
    public bool showGizmos = false;
    public Tile[,] tilesArray = new Tile[0, 0];

    [Space(30)]
    [Header("Map spawn stuff")]

    public List<GameObject> trees;
    public List<GameObject> rocks;
    public List<GameObject> bushes;

    [Space(10)]
    [Range(0.005f, 0.025f)]
    public float percOfStoneOnGrass;

    [Range(0.05f, 0.15f)]
    public float percOfStoneOnHill;

    [Space(10)]
    [Range(3,12)]
    public int regions = 3;

    [Space(10)]
    [Range(0.05f, 0.15f)]
    public float percOfBushInGrass;

    public GameObject agent;

    public GameObject plane;

    public GameObject resourceHolder;
    private Vector3 bottomLeft = new Vector3();
    private Vector3 bottomRight = new Vector3();

    private Vector3 topLeft = new Vector3();
    private Vector3 topRight = new Vector3();

    private List<Vector3> textureVertecies = new List<Vector3>();
    public Tile clickedTile = null;

    public CAtile[,] currCAgrid = new CAtile[0, 0];
    private List<Vector2Int> placesForRocks = new List<Vector2Int>();

    public Texture2D textMap;


    private void Awake()
    {
        GeneralUtil.map = this;
        GeneralUtil.buildingScritpable = Resources.Load("buildingTypes") as BuildingTypes;
    }

    private void Start()
    {
        bool randomize = PlayerPrefs.GetInt("RandomGen") == 1 ? true : false;

        if (randomize) 
        {
            RandomizePerlin();
        }

        currCAgrid = new CAtile[textSize, textSize];

        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = transform;
        plane.transform.localScale = new Vector3(40, 40, 40);
        plane.gameObject.layer = 6;

        tilesArray = PerlinNoise2D(scale, octaves, pers, lacu, offsetX, offsetY);
        
        CreateMapTexture();
        plane.transform.Translate(new Vector3(200, 0, 200));

        MeshRenderer meshRenderer = plane.GetComponent<MeshRenderer>();
        Mesh mesh = meshRenderer.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        bottomLeft = plane.transform.TransformPoint(vertices[0]);
        bottomRight = plane.transform.TransformPoint(vertices[10]);

        topLeft = plane.transform.TransformPoint(vertices[110]);
        topRight = plane.transform.TransformPoint(vertices[vertices.Length - 1]);

        for (float x = 0; x < textSize + 1; x++)
        {
            var left = Vector3.Lerp(topLeft, bottomLeft, x / textSize);
            var right = Vector3.Lerp(topRight, bottomRight, x / textSize);
            
            for (float y = 0; y < textSize + 1; y++)
            {
                textureVertecies.Add(Vector3.Lerp(right, left, y / textSize));
            }
        }

        int cor = 0;
        for (int y = 0; y < textSize; y++)
        {
            for (int x = 0; x < textSize; x++)
            {
                tilesArray[x, y].BotLeft = textureVertecies[tilesArray[x, y].oneDcoord + y];
                tilesArray[x , y ].BotRight = textureVertecies[tilesArray[x, y].oneDcoord + y +1];
                tilesArray[x , y ].TopLeft = textureVertecies[tilesArray[x, y].oneDcoord + y + 1 + textSize];
                tilesArray[x , y ].TopRight = textureVertecies[tilesArray[x, y].oneDcoord + y +textSize + 2];
                tilesArray[x , y ].midCoord = Vector3.Lerp( tilesArray[x,y].TopLeft, tilesArray[x,y].BotRight,0.5f);
            }
            cor++;
        }

        plane.transform.Rotate(new Vector3(0, 180, 0));    // this is a temp fix but i think the issue is with the for loop above cheking x first other than y

        GenerateResources();

        SetCAgrid();

        GeneralUtil.Ui.SetMessage("Place the council to start the simulation", Color.blue);
        GeneralUtil.Ui.SetMessage("Use the time slider to speed things up", Color.blue);
        GeneralUtil.Ui.SetMessage("Press Space to see the graph of your current progress", Color.blue);
        GeneralUtil.Ui.SetMessage("You can either let the game take its course but you can also take control and place buildings your self", Color.blue);
        GeneralUtil.Ui.SetMessage("Press Esc to pause the game and change the values for the AI behaviour", Color.blue);
        GeneralUtil.Ui.SetMessage("You can click on buildings or NPCs to see their stats, but remember to press right click to deselect them!!", Color.blue);
        GeneralUtil.Ui.SetMessage("Use WASD to move around, scrol wheel to go up or down, press scroll wheel to pan around", Color.blue);
    }

    public void RandomizePerlin()
    {
        offsetX = Random.Range(1, 10000);
        offsetY = Random.Range(1, 10000);

        scale = Random.Range(20, 50);
        lacu = Random.Range(0.1f, 0.8f);

        pers = Random.Range(0.1f, 0.8f);

        octaves = Random.Range(2, 8);

        regions = Random.Range(3, 8);
    }

    // in a way the setting of the weight can be here as we are redrawin the map but should theroatically be put somewhere else
    public void CreateMapTexture()
    {
        Texture2D texture = new Texture2D(textSize, textSize);

        for (int y = 0; y < tilesArray.GetLength(0); y++)
        {
            for (int x = 0; x < tilesArray.GetLength(1); x++)
            {
                switch (tilesArray[x, y].tileType)
                {
                    case TileType.GRASS:
                        texture.SetPixel(x, y, Color.green);
                        break;
                    case TileType.HILL:
                        texture.SetPixel(x, y, GeneralUtil.colorBrown);
                        break;
                    case TileType.SNOW:
                        texture.SetPixel(x, y, Color.white);
                        break;
                    case TileType.WATER:
                        texture.SetPixel(x, y, Color.cyan);
                        break;
                    case TileType.NULL:
                        texture.SetPixel(x, y, Color.red);
                        break;
                    case TileType.BLOCKED:
                        texture.SetPixel(x, y, Color.black);
                        break;
                    case TileType.PATH:
                        texture.SetPixel(x, y, Color.yellow);
                        break;
                    case TileType.ENTRANCE:
                        texture.SetPixel(x, y, Color.gray);
                        break;
                    default:
                        break;
                }
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();

        textMap = texture;

        plane.GetComponent<Renderer>().material.mainTexture = textMap;
    }

    public GameObject SpawnAgent(string guid, Tile exitPoint,bool checkObj = true) 
    {
        if (GeneralUtil.dataBank.npcDict[guid].agentObj != null && checkObj) 
        {
            return null;
        }
        var objRef = Instantiate(agent, transform);
        var comp = objRef.GetComponent<Agent>();

        comp.LoadData(guid);
        comp.SetPosition(exitPoint);

        return objRef;
    }

    public void DrawAllowedTiles() 
    {
        Texture2D texture = new Texture2D(textSize, textSize);

        for (int y = 0; y < tilesArray.GetLength(0); y++)
        {
            for (int x = 0; x < tilesArray.GetLength(1); x++)
            {
                var newVec = new Vector2Int(x, y);

                if (GeneralUtil.dataBank.allowedBuildingLocations.Contains(newVec))
                    texture.SetPixel(x, y, Color.green);
                else
                    texture.SetPixel(x, y, Color.red);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();

        plane.GetComponent<Renderer>().material.mainTexture = texture;
    }


    #region perTurn Gen
    

    public void RespawnSomeTrees(int amountOfTries) 
    {
        GeneralUtil.Ui.SetProgressState(1);
        for (int i = 0; i < amountOfTries; i++)
        {
            var randomCoor = new Vector2Int(Random.Range(0, textSize - 1), Random.Range(0, textSize - 1));

            for (int k = -1; k <= 1; k++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int neighborX = randomCoor.x + k;
                    int neighborY = randomCoor.y + j;

                    if (neighborX < 0 || neighborX >= textSize || neighborY < 0 || neighborY >= textSize)
                    {
                        continue;
                    }

                    if (tilesArray[neighborX,neighborY].tileType == TileType.GRASS   &&    tilesArray[neighborX, neighborY].tileObject != null) 
                    {
                        SpawnTreeType(tilesArray[neighborX, neighborY]);
                        currCAgrid[neighborX, neighborY].alive = true;
                    }
                }
            }
        }
    }

    public void RespawnSomeBushes(int amoutnOfTries) 
    {
        GeneralUtil.Ui.SetProgressState(0);
        for (int i = 0; i < amoutnOfTries; i++)
        {
            var randomCoor = new Vector2Int(Random.Range(0, textSize - 1), Random.Range(0, textSize - 1));

            if (tilesArray[randomCoor.x, randomCoor.y].tileType == TileType.GRASS && tilesArray[randomCoor.x, randomCoor.y].tileObject != null)
            {
                SpawnBushType(tilesArray[randomCoor.x, randomCoor.y]);
                currCAgrid[randomCoor.x, randomCoor.y].alive = false;
                currCAgrid[randomCoor.x, randomCoor.y].interactable = false;
            }
        }
    }

    public void GrowTreesPerTurn() 
    {

        GeneralUtil.Ui.SetProgressState(2);

        var newCaGrid = new CAtile[textSize, textSize]; 

        for (int x = 0; x < currCAgrid.GetLength(0); x++)  //copies the grid and runs the tick
        {
            for (int y = 0; y < currCAgrid.GetLength(1); y++)
            {
                if (tilesArray[x, y].tileType != TileType.GRASS)
                    currCAgrid[x, y] = new CAtile(false, false);


                if (tilesArray[x, y].tileObject != null)
                    tilesArray[x, y].tileObject.GetComponent<Resource>().available = true;


                newCaGrid[x, y] = new CAtile();

                if (!currCAgrid[x, y].interactable)
                    continue;

                currCAgrid[x, y].Tick(tilesArray[x, y]);

                newCaGrid[x, y] = new CAtile(currCAgrid[x, y]);

                
            }
        }


        for (int x = 0; x < newCaGrid.GetLength(0); x++)
        {
            for (int y = 0; y < newCaGrid.GetLength(1); y++)
            {
                if (tilesArray[x, y].tileType != TileType.GRASS)
                    continue;

                int neigboursNum = CountNeighbors(x, y, newCaGrid);

                if (neigboursNum <= 2) //set to die
                {
                    currCAgrid[x, y].sick = true;
                }
                else if (neigboursNum >2  &&  neigboursNum <= 4)  //live
                {
                    currCAgrid[x, y].alive = true;
                    currCAgrid[x, y].sick = false;
                    currCAgrid[x, y].daysLeft = 1;

                    if (tilesArray[x,y].tileObject == null) 
                    {
                        SpawnTreeType(tilesArray[x, y]);
                    }
                
                }
                else if (neigboursNum > 4) //set to die
                {
                    currCAgrid[x, y].sick = true;
                }
            }
        }
    }

    private int CountNeighbors(int x, int y, CAtile[,] copyGrid)
    {
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int neighborX = x + i;
                int neighborY = y + j;

                if (neighborX < 0 || neighborX >= copyGrid.GetLength(0) || neighborY < 0 || neighborY >= copyGrid.GetLength(1))
                {
                    continue;
                }

                if (copyGrid[neighborX, neighborY].alive && copyGrid[neighborX, neighborY].interactable)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void SpawnTreeType(Tile tile) 
    {
        tile.busy = true;
        var objRef = Instantiate(trees.Count == 0 ? trees[0] : trees[Random.Range(0, trees.Count)]);
        objRef.GetComponent<Resource>().tile = tile;

        objRef.transform.parent = resourceHolder.transform;
        objRef.transform.position = new Vector3(tile.midCoord.x, 0f, tile.midCoord.z);

        GeneralUtil.dataBank.numOfResourcesWood++;

        tile.tileObject = objRef;
    }
    
    private void SpawnStoneType(Tile tile) 
    {
        tile.busy = true;
        var objRef = Instantiate(rocks.Count == 0 ? rocks[0] : rocks[Random.Range(0, rocks.Count)]);
        objRef.GetComponent<Resource>().tile = tile;

        objRef.transform.parent = resourceHolder.transform;
        objRef.transform.position = new Vector3(tile.midCoord.x, 0.25f, tile.midCoord.z);

        GeneralUtil.dataBank.numOfResourcesStone++;

        tile.tileObject = objRef;
    }
    
    private void SpawnBushType(Tile tile) 
    {
        tile.busy = true;
        var objRef = Instantiate(bushes.Count == 0 ? bushes[0] : bushes[Random.Range(0, bushes.Count)]);
        objRef.GetComponent<Resource>().tile = tile;

        objRef.transform.parent = resourceHolder.transform;
        objRef.transform.position = new Vector3(tile.midCoord.x, 0.1f, tile.midCoord.z);

        GeneralUtil.dataBank.numOfResourcesFood++;

        objRef.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        tile.tileObject = objRef;
    }

    public IEnumerator CallNewTurnSpawn(int bushAmount, int treeAmount)
    {
        RespawnSomeBushes(bushAmount);
        RespawnSomeTrees(treeAmount);
        GrowTreesPerTurn();

        yield return null;
    }

    #endregion

    #region Initial map generation
    public Tile[,] PerlinNoise2D(float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY)
    {
        if (threasholdHill <= threasholdGrass)
            threasholdHill = threasholdGrass + 0.01f;

        if (threasholdSnow <= threasholdHill)
            threasholdSnow = threasholdHill + 0.01f;


        float[,] noiseMap = new float[textSize, textSize];
        Tile[,] tiles = new Tile[textSize, textSize];


        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxN = float.MinValue;
        float minN = float.MaxValue;
        int index = 0;

        for (int y = 0; y < noiseMap.GetLength(0); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(1); x++)
            {

                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;


                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * freq + offsetX;
                    float sampleY = y / scale * freq + offsetY;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;

                    freq *= lacu;
                }


                if (noiseHeight > maxN) { maxN = noiseHeight; }
                else if (noiseHeight < minN) { minN = noiseHeight; }

                noiseMap[x, y] = Mathf.Abs(noiseHeight);

                tiles[x, y] = new Tile();


                if (noiseMap[x, y] > threasholdSnow)
                    tiles[x, y].tileType = TileType.SNOW;

                else if (noiseMap[x, y] > threasholdHill)
                    tiles[x, y].tileType = TileType.HILL;

                else if (noiseMap[x, y] > threasholdGrass)
                    tiles[x, y].tileType = TileType.GRASS;

                else
                    tiles[x, y].tileType = TileType.WATER;

                tiles[x, y].noiseVal = noiseHeight;
                tiles[x, y].oneDcoord = index;
                tiles[x, y].coord = new Vector2Int(x,y);
                index++;

            }
        }

        return tiles;
    }
    public List<Tile>[] Voronoi() 
    {
        var pointsArr = new List<Vector2>();
        var regionsVor = new List<Tile>[regions];

        for (int i = 0; i < regionsVor.Length; i++)
        {
            regionsVor[i] = new List<Tile>();
        }

        int totalSize = tilesArray.GetLength(0) * tilesArray.GetLength(1);

        for (int i = 0; i < regions; i++)
        {
            int ran = UnityEngine.Random.Range(0, totalSize);

            var wantedCoor = new Vector2(ran / textSize, ran % textSize);

            if (pointsArr.Contains(wantedCoor))
            {
                i--;
            }
            else
            {
                pointsArr.Add(wantedCoor);
            }
        }



        for (int y = 0; y < textSize; y++)
        {
            for (int x = 0; x < textSize; x++)
            {
                int closestIndex = 0;
                float closestDistance = -1;

                for (int i = 0; i < pointsArr.Count; i++)
                {
                    if (closestDistance < 0)
                    {
                        closestDistance = GeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(tilesArray[x,y].midCoord.x, tilesArray[x,y].midCoord.z));
                    }
                    else
                    {
                        float newDist = GeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(tilesArray[x,y].midCoord.x, tilesArray[x,y].midCoord.z));

                        if (closestDistance > newDist)
                        {
                            closestDistance = newDist;
                            closestIndex = i;
                        }
                    }
                }

                regionsVor[closestIndex].Add(tilesArray[x, y]);
            }
        }




        return regionsVor;


    }
    public void GenerateResources() 
    {
        var voronoiOutcome = Voronoi();
        var regionsOfGreen = voronoiOutcome.Length / 3;

        for (int i = 0; i < regionsOfGreen; i++)//this is how many will have the trees
        {
            for (int j = 0; j < voronoiOutcome[i].Count; j++)
            {
                var ran = Random.value;

                if (ran < 0.1f) 
                {
                    if (voronoiOutcome[i][j].tileType == TileType.GRASS) 
                    {
                        SpawnTreeType(voronoiOutcome[i][j]);
                    }
                }
            }
        }
        foreach (var tile in tilesArray)
        {
            if (tile.busy == false) 
            {
                float ran = float.MinValue;

                switch (tile.tileType)
                {
                    case TileType.GRASS:

                        ran = Random.Range(0.000f, 1.000f);

                        if (ran < percOfBushInGrass)
                        {
                            SpawnBushType(tile);
                        }

                        ran = Random.Range(0.000f, 1.000f);
                        if (tile.busy == false) 
                        {
                            if (ran < percOfBushInGrass)
                            {
                                SpawnTreeType(tile);
                            }
                        }

                        if (tile.busy == false) 
                        {
                            if (ran < percOfStoneOnGrass)
                            {
                                SpawnStoneType(tile);
                            }
                        }

                        break;
                    case TileType.HILL:

                        ran = Random.Range(0.000f, 1.000f);

                        if (ran < percOfStoneOnHill)
                        {
                            SpawnStoneType(tile);
                        }
                        break;
                    case TileType.SNOW:

                        break;
                   
                    default:
                        break;
                }
            }
        }
    }

    public void SetCAgrid() 
    {

        for (int x = 0; x < tilesArray.GetLength(0); x++)
        {
            for (int y = 0; y < tilesArray.GetLength(1); y++)
            {
                if (tilesArray[x, y].tileType == TileType.GRASS)
                {
                    if (tilesArray[x, y].tileObject != null)
                    {
                        var comp = tilesArray[x, y].tileObject.GetComponent<Resource>();

                        if (comp.type == Resource.RESOURCE_TYPE.WOOD)
                            currCAgrid[x, y] = new CAtile(true, true);
                        else
                            currCAgrid[x, y] = new CAtile(false, false);
                    }
                    else
                    {
                        currCAgrid[x, y] = new CAtile(false, true);
                    }
                }
                else
                {
                    currCAgrid[x, y] = new CAtile(false, false);
                }
            }
        }
    }

    #endregion

    public class CAtile
    {
        public bool alive = false;
        public bool interactable = false;
        public bool sick = false;
        public int daysLeft = 2;


        public CAtile(bool alive, bool interactable)
        {
            this.alive = alive;
            this.interactable = interactable;
        }

        public CAtile()
        {
        }


        public void Tick(Tile tile)
        {
            if (sick)
            {
                daysLeft--;
            }

            if (daysLeft < 0)
            {
                alive = false;

                if (tile.tileObject != null)
                {
                    Destroy(tile.tileObject);
                    tile.busy = false;
                    GeneralUtil.dataBank.numOfResourcesWood--;
                }
            }
        }


        public CAtile(CAtile copy)
        {
            alive = copy.alive;
            interactable = copy.interactable;
            daysLeft = copy.daysLeft;
            sick = copy.sick;
        }
    }

}

public class Tile
{
    public Color color;
    public TileType tileType;

    public bool busy;
    public GameObject tileObject;

    public float noiseVal;

    public Vector3 BotRight = new Vector3();
    public Vector3 TopLeft = new Vector3();
    public Vector3 TopRight = new Vector3();
    public Vector3 BotLeft = new Vector3();

    public Vector3 midCoord = new Vector3();

    public Vector2Int coord = new Vector2Int();
    public int oneDcoord;
}

public enum TileType
{
    GRASS = 0,
    HILL,
    SNOW,
    WATER,
    NULL,
    BLOCKED,
    PATH,
    ENTRANCE
}
