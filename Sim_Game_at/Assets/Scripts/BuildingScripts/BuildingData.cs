using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BuildingData : Entity
{
    public BuildingData( Vector2Int mid, BuildingStatistics stats )
    {
        typeOfBuilding = stats.type;
        buildingSize = stats.size;
        centerCoord = mid;
        range = stats.tileRange;
        this.stats = stats;

        maxWorkers = stats.maxWorkers;
    }

    public BuildingStatistics stats;
    public int health = 100;
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

    public float buildTime;

    public List<Tile> tilesInRange = new List<Tile>();
    public List<Tile> tilesWithResourcesInRange = new List<Tile>();

    public void ChangeHealth(int amount) 
    {
        health += amount;

        if (health > 100) 
        {
            health = 100;
            return;
        }

        if (health <= 0)
        {
            GeneralUtil.mapInteraction.SpawnFloatingText("*Crumbles*",Color.red, this.buildingID.transform);
            buildingID.DeleteBuilding();
            return;
        }
    }
}
