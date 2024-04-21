using System;
using System.Collections;
using System.Data;
using UnityEngine;

public class earthquake : MonoBehaviour
{
    [SerializeField] GameObject graphContainer;
    public UnityEngine.UI.Slider amplificationSlider;
    float maxDisplacement = 500 / 100;//500 is nearly maxima in this displacement data

    DataTable displacement;

    static int currentStep = 0;
    static bool isMoving;

    static float UpdateInterval = 0.1f;

    public void StartEarthquakeSimulation()
    {
        currentStep = 0;
        isMoving = true;
        Gradient gradient = ColorBar();
        new MyClass.Graph(graphContainer.GetComponent<RectTransform>());
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
                building.UpdateBuilding(currentStep, maxDisplacement, gradient, amplificationSlider.value);
                displacement = building.displacement;
            }
            currentStep++;
            if (currentStep >= displacement.Columns.Count) { isMoving = false; }
            yield return new WaitForSeconds(interval);
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
        colorKeys[0].color = Color.blue;//new Color(100,149, 237);
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
