using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.LevelObjects
{
    class Asteroid : StaticGraphic
    {
        private BoundingSphere m_boundingSphere;
        private Rectangle m_drawingRect;
        private Vector2 m_center;
        private int m_size;
        private int m_res;
        public Color Tint
        {
            get
            {
                return m_tint;
            }
            set
            {
                m_tint = value;
            }
        }
        public int Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }
        public BoundingSphere Sphere
        {
            get
            {
                return m_boundingSphere;
            }
        }
        public Vector2 Center
        {
            get
            {
                return m_center;
            }
        }


        public Asteroid(AssetManager AssetManager, Vector2 Position)
            : base(AssetManager.SpriteLib["Asteroid01"], Position)
        {
            m_size = m_txr.Width;
            m_res = 100;
            m_drawingRect = new Rectangle((int)m_pos.X, (int)m_pos.Y, m_size, m_size);
            m_center = new Vector2(m_pos.X + m_size / 2, m_pos.Y + m_size/2);
            m_boundingSphere = new BoundingSphere(new Vector3(m_center,0), m_size/2);
        }
        public void updateMe(GameTime gt)
        {
        }
        public override void Draw(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_drawingRect, m_tint);
        }
    }
}
