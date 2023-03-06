using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text selectedIndexText;

    [SerializeField] TMP_Text woodResourcesText;
    [SerializeField] TMP_Text stoneResourcesText;
    [SerializeField] TMP_Text sandResourcesText;
    [SerializeField] TMP_Text foodResourcesText;

    [SerializeField] TMP_Text peopleAmountText;

    [SerializeField] TMP_Text timeModifierText;
    [SerializeField] Slider timeSlider;

    public bool showingMenu = false;

    private void Awake() => GeneralUtil.Ui = this;


    public void SetSelIndexText(string buildName) => selectedIndexText.text = $"Selected building {buildName}";
    public void SetFoodResText(int val) => foodResourcesText.text = $"Food: {val}";
    public void SetSandResText(int val) => sandResourcesText.text = $"Sand: {val}";
    public void SetStoneResText(int val) => stoneResourcesText.text = $"Stone {val}";
    public void SetWoodResText(int val) => woodResourcesText.text = $"Wood: {val}";
    public void SetPeepAmoungText(int val) => peopleAmountText.text = $"People Amount: {val}";

    private void Update()
    {
        if (!showingMenu) 
        {
            var val = Mathf.Round(timeSlider.value * 100f) / 100f;

            if (val >= 0.8 && val <= 1.2)
                val = 1;

            Time.timeScale = val;

            timeSlider.value = Time.timeScale;
            timeModifierText.text = $"Time Modifier: {Time.timeScale}";
        }
    }


    void OnGUI()
    {
        if (showingMenu)
        {
            // Set up the GUI skin
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            // Define the menu dimensions
            float menuWidth = 200f;
            float menuHeight = 150f;
            float menuX = Screen.width / 2 - menuWidth / 2;
            float menuY = Screen.height / 2 - menuHeight / 2;

            // Draw the menu background
            GUI.Box(new Rect(menuX, menuY, menuWidth, menuHeight), "Menu");

            // Draw the menu options
            float optionY = menuY + 50f;
            float buttonWidth = 100f;
            float buttonHeight = 30f;

            if (GUI.Button(new Rect(menuX + menuWidth / 2 - buttonWidth / 2, optionY, buttonWidth, buttonHeight), "Option 1"))
            {
                // Handle option 1
                Debug.Log("Option 1 selected");
            }

            optionY += buttonHeight + 10f;

            if (GUI.Button(new Rect(menuX + menuWidth / 2 - buttonWidth / 2, optionY, buttonWidth, buttonHeight), "Option 2"))
            {
                // Handle option 2
                Debug.Log("Option 2 selected");
            }
        }
    }


}
