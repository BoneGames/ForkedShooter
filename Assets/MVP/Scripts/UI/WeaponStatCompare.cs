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
    public bool isComparing = false;
    List<GameObject> textObjects = new List<GameObject>();
    LayoutElement layout;

    private void Awake()
    {
        layout = GetComponent<LayoutElement>();
    }

    public void ShowStatComparison(UniqueWeaponStats _pickupStats, UniqueWeaponStats _currentStats)
    {
        // Create dictionary with: key(variable name) and Value 0: current stat, Value 1: pickup stat
        Dictionary<string, List<float>> weaponStatsCollated = CalculateStats(_pickupStats, _currentStats);

        // Iterate through dictionary to fill instantiated text objects with correct display text
        string text = "";
        foreach (KeyValuePair<string, List<float>> statPair in weaponStatsCollated)
        {
            //GameObject textField = Instantiate(textPrefab, transform.position, Quaternion.identity);
            //textField.transform.SetParent(this.transform);
            //Text t = textField.GetComponent<Text>();
            //t.text = statPair.Key + ": " + statPair.Value[0] + ", " + statPair.Value[1];
            //textObjects.Add(textField);
            text += statPair.Key + ": " + statPair.Value[0] + ", " + statPair.Value[1] + "\n";
        }
        GameObject textField = Instantiate(textPrefab, transform.position, Quaternion.identity);
        textField.transform.SetParent(this.transform);
        Text t = textField.GetComponent<Text>();
        t.text = text;
        //t.GetComponent<RectTransform>().rect.height = Screen.height;
        //t.GetComponent<RectTransform>().rect.width = Screen.width;
        //layout.cellSize = new Vector2(Screen.width, Screen.height);
        layout.preferredHeight = Screen.height;
        layout.preferredWidth = Screen.width;

        isComparing = true;
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
