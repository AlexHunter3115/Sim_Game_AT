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

    private void Awake()
    {
        GeneralUtil.resourceBank = this;
    }

    private void Start()
    {
        ChangeWoodAmount(0);
        ChangeSandAmount(0);
        ChangeStoneAmount(0);
        ChangeFoodAmount(0);
    }

    public void ChangeWoodAmount(int amount) 
    {
        if (amount >= 0)
        {
            woodAmount += amount;
            GeneralUtil.Ui.SetWoodResText(woodAmount);
        }
        else
        {
            if (CheckWoodAmount(amount))
            {
                woodAmount += amount;
                GeneralUtil.Ui.SetWoodResText(woodAmount);
            }
        }

    }

    public void ChangeSandAmount(int amount)
    {
        if (amount >= 0)
        {
            sandAmount += amount;
            GeneralUtil.Ui.SetSandResText(foodAmount);
        }
        else
        {
            if (CheckSandAmount(amount))
            {
                sandAmount += amount;
                GeneralUtil.Ui.SetSandResText(sandAmount);
            }
        }
    }

    public void ChangeStoneAmount(int amount)
    {
        if (amount >= 0)
        {
            stoneAmount += amount;
            GeneralUtil.Ui.SetStoneResText(stoneAmount);
        }
        else
        {
            if (CheckStoneAmount(amount))
            {
                stoneAmount += amount;
                GeneralUtil.Ui.SetStoneResText(stoneAmount);
            }
        }
    }

    public void ChangeFoodAmount(int amount)
    {
        if (amount >= 0) 
        {
            foodAmount += amount;
            GeneralUtil.Ui.SetFoodResText(foodAmount);
        }
        else 
        {
            if (CheckFoodAmount(amount))
            {
                foodAmount += amount;
                GeneralUtil.Ui.SetFoodResText(foodAmount);
            }
        }
    }


    public void SetPeopleAmount(int amount) 
    {
        peopleAmount = amount;
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

}
