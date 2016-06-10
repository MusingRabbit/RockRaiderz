using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RockRaiders.UI
{
    public class CrossHair : StaticGraphic
    {
        private Vector2 m_center;
        private bool m_mouseClicked;
        private Vector2 mousePos;

        public Vector2 TargetPos
        {
            get
            {
                return m_center;
            }
            set
            {
                m_center = value;
            }
        }
        public bool isFiring
        {
            get
            {
                return m_mouseClicked;
            }
            set
            {
                m_mouseClicked = value;
            }
        }

        public CrossHair(Texture2D Texture2D)
            :base(Texture2D, Vector2.Zero)
        {
            m_mouseClicked = false;
        }

        public void updateMe(MouseState mouseCurr)
        {
            mousePos.X = mouseCurr.X;
            mousePos.Y = mouseCurr.Y;
            m_pos = mousePos;
            m_center.X = (m_pos.X + (m_txr.Width / 2));
            m_center.Y = (m_pos.Y + (m_txr.Height / 2));

            if (mouseCurr.LeftButton == ButtonState.Pressed)
            {
                m_tint = Color.Red;
                m_mouseClicked = true;
            }
            else
            {
                m_tint = Color.GreenYellow;
                m_mouseClicked = false;
            }
        }

        public void updateMe(MouseState mouseCurr, Camera2D camera, Actor Player)
        {
            mousePos = new Vector2(mouseCurr.X - (int)camera.FocusPoint.X, mouseCurr.Y - (int)camera.FocusPoint.Y);
            m_pos = mousePos;
            m_pos += Player.Position;

            m_center.X = (m_pos.X + (m_txr.Width / 2));
            m_center.Y = (m_pos.Y + (m_txr.Height / 2));

            if (mouseCurr.LeftButton == ButtonState.Pressed)
            {
                m_tint = Color.Red;
                m_mouseClicked = true;
            }
            else
            {
                m_tint = Color.GreenYellow;
                m_mouseClicked = false;
            }
        }

        public override void Draw(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_pos, m_tint);
        }
    }
}
