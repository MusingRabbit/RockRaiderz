using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RockRaiders.GameObjects
{
    public class Camera2D
    {
        #region Class Properties
        public Viewport View
        {
            get 
            {
                return m_viewPort; 
            }
        }
        public Vector2 Position
        {
            get 
            {
                return m_pos; 
            }
        }
        public Vector2 SavedPosition
        {
            get 
            {
                return m_savedPos; 
            }
        }
        public Vector2 FocusPoint
        {
            get 
            {
                return m_focusPoint; 
            }
        }
        public Vector2 Center
        {
            get 
            {
                return m_center; 
            }
        }
        public float Zoom
        {
            get 
            {
                return m_zoom; 
            }
        }
        public float Rotation
        {
            get 
            {
                return m_rotation; 
            }
        }
        public float SavedRotation
        {
            get 
            {
                return m_savedRotation; 
            }
        }
        public float PositionShakeAmount
        {
            get 
            {
                return m_positionShake;
            }
        }
        public float RotationShakeAmout
        {
            get 
            { 
                return m_rotationShake;
            }
        }
        public float MaxShakeTime
        {
            get 
            {
                return m_maxShakeTime; 
            }
        }
        public Matrix Transform
        {
            get 
            {
                return m_transform; 
            }
        }
        public Actor Source
        {
            get
            {
                return m_source; 
            }
            set
            {
                m_source = value;
            }
        }
        public float SourceRotationOffset
        {
            get 
            {
                return m_sourceRotationOffset; 
            }
        }
        public bool rotateWithSource
        {
            get
            {
                return m_rotateWithSource;
            }
            set
            {
                m_rotateWithSource = value;
            }
        }
        #endregion
        #region Class Variables
        private Viewport m_viewPort;
        private Vector2 m_pos;
        private Vector2 m_savedPos;
        private Vector2 m_center;
        private Vector2 m_focusPoint;
        private float m_zoom;
        private float m_rotation;
        private float m_savedRotation;
        private float m_positionShake;
        private float m_maxShakeTime;
        private Matrix m_transform;
        private Actor m_source;
        private float m_sourceRotationOffset;
        private float m_rotationShake;
        private TimeSpan m_shakeTimer;
        private bool m_rotateWithSource;
        private MouseState oldMouse;
        #endregion

        public Camera2D(Viewport View, Vector2 Position)
        {
            m_viewPort = View;
            m_pos = Position;
            m_zoom = 1.0f;
            m_rotation = 0;
            m_focusPoint = new Vector2(m_viewPort.Width / 2, m_viewPort.Height / 2);
            m_rotateWithSource = false;
        }
        public Camera2D(Viewport View, Vector2 Position, Vector2 Focus, float Zoom, float Rotation)
        {
            m_viewPort = View;
            m_pos = Position;
            m_rotation = Rotation;
            m_focusPoint = Focus;
            m_rotateWithSource = false;
        }
        public void updateMe(GameTime gt, KeyboardState kbCurr,MouseState mouseCurr)
        {
            if (m_shakeTimer.TotalSeconds > 0)
            {
                m_focusPoint = m_savedPos;
                m_rotation = m_savedRotation;
                m_shakeTimer = m_shakeTimer.Subtract(gt.ElapsedGameTime);

                if (m_shakeTimer.TotalSeconds > 0)
                {
                    m_focusPoint += new Vector2((float)((Game1.RNG.NextDouble() * 2) - 1) * m_positionShake,
                        (float)((Game1.RNG.NextDouble() * 2) - 1) * m_positionShake);
                    m_rotation += (float)((Game1.RNG.NextDouble() * 2) - 1) * m_rotationShake;
                }
            }
            else
            {
                if ((kbCurr.IsKeyDown(Keys.PageUp)) || (mouseCurr.ScrollWheelValue > oldMouse.ScrollWheelValue))
                {
                    if (m_zoom < 1.1)
                    m_zoom += 0.009f;
                }
                if ((kbCurr.IsKeyDown(Keys.PageDown)) || (mouseCurr.ScrollWheelValue < oldMouse.ScrollWheelValue))
                {
                    if (m_zoom > 0.9)
                    m_zoom -= 0.009f;
                }
                if (Source == null)
                {
                    Vector2 moveCam = Vector2.Zero;
                    if (kbCurr.IsKeyDown(Keys.W))
                        moveCam.Y -= 5.0f / Zoom;
                    if (kbCurr.IsKeyDown(Keys.S))
                        moveCam.Y += 5.0f / Zoom;
                    if (kbCurr.IsKeyDown(Keys.A))
                        moveCam.X -= 5.0f / Zoom;
                    if (kbCurr.IsKeyDown(Keys.D))
                        moveCam.X += 5.0f / Zoom;

                    m_pos = Vector2.Add(m_pos, moveCam);
                }
                else
                {
                    m_pos = Source.Position;
                }
            }
                

            Vector2 objectPosition = m_source != null ? m_source.Position : m_pos;
            float objectRotation = m_source != null ? m_source.Rotation : m_rotation;
            float deltaRotation = m_source != null ? m_sourceRotationOffset : 0.0f;


            if (m_rotateWithSource)
            m_transform = Matrix.CreateTranslation(new Vector3(-objectPosition, 0)) *
                Matrix.CreateScale(new Vector3((float)Math.Pow(m_zoom, 10), (float)Math.Pow(m_zoom, 10), 0)) *
                Matrix.CreateRotationZ(-objectRotation + deltaRotation) *
                Matrix.CreateTranslation(new Vector3(m_focusPoint.X, m_focusPoint.Y, 0));
            else
                m_transform = Matrix.CreateTranslation(new Vector3(-objectPosition, 0)) *
                    Matrix.CreateScale(new Vector3((float)Math.Pow(m_zoom, 10), (float)Math.Pow(m_zoom, 10), 0)) *
                    Matrix.CreateTranslation(new Vector3(m_focusPoint.X, m_focusPoint.Y, 0));

            oldMouse = mouseCurr;
        }

        public void doShake(float ShakeTime, float PositionShake, float RotationShake)
        {
            if (m_shakeTimer.TotalSeconds <= 0)
            {
                m_maxShakeTime = ShakeTime;
                m_shakeTimer = TimeSpan.FromSeconds(m_maxShakeTime);
                m_positionShake = PositionShake;
                m_rotationShake = RotationShake;

                m_savedPos = m_focusPoint;
                m_savedRotation = m_rotation;
            }

        }
        public void Follow(Actor Source, float rotationOffset)
        {
            m_source = Source;
            m_sourceRotationOffset = rotationOffset;
        }
        public void Reset()
        {
            m_pos = Vector2.Zero;
            m_rotation = 0;
            m_zoom = 1;
            m_shakeTimer = TimeSpan.FromSeconds(0);
            m_source = null;
        }
    }
}
