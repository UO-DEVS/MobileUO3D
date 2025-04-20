using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

[CreateAssetMenu(fileName = "Splash Screen", menuName = "GAME/Launcher/Splash Screen")]
public class SplashScreen : ScriptableObject
{
	/*
    [Header("SCENE TRANSITION")]
    public bool autoloadNextScene = false;
    public bool loadSceneAdditive = true;
	*/
    [Header("AUDIO")]
    public AudioClip splashSound;   // Assign in Inspector
    public float volume = 0.5f;   // Logo zoom speed

	[Header("IMAGE")]
	public Sprite background;
	public Sprite logo;
    
    [Header("SPLASH ANIMATION")]
    public float splashDuration = 16f;
    public float fadeInDuration = 5f;
    public bool zoomEffect = true;
    public float logoZoomSpeed = 0.5f;   // Logo zoom speed
    public float backgroundZoomSpeed = 0.3f; // Background zoom speed
	/*
    [Header("TOGGLE ON LAUNCH")]
    public List<Transform> _toggleOn = new List<Transform>();
    public List<Transform> _toggleOff = new List<Transform>();

    [Header("COMPONENT LINKS")]
    public AudioSource audioPlayer;
    public Image backgroundImage;   // Assign background image
	public Image logoImage;         // Assign logo image
	*/
}
