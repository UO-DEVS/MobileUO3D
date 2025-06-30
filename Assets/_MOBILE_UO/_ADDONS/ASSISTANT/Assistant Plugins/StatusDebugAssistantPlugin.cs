using System.Collections.Generic;
using UnityEngine;
using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Utility.Platforms;

[CreateAssetMenu(fileName = "Status Debugger Plugin", menuName = "UO/Plugins/Assistant/Status Debugger Plugin")]
public class StatusDebugAssistantPlugin : AssistantPlugin
{
	//DESCRIPTION
	const string DESCRIPTION = "Generates debug messages to track changes to health/mana/stamina/etc.";
	[Header("DESCRIPTION")]
	[SerializeField, TextArea(1, 6)] internal string _pluginDescription = DESCRIPTION;
	
	//ON ACTIVATED
	internal override void OnActivated(PlayerMobile activePlayer)
	{
		Log(name, nameof(OnActivated));
	}
	
	//MAP CHANGED
	internal override void OnMapChanged(World world, int newMap)
	{
		//Log(name, nameof(OnMapChanged));
	}
	//MESSAGE
	internal override void OnMessage(World world, string message)
	{
		//Log(name, nameof(OnMessage));
	}
	//HEALTH
	internal override void OnHealthUpdate(World gameworld)
	{
		//Log(name, nameof(OnHealthUpdate));
		int percentage = ClassicUO.Utility.MathHelper.PercetangeOf(gameworld.Player.Hits, gameworld.Player.HitsMax);
		percentage = percentage > 100 ? 100 : percentage < 0 ? 0 : percentage;
		
		ushort lastValue = character.health;
		ushort newValue = gameworld.Player.Hits;
		int changeAmount = newValue - lastValue;
		
		
		if (changeAmount != 0)
		{
			string priorValue = "BEFORE " + character.health + " (" + (character.healthFill * 100) + "%)";
			Log("HEALTH <color=" + (changeAmount > 0 ? "green" : "red") + ">" + changeAmount + "</color>"
			+ "\n" + gameworld.Player.Hits + "/" + gameworld.Player.HitsMax + " (" + percentage + "%)"
				+ "\n" + priorValue
			);
		}
		else
		{
			string before = "\nBEFORE " + character.health + " (" + (character.healthFill * 100) + "%)";
			Log(nameof(OnHealthUpdate) + " " + gameworld.Player.Hits + "/" + gameworld.Player.HitsMax + " (" + percentage + "%)" + before);
		}
		
	}
	//MANA
	internal override void OnManaUpdate(World gameworld)
	{
		//Log(name, nameof(OnManaUpdate));
		int percentage = ClassicUO.Utility.MathHelper.PercetangeOf(gameworld.Player.Mana, gameworld.Player.ManaMax);
		percentage = percentage > 100 ? 100 : percentage < 0 ? 0 : percentage;
		
		ushort lastValue = character.mana;
		ushort newValue = gameworld.Player.Mana;
		int changeAmount = newValue - lastValue;
		if (changeAmount != 0)
		{
			string priorValue = "BEFORE " + character.mana + " (" + (character.manaFill * 100) + "%)";
			Log("MANA <color=" + (changeAmount > 0 ? "cyan" : "purple") + ">" + changeAmount + "</color>"
				+ "\n" + gameworld.Player.Mana + "/" + gameworld.Player.ManaMax + " (" + percentage + "%)"
				+ "\n" + priorValue
			);
		}
		else
		{
			string before = "\nBEFORE " + character.mana + " (" + (character.manaFill * 100) + "%)";
			Log(nameof(OnManaUpdate) + " " + gameworld.Player.Mana + "/" + gameworld.Player.ManaMax + " (" + percentage + "%)" + before);
		}
		
	}
	//STAMINA
	internal override void OnStaminaUpdate(World gameworld)
	{
		//Log(name, nameof(OnStaminaUpdate));
		int percentage = ClassicUO.Utility.MathHelper.PercetangeOf(gameworld.Player.Stamina, gameworld.Player.StaminaMax);
		percentage = percentage > 100 ? 100 : percentage < 0 ? 0 : percentage;
		
		ushort lastValue = character.stamina;
		ushort newValue = gameworld.Player.Stamina;
		int changeAmount = newValue - lastValue;
		if (changeAmount != 0)
		{
			string priorValue = "BEFORE " + character.stamina + " (" + (character.staminaFill * 100) + "%)";
			
			Log("STAMINA <color=" + (changeAmount > 0 ? "yellow" : "orange") + ">" + changeAmount + "</color>"
				+ "\n" + gameworld.Player.Stamina + "/" + gameworld.Player.StaminaMax + " (" + percentage + "%)"
				+ "\n" + priorValue
			);
		}
		else
		{
			string before = "\nBEFORE " + character.stamina + " (" + (character.staminaFill * 100) + "%)";
			Log(nameof(OnStaminaUpdate) + " " + gameworld.Player.Stamina + "/" + gameworld.Player.StaminaMax + " (" + percentage + "%)" + before);
		}
		
	}
	//ADD MULTI
	internal override void OnAddMulti(World world, ushort graphic, ushort x, ushort y)
	{
		//Log(name, nameof(OnAddMulti));
	}
	/*
	//LOG
	void Log(string message)
	{
		Debug.Log(message);
	}
	void LogIssue(string message)
	{
		Debug.LogWarning("<color=red>ISSUE</color>: " + message);
	}
	void Log(string header, string message, string color = "white")
	{
		Log("<color=" + color + ">" + header + "</color>: " + message);
	}
	*/
}
