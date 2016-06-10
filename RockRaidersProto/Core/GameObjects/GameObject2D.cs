using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using RockRaidersProto.Core.Primatives;

namespace RockRaidersProto.Core.GameObjects
{
    public class GameObject2D : Graphic2D 
    {
        private Guid m_UID;
        private RRBoundingBox m_box;
        private Vector2 m_vel;
        private float m_fRot;
        private float m_fInertia;
        
        public GameObject2D()
        {
            Init();
        }

        public GameObject2D(Vector2 position, RRTexture2D texture)
            :base(position,texture)
        {
            Init();
        }

        private void Init()
        {
            m_box = new RRBoundingBox();
            m_UID = Guid.NewGuid();
            m_vel = Vector2.Zero;
            m_fInertia = 1;
        }

        public virtual void Update()
        {
            m_pos += m_vel;
            m_vel *= m_fInertia;
        }
        
    }
}
