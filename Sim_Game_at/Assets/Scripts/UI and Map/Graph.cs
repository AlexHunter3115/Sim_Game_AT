using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{

    [SerializeField] RectTransform graphContainer;
    public Transform MainGraph;
    [SerializeField] private GameObject dotRedSprite;
    [SerializeField] private GameObject dotBlueSprite;
    [SerializeField] private int resolutionGraph;
    [SerializeField] private float gapInPoints = 5;
    [SerializeField] private float LineWidth = 2;


    private void Start()
    {
        MainGraph.gameObject.SetActive(false);
        GeneralUtil.graphRef = this;
    }

    /// <summary>
    /// first one is blue other one is red
    /// </summary>
    /// <param name="LineOne"></param>
    /// <param name="LineTwo"></param>
    public void DrawGraph(List<int> LineOne, List<int> LineTwo) 
    {
        DeleteAllChildren(graphContainer.transform);

        ShowGraph(LineOne,true);
        ShowGraph(LineTwo,false);
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
        float xSize = 25;
        float yMaximum = 200;
        float graphHeight = graphContainer.sizeDelta.y;

        GameObject prevDot = null;

        for (int i = 0; i < resolutionGraph; i++)
        {
            if (i >= valueList.Count) 
            {
                Debug.Log("out of range");
            }
            else 
            {
                float xPosition = i * xSize + gapInPoints;
                float yPosition = (valueList[i] / yMaximum) * graphHeight + gapInPoints;

                var dot = CreateDot(new Vector2(xPosition, yPosition));

                if (prevDot != null)
                {
                    Debug.Log(prevDot.GetComponent<RectTransform>().anchoredPosition);
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
