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

    public void ShowStatComparison(UniqueWeaponStats _pickupStats, UniqueWeaponStats _currentStats)
    {
        Debug.Log("raycast3");
        Dictionary<string, List<float>> pickupStats = CalculateStats(_pickupStats, _currentStats);
       // Dictionary<string, float> currentStats = CalculateStats(_currentStats);


        // Get Text Fields for 
        foreach (KeyValuePair<string, float> statField in _pickupStats.baseStats)
        {
            //for (int i = 0; i < length; i++)
            //{

            //}
            GameObject textField = Instantiate(textPrefab, transform.position, Quaternion.identity);
            //statCompareField.Add(textField);
        }
    }

    Dictionary<string, List<float>> CalculateStats(UniqueWeaponStats _pickupStats, UniqueWeaponStats _currentStats)
    {
        Debug.Log("raycast4");
        Dictionary<string, List<float>> statsToReturn = new Dictionary<string, List<float>>();

        // Get Array of all variable names in class
        var weaponVariableNames = _pickupStats.GetType()
                     .GetFields()
                     .Select(field => field.Name);

        foreach (var stat in weaponVariableNames)
        {
            string key = stat;

            float baseValue_pick = _pickupStats.baseStats[stat];
            var multiplier_pick = _pickupStats.GetType().GetField(stat);
            float finalValue_pick = baseValue_pick * (float)multiplier_pick.GetValue(this);

            float baseValue_curr = _currentStats.baseStats[stat];
            var multiplier_curr = _currentStats.GetType().GetField(stat);
            float finalValue_curr = baseValue_curr * (float)multiplier_curr.GetValue(this);


            statsToReturn[key] = new List<float> { finalValue_pick, finalValue_curr };
            Debug.Log(statsToReturn[key]);
        }
        return statsToReturn;
    }
}
