using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouncilBuilding : MonoBehaviour
{
    private BuildingIdentifier buildingId;



    private void Start()
    {
        buildingId = transform.GetComponent<BuildingIdentifier>();
    }

    public void InitiateCouncil() 
    {
        for (int i = 0; i < 4; i++)
        {
            var newCitizen = new NpcData(NpcData.AGE_STATE.ADULT, buildingId.buildingData);
            GeneralUtil.dataBank.npcDict.Add(newCitizen.guid, newCitizen);

            buildingId.buildingData.workers.Add(newCitizen);
        }



    }











}
