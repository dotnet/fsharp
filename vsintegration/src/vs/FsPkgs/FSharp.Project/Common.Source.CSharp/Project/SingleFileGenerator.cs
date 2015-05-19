// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if SINGLE_FILE_GENERATOR

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.FSharp.Package;

    /// <summary>
    /// Provides support for single file generator.
    /// </summary>
    internal class SingleFileGenerator : ISingleFileGenerator, IVsGeneratorProgress
    {

        #region fields
        private bool gettingCheckoutStatus;
        private bool runningGenerator;
        private bool hasRunGenerator = false;
        private ProjectNode projectMgr;
        #endregion

        #region ctors
        /// <summary>
        /// Overloadde ctor.
        /// </summary>
        /// <param name="ProjectNode">The associated project</param>
        public SingleFileGenerator(ProjectNode projectMgr)
        {
            this.projectMgr = projectMgr;
        }
        #endregion

        #region IVsGeneratorProgress Members

        public virtual int GeneratorError(int warning, uint level, string err, uint line, uint col)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int Progress(uint complete, uint total)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region ISingleFileGenerator
        /// <summary>
        /// Runs the generator on the current project item.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public virtual void RunGenerator(string document)
        {
            // Go run the generator on that node, but only if the file is dirty
            // in the running document table.  Otherwise there is no need to rerun
            // the generator because if the original document is not dirty then
            // the generated output should be already up to date.
            uint itemid = VSConstants.VSITEMID_NIL;
            IVsHierarchy hier = (IVsHierarchy)this.projectMgr;
            if (document != null && hier != null && ErrorHandler.Succeeded(hier.ParseCanonicalName((string)document, out itemid)))
            {
                IVsHierarchy rdtHier;
                IVsPersistDocData perDocData;
                uint cookie;
                if (!this.hasRunGenerator || this.VerifyFileDirtyInRdt((string)document, out rdtHier, out perDocData, out cookie))
                {
                    // Run the generator on the indicated document
                    FileNode node = (FileNode)this.projectMgr.NodeFromItemId(itemid);
                    this.InvokeGenerator(node);
                    if (!this.hasRunGenerator)
                        this.hasRunGenerator=true;
                }
            }
        }
        #endregion
        
        #region virtual methods
        /// <summary>
        /// Invokes the specified generator
        /// </summary>
        /// <param name="fileNode">The node on which to invoke the generator.</param>
        public virtual void InvokeGenerator(FileNode fileNode)
        {
            if (fileNode == null)
            {
                throw new ArgumentNullException("node");
            }

            SingleFileGeneratorNodeProperties nodeproperties = fileNode.NodeProperties as SingleFileGeneratorNodeProperties;
            if (nodeproperties == null)
            {
                throw new InvalidOperationException();
            }

            string customToolProgID = nodeproperties.CustomTool;
            if (string.IsNullOrEmpty(customToolProgID))
            {
                return;
            }

            string customToolNamespace = nodeproperties.CustomToolNamespace;

            try
            {
                if (!this.runningGenerator)
                {
                    //Get the buffer contents for the current node
                    string moniker = fileNode.GetMkDocument();

                    this.runningGenerator = true;

                    //Get the generator
                    IVsSingleFileGenerator generator;
                    int generateDesignTimeSource;
                    int generateSharedDesignTimeSource;
                    int generateTempPE;
                    SingleFileGeneratorFactory factory = new SingleFileGeneratorFactory(this.projectMgr.ProjectGuid, this.projectMgr.Site);
                    ErrorHandler.ThrowOnFailure(factory.CreateGeneratorInstance(customToolProgID, out generateDesignTimeSource, out generateSharedDesignTimeSource, out generateTempPE, out generator));

                    //Check to see if the generator supports siting
                    IObjectWithSite objWithSite = generator as IObjectWithSite;
                    if (objWithSite != null)
                    {
                        objWithSite.SetSite(fileNode.OleServiceProvider);
                    }

                    //Determine the namespace
                    if (string.IsNullOrEmpty(customToolNamespace))
                    {
                        customToolNamespace = this.ComputeNamespace(moniker);
                    }

                    //Run the generator
                    IntPtr[] output = new IntPtr[1];
                    output[0] = IntPtr.Zero;
                    uint outPutSize;
                    string extension;
                    ErrorHandler.ThrowOnFailure(generator.DefaultExtension(out extension));

                    //Find if any dependent node exists
                    string dependentNodeName = Path.GetFileNameWithoutExtension(fileNode.FileName) + extension;
                    HierarchyNode dependentNode = fileNode.FirstChild;
                    while (dependentNode != null)
                    {
                        if (string.Compare(dependentNode.ItemNode.GetMetadata(ProjectFileConstants.DependentUpon), fileNode.FileName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            dependentNodeName = ((FileNode)dependentNode).FileName;
                            break;
                        }

                        dependentNode = dependentNode.NextSibling;
                    }

                    //If you found a dependent node. 
                    if (dependentNode != null)
                    {
                        //Then check out the node and dependent node from SCC
                        if (!this.CanEditFile(dependentNode.GetMkDocument()))
                        {
                            throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
                        }
                    }
                    else //It is a new node to be added to the project
                    {
                        // Check out the project file if necessary.
                        if (!this.projectMgr.QueryEditProjectFile(false))
                        {
                            throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
                        }
                    }
                    IVsTextStream stream;
                    string inputFileContents = this.GetBufferContents(moniker, out stream);

                    ErrorHandler.ThrowOnFailure(generator.Generate(moniker, inputFileContents, customToolNamespace, output, out outPutSize, this));
                    byte[] data = new byte[outPutSize];

                    if (output[0] != IntPtr.Zero)
                    {
                        Marshal.Copy(output[0], data, 0, (int)outPutSize);
                        Marshal.FreeCoTaskMem(output[0]);
                    }

                    //Todo - Create a file and add it to the Project
                    string fileToAdd = this.UpdateGeneratedCodeFile(fileNode, data, (int)outPutSize, dependentNodeName);
                }
            }
            finally
            {
                this.runningGenerator = false;
            }
        }

        /// <summary>
        /// Computes the names space based on the folder for the ProjectItem. It just replaces DirectorySeparatorCharacter
        /// with "." for the directory in which the file is located.
        /// </summary>
        /// <returns>Returns the computed name space</returns>
        public virtual string ComputeNamespace(string projectItemPath)
        {
            if (String.IsNullOrEmpty(projectItemPath))
            {
                throw new ArgumentException(SR.GetString(SR.ParameterCannotBeNullOrEmpty, CultureInfo.CurrentUICulture), "projectItemPath");
            }


            string nspace = "";
            string filePath = Path.GetDirectoryName(projectItemPath);
            string[] toks = filePath.Split(new char[] { ':', '\\' });
            foreach (string tok in toks)
            {
                if (tok != "")
                {
                    string temp = tok.Replace(" ", "");
                    nspace += (temp + ".");
                }
            }
            nspace = nspace.Remove(nspace.LastIndexOf(".", StringComparison.Ordinal), 1);
            return nspace;
        }

        /// <summary>
        /// This is called after the single file generator has been invoked to create or update the code file.
        /// </summary>
        /// <param name="fileNode">The node associated to the generator</param>
        /// <param name="data">data to update the file with</param>
        /// <param name="size">size of the data</param>
        /// <param name="fileName">Name of the file to update or create</param>
        /// <returns>full path of the file</returns>
        public virtual string UpdateGeneratedCodeFile(FileNode fileNode, byte[] data, int size, string fileName)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(fileNode.GetMkDocument()), fileName);
            IVsRunningDocumentTable rdt = this.projectMgr.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;

            // (kberes) Shouldn't this be an InvalidOperationException instead with some not to annoying errormessage to the user?
            if (rdt == null)
            {
                ErrorHandler.ThrowOnFailure(VSConstants.E_FAIL);
            }

            IVsHierarchy hier;
            uint cookie;
            uint itemid;
            IntPtr docData = IntPtr.Zero;
            try
            {
                ErrorHandler.ThrowOnFailure(rdt.FindAndLockDocument((uint)(_VSRDTFLAGS.RDT_NoLock), filePath, out hier, out itemid, out docData, out cookie));
            }
            catch (Exception)
            {
                if (docData != IntPtr.Zero) Marshal.Release(docData);
                throw;
            }

            if (docData != IntPtr.Zero)
            {
                Marshal.Release(docData);
                IVsTextStream srpStream;
                string inputFileContents = this.GetBufferContents(filePath, out srpStream);
                if (srpStream != null)
                {
                    int oldLen = 0;
                    int hr = srpStream.GetSize(out oldLen);
                    if (ErrorHandler.Succeeded(hr))
                    {
                        IntPtr dest = IntPtr.Zero;
                        try
                        {
                            dest = Marshal.AllocCoTaskMem(data.Length);
                            Marshal.Copy(data, 0, dest, data.Length);
                            ErrorHandler.ThrowOnFailure(srpStream.ReplaceStream(0, oldLen, dest, size / 2));
                        }
                        finally
                        {
                            if (dest != IntPtr.Zero)
                            {
                                Marshal.FreeCoTaskMem(dest);
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream generatedFileStream = File.Open(filePath, FileMode.OpenOrCreate))
                {
                    generatedFileStream.Write(data, 0, size);
                }

                EnvDTE.ProjectItem projectItem = fileNode.GetAutomationObject() as EnvDTE.ProjectItem;
                if (projectItem != null && (this.projectMgr.FindChild(fileNode.FileName) == null))
                {
                    projectItem.ProjectItems.AddFromFile(filePath);
                }
            }
            return filePath;
        }
        #endregion

        #region helpers
        /// <summary>
        /// Returns the buffer contents for a moniker.
        /// </summary>
        /// <returns>Buffer contents</returns>
        private string GetBufferContents(string fileName, out IVsTextStream srpStream)
        {
            Guid CLSID_VsTextBuffer = new Guid("{8E7B96A8-E33D-11d0-A6D5-00C04FB67F6A}");
            string bufferContents = "";
            srpStream = null;

            IVsRunningDocumentTable rdt = this.projectMgr.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (rdt != null)
            {
                IVsHierarchy hier;
                IVsPersistDocData persistDocData;
                uint itemid, cookie;
                bool docInRdt = true;
                IntPtr docData = IntPtr.Zero;
                int hr = NativeMethods.E_FAIL;
                try
                {
                    //Getting a read lock on the document. Must be released later.
                    hr = rdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, fileName, out hier, out itemid, out docData, out cookie);
                    if (ErrorHandler.Failed(hr) || docData == IntPtr.Zero)
                    {
                        Guid iid = VSConstants.IID_IUnknown;
                        cookie = 0;
                        docInRdt = false;
                        ILocalRegistry localReg = this.projectMgr.GetService(typeof(SLocalRegistry)) as ILocalRegistry;
                        ErrorHandler.ThrowOnFailure(localReg.CreateInstance(CLSID_VsTextBuffer, null, ref iid, (uint)CLSCTX.CLSCTX_INPROC_SERVER, out docData));
                    }

                    persistDocData = Marshal.GetObjectForIUnknown(docData) as IVsPersistDocData;
                }
                finally
                {
                    if (docData != IntPtr.Zero)
                    {
                        Marshal.Release(docData);
                    }
                }

                //Try to get the Text lines
                IVsTextLines srpTextLines = persistDocData as IVsTextLines;
                if (srpTextLines == null)
                {
                    // Try getting a text buffer provider first
                    IVsTextBufferProvider srpTextBufferProvider = persistDocData as IVsTextBufferProvider;
                    if (srpTextBufferProvider != null)
                    {
                        hr = srpTextBufferProvider.GetTextBuffer(out srpTextLines);
                    }
                }

                if (ErrorHandler.Succeeded(hr))
                {
                    srpStream = srpTextLines as IVsTextStream;
                    if (srpStream != null)
                    {
                        // QI for IVsBatchUpdate and call FlushPendingUpdates if they support it
                        IVsBatchUpdate srpBatchUpdate = srpStream as IVsBatchUpdate;
                        if (srpBatchUpdate != null)
                            srpBatchUpdate.FlushPendingUpdates(0);

                        int lBufferSize = 0;
                        hr = srpStream.GetSize(out lBufferSize);

                        if (ErrorHandler.Succeeded(hr))
                        {
                            IntPtr dest = IntPtr.Zero;
                            try
                            {
                                // Note that GetStream returns Unicode to us so we don't need to do any conversions
                                dest = Marshal.AllocCoTaskMem((lBufferSize + 1) * 2);
                                ErrorHandler.ThrowOnFailure(srpStream.GetStream(0, lBufferSize, dest));
                                //Get the contents
                                bufferContents = Marshal.PtrToStringUni(dest);
                            }
                            finally
                            {
                                if (dest != IntPtr.Zero)
                                    Marshal.FreeCoTaskMem(dest);
                            }
                        }
                    }

                }
                // Unlock the document in the RDT if necessary
                if (docInRdt && rdt != null)
                {
                    ErrorHandler.ThrowOnFailure(rdt.UnlockDocument((uint)(_VSRDTFLAGS.RDT_ReadLock | _VSRDTFLAGS.RDT_Unlock_NoSave), cookie));
                }

                if (ErrorHandler.Failed(hr))
                {
                    // If this failed then it's probably not a text file.  In that case,
                    // we just read the file as a binary
                    bufferContents = File.ReadAllText(fileName);
                }


            }
            return bufferContents;
        }

        /// <summary>
        /// Returns TRUE if open and dirty. Note that documents can be open without a
        /// window frame so be careful. Returns the DocData and doc cookie if requested
        /// </summary>
        /// <param name="document">document path</param>
        /// <param name="pHier">hierarchy</param>
        /// <param name="ppDocData">doc data associated with document</param>
        /// <param name="cookie">item cookie</param>
        /// <returns>True if FIle is dirty</returns>
        private bool VerifyFileDirtyInRdt(string document, out IVsHierarchy pHier, out IVsPersistDocData ppDocData, out uint cookie)
        {
            int ret = 0;
            pHier = null;
            ppDocData = null;
            cookie = 0;

            IVsRunningDocumentTable rdt = this.projectMgr.GetService(typeof(IVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (rdt != null)
            {
                IntPtr docData = IntPtr.Zero;
                uint dwCookie = 0;
                IVsHierarchy srpHier;
                uint itemid = VSConstants.VSITEMID_NIL;

                try
                {
                    ErrorHandler.ThrowOnFailure(rdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock, document, out srpHier, out itemid, out docData, out dwCookie));
                    IVsPersistHierarchyItem srpIVsPersistHierarchyItem = srpHier as IVsPersistHierarchyItem;
                    if (srpIVsPersistHierarchyItem != null)
                    {
                        // Found in the RDT. See if it is dirty
                        try
                        {
                            ErrorHandler.ThrowOnFailure(srpIVsPersistHierarchyItem.IsItemDirty(itemid, docData, out ret));
                            cookie = dwCookie;
                            ppDocData = Marshal.GetObjectForIUnknown(docData) as IVsPersistDocData;
                        }
                        finally
                        {
                            pHier = srpHier;
                        }
                    }
                }
                finally
                {
                    if (docData != IntPtr.Zero)
                    {
                        Marshal.Release(docData);
                    }
                }
            }
            return (ret == 1);
        }
        #endregion
        



        #region QueryEditQuerySave helpers
        /// <summary>
        /// This function asks to the QueryEditQuerySave service if it is possible to
        /// edit the file.
        /// </summary>
        private bool CanEditFile(string documentMoniker)
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t**** CanEditFile called ****"));

            // Check the status of the recursion guard
            if (this.gettingCheckoutStatus)
            {
                return false;
            }

            try
            {
                // Set the recursion guard
                this.gettingCheckoutStatus = true;

                // Get the QueryEditQuerySave service
                IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2)this.projectMgr.GetService(typeof(SVsQueryEditQuerySave));

                // Now call the QueryEdit method to find the edit status of this file
                string[] documents = { documentMoniker };
                uint result;
                uint outFlags;

                // Note that this function can popup a dialog to ask the user to checkout the file.
                // When this dialog is visible, it is possible to receive other request to change
                // the file and this is the reason for the recursion guard.
                int hr = queryEditQuerySave.QueryEditFiles(
                    0,              // Flags
                    1,              // Number of elements in the array
                    documents,      // Files to edit
                    null,           // Input flags
                    null,           // Input array of VSQEQS_FILE_ATTRIBUTE_DATA
                    out result,     // result of the checkout
                    out outFlags    // Additional flags
                );

                if (ErrorHandler.Succeeded(hr) && (result == (uint)tagVSQueryEditResult.QER_EditOK))
                {
                    // In this case (and only in this case) we can return true from this function.
                    return true;
                }
            }
            finally
            {
                this.gettingCheckoutStatus = false;
            }

            return false;
        }        
        #endregion
    }
}
#endif
