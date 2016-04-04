// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using VSLangProj;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem.Web
{
    internal class VsHierarchyItem
    {
        private uint _vsitemid;
        private IVsHierarchy _hier;
        private ServiceProvider _serviceProvider;

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Hiearchy item id</param>
        /// <param name="hier">Hiearchy interface (typically the project)</param>
        //--------------------------------------------------------------------------------------------
        internal VsHierarchyItem(uint id, IVsHierarchy hier)
        {
            Debug.Assert(hier != null, "hier cannot be null");
            _vsitemid = id;
            _hier = hier;
        }

        internal VsHierarchyItem(IVsHierarchy hier)
        {
            Debug.Assert(hier != null, "hier cannot be null");
            _vsitemid = VSConstants.VSITEMID_ROOT;
            _hier = hier;
        }

        public override string ToString()
        {
            return Name();
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        ///     Locates the item in the provided hierarchy using the provided moniker
        ///     and return a VsHierarchyItem for it
        /// </summary>
        //--------------------------------------------------------------------------------------------
        internal static VsHierarchyItem CreateFromMoniker(string moniker, IVsHierarchy hier)
        {
            VsHierarchyItem item = null;

            if (!string.IsNullOrEmpty(moniker) && hier != null)
            {
                IVsProject proj = hier as IVsProject;
                if (proj != null)
                {
                    int hr;
                    int isFound = 0;
                    uint itemid = VSConstants.VSITEMID_NIL;
                    VSDOCUMENTPRIORITY[] priority = new VSDOCUMENTPRIORITY[1];
                    hr = proj.IsDocumentInProject(moniker, out isFound, priority, out itemid);
                    if (ErrorHandler.Succeeded(hr) && isFound != 0 && itemid != VSConstants.VSITEMID_NIL)
                    {
                        item = new VsHierarchyItem(itemid, hier);
                    }

                }
            }

            return item;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Item's id which is unique for the hieararchy
        /// </summary>
        //--------------------------------------------------------------------------------------------
        internal uint VsItemID
        {
            get
            {
                return _vsitemid;
            }
            set
            {
                _vsitemid = value;
            }
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Read only access to the hierarchy interface
        /// </summary>
        //--------------------------------------------------------------------------------------------
        internal IVsHierarchy Hierarchy
        {
            get
            {
                return _hier;
            }
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Read only access to the uihierarchy interface
        /// </summary>
        //--------------------------------------------------------------------------------------------
        public IVsUIHierarchy UIHierarchy()
        {
            return _hier as IVsUIHierarchy;
        }
        
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the root as a VsHierarchyItem
        /// </summary>
        //--------------------------------------------------------------------------------------------
        internal VsHierarchyItem Root()
        {
            return new VsHierarchyItem(VSConstants.VSITEMID_ROOT, _hier);
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the save name for the item.  The save name is the string
        /// shown in the save and save changes dialog boxes.
        /// </summary>
        /// <returns></returns>
        //--------------------------------------------------------------------------------------------
        internal string SaveName()
        {
            object o = GetPropHelper(__VSHPROPID.VSHPROPID_SaveName);

            return (o is string) ? (string)o : string.Empty;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the string displayed in the project window for a particular item
        /// </summary>
        /// <returns>Display name for item</returns>
        //--------------------------------------------------------------------------------------------
        internal string Caption()
        {
            object o = GetPropHelper(__VSHPROPID.VSHPROPID_Caption);

            return (o is string) ? (string)o : string.Empty;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the name of the item which is basically the file name
        /// plus extension minus the directory
        /// </summary>
        /// <returns>Name of item</returns>
        //--------------------------------------------------------------------------------------------
        internal string Name()
        {
            object o = GetPropHelper(__VSHPROPID.VSHPROPID_Name);

            return (o is string) ? (string)o : string.Empty;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the extensibility object
        /// </summary>
        /// <returns>Name of item</returns>
        //--------------------------------------------------------------------------------------------
        internal object ExtObject()
        {
            return GetPropHelper(__VSHPROPID.VSHPROPID_ExtObject);
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the ProjectItem extensibility object
        /// </summary>
        /// <returns>Name of item</returns>
        //--------------------------------------------------------------------------------------------
        internal ProjectItem ProjectItem()
        {
            return ExtObject() as ProjectItem;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the ProjectItems extensibility object
        /// </summary>
        /// <returns>Name of item</returns>
        //--------------------------------------------------------------------------------------------
        internal ProjectItems ProjectItems()
        {
            if (IsRootNode())
            {
                Project project = ExtObject() as Project;
                if (project != null)
                {
                    return project.ProjectItems;
                }
            }
            else
            {
                ProjectItem projectItem = ProjectItem();
                if (projectItem != null)
                {
                    return projectItem.ProjectItems;
                }
            }
            return null;
        }
        
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the the full path of the item using extensibility.
        /// </summary>
        /// <returns>Name of item</returns>
        //--------------------------------------------------------------------------------------------
        internal string FullPath()
        {
            string fullPath = "";
            try
            {
                object obj = ExtObject();
                
                if(obj is EnvDTE.Project)
                {
                    EnvDTE.Project project = obj as EnvDTE.Project;
                    if(project != null)
                        fullPath = project.Properties.Item("FullPath").Value as string;
                }
                else if(obj is EnvDTE.ProjectItem)
                {
                    EnvDTE.ProjectItem projItem = obj as EnvDTE.ProjectItem;
                    if(projItem != null)
                        fullPath = projItem.Properties.Item("FullPath").Value as string;       
                }
            }
            catch
            {
            }
            return fullPath;
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns the relative path of the project item
        ///
        ///     folder\file.ext
        /// </summary>
        ///--------------------------------------------------------------------------------------------
        internal string ProjRelativePath()
        {
            string projRelativePath = null;

            string rootProjectDir = Root().ProjectDir();
			rootProjectDir = WAUtilities.EnsureTrailingBackSlash(rootProjectDir);
            string fullPath = FullPath();

            if (!string.IsNullOrEmpty(rootProjectDir) && !string.IsNullOrEmpty(fullPath))
            {
				projRelativePath = WAUtilities.MakeRelativePath(fullPath, rootProjectDir);
            }
            
            return projRelativePath;
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns the relative path of the project item
        ///
        ///     folder/file.ext
        /// </summary>
        ///--------------------------------------------------------------------------------------------
        internal string ProjRelativeUrl()
        {
            string projRelativeUrl = null;
            
            string projRelativePath = ProjRelativePath();
            if (!string.IsNullOrEmpty(projRelativePath))
            {
                projRelativeUrl = projRelativePath.Replace(Path.DirectorySeparatorChar, '/');
            }
            
            return projRelativeUrl;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets full path to project directory for this item
        /// </summary>
        /// <returns></returns>
        //--------------------------------------------------------------------------------------------
        internal string ProjectDir()
        {
            object o = GetPropHelper(__VSHPROPID.VSHPROPID_ProjectDir);

            return (o is string) ? (string)o : string.Empty;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Get type name for this item.  This is the display name used in the 
        /// title bar to identify the type of the node or hierarchy.
        /// </summary>
        /// <returns></returns>
        //--------------------------------------------------------------------------------------------
        internal bool IsRootNode()
        {
            return _vsitemid == VSConstants.VSITEMID_ROOT;
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the type guid for this item and compares against GUID_ItemType_PhysicalFile
        /// </summary>
        /// <returns></returns>
        //--------------------------------------------------------------------------------------------
        internal bool IsFile()
        {
            Guid guid = GetGuidPropHelper(__VSHPROPID.VSHPROPID_TypeGuid);
            return guid.CompareTo(VSConstants.GUID_ItemType_PhysicalFile) == 0;
        }

        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the current contents of a document
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public string GetDocumentText()
        {
            string text = null;
            IVsPersistDocData docData = null;

            try
            {
                // Get or create the buffer
                IVsTextLines buffer = GetRunningDocumentTextBuffer();
                if (buffer == null)
                {
                    docData = CreateDocumentData();
                    buffer = docData as IVsTextLines;
                }

                // get the text from the buffer
                if (buffer != null)
                {
                    IVsTextStream textStream = buffer as IVsTextStream;
                    if (textStream != null)
                    {
                        int length;
                        int hr = textStream.GetSize(out length);
                        if (ErrorHandler.Succeeded(hr))
                        {
                            if (length > 0)
                            {
                                IntPtr pText = Marshal.AllocCoTaskMem((length + 1) * 2);
                                try
                                {
                                    hr = textStream.GetStream(0, length, pText);
                                    if (ErrorHandler.Succeeded(hr))
                                    {
                                        text = Marshal.PtrToStringUni(pText);
                                    }
                                }
                                finally
                                {
                                    Marshal.FreeCoTaskMem(pText);
                                }
                            }
                            else
                            {
                                text = string.Empty;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (docData != null)
                {
                    docData.Close();
                }
            }

            return text;
        }

        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates and loads the document data
        /// (You must Close() it when done)
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public IVsPersistDocData CreateDocumentData()
        {
            if (IsFile())
            {
                string fullPath = FullPath();
                if (!string.IsNullOrEmpty(fullPath))
                {
					IOleServiceProvider serviceProvider = (IOleServiceProvider)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(IOleServiceProvider));
					IVsPersistDocData docData = WAUtilities.CreateSitedInstance<IVsPersistDocData>(serviceProvider, typeof(VsTextBufferClass).GUID);
                    if (docData != null)
                    {
                        int hr = docData.LoadDocData(fullPath);
                        if (ErrorHandler.Succeeded(hr))
                        {
                            return docData;
                        }
                    }
                }
            }
            return null;
        }

        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// If the document is open, it returns the IVsTextLines.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public IVsTextLines GetRunningDocumentTextBuffer()
        {
            IVsTextLines buffer = null;

            IVsPersistDocData docData = GetRunningDocumentData();
            if (docData != null)
            {
                buffer = docData as IVsTextLines;
                if (buffer == null)
                {
                    IVsTextBufferProvider provider = docData as IVsTextBufferProvider;
                    if (provider != null)
                    {
                        provider.GetTextBuffer(out buffer);
                    }
                }
            }

            return buffer;
        }
        
        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// If the document is open, it returns the IVsPersistDocData for it.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public IVsPersistDocData GetRunningDocumentData()
        {
            IVsPersistDocData persistDocData = null;

            IntPtr docData = IntPtr.Zero;
            try
            {
                docData = GetRunningDocData();
                if (docData != IntPtr.Zero)
                {
                    persistDocData = Marshal.GetObjectForIUnknown(docData) as IVsPersistDocData;
                }
            }
            finally
            {
                if (docData != IntPtr.Zero)
                {
                    Marshal.Release(docData);
                }
            }

            return persistDocData;
        }
        
        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// If the document is open, it returns the IntPtr to the doc data.
        /// (This is ref-counted and must be released with Marshal.Release())
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public IntPtr GetRunningDocData()
        {
            IntPtr docData = IntPtr.Zero;

            if (IsFile())
            {
                string fullPath = FullPath();
                if (!string.IsNullOrEmpty(fullPath))
                {
                    IVsRunningDocumentTable rdt = GetService<IVsRunningDocumentTable>();
                    if (rdt != null)
                    {
                        _VSRDTFLAGS flags = _VSRDTFLAGS.RDT_NoLock;
                        uint itemid;
                        IVsHierarchy hierarchy;
                        uint docCookie;
                        rdt.FindAndLockDocument
                        (
                            (uint)flags,
                            fullPath,
                            out hierarchy,
                            out itemid,
                            out docData,
                            out docCookie
                        );
                    }
                }
            }

            return docData;
        }
        
        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Gets the project item Properties collection.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public Properties Properties()
        {
            object obj = ExtObject();
            if(obj is EnvDTE.Project)
                return ((EnvDTE.Project)obj).Properties;
            else if(obj is EnvDTE.ProjectItem)
                return ((EnvDTE.ProjectItem)obj).Properties;
            return null;
        }

        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Gets the build action of the item.
        ///     If not found returns build action none.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public prjBuildAction BuildAction()
        {
            Properties props = Properties();
            if (props != null)
            {
                Property propBuildAction = props.Item("BuildAction");
                if (propBuildAction != null)
                {
                    object objValue = propBuildAction.Value;
                    if (objValue != null)
                    {
                        prjBuildAction buildAction = (prjBuildAction)objValue;
                        return buildAction;
                    }
                }
            }
            return prjBuildAction.prjBuildActionNone;
        }

        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns true if the build action of the item is compile.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public bool IsBuildActionCompile()
        {
            prjBuildAction buildAction = BuildAction();
            if (buildAction == prjBuildAction.prjBuildActionCompile)
            {
                return true;
            }
            return false;
        }
                        

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the specified property from the __VSHPROPID enumeration for this item
        /// </summary>
        //--------------------------------------------------------------------------------------------
        private object GetPropHelper(__VSHPROPID propid)
        {
            return GetPropHelper(_vsitemid, (int)propid);
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the specified property from the __VSHPROPID2 enumeration for this item
        /// </summary>
        //--------------------------------------------------------------------------------------------
        private object GetPropHelper(__VSHPROPID2 propid)
        {
            return GetPropHelper(_vsitemid, (int)propid);
        }
        
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the specified property for the specified item
        /// </summary>
        //--------------------------------------------------------------------------------------------
        private object GetPropHelper(uint itemid, int propid)
        {
            try
            {
                object o = null;
                
                if (_hier != null)
                {
                    int hr = _hier.GetProperty(itemid, propid, out o);
                }

                return o;
            }
            catch(Exception)
            {
                return null;
            }
        }

        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// Calls IVsHIerachy::GetGuidProperty
        /// </summary>
        //--------------------------------------------------------------------------------------------
        internal Guid GetGuidPropHelper(Microsoft.VisualStudio.Shell.Interop.__VSHPROPID propid)
        {
            Guid guid;
            try
            {
                _hier.GetGuidProperty(_vsitemid, (int)propid, out guid);
            }
            catch(Exception)
            {
                guid = Guid.Empty;
            }
            return guid;
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Get hierarchy site
        /// </summary>
        ///--------------------------------------------------------------------------------------------
        public IOleServiceProvider Site()
        {
            IOleServiceProvider serviceProvider = null;
            if (_hier != null)
            {
                _hier.GetSite(out serviceProvider);
            }
            return serviceProvider;
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Helper to get a shell service interface
        /// </summary>
        ///--------------------------------------------------------------------------------------------
        public InterfaceType GetService<InterfaceType>() where InterfaceType : class
        {
            InterfaceType service = null;

            try
            {
                if (_serviceProvider == null)
                {
                    IOleServiceProvider serviceProvider = Site();
                    if (serviceProvider != null)
                    {
                        _serviceProvider = new ServiceProvider(serviceProvider);
                    }
                }

                if (_serviceProvider != null)
                {
                    service = _serviceProvider.GetService(typeof(InterfaceType)) as InterfaceType;
                }
            }
            catch
            {
            }

            return service;
        }

    }

}

