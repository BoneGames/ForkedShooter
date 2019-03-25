using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boolTest : MonoBehaviour {

	public int boolInt; 
	private bool[] weaponsBools = new bool[3];
	public bool[] WeaponsBools{
		get{ return weaponsBools;}
		set
		{
			Debug.Log("1");
			weaponsBools = value;
			ShowBools();
		}
	}
	void ShowBools(){
		foreach(bool b in weaponsBools)
		{
			Debug.Log(b);
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			bool[] dummyBools = new bool[3];
			for(int bo = 0;bo <= dummyBools.Length -1; bo++)
			{
				if(bo == boolInt)
				{
					dummyBools[bo] = true;
				}
				else
				{
					dummyBools[bo] = false;
				}
			}
			WeaponsBools = dummyBools;
		}
	}


}
