using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBank : MonoBehaviour
{
    public int woodAmount = 0;
    public int stoneAmount = 0;
    public int foodAmount = 0;
    public int sandAmount = 0;
    public int peopleAmount = 0;


    public int woodMaxAmount = 0;
    public int stoneMaxAmount = 0;
    public int foodMaxAmount = 0;
    public int sandMaxAmount = 0;
    private void Awake() => GeneralUtil.resourceBank = this;

    private void Start()
    {
       
    }

    public void ChangeWoodAmount(int amount) 
    {
        if (amount >= 0)  // if its a positive change
        {
            woodAmount += amount;

            if (woodAmount > woodMaxAmount)
                woodAmount = woodMaxAmount;

            GeneralUtil.Ui.SetWoodResText(woodAmount,woodMaxAmount);
        }
        else   // if its a negative change
        {
            if (CheckWoodAmount(amount))
            {
                woodAmount += amount;
                GeneralUtil.Ui.SetWoodResText(woodAmount, woodMaxAmount);
            }
        }

    }

    public void ChangeSandAmount(int amount)
    {
        if (amount >= 0)
        {
            sandAmount += amount;

            if (sandAmount > sandMaxAmount)
                sandAmount = sandMaxAmount;

            GeneralUtil.Ui.SetSandResText(sandMaxAmount, sandMaxAmount);
        }
        else
        {
            if (CheckSandAmount(amount))
            {
                sandAmount += amount;
                GeneralUtil.Ui.SetSandResText(sandAmount, sandMaxAmount);
            }
        }
    }

    public void ChangeStoneAmount(int amount)
    {
        if (amount >= 0)
        {
            stoneAmount += amount;

            if (stoneAmount > stoneMaxAmount)
                stoneAmount = stoneMaxAmount;

            GeneralUtil.Ui.SetStoneResText(stoneAmount, stoneMaxAmount);
        }
        else
        {
            if (CheckStoneAmount(amount))
            {
                stoneAmount += amount;
                GeneralUtil.Ui.SetStoneResText(stoneAmount, stoneMaxAmount);
            }
        }
    }

    public void ChangeFoodAmount(int amount)
    {
        if (amount >= 0) 
        {
            foodAmount += amount;

            if (foodAmount > foodMaxAmount)
                foodAmount = foodMaxAmount;

            GeneralUtil.Ui.SetFoodResText(foodAmount,foodMaxAmount);
        }
        else 
        {
            if (CheckFoodAmount(amount))
            {
                foodAmount += amount;
                GeneralUtil.Ui.SetFoodResText(foodAmount, foodMaxAmount);
            }
        }
    }


    public void ChangePeopleAmount(int amount) 
    {

        if (amount >= 0)
        {
            peopleAmount += amount;
            GeneralUtil.Ui.SetPeopleAmountText(peopleAmount);
        }
        else
        {
            if (CheckFoodAmount(amount))
            {
                peopleAmount += amount;
                GeneralUtil.Ui.SetPeopleAmountText(peopleAmount);
            }
        }
    }


    public bool CheckWoodAmount(int amount) 
    {
        if (woodAmount - amount < 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckStoneAmount(int amount)
    {
        if (stoneAmount - amount < 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckFoodAmount(int amount)
    {
        if (foodAmount - amount < 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckSandAmount(int amount)
    {
        if (sandAmount - amount < 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckPeopleAmount(int amount)
    {
        if (peopleAmount - amount < 0)
        {
            return false;
        }

        return true;
    }


    public void AddStartingAmount() 
    {
        ChangeWoodAmount(300);
        ChangeSandAmount(300);
        ChangeStoneAmount(300);
        ChangeFoodAmount(300);
    }
}
