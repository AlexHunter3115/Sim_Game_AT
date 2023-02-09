using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text selectedIndexText;

    [SerializeField] TMP_Text woodResourcesText;
    [SerializeField] TMP_Text stoneResourcesText;
    [SerializeField] TMP_Text sandResourcesText;
    [SerializeField] TMP_Text foodResourcesText;

    [SerializeField] TMP_Text peopleAmountText;

    private void Awake() => GeneralUtil.TrialTest(this);

    public void SetSelIndexText(string buildName) => selectedIndexText.text = $"Selected building {buildName}";
    public void SetFoodResText(int val) => foodResourcesText.text = $"Food: {val}";
    public void SetSandResText(int val) => sandResourcesText.text = $"Sand: {val}";
    public void SetStoneResText(int val) => stoneResourcesText.text = $"Stone {val}";
    public void SetWoodResText(int val) => woodResourcesText.text = $"Wood: {val}";
    public void SetPeepAmoungText(int val) => peopleAmountText.text = $"People Amount: {val}";

}
