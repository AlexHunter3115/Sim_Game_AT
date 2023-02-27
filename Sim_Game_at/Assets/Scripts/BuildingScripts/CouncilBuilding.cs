using UnityEngine;

public class CouncilBuilding : MonoBehaviour, IAgentInteractions, ITimeTickers, IBuildingActions
{
    private BuildingIdentifier buildingId;
    public bool test = false;
    public bool beforeTest = false;



    private void Start()
    {
        buildingId = transform.GetComponent<BuildingIdentifier>();
        buildingId.agentActions = this;
        buildingId.buildingActions = this;
        buildingId.buildingTimer = this;
        InitiateCouncil();
    }


    //the council is the starting point its where the player gets its first 4 people
    public void InitiateCouncil()
    {
        for (int i = 0; i < 4; i++)
        {
            //get the position to spawn the agents
            var destination = new Vector2Int(buildingId.buildingData.centerCoord.x + Random.Range(-5, 5), buildingId.buildingData.centerCoord.y + Random.Range(-5, 5));   //chooses the random pos to spawn on
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
            newCitizen.refToWorkPlace = buildingId.buildingData;    //sets the workplace to the council
            buildingId.AddWorker(newCitizen.guid); //set the place to work in

            GeneralUtil.map.SpawnAgent(newCitizen.guid, GeneralUtil.map.tilesArray[destination.x, destination.y]);   //spawns the agent



            newCitizen.SetAgentPathing(destination, buildingId.buildingData.entrancePoints[0], true);  //sets the pathing to the workplace at the start

            // newCitizen.moving = true;
        }

        buildingId.buildingData.LoadCloseResources();

    }



    private void Update()
    {
        if (test)
        {
            test = false;
            HourTick();
        }
    }

    public void LandedOnEntrance(AgentData data)
    {
        Destroy(data.agentObj);
        data.readyToWork = true;
        data.atWork = true;
    }




    public void HourTick()
    {
        buildingId.GetResourceNearby();

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

            foreach (var worker in buildingId.buildingData.workers) //this gives the workers the job 
            {
                if (worker.atWork)
                {
                    if (worker.readyToWork == true && worker.currAction == AgentData.CURRENT_ACTION.WORKING)
                    {
                        for (int i = 0; i < buildingId.buildingData.tilesWithResourcesInRange.Count; i++)
                        {
                           // Debug.Log(buildingId.buildingData.tilesWithResourcesInRange[i].coord);
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
    }

    public void DayTick()
    {
    }

    public void MinuteTick()
    {

    }

    public void DeleteBuilding()
    {
        buildingId.DeleteBuilding();
        Debug.Log($"Deliting this buidling");
        // this is where the deletion of the workers should go
        for (int i = 0; i < buildingId.buildingData.workers.Count; i++)
        {
            buildingId.buildingData.workers[i].refToWorkPlace = null;

            if (buildingId.buildingData.workers[i].atWork) 
            {
                GeneralUtil.map.SpawnAgent(buildingId.buildingData.workers[i].guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[i].x, buildingId.buildingData.entrancePoints[0].y]);
            }
        }


    }
}
