using UnityEngine;

[System.Serializable]
public struct LauncherPluginEvents
{
	public bool OnLoad;
	public bool OnServerChanged;
	public bool OnBeforeServerLaunch;
	public bool OnServerLaunch;
	public bool OnGameClientStart;
	public bool OnClientDownloadStart;
	public bool OnDownloadsDeleted;
}

[CreateAssetMenu(fileName = "Messenger Plugin", menuName = "UO/Launcher/Plugin/Messenger Plugin")]
public class ShardLauncherPluginMessages : LauncherPlugin
{
	const string MESSAGE = "Leeeroy Jenkins!!!";
	[SerializeField] string _say = MESSAGE;
	public string AlwaysSay => "<b>" + name + "</b>: " + _say;
	private void Say(string _message)
	{
		#if UNITY_EDITOR// && DEBUG
			Debug.Log(_message);
		#endif
	}
	
	[SerializeField] LauncherPluginEvents config;
	
	//ON LOAD
	public override void OnLoad(ShardConfiguration shard)
	{
		if (!config.OnLoad) return;
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnLoad");
	}
	
	//ON SERVER CHANGED
	public override void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer)
	{
		if (!config.OnServerChanged) return;
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnServerChanged");
	}
	
	//ON SERVER LAUNCHING
	public override void OnBeforeServerLaunch(ShardConfiguration shard)
	{
		if (!config.OnBeforeServerLaunch) return;
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnBeforeServerLaunch");
	}
	//ON SERVER LAUNCHED
	public override void OnServerLaunch(ShardConfiguration shard)
	{
		if (!config.OnServerLaunch) return;
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnServerLaunch");
	}
	//ON GAME CLIENT START
	public override void OnGameClientStart()
	{
		if (!config.OnGameClientStart) return;
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnGameClientStart");
	}
	
	//ON CLIENT DOWNLOAD START
	public override void OnClientDownloadStart()
	{
		if (!config.OnClientDownloadStart) return;
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnClientDownloadStart");
	}
    
	//ON DOWNLOADS DELETED
	public override void OnDownloadsDeleted(ServerConfiguration server)
	{
		if (!config.OnDownloadsDeleted) return;
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnDownloadsDeleted");
	}
}
