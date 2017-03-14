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

using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudioTools.Navigation {
    /// <summary>
    /// Class storing the data about a parsing task on a language module.
    /// A module in dynamic languages is a source file, so here we use the file name to
    /// identify it.
    /// </summary>
    public class LibraryTask {
        private string _fileName;
        private ITextBuffer _textBuffer;
        private ModuleId _moduleId;

        public LibraryTask(string fileName, ITextBuffer textBuffer, ModuleId moduleId) {
            _fileName = fileName;
            _textBuffer = textBuffer;
            _moduleId = moduleId;
        }

        public string FileName {
            get { return _fileName; }
        }

        public ModuleId ModuleID {
            get { return _moduleId; }
            set { _moduleId = value; }
        }

        public ITextBuffer TextBuffer {
            get { return _textBuffer; }
        }
    }

}
