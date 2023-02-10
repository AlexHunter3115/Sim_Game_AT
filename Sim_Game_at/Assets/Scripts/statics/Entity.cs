using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    public string guid;


    public Entity() 
    {
        guid = System.Guid.NewGuid().ToString();
    }

    public virtual void TickMinuteCycle() { }
    public virtual void TickHourCycle() { }
    public virtual void TickDailyCycle() { }


}
