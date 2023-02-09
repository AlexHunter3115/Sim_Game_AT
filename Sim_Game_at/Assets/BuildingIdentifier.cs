using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingIdentifier : MonoBehaviour
{

    public BuildingData buildingData;
    public string guid;



    public void init(Tile middleTile, Vector2Int size) 
    {
        buildingData = new BuildingData(BuildingData.BUILDING_TYPE.COUNCIL,size,middleTile.coord);
        this.guid = buildingData.guid;
    }






}
