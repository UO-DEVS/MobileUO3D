﻿// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Assets;
using ClassicUO.Configuration;
using ClassicUO.Game;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Game.Scenes;
// MobileUO: import
using ClassicUO.Game.UI.Controls;
using ClassicUO.Game.UI.Gumps;
using ClassicUO.Input;
using ClassicUO.Network;
using ClassicUO.Network.Encryption;
using ClassicUO.Renderer;
using ClassicUO.Resources;
using ClassicUO.Utility;
using ClassicUO.Utility.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using static SDL2.SDL;

namespace ClassicUO
{
    internal unsafe class GameController : Microsoft.Xna.Framework.Game
	{
		const string WINDOW_TITLE = "ClassicUO"; //ADDED DX4D
		
        private SDL_EventFilter _filter;

	    private bool _ignoreNextTextInput;
	    //ADDED DX4D
	    public void IgnoreNextTextInput(bool ignore = true)
	    {
	    	_ignoreNextTextInput = ignore;
	    }
	    //END ADDED
        private readonly float[] _intervalFixedUpdate = new float[2];
        private double _totalElapsed, _currentFpsTime;
        private uint _totalFrames;
        private UltimaBatcher2D _uoSpriteBatch;
        private bool _suppressedDraw;
        private Texture2D _background;
        private bool _pluginsInitialized = false;

        // MobileUO: Batcher and TouchScreenKeyboard
        public UltimaBatcher2D Batcher => _uoSpriteBatch;
        public static UnityEngine.TouchScreenKeyboard TouchScreenKeyboard;

        public GameController(IPluginHost pluginHost)
        {
            GraphicManager = new GraphicsDeviceManager(this);
            // MobileUO: commented out
            //GraphicManager.PreparingDeviceSettings += (sender, e) =>
            //{
            //    e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage =
            //        RenderTargetUsage.DiscardContents;
            //};

            GraphicManager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            SetVSync(false);

            Window.ClientSizeChanged += WindowOnClientSizeChanged;
            Window.AllowUserResizing = true;
            Window.Title = $"ClassicUO - {CUOEnviroment.Version}";
            IsMouseVisible = Settings.GlobalSettings.RunMouseInASeparateThread;

            IsFixedTimeStep = false; // Settings.GlobalSettings.FixedTimeStep;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / 250.0);
            PluginHost = pluginHost;
        }

        public Scene Scene { get; private set; }
        public AudioManager Audio { get; private set; }
	    //ADDED DX4D
	    private UltimaOnline _uo = null;
	    public UltimaOnline UO
	    {
	    	get
	    	{
		    	if (_uo == null) _uo = new UltimaOnline();
		    	return _uo;
	    	}
	    }
	    //END ADDED
	    //public UltimaOnline UO { get; } = new UltimaOnline(); //REMOVED DX4D
        public IPluginHost PluginHost { get; private set; }
        public GraphicsDeviceManager GraphicManager { get; }
        public readonly uint[] FrameDelay = new uint[2];

        private readonly List<(uint, Action)> _queuedActions = new ();

        public void EnqueueAction(uint time, Action action)
        {
            _queuedActions.Add((Time.Ticks + time, action));
        }

        protected override void Initialize()
        {
            // MobileUO: commented out
            //if (GraphicManager.GraphicsDevice.Adapter.IsProfileSupported(GraphicsProfile.HiDef))
            //{
            //    GraphicManager.GraphicsProfile = GraphicsProfile.HiDef;
            //}

            GraphicManager.ApplyChanges();

            SetRefreshRate(Settings.GlobalSettings.FPS);
            _uoSpriteBatch = new UltimaBatcher2D(GraphicsDevice);

            _filter = HandleSdlEvent;
            SDL_SetEventFilter(_filter, IntPtr.Zero);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Fonts.Initialize(GraphicsDevice);
            SolidColorTextureCache.Initialize(GraphicsDevice);
            Audio = new AudioManager();

            // MobileUO: commented out
            //var bytes = Loader.GetBackgroundImage().ToArray();
            //using var ms = new MemoryStream(bytes);
            //_background = Texture2D.FromStream(GraphicsDevice, ms);

#if false
            SetScene(new MainScene(this));
#else
            UO.Load(this);
            Audio.Initialize();
            // TODO: temporary fix to avoid crash when laoding plugins
            Settings.GlobalSettings.Encryption = (byte) NetClient.Socket.Load(UO.FileManager.Version, (EncryptionType) Settings.GlobalSettings.Encryption);

            Log.Trace("Loading plugins...");
            PluginHost?.Initialize();

            foreach (string p in Settings.GlobalSettings.Plugins)
            {
                Plugin.Create(p);
            }
            _pluginsInitialized = true;

            Log.Trace("Done!");

            SetScene(new LoginScene(UO.World));
#endif
            SetWindowPositionBySettings();
        }

        // MobileUO: makes public
        public override void UnloadContent()
        {
            SDL_GetWindowBordersSize(Window.Handle, out int top, out int left, out _, out _);

            Settings.GlobalSettings.WindowPosition = new Point(
                Math.Max(0, Window.ClientBounds.X - left),
                Math.Max(0, Window.ClientBounds.Y - top)
            );

            Audio?.StopMusic();
            Settings.GlobalSettings.Save();
            Plugin.OnClosing();

            UO.Unload();

            // MobileUO: NOTE: My dispose related changes, see if they're still necessary
            // MobileUO: TODO: hueSamplers were moved to Client.cs
            //_hueSamplers[0]?.Dispose();
            //_hueSamplers[0] = null;
            //_hueSamplers[1]?.Dispose();
            //_hueSamplers[1] = null;
            Scene?.Dispose();
            //AuraManager.Dispose();
            UIManager.Dispose();
            SolidColorTextureCache.Dispose();
            RenderedText.Dispose();

            // MobileUO: NOTE: We force the sockets to disconnect in case they haven't already been disposed
            //This is good practice since the Client can be quit while the socket is still active
            // MobileUO: TODO: version 1.0.0.0 drops IsDisposed property
            //if (NetClient.Socket.IsDisposed == false)
            //{
            //    NetClient.Socket.Disconnect();
            //}
            base.UnloadContent();
        }
        
		//ADDED DX4D
		public void SetWindowTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
#if DEV_BUILD
                Window.Title = $"{WINDOW_TITLE} [dev] - {CUOEnviroment.Version}";
#else
                Window.Title = $"{WINDOW_TITLE} - {CUOEnviroment.Version}";
#endif
            }
            else
            {
#if DEV_BUILD
                Window.Title = $"{title} - {WINDOW_TITLE} [dev] - {CUOEnviroment.Version}";
#else
	            Window.Title = $"{title} - {WINDOW_TITLE} - {CUOEnviroment.Version}";
#endif
            }
        }
		//END ADDED
		//REMOVED DX4D
		/*public void SetWindowTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
#if DEV_BUILD
                Window.Title = $"ClassicUO [dev] - {CUOEnviroment.Version}";
#else
                Window.Title = $"ClassicUO - {CUOEnviroment.Version}";
#endif
            }
            else
            {
#if DEV_BUILD
                Window.Title = $"{title} - ClassicUO [dev] - {CUOEnviroment.Version}";
#else
                Window.Title = $"{title} - ClassicUO - {CUOEnviroment.Version}";
#endif
            }
        }*/
		//END REMOVED

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetScene<T>() where T : Scene
        {
            return Scene as T;
        }

        public void SetScene(Scene scene)
        {
            Scene?.Dispose();
            Scene = scene;

            // MobileUO: NOTE: Added this to be able to react to scene changes, mainly for calculating render scale factor
            Client.InvokeSceneChanged();

            Scene?.Load();
        }

        public void SetVSync(bool value)
        {
            GraphicManager.SynchronizeWithVerticalRetrace = value;
        }

        public void SetRefreshRate(int rate)
        {
            if (rate < Constants.MIN_FPS)
            {
                rate = Constants.MIN_FPS;
            }
            else if (rate > Constants.MAX_FPS)
            {
                rate = Constants.MAX_FPS;
            }

            float frameDelay;

            if (rate == Constants.MIN_FPS)
            {
                // The "real" UO framerate is 12.5. Treat "12" as "12.5" to match.
                frameDelay = 80;
            }
            else
            {
                frameDelay = 1000.0f / rate;
            }

            FrameDelay[0] = FrameDelay[1] = (uint)frameDelay;
            FrameDelay[1] = FrameDelay[1] >> 1;

            Settings.GlobalSettings.FPS = rate;

            _intervalFixedUpdate[0] = frameDelay;
            _intervalFixedUpdate[1] = 217; // 5 FPS
        }

        private void SetWindowPosition(int x, int y)
        {
            SDL_SetWindowPosition(Window.Handle, x, y);
        }

        public void SetWindowSize(int width, int height)
        {
            //width = (int) ((double) width * Client.Game.GraphicManager.PreferredBackBufferWidth / Client.Game.Window.ClientBounds.Width);
            //height = (int) ((double) height * Client.Game.GraphicManager.PreferredBackBufferHeight / Client.Game.Window.ClientBounds.Height);

            /*if (CUOEnviroment.IsHighDPI)
            {
                width *= 2;
                height *= 2;
            }
            */

            GraphicManager.PreferredBackBufferWidth = width;
            GraphicManager.PreferredBackBufferHeight = height;
            GraphicManager.ApplyChanges();
        }

        public void SetWindowBorderless(bool borderless)
        {
            SDL_WindowFlags flags = (SDL_WindowFlags)SDL_GetWindowFlags(Window.Handle);

            if ((flags & SDL_WindowFlags.SDL_WINDOW_BORDERLESS) != 0 && borderless)
            {
                return;
            }

            if ((flags & SDL_WindowFlags.SDL_WINDOW_BORDERLESS) == 0 && !borderless)
            {
                return;
            }

            SDL_SetWindowBordered(
                Window.Handle,
                borderless ? SDL_bool.SDL_FALSE : SDL_bool.SDL_TRUE
            );
            SDL_GetCurrentDisplayMode(
                SDL_GetWindowDisplayIndex(Window.Handle),
                out SDL_DisplayMode displayMode
            );

            int width = displayMode.w;
            int height = displayMode.h;

            if (borderless)
            {
                SetWindowSize(width, height);
                SDL_GetDisplayUsableBounds(
                    SDL_GetWindowDisplayIndex(Window.Handle),
                    out SDL_Rect rect
                );
                SDL_SetWindowPosition(Window.Handle, rect.x, rect.y);
            }
            else
            {
                SDL_GetWindowBordersSize(Window.Handle, out int top, out _, out int bottom, out _);

                SetWindowSize(width, height - (top - bottom));
                SetWindowPositionBySettings();
            }

            WorldViewportGump viewport = UIManager.GetGump<WorldViewportGump>();

            if (viewport != null && ProfileManager.CurrentProfile.GameWindowFullSize)
            {
                viewport.ResizeGameWindow(new Point(width, height));
                viewport.X = -5;
                viewport.Y = -5;
            }
        }

        public void MaximizeWindow()
        {
            SDL_MaximizeWindow(Window.Handle);

            GraphicManager.PreferredBackBufferWidth = Client.Game.Window.ClientBounds.Width;
            GraphicManager.PreferredBackBufferHeight = Client.Game.Window.ClientBounds.Height;
            GraphicManager.ApplyChanges();
        }

        public bool IsWindowMaximized()
        {
            SDL_WindowFlags flags = (SDL_WindowFlags)SDL_GetWindowFlags(Window.Handle);

            return (flags & SDL_WindowFlags.SDL_WINDOW_MAXIMIZED) != 0;
        }

        public void RestoreWindow()
        {
            SDL_RestoreWindow(Window.Handle);
        }

        public void SetWindowPositionBySettings()
        {
            SDL_GetWindowBordersSize(Window.Handle, out int top, out int left, out _, out _);

            if (Settings.GlobalSettings.WindowPosition.HasValue)
            {
                int x = left + Settings.GlobalSettings.WindowPosition.Value.X;
                int y = top + Settings.GlobalSettings.WindowPosition.Value.Y;
                x = Math.Max(0, x);
                y = Math.Max(0, y);

                SetWindowPosition(x, y);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Profiler.InContext("OutOfContext"))
            {
                Profiler.ExitContext("OutOfContext");
            }

            Time.Ticks = (uint)gameTime.TotalGameTime.TotalMilliseconds;
            Time.Delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // MobileUO: new MouseUpdate function
            // Mouse.Update();
            MouseUpdate();

            var data = NetClient.Socket.CollectAvailableData();
            var packetsCount = PacketHandlers.Handler.ParsePackets(NetClient.Socket, UO.World, data);

            NetClient.Socket.Statistics.TotalPacketsReceived += (uint)packetsCount;
            NetClient.Socket.Flush();

            Plugin.Tick();

            if (Scene != null && Scene.IsLoaded && !Scene.IsDestroyed)
            {
                Profiler.EnterContext("Update");
                Scene.Update();
                Profiler.ExitContext("Update");
            }

            // MobileUO: Unity input
            UnityInputUpdate();

            UIManager.Update();

            _totalElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
            _currentFpsTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_currentFpsTime >= 1000)
            {
                CUOEnviroment.CurrentRefreshRate = _totalFrames;

                _totalFrames = 0;
                _currentFpsTime = 0;
            }

            double x = _intervalFixedUpdate[
                !IsActive
                && ProfileManager.CurrentProfile != null
                && ProfileManager.CurrentProfile.ReduceFPSWhenInactive
                    ? 1
                    : 0
            ];
            _suppressedDraw = false;

            if (_totalElapsed > x)
            {
                _totalElapsed %= x;
            }
            else
            {
                _suppressedDraw = true;
                SuppressDraw();

                if (!gameTime.IsRunningSlowly)
                {
                    Thread.Sleep(1);
                }
            }

            UO.GameCursor?.Update();
            Audio?.Update();


            for (var i = _queuedActions.Count - 1; i >= 0; i--)
            {
                (var time, var fn) = _queuedActions[i];

                if (Time.Ticks > time)
                {
                    fn();
                    _queuedActions.RemoveAt(i);
                    break;
                }
            }

             base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Profiler.EndFrame();
            Profiler.BeginFrame();

            if (Profiler.InContext("OutOfContext"))
            {
                Profiler.ExitContext("OutOfContext");
            }

            Profiler.EnterContext("RenderFrame");

            _totalFrames++;

            GraphicsDevice.Clear(Color.Black);

            _uoSpriteBatch.Begin();
            // MobileUO: commented out
            //var rect = new Rectangle(
            //    0,
            //    0,
            //    GraphicManager.PreferredBackBufferWidth,
            //    GraphicManager.PreferredBackBufferHeight
            //);
            //_uoSpriteBatch.DrawTiled(
            //    _background,
            //    rect,
            //    _background.Bounds,
            //    new Vector3(0, 0, 0.1f)
            //);
            _uoSpriteBatch.End();

            if (Scene != null && Scene.IsLoaded && !Scene.IsDestroyed)
            {
                Scene.Draw(_uoSpriteBatch);
            }

            UIManager.Draw(_uoSpriteBatch);

            if ((UO.World?.InGame ?? false) && SelectedObject.Object is TextObject t)
            {
                if (t.IsTextGump)
                {
                    t.ToTopD();
                }
                else
                {
                    UO.World.WorldTextManager?.MoveToTop(t);
                }
            }

            SelectedObject.HealthbarObject = null;
            SelectedObject.SelectedContainer = null;

            _uoSpriteBatch.Begin();
            UO.GameCursor?.Draw(_uoSpriteBatch);
            _uoSpriteBatch.End();

            Profiler.ExitContext("RenderFrame");
            Profiler.EnterContext("OutOfContext");

            Plugin.ProcessDrawCmdList(GraphicsDevice);

            base.Draw(gameTime);
        }

        // MobileUO: commented out
        // MobileUO: TODO: do we need to implement it?
        //protected override bool BeginDraw()
        //{
        //    return !_suppressedDraw && base.BeginDraw();
        //}

        private void WindowOnClientSizeChanged(object sender, EventArgs e)
        {
            int width = Window.ClientBounds.Width;
            int height = Window.ClientBounds.Height;

            if (!IsWindowMaximized())
            {
                if (ProfileManager.CurrentProfile != null)
                    ProfileManager.CurrentProfile.WindowClientBounds = new Point(width, height);
            }

            SetWindowSize(width, height);

            WorldViewportGump viewport = UIManager.GetGump<WorldViewportGump>();

            if (viewport != null && ProfileManager.CurrentProfile != null && ProfileManager.CurrentProfile.GameWindowFullSize)
            {
                viewport.ResizeGameWindow(new Point(width, height));
                viewport.X = -5;
                viewport.Y = -5;
            }
        }

        // MobileUO: NOTE: SDL events are not handled in Unity! This function will NOT be hit!
        private int HandleSdlEvent(IntPtr userData, IntPtr ptr)
        {
            SDL_Event* sdlEvent = (SDL_Event*)ptr;

            // Don't pass SDL events to the plugin host before the plugins are initialized
            // or the garbage collector can get screwed up
            if (_pluginsInitialized && Plugin.ProcessWndProc(sdlEvent) != 0)
            {
                if (sdlEvent->type == SDL_EventType.SDL_MOUSEMOTION)
                {
                    if (UO.GameCursor != null)
                    {
                        UO.GameCursor.AllowDrawSDLCursor = false;
                    }
                }

                return 1;
            }

            switch (sdlEvent->type)
            {
                case SDL_EventType.SDL_AUDIODEVICEADDED:
                    Console.WriteLine("AUDIO ADDED: {0}", sdlEvent->adevice.which);

                    break;

                case SDL_EventType.SDL_AUDIODEVICEREMOVED:
                    Console.WriteLine("AUDIO REMOVED: {0}", sdlEvent->adevice.which);

                    break;

                case SDL_EventType.SDL_WINDOWEVENT:

                    switch (sdlEvent->window.windowEvent)
                    {
                        case SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
                            Mouse.MouseInWindow = true;

                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
                            Mouse.MouseInWindow = false;

                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                            Plugin.OnFocusGained();

                            break;

                        case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                            Plugin.OnFocusLost();

                            break;
                    }

                    break;

                case SDL_EventType.SDL_KEYDOWN:

                    Keyboard.OnKeyDown(sdlEvent->key);

                    if (
                        Plugin.ProcessHotkeys(
                            (int)sdlEvent->key.keysym.sym,
                            (int)sdlEvent->key.keysym.mod,
                            true
                        )
                    )
                    {
                        _ignoreNextTextInput = false;

                        UIManager.KeyboardFocusControl?.InvokeKeyDown(
                            sdlEvent->key.keysym.sym,
                            sdlEvent->key.keysym.mod
                        );

                        Scene.OnKeyDown(sdlEvent->key);
                    }
                    else
                    {
                        _ignoreNextTextInput = true;
                    }

                    break;

                case SDL_EventType.SDL_KEYUP:

                    Keyboard.OnKeyUp(sdlEvent->key);
                    UIManager.KeyboardFocusControl?.InvokeKeyUp(
                        sdlEvent->key.keysym.sym,
                        sdlEvent->key.keysym.mod
                    );
                    Scene.OnKeyUp(sdlEvent->key);
                    Plugin.ProcessHotkeys(0, 0, false);

                    if (sdlEvent->key.keysym.sym == SDL_Keycode.SDLK_PRINTSCREEN)
                    {
                        // MobileUO: commented out
                        // TakeScreenshot();
                    }

                    break;

                case SDL_EventType.SDL_TEXTINPUT:

                    if (_ignoreNextTextInput)
                    {
                        break;
                    }

                    // Fix for linux OS: https://github.com/andreakarasho/ClassicUO/pull/1263
                    // Fix 2: SDL owns this behaviour. Cheating is not a real solution.
                    /*if (!Utility.Platforms.PlatformHelper.IsWindows)
                    {
                        if (Keyboard.Alt || Keyboard.Ctrl)
                        {
                            break;
                        }
                    }*/

                    string s = UTF8_ToManaged((IntPtr)sdlEvent->text.text, false);

                    if (!string.IsNullOrEmpty(s))
                    {
                        UIManager.KeyboardFocusControl?.InvokeTextInput(s);
                        Scene.OnTextInput(s);
                    }

                    break;

                case SDL_EventType.SDL_MOUSEMOTION:

                    if (UO.GameCursor != null && !UO.GameCursor.AllowDrawSDLCursor)
                    {
                        UO.GameCursor.AllowDrawSDLCursor = true;
                        UO.GameCursor.Graphic = 0xFFFF;
                    }

                    Mouse.Update();

                    if (Mouse.IsDragging)
                    {
                        if (!Scene.OnMouseDragging())
                        {
                            UIManager.OnMouseDragging();
                        }
                    }

                    break;

                case SDL_EventType.SDL_MOUSEWHEEL:
                    Mouse.Update();
                    bool isScrolledUp = sdlEvent->wheel.y > 0;

                    Plugin.ProcessMouse(0, sdlEvent->wheel.y);

                    if (!Scene.OnMouseWheel(isScrolledUp))
                    {
                        UIManager.OnMouseWheel(isScrolledUp);
                    }

                    break;

                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                {
                    SDL_MouseButtonEvent mouse = sdlEvent->button;

                    // The values in MouseButtonType are chosen to exactly match the SDL values
                    MouseButtonType buttonType = (MouseButtonType)mouse.button;

                    uint lastClickTime = 0;

                    switch (buttonType)
                    {
                        case MouseButtonType.Left:
                            lastClickTime = Mouse.LastLeftButtonClickTime;

                            break;

                        case MouseButtonType.Middle:
                            lastClickTime = Mouse.LastMidButtonClickTime;

                            break;

                        case MouseButtonType.Right:
                            lastClickTime = Mouse.LastRightButtonClickTime;

                            break;

                        case MouseButtonType.XButton1:
                        case MouseButtonType.XButton2:
                            break;

                        default:
                            Log.Warn($"No mouse button handled: {mouse.button}");

                            break;
                    }

                    Mouse.ButtonPress(buttonType);
                    Mouse.Update();

                    uint ticks = Time.Ticks;

                    if (lastClickTime + Mouse.MOUSE_DELAY_DOUBLE_CLICK >= ticks)
                    {
                        lastClickTime = 0;

                        bool res =
                            Scene.OnMouseDoubleClick(buttonType)
                            || UIManager.OnMouseDoubleClick(buttonType);

                        if (!res)
                        {
                            if (!Scene.OnMouseDown(buttonType))
                            {
                                UIManager.OnMouseButtonDown(buttonType);
                            }
                        }
                        else
                        {
                            lastClickTime = 0xFFFF_FFFF;
                        }
                    }
                    else
                    {
                        if (
                            buttonType != MouseButtonType.Left
                            && buttonType != MouseButtonType.Right
                        )
                        {
                            Plugin.ProcessMouse(sdlEvent->button.button, 0);
                        }

                        if (!Scene.OnMouseDown(buttonType))
                        {
                            UIManager.OnMouseButtonDown(buttonType);
                        }

                        lastClickTime = Mouse.CancelDoubleClick ? 0 : ticks;
                    }

                    switch (buttonType)
                    {
                        case MouseButtonType.Left:
                            Mouse.LastLeftButtonClickTime = lastClickTime;

                            break;

                        case MouseButtonType.Middle:
                            Mouse.LastMidButtonClickTime = lastClickTime;

                            break;

                        case MouseButtonType.Right:
                            Mouse.LastRightButtonClickTime = lastClickTime;

                            break;
                    }

                    break;
                }

                case SDL_EventType.SDL_MOUSEBUTTONUP:
                {
                    SDL_MouseButtonEvent mouse = sdlEvent->button;

                    // The values in MouseButtonType are chosen to exactly match the SDL values
                    MouseButtonType buttonType = (MouseButtonType)mouse.button;

                    uint lastClickTime = 0;

                    switch (buttonType)
                    {
                        case MouseButtonType.Left:
                            lastClickTime = Mouse.LastLeftButtonClickTime;

                            break;

                        case MouseButtonType.Middle:
                            lastClickTime = Mouse.LastMidButtonClickTime;

                            break;

                        case MouseButtonType.Right:
                            lastClickTime = Mouse.LastRightButtonClickTime;

                            break;

                        default:
                            Log.Warn($"No mouse button handled: {mouse.button}");

                            break;
                    }

                    if (lastClickTime != 0xFFFF_FFFF)
                    {
                        if (
                            !Scene.OnMouseUp(buttonType)
                            || UIManager.LastControlMouseDown(buttonType) != null
                        )
                        {
                            UIManager.OnMouseButtonUp(buttonType);
                        }
                    }

                    Mouse.ButtonRelease(buttonType);
                    Mouse.Update();

                    break;
                }
            }

            return 1;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Scene?.Dispose();

            base.OnExiting(sender, args);
        }

        // MobileUO: commented out
        //private void TakeScreenshot()
        //{
        //    string screenshotsFolder = FileSystemHelper.CreateFolderIfNotExists
        //        (CUOEnviroment.ExecutablePath, "Data", "Client", "Screenshots");

        //    string path = Path.Combine(screenshotsFolder, $"screenshot_{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.png");

        //    Color[] colors =
        //        new Color[GraphicManager.PreferredBackBufferWidth * GraphicManager.PreferredBackBufferHeight];

        //    GraphicsDevice.GetBackBufferData(colors);

        //    using (Texture2D texture = new Texture2D
        //    (
        //        GraphicsDevice, GraphicManager.PreferredBackBufferWidth, GraphicManager.PreferredBackBufferHeight,
        //        false, SurfaceFormat.Color
        //    ))
        //    using (FileStream fileStream = File.Create(path))
        //    {
        //        texture.SetData(colors);
        //        texture.SaveAsPng(fileStream, texture.Width, texture.Height);
        //        string message = string.Format(ResGeneral.ScreenshotStoredIn0, path);

        //        if (ProfileManager.CurrentProfile == null || ProfileManager.CurrentProfile.HideScreenshotStoredInMessage)
        //        {
        //            Log.Info(message);
        //        }
        //        else
        //        {
        //            GameActions.Print(message, 0x44, MessageType.System);
        //        }
        //    }
        //}

        // MobileUO: here to end of file for Unity functions to help support inputs
        private readonly UnityEngine.KeyCode[] _keyCodeEnumValues = (UnityEngine.KeyCode[]) Enum.GetValues(typeof(UnityEngine.KeyCode));
        private UnityEngine.Vector3 lastMousePosition;
        public SDL_Keymod KeymodOverride;
	    public bool EscOverride;
        
	    //ADDED DX4D
	    //F KEYS
	    public bool F1Override;
	    public bool F2Override;
	    public bool F3Override;
	    public bool F4Override;
	    
	    public bool F5Override;
	    public bool F6Override;
	    public bool F7Override;
	    public bool F8Override;
	    
	    public bool F9Override;
	    public bool F10Override;
	    public bool F11Override;
	    public bool F12Override;
	    //NUM KEYS
	    public bool NUM1Override;
	    public bool NUM2Override;
	    public bool NUM3Override;
	    public bool NUM4Override;
	    
	    public bool NUM5Override;
	    public bool NUM6Override;
	    public bool NUM7Override;
	    public bool NUM8Override;
	    
	    public bool NUM9Override;
	    public bool NUM0Override;
	    //END ADDED
        
        private int zoomCounter;

        private void MouseUpdate()
        {
            var oneOverScale = 1f / Batcher.scale;
            
            //Finger/mouse handling
            if (UnityEngine.Application.isMobilePlatform && UserPreferences.UseMouseOnMobile.CurrentValue == 0)
            {
                var fingers = Lean.Touch.LeanTouch.GetFingers(true, false);

                //Only process one finger that has not started over gui because using multiple fingers with UIManager
                //causes issues due to the assumption that there's only one pointer, such as on finger "stealing"
                //a dragged gump from another
                if (fingers.Count > 0)
                {
                    var finger = fingers[0];
                    
                    var leftMouseDown = finger.Down;
                    var leftMouseHeld = finger.Set;

                    var mousePositionPoint = ConvertUnityMousePosition(finger.ScreenPosition, oneOverScale);
                    Mouse.Position = mousePositionPoint;
                    Mouse.LButtonPressed = leftMouseDown || leftMouseHeld;
                    Mouse.RButtonPressed = false;
                    Mouse.IsDragging = Mouse.LButtonPressed || Mouse.RButtonPressed;
                    //Mouse.RealPosition = Mouse.Position;
                }
            }
            else
            {
                var leftMouseDown = UnityEngine.Input.GetMouseButtonDown(0);
                var leftMouseHeld = UnityEngine.Input.GetMouseButton(0);
                var rightMouseDown = UnityEngine.Input.GetMouseButtonDown(1);
                var rightMouseHeld = UnityEngine.Input.GetMouseButton(1);
                var mousePosition = UnityEngine.Input.mousePosition;

                if (Lean.Touch.LeanTouch.PointOverGui(mousePosition))
                {
                    Mouse.Position.X = 0;
                    Mouse.Position.Y = 0;
                    leftMouseDown = false;
                    leftMouseHeld = false;
                    rightMouseDown = false;
                    rightMouseHeld = false;
                }
                
                var mousePositionPoint = ConvertUnityMousePosition(mousePosition, oneOverScale);
                Mouse.Position = mousePositionPoint;
                Mouse.LButtonPressed = leftMouseDown || leftMouseHeld;
                Mouse.RButtonPressed = rightMouseDown || rightMouseHeld;
                Mouse.IsDragging = Mouse.LButtonPressed || Mouse.RButtonPressed;
                //Mouse.RealPosition = Mouse.Position;
            }
        }

        private void UnityInputUpdate()
        {
            var oneOverScale = 1f / Batcher.scale;
            
            //Finger/mouse handling
            if (UnityEngine.Application.isMobilePlatform && UserPreferences.UseMouseOnMobile.CurrentValue == 0)
            {
                var fingers = Lean.Touch.LeanTouch.GetFingers(true, false);

                //Detect two finger tap gesture for closing gumps, only when one of the fingers' state is Down
                if (fingers.Count == 2 && (fingers[0].Down || fingers[1].Down))
                {
                    var firstMousePositionPoint = ConvertUnityMousePosition(fingers[0].ScreenPosition, oneOverScale);
                    var secondMousePositionPoint = ConvertUnityMousePosition(fingers[1].ScreenPosition, oneOverScale);
                    var firstControlUnderFinger = UIManager.GetMouseOverControl(firstMousePositionPoint);
                    var secondControlUnderFinger = UIManager.GetMouseOverControl(secondMousePositionPoint);
                    //We prefer to get the root parent but sometimes it can be null (like with GridLootGump), in which case we revert to the initially found control
                    firstControlUnderFinger = firstControlUnderFinger?.RootParent ?? firstControlUnderFinger;
                    secondControlUnderFinger = secondControlUnderFinger?.RootParent ?? secondControlUnderFinger;
                    if (firstControlUnderFinger != null && firstControlUnderFinger == secondControlUnderFinger)
                    {
                        //Simulate right mouse down and up
                        SimulateMouse(false, false, true, false, false, true);
                        SimulateMouse(false, false, false, true, false, true);
                    }
                }
                //Only process one finger that has not started over gui because using multiple fingers with UIManager
                //causes issues due to the assumption that there's only one pointer, such as one finger "stealing" a
                //dragged gump from another
                else if (fingers.Count > 0)
                {
                    var finger = fingers[0];
                    var mouseMotion = finger.ScreenPosition != finger.LastScreenPosition;
                    SimulateMouse(finger.Down, finger.Up, false, false, mouseMotion, false);
                }
                
                if (fingers.Count == 2 && ProfileManager.CurrentProfile.EnableMousewheelScaleZoom && UIManager.IsMouseOverWorld)
                {                    
                    var scale = Lean.Touch.LeanGesture.GetPinchScale(fingers);                  
                    if(scale < 1)
                    {
                        zoomCounter--;
                    }
                    else if(scale > 1)
                    {
                        zoomCounter++;
                    }

                    if(zoomCounter > 3)
                    {
                        zoomCounter = 0;
                        --Client.Game.Scene.Camera.Zoom;
                    }
                    else if(zoomCounter < -3)
                    {
                        zoomCounter = 0;
                        ++Client.Game.Scene.Camera.Zoom;
                    }
                }

            }
            else
            {
                var leftMouseDown = UnityEngine.Input.GetMouseButtonDown(0);
                var leftMouseUp = UnityEngine.Input.GetMouseButtonUp(0);
                var rightMouseDown = UnityEngine.Input.GetMouseButtonDown(1);
                var rightMouseUp = UnityEngine.Input.GetMouseButtonUp(1);
                var mousePosition = UnityEngine.Input.mousePosition;
                var mouseMotion = mousePosition != lastMousePosition;
                lastMousePosition = mousePosition;
                
                if (Lean.Touch.LeanTouch.PointOverGui(mousePosition))
                {
                    Mouse.Position.X = 0;
                    Mouse.Position.Y = 0;
                    leftMouseDown = false;
                    leftMouseUp = false;
                    rightMouseDown = false;
                    rightMouseUp = false;
                }
                
                SimulateMouse(leftMouseDown, leftMouseUp, rightMouseDown, rightMouseUp, mouseMotion, false);
            }


	        var keymod = GetModKeys(); //ADDED DX4D
	        //var keycode = GetKeys(); //ADDED DX4D
            
            Keyboard.Shift = (keymod & SDL_Keymod.KMOD_SHIFT) != SDL_Keymod.KMOD_NONE;
            Keyboard.Alt = (keymod & SDL_Keymod.KMOD_ALT) != SDL_Keymod.KMOD_NONE;
            Keyboard.Ctrl = (keymod & SDL_Keymod.KMOD_CTRL) != SDL_Keymod.KMOD_NONE;
            
            foreach (var keyCode in _keyCodeEnumValues)
            {
                var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) keyCode, mod = keymod}};
                if (UnityEngine.Input.GetKeyDown(keyCode))
                {
                    Keyboard.OnKeyDown(key);

                    if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
                    {
                        _ignoreNextTextInput = false;
                        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
                        Scene.OnKeyDown(key);
                    }
                    else
                        _ignoreNextTextInput = true;
                }
                if (UnityEngine.Input.GetKeyUp(keyCode))
                {
                    Keyboard.OnKeyUp(key);
                    UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
                    Scene.OnKeyUp(key);
                    Plugin.ProcessHotkeys(0, 0, false);
                }
            }

            if (EscOverride)
            {
	            EscOverride = false;
            	PressKey(UnityEngine.KeyCode.Escape);
            }
            
	        //ADDED DX4D
	        if (NUM1Override)
	        {
		        NUM1Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha1);
	        }
	        if (NUM2Override)
	        {
		        NUM2Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha2);
	        }
	        if (NUM3Override)
	        {
		        NUM3Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha3);
	        }
	        if (NUM4Override)
	        {
		        NUM4Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha4);
	        }
	        if (NUM5Override)
	        {
		        NUM5Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha5);
	        }
	        if (NUM6Override)
	        {
		        NUM6Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha6);
	        }
	        if (NUM7Override)
	        {
		        NUM7Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha7);
	        }
	        if (NUM8Override)
	        {
		        NUM8Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha8);
	        }
	        if (NUM9Override)
	        {
		        NUM9Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha9);
	        }
	        if (NUM0Override)
	        {
		        NUM0Override = false;
		        PressKey(UnityEngine.KeyCode.Alpha0);
	        }
	        // F1 > F4
	        //F1
	        if (F1Override)
	        {
		        F1Override = false;
		        PressKey(UnityEngine.KeyCode.F1);
	        }
	        //F2
	        if (F2Override)
	        {
		        F2Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F2;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F3
	        if (F3Override)
	        {
		        F3Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F3;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F4
	        if (F1Override)
	        {
		        F4Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F4;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F5
	        if (F5Override)
	        {
		        F5Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F5;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F6
	        if (F6Override)
	        {
		        F6Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F6;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F7
	        if (F7Override)
	        {
		        F7Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F7;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F8
	        if (F8Override)
	        {
		        F8Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F8;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F9
	        if (F9Override)
	        {
		        F9Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F9;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F10
	        if (F10Override)
	        {
		        F10Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F10;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F11
	        if (F11Override)
	        {
		        F11Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F11;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //F12
	        if (F12Override)
	        {
		        F12Override = false;
		        UnityEngine.KeyCode activationkey = UnityEngine.KeyCode.F12;
		        
		        var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) activationkey, mod = keymod}};
		        //KEY DOWN
		        // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		        {
			        Keyboard.OnKeyDown(key);

			        if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			        {
				        _ignoreNextTextInput = false;
				        UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				        Scene.OnKeyDown(key);
			        }
			        else
				        _ignoreNextTextInput = true;
		        }
		        //KEY UP
		        // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		        {
			        Keyboard.OnKeyUp(key);
			        UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			        Scene.OnKeyUp(key);
			        Plugin.ProcessHotkeys(0, 0, false);
		        }
	        }
	        //END ADDED

            //Input text handling
            if (UnityEngine.Application.isMobilePlatform && TouchScreenKeyboard != null)
            {
                var text = TouchScreenKeyboard.text;
                
                if (_ignoreNextTextInput == false && TouchScreenKeyboard.status == UnityEngine.TouchScreenKeyboard.Status.Done)
                {
                    //Clear the text of TouchScreenKeyboard, otherwise it stays there and is re-evaluated every frame
                    TouchScreenKeyboard.text = string.Empty;
                    
                    //Set keyboard to null so we process its text only once when its status is set to Done
                    TouchScreenKeyboard = null;
                    
                    //Need to clear the existing text in textbox before "pasting" new text from TouchScreenKeyboard
                    if (UIManager.KeyboardFocusControl is StbTextBox stbTextBox)
                    {
                        stbTextBox.SetText(string.Empty);
                    }
                    
                    UIManager.KeyboardFocusControl?.InvokeTextInput(text);
                    Scene.OnTextInput(text);
                    
                    //When targeting SystemChat textbox, "auto-press" return key so that the text entered on the TouchScreenKeyboard is submitted right away
                    if (UIManager.KeyboardFocusControl != null && UIManager.KeyboardFocusControl == UIManager.SystemChat?.TextBoxControl)
                    {
                        //Handle different chat modes
                        HandleChatMode(text);
                        //"Press" return
                        UIManager.KeyboardFocusControl.InvokeKeyDown(SDL_Keycode.SDLK_RETURN, SDL_Keymod.KMOD_NONE);
                        //Revert chat mode to default
                        UIManager.SystemChat.Mode = ChatMode.Default;
                    }
                }
            }
            else
            {
                var text = UnityEngine.Input.inputString;
                //Backspace character should not be sent as text input
                text = text.Replace("\b", "");
                if (_ignoreNextTextInput == false && string.IsNullOrEmpty(text) == false)
                {
                    UIManager.KeyboardFocusControl?.InvokeTextInput(text);
                    Scene.OnTextInput(text);
                }
            }
        }
        
        
	    //ADDED DX4D
	    void PressKey(UnityEngine.KeyCode keyToPress)
	    {
	    	var modkeys = GetModKeys();
		    var key = new SDL_KeyboardEvent {keysym = new SDL_Keysym {sym = (SDL_Keycode) keyToPress, mod = modkeys}};
		    
		    // if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		    {
			    Keyboard.OnKeyDown(key);

			    if (Plugin.ProcessHotkeys((int) key.keysym.sym, (int) key.keysym.mod, true))
			    {
				    _ignoreNextTextInput = false;
				    UIManager.KeyboardFocusControl?.InvokeKeyDown(key.keysym.sym, key.keysym.mod);
				    Scene.OnKeyDown(key);
			    }
			    else
				    _ignoreNextTextInput = true;
		    }
		    // if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		    {
			    Keyboard.OnKeyUp(key);
			    UIManager.KeyboardFocusControl?.InvokeKeyUp(key.keysym.sym, key.keysym.mod);
			    Scene.OnKeyUp(key);
			    Plugin.ProcessHotkeys(0, 0, false);
		    }
	    }
	    
	    SDL2.SDL.SDL_Keymod GetModKeys()
	    {
		    //Keyboard handling
		    var keymod = KeymodOverride;
		    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftAlt))
		    {
			    keymod |= SDL_Keymod.KMOD_LALT;
		    }
		    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightAlt))
		    {
			    keymod |= SDL_Keymod.KMOD_RALT;
		    }
		    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift))
		    {
			    keymod |= SDL_Keymod.KMOD_LSHIFT;
		    }
		    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift))
		    {
			    keymod |= SDL_Keymod.KMOD_RSHIFT;
		    }
		    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl))
		    {
			    keymod |= SDL_Keymod.KMOD_LCTRL;
		    }
		    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightControl))
		    {
			    keymod |= SDL_Keymod.KMOD_RCTRL;
		    }
		    return keymod;
	    }
	    /*
	    SDL2.SDL.SDL_Keycode GetKeys()
	    {
		    SDL_Keycode keycode = SDL2.SDL.SDL_Keycode.SDLK_F1;
		
		    if (ctrlKeyButtonPresenter.ToggledOn)
		    {
			    keycode |= SDL_Keycode.SDLK_F1;
		    }
		    if (altKeyButtonPresenter.ToggledOn)
		    {
			    keycode |= SDL_Keycode.SDLK_F2;
		    }
		    if (shiftKeyButtonPresenter.ToggledOn)
		    {
			    keycode |= SDL_Keycode.SDLK_F3;
		    }
		    return keycode;
	    }
	    */
	    //END ADDED

        private void HandleChatMode(string text)
        {
            if (text.Length > 0)
            {
                switch (text[0])
                {                  
                    case '/':
                        UIManager.SystemChat.Mode = ChatMode.Party;
                        //Textbox text has been cleared, set it again
                        UIManager.SystemChat.TextBoxControl.InvokeTextInput(text.Substring(1));
                        break;
                    case '\\':
                        UIManager.SystemChat.Mode = ChatMode.Guild;
                        //Textbox text has been cleared, set it again
                        UIManager.SystemChat.TextBoxControl.InvokeTextInput(text.Substring(1));
                        break;
                    case '|':
                        UIManager.SystemChat.Mode = ChatMode.Alliance;
                        //Textbox text has been cleared, set it again
                        UIManager.SystemChat.TextBoxControl.InvokeTextInput(text.Substring(1));
                        break;
                    case '-':
                        UIManager.SystemChat.Mode = ChatMode.ClientCommand;
                        //Textbox text has been cleared, set it again
                        UIManager.SystemChat.TextBoxControl.InvokeTextInput(text.Substring(1));
                        break;
                    case ',' when UO.World.ChatManager.ChatIsEnabled == ChatStatus.Enabled:
                        UIManager.SystemChat.Mode = ChatMode.UOChat;
                        //Textbox text has been cleared, set it again
                        UIManager.SystemChat.TextBoxControl.InvokeTextInput(text.Substring(1));
                        break;
                    case ':' when text.Length > 1 && text[1] == ' ':
                        UIManager.SystemChat.Mode = ChatMode.Emote;
                        //Textbox text has been cleared, set it again
                        UIManager.SystemChat.TextBoxControl.InvokeTextInput(text.Substring(2));
                        break;
                    case ';' when text.Length > 1 && text[1] == ' ':
                        UIManager.SystemChat.Mode = ChatMode.Whisper;
                        //Textbox text has been cleared, set it again
                        UIManager.SystemChat.TextBoxControl.InvokeTextInput(text.Substring(2));
                        break;
                    case '!' when text.Length > 1 && text[1] == ' ':
                        UIManager.SystemChat.Mode = ChatMode.Yell;
                        //Textbox text has been cleared, set it again
                        UIManager.SystemChat.TextBoxControl.InvokeTextInput(text.Substring(2));
                        break;
                }
            }
        }

        private static Point ConvertUnityMousePosition(UnityEngine.Vector2 screenPosition, float oneOverScale)
        {
            var x = UnityEngine.Mathf.RoundToInt(screenPosition.x * oneOverScale);
            var y = UnityEngine.Mathf.RoundToInt((UnityEngine.Screen.height - screenPosition.y) * oneOverScale);
            return new Point(x, y);
        }

        private void SimulateMouse(bool leftMouseDown, bool leftMouseUp, bool rightMouseDown, bool rightMouseUp, bool mouseMotion, bool skipSceneInput)
        {
            // MobileUO: TODO: do we need to bring this back?
            //if (_dragStarted && !Mouse.LButtonPressed)
            //{
            //    _dragStarted = false;
            //}
            
            if (leftMouseDown)
            {
                Mouse.LClickPosition = Mouse.Position;
                Mouse.CancelDoubleClick = false;
                uint ticks = Time.Ticks;
                if (Mouse.LastLeftButtonClickTime + Mouse.MOUSE_DELAY_DOUBLE_CLICK >= ticks)
                {
                    Mouse.LastLeftButtonClickTime = 0;

                    var res = false;
                    if (skipSceneInput)
                    {
                        res = UIManager.OnMouseDoubleClick(MouseButtonType.Left);
                    }
                    else
                    {
                        res = Scene.OnMouseDoubleClick(MouseButtonType.Left) || UIManager.OnMouseDoubleClick(MouseButtonType.Left);
                    }

                    if (!res)
                    {
                        if (skipSceneInput || !Scene.OnMouseDown(MouseButtonType.Left))
                            UIManager.OnMouseButtonDown(MouseButtonType.Left);
                    }
                    else
                    {
                        Mouse.LastLeftButtonClickTime = 0xFFFF_FFFF;
                    }
                }
                else
                {
                    if (skipSceneInput || !Scene.OnMouseDown(MouseButtonType.Left))
                        UIManager.OnMouseButtonDown(MouseButtonType.Left);
                    Mouse.LastLeftButtonClickTime = Mouse.CancelDoubleClick ? 0 : ticks;
                }
            }
            else if (leftMouseUp)
            {
                if (Mouse.LastLeftButtonClickTime != 0xFFFF_FFFF)
                {
                    if (skipSceneInput || !Scene.OnMouseUp(MouseButtonType.Left) || UIManager.LastControlMouseDown(MouseButtonType.Left) != null)
                        UIManager.OnMouseButtonUp(MouseButtonType.Left);
                }

                //Mouse.End();
            }

            if (rightMouseDown)
            {
                Mouse.RClickPosition = Mouse.Position;
                Mouse.CancelDoubleClick = false;
                uint ticks = Time.Ticks;

                if (Mouse.LastRightButtonClickTime + Mouse.MOUSE_DELAY_DOUBLE_CLICK >= ticks)
                {
                    Mouse.LastRightButtonClickTime = 0;

                    var res = false;
                    if (skipSceneInput)
                    {
                        res = UIManager.OnMouseDoubleClick(MouseButtonType.Right);
                    }
                    else
                    {
                        res = Scene.OnMouseDoubleClick(MouseButtonType.Right) || UIManager.OnMouseDoubleClick(MouseButtonType.Right);
                    }
                    
                    if (!res)
                    {
                        if (skipSceneInput || !Scene.OnMouseDown(MouseButtonType.Right))
                            UIManager.OnMouseButtonDown(MouseButtonType.Right);
                    }
                    else
                    {
                        Mouse.LastRightButtonClickTime = 0xFFFF_FFFF;
                    }
                }
                else
                {
                    if (skipSceneInput || !Scene.OnMouseDown(MouseButtonType.Right))
                        UIManager.OnMouseButtonDown(MouseButtonType.Right);
                    Mouse.LastRightButtonClickTime = Mouse.CancelDoubleClick ? 0 : ticks;
                }
            }
            else if (rightMouseUp)
            {
                if (Mouse.LastRightButtonClickTime != 0xFFFF_FFFF)
                {
                    if (skipSceneInput || !Scene.OnMouseUp(MouseButtonType.Right))
                        UIManager.OnMouseButtonUp(MouseButtonType.Right);
                }

                //Mouse.End();
            }

            if (mouseMotion)
            {
                if (Mouse.IsDragging)
                {
                    if (skipSceneInput || !Scene.OnMouseDragging())
                        UIManager.OnMouseDragging();
                }

                // MobileUO: TODO: do we need to bring this back?
                //if (Mouse.IsDragging && !_dragStarted)
                //{
                //    _dragStarted = true;
                //}
            }
        }
    }
}