using System.Collections.Generic;
using DG.Tweening;
using PreferenceEnums;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//public enum MenuAlignment { Left, Right }

public class MenuPresenter : MonoBehaviour
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
    private void OnEnable() => menuButton.onClick.AddListener(OnMenuButtonClicked);
    private void OnDisable() => menuButton.onClick.RemoveListener(OnMenuButtonClicked);

    void Awake()
    {
        //Only show login button when UO client is running and we're in the login scene
        loginButtonGameObject.transform.SetAsFirstSibling();
        loginButtonGameObject.SetActive(false);
        
        GetOptionEnumViewInstance().Initialize(typeof(ShowCloseButtons), UserPreferences.ShowCloseButtons, "Close Buttons", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(ScaleSizes), UserPreferences.ScaleSize, "View Scale", true, true);
        GetOptionEnumViewInstance().Initialize(typeof(EnlargeSmallButtons), UserPreferences.EnlargeSmallButtons, "Enlarge Small Buttons", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(ForceUseXbr), UserPreferences.ForceUseXbr, "Force Use Xbr", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(TargetFrameRates), UserPreferences.TargetFrameRate, "Target Frame Rate", true, false);
        GetOptionEnumViewInstance().Initialize(typeof(TextureFilterMode), UserPreferences.TextureFiltering, "Texture Filtering", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(ContainerItemSelection), UserPreferences.ContainerItemSelection, "Container Item Selection", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(UseMouseOnMobile), UserPreferences.UseMouseOnMobile, "Use Mouse", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(DisableTouchscreenKeyboardOnMobile), UserPreferences.DisableTouchscreenKeyboardOnMobile, "Disable Touchscreen Keyboard", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(JoystickSizes), UserPreferences.JoystickSize, "Joystick Size", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(JoystickOpacity), UserPreferences.JoystickOpacity, "Joystick Opacity", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(JoystickDeadZone), UserPreferences.JoystickDeadZone, "Joystick DeadZone", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(JoystickRunThreshold), UserPreferences.JoystickRunThreshold, "Joystick Run Threshold", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(UseLegacyJoystick), UserPreferences.UseLegacyJoystick, "Use Legacy Joystick", false, false);
        GetOptionEnumViewInstance().Initialize(typeof(JoystickCancelsFollow), UserPreferences.JoystickCancelsFollow, "Joystick Cancels Follow", false, false);

        //Only show customize joystick button when UO client is running and we're in the game scene
        customizeJoystickButtonGameObject.transform.SetAsLastSibling();
        customizeJoystickButtonGameObject.SetActive(false);
        
        GetOptionEnumViewInstance().Initialize(typeof(ShowModifierKeyButtons), UserPreferences.ShowModifierKeyButtons, "Show Modifier Key Buttons", false, false);
#if ENABLE_INTERNAL_ASSISTANT
        GetOptionEnumViewInstance().Initialize(typeof(EnableAssistant), UserPreferences.EnableAssistant, "Enable Assistant", false, false);
#endif
        
        //Options that are hidden by default
        GetOptionEnumViewInstance().Initialize(typeof(VisualizeFingerInput), UserPreferences.VisualizeFingerInput, "Visualize Finger Input", false, false);

        clientRunner.SceneChanged += OnUoSceneChanged;
        
        optionEnumViewInstance.gameObject.SetActive(false);
    }   

    private void OnUoSceneChanged(bool isGameScene)
    {
        customizeJoystickButtonGameObject.SetActive(isGameScene);
        loginButtonGameObject.SetActive(isGameScene == false);
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
