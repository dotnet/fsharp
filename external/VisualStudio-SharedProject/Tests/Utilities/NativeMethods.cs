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
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Automation;
using Accessibility;
using Microsoft.VisualStudio.OLE.Interop;

namespace TestUtilities {
    /// <summary>
    /// Unmanaged API wrappers.
    /// </summary>
    public static class NativeMethods {
        public const int SW_SHOW = 5;

        /// <summary>
        /// Get the ROT.
        /// </summary>
        /// <param name="reserved">Reserved.</param>
        /// <param name="prot">Pointer to running object table interface</param>
        /// <returns></returns>
        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        /// <summary>
        /// Win32 GetWindowThreadProcessId: Get process ID from a window handle
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr windowHandle, out uint processId);

        /// <summary>
        /// Create a Bind context.
        /// </summary>
        /// <param name="reserved">Reserved.</param>
        /// <param name="ppbc">Bind context.</param>
        /// <returns>HRESULT</returns>
        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        /// <summary>
        /// Register message filter for COM.
        /// </summary>
        /// <param name="lpMessageFilter">New filter to register.</param>
        /// <param name="lplpMessageFilter">Old filter. Save it if you need to restore it later.</param>
        [DllImport("ole32.dll")]
        public static extern int CoRegisterMessageFilter(IMessageFilter lpMessageFilter, out IMessageFilter lplpMessageFilter);

        /// <summary>
        /// Get the foreground window
        /// </summary>
        /// <returns>An HWND for the current foreground window</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Sets the foreground window
        /// </summary>
        /// <param name="hWnd">HWND of the new foreground window</param>
        /// <returns>true if the operation succeeded</returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Posts a windows message to the message queue of a target window and waits for the message to be processed.
        /// </summary>
        /// <param name="hWnd">The HWND for the target window</param>
        /// <param name="nMessage">The message ID</param>
        /// <param name="wParam">Message-specific WPARAM value</param>
        /// <param name="lParam">Message-specific LPARAM value</param>
        /// <returns>Message-specific LRESULT</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint nMessage, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Posts a windows message to the message queue of a target window and returns immediately.
        /// </summary>
        /// <param name="hWnd">The HWND for the target window</param>
        /// <param name="nMessage">The message ID</param>
        /// <param name="wParam">Message-specific WPARAM value</param>
        /// <param name="lParam">Message-specific LPARAM value</param>
        /// <returns>Message-specific LRESULT</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint nMessage, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Registers a new message ID based on the message name; the ID is uniquely tied to the name.
        /// </summary>
        /// <param name="lpString">Message name</param>
        /// <returns>A new message ID</returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        /// <summary>
        /// Return the thread id and process id of the thread that owns the given window.
        /// </summary>
        /// <param name="hWnd">The HWND for the target window</param>
        /// <param name="processId">The process id which owns hWnd</param>
        /// <returns>The thread id which owns hWnd</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        /// <summary>
        /// Return the thread id of the thread that owns the given window.
        /// </summary>
        /// <param name="hWnd">The HWND for the target window</param>
        /// <param name="processId">Set this to IntPtr.Zero</param>
        /// <returns>The thread id which owns hWnd</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern int GetWindowThreadId(IntPtr hWnd, IntPtr processId);

        /// <summary>
        /// Attaches or detaches the input processing mechanism of one thread to that of another thread.
        /// </summary>
        /// <param name="idAttach">The identifier of the thread to be attached to another thread</param>
        /// <param name="idAttachTo">The identifier of the thread to which idAttach will be attached</param>
        /// <param name="fAttach">If true, the two threads are attached. If false, the threads are detached</param>
        /// <returns>true if the operation succeeded</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">The HWND for the window to be shown</param>
        /// <param name="cmdShow">A command which controls how the window will be shown</param>
        /// <returns>true if the operation succeeded</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        public const int WM_CLOSE = 0x0010;
        public static readonly IntPtr IDC_CONTINUE = new IntPtr(4802);
        public static readonly IntPtr IDC_BREAK = new IntPtr(4943);
        public const int IDC_EXCEPTION_TEXT = 4941;
        public const int IDD_EXCEPTION_THROWN = 4095;

        [DllImport("user32.dll")]
        public static extern bool EndDialog(IntPtr hDlg, IntPtr nResult);

        [DllImport("user32.dll")]
        public static extern uint GetDlgItemText(IntPtr hDlg, int nIDDlgItem, [Out]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetDlgCtrlID(IntPtr hwndCtl);

        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(IntPtr hWnd, int dwObjectID, ref Guid riid, ref IAccessible pAcc);

        public static IAccessible GetAccessibleObject(IntPtr handle) {
            Guid iid = typeof(IAccessible).GUID;
            const int OBJID_WINDOW = 0;
            IAccessible result = null;

            Marshal.ThrowExceptionForHR(AccessibleObjectFromWindow(handle, OBJID_WINDOW, ref iid, ref result));
            return result;
        }

        public static IAccessible GetAccessibleObject(AutomationElement element) {
            return GetAccessibleObject(new IntPtr(element.Current.NativeWindowHandle));
        }

        //User32 wrappers cover API's used for Mouse input
        #region User32
        // Two special bitmasks we define to be able to grab
        // shift and character information out of a VKey.
        public const int VKeyShiftMask = 0x0100;
        public const int VKeyCharMask = 0x00FF;

        // Various Win32 constants
        public const int KeyeventfExtendedkey = 0x0001;
        public const int KeyeventfKeyup = 0x0002;
        public const int KeyeventfScancode = 0x0008;

        public const int MouseeventfVirtualdesk = 0x4000;

        public const int SMXvirtualscreen = 76;
        public const int SMYvirtualscreen = 77;
        public const int SMCxvirtualscreen = 78;
        public const int SMCyvirtualscreen = 79;

        public const int XButton1 = 0x0001;
        public const int XButton2 = 0x0002;
        public const int WheelDelta = 120;

        public const int InputMouse = 0;
        public const int InputKeyboard = 1;

        // Various Win32 data structures
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT {
            public int type;
            public INPUTUNION union;
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION {
            [FieldOffset(0)]
            public MOUSEINPUT mouseInput;
            [FieldOffset(0)]
            public KEYBDINPUT keyboardInput;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        };

        [Flags]
        public enum SendMouseInputFlags {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            Absolute = 0x8000,
        };

        // Importing various Win32 APIs that we need for input
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MapVirtualKey(int nVirtKey, int nMapType);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT mi, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short VkKeyScan(char ch);

        #endregion

        /// <summary>
        /// Recursively deletes a directory using the shell API which can 
        /// handle long file names
        /// </summary>
        /// <param name="dir"></param>
        public static void RecursivelyDeleteDirectory(string dir) {
            SHFILEOPSTRUCT fileOp = new SHFILEOPSTRUCT();
            fileOp.pFrom = dir + '\0';  // pFrom must be double null terminated
            fileOp.wFunc = FO_Func.FO_DELETE;
            fileOp.fFlags = FILEOP_FLAGS_ENUM.FOF_NOCONFIRMATION |
                FILEOP_FLAGS_ENUM.FOF_NOERRORUI;
            int res = SHFileOperation(ref fileOp);
            if (res != 0) {
                throw new System.IO.IOException("Failed to delete dir " + res);
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHFileOperation([In, Out] ref SHFILEOPSTRUCT lpFileOp);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        struct SHFILEOPSTRUCT {
            public IntPtr hwnd;
            public FO_Func wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            public FILEOP_FLAGS_ENUM fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;

        }

        [Flags]
        private enum FILEOP_FLAGS_ENUM : ushort {
            FOF_MULTIDESTFILES = 0x0001,
            FOF_CONFIRMMOUSE = 0x0002,
            FOF_SILENT = 0x0004,  // don't create progress/report
            FOF_RENAMEONCOLLISION = 0x0008,
            FOF_NOCONFIRMATION = 0x0010,  // Don't prompt the user.
            FOF_WANTMAPPINGHANDLE = 0x0020,  // Fill in SHFILEOPSTRUCT.hNameMappings
            // Must be freed using SHFreeNameMappings
            FOF_ALLOWUNDO = 0x0040,
            FOF_FILESONLY = 0x0080,  // on *.*, do only files
            FOF_SIMPLEPROGRESS = 0x0100,  // means don't show names of files
            FOF_NOCONFIRMMKDIR = 0x0200,  // don't confirm making any needed dirs
            FOF_NOERRORUI = 0x0400,  // don't put up error UI
            FOF_NOCOPYSECURITYATTRIBS = 0x0800,  // dont copy NT file Security Attributes
            FOF_NORECURSION = 0x1000,  // don't recurse into directories.
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,  // don't operate on connected elements.
            FOF_WANTNUKEWARNING = 0x4000,  // during delete operation, warn if deleting instead of recycling (partially overrides FOF_NOCONFIRMATION)
            FOF_NORECURSEREPARSE = 0x8000,  // treat reparse points as objects, not containers
        }

        public enum FO_Func : uint {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004,
        }

        public const int MAX_PATH = 260;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);

        /// <summary>
        /// Use caution when using this directly as the errors it gives when you are not running as elevated are not
        //  very good.  Please use the Wrapper method.
        /// </summary>
        /// <param name="lpSymlinkFileName">Path to the symbolic link you wish to create.</param>
        /// <param name="lpTargetFileName">Path to the file/directory that the symbolic link should be pointing to.</param>
        /// <param name="dwFlags">Flag specifying either file or directory.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        internal static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        internal enum SymbolicLink {
            File = 0,
            Directory = 1
        }

        /// <summary>
        /// Wrapper for extern method CreateSymbolicLink.  This handles some error cases and provides better information
        /// in error cases.
        /// </summary>
        /// <param name="symlinkPath">Path to the symbolic link you wish to create.</param>
        /// <param name="targetPath">Path to the file/directory that the symbolic link should be pointing to.</param>
        public static void CreateSymbolicLink(string symlinkPath, string targetPath) {
            // Pre-checks.
            if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))) {
                throw new UnauthorizedAccessException("Process must be run in elevated permissions in order to create symbolic link.");
            } else if (Directory.Exists(symlinkPath) || File.Exists(symlinkPath)) {
                throw new IOException("Path Already Exists.  We cannot create a symbolic link here");
            }

            // Create the correct symbolic link.
            bool result;
            if (File.Exists(targetPath)) {
                result = CreateSymbolicLink(symlinkPath, targetPath, NativeMethods.SymbolicLink.File);
            } else if (Directory.Exists(targetPath)) {
                result = CreateSymbolicLink(symlinkPath, targetPath, NativeMethods.SymbolicLink.Directory);
            } else {
                throw new FileNotFoundException("Target File/Directory was not found.  Cannot make a symbolic link.");
            }

            // Validate that we created a symbolic link.
            // If we failed and the symlink doesn't exist throw an exception here.
            if (!result) {
                if (!Directory.Exists(symlinkPath) && !File.Exists(symlinkPath)) {
                    throw new FileNotFoundException("Unable to find symbolic link after creation.");
                } else {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
        }

        public const int OLECMDERR_E_NOTSUPPORTED = unchecked((int)0x80040100);
        public const int OLECMDERR_E_CANCELED = -2147221245;
        public const int OLECMDERR_E_UNKNOWNGROUP = unchecked((int)0x80040104);
    }
}
