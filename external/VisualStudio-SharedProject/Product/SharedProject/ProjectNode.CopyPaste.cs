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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleDataObject = Microsoft.VisualStudio.OLE.Interop.IDataObject;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Manages the CopyPaste and Drag and Drop scenarios for a Project.
    /// </summary>
    /// <remarks>This is a partial class.</remarks>
    internal partial class ProjectNode : IVsUIHierWinClipboardHelperEvents {
        private uint copyPasteCookie;
        private DropDataType _dropType;
        /// <summary>
        /// Current state of whether we have initiated a cut/copy from within our hierarchy.
        /// </summary>
        private CopyCutState _copyCutState;
        /// <summary>
        /// True if we initiated a drag from within our project, false if the drag
        /// was initiated from another project or there is currently no drag/drop operation
        /// in progress.
        /// </summary>
        private bool _dragging;

        enum CopyCutState {
            /// <summary>
            /// Nothing has been copied to the clipboard from our project
            /// </summary>
            None,
            /// <summary>
            /// Something was cut from our project
            /// </summary>
            Cut,
            /// <summary>
            /// Something was copied from our project
            /// </summary>
            Copied
        }

        #region override of IVsHierarchyDropDataTarget methods
        /// <summary>
        /// Called as soon as the mouse drags an item over a new hierarchy or hierarchy window
        /// </summary>
        /// <param name="pDataObject">reference to interface IDataObject of the item being dragged</param>
        /// <param name="grfKeyState">Current state of the keyboard and the mouse modifier keys. See docs for a list of possible values</param>
        /// <param name="itemid">Item identifier for the item currently being dragged</param>
        /// <param name="pdwEffect">On entry, a pointer to the current DropEffect. On return, must contain the new valid DropEffect</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public int DragEnter(IOleDataObject pDataObject, uint grfKeyState, uint itemid, ref uint pdwEffect) {
            pdwEffect = (uint)DropEffect.None;

            var item = NodeFromItemId(itemid);

            if (item.GetDragTargetHandlerNode().CanAddFiles) {
                _dropType = QueryDropDataType(pDataObject);
                if (_dropType != DropDataType.None) {
                    pdwEffect = (uint)QueryDropEffect(grfKeyState);
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when one or more items are dragged out of the hierarchy or hierarchy window, or when the drag-and-drop operation is cancelled or completed.
        /// </summary>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public int DragLeave() {
            _dropType = DropDataType.None;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when one or more items are dragged over the target hierarchy or hierarchy window. 
        /// </summary>
        /// <param name="grfKeyState">Current state of the keyboard keys and the mouse modifier buttons. See <seealso cref="IVsHierarchyDropDataTarget"/></param>
        /// <param name="itemid">Item identifier of the drop data target over which the item is being dragged</param>
        /// <param name="pdwEffect"> On entry, reference to the value of the pdwEffect parameter of the IVsHierarchy object, identifying all effects that the hierarchy supports. 
        /// On return, the pdwEffect parameter must contain one of the effect flags that indicate the result of the drop operation. For a list of pwdEffects values, see <seealso cref="DragEnter"/></param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public int DragOver(uint grfKeyState, uint itemid, ref uint pdwEffect) {
            pdwEffect = (uint)DropEffect.None;

            // Dragging items to a project that is being debugged is not supported
            // (see VSWhidbey 144785)            
            DBGMODE dbgMode = VsShellUtilities.GetDebugMode(this.Site) & ~DBGMODE.DBGMODE_EncMask;
            if (dbgMode == DBGMODE.DBGMODE_Run || dbgMode == DBGMODE.DBGMODE_Break) {
                return VSConstants.S_OK;
            }

            if (this.isClosed) {
                return VSConstants.E_UNEXPECTED;
            }

            // TODO: We should also analyze if the node being dragged over can accept the drop.

            pdwEffect = (uint)QueryDropEffect(grfKeyState);

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when one or more items are dropped into the target hierarchy or hierarchy window when the mouse button is released.
        /// </summary>
        /// <param name="pDataObject">Reference to the IDataObject interface on the item being dragged. This data object contains the data being transferred in the drag-and-drop operation. 
        /// If the drop occurs, then this data object (item) is incorporated into the target hierarchy or hierarchy window.</param>
        /// <param name="grfKeyState">Current state of the keyboard and the mouse modifier keys. See <seealso cref="IVsHierarchyDropDataTarget"/></param>
        /// <param name="itemid">Item identifier of the drop data target over which the item is being dragged</param>
        /// <param name="pdwEffect">Visual effects associated with the drag-and drop-operation, such as a cursor, bitmap, and so on. 
        /// The value of dwEffects passed to the source object via the OnDropNotify method is the value of pdwEffects returned by the Drop method</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public int Drop(IOleDataObject pDataObject, uint grfKeyState, uint itemid, ref uint pdwEffect) {
            if (pDataObject == null) {
                return VSConstants.E_INVALIDARG;
            }

            pdwEffect = (uint)DropEffect.None;

            // Get the node that is being dragged over and ask it which node should handle this call
            HierarchyNode targetNode = NodeFromItemId(itemid);
            if (targetNode == null) {
                // There is no target node. The drop can not be completed.
                return VSConstants.S_FALSE;
            }

            int returnValue;
            try {
                DropDataType dropDataType = DropDataType.None;
                pdwEffect = (uint)QueryDropEffect(grfKeyState);
                dropDataType = ProcessSelectionDataObject(pDataObject, targetNode, true, (DropEffect)pdwEffect);
                if (dropDataType == DropDataType.None) {
                    pdwEffect = (uint)DropEffect.None;
                }

                // If it is a drop from windows and we get any kind of error we return S_FALSE and dropeffect none. This
                // prevents bogus messages from the shell from being displayed
                returnValue = (dropDataType != DropDataType.Shell) ? VSConstants.E_FAIL : VSConstants.S_OK;
            } catch (System.IO.FileNotFoundException e) {
                Trace.WriteLine("Exception : " + e.Message);

                if (!Utilities.IsInAutomationFunction(this.Site)) {
                    string message = e.Message;
                    string title = string.Empty;
                    OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                    OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                    Utilities.ShowMessageBox(this.Site, title, message, icon, buttons, defaultButton);
                }

                returnValue = VSConstants.E_FAIL;
            }

            _dragging = false;

            return returnValue;
        }
        #endregion

        #region override of IVsHierarchyDropDataSource2 methods
        /// <summary>
        /// Returns information about one or more of the items being dragged
        /// </summary>
        /// <param name="pdwOKEffects">Pointer to a DWORD value describing the effects displayed while the item is being dragged, 
        /// such as cursor icons that change during the drag-and-drop operation. 
        /// For example, if the item is dragged over an invalid target point 
        /// (such as the item's original location), the cursor icon changes to a circle with a line through it. 
        /// Similarly, if the item is dragged over a valid target point, the cursor icon changes to a file or folder.</param>
        /// <param name="ppDataObject">Pointer to the IDataObject interface on the item being dragged. 
        /// This data object contains the data being transferred in the drag-and-drop operation. 
        /// If the drop occurs, then this data object (item) is incorporated into the target hierarchy or hierarchy window.</param>
        /// <param name="ppDropSource">Pointer to the IDropSource interface of the item being dragged.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public int GetDropInfo(out uint pdwOKEffects, out IOleDataObject ppDataObject, out IDropSource ppDropSource) {
            //init out params
            pdwOKEffects = (uint)DropEffect.None;
            ppDataObject = null;
            ppDropSource = null;

            IOleDataObject dataObject = PackageSelectionDataObject(false);
            if (dataObject == null) {
                return VSConstants.E_NOTIMPL;
            }

            _dragging = true;
            pdwOKEffects = (uint)(DropEffect.Move | DropEffect.Copy);

            ppDataObject = dataObject;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies clients that the dragged item was dropped. 
        /// </summary>
        /// <param name="fDropped">If true, then the dragged item was dropped on the target. If false, then the drop did not occur.</param>
        /// <param name="dwEffects">Visual effects associated with the drag-and-drop operation, such as cursors, bitmaps, and so on. 
        /// The value of dwEffects passed to the source object via OnDropNotify method is the value of pdwEffects returned by Drop method.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public int OnDropNotify(int fDropped, uint dwEffects) {
            if (dwEffects == (uint)DropEffect.Move) {
                foreach (var item in ItemsDraggedOrCutOrCopied) {
                    item.Remove(true);
                }
            }
            ItemsDraggedOrCutOrCopied.Clear();
            _dragging = false;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Allows the drag source to prompt to save unsaved items being dropped. 
        /// Notifies the source hierarchy that information dragged from it is about to be dropped on a target. 
        /// This method is called immediately after the mouse button is released on a drop. 
        /// </summary>
        /// <param name="o">Reference to the IDataObject interface on the item being dragged. 
        /// This data object contains the data being transferred in the drag-and-drop operation. 
        /// If the drop occurs, then this data object (item) is incorporated into the hierarchy window of the new hierarchy.</param>
        /// <param name="dwEffect">Current state of the keyboard and the mouse modifier keys.</param>
        /// <param name="fCancelDrop">If true, then the drop is cancelled by the source hierarchy. If false, then the drop can continue.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public int OnBeforeDropNotify(IOleDataObject o, uint dwEffect, out int fCancelDrop) {
            // If there is nothing to be dropped just return that drop should be cancelled.
            if (this.ItemsDraggedOrCutOrCopied == null) {
                fCancelDrop = 1;
                return VSConstants.S_OK;
            }

            fCancelDrop = 0;
            bool dirty = false;
            foreach (HierarchyNode node in this.ItemsDraggedOrCutOrCopied) {
                if (node.IsLinkFile) {
                    continue;
                }

                DocumentManager manager = node.GetDocumentManager();
                if (manager != null &&
                    manager.IsDirty &&
                    manager.IsOpenedByUs) {
                    dirty = true;
                    break;
                }
            }

            // if there are no dirty docs we are ok to proceed
            if (!dirty) {
                return VSConstants.S_OK;
            }

            // Prompt to save if there are dirty docs
            string message = SR.GetString(SR.SaveModifiedDocuments);
            string title = string.Empty;
            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_WARNING;
            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL;
            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
            int result = Utilities.ShowMessageBox(Site, title, message, icon, buttons, defaultButton);
            switch (result) {
                case NativeMethods.IDYES:
                    break;

                case NativeMethods.IDNO:
                    return VSConstants.S_OK;

                case NativeMethods.IDCANCEL:
                    goto default;

                default:
                    fCancelDrop = 1;
                    ItemsDraggedOrCutOrCopied.Clear();
                    return VSConstants.S_OK;
            }

            // Save all dirty documents
            foreach (HierarchyNode node in this.ItemsDraggedOrCutOrCopied) {
                DocumentManager manager = node.GetDocumentManager();
                if (manager != null) {
                    manager.Save(true);
                }
            }

            return VSConstants.S_OK;
        }

        #endregion

        #region IVsUIHierWinClipboardHelperEvents Members
        /// <summary>
        /// Called after your cut/copied items has been pasted
        /// </summary>
        ///<param name="wasCut">If true, then the IDataObject has been successfully pasted into a target hierarchy. 
        /// If false, then the cut or copy operation was cancelled.</param>
        /// <param name="dropEffect">Visual effects associated with the drag and drop operation, such as cursors, bitmaps, and so on. 
        /// These should be the same visual effects used in OnDropNotify</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int OnPaste(int wasCut, uint dropEffect) {
            if (dropEffect == (uint)DropEffect.None) {
                return OnClear(wasCut);
            }

            // Check both values here.  If the paste is coming from another project system then
            // they should always pass Move, and we'll know whether or not it's a cut from wasCut.
            // If they copied it from the project system wasCut will be false, and DropEffect
            // will still be Move, resulting in a copy.
            if (wasCut != 0 && dropEffect == (uint)DropEffect.Move) {
                // If we just did a cut, then we need to free the data object. Otherwise, we leave it
                // alone so that you can continue to paste the data in new locations.
                CleanAndFlushClipboard();
                foreach (HierarchyNode node in ItemsDraggedOrCutOrCopied) {
                    node.Remove(true);
                }
                ItemsDraggedOrCutOrCopied.Clear();
                ClearCopyCutState();
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when your cut/copied operation is canceled
        /// </summary>
        /// <param name="wasCut">This flag informs the source that the Cut method was called (true), 
        /// rather than Copy (false), so the source knows whether to "un-cut-highlight" the items that were cut.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int OnClear(int wasCut) {
            if (wasCut != 0) {
                AssertHasParentHierarchy();
                IVsUIHierarchyWindow w = UIHierarchyUtilities.GetUIHierarchyWindow(this.site, HierarchyNode.SolutionExplorer);
                if (w != null) {
                    foreach (HierarchyNode node in ItemsDraggedOrCutOrCopied) {
                        node.ExpandItem(EXPANDFLAGS.EXPF_UnCutHighlightItem);
                    }
                }
            }

            ItemsDraggedOrCutOrCopied.Clear();

            ClearCopyCutState();
            return VSConstants.S_OK;
        }
        #endregion

        #region virtual methods

        /// <summary>
        /// Returns a dataobject from selected nodes
        /// </summary>
        /// <param name="cutHighlightItems">boolean that defines if the selected items must be cut</param>
        /// <returns>data object for selected items</returns>
        private DataObject PackageSelectionDataObject(bool cutHighlightItems) {
            StringBuilder sb = new StringBuilder();

            DataObject dataObject = null;

            IList<HierarchyNode> selectedNodes = this.GetSelectedNodes();
            if (selectedNodes != null) {
                this.InstantiateItemsDraggedOrCutOrCopiedList();

                // If there is a selection package the data
                foreach (HierarchyNode node in selectedNodes) {
                    string selectionContent = node.PrepareSelectedNodesForClipBoard();
                    if (selectionContent != null) {
                        sb.Append(selectionContent);
                    }
                }
            }

            // Add the project items first.
            IntPtr ptrToItems = this.PackageSelectionData(sb, false);
            if (ptrToItems == IntPtr.Zero) {
                return null;
            }

            FORMATETC fmt = DragDropHelper.CreateFormatEtc(DragDropHelper.CF_VSSTGPROJECTITEMS);
            dataObject = new DataObject();
            dataObject.SetData(fmt, ptrToItems);

            // Now add the project path that sourced data. We just write the project file path.
            IntPtr ptrToProjectPath = this.PackageSelectionData(new StringBuilder(this.GetMkDocument()), true);

            if (ptrToProjectPath != IntPtr.Zero) {
                dataObject.SetData(DragDropHelper.CreateFormatEtc(DragDropHelper.CF_VSPROJECTCLIPDESCRIPTOR), ptrToProjectPath);
            }

            if (cutHighlightItems) {
                bool first = true;
                foreach (HierarchyNode node in this.ItemsDraggedOrCutOrCopied) {
                    node.ExpandItem(first ? EXPANDFLAGS.EXPF_CutHighlightItem : EXPANDFLAGS.EXPF_AddCutHighlightItem);
                    first = false;
                }
            }
            return dataObject;
        }

        class ProjectReferenceFileAdder {
            /// <summary>
            /// This hierarchy which is having items added/moved
            /// </summary>
            private readonly ProjectNode Project;
            /// <summary>
            /// The node which we're adding/moving the items to
            /// </summary>
            private readonly HierarchyNode TargetNode;
            /// <summary>
            /// The references we're adding, using the format {Guid}|project|folderPath
            /// </summary>
            private readonly string[] ProjectReferences;
            /// <summary>
            /// True if this is the result of a mouse drop, false if this is the result of a paste
            /// </summary>
            private readonly bool MouseDropping;
            /// <summary>
            /// Move or Copy
            /// </summary>
            private readonly DropEffect DropEffect;
            private bool? OverwriteAllItems;

            public ProjectReferenceFileAdder(ProjectNode project, HierarchyNode targetNode, string[] projectReferences, bool mouseDropping, DropEffect dropEffect) {
                Utilities.ArgumentNotNull("targetNode", targetNode);
                Utilities.ArgumentNotNull("project", project);
                Utilities.ArgumentNotNull("projectReferences", projectReferences);

                TargetNode = targetNode;
                Project = project;
                ProjectReferences = projectReferences;
                MouseDropping = mouseDropping;
                DropEffect = dropEffect;
            }

            internal bool AddFiles() {
                // Collect all of the additions.
                List<Addition> additions = new List<Addition>();
                List<string> folders = new List<string>();
                // process folders first
                foreach (string projectReference in ProjectReferences) {
                    if (projectReference == null) {
                        // bad projectref, bail out
                        return false;
                    }
                    if (CommonUtils.HasEndSeparator(projectReference)) {

                        var addition = CanAddFolderFromProjectReference(projectReference);
                        if (addition == null) {
                            return false;
                        }
                        additions.Add(addition);
                        FolderAddition folderAddition = addition as FolderAddition;
                        if (folderAddition != null) {
                            folders.Add(folderAddition.SourceFolder);
                        }
                    }
                }
                foreach (string projectReference in ProjectReferences) {
                    if (projectReference == null) {
                        // bad projectref, bail out
                        return false;
                    }
                    if (!CommonUtils.HasEndSeparator(projectReference)) {
                        var addition = CanAddFileFromProjectReference(projectReference, TargetNode.GetDragTargetHandlerNode().FullPathToChildren);
                        if (addition == null) {
                            return false;
                        }
                        FileAddition fileAddition = addition as FileAddition;
                        bool add = true;
                        if (fileAddition != null) {
                            foreach (var folder in folders) {
                                if (fileAddition.SourceMoniker.StartsWith(folder, StringComparison.OrdinalIgnoreCase)) {
                                    // this will be moved/copied by the folder, it doesn't need another move/copy
                                    add = false;
                                    break;
                                }
                            }
                        }
                        if (add) {
                            additions.Add(addition);
                        }
                    }
                }

                bool result = true;
                bool? overwrite = null;
                foreach (var addition in additions) {
                    try {
                        addition.DoAddition(ref overwrite);
                    } catch (CancelPasteException) {
                        return false;
                    }
                    if (addition is SkipOverwriteAddition) {
                        result = false;
                    }
                }

                return result;
            }

            [Serializable]
            sealed class CancelPasteException : Exception {
            }

            /// <summary>
            /// Tests to see if we can add the folder to the project.  Returns true if it's ok, false if it's not.
            /// </summary>
            /// <param name="folderToAdd">Project reference (from data object) using the format: {Guid}|project|folderPath</param>
            /// <param name="targetNode">Node to add the new folder to</param>
            private Addition CanAddFolderFromProjectReference(string folderToAdd) {
                Utilities.ArgumentNotNullOrEmpty(folderToAdd, "folderToAdd");

                var targetFolderNode = TargetNode.GetDragTargetHandlerNode();

                string folder;
                IVsHierarchy sourceHierarchy;
                GetPathAndHierarchy(folderToAdd, out folder, out sourceHierarchy);

                // Ensure we don't end up in an endless recursion
                if (Utilities.IsSameComObject(Project, sourceHierarchy)) {
                    if (String.Equals(folder, targetFolderNode.FullPathToChildren, StringComparison.OrdinalIgnoreCase)) {
                        if (DropEffect == DropEffect.Move &&
                            IsBadMove(targetFolderNode.FullPathToChildren, folder, false)) {
                            return null;
                        }
                    }

                    if (targetFolderNode.FullPathToChildren.StartsWith(folder, StringComparison.OrdinalIgnoreCase) &&
                        !String.Equals(targetFolderNode.FullPathToChildren, folder, StringComparison.OrdinalIgnoreCase)) {
                        // dragging a folder into a child, that's not allowed
                        Utilities.ShowMessageBox(
                            Project.Site,
                            SR.GetString(SR.CannotMoveIntoSubfolder, CommonUtils.GetFileOrDirectoryName(folder)),
                            null,
                            OLEMSGICON.OLEMSGICON_CRITICAL,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        return null;
                    }
                }

                var targetPath = Path.Combine(targetFolderNode.FullPathToChildren, CommonUtils.GetFileOrDirectoryName(folder));
                if (File.Exists(targetPath)) {
                    Utilities.ShowMessageBox(
                       Project.Site,
                       SR.GetString(SR.CannotAddFileExists, CommonUtils.GetFileOrDirectoryName(folder)),
                       null,
                       OLEMSGICON.OLEMSGICON_CRITICAL,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return null;
                }

                if (Directory.Exists(targetPath)) {
                    if (DropEffect == DropEffect.Move) {
                        if (targetPath == folderToAdd) {
                            CannotMoveSameLocation(folderToAdd);
                        } else {
                            Utilities.ShowMessageBox(
                               Project.Site,
                               SR.GetString(SR.CannotMoveFolderExists, CommonUtils.GetFileOrDirectoryName(folder)),
                               null,
                               OLEMSGICON.OLEMSGICON_CRITICAL,
                               OLEMSGBUTTON.OLEMSGBUTTON_OK,
                               OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        }
                        return null;
                    }

                    var dialog = new OverwriteFileDialog(
                        SR.GetString(SR.OverwriteFilesInExistingFolder, CommonUtils.GetFileOrDirectoryName(folder)),
                        false
                    );
                    dialog.Owner = Application.Current.MainWindow;
                    var res = dialog.ShowDialog();
                    if (res == null) {
                        // cancel, abort the whole copy
                        return null;
                    } else if (!dialog.ShouldOverwrite) {
                        // no, don't copy the folder
                        return SkipOverwriteAddition.Instance;
                    }
                    // otherwise yes, and we'll prompt about the files.
                }

                string targetFileName = CommonUtils.GetFileOrDirectoryName(folder);
                if (Utilities.IsSameComObject(Project, sourceHierarchy) &&
                    String.Equals(targetFolderNode.FullPathToChildren, folder, StringComparison.OrdinalIgnoreCase)) {
                    // copying a folder onto its self, make a copy
                    targetFileName = GetCopyName(targetFolderNode.FullPathToChildren);
                }

                List<Addition> additions = new List<Addition>();
                uint folderId;
                if (ErrorHandler.Failed(sourceHierarchy.ParseCanonicalName(folder, out folderId))) {
                    // the folder may have been deleted between the copy & paste
                    ReportMissingItem(folder);
                    return null;
                }

                if (Path.Combine(targetFolderNode.FullPathToChildren, targetFileName).Length >= NativeMethods.MAX_FOLDER_PATH) {
                    Utilities.ShowMessageBox(
                        Project.Site,
                        SR.GetString(SR.FolderPathTooLongShortMessage),
                        null,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return null;
                }

                if (!WalkSourceProjectAndAdd(sourceHierarchy, folderId, targetFolderNode.FullPathToChildren, false, additions, targetFileName)) {
                    return null;
                }

                if (additions.Count == 1) {
                    return (FolderAddition)additions[0];
                }

                Debug.Assert(additions.Count == 0);
                return null;
            }

            private void ReportMissingItem(string folder) {
                Utilities.ShowMessageBox(
                    Project.Site,
                    SR.GetString(SR.SourceUrlNotFound, CommonUtils.GetFileOrDirectoryName(folder)),
                    null,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }

            /// <summary>
            /// Recursive method that walk a hierarchy and add items it find to our project.
            /// Note that this is meant as an helper to the Copy&Paste/Drag&Drop functionality.
            /// </summary>
            /// <param name="sourceHierarchy">Hierarchy to walk</param>
            /// <param name="itemId">Item ID where to start walking the hierarchy</param>
            /// <param name="targetNode">Node to start adding to</param>
            /// <param name="addSibblings">Typically false on first call and true after that</param>
            private bool WalkSourceProjectAndAdd(IVsHierarchy sourceHierarchy, uint itemId, string targetPath, bool addSiblings, List<Addition> additions, string name = null) {
                Utilities.ArgumentNotNull("sourceHierarchy", sourceHierarchy);

                if (itemId != VSConstants.VSITEMID_NIL) {
                    // Before we start the walk, add the current node
                    object variant = null;

                    // Calculate the corresponding path in our project
                    string source;
                    ErrorHandler.ThrowOnFailure(((IVsProject)sourceHierarchy).GetMkDocument(itemId, out source));
                    if (name == null) {
                        name = CommonUtils.GetFileOrDirectoryName(source);
                    }

                    Guid guidType;
                    ErrorHandler.ThrowOnFailure(sourceHierarchy.GetGuidProperty(itemId, (int)__VSHPROPID.VSHPROPID_TypeGuid, out guidType));

                    IVsSolution solution = Project.GetService(typeof(IVsSolution)) as IVsSolution;
                    if (solution != null) {
                        if (guidType == VSConstants.GUID_ItemType_PhysicalFile) {
                            string projRef;
                            ErrorHandler.ThrowOnFailure(solution.GetProjrefOfItem(sourceHierarchy, itemId, out projRef));
                            var addition = CanAddFileFromProjectReference(projRef, targetPath);
                            if (addition == null) {
                                // cancelled
                                return false;
                            }
                            additions.Add(addition);
                        }
                    }

                    // Start with child nodes (depth first)
                    if (guidType == VSConstants.GUID_ItemType_PhysicalFolder) {
                        variant = null;
                        ErrorHandler.ThrowOnFailure(sourceHierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_FirstVisibleChild, out variant));
                        uint currentItemID = (uint)(int)variant;

                        List<Addition> nestedAdditions = new List<Addition>();

                        string newPath = Path.Combine(targetPath, name);

                        if (!WalkSourceProjectAndAdd(sourceHierarchy, currentItemID, newPath, true, nestedAdditions)) {
                            // cancelled
                            return false;
                        }

                        if (!Project.Tracker.CanRenameItem(
                            source,
                            newPath,
                            VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_Directory)) {
                            return false;
                        }

                        additions.Add(new FolderAddition(Project, Path.Combine(targetPath, name), source, DropEffect, nestedAdditions.ToArray()));
                    }

                    if (addSiblings) {
                        // Then look at siblings
                        uint currentItemID = itemId;
                        while (currentItemID != VSConstants.VSITEMID_NIL) {
                            variant = null;
                            // http://mpfproj10.codeplex.com/workitem/11618 - pass currentItemID instead of itemId
                            ErrorHandler.ThrowOnFailure(sourceHierarchy.GetProperty(currentItemID, (int)__VSHPROPID.VSHPROPID_NextVisibleSibling, out variant));
                            currentItemID = (uint)(int)variant;
                            if (!WalkSourceProjectAndAdd(sourceHierarchy, currentItemID, targetPath, false, additions)) {
                                // cancelled
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            private static string GetCopyName(string existingFullPath) {
                string newDir, name, extension;
                if (CommonUtils.HasEndSeparator(existingFullPath)) {
                    name = CommonUtils.GetFileOrDirectoryName(existingFullPath);
                    extension = "";
                } else {
                    extension = Path.GetExtension(existingFullPath);
                    name = Path.GetFileNameWithoutExtension(existingFullPath);
                }

                string folder = CommonUtils.GetParent(existingFullPath);
                int copyCount = 1;
                do {
                    string newName = name + " - Copy";
                    if (copyCount != 1) {
                        newName += " (" + copyCount + ")";
                    }
                    newName += extension;
                    copyCount++;
                    newDir = Path.Combine(folder, newName);
                } while (File.Exists(newDir) || Directory.Exists(newDir));
                return newDir;
            }

            /// <summary>
            /// This is used to recursively add a folder from an other project.
            /// Note that while we copy the folder content completely, we only
            /// add to the project items which are part of the source project.
            /// </summary>
            class FolderAddition : Addition {
                private readonly ProjectNode Project;
                private readonly string NewFolderPath;
                public readonly string SourceFolder;
                private readonly Addition[] Additions;
                private readonly DropEffect DropEffect;

                public FolderAddition(ProjectNode project, string newFolderPath, string sourceFolder, DropEffect dropEffect, Addition[] additions) {
                    Project = project;
                    NewFolderPath = newFolderPath;
                    SourceFolder = sourceFolder;
                    Additions = additions;
                    DropEffect = dropEffect;
                }

                public override void DoAddition(ref bool? overwrite) {
                    bool wasExpanded = false;
                    HierarchyNode newNode;
                    var sourceFolder = Project.FindNodeByFullPath(SourceFolder) as FolderNode;
                    if (sourceFolder == null || DropEffect != DropEffect.Move) {
                        newNode = Project.CreateFolderNodes(NewFolderPath);
                    } else {
                        // Rename the folder & reparent our existing FolderNode w/ potentially w/ a new ID,
                        // but don't update the children as we'll handle that w/ our file additions...
                        wasExpanded = sourceFolder.GetIsExpanded();
                        Directory.CreateDirectory(NewFolderPath);
                        sourceFolder.ReparentFolder(NewFolderPath);

                        sourceFolder.ExpandItem(wasExpanded ? EXPANDFLAGS.EXPF_ExpandFolder : EXPANDFLAGS.EXPF_CollapseFolder);
                        newNode = sourceFolder;
                    }

                    foreach (var addition in Additions) {
                        addition.DoAddition(ref overwrite);
                    }

                    if (sourceFolder != null) {
                        if (sourceFolder.IsNonMemberItem) {
                            // copying or moving an existing excluded folder, new folder
                            // is excluded too.
                            ErrorHandler.ThrowOnFailure(newNode.ExcludeFromProject());
                        } else if (sourceFolder.Parent.IsNonMemberItem) {
                            // We've moved an included folder to a show all files folder,
                            //     add the parent to the project   
                            ErrorHandler.ThrowOnFailure(sourceFolder.Parent.IncludeInProject(false));
                        }

                        if (DropEffect == DropEffect.Move) {
                            Directory.Delete(SourceFolder);

                            // we just handled the delete, the updated folder has the new filename,
                            // and we don't want to delete where we just moved stuff...
                            Project.ItemsDraggedOrCutOrCopied.Remove(sourceFolder);
                        }
                    }

                    // Send OnItemRenamed for the folder now, after all of the children have been renamed
                    Project.Tracker.OnItemRenamed(SourceFolder, NewFolderPath, VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_Directory);

                    if (sourceFolder != null && Project.ParentHierarchy != null) {
                        sourceFolder.ExpandItem(wasExpanded ? EXPANDFLAGS.EXPF_ExpandFolder : EXPANDFLAGS.EXPF_CollapseFolder);
                    }
                }
            }

            /// <summary>
            /// Given the reference used for drag and drop returns the path to the item and it's
            /// containing hierarchy.
            /// </summary>
            /// <param name="projectReference"></param>
            /// <param name="path"></param>
            /// <param name="sourceHierarchy"></param>
            private void GetPathAndHierarchy(string projectReference, out string path, out IVsHierarchy sourceHierarchy) {
                Guid projectInstanceGuid;

                GetPathAndProjectId(projectReference, out projectInstanceGuid, out path);
                // normalize the casing in case the project system gave us casing different from the file system
                if (CommonUtils.HasEndSeparator(path)) {
                    try {
                        var trimmedPath = CommonUtils.TrimEndSeparator(path);
                        foreach (var dir in Directory.GetDirectories(Path.GetDirectoryName(trimmedPath), Path.GetFileName(trimmedPath))) {
                            if (String.Equals(dir, trimmedPath, StringComparison.OrdinalIgnoreCase)) {
                                path = dir + Path.DirectorySeparatorChar;
                                break;
                            }
                        }
                    } catch {
                    }
                } else {
                    try {
                        foreach (var file in Directory.GetFiles(Path.GetDirectoryName(path))) {
                            if (String.Equals(file, path, StringComparison.OrdinalIgnoreCase)) {
                                path = file;
                                break;
                            }
                        }
                    } catch {
                    }
                }

                // Retrieve the project from which the items are being copied

                IVsSolution solution = (IVsSolution)Project.GetService(typeof(SVsSolution));
                ErrorHandler.ThrowOnFailure(solution.GetProjectOfGuid(ref projectInstanceGuid, out sourceHierarchy));
            }

            private static void GetPathAndProjectId(string projectReference, out Guid projectInstanceGuid, out string folder) {
                // Split the reference in its 3 parts
                int index1 = Guid.Empty.ToString("B").Length;
                if (index1 + 1 >= projectReference.Length)
                    throw new ArgumentOutOfRangeException("folderToAdd");

                // Get the Guid
                string guidString = projectReference.Substring(1, index1 - 2);
                projectInstanceGuid = new Guid(guidString);

                // Get the project path
                int index2 = projectReference.IndexOf('|', index1 + 1);
                if (index2 < 0 || index2 + 1 >= projectReference.Length)
                    throw new ArgumentOutOfRangeException("folderToAdd");

                // Finally get the source path
                folder = projectReference.Substring(index2 + 1);
            }

            /// <summary>
            /// Adds an item from a project refererence to target node.
            /// </summary>
            /// <param name="projectRef"></param>
            /// <param name="targetNode"></param>
            private Addition CanAddFileFromProjectReference(string projectRef, string targetFolder, bool fromFolder = false) {
                Utilities.ArgumentNotNullOrEmpty("projectRef", projectRef);

                IVsSolution solution = Project.GetService(typeof(IVsSolution)) as IVsSolution;
                Utilities.CheckNotNull(solution);

                uint itemidLoc;
                IVsHierarchy hierarchy;
                string str;
                VSUPDATEPROJREFREASON[] reason = new VSUPDATEPROJREFREASON[1];
                if (ErrorHandler.Failed(solution.GetItemOfProjref(projectRef, out hierarchy, out itemidLoc, out str, reason))) {
                    // the file may have been deleted between the copy & paste
                    string path;
                    Guid projectGuid;
                    GetPathAndProjectId(projectRef, out projectGuid, out path);
                    ReportMissingItem(path);
                    return null;
                }

                Utilities.CheckNotNull(hierarchy);

                // This will throw invalid cast exception if the hierrachy is not a project.
                IVsProject project = (IVsProject)hierarchy;
                object isLinkValue;
                bool isLink = false;
                if (ErrorHandler.Succeeded(((IVsHierarchy)project).GetProperty(itemidLoc, (int)__VSHPROPID2.VSHPROPID_IsLinkFile, out isLinkValue))) {
                    if (isLinkValue is bool) {
                        isLink = (bool)isLinkValue;
                    }
                }

                string moniker;
                ErrorHandler.ThrowOnFailure(project.GetMkDocument(itemidLoc, out moniker));

                if (DropEffect == DropEffect.Move && IsBadMove(targetFolder, moniker, true)) {
                    return null;
                }

                if (!File.Exists(moniker)) {
                    Utilities.ShowMessageBox(
                            Project.Site,
                            String.Format("The item '{0}' does not exist in the project directory. It may have been moved, renamed or deleted.", Path.GetFileName(moniker)),
                            null,
                            OLEMSGICON.OLEMSGICON_CRITICAL,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return null;
                }

                // Check that the source and destination paths aren't the same since we can't move an item to itself.
                // If they are in fact the same location, throw an error that copy/move will not work correctly.
                if (DropEffect == DropEffect.Move && !CommonUtils.IsSamePath(Path.GetDirectoryName(moniker), Path.GetDirectoryName(targetFolder))) {
                    try {
                        string sourceLinkTarget = NativeMethods.GetAbsolutePathToDirectory(Path.GetDirectoryName(moniker));
                        string destinationLinkTarget = null;

                        // if the directory doesn't exist, just skip this.  We will create it later.
                        if (Directory.Exists(targetFolder)) {
                            try {
                                destinationLinkTarget = NativeMethods.GetAbsolutePathToDirectory(targetFolder);
                            } catch (FileNotFoundException) {
                                // This can occur if the user had a symlink'd directory and deleted the backing directory.
                                Utilities.ShowMessageBox(
                                            Project.Site,
                                            String.Format(
                                                "Unable to find the destination folder."),
                                            null,
                                            OLEMSGICON.OLEMSGICON_CRITICAL,
                                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                                return null;
                            }
                        }

                        // If the paths are the same, we can't really move the file...
                        if (destinationLinkTarget != null && CommonUtils.IsSamePath(sourceLinkTarget, destinationLinkTarget)) {
                            CannotMoveSameLocation(moniker);
                            return null;
                        }
                    } catch (Exception e) {
                        if (e.IsCriticalException()) {
                            throw;
                        }
                        TaskDialog.ForException(Project.Site, e, String.Empty, Project.IssueTrackerUrl).ShowModal();
                        return null;
                    }
                }

                // Begin the move operation now that we are past pre-checks.
                var existingChild = Project.FindNodeByFullPath(moniker);
                if (isLink) {
                    // links we just want to update the link node for...
                    if (existingChild != null) {
                        if (ComUtilities.IsSameComObject(Project, project)) {
                            if (DropEffect != DropEffect.Move) {
                                Utilities.ShowMessageBox(
                                        Project.Site,
                                        String.Format("Cannot copy linked files within the same project. You cannot have more than one link to the same file in a project."),
                                        null,
                                        OLEMSGICON.OLEMSGICON_CRITICAL,
                                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                                return null;
                            }
                        } else {
                            Utilities.ShowMessageBox(
                                    Project.Site,
                                    String.Format("There is already a link to '{0}'. You cannot have more than one link to the same file in a project.", moniker),
                                    null,
                                    OLEMSGICON.OLEMSGICON_CRITICAL,
                                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                            return null;
                        }
                    }

                    return new ReparentLinkedFileAddition(Project, targetFolder, moniker);
                }

                string newPath = Path.Combine(targetFolder, Path.GetFileName(moniker));
                if (File.Exists(newPath) &&  
                    CommonUtils.IsSamePath(
                        NativeMethods.GetAbsolutePathToDirectory(newPath), 
                        NativeMethods.GetAbsolutePathToDirectory(moniker))) {
                    newPath = GetCopyName(newPath);
                }

                bool ok = false;
                if (DropEffect == DropEffect.Move && Utilities.IsSameComObject(project, Project)) {
                    if (existingChild != null && existingChild.ItemNode != null && existingChild.ItemNode.IsExcluded) {
                        // https://nodejstools.codeplex.com/workitem/271
                        // The item is excluded, so we don't need to ask if we can rename it.
                        ok = true;
                    } else {
                        ok = Project.Tracker.CanRenameItem(moniker, newPath, VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_NoFlags);
                    }
                } else {
                    ok = Project.Tracker.CanAddItems(
                        new[] { newPath },
                        new VSQUERYADDFILEFLAGS[] { VSQUERYADDFILEFLAGS.VSQUERYADDFILEFLAGS_NoFlags });
                }

                if (ok) {
                    if (File.Exists(newPath)) {
                        if (DropEffect == DropEffect.Move &&
                            Utilities.IsSameComObject(project, Project) &&
                            Project.FindNodeByFullPath(newPath) != null) {
                            // if we're overwriting an item, we're moving it, make sure that's ok.
                            // OverwriteFileAddition will handle the remove from the hierarchy
                            if (!Project.Tracker.CanRemoveItems(new[] { newPath }, new[] { VSQUERYREMOVEFILEFLAGS.VSQUERYREMOVEFILEFLAGS_NoFlags })) {
                                return null;
                            }
                        }
                        bool? overwrite = OverwriteAllItems;

                        if (overwrite == null) {
                            OverwriteFileDialog dialog;
                            if (!PromptOverwriteFile(moniker, out dialog)) {
                                return null;
                            }

                            overwrite = dialog.ShouldOverwrite;

                            if (dialog.AllItems) {
                                OverwriteAllItems = overwrite;
                            }
                        }

                        if (overwrite.Value) {
                            return new OverwriteFileAddition(Project, targetFolder, DropEffect, moniker, Path.GetFileName(newPath), project);
                        } else {
                            return SkipOverwriteAddition.Instance;
                        }
                    } else if (Directory.Exists(newPath)) {
                        Utilities.ShowMessageBox(
                            Project.Site,
                            SR.GetString(SR.DirectoryExists, CommonUtils.GetFileOrDirectoryName(newPath)),
                            null,
                            OLEMSGICON.OLEMSGICON_CRITICAL,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        return null;
                    }

                    if (newPath.Length >= NativeMethods.MAX_PATH) {
                        Utilities.ShowMessageBox(
                            Project.Site,
                            SR.GetString(SR.PathTooLongShortMessage),
                            null,
                            OLEMSGICON.OLEMSGICON_CRITICAL,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        return null;
                    }
                    return new FileAddition(Project, targetFolder, DropEffect, moniker, Path.GetFileName(newPath), project);
                }
                return null;
            }

            /// <summary>
            /// Prompts if the file should be overwriten.  Returns false if the user cancels, true if the user answered yes/no
            /// </summary>
            /// <param name="filename"></param>
            /// <param name="dialog"></param>
            /// <returns></returns>
            private static bool PromptOverwriteFile(string filename, out OverwriteFileDialog dialog) {
                dialog = new OverwriteFileDialog(SR.GetString(SR.FileAlreadyExists, Path.GetFileName(filename)), true);
                dialog.Owner = Application.Current.MainWindow;
                bool? dialogResult = dialog.ShowDialog();

                if (dialogResult != null && !dialogResult.Value) {
                    // user cancelled
                    return false;
                }
                return true;
            }

            private bool IsBadMove(string targetFolder, string moniker, bool file) {
                if (TargetNode.GetMkDocument() == moniker) {
                    // we are moving the file onto it's self.  If it's a single file via mouse
                    // we'll ignore it.  If it's multiple files, or a cut and paste, then we'll
                    // report the error.
                    if (ProjectReferences.Length > 1 || !MouseDropping) {
                        CannotMoveSameLocation(moniker);
                    }
                    return true;
                }

                if ((file || !MouseDropping) &&
                    Directory.Exists(targetFolder) &&
                    CommonUtils.IsSameDirectory(Path.GetDirectoryName(moniker), targetFolder)) {
                    // we're moving a file into it's own folder, report an error.
                    CannotMoveSameLocation(moniker);
                    return true;
                }
                return false;
            }

            private void CannotMoveSameLocation(string moniker) {
                Utilities.ShowMessageBox(
                    Project.Site,
                    SR.GetString(SR.CannotMoveIntoSameDirectory, CommonUtils.GetFileOrDirectoryName(moniker)),
                    null,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }

            private bool IsOurProject(IVsProject project) {
                string projectDoc;
                project.GetMkDocument((uint)VSConstants.VSITEMID.Root, out projectDoc);
                return projectDoc == Project.Url;
            }

            abstract class Addition {
                public abstract void DoAddition(ref bool? overwrite);
            }

            /// <summary>
            /// Addition which doesn't add anything.  It's used when the user answers no to
            /// overwriting a file, which results in us reporting an overall failure to the 
            /// copy and paste.  This causes the file not to be deleted if it was a move.
            /// 
            /// This also means that if you're moving multiple files and answer no to one 
            /// of them but not the other that the files are not removed from the source
            /// hierarchy.
            /// </summary>
            class SkipOverwriteAddition : Addition {
                internal static SkipOverwriteAddition Instance = new SkipOverwriteAddition();

                public override void DoAddition(ref bool? overwrite) {
                }
            }

            class ReparentLinkedFileAddition : Addition {
                private readonly ProjectNode Project;
                private readonly string TargetFolder;
                private readonly string Moniker;

                public ReparentLinkedFileAddition(ProjectNode project, string targetFolder, string moniker) {
                    Project = project;
                    TargetFolder = targetFolder;
                    Moniker = moniker;
                }

                public override void DoAddition(ref bool? overwrite) {
                    var existing = Project.FindNodeByFullPath(Moniker);
                    bool created = false;
                    if (existing != null) {
                        Project.OnItemDeleted(existing);
                        existing.Parent.RemoveChild(existing);
                        Project.Site.GetUIThread().MustBeCalledFromUIThread();
                        existing.ID = Project.ItemIdMap.Add(existing);
                    } else {
                        existing = Project.CreateFileNode(Moniker);
                        created = true;
                    }


                    var newParent = TargetFolder == Project.ProjectHome ? Project : Project.FindNodeByFullPath(TargetFolder);
                    newParent.AddChild(existing);
                    if (Project.ItemsDraggedOrCutOrCopied != null) {
                        Project.ItemsDraggedOrCutOrCopied.Remove(existing); // we don't need to remove the file after Paste
                    }

                    var link = existing.ItemNode.GetMetadata(ProjectFileConstants.Link);
                    if (link != null || created) {
                        // update the link to the new location within solution explorer
                        existing.ItemNode.SetMetadata(
                            ProjectFileConstants.Link,
                            Path.Combine(
                                CommonUtils.GetRelativeDirectoryPath(
                                    Project.ProjectHome,
                                    TargetFolder
                                ),
                                Path.GetFileName(Moniker)
                            )
                        );
                    }
                }
            }

            class FileAddition : Addition {
                public readonly ProjectNode Project;
                public readonly string TargetFolder;
                public readonly DropEffect DropEffect;
                public readonly string SourceMoniker;
                public readonly IVsProject SourceHierarchy;
                public readonly string NewFileName;

                public FileAddition(ProjectNode project, string targetFolder, DropEffect dropEffect, string sourceMoniker, string newFileName, IVsProject sourceHierarchy) {
                    Project = project;
                    TargetFolder = targetFolder;
                    DropEffect = dropEffect;
                    SourceMoniker = sourceMoniker;
                    SourceHierarchy = sourceHierarchy;
                    NewFileName = newFileName;
                }

                public override void DoAddition(ref bool? overwrite) {
                    string newPath = Path.Combine(TargetFolder, NewFileName);

                    DirectoryInfo dirInfo = null;                    
                        
                    try {
                        dirInfo = Directory.CreateDirectory(TargetFolder);
                    } catch (ArgumentException) {
                    } catch (UnauthorizedAccessException) {
                    } catch (IOException) {
                    } catch (NotSupportedException) {
                    }

                    if (dirInfo == null) {
                        //Something went wrong and we failed to create the new directory
                        //   Inform the user and cancel the addition
                        Utilities.ShowMessageBox(
                                            Project.Site,
                                            SR.GetString(SR.FolderCannotBeCreatedOnDisk, CommonUtils.GetFileOrDirectoryName(TargetFolder)),
                                            null,
                                            OLEMSGICON.OLEMSGICON_CRITICAL,
                                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        return;
                    }
                    

                    if (DropEffect == DropEffect.Move && Utilities.IsSameComObject(Project, SourceHierarchy)) {
                        // we are doing a move, we need to remove the old item, and add the new.
                        // This also allows us to have better behavior if the user is selectively answering
                        // no to files within the hierarchy.  We can do the rename of the individual items
                        // which the user opts to move and not touch the ones they don't.  With a cross
                        // hierarchy move if the user answers no to any of the items none of the items
                        // are removed from the source hierarchy.
                        var fileNode = Project.FindNodeByFullPath(SourceMoniker);
                        Debug.Assert(fileNode is FileNode);

                        Project.ItemsDraggedOrCutOrCopied.Remove(fileNode); // we don't need to remove the file after Paste                        

                        if (File.Exists(newPath)) {
                            // we checked before starting the copy, but somehow a file has snuck in.  Could be a race,
                            // or the user could have cut and pasted 2 files from different folders into the same folder.
                            bool shouldOverwrite;
                            if (overwrite == null) {
                                OverwriteFileDialog dialog;
                                if (!PromptOverwriteFile(Path.GetFileName(newPath), out dialog)) {
                                    // user cancelled
                                    fileNode.ExpandItem(EXPANDFLAGS.EXPF_UnCutHighlightItem);
                                    throw new CancelPasteException();
                                }

                                if (dialog.AllItems) {
                                    overwrite = dialog.ShouldOverwrite;
                                }

                                shouldOverwrite = dialog.ShouldOverwrite;
                            } else {
                                shouldOverwrite = overwrite.Value;
                            }

                            if (!shouldOverwrite) {
                                fileNode.ExpandItem(EXPANDFLAGS.EXPF_UnCutHighlightItem);
                                return;
                            }

                            var existingNode = Project.FindNodeByFullPath(newPath);
                            if (existingNode != null) {
                                existingNode.Remove(true);
                            } else {
                                File.Delete(newPath);
                            }
                        }

                        FileNode file = fileNode as FileNode;
                        file.RenameInStorage(fileNode.Url, newPath);
                        file.RenameFileNode(fileNode.Url, newPath);

                        Project.Tracker.OnItemRenamed(SourceMoniker, newPath, VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_NoFlags);
                    } else {
                        // we are copying and adding a new file node
                        File.Copy(SourceMoniker, newPath, true);

                        // best effort to reset the ReadOnly attribute
                        try {
                            File.SetAttributes(newPath, File.GetAttributes(newPath) & ~FileAttributes.ReadOnly);
                        } catch (ArgumentException) {
                        } catch (UnauthorizedAccessException) {
                        } catch (IOException) {
                        }

                        var existing = Project.FindNodeByFullPath(newPath);
                        if (existing == null) {
                            var fileNode = Project.CreateFileNode(newPath);
                            if (String.Equals(TargetFolder, Project.FullPathToChildren, StringComparison.OrdinalIgnoreCase)) {
                                Project.AddChild(fileNode);
                            } else {
                                var targetFolder = Project.CreateFolderNodes(TargetFolder);

                                //If a race occurrs simply treat the source as a non-included item
                                bool wasMemberItem = false;
                                var sourceItem = Project.FindNodeByFullPath(SourceMoniker);
                                if (sourceItem != null) {
                                    wasMemberItem = !sourceItem.IsNonMemberItem;
                                }

                                if (wasMemberItem && targetFolder.IsNonMemberItem) {
                                    // dropping/pasting folder into non-member folder, non member folder
                                    // should get included into the project.
                                    ErrorHandler.ThrowOnFailure(targetFolder.IncludeInProject(false));
                                }

                                targetFolder.AddChild(fileNode);
                                if (!wasMemberItem) {
                                    // added child by default is included,
                                    //   non-member copies are not added to the project
                                    ErrorHandler.ThrowOnFailure(fileNode.ExcludeFromProject());
                                }
                            }
                            Project.tracker.OnItemAdded(fileNode.Url, VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags);
                        } else if (existing.IsNonMemberItem) {
                            // replacing item that already existed, just include it in the project.
                            existing.IncludeInProject(false);
                        }
                    }
                }
            }

            class OverwriteFileAddition : FileAddition {
                public OverwriteFileAddition(ProjectNode project, string targetFolder, DropEffect dropEffect, string sourceMoniker, string newFileName, IVsProject sourceHierarchy)
                    : base(project, targetFolder, dropEffect, sourceMoniker, newFileName, sourceHierarchy) {
                }

                public override void DoAddition(ref bool? overwrite) {
                    if (DropEffect == DropEffect.Move) {
                        // File.Move won't overwrite, do it now.
                        File.Delete(Path.Combine(TargetFolder, Path.GetFileName(NewFileName)));

                        HierarchyNode existingNode;
                        if (Utilities.IsSameComObject(SourceHierarchy, Project) &&
                            (existingNode = Project.FindNodeByFullPath(Path.Combine(TargetFolder, NewFileName))) != null) {
                            // remove the existing item from the hierarchy, base.DoAddition will add a new one
                            existingNode.Remove(true);
                        }
                    }
                    base.DoAddition(ref overwrite);
                }
            }
        }

        /// <summary>
        /// Add an existing item (file/folder) to the project if it already exist in our storage.
        /// </summary>
        /// <param name="parentNode">Node to that this item to</param>
        /// <param name="name">Name of the item being added</param>
        /// <param name="targetPath">Path of the item being added</param>
        /// <returns>Node that was added</returns>
        protected virtual HierarchyNode AddNodeIfTargetExistInStorage(HierarchyNode parentNode, string name, string targetPath) {
            if (parentNode == null) {
                return null;
            }

            HierarchyNode newNode = parentNode;
            // If the file/directory exist, add a node for it
            if (File.Exists(targetPath)) {
                VSADDRESULT[] result = new VSADDRESULT[1];
                ErrorHandler.ThrowOnFailure(this.AddItem(parentNode.ID, VSADDITEMOPERATION.VSADDITEMOP_OPENFILE, name, 1, new string[] { targetPath }, IntPtr.Zero, result));
                if (result[0] != VSADDRESULT.ADDRESULT_Success)
                    throw new Exception();
                newNode = this.FindNodeByFullPath(targetPath);
                if (newNode == null)
                    throw new Exception();
            } else if (Directory.Exists(targetPath)) {
                newNode = this.CreateFolderNodes(targetPath);
            }
            return newNode;
        }

        #endregion

        #region non-virtual methods
        /// <summary>
        /// Handle the Cut operation to the clipboard
        /// </summary>
        protected internal int CutToClipboard() {
            int returnValue = (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;

            this.RegisterClipboardNotifications(true);

            // Create our data object and change the selection to show item(s) being cut
            IOleDataObject dataObject = this.PackageSelectionDataObject(true);
            if (dataObject != null) {
                _copyCutState = CopyCutState.Cut;

                // Add our cut item(s) to the clipboard
                Site.GetClipboardService().SetClipboard(dataObject);

                // Inform VS (UiHierarchyWindow) of the cut
                IVsUIHierWinClipboardHelper clipboardHelper = (IVsUIHierWinClipboardHelper)GetService(typeof(SVsUIHierWinClipboardHelper));
                if (clipboardHelper == null) {
                    return VSConstants.E_FAIL;
                }

                returnValue = ErrorHandler.ThrowOnFailure(clipboardHelper.Cut(dataObject));
            }

            return returnValue;
        }

        /// <summary>
        /// Handle the Copy operation to the clipboard
        /// </summary>
        protected internal int CopyToClipboard() {
            int returnValue = (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
            this.RegisterClipboardNotifications(true);

            // Create our data object and change the selection to show item(s) being copy
            IOleDataObject dataObject = this.PackageSelectionDataObject(false);
            if (dataObject != null) {
                _copyCutState = CopyCutState.Copied;

                // Add our copy item(s) to the clipboard
                Site.GetClipboardService().SetClipboard(dataObject);

                // Inform VS (UiHierarchyWindow) of the copy
                IVsUIHierWinClipboardHelper clipboardHelper = (IVsUIHierWinClipboardHelper)GetService(typeof(SVsUIHierWinClipboardHelper));
                if (clipboardHelper == null) {
                    return VSConstants.E_FAIL;
                }
                returnValue = ErrorHandler.ThrowOnFailure(clipboardHelper.Copy(dataObject));
            }
            return returnValue;
        }

        /// <summary>
        /// Handle the Paste operation to a targetNode
        /// </summary>
        protected internal int PasteFromClipboard(HierarchyNode targetNode) {
            int returnValue = (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;

            if (targetNode == null) {
                return VSConstants.E_INVALIDARG;
            }

            //Get the clipboardhelper service and use it after processing dataobject
            IVsUIHierWinClipboardHelper clipboardHelper = (IVsUIHierWinClipboardHelper)GetService(typeof(SVsUIHierWinClipboardHelper));
            if (clipboardHelper == null) {
                return VSConstants.E_FAIL;
            }

            try {
                //Get dataobject from clipboard
                IOleDataObject dataObject = Site.GetClipboardService().GetClipboard();
                if (dataObject == null) {
                    return VSConstants.E_UNEXPECTED;
                }

                DropEffect dropEffect = DropEffect.None;
                DropDataType dropDataType = DropDataType.None;
                try {
                    // if we didn't initiate the cut, default to Move.  If we're dragging to another
                    // project then their IVsUIHierWinClipboardHelperEvents.OnPaste method will
                    // check both the drop effect AND whether or not a cut was initiated, and only
                    // do a move if both are true.  Otherwise if we have a value non-None _copyCurState the
                    // cut/copy initiated from within our project system and we're now pasting
                    // back into ourselves, so we should simply respect it's value.
                    dropEffect = _copyCutState == CopyCutState.Copied ? DropEffect.Copy : DropEffect.Move;
                    dropDataType = this.ProcessSelectionDataObject(dataObject, targetNode, false, dropEffect);
                    if (dropDataType == DropDataType.None) {
                        dropEffect = DropEffect.None;
                    }
                } catch (ExternalException e) {
                    Trace.WriteLine("Exception : " + e.Message);

                    // If it is a drop from windows and we get any kind of error ignore it. This
                    // prevents bogus messages from the shell from being displayed
                    if (dropDataType != DropDataType.Shell) {
                        throw;
                    }
                } finally {
                    // Inform VS (UiHierarchyWindow) of the paste 
                    returnValue = clipboardHelper.Paste(dataObject, (uint)dropEffect);
                }
            } catch (COMException e) {
                Trace.WriteLine("Exception : " + e.Message);

                returnValue = e.ErrorCode;
            }

            return returnValue;
        }

        /// <summary>
        /// Determines if the paste command should be allowed.
        /// </summary>
        /// <returns></returns>
        protected internal bool AllowPasteCommand() {
            try {
                IOleDataObject dataObject = Site.GetClipboardService().GetClipboard();
                if (dataObject == null) {
                    return false;
                }

                // First see if this is a set of storage based items
                FORMATETC format = DragDropHelper.CreateFormatEtc((ushort)DragDropHelper.CF_VSSTGPROJECTITEMS);
                if (dataObject.QueryGetData(new FORMATETC[] { format }) == VSConstants.S_OK)
                    return true;
                // Try reference based items
                format = DragDropHelper.CreateFormatEtc((ushort)DragDropHelper.CF_VSREFPROJECTITEMS);
                if (dataObject.QueryGetData(new FORMATETC[] { format }) == VSConstants.S_OK)
                    return true;
                // Try windows explorer files format
                format = DragDropHelper.CreateFormatEtc((ushort)NativeMethods.CF_HDROP);
                return (dataObject.QueryGetData(new FORMATETC[] { format }) == VSConstants.S_OK);
            }
                // We catch External exceptions since it might be that it is not our data on the clipboard.
            catch (ExternalException e) {
                Trace.WriteLine("Exception :" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Register/Unregister for Clipboard events for the UiHierarchyWindow (solution explorer)
        /// </summary>
        /// <param name="register">true for register, false for unregister</param>
        protected internal void RegisterClipboardNotifications(bool register) {
            // Get the UiHierarchy window clipboard helper service
            IVsUIHierWinClipboardHelper clipboardHelper = (IVsUIHierWinClipboardHelper)GetService(typeof(SVsUIHierWinClipboardHelper));
            if (clipboardHelper == null) {
                return;
            }

            if (register && this.copyPasteCookie == 0) {
                // Register
                ErrorHandler.ThrowOnFailure(clipboardHelper.AdviseClipboardHelperEvents(this, out this.copyPasteCookie));
                Debug.Assert(this.copyPasteCookie != 0, "AdviseClipboardHelperEvents returned an invalid cookie");
            } else if (!register && this.copyPasteCookie != 0) {
                // Unregister
                ErrorHandler.ThrowOnFailure(clipboardHelper.UnadviseClipboardHelperEvents(this.copyPasteCookie));
                this.copyPasteCookie = 0;
            }
        }

        /// <summary>
        /// Process dataobject from Drag/Drop/Cut/Copy/Paste operation
        /// 
        /// drop indicates if it is a drag/drop or a cut/copy/paste.
        /// </summary>
        /// <remarks>The targetNode is set if the method is called from a drop operation, otherwise it is null</remarks>
        internal DropDataType ProcessSelectionDataObject(IOleDataObject dataObject, HierarchyNode targetNode, bool drop, DropEffect dropEffect) {
            Utilities.ArgumentNotNull("targetNode", targetNode);

            DropDataType dropDataType = DropDataType.None;
            bool isWindowsFormat = false;

            // Try to get it as a directory based project.
            List<string> filesDropped = DragDropHelper.GetDroppedFiles(DragDropHelper.CF_VSSTGPROJECTITEMS, dataObject, out dropDataType);
            if (filesDropped.Count == 0) {
                filesDropped = DragDropHelper.GetDroppedFiles(DragDropHelper.CF_VSREFPROJECTITEMS, dataObject, out dropDataType);
            }
            if (filesDropped.Count == 0) {
                filesDropped = DragDropHelper.GetDroppedFiles(NativeMethods.CF_HDROP, dataObject, out dropDataType);
                isWindowsFormat = (filesDropped.Count > 0);
            }

            if (dropDataType != DropDataType.None && filesDropped.Count > 0) {
                string[] filesDroppedAsArray = filesDropped.ToArray();

                HierarchyNode node = targetNode;

                // For directory based projects the content of the clipboard is a double-NULL terminated list of Projref strings.
                if (isWindowsFormat) {
                    DropFilesOrFolders(filesDroppedAsArray, node);

                    return dropDataType;
                } else {
                    if (AddFilesFromProjectReferences(node, filesDroppedAsArray, drop, dropEffect)) {
                        return dropDataType;
                    }
                }
            }

            // If we reached this point then the drop data must be set to None.
            // Otherwise the OnPaste will be called with a valid DropData and that would actually delete the item.
            return DropDataType.None;
        }

        internal void DropFilesOrFolders(string[] filesDropped, HierarchyNode ontoNode) {
            var waitDialog = (IVsThreadedWaitDialog)Site.GetService(typeof(SVsThreadedWaitDialog));
            int waitResult = waitDialog.StartWaitDialog(
                "Adding files and folders...",
                "Adding files to your project, this may take several seconds...",
                null,
                0,
                null,
                null
            );
            try {
                ontoNode = ontoNode.GetDragTargetHandlerNode();
                string nodePath = ontoNode.FullPathToChildren;
                bool droppingExistingDirectory = true;
                foreach (var droppedFile in filesDropped) {
                    if (!Directory.Exists(droppedFile) ||
                        !String.Equals(Path.GetDirectoryName(droppedFile), nodePath, StringComparison.OrdinalIgnoreCase)) {
                        droppingExistingDirectory = false;
                        break;
                    }
                }

                if (droppingExistingDirectory) {
                    // we're dragging a directory/directories that already exist
                    // into the location where they exist, we can do this via a fast path,
                    // and pop up a nice progress bar.
                    AddExistingDirectories(ontoNode, filesDropped);
                } else {
                    foreach (var droppedFile in filesDropped) {
                        if (Directory.Exists(droppedFile) &&
                            CommonUtils.IsSubpathOf(droppedFile, nodePath)) {
                            int cancelled = 0;
                            waitDialog.EndWaitDialog(ref cancelled);
                            waitResult = VSConstants.E_FAIL; // don't end twice

                            Utilities.ShowMessageBox(
                                Site,
                                SR.GetString(SR.CannotAddAsDescendantOfSelf, CommonUtils.GetFileOrDirectoryName(droppedFile)),
                                null,
                                OLEMSGICON.OLEMSGICON_CRITICAL,
                                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                            return;
                        }
                    }

                    // This is the code path when source is windows explorer
                    VSADDRESULT[] vsaddresults = new VSADDRESULT[1];
                    vsaddresults[0] = VSADDRESULT.ADDRESULT_Failure;
                    int addResult = AddItem(ontoNode.ID, VSADDITEMOPERATION.VSADDITEMOP_OPENFILE, null, (uint)filesDropped.Length, filesDropped, IntPtr.Zero, vsaddresults);
                    if (addResult != VSConstants.S_OK && addResult != VSConstants.S_FALSE && addResult != (int)OleConstants.OLECMDERR_E_CANCELED
                        && vsaddresults[0] != VSADDRESULT.ADDRESULT_Success) {
                        ErrorHandler.ThrowOnFailure(addResult);
                    }
                }
            } finally {
                if (ErrorHandler.Succeeded(waitResult)) {
                    int cancelled = 0;
                    waitDialog.EndWaitDialog(ref cancelled);
                }
            }
        }

        internal void AddExistingDirectories(HierarchyNode node, string[] filesDropped) {
            List<KeyValuePair<HierarchyNode, HierarchyNode>> addedItems = new List<KeyValuePair<HierarchyNode, HierarchyNode>>();

            var oldTriggerFlag = this.EventTriggeringFlag;
            EventTriggeringFlag |= ProjectNode.EventTriggering.DoNotTriggerHierarchyEvents;
            try {

                foreach (var dir in filesDropped) {
                    AddExistingDirectory(GetOrAddDirectory(node, addedItems, dir), dir, addedItems);
                }
            } finally {
                EventTriggeringFlag = oldTriggerFlag;
            }

            if (addedItems.Count > 0) {
                foreach (var item in addedItems) {
                    OnItemAdded(item.Key, item.Value);
                    this.tracker.OnItemAdded(item.Value.Url, VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags);
                }
                OnInvalidateItems(node);
            }
        }

        private void AddExistingDirectory(HierarchyNode node, string path, List<KeyValuePair<HierarchyNode, HierarchyNode>> addedItems) {
            foreach (var dir in Directory.GetDirectories(path)) {
                var existingDir = GetOrAddDirectory(node, addedItems, dir);

                AddExistingDirectory(existingDir, dir, addedItems);
            }

            foreach (var file in Directory.GetFiles(path)) {
                var existingFile = node.FindImmediateChildByName(Path.GetFileName(file));
                if (existingFile == null) {
                    existingFile = CreateFileNode(file);
                    addedItems.Add(new KeyValuePair<HierarchyNode, HierarchyNode>(node, existingFile));
                    node.AddChild(existingFile);
                }
            }
        }

        private HierarchyNode GetOrAddDirectory(HierarchyNode node, List<KeyValuePair<HierarchyNode, HierarchyNode>> addedItems, string dir) {
            var existingDir = node.FindImmediateChildByName(Path.GetFileName(dir));
            if (existingDir == null) {
                existingDir = CreateFolderNode(dir);
                addedItems.Add(new KeyValuePair<HierarchyNode, HierarchyNode>(node, existingDir));
                node.AddChild(existingDir);
            }
            return existingDir;
        }

        /// <summary>
        /// Get the dropdatatype from the dataobject
        /// </summary>
        /// <param name="pDataObject">The dataobject to be analysed for its format</param>
        /// <returns>dropdatatype or none if dataobject does not contain known format</returns>
        internal static DropDataType QueryDropDataType(IOleDataObject pDataObject) {
            if (pDataObject == null) {
                return DropDataType.None;
            }

            // known formats include File Drops (as from WindowsExplorer),
            // VSProject Reference Items and VSProject Storage Items.
            FORMATETC fmt = DragDropHelper.CreateFormatEtc(NativeMethods.CF_HDROP);

            if (DragDropHelper.QueryGetData(pDataObject, ref fmt) == VSConstants.S_OK) {
                return DropDataType.Shell;
            }

            fmt.cfFormat = DragDropHelper.CF_VSREFPROJECTITEMS;
            if (DragDropHelper.QueryGetData(pDataObject, ref fmt) == VSConstants.S_OK) {
                // Data is from a Ref-based project.
                return DropDataType.VsRef;
            }

            fmt.cfFormat = DragDropHelper.CF_VSSTGPROJECTITEMS;
            if (DragDropHelper.QueryGetData(pDataObject, ref fmt) == VSConstants.S_OK) {
                return DropDataType.VsStg;
            }

            return DropDataType.None;
        }

        /// <summary>
        /// Returns the drop effect.
        /// </summary>
        /// <remarks>
        /// // A directory based project should perform as follow:
        ///		NO MODIFIER 
        ///			- COPY if not from current hierarchy, 
        ///			- MOVE if from current hierarchy
        ///		SHIFT DRAG - MOVE
        ///		CTRL DRAG - COPY
        ///		CTRL-SHIFT DRAG - NO DROP (used for reference based projects only)
        /// </remarks>
        internal DropEffect QueryDropEffect(uint grfKeyState) {
            //Validate the dropdatatype
            if ((_dropType != DropDataType.Shell) && (_dropType != DropDataType.VsRef) && (_dropType != DropDataType.VsStg)) {
                return DropEffect.None;
            }

            // CTRL-SHIFT
            if ((grfKeyState & NativeMethods.MK_CONTROL) != 0 && (grfKeyState & NativeMethods.MK_SHIFT) != 0) {
                // Because we are not referenced base, we don't support link
                return DropEffect.None;
            }

            // CTRL
            if ((grfKeyState & NativeMethods.MK_CONTROL) != 0)
                return DropEffect.Copy;

            // SHIFT
            if ((grfKeyState & NativeMethods.MK_SHIFT) != 0)
                return DropEffect.Move;

            // no modifier
            if (_dragging) {
                // we are dragging from our project to our project, default to a Move
                return DropEffect.Move;
            } else {
                // we are dragging, but we didn't initiate it, so it's cross project.  Default to
                // a copy.
                return DropEffect.Copy;
            }
        }

        /// <summary>
        /// Moves files from one part of our project to another.
        /// </summary>
        /// <param name="targetNode">the targetHandler node</param>
        /// <param name="projectReferences">List of projectref string</param>
        /// <returns>true if succeeded</returns>
        internal bool AddFilesFromProjectReferences(HierarchyNode targetNode, string[] projectReferences, bool mouseDropping, DropEffect dropEffect) {
            //Validate input
            Utilities.ArgumentNotNull("projectReferences", projectReferences);
            Utilities.CheckNotNull(targetNode);

            if (!QueryEditProjectFile(false)) {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            return new ProjectReferenceFileAdder(this, targetNode, projectReferences, mouseDropping, dropEffect).AddFiles();
        }


        #endregion

        #region private helper methods
        /// <summary>
        /// Empties all the data structures added to the clipboard and flushes the clipboard.
        /// </summary>
        private void CleanAndFlushClipboard() {
            var clippy = Site.GetClipboardService();
            IOleDataObject oleDataObject = clippy.GetClipboard();
            if (oleDataObject == null) {
                return;
            }


            string sourceProjectPath = DragDropHelper.GetSourceProjectPath(oleDataObject);

            if (!String.IsNullOrEmpty(sourceProjectPath) && CommonUtils.IsSamePath(sourceProjectPath, this.GetMkDocument())) {
                clippy.FlushClipboard();
                bool opened = false;
                try {
                    opened = clippy.OpenClipboard();
                    clippy.EmptyClipboard();
                } finally {
                    if (opened) {
                        clippy.CloseClipboard();
                    }
                }
            }
        }

        private IntPtr PackageSelectionData(StringBuilder sb, bool addEndFormatDelimiter) {
            if (sb == null || sb.ToString().Length == 0 || this.ItemsDraggedOrCutOrCopied.Count == 0) {
                return IntPtr.Zero;
            }

            // Double null at end.
            if (addEndFormatDelimiter) {
                if (sb.ToString()[sb.Length - 1] != '\0') {
                    sb.Append('\0');
                }
            }

            // We request unmanaged permission to execute the below.
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

            _DROPFILES df = new _DROPFILES();
            int dwSize = Marshal.SizeOf(df);
            Int16 wideChar = 0;
            int dwChar = Marshal.SizeOf(wideChar);
            int structSize = dwSize + ((sb.Length + 1) * dwChar);
            IntPtr ptr = Marshal.AllocHGlobal(structSize);
            df.pFiles = dwSize;
            df.fWide = 1;
            IntPtr data = IntPtr.Zero;
            try {
                data = UnsafeNativeMethods.GlobalLock(ptr);
                Marshal.StructureToPtr(df, data, false);
                IntPtr strData = new IntPtr((long)data + dwSize);
                DragDropHelper.CopyStringToHGlobal(sb.ToString(), strData, structSize);
            } finally {
                if (data != IntPtr.Zero)
                    UnsafeNativeMethods.GlobalUnLock(data);
            }

            return ptr;
        }

        #endregion

        /// <summary>
        /// Clears our current copy/cut state - happens after a paste
        /// </summary>
        private void ClearCopyCutState() {
            _copyCutState = CopyCutState.None;
        }


    }
}
