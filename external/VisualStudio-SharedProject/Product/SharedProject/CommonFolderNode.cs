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
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using VSConstants = Microsoft.VisualStudio.VSConstants;
#if DEV14_OR_LATER
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
#endif

namespace Microsoft.VisualStudioTools.Project {

    internal class CommonFolderNode : FolderNode {
        private CommonProjectNode _project;

        public CommonFolderNode(CommonProjectNode root, ProjectElement element)
            : base(root, element) {
            _project = root;
        }

        public override bool IsNonMemberItem {
            get {
                return ItemNode is AllFilesProjectElement;
            }
        }

#if DEV14_OR_LATER
        protected override ImageMoniker GetIconMoniker(bool open) {
            if (ItemNode.IsExcluded) {
                return open ? KnownMonikers.HiddenFolderOpened : KnownMonikers.HiddenFolderClosed;
            }
            return base.GetIconMoniker(open);
        }
#else
        public override object GetIconHandle(bool open) {
            if (ItemNode.IsExcluded) {
                return ProjectMgr.GetIconHandleByName(open ?
                    ProjectNode.ImageName.OpenExcludedFolder :
                    ProjectNode.ImageName.ExcludedFolder
                );
            }
            return base.GetIconHandle(open);
        }
#endif

        internal override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result) {
            //Hide Exclude from Project command, show everything else normal Folder node supports
            if (cmdGroup == Microsoft.VisualStudioTools.Project.VsMenus.guidStandardCommandSet2K) {
                switch ((VsCommands2K)cmd) {
                    case VsCommands2K.EXCLUDEFROMPROJECT:
                        if (ItemNode.IsExcluded) {
                            result |= QueryStatusResult.NOTSUPPORTED | QueryStatusResult.INVISIBLE;
                            return VSConstants.S_OK;
                        }
                        break;
                    case VsCommands2K.INCLUDEINPROJECT:
                        if (ItemNode.IsExcluded) {
                            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                            return VSConstants.S_OK;
                        }
                        break;
                    case CommonConstants.OpenFolderInExplorerCmdId:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;
                }
            } else if (cmdGroup == ProjectMgr.SharedCommandGuid) {
                switch ((SharedCommands)cmd) {
                    case SharedCommands.AddExistingFolder:
                        if (!ItemNode.IsExcluded) {
                            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                            return VSConstants.S_OK;
                        }
                        break;
                }
            }
            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }

        internal override int ExecCommandOnNode(Guid cmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            if (cmdGroup == Microsoft.VisualStudioTools.Project.VsMenus.guidStandardCommandSet2K) {
                if ((VsCommands2K)cmd == CommonConstants.OpenFolderInExplorerCmdId) {
                    Process.Start(this.Url);
                    return VSConstants.S_OK;
                }
            } else if (cmdGroup == ProjectMgr.SharedCommandGuid) {
                switch ((SharedCommands)cmd) {
                    case SharedCommands.AddExistingFolder:
                        return ProjectMgr.AddExistingFolderToNode(this);
                    case SharedCommands.OpenCommandPromptHere:
                        var psi = new ProcessStartInfo(
                            Path.Combine(
                                Environment.SystemDirectory,
                                "cmd.exe"
                            )
                        );
                        psi.WorkingDirectory = FullPathToChildren;
                        Process.Start(psi);
                        return VSConstants.S_OK;
                }
            }

            return base.ExecCommandOnNode(cmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Handles the exclude from project command.
        /// </summary>
        /// <returns></returns>
        internal override int ExcludeFromProject() {
            ProjectMgr.Site.GetUIThread().MustBeCalledFromUIThread();

            Debug.Assert(this.ProjectMgr != null, "The project item " + this.ToString() + " has not been initialised correctly. It has a null ProjectMgr");
            if (!ProjectMgr.QueryEditProjectFile(false) ||
                !ProjectMgr.QueryFolderRemove(Parent, Url)) {
                return VSConstants.E_FAIL;
            }

            for (var child = FirstChild; child != null; child = child.NextSibling) {
                // we automatically exclude all children below us too
                int hr = child.ExcludeFromProject();
                if (ErrorHandler.Failed(hr)) {
                    return hr;
                }
            }

            ResetNodeProperties();
            ItemNode.RemoveFromProjectFile();
            if (!Directory.Exists(CommonUtils.TrimEndSeparator(Url))) {
                ProjectMgr.OnItemDeleted(this);
                Parent.RemoveChild(this);
            } else {
                ItemNode = new AllFilesProjectElement(Url, ItemNode.ItemTypeName, ProjectMgr);
                if (!ProjectMgr.IsShowingAllFiles) {
                    IsVisible = false;
                    ProjectMgr.OnInvalidateItems(Parent);
                }
                ProjectMgr.ReDrawNode(this, UIHierarchyElement.Icon);
                ProjectMgr.OnPropertyChanged(this, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, 0);
            }
            ((IVsUIShell)GetService(typeof(SVsUIShell))).RefreshPropertyBrowser(0);

            return VSConstants.S_OK;
        }

        internal override int ExcludeFromProjectWithProgress() {
            using (new WaitDialog(
                "Excluding files and folders...",
                "Excluding files and folders in your project, this may take several seconds...",
                ProjectMgr.Site)) {
                return base.ExcludeFromProjectWithProgress();
            }
        }

        internal override int IncludeInProject(bool includeChildren) {
            if (Parent.ItemNode != null && Parent.ItemNode.IsExcluded) {
                // if our parent is excluded it needs to first be included
                int hr = Parent.IncludeInProject(false);
                if (ErrorHandler.Failed(hr)) {
                    return hr;
                }
            }

            if (!ProjectMgr.QueryEditProjectFile(false) ||
                !ProjectMgr.QueryFolderAdd(Parent, Url)) {
                return VSConstants.E_FAIL;
            }

            ResetNodeProperties();
            ItemNode = ProjectMgr.CreateMsBuildFileItem(
                CommonUtils.GetRelativeDirectoryPath(ProjectMgr.ProjectHome, Url),
                ProjectFileConstants.Folder
            );
            IsVisible = true;

            if (includeChildren) {
                for (var child = FirstChild; child != null; child = child.NextSibling) {
                    // we automatically include all children below us too
                    int hr = child.IncludeInProject(includeChildren);
                    if (ErrorHandler.Failed(hr)) {
                        return hr;
                    }
                }
            }
            ProjectMgr.ReDrawNode(this, UIHierarchyElement.Icon);
            ProjectMgr.OnPropertyChanged(this, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, 0);
            ((IVsUIShell)GetService(typeof(SVsUIShell))).RefreshPropertyBrowser(0);

            // On include, the folder should be added to source control.
            this.ProjectMgr.Tracker.OnFolderAdded(this.Url, VSADDDIRECTORYFLAGS.VSADDDIRECTORYFLAGS_NoFlags);

            return VSConstants.S_OK;
        }

        internal override int IncludeInProjectWithProgress(bool includeChildren) {
            using (new WaitDialog(
                "Including files and folders...",
                "Including files and folders to your project, this may take several seconds...",
                ProjectMgr.Site)) {

                return IncludeInProject(includeChildren);
            }
        }

        public override void RenameFolder(string newName) {
            string oldName = Url;
            _project.SuppressFileChangeNotifications();
            try {
                base.RenameFolder(newName);
            } finally {
                _project.RestoreFileChangeNotifications();
            }

            if (ProjectMgr.TryDeactivateSymLinkWatcher(this)) {
                ProjectMgr.CreateSymLinkWatcher(Url);
            }
        }

        public override void Remove(bool removeFromStorage) {
            base.Remove(removeFromStorage);

            // if we were a symlink folder, we need to stop watching now.
            ProjectMgr.TryDeactivateSymLinkWatcher(this);
        }

        public override void Close() {
            base.Close();

            // make sure this thing isn't hanging around...
            ProjectMgr.TryDeactivateSymLinkWatcher(this);
        }

        /// <summary>
        /// Common Folder Node can only be deleted from file system.
        /// </summary>        
        internal override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation) {
            return deleteOperation == __VSDELETEITEMOPERATION.DELITEMOP_DeleteFromStorage;
        }

        public new CommonProjectNode ProjectMgr {
            get {
                return (CommonProjectNode)base.ProjectMgr;
            }
        }
    }
}
