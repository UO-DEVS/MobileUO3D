using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterUIElement : MonoBehaviour
{
	//ON ENABLE
	protected void OnEnable()
	{
		Center();
	}
	
	//CENTER
	public void Center()
	{
		transform.SetParent(transform);
		transform.position = new Vector3(Screen.width / 2, Screen.height / 2, -99);
	}
}
