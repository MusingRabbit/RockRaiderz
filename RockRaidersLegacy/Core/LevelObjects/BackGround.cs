using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.LevelObjects
{
    class BackGround : StaticGraphic
    {
        private Rectangle m_backGroundRect;
        private Vector2 m_offset;
        private AssetManager m_assetManager;


        public BackGround(AssetManager assetManager, int ScreenWidth, int ScreenHeight)
            : base(assetManager.SpriteLib["BackGround_1"], Vector2.Zero)
        {
            m_backGroundRect = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
            m_offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            m_assetManager = assetManager;
        }
        public void updateMe(GameManager gameManager)
        {
            if (gameManager.GameWorld != null)
            {
                string currMap = gameManager.GameWorld.CurrentMap;

                switch (currMap)
                {
                    case ("Fool's Harvest"):
                        m_txr = m_assetManager.SpriteLib["BackGround_1"];
                        break;
                    case ("Drifter"):
                        m_txr = m_assetManager.SpriteLib["BackGround_2"];
                        break;
                }
            }
        }

        public override void Draw(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_backGroundRect, m_tint);
        }

    }
}
