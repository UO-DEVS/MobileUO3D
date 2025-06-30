using System;
using System.IO;
using System.Linq;
using UnityEngine;
using ClassicUO;
using ClassicUO.Utility.Logging;
using ClassicUO.Configuration;
using ClassicUO.Utility;
using ClassicUO.Game;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Game.Scenes;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Game.UI.Gumps;
using ClassicUO.Game.UI.Gumps.Login;
using Newtonsoft.Json;
using ClassicUO.Network;
using Microsoft.Xna.Framework;
using SDL2;
using GameObject = UnityEngine.GameObject;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using ClassicUO.Network.Encryption;

public class ClientRunner : MonoBehaviour
{
	[Header("RENDER SETTINGS")]
	[Tooltip("When this is enabled the UO client will not be rendered after patching and launching (used to override the default client)")]
	[SerializeField] private bool disableClientRenderer = false;
	
	[Header("AUTOLOGIN")]
	[SerializeField] private bool autologin;
	[Space]
	[SerializeField] string autologinServer = "fakeserver";
	[SerializeField] string autologinAccount = "fakeaccount";
	[SerializeField] string autologinCharacter = "fakecharacter";
	[Space]
	[SerializeField] int autologinMap = 0;
	[SerializeField] ushort autologinX = 1443;
	[SerializeField] ushort autologinY = 1677;
	[SerializeField] sbyte autologinZ = 0;
	
	[Header("ENCRYPTION")]
	[SerializeField] private EncryptionType encryption = EncryptionType.NONE;
	
	[Header("GRAPHICS")]
	[SerializeField] public bool useGraphicsDrawTexture;
	[SerializeField] private bool scaleGameToFitScreen;
	
	[Header("MOBILE JOYSTICK")]
	[SerializeField] private MobileJoystick movementJoystick;
	[SerializeField] private bool showMovementJoystickOnNonMobilePlatforms;
	[SerializeField] private float[] joystickDeadZoneValues;
	[SerializeField] private float[] joystickRunThresholdValues;
	
	[Header("VIRTUAL KEYBOARD")]
	[SerializeField] private GameObject modifierKeyButtonsParent;
	[SerializeField] private ModifierKeyButtonPresenter ctrlKeyButtonPresenter;
	[SerializeField] private ModifierKeyButtonPresenter altKeyButtonPresenter;
	[SerializeField] private ModifierKeyButtonPresenter shiftKeyButtonPresenter;
	[SerializeField] private UnityEngine.UI.Button escButton;
	
	//ADDED DX4D
	[Header("F KEYS")]
	[SerializeField] private UnityEngine.UI.Button f1Button;
	[SerializeField] private UnityEngine.UI.Button f2Button;
	[SerializeField] private UnityEngine.UI.Button f3Button;
	[SerializeField] private UnityEngine.UI.Button f4Button;
	
	[SerializeField] private UnityEngine.UI.Button f5Button;
	[SerializeField] private UnityEngine.UI.Button f6Button;
	[SerializeField] private UnityEngine.UI.Button f7Button;
	[SerializeField] private UnityEngine.UI.Button f8Button;
	
	[SerializeField] private UnityEngine.UI.Button f9Button;
	[SerializeField] private UnityEngine.UI.Button f10Button;
	[SerializeField] private UnityEngine.UI.Button f11Button;
	[SerializeField] private UnityEngine.UI.Button f12Button;
	
	[Header("NUM KEYS")]
	[SerializeField] private UnityEngine.UI.Button NUM1Button;
	[SerializeField] private UnityEngine.UI.Button NUM2Button;
	[SerializeField] private UnityEngine.UI.Button NUM3Button;
	[SerializeField] private UnityEngine.UI.Button NUM4Button;
	
	[SerializeField] private UnityEngine.UI.Button NUM5Button;
	[SerializeField] private UnityEngine.UI.Button NUM6Button;
	[SerializeField] private UnityEngine.UI.Button NUM7Button;
	[SerializeField] private UnityEngine.UI.Button NUM8Button;
	
	[SerializeField] private UnityEngine.UI.Button NUM9Button;
	[SerializeField] private UnityEngine.UI.Button NUM0Button;
	//END ADDED
	

	private int lastScreenWidth;
	private int lastScreenHeight;
	
	public Action<string> OnError;
	public Action OnExiting;
	public Action<bool> SceneChanged;

	private void Awake()
	{
		UserPreferences.ScaleSize.ValueChanged += OnCustomScaleSizeChanged;
		UserPreferences.ForceUseXbr.ValueChanged += OnForceUseXbrChanged;
		UserPreferences.ShowCloseButtons.ValueChanged += OnShowCloseButtonsChanged;
		UserPreferences.UseMouseOnMobile.ValueChanged += OnUseMouseOnMobileChanged;
		UserPreferences.TargetFrameRate.ValueChanged += OnTargetFrameRateChanged;
		UserPreferences.TextureFiltering.ValueChanged += UpdateTextureFiltering;
		UserPreferences.JoystickDeadZone.ValueChanged += OnJoystickDeadZoneChanged;
		UserPreferences.JoystickRunThreshold.ValueChanged += OnJoystickRunThresholdChanged;
		UserPreferences.ContainerItemSelection.ValueChanged += OnContainerItemSelectionChanged;
		UserPreferences.ShowModifierKeyButtons.ValueChanged += OnShowModifierKeyButtonsChanged;
		UserPreferences.EnableAssistant.ValueChanged += OnEnableAssistantChanged;
		UserPreferences.EnlargeSmallButtons.ValueChanged += OnEnlargeSmallButtonsChanged;
		OnCustomScaleSizeChanged(UserPreferences.ScaleSize.CurrentValue);
		OnForceUseXbrChanged(UserPreferences.ForceUseXbr.CurrentValue);
		OnShowCloseButtonsChanged(UserPreferences.ShowCloseButtons.CurrentValue);
		OnUseMouseOnMobileChanged(UserPreferences.UseMouseOnMobile.CurrentValue);
		OnTargetFrameRateChanged(UserPreferences.TargetFrameRate.CurrentValue);
		UpdateTextureFiltering(UserPreferences.TextureFiltering.CurrentValue);
		OnJoystickDeadZoneChanged(UserPreferences.JoystickDeadZone.CurrentValue);
		OnJoystickRunThresholdChanged(UserPreferences.JoystickRunThreshold.CurrentValue);
		OnContainerItemSelectionChanged(UserPreferences.ContainerItemSelection.CurrentValue);
		OnShowModifierKeyButtonsChanged(UserPreferences.ShowModifierKeyButtons.CurrentValue);
		OnEnableAssistantChanged(UserPreferences.EnableAssistant.CurrentValue);
		OnEnlargeSmallButtonsChanged(UserPreferences.EnlargeSmallButtons.CurrentValue);
		
		escButton.onClick.AddListener(OnEscButtonClicked);
		
		//ADDED DX4D
		//F KEYS
		f1Button.onClick.AddListener(OnF1ButtonClicked);
		f2Button.onClick.AddListener(OnF2ButtonClicked);
		f3Button.onClick.AddListener(OnF3ButtonClicked);
		f4Button.onClick.AddListener(OnF4ButtonClicked);
		
		f5Button.onClick.AddListener(OnF5ButtonClicked);
		f6Button.onClick.AddListener(OnF6ButtonClicked);
		f7Button.onClick.AddListener(OnF7ButtonClicked);
		f8Button.onClick.AddListener(OnF8ButtonClicked);
		
		f9Button.onClick.AddListener(OnF9ButtonClicked);
		f10Button.onClick.AddListener(OnF10ButtonClicked);
		f11Button.onClick.AddListener(OnF11ButtonClicked);
		f12Button.onClick.AddListener(OnF12ButtonClicked);
		//NUM KEYS
		NUM1Button.onClick.AddListener(OnNUM1ButtonClicked);
		NUM2Button.onClick.AddListener(OnNUM2ButtonClicked);
		NUM3Button.onClick.AddListener(OnNUM3ButtonClicked);
		NUM4Button.onClick.AddListener(OnNUM4ButtonClicked);
		
		NUM5Button.onClick.AddListener(OnNUM5ButtonClicked);
		NUM6Button.onClick.AddListener(OnNUM6ButtonClicked);
		NUM7Button.onClick.AddListener(OnNUM7ButtonClicked);
		NUM8Button.onClick.AddListener(OnNUM8ButtonClicked);
		
		NUM9Button.onClick.AddListener(OnNUM9ButtonClicked);
		NUM0Button.onClick.AddListener(OnNUM0ButtonClicked);
		//END ADDED
	}

	private void OnDisable()
	{
		if (modifierKeyButtonsParent != null)
		{
			modifierKeyButtonsParent.SetActive(false);
		}
	}

	private void OnEscButtonClicked()
	{
		if (Client.Game != null)
		{
			Client.Game.EscOverride = true;
		}
	}
	
	//ADDED DX4D
	// F  K E Y S
	//F1-F4
	private void OnF1ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F1Override = true; }
	}
	private void OnF2ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F2Override = true; }
	}
	private void OnF3ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F3Override = true; }
	}
	private void OnF4ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F4Override = true; }
	}
	//F5-F8
	private void OnF5ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F5Override = true; }
	}
	private void OnF6ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F6Override = true; }
	}
	private void OnF7ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F7Override = true; }
	}
	private void OnF8ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F8Override = true; }
	}
	//F5-F8
	private void OnF9ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F9Override = true; }
	}
	private void OnF10ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F10Override = true; }
	}
	private void OnF11ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F11Override = true; }
	}
	private void OnF12ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.F12Override = true; }
	}
	
	// N U M B E R S
	private void OnNUM1ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM1Override = true; }
	}
	private void OnNUM2ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM2Override = true; }
	}
	private void OnNUM3ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM3Override = true; }
	}
	private void OnNUM4ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM4Override = true; }
	}
	private void OnNUM5ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM5Override = true; }
	}
	private void OnNUM6ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM6Override = true; }
	}
	private void OnNUM7ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM7Override = true; }
	}
	private void OnNUM8ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM8Override = true; }
	}
	private void OnNUM9ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM9Override = true; }
	}
	private void OnNUM0ButtonClicked()
	{
		if (Client.Game != null) { Client.Game.NUM0Override = true; }
	}
	//END ADDED
	
	private void OnEnlargeSmallButtonsChanged(int currentValue)
	{
		var enlarge = currentValue == (int) PreferenceEnums.EnlargeSmallButtons.On;
		if (UIManager.Gumps == null)
		{
			return;
		}
		foreach (var control in UIManager.Gumps)
		{
			ToggleSmallButtonsSize(control, enlarge);
		}
	}

	private void ToggleSmallButtonsSize(Control control, bool enlarge)
	{
		if (control is Button button)
		{
			button.ToggleSize(enlarge);
		}
		foreach (var child in control.Children)
		{
			ToggleSmallButtonsSize(child, enlarge);
		}
	}

	private void OnEnableAssistantChanged(int enableAssistantCurrentValue)
	{
#if ENABLE_INTERNAL_ASSISTANT
		if (UserPreferences.EnableAssistant.CurrentValue == (int) PreferenceEnums.EnableAssistant.On && Client.Game != null)
		{
			if (Plugin.LoadInternalAssistant())
			{
				//If we're already in the GameScene, trigger OnConnected callback since the Assistant won't receive it and
				//because it's needed for initialization
				if (Client.Game.Scene is GameScene)
				{
					Plugin.OnConnected();
				}
			}
		}
#endif
	}

	private void OnShowModifierKeyButtonsChanged(int currentValue)
	{
		if (Client.Game != null)
		{
			modifierKeyButtonsParent.SetActive(currentValue == (int) PreferenceEnums.ShowModifierKeyButtons.On);
		}
	}

	private void OnForceUseXbrChanged(int currentValue)
	{
		if (ProfileManager.CurrentProfile != null)
		{
			ProfileManager.CurrentProfile.UseXBR = currentValue == (int) PreferenceEnums.ForceUseXbr.On;
		}
	}

	private void OnContainerItemSelectionChanged(int currentValue)
	{
		ItemGump.PixelCheck = currentValue == (int) PreferenceEnums.ContainerItemSelection.Fine;
	}

	private void OnJoystickRunThresholdChanged(int currentValue)
	{
		if (Client.Game?.Scene is GameScene gameScene)
		{
			gameScene.JoystickRunThreshold = joystickRunThresholdValues[UserPreferences.JoystickRunThreshold.CurrentValue];
		}
	}

	private void OnJoystickDeadZoneChanged(int currentValue)
	{
		movementJoystick.deadZone = joystickDeadZoneValues[currentValue];
	}

	private static void OnTargetFrameRateChanged(int frameRate)
	{
		Application.targetFrameRate = frameRate;
	}
    
	private void UpdateTextureFiltering(int textureFiltering)
	{
		var filterMode = (FilterMode) textureFiltering;
		Texture2D.defaultFilterMode = filterMode;
		if (Client.Game != null)
		{
			var textures = FindObjectsOfType<Texture>();
			foreach (var t in textures)
			{
				t.filterMode = filterMode;
			}
			Client.Game.GraphicsDevice.Textures[1].UnityTexture.filterMode = FilterMode.Point;
			Client.Game.GraphicsDevice.Textures[2].UnityTexture.filterMode = FilterMode.Point;
			Client.Game.GraphicsDevice.Textures[3].UnityTexture.filterMode = FilterMode.Point;
		}
	}
	
	private void OnUseMouseOnMobileChanged(int useMouse)
	{
		UpdateMovementJoystick();
	}

	private void OnCustomScaleSizeChanged(int customScaleSize)
	{
		ApplyScalingFactor();
	}
	
	private void OnShowCloseButtonsChanged(int showCloseButtons)
	{
		Gump.CloseButtonsEnabled = showCloseButtons != 0;
        
		foreach (var control in UIManager.Gumps)
		{
			if (control is Gump gump)
			{
				gump.UpdateCloseButton();
			}
		}
	}

	private void Update()
	{
		if (Client.Game == null) return;

		if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
		{
			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;
			Client.Game.Window.ClientBounds = new Rectangle(0, 0, Screen.width, Screen.height);
			ApplyScalingFactor();
		}

		if (autologin && Client.Game.Scene is LoginScene)
		{
			ProfileManager.Load(autologinServer, autologinAccount, autologinCharacter);
			Client.Game.UO.World.Mobiles.Add(Client.Game.UO.World.Player = new PlayerMobile(Client.Game.UO.World, 0));
			Client.Game.UO.World.MapIndex = autologinMap;
			Client.Game.UO.World.Player.X = autologinX;
			Client.Game.UO.World.Player.Y = autologinY;
			Client.Game.UO.World.Player.Z = autologinZ;
			Client.Game.UO.World.Player.UpdateScreenPosition();
			Client.Game.UO.World.Player.AddToTile();
			Client.Game.SetScene(new GameScene(Client.Game.UO.World));
		}

		float deltaTime = UnityEngine.Time.deltaTime;
		
		//TODO: Evaluate if this is needed to keep timings in sync
		//NOTE: Removed this for testing...all it seems to do is pretend "everything is fine" when the machine is lagging
		//REMOVED DX4D
		/*
		//Is this necessary? Wouldn't it slow down the game even further when it dips below 20 FPS?
		if(deltaTime > 0.050f)
        {
            deltaTime = 0.050f;
		}*/
		//END REMOVED

        if (movementJoystick.isActiveAndEnabled && Client.Game.Scene is GameScene gameScene)
        {
	        gameScene.JoystickInput = new Microsoft.Xna.Framework.Vector2(movementJoystick.Input.x, -1 * movementJoystick.Input.y);
        }

		var keymod = GetModKeys(); //ADDED DX4D
		//var keycode = GetKeys(); //ADDED DX4D

		Client.Game.KeymodOverride = keymod;
		//Client.Game.EscOverride;
        Client.Game.Tick(deltaTime);
	}
	
	//ADDED DX4D
	SDL.SDL_Keymod GetModKeys()
	{
		var keymod = SDL.SDL_Keymod.KMOD_NONE;
        if (ctrlKeyButtonPresenter.ToggledOn)
        {
	        keymod |= SDL.SDL_Keymod.KMOD_CTRL;
        }
        if (altKeyButtonPresenter.ToggledOn)
        {
	        keymod |= SDL.SDL_Keymod.KMOD_ALT;
        }
        if (shiftKeyButtonPresenter.ToggledOn)
        {
	        keymod |= SDL.SDL_Keymod.KMOD_SHIFT;
        }
		return keymod;
	}
	/*
	SDL.SDL_Keycode GetKeys()
	{
		SDL.SDL_Keycode keycode = SDL.SDL_Keycode.SDLK_F1;
		
		if (f1KeyButtonPresenter.ToggledOn)
        {
	        keycode |= SDL.SDL_Keycode.SDLK_F1;
        }
        if (altKeyButtonPresenter.ToggledOn)
        {
	        keycode |= SDL.SDL_Keycode.SDLK_F2;
        }
        if (shiftKeyButtonPresenter.ToggledOn)
        {
	        keycode |= SDL.SDL_Keycode.SDLK_F3;
        }
		return keycode;
	}*/
	//END ADDED

	private void OnPostRender()
    {
	    if (Client.Game == null) return;
	    
	    GL.LoadPixelMatrix( 0, Screen.width, Screen.height, 0 );
	    
	    //REMOVED DX4D
        // MobileUO: turning off graphics draw texture flag - this fixes some rendering issues where tiles are flipped
	    //Client.Game.Batcher.UseGraphicsDrawTexture = false;//useGraphicsDrawTexture;
	    //END REMOVED
	    //ADDED DX4D
	    if (Application.platform == RuntimePlatform.Android) Client.Game.Batcher.UseGraphicsDrawTexture = false;
	    else Client.Game.Batcher.UseGraphicsDrawTexture = useGraphicsDrawTexture;
	    //END ADDED
	    
	    if (!disableClientRenderer) Client.Game.DrawUnity(UnityEngine.Time.deltaTime);
        
	    autologin = false;
    }

    public void StartGame(ServerConfiguration config)
    {
	    CUOEnviroment.ExecutablePath = config.GetPathToSaveFiles();

	    //Load and adjust settings
	    var settingsFilePath = Settings.GetSettingsFilepath();
	    if (File.Exists(settingsFilePath))
	    {
		    Settings.GlobalSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsFilePath));
	    }
	    else
	    {
		    Settings.GlobalSettings = JsonConvert.DeserializeObject<Settings>(Resources.Load<TextAsset>("settings").text);
	    }

	    Settings.GlobalSettings.IP = config.UoServerUrl;
	    Settings.GlobalSettings.Port = ushort.Parse(config.UoServerPort);
	    
	    //Reset static encryption type variable
	    if (config.UseEncryption)
	    {
	    	Settings.GlobalSettings.Encryption = 1;
	    	NetClient.Socket.Encryption.Type = encryption;
	    }
	    else
	    {
	    	Settings.GlobalSettings.Encryption = 0;
	    }


	    //TODO: Strip out broken plugin system logic
	    
	    
	    //Empty the plugins array because no plugins are working at the moment
	    Settings.GlobalSettings.Plugins = new string[0];
	    
		// MobileUO: this was removed in CUO 1.1.0.x
	    //If connecting to UO Outlands, set shard type to 2 for outlands
	    //Settings.GlobalSettings.ShardType = config.UoServerUrl.ToLower().Contains("uooutlands") ? 2 : 0;

	    //Try to detect old client version to set ShardType to 1, for using StatusGumpOld. Otherwise, it's possible
	    //to get null-refs in StatusGumpModern.
	    //if (ClientVersionHelper.IsClientVersionValid(config.ClientVersion, out var clientVersion))
	    //{
		   // if (clientVersion < ClientVersion.CV_308Z)
		   // {
			  //  Settings.GlobalSettings.ShardType = 1;
		   // }
	    //}
	    
	    //CUOEnviroment.IsOutlands = Settings.GlobalSettings.ShardType == 2;

	    Settings.GlobalSettings.ClientVersion = config.ClientVersion;
	    
	    if (Application.isMobilePlatform == false && string.IsNullOrEmpty(config.ClientPathForUnityEditor) == false)
	    {
		    Settings.GlobalSettings.UltimaOnlineDirectory = config.ClientPathForUnityEditor;
	    }
	    else
	    {
		    Settings.GlobalSettings.UltimaOnlineDirectory = config.GetPathToSaveFiles();
	    }

	    //This flag is tied to whether the GameCursor gets drawn, in a convoluted way
	    //On mobile platform, set this flag to true to prevent the GameCursor from being drawn
	    Settings.GlobalSettings.RunMouseInASeparateThread = Application.isMobilePlatform;

	    //Some mobile specific overrides need to be done on the Profile but they can only be done once the Profile has been loaded
	    ProfileManager.ProfileLoaded += OnProfileLoaded;

	    // Add an audio source and tell the media player to use it for playing sounds
	    Log.Start( LogTypes.All );

	    try
	    {
		    Client.SceneChanged += OnSceneChanged;
			// MobileUO: TODO: will passing null be a problem?
		    Client.Run(null);
#if ENABLE_INTERNAL_ASSISTANT
		    if (UserPreferences.EnableAssistant.CurrentValue == (int) PreferenceEnums.EnableAssistant.On)
		    {
			    Plugin.LoadInternalAssistant();
		    }
#endif
		    Client.Game.Exiting += OnGameExiting;
		    ApplyScalingFactor();

		    if (UserPreferences.ShowModifierKeyButtons.CurrentValue == (int) PreferenceEnums.ShowModifierKeyButtons.On)
		    {
			    modifierKeyButtonsParent.SetActive(true);
		    }
	    }
	    catch (Exception e)
	    {
		    Console.WriteLine(e);
		    OnError?.Invoke(e.ToString());
	    }
    }

    public static void Login()
    {
	    if (Client.Game == null || !(Client.Game.Scene is LoginScene loginScene) || loginScene.CurrentLoginStep != LoginSteps.Main)
	    {
		    return;
	    }
	    var loginGump = UIManager.Gumps.FirstOrDefault(g => g is LoginGump) as LoginGump;
	    loginGump?.OnButtonClick((int) LoginGump.Buttons.NextArrow);
    }

    private void OnProfileLoaded()
    {
	    //Disable auto move on mobile platform
	    ProfileManager.CurrentProfile.DisableAutoMove = Application.isMobilePlatform;
	    //Prevent stack split gump from appearing on mobile
	    //ProfileManager.Current.HoldShiftToSplitStack = Application.isMobilePlatform;
	    //Scale items inside containers by default on mobile (won't have any effect if container scale isn't changed)
	    ProfileManager.CurrentProfile.ScaleItemsInsideContainers = Application.isMobilePlatform;
	    OnForceUseXbrChanged(UserPreferences.ForceUseXbr.CurrentValue);
    }

    private void OnSceneChanged()
    {
	    ApplyScalingFactor();
	    UpdateMovementJoystick();
	    var isGameScene = Client.Game.Scene is GameScene;
	    SceneChanged?.Invoke(isGameScene);
	    if (isGameScene)
	    {
		    OnJoystickRunThresholdChanged(UserPreferences.JoystickRunThreshold.CurrentValue);
	    }
    }

    private void UpdateMovementJoystick()
    {
	    movementJoystick.gameObject.SetActive((Application.isMobilePlatform || showMovementJoystickOnNonMobilePlatforms)
	                                          && Client.Game != null && Client.Game.Scene is GameScene
	                                          && UserPreferences.UseMouseOnMobile.CurrentValue == 0);
    }

    private void ApplyScalingFactor()
    {
	    var scale = 1f;

	    if (Client.Game == null)
	    {
		    return;
	    }

	    var gameScene = Client.Game.Scene as GameScene;
	    var isGameScene = gameScene != null;

	    if (scaleGameToFitScreen)
	    {
		    var loginScale = Mathf.Min(Screen.width / 640f, Screen.height / 480f);
		    var gameScale = Mathf.Max(1, loginScale * 0.75f);
		    scale = isGameScene ? gameScale : loginScale;
	    }

	    if (UserPreferences.ScaleSize.CurrentValue != (int) PreferenceEnums.ScaleSizes.Default && isGameScene)
	    {
		    scale *= UserPreferences.ScaleSize.CurrentValue / 100f;
	    }

	    ((UnityGameWindow) Client.Game.Window).Scale = scale;
	    Client.Game.Batcher.scale = scale;
    }

    private void OnGameExiting(object sender, EventArgs e)
    {
	    Client.Game.UnloadContent();
	    Client.Game.Dispose();
	    OnExiting?.Invoke();
    }
}
