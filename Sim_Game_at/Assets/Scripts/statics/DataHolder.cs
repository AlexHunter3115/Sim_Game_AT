using System;
using System.Collections.Generic;
using UnityEngine;
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

    public HashSet<Vector2> allowedBuildingLocations = new HashSet<Vector2>();

    public static List<int> listOfValueForResourceStone = new List<int>();
    public static List<int> listOfValueForResourceWood = new List<int>();
    public static List<int> listOfValueForResourceFood = new List<int>();
    public static List<int> listOfValueForResourceTotal = new List<int>();

    public static List<int> listOfValueForPeople = new List<int>();

    public static List<int> listOfValueForWoodBuildings = new List<int>();
    public static List<int> listOfValueForStoneBuildings = new List<int>();
    public static List<int> listOfValueForAllBuildings = new List<int>();

    public static List<int> listOfCostsWood = new List<int>();
    public static List<int> listOfCostsFood = new List<int>();
    public static List<int> listOfCostsStone = new List<int>();
    public static List<int> listOfCostsSand = new List<int>();

    public static List<int> listOfHoldingsWood = new List<int>();
    public static List<int> listOfHoldingsFood = new List<int>();
    public static List<int> listOfHoldingsStone = new List<int>();
    public static List<int> listOfHoldingsSand = new List<int>();

    public static List<int> failureToDeliverList = new List<int>();



    private List<int>[] arrayOfLists = new List<int>[17]
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
        listOfHoldingsWood,
        listOfHoldingsFood,
        listOfHoldingsStone,
        listOfHoldingsSand,
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

            listOfCostsWood.RemoveAt(0);
            listOfCostsFood.RemoveAt(0);
            listOfCostsStone.RemoveAt(0);
            listOfCostsSand.RemoveAt(0);

            listOfHoldingsWood.RemoveAt(0);
            listOfHoldingsFood.RemoveAt(0);
            listOfHoldingsStone.RemoveAt(0);
            listOfHoldingsSand.RemoveAt(0);

            failureToDeliverList.RemoveAt(0);
        }

        listOfValueForResourceStone.Add(numOfResourcesStone);
        listOfValueForResourceFood.Add(numOfResourcesFood);
        listOfValueForResourceWood.Add(numOfResourcesWood);
        listOfValueForResourceTotal.Add(numOfResourcesStone + numOfResourcesFood + numOfResourcesWood);

        listOfValueForPeople.Add(GeneralUtil.resourceBank.peopleAmount);
        listOfValueForWoodBuildings.Add(numOfBuidlingsStoneSpecific);
        listOfValueForStoneBuildings.Add(numOfBuidlingsWoodSpecific);
        listOfValueForAllBuildings.Add(buildingDict.Count);

        listOfCostsWood.Add(totalWoodDay);
        listOfCostsFood.Add(totalFoodDay);
        listOfCostsStone.Add(totalStoneDay);
        listOfCostsSand.Add(totalSandDay);

        listOfHoldingsWood.Add(GeneralUtil.resourceBank.woodAmount);
        listOfHoldingsFood.Add(GeneralUtil.resourceBank.foodAmount);
        listOfHoldingsStone.Add(GeneralUtil.resourceBank.stoneAmount);
        listOfHoldingsSand.Add(GeneralUtil.resourceBank.sandAmount);

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

                Debug.Log($"Thaddais");
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

            totalFoodAmountToPay += building.stats.keepUpCostWSFS[2];
            totalSandAmountToPay += building.stats.keepUpCostWSFS[3];
            totalWoodAmountToPay += building.stats.keepUpCostWSFS[0];
            totalStoneAmountToPay += building.stats.keepUpCostWSFS[1];
        }
        
        GeneralUtil.resourceBank.ChangeFoodAmount(-totalFoodAmountToPay);
        GeneralUtil.resourceBank.ChangeStoneAmount(-totalStoneAmountToPay);
        GeneralUtil.resourceBank.ChangeSandAmount(-totalSandAmountToPay);
        GeneralUtil.resourceBank.ChangeWoodAmount(-totalWoodAmountToPay);

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
            if ( (building.buildingID.buildingIndex == 0 || building.buildingID.buildingIndex == 4)) 
            {
                var listOfTiles = GeneralUtil.GetResourcesCloseSpiral(building.centerCoord, building.stats.tileRange);
                
                for (int i = 0; i < listOfTiles.Count; i++)
                {
                    allowedBuildingLocations.Add(listOfTiles[i].coord);
                }
            }
        }
    }




    public List<string> unemployedNpc = new List<string>();
    public float importanceDivision = 0.5f;

    public bool prioritizeHousing = true;
    public float leftAreaThreashold = 0.8f;

    public float importanceOfFood = 1;
    public float importanceOfStone = 1;
    public float importanceOfSand = 1;
    public float importanceOfWood = 1;


    public void CalculationForNewDay(int foodSpendingAmount, int stoneSpendingAmount, int sandSpendingAmount, int woodSpendingAmount) 
    {

        int jobLess = 0;
        int homeLess = 0;

        foreach (var npc in npcDict.Values)
        {
            if (npc.currAge == AgentData.AGE_STATE.BABY) 
            {
                if (npc.daysAlive > 3) 
                {
                    jobLess++;
                    homeLess++;
                }
                continue;
            }
            else 
            {
                if (npc.refToHouse == null)
                    homeLess++;
                if (npc.refToWorkPlace == null)
                    jobLess++;
            }
        }

        int openHouses = 0;
        int openJob = 0;
        float totalAreaTaken = 0;

        foreach (var building in buildingDict.Values)
        {
            if (building.stats.type == BuildingData.BUILDING_TYPE.HOUSE) 
            {
                openHouses += building.maxWorkers - building.workers.Count;
            }
            else 
            {
                openJob += building.maxWorkers - building.workers.Count;
            }

            totalAreaTaken += (float)GetCircleArea((double)building.effectiveRadius);

        }

        // negative means is in need
        var jobDiscrepency =  openJob - jobLess;
        var houseDiscrepency = openHouses - homeLess;

        float percTaken = totalAreaTaken / allowedBuildingLocations.Count;

        if (leftAreaThreashold < percTaken)
        {
            RunPoissant(3);
            return;
        }

        if (prioritizeHousing) 
        {
            if (houseDiscrepency < 0) 
            {
                RunPoissant(6);
                return;
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
                else { decision = 6; }


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
                else { decision = 6; }


                RunPoissant(decision);
                return;
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
