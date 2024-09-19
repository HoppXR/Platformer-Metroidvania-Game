using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class HighScoreBoard : MonoBehaviour
{ 
    TMP_Text text;
    
    string filePath;

    // Constructor
    [Serializable]
    public class HighScoreEntry
    {
        public string name;
        public float time;
    }

    // Creates array list of high scores
    [Serializable]
    public class HighScoreData
    {
        public List<HighScoreEntry> entries;

        public void Sort()
        {
            entries = entries.OrderBy(entry => entry.time).ToList();
        }
    }

    HighScoreData data;
    
    void Start()
    {
        Debug.Log("persistance data path: " + Application.persistentDataPath);
        filePath = Application.persistentDataPath + "/highscores.json";

        text = GetComponent<TMP_Text>();
        
        Load();
        DisplayText();
    }

    // Temp data
    void CreateDummyData()
    {
        data = new HighScoreData();
        data.entries = new List<HighScoreEntry>
        {

        };
    }
    
    // Sorts the data before saving to the json file
    void Save()
    {
        data.Sort();
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    // Looks for existing data to load
    void Load()
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<HighScoreData>(json);
        }
        else
        {
            CreateDummyData();
            Save();
        }
    }

    // Displays the sorted data
    void DisplayText()
    {
        if (text == null)
        {
            return;
        }
        
        data.Sort();
        text.text = "";
        var i = 1;
        foreach (var entry in data.entries)
        {
            text.text += i + ".\t" + entry.name + "\t" + entry.time + "\n";
            i++;
        }
    }

    // Method can be called to add high scores
    public void AddHighScore(string name, float time)
    {
        var entry = new HighScoreEntry { name = name, time = time };
        data.entries.Add(entry);
        Save();
        DisplayText();
    }
}
