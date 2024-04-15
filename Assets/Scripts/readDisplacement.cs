using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class readDisplacement : MonoBehaviour
{
    public int seedOfEarthquake;

    public void ReadDisplacement()
    {
        float startTime = Time.realtimeSinceStartup;
        getSeed(out seedOfEarthquake);
        LoadDisplacement(seedOfEarthquake, MyClass.Building.buildings);

        DataTable groundmove = MyClass.Building.buildings[0].displacement;//need to optimize
        List<float>fullList = new List<float>();
        for (int i = 0; i < groundmove.Columns.Count; i++) 
        {
            fullList.Add(Convert.ToSingle(groundmove.Rows[0][i]) / 100);            //fullList.Add((float)groundmove.Rows[0][i] / 100);  //Error
        }
        MyClass.Graph.fullList = fullList;

        float endTime = Time.realtimeSinceStartup;
        Debug.Log("readCsvTime: " + (endTime - startTime));
    }

    void LoadDisplacement(int seedOfEarthquake, List<MyClass.Building> Buildings)
    {
        //read displacements
        string displacementPath = Application.streamingAssetsPath + "/" + seedOfEarthquake;//folder including displacements' information
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
                        Buildings[j].displacement = dt;
                        buildingsWithDisplacement[j] = true;
                        additionalDis = false;
                        Debug.Log(displacementFiles[i].Name + " has loaded displacement.");
                    }
                }
                if (additionalDis) { Debug.Log("Additional displacement: " + displacementFiles[i].Name); }
            }
            for (int i = 0; i < buildingsWithDisplacement.Length; i++)
            {
                if (!buildingsWithDisplacement[i]) { Debug.Log(Buildings[i].name + " dont got displacement."); }
            }
        }
        else { 
            
            Debug.Log("Invalid Seed"); return; }
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
            catch (Exception e)
            {
                Debug.Log("please input an integer. " + e);
            }
        }
    }
}
