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

namespace Microsoft.VisualStudioTools.Project {
    public sealed class DirtyChangedEventArgs : EventArgs {
        private readonly bool _isDirty;
        public static readonly DirtyChangedEventArgs DirtyValue = new DirtyChangedEventArgs(true);
        public static readonly DirtyChangedEventArgs SavedValue = new DirtyChangedEventArgs(false);

        public DirtyChangedEventArgs(bool isDirty) {
            _isDirty = isDirty;
        }

        public bool IsDirty {
            get {
                return _isDirty;
            }
        }
    }
}
