using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Engine
{
    class ParticleSystem : Element, Drawable
    {
        private class Particle
        {
            public PointF Position;
            public int FramesLeft;
            public int TotalLifetime;
        }
        public int ZOrder
        {
            get;
            set;
        }
        public Color c;
        public int Radius;
        public int ParticleDuration;
        public int ParticleSize;
        private static Random r = new Random();
        private List<Particle> Particles;
        public int Intensity;
        public ParticleSystem(string Name, Color c, int Radius, int Intensity, int ParticleDuration) : base(Name)
        {
            this.c = c;
            this.Radius = Radius;
            this.Particles = new List<Particle>(Intensity);
            this.Intensity = Intensity;
            this.Visible = true;
            this.ParticleDuration = ParticleDuration * (int)Game.FrameRate;
            this.Position = new PointF(0, 0);
            this.ParticleSize = 3;
        }
        public void Draw(Graphics g)
        {
            int ToGen = r.Next(0, Intensity);
            for (int i = 0; i < ToGen; i++)
            {
                if (Particles.Count <= Intensity && r.Next(0, 101) <= 1)
                {
                    Particle p = new Particle();
                    p.Position = new PointF(this.Position.X, this.Position.Y);
                    float Distance = (float)r.NextDouble() * Radius;
                    float Angle = (float)(r.NextDouble() * 2 * Math.PI);
                    p.Position.X += Distance * (float)Math.Cos(Angle);
                    p.Position.Y += Distance * (float)Math.Sin(Angle);
                    p.FramesLeft = p.TotalLifetime = ParticleDuration;
                    Particles.Add(p);
                }
            }
            for(int i = 0; i < Particles.Count; i++)
            {
                Particle p = Particles[i];
                PointF Position = p.Position;
                PointF ScreenPosition = Game.GameToScreenCoordinates(Position, 0, 0);

                Brush b = new SolidBrush(
                    Color.FromArgb((int)(255 * (float)p.FramesLeft / (float)p.TotalLifetime),
                    c));
                g.FillEllipse(b, ScreenPosition.X, ScreenPosition.Y, ParticleSize, ParticleSize);
                p.FramesLeft -= 1;

                if(p.FramesLeft == 0)
                    Particles.RemoveAt(i--);
            }
        }
    }
}
