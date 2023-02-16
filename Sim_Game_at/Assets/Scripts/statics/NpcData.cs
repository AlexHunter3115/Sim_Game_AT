using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static TimeCycle;
using static UnityEditor.PlayerSettings;

public class AgentData : Entity
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
        RETURNING,   // gcoming back from somewhere
            //if its working and got nothgin to do
        MOVING,
        TRANSITION
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
    }




    #region time tickers overrides
    public override void TickDailyCycle()
    {
        base.TickDailyCycle();

        AgeUp();
        //we take out health for not having a house at night
      
    }
    public override void TickMinuteCycle()
    {
        base.TickMinuteCycle();
    }
    public override void TickHourCycle() // this ticks every hour
    {
        base.TickHourCycle();
        
        if (GeneralUtil.timeCycle.currentDayState == TIME.NIGHT && !atHouse) //if its night time and not in his house
        {

            Debug.Log($"Getting called on the night setup");
            currAction = CURRENT_ACTION.TRANSITION;
            if (refToHouse == null) 
            {
                //pick a random position from the entrance of work and sleep there
                hardSetAction = CURRENT_ACTION.WONDERING;
            }
            else 
            {
                //you are going to your house
                if (refToWorkPlace != null) 
                {
                    GeneralUtil.map.SpawnAgent(guid, GeneralUtil.Vector2Tile(refToWorkPlace.entrancePoints[0]));
                    SetAgentPathing(refToWorkPlace.entrancePoints[0], refToHouse.entrancePoints[0], true);
                }
                else //if he doesnt have a job he is already out there fore
                {
                    var tile = GeneralUtil.WorldTileCoord(agentObj.transform.position);   //gets the world pos of this currently there agent
                    SetAgentPathing(tile.coord, refToHouse.entrancePoints[0], true);   //sets the pathing
                }

                hardSetAction = CURRENT_ACTION.SLEEPING;
            }

            atHouse = true;
            atWork = false;
        }
        else if (GeneralUtil.timeCycle.currentDayState != TIME.NIGHT && !atWork) // if its not night time and its not at work then
        {
            Debug.Log($"Getting called on the day setup");
            currAction = CURRENT_ACTION.TRANSITION;
            if (refToWorkPlace == null)
            {
                //set the char to wonder
                hardSetAction = CURRENT_ACTION.WONDERING;
            }
            else
            {
                //you are going to your house
                if (refToHouse != null)
                {
                    GeneralUtil.map.SpawnAgent(guid, GeneralUtil.Vector2Tile(refToHouse.entrancePoints[0]));
                    SetAgentPathing(refToHouse.entrancePoints[0], refToWorkPlace.entrancePoints[0], true);
                }
                else //if he doesnt have a house he is already out there fore
                {
                    var tile = GeneralUtil.WorldTileCoord(agentObj.transform.position);   //gets the world pos of this currently there agent
                    SetAgentPathing(tile.coord, refToWorkPlace.entrancePoints[0], true);   //sets the pathing
                }
                hardSetAction = CURRENT_ACTION.WORKING;
                readyToWork = true;
            }

            atHouse = false;
            atWork = true;
        }


    }
    #endregion




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
    public void SetAgentPathing(Vector2Int start,  Vector2Int end, bool forced = false) 
    {
        pathTile = GeneralUtil.A_StarPathfinding(start, end, this,forced);
        tileStart = GeneralUtil.map.tilesArray[start.x, start.y];
        tileDestination = GeneralUtil.map.tilesArray[end.x, end.y];
    }



    //the npc here should have an inventory
}
