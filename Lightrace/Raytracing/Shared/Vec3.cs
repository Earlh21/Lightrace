using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightrace.Raytracing.Shared
{
    public struct Vec3
    {
        public float X;
        public float Y;
        public float Z;

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(double x, double y, double z)
        {
            X = (float)x;
            Y = (float)y;
            Z = (float)z;
        }

        public float Norm()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z + Z);
        }

        public Vec3 Normalized()
        {
            return this / Norm();
        }

        public Vec3 Multiply(Vec3 b)
        {
            return new Vec3(X * b.X, Y * b.Y, Z * b.Z);
        }

        public Vec3 Min(Vec3 b)
        {
            return new Vec3(Math.Min(X, b.X), Math.Min(Y, b.Y), Math.Min(Z, b.Z));
        }

        public Vec3 Pow(float exponent)
        {
            return new Vec3(Math.Pow(X, exponent), Math.Pow(Y, exponent), Math.Pow(Z, exponent));
        }

        public float Sum()
        {
            return X + Y + Z;
        }

        public static Vec3 operator +(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3 operator +(Vec3 a, float b) => new Vec3(a.X + b, a.Y + b, a.Z + b);
        public static Vec3 operator /(Vec3 a, float b) => new Vec3(a.X / b, a.Y / b, a.Z / b);
        public static Vec3 operator *(Vec3 a, float b) => new Vec3(a.X * b, a.Y * b, a.Z * b);
    }
}
