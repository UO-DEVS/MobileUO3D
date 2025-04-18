using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Launcher Plugin Collection", menuName = "UO/Launcher/Launcher Plugin Collection")]
public class LauncherPluginCollection : LauncherPlugin
{
	[Header("PLUGINS")]
	[SerializeField] private bool _enablePlugins = true;
	public bool PluginsEnabled => _enablePlugins;
	[SerializeField] private List<LauncherPlugin> _plugins = new List<LauncherPlugin>();
	public List<LauncherPlugin> Plugins => _plugins;
	
	//ON LOAD
	public override void OnLoad(ShardConfiguration shard)
	{
		if (PluginsEnabled)
		{
			foreach (ILauncherPlugin plugin in Plugins)
			{
				plugin.OnLoad(shard);
			}
		}
	}
	
	//ON SERVER CHANGED
	public override void OnServerChanged(ShardConfiguration oldServer, ShardConfiguration newServer)
	{
		if (PluginsEnabled)
		{
			foreach (ILauncherPlugin plugin in Plugins)
			{
				plugin.OnServerChanged(oldServer, newServer);
			}
		}
	}
	
	//ON SERVER LAUNCHING
	public override void OnBeforeServerLaunch(ShardConfiguration shard)
	{
		if (PluginsEnabled)
		{
			foreach (ILauncherPlugin plugin in Plugins)
			{
				plugin.OnBeforeServerLaunch(shard);
			}
		}
	}
	
	//ON SERVER LAUNCHED
	public override void OnServerLaunch(ShardConfiguration shard)
	{
		if (PluginsEnabled)
		{
			foreach (ILauncherPlugin plugin in Plugins)
			{
				plugin.OnServerLaunch(shard);
			}
		}
	}
	
	//ON GAME CLIENT START
	public override void OnGameClientStart()
	{
		if (PluginsEnabled)
		{
			foreach (ILauncherPlugin plugin in Plugins)
			{
				plugin.OnGameClientStart();
			}
		}
	}
	
	//ON CLIENT DOWNLOAD START
	public override void OnClientDownloadStart()
	{
		if (PluginsEnabled)
		{
			foreach (ILauncherPlugin plugin in Plugins)
			{
				plugin.OnClientDownloadStart();
			}
		}
	}
    
	//ON DOWNLOADS DELETED
	public override void OnDownloadsDeleted(ServerConfiguration server)
	{
		if (PluginsEnabled)
		{
			foreach (ILauncherPlugin plugin in Plugins)
			{
				plugin.OnDownloadsDeleted(server);
			}
		}
	}
}
