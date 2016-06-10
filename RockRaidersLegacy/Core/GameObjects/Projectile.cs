using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.GameObjects
{
    class Projectile : GameObject
    {
        protected Actor m_user;
        protected List<Asteroid> m_asteroidList;
        protected List<BoundingBox> m_tileList;
        private float m_rot;
        private int m_bounceLimit, m_bounced;
        private bool m_primed;
        private MatchType m_currMatch;
        private Weapon m_weapon;
        protected int m_Damage;

        public EntityState State
        {
            get
            {
                return m_state;
            }
        }

        public Projectile(Texture2D Texture, Vector2 Position, Vector2 initialVelocity, float rotation, World currLevel, Actor user, Weapon weapon)
            :base (Texture,Position,initialVelocity,0.1f)
        {
            m_boundingSphere = new BoundingSphere(new Vector3((float)m_pos.X + (m_txr.Width / 2), (float)m_pos.Y + (m_txr.Height / 2), 0), (float)m_txr.Width / 2);
            m_asteroidList = currLevel.Asteroids;
            m_tileList = currLevel.Map.CollisionList;
            m_rot = rotation;
            m_velocity = initialVelocity;
            m_state = EntityState.Live;
            m_tint = Color.White;
            m_inertia = 1;
            m_user = user;
            m_currMatch = currLevel.CurrentMatch;
            m_weapon = weapon;
            m_Damage = getWeaponDamage(m_weapon);

            if (this.GetType() == typeof (PlasmaBolt))
                m_bounceLimit = 4;

            m_bounced = 0;
        }

        private int getWeaponDamage(Weapon wpn)
        {
            Type wpnType = wpn.GetType();

            if (wpnType == typeof (MachineGun))
                return 5;
            if (wpnType == typeof(Pistol))
                return 7;
            if (wpnType == typeof(LaserPistol))
                return 9;
            if (wpnType == typeof(LaserRifle))
                return 12;
            if (wpnType == typeof(PlasmaRifle))
                return 20;

            return 0;
        }

        private void doNormalCollision()
        {
            foreach (Asteroid rock in m_asteroidList)
            {
                if (m_boundingSphere.Intersects(rock.Sphere))
                {
                    m_state = EntityState.Dead;
                    break;
                }
            }
            foreach (BoundingBox Tile in m_tileList)
            {
                if (m_boundingSphere.Intersects(Tile))
                {
                    m_state = EntityState.Dead;
                    break;
                }
            }
        }

        private void doBouncingCollision()
        {
            foreach (Asteroid rock in m_asteroidList)
            {
                if (m_boundingSphere.Intersects(rock.Sphere))
                {
                    m_bounced++;
                    m_velocity.Y -= m_velocity.Y * 2;
                    break;
                }
            }
            foreach (BoundingBox Tile in m_tileList)
            {
                if (m_boundingSphere.Intersects(Tile))
                {
                    m_bounced++;
                    m_velocity.Y -= m_velocity.Y * 2;
                    break;
                }
            }

            if (m_bounced >= m_bounceLimit)
                m_state = EntityState.Dead;
        }

        public virtual void updateMe(GameTime gt, Rectangle gameBounds, SessionManager SessionManager)
        {
            if (gameBounds.Contains(new Point((int)m_boundingSphere.Center.X, (int)m_boundingSphere.Center.Y)))
            {
                if ((this.GetType() == typeof(PlasmaBolt)) && ((m_velocity.Y > 1) || (m_velocity.Y < -1)))
                        doBouncingCollision();
                else
                    doNormalCollision();

                foreach (NetworkGamer player in SessionManager.NetSession.AllGamers)
                {
                    SpaceMarine playerCharacter = player.Tag as SpaceMarine;
                    doCollisionCheck(playerCharacter);
                }

                base.updateMe(gt);
            }
            else
            {
                m_state = EntityState.Dead;
            }
        }

        public override void Draw(SpriteBatch sBatch)
        {
            base.Draw(sBatch);
            sBatch.Draw(m_txr, m_pos, null, m_tint, m_rot, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        private void doCollisionCheck(SpaceMarine Player)
        {
            if (m_boundingSphere.Intersects(Player.Sphere))
            {
                if (Player != m_user)
                {
                    Vector2 vel = Player.Velocity;
                    CalculationFunctions.cResponse(m_pos, Player.Position, ref m_velocity, ref vel, m_mass, Player.Mass);
                    Player.Velocity = vel;
                    Player.Health -= m_Damage;
                    Player.DamageTicker = 15;
                    m_state = EntityState.Dead;

                    if ((Player.Health <= 0) && Player.State != ActorState.Dying)
                    {
                        if (m_currMatch != MatchType.DM)
                        {
                            if (Player.Team != m_user.Team)
                                ((SpaceMarine)m_user).Score++;
                            else
                                ((SpaceMarine)m_user).Score--;
                        }
                        else
                        {
                            ((SpaceMarine)m_user).Score++;
                        }

                        Player.Killer = (string)((SpaceMarine)m_user).PlayerName;
                    }
                }
            }
        }
    }



    class BalisticBullet : Projectile
    {
        Vector2 vel;
        public BalisticBullet(Texture2D Texture, Vector2 Position, Vector2 initialVelocity, float Rotation, World currentLevel, Actor user, Weapon weapon)
            : base(Texture, Position, initialVelocity, Rotation, currentLevel, user, weapon)
        {
            m_mass = 0.1f;
            vel = user.Velocity;
            CalculationFunctions.cRecoil(m_velocity, ref vel, user.Mass, m_mass);
            user.Velocity = vel;
            m_tint = Color.LightGoldenrodYellow;
        }
        public override void updateMe(GameTime gt, Rectangle gameBounds, SessionManager sessionManager)
        {
            base.updateMe(gt, gameBounds, sessionManager);
        }
    }

    class PlasmaBolt : Projectile
    {
        public PlasmaBolt(Texture2D Texture, Vector2 Position, Vector2 initialVelocity, float Rotation, World currentLevel, Actor user, Weapon weapon)
            : base(Texture, Position, initialVelocity, Rotation, currentLevel, user,weapon)
        {
        }
        public override void updateMe(GameTime gt, Rectangle gameBounds, SessionManager sessionManager)
        {
            base.updateMe(gt, gameBounds, sessionManager);
        }
    }

    class Lazer : Projectile
    {
        public Lazer(Texture2D Texture, Vector2 Position, Vector2 initialVelocity, float Rotation, World currentLevel, Actor user, Weapon weapon)
            : base(Texture, Position, initialVelocity, Rotation, currentLevel, user,weapon)
        {
            m_mass = 0;
        }
    }
}
