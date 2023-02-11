using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : Entity
{
    public BuildingData(BUILDING_TYPE typeOfBuilding, Vector2Int size, Vector2Int mid, int range )
    {
        this.typeOfBuilding = typeOfBuilding;
        buildingSize = size;
        centerCoord = mid;

        int halfWidth = range / 2;
        int halfHeight = range / 2;


        for (int y = mid.y - (range - halfHeight); y < mid.y + halfHeight; y++)
        {
            for (int x = mid.x - (range - halfWidth); x < mid.x + halfWidth; x++)
            {
                if (x < 0 || y < 0 || x >= GeneralUtil.map.textSize || y >= GeneralUtil.map.textSize)
                {
                    // out of range
                }
                else 
                {
                    if (GeneralUtil.map.tilesArray[x,y].tileObject != null) 
                    {
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

    public Vector2Int botLeft;
    public Vector2Int botRight;
    public Vector2Int topLeft;
    public Vector2Int topRight;

    public int effectiveRadius;
    #endregion

    public BuildingIdentifier buildingID;
    public List<NpcData> workers = new List<NpcData>();
    public int maxWorkers;

    public int wood;
    public int stone;
    public int sand;
    public int food;

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
