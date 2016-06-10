using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.Primatives
{
    class Sprite : MotionGraphic
    {
        protected Rectangle m_srcRect;
        protected bool m_isAnimated;
        private float m_trigger;
        private int m_fps;
        protected float m_rotation;
        protected Vector2 m_rotationPoint;
        protected SpriteEffects m_sEffect;
        
        public int FrameRate
        {
            get
            {
                return m_fps;
            }
            set
            {
                m_fps = value;
            }
        }
        public bool animationEnabled
        {
            get
            {
                return m_isAnimated;
            }
            set
            {
                m_isAnimated = value;
            }
        }
        public SpriteEffects spriteEffect
        {
            get
            {
                return m_sEffect;
            }
            set
            {
                m_sEffect = value;
            }
        }
        public Vector2 RotationPoint
        {
            get
            {
                return m_rotationPoint;
            }
            set
            {
                m_rotationPoint = value;
            }
        }
        public float Rotation
        {
            get
            {
                return m_rotation;
            }
            set
            {
                m_rotation = value;
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
        public Texture2D Texture
        {
            get
            {
                return m_txr;
            }
            set
            {
                m_txr = value;
            }
        }

        public Sprite(Texture2D SpriteSheet, Vector2 Position, Rectangle srcRect)
            : base(SpriteSheet, Position, Vector2.Zero)
        {
            m_fps = 10;
            m_srcRect = srcRect;
            m_isAnimated = false;
            m_trigger = 0;
            m_rotation = 0f;
            m_rotationPoint = new Vector2(m_srcRect.Width / 2, m_srcRect.Height / 2);
            m_sEffect = SpriteEffects.None;

        }

        public Sprite(Texture2D Texture, Vector2 Position)
            : base (Texture,Position,Vector2.Zero)
        {
            m_isAnimated = false;
            m_trigger = 0;
            m_rotation = 0f;
            m_sEffect = SpriteEffects.None;

        }

        public override void updateMe(GameTime gt)
        {
            if (m_isAnimated)
            {
                m_trigger += (float)gt.ElapsedGameTime.TotalSeconds * m_fps;
            }
            base.updateMe(gt);
        }
        public override void Draw(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_pos, m_srcRect, m_tint, m_rotation, m_rotationPoint, 1, m_sEffect, 0);
        }

        protected void playCycle()
        {
            if (m_trigger >= 1)
            {
                m_trigger = 0;
                m_srcRect.X += m_srcRect.Width;
                if (m_srcRect.X >= m_txr.Width)
                    m_srcRect.X = m_srcRect.Width * 2;
            }
        }
        protected void rewindCycle()
        {
            if (m_trigger >= 1)
            {
                m_trigger = 0;
                m_srcRect.X -= m_srcRect.Width;
                if (m_srcRect.X <= m_srcRect.Width * 2)
                    m_srcRect.X = m_srcRect.Width * 9;
            }
        }
    }
}

