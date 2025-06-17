using UnityEngine;

public abstract class LauncherPlugin : ScriptableObject, ILauncherPlugin
{
	//ON LOAD
	public virtual void OnLoad(ShardConfiguration shard)
	{
		
	}
	
	//ON SERVER CHANGED
	public virtual void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer)
	{
		
	}
	
	//ON SERVER LAUNCHING
	public virtual void OnBeforeServerLaunch(ShardConfiguration shard)
	{
		
	}
	//ON SERVER LAUNCHED
	public virtual void OnServerLaunch(ShardConfiguration shard)
	{
		
	}
	//ON GAME CLIENT START
	public virtual void OnGameClientStart()
	{
		
	}
	
	//ON CLIENT DOWNLOAD START
	public virtual void OnClientDownloadStart()
	{
		
	}
    
	//ON DOWNLOADS DELETED
	public virtual void OnDownloadsDeleted(ServerConfiguration server)
	{
		
	}
}
