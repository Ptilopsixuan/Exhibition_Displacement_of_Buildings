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
        // 创建一个新的2D纹理  
        Texture2D curveTexture = new Texture2D(textureWidth, textureHeight);

        // 为纹理分配颜色数组  
        Color[] pixels = new Color[textureWidth * textureHeight];

        // 在这里定义你的曲线函数
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
        // 应用像素数据到纹理  
        curveTexture.SetPixels(pixels);
        curveTexture.Apply();

        // 创建Sprite并设置为Image组件的sprite  
        Sprite curveSprite = Sprite.Create(curveTexture, new Rect(0, 0, curveTexture.width, curveTexture.height), new Vector2(0.5f, 0.5f));

        gameObject.GetComponent<Image>().sprite = curveSprite;
    }
}
