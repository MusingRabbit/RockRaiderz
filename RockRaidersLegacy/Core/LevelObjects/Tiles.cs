using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace RockRaiders.Core.LevelObjects
{
    class Tiles
    {
        protected Texture2D m_txr;
        protected int m_tileNum;
        private Rectangle m_rect;
        private BoundingBox m_box;
        protected Rectangle m_srcRect;

        public Rectangle Rectangle
        {
            get
            {
                return m_rect;
            }
            protected set
            {
                m_rect = value;
            }
        }
        public Rectangle SourceRect
        {
            get
            {
                return m_srcRect;
            }
            set
            {
                m_srcRect = value;
            }
        }
        public BoundingBox Box
        {
            get
            {
                return m_box;
            }
            protected set
            {
                m_box = value;
            }
        }
        public int TileNumber
        {
            get
            {
                return m_tileNum;
            }
        }

        public void drawMe(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_rect, m_srcRect, Color.White);
        }
    }

    class CollisionTiles : Tiles
    {
        public CollisionTiles(int i, Rectangle newRect, Texture2D TileSet)
        {
            m_tileNum = i;
            m_txr = TileSet;
            this.Rectangle = newRect;
            this.SourceRect = getTile(m_txr, m_tileNum);
            this.Box = new BoundingBox(new Vector3((float)this.Rectangle.X, (float)this.Rectangle.Y, 0),
                new Vector3((float)this.Rectangle.X + this.Rectangle.Width, (float)this.Rectangle.Y + this.Rectangle.Height, 0));
        }

        private Rectangle getTile(Texture2D tileSet, int TileID)
        {
            int tileSize = 32;
            int[,] tileGrid = new int[15, 16];
            int index = 0;
            int searchValue = TileID;
            int xPos, yPos;

            for (int y = 0; y < tileGrid.GetLength(0); y++)
            {
                for (int x = 0; x < tileGrid.GetLength(1); x++)
                {
                    yPos = y * tileSize;
                    xPos = x * tileSize;
                    if (index == searchValue)
                    {
                        return new Rectangle(xPos, yPos, tileSize, tileSize);
                    }
                    index++;
                }
            }
            Trace.WriteLine("Tile No : " + searchValue + ", not found.");
            return new Rectangle(0, 0, tileSize, tileSize);
        }
    }
}

