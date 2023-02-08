using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public Dictionary<string, NpcData> npcDict = new Dictionary<string, NpcData>();
    public Dictionary<string, BuildingData> buildingDict = new Dictionary<string, BuildingData>();
}
