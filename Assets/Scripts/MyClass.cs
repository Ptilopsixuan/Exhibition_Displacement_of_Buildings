using System;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class MyClass : MonoBehaviour
{
    public class Building
    {
        public string name;
        public string filePath;
        public Material material;
        public GameObject original;
        public Vector3[] pos;
        public Vector2[] conn;
        public Vector3[] s3r;
        public Vector4[] s4r;
        public List<GameObject> childrenConn = new List<GameObject>();
        public List<GameObject> childrenS3R = new List<GameObject>();
        public List<GameObject> childrenS4R = new List<GameObject>();
        public DataTable displacement;//one column for one timestep, one row for one node

        public static List<Building> buildings = new List<Building>();

        public Building(string name, GameObject original, string filePath, Material material)
        {
            this.name = name;
            this.original = original;
            this.filePath = filePath;
            this.material = material;
        }

        public void ReadInp(string filePath)
        {
            //judge different data
            bool nodeBegin = false;
            bool connBegin = false;
            bool s4rBegin = false;
            bool s3rBegin = false;

            //store different data
            DataTable nodeDt = new DataTable();
            for (int i = 0; i < 3; i++) { nodeDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable connDt = new DataTable();
            for (int i = 0; i < 2; i++) { connDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable s3rDt = new DataTable();
            for (int i = 0; i < 3; i++) { s3rDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable s4rDt = new DataTable();
            for (int i = 0; i < 4; i++) { s4rDt.Columns.Add(new DataColumn(i.ToString())); }

            Regex IsStop = new Regex("^\\*.*");//to stop
            Regex IsNode = new Regex("\\*Node$");
            Regex IsConn = new Regex(".*=B3.*");//to find connection
            Regex IsS3R = new Regex(".*=S3R");//to find 3 points' wall
            Regex IsS4R = new Regex(".*=S4R");//to find 4 points' wall

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string strLine = "";//record words of every line
                    string[] aryLine = null;//trans data
                    int n = 0;
                    while ((strLine = sr.ReadLine()) != null)//collect corresponding data
                    {

                        if (IsStop.IsMatch(strLine)) { nodeBegin = false; connBegin = false; s3rBegin = false; s4rBegin = false; }
                        if (IsNode.IsMatch(strLine)) { nodeBegin = true; n++; continue; }//print("n1" + n); 
                        if (IsConn.IsMatch(strLine)) { connBegin = true; n++; continue; }//print("n2" + n);
                        if (IsS3R.IsMatch(strLine)) { s3rBegin = true; n++; continue; }//print("n3" + n);
                        if (IsS4R.IsMatch(strLine)) { s4rBegin = true; n++; continue; }//print("n4" + n); 

                        if (nodeBegin)
                        {
                            aryLine = strLine.Split(',');
                            str2dt(nodeDt, aryLine, 3);
                        }
                        else if (connBegin)
                        {
                            aryLine = strLine.Split(',');
                            str2dt(connDt, aryLine, 2);
                        }
                        else if (s3rBegin)
                        {
                            aryLine = strLine.Split(',');
                            str2dt(s3rDt, aryLine, 3);
                        }
                        else if (s4rBegin)
                        {
                            aryLine = strLine.Split(',');
                            str2dt(s4rDt, aryLine, 4);
                        }
                        n++;
                    }
                    fs.Close();
                }
            }

            //(pos, _) = dt2pos(nodeDt);
            pos = dt2vec(nodeDt, 3).Select(v => new Vector3(v.x / 10000, v.z / 10000, v.y / 10000)).ToArray();//y is height in Unity, while z is height in .inp
            conn = dt2vec(connDt, 2).Select(v => new Vector2(v.x, v.y)).ToArray();
            s3r = dt2vec(s3rDt, 3).Select(v => new Vector3(v.x, v.y, v.z)).ToArray();
            s4r = dt2vec(s4rDt, 4);
        }

        public void ReadTxt(string filePath)
        {
            DataTable nodeDt = new DataTable();
            for (int i = 0; i < 3; i++) { nodeDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable connDt = new DataTable();
            for (int i = 0; i < 2; i++) { connDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable s3rDt = new DataTable();
            for (int i = 0; i < 3; i++) { s3rDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable s4rDt = new DataTable();
            for (int i = 0; i < 4; i++) { s4rDt.Columns.Add(new DataColumn(i.ToString())); }

            read(filePath + "/Node.txt", nodeDt, 3);
            read(filePath + "/Conn.txt", connDt, 2);
            read(filePath + "/S3R.txt", s3rDt, 3);
            read(filePath + "/S4R.txt", s4rDt, 4);

            pos = dt2vec(nodeDt, 3).Select(v => new Vector3(v.x / 10000, v.z / 10000, v.y / 10000)).ToArray();//y is height in Unity, while z is height in .inp
            conn = dt2vec(connDt, 2).Select(v => new Vector2(v.x, v.y)).ToArray();
            s3r = dt2vec(s3rDt, 3).Select(v => new Vector3(v.x, v.y, v.z)).ToArray();
            s4r = dt2vec(s4rDt, 4);
        }

        public void DrawBuilding()
        {
            //draw the points of .inp
            //for (int i = 0; i < building.pos.Length; i++)//iterate each Node
            //{
            //    GameObject point = new GameObject(building.name + i);
            //    point.transform.position = building.pos[i];
            //    point.name = building.name + i;//name each copied Node
            //    point.transform.parent = building.original.transform;//set points' parent object in order to control
            //    point.tag = "point";
            //}
            if (conn != null)
            {
                for (int i = 0; i < conn.Length; i++)
                //foreach (Vector2 conn in building.connect) //iterate each connection
                {
                    int node1Index = (int)conn[i][0];
                    int node2Index = (int)conn[i][1];
                    Vector3 pos1 = pos[node1Index];
                    Vector3 pos2 = pos[node2Index];
                    Vector3[] vec = new Vector3[2] { pos1, pos2 };
                    GameObject connection = createConn(vec, material, "Con" + i, "line");
                    connection.transform.parent = original.transform;
                    childrenConn.Add(connection);
                }
            }
            if (s3r != null)
            {
                for (int i = 0; i < s3r.Length; i++)//iterate each s3r
                {
                    int node1Index = (int)s3r[i][0];
                    int node2Index = (int)s3r[i][1];
                    int node3Index = (int)s3r[i][2];
                    Vector3 pos1 = pos[node1Index];
                    Vector3 pos2 = pos[node2Index];
                    Vector3 pos3 = pos[node3Index];
                    Vector3[] vec = new Vector3[3] { pos1, pos2, pos3 };
                    GameObject shell3 = createS3R(vec, material, "S3R" + i, "shell3");
                    shell3.transform.parent = original.transform;
                    childrenS3R.Add(shell3);
                }
            }
            if (s4r != null)
            {
                for (int i = 0; i < s4r.Length; i++)//iterate each s4r
                {
                    int node1Index = (int)s4r[i][0];
                    int node2Index = (int)s4r[i][1];
                    int node3Index = (int)s4r[i][2];
                    int node4Index = (int)s4r[i][3];
                    Vector3 pos1 = pos[node1Index];
                    Vector3 pos2 = pos[node2Index];
                    Vector3 pos3 = pos[node3Index];
                    Vector3 pos4 = pos[node4Index];
                    Vector3[] vec = new Vector3[4] { pos1, pos2, pos3, pos4 };
                    GameObject shell4 = createS4R(vec, material, "S4R" + i, "shell4");
                    shell4.transform.parent = original.transform;
                    childrenS4R.Add(shell4);
                }
            }
        }

        private void read(string filePath, DataTable dt, int n)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string strLine = "";//record words of every line
                    string[] aryLine = null;//trans data
                    while ((strLine = sr.ReadLine()) != null)//collect corresponding data
                    {
                        aryLine = strLine.Split(',');
                        str2dt(dt, aryLine, n);
                    }
                }
                fs.Close();
            }
        }

        private void str2dt(DataTable dt, string[] aryLine, int n)
        {
            DataRow dr = dt.NewRow();
            for (int i = 0; i < n; i++)
            {
                dr[i] = aryLine[i + 1];
            }
            dt.Rows.Add(dr);
        }

        private Vector4[] dt2vec(DataTable dt, int n)
        {
            int count = dt.Rows.Count;
            Vector4[] vec = new Vector4[count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    vec[i][j] = Convert.ToSingle(dt.Rows[i][j]) - 1;// first node in .inp is 1, while in code is 0.
                }
            }
            return vec;
        }

        private GameObject createConn(Vector3[] vec, Material material, string name, string tag)
        {
            GameObject go = new GameObject(name);
            go.tag = tag;
            LineRenderer line = go.AddComponent<LineRenderer>();

            line.material = material;
            line.startWidth = 0.03f;
            line.endWidth = 0.03f;
            line.positionCount = 2;
            line.SetPositions(vec);

            return go;
        }

        private GameObject createS3R(Vector3[] vec, Material material, string name, string tag)
        {
            GameObject go = new GameObject(name);
            go.tag = tag;
            Vector3[] vertices = vec;
            int[] tris = new int[3] { 0, 2, 1 };
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.RecalculateNormals();

            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();

            meshRenderer.material = material;
            meshFilter.mesh = mesh;

            return go;
        }

        private GameObject createS4R(Vector3[] vec, Material material, string name, string tag)
        {
            GameObject go = new GameObject(name);
            go.tag = tag;
            Vector3[] vertices = vec;
            int[] tris = new int[6] { 0, 2, 1, 3, 2, 0 };
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.RecalculateNormals();

            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();

            meshRenderer.material = material;
            meshFilter.mesh = mesh;

            return go;
        }

        //private static (Vector3[], int) dt2pos(DataTable dt)
        //{
        //    int n = dt.Rows.Count;
        //    Vector3[] vec = new Vector3[n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        float x = Convert.ToSingle(dt.Rows[i][0]) / 10000;
        //        float z = Convert.ToSingle(dt.Rows[i][1]) / 10000;
        //        float y = Convert.ToSingle(dt.Rows[i][2]) / 10000;
        //        vec[i] = new Vector3(x, y, z);
        //    }
        //    return (vec, n);
        //}
    }


    public class Graph
    {
        private RectTransform graphContainer = GameObject.Find("graphContainer").GetComponent<RectTransform>();
        public static List<float> fullList = new List<float>();
        public static Graph graph;

        public Graph() { graph = this; }

        public void Exhibition(int currentStep)
        {
            if (currentStep != 0)
            {
                setDefault(graphContainer.GetComponent<Transform>());
                List<float> currentList = fullList.Take(currentStep).ToList();
                ShowGraph(currentList);
            }
        }

        private void ShowGraph(List<float> valueList)
        {
            float graphWidth = graphContainer.sizeDelta.x;
            float graphHeight = graphContainer.sizeDelta.y;
            float yMax = valueList.Max();
            float yMin = valueList.Min();
            float xSize = graphWidth / valueList.Count;

            GameObject lastCircleGameObject = null;
            for (int i = 0; i < valueList.Count; i++)
            {
                float xPosition = i * xSize;
                float yPosition = ((valueList[i] - yMin) / (yMax - yMin)) * graphHeight;
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
                if (lastCircleGameObject != null)
                {
                    CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                                        circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                }
                lastCircleGameObject = circleGameObject;
            }
        }

        private GameObject CreateCircle(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("circle", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            //gameObject.GetComponent<Image>().sprite = circleSprite;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(2, 2);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            return gameObject;
        }

        private void CreateDotConnection(Vector2 A, Vector2 B)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = new Color(256, 1, 0);
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Vector2 dir = (B - A).normalized;
            float distance = Vector2.Distance(A, B);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(distance, 2f);
            rectTransform.anchoredPosition = A + 0.5f * distance * dir;
            rectTransform.localEulerAngles = new Vector3(0, 0, (float)(Math.Atan(dir.y / dir.x) * 180 / Math.PI));
        }

        private void setDefault(Transform parent)
        {
            if (parent.childCount > 0)
            {
                for (int i = 0; i < parent.childCount; i++)
                { Destroy(parent.GetChild(i).gameObject); }
            }
        }
    }

    public class Message
    {
        private GameObject mainmenu = GameObject.Find("MainMenu");
        private GameObject imgMessage = new GameObject("imgMessage");
        private GameObject bgMessage = new GameObject("bgMessage", typeof(Image));
        private GameObject txtMessage = new GameObject("txtMessage");
        private GameObject btnConfirm = new GameObject("btnConfirm");
        private GameObject txtConfirm = new GameObject("txtConfirm");
        Font font = Resources.Load<Font>("DottedSongtiDiamondRegular");
        public static Message message;

        public Message(string content)
        {
            imgMessage.transform.SetParent(mainmenu.transform, false);
            RectTransform rectImgMessage = imgMessage.AddComponent<RectTransform>();
            rectImgMessage.anchorMin = new Vector2(.5f, .5f);
            rectImgMessage.anchorMax = new Vector2(.5f, .5f);
            rectImgMessage.sizeDelta = new Vector2(300, 100);

            bgMessage.transform.SetParent(imgMessage.transform, false);
            //bgMessage.AddComponent<Image>().color;
            bgMessage.GetComponent<Image>().color = new Color(0,1,1,.3f);
            RectTransform rectBgMessage = bgMessage.GetComponent<RectTransform>();
            rectBgMessage.anchorMin = new Vector2(0, 0);
            rectBgMessage.anchorMax = new Vector2(1, 1);
            rectBgMessage.offsetMin = new Vector2(0, 0);
            rectBgMessage.offsetMax = new Vector2(0, 0);
            //rectBgMessage.anchoredPosition

            txtMessage.transform.SetParent(imgMessage.transform, false);
            Text txtTxtMessage = txtMessage.AddComponent<Text>();
            txtTxtMessage.text = content;
            txtTxtMessage.color = Color.black;
            txtTxtMessage.font = font;
            txtTxtMessage.fontSize = 18;
            txtTxtMessage.alignment = TextAnchor.MiddleCenter;
            RectTransform rectTxtMessage = txtMessage.GetComponent<RectTransform>();
            rectTxtMessage.anchorMin = new Vector2(0, .3f);
            rectTxtMessage.anchorMax = new Vector2(1f, 1f);
            rectTxtMessage.offsetMin = new Vector2(0, 0);
            rectTxtMessage.offsetMax = new Vector2(0, 0);

            btnConfirm.transform.SetParent(imgMessage.transform, false);
            btnConfirm.AddComponent<Image>().color = Color.white;
            RectTransform rectBtnConfirm = btnConfirm.GetComponent<RectTransform>();
            rectBtnConfirm.anchorMin = new Vector2(.35f, 0f);
            rectBtnConfirm.anchorMax = new Vector2(.65f, .3f);
            rectBtnConfirm.anchoredPosition = new Vector2(.5f, 0f);
            rectBtnConfirm.offsetMin = new Vector2(0, 0);
            rectBtnConfirm.offsetMax = new Vector2(0, 0);
            btnConfirm.AddComponent<Button>().onClick.AddListener(Close);

            txtConfirm.transform.SetParent(btnConfirm.transform, false);
            Text txtTxtConfirm = txtConfirm.AddComponent<Text>();
            txtTxtConfirm.text = "Confirm";
            txtTxtConfirm.font = font;
            txtTxtConfirm.color = Color.black;
            txtTxtConfirm.alignment = TextAnchor.MiddleCenter;
            txtTxtConfirm.fontStyle = FontStyle.BoldAndItalic;
            txtTxtConfirm.resizeTextForBestFit = true;
            txtTxtConfirm.resizeTextMaxSize = 20;
            RectTransform rectTxtConfirm = txtConfirm.GetComponent<RectTransform>();
            rectTxtConfirm.anchorMin = Vector2.zero;
            rectTxtConfirm.anchorMax = Vector2.one;
            rectTxtConfirm.offsetMin = new Vector2(0, 0);
            rectTxtConfirm.offsetMax = new Vector2(0, 0);

            message = this;
        }

        public void ChangeTxt(string content)
        {
            txtMessage.GetComponent<Text>().text = content;
        }

        private void Close()
        {
            Destroy(imgMessage);
        }
    }
}