// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MSBuild = Microsoft.Build.BuildEngine;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;
using Microsoft.Build.Construction;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    /// <summary>
    /// Creates projects within the solution
    /// </summary>
    internal abstract class ProjectFactory : Microsoft.VisualStudio.Shell.Flavor.FlavoredProjectFactoryBase
                    , IVsProjectUpgradeViaFactory, IVsProjectUpgradeViaFactory4

	{
		private Microsoft.VisualStudio.Shell.Package package;
		private System.IServiceProvider site;

        private Microsoft.Build.Evaluation.ProjectCollection buildEngine;
        private Microsoft.Build.Evaluation.Project buildProject;

        public Microsoft.VisualStudio.Shell.Package Package
        {
            get
            {
                return this.package;
            }
        }

        public System.IServiceProvider Site
        {
            get
            {
                return this.site;
            }
        }

        /// <summary>
        /// The msbuild engine that we are going to use.
        /// </summary>
        public Microsoft.Build.Evaluation.ProjectCollection BuildEngine
        {
            get
            {
                return this.buildEngine;
            }
        }

        /// <summary>
        /// The msbuild project for the temporary project file.
        /// </summary>
        public Microsoft.Build.Evaluation.Project BuildProject
        {
            get
            {
                return this.buildProject;
            }
            set
            {
                this.buildProject = value;
            }
        }

        public ProjectFactory(Microsoft.VisualStudio.Shell.Package package)
        {
            this.package = package;
            this.site = package;
            this.buildEngine = Utilities.InitializeMsBuildEngine(this.buildEngine);
        }

        protected abstract ProjectNode CreateProject();

        /// <summary>
        /// Rather than directly creating the project, ask VS to initate the process of
        /// creating an aggregated project in case we are flavored. We will be called
        /// on the IVsAggregatableProjectFactory to do the real project creation.
        /// </summary>
        /// <param name="fileName">Project file</param>
        /// <param name="location">Path of the project</param>
        /// <param name="name">Project Name</param>
        /// <param name="flags">Creation flags</param>
        /// <param name="projectGuid">Guid of the project</param>
        /// <param name="project">Project that end up being created by this method</param>
        /// <param name="canceled">Was the project creation canceled</param>
        protected override void CreateProject(string fileName, string location, string name, uint flags, ref Guid projectGuid, out IntPtr project, out int canceled)
        {
            project = IntPtr.Zero;
            canceled = 0;

            if ((flags & ((uint)__VSCREATEPROJFLAGS2.CPF_DEFERREDSAVE)) != 0)
            {
                throw new NotSupportedException(SR.GetString(SR.NoZeroImpactProjects));
            }

#if FX_ATLEAST_45
            if ((flags & ((uint)__VSCREATEPROJFLAGS.CPF_OPENFILE)) != 0)
            {
                if (new ProjectInspector(fileName).IsPoisoned(Site))
                {
                    // error out
                    int ehr = unchecked((int)0x80042003); // VS_E_INCOMPATIBLEPROJECT
                    ErrorHandler.ThrowOnFailure(ehr);
                }
            }
#endif

            // Get the list of GUIDs from the project/template
            string guidsList = this.ProjectTypeGuids(fileName);

            // Launch the aggregate creation process (we should be called back on our IVsAggregatableProjectFactoryCorrected implementation)
            IVsCreateAggregateProject aggregateProjectFactory = (IVsCreateAggregateProject)this.Site.GetService(typeof(SVsCreateAggregateProject));
            int hr = aggregateProjectFactory.CreateAggregateProject(guidsList, fileName, location, name, flags, ref projectGuid, out project);
            if (hr == VSConstants.E_ABORT)
                canceled = 1;
            ErrorHandler.ThrowOnFailure(hr);

            this.buildProject = null;
        }

        /// <summary>
        /// Instantiate the project class, but do not proceed with the
        /// initialization just yet.
        /// Delegate to CreateProject implemented by the derived class.
        /// </summary>
        protected override object PreCreateForOuter(IntPtr outerProjectIUnknown)
        {
            Debug.Assert(this.buildProject != null, "The build project should have been initialized before calling PreCreateForOuter.");

            // Please be very carefull what is initialized here on the ProjectNode. Normally this should only instantiate and return a project node.
            // The reason why one should very carefully add state to the project node here is that at this point the aggregation has not yet been created and anything that would cause a CCW for the project to be created would cause the aggregation to fail
            // Our reasoning is that there is no other place where state on the project node can be set that is known by the Factory and has to execute before the Load method.
            ProjectNode node = this.CreateProject();
            Debug.Assert(node != null, "The project failed to be created");
            node.BuildEngine = this.buildEngine;
            node.BuildProject = this.buildProject;
            node.Package = this.package as ProjectPackage;
            node.ProjectEventsProvider = GetProjectEventsProvider();
            return node;
        }

        /// <summary>
        /// Retrives the list of project guids from the project file.
        /// If you don't want your project to be flavorable, override
        /// to only return your project factory Guid:
        ///      return this.GetType().GUID.ToString("B");
        /// </summary>
        /// <param name="file">Project file to look into to find the Guid list</param>
        /// <returns>List of semi-colon separated GUIDs</returns>
        protected override string ProjectTypeGuids(string file)
        {
            // Load the project so we can extract the list of GUIDs

            this.buildProject = Utilities.ReinitializeMsBuildProject(this.buildEngine, file, this.buildProject);

            // Retrieve the list of GUIDs, if it is not specify, make it our GUID
            string guids = buildProject.GetPropertyValue(ProjectFileConstants.ProjectTypeGuids);
            if (String.IsNullOrEmpty(guids))
                guids = this.GetType().GUID.ToString("B");

            return guids;
        }

#if FX_ATLEAST_45

        private class ProjectInspector
        {
            private Microsoft.Build.Construction.ProjectRootElement xmlProj;
            private const string MinimumVisualStudioVersion = "MinimumVisualStudioVersion";

            public ProjectInspector(string filename)
            {
                try
                {
                    xmlProj = Microsoft.Build.Construction.ProjectRootElement.Open(filename);
                }
                catch (Microsoft.Build.BuildEngine.InvalidProjectFileException)
                {
                    // leave xmlProj non-initialized, other methods will check its state in prologue
                }                
            }
            
            /// we consider project to be Dev10- if it doesn't have any imports that belong to higher versions
            public bool IsLikeDev10MinusProject()
            {
                if (xmlProj == null)
                    return false;
                
                const string fsharpFS45TargetsPath      = @"$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.FSharp.Targets";
                const string fsharpPortableDev11TargetsPath = @"$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.Portable.FSharp.Targets";
                // Dev12+ projects import *.targets files using property
                const string fsharpDev12PlusImportsValue = @"$(FSharpTargetsPath)";

                foreach(var import in xmlProj.Imports)
                {
                    if (
                        IsEqual(import.Project, fsharpFS45TargetsPath) || 
                        IsEqual(import.Project, fsharpPortableDev11TargetsPath) ||
                        IsEqual(import.Project, fsharpDev12PlusImportsValue)
                       )
                    {
                        return false;
                    }
                }
                return true;
            }

            private bool IsEqual(string s1, string s2)
            {
                return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
            }

            public bool IsPoisoned(System.IServiceProvider sp)
            {
                if (xmlProj == null)
                    return false;

                // use IVsAppCompat service to determine current VS version
                var appCompatService = (IVsAppCompat)sp.GetService(typeof(SVsSolution));
                string currentDesignTimeCompatVersionString = null;
                appCompatService.GetCurrentDesignTimeCompatVersion(out currentDesignTimeCompatVersionString);
                // currentDesignTimeCompatVersionString can look like 12.0 
                // code in vsproject\vsproject\vsprjfactory.cpp uses _wtoi that will ignore part of string after dot
                // we'll do the same trick
                var indexOfDot = currentDesignTimeCompatVersionString.IndexOf('.');
                if (indexOfDot != -1)
                {
                    currentDesignTimeCompatVersionString = currentDesignTimeCompatVersionString.Substring(0, indexOfDot);
                }
                var currentDesignTimeCompatVersion = int.Parse(currentDesignTimeCompatVersionString);
                foreach (var pg in xmlProj.PropertyGroups)
                {
                    foreach (var p in pg.Properties)
                    {
                        if (string.CompareOrdinal(p.Name, MinimumVisualStudioVersion) == 0)
                        {
                            var s = p.Value;
                            if (!string.IsNullOrEmpty(s))
                            {
                                int ver;
                                int.TryParse(s, out ver);
                                if (ver > currentDesignTimeCompatVersion)
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

#endif

        private IProjectEvents GetProjectEventsProvider()
        {
            ProjectPackage projectPackage = this.package as ProjectPackage;
            Debug.Assert(projectPackage != null, "Package not inherited from framework");
            if (projectPackage != null)
            {
                foreach (SolutionListener listener in projectPackage.SolutionListeners)
                {
                    IProjectEvents projectEvents = listener as IProjectEvents;
                    if (projectEvents != null)
                    {
                        return projectEvents;
                    }
                }
            }

            return null;
        }

        private string m_lastUpgradedProjectFile;
        private const string SCC_PROJECT_NAME = "SccProjectName";
        private string m_sccProjectName;
        private const string SCC_AUX_PATH = "SccAuxPath";
        private string m_sccAuxPath;
        private const string SCC_LOCAL_PATH = "SccLocalPath";
        private string m_sccLocalPath;
        private const string SCC_PROVIDER = "SccProvider";
        private string m_sccProvider;
        public virtual int GetSccInfo(string projectFileName, out string sccProjectName, out string sccAuxPath, out string sccLocalPath, out string provider)
        {
            // we should only be asked for SCC info on a project that we have just upgraded.
            if (!String.Equals(this.m_lastUpgradedProjectFile, projectFileName, StringComparison.OrdinalIgnoreCase))
            {
                sccProjectName = "";
                sccAuxPath = "";
                sccLocalPath = "";
                provider = "";
                return VSConstants.E_FAIL;
            }
            sccProjectName = this.m_sccProjectName;
            sccAuxPath = this.m_sccAuxPath;
            sccLocalPath = this.m_sccLocalPath;
            provider = this.m_sccProvider;
            return VSConstants.S_OK;
        }

        int IVsProjectUpgradeViaFactory.UpgradeProject_CheckOnly(string projectFileName, IVsUpgradeLogger upgradeLogger, out int upgradeRequired, out Guid newProjectFactory, out uint upgradeCapabilityFlags)
        {
            __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS upgradeRequiredFlag;
            var hr = DoUpgradeProject_CheckOnly(projectFileName, upgradeLogger, out upgradeRequiredFlag, out newProjectFactory, out upgradeCapabilityFlags);
            upgradeRequired = upgradeRequiredFlag != __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_NOREPAIR ? 1 : 0;
            return hr;
        }

        void IVsProjectUpgradeViaFactory4.UpgradeProject_CheckOnly(string projectFileName, IVsUpgradeLogger upgradeLogger, out uint upgradeRequired, out Guid newProjectFactory, out uint upgradeCapabilityFlags)
        {
            __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS upgradeRequiredFlag;
            DoUpgradeProject_CheckOnly(projectFileName, upgradeLogger, out upgradeRequiredFlag, out newProjectFactory, out upgradeCapabilityFlags);
            upgradeRequired = (uint)upgradeRequiredFlag;
        }

        public virtual int DoUpgradeProject_CheckOnly(string projectFileName, IVsUpgradeLogger upgradeLogger, out __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS upgradeRequired, out Guid newProjectFactory, out uint upgradeCapabilityFlags)
        {
            newProjectFactory = GetType().GUID;
            var project = ProjectRootElement.Open(projectFileName);
            // enable Side-by-Side and CopyBackup support
            upgradeCapabilityFlags = (uint)(__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_BACKUPSUPPORTED | __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_COPYBACKUP | __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_SXSBACKUP);
#if FX_ATLEAST_45

            if (this.buildEngine.GetLoadedProjects(projectFileName).Count > 0)
            {
                // project has already been loaded
                upgradeRequired = __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_NOREPAIR;
                return VSConstants.S_OK;
            }
            var projectInspector = new ProjectInspector(projectFileName);
            if (projectInspector.IsPoisoned(Site))
            {
                // poisoned project cannot be opened (does not require upgrade)
                upgradeRequired = __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_NOREPAIR;
                return VSConstants.S_OK;
            }
#endif

            // only upgrade known tool versions.
#if FX_ATLEAST_45
            if (string.Equals("4.0", project.ToolsVersion, StringComparison.Ordinal))
            {
                // For 4.0, we need to take a deeper look.  The logic is in 
                //     vsproject\xmake\XMakeConversion\ProjectFileConverter.cs
                var projectConverter = new Microsoft.Build.Conversion.ProjectFileConverter();
                projectConverter.OldProjectFile = projectFileName;
                projectConverter.NewProjectFile = projectFileName;
                if (projectConverter.FSharpSpecificConversions(false))
                {
                    upgradeRequired = 
                        projectInspector.IsLikeDev10MinusProject() 
                            ? __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_ONEWAYUPGRADE 
                            : __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_SAFEREPAIR;
                    return VSConstants.S_OK;
                }
                else
                {
                    upgradeRequired = __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_NOREPAIR;
                    return VSConstants.S_OK;
                }
            }
            else 
#endif
            if (string.Equals("3.5", project.ToolsVersion, StringComparison.Ordinal) 
                     || string.Equals("2.0", project.ToolsVersion, StringComparison.Ordinal))

             {
                // For 3.5 or 2.0, we always need to upgrade.
                upgradeRequired = __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_ONEWAYUPGRADE;
                return VSConstants.S_OK;
             }
            upgradeRequired = __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_NOREPAIR;
            return VSConstants.S_OK;
        }

        private int NormalizeUpgradeFlag(uint upgradeFlag, out __VSPPROJECTUPGRADEVIAFACTORYFLAGS flag, ref string copyLocation)
        {
            flag = (__VSPPROJECTUPGRADEVIAFACTORYFLAGS)upgradeFlag;
            // normalize flags
            switch (flag)
            {
                case __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_COPYBACKUP:
                case __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_SXSBACKUP:
                    break;
                default:
                    // ignore unknown flags
                    flag &= (__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_COPYBACKUP | __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_SXSBACKUP);
                    break;
            }

            //if copy upgrade, then we need a copy location that ends in a backslash.
            if (HasCopyBackupFlag(flag))
            {
                if (string.IsNullOrEmpty(copyLocation) || copyLocation[copyLocation.Length - 1] != '\\')
                {                    
                    if (HasSxSBackupFlag(flag))
                    {
                        //if both SxS AND CopyBack were specified, then fall back to SxS
                        flag &= ~__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_COPYBACKUP;
                    }
                    else
                    {
                        //Only CopyBackup was specified and an invalid backup location was given, so bail
                        return VSConstants.E_INVALIDARG;
                    }
                }
                else
                {
                    //Favor copy backup to SxS backup
                    flag &= ~__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_SXSBACKUP;
                }
            }
            return VSConstants.S_OK;
        }

        public virtual int UpgradeProject(
            string projectFilePath, 
            uint upgradeFlag, 
            string initialCopyLocation, 
            out string upgradeFullyQualifiedFileName, 
            IVsUpgradeLogger upgradeLogger, 
            out int upgradeRequired,
            out Guid newProjectFactory
            )
        {
            // initialize out params in case of failure
            upgradeFullyQualifiedFileName = null;
            upgradeRequired = 0;
            newProjectFactory = Guid.Empty;

            __VSPPROJECTUPGRADEVIAFACTORYFLAGS flag;
            string copyLocation = initialCopyLocation;
            var r = NormalizeUpgradeFlag(upgradeFlag, out flag, ref copyLocation);
            if (r != VSConstants.S_OK)
            {
                return r;
            }

            string projectName = Path.GetFileNameWithoutExtension(projectFilePath);
            var logger = new ProjectUpgradeLogger(upgradeLogger, projectName, projectFilePath);

            uint ignore;
            ((IVsProjectUpgradeViaFactory)this).UpgradeProject_CheckOnly(projectFilePath, upgradeLogger, out upgradeRequired, out newProjectFactory, out ignore);

            // no upgrade required and not 'copy-backup'
            if (upgradeRequired == 0 && !HasCopyBackupFlag(flag))
            {
                //Write an informational message "No upgrade required for project foo"?
                logger.LogInfo(SR.GetString(SR.ProjectConversionNotRequired));
                logger.LogInfo(SR.GetString(SR.ConversionNotRequired));

                upgradeFullyQualifiedFileName = projectFilePath;
                return VSConstants.S_OK;
            }

            // upgrade is not required but backup may still be needed

            var projectFileName = Path.GetFileName(projectFilePath);
            upgradeFullyQualifiedFileName = projectFilePath;

            if (HasSxSBackupFlag(flag))
            {
                // for SxS call use location near the original file
                copyLocation = Path.GetDirectoryName(projectFilePath);
            }

            // workflow is taken from vsprjfactory.cpp (vsproject\vsproject)
            // 1. convert the project (in-memory)
            // 2. save SCC related info
            // 3. use data stored on step 2 in GetSccInfo calls (during QueryEditFiles)
            // 4. if succeeded - save project on disk
            // F# doesn't use .user file so all related code is skipped
            try
            {
                // load MSBuild project in memory: this will be needed in all cases not depending whether upgrade is required or not
                // we use this project to determine the set of files to copy
                ProjectRootElement convertedProject = ConvertProject(projectFilePath, logger);
                if (convertedProject == null)
                {
                    throw new ProjectUpgradeFailedException();
                }

                // OK, we need upgrade, save SCC info and ask if project file can be edited
                if (upgradeRequired != 0)
                {
                    // 2. save SCC related info
                    this.m_lastUpgradedProjectFile = projectFilePath;
                    foreach (var property in convertedProject.Properties)
                    {
                        switch (property.Name)
                        {
                            case SCC_LOCAL_PATH:
                                this.m_sccLocalPath = property.Value;
                                break;
                            case SCC_AUX_PATH:
                                this.m_sccAuxPath = property.Value;
                                break;
                            case SCC_PROVIDER:
                                this.m_sccProvider = property.Value;
                                break;
                            case SCC_PROJECT_NAME:
                                this.m_sccProjectName = property.Value;
                                break;
                            default:
                                break;
                        }
                    }

                    // 3. Query for edit (this call may query information stored on previous step)
                    IVsQueryEditQuerySave2 queryEdit = site.GetService(typeof(SVsQueryEditQuerySave)) as IVsQueryEditQuerySave2;
                    if (queryEdit != null)
                    {
                        uint editVerdict;
                        uint queryEditMoreInfo;
                        const tagVSQueryEditFlags tagVSQueryEditFlags_QEF_AllowUnopenedProjects = (tagVSQueryEditFlags)0x80;

                        int hr = queryEdit.QueryEditFiles(
                            (uint)(tagVSQueryEditFlags.QEF_ForceEdit_NoPrompting | tagVSQueryEditFlags.QEF_DisallowInMemoryEdits | tagVSQueryEditFlags_QEF_AllowUnopenedProjects),
                            1, new[] { projectFilePath }, null, null, out editVerdict, out queryEditMoreInfo);

                        if (ErrorHandler.Failed(hr))
                        {
                            throw new ProjectUpgradeFailedException();
                        }

                        if (editVerdict != (uint)tagVSQueryEditResult.QER_EditOK)
                        {
                            throw new ProjectUpgradeFailedException(SR.GetString(SR.UpgradeCannotOpenProjectFileForEdit));
                        }

                        // If file was modified during the checkout, maybe upgrade is not needed
                        if ((queryEditMoreInfo & (uint)tagVSQueryEditResultFlags.QER_MaybeChanged) != 0)
                        {
                            ((IVsProjectUpgradeViaFactory)this).UpgradeProject_CheckOnly(projectFilePath, upgradeLogger, out upgradeRequired, out newProjectFactory, out ignore);
                            if (upgradeRequired == 0)
                            {
                                if (logger != null)
                                {
                                    logger.LogInfo(SR.GetString(SR.UpgradeNoNeedToUpgradeAfterCheckout));
                                }

                                return VSConstants.S_OK;
                            }
                        }
                    }
                }

                // 3.1 copy backup 
                BackupProjectFilesIfNeeded(projectFilePath, logger, flag, copyLocation, convertedProject);

                // 4. if we have performed upgrade - save project to disk
                if (upgradeRequired != 0)
                {
                    try
                    {
                        convertedProject.Save(projectFilePath);
                    }
                    catch (Exception ex)
                    {
                        throw new ProjectUpgradeFailedException(ex.Message, ex);
                    }
                }
                // 821083: "Converted" should NOT be localized - it is referenced in the XSLT used to display the UpgradeReport
                logger.LogStatus("Converted");

            }
            catch (ProjectUpgradeFailedException ex)
            {
                var exception = ex.InnerException ?? ex;

                if (exception != null && !string.IsNullOrEmpty(exception.Message))
                    logger.LogError(exception.Message);

                upgradeFullyQualifiedFileName = "";
                m_lastUpgradedProjectFile = null;
                return VSConstants.E_FAIL;
            }
            return VSConstants.S_OK;
        }

        private void BackupProjectFilesIfNeeded(
            string projectFilePath, 
            ProjectUpgradeLogger logger, 
            __VSPPROJECTUPGRADEVIAFACTORYFLAGS flag, 
            string copyLocation, 
            ProjectRootElement convertedProject
            )
        {
            var projectName = Path.GetFileNameWithoutExtension(projectFilePath);
            var projectFileName = Path.GetFileName(projectFilePath);

            if (HasCopyBackupFlag(flag) || HasSxSBackupFlag(flag))
            {
                if (HasSxSBackupFlag(flag) && !Directory.Exists(copyLocation))
                {
                    Debug.Assert(false, "Env should create the directory for us");
                    throw new ProjectUpgradeFailedException();
                }

                // copy project file
                {
                    var targetFilePath = Path.Combine(copyLocation, projectFileName);
                    if (HasSxSBackupFlag(flag))
                    {
                        bool ignored;
                        targetFilePath = GetUniqueFileName(targetFilePath + ".old", out ignored);
                    }

                    try
                    {
                        File.Copy(projectFilePath, targetFilePath);
                        logger.LogInfo(SR.GetString(SR.ProjectBackupSuccessful, targetFilePath));
                    }
                    catch (Exception ex)
                    {
                        var message = SR.GetString(SR.ErrorMakingProjectBackup, targetFilePath);
                        throw new ProjectUpgradeFailedException(string.Format("{0} : {1}", message, ex.Message));
                    }
                }

                if (HasCopyBackupFlag(flag))
                {
                    //Now iterate through the project items and copy them to the new location
                    //All projects under the solution retain its folder hierarchy
                    var types = new[] { "Compile", "None", "Content", "EmbeddedResource", "Resource", "BaseApplicationManifest", "ApplicationDefinition", "Page" };

                    var metadataLookup =
                            convertedProject
                            .Items
                            .GroupBy(i => i.ItemType)
                            .ToDictionary(x => x.Key);

                    var sourceProjectDir = Path.GetDirectoryName(projectFilePath);
                    foreach (var ty in types)
                    {
                        if (metadataLookup.ContainsKey(ty))
                        {
                            foreach (var item in metadataLookup[ty])
                            {
                                var linkMetadataElement = item.Metadata.FirstOrDefault(me => me.Name == "Link");
                                string linked = linkMetadataElement != null && !string.IsNullOrEmpty(linkMetadataElement.Value) ? linkMetadataElement.Value : null;
                                
                                var include = item.Include;

                                Debug.Assert(!string.IsNullOrEmpty(include));

                                string sourceFilePath;

                                var targetFileName = Path.Combine(copyLocation, linked ?? include);

                                if (Path.IsPathRooted(include))
                                {
                                    //if the path is fully qualified already, then just use it
                                    sourceFilePath = include;
                                }
                                else
                                {
                                    //otherwise tack the filename on to the path
                                    sourceFilePath = Path.Combine(sourceProjectDir, include);
                                }
                                if (linked != null && include[0] == '.')
                                {
                                    //if linked file up a level (or more), then turn it into a path without the ..\ in the middle
                                    sourceFilePath = Path.GetFullPath(sourceFilePath);
                                }

                                bool initiallyUnique;
                                targetFileName = GetUniqueFileName(targetFileName, out initiallyUnique);
                                if (!initiallyUnique)
                                {
                                    logger.LogInfo(SR.GetString(SR.BackupNameConflict, targetFileName));
                                }

                                //Warn user in upgrade log if linked files are used "project may not build"
                                if (linked != null && HasCopyBackupFlag(flag))
                                {
                                    logger.LogWarning(SR.GetString(SR.ProjectContainsLinkedFile, targetFileName));
                                }

                                // ensure target folder exists
                                Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));

                                try
                                {
                                    File.Copy(sourceFilePath, targetFileName);
                                    logger.LogInfo(SR.GetString(SR.BackupSuccessful, targetFileName));
                                }
                                catch (Exception ex)
                                {
                                    var message = SR.GetString(SR.ErrorMakingBackup, targetFileName);
                                    logger.LogError(string.Format("{0} : {1}", message, ex.Message));
                                }                                
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allows to avoid repeatable checks if logger exists + provides more convinient interface
        /// </summary>
        private class ProjectUpgradeLogger
        {
            private Action<__VSUL_ERRORLEVEL, string> write;
            public ProjectUpgradeLogger(IVsUpgradeLogger logger, string projectName, string projectFileName)
            {
                if (logger != null)
                {
                    write = (errLevel, message) => logger.LogMessage((uint)errLevel, projectName, projectFileName, message) ;
                }
                else
                {
                    write = delegate { };
                }
            }

            public void LogInfo(string message)
            {
                write(__VSUL_ERRORLEVEL.VSUL_INFORMATIONAL, message);
            }
            public void LogWarning(string message)
            {
                write(__VSUL_ERRORLEVEL.VSUL_WARNING, message);
            }
            public void LogError(string message)
            {
                write(__VSUL_ERRORLEVEL.VSUL_ERROR, message);
            }
            public void LogStatus(string message)
            {
                write(__VSUL_ERRORLEVEL.VSUL_STATUSMSG, message);
            }
        }

        private class ProjectUpgradeFailedException : Exception
        {
            public ProjectUpgradeFailedException() : base() { }
            public ProjectUpgradeFailedException(string message) : base(message) { }
            public ProjectUpgradeFailedException(string message, Exception inner) : base(message, inner) { }
        }

        /// <summary>
        /// Performs in-memory conversion of the project with a given path
        /// </summary>
        /// <returns>Root element of the converted project or <c>null</c> if conversion failed.</returns>
        private ProjectRootElement ConvertProject(string projectFileName, ProjectUpgradeLogger logger)
        {
            var projectConverter = new Microsoft.Build.Conversion.ProjectFileConverter();
            projectConverter.OldProjectFile = projectFileName;
            projectConverter.NewProjectFile = projectFileName;
            ProjectRootElement convertedProject = null;
            try
            {
                convertedProject = projectConverter.ConvertInMemory();
            }
            catch (Exception ex)
            {
                logger.LogInfo(ex.Message);
            }
            return convertedProject;
        }

        /// <summary>
        /// Helper for checking if flag has PUVFF_SXSBACKUP value
        /// </summary>
        private static bool HasSxSBackupFlag(__VSPPROJECTUPGRADEVIAFACTORYFLAGS flag)
        {
            return flag.HasFlag(__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_SXSBACKUP);
        }

        /// <summary>
        /// Helper for checking if flag has PUVFF_COPYBACKUP value
        /// </summary>
        private static bool HasCopyBackupFlag(__VSPPROJECTUPGRADEVIAFACTORYFLAGS flag)
        {
            return flag.HasFlag(__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_COPYBACKUP);
        }

        /// <summary>
        /// Generates unique name for the given path by appending 0..n to the file name
        /// </summary>
        /// <param name="initialPath">Initial location</param>
        /// <param name="initiallyUnique"><c>true</c> if original file name was already unique, otherwise - <c>false</c></param>
        /// <returns>Unique file path</returns>
        private string GetUniqueFileName(string initialPath, out bool initiallyUnique)
        {
            initiallyUnique = true;
            if (!File.Exists(initialPath))
                return initialPath;

            initiallyUnique = false;
            var originalExtension = Path.GetExtension(initialPath);
            var pathSansExtension = Path.Combine(Path.GetDirectoryName(initialPath), Path.GetFileNameWithoutExtension(initialPath));
            var i = 1;
            while(true)
            {
                var f = string.Format("{0}{1}{2}", pathSansExtension, i, originalExtension);
                if (!File.Exists(f))
                    return f;
                i++;
            }
        }
    }
}
