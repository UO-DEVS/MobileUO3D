namespace PreferenceEnums
{
    public enum ShowDebugConsole
    {
        Off = 0,
        On = 1
    }
    
	//HUD
    public enum ShowCloseButtons
    {
        Off = 0,
        On = 1
    }
    public enum ContainerItemSelection
    {
        Coarse = 0,
        Fine = 1
    }
    public enum ScaleSizes
    {
        Fifty = 50,
        SeventyFive = 75,
        Default = 100,
        OneTwentyFive = 125,
        OneHalf = 150,
        OneSeventyFive = 175,
	    Two = 200,
	    Three = 300, //ADDED DX4D
	    Four = 400, //ADDED DX4D
	    Five = 500 //ADDED DX4D
    }
    public enum EnlargeSmallButtons
    {
        Off = 0,
        On = 1
    }
    public enum ShowModifierKeyButtons
    {
        Off = 0,
        On = 1
    }

	//GRAPHICS
    public enum TargetFrameRates
    {
        Thirty = 30,
	    Sixty = 60,
	    Ninety = 90, //ADDED DX4D
	    OneTwenty = 120, //ADDED DX4D
	    OneEighty = 180, //ADDED DX4D
	    TwoHundred = 200 //ADDED DX4D
    }
    public enum TextureFilterMode
    {
        Sharp = 0,
        Smooth = 1
    }
    public enum ForceUseXbr
    {
        Off = 0,
        On = 1
    }
    
	//INPUT
    public enum UseMouseOnMobile
    {
        Off = 0,
        On = 1
    }
    public enum VisualizeFingerInput
    {
        Off = 0,
        On = 1
    }
    public enum DisableTouchscreenKeyboardOnMobile
    {
        Off = 0,
        On = 1
    }
	
	//JOYSTICK
    public enum JoystickSizes
    {
        Small = 0,
        Normal = 1,
        Large = 2,
        Custom = 3
    }
    public enum JoystickOpacity
    {
        VeryLow = 0,
        Low = 1,
        Normal = 2,
        High = 3
    }
    public enum JoystickDeadZone
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
    public enum JoystickRunThreshold
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
    public enum UseLegacyJoystick
    {
        Off = 0,
        On = 1
    }
    public enum JoystickCancelsFollow
    {
        Off = 0,
        On = 1
    }

	//ASSISTANT
    public enum EnableAssistant
    {
        Off = 0,
        On = 1
    }
    public enum AssistantMinimized
    {
        Off = 0,
        On = 1
    }
}
