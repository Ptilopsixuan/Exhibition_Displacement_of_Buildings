using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;
using System.Linq;
using JetBrains.Annotations;

public class model : MonoBehaviour
{
    public Material preMaterial;
    public int seedOfEarthquake;

    string dataPath = Application.streamingAssetsPath;
    public static List<MyClass.Building> buildings = new List<MyClass.Building>();//store the buildings' information
    
    public void Create()
    {
        float startTime = Time.realtimeSinceStartup;//count time cost
        BuildModel();
        float endTime = Time.realtimeSinceStartup;
        Debug.Log("readInpTime: " + (endTime - startTime));
    }

    public void ReadDisplacement()
    {
        float startTime = Time.realtimeSinceStartup;
        getSeed(out seedOfEarthquake);
        LoadDisplacement(seedOfEarthquake, buildings);
        float endTime = Time.realtimeSinceStartup;
        Debug.Log("readCsvTime: " + (endTime - startTime));
    }

    void BuildModel()
    {
        //read model's points in .inp
        string modelPath = dataPath + "/model";//folder including models' information
        DirectoryInfo modelDir = new DirectoryInfo(modelPath);
        FileInfo[] modelFiles = modelDir.GetFiles();//get all files' names

        for (int i = 0; i < modelFiles.Length; i++)//iterate the file
        {
            //if (modelFiles[i].Name.EndsWith("inp"))//("meta")){ continue; }
            if (modelFiles[i].Extension == ".inp" && File.Exists(modelFiles[i].FullName))
            {
                string name = Path.GetFileNameWithoutExtension(modelFiles[i].FullName);
                MyClass.Building building = new MyClass.Building(name, new GameObject(name));//create a building with name and parent GameObject

                building.ReadInp(modelPath + "/" + modelFiles[i].Name);//read .inp file
                buildings.Add(building);//to transmit the information

                MyFunc.DrawBuilding(building, preMaterial);//create the buildings
                Debug.Log(i + ": " + modelFiles[i].Name + " created.");
            }
        }
    }

    void LoadDisplacement(int seedOfEarthquake, List<MyClass.Building> Buildings)
    {
        //read displacements
        string displacementPath = dataPath + "/" + seedOfEarthquake;//folder including displacements' information
        if (Directory.Exists(displacementPath))// if this seed of earthquake exists
        {
            FileInfo[] displacementFiles = new DirectoryInfo(displacementPath).GetFiles();//get all files' name

            bool[] buildingsWithDisplacement = new bool[Buildings.Count];
            for (int i = 0; i < displacementFiles.Length; i++)//iterate all buildings
            {
                //method 1
                bool additionalDis = true;
                for (int j = 0; j < Buildings.Count; j++)
                {
                    string displaceName = displacementFiles[i].Name;
                    string buildingName = Buildings[j].name + ".csv";
                    if (buildingName == displaceName)
                    {
                        DataTable dt = MyFunc.OpenCSV(displacementPath + "/" + displacementFiles[i].Name);
                        buildings[j].displacement = dt;
                        buildingsWithDisplacement[j] = true;
                        additionalDis = false;
                        Debug.Log(displacementFiles[i].Name + " has loaded displacement.");
                    }
                }
                if (additionalDis) { Debug.Log("Additional displacement: " + displacementFiles[i].Name); }

                ////method 2
                //int n = 0;
                //for (int j = 0; j < displacementFiles.Length; j++)//iterate the file
                //{
                //    if (displacementFiles[j].Name.EndsWith("meta"))
                //    {
                //        n++;
                //    }
                //    if (displacementFiles[j].Name.EndsWith("csv"))//("meta")){ continue; }
                //    {
                //        DataTable dt = MyFunc.OpenCSV(displacementPath + "/" + displacementFiles[j].Name);//load path
                //        buildings[j - n].displacement = dt;//if the displacement files' sequence is the same as the model files'

                //        Debug.Log(j + ": " + displacementFiles[j].Name);
                //    }
                //}
            }
            for (int i = 0; i < buildingsWithDisplacement.Length; i++)
            {
                if (!buildingsWithDisplacement[i]) { Debug.Log(Buildings[i].name + " dont got displacement."); }
            }
        }
        else { Debug.Log("Invalid Seed"); return; }
    }

    void getSeed(out int seedOfEarthquake)
    {
        seedOfEarthquake = 0;
        GameObject inputSeed = GameObject.FindGameObjectWithTag("input");
        if (inputSeed != null)
        {
            try
            {
                seedOfEarthquake = int.Parse(inputSeed.GetComponent<InputField>().text);
            }
            catch(Exception e) 
            {
                Debug.Log("please input an integer. "+ e);
            }
        }
    }
}
