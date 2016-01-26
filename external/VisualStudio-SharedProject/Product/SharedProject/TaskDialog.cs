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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools {
    sealed class TaskDialog {
        private readonly IServiceProvider _provider;
        private readonly List<TaskDialogButton> _buttons;
        private readonly List<TaskDialogButton> _radioButtons;

        public TaskDialog(IServiceProvider provider) {
            _provider = provider;
            _buttons = new List<TaskDialogButton>();
            _radioButtons = new List<TaskDialogButton>();
            UseCommandLinks = true;
        }

        public static TaskDialog ForException(
            IServiceProvider provider,
            Exception exception,
            string message = null,
            string issueTrackerUrl = null
        ) {
            string suffix = string.IsNullOrEmpty(issueTrackerUrl) ?
                "Please press Ctrl+C to copy the contents of this dialog and report this error." :
                "Please press Ctrl+C to copy the contents of this dialog and report this error to our <a href=\"issuetracker\">issue tracker</a>.";

            if (string.IsNullOrEmpty(message)) {
                message = suffix;
            } else {
                message += Environment.NewLine + Environment.NewLine + suffix;
            }
            
            var td = new TaskDialog(provider) {
                MainInstruction = "An unexpected error occurred",
                Content = message,
                EnableHyperlinks = true,
                CollapsedControlText = "Show &details",
                ExpandedControlText = "Hide &details",
                ExpandedInformation = exception.ToString()
            };
            td.Buttons.Add(TaskDialogButton.Close);
            if (!string.IsNullOrEmpty(issueTrackerUrl)) {
                td.HyperlinkClicked += (s, e) => {
                    if (e.Url == "issuetracker") {
                        Process.Start(issueTrackerUrl);
                    }
                };
            }
            return td;
        }

        public static void CallWithRetry(
            Action<int> action,
            IServiceProvider provider,
            string title,
            string failedText,
            string expandControlText,
            string retryButtonText,
            string cancelButtonText,
            Func<Exception, bool> canRetry = null
        ) {
            for (int retryCount = 1; ; ++retryCount) {
                try {
                    action(retryCount);
                    return;
                } catch (Exception ex) {
                    if (ex.IsCriticalException()) {
                        throw;
                    }
                    if (canRetry != null && !canRetry(ex)) {
                        throw;
                    }

                    var td = new TaskDialog(provider) {
                        Title = title,
                        MainInstruction = failedText,
                        Content = ex.Message,
                        CollapsedControlText = expandControlText,
                        ExpandedControlText = expandControlText,
                        ExpandedInformation = ex.ToString()
                    };
                    var retry = new TaskDialogButton(retryButtonText);
                    td.Buttons.Add(retry);
                    td.Buttons.Add(new TaskDialogButton(cancelButtonText));
                    var button = td.ShowModal();
                    if (button != retry) {
                        throw new OperationCanceledException();
                    }
                }
            }
        }

        public static T CallWithRetry<T>(
            Func<int, T> func,
            IServiceProvider provider,
            string title,
            string failedText,
            string expandControlText,
            string retryButtonText,
            string cancelButtonText,
            Func<Exception, bool> canRetry = null
        ) {
            for (int retryCount = 1; ; ++retryCount) {
                try {
                    return func(retryCount);
                } catch (Exception ex) {
                    if (ex.IsCriticalException()) {
                        throw;
                    }
                    if (canRetry != null && !canRetry(ex)) {
                        throw;
                    }

                    var td = new TaskDialog(provider) {
                        Title = title,
                        MainInstruction = failedText,
                        Content = ex.Message,
                        CollapsedControlText = expandControlText,
                        ExpandedControlText = expandControlText,
                        ExpandedInformation = ex.ToString()
                    };
                    var retry = new TaskDialogButton(retryButtonText);
                    var cancel = new TaskDialogButton(cancelButtonText);
                    td.Buttons.Add(retry);
                    td.Buttons.Add(cancel);
                    var button = td.ShowModal();
                    if (button == cancel) {
                        throw new OperationCanceledException();
                    }
                }
            }
        }

        public TaskDialogButton ShowModal() {
            var config = new NativeMethods.TASKDIALOGCONFIG();
            config.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.TASKDIALOGCONFIG));
            config.pButtons = IntPtr.Zero;
            config.pRadioButtons = IntPtr.Zero;

            var uiShell = (IVsUIShell)_provider.GetService(typeof(SVsUIShell));
            uiShell.GetDialogOwnerHwnd(out config.hwndParent);
            uiShell.EnableModeless(0);

            var customButtons = new List<TaskDialogButton>();
            config.dwCommonButtons = 0;

            foreach (var button in Buttons) {
                var flag = GetButtonFlag(button);
                if (flag != 0) {
                    config.dwCommonButtons |= flag;
                } else {
                    customButtons.Add(button);
                }
            }

            try {
                if (customButtons.Any()) {
                    config.cButtons = (uint)customButtons.Count;
                    var ptr = config.pButtons = Marshal.AllocHGlobal(customButtons.Count * Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON)));
                    for (int i = 0; i < customButtons.Count; ++i) {
                        NativeMethods.TASKDIALOG_BUTTON data;
                        data.nButtonID = GetButtonId(null, null, i);
                        if (string.IsNullOrEmpty(customButtons[i].Subtext)) {
                            data.pszButtonText = customButtons[i].Text;
                        } else {
                            data.pszButtonText = string.Format("{0}\n{1}", customButtons[i].Text, customButtons[i].Subtext);
                        }
                        Marshal.StructureToPtr(data, ptr + i * Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON)), false);
                    }
                } else {
                    config.cButtons = 0;
                    config.pButtons = IntPtr.Zero;
                }

                if (_buttons.Any() && SelectedButton != null) {
                    config.nDefaultButton = GetButtonId(SelectedButton, customButtons);
                } else {
                    config.nDefaultButton = 0;
                }

                if (_radioButtons.Any()) {
                    config.cRadioButtons = (uint)_radioButtons.Count;
                    var ptr = config.pRadioButtons = Marshal.AllocHGlobal(_radioButtons.Count * Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON)));
                    for (int i = 0; i < _radioButtons.Count; ++i) {
                        NativeMethods.TASKDIALOG_BUTTON data;
                        data.nButtonID = GetRadioId(null, null, i);
                        data.pszButtonText = _radioButtons[i].Text;
                        Marshal.StructureToPtr(data, ptr + i * Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON)), false);
                    }

                    if (SelectedRadioButton != null) {
                        config.nDefaultRadioButton = GetRadioId(SelectedRadioButton, _radioButtons);
                    } else {
                        config.nDefaultRadioButton = 0;
                    }
                }

                config.pszWindowTitle = Title;
                config.pszMainInstruction = MainInstruction;
                config.pszContent = Content;
                config.pszExpandedInformation = ExpandedInformation;
                config.pszExpandedControlText = ExpandedControlText;
                config.pszCollapsedControlText = CollapsedControlText;
                config.pszFooter = Footer;
                config.pszVerificationText = VerificationText;
                config.pfCallback = Callback;
                config.hMainIcon = (IntPtr)GetIconResource(MainIcon);
                config.hFooterIcon = (IntPtr)GetIconResource(FooterIcon);

                if (Width.HasValue) {
                    config.cxWidth = (uint)Width.Value;
                } else {
                    config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_SIZE_TO_CONTENT;
                }
                if (EnableHyperlinks) {
                    config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_ENABLE_HYPERLINKS;
                }
                if (AllowCancellation) {
                    config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION;
                }
                if (UseCommandLinks && config.cButtons > 0) {
                    config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS;
                }
                if (!ShowExpandedInformationInContent) {
                    config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_EXPAND_FOOTER_AREA;
                }
                if (ExpandedByDefault) {
                    config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_EXPANDED_BY_DEFAULT;
                }
                if (SelectedVerified) {
                    config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_VERIFICATION_FLAG_CHECKED;
                }
                if (CanMinimize) {
                    config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_CAN_BE_MINIMIZED;
                }

                config.dwFlags |= NativeMethods.TASKDIALOG_FLAGS.TDF_POSITION_RELATIVE_TO_WINDOW;

                int selectedButton, selectedRadioButton;
                bool verified;
                ErrorHandler.ThrowOnFailure(NativeMethods.TaskDialogIndirect(
                    ref config,
                    out selectedButton,
                    out selectedRadioButton,
                    out verified
                ));

                SelectedButton = GetButton(selectedButton, customButtons);
                SelectedRadioButton = GetRadio(selectedRadioButton, _radioButtons);
                SelectedVerified = verified;
            } finally {
                uiShell.EnableModeless(1);

                if (config.pButtons != IntPtr.Zero) {
                    for (int i = 0; i < customButtons.Count; ++i) {
                        Marshal.DestroyStructure(config.pButtons + i * Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON)), typeof(NativeMethods.TASKDIALOG_BUTTON));
                    }
                    Marshal.FreeHGlobal(config.pButtons);
                }
                if (config.pRadioButtons != IntPtr.Zero) {
                    for (int i = 0; i < _radioButtons.Count; ++i) {
                        Marshal.DestroyStructure(config.pRadioButtons + i * Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON)), typeof(NativeMethods.TASKDIALOG_BUTTON));
                    }
                    Marshal.FreeHGlobal(config.pRadioButtons);
                }
            }

            return SelectedButton;
        }

        private int Callback(IntPtr hwnd, uint uNotification, UIntPtr wParam, IntPtr lParam, IntPtr lpRefData) {
            try {
                switch ((NativeMethods.TASKDIALOG_NOTIFICATION)uNotification) {
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_CREATED:
                        foreach (var btn in _buttons.Where(b => b.ElevationRequired)) {
                            NativeMethods.SendMessage(
                                hwnd,
                                (int)NativeMethods.TASKDIALOG_MESSAGE.TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE,
                                new IntPtr(GetButtonId(btn, _buttons)),
                                new IntPtr(1)
                            );
                        }
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_NAVIGATED:
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_BUTTON_CLICKED:
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_HYPERLINK_CLICKED:
                        var url = Marshal.PtrToStringUni(lParam);
                        var hevt = HyperlinkClicked;
                        if (hevt != null) {
                            hevt(this, new TaskDialogHyperlinkClickedEventArgs(url));
                        } else {
                            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                        }
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_TIMER:
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_DESTROYED:
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_RADIO_BUTTON_CLICKED:
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_DIALOG_CONSTRUCTED:
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_VERIFICATION_CLICKED:
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_HELP:
                        break;
                    case NativeMethods.TASKDIALOG_NOTIFICATION.TDN_EXPANDO_BUTTON_CLICKED:
                        break;
                    default:
                        break;
                }
                return VSConstants.S_OK;
            } catch (Exception ex) {
                if (ex.IsCriticalException()) {
                    throw;
                }
                return Marshal.GetHRForException(ex);
            }
        }

        public string Title { get; set; }
        public string MainInstruction { get; set; }
        public string Content { get; set; }
        public string VerificationText { get; set; }
        public string ExpandedInformation { get; set; }
        public string Footer { get; set; }

        public bool ExpandedByDefault { get; set; }
        public bool ShowExpandedInformationInContent { get; set; }
        public string ExpandedControlText { get; set; }
        public string CollapsedControlText { get; set; }

        public int? Width { get; set; }
        public bool EnableHyperlinks { get; set; }
        public bool AllowCancellation { get; set; }
        public bool UseCommandLinks { get; set; }
        public bool CanMinimize { get; set; }

        public TaskDialogIcon MainIcon { get; set; }
        public TaskDialogIcon FooterIcon { get; set; }

        /// <summary>
        /// Raised when a hyperlink in the dialog is clicked. If no event
        /// handlers are added, the default behavior is to open an external
        /// browser.
        /// </summary>
        public event EventHandler<TaskDialogHyperlinkClickedEventArgs> HyperlinkClicked;

        public List<TaskDialogButton> Buttons {
            get {
                return _buttons;
            }
        }

        public List<TaskDialogButton> RadioButtons {
            get {
                return _radioButtons;
            }
        }

        public TaskDialogButton SelectedButton { get; set; }
        public TaskDialogButton SelectedRadioButton { get; set; }
        public bool SelectedVerified { get; set; }


        private static NativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS GetButtonFlag(TaskDialogButton button) {
            if (button == TaskDialogButton.OK) {
                return NativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON;
            } else if (button == TaskDialogButton.Cancel) {
                return NativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CANCEL_BUTTON;
            } else if (button == TaskDialogButton.Yes) {
                return NativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_YES_BUTTON;
            } else if (button == TaskDialogButton.No) {
                return NativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_NO_BUTTON;
            } else if (button == TaskDialogButton.Retry) {
                return NativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_RETRY_BUTTON;
            } else if (button == TaskDialogButton.Close) {
                return NativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CLOSE_BUTTON;
            } else {
                return 0;
            }
        }

        private static NativeMethods.TASKDIALOG_ICON GetIconResource(TaskDialogIcon icon) {
            switch (icon) {
                case TaskDialogIcon.None:
                    return 0;
                case TaskDialogIcon.Error:
                    return NativeMethods.TASKDIALOG_ICON.TD_ERROR_ICON;
                case TaskDialogIcon.Warning:
                    return NativeMethods.TASKDIALOG_ICON.TD_WARNING_ICON;
                case TaskDialogIcon.Information:
                    return NativeMethods.TASKDIALOG_ICON.TD_INFORMATION_ICON;
                case TaskDialogIcon.Shield:
                    return NativeMethods.TASKDIALOG_ICON.TD_SHIELD_ICON;
                default:
                    throw new ArgumentException("Invalid TaskDialogIcon value", "icon");
            }
        }

        private static int GetButtonId(
            TaskDialogButton button,
            IList<TaskDialogButton> customButtons = null,
            int indexHint = -1
        ) {
            if (indexHint >= 0) {
                return indexHint + 1000;
            }

            if (button == TaskDialogButton.OK) {
                return NativeMethods.IDOK;
            } else if (button == TaskDialogButton.Cancel) {
                return NativeMethods.IDCANCEL;
            } else if (button == TaskDialogButton.Yes) {
                return NativeMethods.IDYES;
            } else if (button == TaskDialogButton.No) {
                return NativeMethods.IDNO;
            } else if (button == TaskDialogButton.Retry) {
                return NativeMethods.IDRETRY;
            } else if (button == TaskDialogButton.Close) {
                return NativeMethods.IDCLOSE;
            } else if (customButtons != null) {
                int i = customButtons.IndexOf(button);
                if (i >= 0) {
                    return i + 1000;
                }
            }

            return -1;
        }

        private static TaskDialogButton GetButton(int id, IList<TaskDialogButton> customButtons = null) {
            switch (id) {
                case NativeMethods.IDOK:
                    return TaskDialogButton.OK;
                case NativeMethods.IDCANCEL:
                    return TaskDialogButton.Cancel;
                case NativeMethods.IDYES:
                    return TaskDialogButton.Yes;
                case NativeMethods.IDNO:
                    return TaskDialogButton.No;
                case NativeMethods.IDRETRY:
                    return TaskDialogButton.Retry;
                case NativeMethods.IDCLOSE:
                    return TaskDialogButton.Close;
            }

            if (customButtons != null && id >= 1000 && id - 1000 < customButtons.Count) {
                return customButtons[id - 1000];
            }

            return null;
        }

        private static int GetRadioId(
            TaskDialogButton button,
            IList<TaskDialogButton> buttons,
            int indexHint = -1
        ) {
            if (indexHint >= 0) {
                return indexHint + 2000;
            }

            return buttons.IndexOf(button) + 2000;
        }

        private static TaskDialogButton GetRadio(int id, IList<TaskDialogButton> buttons) {
            if (id >= 2000 && id - 2000 < buttons.Count) {
                return buttons[id - 2000];
            }

            return null;
        }

        private static class NativeMethods {
            internal const int IDOK = 1;
            internal const int IDCANCEL = 2;
            internal const int IDABORT = 3;
            internal const int IDRETRY = 4;
            internal const int IDIGNORE = 5;
            internal const int IDYES = 6;
            internal const int IDNO = 7;
            internal const int IDCLOSE = 8;

            internal enum TASKDIALOG_FLAGS {
                TDF_ENABLE_HYPERLINKS = 0x0001,
                TDF_USE_HICON_MAIN = 0x0002,
                TDF_USE_HICON_FOOTER = 0x0004,
                TDF_ALLOW_DIALOG_CANCELLATION = 0x0008,
                TDF_USE_COMMAND_LINKS = 0x0010,
                TDF_USE_COMMAND_LINKS_NO_ICON = 0x0020,
                TDF_EXPAND_FOOTER_AREA = 0x0040,
                TDF_EXPANDED_BY_DEFAULT = 0x0080,
                TDF_VERIFICATION_FLAG_CHECKED = 0x0100,
                TDF_SHOW_PROGRESS_BAR = 0x0200,
                TDF_SHOW_MARQUEE_PROGRESS_BAR = 0x0400,
                TDF_CALLBACK_TIMER = 0x0800,
                TDF_POSITION_RELATIVE_TO_WINDOW = 0x1000,
                TDF_RTL_LAYOUT = 0x2000,
                TDF_NO_DEFAULT_RADIO_BUTTON = 0x4000,
                TDF_CAN_BE_MINIMIZED = 0x8000,
                TDF_SIZE_TO_CONTENT = 0x01000000
            }

            internal enum TASKDIALOG_COMMON_BUTTON_FLAGS {
                TDCBF_OK_BUTTON = 0x0001,
                TDCBF_YES_BUTTON = 0x0002,
                TDCBF_NO_BUTTON = 0x0004,
                TDCBF_CANCEL_BUTTON = 0x0008,
                TDCBF_RETRY_BUTTON = 0x0010,
                TDCBF_CLOSE_BUTTON = 0x0020
            }

            internal enum TASKDIALOG_NOTIFICATION : uint {
                TDN_CREATED = 0,
                TDN_NAVIGATED = 1,
                TDN_BUTTON_CLICKED = 2,     // wParam = Button ID
                TDN_HYPERLINK_CLICKED = 3,  // lParam = (LPCWSTR)pszHREF
                TDN_TIMER = 4,              // wParam = Milliseconds since dialog created or timer reset
                TDN_DESTROYED = 5,
                TDN_RADIO_BUTTON_CLICKED = 6,   // wParam = Radio Button ID
                TDN_DIALOG_CONSTRUCTED = 7,
                TDN_VERIFICATION_CLICKED = 8,   // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
                TDN_HELP = 9,
                TDN_EXPANDO_BUTTON_CLICKED = 10 // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
            };

            internal enum TASKDIALOG_ICON : ushort {
              TD_WARNING_ICON = unchecked((ushort)-1),
              TD_ERROR_ICON = unchecked((ushort)-2),
              TD_INFORMATION_ICON = unchecked((ushort)-3),
              TD_SHIELD_ICON = unchecked((ushort)-4)
            }

            const int WM_USER = 0x0400;

            internal enum TASKDIALOG_MESSAGE : int {
                TDM_NAVIGATE_PAGE = WM_USER + 101,
                TDM_CLICK_BUTTON = WM_USER + 102, // wParam = Button ID
                TDM_SET_MARQUEE_PROGRESS_BAR = WM_USER + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
                TDM_SET_PROGRESS_BAR_STATE = WM_USER + 104, // wParam = new progress state
                TDM_SET_PROGRESS_BAR_RANGE = WM_USER + 105, // lParam = MAKELPARAM(nMinRange, nMaxRange)
                TDM_SET_PROGRESS_BAR_POS = WM_USER + 106, // wParam = new position
                TDM_SET_PROGRESS_BAR_MARQUEE = WM_USER + 107, // wParam = 0 (stop marquee), wParam != 0 (start marquee), lparam = speed (milliseconds between repaints)
                TDM_SET_ELEMENT_TEXT = WM_USER + 108, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
                TDM_CLICK_RADIO_BUTTON = WM_USER + 110, // wParam = Radio Button ID
                TDM_ENABLE_BUTTON = WM_USER + 111, // lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID
                TDM_ENABLE_RADIO_BUTTON = WM_USER + 112, // lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID
                TDM_CLICK_VERIFICATION = WM_USER + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
                TDM_UPDATE_ELEMENT_TEXT = WM_USER + 114, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
                TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE = WM_USER + 115, // wParam = Button ID, lParam = 0 (elevation not required), lParam != 0 (elevation required)
                TDM_UPDATE_ICON = WM_USER + 116  // wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)
            }

            [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist",
                Justification = "Entry point exists but CA can't find it")]
            [DllImport("comctl32.dll", SetLastError = true)]
            internal static extern int TaskDialogIndirect(
                ref TASKDIALOGCONFIG pTaskConfig,
                out int pnButton,
                out int pnRadioButton,
                [MarshalAs(UnmanagedType.Bool)] out bool pfverificationFlagChecked);

            internal delegate int PFTASKDIALOGCALLBACK(IntPtr hwnd, uint uNotification, UIntPtr wParam, IntPtr lParam, IntPtr lpRefData);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct TASKDIALOG_BUTTON {
                public int nButtonID;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszButtonText;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct TASKDIALOGCONFIG {
                public uint cbSize;
                public IntPtr hwndParent;
                public IntPtr hInstance;
                public TASKDIALOG_FLAGS dwFlags;
                public TASKDIALOG_COMMON_BUTTON_FLAGS dwCommonButtons;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszWindowTitle;
                public IntPtr hMainIcon;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszMainInstruction;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszContent;
                public uint cButtons;
                public IntPtr pButtons;
                public int nDefaultButton;
                public uint cRadioButtons;
                public IntPtr pRadioButtons;
                public int nDefaultRadioButton;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszVerificationText;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszExpandedInformation;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszExpandedControlText;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszCollapsedControlText;
                public IntPtr hFooterIcon;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszFooter;
                public PFTASKDIALOGCALLBACK pfCallback;
                public IntPtr lpCallbackData;
                public uint cxWidth;
            }

            [DllImport("user32.dll")]
            internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        }
    }

    class TaskDialogButton {
        public TaskDialogButton(string text) {
            int i = text.IndexOfAny(Environment.NewLine.ToCharArray());
            if (i < 0) {
                Text = text;
            } else {
                Text = text.Remove(i);
                Subtext = text.Substring(i).TrimStart();
            }
        }

        public TaskDialogButton(string text, string subtext) {
            Text = text;
            Subtext = subtext;
        }

        public string Text { get; set; }
        public string Subtext { get; set; }
        public bool ElevationRequired { get; set; }

        private TaskDialogButton() { }
        public static readonly TaskDialogButton OK = new TaskDialogButton();
        public static readonly TaskDialogButton Cancel = new TaskDialogButton();
        public static readonly TaskDialogButton Yes = new TaskDialogButton();
        public static readonly TaskDialogButton No = new TaskDialogButton();
        public static readonly TaskDialogButton Retry = new TaskDialogButton();
        public static readonly TaskDialogButton Close = new TaskDialogButton();
    }

    sealed class TaskDialogHyperlinkClickedEventArgs : EventArgs {
        private readonly string _url;

        public TaskDialogHyperlinkClickedEventArgs(string url) {
            _url = url;
        }

        public string Url { get { return _url; } }
    }

    enum TaskDialogIcon {
        None,
        Error,
        Warning,
        Information,
        Shield
    }
}