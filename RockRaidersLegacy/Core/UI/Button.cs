using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockRaiders.Primatives;

namespace RockRaiders.UI
{
    public class Button : StaticGraphic
    {
        private string m_name = "";
        private bool m_hover = false;
        private bool m_clicked = false;
        private Rectangle m_rect;
        private Point m_cursorPos;
        private Texture2D m_txrBase, m_txrLit;

        public bool Hover
        {
            get
            {
                return m_hover;
            }
        }
        public bool Clicked
        {
            get
            {
                return m_clicked;
            }
            set
            {
                m_clicked = value;
            }
        }
        public Texture2D BaseTexture
        {
            get 
            {
                return m_txrBase;
            }
            set 
            {
                m_txrBase = value;
                m_txr = value;
            }
        }
        public Texture2D LitTexture
        {
            get 
            {
                return m_txrLit;
            }
            set 
            {
                m_txrLit = value;
            }

        }
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        public Button(int Xpos, int Ypos, Texture2D BaseTexture, Texture2D LitTexture)
            :base(BaseTexture, new Vector2(Xpos,Ypos))
        {
            m_rect = new Rectangle(Xpos, Ypos, BaseTexture.Width, BaseTexture.Height);
            m_txrBase = BaseTexture;
            m_txrLit = LitTexture;
        }

        public Button(int Xpos, int Ypos, int Width, int Height)
            :base(null,new Vector2(Xpos,Ypos))
        {
            m_rect = new Rectangle((int)m_pos.X, (int)m_pos.Y, Width, Height);
        }

        public void updateMe(MouseState cursor)
        {
            if (m_clicked)
                m_clicked = false;

            m_cursorPos.X = (int)cursor.X;
            m_cursorPos.Y = (int)cursor.Y;
            m_rect.X = (int)m_pos.X;
            m_rect.Y = (int)m_pos.Y;
            checkMouse(cursor);
            doAction();
        }
        private void checkMouse(MouseState cursor)
        {
            if (m_rect.Contains(m_cursorPos))
            {
                m_hover = true;
                if (cursor.LeftButton == ButtonState.Pressed)
                    m_clicked = true;
            }
            else 
            {
                m_hover = false;
            }
        }
        private void doAction()
        {
            if (m_hover)
            {
                if (m_txrLit != null)
                    m_txr = m_txrLit;
            }
            else
                m_txr = m_txrBase;
        }
    }
}