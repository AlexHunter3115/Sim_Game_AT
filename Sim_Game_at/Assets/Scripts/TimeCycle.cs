using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeCycle : MonoBehaviour
{
    [Header("in seconds")]
    public float minuteCheck = 5;
    public float hourCheck = 25;
    public float dayCheck = 125;

    public float timerMultiplier = 1;
    private bool timeOn = true;   //deal with this later
    

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








    [HideInInspector]
    public UnityEvent minuteTick;

    [HideInInspector]
    public UnityEvent hourTick;

    [HideInInspector] 
    public UnityEvent dayTick;


    public void MinuteCycle() => minuteTick.Invoke();
    public void HourCycle() => hourTick.Invoke();
    public void DayCycle() => dayTick.Invoke();









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
        if (changing) 
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
        }
    }

    private IEnumerator HourCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(hourCheck / timerMultiplier);
            currentHour++;
            if (currentHour >= 24)
                currentHour = 0;

            SetDayTime();



            foreach (var npc in GeneralUtil.dataBank.npcDict.Values)
            {
                npc.TickHourCycle();
            }



        }
    }

    private IEnumerator MinuteCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(minuteCheck / timerMultiplier);

           

            minuteTick.Invoke();


            //foreach (var building in GeneralUtil.dataBank.buildingDict.Values)
            //{
            //    building.TickMinuteCycle();
            //}


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
        }

        if ((int)TIME.MORNING >= currentHour)
        {
            startColor = nightColor;
            endColor = morningColor;

            currentDayState = TIME.MORNING;
        }

        if ((int)TIME.DAY <= currentHour && (int)TIME.AFTERNOON > currentHour)
        {
            startColor = morningColor;
            endColor = dayColor;

            currentDayState = TIME.DAY;
        }

        if ((int)TIME.AFTERNOON <= currentHour && (int)TIME.NIGHT > currentHour)
        {
            startColor = dayColor;
            endColor = afternoonColor;

            currentDayState = TIME.AFTERNOON;
        }


        if (currentDayState != prevState)
        {
            timeElapsed = 0;
            changing = true;
        }
    }




}
