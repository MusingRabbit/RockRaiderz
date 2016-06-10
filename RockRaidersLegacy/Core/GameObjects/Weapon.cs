using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.GameObjects
{

    class Weapon : PickupItem
    {
        protected int m_itemAmmo, m_shootTimer;
        protected string m_name;
        private Texture2D m_txrAmmo;
        private Sprite m_mussleFlash;
        private Vector2 m_projectileDir, m_projectileDisplacement;
        protected Vector2 m_barrelEnd;
        private float m_projectileRot;
        private bool m_weaponDropped;
        
        protected int m_projectileSpeed;
        private int m_delay;
        private int m_spawnTimer;

        public Texture2D AmmoTexture
        {
            get
            {
                return m_txrAmmo;
            }
        }
        public int AmmoCount
        {
            get
            {
                return m_itemAmmo;
            }
            set
            {
                m_itemAmmo = value;
            }
        }
        public int ShootTimer
        {
            get
            {
                return m_shootTimer;
            }
            set
            {
                m_shootTimer = value;
            }
        }
        public int ProjectileSpeed
        {
            get
            {
                return m_projectileSpeed;
            }
        }
        public string Name
        {
            get
            {
                return m_name;
            }
        }
        public Vector2 Offset
        {
            get
            {
                return m_itemOffset;
            }
        }

        


        public Weapon(Texture2D WeaponTexture, Texture2D AmmoTexture, Texture2D TextureFlash, Vector2 Position, Rectangle srcRect, float Mass, string weaponName)
            :base(WeaponTexture,Position,srcRect,Mass, 0)
        {
            m_txrAmmo = AmmoTexture;
            m_txr = WeaponTexture;
            m_pos = Position;
            m_srcRect = srcRect;
            m_mass = Mass;
            m_name = weaponName;
            m_spawnTimer = 2000;
            m_weaponDropped = false;
        }

        public override void Update(GameTime gt, World gameWorld)
        {
            //m_mussleFlash.RotationPoint = m_mussleFlash.Position - (m_pos + m_barrelEnd);
            //m_mussleFlash.animationEnabled = true;
            //m_mussleFlash.updateMe(gt);

            if (m_itemDropped)
                m_weaponDropped = true;

            if (m_weaponDropped)
                m_spawnTimer--;

            if (m_spawnTimer <= 0)
                m_state = EntityState.Dead;

            base.Update(gt, gameWorld);
        }

        public override void UpdatePos(Actor user)
        {
            base.UpdatePos(user);
        }

        public void Shoot(Actor user, CrossHair crossHair, World currLevel)
        {
            if ((user.State == ActorState.Floating) || (user.State == ActorState.Bound))
            {
                m_delay--;
                if (m_delay <= 0)
                {
                    m_projectileDir = new Vector2(m_pos.X - crossHair.TargetPos.X, m_pos.Y - crossHair.TargetPos.Y);
                    m_projectileDisplacement = -Vector2.Normalize(m_projectileDir) * m_projectileSpeed + user.Velocity;
                    m_projectileRot = (float)Math.Atan2(m_projectileDisplacement.Y, m_projectileDisplacement.X);

                    if (m_itemAmmo > 0)
                    {
                        if (m_name.Contains("Plasma"))
                            user.ProjectileList.Add(new PlasmaBolt(m_txrAmmo, m_pos, m_projectileDisplacement, m_rotation, currLevel, user,this));
                        else if (m_name.Contains("MachineGun") || m_name == ("Pistol"))
                            user.ProjectileList.Add(new BalisticBullet(m_txrAmmo, m_pos, m_projectileDisplacement, m_rotation, currLevel, user,this));
                        else if (m_name.Contains("Laser"))
                            user.ProjectileList.Add(new Lazer(m_txrAmmo, m_pos, m_projectileDisplacement, m_rotation, currLevel, user,this));

                        m_itemAmmo -= 1;
                    }

                    m_delay = m_shootTimer;
                }
            }
        }

        public override void Draw(SpriteBatch sBatch)
        {
            base.Draw(sBatch);
            //m_mussleFlash.drawMe(sBatch);
        }
    }

    class MachineGun : Weapon
    {
        public MachineGun(AssetManager AssetManager, int Xpos, int Ypos)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Bullet"], AssetManager.SpriteLib["MuzzleFlash"], new Vector2(Xpos, Ypos),
             new Rectangle(0, 300, 38, 15), 0.3f, "MachineGun")
        {
            m_itemAmmo = 300;
            m_shootTimer = 5;
            m_projectileSpeed = 10;
            m_itemOffset = new Vector2(-15, -5);
            m_barrelEnd = new Vector2(5, 0);
            m_itemID = 11;
        }

        public MachineGun(AssetManager AssetManager)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Bullet"], AssetManager.SpriteLib["MuzzleFlash"], Vector2.Zero,
             new Rectangle(0, 300, 38, 15), 0.3f, "MachineGun")
        {
            m_itemAmmo = 300;
            m_shootTimer = 5;
            m_projectileSpeed = 10;
            m_itemOffset = new Vector2(-15, -5);
            m_barrelEnd = new Vector2(5, 0);
            m_itemID = 11;
        }
    }

    class Pistol : Weapon
    {
        public Pistol(AssetManager AssetManager, int Xpos, int Ypos)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Bullet"], AssetManager.SpriteLib["MuzzleFlash"], new Vector2(Xpos, Ypos),
             new Rectangle(100, 300, 24, 15), 0.3f, "Pistol")
        {
            m_itemAmmo = 120;
            m_shootTimer = 20;
            m_projectileSpeed = 12;
            m_itemOffset = new Vector2(3, -8);
            m_itemID = 12;
        }

        public Pistol(AssetManager AssetManager)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Bullet"], AssetManager.SpriteLib["MuzzleFlash"], new Vector2(-500,-500),
             new Rectangle(100, 300, 24, 15), 0.3f, "Pistol")
        {
            m_itemAmmo = 120;
            m_shootTimer = 20;
            m_projectileSpeed = 12;
            m_itemOffset = new Vector2(3, -8);
            m_itemID = 12;
        }
    }

    class LaserPistol : Weapon
    {
        public LaserPistol(AssetManager AssetManager, int Xpos, int Ypos)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Laser"], AssetManager.SpriteLib["MuzzleFlash"], 
            new Vector2(Xpos,Ypos), new Rectangle(300,300,28,15), 0.5f, "LaserPistol")
        {
            m_itemAmmo = 250;
            m_shootTimer = 30;
            m_projectileSpeed = 30;
            m_itemOffset = new Vector2(3, -8);
            m_itemID = 13;
        }


        public LaserPistol(AssetManager AssetManager)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Laser"], AssetManager.SpriteLib["MuzzleFlash"],
            new Vector2(-500, -500), new Rectangle(300, 300, 28, 15), 0.5f, "LaserPistol")
        {
            m_itemAmmo = 250;
            m_shootTimer = 30;
            m_projectileSpeed = 30;
            m_itemOffset = new Vector2(3, -8);
            m_itemID = 13;
        }
    }

    class LaserRifle : Weapon
    {
        public LaserRifle(AssetManager AssetManager, int Xpos, int Ypos)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Laser"], AssetManager.SpriteLib["MuzzleFlash"], 
            new Vector2(Xpos,Ypos), new Rectangle(50,300,38,15), 0.5f, "LaserRifle")
        {
            m_itemAmmo = 32;
            m_shootTimer = 50;
            m_projectileSpeed = 30;
            m_itemOffset = new Vector2(-15, -5);
            m_itemID = 14;
        }
    

        public LaserRifle(AssetManager AssetManager)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Laser"], AssetManager.SpriteLib["MuzzleFlash"],
            new Vector2(-500, -500), new Rectangle(50, 300, 38, 15), 0.5f, "LaserRifle")
        {
            m_itemAmmo = 50;
            m_shootTimer = 50;
            m_projectileSpeed = 30;
            m_itemOffset = new Vector2(-15, -5);
            m_itemID = 14;
        }
    }

    class RocketLauncher : Weapon
    {
        public RocketLauncher(AssetManager AssetManager, int Xpos, int Ypos)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Bullet"], AssetManager.SpriteLib["MuzzleFlash"],
            new Vector2(Xpos,Ypos), new Rectangle(250, 300, 50, 25), 0.9f, "RocketLancher")
        {
            m_itemAmmo = 5;
            m_shootTimer = 100;
            m_projectileSpeed = 5;
            m_itemID = 15;
        }
        public RocketLauncher(AssetManager AssetManager)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Bullet"], AssetManager.SpriteLib["MuzzleFlash"],
            new Vector2(-500, -500), new Rectangle(250, 300, 50, 25), 0.9f, "RocketLancher")
        {
            m_itemAmmo = 5;
            m_shootTimer = 100;
            m_projectileSpeed = 5;
            m_itemID = 15;
        }
    }

    class PlasmaRifle : Weapon
    {
        public PlasmaRifle(AssetManager AssetManager, int Xpos, int Ypos)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Plasma"], AssetManager.SpriteLib["MuzzleFlash"],
             new Vector2(Xpos, Ypos), new Rectangle(250, 300, 37, 17), 0.9f, "PlasmaRifle")
        {
            m_itemAmmo = 100;
            m_shootTimer = 25;
            m_projectileSpeed = 5;
            m_itemOffset = new Vector2(-15, -5);
            m_itemID = 16;
        }

        public PlasmaRifle(AssetManager AssetManager)
            : base(AssetManager.SpriteLib["MarineSpriteSheet"], AssetManager.SpriteLib["Plasma"], AssetManager.SpriteLib["MuzzleFlash"],
             new Vector2(-500, -500), new Rectangle(250, 300, 37, 17), 0.9f, "PlasmaRifle")
        {
            m_itemAmmo = 100;
            m_shootTimer = 25;
            m_projectileSpeed = 5;
            m_itemOffset = new Vector2(-15, -5);
            m_itemID = 16;
        }
    }
}