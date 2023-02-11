using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BuildingIdentifier : MonoBehaviour
{
    //this is just used as a comp fetcher for th guid and stuff like that becuase of the different things a builiding can 

    public BuildingData buildingData;
    public string guid;

    [Header("the entrance point is based on the middle point")]
    public List<Vector2Int> entrances = new List<Vector2Int>();  // this also gives the entrance of the building
    public int ActionRange = 25;
    
     


    //private MethodInfo minuteCycle;
    //private MethodInfo HourCycle;
    //private MethodInfo DayCycle;

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
            //GeneralUtil.map.tilesArray[entrance.x + buildingData.centerCoord.x, entrance.y + buildingData.centerCoord.y].tileType = TileType.BLOCKED;
            Debug.Log(entranceLocation[0]);
        }

        //GeneralUtil.map.UpdateMapTexture();




        //to get a comp if needed
        //Component[] components = gameObject.GetComponents<Component>();
        //comp = components[4];


        //Type componentType = comp.GetType();
        //minuteCycle = componentType.GetMethod("MinuteTick");
        //minuteCycle = componentType.GetMethod("HourTick");
        //minuteCycle = componentType.GetMethod("DayTick");









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




