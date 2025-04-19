using System.Collections.Generic;
using DG.Tweening;
using PreferenceEnums;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//public enum MenuAlignment { Left, Right }

public class MobileSettingsMenu : MonoBehaviour
{
    [Header("MENU POSITION")]
    //[SerializeField] private MenuAlignment menuAlignment = MenuAlignment.Right;
    [SerializeField] private Vector3 menuOpenPosition;
    [SerializeField] private Vector3 menuClosedPosition;
    [SerializeField] private float menuSpeed = 0.25f;

    [Header("UI COMPONENT LINKS")]
    [SerializeField] private Button menuButton;
    [SerializeField] private RectTransform menuPanel;
    [SerializeField] private OptionEnumView optionEnumViewInstance;
    [SerializeField] private GameObject customizeJoystickButtonGameObject;
    [SerializeField] private GameObject loginButtonGameObject;

    [Header("CLIENT COMPONENT LINKS")]
    [SerializeField] private ClientRunner clientRunner;

    private readonly List<OptionEnumView> optionEnumViews = new List<OptionEnumView>();
    
    private bool menuOpened;

    //EVENTS
	private void OnEnable() => menuButton?.onClick.AddListener(OnMenuButtonClicked);
	private void OnDisable() => menuButton?.onClick.RemoveListener(OnMenuButtonClicked);

	//ON VALIDATE
	// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
	protected void OnValidate()
	{
		if (!clientRunner) clientRunner = FindObjectOfType<ClientRunner>();
	}
	
	//AWAKE
    void Awake()
	{
		BuildMenu();
		
        clientRunner.SceneChanged += OnUoSceneChanged;
        
		HideOptions();
	}
	
	//BUILD MENU
	internal virtual void BuildMenu()
	{
		//LOGIN
        //Only show login button when UO client is running and we're in the login scene
        loginButtonGameObject.transform.SetAsFirstSibling();
        loginButtonGameObject.SetActive(false);
	    //JOYSTICK
        //Only show customize joystick button when UO client is running and we're in the game scene
        customizeJoystickButtonGameObject.transform.SetAsLastSibling();
		customizeJoystickButtonGameObject.SetActive(false);
        
		ShowHudButtons();
		ShowGraphicsButtons();
		ShowInputButtons();
		ShowJoystickButtons();
		ShowAssistantButtons();
	}
	//ADD BUTTON
	internal void AddButton(string labelText, System.Type enumType, UserPreferences.IntPreference intPreference, bool useValuesInsteadOfNames = false, bool usePercentage = false)
	{
		GetOptionEnumViewInstance().Initialize(enumType, intPreference, labelText, useValuesInsteadOfNames, usePercentage);
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
		GetOptionEnumViewInstance().Initialize(typeof(ShowModifierKeyButtons), UserPreferences.ShowModifierKeyButtons, "Show Modifier Key Buttons", false, false);
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
			GetOptionEnumViewInstance().Initialize(typeof(EnableAssistant), UserPreferences.EnableAssistant, "Enable Assistant", false, false);
		#endif
	}

	//ON UO SCENE CHANGED
    private void OnUoSceneChanged(bool isGameScene)
	{
		UpdateMenuOptions(isGameScene);
	}
	

	internal virtual void UpdateMenuOptions(bool inGame)
	{
		//LOGIN
		loginButtonGameObject.SetActive(inGame == false);
		//JOYSTICK
		customizeJoystickButtonGameObject.SetActive(inGame);
	}

    private OptionEnumView GetOptionEnumViewInstance()
    {
        var instance = Instantiate(optionEnumViewInstance.gameObject, optionEnumViewInstance.transform.parent).GetComponent<OptionEnumView>();
        optionEnumViews.Add(instance);
        return instance;
    }

    private void OnMenuButtonClicked()
    {
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
}
