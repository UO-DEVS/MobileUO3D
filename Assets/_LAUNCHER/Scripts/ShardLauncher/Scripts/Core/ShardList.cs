using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shard List", menuName = "UO/Shard/Shard List", order = 99 )]
public class ShardList : ScriptableObject
{
	[SerializeField] private List<ShardConfiguration> _shards = new List<ShardConfiguration>();
	public List<ShardConfiguration> Shards => _shards;
}
