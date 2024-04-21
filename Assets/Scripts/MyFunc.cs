using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

public class MyFunc : MonoBehaviour
{
    public GameObject cam;

    public static DataTable OpenCSV(string filePath)
    {
        DataTable dt = new DataTable();
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                string strLine = "";// record every line 
                string[] aryLine = null;// record every word
                string[] tableHead = null;// record the head of one column
                int columnCount = 0;// mark count of column
                bool IsFirst = true;// judge if this row is head
                //read each row
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (IsFirst)// name of each column
                    {
                        tableHead = strLine.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;
                        for (int i = 0; i < columnCount; i++)// create each column
                        {
                            if (tableHead[i] != null)
                            {
                                DataColumn dc = new DataColumn(tableHead[i]);
                                dt.Columns.Add(dc);
                            }
                            else { break; }// in case of empty cell at the last of .csv
                        }
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < columnCount; i++)
                        {
                            dr[i] = aryLine[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                
                sr.Close();
                fs.Close();
                return dt;
            }
        }
    }
    
}
