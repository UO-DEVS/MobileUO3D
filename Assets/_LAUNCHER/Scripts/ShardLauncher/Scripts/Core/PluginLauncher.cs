using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PluginLauncher : MonoBehaviour, ILauncherPlugin
{
	[SerializeField] LauncherPlugin _plugin;
	public LauncherPlugin Plugin => _plugin;
	
	[Header("EVENTS")]
	[SerializeField] UnityEvent<ShardConfiguration> _onLoad;
	[SerializeField] UnityEvent<ShardConfiguration, ShardConfiguration> _onServerChanged;
	[SerializeField] UnityEvent<ShardConfiguration> _onBeforeServerLaunch;
	[SerializeField] UnityEvent<ShardConfiguration> _onServerLaunch;
	
	[SerializeField] UnityEvent _onGameClientStart = new UnityEvent();
	[SerializeField] UnityEvent _onClientDownloadStart = new UnityEvent();
	[SerializeField] UnityEvent<ServerConfiguration> _onDownloadsDeleted = new UnityEvent<ServerConfiguration>();
	
	//ON LOAD
	public virtual void OnLoad(ShardConfiguration shard)
	{
		Plugin.OnLoad(shard);
		_onLoad.Invoke(shard);
	}
	
	//ON SERVER CHANGED
	public virtual void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer)
	{
		Plugin.OnServerChanged(oldServer, newServer);
		_onServerChanged.Invoke(oldServer, newServer);
	}
	
	//ON SERVER LAUNCHING
	public virtual void OnBeforeServerLaunch(ShardConfiguration shard)
	{
		Plugin.OnBeforeServerLaunch(shard);
		_onBeforeServerLaunch.Invoke(shard);
	}
	//ON SERVER LAUNCHED
	public virtual void OnServerLaunch(ShardConfiguration shard)
	{
		Plugin.OnServerLaunch(shard);
		_onServerLaunch.Invoke(shard);
	}
	//ON GAME CLIENT START
	public virtual void OnGameClientStart()
	{
		Plugin.OnGameClientStart();
		_onGameClientStart.Invoke();
	}
	
	//ON CLIENT DOWNLOAD START
	public virtual void OnClientDownloadStart()
	{
		Plugin.OnClientDownloadStart();
		_onClientDownloadStart.Invoke();
	}
    
	//ON DOWNLOADS DELETED
	public virtual void OnDownloadsDeleted(ServerConfiguration server)
	{
		Plugin.OnDownloadsDeleted(server);
		_onDownloadsDeleted.Invoke(server);
	}
}
