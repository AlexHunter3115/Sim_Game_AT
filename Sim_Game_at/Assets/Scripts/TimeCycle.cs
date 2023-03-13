using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TimeCycle : MonoBehaviour
{
    [Header("in seconds")]
    public float minuteCheck = 5;
    public float hourCheck = 25;
    public float dayCheck = 125;

    public float timerMultiplier = 1;
    private bool timeOn = true;   //deal with this later

    public bool isNightTime; 

    public enum TIME 
    {
        MORNING = 4,
        DAY = 8,
        AFTERNOON = 16,
        NIGHT = 20
    }
    public TIME currentDayState;

    private int currentHour = 8;


    private bool changing = false;
    private float timeElapsed = 0;
    [SerializeField] Color morningColor;
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;
    [SerializeField] Color afternoonColor;

    private Color startColor;
    private Color endColor;

    public float colorLerpDur = 0.5f;

    [SerializeField] Light sunLight;

    public UnityEvent OnFunctionCalled;


    private delegate IEnumerator DailyCycleCorutineVar();
    private DailyCycleCorutineVar _updateDayCheck;

    private delegate IEnumerator MinuteCycleCorutineVar();
    private MinuteCycleCorutineVar _updateMinuteCheck;

    private delegate IEnumerator HourCycleCorutineVar();
    private HourCycleCorutineVar _updateHourCheck;


    private void Awake()
    {
        GeneralUtil.timeCycle = this;

        _updateMinuteCheck = MinuteCycleCorutine;
        _updateHourCheck = HourCycleCorutine;
        _updateDayCheck = DailyCycleCorutine;
        SetDayTime();
    }

    private void Start()
    {
        StartCoroutine(_updateDayCheck());
        StartCoroutine(_updateHourCheck());
        StartCoroutine(_updateMinuteCheck());
    }

    private void Update()
    {
        if (changing)    // this is for the color swap
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / colorLerpDur);
            Color lerpedColor = Color.Lerp(startColor, endColor, t);
            sunLight.color = lerpedColor;

            if (sunLight.color == endColor)
                changing = false;
        }
    }

   
    private IEnumerator DailyCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(dayCheck/timerMultiplier);

            for (int i = 0; i < GeneralUtil.dataBank.npcDict.Count; i++)
            {
                CallDayInterface(GeneralUtil.dataBank.npcDict.Values.ElementAt(i));
            }


            for (int i = 0; i < GeneralUtil.dataBank.buildingDict.Count; i++)
            {
                CallDayInterface(GeneralUtil.dataBank.buildingDict.Values.ElementAt(i).buildingID.buildingTimer);
            }
        }
    }

    private IEnumerator HourCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(hourCheck / timerMultiplier);
            currentHour++;
            if (currentHour >= 24) 
            {
                GeneralUtil.map.StartCoroutine(GeneralUtil.map.CallNewTurnSpawn(Random.Range(5, 15), Random.Range(10, 30)));
                GeneralUtil.dataBank.SpendDailyNeeds();
                currentHour = 0;
            }

            SetDayTime();
            
            for (int i = 0; i < GeneralUtil.dataBank.buildingDict.Count; i++)
            {
                if (GeneralUtil.dataBank.buildingDict.Values.ElementAt(i).buildingID.buildingTimer !=null)
                    CallHourInterface(GeneralUtil.dataBank.buildingDict.Values.ElementAt(i).buildingID.buildingTimer);
            }
            //checm for null so it doesnt crash
            for (int i = 0; i < GeneralUtil.dataBank.npcDict.Count; i++)
            {
                if (GeneralUtil.dataBank.npcDict.Values.ElementAt(i) != null)
                    CallHourInterface(GeneralUtil.dataBank.npcDict.Values.ElementAt(i));
            }

            GeneralUtil.Ui.SetHoursText(currentHour);
        }
    }

    private IEnumerator MinuteCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(minuteCheck / timerMultiplier);

            for (int i = 0; i < GeneralUtil.dataBank.buildingDict.Count; i++)
            {
                CallMinuteInterface(GeneralUtil.dataBank.buildingDict.Values.ElementAt(i).buildingID.buildingTimer);
            }

            for (int i = 0; i < GeneralUtil.dataBank.npcDict.Count; i++)
            {
                CallMinuteInterface(GeneralUtil.dataBank.npcDict.Values.ElementAt(i));
            }
        }
    }


    private void SetDayTime() 
    {
        var prevState = currentDayState;

        if ((int)TIME.NIGHT <= currentHour) 
        {
            startColor = afternoonColor;
            endColor = nightColor;

            currentDayState = TIME.NIGHT;

            isNightTime = true;
        }

        if ((int)TIME.MORNING >= currentHour)
        {
            startColor = nightColor;
            endColor = morningColor;

            currentDayState = TIME.MORNING;

            isNightTime = false;
        }

        if ((int)TIME.DAY <= currentHour && (int)TIME.AFTERNOON > currentHour)
        {
            startColor = morningColor;
            endColor = dayColor;

            currentDayState = TIME.DAY;
            isNightTime = false;
        }

        if ((int)TIME.AFTERNOON <= currentHour && (int)TIME.NIGHT > currentHour)
        {
            startColor = dayColor;
            endColor = afternoonColor;

            currentDayState = TIME.AFTERNOON;
            isNightTime = false;
        }


        if (currentDayState != prevState)
        {
            timeElapsed = 0;
            changing = true;
            if (currentDayState == TIME.NIGHT)
            {
                DayChangeStateEvent();
            }
            else if (currentDayState == TIME.MORNING)
            {
                DayChangeStateEvent();
            }
        }
    }

    public void DayChangeStateEvent()
    {
        OnFunctionCalled?.Invoke();
    }


    private void CallMinuteInterface(ITimeTickers timeInterface) => timeInterface.MinuteTick();
    private void CallHourInterface(ITimeTickers timeInterface) => timeInterface.HourTick();
    private void CallDayInterface(ITimeTickers timeInterface) => timeInterface.DayTick();

}
