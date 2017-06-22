using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Engine
{
    class Sound
    {
        public readonly string FilePath; //The filename of the file to play
        private string Name; //The name of the sound

        private static int c = 0;

        public Sound(string FilePath)
        {
            this.FilePath = FilePath;
            Name = "MediaFile" + c++; //Open the file
            SendCommand("open \"" + "Assets\\" + FilePath + "\" type mpegvideo alias " + Name);
            SendCommand("set " + Name + " time format ms");
        }

        ~Sound()
        {
            Close();
        }
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public void Play() //To play it, just send the command
        {
            SendCommand("play " + Name);
        }
        public void Pause()
        {
            SendCommand("pause " + Name);
        }
        public void SeekToBeginning()
        {
            SendCommand("seek " + Name + " to start");
        }
        public void SeekTo(long TimeInMilliseconds)
        {
            SendCommand("seek " + Name + " to " + TimeInMilliseconds);
        }

        private static void SendCommand(string Command)
        {
            mciSendString(Command, null, 0, IntPtr.Zero);
        }
        public void Close()
        {
            SendCommand("close " + Name);
        }
    }
}
