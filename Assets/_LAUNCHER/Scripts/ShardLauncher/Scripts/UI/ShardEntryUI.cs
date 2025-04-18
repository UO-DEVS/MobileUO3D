using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShardEntryUI : MonoBehaviour
{
	[Header("CONFIG")]
	//TEXT BUTTON COLOR
	[SerializeField] Color _textButtonColor = new Color(0.09f, 0.09f, 0.09f);
	public Color TextButtonColor => _textButtonColor;
	
	[Header("UI TARGETS")]
	//TEXT
	[SerializeField] private TMPro.TMP_Text textLabel;
	public void SetTitle(string newLabel)
	{
		textLabel.text = newLabel;
	}
	//IMAGE
	[SerializeField] private UnityEngine.UI.Image serverEntryPrefab;
	public bool SetImage(Sprite newImage)
	{
		if (newImage == null)
		{
			Image img = gameObject.GetComponent<UnityEngine.UI.Image>();
			if (img)
			{
				img.color = TextButtonColor;
			}
			serverEntryPrefab.gameObject.SetActive(false);
			textLabel.gameObject.SetActive(true);
			return false;
		}
		else
		{
			serverEntryPrefab.gameObject.SetActive(true);
			textLabel.gameObject.SetActive(false);
			serverEntryPrefab.sprite = newImage;
			return true;
		}
	}
	
	//SHARD CONFIG
	private ShardConfiguration _shardEntry;
	public ShardConfiguration Shard => _shardEntry;
	
	//SHARD SELECTOR
	private ShardSelector _shardSelector;
	public ShardSelector Selector => _shardSelector;
	
	//ADD SERVER ENTRY
	public void Assign(ShardSelector newShardSelector, ShardConfiguration newShardEntry)
	{
		_shardSelector = newShardSelector;
		_shardEntry = newShardEntry;
		SetTitle(newShardEntry.ShardName);
		SetImage(newShardEntry.ShardBanner);
	}
	
	//LAUNCH SHARD
	public void LaunchShard()
	{
		Selector.LaunchShard(Shard, false);
	}
	
	//UPDATE SHARD
	public void UpdateShard()
	{
		Selector.LaunchShard(Shard, true);
	}
}
