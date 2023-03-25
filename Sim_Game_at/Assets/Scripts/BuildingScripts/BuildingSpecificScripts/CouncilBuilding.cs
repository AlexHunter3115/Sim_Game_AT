using UnityEngine;
using UnityEngine.Events;

public class CouncilBuilding : MonoBehaviour, IAgentInteractions, ITimeTickers, IBuildingActions
{
    private BuildingIdentifier buildingId;


    private void Start()
    {
        buildingId = transform.GetComponent<BuildingIdentifier>();
        buildingId.agentActions = this;
        buildingId.buildingActions = this;
        buildingId.buildingTimer = this;
        InitiateCouncil();
        //stuff.AddListener(GeneralUtil.timeCycle.DayChangeStateEvent);
    }


    //the council is the starting point its where the player gets its first 4 people
    public void InitiateCouncil()
    {
        GeneralUtil.mapInteraction.spawnedCouncil = true;

        for (int i = 0; i < 4; i++)
        {
            //get the position to spawn the agents
            var destination = new Vector2Int(buildingId.buildingData.centerCoord.x + Random.Range(-5, 5), buildingId.buildingData.centerCoord.y + Random.Range(-5, 5));   //chooses the random pos to spawn on
            // that is not the entrance this is to add, this will lead to issues

            if (destination.x < 0 || destination.y < 0 || destination.x >= GeneralUtil.map.tilesArray.GetLength(0) || destination.y >= GeneralUtil.map.tilesArray.GetLength(1))
            {
                destination = new Vector2Int(buildingId.buildingData.entrancePoints[0].x + 3, buildingId.buildingData.entrancePoints[0].y + 1);
            }
            else if (GeneralUtil.map.tilesArray[destination.x, destination.y].tileType == TileType.WATER)
            {
                destination = new Vector2Int(buildingId.buildingData.entrancePoints[0].x + 3, buildingId.buildingData.entrancePoints[0].y + 1);
            }

            var newCitizen = new AgentData(AgentData.AGE_STATE.ADULT);     // initiates the class
            GeneralUtil.dataBank.npcDict.Add(newCitizen.guid, newCitizen);   // adds it to the dict
                                                                             //sets the workplace to the council
                                                                             // buildingId.AddWorker(newCitizen.guid); //set the place to work in

            buildingId.buildingData.workers.Add(newCitizen);

            newCitizen.refToWorkPlace = buildingId.buildingData;

            var agent = GeneralUtil.map.SpawnAgent(newCitizen.guid, GeneralUtil.map.tilesArray[destination.x, destination.y]);   //spawns the agent

            if (GeneralUtil.WorldPosToTile(agent.transform.position).coord == buildingId.buildingData.entrancePoints[0])
            {
                destination = new Vector2Int(buildingId.buildingData.entrancePoints[0].x + 1, buildingId.buildingData.entrancePoints[0].y + 1);
                agent.GetComponent<Agent>().SetPosition(GeneralUtil.Vector2Tile(destination));
            }

            newCitizen.SetAgentPathing(destination, buildingId.buildingData.entrancePoints[0], true);  //sets the pathing to the workplace at the start

            GeneralUtil.resourceBank.ChangePeopleAmount(1);
        }


        GeneralUtil.dataBank.RecalcAllAllowedTiles();
        GeneralUtil.resourceBank.AddStartingAmount();
    }


    public void LandedOnEntrance(AgentData data)
    {
        Destroy(data.agentObj);
        data.readyToWork = true;
        data.atWork = true;

        if (GeneralUtil.timeCycle.isNightTime)
        {
            if (data.refToHouse != null)
            {
                if (data.readyToWork == true && data.currAction == AgentData.CURRENT_ACTION.WORKING)
                {
                    data.SetAgentPathing(buildingId.buildingData.entrancePoints[0], data.refToHouse.entrancePoints[0], true);
                    GeneralUtil.map.SpawnAgent(data.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y], false);
                    data.readyToWork = false;
                    data.atWork = false;
                }
            }
        }
    }


    public UnityEvent stuff;

    //public void CheckingEvent() 
    //{
    //    Debug.Log("this is calle dint he council building ");
    //}


    #region timeTickers ITimeTickers

    public void HourTick()
    {

        if (GeneralUtil.timeCycle.isNightTime)
        {
            buildingId.buildingData.shut = true;

            foreach (var worker in buildingId.buildingData.workers) //this gives the workers the job 
            {
                if (worker.refToHouse != null)
                {
                    if (worker.readyToWork == true && worker.currAction == AgentData.CURRENT_ACTION.WORKING)
                    {
                        worker.SetAgentPathing(buildingId.buildingData.entrancePoints[0], worker.refToHouse.entrancePoints[0], true);
                        GeneralUtil.map.SpawnAgent(worker.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);
                        worker.readyToWork = false;
                        worker.atWork = false;
                    }
                }
            }
        }
        else
        {
            buildingId.buildingData.shut = false;
            
            GeneralUtil.resourceBank.ChangeWoodAmount((int)(buildingId.buildingData.stats.hourlyProductionWSFS[0] * (buildingId.buildingData.workers.Count / buildingId.buildingData.maxWorkers * 1.0f)));
            GeneralUtil.resourceBank.ChangeStoneAmount((int)(buildingId.buildingData.stats.hourlyProductionWSFS[1] * (buildingId.buildingData.workers.Count / buildingId.buildingData.maxWorkers * 1.0f)));
            GeneralUtil.resourceBank.ChangeFoodAmount((int)(buildingId.buildingData.stats.hourlyProductionWSFS[2] * (buildingId.buildingData.workers.Count / buildingId.buildingData.maxWorkers * 1.0f)));
            GeneralUtil.resourceBank.ChangeSandAmount((int)(buildingId.buildingData.stats.hourlyProductionWSFS[3] * (buildingId.buildingData.workers.Count / buildingId.buildingData.maxWorkers * 1.0f)));

            buildingId.GetResourceNearby();

            foreach (var worker in buildingId.buildingData.workers) //this gives the workers the job 
            {
                if (worker.readyToWork == true && worker.currAction == AgentData.CURRENT_ACTION.WORKING && worker.atWork)
                {
                    for (int i = 0; i < buildingId.buildingData.tilesWithResourcesInRange.Count; i++)
                    {

                        if (buildingId.buildingData.tilesWithResourcesInRange[i].tileObject == null)
                            continue;

                        if (buildingId.buildingData.tilesWithResourcesInRange[i].tileObject.GetComponent<Resource>().available)
                        {
                            buildingId.buildingData.tilesWithResourcesInRange[i].tileObject.GetComponent<Resource>().available = false;
                            if (worker.SetAgentPathing(buildingId.buildingData.entrancePoints[0], buildingId.buildingData.tilesWithResourcesInRange[i].coord))
                            {
                                GeneralUtil.map.SpawnAgent(worker.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);
                                worker.readyToWork = false;
                            }
                            buildingId.buildingData.tilesWithResourcesInRange.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void DayTick()
    {
    }

    public void MinuteTick()
    {
    }

    #endregion





    public bool DeleteBuilding()
    {
        return false;
    }
}
