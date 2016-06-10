using Microsoft.Xna.Framework;

namespace RockRaiders.Core.GameObjects
{

    class SpaceMarine : Actor
    {
        private int m_hitPoints;
        private int m_score;
        private bool m_Local;
        private string m_playerName;
        private AssetManager m_assetManager;


        public int Health
        {
            get
            {
                return m_hitPoints;
            }
            set
            {
                m_hitPoints = value;
            }
        }
        public int Score
        {
            get
            {
                return m_score;
            }
            set
            {
                m_score = value;
            }
        }
        public string PlayerName
        {
            get
            {
                return m_playerName;
            }
            set
            {
                m_playerName = value;
            }
        }
        public bool isLocal
        {
            get
            {
                return m_Local;
            }
            set
            {
                m_Local = value;
            }
        }

        public SpaceMarine(AssetManager AssetContainer)
            : base(AssetContainer.SpriteLib["MarineSpriteSheet"], new Vector2(-100,-100), Vector2.Zero, AssetContainer.FontLib["DebugFont"])
        {
            m_hitPoints = 100;
            m_score = 0;
            setTeamValue("");
            m_Local = false;
            m_assetManager = AssetContainer;
        }
        public override void Update(GameTime gt, PolyInputController controller, World CurrentLevel, CrossHair crossHair)
        {
            if (m_hitPoints <= 0)
            {
                playerState = ActorState.Dying;
            }
            if (m_team == 2)
            {
                m_txr = m_assetManager.SpriteLib["MarineSpriteSheet2"];
                m_playerArm.Texture = m_assetManager.SpriteLib["MarineSpriteSheet2"];
            }
            else
            {
                m_txr = m_assetManager.SpriteLib["MarineSpriteSheet"];
                m_playerArm.Texture = m_assetManager.SpriteLib["MarineSpriteSheet"];
            }

            base.Update(gt, controller, CurrentLevel, crossHair);
        }

        private void setTeamValue(string TeamName)
        {
            if (TeamName == "Red")
            {
                m_team = 1;
                m_tint = Color.LightPink;
            }
            if (TeamName == "Blue")
            {
                m_team = 2;
                m_tint = Color.CadetBlue;
            }
            else
            {
                m_team = 0;
            }
            m_playerArm.Tint = m_tint;
        }
    }
}
