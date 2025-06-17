using System.Collections.Generic;
using UnityEngine;

namespace UI.Tools
{
	public class SubstituteTargets : MonoBehaviour
	{
		[SerializeField] List<Transform> _targets = new List<Transform>();
		
		protected void OnEnable()
		{
			foreach (Transform target in _targets)
			{
				target.gameObject.SetActive(true);
			}
			
			gameObject.SetActive(false);
		}
	}
}
