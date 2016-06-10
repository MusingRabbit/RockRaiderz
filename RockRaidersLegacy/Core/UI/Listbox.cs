using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RockRaiders.UI
{
    public class Listbox
    {
        private List<label> m_itemList;
        private Rectangle m_rect, m_borderRect;
        private AssetManager m_assetManager;
        private label m_selected;
        private Texture2D m_blankTxr;
        private bool m_visible;
        private int m_xpos, m_ypos;
        private SpriteFont _font;
        private int _verticalSpacing;
        private MouseState oldMouse;

        public label SelectedItem
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
        public bool Visible
        {
            get
            { return m_visible; }
            set
            { m_visible = value; }
        }
        public SpriteFont Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }
        public int Spacing
        {
            get
            {
                return _verticalSpacing;
            }
            set
            {
                _verticalSpacing = value;
            }
        }

        public Listbox(AssetManager AssetManager, int Xpos, int Ypos)
        {
            m_itemList = new List<label>();
            m_assetManager = AssetManager;
            m_xpos=Xpos;
            m_ypos = Ypos;
            m_rect = new Rectangle(Xpos, Ypos, 200, 0);
            m_borderRect = new Rectangle(m_rect.X - 5, m_rect.Y - 5, m_rect.Width + 10, m_rect.Height + 10);
            _font = m_assetManager.FontLib["UIFont"];
            m_visible = true;
            _verticalSpacing = 30;
        }
        public void updateMe(MouseState mouseState)
        {
            Point mousePos = new Point(mouseState.X,mouseState.Y);
            if (m_visible)
            {
                foreach (label lbl in m_itemList)
                {
                    lbl.updateMe();
                    if (m_selected == lbl)
                        lbl.Selected = true;
                    else
                        lbl.Selected = false;

                    if (lbl.Rect.Contains(mousePos))
                    {
                        lbl.BackLit = true;
                        if ((mouseState.LeftButton == ButtonState.Pressed) && oldMouse.LeftButton == ButtonState.Released)
                            m_selected = lbl;
                    }
                    else if (lbl != m_selected)
                    {
                        lbl.BackLit = false;
                    }
                }
            }
            oldMouse = mouseState;
        }
        public void drawMe(SpriteBatch sBatch)
        {
            if (m_visible)
            {
                m_blankTxr = new Texture2D(sBatch.GraphicsDevice, 1, 1);
                m_blankTxr.SetData(new Color[] { Color.SlateGray });

                //if (false)
                //{
                //    sBatch.Draw(m_blankTxr, m_borderRect, Color.White);
                //    sBatch.Draw(m_blankTxr, m_rect, Color.Black);
                //}


                foreach (label lbl in m_itemList)
                    lbl.drawMe(sBatch);
            }
        }
        public void Add(string Text)
        {
            label newLable = new label(_font);
            Vector2 stringDimensions = newLable.Font.MeasureString(Text);
            newLable.Text = Text;
            newLable.Width = (int)stringDimensions.X;
            newLable.Height = newLable.Font.LineSpacing;
            if (m_itemList.Count == 0)
                newLable.Position = new Point(m_xpos, (m_ypos + m_rect.Height));
            else
                newLable.Position = new Point(m_xpos, m_itemList[m_itemList.Count-1].Position.Y + _verticalSpacing);
            
            m_itemList.Add(newLable);
        }
    }
}
