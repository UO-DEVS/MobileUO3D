using TMPro;
using UnityEngine;
using System.Collections.Generic;

public abstract class EnumDropdownBuilder<T> : MonoBehaviour
{
	public bool _activate = false;
	public TMP_Dropdown _dropdown; // Assign in Inspector

	protected void OnValidate()
	{
		if (!_dropdown) _dropdown = GetComponent<TMP_Dropdown>();
		
		if (_activate)
		{
			_activate = false;
			
			Activate();
		}
	}
	
	void Activate()
	{
		PopulateDropdown();
	}

	void PopulateDropdown()
	{
		_dropdown.ClearOptions(); // Clear existing options

		// Convert enum values to strings and add them to the dropdown
		_dropdown.AddOptions(new List<string>(System.Enum.GetNames(typeof(T))));
	}
}
