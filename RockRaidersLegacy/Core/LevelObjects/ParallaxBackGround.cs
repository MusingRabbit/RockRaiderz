using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.LevelObjects
{
    class ParallaxBackGround : StaticGraphic
    {
        private Rectangle m_backGroundRect;
        private Vector2 m_offset;


        public ParallaxBackGround(Texture2D txrBack, Texture2D txrDeep, Texture2D txrMid, int ScreenWidth, int ScreenHeight)
            : base(txrBack, Vector2.Zero)
        {
            m_backGroundRect = new Rectangle((int)m_pos.X, (int)m_pos.Y, ScreenWidth, ScreenHeight);
            m_offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
        }
        public void updateMe(GameTime gt, Camera2D camera)
        {
            m_pos = camera.Position - m_offset;
            m_backGroundRect.X = (int)m_pos.X;
            m_backGroundRect.Y = (int)m_pos.Y;
        }

        public override void Draw(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_backGroundRect, m_tint);
        }

    }
}
