using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.GameObjects
{
    public class GameObject : Sprite
    {

        protected BoundingSphere m_boundingSphere;
        protected Guid m_UID;
        protected float m_mass;
        protected EntityState m_state;
        protected object m_tag;

        public float Mass
        {
            get
            {
                return m_mass;
            }
            set
            {
                m_mass = value;
            }
        }
        public Guid ID
        {
            get
            {
                return m_UID;
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
                return new Vector2(m_boundingSphere.Center.X, m_boundingSphere.Center.Y);
            }
        }
        public Vector2 Velocity
        {
            get
            {
                return m_velocity;
            }
            set
            {
                m_velocity = value;
            }
        }
        public object Tag
        {
            get
            {
                return m_tag;
            }
            set
            {
                m_tag = value;
            }
        }
        

        public GameObject(Texture2D SpriteSheet, Vector2 Position, Rectangle srcRect, Vector2 initialVelocity, float Mass)
            :base(SpriteSheet,Position,srcRect)
        {
            m_boundingSphere = new BoundingSphere (new Vector3(m_pos,0),m_srcRect.Width/2);
            m_mass = Mass;
            m_UID = Guid.NewGuid();
            m_inertia = 0.98f;
            m_state = EntityState.Live;
        }

        public GameObject(Texture2D Texture, Vector2 Position, Vector2 initialVelocity, float Mass)
            : base(Texture, Position)
        {
            m_boundingSphere.Center.X = m_pos.X;
            m_boundingSphere.Center.Y = m_pos.Y;
            m_mass = Mass;
            m_UID = Guid.NewGuid();
            m_inertia = 0.98f;
            m_state = EntityState.Live;
        }

        public override void updateMe(GameTime gt)
        {
            base.updateMe(gt);

            m_boundingSphere.Center = new Vector3(m_pos, 0);
        }

        public override void Draw(SpriteBatch sBatch)
        {
            base.Draw(sBatch);
        }


    }
}
