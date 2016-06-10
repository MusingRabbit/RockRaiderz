using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RockRaiders.Core.LevelObjects
{
    class Map
    {
        private List<CollisionTiles> m_collisionTiles;
        private List<CollisionTiles> m_backTiles;
        private List<Asteroid> asteroidList;
        private Texture2D m_tileSet;
        private int m_width, m_height;
        private int m_tileSize;

        public List<CollisionTiles> CollisionTile
        {
            get
            {
                return m_collisionTiles;
            }
        }

        public int Width
        {
            get
            {
                return m_width;
            }
        }
        public int Height
        {
            get
            {
                return m_height;
            }
        }

        public Map(Texture2D TileSet, int TileSize)
        {
            m_collisionTiles = new List<CollisionTiles>();
            m_tileSet = TileSet;
            m_tileSize = TileSize;
            m_width = 0;
            m_height = 0;
        }


        public void GenerateCollisionMap(int[,] map)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];

                    if (number > 0)
                    {
                        m_collisionTiles.Add(new CollisionTiles(number, new Rectangle(x * m_width, y * m_height, m_tileSize, m_tileSize), m_tileSet));
                    }


                    m_width = (x + 1) * m_tileSize;
                    m_height = (y + 1) * m_tileSize;
                }
            }
        }
        public void GenerateBackMap(int[,] map, int size)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];

                    if (number > 0)
                    {
                        m_backTiles.Add(new CollisionTiles(number, new Rectangle(x * size, y * size, size, size), m_tileSet));
                    }


                    m_width = (x + 1) * size;
                    m_height = (y + 1) * size;
                }
            }
        }

        public void drawMe(SpriteBatch sBatch)
        {
            foreach (CollisionTiles tile in m_backTiles)
            {
                tile.drawMe(sBatch);
            }
            foreach (CollisionTiles tile in m_collisionTiles)
                tile.drawMe(sBatch);
        }

    }
}
