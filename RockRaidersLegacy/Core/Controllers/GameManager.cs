using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace RockRaiders.Core.Controllers
{
    class GameController
    {
        private World gameWorld;
        private SpaceMarine localPlayer;
        private CrossHair m_crossHair;
        private Camera2D m_camera;
        private GraphicsDevice m_graphicsDevice;
        private PolyInputController m_controller;
        private SpawnController m_spawnManager;
        private MatchController m_matchController;
        private SessionController m_sessionManager;
        private int localTeam;

        public World GameWorld
        {
            get
            {
                return gameWorld;
            }
        }
        public Camera2D Camera
        {
            get
            {
                return m_camera;
            }
        }
        public SpaceMarine LocalCharacter
        {
            get
            {
                return localPlayer;
            }
        }
        public MatchController MatchController
        {
            get
            {
                return m_matchController;
            }
            set
            {
                m_matchController = new MatchController();
            }
        }
        public SessionController SessionManager
        {
            get
            {
                return m_sessionManager;
            }
            set
            {
                m_sessionManager = value;
            }
        }
        public SpawnController SpawnManager
        {
            get
            {
                return m_spawnManager;
            }
        }
        


        public GameController(Game1 Game, AssetManager assetManager, SessionController sessionManager, GraphicsDevice graphicsDevice)
        {
            m_graphicsDevice = graphicsDevice;
            gameWorld = new World(assetManager);
            m_crossHair = new CrossHair(assetManager.SpriteLib["Crosshair"]);
            m_camera = new Camera2D(graphicsDevice.Viewport, Vector2.Zero);
            m_controller = new PolyInputController();
            m_matchController = new MatchController();
            m_sessionManager = sessionManager;
            try
            {
                if (sessionManager.RecievedMapName != "")
                {
                    m_matchController = sessionManager.RecievedMatch;
                    gameWorld.LoadMap(sessionManager.RecievedMapName,m_matchController);
                }
                else
                {
                    setMatch(Game.SelectedMatch);
                    gameWorld.LoadMap(Game.SelectedMap, m_matchController);
                }
            }
            catch (Exception ex)
            {
                Game1.currentGameState = GameState.Menu;
                gameWorld = null;
                if (sessionManager.isHost) 
                sessionManager.NetSession.EndGame();
                Game.Exception = ex;
                return;
            }


            m_spawnManager = new SpawnController();
            m_spawnManager.SpawnList = gameWorld.SpawnPoints;
            m_spawnManager.WeaponSpawnList = gameWorld.WeaponSpawns;
            m_spawnManager.SpawnAllWeapons(gameWorld.PickupItems);

            foreach (NetworkGamer gamer in sessionManager.NetSession.LocalGamers)
                if (gamer.IsLocal)
                    localPlayer = (SpaceMarine)gamer.Tag;
            m_matchController.NoTeam.AddTeamMember(localPlayer);

            if (sessionManager.RecievedItemList != null)
            {
                gameWorld.PickupItems = sessionManager.RecievedItemList;
                foreach (PickupItem item in gameWorld.PickupItems)
                {
                    if (item.GetType().BaseType == typeof(Weapon))
                        ((Weapon)item).SpawnPont = getSpawn(item);

                    else if (item.GetType() == typeof(Flag))
                    {
                        if (((Flag)item).Team == 1)
                            ((Flag)item).Base = gameWorld.BlueBase;
                        if (((Flag)item).Team == 2)
                            ((Flag)item).Base = gameWorld.RedBase;
                    }
                }
            }

            


            if (gameWorld.CurrentMatch == MatchType.CTF)
            {
                if (sessionManager.isHost)
                {
                    gameWorld.BlueBase.SpawnFlag(gameWorld.PickupItems);
                    gameWorld.RedBase.SpawnFlag(gameWorld.PickupItems);
                }
            }

            sessionManager.GameManager = this;


        }

        public void Update(GameTime gt, Game1 Game)
        {
            if (!MatchController.MatchDone)
            {
                localTeam = localPlayer.Team;

                m_controller.ProcessInput(Game.KbState);
                m_sessionManager.GetGameUpdates(gt, gameWorld);


                for (int i = 0; i < gameWorld.PickupItems.Count; i++)
                {
                    Type itemType = gameWorld.PickupItems[i].GetType();
                    if ((itemType == typeof(Weapon)) || (itemType.BaseType == typeof(Weapon)))
                    {
                        Weapon gun = (Weapon)gameWorld.PickupItems[i];
                        gun.Update(gt, gameWorld);
                    }
                    else if (itemType == typeof(Flag))
                    {
                        Flag flag = (Flag)gameWorld.PickupItems[i];
                        flag.Update(gt, gameWorld);
                        flag.CheckCapState(gameWorld.RedBase);
                        flag.CheckCapState(gameWorld.BlueBase);
                    }

                    if (gameWorld.PickupItems[i].State == EntityState.Dead)
                    {
                        if (gameWorld.PickupItems[i].GetType().BaseType == typeof(Weapon))
                        {
                            if (gameWorld.PickupItems[i].SpawnPont != null)
                            {
                                ((Weapon)gameWorld.PickupItems[i]).SpawnPont.SpawnWeapon(gameWorld.PickupItems);
                            }
                            else
                            {
                                gameWorld.PickupItems[i].SpawnPont = getSpawn(gameWorld.PickupItems[i]);
                                ((Weapon)gameWorld.PickupItems[i]).SpawnPont.SpawnWeapon(gameWorld.PickupItems);
                            }
                            gameWorld.PickupItems.RemoveAt(i);
                        }
                        else if (gameWorld.PickupItems[i].GetType() == typeof(Flag))
                        {
                            if (((Flag)gameWorld.PickupItems[i]).Base == null)
                            {
                                if (((Flag)gameWorld.PickupItems[i]).Team == 1)
                                    ((Flag)gameWorld.PickupItems[i]).Base = gameWorld.BlueBase;
                                else if (((Flag)gameWorld.PickupItems[i]).Team == 2)
                                    ((Flag)gameWorld.PickupItems[i]).Base = gameWorld.RedBase;
                            }
                            else
                            {
                                ((Flag)gameWorld.PickupItems[i]).Base.SpawnFlag(gameWorld.PickupItems);
                                gameWorld.PickupItems.RemoveAt(i);
                            }
                        }
                    }
                }

                foreach (NetworkGamer player in m_sessionManager.NetSession.AllGamers)
                {
                    ((SpaceMarine)player.Tag).UpdateProjectiles(gt, m_sessionManager);
                }

                localPlayer.Update(gt, m_controller, gameWorld, m_crossHair);


                if (localPlayer != null)
                {
                    if (localPlayer.Rectangle.Intersects(gameWorld.WorldBoundary))
                        m_camera.updateMe(gt, Game.KbState, Game.MouseState);

                    if (localPlayer.isHit)
                        m_camera.doShake(1f, 3f, 1f);

                    m_crossHair.updateMe(Game.MouseState, m_camera, localPlayer);
                }

                m_matchController.updateMe(gameWorld);
                m_sessionManager.SendLocalUpdate(localPlayer);

                if (localPlayer.State != ActorState.Dying)
                {
                    m_camera.Follow(localPlayer, 0f);
                }

                if (m_matchController.Match != MatchType.DM)
                {
                    if (localPlayer.Team != 0)
                    {
                        if (localPlayer.State == ActorState.Dead)
                        {
                            m_spawnManager.RespawnPlayer(localPlayer, m_matchController);
                        }
                    }
                }
                else
                {
                    if (localPlayer.State == ActorState.Dead)
                    {
                        m_spawnManager.RespawnPlayer(localPlayer, m_matchController);
                    }
                }
            }

            if (m_sessionManager.gameEnded)
            {
                m_sessionManager.Dispose();
                Game.State = GameState.Menu;
                MessageBox.Show("Game Session has ended.");
            }
        }

        public void drawMe(SpriteBatch sBatch, SessionController sessionManager)
        {
            if (gameWorld.Map !=null)
            gameWorld.Map.drawMe(sBatch);

            foreach (NetworkGamer player in sessionManager.NetSession.AllGamers)
            {
                ((SpaceMarine)player.Tag).Draw(sBatch);
            }
            foreach (Asteroid asteroid in gameWorld.Asteroids)
            {
                asteroid.Draw(sBatch);
            }
            foreach (PickupItem item in gameWorld.PickupItems)
            {
                item.Draw(sBatch);
            }

            m_crossHair.Draw(sBatch);
        }

        private void setMatch(string Match)
        {
            if (Match == "DeathMatch")
                m_matchController.Match = MatchType.DM;
            if (Match == "Team Deathmatch")
                m_matchController.Match = MatchType.TDM;
            if (Match == "Capture The Flag")
                m_matchController.Match = MatchType.CTF;
        }
        private void setMatch(int Match)
        {
            m_matchController.Match = (MatchType)Match;
        }

        private SpawnPoint getSpawn(PickupItem Item)
        {
            foreach (SpawnPoint spawn in gameWorld.WeaponSpawns)
            {
                if (spawn.SpawnID == Item.SpawnID)
                    return spawn;
            }
            return null;
        }

        private Weapon createWeapon(string WeaponName)
        {
            switch (WeaponName)
            {
                case "MachineGun":
                    return new MachineGun(gameWorld.AssetManager);
                case "Pistol":
                    return new Pistol(gameWorld.AssetManager);
                case "LaserPistol":
                    return new LaserPistol(gameWorld.AssetManager);
                case "LaserRifle":
                    return new LaserRifle(gameWorld.AssetManager);
                case "RocketLancher":
                    return new RocketLauncher(gameWorld.AssetManager);
                case "PlasmaRifle":
                    return new PlasmaRifle(gameWorld.AssetManager);
            }
            return null;
        }
    }
}
