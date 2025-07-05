using System;
using UnityEngine;
using ClassicUO.Game.Managers;

[System.Serializable]
internal struct UOMacroHandler
{
	public HotkeyAction macro;
	public Action action;
	
	public UOMacroHandler(HotkeyAction newMacro, Action newAction)
	{
		macro = newMacro;
		action = newAction;
	}
}
