using Newtonsoft.Json.Linq;
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
    public bool drawResources = false;

    public void init(Tile middleTile, Vector2Int size, List<Tile> controlledTiles, int index)
    {
        var stats = GeneralUtil.buildingScritpable.buildingStats[index];

        buildingIndex = index;
        buildingData = new BuildingData( middleTile.coord,stats);
        this.guid = buildingData.guid;
        buildingData.takenTiles.Clear();
        foreach (var tile in controlledTiles)
        {
            buildingData.takenTiles.Add(tile);
        }

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
        if (buildingIndex == 0)
            return;

        if (!DeleteBuidlingInterfaceCall(buildingActions))
            return;

        MeshRenderer meshRenderer = GeneralUtil.map.plane.GetComponent<MeshRenderer>();
        Material material = meshRenderer.material;
        Texture2D texture = (Texture2D)material.mainTexture;

        foreach (var item in buildingData.takenTiles)
        {
            Color pixelColor = texture.GetPixel(item.coord.x, item.coord.y);

            if (pixelColor == Color.green) 
            {
                item.tileType = TileType.GRASS;
            }
            else if (pixelColor == GeneralUtil.colorBrown) 
            {
                item.tileType = TileType.HILL;
            }
            else if (pixelColor == Color.white) 
            {
                item.tileType = TileType.SNOW;
            }
            else if (pixelColor == Color.cyan)
            {
                item.tileType = TileType.WATER;
            }
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

        GeneralUtil.resourceBank.woodMaxAmount -= buildingData.stats.BankAmountWSFS[0];
        GeneralUtil.resourceBank.stoneMaxAmount -= buildingData.stats.BankAmountWSFS[1];
        GeneralUtil.resourceBank.foodMaxAmount -= buildingData.stats.BankAmountWSFS[2];
        GeneralUtil.resourceBank.sandMaxAmount -= buildingData.stats.BankAmountWSFS[3];

        Destroy(gameObject);
    }

    private bool DeleteBuidlingInterfaceCall(IBuildingActions timeInterface) => timeInterface.DeleteBuilding();

    public void GetResourceNearby() 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("Resources"));

        Array.Sort(hitColliders, new DistanceComparer(transform));

        buildingData.tilesWithResourcesInRange.Clear();

        foreach (var tiles in hitColliders)
        {
            var comp = tiles.GetComponent<Resource>();
            if (!comp.available)
                continue;

            if (buildingData.stats.whatResourceLookingFor.Contains((int)comp.type)) 
            {
                buildingData.tilesWithResourcesInRange.Add(comp.tile);
            }
        }


        if (GeneralUtil.resourceBank.stoneAmount / (float)GeneralUtil.resourceBank.stoneMaxAmount > 0.9f && buildingData.stats.whatResourceLookingFor.Contains(0)) 
        {
            for (int i = buildingData.tilesWithResourcesInRange.Count; i-- > 0;)
            {
                if (buildingData.tilesWithResourcesInRange[i].tileObject != null)
                {
                    var comp = buildingData.tilesWithResourcesInRange[i].tileObject.GetComponent<Resource>();

                    if (!comp.available)
                        continue;

                    if ((int)comp.type == 0)
                    {
                        buildingData.tilesWithResourcesInRange.RemoveAt(i);
                    }
                }
            }
        }

        if (GeneralUtil.resourceBank.foodAmount / (float)GeneralUtil.resourceBank.foodMaxAmount > 0.9f && buildingData.stats.whatResourceLookingFor.Contains(1))
        {
            for (int i = buildingData.tilesWithResourcesInRange.Count; i-- > 0;)
            {
                if (buildingData.tilesWithResourcesInRange[i].tileObject != null) 
                {
                    var comp = buildingData.tilesWithResourcesInRange[i].tileObject.GetComponent<Resource>();

                    if (!comp.available)
                        continue;

                    if ((int)comp.type == 1)
                    {
                        buildingData.tilesWithResourcesInRange.RemoveAt(i);
                    }
                }
            }
        }

        if (GeneralUtil.resourceBank.woodAmount / (float)GeneralUtil.resourceBank.woodMaxAmount > 0.9f && buildingData.stats.whatResourceLookingFor.Contains(2))
        {
            for (int i = buildingData.tilesWithResourcesInRange.Count; i-- > 0;)
            {
                if (buildingData.tilesWithResourcesInRange[i].tileObject != null)
                {
                    var comp = buildingData.tilesWithResourcesInRange[i].tileObject.GetComponent<Resource>();

                    if (!comp.available)
                        continue;

                    if ((int)comp.type == 2)
                    {
                        buildingData.tilesWithResourcesInRange.RemoveAt(i);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawResources) 
        {
            if (buildingData.tilesWithResourcesInRange.Count > 0)
            {
                for (int i = 0; i < buildingData.tilesWithResourcesInRange.Count; i++)
                {
                    Gizmos.DrawSphere(buildingData.tilesWithResourcesInRange[i].midCoord, 0.5f);
                }
            }
        }
    }

}




