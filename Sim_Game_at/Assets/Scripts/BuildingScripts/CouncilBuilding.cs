using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouncilBuilding : MonoBehaviour
{
    private BuildingIdentifier buildingId;
    public bool test = false;
    public bool beforeTest = false;


    private void Start()
    {
        buildingId = transform.GetComponent<BuildingIdentifier>();
        InitiateCouncil();
    }

    public void InitiateCouncil() 
    {
        for (int i = 0; i < 4; i++)
        {
            var newCitizen = new NpcData(NpcData.AGE_STATE.ADULT, buildingId.buildingData);
            GeneralUtil.dataBank.npcDict.Add(newCitizen.guid, newCitizen);
            newCitizen.refToWorkPlace = buildingId.buildingData;
            buildingId.buildingData.workers.Add(newCitizen);
        }
    }


    private void Update()
    {
        if (test) 
        {
            test = false;


            GeneralUtil.map.SpawnAgent(buildingId.buildingData.workers[0].guid,  GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]  );
            GeneralUtil.dataBank.npcDict[buildingId.buildingData.workers[0].guid].pathTile = GeneralUtil.A_StarPathfinding(buildingId.buildingData.entrancePoints[0], buildingId.buildingData.tileInRange[0]);
            
            Debug.Log(buildingId.buildingData.entrancePoints[0]);
            Debug.Log(buildingId.buildingData.tileInRange[0]);
            Debug.Log(buildingId.buildingData.centerCoord);
        }

    }


    
    public void DelCoord(Vector2Int coord) 
    {
    }





}
