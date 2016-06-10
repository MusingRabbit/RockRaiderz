using System;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.Helpers
{
    class MiscFunctions
    {
        public static Vector2 ScreenToWorld(Vector2 onScreen, Camera2D WorldCam)
        {
            var matrix = Matrix.Invert(WorldCam.Transform);
            return Vector2.Transform(onScreen, matrix);
        }

        public static bool RectIntersectSphere(BoundingSphere Sphere, Rectangle Rectangle)
        {
            Vector2 SphereDistance;

            SphereDistance.X = Math.Abs(Sphere.Center.X - Rectangle.Center.X);
            SphereDistance.Y = Math.Abs(Sphere.Center.Y - Rectangle.Center.Y);

            if (SphereDistance.X <= (Rectangle.Width / 2))
            {
                return true;
            }
            if (SphereDistance.Y <= (Rectangle.Height / 2))
            {
                return true;
            }

            return false;
        }
    }
}
