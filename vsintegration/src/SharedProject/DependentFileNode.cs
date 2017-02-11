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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
#if DEV14_OR_LATER
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
#endif

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Defines the logic for all dependent file nodes (solution explorer icon, commands etc.)
    /// </summary>

    internal class DependentFileNode : FileNode {
        #region fields
        /// <summary>
        /// Defines if the node has a name relation to its parent node
        /// e.g. Form1.ext and Form1.resx are name related (until first occurence of extention separator)
        /// </summary>
        #endregion

        #region ctor
        /// <summary>
        /// Constructor for the DependentFileNode
        /// </summary>
        /// <param name="root">Root of the hierarchy</param>
        /// <param name="e">Associated project element</param>
        internal DependentFileNode(ProjectNode root, MsBuildProjectElement element)
            : base(root, element) {
            this.HasParentNodeNameRelation = false;
        }


        #endregion

        #region overridden methods
        /// <summary>
        /// Disable rename
        /// </summary>
        /// <param name="label">new label</param>
        /// <returns>E_NOTIMPL in order to tell the call that we do not support rename</returns>
        public override string GetEditLabel() {
            throw new NotImplementedException();
        }

#if DEV14_OR_LATER
        protected override bool SupportsIconMonikers {
            get { return true; }
        }

        protected override ImageMoniker GetIconMoniker(bool open) {
            return CanShowDefaultIcon() ?
                // TODO: Check this image
                KnownMonikers.ReferencedElement :
                KnownMonikers.DocumentWarning;
        }
#else
        public override int ImageIndex {
            get { return (this.CanShowDefaultIcon() ? (int)ProjectNode.ImageName.DependentFile : (int)ProjectNode.ImageName.MissingFile); }
        }

#endif

        /// <summary>
        /// Disable certain commands for dependent file nodes 
        /// </summary>
        internal override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result) {
            if (cmdGroup == VsMenus.guidStandardCommandSet97) {
                switch ((VsCommands)cmd) {
                    case VsCommands.Copy:
                    case VsCommands.Paste:
                    case VsCommands.Cut:
                    case VsCommands.Rename:
                        result |= QueryStatusResult.NOTSUPPORTED;
                        return VSConstants.S_OK;

                    case VsCommands.ViewCode:
                    case VsCommands.Open:
                    case VsCommands.OpenWith:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;
                }
            } else if (cmdGroup == VsMenus.guidStandardCommandSet2K) {
                if ((VsCommands2K)cmd == VsCommands2K.EXCLUDEFROMPROJECT) {
                    result |= QueryStatusResult.NOTSUPPORTED;
                    return VSConstants.S_OK;
                }
            } else {
                return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
            }
            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }

        /// <summary>
        /// DependentFileNodes node cannot be dragged.
        /// </summary>
        /// <returns>null</returns>
        protected internal override string PrepareSelectedNodesForClipBoard() {
            return null;
        }

        protected override NodeProperties CreatePropertiesObject() {
            return new DependentFileNodeProperties(this);
        }

        /// <summary>
        /// Redraws the state icon if the node is not excluded from source control.
        /// </summary>
        protected internal override void UpdateSccStateIcons() {
            if (!this.ExcludeNodeFromScc) {
                ProjectMgr.ReDrawNode(this.Parent, UIHierarchyElement.SccState);
            }
        }

        public override int QueryService(ref Guid guidService, out object result) {
            //
            // If you have a code dom provider you'd provide it here.
            // if (guidService == typeof(SVSMDCodeDomProvider).GUID) {
            // }

            return base.QueryService(ref guidService, out result);
        }

        internal override FileNode RenameFileNode(string oldFileName, string newFileName) {
            return this.RenameFileNode(oldFileName, newFileName, Parent);
        }

        #endregion

    }
}
