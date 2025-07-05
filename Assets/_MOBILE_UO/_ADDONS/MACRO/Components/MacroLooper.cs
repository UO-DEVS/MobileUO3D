using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static SDL2.SDL;
//using Assistant;
using ClassicUO.Input;
using ClassicUO.Network;
//using ClassicUO.Game;
using ClassicUO.Game.Scenes;
using ClassicUO.Game.Managers;
//using ClassicUO.Game.Data;

public class MacroLooper : MonoBehaviour
{
	const int MIN = 100;
	const int MAX = 1000000;
	
	[Header("COMPONENT LINKS")]
	//TARGET SELF INDICATOR
	[SerializeField] Transform _targetSelfIndicator;
	public void UpdateTargetSelfIndicator()
	{
		if (_targetSelfIndicator) _targetSelfIndicator.gameObject.SetActive(_targetSelf);
	}
	//TARGET LAST INDICATOR
	[SerializeField] Transform _targetLastIndicator;
	public void UpdateTargetLastIndicator()
	{
		if (_targetLastIndicator) _targetLastIndicator.gameObject.SetActive(_targetLast);
	}
	//LOOP INDICATOR
	[SerializeField] Transform _loopIndicator;
	public void UpdateLoopIndicator()
	{
		if (_loopIndicator) _loopIndicator.gameObject.SetActive(_loop);
	}
	[Header("MACRO LABEL")]
	//MACRO LABEL
	[SerializeField] TMPro.TMP_Text _macroLabel;
	public void UpdateMacroLabel()
	{
		string macroLabelText = string.Empty;
		
		bool hasSkill = (int)_skill != (int)UOActiveSkill.NoSkill;
		bool hasSpell = (int)_spell != (int)UOSpell.NoSpell;
		if (hasSkill) macroLabelText += _skill.ToString();
		if (hasSkill && hasSpell) macroLabelText += " ";
		if (hasSpell) macroLabelText += _spell.ToString();
		
		if (_macroLabel) _macroLabel.SetText(macroLabelText);
	}
	
	[Header("_skills_")]
	//SKILL LABEL
	//[SerializeField] TMPro.TMP_Text _skillLabel;
	//public void UpdateSkillLabel()
	//{
	//	if (_skillLabel) _skillLabel.SetText(_skill.ToString());
	//}
	//SKILL CONFIG DROPDOWN
	[SerializeField] TMPro.TMP_Dropdown _skillDropdown;
	public void UpdateSkillDropdown()
	{
		if (_skillDropdown) _skillDropdown.value = (int)_skill;
	}
	
	[Header("_spells_")]
	//SPELL LABEL
	//[SerializeField] TMPro.TMP_Text _spellLabel;
	//public void UpdateSpellLabel()
	//{
	//	if (_spellLabel) _spellLabel.SetText(_spell.ToString());
	//}
	//SPELL CONFIG DROPDOWN
	[SerializeField] TMPro.TMP_Dropdown _spellDropdown;
	public void UpdateSpellDropdown()
	{
		if (_spellDropdown) _spellDropdown.value = (int)_spell;
	}
	
	[Header("_delay_")]
	//DELAY LABEL
	[SerializeField] TMPro.TMP_Text _delayLabel;
	public void UpdateDelayLabel()
	{
		if (_delayLabel) _delayLabel.SetText(_delay.ToString());
	}
	//DELAY CONFIG BOX
	[SerializeField] TMPro.TMP_InputField _delayBox;
	public void UpdateDelayTextBox()
	{
		if (_delayBox) _delayBox.text = _delay.ToString();
	}
	
	//SETTINGS PANEL
	[SerializeField] Transform _settingsPanel;
	public void ShowSettings() => ToggleSettings(true);
	public void HideSettings() => ToggleSettings(false);
	public void ToggleSettings() => ToggleSettings(!_settingsPanel.gameObject.active);
	
	//TOGGLE SETTINGS
	public void ToggleSettings(bool show)
	{
		if (_settingsPanel) _settingsPanel.gameObject.SetActive(show);
	}
	
	[Header("SETTINGS")]
	//LOOP
	[SerializeField] bool _loop = false;
	public void SetLoop(bool looping)
	{
		_loop = looping;
		UpdateLoopIndicator();
	}
	//TARGET SELF
	[SerializeField] bool _targetSelf = false;
	public void SetTargetSelf(bool targetingSelf)
	{
		_targetSelf = targetingSelf;
		UpdateTargetSelfIndicator();
	}
	//TARGET LAST
	[SerializeField] bool _targetLast = false;
	public void SetTargetLast(bool targetingLast)
	{
		_targetLast = targetingLast;
		UpdateTargetLastIndicator();
	}
	//KEYPRESS
	[SerializeField] KeyCode _key = KeyCode.None;
	//SKILL
	[SerializeField] UOActiveSkill _skill = UOActiveSkill.NoSkill;
	public void SetSkill(int id)
	{
		
		_skill = (UOActiveSkill)GetSkill(_skillDropdown.options[id].text);
		//_skill = (UOSkill)id;
		//UpdateSkillDropdown();
		UpdateMacroLabel();
	}
	//SPELL
	[SerializeField] UOSpell _spell = UOSpell.NoSpell;
	public void SetSpell(int id)
	{
		_spell = (UOSpell)GetSpell(_spellDropdown.options[id].text);
		//_spell = (UOSpell)id;
		//UpdateSpellDropdown();
		UpdateMacroLabel();
	}
	//DELAY
	[SerializeField, Range(MIN,MAX)] int _delay = 10000;
	public void SetDelay(string delayToSetString)
	{
		int val = 2500;
		if (int.TryParse(delayToSetString, out val))
		{
			SetDelay(val);
		} else Debug.LogWarning("<color=red>ISSUE</color>: " + "You must enter a number in this textbox..." + _delayBox.name);
	}
	public void SetDelay(int delayToSet)
	{
		_delay = delayToSet;
		UpdateDelayLabel();
		//if(delayBox != null) delayBox.text = delayToSet.ToString();
	}
	
	public SDL_Keymod KeymodOverride;
	
	bool running = false;
	uint iteration = 0;
	
	protected void OnValidate()
	{
		if (!_macroLabel) Debug.LogWarning("You must assign the macro label to " + name);
		if (!_delayLabel) Debug.LogWarning("You must assign the delay label to " + name);
		if (!_loopIndicator) Debug.LogWarning("You must assign the loop indicator to " + name);
		if (!_targetSelfIndicator) Debug.LogWarning("You must assign the target self indicator to " + name);
		if (!_targetLastIndicator) Debug.LogWarning("You must assign the target last indicator to " + name);
		if (!_delayBox) _delayBox = GetComponentInChildren<TMPro.TMP_InputField>();
		if (!_skillDropdown) Debug.LogWarning("You must assign the skill dropdown to " + name);
		if (!_spellDropdown) Debug.LogWarning("You must assign the spell dropdown to " + name);
		if (!_settingsPanel) Debug.LogWarning("You must assign the settings panel to " + name);
	}
	protected void Awake()
	{
		UpdateSkillDropdown();
		UpdateSpellDropdown();
		UpdateDelayTextBox();
		UpdateDelayLabel();
		UpdateMacroLabel();
		UpdateLoopIndicator();
		UpdateTargetSelfIndicator();
		UpdateTargetLastIndicator();
	}

	//SKILLS
	// Convert steam-compatible skill names to Skills
	internal static int GetSkill(string skillName)
	{
		UOSkill myskill = UOSkill.NoSkill;
		if (Enum.TryParse<UOSkill>(skillName, true, out myskill))
		{
			return (int)myskill;
		} else Debug.LogWarning("<color=red>ISSUE</color>: " + "Skill " + skillName + " was not found");

		return (int)UOSkill.NoSkill; 
	}
	void UseSkill(string skillName)
	{
		UseSkill(GetSkill(skillName));
	}
	void UseSkill(UOActiveSkill skillToUse)
	{
		if (skillToUse != UOActiveSkill.NoSkill) UseSkill((int)skillToUse);
	}
	void UseSkill(int id)
	{
		if (ClassicUO.Client.Game == null || ClassicUO.Client.Game.UO == null) return;
		
		Debug.Log("USING SKILL #" + id + " " + ((UOSkill)id).ToString());
		if (id != (int)UOSkill.NoSkill) ClassicUO.Game.GameActions.UseSkill(id);
		else Debug.LogWarning("<color=red>ISSUE</color>: " + "Skill " + id + " was not found");
	}
	
	//SPELLS
	internal static int GetSpell(string spellName)
	{
		UOSpell myspell = UOSpell.NoSpell;
		if (Enum.TryParse<UOSpell>(spellName, true, out myspell))
		{
			return (int)myspell;
		} else Debug.LogWarning("<color=red>ISSUE</color>: " + "Spell " + spellName + " was not found");

		return (int)UOSpell.NoSpell; 
	}
	void CastSpell(string skillName)
	{
		CastSpell(GetSkill(skillName));
	}
	void CastSpell(UOSpell skillToUse)
	{
		CastSpell((int)skillToUse);
	}
	void CastSpell(int id)
	{
		if (ClassicUO.Client.Game == null || ClassicUO.Client.Game.UO == null) return;
		
		Debug.Log("USING SPELL #" + id + " " + ((UOSpell)id).ToString());
		if (id != (int)UOSpell.NoSpell) ClassicUO.Game.GameActions.CastSpell(id);
		else Debug.LogWarning("<color=red>ISSUE</color>: " + "Spell " + id + " was not found");
	}
	
	
	//PLAY
	public void Play()
	{
		running = !running;
		if (running)
		{
			Debug.Log("<color=green>MACRO ON</color>\n" + "|" + _key.ToString() + "|" + _skill.ToString() + "|");
		}
		else
		{
			Debug.Log("<color=grey>MACRO OFF</color>\n" + "|" + _key.ToString() + "|" + _skill.ToString() + "|");
			return;
		}
		iteration = 0;
		float time = _delay * 0.001f;
		ExecuteMacro(_key, time);
	}
	
	void ExecuteMacro(KeyCode _key, float _interval)
	{
		if (running) StartCoroutine(PlayMacro(_key, _interval));
	}
	
	//PLAY MACRO
	IEnumerator PlayMacro(KeyCode _key, float _interval)
	{
		while (running)
		{
			yield return new WaitForSeconds(_interval); //WAIT
		
			yield return new WaitForEndOfFrame();// WaitForSeconds(0.05f); //WAIT
			iteration++;
			Debug.Log("Delay: " + _interval + " Iteration: " + iteration + " TimeScale: " + Time.timeScale);
			if (_key != KeyCode.None) PressKey(_key, ClassicUO.Client.Game);
			if (_skill != UOActiveSkill.NoSkill) UseSkill(_skill);
			if (_spell != UOSpell.NoSpell) CastSpell(_spell);
			//PressKey(_key, ClassicUO.Client.Game);
			//ReleaseKey(_key, ClassicUO.Client.Game);
			if (_targetSelf || _targetLast)// && Assistant.UOSObjects.Player != null)
			{
				float delay = 0.25f;
				float elapsed = 0f;
				float duration = _interval;
				if (duration < 1f) duration = 1f;
				while (ClassicUO.Client.Game.UO.World.TargetManager.IsTargeting == false)
				{
					if (elapsed >= duration) break;
					//if (elapsed >= duration) continue;
					Debug.Log("Waiting for target...");
					yield return new WaitForSeconds(delay);
					elapsed += delay;
				}
				Debug.Log("TARGETING");
				//Assistant.UOSObjects.Player.SendMessage(Assistant.MsgLevel.Warning, "Targeting self...");
				//Assistant.Targeting.ClearQueue();
				//Assistant.Targeting.DoTargetSelf(true);
				
				if (_targetSelf) ClassicUO.Client.Game.UO.World.TargetManager.Target(ClassicUO.Client.Game.UO.World.Player);
				else if (_targetLast) ClassicUO.Client.Game.UO.World.TargetManager.TargetLast();
				//Assistant.Targeting.SetLastTargetTo(Assistant.UOSObjects.Player);
				//Assistant.Targeting.LastTarget(true);
				//Assistant.Targeting.TargetSelf(true);
			}
				/*
				if (UOSObjects.Player == null)
					return true;

				UOItem pack = UOSObjects.Player.Backpack;
				if (pack != null)
				{
					UOItem obj = pack.FindItemByID(3617);
					if (obj == null)
					{
						UOSObjects.Player.SendMessage(MsgLevel.Warning, "No bandages found");
					}
					else
					{
						Engine.Instance.SendToServer(new DoubleClick(obj.Serial));
						if (force)
						{
							Assistant.Targeting.ClearQueue();
							Assistant.Targeting.DoTargetSelf(true);
						}
						else
							Assistant.Targeting.TargetSelf(true);
				}*/
					
			if (!_loop) running = false;
		}
	}
	
	internal void PressKey(UnityEngine.KeyCode keyToPress, ClassicUO.GameController _game)
	{
		Debug.Log("KEY PRESS " + keyToPress);
		
		var modkeys = GetModKeys();
		var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) keyToPress, mod = modkeys}};
		    
		Keyboard.OnKeyDown(key);
		Keyboard.OnKeyUp(key);

		if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
		{
			if (_game != null) _game.IgnoreNextTextInput(false);
			
			if (UIManager.KeyboardFocusControl != null)
			{
				UIManager.KeyboardFocusControl.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				UIManager.KeyboardFocusControl.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			} else Debug.LogWarning("NO KEYBOARD FOCUS CONTROL");
			
			if (_game != null)
			{
				_game.Scene.OnKeyDown(key);
				_game.Scene.OnKeyUp(key);
			} else Debug.LogWarning("NO GAME");
			
			Plugin.ProcessHotkeys((int)keyToPress, (int)modkeys, false);
		}
		else if (_game != null) _game?.IgnoreNextTextInput(true);
	}
	    
	SDL2.SDL.SDL_Keymod GetModKeys()
	{
		//Keyboard handling
		var keymod = KeymodOverride;
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftAlt))
		{
			keymod |= SDL_Keymod.KMOD_LALT;
		}
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightAlt))
		{
			keymod |= SDL_Keymod.KMOD_RALT;
		}
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift))
		{
			keymod |= SDL_Keymod.KMOD_LSHIFT;
		}
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift))
		{
			keymod |= SDL_Keymod.KMOD_RSHIFT;
		}
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl))
		{
			keymod |= SDL_Keymod.KMOD_LCTRL;
		}
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightControl))
		{
			keymod |= SDL_Keymod.KMOD_RCTRL;
		}
		return keymod;
	}
}
	/*
	internal void PressKey(UnityEngine.KeyCode keyToPress, ClassicUO.GameController _game)
	{
		var modkeys = GetModKeys();
		var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) keyToPress, mod = modkeys}};
		    
		Keyboard.OnKeyDown(key);

		if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
		{
			_game?.IgnoreNextTextInput(false);
			UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
			_game?.Scene.OnKeyDown(key);
			Debug.Log("PLUGIN KEY PRESS");
		}
		else _game?.IgnoreNextTextInput(true);
	}
	internal void ReleaseKey(UnityEngine.KeyCode keyToPress, ClassicUO.GameController _game)
	{
		var modkeys = GetModKeys();
		var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) keyToPress, mod = modkeys}};

		Keyboard.OnKeyUp(key);
		
		UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
		_game?.Scene.OnKeyUp(key);
		Plugin.ProcessHotkeys((int)keyToPress, (int)modkeys, false);
		//Plugin.ProcessHotkeys(0, 0, false);
		Debug.Log("PLUGIN KEY RELEASE");
	}
	*/
	/*
	internal static Dictionary<string, int> SkillMap = new Dictionary<string, int>()
	{
		{ "alchemy", 0 },
		{ "anatomy", 1 },
		{ "animallore", 2 }, { "animal lore", 2 },
		{ "itemidentification", 3 }, {"itemid", 3 }, { "item identification", 3 }, { "item id", 3 },
		{ "armslore", 4 }, { "arms lore", 4 },
		{ "parry", 5 }, { "parrying", 5 },
		{ "begging", 6 },
		{ "blacksmith", 7 }, { "blacksmithing", 7 },
		{ "fletching", 8 }, { "bowcraft", 8 },
		{ "peacemaking", 9 }, { "peace", 9 }, { "peacemake", 9 },
		{ "camping", 10 }, { "camp", 10 },
		{ "carpentry", 11 },
		{ "cartography", 12 },
		{ "cooking", 13 }, { "cook", 13 },
		{ "detectinghidden", 14 }, { "detect", 14 }, { "detecthidden", 14 }, { "detecting hidden", 14 }, { "detect hidden", 14 },
		{ "discordance", 15 }, { "discord", 15 }, { "enticement", 15 }, { "entice", 15 },
		{ "evaluatingintelligence", 16 }, { "evalint", 16 }, { "eval", 16 }, { "evaluating intelligence", 16 },
		{ "healing", 17 },
		{ "fishing", 18 },
		{ "forensicevaluation", 19 }, { "forensiceval", 19 }, { "forensics", 19 },
		{ "herding", 20 },
		{ "hiding", 21 },
		{ "provocation", 22 }, { "provo", 22 },
		{ "inscription", 23 }, { "scribe", 23 },
		{ "lockpicking", 24 },
		{ "magery", 25 }, { "mage", 25 },
		{ "magicresist", 26 }, { "resist", 26 }, { "resistingspells", 26 },
		{ "tactics", 27 },
		{ "snooping", 28 }, { "snoop", 28 },
		{ "musicianship", 29 }, { "music", 29 },
		{ "poisoning", 30 },
		{ "archery", 31 },
		{ "spiritspeak", 32 },
		{ "stealing", 33 },
		{ "tailoring", 34 },
		{ "taming", 35 }, { "animaltaming", 35 }, { "animal taming", 35 },
		{ "tasteidentification", 36 }, { "tasteid", 36 },
		{ "tinkering", 37 },
		{ "tracking", 38 },
		{ "veterinary", 39 }, { "vet", 39 },
		{ "swords", 40 }, { "swordsmanship", 40 },
		{ "macing", 41 }, { "macefighting", 41 }, { "mace fighting", 41 },
		{ "fencing", 42 },
		{ "wrestling", 43 },
		{ "lumberjacking", 44 },
		{ "mining", 45 },
		{ "meditation", 46 },
		{ "stealth", 47 },
		{ "removetrap", 48 },
		{ "necromancy", 49 }, { "necro", 49 },
		{ "focus", 50 },
		{ "chivalry", 51 },
		{ "bushido", 52 },
		{ "ninjitsu", 53 },
		{ "herboristery", 54 }
	};*/
	
