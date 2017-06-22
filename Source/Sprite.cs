using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Engine
{
    class Sprite : Drawable, Updatable, ICloneable
    {
        public bool Visible
        {
            get;
            set;
        }
        public int ZOrder
        {
            get;
            set;
        }
        public PointF Offset
        {
            get
            {
                return new PointF(OffsetX, OffsetY);
            }
            set
            {
                OffsetX = value.X;
                OffsetY = value.Y;
            }
        }

        public PointF Scale
        {
            get
            {
                return new PointF((float)scalex, (float)scaley);
            }
            set
            {
                ScaleX = value.X;
                ScaleY = value.Y;
            }
        }

        public Element Owner;
        public float OffsetX;
        public float OffsetY;
        private float opacity = 1.0f;
        private float luminosity = 0.0f;
        private int Width;
        private int Height;
        private double scalex, scaley;
        private List<Frame> Originals;

        public bool SingleAnimation = false;
        public bool DiffOffsets = false;
        public List<PointF> Offsets;

        public void Kill()
        {
            var remove_drawable = typeof(Game).GetMethod("RemoveDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            remove_drawable.Invoke(null, new object[] { this });
        }

        private void refactor_images(float scalex, float scaley, float opacity, float luminosity)
        {
            try
            {
                for (int i = 0; i < Originals.Count; i++)
                {
                    Models[i].Image = ImageHelper.ChangeOpacity(Originals[i].Image, opacity);
                    Models[i].Image = ImageHelper.AdjustLuminosity(Models[i].Image, luminosity);
                    Models[i].Image = ImageHelper.ResizeImage(Models[i].Image, (int)(Width * scalex), (int)(Height * scaley));
                }
                Model = Models[(int)CurrentModel].Image;
            }
            catch
            {

            }
        }

        public float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                refactor_images((float)scalex, (float)scaley, value, luminosity);
                opacity = value;
            }
        }
        public float Luminosity
        {
            get
            {
                return luminosity;
            }
            set
            {
                refactor_images((float)scalex, (float)scaley, opacity, value);
                luminosity = value;
            }
        }
        public double ScaleX
        {
            get
            {
                return scalex;
            }
            set
            {
                refactor_images((float)value, (float)scaley, opacity, luminosity);
                scalex = value;
            }
        }
        public double ScaleY
        {
            get
            {
                return scaley;
            }
            set
            {
                refactor_images((float)scalex, (float)value, opacity, luminosity);
                scaley = value;
            }
        }
        public const int BACKGROUND = -1;
        public const int MIDGROUND = 0;
        public const int FOREGROUND = 1;

        public float Rotation = 0; //Rotates the image
        public bool RotateAboutOwner = true;

        protected List<Frame> Models; //This is the list of all of the models
        protected Image Model; //This is the current model

        private int AnimationDirection; //Is set to 1 if it is animating forward and -1 if it is animating backwards
        private float AnimationTimer; //Keeps track of the duration of each frame.
        protected int CurrentModel; //The index of the current image in the list

        //Makes sure you can't change CurrentFrameIndex
        public int CurrentFrameIndex
        {
            get
            {
                return CurrentModel;
            }
        }

        public void AnimateForwards() //Start animating forwards
        {
            AnimationDirection = 1;
        }
        public void AnimateBackwards() //Start animating backwards
        {
            AnimationDirection = -1;
        }
        public void StopAnimation() //Stop Animating
        {
            AnimationDirection = 0;
        }
        protected class Frame //This holds an image and a duration for the frame
        {
            public Image Image;
            public float Duration;
            public Frame(Image Model, float Duration)
            {
                this.Image = Model;
                this.Duration = Duration;
            }
        }
        public void SetFrameDuration(uint Frame, float Duration) //Sets the duration of the specified frame
        {
            Models[(int)Frame].Duration = Duration;
        }
        public void SetFrameModel(uint Frame, string Filename) //Sets the model of the specified frame
        {
            Models[(int)Frame].Image = Image.FromFile("Assets\\" + Filename);
        }
        public void SetFrameModel(uint Frame, Image i)
        {
            Models[(int)Frame].Image = i;
        }
        public void FlipFrame(uint Frame, RotateFlipType f)
        {
            Models[(int)Frame].Image.RotateFlip(f);
        }
        public void AddFrame(string Filename, float Duration) //Adds a new frame
        {
            Image i = Image.FromFile("Assets\\" + Filename);
            Models.Add(new Frame(i, (float)Duration));
            Originals.Add(new Frame(i, (float)Duration));
        }
        public void AddFrame(Image i, float Duration) //Adds a new frame
        {
            Models.Add(new Frame(i, (float)Duration));
            Originals.Add(new Frame(i, (float)Duration));
        }

        public void RemoveFrame(uint Frame) //Removes a frame
        {
            Models.RemoveAt((int)Frame);
            Originals.RemoveAt((int)Frame);
        }

        public void ClearFrames()
        {
            Models.Clear();
            Originals.Clear();
            this.CurrentModel = 0;
        }

        public uint NumberOfFrames()
        {
            return (uint)this.Models.Count;
        }

        public void NextFrame() //Goes to the next frame
        {
            CurrentModel += AnimationDirection;
            if (CurrentModel <= -1)
                CurrentModel = Models.Count - 1;
            else if (CurrentModel >= Models.Count)
                CurrentModel = 0;

            Model = Models[CurrentModel].Image;
            AnimationTimer = Models[CurrentModel].Duration;

            if (DiffOffsets)
                this.Offset = Offsets[(CurrentModel < Offsets.Count) ? CurrentModel : Offsets.Count - 1];
        }

        public void PreviousFrame() //Goes to the previous frame
        {
            CurrentModel -= AnimationDirection;
            if (CurrentModel <= -1)
                CurrentModel = Models.Count - 1;
            else if (CurrentModel >= Models.Count)
                CurrentModel = 0;

            Model = Models[CurrentModel].Image;
            AnimationTimer = Models[CurrentModel].Duration;
        }

        public void GoToFrame(uint Frame) //Jumps to the specified frame
        {
            if (Frame < Models.Count)
            {
                CurrentModel = (int)Frame;
                Model = Models[CurrentModel].Image;
                AnimationTimer = Models[CurrentModel].Duration;
            }
        }

        private Frame CurrentFrame()
        {
            return Models[CurrentModel];
        }

        public Sprite(Element Owner, string Texture, PointF Offset) : this(Owner, Texture, Offset.X, Offset.Y)
        {
        }

        public Sprite(Element Owner, string Texture, float OffsetX = 0.0f, float OffsetY = 0.0f)
        {
            this.Owner = Owner;
            this.Visible = true;
            var belongings = typeof(Element).GetField("Belongings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            ((List<Updatable>)belongings.GetValue(Owner)).Add(this);
            var add_drawable = typeof(Game).GetMethod("AddDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            add_drawable.Invoke(null, new object[] { this });
            ZOrder = MIDGROUND; //The default ZOrder is 0 (MIDGROUND)
            Models = new List<Frame>(); //The list of Frames that the object has
            Originals = new List<Frame>();
            Offsets = new List<PointF>();
            this.OffsetX = OffsetX;
            this.OffsetY = OffsetY;
            try
            {
                Image i = Image.FromFile("Assets\\" + Texture); //Load the image
                Width = i.Width;
                Height = i.Height;
                Models.Add(new Frame(i, 0)); //Add the first image into the Frame
                Originals.Add(new Frame(i, 0));
                Model = i; //Set the current model to the default texture
            }
            catch
            {
                Models.Add(null); //Otherwise add a black box
                Originals.Add(null);
                Model = null;
            }
            scalex = 1;
            scaley = 1;
        }

        private Sprite(Sprite s)
        {
            this.AnimationDirection = s.AnimationDirection;
            this.AnimationTimer = s.AnimationTimer;
            this.CurrentModel = s.CurrentModel;
            this.Width = s.Width;
            this.Height = s.Height;
            this.Model = s.Model;
            this.Models = s.Models;
            this.Offset = s.Offset;
            this.Opacity = s.Opacity;
            this.Originals = s.Originals;
            this.Owner = s.Owner;
            this.RotateAboutOwner = s.RotateAboutOwner;
            this.Rotation = s.Rotation;
            this.ScaleX = s.ScaleX;
            this.ScaleY = s.ScaleY;
            this.Visible = s.Visible;
            this.ZOrder = s.ZOrder;

            var belongings = typeof(Element).GetField("Belongings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            ((List<Updatable>)belongings.GetValue(Owner)).Add(this);
            var add_drawable = typeof(Game).GetMethod("AddDrawable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            add_drawable.Invoke(null, new object[] { this });
        }



        public void Draw(Graphics G)
        {
            if (Model != null)
            {
                PointF ScreenPosition = Game.GameToScreenCoordinates(new PointF(Owner.Position.X + this.OffsetX, Owner.Position.Y + this.OffsetY), Width, Height);
                PointF OwnerScreenPosition = Game.GameToScreenCoordinates(Owner.Position, 0, 0);

                var prev_state = G.Save();
                if (RotateAboutOwner)
                {
                    G.TranslateTransform(OwnerScreenPosition.X, OwnerScreenPosition.Y);
                    G.RotateTransform(-Rotation);
                    G.TranslateTransform(-OwnerScreenPosition.X, -OwnerScreenPosition.Y); //Apply Rotation
                }
                else
                {
                    G.TranslateTransform(ScreenPosition.X + Width / 2, ScreenPosition.Y + Height / 2);
                    G.RotateTransform(-Rotation);
                    G.TranslateTransform(-ScreenPosition.X - Width / 2, -ScreenPosition.Y - Height / 2); //Apply Rotation
                }

                G.DrawImage(Model, ScreenPosition);
                G.Restore(prev_state);
            }
        }

        public void Update(float dt)
        {
            AnimationTimer += dt;
            if (AnimationTimer >= CurrentFrame()?.Duration && AnimationDirection != 0)
            {
                if (SingleAnimation && CurrentModel == Models.Count - 1)
                {
                    StopAnimation();
                    this.Visible = false;
                }
                else NextFrame();
                AnimationTimer = 0;
            }
        }

        public object Clone()
        {
            return new Sprite(this);
        }
    }
}
