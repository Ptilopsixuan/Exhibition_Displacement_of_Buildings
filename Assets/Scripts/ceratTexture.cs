using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ceratTexture : MonoBehaviour
{

    private int textureWidth = 128;
    private int textureHeight = 128;
    void Start()
    {
        GenerateCurveTexture();
    }

    void GenerateCurveTexture()
    {
        // ����һ���µ�2D����  
        Texture2D curveTexture = new Texture2D(textureWidth, textureHeight);

        // Ϊ���������ɫ����  
        Color[] pixels = new Color[textureWidth * textureHeight];

        // �����ﶨ��������ߺ���
        //for (int y = 0; y < textureHeight; y++)
        //{
        //    //double y = Math.Sin(x / textureWidth * Math.PI);
        //    //Debug.Log(x * textureHeight);
        //    pixels[y * textureWidth] = Color.blue;

        //    //pixels[(int)(x + y * textureHeight)] = Color.red;
        //}

        for (int x = 0; x < textureWidth; x++)
        {

            int res = x * textureWidth + x;
            try
            {
                pixels[res] = Color.red;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            Debug.Log("res: " + res);
        }
        // Ӧ���������ݵ�����  
        curveTexture.SetPixels(pixels);
        curveTexture.Apply();

        // ����Sprite������ΪImage�����sprite  
        Sprite curveSprite = Sprite.Create(curveTexture, new Rect(0, 0, curveTexture.width, curveTexture.height), new Vector2(0.5f, 0.5f));

        gameObject.GetComponent<Image>().sprite = curveSprite;
    }
}
