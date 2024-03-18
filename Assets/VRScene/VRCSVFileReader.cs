using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable] public class PrefabSpecification
{
   [Inspectable] public string word = "null";
   [Inspectable] public GameObject Prefab; 
}
public class VRCSVFileReader : MonoBehaviour
{
    [Range(1, 10)] public float GridSpacing =5 ;
    public TextAsset csvFile; // Reference to your .csv file (drag and drop it in the Unity inspector)
    public string[,] dataArray;

    public Dictionary<string, List<string>> subLists = new Dictionary<string, List<string>>();
    public Dictionary<string, Tuple<int, int>> subListPositions = new Dictionary<string, Tuple<int, int>>();
    public List<string> mainList = new List<string>();

    [Inspectable] public List<PrefabSpecification> PrefabSpecifications = new List<PrefabSpecification>();
    public bool LoadMap = false, UnloadMap = false; 
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnValidate()
    {
        if (this.transform.childCount > 0)
        {
            LoadMap = false;
        }
        if (LoadMap == true)
        {
            LoadMapFromCSV();
            LoadMap = false;
        }
        if(UnloadMap ==true)
        {
            for (int i=0; i<this.transform.childCount; i++)
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            }
            UnloadMap = false;
        }
    }
    private void dothing()
    {
        
    }
    public void StoreStrings(string inputString, int row, int column)
    {
        if (inputString.Contains(";"))
        {
            string[] substrings = inputString.Split(';');
            //Debug.Log(substrings[0] + " | " + substrings[1]);
            if (substrings.Length >= 2)
            {
                string mainItem = substrings[0];
                string subItem = substrings[1];

                // Store in main list
                if (!mainList.Contains(mainItem))
                {
                    mainList.Add(mainItem);
                }

                // Store in sublist
                if (!subLists.ContainsKey(mainItem))
                {
                    subLists[mainItem] = new List<string>();
                }
                if (!subLists[mainItem].Contains(subItem))
                {
                    subLists[mainItem].Add(subItem);
                    subListPositions[subItem] = Tuple.Create(row, column);

                }
            }
            else
            {
                // Handle invalid input
                Console.WriteLine("Invalid input string format.");
            }
        }

    }
    void LoadMapFromCSV()
    {
        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split('\n');
           // Debug.Log(lines.Length);
            int numRows = lines.Length -1 ; // for some reason, converting from excel to CSV adds an extra line ***************************************** may have to subtract 1 or not
            int numCols = lines[0].Split(',').Length;
            Debug.Log("Total number of Rows: " + numRows);
            Debug.Log("Total number of Columns: " + numCols);
            //gridManager.rows = numRows;
            //gridManager.columns = numCols;


            dataArray = new string[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                string[] row = lines[i].Split(',');

                for (int j = 0; j < numCols; j++)
                {
                    dataArray[i, j] = row[j];
                   // gridManager.SetText(i, j, dataArray[i, j]); //gets rows and colums from csv file
                    if (dataArray[i, j].Contains("*"))
                    {
                       
                        //make a list of audioclip locations, track the grid locations that have the audio source
                        string[] substring = dataArray[i, j].Split("*");
                        //Debug.Log(substring[0]);
                         Debug.Log(substring[1]);
                        
                    }
                   // Debug.Log(i + " " + j + " = " + dataArray[i, j]);
                    int ObjIndex = LoadItemIndex(dataArray[i, j]);
                   // Debug.Log(ObjIndex+" "+PrefabSpecifications.Count);
                    GameObject TempObj;

                    if (ObjIndex <= PrefabSpecifications.Count)
                    {
                        TempObj = GameObject.Instantiate(PrefabSpecifications[ObjIndex].Prefab, new Vector3(i * GridSpacing, 0f, j * GridSpacing), PrefabSpecifications[ObjIndex].Prefab.transform.rotation, transform);
                        //TempObj.transform.parent = this.transform; 
                    }

                    //find if theres an asterisk, 
                    //AppendIfNotExists(dataArray[i, j], i, j);
                    StoreStrings(dataArray[i, j], i, j);
                }
            }
        }
    }
    private int LoadItemIndex(string text)
    {
        for (int i=0; i< PrefabSpecifications.Count; i++)
        {
            if (PrefabSpecifications[i].word == text)
                return i;
        }
        return PrefabSpecifications.Count+1; 
    }
}
