using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using static System.Net.Mime.MediaTypeNames;
using UnityEditor.VersionControl;
using static UserUIManager;

public class UserUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text selectedIndexText;

    [Space(10)]
    [SerializeField] TMP_Text woodResourcesText;
    [SerializeField] TMP_Text stoneResourcesText;
    [SerializeField] TMP_Text sandResourcesText;
    [SerializeField] TMP_Text foodResourcesText;

    [Space(10)]
    [SerializeField] TMP_Text peopleAmountText;

    [Space(10)]
    [SerializeField] TMP_Text timeModifierText;
    [SerializeField] TMP_Text daysPassedText;
    [SerializeField] Slider timeSlider;
    [SerializeField] TMP_Text currentHourText;

    [Space(10)]
    [SerializeField] TMP_Text progressText;
    [SerializeField] Slider progressSlider;
    [SerializeField] GameObject progressObj;

    [Space(10)]
    [SerializeField] GameObject messageObj;
    private Queue<Messages> messages = new Queue<Messages>();
    private bool showingText = false;
    private GameObject currentMessage;



    public bool showingMenu = false;
    private bool prefSave = false;

    private void Awake() => GeneralUtil.Ui = this;


    public void SetSelIndexText(string buildName) => selectedIndexText.text = $"Selected building {buildName}";
    public void SetFoodResText(int currVal, int maxVal) => foodResourcesText.text = $"Food: {currVal}/{maxVal}";
    public void SetSandResText(int currVal, int maxVal) => sandResourcesText.text = $"Sand: {currVal}/{maxVal}";
    public void SetStoneResText(int currVal, int maxVal) => stoneResourcesText.text = $"Stone: {currVal}/{maxVal}";
    public void SetWoodResText(int currVal, int maxVal) => woodResourcesText.text = $"Wood: {currVal}/{maxVal}";
    public void SetPeopleAmountText(int val) => peopleAmountText.text = $"People Amount: {val}";
 
    public void SetDaysText(int val) => daysPassedText.text = $"Days passed: {val}";
    public void SetHoursText(int val) => currentHourText.text = $"Current Hour: {val}";

    public void SetProgressState(int val)
    {

        switch (val)
        {
            case 0:
                progressObj.SetActive(true);
                //respawn bushes
                progressText.text = "Spawning Some Bushes...";

                progressSlider.value = 0f;
                break;

            case 1:

                progressText.text = "Spawning Some Trees...";

                progressSlider.value = 0.25f;
                //tree spawn
                break;

            case 2:
                //poissant

                progressText.text = "Running CA Algo";

                progressSlider.value = 0.5f;
                break;

            case 3:

                progressText.text = "Checking Best Outcome";
                progressSlider.value = 0.75f;
                //daily needs
                break;

            case 4:
                progressObj.SetActive(false);
                break;

            default:
                break;
        }




    } 


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


        if (!showingText && messages.Count > 0)
        {
            showingText = true;
            var comp = messages.Dequeue();
            currentMessage = Instantiate(messageObj, transform);
            currentMessage.GetComponent<MessageUI>().CallSetMessage(comp.text, comp.color);
        }
    }

 

    void OnGUI()
    {
        if (showingMenu)
        {
            var databank = GeneralUtil.dataBank;

            float buttonWidth = 100;
            float buttonHeight = 30;
            float toggleWidth = 225;
            float toggleHeight = 30;
            float sliderWidth = 100;
            float sliderHeight = 30;
            float spacing = 10;

            Color backgroundColor = new Color(0.4f, 0.4f, 0.4f, 0.9f); // Dark gray color for the background box

            // Set GUI color to the background color
            GUI.color = backgroundColor;


            float totalWidth = buttonWidth + spacing + toggleWidth + spacing + sliderWidth * 5 + spacing * 3;
            float totalHeight = buttonHeight + spacing + sliderHeight;

            float x = (Screen.width - totalWidth) / 2;
            float y = (Screen.height - totalHeight) / 2;

            GUI.Box(new Rect(x, y, totalWidth, totalHeight + 20), "");

            GUI.color = Color.white;
            if (GUI.Button(new Rect(x + spacing, y + spacing, buttonWidth, buttonHeight), "Restart"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            GUI.tooltip = "Restart the game to get a new map";

            prefSave = GUI.Toggle(new Rect(x + spacing + buttonWidth + spacing, y + spacing, toggleWidth, toggleHeight), prefSave, "Randomize New Game");
            GUI.tooltip = "This project uses perlin noise to generate its map\nWould you like to randomize it?";

            PlayerPrefs.SetInt("RandomGen", prefSave ? 1 : 0);


            GUI.Label(new Rect(x + spacing, y + spacing * 2 + buttonHeight - 4, sliderWidth, sliderHeight + 4), new GUIContent() { text = "AI: division Organiser", tooltip = "Lower Value, Prioritize the percentage of total missing resources\nHigher Value, Prioritize the resource whihc was spent the most this round" });
            databank.importanceDivision = GUI.HorizontalSlider(new Rect(x + spacing, y + spacing * 3 + buttonHeight + 15, sliderWidth, sliderHeight), databank.importanceDivision, 0.2f, 0.8f);


            GUI.Label(new Rect(x + spacing * 2 + sliderWidth + 40, y + spacing * 2 + buttonHeight - 6, sliderWidth, sliderHeight + 4), new GUIContent() { text = "AI: importance food", tooltip = "test" }  );
            databank.importanceOfFood = GUI.HorizontalSlider(new Rect(x + spacing * 2 + sliderWidth + 40, y + spacing * 3 + buttonHeight + 15, sliderWidth, sliderHeight), databank.importanceOfFood, 0.7f, 1.3f);

            GUI.Label(new Rect(x + spacing * 3 + sliderWidth * 2 + 40, y + spacing * 2 + buttonHeight - 6, sliderWidth, sliderHeight + 4), new GUIContent() { text = "AI: importance stone", tooltip = "" }  );
            databank.importanceOfStone = GUI.HorizontalSlider(new Rect(x + spacing * 3 + sliderWidth * 2 + 40, y + spacing * 3 + buttonHeight + 15, sliderWidth, sliderHeight), databank.importanceOfStone, 0.7f, 1.3f);

            GUI.Label(new Rect(x + spacing * 4 + sliderWidth * 3 + 40, y + spacing * 2 + buttonHeight - 6, sliderWidth, sliderHeight + 4), new GUIContent() { text = "AI: importance sand", tooltip = "" });
            databank.importanceOfSand = GUI.HorizontalSlider(new Rect(x + spacing * 4 + sliderWidth * 3 + 40, y + spacing * 3 + buttonHeight + 15, sliderWidth, sliderHeight), databank.importanceOfSand, 0.7f, 1.3f);

            GUI.Label(new Rect(x + spacing * 5 + sliderWidth * 4 + 40, y + spacing * 2 + buttonHeight - 6, sliderWidth, sliderHeight + 4), new GUIContent() { text = "AI: importance wood", tooltip = "" });
             databank.importanceOfWood = GUI.HorizontalSlider(new Rect(x + spacing * 5 + sliderWidth * 4 + 40, y + spacing * 3 + buttonHeight + 15, sliderWidth, sliderHeight), databank.importanceOfWood, 0.7f, 1.3f);


            GUI.Label(new Rect(x + spacing * 5 + sliderWidth * 5 +5 + 80, y + spacing * 2 + buttonHeight - 6, sliderWidth, sliderHeight + 4), new GUIContent() { text = "AI: Threshold Space", tooltip = "" });
            databank.leftAreaThreashold = GUI.HorizontalSlider(new Rect(x + spacing * 6 + sliderWidth * 5 + 80, y + spacing * 3 + buttonHeight + 15, sliderWidth, sliderHeight), databank.leftAreaThreashold, 0.5f, 0.9f);
        }
    }


    public void SetMessage(string text, Color color) => messages.Enqueue(new Messages(text, color));

    public void SetDoneText()
    {
        Destroy(currentMessage);
        showingText = false;
    }



    public class Messages 
    {
        public string text;
        public Color color;

        public Messages(string text, Color color)
        {
            this.text = text;
            this.color = color;
        }
    }
}
