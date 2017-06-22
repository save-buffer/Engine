using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Engine
{
    static class XInput
    {
        [DllImport("xinput9_1_0.dll")]
        public static extern int XInputGetState(int ControllerIndex, ref XInputState State);

        [DllImport("xinput9_1_0.dll")]
        public static extern int XInputSetState(int ControllerIndex, ref XInputVibration Vibration);
    }
}
