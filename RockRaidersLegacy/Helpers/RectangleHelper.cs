using System;
using Microsoft.Xna.Framework;

namespace RockRaiders.Core.Helpers
{
    static class RectangleHelper
    {
        public static bool TouchTopOf(this Rectangle rect_1, Rectangle rect_2)
        {
            return (rect_1.Bottom >= rect_2.Top - 1 &&
                rect_1.Bottom <= rect_2.Top + (rect_2.Height / 2) &&
                rect_1.Right >= rect_2.Left + (rect_2.Width / 5) &&
                rect_1.Left <= rect_2.Right - (rect_2.Width / 5));
        }
        public static bool TouchBottomOf(this Rectangle rect_1, Rectangle rect_2)
        {
            return (rect_1.Top <= rect_2.Bottom + (rect_2.Height / 5) &&
                rect_1.Top >= rect_2.Bottom - 1 &&
                rect_1.Right >= rect_2.Left + (rect_2.Width / 5) &&
                rect_1.Left <= rect_2.Right - (rect_2.Width / 5));
        }
        public static bool TouchLeftOf(this Rectangle rect_1, Rectangle rect_2)
        {
            return (rect_1.Right <= rect_2.Right &&
                rect_1.Right >= rect_2.Left - 5 &&
                rect_1.Top <= rect_2.Bottom - (rect_2.Width / 4) &&
                rect_1.Bottom >= rect_2.Top + (rect_2.Width / 4));
        }
        public static bool TouchRightOf(this Rectangle rect_1, Rectangle rect_2)
        {
            return (rect_1.Left >= rect_2.Left &&
                rect_1.Left <= rect_2.Right + 5 &&
                rect_1.Top <= rect_2.Bottom - (rect_2.Width / 4) &&
                rect_1.Bottom >= rect_2.Top + (rect_2.Width / 4));
        }
    }

    static class BoxHelper
    {
        static int boxWidth, boxHeight;
        static Vector3 sphereTop, sphereBot, sphereLeft, sphereRight;
        static Vector2 boxCenter, sphereCenter;

        public static bool TouchTopOf(this BoundingSphere obj_1, BoundingBox obj_2)
        {
            getValues(obj_1, obj_2);

            return (sphereCenter.Y >= obj_2.Max.Y);

        }
        public static bool TouchBottomOf(this BoundingSphere obj_1, BoundingBox obj_2)
        {
            getValues(obj_1, obj_2);

            return (sphereCenter.Y <= obj_2.Min.Y);
        }
        public static bool TouchLeftOf(this BoundingSphere obj_1, BoundingBox obj_2)
        {
            getValues(obj_1, obj_2);

            return (sphereRight.X >= obj_2.Max.X);
        }
        public static bool TouchRightOf(this BoundingSphere obj_1, BoundingBox obj_2)
        {
            getValues(obj_1, obj_2);

            return (sphereLeft.X <= obj_2.Min.X);
        }
        private static void getValues(BoundingSphere obj_1, BoundingBox obj_2)
        {
            boxWidth = Convert.ToInt32((obj_2.Max.X - obj_2.Min.X));
            boxHeight = Convert.ToInt32((obj_2.Max.Y - obj_2.Min.Y));
            boxCenter = new Vector2(obj_2.Max.X - boxWidth, obj_2.Max.Y - boxHeight);
            sphereCenter = new Vector2(obj_1.Center.X, obj_1.Center.Y);
            sphereTop = new Vector3(obj_1.Center.Y - obj_1.Radius);
            sphereBot = new Vector3(obj_1.Center.Y + obj_1.Radius);
            sphereLeft = new Vector3(obj_1.Center.X - obj_1.Radius);
            sphereRight = new Vector3(obj_1.Center.X + obj_1.Radius);
        }
    }
}
