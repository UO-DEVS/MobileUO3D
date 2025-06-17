using UnityEngine;

public class KeepHidden : MonoBehaviour
{
	protected void OnEnable()
	{
		gameObject.SetActive(false);
	}
}
