using System.Collections.Generic;
using UnityEngine;

public class ShardSelector : MonoBehaviour
{
	[Header("COMPONENT LINKS")]
	[SerializeField] private ShardLauncher _launcher;
	public ShardLauncher Launcher => _launcher;
	public ShardConfiguration ActiveShard => Launcher.ActiveShard;
	
	[Header("LINKED OBJECTS")]
	[SerializeField] private List<Transform> _enableOnLaunch = new List<Transform>();
	[SerializeField] private bool _deleteInstead = false;
	[SerializeField] private List<Transform> _disableOnLaunch = new List<Transform>();
	
	[Header("SHARD LIST")]
	[SerializeField] private ShardList _shardlist;
	public List<ShardConfiguration> ShardList => _shardlist?.Shards;
    
	//ON VALIDATE
	protected void OnValidate()
	{
		if (!_launcher) _launcher = gameObject.GetComponent<ShardLauncher>();
	}
	
	//ON ENABLE
    private void OnEnable()
	{
		if (!_shardlist) return;
		
		if (_shardlist.Shards.Count > 0)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			output.AppendLine("<color=yellow><b>-SHARD LIST-</b></color>");
			
			foreach (ShardConfiguration shard in _shardlist.Shards)
			{
				output.AppendLine(shard.ShardName + " [" + shard.ShardAddress + "]");
			}
			
			Log(output.ToString());
		}
	}
	
	//LAUNCH SHARD
	public void LaunchShard(ShardConfiguration shard, bool update)
	{
		if (!_launcher) return;
		
		_launcher.ChangeServer(shard, update);
		OnServerLaunch();
		_launcher.LaunchServer();
		
		//_shardSelectorUI.OnServerSelected(shard);
	}
	
	//DELETE DOWNLOADED FILES
	public void DeleteDownloadedFiles(ServerConfiguration serverConfig)
	{
		if (!_launcher) return;
		
		_launcher.DeleteDownloadedFiles(serverConfig);
	}
	
	//ON SERVER LAUNCH
	private void OnServerLaunch()
	{
		EnableAll(_enableOnLaunch);
		if (_deleteInstead) DeleteAll(_disableOnLaunch);
		else DisableAll(_disableOnLaunch);
	}
	
	//ENABLE/DISABLE ALL
	private void EnableAll(List<Transform> toEnable) => SetAll(toEnable, true);
	private void DisableAll(List<Transform> toDisable) => SetAll(toDisable, false);
	
	//SET ALL
	private void SetAll(List<Transform> toSet, bool setTo)
	{
		foreach (Transform obj in toSet)
		{
			obj.gameObject.SetActive(setTo);
		}
	}
	//DELETE ALL
	private void DeleteAll(List<Transform> toSet, float delay = 0.05f)
	{
		foreach (Transform obj in toSet)
		{
			GameObject.Destroy(obj.gameObject, delay);
		}
	}
    
	//LOG
	private void Log(string message)
	{
		Debug.Log(message);
	}
}
