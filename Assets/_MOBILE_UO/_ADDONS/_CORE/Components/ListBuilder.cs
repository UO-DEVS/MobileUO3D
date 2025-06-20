using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListBuilder : MonoBehaviour
{
	const float CLEARDELAY = 0.05f;
	
	[Header("PREFAB TARGETS")]
	[SerializeField] private Transform _prefab;
	
	[Header("TARGET LIST PARENT")]
	[SerializeField] private Transform _container;
	public Transform Container => _container;
	
	//ON VALIDATE
	protected void OnValidate()
	{
		if (Container == null)
		{
			_container = transform;
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
	public virtual void BuildList()
	{
		if (Container == null) return;
		
		/*foreach (ShardConfiguration shard in Selector.ShardList)
		{
			AddServerEntry(shard, Target);
		}*/
	}
	//CLEAR SERVER LIST
	private void ClearList()
	{
		ClearChildren(Container);
		/*foreach (Transform obj in serverListParent.GetComponentsInChildren<Transform>(true))
		{
			if (obj.parent == serverListParent) GameObject.Destroy(obj.gameObject, CLEARDELAY);
		}*/
	}
	//SHOW SERVER LIST
	private void ShowList(bool show = true)
	{
		if (Container == null) return;
		
		Container.gameObject.SetActive(show);
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
	
	//ADD MACRO
	public virtual void SpawnPrefab()
	{
		//if (!serverEntryPrefab) return false;
		
		//SERVER ENTRY
		Transform entry = Instantiate(_prefab);
		if (entry)
		{
			if (!_container) entry.transform.SetParent(this.transform);//, true);
			else entry.transform.SetParent(_container);//, true);
			
			entry.localPosition = Vector3.zero;
			entry.localScale = Vector3.one;

			entry.gameObject.name = _prefab.name; //(Get rid of (Clone))
			entry.gameObject.SetActive(true);
			
			
			//CONFIG
			/*
			if (serverConfigPrefab)
			{
				ShardConfigUI config = Instantiate(serverConfigPrefab);
				if (config)
				{
					config.gameObject.name = entry.name;
					config.transform.SetParent(entry.transform);
					config.gameObject.SetActive(true);
					
					//config.Center();
					
					config.AssignServer(shard.ShardName, shard.ShardIP, shard.ShardPort);
					config.AssignPatch(shard.ShardFileDownloadIP, shard.ShardFileDownloadPort);
					config.AssignClient(shard.ClientVersion);
					config.Init();
					config.Hide();
					
					//config.Initialize(shard);
					//config.gameObject.SetActive(false);
				}
			} else LogIssue("You must assign a serverConfigPrefab to " + name);
			*/
		}
	}
	
	
	//ON ENABLE
    private void OnEnable()
	{
		/*
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
		*/
		
		//BuildList();
		
		//ClearSelectionList();
	}
	//ON DISABLE
	protected void OnDisable()
	{
		//ClearList();
		
		//BuildSelectionList();
		//ClearSelectionList();
	}
    
	//LOG
	private void Log(string message)
	{
		Debug.Log(message);
	}
	private void Log(string header, string message, string color)
	{
		Debug.Log("<color=" + color + ">" + header + "</color>: " + message);
	}
	private void LogIssue(string message)
	{
		Log("ISSUE", message, "red");
	}
}
