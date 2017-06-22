using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class XboxController
    {
        int index;
        XInputState curr_state;
        XInputState prev_state;

        public XboxController(int Index)
        {
            index = Index;
            curr_state = new XInputState();
            prev_state = new XInputState();
        }

        public bool IsButtonPressed(XInputButtons Button)
        {
            return curr_state.Gamepad.IsButtonPressed(Button);
        }

        public bool IsButtonTriggered(XInputButtons Button)
        {
            return curr_state.Gamepad.IsButtonPressed(Button) & !prev_state.Gamepad.IsButtonPressed(Button);
        }

        public bool IsKeyReleased(XInputButtons Button)
        {
            return !curr_state.Gamepad.IsButtonPressed(Button) & prev_state.Gamepad.IsButtonPressed(Button);
        }

        public Engine.PointF ThumbstickR
        {
            get
            {
                float x = Math.Abs((curr_state.Gamepad.ThumbRX / XInputConstants.RightThumbDeadzone)) > 0 ? curr_state.Gamepad.ThumbRX : 0;
                float y = Math.Abs((curr_state.Gamepad.ThumbRY / XInputConstants.RightThumbDeadzone)) > 0 ? curr_state.Gamepad.ThumbRY : 0;
                return (new PointF(x, y)) / 32768.0f;
            }
        }

        public Engine.PointF ThumbstickL
        {
            get
            {
                float x = Math.Abs((curr_state.Gamepad.ThumbLX / XInputConstants.RightThumbDeadzone)) > 0 ? curr_state.Gamepad.ThumbLX : 0;
                float y = Math.Abs((curr_state.Gamepad.ThumbLY / XInputConstants.RightThumbDeadzone)) > 0 ? curr_state.Gamepad.ThumbLY : 0;
                return (new PointF(x, y)) / 32768.0f;
            }
        }

        public float RightTrigger
        {
            get
            {
                return (curr_state.Gamepad.RightTrigger > XInputConstants.TriggerThreshold ? curr_state.Gamepad.RightTrigger : 0.0f) / 255.0f;
            }
        }

        public float LeftTrigger
        {
            get
            {
                return (curr_state.Gamepad.LeftTrigger > XInputConstants.TriggerThreshold ? curr_state.Gamepad.LeftTrigger : 0.0f) / 255.0f;
            }
        }

        public void Vibrate(XInputVibration vibration)
        {
            XInput.XInputSetState(index, ref vibration);
        }

        public void Update()
        {
            prev_state = curr_state;
            if(XInput.XInputGetState(index, ref curr_state) != 0x0) //0x0 is ERROR_SUCCESS
            {
                curr_state.Gamepad.Zero();
            }
        }
    }
}
