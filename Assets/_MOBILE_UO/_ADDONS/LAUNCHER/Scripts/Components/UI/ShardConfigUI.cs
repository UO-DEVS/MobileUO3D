using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShardConfigUI : UIBehaviour
{
	[Header("UI TARGETS")]
	[SerializeField] UI.Tools.ToggleTargets configButtonGroup;
	
	[Header("UI ELEMENTS")]
	[SerializeField] Transform configPanel;
	[SerializeField] Transform configButton;
	[SerializeField] Transform closeButton;
	
	[Header("SHARD CONFIG")]
	//SHARD
	[SerializeField] private TMP_InputField _shardNameText;
	[SerializeField] private TMP_InputField _serverURLText;
	[SerializeField] private TMP_InputField _serverPortText;
	//DOWNLOAD
	[SerializeField] private TMP_InputField _downloadURLText;
	[SerializeField] private TMP_InputField _downloadPortText;
	//CLIENT
	[SerializeField] private TMP_InputField _clientVersionText;
	
	//INIT
	public override void Init()
	{
		if (configButtonGroup && configButton) configButtonGroup.Add(configButton);
		//Attach(configButton, configButtonGroup, true);
	}
	//SHOW
	public override void Show()
	{
		Activate(configPanel);
		Activate(closeButton);
		
		Deactivate(configButton);
		
		BringToFront(configPanel);
		Center(configPanel);
		
		BringToFront(closeButton);
		TopCenter(closeButton);
	}
	//HIDE
	public override void Hide()
	{
		Deactivate(configPanel);
		Deactivate(closeButton);
		
		if (configButtonGroup.gameObject.activeSelf) Activate(configButton);
		else Deactivate(configButton);
	}
	
	//ASSIGNMENT
	public void AssignServer(string serverName, string serverIP, string serverPort)
	{
		Debug.Log(serverName + "-" + serverIP + ":" + serverPort);
		SetServerName(serverName);
		SetServerAddress(serverIP, serverPort);
	}
	public void AssignPatch(string downloadIP, string downloadPort)
	{
		SetPatchAddress(downloadIP, downloadPort);
	}
	public void AssignClient(string clientVersion)
	{
		SetClientVersion(clientVersion);
	}
	
	//DISPLAY SHARD CONFIGURATION
	//void DisplayShard(ShardConfiguration newShard)
	//{
	//	DisplayShardName(newShard.ShardName);
	//	DisplayShardAddress(newShard.ShardIP, newShard.ShardPort);
	//	DisplayDownloadAddress(newShard.ShardFileDownloadIP, newShard.ShardFileDownloadPort);
	//	DisplayClientVersion(newShard.ClientVersion);
	//}
	
	//SET SERVER NAME
	void SetServerName(string newName)
	{
		if (_shardNameText) _shardNameText.text = newName;
	}
	//SET SERVER ADDRESS
	void SetServerAddress(string newURL, string newPort)
	{
		if (_serverURLText) _serverURLText.text = newURL;
		if (_serverPortText) _serverPortText.text = newPort;
	}
	//SET PATCH ADDRESS
	void SetPatchAddress(string newURL, string newPort)
	{
		if (_downloadURLText) _downloadURLText.text = newURL;
		if (_downloadPortText) _downloadPortText.text = newPort;
	}
	//SET CLIENT VERSION
	void SetClientVersion(string newVersion)
	{
		if (_clientVersionText) _clientVersionText.text = newVersion;
	}
}
