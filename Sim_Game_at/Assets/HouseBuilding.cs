using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilding : MonoBehaviour
{
    private BuildingIdentifier buildingId;
    public bool test = false;
    public bool beforeTest = false;


    private void Start()
    {
        buildingId = transform.GetComponent<BuildingIdentifier>();
    }

    public void Init() 
    {
        FindHabitant();
    }


    public void FindHabitant() 
    {
        int num = 0;

        foreach (var agent in GeneralUtil.dataBank.npcDict.Values)
        {
            if (agent.refToHouse == null) 
            {
                num++;

                agent.refToHouse = buildingId.buildingData;

                if (num == 2)
                    break;
            }
        }
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            FindHabitant();

        }
    }
}
