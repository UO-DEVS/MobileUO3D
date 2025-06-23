using System.Collections.Generic;
using UnityEngine;
using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Utility.Platforms;

[CreateAssetMenu(fileName = "Assistant Debugger", menuName = "UO/Plugins/Assistant/Assistant Debugger Plugin")]
public class DebugAssistantPlugin : AssistantPlugin
{
	//DESCRIPTION
	const string DESCRIPTION = "Logs to the console every time the plugin is triggered.";
	[Header("DESCRIPTION")]
	[SerializeField, TextArea(1, 6)] internal string _pluginDescription = DESCRIPTION;
	
	//ON ACTIVATED
	internal override void OnActivated(PlayerMobile activePlayer) { Log(name, nameof(OnActivated)); }
	
	//MAP CHANGED
	internal override void OnMapChanged(World world, int newMap) { Log(name, nameof(OnMapChanged)); }
	//MESSAGE
	internal override void OnMessage(World world, string message) { Log(name, nameof(OnMessage)); }
	//HEALTH
	internal override void OnHealthUpdate(World world) { Log(name, nameof(OnHealthUpdate)); }
	//MANA
	internal override void OnManaUpdate(World world) { Log(name, nameof(OnManaUpdate)); }
	//STAMINA
	internal override void OnStaminaUpdate(World world) { Log(name, nameof(OnStaminaUpdate)); }
	//ADD MULTI
	internal override void OnAddMulti(World world, ushort graphic, ushort x, ushort y) { Log(name, nameof(OnAddMulti)); }
}
