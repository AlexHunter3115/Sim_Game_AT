using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "buildingTypes", menuName = "BuildTypes", order = 1)]
[Serializable]
public class BuildingTypes : ScriptableObject
{
    public List<BuildingStatistics> buildingStats = new List<BuildingStatistics>();
}


[Serializable]
public class BuildingStatistics 
{
    public BuildingData.BUILDING_TYPE type;
    public Vector2 centerOffset = new Vector2(0,0);
    public string name = "";
    public Vector2Int size = new Vector2Int(0,0);
    public List<Vector2Int> entrances = new List<Vector2Int>();
    public int tileRange = 0;
    public int[] startCostWSFS = new int[4];
    public int[] keepUpCostWSFS = new int[4];
    public int[] hourlyProductionWSFS = new int[4];
    public int[] BankAmountWSFS = new int[4];
    public List<int> allowedTileTypes = new List<int>();
    public List<int> whatResourceLookingFor = new List<int>();
    public GameObject building;
    public int maxWorkers;
    public float poissantRadius = 1;
}
