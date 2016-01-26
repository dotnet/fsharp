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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
#if DEV14_OR_LATER
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
#endif

namespace Microsoft.VisualStudioTools.Project {
    internal abstract class ReferenceNode : HierarchyNode {
        internal delegate void CannotAddReferenceErrorMessage();

        #region ctors
        /// <summary>
        /// constructor for the ReferenceNode
        /// </summary>
        protected ReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element) {
            this.ExcludeNodeFromScc = true;
        }

        /// <summary>
        /// constructor for the ReferenceNode
        /// </summary>
        internal ReferenceNode(ProjectNode root)
            : base(root) {
            this.ExcludeNodeFromScc = true;
        }

        #endregion

        #region overridden properties
        public override int MenuCommandId {
            get { return VsMenus.IDM_VS_CTXT_REFERENCE; }
        }

        public override Guid ItemTypeGuid {
            get { return Guid.Empty; }
        }

        public override string Url {
            get {
                return String.Empty;
            }
        }

        public override string Caption {
            get {
                return String.Empty;
            }
        }
        #endregion

        #region overridden methods
        protected override NodeProperties CreatePropertiesObject() {
            return new ReferenceNodeProperties(this);
        }

        /// <summary>
        /// Get an instance of the automation object for ReferenceNode
        /// </summary>
        /// <returns>An instance of Automation.OAReferenceItem type if succeeded</returns>
        public override object GetAutomationObject() {
            if (this.ProjectMgr == null || this.ProjectMgr.IsClosed) {
                return null;
            }

            return new Automation.OAReferenceItem(this.ProjectMgr.GetAutomationObject() as Automation.OAProject, this);
        }

        /// <summary>
        /// Disable inline editing of Caption of a ReferendeNode
        /// </summary>
        /// <returns>null</returns>
        public override string GetEditLabel() {
            return null;
        }


#if DEV14_OR_LATER
        protected override bool SupportsIconMonikers {
            get { return true; }
        }

        protected override ImageMoniker GetIconMoniker(bool open) {
            return CanShowDefaultIcon() ? KnownMonikers.Reference : KnownMonikers.ReferenceWarning;
        }
#else
        public override int ImageIndex {
            get {
                return ProjectMgr.GetIconIndex(CanShowDefaultIcon() ?
                    ProjectNode.ImageName.Reference :
                    ProjectNode.ImageName.DanglingReference
                );
            }
        }
#endif

        /// <summary>
        /// Not supported.
        /// </summary>
        internal override int ExcludeFromProject() {
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        public override void Remove(bool removeFromStorage) {
            ReferenceContainerNode parent = Parent as ReferenceContainerNode;
            base.Remove(removeFromStorage);
            if (parent != null) {
                parent.FireChildRemoved(this);
            }
        }

        /// <summary>
        /// References node cannot be dragged.
        /// </summary>
        /// <returns>A stringbuilder.</returns>
        protected internal override string PrepareSelectedNodesForClipBoard() {
            return null;
        }

        internal override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result) {
            if (cmdGroup == VsMenus.guidStandardCommandSet2K) {
                if ((VsCommands2K)cmd == VsCommands2K.QUICKOBJECTSEARCH) {
                    result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                    return VSConstants.S_OK;
                }
            } else {
                return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
            }
            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }

        internal override int ExecCommandOnNode(Guid cmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            if (cmdGroup == VsMenus.guidStandardCommandSet2K) {
                if ((VsCommands2K)cmd == VsCommands2K.QUICKOBJECTSEARCH) {
                    return this.ShowObjectBrowser();
                }
            }

            return base.ExecCommandOnNode(cmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);
        }

        protected internal override void ShowDeleteMessage(IList<HierarchyNode> nodes, __VSDELETEITEMOPERATION action, out bool cancel, out bool useStandardDialog) {
            // Don't prompt if all the nodes are references
            useStandardDialog = !nodes.All(n => n is ReferenceNode);
            cancel = false;
        }

        #endregion

        #region  methods


        /// <summary>
        /// Links a reference node to the project and hierarchy.
        /// </summary>
        public virtual void AddReference() {
            ProjectMgr.Site.GetUIThread().MustBeCalledFromUIThread();

            ReferenceContainerNode referencesFolder = this.ProjectMgr.GetReferenceContainer() as ReferenceContainerNode;
            Utilities.CheckNotNull(referencesFolder, "Could not find the References node");

            CannotAddReferenceErrorMessage referenceErrorMessageHandler = null;

            if (!this.CanAddReference(out referenceErrorMessageHandler)) {
                if (referenceErrorMessageHandler != null) {
                    referenceErrorMessageHandler.DynamicInvoke(new object[] { });
                }
                return;
            }

            // Link the node to the project file.
            this.BindReferenceData();

            // At this point force the item to be refreshed
            this.ItemNode.RefreshProperties();

            referencesFolder.AddChild(this);

            return;
        }

        /// <summary>
        /// Refreshes a reference by re-resolving it and redrawing the icon.
        /// </summary>
        internal virtual void RefreshReference() {
            this.ResolveReference();
            ProjectMgr.ReDrawNode(this, UIHierarchyElement.Icon);
        }

        /// <summary>
        /// Resolves references.
        /// </summary>
        protected virtual void ResolveReference() {

        }

        /// <summary>
        /// Validates that a reference can be added.
        /// </summary>
        /// <param name="errorHandler">A CannotAddReferenceErrorMessage delegate to show the error message.</param>
        /// <returns>true if the reference can be added.</returns>
        protected virtual bool CanAddReference(out CannotAddReferenceErrorMessage errorHandler) {
            // When this method is called this refererence has not yet been added to the hierarchy, only instantiated.
            errorHandler = null;
            if (this.IsAlreadyAdded()) {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Checks if a reference is already added. The method parses all references and compares the Url.
        /// </summary>
        /// <returns>true if the assembly has already been added.</returns>
        protected virtual bool IsAlreadyAdded() {
            ReferenceContainerNode referencesFolder = this.ProjectMgr.GetReferenceContainer() as ReferenceContainerNode;
            Utilities.CheckNotNull(referencesFolder, "Could not find the References node");

            for (HierarchyNode n = referencesFolder.FirstChild; n != null; n = n.NextSibling) {
                ReferenceNode refererenceNode = n as ReferenceNode;
                if (null != refererenceNode) {
                    // We check if the Url of the assemblies is the same.
                    if (CommonUtils.IsSamePath(refererenceNode.Url, this.Url)) {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Shows the Object Browser
        /// </summary>
        /// <returns></returns>
        protected virtual int ShowObjectBrowser() {
            if (!File.Exists(this.Url)) {
                return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
            }

            // Request unmanaged code permission in order to be able to create the unmanaged memory representing the guid.
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

            Guid guid = VSConstants.guidCOMPLUSLibrary;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(guid.ToByteArray().Length);

            System.Runtime.InteropServices.Marshal.StructureToPtr(guid, ptr, false);
            int returnValue = VSConstants.S_OK;
            try {
                VSOBJECTINFO[] objInfo = new VSOBJECTINFO[1];

                objInfo[0].pguidLib = ptr;
                objInfo[0].pszLibName = this.Url;

                IVsObjBrowser objBrowser = this.ProjectMgr.Site.GetService(typeof(SVsObjBrowser)) as IVsObjBrowser;

                ErrorHandler.ThrowOnFailure(objBrowser.NavigateTo(objInfo, 0));
            } catch (COMException e) {
                Trace.WriteLine("Exception" + e.ErrorCode);
                returnValue = e.ErrorCode;
            } finally {
                if (ptr != IntPtr.Zero) {
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
                }
            }

            return returnValue;
        }

        internal override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation) {
            if (deleteOperation == __VSDELETEITEMOPERATION.DELITEMOP_RemoveFromProject) {
                return true;
            }
            return false;
        }

        protected abstract void BindReferenceData();

        #endregion
    }
}
