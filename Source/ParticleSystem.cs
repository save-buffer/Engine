using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Engine
{
    class ParticleSystem : Element, Drawable, Updatable
    {
        private class Particle
        {
            public PointF Position;
            public float SecondsLeft;
        }
        public int ZOrder
        {
            get;
            set;
        }
        public Color c;
        public int Radius;
        public float ParticleDuration;
        public int ParticleSize;
        private static Random r = new Random();
        private List<Particle> Particles;
        public int NumberOfParticles;
        public ParticleSystem(string Name, Color c, int Radius, int NumberOfParticles, float ParticleDuration) : base(Name)
        {
            this.c = c;
            this.Radius = Radius;
            this.Particles = new List<Particle>(NumberOfParticles);
            this.NumberOfParticles = NumberOfParticles;
            this.Visible = true;
            this.ParticleDuration = ParticleDuration;
            this.Position = new PointF(0, 0);
            this.ParticleSize = 3;

            var add_drawable = typeof(Game).GetMethod("AddDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            add_drawable.Invoke(null, new object[] { this });
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            int ToGen = r.Next(0, NumberOfParticles);

            for(int i = 0; i < Particles.Count; i++)
            {
                Particles[i].SecondsLeft -= dt;
                if (Particles[i].SecondsLeft <= 0)
                    Particles.RemoveAt(i--);
            }

            for (int i = 0; i < ToGen; i++)
            {
                if (Particles.Count <= NumberOfParticles && r.Next(0, 101) <= 1)
                {
                    Particle p = new Particle();
                    p.Position = new PointF(this.Position.X, this.Position.Y);
                    float Distance = (float)r.NextDouble() * Radius;
                    float Angle = (float)(r.NextDouble() * 2 * Math.PI);
                    p.Position.X += Distance * (float)Math.Cos(Angle);
                    p.Position.Y += Distance * (float)Math.Sin(Angle);
                    p.SecondsLeft = ParticleDuration;
                    Particles.Add(p);
                }
            }
        }

        public void Draw(Graphics g)
        {
            for(int i = 0; i < Particles.Count; i++)
            {
                Particle p = Particles[i];
                PointF Position = p.Position;
                PointF ScreenPosition = Game.GameToScreenCoordinates(Position, 0, 0);

                Brush b = new SolidBrush(
                    Color.FromArgb((int)(255 * p.SecondsLeft / ParticleDuration),
                    c));
                g.FillEllipse(b, ScreenPosition.X, ScreenPosition.Y, ParticleSize, ParticleSize);
            }
        }

        public new void Kill()
        {
            base.Kill();
            var remove_drawable = typeof(Game).GetMethod("RemoveDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            remove_drawable.Invoke(null, new object[] { this });
        }
    }
}
