using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputGamepad
    {
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(0)]
        public short Buttons;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(2)]
        public byte LeftTrigger;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(3)]
        public byte RightTrigger;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(4)]
        public short ThumbLX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(6)]
        public short ThumbLY;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(8)]
        public short ThumbRX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(10)]
        public short ThumbRY;

        public void Zero()
        {
            Buttons = 0;
            LeftTrigger = 0;
            RightTrigger = 0;
            ThumbLX = 0;
            ThumbLY = 0;
            ThumbRX = 0;
            ThumbRY = 0;
        }

        public bool IsButtonPressed(XInputButtons Flag)
        {
            return (Buttons & (short)Flag) != 0;
        }

    }
}
