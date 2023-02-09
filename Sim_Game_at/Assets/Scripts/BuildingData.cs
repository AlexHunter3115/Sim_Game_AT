using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : Entity
{
    public BuildingData(BUILDING_TYPE typeOfBuilding, Vector2Int size, Vector2Int mid)
    {
        this.typeOfBuilding = typeOfBuilding;
        this.buildingSize = size;
        this.centerCoord = mid;

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

    public Vector2Int botLeft;
    public Vector2Int botRight;
    public Vector2Int topLeft;
    public Vector2Int topRight;

    public int effectiveRadius;
    #endregion


    public List<NpcData> workers;
    public int maxWorkers;


    public int wood;
    public int stone;
    public int sand;
    public int food;

    public float buildTime;







    #region overrides
    public override void TickDailyCycle()
    {
        base.TickDailyCycle();
    }
    public override void TickMinuteCycle()
    {
        base.TickMinuteCycle();
    }
    public override void TickHourCycle()
    {
        base.TickHourCycle();
    }
    #endregion


}
