using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.GameObjects
{
    class FlagBase
    {
        private Vector2 m_pos;
        private BoundingSphere m_boundingSphere;
        private AssetManager m_assetManager;
        private Flag m_Flag;
        private int m_team;
        private bool m_scored;

        public BoundingSphere Sphere
        {
            get
            {
                return m_boundingSphere;
            }
        }
        public int Team
        {
            get
            {
                return m_team;
            }
        }
        public bool MadeGoal
        {
            get
            {
                return m_scored;
            }
            set
            {
                m_scored = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return m_pos;
            }
        }

        public FlagBase(Vector2 Position, int Team, AssetManager AssetManager)
        {
            m_pos = Position;
            m_team = Team;
            m_boundingSphere.Center = new Vector3(m_pos, 0);
            m_boundingSphere.Radius = 50;
            m_assetManager = AssetManager;
        }

        public void UpdateMe(Flag Flag)
        {
            if (Flag.Team != m_team)
                if (m_boundingSphere.Intersects(Flag.Sphere))
                    m_scored = true;
                else
                    m_scored = false;
        }

        public void SpawnFlag(List<PickupItem> itemList)
        {
            m_Flag = new Flag(m_assetManager);
            m_Flag.Team = m_team;
            m_Flag.Base = this;
            m_Flag.Position = m_pos;
            m_Flag.atHome = true;
            
            itemList.Add(m_Flag);
        }
    }
}
