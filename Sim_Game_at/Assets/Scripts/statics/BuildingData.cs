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

        LookForResroucesTilesInRange();

    }





    public void LookForResroucesTilesInRange()
    {

        int halfWidth = range / 2;
        int halfHeight = range / 2;


        for (int y = centerCoord.y - (range - halfHeight); y < centerCoord.y + halfHeight; y++)
        {
            for (int x = centerCoord.x - (range - halfWidth); x < centerCoord.x + halfWidth; x++)
            {
                if (x < 0 || y < 0 || x >= GeneralUtil.map.textSize || y >= GeneralUtil.map.textSize)
                {
                    // out of range
                }
                else
                {
                    bool accept = true;

                    if (GeneralUtil.map.tilesArray[x, y].tileObject != null) // if there is an object in here
                    {
                        // if the object has the resource of this building
                        var resource = GeneralUtil.map.tilesArray[x, y].tileObject.GetComponent<Resource>();

                        if (upKeepWoodCost > 0)
                            if (resource.woodAmount <= 0)
                                accept = false;

                        if (upKeepStoneCost > 0)
                            if (resource.stoneAmount <= 0)
                                accept = false;

                        if (upKeepFoodCost > 0)
                            if (resource.foodAmount <= 0)
                                accept = false;

                        if (accept == false)
                            break;


                        //need to think about this water pathing shit

                        tileInRange.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }





    public enum BUILDING_TYPE 
    {
        COUNCIL,
        SAWMILL,
        MINE,
        FARM,
        HOUSE,
        DOCK
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
    public List<NpcData> workers = new List<NpcData>();
    public int maxWorkers;


    //upkeep
    public int upKeepWoodCost;
    public int upKeepStoneCost;
    public int upKeepSandCost;
    public int upKeepFoodCost;

    public float buildTime;

    public List<Vector2Int> tileInRange = new List<Vector2Int>();





    #region overrides
    public override void TickDailyCycle()
    {
        base.TickDailyCycle();


        buildingID.DayCycle();
    }
    public override void TickMinuteCycle()
    {
        base.TickMinuteCycle();
        buildingID.MinuteCycle();
    }
    public override void TickHourCycle()
    {
        base.TickHourCycle();

        buildingID.HourCycle();
    }
    #endregion


}
