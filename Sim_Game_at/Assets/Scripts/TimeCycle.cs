using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCycle : MonoBehaviour
{
    public float minuteCheck = 4;
    public float hourCheck = 8;
    public float dayCheck = 12;

    private bool timeOn = true;   //deal with this later


    private delegate IEnumerator DailyCycleCorutineVar();
    private DailyCycleCorutineVar _updateDayCheck;


    private delegate IEnumerator MinuteCycleCorutineVar();
    private MinuteCycleCorutineVar _updateMinuteCheck;


    private delegate IEnumerator HourCycleCorutineVar();
    private HourCycleCorutineVar _updateHourCheck;


    private void Awake()
    {
        _updateMinuteCheck = MinuteCycleCorutine;
        _updateHourCheck = HourCycleCorutine;
        _updateDayCheck = DailyCycleCorutine;
    }

    private void Start()
    {
        StartCoroutine(_updateDayCheck());
        StartCoroutine(_updateHourCheck());
        StartCoroutine(_updateMinuteCheck());
    }

    private IEnumerator DailyCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(dayCheck);
            Debug.Log("This is a call for the daily check");
        }
    }

    private IEnumerator HourCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(hourCheck);
            Debug.Log("This is a call for the Hour check");
        }
    }

    private IEnumerator MinuteCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(minuteCheck);
            Debug.Log("This is a call for the minute check");
        }
    }
}
