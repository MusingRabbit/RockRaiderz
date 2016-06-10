using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RockRaidersProto.Core.Primatives
{
    public class RRTexture2D : Texture2D
    {
        public RRTexture2D(int width, int height)
            : base(Program.RockRaiders.GraphicsDevice, width, height)
        {
        }
        public RRTexture2D(int width, int height, bool mipmap, SurfaceFormat format)
            : base(Program.RockRaiders.GraphicsDevice, width, height, mipmap, format)
        {
        }
        public RRTexture2D(int width, int height, bool mipmap, SurfaceFormat format, int arraySize)
            : base(Program.RockRaiders.GraphicsDevice, width, height, mipmap, format, arraySize)
        {
        }

        public static RRTexture2D CreateBlankTexture(int Size)
        {
            return CreateBlankTexture(Size, Size);
        }

        public static RRTexture2D CreateBlankTexture(int SizeX, int SizeY)
        {
            RRTexture2D result = new RRTexture2D(SizeX, SizeY, false, SurfaceFormat.Color);
            List<Color> colorList = new List<Color>();
            colorList.Capacity = SizeX * SizeY;

            for (int i = 0; i < colorList.Capacity; i++)
                colorList.Add(Color.White);

            result.SetData(colorList.ToArray());
            return result;
        }

    }

}
