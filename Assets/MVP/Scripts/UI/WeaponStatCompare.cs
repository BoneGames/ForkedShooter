using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class WeaponStatCompare : MonoBehaviour
{
    Dictionary<string, GameObject> statCompareField = new Dictionary<string, GameObject>();
    public GameObject textPrefab;
    Text compareText;
    public GameObject backdrop;
    public float cellScaler;
    float canvasScaler;
    public bool IsComparing
    {
        get
        {
            return isComparing;
        }
        set
        {
            if(value != isComparing)
            {
                isComparing = value;
                if(!isComparing)
                {
                    EnableCompareText(isComparing);
                }             
            }
        }
    }
    bool isComparing;
    public List<GameObject> textObjects = new List<GameObject>();
    GridLayoutGroup layout;

    private void Awake()
    {
        layout = GetComponent<GridLayoutGroup>();
        compareText = GetComponent<Text>();
        canvasScaler = GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x;
    }

    public void EnableCompareText(bool textActive)
    {
        backdrop.SetActive(false);
        foreach (var item in textObjects)
        {
            Destroy(item);
        }
        //compareText.enabled = textActive;
    }

    public void ShowStatComparison(UniqueWeaponStats _pickupStats, UniqueWeaponStats _currentStats)
    {
        IsComparing = true;
        backdrop.SetActive(true);
        textObjects.Clear();
        textObjects.TrimExcess();
        // Create dictionary with: key(variable name) and Value 0: current stat, Value 1: pickup stat
        Dictionary<string, List<float>> weaponStatsCollated = CalculateStats(_pickupStats, _currentStats);

        // Iterate through dictionary to fill instantiated text objects with correct display text

        //compareText.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 1/GetComponentInParent<Canvas>().transform.localScale.x, Screen.height * 1/GetComponentInParent<Canvas>().transform.localScale.y);
        NewText("", Color.black, true);
        NewText("Current", Color.black, false);
        NewText("Pickup", Color.black, false);
        Color val0, val1;
        foreach (KeyValuePair<string, List<float>> statPair in weaponStatsCollated)
        {
            if(statPair.Value[0] == statPair.Value[1])
            {
                val0 = Color.black;
                val1 = Color.black;
            }
            else if(statPair.Value[0] > statPair.Value[1])
            {
                val0 = Color.green;
                val1 = Color.red;
            }
            else
            {
                val0 = Color.red;
                val1 = Color.green;
            }

            // variable name
            NewText(statPair.Key + ":", Color.black, true);
            // current stats
            NewText(statPair.Value[0].ToString(), val0, false);
            // pickup stats
            NewText(statPair.Value[1].ToString(), val1, false);
        }
        Vector2 cellSize = new Vector2(Screen.width / layout.constraintCount, 
                                       Screen.height / (textObjects.Count / layout.constraintCount)) 
                                       * cellScaler * canvasScaler;

        layout.cellSize = cellSize;
        Vector2 cellDimensions = new Vector2(layout.constraintCount, (textObjects.Count / layout.constraintCount));
       
        backdrop.GetComponent<RectTransform>().sizeDelta = cellSize * cellDimensions;
    }

    // create and fill text component on GO
    void NewText(string textToDisplay, Color col, bool alignment)
    {
        // Instantiate text object, set parent
        GameObject textField = Instantiate(textPrefab, transform.position, Quaternion.identity);
        textField.transform.SetParent(this.transform);

        // Set Text text, color, and alignment
        Text t = textField.GetComponent<Text>();
        t.text = textToDisplay;
        t.color = col;
        t.alignment = alignment ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;

        // add text gameObject to list to be destroyed later (when not needed)
        textObjects.Add(textField);
    }

    string AddSpacesAndCapitalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                newText.Append(' ');
            newText.Append(text[i]);
        }
        string firstLetter = text.Substring(0, 1);
        firstLetter = firstLetter.ToUpper();
        string finalText = newText.ToString();
        finalText = finalText.Remove(0, 1);
        finalText = firstLetter + finalText;
        return finalText;
    }

    Dictionary<string, List<float>> CalculateStats(UniqueWeaponStats _pickupStats, UniqueWeaponStats _currentStats)
    {
        Dictionary<string, List<float>> statsToReturn = new Dictionary<string, List<float>>();

        // Get Array of all variable names in class
        var weaponVariableNames = _pickupStats.GetType()
                     .GetFields()
                     .Select(field => field.Name);

        foreach (var stat in weaponVariableNames)
        {
            // skip dictionary variable
            if(stat == "baseStats")
            {
                continue;
            }
            // dictionary Key
            string key = stat;

            key = AddSpacesAndCapitalize(key);
            // weapon stat base value (set in weapon inspector)
            float baseValue = _currentStats.baseStats[stat];
            
            // FieldInfo reference to variable multipler in pickup object
            var multiplier_pick = _pickupStats.GetType().GetField(stat);
            // multiplied value
            float finalValue_pick = baseValue * (float)multiplier_pick.GetValue(_pickupStats);

            // FieldInfo reference to variable multipler in current object
            var multiplier_curr = _currentStats.GetType().GetField(stat);
            // multiplied value
            float finalValue_curr = baseValue * (float)multiplier_curr.GetValue(_currentStats);

            if(key.Contains("max") || key.Contains("mag"))
            {
                finalValue_curr = (float)Math.Round((double)finalValue_curr);
                finalValue_pick = (float)Math.Round((double)finalValue_pick);
            }
            else
            {
                finalValue_curr = (float)Math.Round((double)finalValue_curr, 2);
                finalValue_pick = (float)Math.Round((double)finalValue_pick, 2);
            }

            // Add values to dictionary: Key, current val, pickup val
            statsToReturn[key] = new List<float> { finalValue_curr, finalValue_pick};
        }
        return statsToReturn;
    }
}
