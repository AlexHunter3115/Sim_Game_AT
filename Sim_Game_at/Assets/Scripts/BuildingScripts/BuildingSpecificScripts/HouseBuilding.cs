using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class HouseBuilding : MonoBehaviour, IAgentInteractions,IBuildingActions, ITimeTickers
{
    private BuildingIdentifier buildingId;
    public bool test = false;

    private bool firstInHouseCheck = false;   // this is a check for the first person that comes into the house they should check if this house has enough food
    private bool morningCall = true;

    public List<string> childrenHabitantsGUID = new List<string>();

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
        if (buildingId.buildingData.workers.Count < 2) 
        {
            foreach (var agent in GeneralUtil.dataBank.npcDict.Values)
            {
                //we could have a homelsee list like the jobless
                if (agent.refToHouse == null)
                {
                    agent.refToHouse = buildingId.buildingData;

                    buildingId.buildingData.workers.Add(agent);

                    if (GeneralUtil.timeCycle.isNightTime) 
                    {
                        agent.SetAgentPathing(GeneralUtil.WorldPosToTile(agent.agentObj.transform.position).coord, this.buildingId.buildingData.entrancePoints[0], true);
                    }

                    if (buildingId.buildingData.workers.Count == 2)
                        return;
                }
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

    public bool DeleteBuilding()
    {
        //need to copy over the delete here

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


        MeshRenderer meshRenderer = GeneralUtil.map.plane.GetComponent<MeshRenderer>();
        Material material = meshRenderer.material;
        Texture2D texture = (Texture2D)material.mainTexture;

        foreach (var item in buildingId.buildingData.takenTiles)
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

        GeneralUtil.dataBank.buildingDict.Remove(buildingId.guid);
        Destroy(gameObject);


        return false;
    }

    //should prob subscribe with a interface too we can use the interface alreday there for buidling actions
    //this gets called every mid day so kick the cunt out
    #region timeTickers ITimeTickers
    public void DayTick()
    {
        for (int i = childrenHabitantsGUID.Count; i-- > 0;)   //check through all the children
        {
            var child = GeneralUtil.dataBank.npcDict[childrenHabitantsGUID[i]];   //get the ref

            if (child.currAge == AgentData.AGE_STATE.ADULT)    // is the child now an adult
            {
                GeneralUtil.dataBank.unemployedNpc.Add(child.guid);  // add it to the unemployed list
                child.refToHouse = null; // deleyte the house ref
                child.refToWorkPlace = null; // deleyte the house ref
                Debug.Log($"is this even calling");
                GeneralUtil.map.SpawnAgent(child.guid, GeneralUtil.ReturnTile(buildingId.buildingData.entrancePoints[0]));
                childrenHabitantsGUID.RemoveAt(i);
                child.SetToWonder();
            }
        }
    }
    public void HourTick()
    {
        if (GeneralUtil.timeCycle.isNightTime)
        {
            buildingId.buildingData.shut = false;
            morningCall = false;

            MakeBaby();
        }
        else
        {
            // if its the day then call the morning call

            if (morningCall == false)
            {
                morningCall = true;

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

    public void MinuteTick()
    {
        FindHabitant();
    }

    #endregion


    public void MakeBaby()
    {
        if (buildingId.buildingData.workers.Count == 2 && childrenHabitantsGUID.Count < 4)
        {
            if (buildingId.buildingData.workers[0].atHouse && buildingId.buildingData.workers[1].atHouse)
            {
                float chanceOfFertility = Mathf.Lerp(buildingId.buildingData.workers[0].fertilityPerc, buildingId.buildingData.workers[1].fertilityPerc, 0.5f);

                if (Random.Range(0.000f, 1.000f) <= chanceOfFertility)
                {
                    AgentData newChild = new AgentData(AgentData.AGE_STATE.BABY);

                    GeneralUtil.dataBank.npcDict.Add(newChild.guid, newChild);

                    newChild.SetParents(buildingId.buildingData.workers[0], buildingId.buildingData.workers[1]);
                    newChild.refToHouse = buildingId.buildingData;

                    childrenHabitantsGUID.Add(newChild.guid);

                    for (int i = 0; i < buildingId.buildingData.workers.Count; i++)
                    {
                        buildingId.buildingData.workers[i].AddChild(newChild);
                    }
                }
            }
        }
    }


    //to delete
    private void Update()
    {
        if (test)
        {
            test = false;

            AgentData newChild = new AgentData(AgentData.AGE_STATE.BABY);

            GeneralUtil.dataBank.npcDict.Add(newChild.guid, newChild);

            newChild.SetParents(buildingId.buildingData.workers[0], buildingId.buildingData.workers[1]);
            newChild.refToHouse = buildingId.buildingData;

            childrenHabitantsGUID.Add(newChild.guid);

            for (int i = 0; i < buildingId.buildingData.workers.Count; i++)
            {
                buildingId.buildingData.workers[i].AddChild(newChild);
            }
        }
    }

}
