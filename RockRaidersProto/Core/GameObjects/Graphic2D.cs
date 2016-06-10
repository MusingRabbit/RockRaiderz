using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockRaidersProto.Core.Primatives;

namespace RockRaidersProto.Core.GameObjects
{
    public class Graphic2D
    {
        protected Vector2 m_pos;
        protected Color m_color;
        protected RRTexture2D m_t2dSprite;
        protected SpriteBatch m_sBatch;
        
        public SpriteBatch SpriteBatch
        {
            get
            {
                return m_sBatch;
            }
            set
            {
                m_sBatch = value;
            }
        }

        private void Init()
        {
            m_pos = Vector2.Zero;
            m_t2dSprite = RRTexture2D.CreateBlankTexture(5);
            m_color = Color.White;
            m_sBatch = null;
        }

        public Graphic2D()
        {
            Init();
        }

        public Graphic2D(Vector2 position)
            :this()
        {
            m_pos = position;
        }

        public Graphic2D(Vector2 position, RRTexture2D texture) 
            : this(position)
        {
            m_t2dSprite = texture;
        }

        public Graphic2D(Vector2 position,RRTexture2D texture, Color color) 
            : this(position, texture)
        {
            m_color = color;
        }

        public Graphic2D(Vector2 position, RRTexture2D texture, Color color,SpriteBatch SpriteBatch)
            : this (position, texture, color)
        {
            m_sBatch = SpriteBatch;
        }

        public virtual void Draw()
        {
            if (m_sBatch != null)
                m_sBatch.Draw(m_t2dSprite, m_pos, m_color);
        }

    }
}
