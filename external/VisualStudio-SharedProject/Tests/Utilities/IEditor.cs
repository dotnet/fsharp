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
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

namespace TestUtilities {
    public interface IEditor {
        IIntellisenseSession TopSession {
            get;
        }
        string Text {
            get;
        }
        void Type(string text);

        void Invoke(Action action);

        void MoveCaret(int line, int column);
        void SetFocus();

        IWpfTextView TextView {
            get;
        }

        IClassifier Classifier {
            get;
        }

        void WaitForText(string text);
        void Select(int line, int column, int length);

        SessionHolder<T> WaitForSession<T>() where T : IIntellisenseSession;
        SessionHolder<T> WaitForSession<T>(bool assertIfNoSession) where T : IIntellisenseSession;

        void AssertNoIntellisenseSession();
    }
}
