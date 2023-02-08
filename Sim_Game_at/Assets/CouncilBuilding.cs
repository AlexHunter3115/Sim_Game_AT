using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouncilBuilding : MonoBehaviour
{

    private BuildingData buildingData;
    protected string guid;



    public CouncilBuilding() 
    {
        buildingData = new BuildingData(BuildingData.BUILDING_TYPE.COUNCIL);
        this.guid = buildingData.guid;

        
    }







}
