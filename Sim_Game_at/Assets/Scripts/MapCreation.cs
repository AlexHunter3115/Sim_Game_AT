using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class MapCreation : MonoBehaviour
{
    public int textSize = 512;

    [Space(30)]
    [Range(1, 1000)]
    public int offsetX = 100;
    [Range(1, 1000)]
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
    public Tile ClickedTile = null;



    private void Awake()
    {
        GeneralUtil.map = this;
        GeneralUtil.buildingScritpable = Resources.Load("buildingTypes") as BuildingTypes;
    }

    private void Start()
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = transform;
        plane.transform.localScale = new Vector3(scale, scale, scale);
        plane.gameObject.layer = 6;

        tilesArray = PerlinNoise2D(scale, octaves, pers, lacu, offsetX, offsetY);
        
        UpdateMapTexture();
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
        //thedistance between the two   0.78125

        //Debug.Log(  Vector3.Distance  (tilesArray[0,0].BotLeft, tilesArray[0, 0].BotRight));


    }

    // in a way the setting of the weight can be here as we are redrawin the map but should theroatically be put somewhere else
    public void UpdateMapTexture()
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
                        texture.SetPixel(x, y, new Color(165.0f/255, 42.0f / 255, 42.0f / 255,1));
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

        plane.GetComponent<Renderer>().material.mainTexture =  texture;
    }

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
                        voronoiOutcome[i][j].busy = true;
                        var objRef = Instantiate(trees.Count == 0 ? trees[0] : trees[Random.Range(0, trees.Count)]);
                        objRef.GetComponent<Resource>().tile = voronoiOutcome[i][j];

                        objRef.transform.parent = resourceHolder.transform;
                        objRef.transform.position = new Vector3(voronoiOutcome[i][j].midCoord.x, 0, voronoiOutcome[i][j].midCoord.z);

                        voronoiOutcome[i][j].tileObject = objRef;
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
                            tile.busy = true;
                            var objRef = Instantiate(bushes.Count == 0 ? bushes[0] : bushes[Random.Range(0, bushes.Count)]);
                            objRef.GetComponent<Resource>().tile = tile;

                            objRef.transform.parent = resourceHolder.transform;
                            objRef.transform.position = new Vector3(tile.midCoord.x, 0.1f, tile.midCoord.z);

                            objRef.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                            tile.tileObject = objRef;
                        }

                        ran = Random.Range(0.000f, 1.000f);
                        if (tile.busy == false) 
                        {
                            if (ran < percOfBushInGrass)
                            {
                                tile.busy = true;
                                var objRef = Instantiate(trees.Count == 0 ? trees[0] : trees[Random.Range(0, trees.Count)]);
                                objRef.GetComponent<Resource>().tile = tile;

                                objRef.transform.parent = resourceHolder.transform;
                                objRef.transform.position = new Vector3(tile.midCoord.x, 0f, tile.midCoord.z);

                                tile.tileObject = objRef;
                            }
                        }

                        if (tile.busy == false) 
                        {
                            if (ran < percOfStoneOnGrass)
                            {
                                tile.busy = true;
                                var objRef = Instantiate(rocks.Count == 0 ? rocks[0] : rocks[Random.Range(0, rocks.Count)]);
                                objRef.GetComponent<Resource>().tile = tile;

                                objRef.transform.parent = resourceHolder.transform;
                                objRef.transform.position = new Vector3(tile.midCoord.x, 0.25f, tile.midCoord.z);
                                tile.tileObject = objRef;

                            }
                        }



                       

                        break;
                    case TileType.HILL:


                        ran = Random.Range(0.000f, 1.000f);

                        if (ran < percOfStoneOnHill)
                        {
                            tile.busy = true;
                            var objRef = Instantiate(rocks.Count == 0 ? rocks[0] : rocks[Random.Range(0, rocks.Count)]);
                            objRef.transform.parent = resourceHolder.transform;
                            objRef.transform.position = new Vector3(tile.midCoord.x, 0.25f, tile.midCoord.z);

                            tile.tileObject = objRef;
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

    public void SpawnAgent(string guid, Tile exitPoint) 
    {
        var objRef = Instantiate(agent, transform);
        var comp = objRef.GetComponent<Agent>();

        comp.LoadData(guid);
        comp.SetPosition(exitPoint);
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (showGizmos) 
        {

            if (ClickedTile != null) 
            {
                Gizmos.DrawSphere(ClickedTile.BotLeft, 0.5f);
                Gizmos.DrawSphere(ClickedTile.TopLeft, 0.5f);
                Gizmos.DrawSphere(ClickedTile.TopRight, 0.5f);
                Gizmos.DrawSphere(ClickedTile.BotRight, 0.5f);
            }

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
