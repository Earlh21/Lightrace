using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightrace.Raytracing.Shared
{
    public struct Vec2
    {
        public float X;
        public float Y;

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Cross(Vec2 other)
        {
            return X * other.Y - Y * other.X;
        }

        public Vec2 Rotate(float angle)
        {
            return new Vec2(X * (float)Math.Cos(angle) - Y * (float)Math.Sin(angle), X * (float)Math.Sin(angle) + Y * (float)Math.Cos(angle));
        }

        public float Norm()
        {
            return (float)Math.Sqrt(NormSquared());
        }

        public float NormSquared()
        {
            return X * X + Y * Y;
        }

        public Vec2 Normalized()
        {
            return this / Norm();
        }

        public float AngleBetween(Vec2 b)
        {
            return (float)Math.Acos(this * b / Norm() / b.Norm());
        }

        public float SignedAngleBetween(Vec2 b)
        {
            return -(float)Math.Atan2(X * b.Y - Y * b.X, X * b.X + Y * b.Y);
        }

        public Vec2 Refract(Vec2 normal, float n1, float n2)
        {
            float incidence = (-this).AngleBetween(normal);
            float refracted_incidence = (float)Math.Asin(n1 * (float)Math.Sin(incidence) / n2);

            if (Cross(-normal) >= 0)
            {
                return (-normal).Rotate(-refracted_incidence);
            }
            else
            {
                return (-normal).Rotate(refracted_incidence);
            }
        }

        public float Sum()
        {
            return X + Y;
        }

        public float ScalarProjection(Vec2 b)
        {
            return Norm() * MathF.Cos(AngleBetween(b));
        }

        public static Vec2 GetDir(float angle)
        {
            return new Vec2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }


        public static explicit operator Vec2i(in Vec2 a) => new Vec2i((int)a.X, (int)a.Y);
        public static Vec2 operator +(in Vec2 a, in Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator +(in Vec2 a, in float b) => new Vec2(a.X + b, a.Y + b);
        public static Vec2 operator +(in float a, in Vec2 b) => b + a;
        public static Vec2 operator -(in Vec2 a, in Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static float operator *(in Vec2 a, in Vec2 b) => a.X * b.X + a.Y * b.Y;
        public static Vec2 operator *(in Vec2 a, in float b) => new Vec2(a.X * b, a.Y * b);
        public static Vec2 operator *(in float a, in Vec2 b) => b * a;
        public static Vec2 operator -(in Vec2 v) => new Vec2(-v.X, -v.Y);
        public static Vec2 operator /(in Vec2 a, in float b) => new Vec2(a.X / b, a.Y / b);
    }
}
