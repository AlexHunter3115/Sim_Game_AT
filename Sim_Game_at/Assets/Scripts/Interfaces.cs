


public interface ITestInterfact 
{
    void CallTest();
}

public interface ITimeTickers
{
    void HourTick();
    void DayTick();
}

public interface IAgentInteractions 
{
    void LandedOnEntrance(AgentData data);
}

//start building
public interface IBuildingActions
{
    bool DeleteBuilding();
}