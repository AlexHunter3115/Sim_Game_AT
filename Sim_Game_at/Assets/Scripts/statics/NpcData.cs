using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static TimeCycle;
using static UnityEditor.PlayerSettings;

public class AgentData : Entity
{
    public enum OCCUPATION 
    {
        JOBLESS,
        WOOD_GUY,
        MINER,
        FISHER,
        FOODGUY
    }
    public OCCUPATION currJob;

    public Dictionary<OCCUPATION, int> levelOccupation = new Dictionary<OCCUPATION, int>();

    //this can be just null if no hosue or work
    public BuildingData refToHouse;
    public BuildingData refToWorkPlace;

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
    public int dayAlive;

    public enum CURRENT_ACTION
    {
        WORKING,   // working
        SLEEPING,   // in house or outside just not avaialable  this at night
        WONDERING,  // no job no thing useless citizen not at night 
        RETURNING,   // gcoming back from somewhere
        IDLE,    //if its working and got nothgin to do
        MOVING
    }
    public CURRENT_ACTION currAction;

    #endregion


    #region pathing

    public Tile tileDestination;
    public List<Tile> pathTile;

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


    //night time setup
    //this is called when to age up
    public bool AgeUp()
    {

        dayAlive++;

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




    #region overrides
    // this is the ticks to trickle down health
    public override void TickDailyCycle()
    {
        base.TickDailyCycle();

        //if ()

        // should check if there is a house for free
        // shoudl check if there is a work for free

      
    }
    public override void TickMinuteCycle()
    {
        base.TickMinuteCycle();
    }
    public override void TickHourCycle()
    {
        base.TickHourCycle();
        // if at work, if in path to something or doing something then no
        // if at work and doing nothing than  quick perc liek 0.9% will do somehi
        //also stamina has a choice


        if (GeneralUtil.timeCycle.currentDayState != TimeCycle.TIME.NIGHT)
        {

            if (refToWorkPlace != null)
            {
                if (this.currAction == CURRENT_ACTION.IDLE)   // idle means at work but doing nothing
                {
                    //entrance point should be random
                    // could have a for loop with tries

                    var path = GeneralUtil.A_StarPathfinding(refToWorkPlace.entrancePoints[0], refToWorkPlace.tileInRange[Random.Range(0, refToWorkPlace.tileInRange.Count)], this);

                    if (GeneralUtil.PathContainsTileType(TileType.WATER, path))
                    {
                        // nothing happens
                    }
                    else
                    {
                        this.pathTile = path;
                        this.currAction = CURRENT_ACTION.WORKING;
                    }


                }
            }
        }
        else 
        {

            if (refToHouse != null)
            {
                //has a house
            }
            else 
            {
                //doesnt have a house
                currAction = CURRENT_ACTION.MOVING;



                for (int i = 0; i < 5; i++)
                {
                    var destination = new Vector2Int(refToWorkPlace.entrancePoints[0].x + Random.Range(-10, 10), refToWorkPlace.entrancePoints[0].y + Random.Range(-10, 10));
                    if (destination.x < 0 || destination.y < 0 || destination.x >= GeneralUtil.map.tilesArray.GetLength(0) || destination.y >= GeneralUtil.map.tilesArray.GetLength(1))
                    {
                        continue;
                    }

                    var path = GeneralUtil.A_StarPathfinding(refToWorkPlace.entrancePoints[0], destination, this);

                    if (GeneralUtil.PathContainsTileType(TileType.WATER, path))
                    {
                        continue;
                    }
                    else
                    {
                        this.pathTile = path;

                    }
                }
            }
        }
    }
    #endregion





    //the npc here should have an inventory
}
