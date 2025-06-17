using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class UIBehaviour : MonoBehaviour
{
	// A B S T R A C T
	//INIT//SHOW//HIDE
	public abstract void Init();
	public abstract void Show();
	public abstract void Hide();
	
	// T O P  A L I G N M E N T
	//TOP LEFT
	public void TopLeft(Transform target)
	{
		target.position = new Vector3(0, Screen.height, 0);
	}
	//TOP CENTER
	public void TopCenter(Transform target)
	{
		target.position = new Vector3(Screen.width / 2, Screen.height, 0);
	}
	//TOP RIGHT
	public void TopRight(Transform target)
	{
		target.position = new Vector3(Screen.width, Screen.height, 0);
	}
	
	// M I D D L E  A L I G N M E N T
	//MIDDLE LEFT
	public void MiddleLeft(Transform target)
	{
		target.position = new Vector3(0, Screen.height / 2, 0);
	}
	//CENTER
	public void Center(Transform target)
	{
		target.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
	}
	//MIDDLE RIGHT
	public void MiddleRight(Transform target)
	{
		target.position = new Vector3(Screen.width, Screen.height / 2, 0);
	}
	
	// B O T T O M  A L I G N M E N T
	//BOTTOM LEFT
	public void BottomLeft() => BottomLeft(transform);
	public void BottomLeft(Transform target)
	{
		target.position = new Vector3(0, 0, 0);
	}
	//BOTTOM CENTER
	public void BottomCenter() => BottomCenter(transform);
	public void BottomCenter(Transform target)
	{
		target.position = new Vector3(Screen.width / 2, 0, 0);
	}
	//BOTTOM RIGHT
	public void BottomRight() => BottomRight(transform);
	public void BottomRight(Transform target)
	{
		target.position = new Vector3(Screen.width, 0, 0);
	}
	
	// P O S I T I O N
	//OFFSET
	public void Offset(Transform target, int xoffset, int yoffset, int zoffset)
	{
		target.position = new Vector3(target.position.x + xoffset, target.position.y + yoffset, target.position.z + zoffset);
	}
	//MOVE TO
	public void MoveTo(Transform destination) => MoveTo(transform, destination);
	public void MoveTo(Transform target, Transform destination)
	{
		target.position = destination.position;
	}
	
	// C O N S T R A I N T S
	//ATTACH TO
	public void AttachTo(Transform destination, bool retainWorldPosition) => AttachTo(transform, destination, retainWorldPosition);
	public void AttachTo(Transform target, Transform destination, bool retainWorldPosition)
	{
		target.SetParent(destination, retainWorldPosition);
	}
	//BRING TO FRONT
	public void BringToFront() => BringToFront(transform);
	public void BringToFront(Transform target)
	{
		target.SetParent(transform.parent.parent.parent);
	}
	
	// V I S I B I L I T Y
	//ACTIVATE
	public void Activate(Transform target)
	{
		target.gameObject.SetActive(true);
	}
	//DEACTIVATE
	public void Deactivate(Transform target)
	{
		target.gameObject.SetActive(false);
	}
}
