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

    public int numOfBuidlingsWoodSpecific;
    public int numOfBuidlingsStoneSpecific;

    public int numOfResourcesStone;
    public int numOfResourcesWood;
    public int numOfResourcesFood;

    public HashSet<Vector2> allowedBuildingLocations = new HashSet<Vector2>();

    public static List<int> listOfValueForResourceStone = new List<int>() {23,145,435,456 };
    public static List<int> listOfValueForResourceWood = new List<int>() { 453, 13, 564, 43 };
    public static List<int> listOfValueForResourceFood = new List<int>() { 23, 34, 2, 456 };
    public static List<int> listOfValueForResourceTotal = new List<int>() { 45, 76, 87, 98, 45, 76, 87, 98, 45, 76, 87, 98, 45, 76 ,3};

    public static List<int> listOfValueForPeople = new List<int>();

    public static  List<int> listOfValueForWoodBuildings = new List<int>();
    public static List<int> listOfValueForStoneBuildings = new List<int>();
    public static List<int> listOfValueForAllBuildings = new List<int>();

    public static List<int> listOfCostsWood = new List<int>();
    public static List<int> listOfCostsFood = new List<int>();
    public static List<int> listOfCostsStone = new List<int>();
    public static List<int> listOfCostsSand = new List<int>();

    public List<int>[] arrayOfLists = new List<int>[]
    {
        listOfValueForResourceStone,
        listOfValueForResourceWood,
        listOfValueForResourceFood,
        listOfValueForResourceTotal,
        listOfValueForPeople,
        listOfValueForWoodBuildings,
        listOfValueForStoneBuildings,
        listOfValueForAllBuildings,
        listOfCostsFood,
        listOfCostsStone,
        listOfCostsSand,
        listOfCostsWood,
    };

    public string[] arrayOfListsNames = new string[]
    {
        "Stone resources",
        "Wood resources",
        "Food resources",
        "Total resources",
        "people amount",
        "wood building",
        "stone building",
        "all building",
        "daily cost of food",
        "daily cost of stone",
        "daily cost of sand",
        "daily cost of wood",
    };

    public List<string> unemployedNpc = new List<string>();


    public void SaveAllResourceThisCycle() 
    {
        if (listOfValueForResourceStone.Count == 15) 
        {
            listOfValueForResourceWood.RemoveAt(0);
            listOfValueForResourceStone.RemoveAt(0);
            listOfValueForResourceTotal.RemoveAt(0);
            listOfValueForResourceFood.RemoveAt(0);


            listOfValueForPeople.RemoveAt(0);
            listOfValueForWoodBuildings.RemoveAt(0);
            listOfValueForStoneBuildings.RemoveAt(0);
            listOfValueForAllBuildings.RemoveAt(0);
        }


        listOfValueForResourceStone.Add(numOfResourcesStone);
        listOfValueForResourceFood.Add(numOfResourcesFood);
        listOfValueForResourceWood.Add(numOfResourcesWood);
        listOfValueForResourceTotal.Add(numOfResourcesStone + numOfResourcesFood + numOfResourcesWood);



        listOfValueForPeople.Add(npcDict.Count);
        listOfValueForWoodBuildings.Add(numOfBuidlingsStoneSpecific);
        listOfValueForStoneBuildings.Add(numOfBuidlingsWoodSpecific);
        listOfValueForAllBuildings.Add(buildingDict.Count);
    }

    public void SpendDailyNeeds() 
    {
        int totalStoneAmountToPay = 0;
        int totalFoodAmountToPay = 0;
        int totalSandAmountToPay = 0;
        int totalWoodAmountToPay = 0;

        foreach (var building in buildingDict.Values)
        {

            if (!allowedBuildingLocations.Contains(building.centerCoord)) 
            {

                bool cont = false;
                if (building.health <= 5)
                {
                    cont = true;
                }

                building.ChangeHealth(-5);
                GeneralUtil.mapInteraction.SpawnFloatingText("Not close to outpost", Color.yellow, building.buildingID.transform);

                if (cont)
                    continue;
            }


            if (!GeneralUtil.resourceBank.CheckFoodAmount(building.upKeepFoodCost)) 
            {
              //  building.ChangeHealth(-10);
                GeneralUtil.mapInteraction.SpawnFloatingText("Abbandond", Color.yellow, building.buildingID.transform);
                continue;
            }
            else if (!GeneralUtil.resourceBank.CheckSandAmount(building.upKeepSandCost)) 
            {
               // building.ChangeHealth(-10);
                GeneralUtil.mapInteraction.SpawnFloatingText("Abbandond", Color.yellow, building.buildingID.transform);
                continue;
            }
            else if (!GeneralUtil.resourceBank.CheckStoneAmount(building.upKeepStoneCost))
            {
               // building.ChangeHealth(-10);
                GeneralUtil.mapInteraction.SpawnFloatingText("Abbandond", Color.yellow, building.buildingID.transform);
                continue;
            }
            else if (!GeneralUtil.resourceBank.CheckWoodAmount(building.upKeepWoodCost))
            {
               // building.ChangeHealth(-10);
                GeneralUtil.mapInteraction.SpawnFloatingText("Abbandond", Color.yellow, building.buildingID.transform);
                continue;
            }

            totalFoodAmountToPay += building.upKeepFoodCost;
            totalSandAmountToPay += building.upKeepSandCost;
            totalWoodAmountToPay += building.upKeepWoodCost;
            totalStoneAmountToPay += building.upKeepStoneCost;
        }


        GeneralUtil.resourceBank.ChangeFoodAmount(-totalFoodAmountToPay);
        GeneralUtil.resourceBank.ChangeStoneAmount(-totalStoneAmountToPay);
        GeneralUtil.resourceBank.ChangeSandAmount(-totalSandAmountToPay);
        GeneralUtil.resourceBank.ChangeWoodAmount(-totalWoodAmountToPay);
    }

    public void RecalcAllAllowedTiles() 
    {
        allowedBuildingLocations.Clear();
        foreach (var building in buildingDict.Values)
        {
            if (building.workers.Count > 0 && (building.buildingID.buildingIndex == 0 || building.buildingID.buildingIndex == 4)) 
            {
                var listOfTiles = GeneralUtil.GetResourcesCloseSpiral(building.centerCoord, building.stats.tileRange);
                
                for (int i = 0; i < listOfTiles.Count; i++)
                {
                    allowedBuildingLocations.Add(listOfTiles[i].coord);
                }
            }
        }
    }

}
