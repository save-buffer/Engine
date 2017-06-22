using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    //These flags/constants are #define'd in the original xinput header file.
    [Flags]
    public enum XInputButtons : int
    {
        DpadUp = 0x0001,
        DpadDown = 0x0002,
        DpadLeft = 0x0004,
        DpadRight = 0x0008,
        Start = 0x0010,
        Back = 0x0020,
        LeftStick = 0x0040,
        RightStick = 0x0080,
        LeftShoulder = 0x0100,
        RightShoulder = 0x0200,
        A = 0x1000,
        B = 0x2000,
        X = 0x4000,
        Y = 0x8000,
    };

    public class XInputConstants
    {        
        public const int LeftThumbDeadzone = 7849;
        public const int RightThumbDeadzone = 8689;
        public const int TriggerThreshold = 30;

        public const int XINPUT_FLAG_GAMEPAD = 0x00000001;
    }
}
