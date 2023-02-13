using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BuildingIdentifier : MonoBehaviour
{
   //this script is here to be fetched by others, this is because the actions of each building specifically will be held on another specilised script



    public BuildingData buildingData;
    public string guid;

    [Header("the entrance point is based on the middle point")]
    public List<Vector2Int> entrances = new List<Vector2Int>();  // this also gives the entrance of the building
    public int ActionRange = 25;  
    

    public void init(Tile middleTile, Vector2Int size, List<Tile> controlledTiles)
    {
        buildingData = new BuildingData(BuildingData.BUILDING_TYPE.COUNCIL, size, middleTile.coord,ActionRange);
        this.guid = buildingData.guid;
        buildingData.takenTiles = controlledTiles;
        buildingData.buildingID = this;

        var entranceLocation = new List<Vector2Int>();

        foreach (var entrance in entrances)
        {
            entranceLocation.Add(new Vector2Int(entrance.x + buildingData.centerCoord.x, entrance.y + buildingData.centerCoord.y));
            GeneralUtil.map.tilesArray[entrance.x + buildingData.centerCoord.x, entrance.y + buildingData.centerCoord.y].tileType = TileType.PATH;
        }

        buildingData.entrancePoints = entranceLocation;

    }
    




    public void MinuteCycle() 
    {
        
    }
    public void HourCycle() { }
    public void DayCycle() { }




    public void DeleteBuilding()
    {
        MeshRenderer meshRenderer = GeneralUtil.map.plane.GetComponent<MeshRenderer>();
        Material material = meshRenderer.material;
        Texture2D texture = (Texture2D)material.mainTexture;

        foreach (var item in buildingData.takenTiles)
        {
            Color pixelColor = texture.GetPixel(item.coord.x, item.coord.y);

            if (pixelColor == Color.green)
                item.tileType = TileType.GRASS;
            else if (pixelColor == new Color(165.0f / 255, 42.0f / 255, 42.0f / 255, 1))
                item.tileType = TileType.HILL;
            else if (pixelColor == Color.white)
                item.tileType = TileType.SNOW;

        }

        GeneralUtil.dataBank.buildingDict.Remove(guid);

        Destroy(gameObject);
    }
}




