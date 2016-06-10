using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace RockRaiders.Core.LevelObjects
{
    class TileMap
    {
        private List<CollisionTiles> m_collisionTiles;
        private List<CollisionTiles> m_backTiles;
        private List<BoundingBox> m_collisionList, m_interiorList;
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
        public List<BoundingBox> CollisionList
        {
            get
            {
                return m_collisionList;
            }
        }
        public List<BoundingBox> InteriorList
        {
            get
            {
                return m_interiorList;
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

        public TileMap(Texture2D TileSet, int TileSize)
        {
            m_collisionTiles = new List<CollisionTiles>();
            m_backTiles = new List<CollisionTiles>();
            m_collisionList = new List<BoundingBox>();
            m_interiorList = new List<BoundingBox>();
            m_tileSet = TileSet;
            m_tileSize = TileSize;

        }


        public void GenerateCollisionMap(int[,] map)
        {
            m_width = 0;
            m_height = 0;

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    int number = map[y, x];

                    if (number > 0)
                    {
                        m_collisionTiles.Add(new CollisionTiles(number, new Rectangle(x + m_width, y + m_height, m_tileSize + 1, m_tileSize + 1), m_tileSet));
                    }
                    m_width = (x) * m_tileSize;
                    m_height = (y) * m_tileSize;
                }
            }
            generateCollision();
        }

        public void GenerateBackMap(int[,] map)
        {
            m_width = 0;
            m_height = 0;

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    int number = map[y, x];

                    if (number > 0)
                    {
                        m_backTiles.Add(new CollisionTiles(number, new Rectangle(x + m_width, y + m_height, m_tileSize + 1, m_tileSize + 1), m_tileSet));
                    }
                    m_width = (x) * m_tileSize;
                    m_height = (y) * m_tileSize;
                }
            }
            //generateCollisionInterior();
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

        private void generateCollision()
        {
            int lineCount = 0;
            int tileGap;
            CollisionTiles currTile, firstTile, prevTile;
            prevTile = new CollisionTiles(0, new Rectangle(0, 0, 32, 32), null);

            for (int x = 0; x < m_collisionTiles.Count; x++)
            {
                currTile = m_collisionTiles[x];
                tileGap = currTile.Rectangle.X - prevTile.Rectangle.X;


                if (currTile.Rectangle.Y >= 4000)
                {
                }

                if (currTile.Rectangle.Y == prevTile.Rectangle.Y)
                {
                    if (lineCount == 0)
                    {
                        firstTile = currTile;
                    }

                    if ((tileGap > 33) && (lineCount > 0))
                    {
                        firstTile = m_collisionTiles[x - lineCount];
                        CollisionList.Add(new BoundingBox(new Vector3(firstTile.Rectangle.X - 32, firstTile.Rectangle.Y, 0),
                            new Vector3(prevTile.Rectangle.X + prevTile.Rectangle.Width, prevTile.Rectangle.Y + prevTile.Rectangle.Height, 0)));
                        Thread.Sleep(10);
                        lineCount = 0;
                    }
                    else
                    {
                        lineCount++;
                    }
                }
                else
                {
                    firstTile = m_collisionTiles[x - lineCount];
                    CollisionList.Add(new BoundingBox(new Vector3(firstTile.Rectangle.X - 32, firstTile.Rectangle.Y, 0),
                        new Vector3(prevTile.Rectangle.X + prevTile.Rectangle.Width, prevTile.Rectangle.Y + prevTile.Rectangle.Height, 0)));
                    Thread.Sleep(10);
                    lineCount = 0;
                }
                prevTile = currTile;
            }
            lineCount = 0;
        }

        private void generateCollisionInterior()
        {
            int lineCount = 0;
            int tileGap;
            CollisionTiles currTile, firstTile, prevTile;
            prevTile = new CollisionTiles(0, new Rectangle(0, 0, 32, 32), null);

            for (int x = 0; x < m_backTiles.Count; x++)
            {
                currTile = m_backTiles[x];
                tileGap = currTile.Rectangle.X - prevTile.Rectangle.X;

                if (currTile.Rectangle.Y == prevTile.Rectangle.Y)
                {
                    if (lineCount == 0)
                    {
                        firstTile = currTile;
                    }

                    if ((tileGap > 33) && (lineCount > 1))
                    {
                        firstTile = m_backTiles[x - lineCount];
                        m_interiorList.Add(new BoundingBox(new Vector3(firstTile.Rectangle.X, firstTile.Rectangle.Y, 0),
                            new Vector3(prevTile.Rectangle.X + prevTile.Rectangle.Width, prevTile.Rectangle.Y + prevTile.Rectangle.Height, 0)));
                        lineCount = 0;
                    }
                    else
                    {
                        lineCount++;
                    }
                }
                else
                {
                    firstTile = m_backTiles[x - lineCount];
                    m_interiorList.Add(new BoundingBox(new Vector3(firstTile.Rectangle.X, firstTile.Rectangle.Y, 0),
                        new Vector3(prevTile.Rectangle.X + prevTile.Rectangle.Width, prevTile.Rectangle.Y + prevTile.Rectangle.Height, 0)));
                    lineCount = 0;
                }

                prevTile = currTile;
            }
            lineCount = 0;
        }
    }
    
}
