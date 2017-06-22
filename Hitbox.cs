using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace Engine
{
    public class Hitbox : Drawable, Updatable
    {
        public Element Owner;
        public PointF[] Points;
        public Color DrawColor;
        float rotation;
        float rotation_about_owner;
        PointF Offset;
        public int ZOrder
        {
            get;
            set;
        }

        public bool Visible
        {
            get;
            set;
        }

        float scalex = 1.0f;
        float scaley = 1.0f;

        public PointF Scale
        {
            get
            {
                return new PointF(scalex, scaley);
            }
            set
            {
                ScaleX = value.X;
                ScaleY = value.Y;
            }
        }

        public float ScaleX
        {
            get
            {
                return scalex;
            }
            set
            {
                for (int i = 0; i < Points.Length; i++)
                    Points[i].X = Points[i].X / scalex * value;
                scalex = value;
            }
        }

        public float ScaleY
        {
            get
            {
                return scaley;
            }
            set
            {
                for (int i = 0; i < Points.Length; i++)
                    Points[i].Y = Points[i].Y / scaley * value;
                scaley = value;
            }
        }

        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i] = RotateAboutOrigin(Points[i], (value % 360.0f) - rotation);
                }
                rotation = value % 360.0f;
            }
        }

        public float RotationAboutOwner
        {
            get
            {
                return rotation_about_owner;
            }

            set
            {
                for(int i = 0; i < Points.Length; i++)
                {
                    Points[i] += Offset;
                    Points[i] = RotateAboutOrigin(Points[i], (value % 360.0f) - rotation_about_owner);
                    Points[i] -= Offset;
                }
                rotation_about_owner = value % 360.0f;
            }
        }

        public bool Enabled = true;

        public void Kill()
        {
            var remove_drawable = typeof(Game).GetMethod("RemoveDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            remove_drawable.Invoke(null, new object[] { this });
        }

        private static Vector<float> PointFToVector(PointF p)
        {
            float[] xy = new float[2];
            xy[0] = p.X;
            xy[1] = p.Y;
            return CreateVector.DenseOfArray(xy);
        }
        private static float IntervalDistance(float min_a, float max_a, float min_b, float max_b)
        {
            if (min_a < min_b)
                return min_b - max_a;
            return min_a - max_b;
        }

        private IEnumerable<Vector<float>> Normals()
        {
            for (int i = 0; i < Points.Length - 1; i++)
            {
                PointF n = Points[i + 1] - Points[i];
                Vector<float> normal = PointFToVector(n);
                yield return normal;
            }
            yield return PointFToVector(Points[Points.Length - 1] - Points[0]);
        }

        private static void Project(Vector<float> axis, PointF[] poly, ref float min, ref float max)
        {
            Vector<float> temp = PointFToVector(poly[0]);
            float dot_product = axis.DotProduct(temp);
            min = dot_product;
            max = dot_product;
            for (int i = 0; i < poly.Length; i++)
            {
                temp = PointFToVector(poly[i]);
                dot_product = axis.DotProduct(temp);
                if (dot_product < min)
                    min = dot_product;
                if (dot_product > max)
                    max = dot_product;
            }
        }

        public PointF Center()
        {
            PointF center = new PointF(0, 0);
            foreach (PointF p in this.Points)
                center += p;
            return center / Points.Length;
        }

        public bool IsCollidedWithHitbox(Hitbox b)
        {
            if (!Enabled || !b.Enabled)
                return false;
            PointF[] apoints = new PointF[this.Points.Length];
            for (int i = 0; i < apoints.Length; i++)
            {
                apoints[i] = Game.GameToScreenCoordinates(this.Points[i] + this.Owner.Position + this.Offset, 0, 0);
            }
            PointF[] bpoints = new PointF[b.Points.Length];
            for (int i = 0; i < bpoints.Length; i++)
            {
                bpoints[i] = Game.GameToScreenCoordinates(b.Points[i] + b.Owner.Position + b.Offset, 0, 0);
            }

            foreach (var axis in this.Normals().Concat(b.Normals()))
            {

                float mina = 0;
                float maxa = 0;
                float minb = 0;
                float maxb = 0;
                Project(axis, apoints, ref mina, ref maxa);
                Project(axis, bpoints, ref minb, ref maxb);

                if (minb > maxa || mina > maxb)
                {
                    DrawColor = Color.LimeGreen;
                    return false;
                }
            }
            DrawColor = Color.HotPink;
            return true;
        }

        public static PointF RotateAboutOrigin(PointF p, float Angle)
        {
            float radians = Angle / 180.0f * (float)Math.PI;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            return new PointF(p.X * cos - p.Y * sin, p.Y * cos + p.X * sin);
        }

        public Hitbox(Element Owner, PointF[] Points, PointF Offset) : this(Owner, Points, Offset.X, Offset.Y)
        {

        }

        public Hitbox(Element Owner, PointF[] Points, float OffsetX = 0.0f, float OffsetY = 0.0f)
        {
            Offset = new PointF(OffsetX, OffsetY);
            this.Points = Points;
            this.Owner = Owner;
            this.Visible = false;
            this.ZOrder = 1000;
            this.DrawColor = Color.LimeGreen;
            var belongings = typeof(Element).GetField("Belongings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            ((List<Updatable>)belongings.GetValue(Owner)).Add(this);
            var add_drawable = typeof(Game).GetMethod("AddDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            add_drawable.Invoke(null, new object[] { this });
        }

        public void Draw(Graphics G)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                PointF p1 = Game.GameToScreenCoordinates(Owner.Position + Points[i] + Offset, 0, 0);
                PointF p2 = Game.GameToScreenCoordinates(Owner.Position + Points[(i + 1) % Points.Length] + Offset, 0, 0);
                G.DrawLine(new Pen(DrawColor, 1), p1, p2);
            }
        }

        public void Update(float dt)
        {
        }
    }
}
