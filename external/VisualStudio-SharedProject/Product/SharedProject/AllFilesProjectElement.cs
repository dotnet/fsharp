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


namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Represents a project element which lives on disk and is visible when Show All Files
    /// is enabled.
    /// </summary>
    sealed class AllFilesProjectElement : VirtualProjectElement {
        private string _itemType;

        public AllFilesProjectElement(string path, string itemType, CommonProjectNode project)
            : base(project) {
            Rename(path);
        }

        public override bool IsExcluded {
            get {
                return true;
            }
        }

        public new CommonProjectNode ItemProject {
            get {
                return (CommonProjectNode)base.ItemProject;
            }
        }

        protected override string ItemType {
            get {
                return _itemType;
            }
            set {
                _itemType = value;
                OnItemTypeChanged();
            }
        }
    }
}
