using System.Collections;
using System.IO;
using UnityEngine;

public class model : MonoBehaviour
{
    [SerializeField] GameObject MainCamera;
    public Material preMaterial;
    FileInfo[] modelFiles = new DirectoryInfo(Application.streamingAssetsPath + "/model").GetFiles();//get all files' names
    DirectoryInfo[] modelFolders = new DirectoryInfo(Application.streamingAssetsPath + "/model").GetDirectories();//get all files' names

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
            string name = Path.GetFileNameWithoutExtension(modelFiles[i].FullName);

            if (modelFiles[i].Extension == ".inp" && File.Exists(modelFiles[i].FullName))
            {
                MyClass.Building building = new MyClass.Building(name, new GameObject(name), modelFiles[i].FullName, preMaterial);//create a building with name and parent GameObject
                MyClass.Building.buildings.Add(building);//to transmit the information
                building.ReadInp(modelFiles[i].FullName);//read .inp file
                building.DrawBuilding();//create the buildings
                building.original.transform.parent = area.transform;

                Debug.Log("Total number: " + modelFiles.Length + i + ": " + modelFiles[i].Name + " created.");
                MyClass.Message.message.ChangeTxt("Total number: " + modelFiles.Length + "\n" + (i + 1) + ": " + modelFiles[i].Name + " created.");
            }

            //if (modelFolders.Length > 0)
            //{
            //    MyClass.Building building = new MyClass.Building(name, new GameObject(name), modelFolders[i].FullName, preMaterial);//create a building with name and parent GameObject
            //    MyClass.Building.buildings.Add(building);//to transmit the information
            //    building.ReadTxt(modelFolders[i].FullName);
            //    building.DrawBuilding();//create the buildings
            //    building.original.transform.parent = area.transform;

            //    MyClass.Message.message.ChangeTxt("Total number: " + modelFolders.Length + "\n" + (i + 1) + ": " + modelFolders[i].Name + " created.");
            //}

            if (i == modelFiles.Length - 1) 
            {
                MyClass.Message.message.ShowConfirm();
            }

            yield return null;
        }
    }
}
