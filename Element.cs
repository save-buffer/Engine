using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Engine
{
    public class Element : Updatable
    {
        public Engine.PointF Position;
        private List<Updatable> Belongings;
        public readonly string Name;
        public bool ScreenWrapping;
        public bool Visible
        {
            get;
            set;
        }

        public void Kill()
        {
            OnDeath();
            foreach (var i in Belongings)
                i.Kill();
            Game.RemoveElement(this.Name);
        }

        public virtual void ScreenWrap()
        {
            if (ScreenWrapping) //If we want to screenwrap, then screenwrap!
            {
                if (Position.X > Game.Dimensions.Width / 2)
                    Position.X = -Game.Dimensions.Width / 2;
                if (Position.Y > Game.Dimensions.Height / 2)
                    Position.Y = -Game.Dimensions.Height / 2;
                if (Position.X < -Game.Dimensions.Width / 2)
                    Position.X = Game.Dimensions.Width / 2;
                if (Position.Y < -Game.Dimensions.Height / 2)
                    Position.Y = Game.Dimensions.Height / 2;
            }
        }
        public virtual void OnDeath() //A function that is possible to override in case the user wants to do something when the Element dies.
        {

        }

        public Element(string Name)
        {
            Belongings = new List<Updatable>();
            Position = new PointF(0, 0);
            this.Name = Name;
            Game.AddElement(this);
        }

        public virtual void Update(float dt)
        {
            ScreenWrap();
            foreach (var x in Belongings)
                x.Update(dt);
        }
    }
}
