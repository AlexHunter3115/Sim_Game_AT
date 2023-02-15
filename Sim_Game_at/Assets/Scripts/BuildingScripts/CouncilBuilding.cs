
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
            //get the position to spawn the agents
            var destination = new Vector2Int(buildingId.buildingData.centerCoord.x + Random.Range(-5, 5), buildingId.buildingData.centerCoord.y + Random.Range(-5, 5));   //chooses the random pos to spawn on
            if (destination.x < 0 || destination.y < 0 || destination.x >= GeneralUtil.map.tilesArray.GetLength(0) || destination.y >= GeneralUtil.map.tilesArray.GetLength(1))
            {
                destination = new Vector2Int(buildingId.buildingData.entrancePoints[0].x+3 , buildingId.buildingData.entrancePoints[0].y + 1);
            }
            else if (GeneralUtil.map.tilesArray[destination.x,destination.y].tileType == TileType.WATER) 
            {
                destination = new Vector2Int(buildingId.buildingData.entrancePoints[0].x+3, buildingId.buildingData.entrancePoints[0].y + 1);
            }


            var newCitizen = new AgentData(AgentData.AGE_STATE.ADULT);     // initiates the class
            GeneralUtil.dataBank.npcDict.Add(newCitizen.guid, newCitizen);   // adds it to the dict
            newCitizen.refToWorkPlace = buildingId.buildingData;    //sets the workplace to the council
            buildingId.buildingData.workers.Add(newCitizen);    // adds the worker to the council

            GeneralUtil.map.SpawnAgent(newCitizen.guid, GeneralUtil.map.tilesArray[destination.x,destination.y]);   //spawns the agent


            newCitizen.SetAgentPathing(destination, buildingId.buildingData.entrancePoints[0], true);
           
            newCitizen.moving = true;
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
            //if (worker.currAction == AgentData.CURRENT_ACTION.IDLE)  // is there an agent in the builing, as the woker is set to idle
            //{
            //    Debug.Log($"There is someone idle");
            //    worker.currAction = AgentData.CURRENT_ACTION.WORKING; // set it to working
            //    GeneralUtil.map.SpawnAgent(worker.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);  //spawn the agent

            //}
            
            if (worker.currAction == AgentData.CURRENT_ACTION.IDLE && worker.pathTile.Count>0) 
            {
                worker.currAction = AgentData.CURRENT_ACTION.WORKING;
                GeneralUtil.map.SpawnAgent(worker.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);  //spawn the agent
            }
        
        }
    }
    private void DayTick() { }





    private void Update()
    {
        if (test) 
        {
            test = false;
           HourTick();
            
        }
    }
}
