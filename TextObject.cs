using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Engine
{
    class TextObject : Element, Drawable
    {
        public int ZOrder
        {
            get;
            set;
        }

        new public void Kill()
        {
            var remove_drawable = typeof(Game).GetMethod("RemoveDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            remove_drawable.Invoke(null, new object[] { this });
            Game.RemoveElement(this.Name);
        }

        public string Text;
        public Color TextColor;
        public Font TextFont;
        private int size;
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                TextFont = new Font(TextFont.OriginalFontName, value);
                size = value;
            }
        }

        public Size RectangleSize
        {
            get
            {
                return TextRenderer.MeasureText(this.Text, this.TextFont);
            }
        }

        public TextObject(string Name, string Text, string FontName = "Consolia", int Size = 12) : base(Name)
        {
            this.Text = Text;
            TextColor = Color.White;
            this.size = Size;
            TextFont = new Font(FontName, Size);
            var add_drawable = typeof(Game).GetMethod("AddDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            add_drawable.Invoke(null, new object[] { this });
        }

        public void Draw(Graphics g)
        {
            g.DrawString(this.Text, TextFont, new SolidBrush(TextColor), this.Position);
        }
    }
}
