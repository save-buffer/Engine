using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Threading;
namespace Engine
{

    public static class Game
    {
        #region Static
        private static game_obj game;
        public static Size Dimensions;
        public static int FrameRate; //The framerate

        public static PointF GameToScreenCoordinates(PointF Position, int Width, int Height)
        {
            return new PointF((Position.X + Dimensions.Width / 2f) - Width / 2f,
                       (-Position.Y + Dimensions.Height / 2f) - Height / 2f);
        }

        public static PointF GameToScreenCoordinates(PointF Position, float Width, float Height)
        {
            return new PointF((Position.X + Dimensions.Width / 2f) - Width / 2f,
                       (-Position.Y + Dimensions.Height / 2f) - Height / 2f);
        }

        public static State CurrentState
        {
            get
            {
                return game.CurrentState;
            }
            set
            {
                game.drawables?.Clear();
                game.CurrentState?.Elements?.Clear();
                game.CurrentState = value;
                game.CurrentState.Initialize();
            }
        }
        public static void Initialize(State InitialState, string Name, int Width, int Height, int Framerate = 0)
        {
            FrameRate = Framerate;
            game = new game_obj((uint)Width, (uint)Height, Name, (uint)Framerate);
            XboxControllers = new XboxController[4];
            for (int i = 0; i < 4; i++)
                XboxControllers[i] = new XboxController(i);
            Dimensions = new Size(Width, Height);
            CurrentState = InitialState;
            while (game.Running)
                game.Update();
        }
        public static void Quit()
        {
            game.Running = false;
        }

        public static void AddElement(Element e)
        {
            game.CurrentState.Elements.Add(e.Name, e);
        }
        public static void RemoveElement(string name)
        {
            game.CurrentState.Elements.Remove(name);
        }
        public static Element GetElement(string name)
        {
            return game.CurrentState.Elements[name];
        }

        private static void AddDrawable(Drawable d)
        {
            game.drawables.Add(d);
        }
        private static void RemoveDrawable(Drawable d)
        {
            game.drawables.Remove(d);
        }
        #endregion

        #region Input
        public static XboxController[] XboxControllers;

        const int NumberOfKeys = 194; //That is the number of keys in the Keys enum
        public static bool[] KeysCurrentFrame = new bool[NumberOfKeys]; //All of the keys that are pressed this frame
        public static bool[] KeysLastFrame = new bool[NumberOfKeys]; //All of the keys that were pressed last frame
        public static bool[] KeyBuffer = new bool[NumberOfKeys]; //The asynchronous key buffer that is written into whenever the event fires
        public static void OnKeyDown(object Sender, KeyEventArgs e)
        {
            if (!(e.KeyValue > NumberOfKeys))
                KeyBuffer[e.KeyValue] = true;
        }
        public static void OnKeyReleased(object Sender, KeyEventArgs e)
        {
            if (!(e.KeyValue > NumberOfKeys))
                KeyBuffer[e.KeyValue] = false;
        }
        public static bool IsKeyPressed(Keys Key)
        {
            return KeysCurrentFrame[(int)Key];
        }
        public static bool IsKeyTriggered(Keys Key)
        {
            return KeysCurrentFrame[(int)Key] & !KeysLastFrame[(int)Key];
        }
        public static bool IsKeyReleased(Keys Key)
        {
            return !KeysCurrentFrame[(int)Key] & KeysLastFrame[(int)Key];
        }
        #endregion

        #region Game
        private class game_obj : Form
        {
            private bool running = true;
            public State CurrentState;
            private Stopwatch watch;

            public List<Drawable> drawables = new List<Drawable>();

            public bool Running
            {
                get
                {
                    return running;
                }
                set
                {
                    if (!value)
                        this.OnFormClosing(new FormClosingEventArgs(CloseReason.UserClosing, false));
                }
            }//If Quit is true, the game will exit

            private System.ComponentModel.IContainer Components = null;

            public static PointF Dimensions; //The dimensions of the game

            //If a framerate is not passed or FrameRate is 0, we will use an unlocked framerate
            public game_obj(uint Width, uint Height, string Name, uint FrameRate = 0)
            {
                Components = new System.ComponentModel.Container();
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Text = Name;
                this.Width = (int)Width;
                this.Height = (int)Height;
                this.Visible = true;
                this.MinimizeBox = false;
                this.MaximizeBox = false;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.Activate();
                this.BackColor = Color.Black;
                this.DoubleBuffered = true;
                this.Paint += Game_Paint;

                this.KeyDown += new KeyEventHandler(Game_KeyDown); //Keyboard Input Handler setup
                this.KeyUp += new KeyEventHandler(Game_KeyUp);

                Dimensions = new PointF(Width, Height); //Sets the dimensions of the game
                FormBorderStyle = FormBorderStyle.None;
WindowState = FormWindowState.Maximized;

                watch = new Stopwatch();
                watch.Start();

                this.FormClosing += (object sender, FormClosingEventArgs e) =>
                {
                    running = false;
                };
            }
            void Game_Paint(object sender, PaintEventArgs e)
            {
                if (CurrentState != null && CurrentState.Elements != null)
                {
                    foreach (Drawable Obj in this.drawables.OrderBy(Key => Key.ZOrder)) //Order the gameobjects by ZOrder, drawing the lowest first
                    {
                        if (Obj.Visible)
                        {
                            Obj.Draw(e.Graphics);
                        }
                    }
                }
            }

            void Game_KeyDown(object sender, KeyEventArgs e)
            {
                Game.OnKeyDown(sender, e);
            }

            void Game_KeyUp(object sender, KeyEventArgs e)
            {
                Game.OnKeyReleased(sender, e);
            }

            new public void Update()
            {
                Application.DoEvents();
                if (FrameRate == 0 || watch.ElapsedMilliseconds > 1000.0f / FrameRate) //If we can move onto the next frame
                {
                    for (int i = 0; i < KeyBuffer.Length; i++) //Copy the asynchronous buffer into the synchronous one
                    {
                        KeysCurrentFrame[i] = KeyBuffer[i];
                    }
                    foreach (var controller in XboxControllers)
                        controller.Update();
                    if (CurrentState != null)
                        CurrentState.Update(watch.ElapsedMilliseconds / 1000.0f); //Update the current GameState

                    this.Invalidate(); //Invalidate the current image. This calls the Game_Paint event
                    for (int i = 0; i < KeysLastFrame.Length; i++) //Copy the synchronous buffer into the previous frame buffer
                    {
                        KeysLastFrame[i] = KeysCurrentFrame[i];
                    }
                    watch.Restart();
                }
                base.Update();
            }

            private void InitializeComponent()
            {
                //Game Setup
                this.ClientSize = new System.Drawing.Size(490, 301);
                this.DoubleBuffered = true;
                this.Name = "Game";
                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }
    }
    #endregion
}
