using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.LevelObjects
{
    class SpawnPoint
    {
        private int m_team;
        private Vector2 m_location;
        private AssetManager m_assetManager;
        private Type m_weaponType;
        private int m_spawnID;

        public int Team
        {
            get
            {
                return m_team;
            }
        }
        public Vector2 Location
        {
            get
            {
                return m_location;
            }
        }
        public int SpawnID
        {
            get 
            {
                return m_spawnID;
            }
        }


        public SpawnPoint(Vector2 Location, int Team, int SpawnID)
        {
            m_location = Location;
            m_team = Team;
            m_spawnID = SpawnID;
        }
        public SpawnPoint(Vector2 Location, Weapon Weapon, AssetManager AssetManager, int SpawnID)
        {
            m_location = Location;
            m_assetManager = AssetManager;
            m_weaponType = Weapon.GetType();
            m_spawnID = SpawnID;
        }

        public void SpawnWeapon(List<PickupItem> itemList)
        {
            if (m_weaponType != null)
            {
                Weapon spawnWeapon = getWeapon(m_weaponType);
                spawnWeapon.Position = m_location;
                spawnWeapon.SpawnPont = this;
                spawnWeapon.SpawnID = m_spawnID ;
                itemList.Add(spawnWeapon);
            }
            else
            {

                return;
            }
        }
        private Weapon getWeapon(Type weaponType)
        {
            if (weaponType == typeof(MachineGun))
                return new MachineGun(m_assetManager);
            if (weaponType == typeof(Pistol))
                return new Pistol(m_assetManager);
            if (weaponType == typeof(PlasmaRifle))
                return new PlasmaRifle(m_assetManager);
            if (weaponType == typeof(LaserPistol))
                return new LaserPistol(m_assetManager);
            if (weaponType == typeof(LaserRifle))
                return new LaserRifle(m_assetManager);
            if (weaponType == typeof(RocketLauncher))
                return new RocketLauncher(m_assetManager);

            return null;
        }
    }
}
