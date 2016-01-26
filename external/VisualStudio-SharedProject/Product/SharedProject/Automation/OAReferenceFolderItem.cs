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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudioTools.Project.Automation {
    /// <summary>
    /// Contains OAReferenceItem objects 
    /// </summary>
    [ComVisible(true), CLSCompliant(false)]
    public class OAReferenceFolderItem : OAProjectItem {
        #region ctors
        internal OAReferenceFolderItem(OAProject project, ReferenceContainerNode node)
            : base(project, node) {
        }

        #endregion

        private new ReferenceContainerNode Node {
            get {
                return (ReferenceContainerNode)base.Node;
            }
        }

        #region overridden methods
        /// <summary>
        /// Returns the project items collection of all the references defined for this project.
        /// </summary>
        public override EnvDTE.ProjectItems ProjectItems {
            get {
                return new OANavigableProjectItems(this.Project, this.Node);
            }
        }


        #endregion
    }
}
