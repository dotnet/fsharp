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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
#if NTVS_FEATURE_INTERACTIVEWINDOW
using Microsoft.NodejsTools.Repl;
#else
using Microsoft.VisualStudio.Repl;
#endif
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudioTools.VSTestHost;

namespace TestUtilities.UI {
    public class InteractiveWindow : EditorWindow {
        const string CommandBase = "PythonInteractive.";


        private sealed class ReplWindowInfo {
            public readonly ManualResetEvent Idle = new ManualResetEvent(false);
            public readonly ManualResetEvent ReadyForInput = new ManualResetEvent(false);

            public void OnReadyForInput() {
                Console.WriteLine("Ready for input");
                ReadyForInput.Set();
            }
        }

        private static ConditionalWeakTable<IReplWindow, ReplWindowInfo> _replWindows = new ConditionalWeakTable<IReplWindow, ReplWindowInfo>();

        private readonly VisualStudioApp _app;
        private readonly string _title;
        private readonly ReplWindow _replWindow;
        private readonly ReplWindowInfo _replWindowInfo;

        public InteractiveWindow(string title, AutomationElement element, VisualStudioApp app)
            : base(null, element) {
            _app = app;
            _title = title;

            var compModel = _app.GetService<IComponentModel>(typeof(SComponentModel));
            var replWindowProvider = compModel.GetService<IReplWindowProvider>();
            _replWindow = replWindowProvider.GetReplWindows()
                .OfType<ReplWindow>()
                .FirstOrDefault(p => p.Title.Equals(title, StringComparison.CurrentCulture));

            _replWindowInfo = _replWindows.GetValue(_replWindow, window => {
                var info = new ReplWindowInfo();
                window.ReadyForInput += new Action(info.OnReadyForInput);
                return info;
            });
        }

        public void Close() {
            var frame = _replWindow.Frame as IVsWindowFrame;
            if (frame != null) {
                frame.Hide();
            }
        }

        public static void CloseAll(VisualStudioApp app = null) {
            IComponentModel compModel;
            if (app != null) {
                compModel = app.GetService<IComponentModel>(typeof(SComponentModel));
            } else {
                compModel = (IComponentModel)VSTestContext.ServiceProvider.GetService(typeof(SComponentModel));
            }
            var replWindowProvider = compModel.GetService<IReplWindowProvider>();
            foreach (var frame in replWindowProvider.GetReplWindows()
                .OfType<ReplWindow>()
                .Select(r => r.Frame)
                .OfType<IVsWindowFrame>()) {
                frame.Hide();
            }
        }

        public void WaitForIdleState() {
            DispatchAndWait(_replWindowInfo.Idle, () => { }, DispatcherPriority.ApplicationIdle);
        }

        public void DispatchAndWait(EventWaitHandle waitHandle, Action action, DispatcherPriority priority = DispatcherPriority.Normal) {
            Dispatcher dispatcher = ((FrameworkElement)ReplWindow.TextView).Dispatcher;
            waitHandle.Reset();

            dispatcher.Invoke(new Action(() => {
                action();
                waitHandle.Set();
            }), priority);

            Assert.IsTrue(waitHandle.WaitOne(500));
        }

        public void WaitForText(params string[] text) {
            WaitForText((IList<string>)text);
        }

        public void WaitForText(IList<string> text) {
            string expected = null;
            for (int i = 0; i < 100; i++) {
                WaitForIdleState();
                expected = GetExpectedText(text);
                if (expected.Equals(Text, StringComparison.CurrentCulture)) {
                    return;
                }
                Thread.Sleep(100);
            }

            FailWrongText(expected);
        }

        public void WaitForTextContainsAll(params string[] substrings) {
            for (int i = 0; i < 100; ++i) {
                WaitForIdleState();
                var text = Text;
                if (substrings.All(s => text.Contains(s))) {
                    return;
                }
                Thread.Sleep(100);
            }

            FailWrongText("All of: " + string.Join(", ", substrings.Select(s => "<" + s + ">")));
        }

        public void WaitForTextIPython(params string[] text) {
            WaitForTextIPython((IList<string>)text);
        }

        public void WaitForTextIPython(IList<string> text) {
            string expected = null;
            for (int i = 0; i < 100; i++) {
                WaitForIdleState();
                expected = GetExpectedText(text);
                if (expected.Equals(GetIPythonText(), StringComparison.CurrentCulture)) {
                    return;
                }
                Thread.Sleep(100);
            }

            FailWrongTextIPython(expected);
        }

        private string GetIPythonText() {
            var text = Text;
            var lines = Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < lines.Length; i++) {
                var line = lines[i];

                if (!line.StartsWith("[IPKernelApp] ")) {
                    if (i != lines.Length - 1 || text.EndsWith("\r\n")) {
                        res.AppendLine(line);
                    } else {
                        res.Append(line);
                    }
                }
            }
            return res.ToString();
        }

        public void WaitForTextStartIPython(params string[] text) {
            string expected = GetExpectedText(text);

            for (int i = 0; i < 100; i++) {
                string curText = Text;

                if (GetIPythonText().StartsWith(expected, StringComparison.CurrentCulture)) {
                    return;
                }
                Thread.Sleep(100);
            }

            FailWrongText(expected);
        }

        private void FailWrongText(string expected) {
            StringBuilder msg = new StringBuilder("Did not get text: ");
            AppendRepr(msg, expected);
            msg.Append(" instead got ");
            AppendRepr(msg, Text);
            Assert.Fail(msg.ToString());
        }

        private void FailWrongTextIPython(string expected) {
            StringBuilder msg = new StringBuilder("Did not get text: ");
            AppendRepr(msg, expected);
            msg.Append(" instead got ");
            AppendRepr(msg, GetIPythonText());
            Assert.Fail(msg.ToString());
        }

        public void WaitForSessionDismissed() {
            var sessionStack = IntellisenseSessionStack;
            for (int i = 0; i < 20; i++) {
                if (sessionStack.TopSession == null) {
                    break;
                }
                System.Threading.Thread.Sleep(500);
            }

            while (sessionStack.TopSession != null) {
                sessionStack.TopSession.Dismiss();
            }
            Assert.AreEqual(null, sessionStack.TopSession);
        }

        public ManualResetEvent ReadyForInput {
            get {
                return _replWindowInfo.ReadyForInput;
            }
        }

        public void ClearInput() {
            var buffer = _replWindow.CurrentLanguageBuffer;
            if (buffer == null) {
                return;
            }

            var edit = buffer.CreateEdit();
            edit.Delete(0, edit.Snapshot.Length);
            edit.Apply();
        }

        public void ClearScreen(bool waitForReady = true) {
            Console.WriteLine("REPL Clearing screen");
            if (waitForReady) {
                ReadyForInput.Reset();
            }
            _app.ExecuteCommand(CommandBase + "ClearScreen");
            if (waitForReady) {
                Assert.IsTrue(ReadyForInput.WaitOne(1000));
            }
        }

        public void CancelExecution(int attempts = 100) {
            Console.WriteLine("REPL Cancelling Execution");
            ReadyForInput.Reset();
            for (int i = 0; i < attempts && !_replWindowInfo.ReadyForInput.WaitOne(0); i++) {
                ReadyForInput.Reset();
                try {
                    _app.ExecuteCommand(CommandBase + "CancelExecution");
                } catch {
                    // command may not be immediately available
                }
                if (ReadyForInput.WaitOne(1000)) {
                    break;
                }
            }
            Assert.IsTrue(ReadyForInput.WaitOne(10000));
        }

        internal IReplWindow2 ReplWindow {
            get {
                return _replWindow;
            }
        }

        public override IWpfTextView TextView {
            get {
                return ReplWindow.TextView;
            }
        }

        public void Reset() {
            Console.WriteLine("REPL resetting");

            Assert.IsTrue(ReplWindow.Reset().Wait(10000));
        }

        public void WithStandardInputPrompt(string prompt, Action<string> action) {
            if ((bool)ReplWindow.GetOptionValue(ReplOptions.DisplayPromptInMargin)) {
                action(String.Empty);
                return;
            }

            string oldPrompt = (string)ReplWindow.GetOptionValue(ReplOptions.StandardInputPrompt);
            ReplWindow.SetOptionValue(ReplOptions.StandardInputPrompt, prompt);
            try {
                action(prompt);
            } finally {
                ReplWindow.SetOptionValue(ReplOptions.StandardInputPrompt, oldPrompt);
            }
        }

        internal virtual bool IsTabGroupContainer(AutomationElement element) {
            var clsName = element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty) as string;
            return clsName == "ToolWindowTabGroupContainer" || clsName == "FloatingWindow";
        }
    }
}
