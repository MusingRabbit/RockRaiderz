using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

namespace RockRaiders.Core.LevelObjects
{
    class World
    {
        private List<Flag> m_flagList;
        private List<Asteroid> m_asteroidList;
        private List<SpawnPoint> m_spawnList;
        private List<SpawnPoint> m_wpnSpawnList;
        private List<PickupItem> m_itemList;

        private AssetManager m_assetManager;
        private TileMap m_tileMap;
        private List<int[,]> layerList;
        private List<int[,]> collisionLayer;
        private List<int[,]> backLayer;
        private Rectangle m_worldBoundary;
        private MatchController m_mController;
        private FlagBase _blueBase;
        private FlagBase _redBase;

        private int m_mapWidth, m_mapHeight, m_tileSize;
        private string[] m_mapData;
        private int[] m_mapDimensions;
        private string m_MapName;

        public List<Asteroid> Asteroids
        {
            get
            {
                return m_asteroidList;
            }
        }
        public List<PickupItem> PickupItems
        {
            get
            {
                return m_itemList;
            }
            set
            {
                m_itemList = value;
            }
        }
        public List<SpawnPoint> SpawnPoints
        {
            get
            {
                return m_spawnList;
            }
        }
        public List<SpawnPoint> WeaponSpawns
        {
            get
            {
                return m_wpnSpawnList;
            }
        }
        public AssetManager AssetManager
        {
            get
            {
                return m_assetManager;
            }
        }
        public TileMap Map
        {
            get
            {
                return m_tileMap;
            }
        }
        public Rectangle WorldBoundary
        {
            get
            {
                return m_worldBoundary;
            }
        }
        public string CurrentMap
        {
            get
            {
                return m_MapName;
            }
        }
        public MatchType CurrentMatch
        {
            get
            {
                return m_mController.Match;
            }
        }
        public FlagBase RedBase
        {
            get
            {
                return _redBase;
            }
        }
        public FlagBase BlueBase
        {
            get
            {
                return _blueBase;
            }
        }

        public World(AssetManager AssetManager)
        {
            layerList = new List<int[,]>();
            collisionLayer = new List<int[,]>();
            backLayer = new List<int[,]>();
            m_worldBoundary = new Rectangle();
            m_mapDimensions = new int[4];
            

            m_flagList = new List<Flag>();
            m_asteroidList = new List<Asteroid>();
            m_spawnList = new List<SpawnPoint>();
            m_itemList = new List<PickupItem>();
            m_wpnSpawnList = new List<SpawnPoint>();
            m_assetManager = AssetManager;
        }

        public void LoadMap(string MapName, MatchController matchController)
        {
            m_MapName = MapName;
            m_mController = matchController;
            loadTileMap(MapName);
            switch (MapName)
            {
                case ("Fool's Harvest"):
                    create2ShipsWorld(matchController.Match);
                    break;
                case ("Drifter"):
                    createDrifterWorld(matchController.Match);
                    break;
            }
        }

        private string[] getMapData(string MapName)
        {
            string fullPath = "Content\\Levels\\" + MapName + ".dat";
            bool done = false;
            string[] strSplit;
            string[] MapData = File.ReadAllLines(fullPath);
            for (int x = 0; x < MapData.Length; x++)
            {
                if (MapData[x].Contains("[header]"))
                {
                    for (int y = 1; y < 5; y++)
                    {
                        strSplit = MapData[x + y].Split('=');
                        m_mapDimensions[y - 1] = Convert.ToInt32(strSplit[1]);
                    }
                    x += 4;
                }
                if ((m_mapDimensions.Length > 0) && (!done))
                {
                    m_mapWidth = m_mapDimensions[0];
                    m_mapHeight = m_mapDimensions[1];
                    m_tileSize = (m_mapDimensions[2] + m_mapDimensions[3]) / 2;
                    done = true;
                }

                if (MapData[x].Contains("[layer]"))
                    layerList.Add(getLayerData(x, m_mapWidth, m_mapHeight, MapData));

                if ((MapData[x].Contains("[layer]") && (MapData[x + 1].Contains("BackLayer"))))
                    backLayer.Add(getLayerData(x, m_mapWidth, m_mapHeight, MapData));

                if ((MapData[x].Contains("[layer]")) && (MapData[x + 1].Contains("CollisionLayer")))
                    collisionLayer.Add(getLayerData(x, m_mapWidth, m_mapHeight, MapData));
            }
            return MapData;
        }

        private int[,] getLayerData(int currentIndex, int width, int height, string[] mapData)
        {
            int[,] layerData = new int[height, width];

            int dataIndex = currentIndex + 3;

            for (int y = 0; y < height - 1; y++)
            {
                string[] strRowValues = mapData[y + dataIndex].Split(',');
                for (int x = 0; x < strRowValues.Length - 1; x++)
                {
                    int tileValue = Convert.ToInt32(strRowValues[x]);
                    if (tileValue > 0)
                        tileValue -= 1;

                    layerData[y, x] = tileValue;
                }
            }
            return layerData;
        }

        private TileMap generateTileMap(List<int[,]> layerList, AssetManager AssetManager, int size)
        {
            TileMap map = new TileMap(AssetManager.SpriteLib["TileSet"], size);

            foreach (int[,] layer in backLayer)
            {
                map.GenerateBackMap(layer);
            }
            foreach (int[,] layer in collisionLayer)
            {
                map.GenerateCollisionMap(layer);
            }

            return map;
        }

        private void loadTileMap(string MapName)
        {
            m_mapData = getMapData(MapName);
            m_tileMap = generateTileMap(layerList, m_assetManager, m_tileSize);
        }

        private void createDrifterWorld(MatchType MatchType)
        {
            switch (MatchType)
            {
                case MatchType.DM:
                    createDrifterWorldDM();
                    break;
                case MatchType.TDM:
                    createDrifterWorldTDM();
                    break;
                case MatchType.CTF:
                    createDrifterWorldCTF();
                    break;
            }
            m_worldBoundary = new Rectangle(0, 1700, 7500, 4000);
        }

        private void create2ShipsWorld(MatchType MatchType)
        {
            switch (MatchType)
            {
                case MatchType.DM:
                    create2ShipsWorldDM();
                    break;
                case MatchType.TDM:
                    create2ShipsWorldTDM();
                    break;
                case MatchType.CTF:
                    create2ShipsWorldCTF();
                    break;
            }

            m_worldBoundary = new Rectangle(0, 2200, 7500, 4000);

        }

        private void create2ShipsWorldDM()
        {
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3000, 3800)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(4500, 4000)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3500, 4600)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(2500, 4500)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(4500, 5000)));

            #region Create Weapon Spawns 

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3170, 3775), new MachineGun(m_assetManager), m_assetManager, 1));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3170, 4165), new MachineGun(m_assetManager), m_assetManager, 2));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2675, 4475), new Pistol(m_assetManager), m_assetManager, 3));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2675, 4865), new LaserPistol(m_assetManager), m_assetManager, 4));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3667, 4965), new LaserRifle(m_assetManager), m_assetManager, 5));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(1325, 4135), new PlasmaRifle(m_assetManager), m_assetManager, 6));

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4675, 4970), new MachineGun(m_assetManager), m_assetManager, 7));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4675, 5365), new Pistol(m_assetManager), m_assetManager, 8));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4665, 3976), new LaserPistol(m_assetManager), m_assetManager, 9));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4665, 3465), new LaserRifle(m_assetManager), m_assetManager, 10));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(6000, 5100), new PlasmaRifle(m_assetManager), m_assetManager, 11));

            #endregion

            m_spawnList.Add(new SpawnPoint(new Vector2(1325, 4135), 0, 1));
            m_spawnList.Add(new SpawnPoint(new Vector2(2135, 3900), 0, 2));
            m_spawnList.Add(new SpawnPoint(new Vector2(4000, 3210), 0, 3));
            m_spawnList.Add(new SpawnPoint(new Vector2(6000, 4925), 0, 4));
            m_spawnList.Add(new SpawnPoint(new Vector2(5000, 4135), 0, 5));
            m_spawnList.Add(new SpawnPoint(new Vector2(2500, 5000), 0, 6));
            m_spawnList.Add(new SpawnPoint(new Vector2(1400, 4000), 0, 7));
            m_spawnList.Add(new SpawnPoint(new Vector2(5400, 3125), 0, 8));

        }
        private void create2ShipsWorldTDM()
        {
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3000, 3800)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(4500, 4000)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3500, 4600)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(2500, 4500)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(4500, 5000)));

            #region Create Weapon Spawns

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3170, 3775), new MachineGun(m_assetManager), m_assetManager, 1));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3170, 4165), new MachineGun(m_assetManager), m_assetManager, 2));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2675, 4475), new Pistol(m_assetManager), m_assetManager, 3));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2675, 4865), new LaserPistol(m_assetManager), m_assetManager, 4));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3667, 4965), new LaserRifle(m_assetManager), m_assetManager, 5));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(1325, 4135), new PlasmaRifle(m_assetManager), m_assetManager, 6));

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4675, 4970), new MachineGun(m_assetManager), m_assetManager, 7));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4675, 5365), new Pistol(m_assetManager), m_assetManager, 8));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4665, 3976), new LaserPistol(m_assetManager), m_assetManager, 9));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4665, 3465), new LaserRifle(m_assetManager), m_assetManager, 10));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(6000, 5100), new PlasmaRifle(m_assetManager), m_assetManager, 11));

            #endregion

            m_spawnList.Add(new SpawnPoint(new Vector2(1325, 4135), 1, 1));
            m_spawnList.Add(new SpawnPoint(new Vector2(2135, 3900), 1, 2));
            m_spawnList.Add(new SpawnPoint(new Vector2(4000, 3210), 2, 3));
            m_spawnList.Add(new SpawnPoint(new Vector2(6000, 4925), 2, 4));
            m_spawnList.Add(new SpawnPoint(new Vector2(5000, 4135), 2, 5));
            m_spawnList.Add(new SpawnPoint(new Vector2(2500, 5000), 1, 6));
            m_spawnList.Add(new SpawnPoint(new Vector2(1400, 4000), 1, 7));
            m_spawnList.Add(new SpawnPoint(new Vector2(5400, 3125), 2, 8));
        }
        private void create2ShipsWorldCTF()
        {
            _blueBase = new FlagBase(new Vector2(1350, 4135), 1, m_assetManager);
            _redBase = new FlagBase(new Vector2(5975, 4925), 2, m_assetManager);

            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3000, 3800)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(4500, 4000)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3500, 4600)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(2500, 4500)));
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(4500, 5000)));

            #region Create Weapon Spawns

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3170, 3775), new MachineGun(m_assetManager), m_assetManager, 1));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3170, 4165), new MachineGun(m_assetManager), m_assetManager, 2));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2675, 4475), new Pistol(m_assetManager), m_assetManager, 3));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2675, 4865), new LaserPistol(m_assetManager), m_assetManager, 4));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3667, 4965), new LaserRifle(m_assetManager), m_assetManager, 5));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(1450, 4135), new PlasmaRifle(m_assetManager), m_assetManager, 6));

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4675, 4970), new MachineGun(m_assetManager), m_assetManager, 7));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4675, 5365), new Pistol(m_assetManager), m_assetManager, 8));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4665, 3976), new LaserPistol(m_assetManager), m_assetManager, 9));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4665, 3465), new LaserRifle(m_assetManager), m_assetManager, 10));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(5125, 5100), new PlasmaRifle(m_assetManager), m_assetManager, 11));

            #endregion

            m_spawnList.Add(new SpawnPoint(new Vector2(1325, 4135), 1, 1));
            m_spawnList.Add(new SpawnPoint(new Vector2(2135, 3900), 1, 2));
            m_spawnList.Add(new SpawnPoint(new Vector2(4000, 3210), 2, 3));
            m_spawnList.Add(new SpawnPoint(new Vector2(6000, 4925), 2, 4));
            m_spawnList.Add(new SpawnPoint(new Vector2(5000, 4135), 2, 5));
            m_spawnList.Add(new SpawnPoint(new Vector2(2500, 5000), 1, 6));
            m_spawnList.Add(new SpawnPoint(new Vector2(1400, 4000), 1, 7));
            m_spawnList.Add(new SpawnPoint(new Vector2(5400, 3125), 2, 8));

        }

        private void createDrifterWorldDM()
        {
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3600, 4150)));

            foreach (Asteroid asteroid in m_asteroidList)
                asteroid.Tint = Color.DarkGray;

            #region Create Weapon Spawns

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3196, 3890), new MachineGun(m_assetManager), m_assetManager, 1));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3730, 3890), new MachineGun(m_assetManager), m_assetManager, 2));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4221, 3890), new Pistol(m_assetManager), m_assetManager, 3));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3775, 4120), new LaserPistol(m_assetManager), m_assetManager, 4));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3775, 4516), new LaserRifle(m_assetManager), m_assetManager, 5));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2800, 4400), new PlasmaRifle(m_assetManager), m_assetManager, 6));

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4590, 4420), new MachineGun(m_assetManager), m_assetManager, 7));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(5235, 3740), new Pistol(m_assetManager), m_assetManager, 8));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(5270, 4160), new LaserPistol(m_assetManager), m_assetManager, 9));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2200, 3740), new LaserRifle(m_assetManager), m_assetManager, 10));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(1600, 4160), new PlasmaRifle(m_assetManager), m_assetManager, 11));

            #endregion

            m_spawnList.Add(new SpawnPoint(new Vector2(1690, 4200), 0, 1));
            m_spawnList.Add(new SpawnPoint(new Vector2(1690, 3800), 0, 2));
            m_spawnList.Add(new SpawnPoint(new Vector2(2000, 3800), 0, 3));
            m_spawnList.Add(new SpawnPoint(new Vector2(5400, 4525), 0, 4));
            m_spawnList.Add(new SpawnPoint(new Vector2(5700, 4200), 0, 5));
            m_spawnList.Add(new SpawnPoint(new Vector2(5800, 3700), 0, 6));
            m_spawnList.Add(new SpawnPoint(new Vector2(3120, 3800), 0, 7));
            m_spawnList.Add(new SpawnPoint(new Vector2(2400, 2000), 0, 8));

        }
        private void createDrifterWorldTDM()
        {
            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3600, 4150)));

            foreach (Asteroid asteroid in m_asteroidList)
                asteroid.Tint = Color.DarkGray;

            #region Create Weapon Spawns

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3200, 3890), new MachineGun(m_assetManager), m_assetManager, 1));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3730, 3890), new MachineGun(m_assetManager), m_assetManager, 2));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4220, 3890), new Pistol(m_assetManager), m_assetManager, 3));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3775, 4120), new LaserPistol(m_assetManager), m_assetManager, 4));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3775, 4516), new LaserRifle(m_assetManager), m_assetManager, 5));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2800, 4400), new PlasmaRifle(m_assetManager), m_assetManager, 6));

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4590, 4420), new MachineGun(m_assetManager), m_assetManager, 7));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(5235, 3740), new Pistol(m_assetManager), m_assetManager, 8));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(5270, 4160), new LaserPistol(m_assetManager), m_assetManager, 9));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2200, 3740), new LaserRifle(m_assetManager), m_assetManager, 10));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(1600, 4160), new PlasmaRifle(m_assetManager), m_assetManager, 11));

            #endregion

            m_spawnList.Add(new SpawnPoint(new Vector2(1690, 4200), 1, 1));
            m_spawnList.Add(new SpawnPoint(new Vector2(1690, 3800), 1, 2));
            m_spawnList.Add(new SpawnPoint(new Vector2(2000, 3800), 1, 3));
            m_spawnList.Add(new SpawnPoint(new Vector2(5400, 4525), 2, 4));
            m_spawnList.Add(new SpawnPoint(new Vector2(5700, 4200), 2, 5));
            m_spawnList.Add(new SpawnPoint(new Vector2(5800, 3700), 2, 6));
            m_spawnList.Add(new SpawnPoint(new Vector2(3120, 3800), 2, 7));
            m_spawnList.Add(new SpawnPoint(new Vector2(2400, 2000), 1, 8));

        }
        private void createDrifterWorldCTF()
        {
            _blueBase = new FlagBase(new Vector2(1350, 4135), 1, m_assetManager);
            _redBase = new FlagBase(new Vector2(6070, 4135), 2, m_assetManager);

            m_asteroidList.Add(new Asteroid(m_assetManager, new Vector2(3600, 4150)));

            foreach (Asteroid asteroid in m_asteroidList)
                asteroid.Tint = Color.DarkGray;

            #region Create Weapon Spawns

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3196, 3890), new MachineGun(m_assetManager), m_assetManager, 1));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3730, 3890), new MachineGun(m_assetManager), m_assetManager, 2));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4221, 3890), new Pistol(m_assetManager), m_assetManager, 3));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3775, 4120), new LaserPistol(m_assetManager), m_assetManager, 4));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(3775, 4516), new LaserRifle(m_assetManager), m_assetManager, 5));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2800, 4400), new PlasmaRifle(m_assetManager), m_assetManager, 6));

            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(4590, 4420), new MachineGun(m_assetManager), m_assetManager, 7));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(5235, 3740), new Pistol(m_assetManager), m_assetManager, 8));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(5270, 4160), new LaserPistol(m_assetManager), m_assetManager, 9));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(2200, 3740), new LaserRifle(m_assetManager), m_assetManager, 10));
            m_wpnSpawnList.Add(new SpawnPoint(new Vector2(1600, 4160), new PlasmaRifle(m_assetManager), m_assetManager, 11));

            #endregion

            m_spawnList.Add(new SpawnPoint(new Vector2(1690, 4200), 1, 1));
            m_spawnList.Add(new SpawnPoint(new Vector2(1690, 3800), 1, 2));
            m_spawnList.Add(new SpawnPoint(new Vector2(2000, 3800), 1, 3));
            m_spawnList.Add(new SpawnPoint(new Vector2(5400, 4525), 2, 4));
            m_spawnList.Add(new SpawnPoint(new Vector2(5700, 4200), 2, 5));
            m_spawnList.Add(new SpawnPoint(new Vector2(5800, 3700), 2, 6));
            m_spawnList.Add(new SpawnPoint(new Vector2(3120, 3800), 2, 7));
            m_spawnList.Add(new SpawnPoint(new Vector2(2400, 2000), 1, 8));
        }
        
    }
}
