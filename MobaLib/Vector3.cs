using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaLib
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X=x;
            Y=y;
            Z=z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public override string ToString()
        {
            return string.Format("{{0},{1},{2}}", X, Y, Z);
        }

        public static float Dot(Vector3 vec1, Vector3 vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        public static Vector3 operator +(Vector3 one, Vector3 two)
        {
            return new Vector3(one.X+two.X, one.Y+two.Y, one.Z+two.Z);
        }

        public static Vector3 operator -(Vector3 one, Vector3 two)
        {
            return new Vector3(one.X - two.X, one.Y - two.Y, one.Z - two.Z);
        }

        public static Vector3 operator -(Vector3 one)
        {
            return new Vector3(-one.X, -one.Y, -one.Z);
        }

        public static Vector3 operator *(Vector3 vec, float scale)
        {
            return new Vector3(vec.X * scale, vec.Y * scale, vec.Z * scale);
        }

        public static Vector3 operator /(Vector3 vec, float scale)
        {
            return new Vector3(vec.X / scale, vec.Y / scale, vec.Z / scale);
        }

        public static Vector3 Zero
        {
            get { return new Vector3(0, 0, 0); }
        }
    }
}
