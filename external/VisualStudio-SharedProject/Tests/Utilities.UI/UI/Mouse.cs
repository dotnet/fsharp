/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Input;
using TestUtilities;

namespace TestUtilities.UI {
    /// <summary>
    /// Exposes a simple interface to common mouse operations, allowing the user to simulate mouse input.
    /// </summary>
    /// <example>The following code moves to screen coordinate 100,100 and left clicks.
    /// <code>
    /**
        Mouse.MoveTo(new Point(100, 100));
        Mouse.Click(MouseButton.Left);
    */
    /// </code>
    /// </example>
    public static class Mouse {
        /// <summary>
        /// Clicks a mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to click.</param>
        public static void Click(MouseButton mouseButton = MouseButton.Left) {
            Down(mouseButton);
            Up(mouseButton);
        }

        /// <summary>
        /// Double-clicks a mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to click.</param>
        public static void DoubleClick(MouseButton mouseButton) {
            Down(mouseButton, delay:false);
            Up(mouseButton, delay:false);
            Down(mouseButton, delay: false);
            Up(mouseButton);
        }

        /// <summary>
        /// Performs a mouse-down operation for a specified mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to use.</param>
        public static void Down(MouseButton mouseButton, bool delay = true) {
            switch (mouseButton) {
                case MouseButton.Left:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.LeftDown, delay);
                    break;
                case MouseButton.Right:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.RightDown, delay);
                    break;
                case MouseButton.Middle:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.MiddleDown, delay);
                    break;
                case MouseButton.XButton1:
                    SendMouseInput(0, 0, NativeMethods.XButton1, NativeMethods.SendMouseInputFlags.XDown, delay);
                    break;
                case MouseButton.XButton2:
                    SendMouseInput(0, 0, NativeMethods.XButton2, NativeMethods.SendMouseInputFlags.XDown, delay);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported MouseButton input.");
            }
        }

        public static void MoveTo(System.Windows.Point point) {
            SendMouseInput((int)point.X, (int)point.Y, 0, NativeMethods.SendMouseInputFlags.Move | NativeMethods.SendMouseInputFlags.Absolute);            
        }
        /// <summary>
        /// Moves the mouse pointer to the specified screen coordinates.
        /// </summary>
        /// <param name="point">The screen coordinates to move to.</param>
        public static void MoveTo(System.Drawing.Point point) {
            SendMouseInput(point.X, point.Y, 0, NativeMethods.SendMouseInputFlags.Move | NativeMethods.SendMouseInputFlags.Absolute);
        }

        /// <summary>
        /// Resets the system mouse to a clean state.
        /// </summary>
        public static void Reset() {
            MoveTo(new Point(0, 0));

            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed) {
                SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.LeftUp);
            }

            if (System.Windows.Input.Mouse.MiddleButton == MouseButtonState.Pressed) {
                SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.MiddleUp);
            }

            if (System.Windows.Input.Mouse.RightButton == MouseButtonState.Pressed) {
                SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.RightUp);
            }

            if (System.Windows.Input.Mouse.XButton1 == MouseButtonState.Pressed) {
                SendMouseInput(0, 0, NativeMethods.XButton1, NativeMethods.SendMouseInputFlags.XUp);
            }

            if (System.Windows.Input.Mouse.XButton2 == MouseButtonState.Pressed) {
                SendMouseInput(0, 0, NativeMethods.XButton2, NativeMethods.SendMouseInputFlags.XUp);
            }
        }

        /// <summary>
        /// Simulates scrolling of the mouse wheel up or down.
        /// </summary>
        /// <param name="lines">The number of lines to scroll. Use positive numbers to scroll up and negative numbers to scroll down.</param>
        public static void Scroll(double lines) {
            int amount = (int)(NativeMethods.WheelDelta * lines);

            SendMouseInput(0, 0, amount, NativeMethods.SendMouseInputFlags.Wheel);
        }

        /// <summary>
        /// Performs a mouse-up operation for a specified mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to use.</param>
        public static void Up(MouseButton mouseButton, bool delay = true) {
            switch (mouseButton) {
                case MouseButton.Left:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.LeftUp, delay);
                    break;
                case MouseButton.Right:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.RightUp, delay);
                    break;
                case MouseButton.Middle:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.MiddleUp, delay);
                    break;
                case MouseButton.XButton1:
                    SendMouseInput(0, 0, NativeMethods.XButton1, NativeMethods.SendMouseInputFlags.XUp, delay);
                    break;
                case MouseButton.XButton2:
                    SendMouseInput(0, 0, NativeMethods.XButton2, NativeMethods.SendMouseInputFlags.XUp, delay);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported MouseButton input.");
            }
        }

        /// <summary>
        /// Sends mouse input.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="data">scroll wheel amount</param>
        /// <param name="flags">SendMouseInputFlags flags</param>
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        private static void SendMouseInput(int x, int y, int data, NativeMethods.SendMouseInputFlags flags, bool delay = true) {
            PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
            permissions.Demand();

            int intflags = (int)flags;

            if ((intflags & (int)NativeMethods.SendMouseInputFlags.Absolute) != 0) {
                // Absolute position requires normalized coordinates.
                NormalizeCoordinates(ref x, ref y);
                intflags |= NativeMethods.MouseeventfVirtualdesk;
            }

            NativeMethods.INPUT mi = new NativeMethods.INPUT();
            mi.type = NativeMethods.InputMouse;
            mi.union.mouseInput.dx = x;
            mi.union.mouseInput.dy = y;
            mi.union.mouseInput.mouseData = data;
            mi.union.mouseInput.dwFlags = intflags;
            mi.union.mouseInput.time = 0;
            mi.union.mouseInput.dwExtraInfo = new IntPtr(0);

            if (NativeMethods.SendInput(1, ref mi, Marshal.SizeOf(mi)) == 0) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            if (delay) {
                System.Threading.Thread.Sleep(250);
            }
        }

        private static void NormalizeCoordinates(ref int x, ref int y) {
            int vScreenWidth = NativeMethods.GetSystemMetrics(NativeMethods.SMCxvirtualscreen);
            int vScreenHeight = NativeMethods.GetSystemMetrics(NativeMethods.SMCyvirtualscreen);
            int vScreenLeft = NativeMethods.GetSystemMetrics(NativeMethods.SMXvirtualscreen);
            int vScreenTop = NativeMethods.GetSystemMetrics(NativeMethods.SMYvirtualscreen);

            // Absolute input requires that input is in 'normalized' coords - with the entire
            // desktop being (0,0)...(65536,65536). Need to convert input x,y coords to this
            // first.
            //
            // In this normalized world, any pixel on the screen corresponds to a block of values
            // of normalized coords - eg. on a 1024x768 screen,
            // y pixel 0 corresponds to range 0 to 85.333,
            // y pixel 1 corresponds to range 85.333 to 170.666,
            // y pixel 2 correpsonds to range 170.666 to 256 - and so on.
            // Doing basic scaling math - (x-top)*65536/Width - gets us the start of the range.
            // However, because int math is used, this can end up being rounded into the wrong
            // pixel. For example, if we wanted pixel 1, we'd get 85.333, but that comes out as
            // 85 as an int, which falls into pixel 0's range - and that's where the pointer goes.
            // To avoid this, we add on half-a-"screen pixel"'s worth of normalized coords - to
            // push us into the middle of any given pixel's range - that's the 65536/(Width*2)
            // part of the formula. So now pixel 1 maps to 85+42 = 127 - which is comfortably
            // in the middle of that pixel's block.
            // The key ting here is that unlike points in coordinate geometry, pixels take up
            // space, so are often better treated like rectangles - and if you want to target
            // a particular pixel, target its rectangle's midpoint, not its edge.
            x = ((x - vScreenLeft) * 65536) / vScreenWidth + 65536 / (vScreenWidth * 2);
            y = ((y - vScreenTop) * 65536) / vScreenHeight + 65536 / (vScreenHeight * 2);
        }
    }
}
