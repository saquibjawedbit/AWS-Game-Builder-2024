using System.Collections;
using System.Collections.Generic;
using UnityEngine;using System;
using GoSystem.Control;
namespace GoSystem
{
    public class GoInputSystem
    {
        private bool KeyDown(string Button)
        {
            //try
            //{
                return Input.GetButtonDown(Button);
            //}
            //catch
            //{

            //}
        }
        private bool KeyUp(string Button)
        {
            return Input.GetButtonUp(Button);
        }
        private bool Key(string Button)
        {
            return Input.GetButton(Button);
        }
        public bool GetValue;
        //   private bool GetValueDown, GetValueUp;
        public bool GetKey(string Button, string Joystick, bool MobileInput)
        {

            var keybord = Key(Button);
            var joystick = Key(Joystick);
            if (keybord || joystick || MobileInput)
            {
                GetValue = true;
            }
            else
            {
                GetValue = false;
            }
            return GetValue;
        }
        public bool GetKeyDown(string Button, string Joystick, bool MobileInput)
        {

            var keybord = KeyDown(Button);
            var joystick = KeyDown(Joystick);
            if (keybord || joystick || MobileInput)
            {
                GetValue = true;
            }
            else
            {
                GetValue = false;
            }

            return GetValue;
        }
        public bool GetKeyUp(string Button, string Joystick, bool MobileInput)
        {

            var keybord = KeyUp(Button);
            var joystick = KeyUp(Joystick);

            if (keybord || joystick || MobileInput)
            {
                GetValue = true;
            }
            else
            {
                GetValue = false;
            }
            return GetValue;
        }


    }
    [Serializable]
    public class GoInput
    {
        public input2 Joystick;
        public input1 Kaybord;

        public IEnumerator MobileInput(Action<bool> onSwitchValue)
        {
            onSwitchValue?.Invoke(true);
            yield return new WaitForEndOfFrame();
            onSwitchValue?.Invoke(false);
        }
    }
    [Serializable]
    public class MoveInput
    {
        public input3 MoveAxis;
        public GoUiJoystick MobileJoystick;
    }

    [Serializable]
    public class CameraMobileInput
    {
        public input3 MoveAxis;
        public GoTouchAxis TouchSpace;
        public float panSpeed = 100f;

    }

}