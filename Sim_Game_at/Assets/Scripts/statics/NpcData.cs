using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcData : Entity
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

    public bool hasHouse;
    public BuildingData refToHouse;

    public BuildingData refToWorkPlace;

    #region stats

    public string name;
    public float health;
    public float hunger;
    public float stamina;
    public int gender;
    public float speed;

    public bool busy;

    public enum AGE_STATE 
    {
        BABY,
        TEEN,
        ADULT,
        ELDER
    }
    public AGE_STATE currAge;

    #endregion


    #region pathing

    public float maxCbaTileDist;

    public Tile tileDestination;
    public List<Tile> pathTile;

    #endregion


    #region relationships
    public NpcData spouse;
    public NpcData[] parentsArr = new NpcData[2];
    public List<NpcData> children;

    public void SetParents(NpcData parentOne, NpcData parentTwo)
    {
        parentsArr[0] = parentOne;
        parentsArr[0] = parentTwo;
    }
    public void AddChild(NpcData child) => this.children.Add(child);
    #endregion

    public NpcData(AGE_STATE age, BuildingData refToHouse)  
    {
        currAge = age;
        this.refToHouse = refToHouse;

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


    public bool AgeUp() 
    {



        //there needs to be a var that checks for days alive
        // this will return false delete

        //just set the shit to 0
        return true;
    }
    #region overrides
    // this is the ticks to trickle down health
    public override void TickDailyCycle()
    {
        base.TickDailyCycle();
    }
    public override void TickMinuteCycle()
    {
        base.TickMinuteCycle();
    }
    public override void TickHourCycle()
    {
        base.TickHourCycle();
    }
    #endregion

}
