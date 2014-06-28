// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// WARNING: This code has not been reviewed for COM reference leaks. Review before activating.
#if UNUSED_NESTED_PROJECTS

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Net;
using MSBuild = Microsoft.Build.BuildEngine;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
using VSLangProj80;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    [CLSCompliant(false), ComVisible(true)]
    public class NestedProjectNode : HierarchyNode, IPropertyNotifySink
    {

        #region fields
        private IVsHierarchy nestedHierarchy;

        Guid projectInstanceGuid = Guid.Empty;

        private string projectName = String.Empty;

        private string projectPath = String.Empty;

        private ImageHandler imageHandler;

        /// <summary>
        /// Defines an object that will be a mutex for this object for synchronizing thread calls.
        /// </summary>
        private static volatile object Mutex = new object();

        /// <summary>
        /// Sets the dispose flag on the object.
        /// </summary>
        private bool isDisposed;

        // A cooike retrieved when advising on property chnanged events.
        private uint projectPropertyNotifySinkCookie;
        #endregion

        #region properties
        /*internal, but public for FSharp.Project.dll*/ public IVsHierarchy NestedHierarchy
        {
            get
            {
                return this.nestedHierarchy;
            }
        }
        #endregion

        #region virtual properties
        /// <summary>
        /// Returns the __VSADDVPFLAGS that will be passed in when calling AddVirtualProjectEx
        /// </summary>
        public /*protected, but public for FSharp.Project.dll*/ virtual uint VirtualProjectFlags
        {
            get { return 0; }
        }
        #endregion

        #region overridden properties
        /// <summary>
        /// The path of the nested project.
        /// </summary>
        public override string Url
        {
            get
            {
                return this.projectPath;
            }
        }

        /// <summary>
        /// The Caption of the nested project.
        /// </summary>
        public override string Caption
        {
            get
            {
                return Path.GetFileNameWithoutExtension(this.projectName);
            }
        }

        public override Guid ItemTypeGuid
        {
            get
            {
                return VSConstants.GUID_ItemType_SubProject;
            }
        }

        /// <summary>
        /// Defines whether a node can execute a command if in selection.
        /// We do this in order to let the nested project to handle the execution of its own commands.
        /// </summary>
        public override bool CanExecuteCommand
        {
            get
            {
                return false;
            }
        }

        public override int SortPriority
        {
            get { return DefaultSortOrderNode.NestedProjectNode; }
        }

        public /*protected, but public for FSharp.Project.dll*/ bool IsDisposed
        {
            get { return this.isDisposed; }
            set { this.isDisposed = value; }
        }



        #endregion

        #region ctor

        public /*protected, but public for FSharp.Project.dll*/ NestedProjectNode()
        {
        }

        public NestedProjectNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            this.IsExpanded = true;
        }
        #endregion

        #region IPropertyNotifySink Members
        /// <summary>
        /// Notifies a sink that the [bindable] property specified by dispID has changed. 
        /// If dispID is DISPID_UNKNOWN, then multiple properties have changed together. 
        /// The client (owner of the sink) should then retrieve the current value of each property of interest from the object that generated the notification.
        /// In our case we will care about the  VSLangProj80.VsProjPropId.VBPROJPROPID_FileName and update the changes in the parent project file.
        /// </summary>
        /// <param name="dispid">Dispatch identifier of the property that is about to change or DISPID_UNKNOWN if multiple properties are about to change.</param>
        public virtual void OnChanged(int dispid)
        {
            if (dispid == (int)VSLangProj80.VsProjPropId.VBPROJPROPID_FileName)
            {
                // Get the filename of the nested project. Inetead of asking the label on the nested we ask the filename, since the label might not yet been set.
                IVsProject3 nestedProject = this.nestedHierarchy as IVsProject3;

                if (nestedProject != null)
                {
                    string document;
                    nestedProject.GetMkDocument(VSConstants.VSITEMID_ROOT, out document);
                    this.RenameNestedProjectInParentProject(Path.GetFileNameWithoutExtension(document));

                    // We need to redraw the caption since for some reason, by intervining to the OnChanged event the Caption is not updated.
                    this.ReDraw(UIHierarchyElement.Caption);
                }
            }
        }

        /// <summary>
        /// Notifies a sink that a [requestedit] property is about to change and that the object is asking the sink how to proceed.
        /// </summary>
        /// <param name="dispid">Dispatch identifier of the property that is about to change or DISPID_UNKNOWN if multiple properties are about to change.</param>
        public virtual void OnRequestEdit(int dispid)
        {

        }

        #endregion

        #region public methods
        #endregion

        #region overridden methods

        /// <summary>
        /// Get the automation object for the NestedProjectNode
        /// </summary>
        /// <returns>An instance of the Automation.OANestedProjectItem type if succeded</returns>
        public override object GetAutomationObject()
        {
            //Validate that we are not disposed or the project is closing
            if (this.isDisposed || this.ProjectMgr == null || this.ProjectMgr.IsClosed)
            {
                return null;
            }

            return new Automation.OANestedProjectItem(this.ProjectMgr.GetAutomationObject() as Automation.OAProject, this);
        }

        /// <summary>
        /// Gets properties of a given node or of the hierarchy.
        /// </summary>
        /// <param name="propId">Identifier of the hierarchy property</param>
        /// <returns>It return an object which type is dependent on the propid.</returns>
        public override object GetProperty(int propId)
        {
            __VSHPROPID vshPropId = (__VSHPROPID)propId;
            switch (vshPropId)
            {
                default:
                    return base.GetProperty(propId);

                case __VSHPROPID.VSHPROPID_Expandable:
                    return true;

                case __VSHPROPID.VSHPROPID_BrowseObject:
                case __VSHPROPID.VSHPROPID_HandlesOwnReload:
                    return this.DelegateGetPropertyToNested(propId);
            }
        }


        /// <summary>
        /// Gets properties whose values are GUIDs.
        /// </summary>
        /// <param name="propid">Identifier of the hierarchy property</param>
        /// <param name="guid"> Pointer to a GUID property specified in propid</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public override int GetGuidProperty(int propid, out Guid guid)
        {
            guid = Guid.Empty;
            switch ((__VSHPROPID)propid)
            {
                case __VSHPROPID.VSHPROPID_ProjectIDGuid:
                    guid = this.projectInstanceGuid;
                    break;

                default:
                    return base.GetGuidProperty(propid, out guid);
            }

            CCITracing.TraceCall(String.Format(CultureInfo.CurrentCulture, "Guid for {0} property", propid));
            if (guid.CompareTo(Guid.Empty) == 0)
            {
                return VSConstants.DISP_E_MEMBERNOTFOUND;
            }

            return VSConstants.S_OK;
        }


        /// <summary>
        /// Determines whether the hierarchy item changed.
        /// </summary>
        /// <param name="itemId">Item identifier of the hierarchy item contained in VSITEMID</param>
        /// <param name="punkDocData">Pointer to the IUnknown interface of the hierarchy item. </param>
        /// <param name="pfDirty">TRUE if the hierarchy item changed.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public override int IsItemDirty(uint itemId, IntPtr punkDocData, out int pfDirty)
        {
            Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");
            Debug.Assert(punkDocData != IntPtr.Zero, "docData intptr was zero");

            // Get an IPersistFileFormat object from docData object 
            IPersistFileFormat persistFileFormat = Marshal.GetTypedObjectForIUnknown(punkDocData, typeof(IPersistFileFormat)) as IPersistFileFormat;
            Debug.Assert(persistFileFormat != null, "The docData object does not implement the IPersistFileFormat interface");

            // Call IsDirty on the IPersistFileFormat interface
            ErrorHandler.ThrowOnFailure(persistFileFormat.IsDirty(out pfDirty));

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Saves the hierarchy item to disk.
        /// </summary>
        /// <param name="dwSave">Flags whose values are taken from the VSSAVEFLAGS enumeration.</param>
        /// <param name="silentSaveAsName">File name to be applied when dwSave is set to VSSAVE_SilentSave. </param>
        /// <param name="itemid">Item identifier of the hierarchy item saved from VSITEMID. </param>
        /// <param name="punkDocData">Pointer to the IUnknown interface of the hierarchy item saved.</param>
        /// <param name="pfCancelled">TRUE if the save action was canceled. </param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public override int SaveItem(VSSAVEFLAGS dwSave, string silentSaveAsName, uint itemid, IntPtr punkDocData, out int pfCancelled)
        {
            // Don't ignore/unignore file changes 
            // Use Advise/Unadvise to work around rename situations
            try
            {
                this.StopObservingNestedProjectFile();
                Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");
                Debug.Assert(punkDocData != IntPtr.Zero, "docData intptr was zero");

                // Get an IPersistFileFormat object from docData object (we don't call release on punkDocData since did not increment its ref count)
                IPersistFileFormat persistFileFormat = Marshal.GetTypedObjectForIUnknown(punkDocData, typeof(IPersistFileFormat)) as IPersistFileFormat;
                Debug.Assert(persistFileFormat != null, "The docData object does not implement the IPersistFileFormat interface");

                IVsUIShell uiShell = this.GetService(typeof(SVsUIShell)) as IVsUIShell;
                string newName;
                uiShell.SaveDocDataToFile(dwSave, persistFileFormat, silentSaveAsName, out newName, out pfCancelled);

                // When supported do a rename of the nested project here 
            }
            finally
            {
                // Succeeded or not we must hook to the file change events
                // Don't ignore/unignore file changes 
                // Use Advise/Unadvise to work around rename situations
                this.ObserveNestedProjectFile();
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Gets the icon handle. It tries first the nested to get the icon handle. If that is not supported it will get it from
        /// the image list of the nested if that is supported. If neither of these is supported a default image will be shown.
        /// </summary>
        /// <returns>An object representing the icon.</returns>
        public override object GetIconHandle(bool open)
        {
            Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");

            object iconHandle = null;
            this.nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_IconHandle, out iconHandle);
            if (iconHandle == null)
            {
                if (null == imageHandler)
                {
                    InitImageHandler();
                }
                // Try to get an icon from the nested hierrachy image list.
                if (imageHandler.ImageList != null)
                {
                    object imageIndexAsObject = null;
                    if (this.nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_IconIndex, out imageIndexAsObject) == VSConstants.S_OK &&
                        imageIndexAsObject != null)
                    {
                        int imageIndex = (int)imageIndexAsObject;
                        if (imageIndex < imageHandler.ImageList.Images.Count)
                        {
                            iconHandle = imageHandler.GetIconHandle(imageIndex);
                        }
                    }
                }

                if (null == iconHandle)
                {
                    iconHandle = this.ProjectMgr.ImageHandler.GetIconHandle((int)ProjectNode.ImageName.Application);
                }
            }

            return iconHandle;
        }

        /// <summary>
        /// Return S_OK. Implementation of Closing a nested project is done in CloseNestedProject which is called by CloseChildren.
        /// </summary>
        /// <returns>S_OK</returns>
        public override int Close()
        {
            return VSConstants.S_OK;
        }


        /// <summary>
        /// Returns the moniker of the nested project.
        /// </summary>
        /// <returns></returns>
        public override string GetMkDocument()
        {
            Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");
            if (this.isDisposed || this.ProjectMgr == null || this.ProjectMgr.IsClosed)
            {
                return String.Empty;
            }

            return this.projectPath;
        }

        /// <summary>
        /// Called by the shell when a node has been renamed from the GUI
        /// </summary>
        /// <param name="label">The name of the new label.</param>
        /// <returns>A success or failure value.</returns>
        public override int SetEditLabel(string label)
        {
            int result = this.DelegateSetPropertyToNested((int)__VSHPROPID.VSHPROPID_EditLabel, label);
            if (ErrorHandler.Succeeded(result))
            {
                this.RenameNestedProjectInParentProject(label);
            }

            return result;
        }

        /// <summary>
        /// Called by the shell to get the node caption when the user tries to rename from the GUI
        /// </summary>
        /// <returns>the node cation</returns>
        public override string GetEditLabel()
        {
            return (string)this.DelegateGetPropertyToNested((int)__VSHPROPID.VSHPROPID_EditLabel);
        }      

        /// <summary>
        /// This is temporary until we have support for re-adding a nested item
        /// </summary>
        public /*protected, but public for FSharp.Project.dll*/ override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation)
        {
            return false;
        }

        /// <summary>
        /// Delegates the call to the inner hierarchy.
        /// </summary>
        /// <param name="reserved">Reserved parameter defined at the IVsPersistHierarchyItem2::ReloadItem parameter.</param>
        public /*protected, but public for FSharp.Project.dll*/ override void ReloadItem(uint reserved)
        {
            #region precondition
            if (this.isDisposed || this.ProjectMgr == null || this.ProjectMgr.IsClosed)
            {
                throw new InvalidOperationException();
            }

            Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");
            #endregion

            IVsPersistHierarchyItem2 persistHierachyItem = this.nestedHierarchy as IVsPersistHierarchyItem2;

            // We are expecting that if we get called then the nestedhierarchy supports IVsPersistHierarchyItem2, since then hierrachy should support handling its own reload.
            // There should be no errormessage to the user since this is an /*internal, but public for FSharp.Project.dll*/ public error, that it cannot be fixed at user level.
            if (persistHierachyItem == null)
            {
                throw new InvalidOperationException();
            }

            ErrorHandler.ThrowOnFailure(persistHierachyItem.ReloadItem(VSConstants.VSITEMID_ROOT, reserved));
        }

        /// <summary>
        /// Flag indicating that changes to a file can be ignored when item is saved or reloaded. 
        /// </summary>
        /// <param name="ignoreFlag">Flag indicating whether or not to ignore changes (1 to ignore, 0 to stop ignoring).</param>
        public /*protected, but public for FSharp.Project.dll*/ override void IgnoreItemFileChanges(bool ignoreFlag)
        {
            #region precondition
            if (this.isDisposed || this.ProjectMgr == null || this.ProjectMgr.IsClosed)
            {
                throw new InvalidOperationException();
            }

            Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");
            #endregion

            this.IgnoreNestedProjectFile(ignoreFlag);

            IVsPersistHierarchyItem2 persistHierachyItem = this.nestedHierarchy as IVsPersistHierarchyItem2;

            // If the IVsPersistHierarchyItem2 is not implemented by the nested just return
            if (persistHierachyItem == null)
            {
                return;
            }

            ErrorHandler.ThrowOnFailure(persistHierachyItem.IgnoreItemFileChanges(VSConstants.VSITEMID_ROOT, ignoreFlag ? 1 : 0));
        }

        /// <summary>
        /// Sets the VSADDFILEFLAGS that will be used to call the  IVsTrackProjectDocumentsEvents2 OnAddFiles
        /// </summary>
        /// <param name="files">The files to which an array of VSADDFILEFLAGS has to be specified.</param>
        /// <returns></returns>
        public /*protected, but public for FSharp.Project.dll*/ override VSADDFILEFLAGS[] GetAddFileFlags(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new VSADDFILEFLAGS[1] { VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags };
            }

            VSADDFILEFLAGS[] addFileFlags = new VSADDFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                addFileFlags[i] = VSADDFILEFLAGS.VSADDFILEFLAGS_IsNestedProjectFile;
            }

            return addFileFlags;
        }

        /// <summary>
        /// Sets the VSQUERYADDFILEFLAGS that will be used to call the  IVsTrackProjectDocumentsEvents2 OnQueryAddFiles
        /// </summary>
        /// <param name="files">The files to which an array of VSADDFILEFLAGS has to be specified.</param>
        /// <returns></returns>
        public /*protected, but public for FSharp.Project.dll*/ override VSQUERYADDFILEFLAGS[] GetQueryAddFileFlags(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new VSQUERYADDFILEFLAGS[1] { VSQUERYADDFILEFLAGS.VSQUERYADDFILEFLAGS_NoFlags };
            }

            VSQUERYADDFILEFLAGS[] queryAddFileFlags = new VSQUERYADDFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                queryAddFileFlags[i] = VSQUERYADDFILEFLAGS.VSQUERYADDFILEFLAGS_IsNestedProjectFile;
            }

            return queryAddFileFlags;
        }

        /// <summary>
        /// Sets the VSREMOVEFILEFLAGS that will be used to call the  IVsTrackProjectDocumentsEvents2 OnRemoveFiles
        /// </summary>
        /// <param name="files">The files to which an array of VSREMOVEFILEFLAGS has to be specified.</param>
        /// <returns></returns>
        public /*protected, but public for FSharp.Project.dll*/ override VSREMOVEFILEFLAGS[] GetRemoveFileFlags(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new VSREMOVEFILEFLAGS[1] { VSREMOVEFILEFLAGS.VSREMOVEFILEFLAGS_NoFlags };
            }

            VSREMOVEFILEFLAGS[] removeFileFlags = new VSREMOVEFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                removeFileFlags[i] = VSREMOVEFILEFLAGS.VSREMOVEFILEFLAGS_IsNestedProjectFile;
            }

            return removeFileFlags;
        }

        /// <summary>
        /// Sets the VSQUERYREMOVEFILEFLAGS that will be used to call the  IVsTrackProjectDocumentsEvents2 OnQueryRemoveFiles
        /// </summary>
        /// <param name="files">The files to which an array of VSQUERYREMOVEFILEFLAGS has to be specified.</param>
        /// <returns></returns>
        public /*protected, but public for FSharp.Project.dll*/ override VSQUERYREMOVEFILEFLAGS[] GetQueryRemoveFileFlags(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new VSQUERYREMOVEFILEFLAGS[1] { VSQUERYREMOVEFILEFLAGS.VSQUERYREMOVEFILEFLAGS_NoFlags };
            }

            VSQUERYREMOVEFILEFLAGS[] queryRemoveFileFlags = new VSQUERYREMOVEFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                queryRemoveFileFlags[i] = VSQUERYREMOVEFILEFLAGS.VSQUERYREMOVEFILEFLAGS_IsNestedProjectFile;
            }

            return queryRemoveFileFlags;
        }
        #endregion

        #region virtual methods
        /// <summary>
        /// Initialize the nested hierarhy node.
        /// </summary>
        /// <param name="fileNameParam">The file name of the nested project.</param>
        /// <param name="destinationParam">The location of the nested project.</param>
        /// <param name="projectNameParam">The name of the project.</param>
        /// <param name="createFlags">The nested project creation flags </param>
        /// <remarks>This methos should be called just after a NestedProjectNode object is created.</remarks>
        public virtual void Init(string fileNameParam, string destinationParam, string projectNameParam, __VSCREATEPROJFLAGS createFlags)
        {
            if (String.IsNullOrEmpty(fileNameParam))
            {
                throw new ArgumentException(SR.GetString(SR.ParameterCannotBeNullOrEmpty, CultureInfo.CurrentUICulture), "fileNameParam");
            }

            if (String.IsNullOrEmpty(destinationParam))
            {
                throw new ArgumentException(SR.GetString(SR.ParameterCannotBeNullOrEmpty, CultureInfo.CurrentUICulture), "destinationParam");
            }

            this.projectName = Path.GetFileName(fileNameParam);
            this.projectPath = Path.Combine(destinationParam, this.projectName);

            // get the IVsSolution interface from the global service provider
            IVsSolution solution = this.GetService(typeof(IVsSolution)) as IVsSolution;
            Debug.Assert(solution != null, "Could not get the IVsSolution object from the services exposed by this project");
            if (solution == null)
            {
                throw new InvalidOperationException();
            }

            // Get the project type guid from project element                
            string typeGuidString = this.ItemNode.GetMetadataAndThrow(ProjectFileConstants.TypeGuid, new InvalidOperationException());
            Guid projectFactoryGuid = Guid.Empty;
            if (!String.IsNullOrEmpty(typeGuidString))
            {
                projectFactoryGuid = new Guid(typeGuidString);
            }

            // Get the project factory.
            IVsProjectFactory projectFactory;
            ErrorHandler.ThrowOnFailure(solution.GetProjectFactory((uint)0, new Guid[] { projectFactoryGuid }, fileNameParam, out projectFactory));

            this.CreateProjectDirectory();

            //Create new project using factory
            int cancelled;
            Guid refiid = NativeMethods.IID_IUnknown;
            IntPtr projectPtr = IntPtr.Zero;
            
            try
            {
                ErrorHandler.ThrowOnFailure(projectFactory.CreateProject(fileNameParam, destinationParam, projectNameParam, (uint)createFlags, ref refiid, out projectPtr, out cancelled));

                if (projectPtr != IntPtr.Zero)
                {
                    this.nestedHierarchy = Marshal.GetTypedObjectForIUnknown(projectPtr, typeof(IVsHierarchy)) as IVsHierarchy;
                    Debug.Assert(this.nestedHierarchy != null, "Nested hierarchy could not be created");
                    Debug.Assert(cancelled == 0);
                }
            }
            finally
            {
                if (projectPtr != IntPtr.Zero)
                {
                    // We created a new instance of the project, we need to call release to decrement the ref count
                    // the RCW (this.nestedHierarchy) still has a reference to it which will keep it alive
                    Marshal.Release(projectPtr);
                }
            }

            if (cancelled != 0 && this.nestedHierarchy == null)
            {
                ErrorHandler.ThrowOnFailure(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            // Link into the nested VS hierarchy.
            ErrorHandler.ThrowOnFailure(this.nestedHierarchy.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ParentHierarchy, this.ProjectMgr));
            ErrorHandler.ThrowOnFailure(this.nestedHierarchy.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ParentHierarchyItemid, (object)(int)this.ID));

            this.LockRDTEntry();

            this.ConnectPropertyNotifySink();
        }

        /// <summary>
        /// Links a nested project as a virtual project to the solution.
        /// </summary>
        public /*protected internal, but public for FSharp.Project.dll*/ virtual void AddVirtualProject()
        {
            // This is the second step in creating and adding a nested project. The inner hierarchy must have been
            // already initialized at this point. 
            #region precondition
            if (this.nestedHierarchy == null)
            {
                throw new InvalidOperationException();
            }
            #endregion
            // get the IVsSolution interface from the global service provider
            IVsSolution solution = this.GetService(typeof(IVsSolution)) as IVsSolution;
            Debug.Assert(solution != null, "Could not get the IVsSolution object from the services exposed by this project");
            if (solution == null)
            {
                throw new InvalidOperationException();
            }

            this.InitializeInstanceGuid();

            // Add virtual project to solution.
            ErrorHandler.ThrowOnFailure(solution.AddVirtualProjectEx(this.nestedHierarchy, this.VirtualProjectFlags, ref this.projectInstanceGuid));

            // Now set up to listen on file changes on the nested project node.
            this.ObserveNestedProjectFile();
        }

        /// <summary>
        /// The method that does the cleanup.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // Everybody can go here.
            if (!this.isDisposed)
            {
                try
                {
                    // Synchronize calls to the Dispose simulteniously.
                    lock (Mutex)
                    {
                        if (disposing)
                        {
                            this.DisconnectPropertyNotifySink();
                            this.StopObservingNestedProjectFile();
                            this.imageHandler.Close();
                        }                        
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                    this.isDisposed = true;
                }
            }
        }

        /// <summary>
        /// Creates the project directory if it does not exist.
        /// </summary>
        /// <returns></returns>
        public /*protected, but public for FSharp.Project.dll*/ virtual void CreateProjectDirectory()
        {
            string directoryName = Path.GetDirectoryName(this.projectPath);

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }


        /// <summary>
        /// Lock the RDT Entry for the nested project.
        /// By default this document is marked as "Dont Save as". That means the menu File->SaveAs is disabled for the
        /// nested project node.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RDT")]
        public /*protected, but public for FSharp.Project.dll*/ virtual void LockRDTEntry()
        {
            // Define flags for the nested project document
            _VSRDTFLAGS flags = _VSRDTFLAGS.RDT_VirtualDocument | _VSRDTFLAGS.RDT_ProjSlnDocument; ;

            // Request the RDT service
            IVsRunningDocumentTable rdt = this.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            Debug.Assert(rdt != null, " Could not get running document table from the services exposed by this project");
            if (rdt == null)
            {
                throw new InvalidOperationException();
            }

            // First we see if someone else has opened the requested view of the file.
            uint itemid;
            IntPtr docData = IntPtr.Zero;
            IVsHierarchy ivsHierarchy;
            uint docCookie;
            IntPtr projectPtr = IntPtr.Zero;

            try
            {
                ErrorHandler.ThrowOnFailure(rdt.FindAndLockDocument((uint)flags, this.projectPath, out ivsHierarchy, out itemid, out docData, out docCookie));
                flags |= _VSRDTFLAGS.RDT_EditLock;

                if (ivsHierarchy != null && docCookie != (uint)ShellConstants.VSDOCCOOKIE_NIL)
                {
                    if (docCookie != this.DocCookie)
                    {
                        this.DocCookie = docCookie;
                    }
                }
                else
                {

                    // get inptr for hierarchy
                    projectPtr = Marshal.GetIUnknownForObject(this.nestedHierarchy);
                    Debug.Assert(projectPtr != IntPtr.Zero, " Project pointer for the nested hierarchy has not been initialized");
                    ErrorHandler.ThrowOnFailure(rdt.RegisterAndLockDocument((uint)flags, this.projectPath, this.ProjectMgr, this.ID, projectPtr, out docCookie));

                    this.DocCookie = docCookie;
                    Debug.Assert(this.DocCookie != (uint)ShellConstants.VSDOCCOOKIE_NIL, "Invalid cookie when registering document in the running document table.");

                    //we must also set the doc cookie on the nested hier
                    this.SetDocCookieOnNestedHier(this.DocCookie);
                }
            }
            finally
            {
                // Release all Inptr's that that were given as out pointers
                if (docData != IntPtr.Zero)
                {
                    Marshal.Release(docData);
                }
                if (projectPtr != IntPtr.Zero)
                {
                    Marshal.Release(projectPtr);
                }
            }

        }

        /// <summary>
        /// Unlock the RDT entry for the nested project
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RDT")]
        public /*protected, but public for FSharp.Project.dll*/ virtual void UnlockRDTEntry()
        {
            if (this.isDisposed || this.ProjectMgr == null || this.ProjectMgr.IsClosed)
            {
                return;
            }
            // First we see if someone else has opened the requested view of the file.
            IVsRunningDocumentTable rdt = this.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (rdt != null && this.DocCookie != (int)ShellConstants.VSDOCCOOKIE_NIL)
            {
                _VSRDTFLAGS flags = _VSRDTFLAGS.RDT_EditLock;

                ErrorHandler.ThrowOnFailure(rdt.UnlockDocument((uint)flags, (uint)this.DocCookie));
            }

            this.DocCookie = (int)ShellConstants.VSDOCCOOKIE_NIL;
        }

        /// <summary>
        /// Renames the project file in the parent project structure.
        /// </summary>
        /// <param name="label">The new label.</param>
        public /*protected, but public for FSharp.Project.dll*/ virtual void RenameNestedProjectInParentProject(string label)
        {
            string existingLabel = this.Caption;

            if (String.Compare(existingLabel, label, StringComparison.Ordinal) == 0)
            {
                return;
            }

            string oldFileName = this.projectPath;
            string oldPath = this.Url;

            try
            {
                this.StopObservingNestedProjectFile();
                this.ProjectMgr.SuspendMSBuild();

                // Check out the project file if necessary.
                if (!this.ProjectMgr.QueryEditProjectFile(false))
                {
                    throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
                }


                string newFileName = label + Path.GetExtension(oldFileName);
                this.SaveNestedProjectItemInProjectFile(newFileName);

                string projectDirectory = Path.GetDirectoryName(oldFileName);

                // update state.
                this.projectName = newFileName;
                this.projectPath = Path.Combine(projectDirectory, this.projectName);

                // Unload and lock the RDT entries
                this.UnlockRDTEntry();
                this.LockRDTEntry();

                // Since actually this is a rename in our hierarchy notify the tracker that a rename has happened.
                this.ProjectMgr.Tracker.OnItemRenamed(oldPath, this.projectPath, VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_IsNestedProjectFile);
            }
            finally
            {
                this.ObserveNestedProjectFile();
                this.ProjectMgr.ResumeMSBuild(this.ProjectMgr.ReEvaluateProjectFileTargetName);
            }
        }
        /// <summary>
        /// Saves the nested project information in the project file.
        /// </summary>
        /// <param name="newFileName"></param>
        public /*protected, but public for FSharp.Project.dll*/ virtual void SaveNestedProjectItemInProjectFile(string newFileName)
        {
            string existingInclude = MSBuildItem.GetEvaluatedInclude(this.ItemNode.Item);
            string existingRelativePath = Path.GetDirectoryName(existingInclude);
            string newRelativePath = Path.Combine(existingRelativePath, newFileName);
            this.ItemNode.Rename(newRelativePath);
        }
        #endregion    

        #region helper methods
        /// <summary>
        /// Closes a nested project and releases the nested hierrachy pointer.
        /// </summary>
        /*internal, but public for FSharp.Project.dll*/ public void CloseNestedProjectNode()
        {
            if (this.isDisposed || this.ProjectMgr == null || this.ProjectMgr.IsClosed)
            {
                return;
            }

            uint itemid = VSConstants.VSITEMID_NIL;
            try
            {
                this.DisconnectPropertyNotifySink();

                IVsUIHierarchy hier;

                IVsWindowFrame windowFrame;
                VsShellUtilities.IsDocumentOpen(this.ProjectMgr.Site, this.projectPath, Guid.Empty, out hier, out itemid, out windowFrame);


                if (itemid == VSConstants.VSITEMID_NIL)
                {
                    this.UnlockRDTEntry();
                }

                IVsSolution solution = this.GetService(typeof(IVsSolution)) as IVsSolution;
                if (solution == null)
                {
                    throw new InvalidOperationException();
                }

                ErrorHandler.ThrowOnFailure(solution.RemoveVirtualProject(this.nestedHierarchy, 0));

            }
            finally
            {
                this.StopObservingNestedProjectFile();

                // if we haven't already release the RDT cookie, do so now.
                if (itemid == VSConstants.VSITEMID_NIL)
                {
                    this.UnlockRDTEntry();
                }
                
                this.Dispose(true);
            }
        }

        private void InitializeInstanceGuid()
        {
            if (this.projectInstanceGuid != Guid.Empty)
            {
                return;
            }

            Guid instanceGuid = Guid.Empty;

            Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");

            // This method should be called from the open children method, then we can safely use the IsNewProject property
            if (this.ProjectMgr.IsNewProject)
            {
                instanceGuid = Guid.NewGuid();
                this.ItemNode.SetMetadata(ProjectFileConstants.InstanceGuid, instanceGuid.ToString("B"));
                this.nestedHierarchy.SetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, ref instanceGuid);

            }
            else
            {
                // Get a guid from the nested hiererachy.
                Guid nestedHiererachyInstanceGuid;
                this.nestedHierarchy.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out nestedHiererachyInstanceGuid);

                // Get instance guid from the project file. If it does not exist then we create one.
                string instanceGuidAsString = this.ItemNode.GetMetadata(ProjectFileConstants.InstanceGuid);

                // 1. nestedHiererachyInstanceGuid is empty and instanceGuidAsString is empty then create a new one.
                // 2. nestedHiererachyInstanceGuid is empty and instanceGuidAsString not empty use instanceGuidAsString and update the nested project object by calling SetGuidProperty.
                // 3. nestedHiererachyInstanceGuid is not empty instanceGuidAsString is empty then use nestedHiererachyInstanceGuid and update the outer project element.
                // 4. nestedHiererachyInstanceGuid is not empty instanceGuidAsString is empty then use nestedHiererachyInstanceGuid and update the outer project element.

                if (nestedHiererachyInstanceGuid == Guid.Empty && String.IsNullOrEmpty(instanceGuidAsString))
                {
                    instanceGuid = Guid.NewGuid();
                }
                else if (nestedHiererachyInstanceGuid == Guid.Empty && !String.IsNullOrEmpty(instanceGuidAsString))
                {
                    instanceGuid = new Guid(instanceGuidAsString);

                    this.nestedHierarchy.SetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, ref instanceGuid);
                }
                else if (nestedHiererachyInstanceGuid != Guid.Empty)
                {
                    instanceGuid = nestedHiererachyInstanceGuid;

                    // If the instanceGuidAsString is empty then creating a guid out of it would throw an exception.
                    if (String.IsNullOrEmpty(instanceGuidAsString) || nestedHiererachyInstanceGuid != new Guid(instanceGuidAsString))
                    {
                        this.ItemNode.SetMetadata(ProjectFileConstants.InstanceGuid, instanceGuid.ToString("B"));
                    }
                }
            }

            this.projectInstanceGuid = instanceGuid;
        }

        private void SetDocCookieOnNestedHier(uint itemDocCookie)
        {
            object docCookie = (int)itemDocCookie;
            this.nestedHierarchy.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ItemDocCookie, docCookie);
        }

        private void InitImageHandler()
        {
            Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");

            if (null == imageHandler)
            {
                imageHandler = new ImageHandler();
            }
            object imageListAsPointer = null;
            this.nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_IconImgList, out imageListAsPointer);
            if (imageListAsPointer != null)
            {
                this.imageHandler.ImageList = Utilities.GetImageList(imageListAsPointer);
            }
        }

        /// <summary>
        /// Delegates Getproperty calls to the inner nested.
        /// </summary>
        /// <param name="propID">The property to delegate.</param>
        /// <returns>The return of the GetProperty from nested.</returns>
        private object DelegateGetPropertyToNested(int propID)
        {
            object returnValue = null;
            if (!this.ProjectMgr.IsClosed)
            {
                Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");

                // Do not throw since some project types will return E_FAIL if they do not support a property.
                this.nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, propID, out returnValue);
            }
            return returnValue;
        }

        /// <summary>
        /// Delegates Setproperty calls to the inner nested.
        /// </summary>
        /// <param name="propID">The property to delegate.</param>
        /// <param name="value">The property to set.</param>
        /// <returns>The return of the SetProperty from nested.</returns>
        private int DelegateSetPropertyToNested(int propID, object value)
        {
            if (this.ProjectMgr.IsClosed)
            {
                return VSConstants.E_FAIL;
            }

            Debug.Assert(this.nestedHierarchy != null, "The nested hierarchy object must be created before calling this method");

            // Do not throw since some project types will return E_FAIL if they do not support a property.
            return this.nestedHierarchy.SetProperty(VSConstants.VSITEMID_ROOT, propID, value);
        }

        /// <summary>
        /// Starts observing changes on this file.
        /// </summary>
        private void ObserveNestedProjectFile()
        {
            ProjectContainerNode parent = this.ProjectMgr as ProjectContainerNode;
            Debug.Assert(parent != null, "The parent project for nested projects should be subclassed from ProjectContainerNode");
            parent.NestedProjectNodeReloader.ObserveItem(this.GetMkDocument(), this.ID);
        }

        /// <summary>
        /// Stops observing changes on this file.
        /// </summary>
        private void StopObservingNestedProjectFile()
        {
            ProjectContainerNode parent = this.ProjectMgr as ProjectContainerNode;
            Debug.Assert(parent != null, "The parent project for nested projects should be subclassed from ProjectContainerNode");
            parent.NestedProjectNodeReloader.StopObservingItem(this.GetMkDocument());
        }

        /// <summary>
        /// Ignores observing changes on this file depending on the boolean flag.
        /// </summary>
        /// <param name="ignoreFlag">Flag indicating whether or not to ignore changes (1 to ignore, 0 to stop ignoring).</param>
        private void IgnoreNestedProjectFile(bool ignoreFlag)
        {
            ProjectContainerNode parent = this.ProjectMgr as ProjectContainerNode;
            Debug.Assert(parent != null, "The parent project for nested projects should be subclassed from ProjectContainerNode");
            parent.NestedProjectNodeReloader.IgnoreItemChanges(this.GetMkDocument(), ignoreFlag);
        }

        /// <summary>
        /// We need to advise property notify sink on project properties so that
        /// we know when the project file is renamed through a property. 
        /// </summary>
        private void ConnectPropertyNotifySink()
        {
            if (this.projectPropertyNotifySinkCookie != (uint)ShellConstants.VSCOOKIE_NIL)
            {
                return;
            }

            IConnectionPoint connectionPoint = this.GetConnectionPointFromPropertySink();
            if (connectionPoint != null)
            {
                connectionPoint.Advise(this, out this.projectPropertyNotifySinkCookie);
            }
        }

        /// <summary>
        /// Disconnects the propertynotify sink
        /// </summary>
        private void DisconnectPropertyNotifySink()
        {
            if (this.projectPropertyNotifySinkCookie == (uint)ShellConstants.VSCOOKIE_NIL)
            {
                return;
            }

            IConnectionPoint connectionPoint = this.GetConnectionPointFromPropertySink();
            if (connectionPoint != null)
            {
                connectionPoint.Unadvise(this.projectPropertyNotifySinkCookie);
                this.projectPropertyNotifySinkCookie = (uint)ShellConstants.VSCOOKIE_NIL;
            }
        }

        /// <summary>
        /// Gets a ConnectionPoint for the IPropertyNotifySink interface.
        /// </summary>
        /// <returns></returns>
        private IConnectionPoint GetConnectionPointFromPropertySink()
        {
            IConnectionPoint connectionPoint = null;
            object browseObject = this.GetProperty((int)__VSHPROPID.VSHPROPID_BrowseObject);
            IConnectionPointContainer connectionPointContainer = browseObject as IConnectionPointContainer;

            if (connectionPointContainer != null)
            {
                Guid guid = typeof(IPropertyNotifySink).GUID;
                connectionPointContainer.FindConnectionPoint(ref guid, out connectionPoint);
            }

            return connectionPoint;
        }
        #endregion       
    }
}
#endif
