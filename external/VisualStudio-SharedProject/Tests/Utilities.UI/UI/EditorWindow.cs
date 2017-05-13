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
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudioTools.VSTestHost;
using Task = System.Threading.Tasks.Task;

namespace TestUtilities.UI {
    public class EditorWindow : AutomationWrapper, IEditor {
        private readonly string _filename;

        public EditorWindow(string filename, AutomationElement element)
            : base(element) {
            _filename = filename;
        }

        public string Text {
            get {
                return GetValue();
            }
        }

        public virtual IWpfTextView TextView {
            get {
                return GetTextView(_filename);
            }
        }

        public void MoveCaret(SnapshotPoint newPoint) {
            Invoke((Action)(() => {
                TextView.Caret.MoveTo(newPoint.TranslateTo(newPoint.Snapshot.TextBuffer.CurrentSnapshot, PointTrackingMode.Positive));
            }));
        }

        public void Select(int line, int column, int length) {
            var textLine = TextView.TextViewLines[line - 1];
            Span span;
            if (column - 1 == textLine.Length) {
                span = new Span(textLine.End, length);
            } else {
                span = new Span(textLine.Start + column - 1, length);
            }

            ((UIElement)TextView).Dispatcher.Invoke((Action)(() => {
                TextView.Selection.Select(
                    new SnapshotSpan(TextView.TextBuffer.CurrentSnapshot, span),
                    false
                );
            }));
        }

        /// <summary>
        /// Moves the caret to the 1 based line and column
        /// </summary>
        public void MoveCaret(int line, int column) {
            var textLine = TextView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(line - 1);
            if (column - 1 == textLine.Length) {
                MoveCaret(textLine.End);
            } else {
                MoveCaret(new SnapshotPoint(TextView.TextBuffer.CurrentSnapshot, textLine.Start + column - 1));
            }
        }

        public void WaitForText(string text) {
            for (int i = 0; i < 100; i++) {
                if (Text != text) {
                    System.Threading.Thread.Sleep(100);
                } else {
                    break;
                }
            }

            Assert.AreEqual(text, Text);
        }

        public void WaitForTextStart(params string[] text) {
            string expected = GetExpectedText(text);

            for (int i = 0; i < 100; i++) {
                string curText = Text;

                if (Text.StartsWith(expected, StringComparison.CurrentCulture)) {
                    return;
                }
                Thread.Sleep(100);
            }

            FailWrongText(expected);
        }

        public void WaitForTextEnd(params string[] text) {
            string expected = GetExpectedText(text);

            for (int i = 0; i < 100; i++) {
                string curText = Text.TrimEnd();

                if (Text.EndsWith(expected, StringComparison.CurrentCulture)) {
                    return;
                }
                Thread.Sleep(100);
            }

            FailWrongText(expected);
        }

        public static string GetExpectedText(IList<string> text) {
            StringBuilder finalString = new StringBuilder();
            for (int i = 0; i < text.Count; i++) {
                if (i != 0) {
                    finalString.Append(Environment.NewLine);
                }

                finalString.Append(text[i]);
            }

            string expected = finalString.ToString();
            return expected;
        }

        private void FailWrongText(string expected) {
            StringBuilder msg = new StringBuilder("Did not get text: <");
            AppendRepr(msg, expected);
            msg.Append("> instead got <");
            AppendRepr(msg, Text);
            msg.Append(">");
            Assert.Fail(msg.ToString());
        }

        public static void AppendRepr(StringBuilder msg, string str) {
            for (int i = 0; i < str.Length; i++) {
                if (str[i] >= 32) {
                    msg.Append(str[i]);
                } else {
                    switch (str[i]) {
                        case '\n': msg.Append("\\n"); break;

                        case '\r': msg.Append("\\r"); break;
                        case '\t': msg.Append("\\t"); break;
                        default: msg.AppendFormat("\\u00{0:D2}", (int)str[i]); break;
                    }
                }
            }
        }
#if DEV14_OR_LATER
#pragma warning disable 0618
#endif
        public void StartSmartTagSessionNoSession() {
            ShowSmartTag();
            Thread.Sleep(100);
            Assert.IsNotInstanceOfType(
                IntellisenseSessionStack.TopSession,
#if DEV14_OR_LATER
                typeof(ILightBulbSession)
#else
                typeof(ISmartTagSession)
#endif
            );
        }

        private static void ShowSmartTag() {
            Task.Run(() => {
                for (int i = 0; i < 40; i++) {
                    try {
                        VSTestContext.DTE.ExecuteCommand("View.ShowSmartTag");
                        break;
                    } catch {
                        Thread.Sleep(250);
                    }
                }
            }).Wait();
        }

        public SessionHolder<SmartTagSessionWrapper> StartSmartTagSession() {
            ShowSmartTag();
#if DEV14_OR_LATER
            var sh = WaitForSession<ILightBulbSession>();
#else
            var sh = WaitForSession<ISmartTagSession>();
#endif
            return sh == null ? null : new SessionHolder<SmartTagSessionWrapper>(new SmartTagSessionWrapper(sh), this);
        }

        public SessionHolder<T> WaitForSession<T>() where T : IIntellisenseSession {
            return WaitForSession<T>(true);
        }

        public SessionHolder<T> WaitForSession<T>(bool assertIfNoSession) where T : IIntellisenseSession {
            var sessionStack = IntellisenseSessionStack;
            for (int i = 0; i < 40; i++) {
                if (sessionStack.TopSession is T) {
                    break;
                }
                System.Threading.Thread.Sleep(250);
            }

            if (!(sessionStack.TopSession is T)) {
                if (assertIfNoSession) {
                    Console.WriteLine("Buffer text:\r\n{0}", Text);
                    Console.WriteLine("-----");
                    AutomationWrapper.DumpVS();
                    Assert.Fail("failed to find session " + typeof(T).FullName);
                } else {
                    return null;
                }
            }
            return new SessionHolder<T>((T)sessionStack.TopSession, this);
        }

        public IIntellisenseSessionStack IntellisenseSessionStack {
            get {
                var compModel = (IComponentModel)VSTestContext.ServiceProvider.GetService(typeof(SComponentModel));
                var stackMapService = compModel.GetService<IIntellisenseSessionStackMapService>();

                return stackMapService.GetStackForTextView(TextView);
            }
        }

        public void AssertNoIntellisenseSession() {
            Thread.Sleep(500);
            Assert.IsNull(IntellisenseSessionStack.TopSession);
        }

        public IClassifier Classifier {
            get {

                var compModel = (IComponentModel)VSTestContext.ServiceProvider.GetService(typeof(SComponentModel));

                var provider = compModel.GetService<IClassifierAggregatorService>();
                return provider.GetClassifier(TextView.TextBuffer);
            }
        }

        public ITagAggregator<T> GetTaggerAggregator<T>(ITextBuffer buffer) where T : ITag {
            var compModel = (IComponentModel)VSTestContext.ServiceProvider.GetService(typeof(SComponentModel));

            return compModel.GetService<Microsoft.VisualStudio.Text.Tagging.IBufferTagAggregatorFactoryService>().CreateTagAggregator<T>(buffer);
        }

        internal static IWpfTextView GetTextView(string filePath) {
            IVsUIHierarchy uiHierarchy;
            uint itemID;
            IVsWindowFrame windowFrame;

            if (VsShellUtilities.IsDocumentOpen(VSTestContext.ServiceProvider, filePath, Guid.Empty, out uiHierarchy, out itemID, out windowFrame)) {
                var textView = VsShellUtilities.GetTextView(windowFrame);
                IComponentModel compModel = (IComponentModel)VSTestContext.ServiceProvider.GetService(typeof(SComponentModel));
                var adapterFact = compModel.GetService<IVsEditorAdaptersFactoryService>();
                return adapterFact.GetWpfTextView(textView);
            }

            return null;
        }

        public void Invoke(Action action) {
            ExceptionDispatchInfo excep = null;
            ((UIElement)TextView).Dispatcher.Invoke(
                (Action)(() => {
                    try {
                        action();
                    } catch (Exception e) {
                        excep = ExceptionDispatchInfo.Capture(e);

                    }
                })
            );

            if (excep != null) {
                excep.Throw();
            }
        }

        public T Invoke<T>(Func<T> action) {
            Exception excep = null;
            T res = default(T);
            ((UIElement)TextView).Dispatcher.Invoke(
                (Action)(() => {
                    try {
                        res = action();
                    } catch (Exception e) {
                        excep = e;

                    }
                })
            );

            if (excep != null) {
                Assert.Fail("Exception on UI thread: " + excep.ToString());
            }
            return res;
        }

        public IIntellisenseSession TopSession {
            get { return IntellisenseSessionStack.TopSession;  }
        }

        public void Type(string text) {
            Keyboard.Type(text);
        }
    }
}
