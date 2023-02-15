
using UnityEngine;
using static UnityEditor.PlayerSettings;

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


    private void OnEnable()
    {
        GeneralUtil.timeCycle.dayTick.AddListener(DayTick);
        GeneralUtil.timeCycle.minuteTick.AddListener(MinuteTick);
        GeneralUtil.timeCycle.hourTick.AddListener(HourTick);
    }

    private void OnDisable()
    {
        GeneralUtil.timeCycle.dayTick.RemoveListener(DayTick);
        GeneralUtil.timeCycle.hourTick.RemoveListener(HourTick);
        GeneralUtil.timeCycle.minuteTick.RemoveListener(MinuteTick);
    }


    public void InitiateCouncil() 
    {
        for (int i = 0; i < 4; i++)
        {

            var destination = new Vector2Int(buildingId.buildingData.centerCoord.x + Random.Range(-10, 10), buildingId.buildingData.centerCoord.y + Random.Range(-10, 10));   //chooses the random pos to spawn on
            if (destination.x < 0 || destination.y < 0 || destination.x >= GeneralUtil.map.tilesArray.GetLength(0) || destination.y >= GeneralUtil.map.tilesArray.GetLength(1))
            {
                destination = buildingId.buildingData.entrancePoints[0];
            }

            if (GeneralUtil.map.tilesArray[destination.x,destination.y].tileType == TileType.WATER) 
            {
                destination = buildingId.buildingData.entrancePoints[0];
            }


            var newCitizen = new AgentData(AgentData.AGE_STATE.ADULT);     // initiates the class
            GeneralUtil.dataBank.npcDict.Add(newCitizen.guid, newCitizen);   // adds it to the dict
            newCitizen.refToWorkPlace = buildingId.buildingData;    //sets the workplace to the council
            buildingId.buildingData.workers.Add(newCitizen);    // adds the worker to the council

            GeneralUtil.map.SpawnAgent(newCitizen.guid, GeneralUtil.map.tilesArray[destination.x,destination.y]);   //spawns the agent

        }
    }







    private void MinuteTick() 
    {
        Debug.Log("called on the building");
    }
    private void HourTick() 
    {
        foreach (var worker in buildingId.buildingData.workers)
        {
            if (worker.currAction == AgentData.CURRENT_ACTION.IDLE) 
            {
                worker.currAction = AgentData.CURRENT_ACTION.WORKING;
                GeneralUtil.map.SpawnAgent(worker.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);

            }
        }
    }
    private void DayTick() { }





    private void Update()
    {
        if (test) 
        {
            //test = false;

            //foreach (var worker in buildingId.buildingData.workers)
            //{
            //    if (!worker.currAction != AgentData.CURRENT_ACTION.) 
            //    {
            //        GeneralUtil.map.SpawnAgent(worker.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);

            //        GeneralUtil.dataBank.npcDict[worker.guid].pathTile = GeneralUtil.A_StarPathfinding(buildingId.buildingData.entrancePoints[0], buildingId.buildingData.tileInRange[Random.Range(0, buildingId.buildingData.tileInRange.Count)], GeneralUtil.dataBank.npcDict[worker.guid]);

            //        worker.busy = true;
            //        break;
            //    }
            //}
            
        }
    }
}
