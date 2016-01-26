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
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MSBuild = Microsoft.Build.Evaluation;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Creates projects within the solution
    /// </summary>

    public abstract class ProjectFactory : FlavoredProjectFactoryBase,
#if DEV11_OR_LATER
 IVsAsynchronousProjectCreate,
        IVsProjectUpgradeViaFactory4,
#endif
 IVsProjectUpgradeViaFactory {
        #region fields
        private System.IServiceProvider site;

        /// <summary>
        /// The msbuild engine that we are going to use.
        /// </summary>
        private MSBuild.ProjectCollection buildEngine;

        /// <summary>
        /// The msbuild project for the project file.
        /// </summary>
        private MSBuild.Project buildProject;
#if DEV11_OR_LATER
        private readonly Lazy<IVsTaskSchedulerService> taskSchedulerService;
#endif

        // (See GetSccInfo below.)
        // When we upgrade a project, we need to cache the SCC info in case
        // somebody calls to ask for it via GetSccInfo.
        // We only need to know about the most recently upgraded project, and
        // can fail for other projects.
        private string _cachedSccProject;
        private string _cachedSccProjectName, _cachedSccAuxPath, _cachedSccLocalPath, _cachedSccProvider;

        #endregion

        #region properties
        [Obsolete("Use Site instead")]
        protected Microsoft.VisualStudio.Shell.Package Package {
            get {
                return (Microsoft.VisualStudio.Shell.Package)this.site;
            }
        }

        protected internal System.IServiceProvider Site {
            get {
                return this.site;
            }
            internal set {
                this.site = value;
            }
        }

        #endregion

        #region ctor
        [Obsolete("Provide an IServiceProvider instead of a package")]
        protected ProjectFactory(Microsoft.VisualStudio.Shell.Package package)
            : this((IServiceProvider)package) {
        }

        protected ProjectFactory(IServiceProvider serviceProvider)
            : base(serviceProvider) {
            this.site = serviceProvider;
            this.buildEngine = MSBuild.ProjectCollection.GlobalProjectCollection;
#if DEV11_OR_LATER
            this.taskSchedulerService = new Lazy<IVsTaskSchedulerService>(() => Site.GetService(typeof(SVsTaskSchedulerService)) as IVsTaskSchedulerService);
#endif
        }

        #endregion

        #region abstract methods
        internal abstract ProjectNode CreateProject();
        #endregion

        #region overriden methods
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
        protected override void CreateProject(string fileName, string location, string name, uint flags, ref Guid projectGuid, out IntPtr project, out int canceled) {
            using (new DebugTimer("CreateProject")) {
                project = IntPtr.Zero;
                canceled = 0;

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
        }

        /// <summary>
        /// Instantiate the project class, but do not proceed with the
        /// initialization just yet.
        /// Delegate to CreateProject implemented by the derived class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The global property handles is instantiated here and used in the project node that will Dispose it")]
        protected override object PreCreateForOuter(IntPtr outerProjectIUnknown) {
            Utilities.CheckNotNull(this.buildProject, "The build project should have been initialized before calling PreCreateForOuter.");

            // Please be very carefull what is initialized here on the ProjectNode. Normally this should only instantiate and return a project node.
            // The reason why one should very carefully add state to the project node here is that at this point the aggregation has not yet been created and anything that would cause a CCW for the project to be created would cause the aggregation to fail
            // Our reasoning is that there is no other place where state on the project node can be set that is known by the Factory and has to execute before the Load method.
            ProjectNode node = this.CreateProject();
            Utilities.CheckNotNull(node, "The project failed to be created");
            node.BuildEngine = this.buildEngine;
            node.BuildProject = this.buildProject;
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
        protected override string ProjectTypeGuids(string file) {
            // Load the project so we can extract the list of GUIDs

            this.buildProject = Utilities.ReinitializeMsBuildProject(this.buildEngine, file, this.buildProject);

            // Retrieve the list of GUIDs, if it is not specify, make it our GUID
            string guids = buildProject.GetPropertyValue(ProjectFileConstants.ProjectTypeGuids);
            if (String.IsNullOrEmpty(guids))
                guids = this.GetType().GUID.ToString("B");

            return guids;
        }
        #endregion

#if DEV11_OR_LATER

        public virtual bool CanCreateProjectAsynchronously(ref Guid rguidProjectID, string filename, uint flags) {
            return true;
        }

        public void OnBeforeCreateProjectAsync(ref Guid rguidProjectID, string filename, string location, string pszName, uint flags) {
        }

        public IVsTask CreateProjectAsync(ref Guid rguidProjectID, string filename, string location, string pszName, uint flags) {
            Guid iid = typeof(IVsHierarchy).GUID;
            return VsTaskLibraryHelper.CreateAndStartTask(taskSchedulerService.Value, VsTaskRunContext.UIThreadBackgroundPriority, VsTaskLibraryHelper.CreateTaskBody(() => {
                IntPtr project;
                int cancelled;
                CreateProject(filename, location, pszName, flags, ref iid, out project, out cancelled);
                if (cancelled != 0) {
                    throw new OperationCanceledException();
                }

                return Marshal.GetObjectForIUnknown(project);
            }));
        }

#endif

        #region Project Upgrades

        /// <summary>
        /// Override this method to upgrade project files.
        /// </summary>
        /// <param name="projectXml">
        /// The XML of the project file being upgraded. This may be modified
        /// directly or replaced with a new element.
        /// </param>
        /// <param name="userProjectXml">
        /// The XML of the user file being upgraded. This may be modified
        /// directly or replaced with a new element.
        /// 
        /// If there is no user file before upgrading, this may be null. If it
        /// is non-null on return, the file is created.
        /// </param>
        /// <param name="log">
        /// Callback to log messages. These messages will be added to the
        /// migration log that is displayed after upgrading completes.
        /// </param>
        protected virtual void UpgradeProject(
            ref ProjectRootElement projectXml,
            ref ProjectRootElement userProjectXml,
            Action<__VSUL_ERRORLEVEL, string> log
        ) { }

        /// <summary>
        /// Determines whether a project needs to be upgraded.
        /// </summary>
        /// <param name="projectXml">
        /// The XML of the project file being upgraded.
        /// </param>
        /// <param name="userProjectXml">
        /// The XML of the user file being upgraded, or null if no user file
        /// exists.
        /// </param>
        /// <param name="log">
        /// Callback to log messages. These messages will be added to the
        /// migration log that is displayed after upgrading completes.
        /// </param>
        /// <param name="projectFactory">
        /// The project factory that will be used. This may be replaced with
        /// another Guid if a new project factory should be used to upgrade the
        /// project.
        /// </param>
        /// <param name="backupSupport">
        /// The level of backup support requested for the project. By default,
        /// the project file (and user file, if any) will be copied alongside
        /// the originals with ".old" added to the filenames.
        /// </param>
        /// <returns>
        /// The form of upgrade required.
        /// </returns>
        protected virtual ProjectUpgradeState UpgradeProjectCheck(
            ProjectRootElement projectXml,
            ProjectRootElement userProjectXml,
            Action<__VSUL_ERRORLEVEL, string> log,
            ref Guid projectFactory,
            ref __VSPPROJECTUPGRADEVIAFACTORYFLAGS backupSupport
        ) {
            return ProjectUpgradeState.NotNeeded;
        }



        class UpgradeLogger {
            private readonly string _projectFile;
            private readonly string _projectName;
            private readonly IVsUpgradeLogger _logger;

            public UpgradeLogger(string projectFile, IVsUpgradeLogger logger) {
                _projectFile = projectFile;
                _projectName = Path.GetFileNameWithoutExtension(projectFile);
                _logger = logger;
            }

            public void Log(__VSUL_ERRORLEVEL level, string text) {
                if (_logger != null) {
                    ErrorHandler.ThrowOnFailure(_logger.LogMessage((uint)level, _projectName, _projectFile, text));
                }
            }
        }

        int IVsProjectUpgradeViaFactory.GetSccInfo(
            string bstrProjectFileName,
            out string pbstrSccProjectName,
            out string pbstrSccAuxPath,
            out string pbstrSccLocalPath,
            out string pbstrProvider
        ) {
            if (string.Equals(_cachedSccProject, bstrProjectFileName, StringComparison.OrdinalIgnoreCase)) {
                pbstrSccProjectName = _cachedSccProjectName;
                pbstrSccAuxPath = _cachedSccAuxPath;
                pbstrSccLocalPath = _cachedSccLocalPath;
                pbstrProvider = _cachedSccProvider;
                return VSConstants.S_OK;
            }
            pbstrSccProjectName = null;
            pbstrSccAuxPath = null;
            pbstrSccLocalPath = null;
            pbstrProvider = null;
            return VSConstants.E_FAIL;
        }

        int IVsProjectUpgradeViaFactory.UpgradeProject(
            string bstrFileName,
            uint fUpgradeFlag,
            string bstrCopyLocation,
            out string pbstrUpgradedFullyQualifiedFileName,
            IVsUpgradeLogger pLogger,
            out int pUpgradeRequired,
            out Guid pguidNewProjectFactory
        ) {
            pbstrUpgradedFullyQualifiedFileName = null;

            // We first run (or re-run) the upgrade check and bail out early if
            // there is actually no need to upgrade.
            uint dummy;
            var hr = ((IVsProjectUpgradeViaFactory)this).UpgradeProject_CheckOnly(
                bstrFileName,
                pLogger,
                out pUpgradeRequired,
                out pguidNewProjectFactory,
                out dummy
            );

            if (!ErrorHandler.Succeeded(hr)) {
                return hr;
            }

            var logger = new UpgradeLogger(bstrFileName, pLogger);

            var backup = (__VSPPROJECTUPGRADEVIAFACTORYFLAGS)fUpgradeFlag;
            bool anyBackup, sxsBackup, copyBackup;
            anyBackup = backup.HasFlag(__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_BACKUPSUPPORTED);
            if (anyBackup) {
                sxsBackup = backup.HasFlag(__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_SXSBACKUP);
                copyBackup = !sxsBackup && backup.HasFlag(__VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_COPYBACKUP);
            } else {
                sxsBackup = copyBackup = false;
            }

            if (copyBackup) {
                throw new NotSupportedException("PUVFF_COPYBACKUP is not supported");
            }

            pbstrUpgradedFullyQualifiedFileName = bstrFileName;

            if (pUpgradeRequired == 0 && !copyBackup) {
                // No upgrade required, and no backup required.
                logger.Log(__VSUL_ERRORLEVEL.VSUL_INFORMATIONAL, SR.GetString(SR.UpgradeNotRequired));
                return VSConstants.S_OK;
            }

            try {
                UpgradeLogger logger2 = null;
                var userFileName = bstrFileName + ".user";
                if (File.Exists(userFileName)) {
                    logger2 = new UpgradeLogger(userFileName, pLogger);
                } else {
                    userFileName = null;
                }

                if (sxsBackup) {
                    // For SxS backups we want to put the old project file alongside
                    // the current one.
                    bstrCopyLocation = Path.GetDirectoryName(bstrFileName);
                }

                if (anyBackup) {
                    var namePart = Path.GetFileNameWithoutExtension(bstrFileName);
                    var extPart = Path.GetExtension(bstrFileName) + (sxsBackup ? ".old" : "");
                    var projectFileBackup = Path.Combine(bstrCopyLocation, namePart + extPart);
                    for (int i = 1; File.Exists(projectFileBackup); ++i) {
                        projectFileBackup = Path.Combine(
                            bstrCopyLocation,
                            string.Format("{0}{1}{2}", namePart, i, extPart)
                        );
                    }

                    File.Copy(bstrFileName, projectFileBackup);

                    // Back up the .user file if there is one
                    if (userFileName != null) {
                        if (sxsBackup) {
                            File.Copy(
                                userFileName,
                                Path.ChangeExtension(projectFileBackup, ".user.old")
                            );
                        } else {
                            File.Copy(userFileName, projectFileBackup + ".old");
                        }
                    }

                    // TODO: Implement support for backing up all files
                    //if (copyBackup) {
                    //  - Open the project
                    //  - Inspect all Items
                    //  - Copy those items that are referenced relative to the
                    //    project file into bstrCopyLocation
                    //}
                }


                var queryEdit = site.GetService(typeof(SVsQueryEditQuerySave)) as IVsQueryEditQuerySave2;
                if (queryEdit != null) {
                    uint editVerdict;
                    uint queryEditMoreInfo;
                    var tagVSQueryEditFlags_QEF_AllowUnopenedProjects = (tagVSQueryEditFlags)0x80;

                    ErrorHandler.ThrowOnFailure(queryEdit.QueryEditFiles(
                        (uint)(tagVSQueryEditFlags.QEF_ForceEdit_NoPrompting |
                            tagVSQueryEditFlags.QEF_DisallowInMemoryEdits |
                            tagVSQueryEditFlags_QEF_AllowUnopenedProjects),
                        1,
                        new[] { bstrFileName },
                        null,
                        null,
                        out editVerdict,
                        out queryEditMoreInfo
                    ));

                    if (editVerdict != (uint)tagVSQueryEditResult.QER_EditOK) {
                        logger.Log(__VSUL_ERRORLEVEL.VSUL_ERROR, SR.GetString(SR.UpgradeCannotCheckOutProject));
                        return VSConstants.E_FAIL;
                    }

                    // File may have been updated during checkout, so check
                    // again whether we need to upgrade.
                    if ((queryEditMoreInfo & (uint)tagVSQueryEditResultFlags.QER_MaybeChanged) != 0) {
                        hr = ((IVsProjectUpgradeViaFactory)this).UpgradeProject_CheckOnly(
                            bstrFileName,
                            pLogger,
                            out pUpgradeRequired,
                            out pguidNewProjectFactory,
                            out dummy
                        );

                        if (!ErrorHandler.Succeeded(hr)) {
                            return hr;
                        }
                        if (pUpgradeRequired == 0) {
                            logger.Log(__VSUL_ERRORLEVEL.VSUL_INFORMATIONAL, SR.GetString(SR.UpgradeNotRequired));
                            return VSConstants.S_OK;
                        }
                    }
                }

                // Load the project file and user file into MSBuild as plain
                // XML to make it easier for subclasses.
                var projectXml = ProjectRootElement.Open(bstrFileName);
                if (projectXml == null) {
                    throw new Exception(SR.GetString(SR.UpgradeCannotLoadProject));
                }

                var userXml = userFileName != null ? ProjectRootElement.Open(userFileName) : null;

                // Invoke our virtual UpgradeProject function. If it fails, it
                // will throw and we will log the exception.
                UpgradeProject(ref projectXml, ref userXml, logger.Log);

                // Get the SCC info from the project file.
                if (projectXml != null) {
                    _cachedSccProject = bstrFileName;
                    _cachedSccProjectName = string.Empty;
                    _cachedSccAuxPath = string.Empty;
                    _cachedSccLocalPath = string.Empty;
                    _cachedSccProvider = string.Empty;
                    foreach (var property in projectXml.Properties) {
                        switch (property.Name) {
                            case ProjectFileConstants.SccProjectName:
                                _cachedSccProjectName = property.Value;
                                break;
                            case ProjectFileConstants.SccAuxPath:
                                _cachedSccAuxPath = property.Value;
                                break;
                            case ProjectFileConstants.SccLocalPath:
                                _cachedSccLocalPath = property.Value;
                                break;
                            case ProjectFileConstants.SccProvider:
                                _cachedSccProvider = property.Value;
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Save the updated files.
                if (projectXml != null) {
                    projectXml.Save();
                }
                if (userXml != null) {
                    userXml.Save();
                }

                // Need to add "Converted" (unlocalized) to the report because
                // the XSLT refers to it.
                logger.Log(__VSUL_ERRORLEVEL.VSUL_STATUSMSG, "Converted");
                return VSConstants.S_OK;
            } catch (Exception ex) {
                if (ex.IsCriticalException()) {
                    throw;
                }

                logger.Log(__VSUL_ERRORLEVEL.VSUL_ERROR, SR.GetString(SR.UnexpectedUpgradeError, ex.Message));
                try {
                    ActivityLog.LogError(GetType().FullName, ex.ToString());
                } catch (InvalidOperationException) {
                    // Cannot log to ActivityLog. This may occur if we are
                    // outside of VS right now (for example, unit tests).
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }
                return VSConstants.E_FAIL;
            }
        }

        int IVsProjectUpgradeViaFactory.UpgradeProject_CheckOnly(
            string bstrFileName,
            IVsUpgradeLogger pLogger,
            out int pUpgradeRequired,
            out Guid pguidNewProjectFactory,
            out uint pUpgradeProjectCapabilityFlags
        ) {
            pUpgradeRequired = 0;
            pguidNewProjectFactory = Guid.Empty;

            if (!File.Exists(bstrFileName)) {
                pUpgradeProjectCapabilityFlags = 0;
                return VSConstants.E_INVALIDARG;
            }


            var backupSupport = __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_BACKUPSUPPORTED |
                __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_COPYBACKUP |
                __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_SXSBACKUP;
            var logger = new UpgradeLogger(bstrFileName, pLogger);
            try {
                var projectXml = ProjectRootElement.Open(bstrFileName);
                var userProjectName = bstrFileName + ".user";
                var userProjectXml = File.Exists(userProjectName) ? ProjectRootElement.Open(userProjectName) : null;

                var upgradeRequired = UpgradeProjectCheck(
                    projectXml,
                    userProjectXml,
                    logger.Log,
                    ref pguidNewProjectFactory,
                    ref backupSupport
                );

                if (upgradeRequired != ProjectUpgradeState.NotNeeded) {
                    pUpgradeRequired = 1;
                }
            } catch (Exception ex) {
                if (ex.IsCriticalException()) {
                    throw;
                }
                // Log the error and don't attempt to upgrade the project.
                logger.Log(__VSUL_ERRORLEVEL.VSUL_ERROR, SR.GetString(SR.UnexpectedUpgradeError, ex.Message));
                try {
                    ActivityLog.LogError(GetType().FullName, ex.ToString());
                } catch (InvalidOperationException) {
                    // Cannot log to ActivityLog. This may occur if we are
                    // outside of VS right now (for example, unit tests).
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }
                pUpgradeRequired = 0;
            }
            pUpgradeProjectCapabilityFlags = (uint)backupSupport;

            // If the upgrade checker set the factory GUID to ourselves, we need
            // to clear it
            if (pguidNewProjectFactory == GetType().GUID) {
                pguidNewProjectFactory = Guid.Empty;
            }

            return VSConstants.S_OK;
        }

#if DEV11_OR_LATER
        void IVsProjectUpgradeViaFactory4.UpgradeProject_CheckOnly(
            string bstrFileName,
            IVsUpgradeLogger pLogger,
            out uint pUpgradeRequired,
            out Guid pguidNewProjectFactory,
            out uint pUpgradeProjectCapabilityFlags
        ) {
            pguidNewProjectFactory = Guid.Empty;

            if (!File.Exists(bstrFileName)) {
                pUpgradeRequired = 0;
                pUpgradeProjectCapabilityFlags = 0;
                return;
            }

            var backupSupport = __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_BACKUPSUPPORTED |
                __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_COPYBACKUP |
                __VSPPROJECTUPGRADEVIAFACTORYFLAGS.PUVFF_SXSBACKUP;
            var logger = new UpgradeLogger(bstrFileName, pLogger);
            try {
                var projectXml = ProjectRootElement.Open(bstrFileName);
                var userProjectName = bstrFileName + ".user";
                var userProjectXml = File.Exists(userProjectName) ? ProjectRootElement.Open(userProjectName) : null;

                var upgradeRequired = UpgradeProjectCheck(
                    projectXml,
                    userProjectXml,
                    logger.Log,
                    ref pguidNewProjectFactory,
                    ref backupSupport
                );

                switch (upgradeRequired) {
                    case ProjectUpgradeState.SafeRepair:
                        pUpgradeRequired = (uint)__VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_SAFEREPAIR;
                        break;
                    case ProjectUpgradeState.UnsafeRepair:
                        pUpgradeRequired = (uint)__VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_UNSAFEREPAIR;
                        break;
                    case ProjectUpgradeState.OneWayUpgrade:
                        pUpgradeRequired = (uint)__VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_ONEWAYUPGRADE;
                        break;
                    case ProjectUpgradeState.Incompatible:
                        pUpgradeRequired = (uint)__VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_INCOMPATIBLE;
                        break;
                    case ProjectUpgradeState.Deprecated:
                        pUpgradeRequired = (uint)__VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_DEPRECATED;
                        break;
                    default:
                    case ProjectUpgradeState.NotNeeded:
                        pUpgradeRequired = (uint)__VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_NOREPAIR;
                        break;
                }

            } catch (Exception ex) {
                if (ex.IsCriticalException()) {
                    throw;
                }
                // Log the error and don't attempt to upgrade the project.
                logger.Log(__VSUL_ERRORLEVEL.VSUL_ERROR, SR.GetString(SR.UnexpectedUpgradeError, ex.Message));
                try {
                    ActivityLog.LogError(GetType().FullName, ex.ToString());
                } catch (InvalidOperationException) {
                    // Cannot log to ActivityLog. This may occur if we are
                    // outside of VS right now (for example, unit tests).
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }
                pUpgradeRequired = (uint)__VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS.VSPUVF_PROJECT_NOREPAIR;
            }
            pUpgradeProjectCapabilityFlags = (uint)backupSupport;

            // If the upgrade checker set the factory GUID to ourselves, we need
            // to clear it
            if (pguidNewProjectFactory == GetType().GUID) {
                pguidNewProjectFactory = Guid.Empty;
            }
        }
#endif
        #endregion
    }

    /// <summary>
    /// Status indicating whether a project upgrade should occur and how the
    /// project will be affected.
    /// </summary>
    public enum ProjectUpgradeState {
        /// <summary>
        /// No action will be taken.
        /// </summary>
        NotNeeded,
        /// <summary>
        /// The project will be upgraded without asking the user.
        /// </summary>
        SafeRepair,
        /// <summary>
        /// The project will be upgraded with the user's permission.
        /// </summary>
        UnsafeRepair,
        /// <summary>
        /// The project will be upgraded with the user's permission and they
        /// will be informed that the project will no longer work with earlier
        /// versions of Visual Studio.
        /// </summary>
        OneWayUpgrade,
        /// <summary>
        /// The project will be marked as incompatible.
        /// </summary>
        Incompatible,
        /// <summary>
        /// The project will be marked as deprecated.
        /// </summary>
        Deprecated
    }
}
