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
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    [CLSCompliant(false), ComVisible(true)]
    public abstract class ProjectContainerNode : ProjectNode,
        IVsParentProject,
        IBuildDependencyOnProjectContainer
    {
        #region fields

        /// <summary>
        /// Setting this flag to true will build all nested project when building this project
        /// </summary>
        private bool buildNestedProjectsOnBuild = true;

        private ProjectElement nestedProjectElement;

        /// <summary>
        /// Defines the listener that would listen on file changes on the nested project node.
        /// </summary>
        ///<devremark>            
        ///This might need a refactoring when nested projects can be added and removed by demand.
        /// </devremark>
        private FileChangeManager nestedProjectNodeReloader;
        #endregion

        #region ctors
        internal ProjectContainerNode()
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Returns teh object that handles listening to file changes on the nested project files.
        /// </summary>
        public FileChangeManager NestedProjectNodeReloader
        {
            get
            {
                if (this.nestedProjectNodeReloader == null)
                {
                    this.nestedProjectNodeReloader = new FileChangeManager(this.Site);
                    this.nestedProjectNodeReloader.FileChangedOnDisk += this.OnNestedProjectFileChangedOnDisk;
                }

                return this.nestedProjectNodeReloader;
            }
        }
        #endregion

        #region overridden properties
        /// <summary>
        /// This is the object that will be returned by EnvDTE.Project.Object for this project
        /// </summary>
        public override object Object
        {
            get { return new Automation.OASolutionFolder<ProjectContainerNode>(this); }
        }

        #endregion

        #region public overridden methods
        /// <summary>
        /// Gets the nested hierarchy.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="iidHierarchyNested">Identifier of the interface to be returned in ppHierarchyNested.</param>
        /// <param name="ppHierarchyNested">Pointer to the interface whose identifier was passed in iidHierarchyNested.</param>
        /// <param name="pItemId">Pointer to an item identifier of the root node of the nested hierarchy.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. If ITEMID is not a nested hierarchy, this method returns E_FAIL.</returns>
        [CLSCompliant(false)]
        public override int GetNestedHierarchy(UInt32 itemId, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pItemId)
        {
            pItemId = VSConstants.VSITEMID_ROOT;
            ppHierarchyNested = IntPtr.Zero;
            if (this.FirstChild != null)
            {
                for (HierarchyNode n = this.FirstChild; n != null; n = n.NextSibling)
                {
                    if (n is NestedProjectNode && n.ID == itemId)
                    {
                        NestedProjectNode p = n as NestedProjectNode;

                        if (p.NestedHierarchy != null)
                        {
                            IntPtr iunknownPtr = IntPtr.Zero;
                            int returnValue = VSConstants.S_OK;
                            try
                            {
                                iunknownPtr = Marshal.GetIUnknownForObject(p.NestedHierarchy);
                                Marshal.QueryInterface(iunknownPtr, ref iidHierarchyNested, out ppHierarchyNested);
                            }
                            catch (COMException e)
                            {
                                returnValue = e.ErrorCode;
                            }
                            finally
                            {
                                if (iunknownPtr != IntPtr.Zero)
                                {
                                    Marshal.Release(iunknownPtr);
                                }
                            }

                            return returnValue;
                        }
                        break;
                    }
                }
            }

            return VSConstants.E_FAIL;
        }

        public override int IsItemDirty(uint itemId, IntPtr punkDocData, out int pfDirty)
        {
            HierarchyNode hierNode = this.NodeFromItemId(itemId);
            Debug.Assert(hierNode != null, "Hierarchy node not found");
            if (hierNode != this)
            {
                return ErrorHandler.ThrowOnFailure(hierNode.IsItemDirty(itemId, punkDocData, out pfDirty));
            }
            else
            {
                return ErrorHandler.ThrowOnFailure(base.IsItemDirty(itemId, punkDocData, out pfDirty));
            }
        }

        public override int SaveItem(VSSAVEFLAGS dwSave, string silentSaveAsName, uint itemid, IntPtr punkDocData, out int pfCancelled)
        {
            HierarchyNode hierNode = this.NodeFromItemId(itemid);
            Debug.Assert(hierNode != null, "Hierarchy node not found");
            if (hierNode != this)
            {
                return ErrorHandler.ThrowOnFailure(hierNode.SaveItem(dwSave, silentSaveAsName, itemid, punkDocData, out pfCancelled));
            }
            else
            {
                return ErrorHandler.ThrowOnFailure(base.SaveItem(dwSave, silentSaveAsName, itemid, punkDocData, out pfCancelled));
            }
        }

        public override bool FilterItemTypeToBeAddedToHierarchy(string itemType)
        {
            if (String.Compare(itemType, ProjectFileConstants.SubProject, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            return base.FilterItemTypeToBeAddedToHierarchy(itemType);
        }

        /// <summary>
        /// Called to reload a project item. 
        /// Reloads a project and its nested project nodes.
        /// </summary>
        /// <param name="itemId">Specifies itemid from VSITEMID.</param>
        /// <param name="reserved">Reserved.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public override int ReloadItem(uint itemId, uint reserved)
        {
            #region precondition
            if (this.IsClosed)
            {
                return VSConstants.E_FAIL;
            }
            #endregion

            NestedProjectNode node = this.NodeFromItemId(itemId) as NestedProjectNode;

            if (node != null)
            {
                object propertyAsObject = node.GetProperty((int)__VSHPROPID.VSHPROPID_HandlesOwnReload);

                if (propertyAsObject != null && (bool)propertyAsObject)
                {
                    node.ReloadItem(reserved);
                }
                else
                {
                    this.ReloadNestedProjectNode(node);
                }

                return VSConstants.S_OK;
            }

            return base.ReloadItem(itemId, reserved);
        }

        /// <summary>
        /// Reloads a project and its nested project nodes.
        /// </summary>
        public override void Reload()
        {
            base.Reload();
            this.CreateNestedProjectNodes();
        }
        #endregion

        #region IVsParentProject
        public virtual int OpenChildren()
        {
            IVsSolution solution = this.GetService(typeof(IVsSolution)) as IVsSolution;

            Debug.Assert(solution != null, "Could not retrieve the solution from the services provided by this project");
            if (solution == null)
            {
                return VSConstants.E_FAIL;
            }

            IntPtr iUnKnownForSolution = IntPtr.Zero;
            int returnValue = VSConstants.S_OK; // be optimistic.

            try
            {
                this.DisableQueryEdit = true;
                this.EventTriggeringFlag = ProjectNode.EventTriggering.DoNotTriggerHierarchyEvents | ProjectNode.EventTriggering.DoNotTriggerTrackerEvents;
                iUnKnownForSolution = Marshal.GetIUnknownForObject(solution);

                // notify SolutionEvents listeners that we are about to add children
                IVsFireSolutionEvents fireSolutionEvents = Marshal.GetTypedObjectForIUnknown(iUnKnownForSolution, typeof(IVsFireSolutionEvents)) as IVsFireSolutionEvents;
                ErrorHandler.ThrowOnFailure(fireSolutionEvents.FireOnBeforeOpeningChildren(this));

                this.AddVirtualProjects();

                ErrorHandler.ThrowOnFailure(fireSolutionEvents.FireOnAfterOpeningChildren(this));
            }
            catch (Exception e)
            {
                // Exceptions are digested by the caller but we want then to be shown if not a ComException and if not in automation.
                if (!(e is COMException) && !Utilities.IsInAutomationFunction(this.Site))
                {
                    string title = null;
                    OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                    OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                    VsShellUtilities.ShowMessageBox(this.Site, title, e.Message, icon, buttons, defaultButton);
                }

                Trace.WriteLine("Exception : " + e.Message);
                throw e;
            }
            finally
            {
                this.DisableQueryEdit = false;

                if (iUnKnownForSolution != IntPtr.Zero)
                {
                    Marshal.Release(iUnKnownForSolution);
                }

                this.EventTriggeringFlag = ProjectNode.EventTriggering.TriggerAll;
            }

            return returnValue;
        }

        public virtual int CloseChildren()
        {
            int returnValue = VSConstants.S_OK; // be optimistic.

            IVsSolution solution = this.GetService(typeof(IVsSolution)) as IVsSolution;
            Debug.Assert(solution != null, "Could not retrieve the solution from the services provided by this project");

            if (solution == null)
            {
                return VSConstants.E_FAIL;
            }

            IntPtr iUnKnownForSolution = IntPtr.Zero;

            try
            {
                iUnKnownForSolution = Marshal.GetIUnknownForObject(solution);

                // notify SolutionEvents listeners that we are about to close children
                IVsFireSolutionEvents fireSolutionEvents = Marshal.GetTypedObjectForIUnknown(iUnKnownForSolution, typeof(IVsFireSolutionEvents)) as IVsFireSolutionEvents;
                ErrorHandler.ThrowOnFailure(fireSolutionEvents.FireOnBeforeClosingChildren(this));

                // If the removal crashes we never fire the close children event. IS that a problem?
                this.RemoveNestedProjectNodes();

                ErrorHandler.ThrowOnFailure(fireSolutionEvents.FireOnAfterClosingChildren(this));
            }
            finally
            {
                if (iUnKnownForSolution != IntPtr.Zero)
                {
                    Marshal.Release(iUnKnownForSolution);
                }
            }

            return returnValue;
        }
        #endregion

        #region IBuildDependencyOnProjectContainerNode
        /// <summary>
        /// Defines whether nested projects should be build with the parent project
        /// </summary>
        public virtual bool BuildNestedProjectsOnBuild
        {
            get { return this.buildNestedProjectsOnBuild; }
            set { this.buildNestedProjectsOnBuild = value; }
        }

        /// <summary>
        /// Enumerates the nested hierachies that should be added to the build dependency list.
        /// </summary>
        /// <returns></returns>
        public virtual IVsHierarchy[] EnumNestedHierachiesForBuildDependency()
        {
            List<IVsHierarchy> nestedProjectList = new List<IVsHierarchy>();
            // Add all nested project among projectContainer child nodes
            for (HierarchyNode child = this.FirstChild; child != null; child = child.NextSibling)
            {
                NestedProjectNode nestedProjectNode = child as NestedProjectNode;
                if (nestedProjectNode != null)
                {
                    nestedProjectList.Add(nestedProjectNode.NestedHierarchy);
                }
            }

            return nestedProjectList.ToArray();
        }
        #endregion

        #region helper methods

        protected void RemoveNestedProjectNodes()
        {
            for (HierarchyNode n = this.FirstChild; n != null; n = n.NextSibling)
            {
                NestedProjectNode p = n as NestedProjectNode;
                if (p != null)
                {
                    p.CloseNestedProjectNode();
                }
            }

            // We do not care of file changes after this.
            this.NestedProjectNodeReloader.FileChangedOnDisk -= this.OnNestedProjectFileChangedOnDisk;
            this.NestedProjectNodeReloader.Dispose();
        }

        /// <summary>
        /// This is used when loading the project to loop through all the items
        /// and for each SubProject it finds, it create the project and a node
        /// in our Hierarchy to hold the project.
        /// </summary>
        protected void CreateNestedProjectNodes()
        {
            // 1. Create a ProjectElement with the found item and then Instantiate a new Nested project with this ProjectElement.
            // 2. Link into the hierarchy.            
            // Read subprojects from from msbuildmodel
            __VSCREATEPROJFLAGS creationFlags = __VSCREATEPROJFLAGS.CPF_NOTINSLNEXPLR | __VSCREATEPROJFLAGS.CPF_SILENT;

            if (this.IsNewProject)
            {
                creationFlags |= __VSCREATEPROJFLAGS.CPF_CLONEFILE;
            }
            else
            {
                creationFlags |= __VSCREATEPROJFLAGS.CPF_OPENFILE;
            }

            foreach (Microsoft.Build.Evaluation.ProjectItem item in MSBuildProject.GetItems(this.BuildProject))
            {
                if (String.Compare(MSBuildItem.GetItemType(item), ProjectFileConstants.SubProject, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    this.nestedProjectElement = new ProjectElement(this, item, false);

                    if (!this.IsNewProject)
                    {
                        AddExistingNestedProject(null, creationFlags);
                    }
                    else
                    {
                        // If we are creating the subproject from a vstemplate/vsz file
                        bool isVsTemplate = Utilities.IsTemplateFile(GetProjectTemplatePath(null));
                        if (isVsTemplate)
                        {
                            RunVsTemplateWizard(null, true);
                        }
                        else
                        {
                            // We are cloning the specified project file
                            AddNestedProjectFromTemplate(null, creationFlags);
                        }
                    }
                }
            }

            this.nestedProjectElement = null;
        }
        /// <summary>
        /// Add an existing project as a nested node of our hierarchy.
        /// This is used while loading the project and can also be used
        /// to add an existing project to our hierarchy.
        /// </summary>
        public virtual NestedProjectNode AddExistingNestedProject(ProjectElement element, __VSCREATEPROJFLAGS creationFlags)
        {
            ProjectElement elementToUse = (element == null) ? this.nestedProjectElement : element;

            if (elementToUse == null)
            {
                throw new ArgumentNullException("element");
            }

            string filename = elementToUse.GetFullPathForElement();
            // Delegate to AddNestedProjectFromTemplate. Because we pass flags that specify open project rather then clone, this will works.
            Debug.Assert((creationFlags & __VSCREATEPROJFLAGS.CPF_OPENFILE) == __VSCREATEPROJFLAGS.CPF_OPENFILE, "__VSCREATEPROJFLAGS.CPF_OPENFILE should have been specified, did you mean to call AddNestedProjectFromTemplate?");
            return AddNestedProjectFromTemplate(filename, Path.GetDirectoryName(filename), Path.GetFileName(filename), elementToUse, creationFlags);
        }

        /// <summary>
        /// Let the wizard code execute and provide us the information we need.
        /// Our SolutionFolder automation object should be called back with the
        /// details at which point it will call back one of our method to add
        /// a nested project.
        /// If you are trying to add a new subproject this is probably the
        /// method you want to call. If you are just trying to clone a template
        /// project file, then AddNestedProjectFromTemplate is what you want.
        /// </summary>
        /// <param name="element">The project item to use as the base of the nested project.</param>
        /// <param name="silent">true if the wizard should run silently, otherwise false.</param>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Vs")]
        public void RunVsTemplateWizard(ProjectElement element, bool silent)
        {
            ProjectElement elementToUse = (element == null) ? this.nestedProjectElement : element;

            if (elementToUse == null)
            {
                throw new ArgumentNullException("element");
            }
            this.nestedProjectElement = elementToUse;

            Automation.OAProject oaProject = GetAutomationObject() as Automation.OAProject;
            if (oaProject == null || oaProject.ProjectItems == null)
                throw new System.InvalidOperationException(SR.GetString(SR.InvalidAutomationObject, CultureInfo.CurrentUICulture));
            Debug.Assert(oaProject.Object != null, "The project automation object should have set the Object to the SolutionFolder");
            Automation.OASolutionFolder<ProjectContainerNode> folder = oaProject.Object as Automation.OASolutionFolder<ProjectContainerNode>;

            // Prepare the parameters to pass to RunWizardFile
            string destination = elementToUse.GetFullPathForElement();
            string template = this.GetProjectTemplatePath(elementToUse);

            object[] wizParams = new object[7];
            wizParams[0] = EnvDTE.Constants.vsWizardAddSubProject;
            wizParams[1] = Path.GetFileNameWithoutExtension(destination);
            wizParams[2] = oaProject.ProjectItems;
            wizParams[3] = Path.GetDirectoryName(destination);
            wizParams[4] = Path.GetFileNameWithoutExtension(destination);
            wizParams[5] = Path.GetDirectoryName(folder.DTE.FullName); //VS install dir
            wizParams[6] = silent;

            IVsDetermineWizardTrust wizardTrust = this.GetService(typeof(SVsDetermineWizardTrust)) as IVsDetermineWizardTrust;
            if (wizardTrust != null)
            {
                Guid guidProjectAdding = Guid.Empty;

                // In case of a project template an empty guid should be added as the guid parameter. See env\msenv\core\newtree.h IsTrustedTemplate method definition.
                wizardTrust.OnWizardInitiated(template, ref guidProjectAdding);
            }

            try
            {
                // Make the call to execute the wizard. This should cause AddNestedProjectFromTemplate to be
                // called back with the correct set of parameters.
                EnvDTE.IVsExtensibility extensibilityService = (EnvDTE.IVsExtensibility)GetService(typeof(EnvDTE.IVsExtensibility));
                EnvDTE.wizardResult result = extensibilityService.RunWizardFile(template, 0, ref wizParams);
                if (result == EnvDTE.wizardResult.wizardResultFailure)
                    throw new COMException();
            }
            finally
            {
                if (wizardTrust != null)
                {
                    wizardTrust.OnWizardCompleted();
                }
            }
        }

        /// <summary>
        /// This will clone a template project file and add it as a
        /// subproject to our hierarchy.
        /// If you want to create a project for which there exist a
        /// vstemplate, consider using RunVsTemplateWizard instead.
        /// </summary>
        public virtual NestedProjectNode AddNestedProjectFromTemplate(ProjectElement element, __VSCREATEPROJFLAGS creationFlags)
        {
            ProjectElement elementToUse = (element == null) ? this.nestedProjectElement : element;

            if (elementToUse == null)
            {
                throw new ArgumentNullException("element");
            }
            string destination = elementToUse.GetFullPathForElement();
            string template = this.GetProjectTemplatePath(elementToUse);

            return this.AddNestedProjectFromTemplate(template, Path.GetDirectoryName(destination), Path.GetFileName(destination), elementToUse, creationFlags);
        }

        /// <summary>
        /// This can be called directly or through RunVsTemplateWizard.
        /// This will clone a template project file and add it as a
        /// subproject to our hierarchy.
        /// If you want to create a project for which there exist a
        /// vstemplate, consider using RunVsTemplateWizard instead.
        /// </summary>
        public virtual NestedProjectNode AddNestedProjectFromTemplate(string fileName, string destination, string projectName, ProjectElement element, __VSCREATEPROJFLAGS creationFlags)
        {
            // If this is project creation and the template specified a subproject in its project file, this.nestedProjectElement will be used 
            ProjectElement elementToUse = (element == null) ? this.nestedProjectElement : element;

            if (elementToUse == null)
            {
                // If this is null, this means MSBuild does not know anything about our subproject so add an MSBuild item for it
                elementToUse = new ProjectElement(this, fileName, ProjectFileConstants.SubProject);
            }

            NestedProjectNode node = CreateNestedProjectNode(elementToUse);
            node.Init(fileName, destination, projectName, creationFlags);

            // In case that with did not have an existing element, or the nestedProjectelement was null 
            //  and since Init computes the final path, set the MSBuild item to that path
            if (this.nestedProjectElement == null)
            {
                string relativePath = node.Url;
                if (Path.IsPathRooted(relativePath))
                {
                    relativePath = this.ProjectFolder;
                    if (!relativePath.EndsWith("/\\", StringComparison.Ordinal))
                    {
                        relativePath += Path.DirectorySeparatorChar;
                    }

                    relativePath = new Url(relativePath).MakeRelative(new Url(node.Url));
                }

                elementToUse.Rename(relativePath);
            }

            this.AddChild(node);
            return node;
        }

        /// <summary>
        /// Override this method if you want to provide your own type of nodes.
        /// This would be the case if you derive a class from NestedProjectNode
        /// </summary>
        public virtual NestedProjectNode CreateNestedProjectNode(ProjectElement element)
        {
            return new NestedProjectNode(this, element);
        }

        /// <summary>
        /// Links the nested project nodes to the solution. The default implementation parses all nested project nodes and calles AddVirtualProjectEx on them.
        /// </summary>
        public virtual void AddVirtualProjects()
        {
            for (HierarchyNode child = this.FirstChild; child != null; child = child.NextSibling)
            {
                NestedProjectNode nestedProjectNode = child as NestedProjectNode;
                if (nestedProjectNode != null)
                {
                    nestedProjectNode.AddVirtualProject();
                }
            }
        }

        /// <summary>
        /// Based on the Template and TypeGuid properties of the
        /// element, generate the full template path.
        /// 
        /// TypeGuid should be the Guid of a registered project factory.
        /// Template can be a full path, a project template (for projects
        /// that support VsTemplates) or a relative path (for other projects).
        /// </summary>
        public virtual string GetProjectTemplatePath(ProjectElement element)
        {
            ProjectElement elementToUse = (element == null) ? this.nestedProjectElement : element;

            if (elementToUse == null)
            {
                throw new ArgumentNullException("element");
            }

            string templateFile = elementToUse.GetMetadata(ProjectFileConstants.Template);
            Debug.Assert(!String.IsNullOrEmpty(templateFile), "No template file has been specified in the template attribute in the project file");

            string fullPath = templateFile;
            if (!Path.IsPathRooted(templateFile))
            {
                RegisteredProjectType registeredProjectType = this.GetRegisteredProject(elementToUse);

                // This is not a full path
                Debug.Assert(registeredProjectType != null && (!String.IsNullOrEmpty(registeredProjectType.DefaultProjectExtensionValue) || !String.IsNullOrEmpty(registeredProjectType.WizardTemplatesDirValue)), " Registered wizard directory value not set in the registry.");

                // See if this specify a VsTemplate file
                fullPath = registeredProjectType.GetVsTemplateFile(templateFile);
                if (String.IsNullOrEmpty(fullPath))
                {
                    // Default to using the WizardTemplateDir to calculate the absolute path
                    fullPath = Path.Combine(registeredProjectType.WizardTemplatesDirValue, templateFile);
                }
            }

            return fullPath;
        }

        /// <summary>
        /// Get information from the registry based for the project 
        /// factory corresponding to the TypeGuid of the element
        /// </summary>
        private RegisteredProjectType GetRegisteredProject(ProjectElement element)
        {
            ProjectElement elementToUse = (element == null) ? this.nestedProjectElement : element;

            if (elementToUse == null)
            {
                throw new ArgumentNullException("element");
            }

            // Get the project type guid from project elementToUse                
            string typeGuidString = elementToUse.GetMetadataAndThrow(ProjectFileConstants.TypeGuid, new ApplicationException());
            Guid projectFactoryGuid = new Guid(typeGuidString);

            EnvDTE.DTE dte = this.ProjectMgr.Site.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            Debug.Assert(dte != null, "Could not get the automation object from the services exposed by this project");

            if (dte == null)
                throw new InvalidOperationException();

            RegisteredProjectType registeredProjectType = RegisteredProjectType.CreateRegisteredProjectType(projectFactoryGuid);
            Debug.Assert(registeredProjectType != null, "Could not read the registry setting associated to this project.");
            if (registeredProjectType == null)
            {
                throw new InvalidOperationException();
            }
            return registeredProjectType;
        }

        /// <summary>
        /// Reloads a nested project node by deleting it and readding it.
        /// </summary>
        /// <param name="node">The node to reload.</param>
        public virtual void ReloadNestedProjectNode(NestedProjectNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            IVsSolution solution = this.GetService(typeof(IVsSolution)) as IVsSolution;

            if (solution == null)
            {
                throw new InvalidOperationException();
            }

            NestedProjectNode newNode = null;
            try
            {
                // (VS 2005 UPDATE) When deleting and re-adding the nested project,
                // we do not want SCC to see this as a delete and add operation. 
                this.EventTriggeringFlag = ProjectNode.EventTriggering.DoNotTriggerTrackerEvents;

                // notify SolutionEvents listeners that we are about to add children
                IVsFireSolutionEvents fireSolutionEvents = solution as IVsFireSolutionEvents;

                if (fireSolutionEvents == null)
                {
                    throw new InvalidOperationException();
                }

                ErrorHandler.ThrowOnFailure(fireSolutionEvents.FireOnBeforeUnloadProject(node.NestedHierarchy));

                int isDirtyAsInt = 0;
                this.IsDirty(out isDirtyAsInt);

                bool isDirty = (isDirtyAsInt == 0) ? false : true;

                ProjectElement element = node.ItemNode;
                node.CloseNestedProjectNode();

                // Remove from the solution
                this.RemoveChild(node);

                // Now readd it                
                try
                {
                    __VSCREATEPROJFLAGS flags = __VSCREATEPROJFLAGS.CPF_NOTINSLNEXPLR | __VSCREATEPROJFLAGS.CPF_SILENT | __VSCREATEPROJFLAGS.CPF_OPENFILE;
                    newNode = this.AddExistingNestedProject(element, flags);
                    newNode.AddVirtualProject();
                }
                catch (Exception e)
                {
                    // We get a System.Exception if anything failed, thus we have no choice but catch it. 
                    // Exceptions are digested by VS. Show the error if not in automation.
                    if (!Utilities.IsInAutomationFunction(this.Site))
                    {
                        string message = (String.IsNullOrEmpty(e.Message)) ? SR.GetString(SR.NestedProjectFailedToReload, CultureInfo.CurrentUICulture) : e.Message;
                        string title = string.Empty;
                        OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                        OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                        OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                        VsShellUtilities.ShowMessageBox(this.Site, title, message, icon, buttons, defaultButton);
                    }

                    // Do not digest exception. let the caller handle it. If in a later stage this exception is not digested then the above messagebox is not needed.
                    throw e;
                }

#if DEBUG
                IVsHierarchy nestedHierarchy;
                solution.GetProjectOfUniqueName(newNode.GetMkDocument(), out nestedHierarchy);
                Debug.Assert(nestedHierarchy != null && Utilities.IsSameComObject(nestedHierarchy, newNode.NestedHierarchy), "The nested hierrachy was not reloaded correctly.");
#endif
                this.SetProjectFileDirty(isDirty);

                fireSolutionEvents.FireOnAfterLoadProject(newNode.NestedHierarchy);
            }
            finally
            {
                // In this scenario the nested project failed to unload or reload the nested project. We will unload the whole project, otherwise the nested project is lost.
                // This is similar to the scenario when one wants to open a project and the nested project cannot be loaded because for example the project file has xml errors.
                // We should note that we rely here that if the unload fails then exceptions are not digested and are shown to the user.
                if (newNode == null || newNode.NestedHierarchy == null)
                {
                    solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject | (uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, this, 0);
                }
                else
                {
                    this.EventTriggeringFlag = ProjectNode.EventTriggering.TriggerAll;
                }
            }
        }

        /// <summary>
        /// Event callback. Called when one of the nested project files is changed.
        /// </summary>
        /// <param name="sender">The FileChangeManager object.</param>
        /// <param name="e">Event args containing the file name that was updated.</param>
        private void OnNestedProjectFileChangedOnDisk(object sender, FileChangedOnDiskEventArgs e)
        {
            #region Pre-condition validation
            Debug.Assert(e != null, "No event args specified for the FileChangedOnDisk event");

            // We care only about time change for reload.
            if ((e.FileChangeFlag & _VSFILECHANGEFLAGS.VSFILECHG_Time) == 0)
            {
                return;
            }

            // test if we actually have a document for this id.
            string moniker;
            this.GetMkDocument(e.ItemID, out moniker);
            Debug.Assert(NativeMethods.IsSamePath(moniker, e.FileName), " The file + " + e.FileName + " has changed but we could not retrieve the path for the item id associated to the path.");
            #endregion

            bool reload = true;
            if (!Utilities.IsInAutomationFunction(this.Site))
            {
                // Prompt to reload the nested project file. We use the moniker here since the filename from the event arg is canonicalized.
                string message = String.Format(CultureInfo.CurrentCulture, SR.GetString(SR.QueryReloadNestedProject, CultureInfo.CurrentUICulture), moniker);
                string title = string.Empty;
                OLEMSGICON icon = OLEMSGICON.OLEMSGICON_INFO;
                OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
                OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                reload = (VsShellUtilities.ShowMessageBox(this.Site, message, title, icon, buttons, defaultButton) == NativeMethods.IDYES);
            }

            if (reload)
            {
                // We have to use here the interface method call, since it might be that specialized project nodes like the project container item
                // is owerwriting the default functionality.
                this.ReloadItem(e.ItemID, 0);
            }
        }
        #endregion
    }
}
#endif
