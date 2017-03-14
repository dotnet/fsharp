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
    internal partial class ProjectNode {
        public event EventHandler<ProjectPropertyChangedArgs> OnProjectPropertyChanged;

        protected virtual void RaiseProjectPropertyChanged(string propertyName, string oldValue, string newValue) {
            var onPropChanged = OnProjectPropertyChanged;
            if (onPropChanged != null) {
                onPropChanged(this, new ProjectPropertyChangedArgs(propertyName, oldValue, newValue));
            }
        }
    }
}
