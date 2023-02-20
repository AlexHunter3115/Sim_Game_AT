using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BuildingData : Entity
{
    public BuildingData(BUILDING_TYPE typeOfBuilding, Vector2Int size, Vector2Int mid, int range )
    {
        this.typeOfBuilding = typeOfBuilding;
        buildingSize = size;
        centerCoord = mid;
        this.range = range;

        //LoadCloseResources();

    }


    public void LoadCloseResources() => tileInRange = GeneralUtil.GetResourcesCloseSpiral(centerCoord, range);


    public bool shut;


    public enum BUILDING_TYPE 
    {
        COUNCIL,
        SAWMILL,
        MINE,
        FARM,
        HOUSE,
        DOCK,
        OUTPOST,
        SHOP
    }
    public BUILDING_TYPE typeOfBuilding;

    public enum BUILDING_STATUS 
    {
        BUILDING,
        BUILT,
        ABANDOND,
        BROKEN
    }
    public BUILDING_STATUS buildingStatus;





    #region map pos stuff
    public Vector2Int centerCoord;
    public Vector2Int entrances = Vector2Int.zero;
    public Vector2Int buildingSize = Vector2Int.zero;

    public List<Tile> takenTiles = new List<Tile>();
    public List<Vector2Int> entrancePoints = new List<Vector2Int>();

    private int range;

    public int effectiveRadius;
    #endregion

    public BuildingIdentifier buildingID;
    public List<AgentData> workers = new List<AgentData>();
    public int maxWorkers;


    //upkeep
    public int upKeepWoodCost;
    public int upKeepStoneCost;
    public int upKeepSandCost;
    public int upKeepFoodCost;

    public float buildTime;

    public List<Tile> tileInRange = new List<Tile>();


}
