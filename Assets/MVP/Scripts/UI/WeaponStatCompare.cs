using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WeaponStatCompare : MonoBehaviour
{
    Dictionary<string, GameObject> statCompareField = new Dictionary<string, GameObject>();
    public GameObject textPrefab;
    Text compareText;
    public GameObject backdrop;
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
    LayoutElement layout;

    private void Awake()
    {
        layout = GetComponent<LayoutElement>();
        compareText = GetComponent<Text>();
    }

    public void EnableCompareText(bool textActive)
    {
        foreach (var item in textObjects)
        {
            Destroy(item);
        }
        //compareText.enabled = textActive;
    }

    public void ShowStatComparison(UniqueWeaponStats _pickupStats, UniqueWeaponStats _currentStats)
    {
        IsComparing = true;
        textObjects.Clear();
        textObjects.TrimExcess();
        // Create dictionary with: key(variable name) and Value 0: current stat, Value 1: pickup stat
        Dictionary<string, List<float>> weaponStatsCollated = CalculateStats(_pickupStats, _currentStats);

        // Iterate through dictionary to fill instantiated text objects with correct display text
        string text = "";
        //compareText.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 1/GetComponentInParent<Canvas>().transform.localScale.x, Screen.height * 1/GetComponentInParent<Canvas>().transform.localScale.y);
        NewText("", Color.black);
        NewText("Current", Color.black);
        NewText("Pickup", Color.black);
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
            NewText(statPair.Key + ":", Color.black);
            // current stats
            NewText(statPair.Value[0].ToString(), val0);
            // pickup stats
            NewText(statPair.Value[1].ToString(), val1);
        }
        GameObject _backdrop = Instantiate(backdrop, transform.position, Quaternion.identity);
        textObjects.Add(_backdrop);
        _backdrop.transform.SetParent(this.transform.parent);
        Vector2 bgSizeDelta = Vector2.zero;
        foreach (var item in textObjects)
        {
            Debug.Log("sizeDelta");
            bgSizeDelta += item.GetComponent<RectTransform>().sizeDelta;
        }
        
    }

    void NewText(string textToDisplay, Color col)
    {
        GameObject textField = Instantiate(textPrefab, transform.position, Quaternion.identity);
        textField.transform.SetParent(this.transform);
        Text t = textField.GetComponent<Text>();
        t.text = textToDisplay;
        
            t.color = col;

        textObjects.Add(textField);
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

            // Add values to dictionary: Key, current val, pickup val
            statsToReturn[key] = new List<float> { finalValue_curr, finalValue_pick};
           
        }
        return statsToReturn;
    }
}
