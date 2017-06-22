using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Engine
{
    public abstract class State
    {
        public Dictionary<string, Element> Elements;
        public virtual void Initialize() //What to do on startup of the state
        {
            Elements = new Dictionary<string, Element>();
        }
        public virtual void Update(float dt) //Updates all of the objects in the state
        {
            List<Element> x = Elements.Values.ToList();
            for (int i = 0; i < x.Count; i++)
                x[i].Update(dt);
        }
    }
}