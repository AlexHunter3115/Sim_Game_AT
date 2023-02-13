using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCycle : MonoBehaviour
{
    [Header("in seconds")]
    public float minuteCheck = 5;
    public float hourCheck = 25;
    public float dayCheck = 125;

    public float timerMultiplier = 1;

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
            yield return new WaitForSeconds(dayCheck/timerMultiplier);
        }
    }

    private IEnumerator HourCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(hourCheck / timerMultiplier);
        }
    }

    private IEnumerator MinuteCycleCorutine()
    {
        while (timeOn)
        {
            yield return new WaitForSeconds(minuteCheck / timerMultiplier);
        }
    }
}
