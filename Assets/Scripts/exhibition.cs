using System;
using UnityEngine;
using UnityEngine.UI;

public class exhibition : MonoBehaviour
{
    public void Exhibition()
    {
        int count = gameObject.transform.childCount;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        Vector2 anchor = rect.anchorMax;
        GameObject background = gameObject.transform.GetChild(0).gameObject;
        GameObject botton = gameObject.transform.GetChild(count - 1).gameObject;
        if (background.activeInHierarchy == true)
        {
            for (int i = 0; i < count - 1; i++) { gameObject.transform.GetChild(i).gameObject.SetActive(false); }
            //RectTransform rect = gameObject.GetComponent<RectTransform>();

            rect.anchorMax = new Vector2(0.0105f, anchor.y);
            RectTransform btnRect = botton.GetComponent<RectTransform>();

            btnRect.anchorMax = new Vector2(1f, 0.55f);
            btnRect.anchorMin = new Vector2(0f, 0.13f);
            //btnRect.pivot = new Vector2(0.5f, 1f);
            botton.transform.GetComponentInChildren<Text>().text = ">";
        }
        else
        {
            for (int i = 0; i < count - 1; i++) { gameObject.transform.GetChild(i).gameObject.SetActive(true); }
            //RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0.3f, anchor.y);
            RectTransform btnRect = botton.GetComponent<RectTransform>();
            btnRect.anchorMax = new Vector2(1f, 0.55f);
            btnRect.anchorMin = new Vector2(0.965f, 0.13f);
            botton.transform.GetComponentInChildren<Text>().text = "<";
        }
    }
}
