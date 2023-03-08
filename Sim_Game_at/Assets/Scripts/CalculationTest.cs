using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CalculationTest : MonoBehaviour
{
    public bool calc = false;

    [Space(10)]
    public int totCitizens = 10;  //the thing here is that we dont need to make new building every cycle as if we dont have a new citizen we need to wait for them
    public int childrenAboutToTurn = 10;
    public int Homeless = 10;
    public int JobLess = 10;

    [Space(10)]
    public float AreaFreeNecessity;  // this is the necessity, as soon as this is ticked this means a new outpost is needed asap for new area
    public float currentAreaTaken;  // this is for the poissant circle shit
    public float totalArea;   // this is the count of the hashset

    [Space(10)]
    public float woodHoldingAmount = 0;
    public float woodMaxAmount = 0;
    public float importanceOfWood = 1;
    public float woodSpendingAmount = 0;

    [Space(10)]
    public float stoneHoldingAmount = 0;
    public float stoneMaxAmount = 0;
    public float importanceOfStone = 1;
    public float stoneSpendingAmount = 0;

    [Space(10)]
    public float sandHoldingAmount = 0;
    public float sandMaxAmount = 0;
    public float importanceOfSand = 0;
    public float sandSpendingAmount = 0;

    [Space(10)]
    public float foodHoldingAmount = 0;
    public float foodMaxAmount = 0;
    public float importanceOfFood = 0;
    public float foodSpendingAmount = 0;

   

    // Update is called once per frame
    void Update()
    {
        if (calc) 
        {
            calc = false;


            bool calcNewBuilding = false;




            if (calcNewBuilding) 
            {
                // the lower the better     also the importance is the higer the more important starst at 1
                float importanceFood = (foodHoldingAmount / foodMaxAmount) * (1 - (foodSpendingAmount / foodHoldingAmount)) / importanceOfFood;
                float importanceStone = (stoneHoldingAmount / stoneMaxAmount) * (1 - (stoneSpendingAmount / stoneHoldingAmount)) / importanceOfStone;
                float importanceSand = (sandHoldingAmount / sandMaxAmount) * (1 - (sandSpendingAmount / sandHoldingAmount)) / importanceOfSand;
                float importanceWood = (woodHoldingAmount / woodMaxAmount) * (1 - (woodSpendingAmount / woodHoldingAmount)) / importanceOfWood;

                float smallestNum = Math.Min(Math.Min(importanceFood, importanceStone), Math.Min(importanceSand, importanceWood));


                Debug.Log($"importance for Wood: {importanceWood}\n");
                Debug.Log($"importance for Stone: {importanceStone}\n");
                Debug.Log($"importance for Sand: {importanceSand}\n");
                Debug.Log($"importance for Food: {importanceFood}\n");
            }


        }
    }



}
