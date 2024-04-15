using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class createParent : MonoBehaviour
{
    public Material preMaterial;
    FileInfo[] modelFiles = new DirectoryInfo(Application.streamingAssetsPath + "/model").GetFiles();//get all files' names

    public void CreateParents()
    {
        GameObject area = new GameObject("area");
        StartCoroutine(createParents(area));
    }

    IEnumerator createParents(GameObject area)
    {
        Debug.Log("Start to create models.");
        for (int i = 0; i < modelFiles.Length; i++)//iterate the file
        {
            if (modelFiles[i].Extension == ".inp" && File.Exists(modelFiles[i].FullName))
            {
                string name = Path.GetFileNameWithoutExtension(modelFiles[i].FullName);
                GameObject original = new GameObject(name);
                MyClass.Building building = new MyClass.Building(name, original, modelFiles[i].FullName, preMaterial);//create a building with name and parent GameObject
                MyClass.Building.buildings.Add(building);

                original.AddComponent<building>().buildingInfo = building;
                original.AddComponent<drawBuilding>();
                original.AddComponent<oneTremble>();
                original.transform.parent = area.transform;

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
