using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace RockRaiders.UI
{
    enum MenuState
    {
        TitleScreen,
        Options,
        Host,
        Join
    }
    public class MainMenu
    {
        private AssetManager m_assetManager;
        private Button btnJoin, btnHost, btnOptions, btnExit, btnBack;
        private Rectangle backRect;
        private Texture2D txrBackGround;
        private List<Button> buttonList;
        private List<label> labelList;
        private MenuState _state = MenuState.TitleScreen;
        private int m_sleepCounter, m_delay;
        private Listbox listBoxMap;
        private Listbox listBoxSession;
        private Listbox listBoxMatch;
        private bool m_launch, m_searchDone;
        private label lblFound;
        private label lblNew;
        private label lblMatch;

        public bool LaunchGame
        {
            get
            {
                return m_launch;
            }
            set
            {
                m_launch = value;
            }
        }
        public string SelectedMap
        {
            get
            {
                try
                {
                    return listBoxMap.SelectedItem.Text;
                }
                catch
                {
                    return "2ShipsMap2";
                }
            }
        }
        public string SelectedMatchType
        {
            get
            {
                if (listBoxMatch.SelectedItem != null)
                    return listBoxMatch.SelectedItem.Text;
                else return null;
            }
        }
        public MenuState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public MainMenu(AssetManager AssetManager)
        {
            m_assetManager = AssetManager;
            buttonList = new List<Button>();
            labelList = new List<label>();
            backRect = new Rectangle(0, 0, 800, 600);
            lblFound = new label(m_assetManager.FontLib["UIHeader"]);
            lblNew = new label(m_assetManager.FontLib["UIHeader"]);
            lblMatch = new label(m_assetManager.FontLib["UIHeader"]);

            m_delay = 20;
            m_sleepCounter = m_delay;

            txrBackGround = AssetManager.SpriteLib["BackGround_" + (Game1.RNG.Next(1, 7)).ToString()];
            createButtons();

            listBoxMap = new Listbox(m_assetManager, 400, 100);
            listBoxSession = new Listbox(m_assetManager, 400, 200);
            listBoxMatch = new Listbox(m_assetManager, 400, 350);
            getMapList();
            listBoxMap.Visible = false;
            listBoxSession.Visible = false;

            createHeadings();

            setTitleScreen();
        }

        private void getMapList()
        {
            DirectoryInfo dirInfo = new DirectoryInfo("Content\\Levels\\");
            if (Directory.Exists(dirInfo.FullName))
            {
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    string[] splitName = file.Name.Split('.');
                    listBoxMap.Add(splitName[0]);
                }
            }

            listBoxMatch.Add("DeathMatch");
            listBoxMatch.Add("Team Deathmatch");
            listBoxMatch.Add("Capture The Flag");
        }

        private void createButtons()
        {
            btnJoin = new Button(10, 380, m_assetManager.ButtonLib["Join_U"], m_assetManager.ButtonLib["Join_L"]);
            btnHost = new Button(10, 430, m_assetManager.ButtonLib["Host_U"], m_assetManager.ButtonLib["Host_L"]);
            btnOptions = new Button(10, 480, m_assetManager.ButtonLib["Options_U"], m_assetManager.ButtonLib["Options_L"]);
            btnExit = new Button(10, 530, m_assetManager.ButtonLib["Exit_U"], m_assetManager.ButtonLib["Exit_L"]);
            btnBack = new Button(10, 530, m_assetManager.ButtonLib["Back_U"], m_assetManager.ButtonLib["Back_L"]);
        }

        private void createHeadings()
        {
            
            lblNew.Text = "Map Selection";
            lblNew.Position = new Point(200,50);
            lblNew.isVisible = false;

            lblFound.Text = "Available Sessions";
            lblFound.Position = new Point(200, 100);
            lblFound.isVisible = false;

            lblMatch.Text = "Select Match Type";
            lblMatch.Position = new Point(200, 300);
            lblMatch.isVisible = false;

            labelList.Add(lblNew);
            labelList.Add(lblFound);
            labelList.Add(lblMatch);
        }

        private void setControlVisability()
        {
            if (_state == MenuState.Host)
            {
                listBoxMap.Visible = true;
                labelList[0].isVisible = true;
                labelList[2].isVisible = true;
                listBoxSession.Visible = false;
                listBoxMatch.Visible = true;
            }
            else if (_state == MenuState.Join)
            {
                listBoxSession.Visible = true;
                lblFound.isVisible = true;
            }
            else
            {
                listBoxMap.Visible = false;
                listBoxMatch.Visible = false;
                listBoxSession.Visible = false;
                labelList[0].isVisible = false;
                labelList[2].isVisible = false;
                lblFound.isVisible = false; 

            }
        }

        public void updateMe(Game1 Game, SessionManager sessionManager, MouseState cursor)
        {
            switch (_state)
            {
                case MenuState.TitleScreen:
                    doEventsTitleScreen(Game);
                    break;
                case MenuState.Join:
                    if (SignedInGamer.SignedInGamers.Count > 0)
                        doEventsJoinScreen(Game, sessionManager);
                    else if (!Guide.IsVisible)
                            Guide.ShowSignIn(1, false);

                    listBoxSession.updateMe(cursor);
                    break;

                case MenuState.Host:
                    if (SignedInGamer.SignedInGamers.Count > 0)
                        doEventsHostScreen(Game, sessionManager);
                    else if (!Guide.IsVisible)
                        Guide.ShowSignIn(1,false);

                    listBoxMap.updateMe(cursor);
                    listBoxMatch.updateMe(cursor);
                    break;
                case MenuState.Options:

                    break;
            }
            updateButtons(cursor);
            setControlVisability();
        }

        public void drawMe(SpriteBatch sBatch)
        {
            sBatch.Draw(txrBackGround, backRect, Color.LightGray);

            drawListBoxs(sBatch);
            drawButtons(sBatch);
            drawLabels(sBatch);

            drawLIVEprompt(sBatch);
        }

        private void drawButtons(SpriteBatch sBatch)
        {
            foreach (Button button in buttonList)
            {
                button.Draw(sBatch);
            }
        }
        private void drawLabels(SpriteBatch sBatch)
        {
            foreach (label lbl in labelList)
                lbl.drawMe(sBatch);
        }
        private void drawListBoxs(SpriteBatch sBatch)
        {
            listBoxMap.drawMe(sBatch);
            listBoxSession.drawMe(sBatch);
            listBoxMatch.drawMe(sBatch);
        }

        private void setTitleScreen()
        {
            buttonList.Clear();
            btnJoin.Position = new Vector2(10, 380);
            btnHost.Position = new Vector2(10, 430);
            buttonList.Add(btnJoin);
            buttonList.Add(btnHost);
            buttonList.Add(btnExit);
            btnExit.Clicked = false;
        }
        private void setJoinScreen()
        {
            buttonList.Clear();
            btnJoin.Position = new Vector2(600, 530);
            buttonList.Add(btnJoin);
            buttonList.Add(btnBack);
        }
        private void setHostScreen()
        {
            buttonList.Clear();
            btnHost.Position = new Vector2(600, 530);
            buttonList.Add(btnHost);
            buttonList.Add(btnBack);
        }
        private void setOptionsScreen()
        {
            buttonList.Clear();
            buttonList.Add(btnBack);
        }

        private bool canClick()
        {
            m_sleepCounter--;
            return (m_sleepCounter <= 0);
        }
        private void updateButtons(MouseState cursor)
        {
            foreach (Button button in buttonList)
            {
                button.updateMe(cursor);
            }
            if (btnBack.Clicked)
            {
                _state = MenuState.TitleScreen;
                setTitleScreen();
            }
            btnBack.updateMe(cursor);
        }
        private void doEventsTitleScreen(Game1 Game)
        {
            if (btnJoin.Clicked)
            {
                _state = MenuState.Join;
                setJoinScreen();
                m_sleepCounter = m_delay;
            }
            if (btnHost.Clicked)
            {
                _state = MenuState.Host;
                setHostScreen();
                m_sleepCounter = m_delay;
            }

            if (btnExit.Clicked)
                Game.Exit();
        }
        private void doEventsHostScreen(Game1 Game, SessionManager sessionManager)
        {
            if (canClick())
                setHostScreen();
            
            m_sleepCounter = m_delay;
            if (sessionManager.NetSession == null)
                sessionManager.HostSession();

            if (btnHost.Clicked)
            {
                if ((listBoxMap.SelectedItem != null) && (listBoxMatch.SelectedItem != null))
                {
                    sessionManager.NetSession.StartGame();
                    m_launch = true;
                }

            }
        }
        private void doEventsJoinScreen(Game1 Game, SessionManager sessionManager)
        {
            if (canClick())
                setJoinScreen();

            m_sleepCounter = m_delay;

            if (!m_searchDone)
            {
                sessionManager.doSessionSearch();

                if (sessionManager.SessionList.Count > 0)
                {
                    foreach (AvailableNetworkSession session in sessionManager.SessionList)
                    {
                        listBoxSession.Add(session.HostGamertag);
                    }
                }

                else
                {
                    Console.WriteLine("No Sessions Could be Found");
                    _state = MenuState.TitleScreen;
                }
                m_searchDone = true;
            }

            if (btnJoin.Clicked && (listBoxSession.SelectedItem != null))
            {
                foreach (AvailableNetworkSession session in sessionManager.SessionList)
                {
                    if (session.HostGamertag == listBoxSession.SelectedItem.Text)
                        sessionManager.JoinSession(session);
                    
                    m_launch = true;
                }
            }
        }

        private void drawLIVEprompt(SpriteBatch sBatch)
        {
            string message = "";
            if (SignedInGamer.SignedInGamers.Count == 0)
                message = "No profile signed in! \n Press the Home key on the keyboard to sign in";
            else
                message = "Click Host to create a new session \n or play to join an existing one.";
            sBatch.DrawString(m_assetManager.FontLib["DebugFont"], message, Vector2.Zero, Color.White);
        }
    }
}
