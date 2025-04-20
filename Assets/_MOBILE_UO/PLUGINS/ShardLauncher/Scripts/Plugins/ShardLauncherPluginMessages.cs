using UnityEngine;

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
	
	public override void OnLoad(ShardConfiguration shard)
	{
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnLoad");
	}
	
	//ON SERVER CHANGED
	public override void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer)
	{
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnServerChanged");
	}
	
	//ON SERVER LAUNCHING
	public override void OnBeforeServerLaunch(ShardConfiguration shard)
	{
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnBeforeServerLaunch");
	}
	//ON SERVER LAUNCHED
	public override void OnServerLaunch(ShardConfiguration shard)
	{
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnServerLaunch");
	}
	//ON GAME CLIENT START
	public override void OnGameClientStart()
	{
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnGameClientStart");
	}
	
	//ON CLIENT DOWNLOAD START
	public override void OnClientDownloadStart()
	{
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnClientDownloadStart");
	}
    
	//ON DOWNLOADS DELETED
	public override void OnDownloadsDeleted(ServerConfiguration server)
	{
		if (!string.IsNullOrWhiteSpace(AlwaysSay)) Say(AlwaysSay + "\n OnDownloadsDeleted");
	}
}
