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
    public int gender;
    public float speed;

    public enum AGE_STATE 
    {
        BABY=4,
        TEEN = 12,
        ADULT = 18,
        ELDER = 30
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

    #endregion


    #region relationships
    public AgentData spouse;
    public AgentData[] parentsArr = new AgentData[2];
    public List<AgentData> children;

    public void SetParents(AgentData parentOne, AgentData parentTwo)
    {
        parentsArr[0] = parentOne;
        parentsArr[0] = parentTwo;
    }
    public void AddChild(AgentData child) => this.children.Add(child);
    #endregion


    public GameObject agentObj;
    public bool dead;

    private bool lastTime = false;





    //constructor
    public AgentData(AGE_STATE age)  
    {
        currAge = age;

        if (0.5f > Random.value) 
        {
            gender = 0;
            name = GeneralUtil.femaleNames[Random.Range(0, 49)];
        }
        else
        {
            gender = 1;
            name = GeneralUtil.maleNames[Random.Range(0, 49)];
        }


        GeneralUtil.timeCycle.OnFunctionCalled.AddListener(EndOfDayCall);

    }




    /// <summary>
    /// this is used to set the age of the npc, similar to the time of day thing, should be called every new day
    /// </summary>
    /// <returns></returns>
    public bool AgeUp()
    {

        daysAlive++;

        //if ((int)AGE_STATE.BABY <= dayAlive)
        //{

        //}

        //if ((int)AGE_STATE.TEEN >= currentHour)
        //{

        //}

        //if ((int)TIME.DAY <= currentHour && (int)TIME.AFTERNOON > currentHour)
        //{

        //}

        //if ((int)TIME.AFTERNOON <= currentHour && (int)TIME.NIGHT > currentHour)
        //{

        //}


        //on death need to delete it self from everywhere
        return true;
    }

    public void SetDead()
    {

    }





    /// <summary>
    /// given the start and end of the pathing sets all the vars
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="forced"></param>
    public bool SetAgentPathing(Vector2Int start,  Vector2Int end, bool forced = false) 
    {
        pathTile = GeneralUtil.A_StarPathfinding(start, end, this,forced);

        if (pathTile == null)
            return false;

        tileStart = GeneralUtil.map.tilesArray[start.x, start.y];
        tileDestination = GeneralUtil.map.tilesArray[end.x, end.y];


        return true;
    }



    // i dont like the atHouse toggle thign
    public void HourTick()
    {

        //if (GeneralUtil.timeCycle.isNightTime != lastTime)
        //{
        //    lastTime = GeneralUtil.timeCycle.isNightTime;

        //    if (GeneralUtil.timeCycle.isNightTime) // if its night time 
        //    {
        //        if (refToHouse == null) // and has no house then this llogic takes over
        //        {
        //            Debug.Log("call on the first time for agent at night");
        //            //this needs to take over
        //        }

        //    }
        //    else // if its day time and no job this takes over
        //    {
        //        if (refToWorkPlace == null)
        //        {

        //        }
        //    }
        //}





        //if (GeneralUtil.timeCycle.isNightTime) // if its night time 
        //{
        //    if (refToHouse == null) // and has no house then this llogic takes over
        //    {
        //        //this needs to take over
        //        if (agentObj == null) 
        //        {
        //            GeneralUtil.map.SpawnAgent(this.guid, GeneralUtil.ReturnTile(refToWorkPlace.entrancePoints[0]));
        //        }

        //        pathTile = GeneralUtil.A_StarPathfinding(GeneralUtil.ReturnTile(refToWorkPlace.entrancePoints[0]).coord, GeneralUtil.RandomTileAround(5, GeneralUtil.ReturnTile(refToWorkPlace.entrancePoints[0]).coord, new List<int> { 0, 1 }).coord, this);
        //    }
        //}
        //else // if its day time and no job this takes over
        //{
        //    if (refToWorkPlace == null) 
        //    {
                
        //    }
        //}
    }



    private void EndOfDayCall() 
    {
        if (GeneralUtil.timeCycle.isNightTime) // if its night time 
        {
            if (refToHouse == null) // and has no house then this llogic takes over
            {
                SetToSleeping(GeneralUtil.ReturnTile(refToWorkPlace.entrancePoints[0])); 
            }
        }
        else // if its day time and no job this takes over
        {
            if (refToWorkPlace == null)
            {
                SetToWonder(GeneralUtil.ReturnTile(refToHouse.entrancePoints[0]));
            }
        }
    }




    public void SetToWonder(Tile tile = null) 
    {
        currAction = CURRENT_ACTION.WONDERING;

        if (agentObj == null)
        {
            GeneralUtil.map.SpawnAgent(this.guid, GeneralUtil.map.tilesArray[refToWorkPlace.entrancePoints[0].x, refToWorkPlace.entrancePoints[0].y]);
        }

        agentObj.GetComponent<Agent>().StopAllCoroutines();

        SetAgentPathing(GeneralUtil.WorldTileNoLoop(agentObj.transform.position).coord, GeneralUtil.RandomTileAround(5, GeneralUtil.WorldTileNoLoop(agentObj.transform.position).coord, new List<int> { 0, 1 }).coord,true);


        //pathTile = GeneralUtil.A_StarPathfinding(GeneralUtil.WorldTileNoLoop(agentObj.transform.position).coord, GeneralUtil.RandomTileAround(5,  GeneralUtil.WorldTileNoLoop(agentObj.transform.position).coord, new List<int> { 0, 1 }).coord, this);
    }


    public void SetToSleeping(Tile tile = null) 
    {
        currAction = CURRENT_ACTION.SLEEPING;

        if (agentObj == null)
        {
            GeneralUtil.map.SpawnAgent(this.guid, GeneralUtil.map.tilesArray[ refToHouse.entrancePoints[0].x, refToHouse.entrancePoints[0].y]);
        }

        agentObj.GetComponent<Agent>().StopAllCoroutines();

        SetAgentPathing(GeneralUtil.WorldTileNoLoop(agentObj.transform.position).coord, GeneralUtil.RandomTileAround(5, GeneralUtil.WorldTileNoLoop(agentObj.transform.position).coord, new List<int> { 0, 1 }).coord, true);
    }




    public void DayTick()
    {
       // AgeUp();
       // Debug.Log("Day tick in the npc data class");
    }

    public void MinuteTick()
    {
        //Debug.Log("minute tick in the npc data class");
    }



    //the npc here should have an inventory
}
