using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class BuildingIdentifier : MonoBehaviour
{

    public BuildingData buildingData;
    public string guid;
    public IAgentInteractions agentActions;
    public IBuildingActions buildingActions;
    public ITimeTickers buildingTimer;

    [Header("the entrance point is based on the middle point")]
    public int buildingIndex = 0;
    

    public void init(Tile middleTile, Vector2Int size, List<Tile> controlledTiles, int index)
    {

        buildingIndex = index;
        buildingData = new BuildingData( middleTile.coord, GeneralUtil.buildingScritpable.buildingStats[index]);
        this.guid = buildingData.guid;
        buildingData.takenTiles = controlledTiles;
        buildingData.buildingID = this;

        buildingData.maxWorkers = GeneralUtil.buildingScritpable.buildingStats[index].maxWorkers;

        var entranceLocation = new List<Vector2Int>();

        foreach (var entrance in GeneralUtil.buildingScritpable.buildingStats[index].entrances)
        {
            entranceLocation.Add(new Vector2Int(entrance.x + buildingData.centerCoord.x, entrance.y + buildingData.centerCoord.y));
            GeneralUtil.map.tilesArray[entrance.x + buildingData.centerCoord.x, entrance.y + buildingData.centerCoord.y].tileType = TileType.ENTRANCE;

            GeneralUtil.entranceTileDict.Add(GeneralUtil.map.tilesArray[entrance.x + buildingData.centerCoord.x, entrance.y + buildingData.centerCoord.y], this);
        }

        buildingData.entrancePoints = entranceLocation;
    }

    public void LandedOn(AgentData agent)
    {
        agentActions.LandedOnEntrance(agent);
    }


    /// <summary>
    /// Add a worker
    /// </summary>
    /// <param name="guid"></param>
    /// <returns>Returns false if it failed to add a worker</returns>
    public bool AddWorker(string guid) 
    {
        var worker = GeneralUtil.dataBank.npcDict[guid];

        if (buildingData.workers.Contains(worker))
            return false;
        if (buildingData.maxWorkers == buildingData.workers.Count)
            return false;

        buildingData.workers.Add(worker);
        worker.currJob = AgentData.OCCUPATION.FOREGER;

        return true;
    }
    /// <summary>
    /// remove the given worker
    /// </summary>
    /// <param name="guid"></param>
    /// <returns>reutrns false if it fails to remove the worker</returns>
    public bool RemoveWorker(string guid) 
    {
        var worker = GeneralUtil.dataBank.npcDict[guid];

        if (buildingData.workers.Contains(worker)) 
        {
            buildingData.workers.Remove(worker);
            return true;
        }

        worker.currJob = AgentData.OCCUPATION.JOBLESS;
        return false;
    }

    /// <summary>
    /// what this does it moslty restores the map back to what it was for the pathing
    /// </summary>
    public void DeleteBuilding()
    {
        DeleteBuidlingInterfaceCall(buildingActions);

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

    private void DeleteBuidlingInterfaceCall(IBuildingActions timeInterface) => timeInterface.DeleteBuilding();

    public void GetResourceNearby() 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("Resources"));

        Array.Sort(hitColliders, new DistanceComparer(transform));

        //Debug.Log($"{hitColliders.Length}");

        buildingData.tilesWithResourcesInRange.Clear();

        foreach (var tiles in hitColliders)
        {
                var comp = tiles.GetComponent<Resource>();

                bool addThisResource = false;

                if (buildingData.upKeepFoodCost > 0) 
                {
                    if (comp.foodAmount > 0)
                        addThisResource = true;
                }

                if (buildingData.upKeepStoneCost > 0)
                {
                    if (comp.stoneAmount > 0)
                        addThisResource = true;
                }

                if (buildingData.upKeepWoodCost > 0)
                {
                    if (comp.woodAmount > 0)
                        addThisResource = true;
                }

                if (addThisResource)
                    buildingData.tilesWithResourcesInRange.Add(comp.tile);
            
        }
    }
}




