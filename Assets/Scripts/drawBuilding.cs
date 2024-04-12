using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawBuilding : MonoBehaviour
{
    MyClass.Building building;

    private void Start()
    {
        building = gameObject.GetComponent<building>().buildingInfo;
        draw(building.filePath);
    }

    void draw(string modelFile) //List<Building> buildings, 
    {
        building.ReadInp(modelFile);//read .inp file
        //buildings.Add(building);//to transmit the information

        building.DrawBuilding();//create the buildings

        Debug.Log(modelFile + " created.");
    }
}

