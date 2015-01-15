// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using FSSafe = Internal.Utilities.FileSystem;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using System.Security.Permissions;


namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    [CLSCompliant(false), ComVisible(true)]
    public abstract class ReferenceNode : HierarchyNode
    {
        public /*protected, but public for FSharp.Project.dll*/ delegate void CannotAddReferenceErrorMessage();

        #region ctors
        /// <summary>
        /// constructor for the ReferenceNode
        /// </summary>
        internal ReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            this.ExcludeNodeFromScc = true;
        }

        /// <summary>
        /// constructor for the ReferenceNode
        /// </summary>
        internal ReferenceNode(ProjectNode root)
            : base(root)
        {
            this.ExcludeNodeFromScc = true;
        }

        #endregion

        #region overridden properties
        public override int MenuCommandId
        {
            get { return VsMenus.IDM_VS_CTXT_REFERENCE; }
        }

        public override Guid ItemTypeGuid
        {
            get { return Guid.Empty; }
        }

        public override string Url
        {
            get
            {
                return String.Empty;
            }
        }

        public override string Caption
        {
            get
            {
                return String.Empty;
            }
        }
        #endregion

        #region overridden methods
        public /*protected, but public for FSharp.Project.dll*/ override NodeProperties CreatePropertiesObject()
        {
            return new ReferenceNodeProperties(this);
        }

        /// <summary>
        /// Get an instance of the automation object for ReferenceNode
        /// </summary>
        /// <returns>An instance of Automation.OAReferenceItem type if succeeded</returns>
        public override object GetAutomationObject()
        {
            if (this.ProjectMgr == null || this.ProjectMgr.IsClosed)
            {
                return null;
            }

            return new Automation.OAReferenceItem(this.ProjectMgr.GetAutomationObject() as Automation.OAProject, this);
        }

        /// <summary>
        /// Disable inline editing of Caption of a ReferendeNode
        /// </summary>
        /// <returns>null</returns>
        public override string GetEditLabel()
        {
            return null;
        }


        public override object GetIconHandle(bool open)
        {
            int offset = (this.CanShowDefaultIcon() ? (int)ProjectNode.ImageName.Reference : (int)ProjectNode.ImageName.DanglingReference);
            return this.ProjectMgr.ImageHandler.GetIconHandle(offset);
        }

        /// <summary>
        /// This method is called by the interface method GetMkDocument to specify the item moniker.
        /// </summary>
        /// <returns>The moniker for this item</returns>
        public override string GetMkDocument()
        {
            return this.Url;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public /*protected, but public for FSharp.Project.dll*/ override int ExcludeFromProject()
        {
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        /// <summary>
        /// References node cannot be dragged.
        /// </summary>
        /// <returns>A stringbuilder.</returns>
        public /*protected, but public for FSharp.Project.dll*/ override StringBuilder PrepareSelectedNodesForClipBoard()
        {
            return null;
        }

        public /*protected, but public for FSharp.Project.dll*/ override void DoDefaultAction()
        {
            this.ShowObjectBrowser();
        }

        public virtual bool CanBeReferencedFromFSI()
        {
            return false;
        }

        public virtual string GetReferenceForFSI()
        {
            return null;
        }

        internal override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
        {
            if (cmdGroup == VsMenus.guidStandardCommandSet2K)
            {
                if ((VsCommands2K)cmd == VsCommands2K.QUICKOBJECTSEARCH)
                {
                    result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                    return VSConstants.S_OK;
                }
            }
            else if (cmdGroup == VSProjectConstants.FSharpSendThisReferenceToInteractiveCmd.Guid && cmd == VSProjectConstants.FSharpSendThisReferenceToInteractiveCmd.ID)
            {
                if (CanBeReferencedFromFSI())
                {
                    result |= QueryStatusResult.SUPPORTED; 
                    if (GetReferenceForFSI() != null)
                    {
                        result |= QueryStatusResult.ENABLED;
                    }
                }
                
                return VSConstants.S_OK;
            }
            else
            {
                return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
            }
            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }

        public /*protected, but public for FSharp.Project.dll*/ override int ExecCommandOnNode(Guid cmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (cmdGroup == VsMenus.guidStandardCommandSet2K)
            {
                if ((VsCommands2K)cmd == VsCommands2K.QUICKOBJECTSEARCH)
                {
                    return this.ShowObjectBrowser();
                }
            }
            else if (cmdGroup == VSProjectConstants.FSharpSendThisReferenceToInteractiveCmd.Guid && cmd == VSProjectConstants.FSharpSendThisReferenceToInteractiveCmd.ID)
            {
                var reference = GetReferenceForFSI();
                if (reference != null)
                {
                    ProjectMgr.SendReferencesToFSI(new[] { reference });
                }
                return VSConstants.S_OK;
            }
            return base.ExecCommandOnNode(cmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);

        }

        #endregion

        #region  methods

        /// <summary>
        /// If this is a managed assembly that has been resolved, its simple name.  Else if this is project reference, the filename (sans path/extension).  Else string.Empty.
        /// This is only used in the "IsAlreadyAdded" logic to prevent adding two references to same-named assemblies via the VS UI.
        /// </summary>
        public virtual string SimpleName
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Links a reference node to the project and hierarchy.  Returns true if succeeds, false otherwise.
        /// </summary>
        public virtual bool AddReference()
        {
            ReferenceContainerNode referencesFolder = this.ProjectMgr.FindChild(ReferenceContainerNode.ReferencesNodeVirtualName) as ReferenceContainerNode;
            Debug.Assert(referencesFolder != null, "Could not find the References node");


            var checkResult = CheckIfCanAddReference();
            if (!checkResult.Ok)
            {
                if (!string.IsNullOrEmpty(checkResult.Message))
                {
                    throw new InvalidOperationException(checkResult.Message);
                }
                return false;
            }

            // Link the node to the project file.
            this.BindReferenceData();

            // At this point force the item to be refreshed
            this.ItemNode.RefreshProperties();

            referencesFolder.AddChild(this);

            return true;
        }

        /// <summary>
        /// Refreshes a reference by re-resolving it and redrawing the icon.
        /// </summary>
        /*internal, but public for FSharp.Project.dll*/
        internal virtual void RefreshReference(BuildResult buildResult)
        {
            this.ResolveReference(buildResult);
            this.ReDraw(UIHierarchyElement.Icon);
        }

        /// <summary>
        /// Resolves references.
        /// </summary>
        internal /*protected, but public for FSharp.Project.dll*/ virtual void ResolveReference(BuildResult buildResult)
        {

        }

        /// <summary>
        /// Validates that a reference can be added.
        /// </summary>
        /// <returns>Success if the reference can be added.</returns>
        internal /*protected, but public for FSharp.Project.dll*/ virtual AddReferenceCheckResult CheckIfCanAddReference()
        {
            // When this method is called this refererence has not yet been added to the hierarchy, only instantiated.
            ReferenceNode existingNode;
            if (this.IsAlreadyAdded(out existingNode))
            {
                return AddReferenceCheckResult.Failed();
            }

            return AddReferenceCheckResult.Success;
        }


        /// <summary>
        /// Checks if a reference is already added. The method parses all references and compares the Url.
        /// </summary>
        /// <returns>true if the assembly has already been added.</returns>
        public /*protected, but public for FSharp.Project.dll*/ virtual bool IsAlreadyAdded(out ReferenceNode existingNode)
        {
            ReferenceContainerNode referencesFolder = this.ProjectMgr.FindChild(ReferenceContainerNode.ReferencesNodeVirtualName) as ReferenceContainerNode;
            Debug.Assert(referencesFolder != null, "Could not find the References node");

            for (HierarchyNode n = referencesFolder.FirstChild; n != null; n = n.NextSibling)
            {
                ReferenceNode referenceNode = n as ReferenceNode;
                if (null != referenceNode)
                {
                    if (!string.IsNullOrEmpty(referenceNode.SimpleName) && 0==string.CompareOrdinal(referenceNode.SimpleName, this.SimpleName))
                    {
                        existingNode = referenceNode;
                        return true;
                    }
                }
            }

            existingNode = null;
            return false;
        }

        /// <summary>
        /// Gets the Guid to use to set VSOJBECTINFO.pguidLib for the call to IVsObjBrowser.NavigateTo
        /// </summary>
        public /*protected, but public for FSharp.Project.dll*/ virtual Guid GetBrowseLibraryGuid()
        {
            return Guid.Empty;
        }

        protected virtual bool CanShowUrlInOnObjectBrowser()
        {
            return !string.IsNullOrEmpty(Url) && FSSafe.File.SafeExists(Url);
        }

        /// <summary>
        /// Shows the Object Browser
        /// </summary>
        /// <returns></returns>
        public /*protected, but public for FSharp.Project.dll*/ virtual int ShowObjectBrowser()
        {
            if (!CanShowUrlInOnObjectBrowser())
            {
                return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
            }

            // Request unmanaged code permission in order to be able to creaet the unmanaged memory representing the guid.
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

            Guid guid = GetBrowseLibraryGuid();
            IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(guid.ToByteArray().Length);
            System.Runtime.InteropServices.Marshal.StructureToPtr(guid, ptr, false);

            int returnValue = VSConstants.S_OK;
            try
            {
                VSOBJECTINFO[] objInfo = new VSOBJECTINFO[1];

                objInfo[0].pguidLib = ptr;
                objInfo[0].pszLibName = this.Url;

                IVsObjBrowser objBrowser = this.ProjectMgr.Site.GetService(typeof(SVsObjBrowser)) as IVsObjBrowser;

                ErrorHandler.ThrowOnFailure(objBrowser.NavigateTo(objInfo, 0));
            }
            catch (COMException e)
            {
                Trace.WriteLine("Exception" + e.ErrorCode);
                returnValue = e.ErrorCode;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
                }
            }

            return returnValue;
        }

        public /*protected, but public for FSharp.Project.dll*/ override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation)
        {
            if (deleteOperation == __VSDELETEITEMOPERATION.DELITEMOP_RemoveFromProject)
            {
                return true;
            }
            return false;
        }

        public /*protected, but public for FSharp.Project.dll*/ abstract void BindReferenceData();

        #endregion

    }

    internal class AddReferenceCheckResult
    {
        public static readonly AddReferenceCheckResult Success = new AddReferenceCheckResult(true, null);

        public bool Ok { get; private set; }
        public string Message { get; private set; }

        private AddReferenceCheckResult(bool ok, string message)
        {
            Ok = ok;
            Message = message;
        }

        public static AddReferenceCheckResult Failed(string message = null)
        {
            return new AddReferenceCheckResult(false, message);
        }
    }
}
