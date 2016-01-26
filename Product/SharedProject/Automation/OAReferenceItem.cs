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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudioTools.Project.Automation {
    /// <summary>
    /// Represents the automation object equivalent to a ReferenceNode object
    /// </summary>
    [ComVisible(true), CLSCompliant(false)]
    public class OAReferenceItem : OAProjectItem {
        #region ctors
        internal OAReferenceItem(OAProject project, ReferenceNode node)
            : base(project, node) {
        }

        #endregion

        private new ReferenceNode Node {
            get {
                return (ReferenceNode)base.Node;
            }
        }

        #region overridden methods
        /// <summary>
        /// Not implemented. If called throws invalid operation exception.
        /// </summary>
        public override void Delete() {
            throw new InvalidOperationException();
        }


        /// <summary>
        /// Not implemented. If called throws invalid operation exception.
        /// </summary>
        /// <param name="viewKind"> A Constants. vsViewKind indicating the type of view to use.</param>
        /// <returns></returns>
        public override EnvDTE.Window Open(string viewKind) {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public override string Name {
            get {
                return base.Name;
            }
            set {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the ProjectItems collection containing the ProjectItem object supporting this property.
        /// </summary>
        public override EnvDTE.ProjectItems Collection {
            get {
                // Get the parent node (ReferenceContainerNode)
                ReferenceContainerNode parentNode = this.Node.Parent as ReferenceContainerNode;
                Debug.Assert(parentNode != null, "Failed to get the parent node");

                // Get the ProjectItems object for the parent node
                if (parentNode != null) {
                    // The root node for the project
                    return ((OAReferenceFolderItem)parentNode.GetAutomationObject()).ProjectItems;
                }

                return null;
            }
        }
        #endregion
    }
}
