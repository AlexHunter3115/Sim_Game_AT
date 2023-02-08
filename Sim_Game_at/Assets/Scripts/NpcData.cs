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

    // does the constructor inherit and activate the other const?
    public NpcData()  
    {
        
    }

    #region overrides
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
