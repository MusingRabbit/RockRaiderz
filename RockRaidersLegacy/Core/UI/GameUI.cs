using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RockRaiders.UI
{
    public class GameUI
    {
        private int _score;
        private int _ammo;
        private int _Health;
        private bool _initDone;
        private int _scoreLimit;
        private int _capLimit;

        private Vector2 _scorePos;
        private Vector2 _ammoPos;
        private Vector2 _healthPos;
        private Vector2 _playerListPos;

        private Color _healthColour;
        private Color _ammoColour;

        private Color _teamColor;

        private SpriteFont _font;
        private AssetManager _assetManager;
        private SpaceMarine _player;
        private Game1 _game;

        private TeamSelectionMenu _teamMenu;
        private EscapeMenu _escMenu;
        private EndGameMenu _endGame;

        public TeamSelectionMenu TeamSelection
        {
            get 
            {
                return _teamMenu;
            }
            set
            {
                _teamMenu = value;
            }
        }
        public EscapeMenu EscapeMenu
        {
            get
            {
                return _escMenu;
            }
        }
        private MouseState oldMouse;

        public GameUI(Game1 Game, AssetManager assetManager)
        {
            _assetManager = assetManager;
            _game = Game;
            _font = _assetManager.FontLib["SegoeUI"];
            _healthPos = new Vector2(660, 500);
            _ammoPos = new Vector2(660, 520);
            _scorePos = new Vector2(700, 10);
            _teamMenu = new TeamSelectionMenu(_game, assetManager);
            _escMenu = new EscapeMenu(_game, assetManager,_teamMenu);
            _endGame = new EndGameMenu(_game, assetManager);
            _playerListPos = Vector2.Zero;
            _initDone = false;
            _teamMenu.Close();
            _escMenu.Close();
        }

        public void updateUI(Game1 Game, GameManager GameManager)
        {
            _score = GameManager.LocalCharacter.Score;
            _scoreLimit = GameManager.MatchController.ScoreLimit;
            _capLimit = GameManager.MatchController.CapLimit;

            _Health = GameManager.LocalCharacter.Health;

            if (GameManager.LocalCharacter.CurrentWeapon != null)
            _ammo = GameManager.LocalCharacter.CurrentWeapon.AmmoCount;

            _player = GameManager.LocalCharacter;

            colourStats(_ammo, out _ammoColour);
            colourStats(_Health, out _healthColour);

            if (GameManager == null)
            {
                _initDone = false;
                _teamMenu.Close();
                _escMenu.Close();
                _endGame.Close();
            }

            if (!_initDone)
            {
                if (GameManager.MatchController.Match != MatchType.DM)
                {
                    _teamMenu.Open();
                    _initDone = true;
                }
            }

            if (_teamMenu.Showing)
            {
                _teamMenu.updateMe(GameManager, Game.MouseState);
                _game.IsMouseVisible = true;
            }
            else if (_escMenu.Showing)
            {
                _escMenu.updateMe(GameManager, Game.MouseState);
                _game.IsMouseVisible = true;
            }
            else if (_endGame.Showing)
            {
                _endGame.updateMe(GameManager, Game.MouseState);
                _game.IsMouseVisible = true;
            }
            else
            {
                _game.IsMouseVisible = false;
            }

            doEscMenuUI();


            if (GameManager.MatchController.MatchDone)
                _endGame.Open();

            oldMouse = _game.MouseState;
        }

        private void doEscMenuUI()
        {
            if (_game.KbState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (!_escMenu.Showing)
                    _escMenu.Open();
            }
        }

        private void colourStats(int Value, out Color outColor)
        {
            outColor = Color.White;

                if (Value >= 60)
                    outColor = Color.GreenYellow;
                if (Value < 60)
                    outColor = Color.Yellow;
                if (Value < 30)
                    outColor = Color.Orange;
                if (Value < 10)
                    outColor = Color.Red;
        }

        public void drawMe(SpriteBatch sBatch, GameManager gameManager)
        {

            if (_player != null)
            {

                if (_player.isPromptingUser)
                {
                    if (_player.State == ActorState.Floating)
                        sBatch.DrawString(_font, "PRESS 'F' TO ENABLE GRAVITY BOOTS", new Vector2(_game.ScreenRes_X / 2 - 100, _game.ScreenRes_Y - 60), Color.White);
                    else if (_player.State == ActorState.Bound)
                        sBatch.DrawString(_font, "PRESS 'SPACEBAR' TO DISABLE GRAVITY BOOTS", new Vector2(_game.ScreenRes_X / 2 - 120, _game.ScreenRes_Y - 60), Color.White);
                }

                if (_player.CurrentWeapon != null)
                    sBatch.DrawString(_font, "  AMMO : " + _ammo.ToString(), _ammoPos, _ammoColour);

                sBatch.DrawString(_font, "HEALTH : " + _Health.ToString(), _healthPos, _healthColour);

                if (gameManager.MatchController.Match == MatchType.DM)
                {
                    sBatch.DrawString(_font, "KILLS : " + _score.ToString(), _scorePos, Color.White);
                    sBatch.DrawString(_font, "LIMIT : " + _scoreLimit.ToString(), new Vector2(_scorePos.X,_scorePos.Y + 20), Color.White);
                }
                else
                    sBatch.DrawString(_font, "SCORE : " + _score.ToString(), _scorePos, Color.White);

                if (_player.State == ActorState.Dying)
                {
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "DEAD!", new Vector2(340, 480), Color.Red);
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "You were killed by " + gameManager.LocalCharacter.Killer, new Vector2(300, 500), Color.White);
                    if (_player.CanSpawn)
                        sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "Hit [SpaceBar] to Respawn", new Vector2(300, 520), Color.White);

                    gameManager.Camera.Source = null;
                }


            }
            if ((gameManager.MatchController.Match == MatchType.CTF) ||
                gameManager.MatchController.Match == MatchType.TDM)
            {
                _teamMenu.drawMe(sBatch);
                 if (gameManager.MatchController.Match == MatchType.CTF)
                {
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "Flag Captures", new Vector2(0, 180), Color.White);
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "Blue Team :" + gameManager.MatchController.BlueTeam.Caps.ToString(), new Vector2(0, 200), Color.Blue);
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "Red Team :" + gameManager.MatchController.RedTeam.Caps.ToString(), new Vector2(0, 220), Color.Red);
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "Cap Limit :" + _capLimit, new Vector2(0, 240), Color.SlateGray);
                }
                 if (gameManager.MatchController.Match == MatchType.TDM)
                {
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "Team Score", new Vector2(0, 180), Color.White);
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "Blue Team :" + gameManager.MatchController.BlueTeam.TeamScore.ToString(), new Vector2(0, 200), Color.Blue);
                    sBatch.DrawString(_assetManager.FontLib["SegoeUI"], "Red Team :" + gameManager.MatchController.RedTeam.TeamScore.ToString(), new Vector2(0, 220), Color.Red);

                }
            }

            for (int i = 0; i < gameManager.SessionManager.NetSession.AllGamers.Count; i++ )
            {
                SpaceMarine playerChar = gameManager.SessionManager.NetSession.AllGamers[i].Tag as SpaceMarine;
                if (playerChar.Team == 1)
                    _teamColor = Color.Blue;
                else if (playerChar.Team == 2)
                    _teamColor = Color.Red;
                else
                    _teamColor = Color.White;

                sBatch.DrawString(_assetManager.FontLib["Normal"], playerChar.PlayerName + " " + playerChar.Score.ToString() , new Vector2(_playerListPos.X, _playerListPos.Y + (i*15)), _teamColor);
            }

            _escMenu.drawMe(sBatch);
            _endGame.drawMe(sBatch);

        }
    }
}
