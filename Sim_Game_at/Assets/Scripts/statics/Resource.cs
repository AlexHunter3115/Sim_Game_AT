using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public int woodAmount;
    public int stoneAmount;
    public int foodAmount;

    public enum RESOURCE_TYPE 
    {
        STONE = 0,
        FOOD,
        WOOD
    }
    public RESOURCE_TYPE type;

    public bool available = true;
    public Tile tile;
}
