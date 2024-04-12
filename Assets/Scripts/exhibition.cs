using System;
using UnityEngine;
using UnityEngine.UI;

public class exhibition : MonoBehaviour
{
    public void Exhibition()
    {
        GameObject background = gameObject.transform.GetChild(0).gameObject;
        GameObject graphContainer = gameObject.transform.GetChild(1).gameObject;
        GameObject botton = gameObject.transform.GetChild(2).gameObject;
        if (graphContainer.activeInHierarchy == true)
        {
            graphContainer.SetActive(false); background.SetActive(false);
            RectTransform btnRect = botton.GetComponent<RectTransform>();
            btnRect.anchorMax = new Vector2(0.5f, 1f);
            btnRect.anchorMin = new Vector2(0.5f, 1f);
            btnRect.pivot = new Vector2(0.5f, 1f);
            botton.transform.GetComponentInChildren<Text>().text = "Exhibition";
        }
        else 
        { 
            graphContainer.SetActive(true); background.SetActive(true);
            RectTransform btnRect = botton.GetComponent<RectTransform>();
            btnRect.anchorMax = new Vector2(0.5f, 0f);
            btnRect.anchorMin = new Vector2(0.5f, 0f);
            btnRect.pivot = new Vector2(0.5f, 0f);
            botton.transform.GetComponentInChildren<Text>().text = "UnExhibition";
        }
    }
}
