using System.Collections.Generic;
using UnityEngine;

public class ShardLauncher : MonoBehaviour
{
	[Header("CONFIG")]
	[SerializeField] private ShardConfiguration shard;
	public ShardConfiguration ActiveShard => shard;
	
    [SerializeField] private bool autorun = true;
	[SerializeField] private bool _replaceOldFiles = true;
	public bool ReplaceOldFiles => _replaceOldFiles;
    
	[Header("PLUGINS")]
	//PLUGINS ENABLED
	[SerializeField] private bool _enablePlugins = true;
	public bool PluginEnabled
	{
		get
		{
			if (_enablePlugins && Plugin != null) return true;
			return false;
		}
	}
	//PLUGINS
	[SerializeField] private LauncherPlugin _plugin;
	public LauncherPlugin Plugin => _plugin;
	[SerializeField] private PluginLauncher _event;
	public PluginLauncher GameEvent => _event;

	//ON VALIDATE
	// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
	protected void OnValidate()
	{
		if (!_event) _event = FindObjectOfType<PluginLauncher>();
	}
	
	//ON ENABLE
    private void OnEnable()
	{
		if (PluginEnabled)
		{
			Plugin?.OnLoad(shard);
			GameEvent?.OnLoad(shard);
		}
        if (autorun) LaunchServer();
    }

	//CHANGE SERVER
	public void ChangeServer(ShardConfiguration targetShard, bool update)
	{
		ShardConfiguration oldShard = shard;
		shard = targetShard;
		if (update) shard.ClientFilesDownloaded = false;
		//shard.ClientFilesDownloaded = !update;
		OnServerChanged(oldShard, shard);
	}
	//ON SERVER CHANGED
	public virtual void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer)
	{
		if (PluginEnabled)
		{
			Plugin?.OnServerChanged(oldServer, newServer);
			GameEvent?.OnServerChanged(oldServer, newServer);
		}
	}
	
	//LAUNCH SERVER
	/// <summary>Launch the assigned <see cref="shard"/> in the Server File Downloader</summary>
    public void LaunchServer()
    {
	    if (shard == null)
        {
            Debug.LogWarning("<color=red>ISSUE: </color>" + "You must assign a shard configuration to " + gameObject.name);
            return;
        }

	    LaunchServer(shard);
    }
    private void LaunchServer(ShardConfiguration configuration)
    {
	    OnBeforeServerLaunch(configuration);
	    
        Debug.Log("<color=green>LAUNCHER: </color>" + "Launching shard " + configuration.ShardName
            + "\nAddress: " + configuration.ShardAddress
            + "\nFile Download: " + configuration.ShardFileDownloadAddress
            + "\nClient Version: " + configuration.ClientVersion
        );
        
	    ServerConfiguration server = configuration.GetConfig();
	    server.AllFilesDownloaded = configuration.ClientFilesDownloaded;
	    LaunchServer(server);
	    
	    OnServerLaunch(configuration);
    }
    private void LaunchServer(ServerConfiguration server)
    {
        ServerConfigurationModel.ActiveConfiguration = server;
        if (!server.AllFilesDownloaded)
        {
	        if (ReplaceOldFiles) DeleteShardFiles(server);
            RunClientDownloader();
        }
        else RunGameClient();
    }
	//ON SERVER LAUNCHING
	public virtual void OnBeforeServerLaunch(ShardConfiguration shard)
	{
		if (PluginEnabled)
		{
			Plugin?.OnBeforeServerLaunch(shard);
			GameEvent?.OnBeforeServerLaunch(shard);
		}
	}
	//ON SERVER LAUNCHED
	public virtual void OnServerLaunch(ShardConfiguration shard)
	{
		if (PluginEnabled)
		{
			Plugin?.OnServerLaunch(shard);
			GameEvent?.OnServerLaunch(shard);
		}
	}
    
	//RUN GAME CLIENT
    private void RunGameClient()
	{
		OnGameClientStart();
        StateManager.GoToState<GameState>();
    }
	public virtual void OnGameClientStart()
	{
		if (PluginEnabled)
		{
			Plugin?.OnGameClientStart();
			GameEvent?.OnGameClientStart();
		}
	}
	
	//RUN CLIENT DOWNLOADER
    private void RunClientDownloader()
	{
		OnClientDownloadStart();
        StateManager.GoToState<DownloadState>();
    }
	public virtual void OnClientDownloadStart()
	{
		if (PluginEnabled)
		{
			Plugin?.OnClientDownloadStart();
			GameEvent?.OnClientDownloadStart();
		}
	}
    
	//DELETE SHARD FILES
    private void DeleteShardFiles(ServerConfiguration serverConfig)
    {
	    ServerConfigurationModel.DeleteConfigurationFiles(serverConfig);
	    OnDownloadsDeleted(serverConfig);
    }
	public virtual void OnDownloadsDeleted(ServerConfiguration server)
	{
		if (PluginEnabled)
		{
			Plugin?.OnDownloadsDeleted(server);
			GameEvent?.OnDownloadsDeleted(server);
		}
	}
}
