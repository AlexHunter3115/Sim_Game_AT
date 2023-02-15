using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    private void Awake()
    {
        GeneralUtil.dataBank = this;
    }

    public Dictionary<string, AgentData> npcDict = new Dictionary<string, AgentData>();
    public Dictionary<string, BuildingData> buildingDict = new Dictionary<string, BuildingData>();
}
