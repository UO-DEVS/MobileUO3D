using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerListBuilder : MonoBehaviour
{
	const float CLEARDELAY = 0.05f;
	
	[Header("PREFAB TARGETS")]
	[SerializeField] private ShardEntryUI serverEntryPrefab;
	
	[Header("COMPONENT LINKS")]
	//[SerializeField] private ShardLauncher launcher;
	[SerializeField] private ShardSelector _selector;
	public ShardSelector Selector => _selector;
	
	[Header("TARGET LIST PARENT")]
	[SerializeField] private Transform _serverListParent;
	public Transform Target => _serverListParent;
	//[SerializeField] private Transform _selectionParent;
	//public Transform ServerSelection => _selectionParent;
	//ON VALIDATE
	protected void OnValidate()
	{
		if (Selector == null)
		{
			if (transform.parent != null)
			{
				_selector = transform.parent.GetComponentInChildren<ShardSelector>();
			}
			else
			{
				_selector = transform.GetComponent<ShardSelector>();
			}
		}
		
		if (Target == null)
		{
			_serverListParent = transform;
		}
	}
	
	/**/
	/*
	//ON SERVER SELECTED
	public void OnServerSelected(ShardConfiguration shard)
	{
		ClearServerList();
		ShowServerList(false);
	}*/
	//BUILD LIST
	private void BuildList()
	{
		if (Target == null) return;
		
		foreach (ShardConfiguration shard in Selector.ShardList)
		{
			AddServerEntry(shard, Target);
		}
	}
	//CLEAR SERVER LIST
	private void ClearList()
	{
		ClearChildren(Target);
		/*foreach (Transform obj in serverListParent.GetComponentsInChildren<Transform>(true))
		{
			if (obj.parent == serverListParent) GameObject.Destroy(obj.gameObject, CLEARDELAY);
		}*/
	}
	//SHOW SERVER LIST
	private void ShowList(bool show = true)
	{
		if (Target == null) return;
		
		Target.gameObject.SetActive(show);
	}
	/*
	//BUILD SELECTION LIST
	private void BuildSelectionList()
	{
		if (ServerSelection == null) return;
		
		AddServerEntry(Selector.ActiveShard, ServerSelection);
	}
	//CLEAR SELECTION LIST
	private void ClearSelectionList()
	{
		ClearChildren(ServerSelection);
	}
	//SHOW SERVER SELECTION
	private void ShowServerSelection(bool show = true)
	{
		if (ServerSelection == null) return;
		
		ServerSelection.gameObject.SetActive(show);
	}
	*/
	//CLEAR CHILDREN
	private void ClearChildren(Transform target)
	{
		if (target == null) return;
		
		foreach (Transform obj in target.GetComponentsInChildren<Transform>(true))
		{
			if (obj.parent == target) GameObject.Destroy(obj.gameObject, CLEARDELAY);
		}
	}
	
	//ADD SERVER ENTRY
	private void AddServerEntry(ShardConfiguration shard, Transform target)
	{
		//if (!serverEntryPrefab) return false;
		
		ShardEntryUI entry = Instantiate(serverEntryPrefab);

		if (!target) entry.transform.SetParent(this.transform);//, true);
		else entry.transform.SetParent(target);//, true);

		entry.Assign(Selector, shard);
		entry.gameObject.name = shard.ShardName;
		entry.gameObject.SetActive(true);
	}
	
	
	//ON ENABLE
    private void OnEnable()
	{
		if (!Selector) return;
		
		if (Selector.ShardList.Count > 0)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			output.AppendLine("<color=yellow><b>-UI SHARD LIST-</b></color>");
			
			foreach (ShardConfiguration shard in Selector.ShardList)
			{
				output.AppendLine(shard.ShardName + " [" + shard.ShardAddress + "]");
			}
			
			Log(output.ToString());
		}
		
		BuildList();
		//ClearSelectionList();
	}
	//ON DISABLE
	protected void OnDisable()
	{
		//BuildSelectionList();
		ClearList();
		//ClearSelectionList();
	}
    
	//LOG
	private void Log(string message)
	{
		Debug.Log(message);
	}
}
