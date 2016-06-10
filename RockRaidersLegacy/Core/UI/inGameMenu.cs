using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RockRaiders.UI
{
    public class TeamSelectionMenu : inGameMenu
    {
        private SpaceMarine _player;

        public TeamSelectionMenu(Game1 game, AssetManager assetManager)
            :base(game,assetManager)
        {
            _lblHeader.Text = "Please select a team";
            _lBox.Add("Red Team");
            _lBox.Add("Blue Team");
            _lBox.Add("Auto Assign");

            _selectedLabel = "";
        }

        public override void updateMe(GameManager gameManager, MouseState mState)
        {
            if (!_closed)
            {
                base.updateMe(gameManager, mState);

                _player = gameManager.LocalCharacter;

                if (_lBox.SelectedItem != null)
                {
                    _selectedLabel = _lBox.SelectedItem.Text;

                    if (_selectedLabel == "Blue Team")
                    {
                        gameManager.MatchController.BlueTeam.AddTeamMember(_player);
                    }
                    if (_selectedLabel == "Red Team")
                    {
                        gameManager.MatchController.RedTeam.AddTeamMember(_player);
                    }
                    if (_selectedLabel == "Auto Assign")
                    {
                        if (gameManager.MatchController.BlueTeam.Members.Count < gameManager.MatchController.RedTeam.Members.Count)
                            gameManager.MatchController.BlueTeam.AddTeamMember(_player);
                        else if (gameManager.MatchController.BlueTeam.Members.Count > gameManager.MatchController.RedTeam.Members.Count)
                            gameManager.MatchController.RedTeam.AddTeamMember(_player);
                        else
                            gameManager.MatchController.BlueTeam.AddTeamMember(_player);
                    }

                    gameManager.LocalCharacter.State = ActorState.Dying;
                    this.Close();
                    _lBox.SelectedItem = null;
                }
            }
        }

        public override void drawMe(SpriteBatch sBatch)
        {
            base.drawMe(sBatch);
        }
    }

    class EndGameMenu : inGameMenu
    {
        private List<label> _lblList;
        private Listbox _listBox;
        private MatchController _matchController;

        public EndGameMenu(Game1 game, AssetManager assetManager)
            :base(game,assetManager)
        {
            _lblHeader.Text = "Match Over!";
            _lblList = new List<label>();


            for (int i =0;i< 8; i++)
                _lblList.Add(new label(assetManager.FontLib["SegoeUI"]));

            foreach (label lbl in _lblList)
            {
                lbl.isClickable = false;
                lbl.Text = "";
            }

            // May wish to make this IEnumerable....
            _lblList[0].TextColor = Color.Sienna;
            _lblList[0].Position = new Point(250, 100);
            _lblList[0].isClickable = false;
            _lblList[1].Text = "has won the match!";
            _lblList[1].Position = new Point(_lblList[0].Position.X + 100, _lblList[0].Position.Y);
            _lblList[1].isClickable = false;

            _listBox = new Listbox(assetManager, 650, 500);
            _listBox.Add("EXIT");
            
        }
        public override void updateMe(GameManager gameManager, MouseState MouseState)
        {
            if (!_closed)
            {
                _matchController = gameManager.MatchController;
                if (_matchController.Match != MatchType.DM)
                {
                    if (_matchController.WinningTeam.ID == 1)
                    {
                        _lblList[0].Text = "BLUE TEAM";
                        _lblList[0].TextColor = Color.Blue;
                    }
                    else if (_matchController.WinningTeam.ID == 2)
                    {
                        _lblList[0].Text = "RED TEAM";
                        _lblList[0].TextColor = Color.Red;
                    }
                }
                else
                {
                    _lblList[0].Text = ((SpaceMarine)_matchController.NoTeam.GetWinner()).PlayerName;
                }

                if (_listBox.SelectedItem != null)
                {
                    endGame(gameManager);
                    _listBox.SelectedItem = null;
                }

                base.updateMe(gameManager, MouseState);

                _listBox.updateMe(_game.MouseState);
            }
        }



        public override void drawMe(SpriteBatch sBatch)
        {
            if (!_closed)
            {
                base.drawMe(sBatch);
                _lblList[0].drawMe(sBatch);
                _lblList[1].drawMe(sBatch);
                _lblList[2].drawMe(sBatch);
                _listBox.drawMe(sBatch);
                //foreach (label lbl in _lblList)
                //    lbl.drawMe(sBatch);
            }
        }
    }

    class EscapeMenu : inGameMenu
    {
        private TeamSelectionMenu _teamMenu;
        public EscapeMenu(Game1 game, AssetManager assetManager, TeamSelectionMenu teamMenu)
            :base (game,assetManager)
        {
            _lblHeader.Text = "Game Menu";
            _lBox.Add("Resume Game");
            _lBox.Add("Switch Team");
            _lBox.Add("Leave Match");
            _lBox.Add("Close Game");

            _teamMenu = teamMenu;
        }
        public override void updateMe(GameManager gameManager, MouseState MouseState)
        {
            if (!_closed)
            {
                base.updateMe(gameManager, MouseState);

                if (_lBox.SelectedItem != null)
                {
                    _selectedLabel = _lBox.SelectedItem.Text;

                    if (_selectedLabel == "Resume Game")
                    {
                        this.Close();
                    }
                    if (_selectedLabel == "Switch Team")
                    {
                        if (gameManager.MatchController.Match != MatchType.DM)
                            _teamMenu.Open();
                        else
                            gameManager.LocalCharacter.State = ActorState.Dying;

                        this.Close();
                    }
                    if (_selectedLabel == "Leave Match")
                    {
                        endGame(gameManager);
                    }
                    if (_selectedLabel == "Close Game")
                    {
                        gameManager.SessionManager.Dispose();
                        _game.Exit();
                        this.Close();
                    }
                    _lBox.SelectedItem = null;
                }
            }
        }
        public override void drawMe(SpriteBatch sBatch)
        {
            base.drawMe(sBatch);
        }
    }

    class inGameMenu
    {
        private Rectangle _menuRect;
        private Color _Backtint;
        protected Game1 _game;
        private Texture2D m_blankTxr;
        protected string _selectedLabel;
        protected Listbox _lBox;
        protected label _lblHeader;
        protected bool _closed;
        protected AssetManager _assetmanager;


        public bool Has_Selection
        {
            get
            {
                return (_lBox.SelectedItem != null);
            }
        }

        public inGameMenu(Game1 game, AssetManager assetManager)
        {
            _game = game;
            _assetmanager = assetManager;
            _menuRect = new Rectangle(0, 0, game.ScreenRes_X, game.ScreenRes_Y);
            _lBox = new Listbox(assetManager, game.ScreenRes_X / 3, game.ScreenRes_Y / 3);
            _Backtint = new Color(255, 255, 255, 100);
            _lBox.Font = assetManager.FontLib["SegoeUI"];
            _lblHeader = new label(assetManager.FontLib["UIHeader"]);
            _lBox.Spacing = 80;
            _lBox.Visible = true;
            _closed = true;
        }

        public bool Showing
        {
            get
            {
                return (!_closed);
            }
        }

        public virtual void updateMe(GameManager gameManager, MouseState MouseState)
        {
            if (!_closed)
            {
                _lBox.updateMe(MouseState);
            }
        }
        public void Close()
        {
            _closed = true;
        }
        public void Open()
        {
            _closed = false;
        }
        public virtual void drawMe(SpriteBatch sBatch)
        {
            if (!_closed)
            {
                m_blankTxr = new Texture2D(sBatch.GraphicsDevice, 1, 1);
                m_blankTxr.SetData(new Color[] { Color.Black });

                sBatch.Draw(m_blankTxr, _menuRect, _Backtint);
                _lblHeader.drawMe(sBatch);
                _lBox.drawMe(sBatch);
            }
        }
        protected void endGame(GameManager gameManager)
        {
            gameManager.SessionManager.Dispose();
            _game.State = GameState.Menu;
            gameManager.SessionManager = new SessionManager(_assetmanager);
            gameManager.LocalCharacter.Team = 0;
            gameManager = null;

            this.Close();
        }
    }
}
