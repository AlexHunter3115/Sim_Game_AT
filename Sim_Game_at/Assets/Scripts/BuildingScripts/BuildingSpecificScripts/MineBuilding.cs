using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBuilding : MonoBehaviour, IAgentInteractions, ITimeTickers, IBuildingActions
{
    private BuildingIdentifier buildingId;

    // Start is called before the first frame update
    void Start()
    {
        buildingId = transform.GetComponent<BuildingIdentifier>();
        buildingId.agentActions = this;
        buildingId.buildingActions = this;
        buildingId.buildingTimer = this;
        Init();
    }

    private void Init()
    {
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
                    GeneralUtil.map.SpawnAgent(data.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);
                    data.readyToWork = false;
                    data.atWork = false;
                }
            }
        }
    }




    #region timeTickers ITimeTickers
    public void DayTick()
    {
    }

    public void HourTick()
    {


        if (GeneralUtil.timeCycle.isNightTime)
        {
            buildingId.buildingData.shut = true;
        }
        else
        {
            buildingId.buildingData.shut = false;


            buildingId.GetResourceNearby();

            GeneralUtil.resourceBank.ChangeWoodAmount((int)(buildingId.buildingData.stats.hourlyProductionWSFS[0] * (buildingId.buildingData.workers.Count / buildingId.buildingData.maxWorkers * 1.0f)));
            GeneralUtil.resourceBank.ChangeStoneAmount((int)(buildingId.buildingData.stats.hourlyProductionWSFS[1] * (buildingId.buildingData.workers.Count / buildingId.buildingData.maxWorkers * 1.0f)));
            GeneralUtil.resourceBank.ChangeFoodAmount((int)(buildingId.buildingData.stats.hourlyProductionWSFS[2] * (buildingId.buildingData.workers.Count / buildingId.buildingData.maxWorkers * 1.0f)));
            GeneralUtil.resourceBank.ChangeSandAmount((int)(buildingId.buildingData.stats.hourlyProductionWSFS[3] * (buildingId.buildingData.workers.Count / buildingId.buildingData.maxWorkers * 1.0f)));

            foreach (var worker in buildingId.buildingData.workers) //this gives the workers the job 
            {
                if (worker.atWork)
                {
                    if (worker.readyToWork == true && worker.currAction == AgentData.CURRENT_ACTION.WORKING)
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
    }

    public void MinuteTick()
    {
        LookForWorkers();
    }

    #endregion

    private void LookForWorkers()
    {
        if (buildingId.buildingData.workers.Count < buildingId.buildingData.maxWorkers)
        {
            if (GeneralUtil.dataBank.unemployedNpc.Count > 0)
            {
                for (int i = GeneralUtil.dataBank.unemployedNpc.Count; i-- > 0;)
                {
                    buildingId.AddWorker(GeneralUtil.dataBank.unemployedNpc[i]);
                }
            }
        }
    }


    public bool DeleteBuilding()
    {
        return true;
    }


}
