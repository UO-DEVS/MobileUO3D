using UnityEngine;

//BY DX4D
public class MacroSettingsToggle : MonoBehaviour
{
	[Tooltip("The target container is the transform where this component will search for macros")]
	[SerializeField] Transform _container;
	
	//ON VALIDATE
	protected void OnValidate()
	{
		if (!_container) _container = transform;
	}
	
	bool active = true;
	
	//TOGGLE SETTINGS
	public void ToggleSettings()
	{
		MacroLooper[] macros;
		
		if (_container) macros = _container.GetComponentsInChildren<MacroLooper>();
		else macros = GetComponentsInChildren<MacroLooper>();
		
		for (int i = 0; i < macros.Length; i++)
		{
			macros[i].ToggleSettings(active);
		}
		
		active = !active;
		macros = null; //NOTE: .NET clears this up automatically, but we're clearing up our garbage immediately. (redundant)
	}
	//SHOW SETTINGS
	public void ShowSettings()
	{
		MacroLooper[] macros;
		
		if (_container) macros = _container.GetComponentsInChildren<MacroLooper>();
		else macros = GetComponentsInChildren<MacroLooper>();
		
		for (int i = 0; i < macros.Length; i++)
		{
			macros[i].ShowSettings();
		}
		
		macros = null; //NOTE: .NET clears this up automatically, but we're clearing up our garbage immediately. (redundant)
	}
	//HIDE SETTINGS
	public void HideSettings()
	{
		MacroLooper[] macros = GetComponentsInChildren<MacroLooper>();
		
		for (int i = 0; i < macros.Length; i++)
		{
			macros[i].HideSettings();
		}
		
		macros = null; //NOTE: .NET clears this up automatically, but we're clearing up our garbage immediately. (redundant)
	}
}
