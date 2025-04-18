public interface ILauncherPlugin
{
	//ON LOAD
	public void OnLoad(ShardConfiguration shard);
	
	//ON SERVER CHANGED
	public void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer);
	
	//ON SERVER LAUNCHING
	public void OnBeforeServerLaunch(ShardConfiguration shard);
	
	//ON SERVER LAUNCHED
	public void OnServerLaunch(ShardConfiguration shard);
	
	//ON GAME CLIENT START
	public void OnGameClientStart();
	
	//ON CLIENT DOWNLOAD START
	public void OnClientDownloadStart();
    
	//ON DOWNLOADS DELETED
	public void OnDownloadsDeleted(ServerConfiguration server);
}
