using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShardConfigUI : MonoBehaviour
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
	
	//INITIALIZE
	//public void Initialize(ShardConfiguration newShard)
	//{
	//	if (newShard)
	//	{
	//		DisplayShard(newShard);
	//	}
	//}
	
	//INIT
	public void Init()
	{
		if (configButtonGroup && configButton) configButtonGroup.Add(configButton);
		//Attach(configButton, configButtonGroup, true);
	}
	//SHOW
	public void Show()
	{
		Activate(configPanel);
		Activate(closeButton);
		
		Deactivate(configButton);
		
		BringToFront(configPanel);
		Center(configPanel);
		
		BringToFront(closeButton);
		BottomCenter(closeButton);
		Offset(closeButton, 0, 64, 0);
		//TopCenter(closeButton);
		//Offset(closeButton, 342, 0, 0);
		//StickTo(closeButton, transform);
	}
	//HIDE
	public void Hide()
	{
		Deactivate(configPanel);
		Deactivate(closeButton);
		
		if (configButtonGroup.gameObject.activeSelf) Activate(configButton);
		else Deactivate(configButton);
	}
	
	// A L I G N M E N T
	//OFFSET
	public void Offset(Transform target, int xoffset, int yoffset, int zoffset)
	{
		target.position = new Vector3(target.position.x + xoffset, target.position.y + yoffset, target.position.z + zoffset);
	}
	//CENTER
	public void Center(Transform target)
	{
		target.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
	}
	//TOP RIGHT
	public void TopRight(Transform target)
	{
		target.position = new Vector3(Screen.width, Screen.height, 0);
	}
	//TOP CENTER
	public void TopCenter(Transform target)
	{
		target.position = new Vector3(Screen.width / 2, Screen.height, 0);
	}
	//BOTTOM CENTER
	public void BottomCenter(Transform target)
	{
		target.position = new Vector3(Screen.width / 2, 0, 0);
	}
	// P O S I T I O N
	//BRING TO FRONT
	public void BringToFront(Transform target)
	{
		target.SetParent(transform.parent.parent.parent);
	}
	//STICK TO
	public void StickTo(Transform target, Transform destination)
	{
		target.position = destination.position;
	}
	//ATTACH
	public void Attach(Transform target, Transform destination, bool retainWorldPosition)
	{
		target.SetParent(destination, retainWorldPosition);
	}
	// V I S I B I L I T Y
	//ACTIVATE
	public void Activate(Transform target)
	{
		target.gameObject.SetActive(true);
	}
	//DEACTIVATE
	public void Deactivate(Transform target)
	{
		target.gameObject.SetActive(false);
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
