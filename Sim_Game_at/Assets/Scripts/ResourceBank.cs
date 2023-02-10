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
        GeneralUtil.bank = this;
    }

    private void Start()
    {
        ChangeWoodAmount(0);
        ChangeSandAmount(0);
        ChangeStoneAmount(0);
        ChangeFoodAmount(0);
        ChangePeopleAmount(0);
    }


    public bool ChangeWoodAmount(int amount) 
    {
        if (amount < 0)
        {
            if (amount > woodAmount)
                return false;
        }

        woodAmount += amount;
        GeneralUtil.Ui.SetWoodResText(woodAmount);

        return true;
    }

    public bool ChangeSandAmount(int amount)
    {
        if (amount < 0)
        {
            if (amount > sandAmount)
                return false;
        }

        sandAmount += amount;
        GeneralUtil.Ui.SetSandResText(sandAmount);

        return true;
    }

    public bool ChangeStoneAmount(int amount)
    {
        if (amount < 0)
        {
            if (amount > stoneAmount)
                return false;
        }

        stoneAmount += amount;
        GeneralUtil.Ui.SetStoneResText(stoneAmount);

        return true;
    }

    public bool ChangeFoodAmount(int amount)
    {
        if (amount < 0)
        {
            if (amount > foodAmount)
                return false;
        }

        foodAmount += amount;
        GeneralUtil.Ui.SetFoodResText(foodAmount);

        return true;
    }

    public bool ChangePeopleAmount(int amount)
    {
        peopleAmount += amount;
        GeneralUtil.Ui.SetPeepAmoungText(peopleAmount);

        return true;
    }
}
