using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputState
    {
        [FieldOffset(0)]
        public int PacketNumber;
         
        [FieldOffset(4)]
        public XInputGamepad Gamepad;
    }
}
