using System;
using UnityEngine;

namespace FarseerPhysics.Collision.Shapes
{
    public struct MassData : IEquatable<MassData>
    {
        public float Area;
        public Vector2 Centroid;
        public float Inertia;
        public float Mass;

        public bool Equals(MassData other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }
            if (obj.GetType() != typeof(MassData))
            {
                return false;
            }
            return Equals((MassData)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = Area.GetHashCode();
            hashCode = (hashCode * 397) ^ Centroid.GetHashCode();
            hashCode = (hashCode * 397) ^ Inertia.GetHashCode();
            return (hashCode * 397) ^ Mass.GetHashCode();
        }

        public static bool operator ==(MassData left, MassData right)
        {
            return left.Area == right.Area && left.Mass == right.Mass && left.Centroid == right.Centroid && left.Inertia == right.Inertia;
        }

        public static bool operator !=(MassData left, MassData right)
        {
            return !(left == right);
        }
    }
}
