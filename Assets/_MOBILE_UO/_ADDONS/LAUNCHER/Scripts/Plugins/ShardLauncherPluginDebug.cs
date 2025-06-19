using UnityEngine;

public enum LogColor
{
	white, red, yellow, green, blue, cyan, gray, black
}

[CreateAssetMenu(fileName = "Debug Launcher Plugin", menuName = "UO/Launcher/Plugin/Debug Launcher Plugin")]
public class ShardLauncherPluginDebug : LauncherPlugin
{
	//ON Load
	public override void OnLoad(ShardConfiguration shard)
	{
		string shardInfo = shard.ShardName + " [" + shard.ShardAddress + "]"
			+ "\nClient Version: " + shard.ShardAddress
			+ "\nClient Files: " + shard.ShardFileDownloadAddress;
		Log(nameof(OnLoad), "<b>Loaded Plugin: </b>" + name + "\n" + shardInfo, LogColor.cyan);
	}
	
	//ON SERVER CHANGED
	public override void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer)
	{
		Log(nameof(OnServerChanged), "<b>Changed from " + oldServer.ShardName + " to " + newServer.ShardName + "</b>", LogColor.cyan);
	}
	
	//ON SERVER LAUNCHING
	public override void OnBeforeServerLaunch(ShardConfiguration shard)
	{
		string shardInfo = shard.ShardName + " [" + shard.ShardAddress + "]"
			+ "\nClient Version: " + shard.ShardAddress
			+ "\nClient Files: " + shard.ShardFileDownloadAddress;
		Log(nameof(OnBeforeServerLaunch), "<b>Initializing Launcher: </b>" + shardInfo, LogColor.yellow);
	}
	//ON SERVER LAUNCHED
	public override void OnServerLaunch(ShardConfiguration shard)
	{
		string shardInfo = shard.ShardName + " [" + shard.ShardAddress + "]"
			+ "\nClient Version: " + shard.ShardAddress
			+ "\nClient Files: " + shard.ShardFileDownloadAddress;
		Log(nameof(OnServerLaunch), "<b>Game Launch Complete: </b>" + shardInfo, LogColor.green);
	}
	//ON GAME CLIENT START
	public override void OnGameClientStart()
	{
		Log(nameof(OnGameClientStart), "<b>Launching Game Client...</b>", LogColor.green);
	}
	
	//ON CLIENT DOWNLOAD START
	public override void OnClientDownloadStart()
	{
		Log(nameof(OnClientDownloadStart), "<b>Downloading Game Client Files...</b>", LogColor.blue);
	}
    
	//ON DOWNLOADS DELETED
	public override void OnDownloadsDeleted(ServerConfiguration server)
	{
		string serverInfo = server.Name + " [" + server.UoServerUrl + ":" + server.UoServerPort + "]"
			+ "\nClient Version: " + server.ClientVersion
			+ "\nClient Files: " + server.FileDownloadServerUrl + ":" + server.FileDownloadServerPort;
		Log(nameof(OnDownloadsDeleted), "<b>Existing Downloads Cleared: </b>" + serverInfo, LogColor.red);
	}
	
	//LOG
	private void Log(string title, string message, LogColor titlecolor = LogColor.white)
	{
		#if UNITY_EDITOR
		Debug.Log("<color=white>" + name + ">" + "</color>"
			+ "<color=" + titlecolor.ToString() + ">" + title + "</color>"
			+ "\n" + message
		);
		#endif
	}
}
