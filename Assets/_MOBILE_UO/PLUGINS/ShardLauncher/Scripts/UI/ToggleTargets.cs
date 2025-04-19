using System.Collections.Generic;
using UnityEngine;

namespace UI.Tools
{
	public class ToggleTargets : MonoBehaviour
	{
		[Header("ACTION")]
		//SWAP
		[Tooltip("Setting this to true will enable the targets when this object is disabled and disable them when this object is enabled."
		+ "\n\nOtherwise, the object and targets will be enabled/disabled together.")]
		[SerializeField] bool _swap = true;
		private bool Swap => _swap;
		
		[Header("LINKED OBJECTS")]
		//TARGETS
		[SerializeField] List<Transform> _targets = new List<Transform>();
		
		//ON DISABLE
		protected void OnDisable()
		{
			foreach (Transform target in _targets)
			{
				target.gameObject.SetActive(Swap);
			}
		}
		//ON ENABLE
		protected void OnEnable()
		{
			foreach (Transform target in _targets)
			{
				target.gameObject.SetActive(!Swap);
			}
		}
	}
}
