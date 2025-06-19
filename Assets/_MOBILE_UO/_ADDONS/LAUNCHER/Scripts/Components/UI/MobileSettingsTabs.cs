#define ENABLE_INTERNAL_ASSISTANT //uncomment to enable
using System.Collections.Generic;
using DG.Tweening;
using PreferenceEnums;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//public enum MenuAlignment { Left, Right }
	public enum SettingsTab
	{
		None, All,
		Hud, Graphics,
		Input, Joystick,
		Assistant
	}

public class MobileSettingsTabs : MonoBehaviour
{
	[Header("ACTIVE TAB")]
	//SETTINGS
	[SerializeField] SettingsTab _currentTab = SettingsTab.All;
	public SettingsTab CurrentTab => _currentTab;
	public void SetTab(SettingsTab tab)
	{
		if (_currentTab == tab) return;
		_currentTab = tab;
		UpdateMenuOptions(false);
	}
	
	
    [Header("MENU POSITION")]
    //[SerializeField] private MenuAlignment menuAlignment = MenuAlignment.Right;
    [SerializeField] private Vector3 menuOpenPosition;
    [SerializeField] private Vector3 menuClosedPosition;
    [SerializeField] private float menuSpeed = 0.25f;

    [Header("UI COMPONENT LINKS")]
	//[SerializeField] private Button menuButton;
    [SerializeField] private RectTransform menuPanel;
    [SerializeField] private OptionEnumView optionEnumViewInstance;
    [SerializeField] private GameObject customizeJoystickButtonGameObject;
    [SerializeField] private GameObject loginButtonGameObject;

    [Header("CLIENT COMPONENT LINKS")]
    [SerializeField] private ClientRunner clientRunner;

    private readonly List<OptionEnumView> optionEnumViews = new List<OptionEnumView>();
    
    private bool menuOpened;
	
    //EVENTS
	//private void OnEnable() => menuButton?.onClick.AddListener(OnMenuButtonClicked);
	//private void OnDisable() => menuButton?.onClick.RemoveListener(OnMenuButtonClicked);

	//ON VALIDATE
	// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
	protected void OnValidate()
	{
		if (!clientRunner) clientRunner = FindObjectOfType<ClientRunner>();
	}
	//AWAKE
    void Awake()
	{
		//UpdateMenuOptions(false);
		BuildMenu();
		
        clientRunner.SceneChanged += OnUoSceneChanged;
        
		HideOptions();
	}

	//ON UO SCENE CHANGED
    private void OnUoSceneChanged(bool isGameScene)
	{
		//UpdateMenuOptions(isGameScene);
	}
	internal virtual void UpdateMenuOptions(bool inGame)
	{
		ClearMenu();
		BuildMenu();
		/*
		//LOGIN
		loginButtonGameObject.SetActive(inGame == false);
		//JOYSTICK
		customizeJoystickButtonGameObject.SetActive(inGame);
		*/
	}
	
	//CLEAR MENU
	internal virtual void ClearMenu()
	{
		
		//GameObject.DestroyObject(optionEnumViewInstance.gameObject);
		foreach (OptionEnumView view in optionEnumViews)
		{
			//GameObject.DestroyImmediate(view.gameObject);//, 0.01f);
		}
		//optionEnumViews.Clear();/**/
	}
	//BUILD MENU
	internal virtual void BuildMenu()
	{
		///*
		//LOGIN
        //Only show login button when UO client is running and we're in the login scene
        loginButtonGameObject.transform.SetAsFirstSibling();
        loginButtonGameObject.SetActive(false);
	    //JOYSTICK
        //Only show customize joystick button when UO client is running and we're in the game scene
        customizeJoystickButtonGameObject.transform.SetAsLastSibling();
		customizeJoystickButtonGameObject.SetActive(false);
		//*/
		if (CurrentTab == SettingsTab.None) return;
		else if (CurrentTab == SettingsTab.All)
		{
			ShowHudButtons();
			ShowGraphicsButtons();
			ShowInputButtons();
			ShowJoystickButtons();
			ShowAssistantButtons();
		}
		else if (CurrentTab == SettingsTab.Hud) ShowHudButtons();
		else if (CurrentTab == SettingsTab.Graphics) ShowGraphicsButtons();
		else if (CurrentTab == SettingsTab.Input) ShowInputButtons();
		else if (CurrentTab == SettingsTab.Joystick) ShowJoystickButtons();
		else if (CurrentTab == SettingsTab.Assistant) ShowAssistantButtons();
		#if UNITY_EDITOR && DEBUG
		else Debug.Log("<color=red>ISSUE: </color>" + "You need to add a handler here to add a new Settings Tab");
		#endif
	}
	//ADD BUTTON
	internal void AddButton(string labelText, System.Type enumType, UserPreferences.IntPreference intPreference, bool useValuesInsteadOfNames = false, bool usePercentage = false)
	{
		OptionEnumView view = GetOptionEnumViewInstance();
		view.gameObject.SetActive(true);
		view.Initialize(enumType, intPreference, labelText, useValuesInsteadOfNames, usePercentage);
		
	}
	//SHOW/HIDE OPTIONS
	void HideOptions() => ShowOptions(false);
	void ShowOptions(bool show = true)
	{
		optionEnumViewInstance.gameObject.SetActive(show);
	}
    
    
	// S H O W / H I D E  B U T T O N S
	
	//HUD
	void ShowHudButtons(bool show = true)
	{
		if (!show) return;
		
		AddButton("Close Buttons", typeof(ShowCloseButtons), UserPreferences.ShowCloseButtons, false, false);
		AddButton("Container Item Selection", typeof(ContainerItemSelection), UserPreferences.ContainerItemSelection, false, false);
		AddButton("View Scale", typeof(ScaleSizes), UserPreferences.ScaleSize, true, true);
		AddButton("Enlarge Small Buttons", typeof(EnlargeSmallButtons), UserPreferences.EnlargeSmallButtons, false, false);
		AddButton("Show Modifier Key Buttons", typeof(ShowModifierKeyButtons), UserPreferences.ShowModifierKeyButtons, false, false);
	}
	//GRAPHICS
	void ShowGraphicsButtons(bool show = true)
	{
		if (!show) return;
		
		AddButton("Target Frame Rate", typeof(TargetFrameRates), UserPreferences.TargetFrameRate, true, false);
		AddButton("Texture Filtering", typeof(TextureFilterMode), UserPreferences.TextureFiltering, false, false);
		AddButton("Force Use Xbr", typeof(ForceUseXbr), UserPreferences.ForceUseXbr, false, false);
	}
	//INPUT
	void ShowInputButtons(bool show = true)
	{
		if (!show) return;
		
		AddButton("Use Mouse", typeof(UseMouseOnMobile), UserPreferences.UseMouseOnMobile, false, false);
		AddButton("Visualize Finger Input", typeof(VisualizeFingerInput), UserPreferences.VisualizeFingerInput, false, false); //HIDDEN BY DEFAULT
		AddButton("Disable Touchscreen Keyboard", typeof(DisableTouchscreenKeyboardOnMobile), UserPreferences.DisableTouchscreenKeyboardOnMobile, false, false);
	}
	//JOYSTICK
	void ShowJoystickButtons(bool show = true)
	{
		if (!show) return;
		
		AddButton("Joystick Size", typeof(JoystickSizes), UserPreferences.JoystickSize, false, false);
		AddButton("Joystick Opacity", typeof(JoystickOpacity), UserPreferences.JoystickOpacity, false, false);
		AddButton("Joystick DeadZone", typeof(JoystickDeadZone), UserPreferences.JoystickDeadZone, false, false);
		AddButton("Joystick Run Threshold", typeof(JoystickRunThreshold), UserPreferences.JoystickRunThreshold, false, false);
		AddButton("Use Legacy Joystick", typeof(UseLegacyJoystick), UserPreferences.UseLegacyJoystick, false, false);
		AddButton("Joystick Cancels Follow", typeof(JoystickCancelsFollow), UserPreferences.JoystickCancelsFollow, false, false);

	}
	//ASSISTANT
	void ShowAssistantButtons(bool show = true)
	{
		if (!show) return;
		
		#if ENABLE_INTERNAL_ASSISTANT
			AddButton("Enable Assistant", typeof(EnableAssistant), UserPreferences.EnableAssistant, false, false);
		#endif
	}
	


    private OptionEnumView GetOptionEnumViewInstance()
    {
        var instance = Instantiate(optionEnumViewInstance.gameObject, optionEnumViewInstance.transform.parent).GetComponent<OptionEnumView>();
        optionEnumViews.Add(instance);
        return instance;
    }
	/*
    private void OnMenuButtonClicked()
    {
	    //UpdateMenuOptions(false);
        menuOpened = !menuOpened;

        DOTween.Kill(menuPanel);

        if (menuOpened) HideMenu();
        else ShowMenu();
    }
    private void HideMenu()
    {
        Vector3 newPosition = menuClosedPosition;
        MoveMenu(newPosition);
    }
    private void ShowMenu()
	{
        Vector3 newPosition = menuOpenPosition;
        MoveMenu(newPosition);
    }
    private void MoveMenu(Vector3 newPosition)
    {
        //if (menuAlignment == MenuAlignment.Left) newPosition = new Vector3(-newPosition.x, newPosition.y, newPosition.z);
        menuPanel.DOLocalMove(newPosition, menuSpeed);
	}
	*/
}
