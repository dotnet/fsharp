// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if UNUSED_DEPENDENT_FILES
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using MSBuild = Microsoft.Build.BuildEngine;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    /// <summary>
    /// Defines the logic for all dependent file nodes (solution explorer icon, commands etc.)
    /// </summary>
    [CLSCompliant(false)]
    [ComVisible(true)]
    public class DependentFileNode : FileNode
    {
        /// <summary>
        /// Defines if the node has a name relation to its parent node
        /// e.g. Form1.ext and Form1.resx are name related (until first occurence of extention separator)
        /// </summary>
        public override int ImageIndex
        {
            get { return (this.CanShowDefaultIcon() ? (int)ProjectNode.ImageName.DependentFile : (int) ProjectNode.ImageName.MissingFile); }
        }

        /// <summary>
        /// Constructor for the DependentFileNode
        /// </summary>
        /// <param name="root">Root of the hierarchy</param>
        /// <param name="e">Associated project element</param>
        internal DependentFileNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            this.HasParentNodeNameRelation = false;
        }


        /// <summary>
        /// Disable rename
        /// </summary>
        /// <param name="label">new label</param>
        /// <returns>E_NOTIMPLE in order to tell the call that we do not support rename</returns>
        public override string GetEditLabel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a handle to the icon that should be set for this node
        /// </summary>
        /// <param name="open">Whether the folder is open, ignored here.</param>
        /// <returns>Handle to icon for the node</returns>
        public override object GetIconHandle(bool open)
        {
            return this.ProjectMgr.ImageHandler.GetIconHandle(this.ImageIndex);
        }

        /// <summary>
        /// Disable certain commands for dependent file nodes 
        /// </summary>
        public /*protected, but public for FSharp.Project.dll*/ override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
        {
            if (cmdGroup == VsMenus.guidStandardCommandSet97)
            {
                switch ((VsCommands)cmd)
                {
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
            }
            else if (cmdGroup == VsMenus.guidStandardCommandSet2K)
            {
                if ((VsCommands2K)cmd == VsCommands2K.EXCLUDEFROMPROJECT)
                {
                    result |= QueryStatusResult.NOTSUPPORTED;
                    return VSConstants.S_OK;
                }
            }
            else
            {
                return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
            }
            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }

        /// <summary>
        /// DependentFileNodes node cannot be dragged.
        /// </summary>
        /// <returns>null</returns>
        public /*protected, but public for FSharp.Project.dll*/ override StringBuilder PrepareSelectedNodesForClipBoard()
        {
            return null;
        }

        public /*protected, but public for FSharp.Project.dll*/ override NodeProperties CreatePropertiesObject()
        {
            return new DependentFileNodeProperties(this);
        }

        /// <summary>
        /// Redraws the state icon if the node is not excluded from source control.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Scc")]
        public /*protected, but public for FSharp.Project.dll*/ override void UpdateSccStateIcons()
        {
            if (!this.ExcludeNodeFromScc)
            {
                this.Parent.ReDraw(UIHierarchyElement.SccState);
            }
        }
    }
}
#endif
