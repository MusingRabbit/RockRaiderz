using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RockRaiders.Core.GameObjects
{
    class PickupItem : GameObject
    {
        protected int m_dropTimer;
        protected int m_team;
        protected int m_itemID;
        protected bool m_itemDropped;
        protected int m_spawnID;
        protected SpawnPoint m_spawnPoint;

        public Actor User
        {
            get
            {
                return m_currUser;
            }
            set
            {
                m_currUser = value;
            }
        }
        public int Team
        {
            get
            {
                return m_team;
            }
            set
            {
                m_team = value;
            }
        }
        public bool Dropped
        {
            get
            {
                return m_itemDropped;
            }
            set
            {
                m_itemDropped = value;
            }
        }
        public int ItemID
        {
            get
            {
                return m_itemID;
            }
        }
        public EntityState State
        {
            get
            {
                return m_state;
            }
        }
        public int SpawnID
        {
            get
            {
                return m_spawnID;
            }
            set
            {
                m_spawnID = value;
            }
        }
        public SpawnPoint SpawnPont
        {
            get
            {
                return m_spawnPoint;
            }
            set
            {
                m_spawnPoint = value;
            }
        }

        protected Actor m_currUser;
        protected Vector2 m_itemOffset;

        public PickupItem(Texture2D SpriteSheet, Vector2 Position, Rectangle srcRect, float Mass, int Team)
            : base(SpriteSheet, Position, srcRect, Vector2.Zero, Mass)
        {
            m_currUser = null;
            m_dropTimer = 40;
            m_itemDropped = false;
            m_itemOffset = Vector2.Zero;
            m_team = Team;
            setTeamValue();
            m_state = EntityState.Live;
        }

        private void setTeamValue()
        {
            if (m_team == 1)
            {
                m_tint = Color.CadetBlue;
            }
            if (m_team == 2)
            {
                m_tint = Color.Red;
            }
        }


        public virtual void Update(GameTime gt, World gameWorld)
        {
            if (m_itemDropped)
            {
                m_dropTimer--;
            }
            if (m_dropTimer < 0)
            {
                m_itemDropped = false;
                m_dropTimer = 40;
            }
            foreach (BoundingBox box in gameWorld.Map.CollisionList)
            {
                if (m_boundingSphere.Intersects(box))
                {
                    m_velocity -= m_velocity * 2;
                }
            }
            foreach (Asteroid rock in gameWorld.Asteroids)
            {
                if (m_boundingSphere.Intersects(rock.Sphere))
                {
                    m_velocity -= m_velocity * 2;
                }
            }
            base.updateMe(gt);
        }

        public virtual void UpdatePos(Actor user)
        {
            m_sEffect = SpriteEffects.None;

            if (user.State == ActorState.Floating)
            {
                m_rotation = user.Rotation;
                m_sEffect = user.spriteEffect;
            }
            if (user.State == ActorState.Bound)
            {
                if (user.spriteEffect == SpriteEffects.FlipHorizontally)
                    m_sEffect = SpriteEffects.FlipVertically;

                m_rotation = user.ArmRotation;
            }

            m_pos = user.Position;
            m_rotationPoint = m_pos - (user.Position + m_itemOffset);
        }

        public override void Draw(SpriteBatch sBatch)
        {
            base.Draw(sBatch);
        }
    }
}
