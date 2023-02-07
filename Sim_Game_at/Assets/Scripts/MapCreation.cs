using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.UIElements;

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

    private GameObject plane;

    

    private Vector3 bottomLeft = new Vector3();
    private Vector3 bottomRight = new Vector3();

    private Vector3 topLeft = new Vector3();
    private Vector3 topRight = new Vector3();

    public List<Vector3> textureVertecies = new List<Vector3>();

    private void Start()
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = transform;
        plane.transform.localScale = new Vector3(scale, scale, scale);

        tilesArray = PerlinNoise2D(scale, octaves, pers, lacu, offsetX, offsetY);

        plane.GetComponent<Renderer>().material.mainTexture = ColorArray(tilesArray);
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
                tilesArray[x, y].BotLeft = textureVertecies[tilesArray[x, y].oneDcoord + cor];
                tilesArray[x , y ].BotRight = textureVertecies[tilesArray[x, y].oneDcoord + cor +1];
                tilesArray[x , y ].TopLeft = textureVertecies[tilesArray[x, y].oneDcoord + cor + 1 + textSize];
                tilesArray[x , y ].TopRight = textureVertecies[tilesArray[x, y].oneDcoord + cor +textSize + 2];
            }
            cor++;
        }
    }

    public Texture2D ColorArray(Tile[,] tileArray)
    {
        Texture2D texture = new Texture2D(textSize, textSize);

        for (int y = 0; y < tileArray.GetLength(0); y++)
        {
            for (int x = 0; x < tileArray.GetLength(1); x++)
            {
                switch (tileArray[x, y].tileType)
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
                    default:
                        break;
                }
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
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



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (showGizmos) 
        {
        }
    }

}



public class Tile
{
    public Color color;
    public TileType tileType;

    public float noiseVal;

    public Vector3 BotRight = new Vector3();
    public Vector3 TopLeft = new Vector3();
    public Vector3 TopRight = new Vector3();
    public Vector3 BotLeft = new Vector3();

    public Vector2 coord = new Vector2();
    public int oneDcoord;
}

public enum TileType
{
    GRASS,
    HILL,
    SNOW,
    WATER,
    NULL,
    BLOCKED
}
