using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BuildingIdentifier : MonoBehaviour
{
    public BuildingData buildingData;
    public string guid;

    public IAgentInteractions agentActions;
    public IBuildingActions buildingActions;
    public ITimeTickers buildingTimer;

    public int buildingIndex = 0;

    public bool test = false;

    public void init(Tile middleTile, Vector2Int size, List<Tile> controlledTiles, int index)
    {
        var stats = GeneralUtil.buildingScritpable.buildingStats[index];

        buildingIndex = index;
        buildingData = new BuildingData( middleTile.coord,stats);
        this.guid = buildingData.guid;
        buildingData.takenTiles = controlledTiles;
        buildingData.buildingID = this;

        var entranceLocation = new List<Vector2Int>();

        foreach (var entrance in stats.entrances)
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

        GeneralUtil.dataBank.unemployedNpc.Remove(worker.guid);
        buildingData.workers.Add(worker);
        worker.refToWorkPlace = buildingData;

        worker.currAction = AgentData.CURRENT_ACTION.WORKING;

        if (worker.agentObj != null)
            worker.SetAgentPathing(GeneralUtil.WorldPosToTile(worker.agentObj.transform.position).coord, worker.refToWorkPlace.entrancePoints[0]);

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

            worker.pathTile.Clear();

            worker.SetToWonder();

            worker.currJob = AgentData.OCCUPATION.JOBLESS;

            worker.atWork = false;
            worker.readyToWork = false;

            worker.refToWorkPlace = null;

            GeneralUtil.dataBank.unemployedNpc.Add(worker.guid);

            return true;
        }
        return false;
    }


    private void Update()
    {
        if (test) 
        {
            test = false;
            Test();
        }
    }

    public void Test()
    { }

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

        texture.filterMode = FilterMode.Point;
        texture.Apply();

        GeneralUtil.map.plane.GetComponent<Renderer>().material.mainTexture = texture;

        for (int i = buildingData.workers.Count; i-- > 0;)
        {
            var worker = buildingData.workers[i];

            if (worker.atWork && worker.readyToWork == false) 
            {
                if (worker.tileDestination != null) 
                {
                    if (worker.tileDestination.tileObject != null)
                        worker.tileDestination.tileObject.GetComponent<Resource>().available = true;
                }
            }

            RemoveWorker(worker.guid);
        }

        GeneralUtil.dataBank.buildingDict.Remove(guid);
        Destroy(gameObject);
    }

    private void DeleteBuidlingInterfaceCall(IBuildingActions timeInterface) => timeInterface.DeleteBuilding();

    public void GetResourceNearby() 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("Resources"));

        Array.Sort(hitColliders, new DistanceComparer(transform));

        buildingData.tilesWithResourcesInRange.Clear();

        foreach (var tiles in hitColliders)
        {
            var comp = tiles.GetComponent<Resource>();

            if (buildingData.stats.whatResourceLookingFor.Contains((int)comp.type)) 
            {
                buildingData.tilesWithResourcesInRange.Add(comp.tile);
            }
        }
    }
}




