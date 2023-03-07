using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static TimeCycle;
using static UnityEditor.PlayerSettings;

public class AgentData : Entity, ITimeTickers
{

    #region work and life stuff
    public enum OCCUPATION 
    {
        JOBLESS = 0,
        FOREGER,
        WOOD_GUY,
        MINER,
        FISHER,
        FOODGUY
    }
    public OCCUPATION currJob;

    public Dictionary<OCCUPATION, int> levelOccupation = new Dictionary<OCCUPATION, int>();

    //this can be just null if no hosue or work
    public BuildingData refToHouse;
    public bool atHouse = false;

    public BuildingData refToWorkPlace;
    public bool atWork =false;
    public bool readyToWork = false;
    #endregion


    #region stats

    public string name;
    public float health;
    public float hunger;
    public float stamina;
    public float speed;
    public float fertilityPerc;

    public enum AGE_STATE 
    {
        BABY=4,
        ADULT = 8,
        ELDER = 34
    }
    public AGE_STATE currAge;
    public int daysAlive;

    public enum CURRENT_ACTION
    {
        WORKING,   // working
        SLEEPING,   // in house or outside just not avaialable  this at night
        WONDERING,  // no job no thing useless citizen not at night 
        HOMELESS
    }
    public CURRENT_ACTION currAction;
    public CURRENT_ACTION hardSetAction;

    public int inventoryWood;
    public int inventoryStone;
    public int inventoryFood;

    #endregion


    #region pathing

    public Tile tileDestination;
    public Tile tileStart;
    public List<Tile> pathTile = new List<Tile>();
    public List<Tile> allCheckedDebug = new List<Tile>();

    #endregion

    #region relationships
    public AgentData[] parentsArr = new AgentData[2];
    public List<AgentData> children = new List<AgentData>();

    public void SetParents(AgentData parentOne, AgentData parentTwo)
    {
        parentsArr[0] = parentOne;
        parentsArr[1] = parentTwo;
    }

    public void AddChild(AgentData child) => this.children.Add(child);

    #endregion


    public GameObject agentObj;
    public bool dead;


    //constructor
    public AgentData(AGE_STATE age)  
    {
        currAge = age;

        switch (currAge)
        {
            case AGE_STATE.BABY:
                daysAlive = 0;
                break;

            case AGE_STATE.ADULT:
                daysAlive = 9;
                break;

            case AGE_STATE.ELDER:
                break;
            default:
                break;
        }

        if (0.5f > Random.value) 
        {
            name = GeneralUtil.femaleNames[Random.Range(0, 49)];
        }
        else
        {
            name = GeneralUtil.maleNames[Random.Range(0, 49)];
        }

        fertilityPerc = Random.Range(0.01f, 0.1f);

        GeneralUtil.timeCycle.OnFunctionCalled.AddListener(EndOfDayCall);
    }




    /// <summary>
    /// this is used to set the age of the npc, similar to the time of day thing, should be called every new day
    /// </summary>
    /// <returns></returns>
    public bool AgeUp()
    {

        daysAlive++;

        if ((int)AGE_STATE.BABY <= daysAlive)
        {
            currAge = AGE_STATE.BABY;
        }

        if ((int)AGE_STATE.BABY < daysAlive && (int)AGE_STATE.ADULT >= daysAlive)
        {
            currAge = AGE_STATE.ADULT;
        }

        if ((int)AGE_STATE.ADULT < daysAlive && (int)AGE_STATE.ELDER >= daysAlive)
        {
            currAge = AGE_STATE.ELDER;
        } 

        if ((int)AGE_STATE.ELDER < daysAlive)
        {
            //dead
            Kill();
        }


        //on death need to delete it self from everywhere
        return true;
    }

    public void Kill()
    {
        if (refToWorkPlace != null)
        {
            refToWorkPlace.buildingID.RemoveWorker(guid);
        }

        if (refToHouse != null)
        {
            refToWorkPlace.buildingID.RemoveWorker(guid);
        }

        GeneralUtil.dataBank.npcDict.Remove(guid);
    }



    #region Behaviour stuff

    /// <summary>
    /// given the start and end of the pathing sets all the vars
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="forced"></param>
    public bool SetAgentPathing(Vector2Int start,  Vector2Int end, bool forced = false) 
    {
        var pathData = GeneralUtil.A_StarPathfinding(start, end, this, forced);

        pathTile = pathData.Item1;
        allCheckedDebug = pathData.Item2;

        if (pathTile == null)
            return false;

        tileStart = GeneralUtil.map.tilesArray[start.x, start.y];
        tileDestination = GeneralUtil.map.tilesArray[end.x, end.y];


        return true;
    }

    public void SetToWonder() 
    {
        currAction = CURRENT_ACTION.WONDERING;

        if (agentObj == null)
        {
            GeneralUtil.map.SpawnAgent(this.guid, GeneralUtil.map.tilesArray[refToWorkPlace.entrancePoints[0].x, refToWorkPlace.entrancePoints[0].y]);
        }

        agentObj.GetComponent<Agent>().StopAllCoroutines();

        SetAgentPathing(GeneralUtil.WorldPosToTile(agentObj.transform.position).coord, GeneralUtil.RandomTileAround(3, GeneralUtil.WorldPosToTile(agentObj.transform.position).coord, new List<int> { 0, 1 }).coord,true);
    }

    public void SetToSleeping() 
    {
        currAction = CURRENT_ACTION.SLEEPING;

        if (agentObj == null)
        {
            if (refToHouse != null)
                GeneralUtil.map.SpawnAgent(this.guid, GeneralUtil.map.tilesArray[ refToHouse.entrancePoints[0].x, refToHouse.entrancePoints[0].y]);
            else
                GeneralUtil.map.SpawnAgent(this.guid, GeneralUtil.map.tilesArray[refToWorkPlace.entrancePoints[0].x, refToWorkPlace.entrancePoints[0].y]);
        }

        agentObj.GetComponent<Agent>().StopAllCoroutines();

        SetAgentPathing(GeneralUtil.WorldPosToTile(agentObj.transform.position).coord, GeneralUtil.RandomTileAround(3, GeneralUtil.WorldPosToTile(agentObj.transform.position).coord, new List<int> { 0, 1 }).coord, true);
    }

    #endregion

    #region TimeStuff

    //this gets called at noon because the gaem starts at noon and then its a day from there
    public void DayTick()
    {
       AgeUp();
    }

    public void MinuteTick()
    { }

    public void HourTick()
    { }

    private void EndOfDayCall() 
    {
        if (currAge != AGE_STATE.BABY)
        {
            if (GeneralUtil.timeCycle.isNightTime) // if its night time 
            {
                if (refToWorkPlace == null)
                {
                    if (refToHouse != null) // and has no house then this llogic takes over
                    {
                        this.SetAgentPathing(GeneralUtil.WorldPosToTile(agentObj.transform.position).coord, this.refToHouse.entrancePoints[0], true);
                        this.readyToWork = false;
                        this.atWork = false;
                        currAction = CURRENT_ACTION.SLEEPING;
                    }
                }
                else 
                {
                    if (refToHouse == null) // and has no house then this llogic takes over
                    {
                        SetToSleeping();
                    }
                }
                
            }
            else // if its day time and no job this takes over
            {
                if (refToWorkPlace == null)   // if it doesnt have a job
                {
                    if (refToHouse != null)
                        SetToWonder();
                }
                else  //if it does have a job
                {
                    if (refToHouse == null) // and has no house then this llogic takes over
                    {
                        this.SetAgentPathing(GeneralUtil.WorldPosToTile(agentObj.transform.position).coord, this.refToWorkPlace.entrancePoints[0], true);
                        this.readyToWork = false;
                        this.atWork = false;
                        currAction = CURRENT_ACTION.WORKING;
                    }
                }
            }
        }
        
    }

    #endregion

    //the npc here should have an inventory
}
