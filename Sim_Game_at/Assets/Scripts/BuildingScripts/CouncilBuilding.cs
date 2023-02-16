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


    //the council is the starting point its where the player gets its first 4 people
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
           var some = buildingId.AddWorker(newCitizen.guid); //set the place to work in

            Debug.Log(some);


            GeneralUtil.map.SpawnAgent(newCitizen.guid, GeneralUtil.map.tilesArray[destination.x,destination.y]);   //spawns the agent


            //newCitizen.SetAgentPathing(destination, buildingId.buildingData.entrancePoints[0], true);  //sets the pathing to the workplace
           
           // newCitizen.moving = true;
        }
    }







    private void MinuteTick() 
    {
    }
    private void HourTick() 
    {

        Debug.Log(buildingId.buildingData.workers.Count); 
        foreach (var worker in buildingId.buildingData.workers)
        {
            Debug.Log(worker.readyToWork);
            Debug.Log(worker.currAction);
            if (worker.readyToWork == true && worker.currAction == AgentData.CURRENT_ACTION.WORKING) 
            {
                worker.SetAgentPathing(buildingId.buildingData.entrancePoints[0], buildingId.buildingData.tileInRange[Random.Range(0, buildingId.buildingData.tileInRange.Count)]);
                GeneralUtil.map.SpawnAgent(worker.guid, GeneralUtil.map.tilesArray[buildingId.buildingData.entrancePoints[0].x, buildingId.buildingData.entrancePoints[0].y]);
                worker.readyToWork = false;
                Debug.Log("awdjiwiiwe");
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
