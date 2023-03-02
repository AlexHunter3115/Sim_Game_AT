using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilding : MonoBehaviour, IAgentInteractions,IBuildingActions, ITimeTickers
{
    private BuildingIdentifier buildingId;
    public bool test = false;


    private bool firstInHouseCheck = false;   // this is a check for the first person that comes into the house they should check if this house has enough food
    private bool morningCall = true;


    private void Start()
    {
        buildingId = transform.GetComponent<BuildingIdentifier>();
        Init();
    }

    public void Init() 
    {
        buildingId.agentActions = this;
        buildingId.buildingActions = this;
        buildingId.buildingTimer = this;
        FindHabitant();
    }


    public void FindHabitant() 
    {
        foreach (var agent in GeneralUtil.dataBank.npcDict.Values)
        {
            if (agent.refToHouse == null) 
            {
                agent.refToHouse = buildingId.buildingData;

                buildingId.buildingData.workers.Add(agent);

                if (buildingId.buildingData.workers.Count == 2)
                    return;
            }
        }
    }

    

    public void LandedOnEntrance(AgentData data)
    {
        Destroy(data.agentObj);
        
        if (firstInHouseCheck == false) 
        {//the first person that landed here is checking the food
            // does this place have enough food? 
            // no then set its pathign to go to the shop 
            firstInHouseCheck = true;
        }

        data.atHouse = true;
    }

    public void DeleteBuilding()
    {
        buildingId.DeleteBuilding();

        for (int i = buildingId.buildingData.workers.Count; i-- > 0;)
        {
            if (GeneralUtil.timeCycle.isNightTime && buildingId.buildingData.workers[i].atHouse) 
            {
                GeneralUtil.map.SpawnAgent(buildingId.buildingData.workers[i].guid, GeneralUtil.map.tilesArray[   buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);
            }
            //if at night spawn it in?

            buildingId.buildingData.workers[i].refToHouse = null;
            buildingId.buildingData.workers[i].atHouse = false;

            buildingId.buildingData.workers.RemoveAt(i);
        }
    }


    //should prob subscribe with a interface too we can use the interface alreday there for buidling actions
    public void HourTick()
    {
        if (GeneralUtil.timeCycle.isNightTime)
        { 
            buildingId.buildingData.shut = false;
            morningCall = false;
        }
        else 
        {
            // if its the day then call the morning call

            if (morningCall == false) 
            {
                morningCall= true;

                foreach (var habitant in buildingId.buildingData.workers)
                {
                    if (habitant.refToWorkPlace == null) 
                    {
                        GeneralUtil.map.SpawnAgent(habitant.guid, GeneralUtil.Vector2Tile(habitant.refToHouse.entrancePoints[0]));
                        habitant.SetToWonder();
                        // this is where they spawn and do nothing
                    }
                    else 
                    {
                        if (habitant.agentObj == null) 
                        {
                            GeneralUtil.map.SpawnAgent(habitant.guid, GeneralUtil.Vector2Tile(habitant.refToHouse.entrancePoints[0]));
                        }

                        habitant.SetAgentPathing(habitant.refToHouse.entrancePoints[0], habitant.refToWorkPlace.entrancePoints[0], true);
                    }
                }
            }

            buildingId.buildingData.shut = true;
            firstInHouseCheck = false;
        }
    }








    public void DayTick()
    {
    }

    public void MinuteTick()
    {
    }
}
