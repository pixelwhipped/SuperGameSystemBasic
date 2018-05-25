using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using SuperGameSystemBasic.Basic;
using SuperGameSystemBasic.UI.Window;

namespace SuperGameSystemBasic
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public partial class BasicOne : Game
    {
        public static readonly string DirectorySeparator = Path.DirectorySeparatorChar.ToString();

        public static bool ShowDebugFps;

        public static Texture2D Pixel;

        private readonly string _settingsFile = "settings.json";

        private RenderTarget2D _target;
        public Random Random = new Random();

        public TimeSpan YeildTime = TimeSpan.Zero;

        public BasicOne()
        {
#if DEBUG
            if (Debugger.IsAttached) Application.Current.DebugSettings.EnableFrameRateCounter = false;
#endif
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public Setting GameSettings { get; set; }

        public bool ScanLines
        {
            get => GameSettings.ScanLines;
            set
            {
                GameSettings.ScanLines = value;
                Serialize(GameSettings, _settingsFile);
            }
        }

        public bool ShowHints
        {
            get => GameSettings.ShowHints;
            set
            {
                GameSettings.ShowHints = value;
                Serialize(GameSettings, _settingsFile);
            }
        }

        public bool OverScan
        {
            get => GameSettings.OverScan;
            set
            {
                GameSettings.OverScan = value;
                Serialize(GameSettings, _settingsFile);
            }
        }

        public bool ScreenShot { get; set; }
        public TimeSpan ScreenShotWait { get; set; } = TimeSpan.Zero;

        public static StorageFolder LocalStorageFolder
        {
            get
            {
                if (AsyncIO.DoesFolderExistAsync(ApplicationData.Current.LocalFolder, "Projects"))
                    AsyncIO.GetFolderAsync(ApplicationData.Current.LocalFolder, "Projects");
                return AsyncIO.CreateFolderAsync(ApplicationData.Current.LocalFolder, "Projects");
            }
        }

        public static StorageFolder TutorialsStorageFolder
        {
            get
            {
                if (AsyncIO.DoesFolderExistAsync(Package.Current.InstalledLocation, "Tutorials"))
                    AsyncIO.GetFolderAsync(Package.Current.InstalledLocation, "Tutorials");
                return AsyncIO.CreateFolderAsync(Package.Current.InstalledLocation, "Tutorials");
            }
        }

        public static List<StorageFolder> KnownStorageFolders
        {
            get
            {
                try
                {
                    return new List<StorageFolder>
                    {
                        LocalStorageFolder,
                        KnownFolders.RemovableDevices,
                        KnownFolders.PicturesLibrary
                    };
                }
                catch
                {
                    return new List<StorageFolder> {LocalStorageFolder};
                }
            }
        }

        public GraphicsDeviceManager Graphics { get; set; }
        public SpriteBatch SpriteBatch { get; set; }

        public Texture2D OverlayScanAndEdge { get; set; }
        public Texture2D OverlayScanSync { get; set; }
        public int ScanSyncX { get; set; }
        public Terminal Terminal { get; set; }
        public FullWindow IdeWindow { get; set; }
        public Terminal IdeTerminal { get; set; }

        public IpsTimer RunningIpsTimer { get; set; }
        public IpsTimer IdeIpsTimer { get; set; }

        public Interpreter Interpreter { get; set; }

        // public KeyboardInput Keyboard { get; set; }


        public IdeState IdeState { get; set; } = IdeState.DevKit;
        public static IStorageItem[] FileArguments { get; set; }

        public bool Exists(string path)
        {
            return path.Equals(DirectorySeparator) ||
                   Directory.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, path)) ||
                   File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, path));
        }

        /// <summary>
        ///     Helper Json Deserializer
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize</typeparam>
        /// <param name="file">The file to deserialize form</param>
        /// <returns>The Object</returns>
        public T Deserialize<T>(string file)
        {
            T ret;
            var serializer = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
            using (var jsonTextReader = new JsonTextReader(new StreamReader(OpenFile(file, FileAccess.Read))))
            {
                ret = serializer.Deserialize<T>(jsonTextReader);
            }
            return ret;
        }

        public Stream CreateFile(string path)
        {
            return new FileStream(Path.Combine(ApplicationData.Current.LocalFolder.Path, path), FileMode.Create,
                FileAccess.ReadWrite);
        }

        public Stream OpenFile(string path, FileAccess access)
        {
            return new FileStream(Path.Combine(ApplicationData.Current.LocalFolder.Path, path), FileMode.Open, access);
        }

        /// <summary>
        ///     Helper Json Serializer
        /// </summary>
        /// <typeparam name="T">The Type of the object to serialize.</typeparam>
        /// <param name="data">The object to serialize</param>
        /// <param name="file">The file to serialize to</param>
        /// <returns>the name of the serialized file or null on failure</returns>
        public bool Serialize<T>(T data, string file)
        {
            var serializer = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
            var f = CreateFile(file);
            var sw = new StreamWriter(f);
            try
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, data);
                    sw = null;
                    f = null;
                }
            }
            finally
            {
                sw?.Dispose();
                f?.Dispose();
            }
            return Exists(file);
        }

        public string ReadTutorial(string name)
        {
            return new StreamReader(AsyncIO.GetContentStream(name)).ReadToEnd();
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _mouseDown = Mouse.GetState().LeftButton == ButtonState.Pressed;
            GameSettings = Exists(_settingsFile) ? Deserialize<Setting>(_settingsFile) : new Setting();
            IdeTerminal = new Terminal(this, "Terminal20x24");
            IdeWindow = new FullWindow(this);

            IdeIpsTimer = new IpsTimer(this, IdeState.DevKit);
            RunningIpsTimer = new IpsTimer(this, IdeState.Running);
            ResetRenderTarget();

            InitializeIde();
            BlinkTimer = new Timer(s =>
            {
                if (!(s is BasicOne b)) return;
                b.CursorBlink = !b.CursorBlink;
                BlinkTimer = new Timer(sx => { b.CursorBlink = !b.CursorBlink; }, this, 0, 500);
            }, this, 0, 500);

            Terminal = new Terminal(this, "Terminal20x24");
            base.Initialize();
        }

        private void ResetRenderTarget()
        {
            if (_target == null || _target.Width != GraphicsDevice.PresentationParameters.BackBufferWidth ||
                _target.Height != GraphicsDevice.PresentationParameters.BackBufferHeight)
                _target = new RenderTarget2D(
                    GraphicsDevice,
                    GraphicsDevice.PresentationParameters.BackBufferWidth,
                    GraphicsDevice.PresentationParameters.BackBufferHeight,
                    false,
                    GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24);
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new[] {Color.White});

            IdeTerminal.LoadContent();

            Terminal.LoadContent();
            OverlayScanAndEdge = Content.Load<Texture2D>("OverlayScanAndEdge");
            OverlayScanSync = Content.Load<Texture2D>("OverlayScanSync");

            SetDevKitState();
            //SetRunningState(null);
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Keyboard.Update();
            ScanSyncX++;
            if (ScanSyncX > Graphics.PreferredBackBufferHeight) ScanSyncX = -OverlayScanSync.Height;
            ScreenShotWait -= gameTime.ElapsedGameTime;
            if (Keyboard.GetState().GetPressedKeys().Any())
                if (ScreenShotWait <= TimeSpan.Zero && Keyboard.GetState().IsKeyDown(Keys.Home))
                {
                    ScreenShot = true;
                    ScreenShotWait = TimeSpan.FromSeconds(2);
                }
            if (IdeState == IdeState.Running)
            {
                RunningIpsTimer.Update();
                if (Interpreter != null)
                    try
                    {
                        while (IdeState == IdeState.Running && Interpreter.InterpreterState == LineState.Continue)
                        {
                            Interpreter.BlockingKeys = new List<char>();
                            if (Interpreter.KeyBlocking)
                            {
                                CurrenKeyboardState = Keyboard.GetState();
                                var current = CurrenKeyboardState.GetPressedKeys();
                                var pressed = PreviousKeyboardState.GetPressedKeys().Where(k => !current.Contains(k));
                                var keys = pressed as Keys[] ?? pressed.ToArray();
                                foreach (var k in keys)
                                {
                                    if (k == Keys.LeftShift || k == Keys.RightShift) continue;
                                    if (TryConvertKeyboardInput(k, CurrenKeyboardState, PreviousKeyboardState,
                                        out var c))
                                    {
                                        Interpreter.BlockingKeys.Add(c);
                                    }
                                    else if (k == Keys.Enter && Interpreter.ReadLineToBlocking)
                                    {
                                        Interpreter.BlockingKeys.Add('\n');
                                    }
                                    else if (k == Keys.Tab && Interpreter.ReadLineToBlocking)
                                    {
                                        Interpreter.BlockingKeys.Add(' ');
                                        Interpreter.BlockingKeys.Add(' ');
                                        Interpreter.BlockingKeys.Add(' ');
                                        Interpreter.BlockingKeys.Add(' ');
                                        Interpreter.BlockingKeys.Add(' ');
                                    }
                                    else if (k == Keys.Back && Interpreter.ReadLineToBlocking)
                                    {
                                        Interpreter.BlockingKeys.Add('\r');
                                    }
                                }
                                PreviousKeyboardState = CurrenKeyboardState;
                            }
                            Interpreter.Update(gameTime);
                            if (Interpreter.Exit)
                                SetDevKitState();
                        }
                        if (Interpreter?.InterpreterState == LineState.Yeild)
                        {
                            YeildTime -= gameTime.ElapsedGameTime;
                            //if (Windows.UI.Xaml.Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Escape).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
                            //{ 
                                // Escape key pressed
                            //}
                            if (Keyboard.GetState().IsKeyDown(Keys.Pause))
                            {
                                Interpreter = null;
                                SetDevKitState();
                            }
                            else if (YeildTime < TimeSpan.Zero)
                            {
                                Interpreter.InterpreterState = LineState.Continue;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                         SetErrorState(e);
                    }
            }
            else if (IdeState == IdeState.Error)
            {
                // SetErrorState(LastError);
                ErrorWait += gameTime.ElapsedGameTime;
                if (ErrorWait > TimeSpan.FromSeconds(1) && Keyboard.GetState().GetPressedKeys().Any())
                {
                    SetDevKitState();
                    if (ShowHints && !_errorHintsShown) ShowErrorHints();
                }
            }
            else
            {
                UpdateIde();
            }
            base.Update(gameTime);
        }

        public static void SetGlyphData(int g, int[] data, BasicOne basicOne)
        {
            if (data.Length != 20 * 24) return;
            var terminal = basicOne.Interpreter != null && basicOne.IdeState == IdeState.Running
                ? basicOne.Terminal
                : basicOne.IdeTerminal;

            var tex = new Color[terminal.GlyphSheet.Width * terminal.GlyphSheet.Height];
            terminal.GlyphSheet.GetData(tex);
            var rect = terminal.GlyphRects[(Glyph) g];
            for (var y = 0; y < 24; y++)
            for (var x = 0; x < 20; x++)
                tex[(y + rect.Y) * terminal.GlyphSheet.Width + x + rect.X] =
                    data[y * 20 + x] > 0 ? Color.Black : Color.Transparent;
            var texture = new Texture2D(basicOne.GraphicsDevice, terminal.GlyphSheet.Width, terminal.GlyphSheet.Height);
            texture.SetData(tex);
            terminal.GlyphSheet = texture;
        }

        public static bool[] GetGlyphData(int g, BasicOne basicOne)
        {
            try
            {
                var terminal = basicOne.Interpreter != null && basicOne.IdeState == IdeState.Running
                    ? basicOne.Terminal
                    : basicOne.IdeTerminal;
                var rawData = new Color[20 * 24];
                var rect = terminal.GlyphRects[(Glyph) g];
                terminal.GlyphSheet.GetData(0, rect, rawData, 0, 20 * 24);
                var b = new List<bool>();
                foreach (var c in rawData)
                    b.Add(c.A != 0);
                return b.ToArray();
            }
            catch (Exception e)
            {
                return new bool[0];
            }
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var dest = new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            var scanIssue = new Rectangle(0, ScanSyncX, Graphics.PreferredBackBufferWidth, OverlayScanSync.Height);

            if (IdeState == IdeState.Running || IdeState == IdeState.Error)
            {
                if (Interpreter != null && Interpreter.InterpreterState != LineState.Continue)
                {
                    ResetRenderTarget();
                    GraphicsDevice.SetRenderTarget(_target);

                    Terminal.Draw(dest);
                    SpriteBatch.Begin();
                    if (ScanLines) SpriteBatch.Draw(OverlayScanAndEdge, null, dest);
                    if (OverScan) SpriteBatch.Draw(OverlayScanSync, null, scanIssue);
                    SpriteBatch.End();
                    Interpreter.InterpreterState = Interpreter.InterpreterState == LineState.Yeild
                        ? LineState.Yeild
                        : LineState.Continue;
                }
                else if (Interpreter == null)
                {
                    ResetRenderTarget();
                    GraphicsDevice.SetRenderTarget(_target);
                    IdeTerminal.Draw(dest);
                    SpriteBatch.Begin();
                    if (ScanLines) SpriteBatch.Draw(OverlayScanAndEdge, null, dest);
                    if (OverScan) SpriteBatch.Draw(OverlayScanSync, null, scanIssue);
                    SpriteBatch.End();
                }
            }
            else if (IdeState == IdeState.DevKit)
            {
                ResetRenderTarget();
                GraphicsDevice.SetRenderTarget(_target);
                IdeTerminal.Draw(dest);
                SpriteBatch.Begin();
                SpriteBatch.Draw(Pixel, null, MouseTerminalScreenRect, null, null, 0, null, Color.White * 0.5f);
                if (CursorVisible)
                    SpriteBatch.Draw(Pixel, null, CursorTerminalScreenRect, null, null, 0, null, CursorColor * 0.75f);
                if (ScanLines) SpriteBatch.Draw(OverlayScanAndEdge, null, dest);
                if (OverScan) SpriteBatch.Draw(OverlayScanSync, null, scanIssue);
                SpriteBatch.End();
            }
            if (ScreenShot)
            {
                var stream = new MemoryStream();

                ScreenShot = false;
                _target.SaveAsPng(stream, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
                try
                {
                    if (!AsyncIO.DoesFolderExistAsync(KnownFolders.PicturesLibrary, "ScreenShots"))
                        AsyncIO.CreateFolderAsync(KnownFolders.PicturesLibrary, "ScreenShots");
                    var folder = AsyncIO.GetFolderAsync(KnownFolders.PicturesLibrary, "ScreenShots");
                    var file = AsyncIO.CreateFileAsync(folder, $@"{Guid.NewGuid()}.png");
                    AsyncIO.WriteStreamToFile(file, stream);
                }
                catch
                {
                }
            }
            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Begin();
            SpriteBatch.Draw(_target, null, dest, null, null, 0, null, null);
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}