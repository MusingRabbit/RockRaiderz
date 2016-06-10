using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockRaidersProto.Core.Primatives
{
    public struct RRBoundingBox : IEquatable<RRBoundingBox>
    {
        private Vector3 m_v3Min;
        private Vector3 m_v3Max;
        private const int m_iCOUTNERCOUNT = 8;

        public Vector3 Max
        {
            get
            {
                return m_v3Max;
            }
            set
            {
                m_v3Max = value;
            }
        }
        public Vector3 Min
        {
            get
            {
                return m_v3Min;
            }
            set
            {
                m_v3Min = value;
            }
        }

        internal string DebugDisplayString
        {
            get
            {
                return string.Concat(
                    "Min( ", Min.ToString(), ") \r\n,",
                    "Max( ", Max.ToString(), ")");
            }
        }

        public override string ToString()
        {
            return string.Format("({Min:{0} Max{1}})",Min.ToString(),Max.ToString());
        }

        public RRBoundingBox(Vector3 min, Vector3 max)
        {
            m_v3Min = min;
            m_v3Max = max;
        }

        public RRBoundingBox(IEnumerable<Vector3> points)
        {
            m_v3Min = new Vector3();
            m_v3Max = new Vector3();

            if (points == null)
                throw new ArgumentNullException();

            if (points.Count() < 1)
                throw new ArgumentException();

            foreach (Vector3 item in points)
            {
                m_v3Min.X = (m_v3Min.X < item.X) ? m_v3Min.X : item.X;
                m_v3Min.Y = (m_v3Min.Y < item.Y) ? m_v3Min.Y : item.Y;
                m_v3Max.X = (m_v3Max.X > item.X) ? m_v3Max.X : item.X;
                m_v3Max.Y = (m_v3Max.Y > item.Y) ? m_v3Max.Y : item.Y;
            }
        }

        public RRBoundingBox(BoundingSphere sphere)
        {
            Vector3 corner = new Vector3(sphere.Radius);
            m_v3Min = sphere.Center - corner;
            m_v3Max = sphere.Center + corner;
        }

        public void Contains(ref RRBoundingBox box, out ContainmentType result)
        {
            result = Contains(box);
        }

        public void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            result = Contains(sphere);
        }

        public void Contains(ref Vector3 point, out ContainmentType result)
        {
            result = Contains(point);
        }

        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            result = Intersects(plane);
        }

        public ContainmentType Contains(RRBoundingBox box)
        {
            bool bDisjoint = 
                (box.Max.X < m_v3Min.X
                || box.Min.X > m_v3Max.X
                || box.Max.Y < m_v3Min.Y
                || box.Min.Y > m_v3Max.Y
                || box.Max.Z < m_v3Min.Z
                || box.Min.Z > m_v3Max.Z);

            bool bContains = 
                (box.Min.X >= m_v3Min.X
                && box.Max.X <= m_v3Max.X
                && box.Min.Y >= m_v3Min.Y
                && box.Max.Y <= m_v3Max.Y
                && box.Min.Z >= m_v3Min.Z
                && box.Max.Z <= m_v3Max.Z);

            return bDisjoint ? ContainmentType.Disjoint : bContains ? ContainmentType.Contains : ContainmentType.Intersects;
        }

        public ContainmentType Contains(BoundingFrustum frustum)
        {
            Vector3[] corners = frustum.GetCorners();

            for (int i = 0; i < corners.Length; i++)
                if (Contains(corners[i]) == ContainmentType.Disjoint)
                    return i > 0 ? ContainmentType.Disjoint : ContainmentType.Intersects;

            return ContainmentType.Contains;
        }

        public ContainmentType Contains(Vector3 point)
        {
            bool bDisjoint =
                (point.X < Min.X
                || point.X > Max.X
                || point.Y < Min.Y
                || point.Y > Max.Y
                || point.Z < Min.Z
                || point.Z > Max.Z);
            bool bIntersect = (point.X == Min.X
                || point.X == Max.X
                || point.Y == Min.Y
                || point.Y == Max.Y
                || point.Z == Min.Z
                || point.Z == Max.Z);

            return bDisjoint ? ContainmentType.Disjoint : bIntersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        public ContainmentType Contains(BoundingSphere sphere)
        {
            bool bContains =
                (sphere.Center.X - Min.X >= sphere.Radius
                && sphere.Center.Y - Min.Y >= sphere.Radius
                && sphere.Center.Z - Min.Z >= sphere.Radius
                && Max.X - sphere.Center.X >= sphere.Radius
                && Max.Y - sphere.Center.Y >= sphere.Radius
                && Max.Z - sphere.Center.Z >= sphere.Radius);

            bool bIntersects = Intersects(sphere);



            return bContains ? ContainmentType.Contains : bIntersects ? ContainmentType.Intersects : ContainmentType.Disjoint;
        }

        public bool Intersects(BoundingSphere sphere)
        {
            return (Min.X - sphere.Center.X <= sphere.Radius
                || Min.Y - sphere.Center.Y <= sphere.Radius
                || Min.Z - sphere.Center.Z <= sphere.Radius
                || Max.X - sphere.Center.X <= sphere.Radius
                || Max.Y - sphere.Center.Y <= sphere.Radius
                || Max.Z - sphere.Center.Z <= sphere.Radius);
        }

        public PlaneIntersectionType Intersects(Plane plane)
        {
            Vector3 v3Pos = new Vector3();
            Vector3 v3Neg = new Vector3();

            bool bNormalX = plane.Normal.X > 0;
            bool bNormalY = plane.Normal.Y > 0;
            bool bNormalZ = plane.Normal.Z > 0;

            v3Pos.X = bNormalX ? Max.X : Min.X;
            v3Pos.Y = bNormalY ? Max.Y : Min.Y;
            v3Pos.Z = bNormalZ ? Max.Z : Min.Z;
            v3Neg.X = bNormalX ? Min.X : Max.X;
            v3Neg.Y = bNormalY ? Min.Y : Max.Y;
            v3Neg.Z = bNormalZ ? Min.Z : Max.Z;

            float fDistance = (
                plane.Normal.X * v3Neg.X
                + plane.Normal.Y * v3Neg.Y
                + plane.Normal.Z * v3Neg.Z
                + plane.D);

            if (fDistance > 0)
                return PlaneIntersectionType.Front;

            fDistance = (
                plane.Normal.X * v3Pos.X
                + plane.Normal.Y * v3Pos.Y
                + plane.Normal.Z * v3Pos.Z
                + plane.D);

            if (fDistance < 0)
                return PlaneIntersectionType.Back;

            return PlaneIntersectionType.Intersecting;
        }

        public bool Intersects(BoundingFrustum frustrum)
        {
            return frustrum.Intersects(ToBoundingBox());
        }

        public float? Intersects(Ray ray)
        {
            return ray.Intersects(ToBoundingBox());
        }

        public RRBoundingBox CreateMerged(RRBoundingBox other)
        {
            RRBoundingBox result = new RRBoundingBox();
            result.Min = Vector3.Min(m_v3Min, other.Min);
            result.Max = Vector3.Max(m_v3Max, other.Max);
            return result;
        }

        public bool Equals(RRBoundingBox other)
        {
            return Min == other.Min && Max == other.Max;
        }

        public override bool Equals(object obj)
        {
            return obj is RRBoundingBox ? base.Equals(obj) : false;
        }

        public static bool operator ==(RRBoundingBox a, RRBoundingBox rhs)
        {
            return a.Equals(rhs);
        }

        public static bool operator !=(RRBoundingBox a, RRBoundingBox rhs)
        {
            return !a.Equals(rhs);
        }
     
        public override int GetHashCode()
        {
            return Min.GetHashCode() + Max.GetHashCode();
        }

        private BoundingBox ToBoundingBox()
        {
            return new BoundingBox(m_v3Min, m_v3Min);
        }


    }
}
