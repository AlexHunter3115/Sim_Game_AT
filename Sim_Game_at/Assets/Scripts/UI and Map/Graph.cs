using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Graph : MonoBehaviour
{
    [SerializeField] RectTransform graphContainer;
    public Transform MainGraph;
    [SerializeField] private GameObject dotRedSprite;
    [SerializeField] private GameObject dotBlueSprite;
    [SerializeField] private int resolutionGraph;
    [SerializeField] private float LineWidth = 2;
    [SerializeField] private GameObject graphBackground;

    public TMP_Text graphTwoString;
    public TMP_Text graphOneString;

    public TMP_Text maxRedVal;
    public TMP_Text higherRedVal;
    public TMP_Text lowerRedVal;
    public TMP_Text zeroRedVal;

    public TMP_Text maxBlueVal;
    public TMP_Text higherBlueVal;
    public TMP_Text lowerBlueVal;
    public TMP_Text zeroBlueVal;

    Vector2[] corners = new Vector2[4] { new Vector2(5,5), new Vector2(5, 90), new Vector2(230, 90), new Vector2(230, 5) };

    private int maxValueBlue;
    private int maxValueRed;

    // corners[0] is the bottom-left corner
    // corners[1] is the top-left corner
    // corners[2] is the top-right corner
    // corners[3] is the bottom-right corner

    private void Start()
    {
        MainGraph.gameObject.SetActive(false);
        GeneralUtil.graphRef = this;
    }
    //the graph si completely broken

    /// <summary>
    /// first one is blue other one is red
    /// </summary>
    /// <param name="LineOne"></param>
    /// <param name="LineTwo"></param>
    public void DrawGraph(List<int> LineOne, List<int> LineTwo) 
    {
        DeleteAllChildren(graphContainer.transform);

        var objRef = Instantiate(graphBackground,graphContainer.transform);


        if (LineOne.Count > 0) 
        {
            maxValueRed = LineOne.Max();
            ShowGraph(LineOne, true);
        }

        if (LineTwo.Count > 0) 
        {
            maxValueBlue = LineTwo.Max();
            ShowGraph(LineTwo, false);
        }

        SetUpText();
        objRef.transform.SetSiblingIndex(0);
    }

    private void SetUpText() 
    {
        maxRedVal.text = $"{maxValueRed} -";
        higherRedVal.text = $"{(int)Mathf.Lerp(0, maxValueRed, 0.66f)} -";
        lowerRedVal.text = $"{(int)Mathf.Lerp(0, maxValueRed,0.33f)} -";
        zeroRedVal.text = $"{0} -";

        maxBlueVal.text = $"- {maxValueBlue}";
        higherBlueVal.text = $"- {(int)Mathf.Lerp(0, maxValueBlue, 0.66f)}";
        lowerBlueVal.text = $"- {(int)Mathf.Lerp(0, maxValueBlue, 0.33f)}";
        zeroBlueVal.text = $"- {0}";
    }

    private void DeleteAllChildren(Transform transform) 
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform childTransform = transform.GetChild(i);
            Destroy(childTransform.gameObject);
        }
    }

    private GameObject CreateDot(Vector2 anchoredPos, bool red = false) 
    {
        GameObject objRef = null;

        if (red) 
        {
            objRef = Instantiate(dotRedSprite);
        }
        else 
        {
            objRef = Instantiate(dotBlueSprite);
        }

        objRef.transform.SetParent(graphContainer, false);

        RectTransform rectTransform = objRef.GetComponent<RectTransform>();

        rectTransform.anchoredPosition = anchoredPos;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return objRef;
    }

    private void ShowGraph(List<int> valueList, bool red) 
    {
        GameObject prevDot = null;

        for (int i = 0; i < resolutionGraph; i++)
        {
            if (i >= valueList.Count) 
            {

            }
            else 
            {
                float xPosition = Mathf.Lerp(corners[1].x, corners[2].x, i / 14.0f);

                float yPosition = Mathf.Lerp(corners[3].y, corners[2].y, (float)valueList[i] / (red == true ? maxValueRed : maxValueBlue));

                var dot = CreateDot(new Vector2(xPosition, yPosition),red);

                if (prevDot != null)
                {
                    CreateConnection(prevDot.GetComponent<RectTransform>().anchoredPosition, dot.GetComponent<RectTransform>().anchoredPosition,red);
                }
                prevDot = dot;
            }
        }
    }

    private void CreateConnection(Vector2 dotPosA, Vector2 dotPosB, bool red ) 
    {
        GameObject objRef = new GameObject("dot connection", typeof(Image));
        objRef.transform.SetParent(graphContainer, false);

        if (red)
            objRef.GetComponent<Image>().color = Color.red;
        else
            objRef.GetComponent<Image>().color = Color.green;

        Vector2 dir = (dotPosB - dotPosA).normalized;

        float distance = Vector2.Distance(dotPosA, dotPosB);

        objRef.transform.SetSiblingIndex(0);

        RectTransform rectTransform = objRef.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, LineWidth);

        rectTransform.anchoredPosition = dotPosA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GeneralUtil.GetAngleFromVectorFloat(dir));  
    }

}
