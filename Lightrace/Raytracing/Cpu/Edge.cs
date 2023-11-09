using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Cpu
{
    public struct Edge
    {
        public Vec2 Start { get; }
        public Vec2 End { get; }
        public Vec2 Normal { get; }
        public float Length { get; }

        public Edge(Vec2 start, Vec2 end, bool clockwise)
        {
            Start = start;
            End = end;

            var dir = (End - Start).Normalized();
            Normal = clockwise ? new Vec2(-dir.Y, dir.X) : new Vec2(dir.Y, -dir.X);

            Length = (End - Start).Norm();
        }

        public Edge(float x1, float y1, float x2, float y2, bool clockwise) : this(new Vec2(x1, y1), new Vec2(x2, y2), clockwise)
        {

        }

        public bool GetHit(in Ray ray, in Polygon polygon, ref HitRecord record)
        {
            var a = Start;
            var b = End;

            var v1 = ray.Origin - a;
            var v2 = b - a;
            var v3 = new Vec2(-ray.Direction.Y, ray.Direction.X);

            float dot = v2 * v3;

            if (Math.Abs(dot) < 0.00001)
            {
                return false;
            }

            var t1 = v2.Cross(v1) / dot;

            if (t1 < 0)
            {
                return false;
            }

            var t2 = v1 * v3 / dot;

            if (t2 >= 0.0f && t2 <= 1.0f)
            {
                var normal = Normal;
                int is_inside = ray.Direction * normal > 0 ? 1 : 0;

                if (is_inside == 1)
                {
                    normal *= -1;
                }

                record = new HitRecord(t1, ray.Origin + ray.Direction * t1, normal, ray.Direction, polygon.Material, is_inside, polygon.Id);
                return true;
            }

            return false;
        }
    }
}
