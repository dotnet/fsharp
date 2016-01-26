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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;

namespace Microsoft.VisualStudioTools.Project.Automation {
    /// <summary>
    /// Represents an automation object for a folder in a project
    /// </summary>
    [ComVisible(true)]
    public class OAFolderItem : OAProjectItem {
        #region ctors
        internal OAFolderItem(OAProject project, FolderNode node)
            : base(project, node) {
        }

        #endregion

        private new FolderNode Node {
            get {
                return (FolderNode)base.Node;
            }
        }


        #region overridden methods
        public override ProjectItems Collection {
            get {
                ProjectItems items = new OAProjectItems(this.Project, this.Node.Parent);
                return items;
            }
        }

        public override ProjectItems ProjectItems {
            get {
                return new OAProjectItems(Project, Node);
            }
        }
        #endregion
    }
}
