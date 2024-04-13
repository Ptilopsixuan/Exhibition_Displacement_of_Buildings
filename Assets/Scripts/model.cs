using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class model : MonoBehaviour
{
    public Material preMaterial;
    FileInfo[] modelFiles = new DirectoryInfo(Application.streamingAssetsPath + "/model").GetFiles();//get all files' names

    public void Create()
    {
        GameObject area = new GameObject("area");
        new MyClass.Message("Start");
        StartCoroutine(BuildModelOneByOne(area));
    }

    IEnumerator BuildModelOneByOne(GameObject area)
    {
        for (int i = 0; i < modelFiles.Length; i++)
        {
            if (modelFiles[i].Extension == ".inp" && File.Exists(modelFiles[i].FullName))
            {
                string name = Path.GetFileNameWithoutExtension(modelFiles[i].FullName);
                MyClass.Building building = new MyClass.Building(name, new GameObject(name), modelFiles[i].FullName, preMaterial);//create a building with name and parent GameObject
                MyClass.Building.buildings.Add(building);//to transmit the information

                building.ReadInp(modelFiles[i].FullName);//read .inp file
                building.DrawBuilding();//create the buildings
                building.original.transform.parent = area.transform;

                Debug.Log("Total number: " + modelFiles.Length + "\n" + i + ": " + modelFiles[i].Name + " created.");
                MyClass.Message.message.ChangeTxt("Total number: " + modelFiles.Length + "\n" + (i + 1) + ": " + modelFiles[i].Name + " created.");
            }
            yield return null;
        }
    }
}
