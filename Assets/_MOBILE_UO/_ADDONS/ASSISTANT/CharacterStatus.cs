using UnityEngine;
using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Utility.Platforms;

[System.Serializable]
public struct CharacterStatus
{
	internal CharacterStatus(PlayerMobile activePlayer)
	:
	this(
	activePlayer.Hits,
	activePlayer.HitsMax,
	activePlayer.Mana,
	activePlayer.ManaMax,
	activePlayer.Stamina,
	activePlayer.StaminaMax
	){}
	
	public CharacterStatus
	(
		ushort newHealth, ushort newMaxHealth,
		ushort newMana, ushort newMaxMana,
		ushort newStamina, ushort newMaxStamina
	)
	{
		//SetName(newName);
		_hp = newHealth;
		_hpMax = newMaxHealth;
		_mp = newMana;
		_mpMax = newMaxMana;
		_ap = newStamina;
		_apMax = newMaxStamina;
	}
	
	//LIFE
	ushort _hp;
	ushort _hpMax;
	public ushort health => _hp;
	public float healthFill => ((float)_hp > 0f ? (float)_hp / (float)_hpMax : 0f);
	public void SetHealth(ushort newValue) { _hp = newValue; }
	public void SetMaxHealth(ushort newValue) { _hpMax = newValue; }
	//MANA
	ushort _mp;
	ushort _mpMax;
	public ushort mana => _mp;
	public float manaFill => ((float)_mp > 0f ? (float)_mp / (float)_mpMax : 0f);
	public void SetMana(ushort newValue) { _mp = newValue; }
	public void SetMaxMana(ushort newValue) { _mpMax = newValue; }
	//STAM
	ushort _ap;
	ushort _apMax;
	public ushort stamina => _ap;
	public float staminaFill => ((float)_ap > 0f ? (float)_ap / (float)_apMax : 0f);
	public void SetStamina(ushort newValue) { _ap = newValue; }
	public void SetMaxStamina(ushort newValue) { _apMax = newValue; }
}
