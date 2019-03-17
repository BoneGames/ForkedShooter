using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private static Dictionary<string,Player> players = new Dictionary<string, Player>();
	private GUIStyle guiStyle = new GUIStyle();

	public static void RegisterPlayer(string _netID, Player _player)
	{
		string _playerID = "Player " + _netID;
		players.Add(_playerID, _player);
		_player.transform.name = _playerID;
	}

	public static void UnRegisterPlayer(string _playerID)
	{
		players.Remove(_playerID);
	}

	public static Player GetPlayer(string _playerID)
	{
		return players[_playerID];
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(100,300,400, 1000));
		GUILayout.BeginVertical();

		guiStyle.normal.textColor = Color.red;
		guiStyle.fontSize = 50;

		foreach(KeyValuePair<string,Player> player in players)
		{
			GUILayout.Label (player.Key + " - " + player.Value.currentHealth, guiStyle);
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
