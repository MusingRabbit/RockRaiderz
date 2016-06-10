using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.Controllers
{
    class SessionController
    {
        private AvailableNetworkSessionCollection _availableSessions;
        private List<AvailableNetworkSession> _sessionList;
        private GameController _gameManager;
        private PacketReader _packetReader;
        private PacketWriter _packetWriter;
        private NetworkSession _netSession;
        private AssetManager _assetManager;
        private List<PickupItem> _remoteItemList;
        private MatchController _remoteMatch;
        private NetworkGamer _sender;
        private string _remoteMapName;
        private int _remoteMatchType;
        private bool _failure;

        private bool _isHost;
        private bool _isClient;
        private bool _hasLeft;

        private PolyInputController _controlTransfer;
        private CrossHair _crosshairTransfer;
        private World _currWorldState;

        public NetworkSession NetSession
        {
            get
            {
                return _netSession;
            }
        }
        public GameController GameManager
        {
            set
            {
                _gameManager = value;
            }
        }
        public List<AvailableNetworkSession> SessionList
        {
            get
            {
                return _sessionList;
            }
        }
        public List<PickupItem> RecievedItemList
        {
            get
            {
                return _remoteItemList;
            }
        }
        public string RecievedMapName
        {
            get
            {
                return _remoteMapName;
            }
        }
        public int RecievedMatchType
        {
            get
            {
                return _remoteMatchType;
            }
        }
        public MatchController RecievedMatch
        {
            get
            {
                return _remoteMatch;
            }
        }
        public bool FailedToConnect
        {
            get
            {
                return _failure;
            }
            set
            {
                _failure = value;
            }
        }

        public bool isHost
        {
            get
            {
                return _isHost;
            }
        }
        public bool isClient
        {
            get
            {
                return _isClient;
            }
        }
        public bool hasLeft
        {
            get
            {
                return _hasLeft;
            }
        }
        public bool gameEnded
        {
            get
            {
                if (_netSession != null)
                    return (_netSession.SessionState == NetworkSessionState.Ended);
                else return false;
            }
        }
        public SessionController(AssetManager AssetManager)
        {
            _packetReader = new PacketReader();
            _packetWriter = new PacketWriter();
            _sessionList = new List<AvailableNetworkSession>();
            _isHost = false;
            _isClient = false;
            _hasLeft = false;
            _assetManager = AssetManager;
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
            _remoteMapName = "";
            _remoteMatchType = 0;
        }

        public void Update(World gameWorld)
        {
            if ((_isHost) && (gameWorld != null))
                _currWorldState = gameWorld;

            Update();
        }
        public void Update()
        {
            if (_netSession != null)
                _netSession.Update();
        }

        public void HostSession()
        {
            if (_netSession!=null)
                _netSession.Dispose();

            _netSession = null;
            _netSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 10);
            _netSession.AllowHostMigration = true;
            _netSession.AllowJoinInProgress = true;
            _isHost = true;
            initialiseEventHandlers();
        }

        public void JoinSession(AvailableNetworkSession Session)
        {
            try
            {
                _netSession = NetworkSession.Join(Session);
            }
            catch
            {
                return;
            }
            _isClient = true;
            initialiseEventHandlers();
        }

        public void doSessionSearch()
        {
            if (_availableSessions == null)
            {
                if (_netSession != null)
                {
                    _netSession.Dispose();
                    _netSession = null;
                }
                _availableSessions = NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);
            }

            if (_availableSessions != null)
            {
                foreach (AvailableNetworkSession session in _availableSessions)
                {
                    _sessionList.Add(session);
                }
            }
        }

        public void GetGameUpdates(GameTime gameTime, World GameWorld)
        {
            foreach (LocalNetworkGamer gamer in _netSession.LocalGamers)
            {
                ProcessRemoteGamer(gamer, gameTime, GameWorld);
            }
        }

        public void SendLocalUpdate(SpaceMarine LocalPlayer)
        {
            foreach (LocalNetworkGamer gamer in _netSession.LocalGamers)
            {
                WriteLocalProperties(LocalPlayer);
                WriteLocalInput(LocalPlayer);
                WriteLocalAim(LocalPlayer);
                gamer.SendData(_packetWriter, SendDataOptions.None);
            }
        }

        private void ProcessRemoteGamer(LocalNetworkGamer gamer, GameTime gameTime, World GameWorld)
        {
            while (gamer.IsDataAvailable)
            {
                gamer.ReceiveData(_packetReader, out _sender);

                if (!_sender.IsLocal)
                {
                    SpaceMarine player = _sender.Tag as SpaceMarine;
                    ValidatePlayer(player);
                    player.Update(gameTime, ReadRemoteInput(), GameWorld, ReadRemoteAim());
                }
                else
                    break;
            }
        }

        private PolyInputController ReadRemoteInput()
        {
            _controlTransfer = new PolyInputController();

            _controlTransfer.DownPressed = _packetReader.ReadBoolean();
            _controlTransfer.DropFlag = _packetReader.ReadBoolean();
            _controlTransfer.DropWeapon = _packetReader.ReadBoolean();
            _controlTransfer.LeftPressed = _packetReader.ReadBoolean();
            _controlTransfer.OnePressed = _packetReader.ReadBoolean();
            _controlTransfer.RightPressed = _packetReader.ReadBoolean();
            _controlTransfer.SpacePressed = _packetReader.ReadBoolean();
            _controlTransfer.TwoPressed = _packetReader.ReadBoolean();
            _controlTransfer.UpPressed = _packetReader.ReadBoolean();
            _controlTransfer.UsePressed = _packetReader.ReadBoolean();

            return _controlTransfer;
        }

        private CrossHair ReadRemoteAim()
        {
            _crosshairTransfer = new CrossHair(_assetManager.SpriteLib["Crosshair"]);
            _crosshairTransfer.TargetPos = _packetReader.ReadVector2();
            _crosshairTransfer.Position = _packetReader.ReadVector2();
            _crosshairTransfer.isFiring = _packetReader.ReadBoolean();
            return _crosshairTransfer;
        }

        private string ReadMapData()
        {
            string mapName = "";

                mapName = _packetReader.ReadString();

            return mapName;
        }

        private int ReadMatchType()
        {
            int matchType=999;

            if (_sender.IsHost)
                matchType = _packetReader.ReadInt32();

            return matchType;
        }
        private List<PickupItem> ReadRemoteItems(LocalNetworkGamer gamer)
        {
            List<PickupItem> itemList = new List<PickupItem>();
            int itemID;
            int ammoCount;
            int spawnID;
            int team;
            Vector2 itemPos;
            PickupItem item;

            while (gamer.IsDataAvailable)
            {
                if (_sender.IsHost)
                {
                    itemID = _packetReader.ReadInt32();
                    itemPos = _packetReader.ReadVector2();
                    spawnID = _packetReader.ReadInt32();

                    item = getItem(itemID);
                    if (item != null)
                    {
                        item.Position = itemPos;
                        if (itemID > 10)
                        {
                            ammoCount = _packetReader.ReadInt32();
                            ((Weapon)item).AmmoCount = ammoCount;
                        }
                        else if (itemID == 1)
                        {
                            team = _packetReader.ReadInt32();
                            ((Flag)item).Team = team;
                        }
                        item.SpawnID = spawnID;
                        itemList.Add(item);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return itemList;
        }

        private PickupItem getItem(int ItemID)
        {
            if (ItemID == 1)
                return new Flag(_assetManager);
            else if (ItemID == 11)
                return new MachineGun(_assetManager);
            else if (ItemID == 12)
                return new Pistol(_assetManager);
            else if (ItemID == 13)
                return new LaserPistol(_assetManager);
            else if (ItemID == 14)
                return new LaserRifle(_assetManager);
            else if (ItemID == 15)
                return new RocketLauncher(_assetManager);
            else if (ItemID == 16)
                return new PlasmaRifle(_assetManager);
            else
                return null;
        }

        private void WriteMapData(World world)
        {

            string MapName;

            MapName = world.CurrentMap;

            _packetWriter.Write(MapName);

        }

        private void WriteMatchData(GameController game)
        {
            int matchType;
            matchType = (int)game.GameWorld.CurrentMatch;
            _packetWriter.Write(matchType);
            _packetWriter.Write(game.MatchController.BlueTeam.TeamScore);
            _packetWriter.Write(game.MatchController.RedTeam.TeamScore);
            _packetWriter.Write(game.MatchController.BlueTeam.Caps);
            _packetWriter.Write(game.MatchController.RedTeam.Caps);
        }

        private void WriteLocalItems(World world)
        {
            Type itemType;
            foreach (PickupItem item in _currWorldState.PickupItems)
            {
                itemType = item.GetType();
                _packetWriter.Write(item.ItemID);
                _packetWriter.Write(item.Position);
                _packetWriter.Write(item.SpawnID);

                if ((itemType.BaseType == typeof(Weapon)) || (itemType == typeof(Weapon)))
                {
                    _packetWriter.Write(((Weapon)item).AmmoCount);
                }
                if ((itemType.BaseType == typeof(Flag)) || (itemType == typeof(Flag)))
                {
                    _packetWriter.Write(((Flag)item).Team);
                }
            }
        }

        private void WriteLocalProperties(SpaceMarine LocalPlayer)
        {
            _packetWriter.Write(LocalPlayer.Position);
            _packetWriter.Write(LocalPlayer.Health);
            _packetWriter.Write(SendState(LocalPlayer));
            _packetWriter.Write(LocalPlayer.Score);
            _packetWriter.Write(LocalPlayer.Team);

            if (LocalPlayer.CurrentWeapon != null)
            {
                _packetWriter.Write(LocalPlayer.CurrentWeapon.ItemID);
                _packetWriter.Write(LocalPlayer.CurrentWeapon.SpawnID);
            }
            else
            {
                _packetWriter.Write(-1);
                _packetWriter.Write(-1);
            }
                

            if (LocalPlayer.FlagSlot != null)
            {
                _packetWriter.Write(LocalPlayer.FlagSlot.ItemID);
                _packetWriter.Write(LocalPlayer.FlagSlot.Team);
            }
            else
            {
                _packetWriter.Write(-1);
                _packetWriter.Write(0);
            }
        }

        private ActorState GetState(int recievedState)
        {
            if (recievedState == 0)
                return ActorState.Dead;
            else if (recievedState == 1)
                return ActorState.Floating;
            else if (recievedState == 2)
                return ActorState.Bound;
            else
                return ActorState.Dying;
        }

        private int SendState(SpaceMarine LocalPlayer)
        {
            switch (LocalPlayer.State)
            {
                case ActorState.Dead:
                    return 0;
                case ActorState.Floating:
                    return 1;
                case ActorState.Bound:
                    return 2;
            }
            return -1;
        }

        private void WriteLocalInput(SpaceMarine LocalPlayer)
        {
            _controlTransfer = LocalPlayer.ActiveControl;

            _packetWriter.Write(_controlTransfer.DownPressed);
            _packetWriter.Write(_controlTransfer.DropFlag);
            _packetWriter.Write(_controlTransfer.DropWeapon);
            _packetWriter.Write(_controlTransfer.LeftPressed);
            _packetWriter.Write(_controlTransfer.OnePressed);
            _packetWriter.Write(_controlTransfer.RightPressed);
            _packetWriter.Write(_controlTransfer.SpacePressed);
            _packetWriter.Write(_controlTransfer.TwoPressed);
            _packetWriter.Write(_controlTransfer.UpPressed);
            _packetWriter.Write(_controlTransfer.UsePressed);
        }

        private void WriteLocalAim(SpaceMarine LocalPlayer)
        {
            _crosshairTransfer = LocalPlayer.CrossHair;
            _packetWriter.Write(_crosshairTransfer.TargetPos);
            _packetWriter.Write(_crosshairTransfer.Position);
            _packetWriter.Write(_crosshairTransfer.isFiring);
        }

        private void initialiseEventHandlers()
        {
            _netSession.GamerJoined += new EventHandler<GamerJoinedEventArgs>(netSession_GamerJoined);
            _netSession.GamerLeft += new EventHandler<GamerLeftEventArgs>(netSession_GamerLeft);
        }

        private SpaceMarine GetPlayer(string gamerTag)
        {
            SpaceMarine playerCharacter = new SpaceMarine(_assetManager);
            playerCharacter.PlayerName = gamerTag;
            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                if (gamer.Gamertag == gamerTag)
                    return gamer.Tag as SpaceMarine;
            }
            return playerCharacter;
        }

        private void ValidatePlayer(SpaceMarine PlayerCharacter)
        {
            Weapon currentWeapon;
            Flag flag;
            Vector2 remotePosition = _packetReader.ReadVector2();
            int remoteHealth = _packetReader.ReadInt32();
            ActorState remoteState = GetState(_packetReader.ReadInt32());
            int remoteScore = _packetReader.ReadInt32();
            int remoteTeam = _packetReader.ReadInt32();
            int weaponID = _packetReader.ReadInt32();
            int weaponSpawnID = _packetReader.ReadInt32();
            int flagID = _packetReader.ReadInt32();
            int flagTeam = _packetReader.ReadInt32();

            if (PositionInvalid(PlayerCharacter.Position, remotePosition))
                PlayerCharacter.Position = remotePosition;

            if (remoteState != PlayerCharacter.State)
                PlayerCharacter.State = remoteState;

            if (weaponID != -1)
            {
                currentWeapon = (Weapon)getItem(weaponID);

                if (currentWeapon == null)
                    return;

                currentWeapon.User = PlayerCharacter;

                if (weaponSpawnID != -1) 
                currentWeapon.SpawnID = weaponSpawnID;

                if (PlayerCharacter.CurrentWeapon == null)
                    PlayerCharacter.CurrentWeapon = currentWeapon;

                else if (currentWeapon.Name != PlayerCharacter.CurrentWeapon.Name)
                    PlayerCharacter.CurrentWeapon = currentWeapon;
            }
            if (flagID != -1)
            {
                flag = (Flag)getItem(flagID);
                flag.Team = flagTeam;
                PlayerCharacter.FlagSlot = flag;
            }

            PlayerCharacter.Health = remoteHealth;
            PlayerCharacter.Score = remoteScore;
            PlayerCharacter.Team = remoteTeam;
            
            if (PlayerCharacter.Team == 1)
            {
                if (!_gameManager.MatchController.BlueTeam.Members.Contains(PlayerCharacter))
                    _gameManager.MatchController.BlueTeam.AddTeamMember(PlayerCharacter);
            }
            if (PlayerCharacter.Team == 2)
            {
                if (!_gameManager.MatchController.RedTeam.Members.Contains(PlayerCharacter))
                    _gameManager.MatchController.RedTeam.AddTeamMember(PlayerCharacter);
            }
            if (PlayerCharacter.Team == 0)
            {
                if (!_gameManager.MatchController.NoTeam.Members.Contains(PlayerCharacter))
                    _gameManager.MatchController.NoTeam.AddTeamMember(PlayerCharacter);
            }
        }

        private bool PositionInvalid(Vector2 localPosition, Vector2 remotePosition)
        {
            Vector2 discrepency;
            float magnitude;
            discrepency = localPosition - remotePosition;
            magnitude = Math.Abs(discrepency.X + discrepency.Y);

            return (magnitude > 1.5f);
        }

        #region EventHandlers

        private void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            e.Gamer.Tag = new SpaceMarine(_assetManager);
            ((SpaceMarine)e.Gamer.Tag).PlayerName = e.Gamer.Gamertag;
        }

        private void netSession_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            SpaceMarine playerCharacter = new SpaceMarine(_assetManager);

            playerCharacter.PlayerName = e.Gamer.Gamertag;

            if (!e.Gamer.IsLocal)
            {
                playerCharacter.isRemote = true;
                e.Gamer.Tag = playerCharacter;
            }
            else
            {
                playerCharacter = GetPlayer(e.Gamer.Gamertag);
                playerCharacter.isLocal = true;
                e.Gamer.Tag = playerCharacter;
            }


            if ((_isHost) && _currWorldState != null)
            {
                WriteMapData(_currWorldState);
                WriteMatchData(_gameManager);
                WriteLocalItems(_currWorldState);
            }

            else if (_isClient)
            {
                _packetReader.BaseStream.Flush();
                if (doClientConnection() == false)
                {
                    _failure = true;
                }
            }
        }

        private bool doClientConnection()
        {
            int loopCount = 0;

            foreach (LocalNetworkGamer gamer in _netSession.LocalGamers)
            {
                gamer.ReceiveData(_packetReader, out _sender);
                while (_sender == null)
                {
                    loopCount++;

                    if (loopCount > 600)
                        return false;
                }

                if (_sender.IsHost)
                {
                    if (_remoteMapName == "")
                    {
                        _remoteMapName = ReadMapData();
                    }
                    if (_remoteMatch == null)
                    {
                        _remoteMatch = new MatchController();
                        _remoteMatch.Match = (MatchType)ReadMatchType();
                        _remoteMatch.BlueTeam.TeamScore = _packetReader.ReadInt32();
                        _remoteMatch.RedTeam.TeamScore = _packetReader.ReadInt32();
                        _remoteMatch.BlueTeam.Caps = _packetReader.ReadInt32();
                        _remoteMatch.RedTeam.Caps = _packetReader.ReadInt32();
                    }

                    if (_remoteItemList == null)
                        _remoteItemList = ReadRemoteItems(gamer);
                }
                else
                {
                    _packetReader.BaseStream.Flush();
                    doClientConnection();
                }
            }
            return true;
        }

        private void netSession_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            if (e.Gamer.IsLocal)
            {
                _netSession.Dispose();
                _netSession = null;
                _hasLeft = true;
                _remoteMapName = "";
                _packetReader.Close();
                _packetWriter.Close();
            }
        }

        public void Dispose()
        {
            if (_netSession.IsHost)
                _netSession.EndGame();

            _netSession.Dispose();
            _netSession = null;
            _availableSessions = null;
            _hasLeft = true;
            _remoteMapName = "";
            _packetReader.BaseStream.Flush();
            _packetWriter.BaseStream.Flush();
            _remoteItemList = null;
            _remoteMatch = null;
            _remoteMatchType = 999;
            _sender = null;
            _sessionList.Clear();
            _isHost = false;
            _isClient = false;
            _failure = false;
        }
        #endregion
    }
}
 