using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Engine
{
    public interface Drawable
    {
        bool Visible
        {
            get;
            set;
        }
        int ZOrder
        {
            get;
            set;
        }
        void Draw(Graphics g);
    }
}
