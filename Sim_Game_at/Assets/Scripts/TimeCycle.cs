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

    private float timer1;
    private float timer2;

    private void Awake()
    {
        GeneralUtil.timeCycle = this;

        SetDayTime();
    }

    private void Start()
    {

        timer1 = hourCheck;
        timer2 = dayCheck;

        HourPassCall();
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

        timer1 -= Time.deltaTime;
        timer2 -= Time.deltaTime;

        if (timer1 <= 0)
        {
            HourPassCall();
            timer1 = hourCheck; // Reset the timer
        }

        if (timer2 <= 0)
        {
            DailyPassCall();
            timer2 = dayCheck; // Reset the timer
        }
    }

    private void DailyPassCall() 
    {
        for (int i = 0; i < GeneralUtil.dataBank.npcDict.Count; i++)
        {
            CallDayInterface(GeneralUtil.dataBank.npcDict.Values.ElementAt(i));
        }

        for (int i = 0; i < GeneralUtil.dataBank.buildingDict.Count; i++)
        {
            CallDayInterface(GeneralUtil.dataBank.buildingDict.Values.ElementAt(i).buildingID.buildingTimer);
        }
    }

    private void HourPassCall() 
    {
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
            if (GeneralUtil.dataBank.buildingDict.Values.ElementAt(i).buildingID.buildingTimer != null)
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

    private void CallHourInterface(ITimeTickers timeInterface) => timeInterface.HourTick();
    private void CallDayInterface(ITimeTickers timeInterface) => timeInterface.DayTick();

}
