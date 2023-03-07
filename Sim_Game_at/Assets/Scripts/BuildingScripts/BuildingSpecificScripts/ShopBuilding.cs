using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBuilding : MonoBehaviour, IAgentInteractions, ITimeTickers, IBuildingActions
{
    private BuildingIdentifier buildingId;

    // Start is called before the first frame update
    void Start()
    {
        buildingId = transform.GetComponent<BuildingIdentifier>();
        buildingId.agentActions = this;
        buildingId.buildingActions = this;
        buildingId.buildingTimer = this;
        Init();
    }

    private void Init()
    {
    }

    public void LandedOnEntrance(AgentData data)
    {
        Destroy(data.agentObj);
        
    }

    #region timeTickers ITimeTickers
    public void DayTick()
    {
    }

    public void HourTick()
    {
    }

    public void MinuteTick()
    {
        LookForWorkers();
    }

    #endregion



    private void LookForWorkers()
    {
        if (buildingId.buildingData.workers.Count < buildingId.buildingData.maxWorkers)
        {
            if (GeneralUtil.dataBank.unemployedNpc.Count > 0)
            {
                for (int i = GeneralUtil.dataBank.unemployedNpc.Count; i-- > 0;)
                {
                    buildingId.AddWorker(GeneralUtil.dataBank.unemployedNpc[i]);
                }
            }
        }
    }

    public bool DeleteBuilding()
    {
        return true;
    }
}
