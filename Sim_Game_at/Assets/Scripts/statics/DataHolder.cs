using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    private void Awake()
    {
        GeneralUtil.dataBank = this;
    }

    public Dictionary<string, AgentData> npcDict = new Dictionary<string, AgentData>();
    public Dictionary<string, BuildingData> buildingDict = new Dictionary<string, BuildingData>();

    public int numOfFarms;
    public int numOfHouses;

    public int numOfResourcesStone;
    public int numOfResourcesWood;
    public int numOfResourcesFood;

    public List<string> unemployedNpc = new List<string>();


    public int ReturnAllResourcesNum() 
    {
        return numOfResourcesFood + numOfResourcesStone + numOfResourcesWood;
    }





    //ability check now

}
