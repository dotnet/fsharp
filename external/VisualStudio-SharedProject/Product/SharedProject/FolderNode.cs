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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
#if DEV14_OR_LATER
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
#endif

namespace Microsoft.VisualStudioTools.Project {

    internal class FolderNode : HierarchyNode, IDiskBasedNode {
        #region ctors
        /// <summary>
        /// Constructor for the FolderNode
        /// </summary>
        /// <param name="root">Root node of the hierarchy</param>
        /// <param name="relativePath">relative path from root i.e.: "NewFolder1\\NewFolder2\\NewFolder3</param>
        /// <param name="element">Associated project element</param>
        public FolderNode(ProjectNode root, ProjectElement element)
            : base(root, element) {
        }
        #endregion

        #region overridden properties
        public override bool CanOpenCommandPrompt {
            get {
                return true;
            }
        }

        internal override string FullPathToChildren {
            get {
                return Url;
            }
        }

        public override int SortPriority {
            get { return DefaultSortOrderNode.FolderNode; }
        }

        /// <summary>
        /// This relates to the SCC glyph
        /// </summary>
        public override VsStateIcon StateIconIndex {
            get {
                // The SCC manager does not support being asked for the state icon of a folder (result of the operation is undefined)
                return VsStateIcon.STATEICON_NOSTATEICON;
            }
        }

        public override bool CanAddFiles {
            get {
                return true;
            }
        }

        #endregion

        #region overridden methods
        protected override NodeProperties CreatePropertiesObject() {
            return new FolderNodeProperties(this);
        }

        protected internal override void DeleteFromStorage(string path) {
            this.DeleteFolder(path);
        }

        /// <summary>
        /// Get the automation object for the FolderNode
        /// </summary>
        /// <returns>An instance of the Automation.OAFolderNode type if succeeded</returns>
        public override object GetAutomationObject() {
            if (this.ProjectMgr == null || this.ProjectMgr.IsClosed) {
                return null;
            }

            return new Automation.OAFolderItem(this.ProjectMgr.GetAutomationObject() as Automation.OAProject, this);
        }

#if DEV14_OR_LATER
        protected override bool SupportsIconMonikers {
            get { return true; }
        }

        protected override ImageMoniker GetIconMoniker(bool open) {
            return open ? KnownMonikers.FolderOpened : KnownMonikers.FolderClosed;
        }
#else
        public override object GetIconHandle(bool open) {
            return ProjectMgr.GetIconHandleByName(open ?
                ProjectNode.ImageName.OpenFolder :
                ProjectNode.ImageName.Folder
            );
        }
#endif

        /// <summary>
        /// Rename Folder
        /// </summary>
        /// <param name="label">new Name of Folder</param>
        /// <returns>VSConstants.S_OK, if succeeded</returns>
        public override int SetEditLabel(string label) {
            if (IsBeingCreated) {
                return FinishFolderAdd(label, false);
            } else {
                if (String.Equals(CommonUtils.GetFileOrDirectoryName(Url), label, StringComparison.Ordinal)) {
                    // Label matches current Name
                    return VSConstants.S_OK;
                }

                string newPath = CommonUtils.GetAbsoluteDirectoryPath(CommonUtils.GetParent(Url), label);

                // Verify that No Directory/file already exists with the new name among current children
                var existingChild = Parent.FindImmediateChildByName(label);
                if (existingChild != null && existingChild != this) {
                    return ShowFileOrFolderAlreadyExistsErrorMessage(newPath);
                }

                // Verify that No Directory/file already exists with the new name on disk.
                // Unless the path exists because it is the path to the source file also.
                if ((Directory.Exists(newPath) || File.Exists(newPath)) && !CommonUtils.IsSamePath(Url, newPath)) {
                    return ShowFileOrFolderAlreadyExistsErrorMessage(newPath);
                }

                if (!ProjectMgr.Tracker.CanRenameItem(Url, newPath, VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_Directory)) {
                    return VSConstants.S_OK;
                }
            }

            try {
                var oldTriggerFlag = this.ProjectMgr.EventTriggeringFlag;
                ProjectMgr.EventTriggeringFlag |= ProjectNode.EventTriggering.DoNotTriggerTrackerQueryEvents;
                try {
                    RenameFolder(label);
                } finally {
                    ProjectMgr.EventTriggeringFlag = oldTriggerFlag;
                }


                //Refresh the properties in the properties window
                IVsUIShell shell = this.ProjectMgr.GetService(typeof(SVsUIShell)) as IVsUIShell;
                Utilities.CheckNotNull(shell, "Could not get the UI shell from the project");
                ErrorHandler.ThrowOnFailure(shell.RefreshPropertyBrowser(0));

                // Notify the listeners that the name of this folder is changed. This will
                // also force a refresh of the SolutionExplorer's node.
                ProjectMgr.OnPropertyChanged(this, (int)__VSHPROPID.VSHPROPID_Caption, 0);
            } catch (Exception e) {
                if (e.IsCriticalException()) {
                    throw;
                }
                throw new InvalidOperationException(SR.GetString(SR.RenameFolder, e.Message));
            }
            return VSConstants.S_OK;
        }

        internal static string PathTooLongMessage {
            get {
                return SR.GetString(SR.PathTooLongShortMessage);
            }
        }

        private int FinishFolderAdd(string label, bool wasCancelled) {
            // finish creation
            string filename = label.Trim();
            if (filename == "." || filename == "..") {
                Debug.Assert(!wasCancelled);   // cancelling leaves us with a valid label
                NativeMethods.SetErrorDescription("{0} is an invalid filename.", filename);
                return VSConstants.E_FAIL;
            }

            var path = Path.Combine(Parent.FullPathToChildren, label);
            if (path.Length >= NativeMethods.MAX_FOLDER_PATH) {
                if (wasCancelled) {
                    // cancelling an edit label doesn't result in the error
                    // being displayed, so we'll display one for the user.
                    Utilities.ShowMessageBox(
                        ProjectMgr.Site,
                        null,
                        PathTooLongMessage,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                    );
                } else {
                    NativeMethods.SetErrorDescription(PathTooLongMessage);
                }
                return VSConstants.E_FAIL;
            }

            if (filename == Caption || Parent.FindImmediateChildByName(filename) == null) {
                if (ProjectMgr.QueryFolderAdd(Parent, path)) {
                    Directory.CreateDirectory(path);
                    IsBeingCreated = false;
                    var relativePath = CommonUtils.GetRelativeDirectoryPath(
                        ProjectMgr.ProjectHome,
                        CommonUtils.GetAbsoluteDirectoryPath(CommonUtils.GetParent(Url), label)
                    );
                    this.ItemNode.Rename(relativePath);

                    ProjectMgr.OnItemDeleted(this);
                    this.Parent.RemoveChild(this);
                    ProjectMgr.Site.GetUIThread().MustBeCalledFromUIThread();
                    this.ID = ProjectMgr.ItemIdMap.Add(this);
                    this.Parent.AddChild(this);

                    ExpandItem(EXPANDFLAGS.EXPF_SelectItem);

                    ProjectMgr.Tracker.OnFolderAdded(
                        path,
                        VSADDDIRECTORYFLAGS.VSADDDIRECTORYFLAGS_NoFlags
                    );
                }
            } else {
                Debug.Assert(!wasCancelled);    // we choose a label which didn't exist when we started the edit
                // Set error: folder already exists
                NativeMethods.SetErrorDescription("The folder {0} already exists.", filename);
                return VSConstants.E_FAIL;
            }
            return VSConstants.S_OK;
        }

        public override int MenuCommandId {
            get { return VsMenus.IDM_VS_CTXT_FOLDERNODE; }
        }

        public override Guid ItemTypeGuid {
            get {
                return VSConstants.GUID_ItemType_PhysicalFolder;
            }
        }

        public override string Url {
            get {
                return CommonUtils.EnsureEndSeparator(ItemNode.Url);
            }
        }

        public override string Caption {
            get {
                // it might have a backslash at the end... 
                // and it might consist of Grandparent\parent\this\
                return CommonUtils.GetFileOrDirectoryName(Url);
            }
        }

        public override string GetMkDocument() {
            Debug.Assert(!string.IsNullOrEmpty(this.Url), "No url specified for this node");
            Debug.Assert(Path.IsPathRooted(this.Url), "Url should not be a relative path");

            return this.Url;
        }

        /// <summary>
        /// Recursively walks the folder nodes and redraws the state icons
        /// </summary>
        protected internal override void UpdateSccStateIcons() {
            for (HierarchyNode child = this.FirstChild; child != null; child = child.NextSibling) {
                child.UpdateSccStateIcons();
            }
        }

        internal override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result) {
            if (cmdGroup == VsMenus.guidStandardCommandSet97) {
                switch ((VsCommands)cmd) {
                    case VsCommands.Copy:
                    case VsCommands.Paste:
                    case VsCommands.Cut:
                    case VsCommands.Rename:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;

                    case VsCommands.NewFolder:
                        if (!IsNonMemberItem) {
                            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                            return VSConstants.S_OK;
                        }
                        break;
                }
            } else if (cmdGroup == VsMenus.guidStandardCommandSet2K) {
                if ((VsCommands2K)cmd == VsCommands2K.EXCLUDEFROMPROJECT) {
                    result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                    return VSConstants.S_OK;
                }
            } else if (cmdGroup != ProjectMgr.SharedCommandGuid) {
                return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
            }
            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }

        internal override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation) {
            if (deleteOperation == __VSDELETEITEMOPERATION.DELITEMOP_DeleteFromStorage) {
                return this.ProjectMgr.CanProjectDeleteItems;
            }
            return false;
        }

        protected internal override void GetSccFiles(IList<string> files, IList<tagVsSccFilesFlags> flags) {
            for (HierarchyNode n = this.FirstChild; n != null; n = n.NextSibling) {
                n.GetSccFiles(files, flags);
            }
        }

        protected internal override void GetSccSpecialFiles(string sccFile, IList<string> files, IList<tagVsSccFilesFlags> flags) {
            for (HierarchyNode n = this.FirstChild; n != null; n = n.NextSibling) {
                n.GetSccSpecialFiles(sccFile, files, flags);
            }
        }

        #endregion

        #region virtual methods
        /// <summary>
        /// Override if your node is not a file system folder so that
        /// it does nothing or it deletes it from your storage location.
        /// </summary>
        /// <param name="path">Path to the folder to delete</param>
        public virtual void DeleteFolder(string path) {
            if (Directory.Exists(path)) {
                try {
                    try {
                        Directory.Delete(path, true);
                    } catch (UnauthorizedAccessException) {
                        // probably one or more files are read only
                        foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)) {
                            // We will ignore all exceptions here and rethrow when
                            // we retry the Directory.Delete.
                            try {
                                File.SetAttributes(file, FileAttributes.Normal);
                            } catch (UnauthorizedAccessException) {
                            } catch (IOException) {
                            }
                        }
                        Directory.Delete(path, true);
                    }
                } catch (IOException ioEx) {
                    // re-throw with a friendly path
                    throw new IOException(ioEx.Message.Replace(path, Caption));
                }
            }
        }

        /// <summary>
        /// creates the physical directory for a folder node
        /// Override if your node does not use file system folder
        /// </summary>
        public virtual void CreateDirectory() {
            if (Directory.Exists(this.Url) == false) {
                Directory.CreateDirectory(this.Url);
            }
        }
        /// <summary>
        /// Creates a folder nodes physical directory
        /// Override if your node does not use file system folder
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public virtual void CreateDirectory(string newName) {
            if (String.IsNullOrEmpty(newName)) {
                throw new ArgumentException(SR.GetString(SR.ParameterCannotBeNullOrEmpty), "newName");
            }

            // on a new dir && enter, we get called with the same name (so do nothing if name is the same
            string strNewDir = CommonUtils.GetAbsoluteDirectoryPath(CommonUtils.GetParent(Url), newName);

            if (!CommonUtils.IsSameDirectory(Url, strNewDir)) {
                if (Directory.Exists(strNewDir)) {
                    throw new InvalidOperationException(SR.GetString(SR.DirectoryExistsShortMessage));
                }
                Directory.CreateDirectory(strNewDir);
            }
        }

        /// <summary>
        /// Rename the physical directory for a folder node
        /// Override if your node does not use file system folder
        /// </summary>
        /// <returns></returns>
        public virtual void RenameDirectory(string newPath) {
            if (Directory.Exists(this.Url)) {
                if (CommonUtils.IsSamePath(this.Url, newPath)) {
                    // This is a rename to the same location with (possible) capitilization changes.
                    // Directory.Move does not allow renaming to the same name so P/Invoke MoveFile to bypass this.
                    if (!NativeMethods.MoveFile(this.Url, newPath)) {
                        // Rather than perform error handling, Call Directory.Move and let it handle the error handling.  
                        // If this succeeds, then we didn't really have errors that needed handling.
                        Directory.Move(this.Url, newPath);
                    }
                } else if (Directory.Exists(newPath)) {
                    // Directory exists and it wasn't the source.  Item cannot be moved as name exists.
                    ShowFileOrFolderAlreadyExistsErrorMessage(newPath);
                } else {
                    Directory.Move(this.Url, newPath);
                }
            }
        }

        void IDiskBasedNode.RenameForDeferredSave(string basePath, string baseNewPath) {
            string oldPath = Path.Combine(basePath, ItemNode.GetMetadata(ProjectFileConstants.Include));
            string newPath = Path.Combine(baseNewPath, ItemNode.GetMetadata(ProjectFileConstants.Include));
            Directory.CreateDirectory(newPath);

            ProjectMgr.UpdatePathForDeferredSave(oldPath, newPath);
        }

        #endregion

        #region helper methods

        /// <summary>
        /// Renames the folder to the new name.
        /// </summary>
        public virtual void RenameFolder(string newName) {
            // Do the rename (note that we only do the physical rename if the leaf name changed)
            string newPath = Path.Combine(Parent.FullPathToChildren, newName);
            string oldPath = Url;
            if (!String.Equals(Path.GetFileName(Url), newName, StringComparison.Ordinal)) {
                RenameDirectory(CommonUtils.GetAbsoluteDirectoryPath(ProjectMgr.ProjectHome, newPath));
            }

            bool wasExpanded = GetIsExpanded();

            ReparentFolder(newPath);

            var oldTriggerFlag = ProjectMgr.EventTriggeringFlag;
            ProjectMgr.EventTriggeringFlag |= ProjectNode.EventTriggering.DoNotTriggerTrackerEvents;
            try {
                // Let all children know of the new path
                for (HierarchyNode child = this.FirstChild; child != null; child = child.NextSibling) {
                    FolderNode node = child as FolderNode;

                    if (node == null) {
                        child.SetEditLabel(child.GetEditLabel());
                    } else {
                        node.RenameFolder(node.Caption);
                    }
                }
            } finally {
                ProjectMgr.EventTriggeringFlag = oldTriggerFlag;
            }

            ProjectMgr.Tracker.OnItemRenamed(oldPath, newPath, VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_Directory);

            // Some of the previous operation may have changed the selection so set it back to us
            ExpandItem(wasExpanded ? EXPANDFLAGS.EXPF_ExpandFolder : EXPANDFLAGS.EXPF_CollapseFolder);
            ExpandItem(EXPANDFLAGS.EXPF_SelectItem);
        }

        /// <summary>
        /// Moves the HierarchyNode from the old path to be a child of the
        /// newly specified node.
        /// 
        /// This is a low-level operation that only updates the hierarchy and our MSBuild
        /// state.  The parents of the node must already be created. 
        /// 
        /// To do a general rename, call RenameFolder instead.
        /// </summary>
        internal void ReparentFolder(string newPath) {
            // Reparent the folder
            ProjectMgr.OnItemDeleted(this);
            Parent.RemoveChild(this);
            ProjectMgr.Site.GetUIThread().MustBeCalledFromUIThread();
            ID = ProjectMgr.ItemIdMap.Add(this);

            ItemNode.Rename(CommonUtils.GetRelativeDirectoryPath(ProjectMgr.ProjectHome, newPath));
            var parent = ProjectMgr.GetParentFolderForPath(newPath);
            Debug.Assert(parent != null, "ReparentFolder called without full path to parent being created");
            parent.AddChild(this);
        }

        /// <summary>
        /// Show error message if not in automation mode, otherwise throw exception
        /// </summary>
        /// <param name="newPath">path of file or folder already existing on disk</param>
        /// <returns>S_OK</returns>
        private int ShowFileOrFolderAlreadyExistsErrorMessage(string newPath) {
            //A file or folder with the name '{0}' already exists on disk at this location. Please choose another name.
            //If this file or folder does not appear in the Solution Explorer, then it is not currently part of your project. To view files which exist on disk, but are not in the project, select Show All Files from the Project menu.
            string errorMessage = SR.GetString(SR.FileOrFolderAlreadyExists, newPath);
            if (!Utilities.IsInAutomationFunction(this.ProjectMgr.Site)) {
                string title = null;
                OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                Utilities.ShowMessageBox(this.ProjectMgr.Site, title, errorMessage, icon, buttons, defaultButton);
                return VSConstants.S_OK;
            } else {
                throw new InvalidOperationException(errorMessage);
            }
        }

        protected override void OnCancelLabelEdit() {
            if (IsBeingCreated) {
                // finish the creation
                FinishFolderAdd(Caption, true);
            }
        }

        internal bool IsBeingCreated {
            get {
                return ProjectMgr.FolderBeingCreated == this;
            }
            set {
                if (value) {
                    ProjectMgr.FolderBeingCreated = this;
                } else {
                    ProjectMgr.FolderBeingCreated = null;
                }
            }
        }

        #endregion

        protected override void RaiseOnItemRemoved(string documentToRemove, string[] filesToBeDeleted) {
            VSREMOVEDIRECTORYFLAGS[] removeFlags = new VSREMOVEDIRECTORYFLAGS[1];
            this.ProjectMgr.Tracker.OnFolderRemoved(documentToRemove, removeFlags[0]);
        }
    }
}
