using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace RockRaiders1
{
    public enum GameState
    {
        Menu,
        Load,
        Play,
    }
    enum ActorState
    {
        Floating,
        Bound,
        Dying,
        Dead
    }
    enum WeaponState
    {
        dropped,
        pickedUp
    }
    enum EntityState
    {
        Live,
        Dead
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState kbCurr;
        MouseState mouseCurr;
        BackGround backGround;
        CrossHair crossHair;
        GameManager gameManager; 
        AssetManager assetManager;
        MainMenu menu;
        label lblLoading;
        SessionManager sessionManager;
        GameUI gameInterface;

        public static Random RNG = new Random();
        public static GameState currentGameState;
        private Exception _error;

        public string SelectedMap
        {
            get
            {
                return menu.SelectedMap;
            }
        }
        public string SelectedMatch
        {
            get
            {
                return menu.SelectedMatchType;
            }
        }
        public KeyboardState KbState
        {
            get
            {
                return kbCurr;
            }
        }
        public MouseState MouseState
        {
            get
            {
                return mouseCurr;
            }
        }
        public int ScreenRes_X
        {
            get
            {
                return graphics.PreferredBackBufferWidth;
            }
        }
        public int ScreenRes_Y
        {
            get
            {
                return graphics.PreferredBackBufferHeight;
            }
        }
        public GameState State
        {
            get
            {
                return currentGameState;
            }
            set
            {
                currentGameState = value;
                if (value == GameState.Menu)
                    menu = new MainMenu(assetManager);
            }
        }
        public Exception Exception
        {
            set
            {
                _error = value;
            }
            get
            {
                return _error;
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Components.Add(new GamerServicesComponent(this));
        }


        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            currentGameState = GameState.Menu;


            base.Initialize();
        }



        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            assetManager = new AssetManager(Content);
            sessionManager = new SessionManager(assetManager);
            crossHair = new CrossHair(assetManager.SpriteLib["Crosshair"]);
            gameInterface = new GameUI(this,assetManager);
            menu = new MainMenu(assetManager);
            backGround = new BackGround(assetManager, ScreenRes_X, ScreenRes_Y);
            createLoadingLabel();
        }

        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            mouseCurr = Mouse.GetState();
            kbCurr = Keyboard.GetState();

            switch (currentGameState)
            {
                case GameState.Menu:
                    if (graphics.IsFullScreen)
                        graphics.ToggleFullScreen();


                    if (Exception != null)
                    {
                        string strMethod = Exception.TargetSite.ToString();

                        if (strMethod.Contains("get_SelectedMatchType()"))
                            //MessageBox.Show("There was an Error loading the map. \n Please ensure that the map '" + sessionManager.RecievedMapName + "' exists within Content\\Levels  \n\n" + Exception.Message, "Map Load Error", MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                        Exception = null;
                        return;
                    }
                    graphics.IsFullScreen = false;
                    IsMouseVisible = true;
                    menu.updateMe(this, sessionManager, mouseCurr);
                    if (menu.LaunchGame)
                    {
                        if (!sessionManager.FailedToConnect)
                        {
                            currentGameState = GameState.Load;
                            menu.LaunchGame = false;
                        }
                        else
                        {
                            //MessageBox.Show("Failed to connect. Could not get response from host.");
                        }
                    }
                    break;
                case GameState.Load:
                    if (!graphics.IsFullScreen)
                    graphics.ToggleFullScreen();
                    currentGameState = GameState.Play;
                      gameManager = new GameManager(this, assetManager,sessionManager, graphics.GraphicsDevice);
                    backGround.updateMe(gameManager);
                    break;
                case GameState.Play:
                    gameManager.Update(gameTime,this);

                    if (gameManager.LocalCharacter != null)
                    gameInterface.updateUI(this,gameManager);
                    break;
            }

            base.Update(gameTime);

            if (gameManager != null)
                sessionManager.Update(gameManager.GameWorld);
            else
                sessionManager.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameState)
            {
                case GameState.Menu:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();
                    menu.drawMe(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameState.Load:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();
                    lblLoading.drawMe(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameState.Play:
                    spriteBatch.Begin();
                    backGround.Draw(spriteBatch);
                    spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    null, null, null, null,
                    gameManager.Camera.Transform);

                     gameManager.drawMe(spriteBatch, sessionManager);

                    spriteBatch.End();



                    spriteBatch.Begin();
                    gameInterface.drawMe(spriteBatch, gameManager);

#if DEBUG
                    spriteBatch.DrawString(gameManager.GameWorld.AssetManager.FontLib["DebugFont"], graphics.PreferredBackBufferWidth + "x" + graphics.PreferredBackBufferHeight
                        + "\nFPS:" + (int)(1 / gameTime.ElapsedGameTime.TotalSeconds), Vector2.Zero, Color.White);
#endif
                    spriteBatch.End();
                    

                    break;
            }

            base.Draw(gameTime);
        }

        private void createLoadingLabel()
        {
            lblLoading = new label(assetManager.FontLib["UIHeader"]);
            lblLoading.isVisible = true;
            lblLoading.Text = "Loading...";
            lblLoading.TextColor = Color.White;
            lblLoading.Position = new Point(graphics.PreferredBackBufferWidth / 2 - lblLoading.Width, graphics.PreferredBackBufferHeight / 2);
        }
    }
}