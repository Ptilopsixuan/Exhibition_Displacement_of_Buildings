using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class earthquake : MonoBehaviour
{
    public UnityEngine.UI.Slider amplificationSlider;
    float maxDisplacement = 500 / 100;//500 is nearly maxima in this displacement data

    DataTable displacement;

    static int currentStep = 0;
    static bool isMoving;

    static float UpdateInterval = 0.2f;

    public void StartEarthquakeSimulation()
    {
        currentStep = 0;
        isMoving = true;
        Gradient gradient = ColorBar();
        new MyClass.Graph();
        StartCoroutine(IterateSimulation(UpdateInterval, gradient));// repeat this function every set interval
    }

    private IEnumerator IterateSimulation(float interval, Gradient gradient)
    {
        while (isMoving)//Loop the time steps
        {
            print(1);
            MyClass.Graph.graph.Exhibition(currentStep);

            foreach (MyClass.Building building in MyClass.Building.buildings)//Loop the buildings
            {
                //try
                //{
                    displacement = building.displacement;
                    Vector3[] positions = building.pos;//maintain the original position
                    float amplification = amplificationSlider.value; // Get amplification value each iteration

                    //classify all the children GameObjects
                    List<GameObject> conns = building.childrenConn;
                    List<GameObject> shell3s = building.childrenS3R;
                    List<GameObject> shell4s = building.childrenS4R;

                    for (int i = 0; i < building.conn.Length; i++)//literate the connections
                    {
                        LineRenderer line = conns[i].GetComponent<LineRenderer>();
                        int node1Index = (int)building.conn[i][0];
                        int node2Index = (int)building.conn[i][1];
                        //devided by 100 to amplify displacements 100 times because of coordinates are devided by 1e4
                        float groundmove = Convert.ToSingle(displacement.Rows[0][currentStep]) / 100;
                        float move1 = Convert.ToSingle(displacement.Rows[node1Index][currentStep]) / 100;
                        float move2 = Convert.ToSingle(displacement.Rows[node2Index][currentStep]) / 100;
                        Vector3 pos1 = positions[node1Index];
                        Vector3 pos2 = positions[node2Index];
                        pos1.x += (move1 - groundmove) * amplification;
                        pos2.x += (move2 - groundmove) * amplification;
                        line.SetPositions(new Vector3[2] { pos1, pos2 });

                        float displacementRatio = Math.Abs(((move1 + move2) / 2) - groundmove) / maxDisplacement;

                        Color lineColor = gradient.Evaluate(displacementRatio);
                        line.GetComponent<LineRenderer>().material.color = lineColor;
                    }
                    //same as the line
                    for (int i = 0; i < building.s3r.Length; i++)//literate the shells
                    {
                        MeshRenderer Mesh = shell3s[i].GetComponent<MeshRenderer>();
                        int node1Index = (int)building.s3r[i][0];
                        int node2Index = (int)building.s3r[i][1];
                        int node3Index = (int)building.s3r[i][2];
                        float groundmove = Convert.ToSingle(displacement.Rows[0][currentStep]) / 100;
                        float move1 = Convert.ToSingle(displacement.Rows[node1Index][currentStep]) / 100;
                        float move2 = Convert.ToSingle(displacement.Rows[node2Index][currentStep]) / 100;
                        float move3 = Convert.ToSingle(displacement.Rows[node3Index][currentStep]) / 100;
                        Vector3 pos1 = positions[node1Index];
                        Vector3 pos2 = positions[node2Index];
                        Vector3 pos3 = positions[node3Index];
                        pos1.x += (move1 - groundmove) * amplification;
                        pos2.x += (move2 - groundmove) * amplification;
                        pos3.x += (move3 - groundmove) * amplification;
                        shell3s[i].GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { pos1, pos2, pos3 };

                        float displacementRatio = Math.Abs(((move1 + move2 + move3) / 3) - groundmove) / maxDisplacement;

                        Color meshColor = gradient.Evaluate(displacementRatio);
                        Mesh.material.color = meshColor;
                    }
                    for (int i = 0; i < building.s4r.Length; i++)//literate the shears
                    {
                        MeshRenderer Mesh = shell4s[i].GetComponent<MeshRenderer>();
                        int node1Index = (int)building.s4r[i][0];
                        int node2Index = (int)building.s4r[i][1];
                        int node3Index = (int)building.s4r[i][2];
                        int node4Index = (int)building.s4r[i][3];
                        float groundmove = Convert.ToSingle(displacement.Rows[0][currentStep]) / 100;
                        float move1 = Convert.ToSingle(displacement.Rows[node1Index][currentStep]) / 100;
                        float move2 = Convert.ToSingle(displacement.Rows[node2Index][currentStep]) / 100;
                        float move3 = Convert.ToSingle(displacement.Rows[node3Index][currentStep]) / 100;
                        float move4 = Convert.ToSingle(displacement.Rows[node4Index][currentStep]) / 100;
                        Vector3 pos1 = positions[node1Index];
                        Vector3 pos2 = positions[node2Index];
                        Vector3 pos3 = positions[node3Index];
                        Vector3 pos4 = positions[node4Index];
                        pos1.x += (move1 - groundmove) * amplification;
                        pos2.x += (move2 - groundmove) * amplification;
                        pos3.x += (move3 - groundmove) * amplification;
                        pos4.x += (move4 - groundmove) * amplification;
                        shell4s[i].GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { pos1, pos2, pos3, pos4 };

                        float displacementRatio = Math.Abs(((move1 + move2 + move3 + move4) / 4) - groundmove) / maxDisplacement;

                        Color meshColor = gradient.Evaluate(displacementRatio);
                        Mesh.material.color = meshColor;
                    }

                //}
                //catch { continue; }
            }
            currentStep++;
            if (currentStep >= displacement.Columns.Count) { isMoving = false; }
            yield return new WaitForSecondsRealtime(interval);
        }

        //recover to the original
        if (!isMoving)
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

    private Gradient ColorBar()
    {
        // Gradient gradient; // Define your gradient object
        Gradient gradient = new Gradient();

        // Define color stops
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        colorKeys[0].color = Color.blue;
        colorKeys[0].time = 0.0f;
        colorKeys[1].color = Color.green;
        colorKeys[1].time = 0.0025f;
        colorKeys[2].color = Color.yellow;
        colorKeys[2].time = 0.01f;
        colorKeys[3].color = Color.red;
        colorKeys[3].time = 0.015f;

        // Define alpha keys (optional)
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1.0f;
        alphaKeys[0].time = 0.0f;
        alphaKeys[1].alpha = 0.0f;
        alphaKeys[1].time = 1.0f; // Fade out at the end

        gradient.SetKeys(colorKeys, alphaKeys);
        return gradient;
    }
}
