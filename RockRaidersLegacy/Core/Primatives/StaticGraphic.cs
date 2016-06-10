using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RockRaiders.Core.Primatives
{
    class StaticGraphic
    {
        protected Vector2 m_pos;
        protected Color m_tint;
        protected Texture2D m_txr;

        public Vector2 Position
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }

        public StaticGraphic(Texture2D Texture, Vector2 Position)
        {
            m_pos = Position;
            m_txr = Texture;
            m_tint = Color.White;
        }
        public virtual void Draw(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr,m_pos,m_tint);
        }
    }
}
