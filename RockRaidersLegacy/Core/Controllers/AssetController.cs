using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RockRaiders.Core.Controllers
{

    class AssetController
    {
        private Dictionary<string, Texture2D> assetDictionary;
        private Dictionary<string, SpriteFont> fontDictionary;
        private Dictionary<string, Texture2D> buttonDictionary;

        public Dictionary<string, Texture2D> SpriteLib
        {
            get
            {
                return assetDictionary;
            }
        }
        public Dictionary<string, SpriteFont> FontLib
        {
            get
            {
                return fontDictionary;
            }
        }
        public Dictionary<string, Texture2D> ButtonLib
        {
            get
            {
                return buttonDictionary;
            }
        }

        public AssetManager(ContentManager Content)
        {
            assetDictionary = new Dictionary<string, Texture2D>();
            assetDictionary.Add("MarineSpriteSheet", Content.Load<Texture2D>("SpriteArt/RRMarinesBlue"));
            assetDictionary.Add("MarineSpriteSheet2", Content.Load<Texture2D>("SpriteArt/RRMarinesRed"));
            assetDictionary.Add("TileSet", Content.Load<Texture2D>("Tiles/TileSet"));
            assetDictionary.Add("Asteroid01", Content.Load<Texture2D>("SpriteArt/Asteroid1"));
            assetDictionary.Add("Crosshair",  Content.Load<Texture2D>("SpriteArt/Crosshair"));
            assetDictionary.Add("MuzzleFlash", Content.Load<Texture2D>("SpriteArt/mussleflash"));
            assetDictionary.Add("Bullet", Content.Load<Texture2D>("SpriteArt/bullet1"));
            assetDictionary.Add("Plasma", Content.Load<Texture2D>("SpriteArt/PlasmaBolt"));
            assetDictionary.Add("Laser", Content.Load<Texture2D>("SpriteArt/redLaserRay"));
            assetDictionary.Add("BackGround_1", Content.Load<Texture2D>("BackGround/bkgnd1"));
            assetDictionary.Add("BackGround_2", Content.Load<Texture2D>("BackGround/bkgnd2"));
            assetDictionary.Add("BackGround_3", Content.Load<Texture2D>("BackGround/bkgnd3"));
            assetDictionary.Add("BackGround_4", Content.Load<Texture2D>("BackGround/bkgnd4"));
            assetDictionary.Add("BackGround_5", Content.Load<Texture2D>("BackGround/bkgnd5"));
            assetDictionary.Add("BackGround_6", Content.Load<Texture2D>("BackGround/bkgnd6"));
            assetDictionary.Add("BackGround_7", Content.Load<Texture2D>("BackGround/bkgnd7"));

            
            fontDictionary = new Dictionary<string, SpriteFont>();
            fontDictionary.Add("DebugFont", Content.Load<SpriteFont>("Fonts/Arial08"));
            fontDictionary.Add("UIFont",Content.Load<SpriteFont>("Fonts/Orator"));
            fontDictionary.Add("UIHeader", Content.Load<SpriteFont>("Fonts/OratorHeading"));
            fontDictionary.Add("SegoeUI", Content.Load<SpriteFont>("Fonts/gameFont"));
            fontDictionary.Add("Normal", Content.Load<SpriteFont>("Fonts/NormalFont"));

            buttonDictionary = new Dictionary<string, Texture2D>();
            buttonDictionary.Add("Host_U", Content.Load<Texture2D>("MenuAssets/HostUnlit"));
            buttonDictionary.Add("Host_L", Content.Load<Texture2D>("MenuAssets/HostLit"));
            buttonDictionary.Add("Join_U", Content.Load<Texture2D>("MenuAssets/JoinUnlit"));
            buttonDictionary.Add("Join_L", Content.Load<Texture2D>("MenuAssets/JoinLit"));
            buttonDictionary.Add("Exit_U", Content.Load<Texture2D>("MenuAssets/ExitUnlit"));
            buttonDictionary.Add("Exit_L", Content.Load<Texture2D>("MenuAssets/ExitLit"));
            buttonDictionary.Add("Options_L", Content.Load<Texture2D>("MenuAssets/OptionsLit"));
            buttonDictionary.Add("Options_U", Content.Load<Texture2D>("MenuAssets/OptionsUnlit"));
            buttonDictionary.Add("Back_L", Content.Load<Texture2D>("MenuAssets/backLit"));
            buttonDictionary.Add("Back_U", Content.Load<Texture2D>("MenuAssets/backUnlit"));
        }
    }
}
