using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class DataHolder : MonoBehaviour
{
    private void Awake()
    {
        GeneralUtil.dataBank = this;
    }

    public Dictionary<string, AgentData> npcDict = new Dictionary<string, AgentData>();
    public Dictionary<string, BuildingData> buildingDict = new Dictionary<string, BuildingData>();

    public int numOfBuidlingsWoodSpecific = 0;
    public int numOfBuidlingsStoneSpecific = 0;

    public int numOfResourcesStone = 0;
    public int numOfResourcesWood = 0;
    public int numOfResourcesFood = 0;

    public int daysPassed = 0;

    public HashSet<Vector2Int> allowedBuildingLocations = new HashSet<Vector2Int>();

    private static List<int> numberOfStoneResources = new List<int>();
    private static List<int> numberOfWoodResources = new List<int>();
    private static List<int> numberOfFoodResources = new List<int>();
    private static List<int> numberOfTotalResources = new List<int>();

    private static List<int> numberOfTotaPeople = new List<int>();

    private static List<int> howManyWoodBuildings = new List<int>();
    private static List<int> howManyStoneBuildings = new List<int>();
    private static List<int> howManyTotalBuildings = new List<int>();

    private static List<int> listOfCostsWood = new List<int>();
    private static List<int> listOfCostsFood = new List<int>();
    private static List<int> listOfCostsStone = new List<int>();
    private static List<int> listOfCostsSand = new List<int>();

    private static List<int> closeDayWoodHoldings = new List<int>();
    private static List<int> closeDayFoodHoldings = new List<int>();
    private static List<int> closeDayStoneHoldings = new List<int>();
    private static List<int> closeDaySandHoldings = new List<int>();

    private static List<int> failureToDeliverList = new List<int>();



    private List<int>[] arrayOfLists = new List<int>[17]
    {
        numberOfStoneResources,
        numberOfWoodResources,
        numberOfFoodResources,
        numberOfTotalResources,

        numberOfTotaPeople,

        howManyWoodBuildings,
        howManyStoneBuildings,
        howManyTotalBuildings,

        listOfCostsFood,
        listOfCostsStone,
        listOfCostsSand,
        listOfCostsWood,

        closeDayWoodHoldings,
        closeDayFoodHoldings,
        closeDayStoneHoldings,
        closeDaySandHoldings,

        failureToDeliverList
    };
    public List<int>[] ArrayOfLists
    {
        get { return arrayOfLists; }
    }

    private string[] arrayOfListsNames = new string[17]
    {
        "Stone resources",
        "Wood resources",
        "Food resources",
        "Total resources",

        "People amount",

        "Wood buildings",
        "Stone buildings",
        "All buildings",

        "Daily cost of food",
        "Daily cost of stone",
        "Daily cost of sand",
        "Daily cost of wood", 

        "Daily close wood amount",
        "Daily close food amount",
        "Daily close stone amount",
        "Daily close sand amount",

        "Failure To Deliver"
    };
    public string[] ArrayOfListsNames
    {
        get { return arrayOfListsNames; }
    }



    public void SaveAllResourceThisCycle(int totalFoodDay, int totalStoneDay, int totalSandDay, int totalWoodDay, int failureToDeliver) 
    {
        if (numberOfStoneResources.Count == 15) 
        {
            numberOfWoodResources.RemoveAt(0);
            numberOfStoneResources.RemoveAt(0);
            numberOfTotalResources.RemoveAt(0);
            numberOfFoodResources.RemoveAt(0);

            numberOfTotaPeople.RemoveAt(0);

            howManyWoodBuildings.RemoveAt(0);
            howManyStoneBuildings.RemoveAt(0);
            howManyTotalBuildings.RemoveAt(0);

            listOfCostsWood.RemoveAt(0);
            listOfCostsFood.RemoveAt(0);
            listOfCostsStone.RemoveAt(0);
            listOfCostsSand.RemoveAt(0);

            closeDayWoodHoldings.RemoveAt(0);
            closeDayFoodHoldings.RemoveAt(0);
            closeDayStoneHoldings.RemoveAt(0);
            closeDaySandHoldings.RemoveAt(0);

            failureToDeliverList.RemoveAt(0);
        }

       
        numberOfStoneResources.Add(numOfResourcesStone);
        numberOfFoodResources.Add(numOfResourcesFood);
        numberOfWoodResources.Add(numOfResourcesWood);
        numberOfTotalResources.Add(numOfResourcesStone + numOfResourcesFood + numOfResourcesWood);

        numberOfTotaPeople.Add(GeneralUtil.resourceBank.peopleAmount);
        howManyWoodBuildings.Add(numOfBuidlingsStoneSpecific);
        howManyStoneBuildings.Add(numOfBuidlingsWoodSpecific);
        howManyTotalBuildings.Add(buildingDict.Count);

        listOfCostsWood.Add(totalWoodDay);
        listOfCostsFood.Add(totalFoodDay);
        listOfCostsStone.Add(totalStoneDay);
        listOfCostsSand.Add(totalSandDay);

        closeDayWoodHoldings.Add(GeneralUtil.resourceBank.woodAmount);
        closeDayFoodHoldings.Add(GeneralUtil.resourceBank.foodAmount);
        closeDayStoneHoldings.Add(GeneralUtil.resourceBank.stoneAmount);
        closeDaySandHoldings.Add(GeneralUtil.resourceBank.sandAmount);

        failureToDeliverList.Add(failureToDeliver);

    }

    public void SpendDailyNeeds() 
    {
        GeneralUtil.Ui.SetProgressState(3);

        daysPassed++;

        GeneralUtil.Ui.SetDaysText(daysPassed);

        int totalStoneAmountToPay = 0;
        int totalFoodAmountToPay = 0;
        int totalSandAmountToPay = 0;
        int totalWoodAmountToPay = 0;

        int failureToDeliver = 0;

        List<AgentData> npcToDelete = new List<AgentData>();

        foreach (var npc in npcDict.Values)
        {
            if (GeneralUtil.resourceBank.CheckFoodAmount(2)) 
            {
                totalFoodAmountToPay += 2;
                GeneralUtil.resourceBank.ChangeFoodAmount(2);
                npc.ChangeHealth(5);
            }
            else 
            {
                npc.ChangeHealth(-5);
            }

            if (npc.refToHouse == null) 
            {
                npc.ChangeHealth(-5);
            }
            if (npc.refToWorkPlace == null)
            {
                npc.ChangeHealth(-5);
            }

            if (npc.health <= 0) 
            {
                npcToDelete.Add(npc);
            }
        }

        foreach (var item in npcToDelete)
        {
            npcDict.Remove(item.guid);
        }

        npcToDelete.Clear();


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

            if (!GeneralUtil.resourceBank.CheckFoodAmount(building.stats.keepUpCostWSFS[2])) 
            {
                failureToDeliver++;
               building.ChangeHealth(-10);
                GeneralUtil.mapInteraction.SpawnFloatingText("Abbandond", Color.yellow, building.buildingID.transform);
                continue;
            }
            else if (!GeneralUtil.resourceBank.CheckSandAmount(building.stats.keepUpCostWSFS[3]))
            {
                failureToDeliver++;
                building.ChangeHealth(-10);
                GeneralUtil.mapInteraction.SpawnFloatingText("Abbandond", Color.yellow, building.buildingID.transform);
                continue;
            }
            else if (!GeneralUtil.resourceBank.CheckStoneAmount(building.stats.keepUpCostWSFS[1]))
            {
                failureToDeliver++;
                building.ChangeHealth(-10);
                GeneralUtil.mapInteraction.SpawnFloatingText("Abbandond", Color.yellow, building.buildingID.transform);
                continue;
            }
            else if (!GeneralUtil.resourceBank.CheckWoodAmount(building.stats.keepUpCostWSFS[0]))
            {
                failureToDeliver++;
                building.ChangeHealth(-10);
                GeneralUtil.mapInteraction.SpawnFloatingText("Abbandond", Color.yellow, building.buildingID.transform);
                continue;
            }


            GeneralUtil.resourceBank.ChangeFoodAmount(-building.stats.keepUpCostWSFS[2]);
            GeneralUtil.resourceBank.ChangeStoneAmount(-building.stats.keepUpCostWSFS[1]);
            GeneralUtil.resourceBank.ChangeSandAmount(-building.stats.keepUpCostWSFS[3]);
            GeneralUtil.resourceBank.ChangeWoodAmount(-building.stats.keepUpCostWSFS[0]);

            totalFoodAmountToPay += building.stats.keepUpCostWSFS[2];
            totalSandAmountToPay += building.stats.keepUpCostWSFS[3];
            totalWoodAmountToPay += building.stats.keepUpCostWSFS[0];
            totalStoneAmountToPay += building.stats.keepUpCostWSFS[1];
        }
        

        SaveAllResourceThisCycle(totalFoodAmountToPay, totalStoneAmountToPay, totalSandAmountToPay, totalWoodAmountToPay,
                failureToDeliver);

        CalculationForNewDay(totalFoodAmountToPay, totalStoneAmountToPay, totalSandAmountToPay, totalWoodAmountToPay);

        GeneralUtil.Ui.SetProgressState(4);
    }

    public void RecalcAllAllowedTiles() 
    {
        allowedBuildingLocations.Clear();
        foreach (var building in buildingDict.Values)
        {
            //building.workers.Count > 0 &&
            if (building.workers.Count > 0 && (building.buildingID.buildingIndex == 0 || building.buildingID.buildingIndex == 3)) 
            {
                for (int x = building.centerCoord.x - building.stats.tileRange; x <= building.centerCoord.x + building.stats.tileRange; x++)
                {
                    for (int y = building.centerCoord.y - building.stats.tileRange; y <= building.centerCoord.y + building.stats.tileRange; y++)
                    {
                        // Calculate the distance from the center tile to the current tile
                        int distance = (int)Math.Sqrt(Math.Pow(building.centerCoord.x - x, 2) + Math.Pow(building.centerCoord.y - y, 2));

                        // Check if the current tile is within the given radius
                        if (distance <= building.stats.tileRange)
                        {
                            allowedBuildingLocations.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
    }




    public List<string> unemployedNpc = new List<string>();
    public float importanceDivision = 0.5f;

    public float leftAreaThreashold = 0.8f;

    public float importanceOfFood = 1;
    public float importanceOfStone = 1;
    public float importanceOfSand = 1;
    public float importanceOfWood = 1;


    public void CalculationForNewDay(int foodSpendingAmount, int stoneSpendingAmount, int sandSpendingAmount, int woodSpendingAmount) 
    {
        //this whoel thing needs a revamp
        int jobLess = 0;
        int homeLess = 0;
        int grownups = 0;

        foreach (var npc in npcDict.Values)
        {
            if (npc.currAge == AgentData.AGE_STATE.BABY) 
            {
                if (npc.daysAlive > 3) 
                {
                    jobLess++;
                    homeLess++;
                    grownups++;
                }
                continue;
            }
            else 
            {
                if (npc.refToHouse == null)
                    homeLess++;
                if (npc.refToWorkPlace == null)
                    jobLess++;

                grownups++;
            }
        }

        int housePlacesAvailable = 0;
        int jobPlacesAvailable = 0;
        float totalAreaTaken = 0;

        foreach (var building in buildingDict.Values)
        {
            if (building.stats.type == BuildingData.BUILDING_TYPE.HOUSE) 
            {
                housePlacesAvailable += building.maxWorkers - building.workers.Count;
            }
            else 
            {
                jobPlacesAvailable += building.maxWorkers - building.workers.Count;
            }

            totalAreaTaken += (float)GetCircleArea((double)building.stats.poissantRadius);
        }

        foreach (var cord in allowedBuildingLocations)
        {
            if (GeneralUtil.map.tilesArray[cord.x,cord.y].tileType == TileType.WATER)
                totalAreaTaken++;
        }



        // negative means is in need
        var jobDiscrepency =  jobPlacesAvailable - jobLess;
        var houseDiscrepency = housePlacesAvailable - homeLess;

        float percTaken = totalAreaTaken / allowedBuildingLocations.Count;

        if (0.4f < percTaken)
        {
            RunPoissant(3);
            return;
        }

        //fix the casting to float stuff, this is disgusting   edit: this whole codebase is disgusting

        float percHouseLess = (float)homeLess / (float)grownups;
        float percJobLess = (float)jobLess / (float)grownups;


        if (percHouseLess <= percJobLess) 
        {
            if (houseDiscrepency < 0) 
            {
                RunPoissant(6);
            }
            if (jobDiscrepency < 0) 
            {
                float importanceFood = ((((float)GeneralUtil.resourceBank.foodAmount / (float)GeneralUtil.resourceBank.foodMaxAmount) * Mathf.Lerp(0.5f, 1.5f, importanceDivision)) * (1 - ((float)foodSpendingAmount / (float)GeneralUtil.resourceBank.foodAmount)) * Mathf.Lerp(1.5f, 0.5f, importanceDivision)) / (float)importanceOfFood;
                float importanceStone = ((((float)GeneralUtil.resourceBank.stoneAmount / (float)GeneralUtil.resourceBank.stoneMaxAmount) * Mathf.Lerp(0.5f, 1.5f, importanceDivision)) * (1 - ((float)stoneSpendingAmount / (float)GeneralUtil.resourceBank.stoneAmount)) * Mathf.Lerp(1.5f, 0.5f, importanceDivision)) / (float)importanceOfStone;
                float importanceSand = ((((float)GeneralUtil.resourceBank.sandAmount / (float)GeneralUtil.resourceBank.sandMaxAmount) * Mathf.Lerp(0.5f, 1.5f, importanceDivision)) * (1 - ((float)sandSpendingAmount / (float)GeneralUtil.resourceBank.sandAmount)) * Mathf.Lerp(1.5f, 0.5f, importanceDivision)) / (float)importanceOfSand;
                float importanceWood = ((((float)GeneralUtil.resourceBank.woodAmount / (float)GeneralUtil.resourceBank.woodMaxAmount) * Mathf.Lerp(0.5f, 1.5f, importanceDivision)) * (1 - ((float)woodSpendingAmount / (float)GeneralUtil.resourceBank.woodAmount)) * Mathf.Lerp(1.5f, 0.5f, importanceDivision)) / (float)importanceOfWood;

                int decision = 0;

                float smallestNum = Math.Min(Math.Min(importanceFood, importanceStone), Math.Min(importanceSand, importanceWood));

                if (importanceFood == smallestNum) { decision = 5; }
                else if (importanceStone == smallestNum) { decision = 2; }
                else if (importanceSand == smallestNum) { decision = 4; }
                else if (importanceWood == smallestNum) { decision = 1; }
                //else { decision = 3; }

                if (decision == 0)
                {
                    Debug.Log(importanceFood);
                    Debug.Log(importanceSand);
                    Debug.Log(importanceStone);
                    Debug.Log(importanceWood);

                    Debug.Log(smallestNum);

                    Debug.Log("there is an issue");
                    return;
                }

                RunPoissant(decision);
            }
        }
        else 
        {
            if (jobDiscrepency < 0)
            {

                float importanceFood = ((((float)GeneralUtil.resourceBank.foodAmount / (float)GeneralUtil.resourceBank.foodMaxAmount) * Mathf.Lerp(0.5f, 1.5f, importanceDivision)) * (1 - ((float)foodSpendingAmount / (float)GeneralUtil.resourceBank.foodAmount)) * Mathf.Lerp(1.5f, 0.5f, importanceDivision)) / (float)importanceOfFood;
                float importanceStone = ((((float)GeneralUtil.resourceBank.stoneAmount / (float)GeneralUtil.resourceBank.stoneMaxAmount) * Mathf.Lerp(0.5f, 1.5f, importanceDivision)) * (1 - ((float)stoneSpendingAmount / (float)GeneralUtil.resourceBank.stoneAmount)) * Mathf.Lerp(1.5f, 0.5f, importanceDivision)) / (float)importanceOfStone;
                float importanceSand = ((((float)GeneralUtil.resourceBank.sandAmount / (float)GeneralUtil.resourceBank.sandMaxAmount) * Mathf.Lerp(0.5f, 1.5f, importanceDivision)) * (1 - ((float)sandSpendingAmount / (float)GeneralUtil.resourceBank.sandAmount)) * Mathf.Lerp(1.5f, 0.5f, importanceDivision)) / (float)importanceOfSand;
                float importanceWood = ((((float)GeneralUtil.resourceBank.woodAmount / (float)GeneralUtil.resourceBank.woodMaxAmount) * Mathf.Lerp(0.5f, 1.5f, importanceDivision)) * (1 - ((float)woodSpendingAmount / (float)GeneralUtil.resourceBank.woodAmount)) * Mathf.Lerp(1.5f, 0.5f, importanceDivision)) / (float)importanceOfWood;

                int decision = 0;

                float smallestNum = Math.Min(Math.Min(importanceFood, importanceStone), Math.Min(importanceSand, importanceWood));
                
                if (importanceFood == smallestNum) { decision = 5; }
                else if (importanceStone == smallestNum) { decision = 2; }
                else if (importanceSand == smallestNum) { decision = 4; }
                else if (importanceWood == smallestNum) { decision = 1; }
                //else { decision = 6; }

                if (decision == 0) 
                {
                    Debug.Log(importanceFood);
                    Debug.Log(importanceSand);
                    Debug.Log(importanceStone);
                    Debug.Log(importanceWood);

                    Debug.Log(smallestNum);

                    Debug.Log("there is an issue");
                    return; 
                }

                RunPoissant(decision);
            }
            if (houseDiscrepency < 0)
            {
                RunPoissant(6);
            }
        }
    }

    private void RunPoissant(int buildingIndex) 
    {
        var listOfPossiblePoints = GeneralUtil.mapInteraction.GeneratePoissantPoints(buildingIndex);

        if (listOfPossiblePoints.Count > 1)
        {
            GeneralUtil.mapInteraction.SpawnBuildingAuto(GeneralUtil.WorldPosToTile((listOfPossiblePoints.Count == 1 ? listOfPossiblePoints[0] : listOfPossiblePoints[Random.Range(0, listOfPossiblePoints.Count)]).position), buildingIndex);
        }

    }

    private double GetCircleArea(double radius) => Math.PI * Math.Pow(radius, 2);
   

}
