using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.Primatives
{
    class MotionGraphic : StaticGraphic
    {
        protected Vector2 m_velocity;
        protected float m_inertia;

        public MotionGraphic(Texture2D Texture, Vector2 Position, Vector2 initialVelocity)
            :base(Texture, Position)
        {
            m_velocity = initialVelocity;
            m_inertia = 1;
        }
        public virtual void updateMe(GameTime gt)
        {
            m_pos += m_velocity;
            m_velocity *= m_inertia;
        }
        public override void Draw(SpriteBatch sBatch)
        {
            base.Draw(sBatch);
        }
    }
}
