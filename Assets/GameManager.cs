using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private static Dictionary<string,Player> players = new Dictionary<string, Player>();

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
		GUILayout.BeginArea(new Rect(200,200,200, 500));
		GUILayout.BeginVertical();

		foreach(KeyValuePair<string,Player> player in players)
		{
			GUILayout.Label (player.Key + " - " + player.Value.currentHealth);
		}

		GUILayout.EndVertical();

		GUILayout.EndArea();
	}
}
