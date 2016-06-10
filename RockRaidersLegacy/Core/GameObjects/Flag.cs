using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.GameObjects
{
    class Flag : PickupItem
    {
        private bool m_atHome;
        private FlagBase m_flagBase;

        public bool atHome
        {
            get
            {
                return m_atHome;
            }
            set
            {
                m_atHome = value; 
            }
        }
        public FlagBase Base
        {
            get
            {
                return m_flagBase;
            }
            set
            {
                m_flagBase = value;
            }
        }

        public Flag(Texture2D SpriteSheet, Vector2 Position, Rectangle srcRect, int Team)
            : base(SpriteSheet, Position, srcRect, 0.5f, Team)
        {
            m_itemOffset = new Vector2(-35, -35);
            m_team = Team;
            m_itemID = 1;
        }
        public Flag(AssetManager assetManager)
            : base(assetManager.SpriteLib["MarineSpriteSheet"], Vector2.Zero, new Rectangle(250, 350, 25, 35), 0.5f, 0)
        {
            m_itemID = 1;
        }

        public Flag(Flag flag)
            : base (flag.Texture,flag.Position,flag.SourceRect, flag.Mass, flag.Team)
        {
            m_dropTimer = 40;
            m_UID = flag.ID;
            m_itemID = flag.ItemID;
            m_velocity = flag.Velocity;
            m_itemOffset = new Vector2(-35, -35);
        }

        public override void Update(GameTime gt, World gameWorld)
        {
            // Flagbase null exception fix - 14/05/2014
            if (m_flagBase == null)
            {
                if (m_team == 1)
                    m_flagBase = gameWorld.BlueBase;
                if (m_team == 2)
                    m_flagBase = gameWorld.RedBase;
            }
            base.Update(gt, gameWorld);
        }

        public override void Draw(SpriteBatch sBatch)
        {
            if (m_team == 1)
                m_tint = Color.CadetBlue;
            if (m_team == 2)
                m_tint = Color.Red;

            base.Draw(sBatch);
        }

        public override void UpdatePos(Actor user)
        {
            m_sEffect = SpriteEffects.None;

            m_itemOffset.Y = -35;

            if (user.State == ActorState.Floating)
            {
                if (user.spriteEffect == SpriteEffects.FlipVertically)
                {
                    m_itemOffset.Y = +4;
                    m_itemOffset.X = -35;
                }
                m_sEffect = user.spriteEffect;
            }
            if (user.State == ActorState.Bound)
            {
                if (user.spriteEffect == SpriteEffects.FlipHorizontally)
                {
                    m_sEffect = SpriteEffects.FlipHorizontally;
                    m_itemOffset.X = +12;
                }
                else
                {
                    m_itemOffset.X = -35;
                }
            }
            m_rotation = user.Rotation;
            m_pos = user.Position;
            m_rotationPoint = m_pos - (user.Position + m_itemOffset);
        }

        public void CheckCapState(FlagBase flagBase)
        {
            if (m_team != flagBase.Team)
            {
                if (m_boundingSphere.Intersects(flagBase.Sphere))
                {
                    if (m_state != EntityState.Dead)
                    {
                        flagBase.MadeGoal = true;
                        m_state = EntityState.Dead;
                    }
                }
            }
        }

    }
}
