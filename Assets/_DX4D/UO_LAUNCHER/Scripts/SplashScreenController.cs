using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class SplashScreenController : MonoBehaviour
{
	[Header("SPLASH SCREEN")]
	public SplashScreen splashScreen;
	/*
    [Header("AUDIO")]
    public AudioClip splashSound;   // Assign in Inspector
    public float volume = 0.5f;   // Logo zoom speed

    [Header("SPLASH ANIMATION")]
    public float splashDuration = 16f;
    public float fadeInDuration = 5f;
    public bool zoomEffect = true;
    public float logoZoomSpeed = 0.5f;   // Logo zoom speed
    public float backgroundZoomSpeed = 0.3f; // Background zoom speed
	//*/
    [Header("SCENE TRANSITION")]
    public bool autoloadNextScene = false;
	public bool loadSceneAdditive = true;
    
    [Header("TOGGLE ON LAUNCH")]
    public List<Transform> _toggleOn = new List<Transform>();
    public List<Transform> _toggleOff = new List<Transform>();

    [Header("COMPONENT LINKS")]
    public AudioSource audioPlayer;
    public Image backgroundImage;   // Assign background image
    public Image logoImage;         // Assign logo image

    private Vector3 initialLogoScale;
    private Vector3 initialBgScale;

    private void Start()
	{
		if (!splashScreen)
		{
			if (autoloadNextScene) Invoke(nameof(LoadMainScene), 0f);
			return;
		}
		
        // Play splash sound
		if (splashScreen.splashSound)
        {
            if (!audioPlayer)
            {
                audioPlayer = GetComponent<AudioSource>();
                if (!audioPlayer)
                {
                    audioPlayer = gameObject.AddComponent<AudioSource>();
                }
            }
            if (audioPlayer)
            {
                audioPlayer.clip = splashScreen.splashSound;
                audioPlayer.playOnAwake = false;
                audioPlayer.volume *= splashScreen.volume;
                audioPlayer.Play();
            }
        }
        
		// Fade In
        logoImage.CrossFadeAlpha(0f, 0f, true);
        
		// Load Images
		backgroundImage.sprite = splashScreen.background;
		logoImage.sprite = splashScreen.logo;
		
		// Fade Out
        logoImage.CrossFadeAlpha(1f, splashScreen.fadeInDuration / 4, false);
        //logoImage.CrossFadeAlpha(0f, splashDuration, false);

        // Store initial scales
        initialLogoScale = logoImage.transform.localScale;
        initialBgScale = backgroundImage.transform.localScale;

        // Scale background dynamically
        ScaleBackground();

        // Start zoom effect if enabled
        if (splashScreen.zoomEffect)
        {
            StartCoroutine(ZoomElements());
        }
        // Load next scene after the duration
        if (autoloadNextScene) Invoke(nameof(LoadMainScene), splashScreen.splashDuration);
    }

    private void ScaleBackground()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float imageRatio = backgroundImage.sprite.bounds.size.x / backgroundImage.sprite.bounds.size.y;

        if (screenRatio >= imageRatio)
        {
            backgroundImage.transform.localScale = new Vector3(screenRatio / imageRatio, screenRatio / imageRatio, 1);
        }
        else
        {
            backgroundImage.transform.localScale = new Vector3(imageRatio / screenRatio, imageRatio / screenRatio, 1);
        }
    }

    private IEnumerator ZoomElements()
    {
        float time = 0;
        while (time < splashScreen.splashDuration)
        {
            time += Time.deltaTime;
            float logoScaleIncrease = splashScreen.logoZoomSpeed * Time.deltaTime;
            float bgScaleIncrease = splashScreen.backgroundZoomSpeed * Time.deltaTime;

            // Zoom background while maintaining aspect ratio
            backgroundImage.transform.localScale += new Vector3(bgScaleIncrease, bgScaleIncrease, 0);

            // Zoom logo independently
            logoImage.transform.localScale += Vector3.one * logoScaleIncrease;

            yield return null;
        }
    }

    public void LoadMainScene()
    {
        ToggleOff();
        SceneManager.LoadScene(1, (loadSceneAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single));  // Ensure the main scene is next in Build Settings
        ToggleOn();
    }
    
    //TOGGLE OFF
    private void ToggleOff()
    {
        foreach (Transform obj in _toggleOff)
        {
            if (obj.gameObject) GameObject.DestroyImmediate(obj.gameObject);
        }
        _toggleOff.Clear();
    }
    //TOGGLE ON
    private void ToggleOn()
    {
        foreach (Transform obj in _toggleOn)
        {
            if (obj.gameObject) obj.gameObject.SetActive(true);
        }
        _toggleOn.Clear();
    }
}
