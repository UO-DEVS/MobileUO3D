﻿using UnityEngine;

[CreateAssetMenu(fileName = "Shard Configuration", menuName = "UO/Shard/Shard Configuration")]
public class ShardConfiguration : ScriptableObject
{
    const string SHARDNAME = "Local Shard";
    const string SHARDIP = "localhost";
    const string SHARDPORT = "2593";
    const string DOWNLOADIP = "localhost";
    const string DOWNLOADPORT = "8000"; //DownloadState.DefaultFileDownloadPort;

    [Header("SHARD INFO")]
    //SHARD NAME
    [SerializeField] private string _shardName = SHARDNAME;
    public string ShardName => _shardName;

    //SHARD ADDRESS
    [SerializeField] private string _shardURL = SHARDIP;
	[SerializeField] private string _shardPort = SHARDPORT;
	public string ShardIP => _shardURL;
	public string ShardPort => _shardPort;
	public string ShardAddress => _shardURL + ":" + _shardPort;
	public bool SetShard(string url, string port = SHARDPORT)
	{
		if (string.IsNullOrWhiteSpace(url)) return false;
		
		_shardURL = url;
		_shardPort = port;
		
		return true;
	}

    [Header("CLIENT INFO")]
    //FILE DOWNLOAD ADDRESS
    [SerializeField] private string _fileDownloadURL = DOWNLOADIP;
	[SerializeField] private string _fileDownloadPort = DOWNLOADPORT;
	public string ShardFileDownloadIP => _shardURL;
	public string ShardFileDownloadPort => _shardPort;
    public string ShardFileDownloadAddress => _fileDownloadURL + ":" + _fileDownloadPort;
	public bool SetDownload(string url, string port = DOWNLOADPORT)
	{
		if (string.IsNullOrWhiteSpace(url)) return false;
		
		_fileDownloadURL = url;
		_fileDownloadPort = port;
		
		return true;
	}

    //CLIENT INFO
    [SerializeField] private string _targetClientVersion;
    [SerializeField] private string _unityEditorClientPath;
    public string ClientVersion => _targetClientVersion;
	public bool SetClient(string version)
	{
		if (string.IsNullOrWhiteSpace(version)) return false;
		
		_targetClientVersion = version;
		
		return true;
	}

    [Header("CONFIGURATION")]
    //DOWNLOAD STATUS
    [SerializeField] private bool _encryption = false;
    [SerializeField] private bool _preferExternalStorage = false;
	[SerializeField] private bool _isSupportedServer = false;
	
    [SerializeField] private bool _clientFilesDownloaded = false;
	public bool ClientFilesDownloaded
	{
		get { return _clientFilesDownloaded; }
		set { _clientFilesDownloaded = value; }
	}

	[Header("LAUNCHER CONFIG")]
	//SHARD BANNER
	[SerializeField] private Sprite _shardBanner;
	public Sprite ShardBanner => _shardBanner;
    
    //GET SERVER CONFIGURATION
    public ServerConfiguration GetConfig()
    {
        return new ServerConfiguration()
        {
            Name = _shardName,

            UoServerUrl = _shardURL,
            UoServerPort = _shardPort,

            FileDownloadServerUrl = _fileDownloadURL,
            FileDownloadServerPort = _fileDownloadPort,

            ClientVersion = _targetClientVersion,
            UseEncryption = _encryption,

            ClientPathForUnityEditor = _unityEditorClientPath,
            AllFilesDownloaded = _clientFilesDownloaded,
            PreferExternalStorage = _preferExternalStorage,
            SupportedServer = _isSupportedServer
        };
    }
}
