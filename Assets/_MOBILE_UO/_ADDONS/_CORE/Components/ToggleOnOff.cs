using UnityEngine;

public class ToggleOnOff : MonoBehaviour
{
	public void Toggle()
	{
		gameObject.SetActive(!gameObject.active);
	}
	public void Show() => gameObject.SetActive(true);
	public void Hide() => gameObject.SetActive(false);
}
