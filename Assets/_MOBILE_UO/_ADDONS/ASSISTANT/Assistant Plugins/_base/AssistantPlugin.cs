using System.Collections.Generic;
using UnityEngine;
using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Utility.Platforms;

//[CreateAssetMenu(fileName = "New Assistant Plugin", menuName = "UO/Plugins/Assistant Plugin")]
public class AssistantPlugin : ScriptableObject//, IAssistantPlugin<World>
{
	//ACTIVATED
	bool _activated = false;
	public bool activated => _activated;
	
	[Header("ATTACHED PLUGINS")]
	//PLUGINS
	[SerializeField] List<AssistantPlugin> _plugins = new List<AssistantPlugin>();
	public List<AssistantPlugin> plugins => _plugins;
	
	//CHARACTER
	[SerializeField] CharacterStatus _character;
	public CharacterStatus character => _character;
	
	//UPDATE CHARACTER
	internal CharacterStatus UpdateCharacter(PlayerMobile playerToUpdate)
	{
		return _character = new CharacterStatus(playerToUpdate);
	}
	
	//ACTIVATE
	internal virtual void Activate(World gameworld)
	{
		//#if DEBUG && UNITY_EDITOR //START DEBUG
		if (_activated) Debug.Log("ASSISTANT PLUGIN: Reactivated " + name);
		else Debug.Log("ASSISTANT PLUGIN: Activated " + name);
		//#endif //END DEBUG
		
		_activated = true;
		UpdateCharacter(gameworld.Player);
		OnActivated(gameworld.Player);
		
		if (plugins != null && plugins.Count > 0)
		{
			foreach (AssistantPlugin plugin in plugins)
			{
				Debug.Log("<b>LOADING PLUGIN</b>: " + plugin.name);
				plugin.Activate(gameworld);
			}
		}
	}
	//ON ACTIVATED
	internal virtual void OnActivated(PlayerMobile activePlayer)
	{
		Debug.Log(nameof(OnActivated));
		
		//TRIGGER PLUGINS
		foreach (AssistantPlugin plugin in plugins)
		{
			plugin.OnActivated(activePlayer);
		}
	}
	
	//MAP CHANGED
	internal void MapChanged(World gameworld, int newMap) => OnMapChanged(gameworld, newMap);
	internal virtual void OnMapChanged(World gameworld, int newMap)
	{
		Debug.Log(nameof(OnMapChanged));
		
		//TRIGGER PLUGINS
		foreach (AssistantPlugin plugin in plugins)
		{
			plugin.OnMapChanged(gameworld, newMap);
		}
	}
	//MESSAGE
	internal void Message(World gameworld, string msg) => OnMessage(gameworld, msg);
	internal virtual void OnMessage(World gameworld, string message)
	{
		Debug.Log(nameof(OnMessage));
		
		//TRIGGER PLUGINS
		foreach (AssistantPlugin plugin in plugins)
		{
			plugin.OnMessage(gameworld, message);
		}
	}
	
	// S T A T S
	
	//HEALTH
	internal void HealthUpdate(World gameworld)
	{
		OnHealthUpdate(gameworld);
		UpdateCharacter(gameworld.Player);
		
		TriggerHealthUpdatePlugins(gameworld);
	}
	internal virtual void OnHealthUpdate(World gameworld) { }
	internal void TriggerHealthUpdatePlugins(World gameworld)
	{
		//TRIGGER PLUGINS
		foreach (AssistantPlugin plugin in plugins)
		{
			plugin.OnHealthUpdate(gameworld);
			plugin.UpdateCharacter(gameworld.Player);
		}
	}
	//MANA
	internal void ManaUpdate(World gameworld)
	{
		OnManaUpdate(gameworld);
		TriggerManaUpdatePlugins(gameworld);
		UpdateCharacter(gameworld.Player);
	}
	internal virtual void OnManaUpdate(World gameworld) { }
	internal void TriggerManaUpdatePlugins(World gameworld)
	{
		//TRIGGER PLUGINS
		foreach (AssistantPlugin plugin in plugins)
		{
			plugin.OnManaUpdate(gameworld);
			plugin.UpdateCharacter(gameworld.Player);
		}
	}
	//STAMINA
	internal void StaminaUpdate(World gameworld)
	{
		OnStaminaUpdate(gameworld);
		TriggerStaminaUpdatePlugins(gameworld);
		UpdateCharacter(gameworld.Player);
	}
	internal virtual void OnStaminaUpdate(World gameworld) { }
	internal void TriggerStaminaUpdatePlugins(World gameworld)
	{
		//TRIGGER PLUGINS
		foreach (AssistantPlugin plugin in plugins)
		{
			plugin.OnStaminaUpdate(gameworld);
			plugin.UpdateCharacter(gameworld.Player);
		}
	}
	
	
	//ADD MULTI
	internal void AddMulti(World gameworld, ushort graphic, ushort x, ushort y) => OnAddMulti(gameworld, graphic, x, y);
	internal virtual void OnAddMulti(World gameworld, ushort graphic, ushort x, ushort y)
	{
		Debug.Log(nameof(OnAddMulti));
		
		//TRIGGER PLUGINS
		foreach (AssistantPlugin plugin in plugins)
		{
			plugin.OnAddMulti(gameworld, graphic, x, y);
		}
	}
		
	//LOG
	internal void Log(string message)
	{
		Debug.Log(message);
	}
	internal void LogIssue(string message)
	{
		Debug.LogWarning("<color=red>ISSUE</color>: " + message);
	}
	internal void Log(string header, string message, string color = "white")
	{
		Log("<color=" + color + ">" + header + "</color>: " + message);
	}
}
