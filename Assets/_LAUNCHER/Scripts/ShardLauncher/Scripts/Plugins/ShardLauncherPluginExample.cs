using UnityEngine;

[CreateAssetMenu(fileName = "Example Launcher Plugin", menuName = "UO/Launcher/Plugin/Example Launcher Plugin")]
public class ShardLauncherPluginExample : LauncherPlugin
{
	//ON LOAD
	public override void OnLoad(ShardConfiguration shard)
	{
		
	}
	
	//ON SERVER CHANGED
	public override void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer)
	{
		
	}
	
	//ON SERVER LAUNCHING
	public override void OnBeforeServerLaunch(ShardConfiguration shard)
	{
		
	}
	//ON SERVER LAUNCHED
	public override void OnServerLaunch(ShardConfiguration shard)
	{
		
	}
	//ON GAME CLIENT START
	public override void OnGameClientStart()
	{
		
	}
	
	//ON CLIENT DOWNLOAD START
	public override void OnClientDownloadStart()
	{
		
	}
    
	//ON DOWNLOADS DELETED
	public override void OnDownloadsDeleted(ServerConfiguration server)
	{
		
	}
}
