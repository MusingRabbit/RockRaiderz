using System.Collections.Generic;

namespace RockRaiders.Core.Controllers
{
    class SpawnController
    {
        private List<SpawnPoint> _spawnList;
        private List<SpawnPoint> _weaponSpawnList;
        private int _spawnIndex;
        private SpawnPoint _selectedSpawn;

        public List<SpawnPoint> SpawnList
        {
            get
            {
                return _spawnList;
            }
            set
            {
                _spawnList = value;
            }
        }
        public List<SpawnPoint> WeaponSpawnList
        {
            get
            {
                return _weaponSpawnList;
            }
            set
            {
                _weaponSpawnList = value;
            }
        }

        public SpawnController()
        {
            _spawnList = new List<SpawnPoint>();
            _weaponSpawnList = new List<SpawnPoint>();
            _spawnIndex = 0;
        }

        public void SpawnAllWeapons(List<PickupItem> worldItems)
        {
            foreach (SpawnPoint wpnSpawn in _weaponSpawnList)
                wpnSpawn.SpawnWeapon(worldItems);
        }
        private bool getAvailableSpawn(List<SpawnPoint> SpawnList)
        {
            if (SpawnList.Count > 0)
            {
                _spawnIndex = Game1.RNG.Next(0, SpawnList.Count - 1);

                if (_spawnIndex != -1)
                {
                    _selectedSpawn = SpawnList[_spawnIndex];
                    return true;
                }
            }

            return false;
        }
        public void RespawnPlayer(SpaceMarine Player, MatchController Controller)
        {
            if (getAvailableSpawn(_spawnList))
            {
                if (Controller.Match != MatchType.CTF)
                    doSpawn(Player, _selectedSpawn);
                else
                    if (_spawnList[_spawnIndex].Team != Player.Team)
                        RespawnPlayer(Player, Controller);
                    else
                        doSpawn(Player, _selectedSpawn);
            }
        }
        private void doSpawn(SpaceMarine Player, SpawnPoint spawn)
        {
            Player.Position = spawn.Location;
            Player.Health = 100;
            Player.State = ActorState.Floating;
        }
    }
}
