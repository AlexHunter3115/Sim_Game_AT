using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapCreation : MonoBehaviour
{


    [Range(1, 1000)]
    public int offsetX = 100;
    [Range(1, 1000)]
    public int offsetY = 100;

    public int textSize = 256;

    public int scale = 10;


    public float threasholdGrass = 0.2f;
    public float threasholdHill = 0.65f;
    public float threasholdSnow = 0.89f;

    public Tile[,] tilesArray = new Tile[0, 0];


    private void Start()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = transform;
        plane.transform.localScale = new Vector3(scale, scale, scale);

        tilesArray = PerlinNoise2D(18, 3, 0.5f, 2, offsetX, offsetY);



        plane.GetComponent<Renderer>().material.mainTexture = ColorArray(tilesArray);

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
                        texture.SetPixel(x, y, new Color(165, 42, 42));
                        break;
                    case TileType.SNOW:
                        texture.SetPixel(x, y, Color.cyan);
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

                noiseMap[x, y] = noiseHeight;



                tiles[x, y] = new Tile();


                if (noiseHeight > threasholdSnow)
                    tiles[x, y].tileType = TileType.SNOW;

                else if (noiseHeight > threasholdHill)
                    tiles[x, y].tileType = TileType.HILL;

                else if (noiseHeight > threasholdGrass)
                    tiles[x, y].tileType = TileType.GRASS;

                else
                    tiles[x, y].tileType = TileType.WATER;

                tiles[x, y].noiseVal = noiseHeight;
                Debug.Log(tiles[x, y].tileType);
                tiles[x, y].coord = new Vector2Int(x,y);

            }
        }


        return tiles;
    }

}



public class Tile
{
    public Color color;
    public TileType tileType;

    public float noiseVal;
    public Vector2Int coord = new Vector2Int(0, 0);
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
