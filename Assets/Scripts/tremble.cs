using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tremble : MonoBehaviour
{
    List<MyClass.Building> buildings = MyClass.Building.buildings;

    public void TrembleTogether()
    {
        StartCoroutine(trembleTogether(buildings));
        //setDefault();
    }

    IEnumerator trembleTogether(List<MyClass.Building> buildings) 
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].original.GetComponent<oneTremble>().Simulation();
           
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    void setDefault() 
    {
        GameObject[] conns = GameObject.FindGameObjectsWithTag("line");
        for (int i = 0; i < conns.Length; i++)//literate the connections
        {
            LineRenderer line = conns[i].GetComponent<LineRenderer>();
            line.GetComponent<LineRenderer>().material.color = Color.blue;
        }
        GameObject[] shell3s = GameObject.FindGameObjectsWithTag("shell3");
        for (int i = 0; i < shell3s.Length; i++)//literate the shears
        {
            shell3s[i].GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        GameObject[] shell4s = GameObject.FindGameObjectsWithTag("shell4");
        for (int i = 0; i < shell4s.Length; i++)//literate the shears
        {
            shell4s[i].GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        Debug.Log("Already back to original style.");
    }
}