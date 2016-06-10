using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

enum MotionState
{
    WalkingRight,
    WalkingLeft
}

namespace RockRaiders.Core.GameObjects
{
    class Actor : GameObject
    {
        #region Variable Creation
        private float m_thrust;
        private bool m_statePrompt;
        private bool m_walkingVer, m_walkingHoz;
        private int m_promptTimer;
        private Vector2 m_direction;
        private Vector2 deltaPosAim, deltaPosBound;
        private MotionState motionState;
        private List<Asteroid> m_Asteroids;
        private List<Weapon> m_inventory;
        private List<Projectile> m_projectileList;
        private List<PickupItem> m_itemList;
        private TileMap m_map;
        private Weapon m_selectedWeapon;
        private World m_currentLevel;
        private object m_lastHit;
        private float m_armRotation;
        private bool isFacingRight , isMovingRight;
        private bool m_initDone;
        private bool m_isFiring;
        private bool m_hit;
        private int m_selectedIndex;
        private int ticker;
        private int deadTimer;
        private bool m_canSpawn;
        private bool m_invert;
        private Flag m_flagSlot;
        private Rectangle m_Rect, m_srcArmRect, m_srcArmRect2, m_srcArmRect3;
        private PolyInputController m_controller;
        private string m_lastKiller;


        protected bool m_isRemote;
        protected Sprite m_playerArm;
        protected ActorState playerState;
        protected int m_team;

        SpriteFont m_debugFont;

        private CrossHair m_crossHair;
        #endregion

        #region Class Properties
        public ActorState State
        {
            get
            {
                return playerState;
            }
            set
            {
                playerState = value;
            }
        }
        public PolyInputController ActiveControl
        {
            get
            {
                return m_controller;
            }
        }
        public CrossHair CrossHair
        {
            get
            {
                return m_crossHair;
            }
            set
            {
                m_crossHair = value;
            }
        }
        public bool isPromptingUser
        {
            get
            {
                return m_statePrompt;
            }
        }
        public bool isFiring
        {
            get
            {
                return m_isFiring;
            }
            set
            {
                m_isFiring = value;
            }
        }
        public Vector2 AimPosition
        {
            get
            {
                return m_crossHair.TargetPos;
            }
        }
        public float ArmRotation
        {
            get
            {
                return m_armRotation;
            }
            set
            {
                m_armRotation = value;
            }
        }
        public Weapon CurrentWeapon
        {
            get
            {
                return m_selectedWeapon;
            }
            set
            {
                m_selectedWeapon = value;
            }
        }
        public Flag FlagSlot
        {
            get
            {
                return m_flagSlot;
            }
            set
            {
                m_flagSlot = value;
            }
        }
        public bool isRemote
        {
            get
            {
                return m_isRemote;
            }
            set
            {
                m_isRemote = value;
            }
        }
        public int DamageTicker
        {
            get
            {
                return ticker;
            }
            set
            {
                ticker = value;
            }
        }
        public bool isHit
        {
            get
            {
                return m_hit;
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
        public string Killer
        {
            get
            {
                return m_lastKiller;
            }
            set
            {
                m_lastKiller = value;
            }
        }
        public bool hasFlag
        {
            get
            {
                return (m_flagSlot != null);
            }
        }

        public bool FacingRight
        {
            get
            {
                return isFacingRight;
            }
            set
            {
                isFacingRight = value;
            }
        }
        public bool MovingRight
        {
            get
            {
                return isMovingRight;
            }
        }
        public bool CanSpawn
        {
            get
            {
                return m_canSpawn;
            }
        }
        public List<Projectile> ProjectileList
        {
            get
            {
                return m_projectileList;
            }
        }
        public Rectangle Rectangle
        {
            get
            {
                return m_Rect;
            }
        }
        public List<Weapon> WeaponInventory
        {
            get
            {
                return m_inventory;
            }
        }
        #endregion

        #region Class Constructor
        public Actor(Texture2D Texture, Vector2 Position, Vector2 InitialVelocity, SpriteFont debugFont)
            : base(Texture, Position, new Rectangle(0,0,50,50), InitialVelocity, 1f)
        {
            playerState = ActorState.Dead;
            m_thrust = 0f;
            isFacingRight = true;
            isMovingRight = true;
            m_direction = Vector2.Zero;
            m_srcArmRect = new Rectangle(0, 250, 30, 17);
            m_srcArmRect2 = new Rectangle(50, 250, 27, 14);
            m_srcArmRect3 = new Rectangle(100, 250, 31, 210);
            m_playerArm = new Sprite(Texture, new Vector2(m_pos.X + 15, m_pos.Y + 10), m_srcArmRect);
            m_statePrompt = false;
            m_promptTimer = 0;
            m_initDone = false;
            m_inventory = new List<Weapon>();
            m_projectileList = new List<Projectile>();
            m_Rect = new Rectangle((int)m_pos.X, (int)m_pos.Y, 50, 50);
            m_debugFont = debugFont;
            m_isRemote = false;
            m_hit = false;
            m_invert = false;
            ticker = 0;
            deadTimer = 0;
        }
        #endregion

        public virtual void Update(GameTime gt, PolyInputController Controller, World CurrentLevel, CrossHair Crosshair)
        {
            m_controller = Controller;
            m_Asteroids = CurrentLevel.Asteroids;
            m_crossHair = Crosshair;
            m_currentLevel = CurrentLevel;
            m_map = m_currentLevel.Map;

            updatePlayerState(CurrentLevel);
            
            managePrompt();
            
            if (m_selectedWeapon != null)
            {
                m_selectedWeapon.Update(gt, CurrentLevel);
            }

            if (m_flagSlot != null)
            {
                m_flagSlot.Update(gt, CurrentLevel);

                m_flagSlot.CheckCapState(CurrentLevel.RedBase);
                m_flagSlot.CheckCapState(CurrentLevel.BlueBase);

                if (m_flagSlot.State == EntityState.Dead)
                    dropFlag();
            }
            
            base.updateMe(gt);

            m_itemList = CurrentLevel.PickupItems;

            m_playerArm.Position = m_pos;

            foreach (PickupItem item in m_inventory)
            {
                item.Position = m_pos;
            }

            if (m_selectedWeapon != null)
            {
                m_selectedWeapon.UpdatePos(this);

                if (m_selectedWeapon.Name.Contains("Pistol"))
                    m_playerArm.SourceRect = m_srcArmRect2;
                else if (m_selectedWeapon.Name == "RocketLauncher")
                    m_playerArm.SourceRect = m_srcArmRect3;
                else
                    m_playerArm.SourceRect = m_srcArmRect;

            }
            if (m_flagSlot != null)
            {
                m_flagSlot.UpdatePos(this);
            }

            updatePlayerControl(Controller, CurrentLevel);

            for (int i = 0; i < m_inventory.Count; i++)
            {
                if (i == m_selectedIndex)
                {
                    m_selectedWeapon = m_inventory[i];
                }
            }

            if (isFiring)
            {
                fireWeapon(m_crossHair, CurrentLevel);
            }
            m_Rect.X = (int)m_pos.X;
            m_Rect.Y = (int)m_pos.Y;
        }

        public override void Draw(SpriteBatch sBatch)
        {
            foreach (Projectile projectile in m_projectileList)
            {
                projectile.Draw(sBatch);
            }
            if (m_flagSlot != null)
            {
                m_flagSlot.Draw(sBatch);
            }

            base.Draw(sBatch);

            if (m_selectedWeapon != null)
                m_selectedWeapon.Draw(sBatch);

            m_playerArm.Tint = m_tint;
            m_playerArm.Draw(sBatch);


#if DEBUG
            sBatch.DrawString(m_debugFont, this + " at " + m_pos + "rot :" + m_rotation
                + "\nVelocity : " + m_velocity.ToString()
                +"\n Moving Right :" + isMovingRight
                +"\n Facing Right : " + isFacingRight
                +"\nSpherePos Min :" + m_boundingSphere.Center.ToString()
                , m_pos, Color.White);

#endif 

        }

        public void UpdateProjectiles(GameTime gt, SessionManager SessionManager)
        {
            for (int i = 0; i < m_projectileList.Count; i++)
            {
                if (m_projectileList[i].State == EntityState.Dead)
                {
                    m_projectileList.RemoveAt(i);
                }
                else
                {
                    m_projectileList[i].updateMe(gt, m_currentLevel.WorldBoundary,SessionManager);
                }
            }
        }

        private void updatePlayerState(World CurrentLevel)
        {
            switch (playerState)
            {
                case ActorState.Floating:
                    m_initDone = false;
                    doFloatingUpdate(m_crossHair);
                    orientActorFloating();
                    doItemCollisionCheck(CurrentLevel.PickupItems);
                    break;

                case ActorState.Bound:
                    doBoundUpdate(m_crossHair);
                    m_statePrompt = false;
                    doItemCollisionCheck(CurrentLevel.PickupItems);
                    break;
                case ActorState.Dying:
                    m_srcRect.X = 51;
                    m_srcRect.Y = 107;
                    doDyingUpdate(m_controller);
                    break;
                case ActorState.Dead:
                    break;
            }
        }

        private void doDyingUpdate(PolyInputController Controller)
        {
            dropAllItems(m_itemList);

            ticker = 0;
            deadTimer ++;

            if (deadTimer > 150)
            {
                m_canSpawn = true;
                if (Controller.SpacePressed)
                    playerState = ActorState.Dead;

            }
            else
            {
                m_canSpawn = false;
            }
        }

        private void updatePlayerControl(PolyInputController Controller, World CurrentLevel)
        {
            if (m_crossHair.isFiring)
                m_isFiring = true;
            else
                m_isFiring = false;

            if (Controller.DropWeapon)
                dropSelectedWeapon(CurrentLevel.PickupItems);

            if (Controller.DropFlag)
                dropFlag();

            if (Controller.OnePressed)
                m_selectedIndex = 0;

            if (Controller.TwoPressed)
                m_selectedIndex = 1;

        }

        private void fireWeapon(CrossHair crossHair, World CurrentLevel)
        {
            if (m_selectedWeapon != null)
            {
                if (playerState == ActorState.Floating)
                {
                    if (isMovingRight == isFacingRight)
                    {
                        m_selectedWeapon.Shoot(this, crossHair, CurrentLevel);
                    }
                }
                else
                {
                    m_selectedWeapon.Shoot(this, crossHair, CurrentLevel);
                }

            }
        }

        private void doFloatingMovement()
        {
            m_thrust = 0f;

            if (m_controller.UpPressed)
            {
                m_direction.Y = -1;
            }
            if (m_controller.DownPressed)
            {
                m_direction.Y = 1;
            }
            if (m_controller.LeftPressed)
            {
                if ((m_rotation > 1.6f) && (m_rotation < 4f))
                {
                    isFacingRight = false;
                }
                m_direction.X = -1;
            }
            if (m_controller.RightPressed)
            {
                if ((m_rotation > -1.6f) && (m_rotation < 1.0f))
                {
                    isFacingRight = true;
                }
                m_direction.X = 1;
            }

            if (m_velocity.X > 0)
            {
                if ((m_rotation > -1.6f) && (m_rotation < 1f))
                {
                    isMovingRight = true;
                }
            }
            if ((m_velocity.X < 0) && (m_rotation < 4f))
            {
                if (m_rotation > 1.6f)
                {
                    isMovingRight = false;
                }
            }
            m_thrust += 0.1f;
            m_velocity += m_direction * m_thrust;
        }

        private void doFloatingUpdate(CrossHair crossHair)
        {
            m_isAnimated = false;
            m_srcRect.X = 0;
            m_srcRect.Y = 100;
            m_thrust = 0f;
            m_direction = Vector2.Zero;
            deltaPosAim.X = crossHair.TargetPos.X - (m_pos.X);
            deltaPosAim.Y = crossHair.TargetPos.Y - (m_pos.Y);

            if (playerState == ActorState.Floating)
            m_rotation = -(float)(Math.Atan2(deltaPosAim.X, deltaPosAim.Y) - (Math.PI * 0.5f));

            doFloatingMovement();

            m_playerArm.Rotation = m_rotation;

            if (m_statePrompt)
            {
                if (m_controller.UsePressed)
                 {
                    playerState = ActorState.Bound;
                }
            }

            doAsteroidCollisionCheck();
            doTileCollisionCheck();
            doDamageFlash();
        }

        private void doBoundUpdate(CrossHair crossHair)
         {
            updateAim(crossHair);

            m_direction.X = (float)Math.Cos(m_rotation);
            m_direction.Y = (float)Math.Sin(m_rotation);
            try
            {
                Type objType = m_lastHit.GetType();

                initAnimation();

                if (objType == typeof(Asteroid))
                {
                    oerientActorBound((Asteroid)m_lastHit);
                }
                if (objType == typeof(BoundingBox))
                {
                    orientActorBound((BoundingBox)m_lastHit);
                }

                doWalkMovement(crossHair);

                if (m_controller.SpacePressed)
                {
                    playerState = ActorState.Floating;
                    m_velocity = -Vector2.Normalize(deltaPosBound) * 3;
                    return;
                }
                doTileCollisionCheck();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception Caught : " + ex.Message + "\n" + ex.StackTrace);
            }
            doDamageFlash();
        }

        private void doWalkMovement(CrossHair crossHair)
        {
            switch (motionState)
            {
                case MotionState.WalkingLeft:
                    doBoundActorMotionLeft(crossHair);
                    break;

                case MotionState.WalkingRight:
                    doBoundActorMotionRight(crossHair);
                    break;
            }
            if (m_lastHit.GetType() == typeof(Asteroid))
            {
                if (m_controller.RightPressed)
                {
                    motionState = MotionState.WalkingRight;
                    m_thrust = 2;
                }
                else if (m_controller.LeftPressed)
                {
                    motionState = MotionState.WalkingLeft;
                    m_thrust = -2;
                }
                else
                {
                    m_srcRect.X = 0;
                    m_thrust = 0;
                }
            }
            else
            {
                if (!m_invert)
                {
                    if (m_controller.RightPressed)
                    {
                        motionState = MotionState.WalkingRight;
                        m_thrust = -2;
                    }
                    else if (m_controller.LeftPressed)
                    {
                        motionState = MotionState.WalkingLeft;
                        m_thrust = 2;
                    }
                    else
                    {
                        m_srcRect.X = 0;
                        m_thrust = 0;
                    }
                }
                else  
                {
                    if (m_controller.RightPressed)
                    {
                        motionState = MotionState.WalkingRight;
                        m_thrust = 2;
                    }
                    else if (m_controller.LeftPressed)
                    {
                        motionState = MotionState.WalkingLeft;
                        m_thrust = -2;
                    }
                    else
                    {
                        m_srcRect.X = 0;
                        m_thrust = 0;
                    }
                }
            }
            m_velocity += m_direction * m_thrust;
        }

        private void initAnimation()
        {
            if (m_initDone == false)
            {
                m_isAnimated = true;
                m_srcRect.X = 0;
                m_srcRect.Y = 0;
                m_initDone = true;
            }
        }

        private void updateAim(CrossHair crossHair)
        {
            deltaPosAim.X = crossHair.TargetPos.X - (m_pos.X);
            deltaPosAim.Y = crossHair.TargetPos.Y - (m_pos.Y);
            m_rotation = -(float)(Math.Atan2(deltaPosBound.X, deltaPosBound.Y));
            m_armRotation = -(float)(Math.Atan2(deltaPosAim.X, deltaPosAim.Y) - (Math.PI * 0.5f));
            m_playerArm.Rotation = m_armRotation;
        }

        private void oerientActorBound(Asteroid Asteroid)
        {
            deltaPosBound.X = Asteroid.Center.X - (m_pos.X);
            deltaPosBound.Y = Asteroid.Center.Y - (m_pos.Y);
            m_velocity = Vector2.Normalize(deltaPosBound) * 3;

            if (m_boundingSphere.Intersects(Asteroid.Sphere))
            {
                m_velocity = m_velocity - m_velocity;
            }
        }

        private void orientActorBound(BoundingBox Tile)
        {
            if (BoxHelper.TouchBottomOf(m_boundingSphere, Tile))
            {
                deltaPosBound.X = 0;
                deltaPosBound.Y = 1;
                m_walkingHoz = true;
                m_invert = true;
            }
            if (BoxHelper.TouchTopOf(m_boundingSphere, Tile))
            {
                deltaPosBound.X = 0;
                deltaPosBound.Y = -1;
                m_walkingHoz = true;
                m_invert = false;
            }
            if (BoxHelper.TouchLeftOf(m_boundingSphere, Tile))
            {
                deltaPosBound.X = -1;
                deltaPosBound.Y = 0;
                m_walkingVer = true;
            }
            if (BoxHelper.TouchRightOf(m_boundingSphere, Tile))
            {
                deltaPosBound.X = 1;
                deltaPosBound.Y = 0;
                m_walkingVer = true;
            }
            m_velocity = Vector2.Normalize(deltaPosBound) * 3;

            if ((m_walkingHoz) && (m_walkingVer))
            {
                playerState = ActorState.Floating;
                m_walkingHoz = false;
                m_walkingVer = false;
            }

            if (m_boundingSphere.Intersects(Tile))
            {
                m_velocity = m_velocity - m_velocity;
            }

        }

        private void doBoundActorMotionRight(CrossHair crossHair)
        {
            if ((m_rotation < 1.6f) || (m_rotation > -1.6f))
            {
                if (crossHair.TargetPos.X < m_pos.X)
                {
                    m_sEffect = SpriteEffects.FlipHorizontally;
                    m_playerArm.spriteEffect = SpriteEffects.FlipVertically;
                    rewindCycle();
                }
                else
                {
                    m_sEffect = SpriteEffects.None;
                    m_playerArm.spriteEffect = SpriteEffects.None;
                    playCycle();
                }
            }
            if ((m_rotation > 1.6f) || (m_rotation < -1.6f))
            {
                if (crossHair.TargetPos.X > m_pos.X)
                {
                    m_sEffect = SpriteEffects.FlipHorizontally;
                    m_playerArm.spriteEffect = SpriteEffects.FlipVertically;
                    rewindCycle();
                }
                else
                {
                    m_sEffect = SpriteEffects.None;
                    m_playerArm.spriteEffect = SpriteEffects.None;
                    playCycle();
                }
            }
        }

        private void doBoundActorMotionLeft(CrossHair crossHair)
        {
            if ((m_rotation < 1.6f) || (m_rotation > -1.6))
            {
                if (crossHair.TargetPos.X > m_pos.X)
                {
                    rewindCycle();
                    m_sEffect = SpriteEffects.None;
                    m_playerArm.spriteEffect = SpriteEffects.None;
                }
                else
                {
                    playCycle();
                    m_sEffect = SpriteEffects.FlipHorizontally;
                    m_playerArm.spriteEffect = SpriteEffects.FlipVertically;
                }
            }
            if ((m_rotation > 1.6f) || (m_rotation < -1.6f))
            {
                if (crossHair.TargetPos.X < m_pos.X)
                {
                    playCycle();
                    m_sEffect = SpriteEffects.None;
                    m_playerArm.spriteEffect = SpriteEffects.None;
                }
                else
                {
                    rewindCycle();
                    m_sEffect = SpriteEffects.FlipHorizontally;
                    m_playerArm.spriteEffect = SpriteEffects.FlipVertically;
                }
            }
        }

        private void managePrompt()
        {
            m_promptTimer--;
            if (m_promptTimer > 0)
            {
                m_statePrompt = true;
            }
            else
            {
                m_statePrompt = false;
            }
        }

        private void orientActorFloating()
        {
            if (isMovingRight)
            {
                if (isFacingRight)
                {
                    m_sEffect = SpriteEffects.None;
                    m_playerArm.spriteEffect = SpriteEffects.None;
                }
                else
                {
                    m_sEffect = SpriteEffects.FlipHorizontally;
                    m_playerArm.spriteEffect = SpriteEffects.FlipHorizontally;
                }
            }
            else
            {
                if (isFacingRight)
                {
                    m_sEffect = SpriteEffects.FlipHorizontally;
                    m_playerArm.spriteEffect = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    m_sEffect = SpriteEffects.FlipVertically;
                    m_playerArm.spriteEffect = SpriteEffects.FlipVertically;
                }
            }
        }

        private void doAsteroidCollisionCheck()
        {
            foreach (Asteroid asteroid in m_Asteroids)
            {
                if (m_boundingSphere.Intersects(asteroid.Sphere))
                {
                    m_velocity -= m_velocity * 2;
                    m_promptTimer = 60;
                    m_lastHit = asteroid;
                    break;
                }
            }
        }

        private void doTileCollisionCheck()
        {
            foreach (BoundingBox Collider in m_map.CollisionList)
            {
                if (m_boundingSphere.Intersects(Collider))
                {
                    m_lastHit = Collider;
                    m_promptTimer = 60;

                    if (m_boundingSphere.TouchTopOf(Collider))
                    {
                        if (playerState == ActorState.Floating)
                            m_velocity.Y -= m_velocity.Y - 1;
                        else
                            m_velocity.Y -= m_velocity.Y;
                    }
                    if (m_boundingSphere.TouchBottomOf(Collider))
                    {
                        if (playerState == ActorState.Floating)
                            m_velocity.Y -= m_velocity.Y + 1;
                        else
                            m_velocity.Y -= m_velocity.Y;
                    }
                    if (m_boundingSphere.TouchLeftOf(Collider))
                    {
                        if (playerState == ActorState.Floating)
                            m_velocity.X -= m_velocity.X - 1;
                        else
                            m_velocity.X -= m_velocity.X;
                    }
                    if (m_boundingSphere.TouchRightOf(Collider))
                    {
                        if (playerState == ActorState.Floating)
                            m_velocity.X -= m_velocity.X + 1;
                        else
                            m_velocity.X -= m_velocity.X;
                    }
                }
            }

        }

        private void doItemCollisionCheck(List<PickupItem> Items)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (m_boundingSphere.Intersects(Items[i].Sphere))
                {
                    Type itemType = Items[i].GetType();

                    if ((itemType.BaseType == typeof(Weapon)) || (itemType == typeof (Weapon))) 
                    {
                        Weapon weaponItem = (Weapon)Items[i];
                        if (!m_isRemote)
                        {
                            if (pickUpWeapon(weaponItem))
                                Items.RemoveAt(i);
                        }
                        else
                        {
                            if (!weaponItem.Dropped)
                            Items.RemoveAt(i);
                        }
                    }
                    else if (itemType == typeof(Flag))
                    {
                        Flag Flag = (Flag)Items[i];

                        if (m_flagSlot == null)
                        {
                            if (!Flag.Dropped)
                            {
                                if (Flag.Team != m_team)
                                {
                                        m_flagSlot = Flag;
                                        m_flagSlot.atHome = false;
                                        Items.RemoveAt(i);
                                }
                                else if (!Flag.atHome)
                                {
                                    Flag.Base.SpawnFlag(Items);
                                    Items.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void doDamageFlash()
        {
            if (ticker > 0)
            {
                ticker--;
                m_tint = Color.Red;
                m_hit = true;
            }
            else
            {
                m_tint = Color.White;
                ticker = 0;
                m_hit = false;
            }
        }

        private bool pickUpWeapon(Weapon weapon)
        {
            if (!weapon.Dropped)
            {
                if (m_inventory.Count < 2)
                {
                    m_inventory.Add(weapon);
                    m_selectedWeapon = weapon;
                    m_selectedWeapon.User = this;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void dropSelectedWeapon(List<PickupItem> ItemList)
        {
            if (m_selectedWeapon != null)
            {
                m_selectedWeapon.Velocity += m_velocity * 3;
                m_selectedWeapon.Dropped = true;
                ItemList.Add(m_selectedWeapon);
            }
            if (!m_isRemote)
            m_inventory.Remove(m_selectedWeapon);
            
            

            m_selectedWeapon = null;
        }

        private void dropAllItems(List<PickupItem> ItemList)
        {
            //foreach (Weapon wpn in m_inventory)
            //{
            //    if (wpn != null)
            //    {
            //        int i = m_inventory.IndexOf(wpn);
            //        wpn.Velocity += m_velocity * Game1.RNG.Next(-5, 5);
            //        wpn.Dropped = true;
            //        ItemList.Add(wpn);
            //        m_inventory[i] = null;
            //    }
            //}
            if (m_selectedWeapon != null)
            {
                m_selectedWeapon.Velocity += Vector2.One;
                m_selectedWeapon.Velocity += m_velocity * 3;
                m_selectedWeapon.Dropped = true;
                ItemList.Add(m_selectedWeapon);
                m_selectedWeapon = null;
                m_inventory.Clear();
            }
            dropFlag();
        }

        public void dropFlag()
        {
            if (m_flagSlot != null)
            {
                m_flagSlot.Velocity += m_velocity * 3;
                m_flagSlot.Dropped = true;
                m_itemList.Add(m_flagSlot);
                m_flagSlot = null;
            }
        }

    }
}
