using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    [SerializeField] RectTransform graphContainer;
    [SerializeField] private GameObject dotRedSprite;
    [SerializeField] private GameObject dotBlueSprite;


    private void Start()
    {
       // CreateDot(new Vector2(10, 10));

        List<int> val =     new List<int>() { 23,56,1,7,3,2,34,65,4,13,87};

        ShowGraph(val);
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




    private void ShowGraph(List<int> valueList) 
    {
        float xSize = 25;
        float yMaximum = 200;
        float graphHeight = graphContainer.sizeDelta.y;

        GameObject prevDot = null;

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = i * xSize + 10;
            float yPosition = (valueList[i] / yMaximum) * graphHeight + 10;

            var dot = CreateDot(new Vector2(xPosition, yPosition));

            if (prevDot != null) 
            {
                Debug.Log(prevDot.GetComponent<RectTransform>().anchoredPosition);
                CreateConnection(prevDot.GetComponent<RectTransform>().anchoredPosition, dot.GetComponent<RectTransform>().anchoredPosition);
            }
            prevDot = dot;

        }
    }



    private void CreateConnection(Vector2 dotPosA, Vector2 dotPosB) 
    {
        GameObject objRef = new GameObject("dot connection", typeof(Image));
        objRef.transform.SetParent(graphContainer, false);

        objRef.GetComponent<Image>().color = Color.green;

        Vector2 dir = (dotPosB - dotPosA).normalized;

        float distance = Vector2.Distance(dotPosA, dotPosB);

        RectTransform rectTransform = objRef.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);

        rectTransform.anchoredPosition = dotPosA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GeneralUtil.GetAngleFromVectorFloat(dir));  
    }



}
