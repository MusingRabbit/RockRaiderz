using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RockRaiders.UI
{
    public class label
    {
        private Rectangle m_rect;
        private string m_txt;
        private Point m_pos;
        private int m_width, m_height;
        private bool m_visible, m_showBack;
        private SpriteFont m_font;
        private Texture2D m_blankTxr;
        private Color m_backColor;
        private Color m_txtColor;
        private bool m_clickable;
        private bool m_selected;

        public string Text
        {
            get
            {
                return m_txt;
            }
            set
            {
                m_txt = value;
            }
        }
        public Point Position
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }
        public int Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }
        public int Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }
        public bool isVisible
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
            }
        }
        public bool BackLit
        {
            get
            {
                return m_showBack;
            }
            set
            {
                m_showBack = value;
            }
        }
        public SpriteFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }
        public Color BackColor
        {
            get
            {
                return m_backColor;
            }
            set
            {
                m_backColor = value;
            }
        }
        public Color TextColor
        {
            get
            {
                return m_txtColor;
            }
            set
            {
                m_txtColor = value;
            }
        }
        public Rectangle Rect
        {
            get
            {
                return m_rect;
            }
        }
        public bool Selected
        {
            get
            {
                return m_selected;
            }
            set
            {
                m_selected = value;
            }
        }
        public bool isClickable
        {
            get
            {
                return m_clickable;
            }
            set
            {
                m_clickable = value;
            }
        }

        public label(SpriteFont Font)
        {
            m_font = Font;
            m_pos = Point.Zero;
            m_width = 125;
            m_height = Font.LineSpacing;
            m_rect = new Rectangle(m_pos.X - m_width/2, m_pos.Y, m_width, m_height);
            m_visible = true;
            m_txtColor = Color.White;
            m_backColor = Color.CadetBlue;
            m_clickable = true;
        }

        public label(int X, int Y, int Width, int Height)
        {
            m_pos = new Point(X, Y);
            m_width = Width;
            m_height = Height;
            m_rect = new Rectangle(m_pos.X - m_width / 2, m_pos.Y, m_width, m_height);
            m_visible = true;
            m_txtColor = Color.White;
            m_backColor = Color.CadetBlue;

        }

        public void updateMe()
        {
            m_rect.X = m_pos.X;
            m_rect.Y = m_pos.Y; 
            m_rect.Width = m_width;
            m_rect.Height = m_height;
            if (m_clickable)
            {
                if (m_selected)
                    m_showBack = true;
                else
                    m_showBack = false;
            }
        }
        public void drawMe(SpriteBatch sBatch)
        {
            m_blankTxr = new Texture2D(sBatch.GraphicsDevice, 1,1);
            m_blankTxr.SetData(new Color[] { Color.White });

            if (m_visible)
            {
                if (m_showBack)
                {
                    sBatch.Draw(m_blankTxr, m_rect, m_backColor);
                }
                sBatch.DrawString(m_font, m_txt, new Vector2(m_pos.X,m_pos.Y), m_txtColor);
            }
        }
    }
}
