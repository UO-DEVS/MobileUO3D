using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SupportedServerConfigurations", menuName = "SupportedServerConfigurations")]
public class SupportedServerConfigurations : ScriptableObject
{
	//public List<ServerConfiguration> ServerConfigurations; //REMOVED DX4D
	public List<ServerConfiguration> ServerConfigurations = new List<ServerConfiguration>(); //ADDED DX4D
}