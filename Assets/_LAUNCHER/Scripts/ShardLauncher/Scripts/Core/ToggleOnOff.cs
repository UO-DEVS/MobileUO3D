using UnityEngine;

public class ToggleOnOff : MonoBehaviour
{
	public void Toggle()
	{
		gameObject.SetActive(!gameObject.active);
	}
}
