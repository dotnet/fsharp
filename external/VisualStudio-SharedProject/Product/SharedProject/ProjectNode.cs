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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using EnvDTE;
using Microsoft.Build.Execution;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using MSBuild = Microsoft.Build.Evaluation;
using MSBuildConstruction = Microsoft.Build.Construction;
using MSBuildExecution = Microsoft.Build.Execution;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Manages the persistent state of the project (References, options, files, etc.) and deals with user interaction via a GUI in the form a hierarchy.
    /// </summary>

    internal abstract partial class ProjectNode : HierarchyNode,
        IVsUIHierarchy,
        IVsPersistHierarchyItem2,
        IVsHierarchyDeleteHandler,
        IVsHierarchyDeleteHandler2,
        IVsHierarchyDropDataTarget,
        IVsHierarchyDropDataSource,
        IVsHierarchyDropDataSource2,
        IVsGetCfgProvider,
        IVsProject3,
        IVsAggregatableProject,
        IVsProjectFlavorCfgProvider,
        IPersistFileFormat,
        IVsBuildPropertyStorage,
        IVsComponentUser,
        IVsDependencyProvider,
        IVsSccProject2,
        IBuildDependencyUpdate,
        IVsProjectSpecialFiles,
        IVsProjectBuildSystem,
        IOleCommandTarget {
        #region nested types

#if DEV14_OR_LATER
        [Obsolete("Use ImageMonikers instead")]
#endif
        public enum ImageName {
            OfflineWebApp = 0,
            WebReferencesFolder = 1,
            OpenReferenceFolder = 2,
            ReferenceFolder = 3,
            Reference = 4,
            SDLWebReference = 5,
            DISCOWebReference = 6,
            Folder = 7,
            OpenFolder = 8,
            ExcludedFolder = 9,
            OpenExcludedFolder = 10,
            ExcludedFile = 11,
            DependentFile = 12,
            MissingFile = 13,
            WindowsForm = 14,
            WindowsUserControl = 15,
            WindowsComponent = 16,
            XMLSchema = 17,
            XMLFile = 18,
            WebForm = 19,
            WebService = 20,
            WebUserControl = 21,
            WebCustomUserControl = 22,
            ASPPage = 23,
            GlobalApplicationClass = 24,
            WebConfig = 25,
            HTMLPage = 26,
            StyleSheet = 27,
            ScriptFile = 28,
            TextFile = 29,
            SettingsFile = 30,
            Resources = 31,
            Bitmap = 32,
            Icon = 33,
            Image = 34,
            ImageMap = 35,
            XWorld = 36,
            Audio = 37,
            Video = 38,
            CAB = 39,
            JAR = 40,
            DataEnvironment = 41,
            PreviewFile = 42,
            DanglingReference = 43,
            XSLTFile = 44,
            Cursor = 45,
            AppDesignerFolder = 46,
            Data = 47,
            Application = 48,
            DataSet = 49,
            PFX = 50,
            SNK = 51,

            ImageLast = 51
        }

        /// <summary>
        /// Flags for specifying which events to stop triggering.
        /// </summary>
        [Flags]
        internal enum EventTriggering {
            TriggerAll = 0,
            DoNotTriggerHierarchyEvents = 1,
            DoNotTriggerTrackerEvents = 2,
            DoNotTriggerTrackerQueryEvents = 4
        }

        #endregion

        #region constants
        /// <summary>
        /// The user file extension.
        /// </summary>
        internal const string PerUserFileExtension = ".user";
        #endregion

        #region fields
        /// <summary>
        /// List of output groups names and their associated target
        /// </summary>
        private static KeyValuePair<string, string>[] outputGroupNames =
        {                                      // Name                    ItemGroup (MSBuild)
            new KeyValuePair<string, string>("Built",                 "BuiltProjectOutputGroup"),
            new KeyValuePair<string, string>("ContentFiles",          "ContentFilesProjectOutputGroup"),
            new KeyValuePair<string, string>("LocalizedResourceDlls", "SatelliteDllsProjectOutputGroup"),
            new KeyValuePair<string, string>("Documentation",         "DocumentationProjectOutputGroup"),
            new KeyValuePair<string, string>("Symbols",               "DebugSymbolsProjectOutputGroup"),
            new KeyValuePair<string, string>("SourceFiles",           "SourceFilesProjectOutputGroup"),
            new KeyValuePair<string, string>("XmlSerializer",         "SGenFilesOutputGroup"),
        };
        private EventSinkCollection _hierarchyEventSinks = new EventSinkCollection();

        /// <summary>A project will only try to build if it can obtain a lock on this object</summary>
        private volatile static object BuildLock = new object();

        /// <summary>Maps integer ids to project item instances</summary>
        private HierarchyIdMap itemIdMap = new HierarchyIdMap();

        /// <summary>A service provider call back object provided by the IDE hosting the project manager</summary>
        private IServiceProvider site;

        private TrackDocumentsHelper tracker;

        /// <summary>
        /// MSBuild engine we are going to use 
        /// </summary>
        private MSBuild.ProjectCollection buildEngine;

        private Microsoft.Build.Utilities.Logger buildLogger;

        private bool useProvidedLogger;

        private MSBuild.Project buildProject;

        private MSBuildExecution.ProjectInstance currentConfig;

        private ConfigProvider configProvider;

        private TaskProvider taskProvider;

        private string filename;

        private Microsoft.VisualStudio.Shell.Url baseUri;

        private string projectHome;

        /// <summary>
        /// Used by OAProject to override the dirty state.
        /// </summary>
        internal bool isDirty;

        private bool projectOpened;

        private string errorString;

        private string warningString;

        private ImageHandler imageHandler;

        private Guid projectIdGuid;

        private bool isClosed, isClosing;

        private EventTriggering eventTriggeringFlag = EventTriggering.TriggerAll;

        private bool canFileNodesHaveChilds;

        private bool isProjectEventsListener = true;

        /// <summary>
        /// The build dependency list passed to IVsDependencyProvider::EnumDependencies 
        /// </summary>
        private List<IVsBuildDependency> buildDependencyList = new List<IVsBuildDependency>();

        /// <summary>
        /// Defines if Project System supports Project Designer
        /// </summary>
        private bool supportsProjectDesigner;

        private bool showProjectInSolutionPage = true;

        private bool buildInProcess;

        private string sccProjectName;

        private string sccLocalPath;

        private string sccAuxPath;

        private string sccProvider;

        /// <summary>
        /// Flag for controling how many times we register with the Scc manager.
        /// </summary>
        private bool isRegisteredWithScc;

        /// <summary>
        /// Flag for controling query edit should communicate with the scc manager.
        /// </summary>
        private bool disableQueryEdit;

        /// <summary>
        /// Control if command with potential destructive behavior such as delete should
        /// be enabled for nodes of this project.
        /// </summary>
        private bool canProjectDeleteItems;

        /// <summary>
        /// Member to store output base relative path. Used by OutputBaseRelativePath property
        /// </summary>
        private string outputBaseRelativePath = "bin";

        /// <summary>
        /// Used for flavoring to hold the XML fragments
        /// </summary>
        private XmlDocument xmlFragments;

        /// <summary>
        /// Used to map types to CATID. This provide a generic way for us to do this
        /// and make it simpler for a project to provide it's CATIDs for the different type of objects
        /// for which it wants to support extensibility. This also enables us to have multiple
        /// type mapping to the same CATID if we choose to.
        /// </summary>
        private Dictionary<Type, Guid> catidMapping;

        /// <summary>
        /// Mapping from item names to their hierarchy nodes for all disk-based nodes.
        /// </summary>
        protected readonly Dictionary<string, HierarchyNode> _diskNodes = new Dictionary<string, HierarchyNode>(StringComparer.OrdinalIgnoreCase);

        // Has the object been disposed.
        private bool isDisposed;

        private IVsHierarchy parentHierarchy;
        private int parentHierarchyItemId;

        private List<HierarchyNode> itemsDraggedOrCutOrCopied;
        /// <summary>
        /// Folder node in the process of being created.  First the hierarchy node
        /// is added, then the label is edited, and when that completes/cancels
        /// the folder gets created.
        /// </summary>
        private FolderNode _folderBeingCreated;

        private readonly ExtensibilityEventsDispatcher extensibilityEventsDispatcher;

        #endregion

        #region abstract properties
        /// <summary>
        /// This Guid must match the Guid you registered under
        /// HKLM\Software\Microsoft\VisualStudio\%version%\Projects.
        /// Among other things, the Project framework uses this 
        /// guid to find your project and item templates.
        /// </summary>
        public abstract Guid ProjectGuid {
            get;
        }

        /// <summary>
        /// Returns a caption for VSHPROPID_TypeName.
        /// </summary>
        /// <returns></returns>
        public abstract string ProjectType {
            get;
        }

        internal abstract string IssueTrackerUrl {
            get;
        }

        #endregion

        #region virtual properties

        /// <summary>
        /// Indicates whether or not the project system supports Show All Files.
        /// 
        /// Subclasses will need to return true here, and will need to handle calls 
        /// </summary>
        public virtual bool CanShowAllFiles {
            get {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether or not the project is currently in the mode where its showing all files.
        /// </summary>
        public virtual bool IsShowingAllFiles {
            get {
                return false;
            }
        }

        /// <summary>
        /// Represents the command guid for the project system.  This enables
        /// using CommonConstants.cmdid* commands.
        /// 
        /// By default these commands are disabled if this isn't overridden
        /// with the packages command guid.
        /// </summary>
        public virtual Guid SharedCommandGuid {
            get {
                return CommonConstants.NoSharedCommandsGuid;
            }
        }

        /// <summary>
        /// This is the project instance guid that is peristed in the project file
        /// </summary>
        [System.ComponentModel.BrowsableAttribute(false)]
        public virtual Guid ProjectIDGuid {
            get {
                return this.projectIdGuid;
            }
            set {
                if (this.projectIdGuid != value) {
                    this.projectIdGuid = value;
                    if (this.buildProject != null) {
                        this.SetProjectProperty("ProjectGuid", this.projectIdGuid.ToString("B"));
                    }
                }
            }
        }

        public override bool CanAddFiles {
            get {
                return true;
            }
        }

        #endregion

        #region properties

        internal bool IsProjectOpened {
            get {
                return projectOpened;
            }
        }

        internal ExtensibilityEventsDispatcher ExtensibilityEventsDispatcher {
            get {
                return extensibilityEventsDispatcher;
            }
        }

        /// <summary>
        /// Gets the folder node which is currently being added to the project via
        /// Solution Explorer.
        /// </summary>
        internal FolderNode FolderBeingCreated {
            get {
                return _folderBeingCreated;
            }
            set {
                _folderBeingCreated = value;
            }
        }

        internal IList<HierarchyNode> ItemsDraggedOrCutOrCopied {
            get {
                return this.itemsDraggedOrCutOrCopied;
            }
        }

        public MSBuildExecution.ProjectInstance CurrentConfig {
            get {
                return currentConfig;
            }
        }

        #region overridden properties

        public override bool CanOpenCommandPrompt {
            get {
                return true;
            }
        }

        internal override string FullPathToChildren {
            get {
                return ProjectHome;
            }
        }

        public override int MenuCommandId {
            get {
                return VsMenus.IDM_VS_CTXT_PROJNODE;
            }
        }

        public override string Url {
            get {
                return this.GetMkDocument();
            }
        }

        public override string Caption {
            get {
                var project = this.buildProject;
                if (project == null) {
                    // Project is not available, which probably means we are
                    // in the process of closing
                    return string.Empty;
                }
                // Use file name
                string caption = project.FullPath;
                if (String.IsNullOrEmpty(caption)) {
                    if (project.GetProperty(ProjectFileConstants.Name) != null) {
                        caption = project.GetProperty(ProjectFileConstants.Name).EvaluatedValue;
                        if (caption == null || caption.Length == 0) {
                            caption = this.ItemNode.GetMetadata(ProjectFileConstants.Include);
                        }
                    }
                } else {
                    caption = Path.GetFileNameWithoutExtension(caption);
                }

                return caption;
            }
        }

        public override Guid ItemTypeGuid {
            get {
                return this.ProjectGuid;
            }
        }

#pragma warning disable 0618, 0672
        // Project subclasses decide whether or not to support using image
        // monikers, and so we need to keep the ImageIndex overrides in case
        // they choose not to.
        public override int ImageIndex {
            get {
                return (int)ProjectNode.ImageName.Application;
            }
        }
#pragma warning restore 0618, 0672


        #endregion

        #region virtual properties

        public virtual string ErrorString {
            get {
                if (this.errorString == null) {
                    this.errorString = SR.GetString(SR.Error);
                }

                return this.errorString;
            }
        }

        public virtual string WarningString {
            get {
                if (this.warningString == null) {
                    this.warningString = SR.GetString(SR.Warning);
                }

                return this.warningString;
            }
        }

        /// <summary>
        /// Override this property to specify when the project file is dirty.
        /// </summary>
        protected virtual bool IsProjectFileDirty {
            get {
                string document = this.GetMkDocument();

                if (String.IsNullOrEmpty(document)) {
                    return this.isDirty;
                }

                return (this.isDirty || !File.Exists(document));
            }
        }

        /// <summary>
        /// True if the project uses the Project Designer Editor instead of the property page frame to edit project properties.
        /// </summary>
        protected bool SupportsProjectDesigner {
            get {
                return this.supportsProjectDesigner;
            }
            set {
                this.supportsProjectDesigner = value;
            }

        }

        protected virtual Guid ProjectDesignerEditor {
            get {
                return VSConstants.GUID_ProjectDesignerEditor;
            }
        }

        /// <summary>
        /// Defines the flag that supports the VSHPROPID.ShowProjInSolutionPage
        /// </summary>
        protected virtual bool ShowProjectInSolutionPage {
            get {
                return this.showProjectInSolutionPage;
            }
            set {
                this.showProjectInSolutionPage = value;
            }
        }

        /// <summary>
        /// A space separated list of the project's capabilities.
        /// </summary>
        /// <remarks>
        /// These may be used by extensions to check whether they support this
        /// project type. In general, this should only contain fundamental
        /// properties of the project, such as the language name.
        /// </remarks>
        protected virtual string ProjectCapabilities {
            get {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the ability of a project filenode to have child nodes (sub items).
        /// Example would be C#/VB forms having resx and designer files.
        /// </summary>
        protected internal bool CanFileNodesHaveChilds {
            get {
                return canFileNodesHaveChilds;
            }
            set {
                canFileNodesHaveChilds = value;
            }
        }

        /// <summary>
        /// Gets a service provider object provided by the IDE hosting the project
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public IServiceProvider Site {
            get {
                return this.site;
            }
        }

        /// <summary>
        /// Gets an ImageHandler for the project node.
        /// </summary>
#if DEV14_OR_LATER
        [Obsolete("Use ImageMonikers instead")]
#endif
        public ImageHandler ImageHandler {
            get {
                if (null == imageHandler) {
                    imageHandler = new ImageHandler(ProjectIconsImageStripStream);
                }
                return imageHandler;
            }
        }

        protected abstract Stream ProjectIconsImageStripStream {
            get;
        }

        /// <summary>
        /// Gets the path to the root folder of the project.
        /// </summary>
        public string ProjectHome {
            get {
                if (projectHome == null) {
                    projectHome = CommonUtils.GetAbsoluteDirectoryPath(
                        this.ProjectFolder,
                        this.GetProjectProperty(CommonConstants.ProjectHome, resetCache: false));
                }

                Debug.Assert(projectHome != null, "ProjectHome should not be null");
                return projectHome;
            }
        }

        /// <summary>
        /// Gets the path to the folder containing the project.
        /// </summary>
        public string ProjectFolder {
            get {
                return Path.GetDirectoryName(this.filename);
            }
        }

        /// <summary>
        /// Gets or sets the project filename.
        /// </summary>
        public string ProjectFile {
            get {
                return Path.GetFileName(this.filename);
            }
            set {
                this.SetEditLabel(value);
            }
        }

        /// <summary>
        /// Gets the Base Uniform Resource Identifier (URI).
        /// </summary>
        public Microsoft.VisualStudio.Shell.Url BaseURI {
            get {
                if (baseUri == null && this.buildProject != null) {
                    string path = CommonUtils.NormalizeDirectoryPath(Path.GetDirectoryName(this.buildProject.FullPath));
                    baseUri = new Url(path);
                }

                Debug.Assert(baseUri != null, "Base URL should not be null. Did you call BaseURI before loading the project?");
                return baseUri;
            }
        }

        protected void BuildProjectLocationChanged() {
            baseUri = null;
            projectHome = null;
        }

        /// <summary>
        /// Gets whether or not the project is closed.
        /// </summary>
        public bool IsClosed {
            get {
                return this.isClosed;
            }
        }

        /// <summary>
        /// Gets whether or not the project has begun closing.
        /// </summary>
        public bool IsClosing {
            get {
                return this.isClosing;
            }
        }

        /// <summary>
        /// Gets whether or not the project is being built.
        /// </summary>
        public bool BuildInProgress {
            get {
                return buildInProcess;
            }
        }


        /// <summary>
        /// Gets or set the relative path to the folder containing the project ouput. 
        /// </summary>
        public virtual string OutputBaseRelativePath {
            get {
                return this.outputBaseRelativePath;
            }
            set {
                if (Path.IsPathRooted(value)) {
                    // TODO: Maybe bring the exception back instead of automatically fixing this?
                    this.outputBaseRelativePath = CommonUtils.GetRelativeDirectoryPath(ProjectHome, value);
                }

                this.outputBaseRelativePath = value;
            }
        }

        /// <summary>
        /// Gets a collection of integer ids that maps to project item instances
        /// </summary>
        internal HierarchyIdMap ItemIdMap {
            get {
                return this.itemIdMap;
            }
        }

        /// <summary>
        /// Get the helper object that track document changes.
        /// </summary>
        internal TrackDocumentsHelper Tracker {
            get {
                return this.tracker;
            }
        }

        /// <summary>
        /// Gets or sets the build logger.
        /// </summary>
        protected Microsoft.Build.Utilities.Logger BuildLogger {
            get {
                return this.buildLogger;
            }
            set {
                this.buildLogger = value;
                this.useProvidedLogger = value != null;
            }
        }

        /// <summary>
        /// Gets the taskprovider.
        /// </summary>
        protected TaskProvider TaskProvider {
            get {
                return this.taskProvider;
            }
        }

        /// <summary>
        /// Gets the project file name.
        /// </summary>
        protected string FileName {
            get {
                return this.filename;
            }
        }


        /// <summary>
        /// Gets the configuration provider.
        /// </summary>
        protected internal ConfigProvider ConfigProvider {
            get {
                if (this.configProvider == null) {
                    this.configProvider = CreateConfigProvider();
                }

                return this.configProvider;
            }
        }

        /// <summary>
        /// Gets or set whether items can be deleted for this project.
        /// Enabling this feature can have the potential destructive behavior such as deleting files from disk.
        /// </summary>
        protected internal bool CanProjectDeleteItems {
            get {
                return canProjectDeleteItems;
            }
            set {
                canProjectDeleteItems = value;
            }
        }

        /// <summary>
        /// Gets or sets event triggering flags.
        /// </summary>
        internal EventTriggering EventTriggeringFlag {
            get {
                return this.eventTriggeringFlag;
            }
            set {
                this.eventTriggeringFlag = value;
            }
        }

        /// <summary>
        /// Defines the build project that has loaded the project file.
        /// </summary>
        protected internal MSBuild.Project BuildProject {
            get {
                return this.buildProject;
            }
            set {
                SetBuildProject(value);
            }
        }

        /// <summary>
        /// Defines the build engine that is used to build the project file.
        /// </summary>
        internal MSBuild.ProjectCollection BuildEngine {
            get {
                return this.buildEngine;
            }
            set {
                this.buildEngine = value;
            }
        }

        #endregion

        #region ctor

        protected ProjectNode(IServiceProvider serviceProvider) {
            this.extensibilityEventsDispatcher = new ExtensibilityEventsDispatcher(this);
            this.Initialize();
            this.site = serviceProvider;
            taskProvider = new TaskProvider(this.site);
        }

        #endregion

        #region overridden methods

        protected internal override void DeleteFromStorage(string path) {
            if (File.Exists(path)) {
                File.Delete(path);
            }
            base.DeleteFromStorage(path);
        }

        /// <summary>
        /// Sets the properties for the project node.
        /// </summary>
        /// <param name="propid">Identifier of the hierarchy property. For a list of propid values, <see cref="__VSHPROPID"/> </param>
        /// <param name="value">The value to set. </param>
        /// <returns>A success or failure value.</returns>
        public override int SetProperty(int propid, object value) {
            __VSHPROPID id = (__VSHPROPID)propid;

            switch (id) {
                case __VSHPROPID.VSHPROPID_ParentHierarchy:
                    parentHierarchy = (IVsHierarchy)value;
                    break;

                case __VSHPROPID.VSHPROPID_ParentHierarchyItemid:
                    parentHierarchyItemId = (int)value;
                    break;

                case __VSHPROPID.VSHPROPID_ShowProjInSolutionPage:
                    this.ShowProjectInSolutionPage = (bool)value;
                    return VSConstants.S_OK;
            }

            return base.SetProperty(propid, value);
        }

        /// <summary>
        /// Renames the project node.
        /// </summary>
        /// <param name="label">The new name</param>
        /// <returns>A success or failure value.</returns>
        public override int SetEditLabel(string label) {
            // Validate the filename. 
            if (Utilities.IsFileNameInvalid(label)) {
                throw new InvalidOperationException(SR.GetString(SR.ErrorInvalidFileName, label));
            } else if (this.ProjectFolder.Length + label.Length + 1 > NativeMethods.MAX_PATH) {
                throw new InvalidOperationException(SR.GetString(SR.PathTooLong, label));
            }


            // TODO: Take file extension into account?
            string fileName = Path.GetFileNameWithoutExtension(label);

            // Nothing to do if the name is the same
            string oldFileName = Path.GetFileNameWithoutExtension(this.Url);
            if (String.Equals(oldFileName, label, StringComparison.Ordinal)) {
                return VSConstants.S_FALSE;
            }

            // Now check whether the original file is still there. It could have been renamed.
            if (!File.Exists(this.Url)) {
                throw new InvalidOperationException(SR.GetString(SR.FileOrFolderCannotBeFound, ProjectFile));
            }

            // Get the full file name and then rename the project file.
            string newFile = Path.Combine(this.ProjectFolder, label);
            string extension = Path.GetExtension(this.Url);

            // Make sure it has the correct extension
            if (!String.Equals(Path.GetExtension(newFile), extension, StringComparison.OrdinalIgnoreCase)) {
                newFile += extension;
            }

            this.RenameProjectFile(newFile);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Gets the automation object for the project node.
        /// </summary>
        /// <returns>An instance of an EnvDTE.Project implementation object representing the automation object for the project.</returns>
        public override object GetAutomationObject() {
            return new Automation.OAProject(this);
        }

        /// <summary>
        /// Gets the properties of the project node. 
        /// </summary>
        /// <param name="propId">The __VSHPROPID of the property.</param>
        /// <returns>A property dependent value. See: <see cref="__VSHPROPID"/> for details.</returns>
        public override object GetProperty(int propId) {
            switch ((__VSHPROPID)propId) {
                case (__VSHPROPID)__VSHPROPID4.VSHPROPID_TargetFrameworkMoniker:
                    // really only here for testing so WAP projects load correctly...
                    // But this also impacts the toolbox by filtering what available items there are.
                    return ".NETFramework,Version=v4.0,Profile=Client";
                case __VSHPROPID.VSHPROPID_ConfigurationProvider:
                    return this.ConfigProvider;

                case __VSHPROPID.VSHPROPID_ProjectName:
                    return this.Caption;

                case __VSHPROPID.VSHPROPID_ProjectDir:
                    return this.ProjectFolder;

                case __VSHPROPID.VSHPROPID_TypeName:
                    return this.ProjectType;

                case __VSHPROPID.VSHPROPID_ShowProjInSolutionPage:
                    return this.ShowProjectInSolutionPage;

                case __VSHPROPID.VSHPROPID_ExpandByDefault:
                    return true;

                case __VSHPROPID.VSHPROPID_DefaultEnableDeployProjectCfg:
                    return true;

                case __VSHPROPID.VSHPROPID_DefaultEnableBuildProjectCfg:
                    return true;

                // Use the same icon as if the folder was closed
                case __VSHPROPID.VSHPROPID_OpenFolderIconIndex:
                    return GetProperty((int)__VSHPROPID.VSHPROPID_IconIndex);

                case __VSHPROPID.VSHPROPID_ParentHierarchyItemid:
                    if (parentHierarchy != null) {
                        return (IntPtr)parentHierarchyItemId; // VS requires VT_I4 | VT_INT_PTR
                    }
                    break;

                case __VSHPROPID.VSHPROPID_ParentHierarchy:
                    return parentHierarchy;
            }

            switch ((__VSHPROPID2)propId) {
                case __VSHPROPID2.VSHPROPID_SupportsProjectDesigner:
                    return this.SupportsProjectDesigner;

                case __VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList:
                    return Utilities.CreateSemicolonDelimitedListOfStringFromGuids(this.GetConfigurationIndependentPropertyPages());

                case __VSHPROPID2.VSHPROPID_CfgPropertyPagesCLSIDList:
                    return Utilities.CreateSemicolonDelimitedListOfStringFromGuids(this.GetConfigurationDependentPropertyPages());

                case __VSHPROPID2.VSHPROPID_PriorityPropertyPagesCLSIDList:
                    return Utilities.CreateSemicolonDelimitedListOfStringFromGuids(this.GetPriorityProjectDesignerPages());

                case __VSHPROPID2.VSHPROPID_Container:
                    return true;
                default:
                    break;
            }

#if DEV11_OR_LATER
            switch ((__VSHPROPID5)propId) {
                case __VSHPROPID5.VSHPROPID_ProjectCapabilities:
                    var caps = ProjectCapabilities;
                    if (!string.IsNullOrEmpty(caps)) {
                        return caps;
                    }
                    break;
            }
#endif

            return base.GetProperty(propId);
        }

        /// <summary>
        /// Gets the GUID value of the node. 
        /// </summary>
        /// <param name="propid">A __VSHPROPID or __VSHPROPID2 value of the guid property</param>
        /// <param name="guid">The guid to return for the property.</param>
        /// <returns>A success or failure value.</returns>
        public override int GetGuidProperty(int propid, out Guid guid) {
            guid = Guid.Empty;
            if ((__VSHPROPID)propid == __VSHPROPID.VSHPROPID_ProjectIDGuid) {
                guid = this.ProjectIDGuid;
            } else if (propid == (int)__VSHPROPID.VSHPROPID_CmdUIGuid) {
                guid = this.ProjectGuid;
            } else if ((__VSHPROPID2)propid == __VSHPROPID2.VSHPROPID_ProjectDesignerEditor && this.SupportsProjectDesigner) {
                guid = this.ProjectDesignerEditor;
            } else {
                base.GetGuidProperty(propid, out guid);
            }

            if (guid.CompareTo(Guid.Empty) == 0) {
                return VSConstants.DISP_E_MEMBERNOTFOUND;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Sets Guid properties for the project node.
        /// </summary>
        /// <param name="propid">A __VSHPROPID or __VSHPROPID2 value of the guid property</param>
        /// <param name="guid">The guid value to set.</param>
        /// <returns>A success or failure value.</returns>
        public override int SetGuidProperty(int propid, ref Guid guid) {
            switch ((__VSHPROPID)propid) {
                case __VSHPROPID.VSHPROPID_ProjectIDGuid:
                    this.ProjectIDGuid = guid;
                    return VSConstants.S_OK;
            }
            return VSConstants.DISP_E_MEMBERNOTFOUND;
        }

        /// <summary>
        /// Removes items from the hierarchy. 
        /// </summary>
        /// <devdoc>Project overwrites this.</devdoc>
        public override void Remove(bool removeFromStorage) {
            // the project will not be deleted from disk, just removed      
            if (removeFromStorage) {
                return;
            }

            // Remove the entire project from the solution
            IVsSolution solution = this.Site.GetService(typeof(SVsSolution)) as IVsSolution;
            uint iOption = 1; // SLNSAVEOPT_PromptSave
            ErrorHandler.ThrowOnFailure(solution.CloseSolutionElement(iOption, this.GetOuterInterface<IVsHierarchy>(), 0));
        }

        /// <summary>
        /// Gets the moniker for the project node. That is the full path of the project file.
        /// </summary>
        /// <returns>The moniker for the project file.</returns>
        public override string GetMkDocument() {
            Debug.Assert(!String.IsNullOrEmpty(this.filename));
            Debug.Assert(this.BaseURI != null && !String.IsNullOrEmpty(this.BaseURI.AbsoluteUrl));
            return CommonUtils.GetAbsoluteFilePath(this.BaseURI.AbsoluteUrl, this.filename);
        }

        /// <summary>
        /// Disposes the project node object.
        /// </summary>
        /// <param name="disposing">Flag determining ehether it was deterministic or non deterministic clean up.</param>
        protected override void Dispose(bool disposing) {
            if (isDisposed) {
                return;
            }

            try {
                try {
                    UnRegisterProject();
                } finally {
                    try {
                        RegisterClipboardNotifications(false);
                    } finally {
                        buildEngine = null;
                    }
                }

                if (buildProject != null) {
                    buildProject.ProjectCollection.UnloadProject(buildProject);
                    buildProject.ProjectCollection.UnloadProject(buildProject.Xml);
                    SetBuildProject(null);
                }

                var logger = BuildLogger as IDisposable;
                BuildLogger = null;
                if (logger != null) {
                    logger.Dispose();
                }

                var tasks = taskProvider;
                taskProvider = null;
                if (tasks != null) {
                    tasks.Dispose();
                }

                isClosing = true;
                isClosed = false;

                if (null != imageHandler) {
                    imageHandler.Close();
                    imageHandler = null;
                }
            } finally {
                base.Dispose(disposing);
                // Note that this isDisposed flag is separate from the base's
                isDisposed = true;
                isClosed = true;
                isClosing = false;
                projectOpened = false;
            }
        }

        /// <summary>
        /// Handles command status on the project node. If a command cannot be handled then the base should be called.
        /// </summary>
        /// <param name="cmdGroup">A unique identifier of the command group. The pguidCmdGroup parameter can be NULL to specify the standard group.</param>
        /// <param name="cmd">The command to query status for.</param>
        /// <param name="pCmdText">Pointer to an OLECMDTEXT structure in which to return the name and/or status information of a single command. Can be NULL to indicate that the caller does not require this information.</param>
        /// <param name="result">An out parameter specifying the QueryStatusResult of the command.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        internal override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result) {
            if (cmdGroup == VsMenus.guidStandardCommandSet97) {
                switch ((VsCommands)cmd) {
                    case VsCommands.Copy:
                    case VsCommands.Paste:
                    case VsCommands.Cut:
                    case VsCommands.Rename:
                    case VsCommands.Exit:
                    case VsCommands.ProjectSettings:
                    case VsCommands.UnloadProject:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;

                    case VsCommands.CancelBuild:
                        result |= QueryStatusResult.SUPPORTED;
                        if (this.buildInProcess)
                            result |= QueryStatusResult.ENABLED;
                        else
                            result |= QueryStatusResult.INVISIBLE;
                        return VSConstants.S_OK;

                    case VsCommands.NewFolder:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;

                    case VsCommands.SetStartupProject:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;
                }
            } else if (cmdGroup == VsMenus.guidStandardCommandSet2K) {

                switch ((VsCommands2K)cmd) {
                    case VsCommands2K.ADDREFERENCE:
                        if (GetReferenceContainer() != null) {
                            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        } else {
                            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.INVISIBLE;
                        }
                        return VSConstants.S_OK;

                    case VsCommands2K.EXCLUDEFROMPROJECT:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.INVISIBLE;
                        return VSConstants.S_OK;

                }
            }

            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }

        /// <summary>
        /// Handles command execution.
        /// </summary>
        /// <param name="cmdGroup">Unique identifier of the command group</param>
        /// <param name="cmd">The command to be executed.</param>
        /// <param name="nCmdexecopt">Values describe how the object should execute the command.</param>
        /// <param name="pvaIn">Pointer to a VARIANTARG structure containing input arguments. Can be NULL</param>
        /// <param name="pvaOut">VARIANTARG structure to receive command output. Can be NULL.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        internal override int ExecCommandOnNode(Guid cmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            if (cmdGroup == VsMenus.guidStandardCommandSet97) {
                switch ((VsCommands)cmd) {

                    case VsCommands.UnloadProject:
                        return this.UnloadProject();
                    case VsCommands.CleanSel:
                    case VsCommands.CleanCtx:
                        return this.CleanProject();
                }
            }

            return base.ExecCommandOnNode(cmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Get the boolean value for the deletion of a project item
        /// </summary>
        /// <param name="deleteOperation">A flag that specifies the type of delete operation (delete from storage or remove from project)</param>
        /// <returns>true if item can be deleted from project</returns>
        internal override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation) {
            if (deleteOperation == __VSDELETEITEMOPERATION.DELITEMOP_RemoveFromProject) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a specific Document manager to handle opening and closing of the Project(Application) Designer if projectdesigner is supported.
        /// </summary>
        /// <returns>Document manager object</returns>
        protected internal override DocumentManager GetDocumentManager() {
            if (this.SupportsProjectDesigner) {
                return new ProjectDesignerDocumentManager(this);
            }
            return null;
        }

        #endregion

        #region virtual methods

        public virtual IEnumerable<string> GetAvailableItemNames() {
            IEnumerable<string> itemTypes = new[] { 
                ProjectFileConstants.None,
                ProjectFileConstants.Compile,
                ProjectFileConstants.Content
            };

            var items = this.buildProject.GetItems("AvailableItemName");
            itemTypes = itemTypes.Union(items.Select(x => x.EvaluatedInclude));

            return itemTypes;
        }

        /// <summary>
        /// Creates a reference node for the given file returning the node, or returns null
        /// if the file doesn't represent a valid file which can be referenced.
        /// </summary>
        public virtual ReferenceNode CreateReferenceNodeForFile(string filename) {
#if FALSE
            return new ComReferenceNode(this.ProjectMgr, selectorData);
#endif
            return null;
        }

        /// <summary>
        /// Executes a wizard.
        /// </summary>
        /// <param name="parentNode">The node to which the wizard should add item(s).</param>
        /// <param name="itemName">The name of the file that the user typed in.</param>
        /// <param name="wizardToRun">The name of the wizard to run.</param>
        /// <param name="dlgOwner">The owner of the dialog box.</param>
        /// <returns>A VSADDRESULT enum value describing success or failure.</returns>
        public virtual VSADDRESULT RunWizard(HierarchyNode parentNode, string itemName, string wizardToRun, IntPtr dlgOwner) {
            Debug.Assert(!String.IsNullOrEmpty(itemName), "The Add item dialog was passing in a null or empty item to be added to the hierrachy.");
            Debug.Assert(!String.IsNullOrEmpty(this.ProjectHome), "ProjectHome is not specified for this project.");

            Utilities.ArgumentNotNull("parentNode", parentNode);
            Utilities.ArgumentNotNullOrEmpty("itemName", itemName);

            // We just validate for length, since we assume other validation has been performed by the dlgOwner.
            if (CommonUtils.GetAbsoluteFilePath(this.ProjectHome, itemName).Length >= NativeMethods.MAX_PATH) {
                string errorMessage = SR.GetString(SR.PathTooLong, itemName);
                if (!Utilities.IsInAutomationFunction(this.Site)) {
                    string title = null;
                    OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                    OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                    Utilities.ShowMessageBox(this.Site, title, errorMessage, icon, buttons, defaultButton);
                    return VSADDRESULT.ADDRESULT_Failure;
                } else {
                    throw new InvalidOperationException(errorMessage);
                }
            }


            // Build up the ContextParams safearray
            //  [0] = Wizard type guid  (bstr)
            //  [1] = Project name  (bstr)
            //  [2] = ProjectItems collection (bstr)
            //  [3] = Local Directory (bstr)
            //  [4] = Filename the user typed (bstr)
            //  [5] = Product install Directory (bstr)
            //  [6] = Run silent (bool)

            object[] contextParams = new object[7];
            contextParams[0] = EnvDTE.Constants.vsWizardAddItem;
            contextParams[1] = this.Caption;
            object automationObject = parentNode.GetAutomationObject();
            if (automationObject is EnvDTE.Project) {
                EnvDTE.Project project = (EnvDTE.Project)automationObject;
                contextParams[2] = project.ProjectItems;
            } else {
                // This would normally be a folder unless it is an item with subitems
                EnvDTE.ProjectItem item = (EnvDTE.ProjectItem)automationObject;
                contextParams[2] = item.ProjectItems;
            }

            contextParams[3] = this.ProjectHome;

            contextParams[4] = itemName;

            object objInstallationDir = null;
            IVsShell shell = (IVsShell)this.GetService(typeof(IVsShell));
            ErrorHandler.ThrowOnFailure(shell.GetProperty((int)__VSSPROPID.VSSPROPID_InstallDirectory, out objInstallationDir));
            string installDir = CommonUtils.NormalizeDirectoryPath((string)objInstallationDir);

            contextParams[5] = installDir;

            contextParams[6] = true;

            IVsExtensibility3 ivsExtensibility = this.GetService(typeof(IVsExtensibility)) as IVsExtensibility3;
            Debug.Assert(ivsExtensibility != null, "Failed to get IVsExtensibility3 service");
            if (ivsExtensibility == null) {
                return VSADDRESULT.ADDRESULT_Failure;
            }

            // Determine if we have the trust to run this wizard.
            IVsDetermineWizardTrust wizardTrust = this.GetService(typeof(SVsDetermineWizardTrust)) as IVsDetermineWizardTrust;
            if (wizardTrust != null) {
                Guid guidProjectAdding = Guid.Empty;
                ErrorHandler.ThrowOnFailure(wizardTrust.OnWizardInitiated(wizardToRun, ref guidProjectAdding));
            }

            int wizResultAsInt;
            try {
                Array contextParamsAsArray = contextParams;

                int result = ivsExtensibility.RunWizardFile(wizardToRun, (int)dlgOwner, ref contextParamsAsArray, out wizResultAsInt);

                if (!ErrorHandler.Succeeded(result) && result != VSConstants.OLE_E_PROMPTSAVECANCELLED) {
                    ErrorHandler.ThrowOnFailure(result);
                }
            } finally {
                if (wizardTrust != null) {
                    ErrorHandler.ThrowOnFailure(wizardTrust.OnWizardCompleted());
                }
            }

            EnvDTE.wizardResult wizardResult = (EnvDTE.wizardResult)wizResultAsInt;

            switch (wizardResult) {
                default:
                    return VSADDRESULT.ADDRESULT_Cancel;
                case wizardResult.wizardResultSuccess:
                    return VSADDRESULT.ADDRESULT_Success;
                case wizardResult.wizardResultFailure:
                    return VSADDRESULT.ADDRESULT_Failure;
            }
        }

        /// <summary>
        /// This overrides the base class method to show the VS 2005 style Add reference dialog. The ProjectNode implementation
        /// shows the VS 2003 style Add Reference dialog.
        /// </summary>
        /// <returns>S_OK if succeeded. Failure other wise</returns>
        public virtual int AddProjectReference() {
            IVsComponentSelectorDlg2 componentDialog;
            Guid guidEmpty = Guid.Empty;
            VSCOMPONENTSELECTORTABINIT[] tabInit = new VSCOMPONENTSELECTORTABINIT[4];
            string strBrowseLocations = Path.GetDirectoryName(ProjectHome);

            //Add the Project page
            tabInit[0].dwSize = (uint)Marshal.SizeOf(typeof(VSCOMPONENTSELECTORTABINIT));
            // Tell the Add Reference dialog to call hierarchies GetProperty with the following
            // propID to enable filtering out ourself from the Project to Project reference
            tabInit[0].varTabInitInfo = (int)__VSHPROPID.VSHPROPID_ShowProjInSolutionPage;
            tabInit[0].guidTab = VSConstants.GUID_SolutionPage;

            // Add the Browse for file page            
            tabInit[1].dwSize = (uint)Marshal.SizeOf(typeof(VSCOMPONENTSELECTORTABINIT));
            tabInit[1].guidTab = VSConstants.GUID_COMPlusPage;
            tabInit[1].varTabInitInfo = 0;

            // Add the Browse for file page            
            tabInit[2].dwSize = (uint)Marshal.SizeOf(typeof(VSCOMPONENTSELECTORTABINIT));
            tabInit[2].guidTab = VSConstants.GUID_BrowseFilePage;
            tabInit[2].varTabInitInfo = 0;

            // Add the WebPI page
            tabInit[3].dwSize = (uint)Marshal.SizeOf(typeof(VSCOMPONENTSELECTORTABINIT));
            tabInit[3].guidTab = typeof(WebPiComponentPickerControl).GUID;
            tabInit[3].varTabInitInfo = 0;

            uint pX = 0, pY = 0;

            componentDialog = GetService(typeof(SVsComponentSelectorDlg)) as IVsComponentSelectorDlg2;
            try {
                // call the container to open the add reference dialog.
                if (componentDialog != null) {
                    // Let the project know not to show itself in the Add Project Reference Dialog page
                    ShowProjectInSolutionPage = false;

                    // call the container to open the add reference dialog.
                    ErrorHandler.ThrowOnFailure(componentDialog.ComponentSelectorDlg2(
                        (System.UInt32)(__VSCOMPSELFLAGS.VSCOMSEL_MultiSelectMode | __VSCOMPSELFLAGS.VSCOMSEL_IgnoreMachineName),
                        (IVsComponentUser)this,
                        0,
                        null,
                        SR.GetString(SR.AddReferenceDialogTitle), // Title
                        "VS.AddReference", // Help topic
                        ref pX,
                        ref pY,
                        (uint)tabInit.Length,
                        tabInit,
                        ref guidEmpty,
                        AddReferenceExtensions.Replace('|', '\0') + "\0",
                        ref strBrowseLocations));
                }
            } catch (COMException e) {
                Trace.WriteLine("Exception : " + e.Message);
                return e.ErrorCode;
            } finally {
                // Let the project know it can show itself in the Add Project Reference Dialog page
                ShowProjectInSolutionPage = true;
            }
            return VSConstants.S_OK;
        }

        protected virtual string AddReferenceExtensions {
            get {
                return SR.GetString(SR.AddReferenceExtensions);
            }
        }

        /// <summary>
        /// Returns the Compiler associated to the project 
        /// </summary>
        /// <returns>Null</returns>
        public virtual ICodeCompiler GetCompiler() {

            return null;
        }

        /// <summary>
        /// Override this method if you have your own project specific
        /// subclass of ProjectOptions
        /// </summary>
        /// <returns>This method returns a new instance of the ProjectOptions base class.</returns>
        public virtual CompilerParameters CreateProjectOptions() {
            return new CompilerParameters();
        }

        /// <summary>
        /// Loads a project file. Called from the factory CreateProject to load the project.
        /// </summary>
        /// <param name="fileName">File name of the project that will be created. </param>
        /// <param name="location">Location where the project will be created.</param>
        /// <param name="name">If applicable, the name of the template to use when cloning a new project.</param>
        /// <param name="flags">Set of flag values taken from the VSCREATEPROJFLAGS enumeration.</param>
        /// <param name="iidProject">Identifier of the interface that the caller wants returned. </param>
        /// <param name="canceled">An out parameter specifying if the project creation was canceled</param>
        public virtual void Load(string fileName, string location, string name, uint flags, ref Guid iidProject, out int canceled) {
            using (new DebugTimer("ProjectLoad")) {
                _diskNodes.Clear();
                bool successful = false;
                try {
                    this.disableQueryEdit = true;

                    // set up internal members and icons
                    canceled = 0;

                    this.ProjectMgr = this;

                    if ((flags & (uint)__VSCREATEPROJFLAGS.CPF_CLONEFILE) == (uint)__VSCREATEPROJFLAGS.CPF_CLONEFILE) {
                        // we need to generate a new guid for the project
                        this.projectIdGuid = Guid.NewGuid();
                    } else {
                        this.SetProjectGuidFromProjectFile();
                    }

                    // Ensure we have a valid build engine.
                    this.buildEngine = this.buildEngine ?? MSBuild.ProjectCollection.GlobalProjectCollection;

                    // based on the passed in flags, this either reloads/loads a project, or tries to create a new one
                    // now we create a new project... we do that by loading the template and then saving under a new name
                    // we also need to copy all the associated files with it.
                    if ((flags & (uint)__VSCREATEPROJFLAGS.CPF_CLONEFILE) == (uint)__VSCREATEPROJFLAGS.CPF_CLONEFILE) {
                        Debug.Assert(File.Exists(fileName), "Invalid filename passed to load the project. A valid filename is expected");

                        // This should be a very fast operation if the build project is already initialized by the Factory.
                        SetBuildProject(Utilities.ReinitializeMsBuildProject(this.buildEngine, fileName, this.buildProject));

                        // Compute the file name
                        // We try to solve two problems here. When input comes from a wizzard in case of zipped based projects 
                        // the parameters are different.
                        // In that case the filename has the new filename in a temporay path.

                        // First get the extension from the template.
                        // Then get the filename from the name.
                        // Then create the new full path of the project.
                        string extension = Path.GetExtension(fileName);

                        string tempName = String.Empty;

                        // We have to be sure that we are not going to lose data here. If the project name is a.b.c then for a project that was based on a zipped template(the wizard calls us) GetFileNameWithoutExtension will suppress "c".
                        // We are going to check if the parameter "name" is extension based and the extension is the same as the one from the "filename" parameter.
                        string tempExtension = Path.GetExtension(name);
                        if (!String.IsNullOrEmpty(tempExtension)) {
                            bool isSameExtension = (String.Equals(tempExtension, extension, StringComparison.OrdinalIgnoreCase));

                            if (isSameExtension) {
                                tempName = Path.GetFileNameWithoutExtension(name);
                            }
                                // If the tempExtension is not the same as the extension that the project name comes from then assume that the project name is a dotted name.
                            else {
                                tempName = Path.GetFileName(name);
                            }
                        } else {
                            tempName = Path.GetFileName(name);
                        }

                        Debug.Assert(!String.IsNullOrEmpty(tempName), "Could not compute project name");
                        string tempProjectFileName = tempName + extension;
                        this.filename = CommonUtils.GetAbsoluteFilePath(location, tempProjectFileName);

                        // Initialize the common project properties.
                        this.InitializeProjectProperties();

                        ErrorHandler.ThrowOnFailure(this.Save(this.filename, 1, 0));

                        string unresolvedProjectHome = this.GetProjectProperty(CommonConstants.ProjectHome);
                        string basePath = CommonUtils.GetAbsoluteDirectoryPath(Path.GetDirectoryName(fileName), unresolvedProjectHome);
                        string baseLocation = CommonUtils.GetAbsoluteDirectoryPath(location, unresolvedProjectHome);

                        if (!CommonUtils.IsSameDirectory(basePath, baseLocation)) {
                            // now we do have the project file saved. we need to create embedded files.
                            foreach (MSBuild.ProjectItem item in this.BuildProject.Items) {
                                // Ignore the item if it is a reference or folder
                                if (this.FilterItemTypeToBeAddedToHierarchy(item.ItemType)) {
                                    continue;
                                }

                                // MSBuilds tasks/targets can create items (such as object files),
                                // such items are not part of the project per say, and should not be displayed.
                                // so ignore those items.
                                if (!IsVisibleItem(item)) {
                                    continue;
                                }

                                string strRelFilePath = item.EvaluatedInclude;
                                string strPathToFile;
                                string newFileName;
                                // taking the base name from the project template + the relative pathname,
                                // and you get the filename
                                strPathToFile = CommonUtils.GetAbsoluteFilePath(basePath, strRelFilePath);
                                // the new path should be the base dir of the new project (location) + the rel path of the file
                                newFileName = CommonUtils.GetAbsoluteFilePath(baseLocation, strRelFilePath);
                                // now the copy file
                                AddFileFromTemplate(strPathToFile, newFileName);
                            }

                            FinishProjectCreation(basePath, baseLocation);
                        }
                    } else {
                        this.filename = fileName;
                    }
                    _diskNodes[this.filename] = this;

                    // now reload to fix up references
                    this.Reload();
                    successful = true;
                } finally {
                    this.disableQueryEdit = false;
                    if (!successful) {
                        this.Close();
                    }
                }
            }
        }

        public override void Close() {
            projectOpened = false;
            isClosing = true;

            if (taskProvider != null) {
                taskProvider.Tasks.Clear();
            }

            try {
                // Walk the tree and close all nodes.
                // This has to be done before the project closes, since we want
                // state still available for the ProjectMgr on the nodes 
                // when nodes are closing.
                CloseAllNodes(this);
            } finally {
                // HierarchyNode.Close() will also call Dispose on us
                base.Close();
            }
        }

        /// <summary>
        /// Performs any new project initialization after the MSBuild project
        /// has been constructed and template files copied to the project directory.
        /// </summary>
        protected virtual void FinishProjectCreation(string sourceFolder, string destFolder) {
        }

        /// <summary>
        /// Called to add a file to the project from a template.
        /// Override to do it yourself if you want to customize the file
        /// </summary>
        /// <param name="source">Full path of template file</param>
        /// <param name="target">Full path of file once added to the project</param>
        public virtual void AddFileFromTemplate(string source, string target) {
            Utilities.ArgumentNotNullOrEmpty("source", source);
            Utilities.ArgumentNotNullOrEmpty("target", target);

            try {
                string directory = Path.GetDirectoryName(target);
                if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }

                File.Copy(source, target, true);

                // best effort to reset the ReadOnly attribute
                File.SetAttributes(target, File.GetAttributes(target) & ~FileAttributes.ReadOnly);
            } catch (IOException e) {
                Trace.WriteLine("Exception : " + e.Message);
            } catch (UnauthorizedAccessException e) {
                Trace.WriteLine("Exception : " + e.Message);
            } catch (ArgumentException e) {
                Trace.WriteLine("Exception : " + e.Message);
            } catch (NotSupportedException e) {
                Trace.WriteLine("Exception : " + e.Message);
            }
        }

        /// <summary>
        /// Called when the project opens an editor window for the given file
        /// </summary>
        public virtual void OnOpenItem(string fullPathToSourceFile) {
        }

        /// <summary>
        /// This add methos adds the "key" item to the hierarchy, potentially adding other subitems in the process
        /// This method may recurse if the parent is an other subitem
        /// 
        /// </summary>
        /// <param name="subitems">List of subitems not yet added to the hierarchy</param>
        /// <param name="key">Key to retrieve the target item from the subitems list</param>
        /// <returns>Newly added node</returns>
        /// <remarks>If the parent node was found we add the dependent item to it otherwise we add the item ignoring the "DependentUpon" metatdata</remarks>
        protected virtual HierarchyNode AddDependentFileNode(IDictionary<String, MSBuild.ProjectItem> subitems, string key) {
            Utilities.ArgumentNotNull("subitems", subitems);

            MSBuild.ProjectItem item = subitems[key];
            subitems.Remove(key);

            HierarchyNode newNode;
            HierarchyNode parent = null;

            string dependentOf = item.GetMetadataValue(ProjectFileConstants.DependentUpon);
            Debug.Assert(String.Compare(dependentOf, key, StringComparison.OrdinalIgnoreCase) != 0, "File dependent upon itself is not valid. Ignoring the DependentUpon metadata");
            if (subitems.ContainsKey(dependentOf)) {
                // The parent item is an other subitem, so recurse into this method to add the parent first
                parent = AddDependentFileNode(subitems, dependentOf);
            } else {
                // See if the parent node already exist in the hierarchy
                uint parentItemID;
                string path = CommonUtils.GetAbsoluteFilePath(this.ProjectHome, dependentOf);
                if (ErrorHandler.Succeeded(this.ParseCanonicalName(path, out parentItemID)) &&
                    parentItemID != 0)
                    parent = this.NodeFromItemId(parentItemID);
                Debug.Assert(parent != null, "File dependent upon a non existing item or circular dependency. Ignoring the DependentUpon metadata");
            }

            // If the parent node was found we add the dependent item to it otherwise we add the item ignoring the "DependentUpon" metatdata
            if (parent != null)
                newNode = this.AddDependentFileNodeToNode(item, parent);
            else
                newNode = this.AddIndependentFileNode(item, GetItemParentNode(item));

            return newNode;
        }

        /// <summary>
        /// Do the build by invoking msbuild
        /// </summary>
        internal virtual void BuildAsync(uint vsopts, string config, IVsOutputWindowPane output, string target, Action<MSBuildResult, string> uiThreadCallback) {
            BuildPrelude(output);
            SetBuildConfigurationProperties(config);
            DoAsyncMSBuildSubmission(target, uiThreadCallback);
        }

        /// <summary>
        /// Return the value of a project property
        /// </summary>
        /// <param name="propertyName">Name of the property to get</param>
        /// <param name="resetCache">True to avoid using the cache</param>
        /// <returns>null if property does not exist, otherwise value of the property</returns>
        public virtual string GetProjectProperty(string propertyName, bool resetCache) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            MSBuildExecution.ProjectPropertyInstance property = GetMsBuildProperty(propertyName, resetCache);
            if (property == null)
                return null;

            return property.EvaluatedValue;
        }

        /// <summary>
        /// Return the value of a project property in it's unevalauted form.
        /// 
        /// New in 1.5.
        /// </summary>
        /// <param name="propertyName">Name of the property to get</param>
        public virtual string GetUnevaluatedProperty(string propertyName) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            var res = this.buildProject.GetProperty(propertyName);

            if (res != null) {
                return res.UnevaluatedValue;
            }
            return null;
        }

        /// <summary>
        /// Set value of project property
        /// </summary>
        /// <param name="propertyName">Name of property</param>
        /// <param name="propertyValue">Value of property</param>
        public virtual void SetProjectProperty(string propertyName, string propertyValue) {
            Utilities.ArgumentNotNull("propertyName", propertyName);
            Site.GetUIThread().MustBeCalledFromUIThread();

            var oldValue = GetUnevaluatedProperty(propertyName) ?? string.Empty;
            propertyValue = propertyValue ?? string.Empty;

            if (oldValue.Equals(propertyValue, StringComparison.Ordinal)) {
                // Property is unchanged or unspecified, so don't set it.
                return;
            }

            // Check out the project file.
            if (!this.QueryEditProjectFile(false)) {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            var newProp = this.buildProject.SetProperty(propertyName, propertyValue);
            RaiseProjectPropertyChanged(propertyName, oldValue, propertyValue);

            // property cache will need to be updated
            this.currentConfig = null;
        }



        public virtual CompilerParameters GetProjectOptions(string config) {
            // This needs to be commented out because if you build for Debug the properties from the Debug 
            // config are cached. When you change configurations the old props are still cached, and 
            // building for release the properties from the Debug config are used. This may not be the best 
            // fix as every time you get properties the objects are reloaded, so for perf it is bad, but 
            // for making it work it is necessary (reload props when a config is changed?).
            ////if(this.options != null)
            ////    return this.options;

            CompilerParameters options = CreateProjectOptions();

            if (config == null)
                return options;

            options.GenerateExecutable = true;

            this.SetConfiguration(config);

            string outputPath = this.GetOutputPath(this.currentConfig);
            if (!String.IsNullOrEmpty(outputPath)) {
                // absolutize relative to project folder location
                outputPath = CommonUtils.GetAbsoluteDirectoryPath(this.ProjectHome, outputPath);
            }

            // Set some default values
            options.OutputAssembly = outputPath + GetAssemblyName(config);

            string outputtype = GetProjectProperty(ProjectFileConstants.OutputType, false);
            if (!string.IsNullOrEmpty(outputtype)) {
                outputtype = outputtype.ToLower(CultureInfo.InvariantCulture);
            }


            options.MainClass = GetProjectProperty("StartupObject", false);

            //    other settings from CSharp we may want to adopt at some point...
            //    AssemblyKeyContainerName = ""  //This is the key file used to sign the interop assembly generated when importing a com object via add reference
            //    AssemblyOriginatorKeyFile = ""
            //    DelaySign = "false"
            //    DefaultClientScript = "JScript"
            //    DefaultHTMLPageLayout = "Grid"
            //    DefaultTargetSchema = "IE50"
            //    PreBuildEvent = ""
            //    PostBuildEvent = ""
            //    RunPostBuildEvent = "OnBuildSuccess"

            if (GetBoolAttr(this.currentConfig, "DebugSymbols")) {
                options.IncludeDebugInformation = true;
            }

            if (GetBoolAttr(this.currentConfig, "RegisterForComInterop")) {
            }

            if (GetBoolAttr(this.currentConfig, "RemoveIntegerChecks")) {
            }

            if (GetBoolAttr(this.currentConfig, "TreatWarningsAsErrors")) {
                options.TreatWarningsAsErrors = true;
            }

            var warningLevel = GetProjectProperty("WarningLevel", resetCache: false);
            if (warningLevel != null) {
                try {
                    options.WarningLevel = Int32.Parse(warningLevel, CultureInfo.InvariantCulture);
                } catch (ArgumentNullException e) {
                    Trace.WriteLine("Exception : " + e.Message);
                } catch (ArgumentException e) {
                    Trace.WriteLine("Exception : " + e.Message);
                } catch (FormatException e) {
                    Trace.WriteLine("Exception : " + e.Message);
                } catch (OverflowException e) {
                    Trace.WriteLine("Exception : " + e.Message);
                }
            }

            return options;
        }

        private string GetOutputPath(MSBuildExecution.ProjectInstance properties) {
            return properties.GetPropertyValue("OutputPath");
        }

        private bool GetBoolAttr(MSBuildExecution.ProjectInstance properties, string name) {
            string s = properties.GetPropertyValue(name);
            return (s != null && s.ToUpperInvariant().Trim() == "TRUE");
        }

        public virtual bool GetBoolAttr(string config, string name) {
            SetConfiguration(config);
            try {
                return GetBoolAttr(currentConfig, name);
            } finally {
                SetCurrentConfiguration();
            }
        }

        /// <summary>
        /// Get the assembly name for a given configuration
        /// </summary>
        /// <param name="config">the matching configuration in the msbuild file</param>
        /// <returns>assembly name</returns>
        public virtual string GetAssemblyName(string config) {
            SetConfiguration(config);
            try {
                var name = currentConfig.GetPropertyValue(ProjectFileConstants.AssemblyName) ?? Caption;
                var outputType = currentConfig.GetPropertyValue(ProjectFileConstants.OutputType);

                if ("library".Equals(outputType, StringComparison.OrdinalIgnoreCase)) {
                    name += ".dll";
                } else {
                    name += ".exe";
                }

                return name;
            } finally {
                SetCurrentConfiguration();
            }
        }

        /// <summary>
        /// Determines whether a file is a code file.
        /// </summary>
        /// <param name="fileName">Name of the file to be evaluated</param>
        /// <returns>false by default for any fileName</returns>
        public virtual bool IsCodeFile(string fileName) {
            return false;
        }

        public virtual string[] CodeFileExtensions {
            get {
                return new string[0];
            }
        }

        /// <summary>
        /// Determines whether the given file is a resource file (resx file).
        /// </summary>
        /// <param name="fileName">Name of the file to be evaluated.</param>
        /// <returns>true if the file is a resx file, otherwise false.</returns>
        public virtual bool IsEmbeddedResource(string fileName) {
            return String.Equals(Path.GetExtension(fileName), ".ResX", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Create a file node based on an msbuild item.
        /// </summary>
        /// <param name="item">msbuild item</param>
        /// <returns>FileNode added</returns>
        public abstract FileNode CreateFileNode(ProjectElement item);

        /// <summary>
        /// Create a file node based on a string.
        /// </summary>
        /// <param name="file">filename of the new filenode</param>
        /// <returns>File node added</returns>
        public abstract FileNode CreateFileNode(string file);

        /// <summary>
        /// Create dependent file node based on an msbuild item
        /// </summary>
        /// <param name="item">msbuild item</param>
        /// <returns>dependent file node</returns>
        public virtual DependentFileNode CreateDependentFileNode(MsBuildProjectElement item) {
            return new DependentFileNode(this, item);
        }

        /// <summary>
        /// Create a dependent file node based on a string.
        /// </summary>
        /// <param name="file">filename of the new dependent file node</param>
        /// <returns>Dependent node added</returns>
        public virtual DependentFileNode CreateDependentFileNode(string file) {
            var item = AddFileToMsBuild(file);
            return this.CreateDependentFileNode(item);
        }

        /// <summary>
        /// Walks the subpaths of a project relative path and checks if the folder nodes hierarchy is already there, if not creates it.
        /// </summary>
        /// <param name="strPath">Path of the folder, can be relative to project or absolute</param>
        public virtual HierarchyNode CreateFolderNodes(string path, bool createOnDisk = true) {
            Utilities.ArgumentNotNull("path", path);

            if (Path.IsPathRooted(path)) {
                // Ensure we are using a path deeper than ProjectHome
                if (!CommonUtils.IsSubpathOf(ProjectHome, path)) {
                    throw new ArgumentException("The path is not within the project", "path");
                }

                path = CommonUtils.GetRelativeDirectoryPath(ProjectHome, path);
            }

            // If the folder already exists, return early
            string strFullPath = CommonUtils.GetAbsoluteDirectoryPath(ProjectHome, path);
            uint uiItemId;
            if (ErrorHandler.Succeeded(ParseCanonicalName(strFullPath, out uiItemId)) &&
                uiItemId != 0) {
                var folder = this.NodeFromItemId(uiItemId) as FolderNode;
                if (folder != null) {
                    // found the folder, return immediately
                    return folder;
                }
            }


            string[] parts = strFullPath.Substring(ProjectHome.Length).Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) {
                // pointing at the project home, it already exists
                return this;
            }
            path = parts[0];
            string fullPath = Path.Combine(ProjectHome, path) + "\\";
            string relPath = path;

            HierarchyNode curParent = VerifySubFolderExists(path, fullPath, this, createOnDisk);

            // now we have an array of subparts....
            for (int i = 1; i < parts.Length; i++) {
                if (parts[i].Length > 0) {
                    fullPath = Path.Combine(fullPath, parts[i]) + "\\";
                    relPath = Path.Combine(relPath, parts[i]);
                    curParent = VerifySubFolderExists(relPath, fullPath, curParent, createOnDisk);
                }
            }
            return curParent;
        }

        /// <summary>
        /// Defines if Node has Designer. By default we do not support designers for nodes
        /// </summary>
        /// <param name="itemPath">Path to item to query for designer support</param>
        /// <returns>true if node has designer</returns>
        public virtual bool NodeHasDesigner(string itemPath) {
            return false;
        }

        /// <summary>
        /// List of Guids of the config independent property pages. It is called by the GetProperty for VSHPROPID_PropertyPagesCLSIDList property.
        /// </summary>
        /// <returns></returns>
        protected virtual Guid[] GetConfigurationIndependentPropertyPages() {
            return new Guid[] { };
        }

        /// <summary>
        /// Returns a list of Guids of the configuration dependent property pages. It is called by the GetProperty for VSHPROPID_CfgPropertyPagesCLSIDList property.
        /// </summary>
        /// <returns></returns>
        protected virtual Guid[] GetConfigurationDependentPropertyPages() {
            return new Guid[] { };
        }

        /// <summary>
        /// An ordered list of guids of the prefered property pages. See <see cref="__VSHPROPID.VSHPROPID_PriorityPropertyPagesCLSIDList"/>
        /// </summary>
        /// <returns>An array of guids.</returns>
        protected virtual Guid[] GetPriorityProjectDesignerPages() {
            return new Guid[] { Guid.Empty };
        }

        /// <summary>
        /// Takes a path and verifies that we have a node with that name.
        /// It is meant to be a helper method for CreateFolderNodes().
        /// For some scenario it may be useful to override.
        /// </summary>
        /// <param name="relativePath">The relative path to the subfolder we want to create, without a trailing \</param>
        /// <param name="fullPath">the full path to the subfolder we want to verify.</param>
        /// <param name="parent">the parent node where to add the subfolder if it does not exist.</param>
        /// <returns>the foldernode correcsponding to the path.</returns>
        protected virtual FolderNode VerifySubFolderExists(string relativePath, string fullPath, HierarchyNode parent, bool createOnDisk = true) {
            Debug.Assert(!CommonUtils.HasEndSeparator(relativePath));

            FolderNode folderNode = null;
            uint uiItemId;
            if (ErrorHandler.Succeeded(this.ParseCanonicalName(fullPath, out uiItemId)) &&
                uiItemId != 0) {
                Debug.Assert(this.NodeFromItemId(uiItemId) is FolderNode, "Not a FolderNode");
                folderNode = (FolderNode)this.NodeFromItemId(uiItemId);
            }

            if (folderNode == null && fullPath != null && parent != null) {
                // folder does not exist yet...
                // We could be in the process of loading so see if msbuild knows about it
                ProjectElement item = null;
                var items = buildProject.GetItemsByEvaluatedInclude(relativePath);
                if (items.Count == 0) {
                    items = buildProject.GetItemsByEvaluatedInclude(relativePath + "\\");
                }
                if (items.Count != 0) {
                    item = new MsBuildProjectElement(this, items.First());
                } else {
                    item = AddFolderToMsBuild(fullPath);
                }
                if (createOnDisk) {
                    Directory.CreateDirectory(fullPath);
                }
                folderNode = CreateFolderNode(item);
                parent.AddChild(folderNode);
            }

            return folderNode;
        }

        /// <summary>
        /// To support virtual folders, override this method to return your own folder nodes
        /// </summary>
        /// <param name="path">Path to store for this folder</param>
        /// <param name="element">Element corresponding to the folder</param>
        /// <returns>A FolderNode that can then be added to the hierarchy</returns>
        protected internal virtual FolderNode CreateFolderNode(ProjectElement element) {
            return new FolderNode(this, element);
        }

        /// <summary>
        /// Gets the list of selected HierarchyNode objects
        /// </summary>
        /// <returns>A list of HierarchyNode objects</returns>
        protected internal virtual IList<HierarchyNode> GetSelectedNodes() {
            // Retrieve shell interface in order to get current selection
            IVsMonitorSelection monitorSelection = this.GetService(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
            Utilities.CheckNotNull(monitorSelection);

            List<HierarchyNode> selectedNodes = new List<HierarchyNode>();
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainer = IntPtr.Zero;
            try {
                // Get the current project hierarchy, project item, and selection container for the current selection
                // If the selection spans multiple hierachies, hierarchyPtr is Zero
                uint itemid;
                IVsMultiItemSelect multiItemSelect = null;
                ErrorHandler.ThrowOnFailure(monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainer));

                // We only care if there are one ore more nodes selected in the tree
                if (itemid != VSConstants.VSITEMID_NIL && hierarchyPtr != IntPtr.Zero) {
                    IVsHierarchy hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;

                    if (itemid != VSConstants.VSITEMID_SELECTION) {
                        // This is a single selection. Compare hirarchy with our hierarchy and get node from itemid
                        if (Utilities.IsSameComObject(this, hierarchy)) {
                            HierarchyNode node = this.NodeFromItemId(itemid);
                            if (node != null) {
                                selectedNodes.Add(node);
                            }
                        }
                    } else if (multiItemSelect != null) {
                        // This is a multiple item selection.

                        //Get number of items selected and also determine if the items are located in more than one hierarchy
                        uint numberOfSelectedItems;
                        int isSingleHierarchyInt;
                        ErrorHandler.ThrowOnFailure(multiItemSelect.GetSelectionInfo(out numberOfSelectedItems, out isSingleHierarchyInt));
                        bool isSingleHierarchy = (isSingleHierarchyInt != 0);

                        // Now loop all selected items and add to the list only those that are selected within this hierarchy
                        if (!isSingleHierarchy || (isSingleHierarchy && Utilities.IsSameComObject(this, hierarchy))) {
                            Debug.Assert(numberOfSelectedItems > 0, "Bad number of selected itemd");
                            VSITEMSELECTION[] vsItemSelections = new VSITEMSELECTION[numberOfSelectedItems];
                            uint flags = (isSingleHierarchy) ? (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs : 0;
                            ErrorHandler.ThrowOnFailure(multiItemSelect.GetSelectedItems(flags, numberOfSelectedItems, vsItemSelections));
                            foreach (VSITEMSELECTION vsItemSelection in vsItemSelections) {
                                if (isSingleHierarchy || Utilities.IsSameComObject(this, vsItemSelection.pHier)) {
                                    HierarchyNode node = this.NodeFromItemId(vsItemSelection.itemid);
                                    if (node != null) {
                                        selectedNodes.Add(node);
                                    }
                                }
                            }
                        }
                    }
                }
            } finally {
                if (hierarchyPtr != IntPtr.Zero) {
                    Marshal.Release(hierarchyPtr);
                }
                if (selectionContainer != IntPtr.Zero) {
                    Marshal.Release(selectionContainer);
                }
            }

            return selectedNodes;
        }

        /// <summary>
        /// Recursevily walks the hierarchy nodes and redraws the state icons
        /// </summary>
        protected internal override void UpdateSccStateIcons() {
            if (this.FirstChild == null) {
                return;
            }

            for (HierarchyNode n = this.FirstChild; n != null; n = n.NextSibling) {
                n.UpdateSccStateIcons();
            }
        }


        /// <summary>
        /// Handles the shows all objects command.
        /// </summary>
        /// <returns></returns>
        protected internal virtual int ShowAllFiles() {
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        /// <summary>
        /// Unloads the project.
        /// </summary>
        /// <returns></returns>
        protected internal virtual int UnloadProject() {
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        /// <summary>
        /// Handles the clean project command.
        /// </summary>
        /// <returns></returns>
        protected virtual int CleanProject() {
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        /// <summary>
        /// Reload project from project file
        /// </summary>
        protected virtual void Reload() {
            Debug.Assert(this.buildEngine != null, "There is no build engine defined for this project");

            try {
                disableQueryEdit = true;

                isClosed = false;
                eventTriggeringFlag = ProjectNode.EventTriggering.DoNotTriggerHierarchyEvents | ProjectNode.EventTriggering.DoNotTriggerTrackerEvents;

                SetBuildProject(Utilities.ReinitializeMsBuildProject(buildEngine, filename, buildProject));

                // Load the guid
                SetProjectGuidFromProjectFile();

                ProcessReferences();

                ProcessFolders();

                ProcessFiles();

                LoadNonBuildInformation();

                InitSccInfo();

                RegisterSccProject();
            } finally {
                isDirty = false;
                eventTriggeringFlag = ProjectNode.EventTriggering.TriggerAll;
                disableQueryEdit = false;
            }
        }

        /// <summary>
        /// Renames the project file
        /// </summary>
        /// <param name="newFile">The full path of the new project file.</param>
        protected virtual void RenameProjectFile(string newFile) {
            IVsUIShell shell = GetService(typeof(SVsUIShell)) as IVsUIShell;
            Utilities.CheckNotNull(shell, "Could not get the UI shell from the project");

            // Figure out what the new full name is
            string oldFile = this.Url;

            int canContinue = 0;
            IVsSolution vsSolution = (IVsSolution)GetService(typeof(SVsSolution));
            if (ErrorHandler.Succeeded(vsSolution.QueryRenameProject(GetOuterInterface<IVsProject>(), oldFile, newFile, 0, out canContinue))
                && canContinue != 0) {
                bool isFileSame = CommonUtils.IsSamePath(oldFile, newFile);

                // If file already exist and is not the same file with different casing
                if (!isFileSame && File.Exists(newFile)) {
                    // Prompt the user for replace
                    string message = SR.GetString(SR.FileAlreadyExists, newFile);

                    if (!Utilities.IsInAutomationFunction(this.Site)) {
                        if (!VsShellUtilities.PromptYesNo(message, null, OLEMSGICON.OLEMSGICON_WARNING, shell)) {
                            throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
                        }
                    } else {
                        throw new InvalidOperationException(message);
                    }

                    // Delete the destination file after making sure it is not read only
                    File.SetAttributes(newFile, FileAttributes.Normal);
                    File.Delete(newFile);
                }

                SuspendFileChanges fileChanges = new SuspendFileChanges(this.Site, this.filename);
                fileChanges.Suspend();
                try {
                    // Actual file rename
                    SaveMSBuildProjectFileAs(newFile);

                    if (!isFileSame) {
                        // Now that the new file name has been created delete the old one.
                        // TODO: Handle source control issues.
                        File.SetAttributes(oldFile, FileAttributes.Normal);
                        File.Delete(oldFile);
                    }

                    OnPropertyChanged(this, (int)__VSHPROPID.VSHPROPID_Caption, 0);

                    // Update solution
                    ErrorHandler.ThrowOnFailure(vsSolution.OnAfterRenameProject((IVsProject)this, oldFile, newFile, 0));

                    ErrorHandler.ThrowOnFailure(shell.RefreshPropertyBrowser(0));
                } finally {
                    fileChanges.Resume();
                }
            } else {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }
        }

        /// <summary>
        /// Filter items that should not be processed as file items. Example: Folders and References.
        /// </summary>
        protected virtual bool FilterItemTypeToBeAddedToHierarchy(string itemType) {
            return (String.Compare(itemType, ProjectFileConstants.Reference, StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(itemType, ProjectFileConstants.ProjectReference, StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(itemType, ProjectFileConstants.COMReference, StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(itemType, ProjectFileConstants.Folder, StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(itemType, ProjectFileConstants.WebReference, StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(itemType, ProjectFileConstants.WebReferenceFolder, StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(itemType, ProjectFileConstants.WebPiReference, StringComparison.OrdinalIgnoreCase) == 0);
        }


        /// <summary>
        /// Associate window output pane to the build logger
        /// </summary>
        /// <param name="output"></param>
        protected virtual void SetOutputLogger(IVsOutputWindowPane output) {
            // Create our logger, if it was not specified
            if (!this.useProvidedLogger || this.buildLogger == null) {
                // Create the logger
                var logger = new IDEBuildLogger(output, this.TaskProvider, GetOuterInterface<IVsHierarchy>());
                logger.ErrorString = ErrorString;
                logger.WarningString = WarningString;
                var oldLogger = this.BuildLogger as IDisposable;
                this.BuildLogger = logger;
                if (oldLogger != null) {
                    oldLogger.Dispose();
                }
            } else {
                ((IDEBuildLogger)this.BuildLogger).OutputWindowPane = output;
            }
        }

        /// <summary>
        /// Set configuration properties for a specific configuration
        /// </summary>
        /// <param name="config">configuration name</param>
        protected virtual void SetBuildConfigurationProperties(string config) {
            CompilerParameters options = null;

            if (!String.IsNullOrEmpty(config)) {
                options = this.GetProjectOptions(config);
            }

            if (options != null && this.buildProject != null) {
                // Make sure the project configuration is set properly
                this.SetConfiguration(config);
            }
        }


        /// <summary>
        /// This execute an MSBuild target for a design-time build.
        /// </summary>
        /// <param name="target">Name of the MSBuild target to execute</param>
        /// <returns>Result from executing the target (success/failure)</returns>
        /// <remarks>
        /// If you depend on the items/properties generated by the target
        /// you should be aware that any call to BuildTarget on any project
        /// will reset the list of generated items/properties
        /// </remarks>
        protected virtual MSBuildResult InvokeMsBuild(string target) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            MSBuildResult result = MSBuildResult.Failed;
            const bool designTime = true;

            IVsBuildManagerAccessor accessor = this.Site.GetService(typeof(SVsBuildManagerAccessor)) as IVsBuildManagerAccessor;
            BuildSubmission submission = null;

            try {
                // Do the actual Build
                if (this.buildProject != null) {
                    if (!TryBeginBuild(designTime, true)) {
                        throw new InvalidOperationException("A build is already in progress.");
                    }

                    string[] targetsToBuild = new string[target != null ? 1 : 0];
                    if (target != null) {
                        targetsToBuild[0] = target;
                    }

                    currentConfig = BuildProject.CreateProjectInstance();

                    BuildRequestData requestData = new BuildRequestData(currentConfig, targetsToBuild, this.BuildProject.ProjectCollection.HostServices, BuildRequestDataFlags.ReplaceExistingProjectInstance);
                    submission = BuildManager.DefaultBuildManager.PendBuildRequest(requestData);
                    if (accessor != null) {
                        ErrorHandler.ThrowOnFailure(accessor.RegisterLogger(submission.SubmissionId, this.buildLogger));
                    }

                    BuildResult buildResult = submission.Execute();

                    result = (buildResult.OverallResult == BuildResultCode.Success) ? MSBuildResult.Successful : MSBuildResult.Failed;
                }
            } finally {
                EndBuild(submission, designTime, true);
            }

            return result;
        }

        /// <summary>
        /// Start MSBuild build submission
        /// </summary>
        /// <param name="target">target to build</param>
        /// <param name="projectInstance">project instance to build; if null, this.BuildProject.CreateProjectInstance() is used to populate</param>
        /// <param name="uiThreadCallback">callback to be run UI thread </param>
        /// <returns>A Build submission instance.</returns>
        protected virtual BuildSubmission DoAsyncMSBuildSubmission(string target, Action<MSBuildResult, string> uiThreadCallback) {
            const bool designTime = false;

            IVsBuildManagerAccessor accessor = (IVsBuildManagerAccessor)this.Site.GetService(typeof(SVsBuildManagerAccessor));
            Utilities.CheckNotNull(accessor);

            if (!TryBeginBuild(designTime, false)) {
                if (uiThreadCallback != null) {
                    uiThreadCallback(MSBuildResult.Failed, target);
                }

                return null;
            }

            string[] targetsToBuild = new string[target != null ? 1 : 0];
            if (target != null) {
                targetsToBuild[0] = target;
            }

            MSBuildExecution.ProjectInstance projectInstance = BuildProject.CreateProjectInstance();

            projectInstance.SetProperty(GlobalProperty.VisualStudioStyleErrors.ToString(), "true");
            projectInstance.SetProperty("UTFOutput", "true");
            projectInstance.SetProperty(GlobalProperty.BuildingInsideVisualStudio.ToString(), "true");

            BuildProject.ProjectCollection.HostServices.SetNodeAffinity(projectInstance.FullPath, NodeAffinity.InProc);
            BuildRequestData requestData = new BuildRequestData(projectInstance, targetsToBuild, this.BuildProject.ProjectCollection.HostServices, BuildRequestDataFlags.ReplaceExistingProjectInstance);
            BuildSubmission submission = BuildManager.DefaultBuildManager.PendBuildRequest(requestData);
            try {
                if (useProvidedLogger && buildLogger != null) {
                    ErrorHandler.ThrowOnFailure(accessor.RegisterLogger(submission.SubmissionId, buildLogger));
                }

                submission.ExecuteAsync(sub => {
                    Site.GetUIThread().Invoke(() => {
                        IDEBuildLogger ideLogger = this.buildLogger as IDEBuildLogger;
                        if (ideLogger != null) {
                            ideLogger.FlushBuildOutput();
                        }
                        EndBuild(sub, designTime, false);
                        uiThreadCallback((sub.BuildResult.OverallResult == BuildResultCode.Success) ? MSBuildResult.Successful : MSBuildResult.Failed, target);
                    });
                }, null);
            } catch (Exception e) {
                Debug.Fail(e.ToString());
                EndBuild(submission, designTime, false);
                if (uiThreadCallback != null) {
                    uiThreadCallback(MSBuildResult.Failed, target);
                }

                throw;
            }

            return submission;
        }

        /// <summary>
        /// Initialize common project properties with default value if they are empty
        /// </summary>
        /// <remarks>The following common project properties are defaulted to projectName (if empty):
        ///    AssemblyName, Name and RootNamespace.
        /// If the project filename is not set then no properties are set</remarks>
        protected virtual void InitializeProjectProperties() {
            // Get projectName from project filename. Return if not set
            string projectName = Path.GetFileNameWithoutExtension(this.filename);
            if (String.IsNullOrEmpty(projectName)) {
                return;
            }

            if (String.IsNullOrEmpty(GetProjectProperty(ProjectFileConstants.AssemblyName))) {
                SetProjectProperty(ProjectFileConstants.AssemblyName, projectName);
            }
            if (String.IsNullOrEmpty(GetProjectProperty(ProjectFileConstants.Name))) {
                SetProjectProperty(ProjectFileConstants.Name, projectName);
            }
            if (String.IsNullOrEmpty(GetProjectProperty(ProjectFileConstants.RootNamespace))) {
                SetProjectProperty(ProjectFileConstants.RootNamespace, projectName);
            }
        }

        /// <summary>
        /// Factory method for configuration provider
        /// </summary>
        /// <returns>Configuration provider created</returns>
        protected abstract ConfigProvider CreateConfigProvider();

        /// <summary>
        /// Factory method for reference container node
        /// </summary>
        /// <returns>ReferenceContainerNode created</returns>
        protected virtual ReferenceContainerNode CreateReferenceContainerNode() {
            return new ReferenceContainerNode(this);
        }

        /// <summary>
        /// Saves the project file on a new name.
        /// </summary>
        /// <param name="newFileName">The new name of the project file.</param>
        /// <returns>Success value or an error code.</returns>
        protected virtual int SaveAs(string newFileName) {
            Debug.Assert(!String.IsNullOrEmpty(newFileName), "Cannot save project file for an empty or null file name");
            Utilities.ArgumentNotNullOrEmpty(newFileName, "newFileName");

            newFileName = newFileName.Trim();

            string errorMessage = String.Empty;

            if (newFileName.Length > NativeMethods.MAX_PATH) {
                errorMessage = SR.GetString(SR.PathTooLong, newFileName);
            } else {
                string fileName = String.Empty;

                try {
                    fileName = Path.GetFileNameWithoutExtension(newFileName);
                }
                    // We want to be consistent in the error message and exception we throw. fileName could be for example #¤&%"¤&"%  and that would trigger an ArgumentException on Path.IsRooted.
                catch (ArgumentException) {
                    errorMessage = SR.GetString(SR.ErrorInvalidFileName, newFileName);
                }

                if (errorMessage.Length == 0) {
                    // If there is no filename or it starts with a leading dot issue an error message and quit.
                    // For some reason the save as dialog box allows to save files like "......ext"
                    if (String.IsNullOrEmpty(fileName) || fileName[0] == '.') {
                        errorMessage = SR.GetString(SR.FileNameCannotContainALeadingPeriod);
                    } else if (Utilities.ContainsInvalidFileNameChars(newFileName)) {
                        errorMessage = SR.GetString(SR.ErrorInvalidFileName, newFileName);
                    }
                }
            }
            if (errorMessage.Length > 0) {
                // If it is not called from an automation method show a dialog box.
                if (!Utilities.IsInAutomationFunction(this.Site)) {
                    string title = null;
                    OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                    OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                    Utilities.ShowMessageBox(this.Site, title, errorMessage, icon, buttons, defaultButton);
                    return VSConstants.OLE_E_PROMPTSAVECANCELLED;
                }

                throw new InvalidOperationException(errorMessage);
            }

            string oldName = this.filename;

            IVsSolution solution = this.Site.GetService(typeof(IVsSolution)) as IVsSolution;
            Utilities.CheckNotNull(solution, "Could not retrieve the solution form the service provider");

            int canRenameContinue = 0;
            ErrorHandler.ThrowOnFailure(solution.QueryRenameProject(this.GetOuterInterface<IVsProject>(), this.filename, newFileName, 0, out canRenameContinue));

            if (canRenameContinue == 0) {
                return VSConstants.OLE_E_PROMPTSAVECANCELLED;
            }

            SuspendFileChanges fileChanges = new SuspendFileChanges(this.Site, oldName);
            fileChanges.Suspend();
            try {
                // Save the project file and project file related properties.
                this.SaveMSBuildProjectFileAs(newFileName);

                // TODO: If source control is enabled check out the project file.

                //Redraw.
                this.OnPropertyChanged(this, (int)__VSHPROPID.VSHPROPID_Caption, 0);

                ErrorHandler.ThrowOnFailure(solution.OnAfterRenameProject(this, oldName, this.filename, 0));

                IVsUIShell shell = this.Site.GetService(typeof(SVsUIShell)) as IVsUIShell;
                Utilities.CheckNotNull(shell, "Could not get the UI shell from the project");

                ErrorHandler.ThrowOnFailure(shell.RefreshPropertyBrowser(0));
            } finally {
                fileChanges.Resume();
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Saves project file related information to the new file name. It also calls msbuild API to save the project file.
        /// It is called by the SaveAs method and the SetEditLabel before the project file rename related events are triggered. 
        /// An implementer can override this method to provide specialized semantics on how the project file is renamed in the msbuild file.
        /// </summary>
        /// <param name="newFileName">The new full path of the project file</param>
        protected virtual void SaveMSBuildProjectFileAs(string newFileName) {
            Debug.Assert(!String.IsNullOrEmpty(newFileName), "Cannot save project file for an empty or null file name");

            string newProjectHome = CommonUtils.GetRelativeDirectoryPath(Path.GetDirectoryName(newFileName), ProjectHome);
            buildProject.SetProperty(CommonConstants.ProjectHome, newProjectHome);

            buildProject.FullPath = newFileName;


            _diskNodes.Remove(this.filename);
            this.filename = newFileName;
            _diskNodes[this.filename] = this;

            string newFileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);

            // Refresh solution explorer
            SetProjectProperty(ProjectFileConstants.Name, newFileNameWithoutExtension);

            // Saves the project file on disk.
            SaveMSBuildProjectFile(newFileName);
        }

        /// <summary>
        /// Adds a file to the msbuild project.
        /// </summary>
        /// <param name="file">The file to be added.</param>
        /// <returns>A Projectelement describing the newly added file.</returns>
        internal virtual MsBuildProjectElement AddFileToMsBuild(string file) {
            MsBuildProjectElement newItem;

            string itemPath = CommonUtils.GetRelativeFilePath(ProjectHome, file);
            Debug.Assert(!Path.IsPathRooted(itemPath), "Cannot add item with full path.");

            if (this.IsCodeFile(itemPath)) {
                newItem = this.CreateMsBuildFileItem(itemPath, ProjectFileConstants.Compile);
                newItem.SetMetadata(ProjectFileConstants.SubType, ProjectFileAttributeValue.Code);
            } else if (this.IsEmbeddedResource(itemPath)) {
                newItem = this.CreateMsBuildFileItem(itemPath, ProjectFileConstants.EmbeddedResource);
            } else {
                newItem = this.CreateMsBuildFileItem(itemPath, ProjectFileConstants.Content);
                newItem.SetMetadata(ProjectFileConstants.SubType, ProjectFileConstants.Content);
            }

            return newItem;
        }

        /// <summary>
        /// Adds a folder to the msbuild project.
        /// </summary>
        /// <param name="folder">The folder to be added.</param>
        /// <returns>A ProjectElement describing the newly added folder.</returns>
        protected virtual ProjectElement AddFolderToMsBuild(string folder) {
            ProjectElement newItem;

            if (Path.IsPathRooted(folder)) {
                folder = CommonUtils.GetRelativeDirectoryPath(ProjectHome, folder);
                Debug.Assert(!Path.IsPathRooted(folder), "Cannot add item with full path.");
            }

            newItem = this.CreateMsBuildFileItem(folder, ProjectFileConstants.Folder);

            return newItem;
        }

        const int E_CANCEL_FILE_ADD = unchecked((int)0xA0010001);      // Severity = Error, Customer Bit set, Facility = 1, Error = 1

        /// <summary>
        /// Checks to see if the user wants to overwrite the specified file name.  
        /// 
        /// Returns:
        ///     E_ABORT if we disallow the user to overwrite the file
        ///     OLECMDERR_E_CANCELED if the user wants to cancel
        ///     S_OK if the user wants to overwrite
        ///     E_CANCEL_FILE_ADD (0xA0010001) if the user doesn't want to overwrite and wants to abort the larger transaction
        /// </summary>
        /// <param name="originalFileName"></param>
        /// <param name="computedNewFileName"></param>
        /// <param name="canCancel"></param>
        /// <returns></returns>
        protected int CanOverwriteExistingItem(string originalFileName, string computedNewFileName, bool inProject = true) {
            if (String.IsNullOrEmpty(originalFileName) || String.IsNullOrEmpty(computedNewFileName)) {
                return VSConstants.E_INVALIDARG;
            }

            string message = String.Empty;
            string title = String.Empty;
            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;

            // File already exists in project... message box
            message = SR.GetString(inProject ? SR.FileAlreadyInProject : SR.FileAlreadyExists, Path.GetFileName(originalFileName));
            icon = OLEMSGICON.OLEMSGICON_QUERY;
            buttons = OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
            int msgboxResult = Utilities.ShowMessageBox(this.Site, title, message, icon, buttons, defaultButton);
            if (msgboxResult == NativeMethods.IDCANCEL) {
                return (int)E_CANCEL_FILE_ADD;
            } else if (msgboxResult != NativeMethods.IDYES) {
                return (int)OleConstants.OLECMDERR_E_CANCELED;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Adds a new file node to the hierarchy.
        /// </summary>
        /// <param name="parentNode">The parent of the new fileNode</param>
        /// <param name="fileName">The file name</param>
        protected virtual void AddNewFileNodeToHierarchy(HierarchyNode parentNode, string fileName) {
            Utilities.ArgumentNotNull("parentNode", parentNode);

            HierarchyNode child;

            // In the case of subitem, we want to create dependent file node
            // and set the DependentUpon property
            if (this.canFileNodesHaveChilds && (parentNode is FileNode || parentNode is DependentFileNode)) {
                child = this.CreateDependentFileNode(fileName);
                child.ItemNode.SetMetadata(ProjectFileConstants.DependentUpon, parentNode.ItemNode.GetMetadata(ProjectFileConstants.Include));

                // Make sure to set the HasNameRelation flag on the dependent node if it is related to the parent by name
                if (!child.HasParentNodeNameRelation && string.Compare(child.GetRelationalName(), parentNode.GetRelationalName(), StringComparison.OrdinalIgnoreCase) == 0) {
                    child.HasParentNodeNameRelation = true;
                }
            } else {
                //Create and add new filenode to the project
                child = this.CreateFileNode(fileName);
            }

            parentNode.AddChild(child);

            // TODO : Revisit the VSADDFILEFLAGS here. Can it be a nested project?
            this.tracker.OnItemAdded(fileName, VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags);
        }

        /// <summary>
        /// Defines whther the current mode of the project is in a supress command mode.
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool IsCurrentStateASuppressCommandsMode() {
            if (VsShellUtilities.IsSolutionBuilding(this.Site)) {
                return true;
            }

            DBGMODE dbgMode = VsShellUtilities.GetDebugMode(this.Site) & ~DBGMODE.DBGMODE_EncMask;
            if (dbgMode == DBGMODE.DBGMODE_Run || dbgMode == DBGMODE.DBGMODE_Break) {
                return true;
            }

            return false;

        }


        /// <summary>
        /// This is the list of output groups that the configuration object should
        /// provide.
        /// The first string is the name of the group.
        /// The second string is the target name (MSBuild) for that group.
        /// 
        /// To add/remove OutputGroups, simply override this method and edit the list.
        /// 
        /// To get nice display names and description for your groups, override:
        ///        - GetOutputGroupDisplayName
        ///        - GetOutputGroupDescription
        /// </summary>
        /// <returns>List of output group name and corresponding MSBuild target</returns>
        protected internal virtual IList<KeyValuePair<string, string>> GetOutputGroupNames() {
            return new List<KeyValuePair<string, string>>(outputGroupNames);
        }

        /// <summary>
        /// Get the display name of the given output group.
        /// </summary>
        /// <param name="canonicalName">Canonical name of the output group</param>
        /// <returns>Display name</returns>
        protected internal virtual string GetOutputGroupDisplayName(string canonicalName) {
            string result = SR.GetString("Output" + canonicalName);
            if (String.IsNullOrEmpty(result))
                result = canonicalName;
            return result;
        }

        /// <summary>
        /// Get the description of the given output group.
        /// </summary>
        /// <param name="canonicalName">Canonical name of the output group</param>
        /// <returns>Description</returns>
        protected internal virtual string GetOutputGroupDescription(string canonicalName) {
            string result = SR.GetString("Output" + canonicalName + "Description");
            if (String.IsNullOrEmpty(result))
                result = canonicalName;
            return result;
        }

        /// <summary>
        /// Set the configuration in MSBuild.
        /// This does not get persisted and is used to evaluate msbuild conditions
        /// which are based on the $(Configuration) property.
        /// </summary>
        protected internal virtual void SetCurrentConfiguration() {
            // Can't ask for the active config until the project is opened, so do nothing in that scenario
            if (!IsProjectOpened)
                return;

            EnvDTE.Project automationObject = GetAutomationObject() as EnvDTE.Project;

            SetConfiguration(Utilities.GetActiveConfigurationName(automationObject));
        }

        /// <summary>
        /// Set the configuration property in MSBuild.
        /// This does not get persisted and is used to evaluate msbuild conditions
        /// which are based on the $(Configuration) property.
        /// </summary>
        /// <param name="config">Configuration name</param>
        protected internal virtual void SetConfiguration(string config) {
            Utilities.ArgumentNotNull("config", config);

            // Can't ask for the active config until the project is opened, so do nothing in that scenario
            if (!IsProjectOpened)
                return;


            bool propertiesChanged = this.BuildProject.SetGlobalProperty(ProjectFileConstants.Configuration, config);
            if (this.currentConfig == null || propertiesChanged) {
                this.currentConfig = this.BuildProject.CreateProjectInstance();
            }
        }

        /// <summary>
        /// Loads reference items from the project file into the hierarchy.
        /// </summary>
        protected internal virtual void ProcessReferences() {
            IReferenceContainer container = GetReferenceContainer();
            if (null == container) {
                // Process References
                ReferenceContainerNode referencesFolder = CreateReferenceContainerNode();
                if (null == referencesFolder) {
                    // This project type does not support references or there is a problem
                    // creating the reference container node.
                    // In both cases there is no point to try to process references, so exit.
                    return;
                }
                this.AddChild(referencesFolder);
                container = referencesFolder;
            }

            // Load the referernces.
            container.LoadReferencesFromBuildProject(buildProject);
        }

        /// <summary>
        /// Loads folders from the project file into the hierarchy.
        /// </summary>
        protected internal virtual void ProcessFolders() {
            // Process Folders (useful to persist empty folder)
            foreach (MSBuild.ProjectItem folder in this.buildProject.GetItems(ProjectFileConstants.Folder).ToArray()) {
                string strPath = folder.EvaluatedInclude;

                // We do not need any special logic for assuring that a folder is only added once to the ui hierarchy.
                // The below method will only add once the folder to the ui hierarchy
                this.CreateFolderNodes(strPath, false);
            }
        }

        /// <summary>
        /// Loads file items from the project file into the hierarchy.
        /// </summary>
        protected internal virtual void ProcessFiles() {
            List<String> subitemsKeys = new List<String>();
            Dictionary<String, MSBuild.ProjectItem> subitems = new Dictionary<String, MSBuild.ProjectItem>();

            // Define a set for our build items. The value does not really matter here.
            Dictionary<String, MSBuild.ProjectItem> items = new Dictionary<String, MSBuild.ProjectItem>();

            // Process Files
            foreach (MSBuild.ProjectItem item in this.buildProject.Items.ToArray()) // copy the array, we could add folders while enumerating
            {
                // Ignore the item if it is a reference or folder
                if (this.FilterItemTypeToBeAddedToHierarchy(item.ItemType))
                    continue;

                // Check if the item is imported.  If it is we'll only show it in the
                // project if it is a Visible item meta data.  Visible can also be used
                // to hide non-imported items.
                if (!IsVisibleItem(item)) {
                    continue;
                }

                // If the item is already contained do nothing.
                // TODO: possibly report in the error list that the the item is already contained in the project file similar to Language projects.
                if (items.ContainsKey(item.EvaluatedInclude.ToUpperInvariant()))
                    continue;

                // Make sure that we do not want to add the item, dependent, or independent twice to the ui hierarchy
                items.Add(item.EvaluatedInclude.ToUpperInvariant(), item);


                string dependentOf = item.GetMetadataValue(ProjectFileConstants.DependentUpon);
                string link = item.GetMetadataValue(ProjectFileConstants.Link);
                if (!String.IsNullOrWhiteSpace(link)) {
                    if (Path.IsPathRooted(link)) {
                        // ignore fully rooted link paths.
                        continue;
                    }

                    if (!Path.IsPathRooted(item.EvaluatedInclude)) {
                        var itemPath = CommonUtils.GetAbsoluteFilePath(ProjectHome, item.EvaluatedInclude);
                        if (CommonUtils.IsSubpathOf(ProjectHome, itemPath)) {
                            // linked file which lives in our directory, don't allow that.
                            continue;
                        }
                    }

                    var linkPath = CommonUtils.GetAbsoluteFilePath(ProjectHome, link);
                    if (!CommonUtils.IsSubpathOf(ProjectHome, linkPath)) {
                        // relative path outside of project, don't allow that.
                        continue;
                    }
                }

                if (!this.CanFileNodesHaveChilds || String.IsNullOrEmpty(dependentOf)) {
                    var parent = GetItemParentNode(item);

                    var itemPath = CommonUtils.GetAbsoluteFilePath(ProjectHome, item.EvaluatedInclude);
                    var existingChild = FindNodeByFullPath(itemPath);
                    if (existingChild != null) {
                        if (existingChild.IsLinkFile) {
                            // remove link node.
                            existingChild.Parent.RemoveChild(existingChild);
                        } else {
                            // we have duplicate entries, or this is a link file.
                            continue;
                        }
                    }

                    AddIndependentFileNode(item, parent);
                } else {
                    // We will process dependent items later.
                    // Note that we use 2 lists as we want to remove elements from
                    // the collection as we loop through it
                    subitemsKeys.Add(item.EvaluatedInclude);
                    subitems.Add(item.EvaluatedInclude, item);
                }
            }

            // Now process the dependent items.
            if (this.CanFileNodesHaveChilds) {
                ProcessDependentFileNodes(subitemsKeys, subitems);
            }

        }

        private static bool IsVisibleItem(MSBuild.ProjectItem item) {
            bool isVisibleItem = true;
            string visible = item.GetMetadataValue(CommonConstants.Visible);
            if ((item.IsImported && !String.Equals(visible, "true", StringComparison.OrdinalIgnoreCase)) ||
                String.Equals(visible, "false", StringComparison.OrdinalIgnoreCase)) {
                isVisibleItem = false;
            }
            return isVisibleItem;
        }

        /// <summary>
        /// Processes dependent filenodes from list of subitems. Multi level supported, but not circular dependencies.
        /// </summary>
        /// <param name="subitemsKeys">List of sub item keys </param>
        /// <param name="subitems"></param>
        protected internal virtual void ProcessDependentFileNodes(IList<String> subitemsKeys, Dictionary<String, MSBuild.ProjectItem> subitems) {
            if (subitemsKeys == null || subitems == null) {
                return;
            }

            foreach (string key in subitemsKeys) {
                // A previous pass could have removed the key so make sure it still needs to be added
                if (!subitems.ContainsKey(key))
                    continue;

                AddDependentFileNode(subitems, key);
            }
        }

        /// <summary>
        /// For flavored projects which implement IPersistXMLFragment, load the information now
        /// </summary>
        protected internal virtual void LoadNonBuildInformation() {
            IPersistXMLFragment outerHierarchy = GetOuterInterface<IPersistXMLFragment>();
            if (outerHierarchy != null) {
                this.LoadXmlFragment(outerHierarchy, null, null);
            }
        }

        /// <summary>
        /// Used to sort nodes in the hierarchy.
        /// </summary>
        internal int CompareNodes(HierarchyNode node1, HierarchyNode node2) {
            Debug.Assert(node1 != null);
            Debug.Assert(node2 != null);

            if (node1.SortPriority == node2.SortPriority) {
                return String.Compare(node2.Caption, node1.Caption, true, CultureInfo.CurrentCulture);
            } else {
                return node2.SortPriority - node1.SortPriority;
            }
        }

        protected abstract void InitializeCATIDs();

        #endregion

        #region non-virtual methods

        internal void InstantiateItemsDraggedOrCutOrCopiedList() {
            itemsDraggedOrCutOrCopied = new List<HierarchyNode>();
        }

        /// <summary>
        /// Overloaded method. Invokes MSBuild using the default configuration and does without logging on the output window pane.
        /// </summary>
        public MSBuildResult Build(string target) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            return this.Build(String.Empty, target);
        }

        /// <summary>
        /// This is called from the main thread before the background build starts.
        ///  cleanBuild is not part of the vsopts, but passed down as the callpath is differently
        ///  PrepareBuild mainly creates directories and cleans house if cleanBuild is true
        /// </summary>
        public virtual void PrepareBuild(string config, bool cleanBuild) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            try {
                SetConfiguration(config);

                string outputPath = Path.GetDirectoryName(GetProjectProperty("OutputPath"));

                PackageUtilities.EnsureOutputPath(outputPath);
            } finally {
                SetCurrentConfiguration();
            }
        }

        /// <summary>
        /// Do the build by invoking msbuild
        /// </summary>
        public virtual MSBuildResult Build(string config, string target) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            lock (ProjectNode.BuildLock) {
                IVsOutputWindowPane output = null;
                var outputWindow = (IVsOutputWindow)GetService(typeof(SVsOutputWindow));
                if (outputWindow != null &&
                    ErrorHandler.Failed(outputWindow.GetPane(VSConstants.GUID_BuildOutputWindowPane, out output))) {
                    outputWindow.CreatePane(VSConstants.GUID_BuildOutputWindowPane, "Build", 1, 1);
                    outputWindow.GetPane(VSConstants.GUID_BuildOutputWindowPane, out output);
                }

                bool engineLogOnlyCritical = this.BuildPrelude(output);

                MSBuildResult result = MSBuildResult.Failed;

                try {
                    SetBuildConfigurationProperties(config);

                    result = InvokeMsBuild(target);
                } finally {
                    // Unless someone specifically request to use an output window pane, we should not output to it
                    if (null != output) {
                        SetOutputLogger(null);
                        BuildEngine.OnlyLogCriticalEvents = engineLogOnlyCritical;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Get value of Project property
        /// </summary>
        /// <param name="propertyName">Name of Property to retrieve</param>
        /// <returns>Value of property</returns>
        public string GetProjectProperty(string propertyName) {
            return this.GetProjectProperty(propertyName, true);
        }

        /// <summary>
        /// Get Node from ItemID.
        /// </summary>
        /// <param name="itemId">ItemID for the requested node</param>
        /// <returns>Node if found</returns>
        public HierarchyNode NodeFromItemId(uint itemId) {
            if (VSConstants.VSITEMID_ROOT == itemId) {
                return this;
            } else if (VSConstants.VSITEMID_NIL == itemId) {
                return null;
            } else if (VSConstants.VSITEMID_SELECTION == itemId) {
                throw new NotImplementedException();
            }

            return (HierarchyNode)this.ItemIdMap[itemId];
        }

        /// <summary>
        /// This method return new project element, and add new MSBuild item to the project/build hierarchy
        /// </summary>
        /// <param name="file">file name</param>
        /// <param name="itemType">MSBuild item type</param>
        /// <returns>new project element</returns>
        public MsBuildProjectElement CreateMsBuildFileItem(string file, string itemType) {
            return new MsBuildProjectElement(this, file, itemType);
        }

        /// <summary>
        /// This method returns new project element based on existing MSBuild item. It does not modify/add project/build hierarchy at all.
        /// </summary>
        /// <param name="item">MSBuild item instance</param>
        /// <returns>wrapping project element</returns>
        public MsBuildProjectElement GetProjectElement(MSBuild.ProjectItem item) {
            return new MsBuildProjectElement(this, item);
        }

        /// <summary>
        /// Create FolderNode from Path
        /// </summary>
        /// <param name="path">Path to folder</param>
        /// <returns>FolderNode created that can be added to the hierarchy</returns>
        protected internal FolderNode CreateFolderNode(string path) {
            ProjectElement item = this.AddFolderToMsBuild(path);
            FolderNode folderNode = CreateFolderNode(item);
            return folderNode;
        }

        internal bool QueryEditFiles(bool suppressUI, params string[] files) {
            bool result = true;
            if (this.disableQueryEdit) {
                return true;
            } else {
                IVsQueryEditQuerySave2 queryEditQuerySave = this.GetService(typeof(SVsQueryEditQuerySave)) as IVsQueryEditQuerySave2;
                if (queryEditQuerySave != null) {
                    tagVSQueryEditFlags qef = tagVSQueryEditFlags.QEF_AllowInMemoryEdits;
                    if (suppressUI)
                        qef |= tagVSQueryEditFlags.QEF_SilentMode;

                    // If we are debugging, we want to prevent our project from being reloaded. To 
                    // do this, we pass the QEF_NoReload flag
                    if (!Utilities.IsVisualStudioInDesignMode(this.Site))
                        qef |= tagVSQueryEditFlags.QEF_NoReload;

                    uint verdict;
                    uint moreInfo;
                    uint[] flags = new uint[files.Length];
                    VSQEQS_FILE_ATTRIBUTE_DATA[] attributes = new VSQEQS_FILE_ATTRIBUTE_DATA[files.Length];
                    int hr = queryEditQuerySave.QueryEditFiles(
                        (uint)qef,
                        files.Length, // 1 file
                        files, // array of files
                        flags, // no per file flags
                        attributes, // no per file file attributes
                        out verdict,
                        out moreInfo // ignore additional results
                    );

                    tagVSQueryEditResult qer = (tagVSQueryEditResult)verdict;
                    if (ErrorHandler.Failed(hr) || (qer != tagVSQueryEditResult.QER_EditOK)) {
                        if (!suppressUI && !Utilities.IsInAutomationFunction(this.Site)) {
                            string message = files.Length == 1 ?
                                SR.GetString(SR.CancelQueryEdit, files[0]) :
                                SR.GetString(SR.CancelQueryEditMultiple);
                            string title = string.Empty;
                            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                            VsShellUtilities.ShowMessageBox(this.Site, title, message, icon, buttons, defaultButton);
                        }
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Verify if the file can be written to.
        /// Return false if the file is read only and/or not checked out
        /// and the user did not give permission to change it.
        /// Note that exact behavior can also be affected based on the SCC
        /// settings under Tools->Options.
        /// </summary>
        internal bool QueryEditProjectFile(bool suppressUI) {
            return QueryEditFiles(suppressUI, filename);
        }

        internal bool QueryFolderAdd(HierarchyNode targetFolder, string path) {
            if (!disableQueryEdit) {
                var queryTrack = this.GetService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
                if (queryTrack != null) {
                    VSQUERYADDDIRECTORYRESULTS[] res = new VSQUERYADDDIRECTORYRESULTS[1];
                    ErrorHandler.ThrowOnFailure(
                        queryTrack.OnQueryAddDirectories(
                            GetOuterInterface<IVsProject>(),
                            1,
                            new[] { CommonUtils.GetAbsoluteFilePath(GetBaseDirectoryForAddingFiles(targetFolder), Path.GetFileName(path)) },
                            new[] { VSQUERYADDDIRECTORYFLAGS.VSQUERYADDDIRECTORYFLAGS_padding },
                            res,
                            res
                        )
                    );

                    if (res[0] == VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddNotOK) {
                        return false;
                    }
                }
            }
            return true;
        }

        internal bool QueryFolderRemove(HierarchyNode targetFolder, string path) {
            if (!disableQueryEdit) {
                var queryTrack = this.GetService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
                if (queryTrack != null) {
                    VSQUERYREMOVEDIRECTORYRESULTS[] res = new VSQUERYREMOVEDIRECTORYRESULTS[1];
                    ErrorHandler.ThrowOnFailure(
                        queryTrack.OnQueryRemoveDirectories(
                            GetOuterInterface<IVsProject>(),
                            1,
                            new[] { CommonUtils.GetAbsoluteFilePath(GetBaseDirectoryForAddingFiles(targetFolder), Path.GetFileName(path)) },
                            new[] { VSQUERYREMOVEDIRECTORYFLAGS.VSQUERYREMOVEDIRECTORYFLAGS_padding },
                            res,
                            res
                        )
                    );

                    if (res[0] == VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveNotOK) {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Given a node determines what is the directory that can accept files.
        /// If the node is a FoldeNode than it is the Url of the Folder.
        /// If the node is a ProjectNode it is the project folder.
        /// Otherwise (such as FileNode subitem) it delegate the resolution to the parent node.
        /// </summary>
        internal string GetBaseDirectoryForAddingFiles(HierarchyNode nodeToAddFile) {
            string baseDir = String.Empty;

            if (nodeToAddFile is FolderNode) {
                baseDir = nodeToAddFile.Url;
            } else if (nodeToAddFile is ProjectNode) {
                baseDir = this.ProjectHome;
            } else if (nodeToAddFile != null) {
                baseDir = GetBaseDirectoryForAddingFiles(nodeToAddFile.Parent);
            }

            return baseDir;
        }

        /// <summary>
        /// For internal use only.
        /// This creates a copy of an existing configuration and add it to the project.
        /// Caller should change the condition on the PropertyGroup.
        /// If derived class want to accomplish this, they should call ConfigProvider.AddCfgsOfCfgName()
        /// It is expected that in the future MSBuild will have support for this so we don't have to
        /// do it manually.
        /// </summary>
        /// <param name="group">PropertyGroup to clone</param>
        /// <returns></returns>
        internal MSBuildConstruction.ProjectPropertyGroupElement ClonePropertyGroup(MSBuildConstruction.ProjectPropertyGroupElement group) {
            // Create a new (empty) PropertyGroup
            MSBuildConstruction.ProjectPropertyGroupElement newPropertyGroup = this.buildProject.Xml.AddPropertyGroup();

            // Now copy everything from the group we are trying to clone to the group we are creating
            if (!String.IsNullOrEmpty(group.Condition))
                newPropertyGroup.Condition = group.Condition;
            foreach (MSBuildConstruction.ProjectPropertyElement prop in group.Properties) {
                MSBuildConstruction.ProjectPropertyElement newProperty = newPropertyGroup.AddProperty(prop.Name, prop.Value);
                if (!String.IsNullOrEmpty(prop.Condition))
                    newProperty.Condition = prop.Condition;
            }

            return newPropertyGroup;
        }

        /// <summary>
        /// Get the project extensions
        /// </summary>
        /// <returns></returns>
        internal MSBuildConstruction.ProjectExtensionsElement GetProjectExtensions() {
            var extensionsElement = this.buildProject.Xml.ChildrenReversed.OfType<MSBuildConstruction.ProjectExtensionsElement>().FirstOrDefault();

            if (extensionsElement == null) {
                extensionsElement = this.buildProject.Xml.CreateProjectExtensionsElement();
                this.buildProject.Xml.AppendChild(extensionsElement);
            }

            return extensionsElement;
        }

        /// <summary>
        /// Set the xmlText as a project extension element with the id passed.
        /// </summary>
        /// <param name="id">The id of the project extension element.</param>
        /// <param name="xmlText">The value to set for a project extension.</param>
        internal void SetProjectExtensions(string id, string xmlText) {
            MSBuildConstruction.ProjectExtensionsElement element = this.GetProjectExtensions();

            // If it doesn't already have a value and we're asked to set it to
            // nothing, don't do anything. Same as old OM. Keeps project neat.
            if (element == null) {
                if (xmlText.Length == 0) {
                    return;
                }

                element = this.buildProject.Xml.CreateProjectExtensionsElement();
                this.buildProject.Xml.AppendChild(element);
            }

            element[id] = xmlText;
        }

        /// <summary>
        /// Register the project with the Scc manager.
        /// </summary>
        protected void RegisterSccProject() {

            if (this.isRegisteredWithScc || String.IsNullOrEmpty(this.sccProjectName)) {
                return;
            }

            IVsSccManager2 sccManager = this.Site.GetService(typeof(SVsSccManager)) as IVsSccManager2;

            if (sccManager != null) {
                ErrorHandler.ThrowOnFailure(sccManager.RegisterSccProject(this, this.sccProjectName, this.sccAuxPath, this.sccLocalPath, this.sccProvider));

                this.isRegisteredWithScc = true;
            }
        }

        /// <summary>
        ///  Unregisters us from the SCC manager
        /// </summary>
        protected void UnRegisterProject() {
            if (!this.isRegisteredWithScc) {
                return;
            }

            IVsSccManager2 sccManager = this.Site.GetService(typeof(SVsSccManager)) as IVsSccManager2;

            if (sccManager != null) {
                ErrorHandler.ThrowOnFailure(sccManager.UnregisterSccProject(this));
                this.isRegisteredWithScc = false;
            }
        }

        /// <summary>
        /// Get the CATID corresponding to the specified type.
        /// </summary>
        /// <param name="type">Type of the object for which you want the CATID</param>
        /// <returns>CATID</returns>
        protected internal Guid GetCATIDForType(Type type) {
            Utilities.ArgumentNotNull("type", type);

            if (catidMapping == null) {
                catidMapping = new Dictionary<Type, Guid>();
                InitializeCATIDs();
            }

            Guid result;
            if (catidMapping.TryGetValue(type, out result)) {
                return result;
            }
            // If you get here and you want your object to be extensible, then add a call to AddCATIDMapping() in your project constructor
            return Guid.Empty;
        }

        /// <summary>
        /// This is used to specify a CATID corresponding to a BrowseObject or an ExtObject.
        /// The CATID can be any GUID you choose. For types which are your owns, you could use
        /// their type GUID, while for other types (such as those provided in the MPF) you should
        /// provide a different GUID.
        /// </summary>
        /// <param name="type">Type of the extensible object</param>
        /// <param name="catid">GUID that extender can use to uniquely identify your object type</param>
        protected void AddCATIDMapping(Type type, Guid catid) {
            if (catidMapping == null) {
                catidMapping = new Dictionary<Type, Guid>();
                InitializeCATIDs();
            }

            catidMapping.Add(type, catid);
        }

        /// <summary>
        /// Initialize an object with an XML fragment.
        /// </summary>
        /// <param name="iPersistXMLFragment">Object that support being initialized with an XML fragment</param>
        /// <param name="configName">Name of the configuration being initialized, null if it is the project</param>
        /// <param name="platformName">Name of the platform being initialized, null is ok</param>
        protected internal void LoadXmlFragment(IPersistXMLFragment persistXmlFragment, string configName, string platformName) {
            Utilities.ArgumentNotNull("persistXmlFragment", persistXmlFragment);

            if (xmlFragments == null) {
                // Retrieve the xml fragments from MSBuild
                xmlFragments = new XmlDocument();

                string fragments = GetProjectExtensions()[ProjectFileConstants.VisualStudio];
                fragments = String.Format(CultureInfo.InvariantCulture, "<root>{0}</root>", fragments);
                xmlFragments.LoadXml(fragments);
            }

            // We need to loop through all the flavors
            string flavorsGuid;
            ErrorHandler.ThrowOnFailure(((IVsAggregatableProject)this).GetAggregateProjectTypeGuids(out flavorsGuid));
            foreach (Guid flavor in Utilities.GuidsArrayFromSemicolonDelimitedStringOfGuids(flavorsGuid)) {
                // Look for a matching fragment
                string flavorGuidString = flavor.ToString("B");
                string fragment = null;
                XmlNode node = null;
                foreach (XmlNode child in xmlFragments.FirstChild.ChildNodes) {
                    if (child.Attributes.Count > 0) {
                        string guid = String.Empty;
                        string configuration = String.Empty;
                        string platform = String.Empty;
                        if (child.Attributes[ProjectFileConstants.Guid] != null)
                            guid = child.Attributes[ProjectFileConstants.Guid].Value;
                        if (child.Attributes[ProjectFileConstants.Configuration] != null)
                            configuration = child.Attributes[ProjectFileConstants.Configuration].Value;
                        if (child.Attributes[ProjectFileConstants.Platform] != null)
                            platform = child.Attributes[ProjectFileConstants.Platform].Value;

                        if (String.Compare(child.Name, ProjectFileConstants.FlavorProperties, StringComparison.OrdinalIgnoreCase) == 0
                                && String.Compare(guid, flavorGuidString, StringComparison.OrdinalIgnoreCase) == 0
                                && ((String.IsNullOrEmpty(configName) && String.IsNullOrEmpty(configuration))
                                    || (String.Compare(configuration, configName, StringComparison.OrdinalIgnoreCase) == 0))
                                && ((String.IsNullOrEmpty(platformName) && String.IsNullOrEmpty(platform))
                                    || (String.Compare(platform, platformName, StringComparison.OrdinalIgnoreCase) == 0))) {
                            // we found the matching fragment
                            fragment = child.InnerXml;
                            node = child;
                            break;
                        }
                    }
                }

                Guid flavorGuid = flavor;
                if (String.IsNullOrEmpty(fragment)) {
                    // the fragment was not found so init with default values
                    ErrorHandler.ThrowOnFailure(persistXmlFragment.InitNew(ref flavorGuid, (uint)_PersistStorageType.PST_PROJECT_FILE));
                    // While we don't yet support user files, our flavors might, so we will store that in the project file until then
                    // TODO: Refactor this code when we support user files
                    ErrorHandler.ThrowOnFailure(persistXmlFragment.InitNew(ref flavorGuid, (uint)_PersistStorageType.PST_USER_FILE));
                } else {
                    ErrorHandler.ThrowOnFailure(persistXmlFragment.Load(ref flavorGuid, (uint)_PersistStorageType.PST_PROJECT_FILE, fragment));
                    // While we don't yet support user files, our flavors might, so we will store that in the project file until then
                    // TODO: Refactor this code when we support user files
                    if (node.NextSibling != null && node.NextSibling.Attributes[ProjectFileConstants.User] != null)
                        ErrorHandler.ThrowOnFailure(persistXmlFragment.Load(ref flavorGuid, (uint)_PersistStorageType.PST_USER_FILE, node.NextSibling.InnerXml));
                }
            }
        }

        /// <summary>
        /// Retrieve all XML fragments that need to be saved from the flavors and store the information in msbuild.
        /// </summary>
        protected void PersistXMLFragments() {
            if (IsFlavorDirty()) {
                XmlDocument doc = new XmlDocument();
                XmlElement root = doc.CreateElement("ROOT");

                // We will need the list of configuration inside the loop, so get it before entering the loop
                uint[] count = new uint[1];
                IVsCfg[] configs = null;
                int hr = this.ConfigProvider.GetCfgs(0, null, count, null);
                if (ErrorHandler.Succeeded(hr) && count[0] > 0) {
                    configs = new IVsCfg[count[0]];
                    hr = this.ConfigProvider.GetCfgs((uint)configs.Length, configs, count, null);
                    if (ErrorHandler.Failed(hr))
                        count[0] = 0;
                }
                if (count[0] == 0)
                    configs = new IVsCfg[0];

                // We need to loop through all the flavors
                string flavorsGuid;
                ErrorHandler.ThrowOnFailure(((IVsAggregatableProject)this).GetAggregateProjectTypeGuids(out flavorsGuid));
                foreach (Guid flavor in Utilities.GuidsArrayFromSemicolonDelimitedStringOfGuids(flavorsGuid)) {
                    IPersistXMLFragment outerHierarchy = GetOuterInterface<IPersistXMLFragment>();
                    // First check the project
                    if (outerHierarchy != null) {
                        // Retrieve the XML fragment
                        string fragment = string.Empty;
                        Guid flavorGuid = flavor;
                        ErrorHandler.ThrowOnFailure((outerHierarchy).Save(ref flavorGuid, (uint)_PersistStorageType.PST_PROJECT_FILE, out fragment, 1));
                        if (!String.IsNullOrEmpty(fragment)) {
                            // Add the fragment to our XML
                            WrapXmlFragment(doc, root, flavor, null, null, fragment);
                        }
                        // While we don't yet support user files, our flavors might, so we will store that in the project file until then
                        // TODO: Refactor this code when we support user files
                        fragment = String.Empty;
                        ErrorHandler.ThrowOnFailure((outerHierarchy).Save(ref flavorGuid, (uint)_PersistStorageType.PST_USER_FILE, out fragment, 1));
                        if (!String.IsNullOrEmpty(fragment)) {
                            // Add the fragment to our XML
                            XmlElement node = WrapXmlFragment(doc, root, flavor, null, null, fragment);
                            node.Attributes.Append(doc.CreateAttribute(ProjectFileConstants.User));
                        }
                    }

                    // Then look at the configurations
                    foreach (IVsCfg config in configs) {
                        // Get the fragment for this flavor/config pair
                        string fragment;
                        ErrorHandler.ThrowOnFailure(((ProjectConfig)config).GetXmlFragment(flavor, _PersistStorageType.PST_PROJECT_FILE, out fragment));
                        if (!String.IsNullOrEmpty(fragment)) {
                            WrapXmlFragment(doc, root, flavor, ((ProjectConfig)config).ConfigName, ((ProjectConfig)config).PlatformName, fragment);
                        }
                    }
                }
                if (root.ChildNodes != null && root.ChildNodes.Count > 0) {
                    // Save our XML (this is only the non-build information for each flavor) in msbuild
                    SetProjectExtensions(ProjectFileConstants.VisualStudio, root.InnerXml.ToString());
                }
            }
        }

#if DEV14_OR_LATER
        [Obsolete("Use ImageMonikers instead")]
#endif
        internal int GetIconIndex(ImageName name) {
            return (int)name;
        }

#if DEV14_OR_LATER
        [Obsolete("Use ImageMonikers instead")]
#endif
        internal IntPtr GetIconHandleByName(ImageName name) {
            return ImageHandler.GetIconHandle(GetIconIndex(name));
        }

        internal Dictionary<string, string> ParseCommandArgs(IntPtr vaIn, Guid cmdGroup, uint cmdId) {
            var switches = QueryCommandArguments(cmdGroup, cmdId, CommandOrigin.UiHierarchy);
            if (string.IsNullOrEmpty(switches)) {
                return null;
            }

            return ParseCommandArgs(vaIn, switches);
        }

        internal Dictionary<string, string> ParseCommandArgs(IntPtr vaIn, string switches) {
            string args;
            if (vaIn == IntPtr.Zero || string.IsNullOrEmpty(args = Marshal.GetObjectForNativeVariant(vaIn) as string)) {
                return null;
            }

            var parse = Site.GetService(typeof(SVsParseCommandLine)) as IVsParseCommandLine;
            if (ErrorHandler.Failed(parse.ParseCommandTail(args, -1))) {
                return null;
            }

            parse.EvaluateSwitches(switches);

            var res = new Dictionary<string, string>();
            int i = -1;
            foreach (var sw in switches.Split(' ')) {
                i += 1;
                var key = sw;
                int comma = key.IndexOf(',');
                if (comma > 0) {
                    key = key.Remove(comma);
                }

                string value;
                int hr;
                switch (hr = parse.IsSwitchPresent(i)) {
                    case VSConstants.S_OK:
                        ErrorHandler.ThrowOnFailure(parse.GetSwitchValue(i, out value));
                        res[key] = value;
                        break;
                    case VSConstants.S_FALSE:
                        break;
                    default:
                        ErrorHandler.ThrowOnFailure(hr);
                        break;
                }
            }

            i = 0;
            int count;
            ErrorHandler.ThrowOnFailure(parse.GetParamCount(out count));
            for (i = 0; i < count; ++i) {
                string key = i.ToString(), value;
                ErrorHandler.ThrowOnFailure(parse.GetParam(i, out value));
                res[key] = value;
            }

            return res;
        }

        #endregion

        #region IVsGetCfgProvider Members
        //=================================================================================

        public virtual int GetCfgProvider(out IVsCfgProvider p) {
            // Be sure to call the property here since that is doing a polymorhic ProjectConfig creation.
            p = this.ConfigProvider;
            return (p == null ? VSConstants.E_NOTIMPL : VSConstants.S_OK);
        }
        #endregion

        #region IPersist Members

        public int GetClassID(out Guid clsid) {
            clsid = this.ProjectGuid;
            return VSConstants.S_OK;
        }
        #endregion

        #region IPersistFileFormat Members

        int IPersistFileFormat.GetClassID(out Guid clsid) {
            clsid = this.ProjectGuid;
            return VSConstants.S_OK;
        }

        public virtual int GetCurFile(out string name, out uint formatIndex) {
            name = this.filename;
            formatIndex = 0;
            return VSConstants.S_OK;
        }

        public virtual int GetFormatList(out string formatlist) {
            formatlist = String.Empty;
            return VSConstants.S_OK;
        }

        public virtual int InitNew(uint formatIndex) {
            return VSConstants.S_OK;
        }

        public virtual int IsDirty(out int isDirty) {
            if (BuildProject.Xml.HasUnsavedChanges || IsProjectFileDirty || IsFlavorDirty()) {
                isDirty = 1;
            } else {
                isDirty = 0;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Get the outer IVsHierarchy implementation.
        /// This is used for scenario where a flavor may be modifying the behavior
        /// </summary>
        internal IVsHierarchy GetOuterHierarchy() {
            IVsHierarchy hierarchy = null;
            // The hierarchy of a node is its project node hierarchy
            IntPtr projectUnknown = Marshal.GetIUnknownForObject(this);
            try {
                hierarchy = (IVsHierarchy)Marshal.GetTypedObjectForIUnknown(projectUnknown, typeof(IVsHierarchy));
            } finally {
                if (projectUnknown != IntPtr.Zero) {
                    Marshal.Release(projectUnknown);
                }
            }
            return hierarchy;
        }

        internal T GetOuterInterface<T>() where T : class {
            return GetOuterHierarchy() as T;
        }

        private bool IsFlavorDirty() {
            int isDirty = 0;
            // See if one of our flavor consider us dirty
            IPersistXMLFragment outerHierarchy = GetOuterInterface<IPersistXMLFragment>();
            if (outerHierarchy != null) {
                // First check the project
                ErrorHandler.ThrowOnFailure(outerHierarchy.IsFragmentDirty((uint)_PersistStorageType.PST_PROJECT_FILE, out isDirty));
                // While we don't yet support user files, our flavors might, so we will store that in the project file until then
                // TODO: Refactor this code when we support user files
                if (isDirty == 0)
                    ErrorHandler.ThrowOnFailure(outerHierarchy.IsFragmentDirty((uint)_PersistStorageType.PST_USER_FILE, out isDirty));
            }
            if (isDirty == 0) {
                // Then look at the configurations
                uint[] count = new uint[1];
                int hr = this.ConfigProvider.GetCfgs(0, null, count, null);
                if (ErrorHandler.Succeeded(hr) && count[0] > 0) {
                    // We need to loop through the configurations
                    IVsCfg[] configs = new IVsCfg[count[0]];
                    hr = this.ConfigProvider.GetCfgs((uint)configs.Length, configs, count, null);
                    Debug.Assert(ErrorHandler.Succeeded(hr), "failed to retrieve configurations");
                    foreach (IVsCfg config in configs) {
                        isDirty = ((ProjectConfig)config).IsFlavorDirty(_PersistStorageType.PST_PROJECT_FILE);
                        if (isDirty != 0)
                            break;
                    }
                }
            }
            return isDirty != 0;
        }

        int IPersistFileFormat.Load(string fileName, uint mode, int readOnly) {
            // This isn't how projects are loaded, C#, VB, and CPS all fail this call
            return VSConstants.E_NOTIMPL;
        }

        public virtual int Save(string fileToBeSaved, int remember, uint formatIndex) {
            // The file name can be null. Then try to use the Url.
            string tempFileToBeSaved = fileToBeSaved;
            if (String.IsNullOrEmpty(tempFileToBeSaved) && !String.IsNullOrEmpty(this.Url)) {
                tempFileToBeSaved = this.Url;
            }

            if (String.IsNullOrEmpty(tempFileToBeSaved)) {
                throw new ArgumentException(SR.GetString(SR.InvalidParameter), "fileToBeSaved");
            }

            int setProjectFileDirtyAfterSave = 0;
            if (remember == 0) {
                ErrorHandler.ThrowOnFailure(IsDirty(out setProjectFileDirtyAfterSave));
            }

            // Update the project with the latest flavor data (if needed)
            PersistXMLFragments();

            int result = VSConstants.S_OK;
            bool saveAs = true;
            if (CommonUtils.IsSamePath(tempFileToBeSaved, this.filename)) {
                saveAs = false;
            }
            if (!saveAs) {
                SuspendFileChanges fileChanges = new SuspendFileChanges(this.Site, this.filename);
                fileChanges.Suspend();
                try {
                    // Ensure the directory exist
                    string saveFolder = Path.GetDirectoryName(tempFileToBeSaved);
                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);
                    // Save the project
                    SaveMSBuildProjectFile(tempFileToBeSaved);
                } finally {
                    fileChanges.Resume();
                }
            } else {
                result = this.SaveAs(tempFileToBeSaved);
                if (result != VSConstants.OLE_E_PROMPTSAVECANCELLED) {
                    ErrorHandler.ThrowOnFailure(result);
                }

            }

            if (setProjectFileDirtyAfterSave != 0) {
                isDirty = true;
            }

            return result;
        }

        protected virtual void SaveMSBuildProjectFile(string filename) {
            buildProject.Save(filename);
            isDirty = false;
        }

        public virtual int SaveCompleted(string filename) {
            // TODO: turn file watcher back on.
            return VSConstants.S_OK;
        }
        #endregion

        #region IVsProject3 Members

        /// <summary>
        /// Callback from the additem dialog. Deals with adding new and existing items
        /// </summary>
        public virtual int GetMkDocument(uint itemId, out string mkDoc) {
            mkDoc = null;
            if (itemId == VSConstants.VSITEMID_SELECTION) {
                return VSConstants.E_UNEXPECTED;
            }

            HierarchyNode n = this.NodeFromItemId(itemId);
            if (n == null) {
                return VSConstants.E_INVALIDARG;
            }

            mkDoc = n.GetMkDocument();

            if (String.IsNullOrEmpty(mkDoc)) {
                return VSConstants.E_FAIL;
            }

            return VSConstants.S_OK;
        }


        public virtual int AddItem(uint itemIdLoc, VSADDITEMOPERATION op, string itemName, uint filesToOpen, string[] files, IntPtr dlgOwner, VSADDRESULT[] result) {
            Guid empty = Guid.Empty;

            return AddItemWithSpecific(
                itemIdLoc,
                op,
                itemName,
                filesToOpen,
                files,
                dlgOwner,
                op == VSADDITEMOPERATION.VSADDITEMOP_CLONEFILE ? (uint)__VSSPECIFICEDITORFLAGS.VSSPECIFICEDITOR_DoOpen : 0,
                ref empty,
                null,
                ref empty,
                result);
        }

        /// <summary>
        /// Creates new items in a project, adds existing files to a project, or causes Add Item wizards to be run
        /// </summary>
        /// <param name="itemIdLoc"></param>
        /// <param name="op"></param>
        /// <param name="itemName"></param>
        /// <param name="filesToOpen"></param>
        /// <param name="files">Array of file names. 
        /// If dwAddItemOperation is VSADDITEMOP_CLONEFILE the first item in the array is the name of the file to clone. 
        /// If dwAddItemOperation is VSADDITEMOP_OPENDIRECTORY, the first item in the array is the directory to open. 
        /// If dwAddItemOperation is VSADDITEMOP_RUNWIZARD, the first item is the name of the wizard to run, 
        /// and the second item is the file name the user supplied (same as itemName).</param>
        /// <param name="dlgOwner"></param>
        /// <param name="editorFlags"></param>
        /// <param name="editorType"></param>
        /// <param name="physicalView"></param>
        /// <param name="logicalView"></param>
        /// <param name="result"></param>
        /// <returns>S_OK if it succeeds </returns>
        /// <remarks>The result array is initalized to failure.</remarks>
        public virtual int AddItemWithSpecific(uint itemIdLoc, VSADDITEMOPERATION op, string itemName, uint filesToOpen, string[] files, IntPtr dlgOwner, uint editorFlags, ref Guid editorType, string physicalView, ref Guid logicalView, VSADDRESULT[] result) {
            return AddItemWithSpecificInternal(itemIdLoc, op, itemName, filesToOpen, files, dlgOwner, editorFlags, ref editorType, physicalView, ref logicalView, result);
        }

        // TODO: Refactor me into something sane
        internal int AddItemWithSpecificInternal(uint itemIdLoc, VSADDITEMOPERATION op, string itemName, uint filesToOpen, string[] files, IntPtr dlgOwner, uint editorFlags, ref Guid editorType, string physicalView, ref Guid logicalView, VSADDRESULT[] result, bool? promptOverwrite = null) {
            if (files == null || result == null || files.Length == 0 || result.Length == 0) {
                return VSConstants.E_INVALIDARG;
            }

            // Locate the node to be the container node for the file(s) being added
            // only projectnode or foldernode and file nodes are valid container nodes
            // We need to locate the parent since the item wizard expects the parent to be passed.
            HierarchyNode n = this.NodeFromItemId(itemIdLoc);
            if (n == null) {
                return VSConstants.E_INVALIDARG;
            }

            while (!n.CanAddFiles && (!this.CanFileNodesHaveChilds || !(n is FileNode))) {
                n = n.Parent;
            }
            Debug.Assert(n != null, "We should at this point have either a ProjectNode or FolderNode or a FileNode as a container for the new filenodes");

            // handle link and runwizard operations at this point
            bool isLink = false;
            switch (op) {
                case VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE:
                    // we do not support this right now
                    isLink = true;
                    break;

                case VSADDITEMOPERATION.VSADDITEMOP_RUNWIZARD:
                    result[0] = this.RunWizard(n, itemName, files[0], dlgOwner);
                    return VSConstants.S_OK;
            }

            string[] actualFiles = new string[files.Length];


            VSQUERYADDFILEFLAGS[] flags = this.GetQueryAddFileFlags(files);

            string baseDir = this.GetBaseDirectoryForAddingFiles(n);
            // If we did not get a directory for node that is the parent of the item then fail.
            if (String.IsNullOrEmpty(baseDir)) {
                return VSConstants.E_FAIL;
            }

            // Pre-calculates some paths that we can use when calling CanAddItems
            List<string> filesToAdd = new List<string>();
            foreach (var file in files) {
                string fileName;
                string newFileName = String.Empty;

                switch (op) {
                    case VSADDITEMOPERATION.VSADDITEMOP_CLONEFILE:
                        fileName = Path.GetFileName(itemName ?? file);
                        newFileName = CommonUtils.GetAbsoluteFilePath(baseDir, fileName);
                        break;
                    case VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE:
                    case VSADDITEMOPERATION.VSADDITEMOP_OPENFILE:
                        fileName = Path.GetFileName(file);
                        newFileName = CommonUtils.GetAbsoluteFilePath(baseDir, fileName);

                        if (isLink && CommonUtils.IsSubpathOf(ProjectHome, file)) {
                            // creating a link to a file that's actually in the project, it's not really a link.
                            isLink = false;

                            // If the file is not going to be added in its
                            // current path (GetDirectoryName(file) != baseDir),
                            // we need to update the filename and also update
                            // the destination node (n). Otherwise, we don't
                            // want to change the destination node (previous
                            // behavior) - just trust that our caller knows
                            // what they are doing. (Web Essentials relies on
                            // this.)
                            if (!CommonUtils.IsSameDirectory(baseDir, Path.GetDirectoryName(file))) {
                                newFileName = file;
                                n = this.CreateFolderNodes(Path.GetDirectoryName(file));
                            }
                        }
                        break;
                }
                filesToAdd.Add(newFileName);
            }

            // Ask tracker objects if we can add files
            if (!this.tracker.CanAddItems(filesToAdd.ToArray(), flags)) {
                // We were not allowed to add the files
                return VSConstants.E_FAIL;
            }

            if (!this.QueryEditProjectFile(false)) {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            // Add the files to the hierarchy
            int actualFilesAddedIndex = 0;
            var itemsToInvalidate = new List<HierarchyNode>();
            for (int index = 0; index < filesToAdd.Count; index++) {
                HierarchyNode child;
                bool overwrite = false;
                MsBuildProjectElement linkedFile = null;
                string newFileName = filesToAdd[index];

                string file = files[index];
                result[0] = VSADDRESULT.ADDRESULT_Failure;

                child = this.FindNodeByFullPath(newFileName);
                if (child != null) {
                    // If the file to be added is an existing file part of the hierarchy then continue.
                    if (CommonUtils.IsSamePath(file, newFileName)) {
                        if (child.IsNonMemberItem) {
                            for (var node = child; node != null; node = node.Parent) {
                                itemsToInvalidate.Add(node);
                                // We want to include the first member item, so
                                // this test is not part of the loop condition.
                                if (!node.IsNonMemberItem) {
                                    break;
                                }
                            }
                            // https://pytools.codeplex.com/workitem/1251
                            ErrorHandler.ThrowOnFailure(child.IncludeInProject(false));
                        }
                        result[0] = VSADDRESULT.ADDRESULT_Cancel;
                        continue;
                    } else if (isLink) {
                        string message = "There is already a file of the same name in this folder.";
                        string title = string.Empty;
                        OLEMSGICON icon = OLEMSGICON.OLEMSGICON_QUERY;
                        OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                        OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;

                        VsShellUtilities.ShowMessageBox(this.Site, title, message, icon, buttons, defaultButton);

                        result[0] = VSADDRESULT.ADDRESULT_Cancel;
                        return (int)OleConstants.OLECMDERR_E_CANCELED;
                    } else {
                        int canOverWriteExistingItem = CanOverwriteExistingItem(file, newFileName, !child.IsNonMemberItem);
                        if (canOverWriteExistingItem == E_CANCEL_FILE_ADD) {
                            result[0] = VSADDRESULT.ADDRESULT_Cancel;
                            return (int)OleConstants.OLECMDERR_E_CANCELED;
                        } else if (canOverWriteExistingItem == (int)OleConstants.OLECMDERR_E_CANCELED) {
                            result[0] = VSADDRESULT.ADDRESULT_Cancel;
                            return canOverWriteExistingItem;
                        } else if (canOverWriteExistingItem == VSConstants.S_OK) {
                            overwrite = true;
                        } else {
                            return canOverWriteExistingItem;
                        }
                    }
                } else {
                    if (isLink) {
                        child = this.FindNodeByFullPath(file);
                        if (child != null) {
                            string message = String.Format("There is already a link to '{0}'. A project cannot have more than one link to the same file.", file);
                            string title = string.Empty;
                            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_QUERY;
                            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;

                            VsShellUtilities.ShowMessageBox(this.Site, title, message, icon, buttons, defaultButton);

                            result[0] = VSADDRESULT.ADDRESULT_Cancel;
                            return (int)OleConstants.OLECMDERR_E_CANCELED;
                        }
                    }

                    if (newFileName.Length >= NativeMethods.MAX_PATH) {
                        OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                        OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                        OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;

                        VsShellUtilities.ShowMessageBox(Site, FolderNode.PathTooLongMessage, null, icon, buttons, defaultButton);

                        result[0] = VSADDRESULT.ADDRESULT_Cancel;
                        return (int)OleConstants.OLECMDERR_E_CANCELED;
                    }

                    // we need to figure out where this file would be added and make sure there's
                    // not an existing link node at the same location
                    string filename = Path.GetFileName(newFileName);
                    var folder = this.FindNodeByFullPath(Path.GetDirectoryName(newFileName));
                    if (folder != null) {
                        if (folder.FindImmediateChildByName(filename) != null) {
                            string message = "There is already a file of the same name in this folder.";
                            string title = string.Empty;
                            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_QUERY;
                            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;

                            VsShellUtilities.ShowMessageBox(this.Site, title, message, icon, buttons, defaultButton);

                            result[0] = VSADDRESULT.ADDRESULT_Cancel;
                            return (int)OleConstants.OLECMDERR_E_CANCELED;
                        }
                    }
                }

                // If the file to be added is not in the same path copy it.
                if (!CommonUtils.IsSamePath(file, newFileName) || Directory.Exists(newFileName)) {
                    if (!overwrite && File.Exists(newFileName)) {
                        var existingChild = this.FindNodeByFullPath(file);
                        if (existingChild == null || !existingChild.IsLinkFile) {
                            string message = SR.GetString(SR.FileAlreadyExists, newFileName);
                            string title = string.Empty;
                            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_QUERY;
                            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
                            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                            if (isLink) {
                                message = "There is already a file of the same name in this folder.";
                                buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                            }

                            int messageboxResult = VsShellUtilities.ShowMessageBox(this.Site, title, message, icon, buttons, defaultButton);
                            if (messageboxResult != NativeMethods.IDYES) {
                                result[0] = VSADDRESULT.ADDRESULT_Cancel;
                                return (int)OleConstants.OLECMDERR_E_CANCELED;
                            }
                        }
                    }

                    var updatingNode = this.FindNodeByFullPath(file);
                    if (updatingNode != null && updatingNode.IsLinkFile) {
                        // we just need to update the link to the new path.
                        linkedFile = updatingNode.ItemNode as MsBuildProjectElement;
                    } else if (Directory.Exists(file)) {
                        // http://pytools.codeplex.com/workitem/546

                        int hr = AddDirectory(result, n, file, promptOverwrite);
                        if (ErrorHandler.Failed(hr)) {
                            return hr;
                        }
                        result[0] = VSADDRESULT.ADDRESULT_Success;
                        continue;
                    } else if (!isLink) {
                        // Copy the file to the correct location.
                        // We will suppress the file change events to be triggered to this item, since we are going to copy over the existing file and thus we will trigger a file change event. 
                        // We do not want the filechange event to ocur in this case, similar that we do not want a file change event to occur when saving a file.
                        IVsFileChangeEx fileChange = this.site.GetService(typeof(SVsFileChangeEx)) as IVsFileChangeEx;
                        Utilities.CheckNotNull(fileChange);

                        try {
                            ErrorHandler.ThrowOnFailure(fileChange.IgnoreFile(VSConstants.VSCOOKIE_NIL, newFileName, 1));
                            if (op == VSADDITEMOPERATION.VSADDITEMOP_CLONEFILE) {
                                this.AddFileFromTemplate(file, newFileName);
                            } else {
                                PackageUtilities.CopyUrlToLocal(new Uri(file), newFileName);

                                // Reset RO attribute on file if present - for example, if source file was under TFS control and not checked out.
                                try {
                                    var fileInfo = new FileInfo(newFileName);
                                    if (fileInfo.Attributes.HasFlag(FileAttributes.ReadOnly)) {
                                        fileInfo.Attributes &= ~FileAttributes.ReadOnly;
                                    }
                                } catch (Exception ex) {
                                    // Best-effort, but no big deal if this fails.
                                    if (ex.IsCriticalException()) {
                                        throw;
                                    }
                                }
                            }
                        } finally {
                            ErrorHandler.ThrowOnFailure(fileChange.IgnoreFile(VSConstants.VSCOOKIE_NIL, newFileName, 0));
                        }
                    }
                }

                if (overwrite) {
                    if (child.IsNonMemberItem) {
                        ErrorHandler.ThrowOnFailure(child.IncludeInProject(false));
                    }
                } else if (linkedFile != null || isLink) {
                    // files not moving, add the old name, and set the link.
                    var friendlyPath = CommonUtils.GetRelativeFilePath(ProjectHome, file);
                    FileNode newChild;
                    if (linkedFile == null) {
                        Debug.Assert(!CommonUtils.IsSubpathOf(ProjectHome, file), "Should have cleared isLink above for file in project dir");
                        newChild = CreateFileNode(file);
                    } else {
                        newChild = CreateFileNode(linkedFile);
                    }

                    newChild.SetIsLinkFile(true);
                    newChild.ItemNode.SetMetadata(ProjectFileConstants.Link, CommonUtils.CreateFriendlyFilePath(ProjectHome, newFileName));
                    n.AddChild(newChild);

                    DocumentManager.RenameDocument(site, file, file, n.ID);

                    LinkFileAdded(file);
                } else {
                    //Add new filenode/dependentfilenode
                    this.AddNewFileNodeToHierarchy(n, newFileName);
                }

                result[0] = VSADDRESULT.ADDRESULT_Success;
                actualFiles[actualFilesAddedIndex++] = newFileName;
            }

            // Notify listeners that items were appended.
            if (actualFilesAddedIndex > 0)
                OnItemsAppended(n);

            foreach (var node in itemsToInvalidate.Where(node => node != null).Reverse()) {
                OnInvalidateItems(node);
            }

            //Open files if this was requested through the editorFlags
            bool openFiles = (editorFlags & (uint)__VSSPECIFICEDITORFLAGS.VSSPECIFICEDITOR_DoOpen) != 0;
            if (openFiles && actualFiles.Length <= filesToOpen) {
                for (int i = 0; i < filesToOpen; i++) {
                    if (!String.IsNullOrEmpty(actualFiles[i])) {
                        string name = actualFiles[i];
                        HierarchyNode child = this.FindNodeByFullPath(name);
                        Debug.Assert(child != null, "We should have been able to find the new element in the hierarchy");
                        if (child != null) {
                            IVsWindowFrame frame;
                            if (editorType == Guid.Empty) {
                                Guid view = child.DefaultOpensWithDesignView ? VSConstants.LOGVIEWID.Designer_guid : Guid.Empty;
                                ErrorHandler.ThrowOnFailure(this.OpenItem(child.ID, ref view, IntPtr.Zero, out frame));
                            } else {
                                ErrorHandler.ThrowOnFailure(this.OpenItemWithSpecific(child.ID, editorFlags, ref editorType, physicalView, ref logicalView, IntPtr.Zero, out frame));
                            }

                            // Show the window frame in the UI and make it the active window
                            if (frame != null) {
                                ErrorHandler.ThrowOnFailure(frame.Show());
                            }
                        }
                    }
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Adds a folder into the project recursing and adding any sub-files and sub-directories.
        /// 
        /// The user can be prompted to overwrite the existing files if the folder already exists
        /// in the project.  They will be initially prompted to overwrite - if they answer no
        /// we'll set promptOverwrite to false and when we recurse we won't prompt.  If they say
        /// yes then we'll set it to true and we will prompt for individual files.  
        /// </summary>
        private int AddDirectory(VSADDRESULT[] result, HierarchyNode n, string file, bool? promptOverwrite) {
            // need to recursively add all of the directory contents

            HierarchyNode targetFolder = n.FindImmediateChildByName(Path.GetFileName(file));
            if (targetFolder == null) {
                var fullPath = Path.Combine(GetBaseDirectoryForAddingFiles(n), Path.GetFileName(file));
                Directory.CreateDirectory(fullPath);
                var newChild = CreateFolderNode(fullPath);
                n.AddChild(newChild);
                targetFolder = newChild;
            } else if (targetFolder.IsNonMemberItem) {
                int hr = targetFolder.IncludeInProject(true);
                if (ErrorHandler.Succeeded(hr)) {
                    OnInvalidateItems(targetFolder.Parent);
                }
                return hr;
            } else if (promptOverwrite == null) {
                var res = MessageBox.Show(
                    String.Format(
                    @"This folder already contains a folder called '{0}'.

If the files in the existing folder have the same names as files in the folder you are copying, do you want to replace the existing files?", Path.GetFileName(file)),
                    "Merge Folders",
                    MessageBoxButtons.YesNoCancel
                );

                // yes means prompt for each file
                // no means don't prompt for any of the files
                // cancel means forget what I'm doing

                switch (res) {
                    case DialogResult.Cancel:
                        result[0] = VSADDRESULT.ADDRESULT_Cancel;
                        return (int)OleConstants.OLECMDERR_E_CANCELED;
                    case DialogResult.No:
                        promptOverwrite = false;
                        break;
                    case DialogResult.Yes:
                        promptOverwrite = true;
                        break;
                }
            }

            Guid empty = Guid.Empty;

            // add the files...
            var dirFiles = Directory.GetFiles(file);
            if (dirFiles.Length > 0) {
                var subRes = AddItemWithSpecificInternal(
                    targetFolder.ID,
                    VSADDITEMOPERATION.VSADDITEMOP_CLONEFILE,
                    null,
                    (uint)dirFiles.Length,
                    dirFiles,
                    IntPtr.Zero,
                    0,
                    ref empty,
                    null,
                    ref empty,
                    result,
                    promptOverwrite: promptOverwrite
                );

                if (ErrorHandler.Failed(subRes)) {
                    return subRes;
                }
            }

            // add any subdirectories...

            var subDirs = Directory.GetDirectories(file);
            if (subDirs.Length > 0) {
                return AddItemWithSpecificInternal(
                    targetFolder.ID,
                    VSADDITEMOPERATION.VSADDITEMOP_CLONEFILE,
                    null,
                    (uint)subDirs.Length,
                    subDirs,
                    IntPtr.Zero,
                    0,
                    ref empty,
                    null,
                    ref empty,
                    result,
                    promptOverwrite: promptOverwrite
                );
            }
            return VSConstants.S_OK;
        }

        protected virtual void LinkFileAdded(string filename) {
        }

        private static string GetIncrementedFileName(string newFileName, int count) {
            return CommonUtils.GetAbsoluteFilePath(Path.GetDirectoryName(newFileName), Path.GetFileNameWithoutExtension(newFileName) + " - Copy (" + count + ")" + Path.GetExtension(newFileName));
        }

        /// <summary>
        /// for now used by add folder. Called on the ROOT, as only the project should need
        /// to implement this.
        /// for folders, called with parent folder, blank extension and blank suggested root
        /// </summary>
        public virtual int GenerateUniqueItemName(uint itemIdLoc, string ext, string suggestedRoot, out string itemName) {
            string root = string.IsNullOrEmpty(suggestedRoot) ? "NewFolder" : suggestedRoot.Trim();
            string extToUse = string.IsNullOrEmpty(ext) ? "" : ext.Trim();
            itemName = string.Empty;

            // Find the folder or project the item is being added to.
            HierarchyNode parent = NodeFromItemId(itemIdLoc);
            while (parent != null && !parent.CanAddFiles) {
                parent = parent.Parent;
            }

            if (parent == null) {
                return VSConstants.E_FAIL;
            }

            var parentProject = parent as ProjectNode;
            var destDirectory = parentProject != null ? parentProject.ProjectHome : parent.Url;

            for (int count = 1; count < int.MaxValue; ++count) {
                var candidate = string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{1}{2}",
                    root,
                    count,
                    extToUse
                );

                var candidatePath = CommonUtils.GetAbsoluteFilePath(destDirectory, candidate);

                if (File.Exists(candidatePath) || Directory.Exists(candidatePath)) {
                    // Cannot create a file or a directory when one exists with
                    // the same name.
                    continue;
                }

                if (parent.AllChildren.Any(n => candidate == n.GetEditLabel())) {
                    // Cannot create a node if one exists with the same name.
                    continue;
                }

                itemName = candidate;
                return VSConstants.S_OK;
            }

            return VSConstants.E_FAIL;
        }


        public virtual int GetItemContext(uint itemId, out Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp) {
            // the as cast isn't necessary, but makes it obvious via Find all refs how this is being used
            psp = this.NodeFromItemId(itemId) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            return VSConstants.S_OK;
        }


        public virtual int IsDocumentInProject(string mkDoc, out int found, VSDOCUMENTPRIORITY[] pri, out uint itemId) {
            if (pri != null && pri.Length >= 1) {
                pri[0] = VSDOCUMENTPRIORITY.DP_Unsupported;
            }
            found = 0;
            itemId = 0;

            // Debugger will pass in non-normalized paths for remote Linux debugging (produced by concatenating a local Windows-style path
            // with a portion of the remote Unix-style path) - need to normalize to look it up.
            mkDoc = CommonUtils.NormalizePath(mkDoc);

            // If it is the project file just return.
            if (CommonUtils.IsSamePath(mkDoc, this.GetMkDocument())) {
                found = 1;
                itemId = VSConstants.VSITEMID_ROOT;
            } else {
                HierarchyNode child = this.FindNodeByFullPath(EnsureRootedPath(mkDoc));
                if (child != null && (!child.IsNonMemberItem || IncludeNonMemberItemInProject(child))) {
                    found = 1;
                    itemId = child.ID;
                }
            }

            if (found == 1) {
                if (pri != null && pri.Length >= 1) {
                    pri[0] = VSDOCUMENTPRIORITY.DP_Standard;
                }
            }

            return VSConstants.S_OK;

        }

        protected virtual bool IncludeNonMemberItemInProject(HierarchyNode node) {
            return false;
        }

        public virtual int OpenItem(uint itemId, ref Guid logicalView, IntPtr punkDocDataExisting, out IVsWindowFrame frame) {
            // Init output params
            frame = null;

            HierarchyNode n = this.NodeFromItemId(itemId);
            if (n == null) {
                throw new ArgumentException(SR.GetString(SR.ParameterMustBeAValidItemId), "itemId");
            }

            // Delegate to the document manager object that knows how to open the item
            DocumentManager documentManager = n.GetDocumentManager();
            if (documentManager != null) {
                return documentManager.Open(ref logicalView, punkDocDataExisting, out frame, WindowFrameShowAction.DoNotShow);
            }

            // This node does not have an associated document manager and we must fail
            return VSConstants.E_FAIL;
        }


        public virtual int OpenItemWithSpecific(uint itemId, uint editorFlags, ref Guid editorType, string physicalView, ref Guid logicalView, IntPtr docDataExisting, out IVsWindowFrame frame) {
            // Init output params
            frame = null;

            HierarchyNode n = this.NodeFromItemId(itemId);
            if (n == null) {
                throw new ArgumentException(SR.GetString(SR.ParameterMustBeAValidItemId), "itemId");
            }

            // Delegate to the document manager object that knows how to open the item
            DocumentManager documentManager = n.GetDocumentManager();
            if (documentManager != null) {
                return documentManager.OpenWithSpecific(editorFlags, ref editorType, physicalView, ref logicalView, docDataExisting, out frame, WindowFrameShowAction.DoNotShow);
            }

            // This node does not have an associated document manager and we must fail
            return VSConstants.E_FAIL;
        }


        public virtual int RemoveItem(uint reserved, uint itemId, out int result) {
            HierarchyNode n = this.NodeFromItemId(itemId);
            if (n == null) {
                throw new ArgumentException(SR.GetString(SR.ParameterMustBeAValidItemId), "itemId");
            }
            n.Remove(true);
            result = 1;
            return VSConstants.S_OK;
        }


        public virtual int ReopenItem(uint itemId, ref Guid editorType, string physicalView, ref Guid logicalView, IntPtr docDataExisting, out IVsWindowFrame frame) {
            // Init output params
            frame = null;

            HierarchyNode n = this.NodeFromItemId(itemId);
            if (n == null) {
                throw new ArgumentException(SR.GetString(SR.ParameterMustBeAValidItemId), "itemId");
            }

            // Delegate to the document manager object that knows how to open the item
            DocumentManager documentManager = n.GetDocumentManager();
            if (documentManager != null) {
                return documentManager.ReOpenWithSpecific(0, ref editorType, physicalView, ref logicalView, docDataExisting, out frame, WindowFrameShowAction.DoNotShow);
            }

            // This node does not have an associated document manager and we must fail
            return VSConstants.E_FAIL;
        }


        /// <summary>
        /// Implements IVsProject3::TransferItem
        /// This function is called when an open miscellaneous file is being transferred
        /// to our project. The sequence is for the shell to call AddItemWithSpecific and
        /// then use TransferItem to transfer the open document to our project.
        /// </summary>
        /// <param name="oldMkDoc">Old document name</param>
        /// <param name="newMkDoc">New document name</param>
        /// <param name="frame">Optional frame if the document is open</param>
        /// <returns></returns>
        public virtual int TransferItem(string oldMkDoc, string newMkDoc, IVsWindowFrame frame) {
            // Fail if hierarchy already closed
            if (this.ProjectMgr == null || this.IsClosed) {
                return VSConstants.E_FAIL;
            }
            //Fail if the document names passed are null.
            if (oldMkDoc == null || newMkDoc == null)
                return VSConstants.E_INVALIDARG;

            int hr = VSConstants.S_OK;
            VSDOCUMENTPRIORITY[] priority = new VSDOCUMENTPRIORITY[1];
            uint itemid = VSConstants.VSITEMID_NIL;
            uint cookie = 0;
            uint grfFlags = 0;

            IVsRunningDocumentTable pRdt = GetService(typeof(IVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (pRdt == null)
                return VSConstants.E_ABORT;

            string doc;
            int found;
            IVsHierarchy pHier;
            uint id, readLocks, editLocks;
            IntPtr docdataForCookiePtr = IntPtr.Zero;
            IntPtr docDataPtr = IntPtr.Zero;
            IntPtr hierPtr = IntPtr.Zero;

            // We get the document from the running doc table so that we can see if it is transient
            try {
                ErrorHandler.ThrowOnFailure(pRdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock, oldMkDoc, out pHier, out id, out docdataForCookiePtr, out cookie));
            } finally {
                if (docdataForCookiePtr != IntPtr.Zero)
                    Marshal.Release(docdataForCookiePtr);
            }

            //Get the document info
            try {
                ErrorHandler.ThrowOnFailure(pRdt.GetDocumentInfo(cookie, out grfFlags, out readLocks, out editLocks, out doc, out pHier, out id, out docDataPtr));
            } finally {
                if (docDataPtr != IntPtr.Zero)
                    Marshal.Release(docDataPtr);
            }

            // Now see if the document is in the project. If not, we fail
            try {
                ErrorHandler.ThrowOnFailure(IsDocumentInProject(newMkDoc, out found, priority, out itemid));
                Debug.Assert(itemid != VSConstants.VSITEMID_NIL && itemid != VSConstants.VSITEMID_ROOT);
                hierPtr = Marshal.GetComInterfaceForObject(this, typeof(IVsUIHierarchy));
                // Now rename the document
                ErrorHandler.ThrowOnFailure(pRdt.RenameDocument(oldMkDoc, newMkDoc, hierPtr, itemid));
            } finally {
                if (hierPtr != IntPtr.Zero)
                    Marshal.Release(hierPtr);
            }

            //Change the caption if we are passed a window frame
            if (frame != null) {
                var newNode = FindNodeByFullPath(newMkDoc);

                if (newNode != null) {
                    string caption = newNode.Caption;
                    hr = frame.SetProperty((int)(__VSFPROPID.VSFPROPID_OwnerCaption), caption);
                }
            }
            return hr;
        }

        #endregion

        #region IVsDependencyProvider Members
        public int EnumDependencies(out IVsEnumDependencies enumDependencies) {
            enumDependencies = new EnumDependencies(this.buildDependencyList);
            return VSConstants.S_OK;
        }

        public int OpenDependency(string szDependencyCanonicalName, out IVsDependency dependency) {
            dependency = null;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsComponentUser methods

        /// <summary>
        /// Add Components to the Project.
        /// Used by the environment to add components specified by the user in the Component Selector dialog 
        /// to the specified project
        /// </summary>
        /// <param name="dwAddCompOperation">The component operation to be performed.</param>
        /// <param name="cComponents">Number of components to be added</param>
        /// <param name="rgpcsdComponents">array of component selector data</param>
        /// <param name="hwndDialog">Handle to the component picker dialog</param>
        /// <param name="pResult">Result to be returned to the caller</param>
        public virtual int AddComponent(VSADDCOMPOPERATION dwAddCompOperation, uint cComponents, System.IntPtr[] rgpcsdComponents, System.IntPtr hwndDialog, VSADDCOMPRESULT[] pResult) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            if (rgpcsdComponents == null || pResult == null) {
                return VSConstants.E_FAIL;
            }

            //initalize the out parameter
            pResult[0] = VSADDCOMPRESULT.ADDCOMPRESULT_Success;

            IReferenceContainer references = GetReferenceContainer();
            if (null == references) {
                // This project does not support references or the reference container was not created.
                // In both cases this operation is not supported.
                return VSConstants.E_NOTIMPL;
            }
            for (int cCount = 0; cCount < cComponents; cCount++) {
                VSCOMPONENTSELECTORDATA selectorData = new VSCOMPONENTSELECTORDATA();
                IntPtr ptr = rgpcsdComponents[cCount];
                selectorData = (VSCOMPONENTSELECTORDATA)Marshal.PtrToStructure(ptr, typeof(VSCOMPONENTSELECTORDATA));
                if (null == references.AddReferenceFromSelectorData(selectorData)) {
                    //Skip further proccessing since a reference has to be added
                    pResult[0] = VSADDCOMPRESULT.ADDCOMPRESULT_Failure;
                    return VSConstants.S_OK;
                }
            }
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSccProject2 Members
        /// <summary>
        /// This method is called to determine which files should be placed under source control for a given VSITEMID within this hierarchy.
        /// </summary>
        /// <param name="itemid">Identifier for the VSITEMID being queried.</param>
        /// <param name="stringsOut">Pointer to an array of CALPOLESTR strings containing the file names for this item.</param>
        /// <param name="flagsOut">Pointer to a CADWORD array of flags stored in DWORDs indicating that some of the files have special behaviors.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int GetSccFiles(uint itemid, CALPOLESTR[] stringsOut, CADWORD[] flagsOut) {
            if (itemid == VSConstants.VSITEMID_SELECTION) {
                throw new ArgumentException(SR.GetString(SR.InvalidParameter), "itemid");
            } else if (itemid == VSConstants.VSITEMID_ROOT) {
                // Root node.  Return our project file path.
                if (stringsOut != null && stringsOut.Length > 0) {
                    stringsOut[0] = Utilities.CreateCALPOLESTR(new[] { filename });
                }

                if (flagsOut != null && flagsOut.Length > 0) {
                    flagsOut[0] = Utilities.CreateCADWORD(new[] { tagVsSccFilesFlags.SFF_NoFlags });
                }
                return VSConstants.S_OK;
            }

            // otherwise delegate to either a file or a folder to get the SCC files
            HierarchyNode n = this.NodeFromItemId(itemid);
            if (n == null) {
                throw new ArgumentException(SR.GetString(SR.InvalidParameter), "itemid");
            }

            List<string> files = new List<string>();
            List<tagVsSccFilesFlags> flags = new List<tagVsSccFilesFlags>();

            n.GetSccFiles(files, flags);

            if (stringsOut != null && stringsOut.Length > 0) {
                stringsOut[0] = Utilities.CreateCALPOLESTR(files);
            }

            if (flagsOut != null && flagsOut.Length > 0) {
                flagsOut[0] = Utilities.CreateCADWORD(flags);
            }

            return VSConstants.S_OK;
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

        /// <summary>
        /// This method is called to discover special (hidden files) associated with a given VSITEMID within this hierarchy. 
        /// </summary>
        /// <param name="itemid">Identifier for the VSITEMID being queried.</param>
        /// <param name="sccFile">One of the files associated with the node</param>
        /// <param name="stringsOut">Pointer to an array of CALPOLESTR strings containing the file names for this item.</param>
        /// <param name="flagsOut">Pointer to a CADWORD array of flags stored in DWORDs indicating that some of the files have special behaviors.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        /// <remarks>This method is called to discover any special or hidden files associated with an item in the project hierarchy. It is called when GetSccFiles returns with the SFF_HasSpecialFiles flag set for any of the files associated with the node.</remarks>
        public virtual int GetSccSpecialFiles(uint itemid, string sccFile, CALPOLESTR[] stringsOut, CADWORD[] flagsOut) {
            if (itemid == VSConstants.VSITEMID_SELECTION) {
                throw new ArgumentException(SR.GetString(SR.InvalidParameter), "itemid");
            }

            HierarchyNode n = this.NodeFromItemId(itemid);
            if (n == null) {
                throw new ArgumentException(SR.GetString(SR.InvalidParameter), "itemid");
            }

            List<string> files = new List<string>();

            List<tagVsSccFilesFlags> flags = new List<tagVsSccFilesFlags>();

            n.GetSccSpecialFiles(sccFile, files, flags);

            if (stringsOut != null && stringsOut.Length > 0) {
                stringsOut[0] = Utilities.CreateCALPOLESTR(files);
            }

            if (flagsOut != null && flagsOut.Length > 0) {
                flagsOut[0] = Utilities.CreateCADWORD(flags);
            }

            // we have no special files.
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This method is called by the source control portion of the environment to inform the project of changes to the source control glyph on various nodes. 
        /// </summary>
        /// <param name="affectedNodes">Count of changed nodes.</param>
        /// <param name="itemidAffectedNodes">An array of VSITEMID identifiers of the changed nodes.</param>
        /// <param name="newGlyphs">An array of VsStateIcon glyphs representing the new state of the corresponding item in rgitemidAffectedNodes.</param>
        /// <param name="newSccStatus">An array of status flags from SccStatus corresponding to rgitemidAffectedNodes. </param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int SccGlyphChanged(int affectedNodes, uint[] itemidAffectedNodes, VsStateIcon[] newGlyphs, uint[] newSccStatus) {
            // if all the paramaters are null adn the count is 0, it means scc wants us to updated everything
            if (affectedNodes == 0 && itemidAffectedNodes == null && newGlyphs == null && newSccStatus == null) {
                ReDrawNode(this, UIHierarchyElement.SccState);
                this.UpdateSccStateIcons();
            } else if (affectedNodes > 0 && itemidAffectedNodes != null && newGlyphs != null && newSccStatus != null) {
                for (int i = 0; i < affectedNodes; i++) {
                    HierarchyNode n = this.NodeFromItemId(itemidAffectedNodes[i]);
                    if (n == null) {
                        throw new ArgumentException(SR.GetString(SR.InvalidParameter), "itemidAffectedNodes");
                    }

                    ReDrawNode(n, UIHierarchyElement.SccState);
                }
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This method is called by the source control portion of the environment when a project is initially added to source control, or to change some of the project's settings.
        /// </summary>
        /// <param name="sccProjectName">String, opaque to the project, that identifies the project location on the server. Persist this string in the project file. </param>
        /// <param name="sccLocalPath">String, opaque to the project, that identifies the path to the server. Persist this string in the project file.</param>
        /// <param name="sccAuxPath">String, opaque to the project, that identifies the local path to the project. Persist this string in the project file.</param>
        /// <param name="sccProvider">String, opaque to the project, that identifies the source control package. Persist this string in the project file.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int SetSccLocation(string sccProjectName, string sccAuxPath, string sccLocalPath, string sccProvider) {
            Utilities.ArgumentNotNull("sccProjectName", sccProjectName);
            Utilities.ArgumentNotNull("sccAuxPath", sccAuxPath);
            Utilities.ArgumentNotNull("sccLocalPath", sccLocalPath);
            Utilities.ArgumentNotNull("sccProvider", sccProvider);

            // Save our settings (returns true if something changed)
            if (!SetSccSettings(sccProjectName, sccLocalPath, sccAuxPath, sccProvider)) {
                return VSConstants.S_OK;
            }

            bool unbinding = (sccProjectName.Length == 0 && sccProvider.Length == 0);

            if (unbinding || QueryEditProjectFile(false)) {
                buildProject.SetProperty(ProjectFileConstants.SccProjectName, sccProjectName);
                buildProject.SetProperty(ProjectFileConstants.SccProvider, sccProvider);
                buildProject.SetProperty(ProjectFileConstants.SccAuxPath, sccAuxPath);
                buildProject.SetProperty(ProjectFileConstants.SccLocalPath, sccLocalPath);
            }

            isRegisteredWithScc = true;

            return VSConstants.S_OK;
        }
        #endregion

        #region IVsProjectSpecialFiles Members
        /// <summary>
        /// Allows you to query the project for special files and optionally create them. 
        /// </summary>
        /// <param name="fileId">__PSFFILEID of the file</param>
        /// <param name="flags">__PSFFLAGS flags for the file</param>
        /// <param name="itemid">The itemid of the node in the hierarchy</param>
        /// <param name="fileName">The file name of the special file.</param>
        /// <returns></returns>
        public virtual int GetFile(int fileId, uint flags, out uint itemid, out string fileName) {
            itemid = VSConstants.VSITEMID_NIL;
            fileName = String.Empty;

            // We need to return S_OK, otherwise the property page tabs will not be shown.
            return VSConstants.E_NOTIMPL;
        }
        #endregion

        #region IAggregatedHierarchy Members

        /// <summary>
        /// Get the inner object of an aggregated hierarchy
        /// </summary>
        /// <returns>A HierarchyNode</returns>
        public virtual HierarchyNode GetInner() {
            return this;
        }

        #endregion

        #region IReferenceDataProvider Members
        /// <summary>
        /// Returns the reference container node.
        /// </summary>
        /// <returns></returns>
        public IReferenceContainer GetReferenceContainer() {
            return FindImmediateChild(node => node is IReferenceContainer) as IReferenceContainer;
        }

        #endregion

        #region IBuildDependencyUpdate Members

        public virtual IVsBuildDependency[] BuildDependencies {
            get {
                return this.buildDependencyList.ToArray();
            }
        }

        public virtual void AddBuildDependency(IVsBuildDependency dependency) {
            if (this.isClosed || dependency == null) {
                return;
            }

            if (!this.buildDependencyList.Contains(dependency)) {
                this.buildDependencyList.Add(dependency);
            }
        }

        public virtual void RemoveBuildDependency(IVsBuildDependency dependency) {
            if (this.isClosed || dependency == null) {
                return;
            }

            if (this.buildDependencyList.Contains(dependency)) {
                this.buildDependencyList.Remove(dependency);
            }
        }

        #endregion

        #region IProjectEventsListener Members
        public bool IsProjectEventsListener {
            get { return this.isProjectEventsListener; }
            set { this.isProjectEventsListener = value; }
        }
        #endregion

        #region IVsAggregatableProject Members

        /// <summary>
        /// Retrieve the list of project GUIDs that are aggregated together to make this project.
        /// </summary>
        /// <param name="projectTypeGuids">Semi colon separated list of Guids. Typically, the last GUID would be the GUID of the base project factory</param>
        /// <returns>HResult</returns>
        public int GetAggregateProjectTypeGuids(out string projectTypeGuids) {
            projectTypeGuids = this.GetProjectProperty(ProjectFileConstants.ProjectTypeGuids, false);
            // In case someone manually removed this from our project file, default to our project without flavors
            if (String.IsNullOrEmpty(projectTypeGuids))
                projectTypeGuids = this.ProjectGuid.ToString("B");
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This is where the initialization occurs.
        /// </summary>
        public virtual int InitializeForOuter(string filename, string location, string name, uint flags, ref Guid iid, out IntPtr projectPointer, out int canceled) {
            canceled = 0;
            projectPointer = IntPtr.Zero;

            // Initialize the project
            this.Load(filename, location, name, flags, ref iid, out canceled);

            if (canceled != 1) {
                // Set ourself as the project
                return Marshal.QueryInterface(Marshal.GetIUnknownForObject(this), ref iid, out projectPointer);
            }

            return VSConstants.OLE_E_PROMPTSAVECANCELLED;
        }

        /// <summary>
        /// This is called after the project is done initializing the different layer of the aggregations
        /// </summary>
        /// <returns>HResult</returns>
        public virtual int OnAggregationComplete() {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Set the list of GUIDs that are aggregated together to create this project.
        /// </summary>
        /// <param name="projectTypeGuids">Semi-colon separated list of GUIDs, the last one is usually the project factory of the base project factory</param>
        /// <returns>HResult</returns>
        public int SetAggregateProjectTypeGuids(string projectTypeGuids) {
            this.SetProjectProperty(ProjectFileConstants.ProjectTypeGuids, projectTypeGuids);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// We are always the inner most part of the aggregation
        /// and as such we don't support setting an inner project
        /// </summary>
        public int SetInnerProject(object innerProject) {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsProjectFlavorCfgProvider Members

        int IVsProjectFlavorCfgProvider.CreateProjectFlavorCfg(IVsCfg pBaseProjectCfg, out IVsProjectFlavorCfg ppFlavorCfg) {
            // Our config object is also our IVsProjectFlavorCfg object
            ppFlavorCfg = pBaseProjectCfg as IVsProjectFlavorCfg;

            return VSConstants.S_OK;
        }

        #endregion

        #region IVsBuildPropertyStorage Members

        /// <summary>
        /// Get the property of an item
        /// </summary>
        /// <param name="item">ItemID</param>
        /// <param name="attributeName">Name of the property</param>
        /// <param name="attributeValue">Value of the property (out parameter)</param>
        /// <returns>HRESULT</returns>
        int IVsBuildPropertyStorage.GetItemAttribute(uint item, string attributeName, out string attributeValue) {
            attributeValue = null;

            HierarchyNode node = NodeFromItemId(item);
            if (node == null)
                throw new ArgumentException("Invalid item id", "item");

            if (node.ItemNode != null) {
                attributeValue = node.ItemNode.GetMetadata(attributeName);
            } else if (node == node.ProjectMgr) {
                attributeName = node.ProjectMgr.GetProjectProperty(attributeName);
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Get the value of the property in the project file
        /// </summary>
        /// <param name="propertyName">Name of the property to remove</param>
        /// <param name="configName">Configuration for which to remove the property</param>
        /// <param name="storage">Project or user file (_PersistStorageType)</param>
        /// <param name="propertyValue">Value of the property (out parameter)</param>
        /// <returns>HRESULT</returns>
        int IVsBuildPropertyStorage.GetPropertyValue(string propertyName, string configName, uint storage, out string propertyValue) {
            // TODO: when adding support for User files, we need to update this method
            propertyValue = null;
            if (string.IsNullOrEmpty(configName)) {
                propertyValue = this.GetProjectProperty(propertyName, false);
            } else {
                IVsCfg configurationInterface;
                int platformStart;
                if ((platformStart = configName.IndexOf('|')) != -1) {
                    // matches C# project system, GetPropertyValue handles display name, not just config name
                    configName = configName.Substring(0, platformStart);
                }
                ErrorHandler.ThrowOnFailure(this.ConfigProvider.GetCfgOfName(configName, string.Empty, out configurationInterface));
                ProjectConfig config = (ProjectConfig)configurationInterface;
                propertyValue = config.GetConfigurationProperty(propertyName, true);
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Delete a property
        /// In our case this simply mean defining it as null
        /// </summary>
        /// <param name="propertyName">Name of the property to remove</param>
        /// <param name="configName">Configuration for which to remove the property</param>
        /// <param name="storage">Project or user file (_PersistStorageType)</param>
        /// <returns>HRESULT</returns>
        int IVsBuildPropertyStorage.RemoveProperty(string propertyName, string configName, uint storage) {
            return ((IVsBuildPropertyStorage)this).SetPropertyValue(propertyName, configName, storage, null);
        }

        /// <summary>
        /// Set a property on an item
        /// </summary>
        /// <param name="item">ItemID</param>
        /// <param name="attributeName">Name of the property</param>
        /// <param name="attributeValue">New value for the property</param>
        /// <returns>HRESULT</returns>
        int IVsBuildPropertyStorage.SetItemAttribute(uint item, string attributeName, string attributeValue) {
            HierarchyNode node = NodeFromItemId(item);

            if (node == null)
                throw new ArgumentException("Invalid item id", "item");

            node.ItemNode.SetMetadata(attributeName, attributeValue);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Set a project property
        /// </summary>
        /// <param name="propertyName">Name of the property to set</param>
        /// <param name="configName">Configuration for which to set the property</param>
        /// <param name="storage">Project file or user file (_PersistStorageType)</param>
        /// <param name="propertyValue">New value for that property</param>
        /// <returns>HRESULT</returns>
        int IVsBuildPropertyStorage.SetPropertyValue(string propertyName, string configName, uint storage, string propertyValue) {
            // TODO: when adding support for User files, we need to update this method
            if (string.IsNullOrEmpty(configName)) {
                this.SetProjectProperty(propertyName, propertyValue);
            } else {
                IVsCfg configurationInterface;
                ErrorHandler.ThrowOnFailure(this.ConfigProvider.GetCfgOfName(configName, string.Empty, out configurationInterface));
                ProjectConfig config = (ProjectConfig)configurationInterface;
                config.SetConfigurationProperty(propertyName, propertyValue);
            }
            return VSConstants.S_OK;
        }

        #endregion

        #region private helper methods

        /// <summary>
        /// Initialize projectNode
        /// </summary>
        private void Initialize() {
            this.ID = VSConstants.VSITEMID_ROOT;
            this.tracker = new TrackDocumentsHelper(this);
        }

        /// <summary>
        /// Add an item to the hierarchy based on the item path
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>Added node</returns>
        private HierarchyNode AddIndependentFileNode(MSBuild.ProjectItem item, HierarchyNode parent) {
            return AddFileNodeToNode(item, parent);
        }

        /// <summary>
        /// Add a dependent file node to the hierarchy
        /// </summary>
        /// <param name="item">msbuild item to add</param>
        /// <param name="parentNode">Parent Node</param>
        /// <returns>Added node</returns>
        private HierarchyNode AddDependentFileNodeToNode(MSBuild.ProjectItem item, HierarchyNode parentNode) {
            FileNode node = this.CreateDependentFileNode(new MsBuildProjectElement(this, item));
            parentNode.AddChild(node);

            // Make sure to set the HasNameRelation flag on the dependent node if it is related to the parent by name
            if (!node.HasParentNodeNameRelation && string.Compare(node.GetRelationalName(), parentNode.GetRelationalName(), StringComparison.OrdinalIgnoreCase) == 0) {
                node.HasParentNodeNameRelation = true;
            }

            return node;
        }

        /// <summary>
        /// Add a file node to the hierarchy
        /// </summary>
        /// <param name="item">msbuild item to add</param>
        /// <param name="parentNode">Parent Node</param>
        /// <returns>Added node</returns>
        private HierarchyNode AddFileNodeToNode(MSBuild.ProjectItem item, HierarchyNode parentNode) {
            FileNode node = this.CreateFileNode(new MsBuildProjectElement(this, item));
            parentNode.AddChild(node);
            return node;
        }

        /// <summary>
        /// Get the parent node of an msbuild item
        /// </summary>
        /// <param name="item">msbuild item</param>
        /// <returns>parent node</returns>
        internal HierarchyNode GetItemParentNode(MSBuild.ProjectItem item) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            var link = item.GetMetadataValue(ProjectFileConstants.Link);
            HierarchyNode currentParent = this;
            string strPath = item.EvaluatedInclude;

            if (!String.IsNullOrWhiteSpace(link)) {
                strPath = Path.GetDirectoryName(link);
            } else {
                HierarchyNode parent;
                if (_diskNodes.TryGetValue(Path.GetDirectoryName(Path.Combine(ProjectHome, strPath)) + "\\", out parent)) {
                    // fast path, filename is normalized, and the folder already exists
                    return parent;
                }

                string absPath = CommonUtils.GetAbsoluteFilePath(ProjectHome, strPath);
                if (CommonUtils.IsSubpathOf(ProjectHome, absPath)) {
                    strPath = CommonUtils.GetRelativeDirectoryPath(ProjectHome, Path.GetDirectoryName(absPath));
                } else {
                    // file lives outside of the project, w/o a link it's just at the top level.
                    return this;
                }
            }

            if (strPath.Length > 0) {
                // Use the relative to verify the folders...
                currentParent = this.CreateFolderNodes(strPath);
            }
            return currentParent;
        }

        private MSBuildExecution.ProjectPropertyInstance GetMsBuildProperty(string propertyName, bool resetCache) {
            if (isDisposed) {
                throw new ObjectDisposedException(null);
            }

            if (resetCache || this.currentConfig == null) {
                // Get properties from project file and cache it
                this.SetCurrentConfiguration();
                this.currentConfig = this.buildProject.CreateProjectInstance();
            }

            if (this.currentConfig == null) {
                throw new Exception(SR.GetString(SR.FailedToRetrieveProperties, propertyName));
            }

            // return property asked for
            return this.currentConfig.GetProperty(propertyName);
        }

        /// <summary>
        /// Updates our scc project settings. 
        /// </summary>
        /// <param name="sccProjectName">String, opaque to the project, that identifies the project location on the server. Persist this string in the project file. </param>
        /// <param name="sccLocalPath">String, opaque to the project, that identifies the path to the server. Persist this string in the project file.</param>
        /// <param name="sccAuxPath">String, opaque to the project, that identifies the local path to the project. Persist this string in the project file.</param>
        /// <param name="sccProvider">String, opaque to the project, that identifies the source control package. Persist this string in the project file.</param>
        /// <returns>Returns true if something changed.</returns>
        private bool SetSccSettings(string sccProjectName, string sccLocalPath, string sccAuxPath, string sccProvider) {
            bool changed = false;
            Debug.Assert(sccProjectName != null && sccLocalPath != null && sccAuxPath != null && sccProvider != null);
            if (String.Compare(sccProjectName, this.sccProjectName, StringComparison.OrdinalIgnoreCase) != 0 ||
                String.Compare(sccLocalPath, this.sccLocalPath, StringComparison.OrdinalIgnoreCase) != 0 ||
                String.Compare(sccAuxPath, this.sccAuxPath, StringComparison.OrdinalIgnoreCase) != 0 ||
                String.Compare(sccProvider, this.sccProvider, StringComparison.OrdinalIgnoreCase) != 0) {
                changed = true;
                this.sccProjectName = sccProjectName;
                this.sccLocalPath = sccLocalPath;
                this.sccAuxPath = sccAuxPath;
                this.sccProvider = sccProvider;
            }


            return changed;
        }

        /// <summary>
        /// Sets the scc info from the project file.
        /// </summary>
        private void InitSccInfo() {
            this.sccProjectName = this.GetProjectProperty(ProjectFileConstants.SccProjectName, false);
            this.sccLocalPath = this.GetProjectProperty(ProjectFileConstants.SccLocalPath, false);
            this.sccProvider = this.GetProjectProperty(ProjectFileConstants.SccProvider, false);
            this.sccAuxPath = this.GetProjectProperty(ProjectFileConstants.SccAuxPath, false);
        }

        internal void OnAfterProjectOpen() {
            this.projectOpened = true;
        }

        private static XmlElement WrapXmlFragment(XmlDocument document, XmlElement root, Guid flavor, string configuration, string platform, string fragment) {
            XmlElement node = document.CreateElement(ProjectFileConstants.FlavorProperties);
            XmlAttribute attribute = document.CreateAttribute(ProjectFileConstants.Guid);
            attribute.Value = flavor.ToString("B");
            node.Attributes.Append(attribute);
            if (!String.IsNullOrEmpty(configuration)) {
                attribute = document.CreateAttribute(ProjectFileConstants.Configuration);
                attribute.Value = configuration;
                node.Attributes.Append(attribute);
                attribute = document.CreateAttribute(ProjectFileConstants.Platform);
                attribute.Value = platform;
                node.Attributes.Append(attribute);
            }
            node.InnerXml = fragment;
            root.AppendChild(node);
            return node;
        }

        /// <summary>
        /// Sets the project guid from the project file. If no guid is found a new one is created and assigne for the instance project guid.
        /// </summary>
        private void SetProjectGuidFromProjectFile() {
            string projectGuid = this.GetProjectProperty(ProjectFileConstants.ProjectGuid, false);
            if (String.IsNullOrEmpty(projectGuid)) {
                this.projectIdGuid = Guid.NewGuid();
            } else {
                Guid guid = new Guid(projectGuid);
                if (guid != this.projectIdGuid) {
                    this.projectIdGuid = guid;
                }
            }
        }

        /// <summary>
        /// Helper for sharing common code between Build() and BuildAsync()
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool BuildPrelude(IVsOutputWindowPane output) {
            bool engineLogOnlyCritical = false;
            // If there is some output, then we can ask the build engine to log more than
            // just the critical events.
            if (null != output) {
                engineLogOnlyCritical = BuildEngine.OnlyLogCriticalEvents;
                BuildEngine.OnlyLogCriticalEvents = false;
            }

            this.SetOutputLogger(output);
            return engineLogOnlyCritical;
        }

        /// <summary>
        /// Recusively parses the tree and closes all nodes.
        /// </summary>
        /// <param name="node">The subtree to close.</param>
        private static void CloseAllNodes(HierarchyNode node) {
            for (HierarchyNode n = node.FirstChild; n != null; n = n.NextSibling) {
                if (n.FirstChild != null) {
                    CloseAllNodes(n);
                }

                n.Close();
            }
        }

        /// <summary>
        /// Set the build project with the new project instance value
        /// </summary>
        /// <param name="project">The new build project instance</param>
        private void SetBuildProject(MSBuild.Project project) {
            bool isNewBuildProject = (this.buildProject != project);
            this.buildProject = project;
            if (this.buildProject != null) {
                SetupProjectGlobalPropertiesThatAllProjectSystemsMustSet();
            }
            if (isNewBuildProject) {
                NewBuildProject(project);
            }
        }

        /// <summary>
        /// Called when a new value for <see cref="BuildProject"/> is available.
        /// </summary>
        protected virtual void NewBuildProject(MSBuild.Project project) { }

        /// <summary>
        /// Setup the global properties for project instance.
        /// </summary>
        private void SetupProjectGlobalPropertiesThatAllProjectSystemsMustSet() {
            string solutionDirectory = null;
            string solutionFile = null;
            string userOptionsFile = null;

            IVsSolution solution = this.Site.GetService(typeof(SVsSolution)) as IVsSolution;
            if (solution != null) {
                // We do not want to throw. If we cannot set the solution related constants we set them to empty string.
                solution.GetSolutionInfo(out solutionDirectory, out solutionFile, out userOptionsFile);
            }

            if (solutionDirectory == null) {
                solutionDirectory = String.Empty;
            }

            if (solutionFile == null) {
                solutionFile = String.Empty;
            }

            string solutionFileName = Path.GetFileName(solutionFile);
            string solutionName = Path.GetFileNameWithoutExtension(solutionFile);
            string solutionExtension = Path.GetExtension(solutionFile);

            this.buildProject.SetGlobalProperty(GlobalProperty.SolutionDir.ToString(), solutionDirectory);
            this.buildProject.SetGlobalProperty(GlobalProperty.SolutionPath.ToString(), solutionFile);
            this.buildProject.SetGlobalProperty(GlobalProperty.SolutionFileName.ToString(), solutionFileName);
            this.buildProject.SetGlobalProperty(GlobalProperty.SolutionName.ToString(), solutionName);
            this.buildProject.SetGlobalProperty(GlobalProperty.SolutionExt.ToString(), solutionExtension);

            // Other misc properties
            this.buildProject.SetGlobalProperty(GlobalProperty.BuildingInsideVisualStudio.ToString(), "true");
            this.buildProject.SetGlobalProperty(GlobalProperty.Configuration.ToString(), ProjectConfig.Debug);
            this.buildProject.SetGlobalProperty(GlobalProperty.Platform.ToString(), ProjectConfig.AnyCPU);

            // DevEnvDir property
            object installDirAsObject = null;

            IVsShell shell = this.Site.GetService(typeof(SVsShell)) as IVsShell;
            if (shell != null) {
                // We do not want to throw. If we cannot set the solution related constants we set them to empty string.
                shell.GetProperty((int)__VSSPROPID.VSSPROPID_InstallDirectory, out installDirAsObject);
            }

            // Ensure that we have traimnling backslash as this is done for the langproj macros too.
            string installDir = CommonUtils.NormalizeDirectoryPath((string)installDirAsObject) ?? String.Empty;

            this.buildProject.SetGlobalProperty(GlobalProperty.DevEnvDir.ToString(), installDir);
        }


        /// <summary>
        /// Attempts to lock in the privilege of running a build in Visual Studio.
        /// </summary>
        /// <param name="designTime"><c>false</c> if this build was called for by the Solution Build Manager; <c>true</c> otherwise.</param>
        /// <param name="requiresUIThread">
        /// Need to claim the UI thread for build under the following conditions:
        /// 1. The build must use a resource that uses the UI thread, such as
        /// - you set HostServices and you have a host object which requires (even indirectly) the UI thread (VB and C# compilers do this for instance.)
        /// or,
        /// 2. The build requires the in-proc node AND waits on the UI thread for the build to complete, such as:
        /// - you use a ProjectInstance to build, or
        /// - you have specified a host object, whether or not it requires the UI thread, or
        /// - you set HostServices and you have specified a node affinity.
        /// - In addition to the above you also call submission.Execute(), or you call submission.ExecuteAsync() and then also submission.WaitHandle.Wait*().
        /// </param>
        /// <returns>A value indicating whether a build may proceed.</returns>
        /// <remarks>
        /// This method must be called on the UI thread.
        /// </remarks>
        private bool TryBeginBuild(bool designTime, bool requiresUIThread = false) {
            IVsBuildManagerAccessor accessor = null;

            if (this.Site != null) {
                accessor = this.Site.GetService(typeof(SVsBuildManagerAccessor)) as IVsBuildManagerAccessor;
            }

            bool releaseUIThread = false;

            try {
                // If the SVsBuildManagerAccessor service is absent, we're not running within Visual Studio.
                if (accessor != null) {
                    if (requiresUIThread) {
                        int result = accessor.ClaimUIThreadForBuild();
                        if (result < 0) {
                            // Not allowed to claim the UI thread right now. Try again later.
                            return false;
                        }

                        releaseUIThread = true; // assume we need to release this immediately until we get through the whole gauntlet.
                    }

                    if (designTime) {
                        int result = accessor.BeginDesignTimeBuild();
                        if (result < 0) {
                            // Not allowed to begin a design-time build at this time. Try again later.
                            return false;
                        }
                    }

                    // We obtained all the resources we need.  So don't release the UI thread until after the build is finished.
                    releaseUIThread = false;
                } else {
                    BuildParameters buildParameters = new BuildParameters(this.buildEngine);
                    BuildManager.DefaultBuildManager.BeginBuild(buildParameters);
                }

                this.buildInProcess = true;
                return true;
            } finally {
                // If we were denied the privilege of starting a design-time build,
                // we need to release the UI thread.
                if (releaseUIThread) {
                    Debug.Assert(accessor != null, "We think we need to release the UI thread for an accessor we don't have!");
                    accessor.ReleaseUIThreadForBuild();
                }
            }
        }

        /// <summary>
        /// Lets Visual Studio know that we're done with our design-time build so others can use the build manager.
        /// </summary>
        /// <param name="submission">The build submission that built, if any.</param>
        /// <param name="designTime">This must be the same value as the one passed to <see cref="TryBeginBuild"/>.</param>
        /// <param name="requiresUIThread">This must be the same value as the one passed to <see cref="TryBeginBuild"/>.</param>
        /// <remarks>
        /// This method must be called on the UI thread.
        /// </remarks>
        private void EndBuild(BuildSubmission submission, bool designTime, bool requiresUIThread = false) {
            IVsBuildManagerAccessor accessor = null;

            if (this.Site != null) {
                accessor = this.Site.GetService(typeof(SVsBuildManagerAccessor)) as IVsBuildManagerAccessor;
            }

            if (accessor != null) {
                // It's very important that we try executing all three end-build steps, even if errors occur partway through.
                try {
                    if (submission != null) {
                        Marshal.ThrowExceptionForHR(accessor.UnregisterLoggers(submission.SubmissionId));
                    }
                } catch (Exception ex) {
                    if (ex.IsCriticalException()) {
                        throw;
                    }

                    Trace.TraceError(ex.ToString());
                }

                try {
                    if (designTime) {
                        Marshal.ThrowExceptionForHR(accessor.EndDesignTimeBuild());
                    }
                } catch (Exception ex) {
                    if (ex.IsCriticalException()) {
                        throw;
                    }

                    Trace.TraceError(ex.ToString());
                }


                try {
                    if (requiresUIThread) {
                        Marshal.ThrowExceptionForHR(accessor.ReleaseUIThreadForBuild());
                    }
                } catch (Exception ex) {
                    if (ex.IsCriticalException()) {
                        throw;
                    }

                    Trace.TraceError(ex.ToString());
                }
            } else {
                BuildManager.DefaultBuildManager.EndBuild();
            }

            this.buildInProcess = false;
        }

        #endregion

        #region IProjectEventsCallback Members

        public virtual void BeforeClose() {
        }

        #endregion

        #region IVsProjectBuildSystem Members

        public virtual int SetHostObject(string targetName, string taskName, object hostObject) {
            Debug.Assert(targetName != null && taskName != null && this.buildProject != null && this.buildProject.Targets != null);

            if (targetName == null || taskName == null || this.buildProject == null || this.buildProject.Targets == null) {
                return VSConstants.E_INVALIDARG;
            }

            this.buildProject.ProjectCollection.HostServices.RegisterHostObject(this.buildProject.FullPath, targetName, taskName, (Microsoft.Build.Framework.ITaskHost)hostObject);

            return VSConstants.S_OK;
        }

        public int BuildTarget(string targetName, out bool success) {
            success = false;

            MSBuildResult result = this.Build(targetName);

            if (result == MSBuildResult.Successful) {
                success = true;
            }

            return VSConstants.S_OK;
        }

        public virtual int CancelBatchEdit() {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int EndBatchEdit() {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int StartBatchEdit() {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Used to determine the kind of build system, in VS 2005 there's only one defined kind: MSBuild 
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public virtual int GetBuildSystemKind(out uint kind) {
            kind = (uint)_BuildSystemKindFlags2.BSK_MSBUILD_VS10;
            return VSConstants.S_OK;
        }

        #endregion

        /// <summary>
        /// Finds a node by it's full path on disk.
        /// </summary>
        internal HierarchyNode FindNodeByFullPath(string name) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            Debug.Assert(Path.IsPathRooted(name));

            HierarchyNode res;
            _diskNodes.TryGetValue(name, out res);
            return res;
        }

        /// <summary>
        /// Gets the parent folder if path were added to be added to the hierarchy
        /// in it's default (non-linked item) location.  Returns null if the path
        /// of parent folders to the item don't exist yet.
        /// </summary>
        /// <param name="path">The full path on disk to the item which is being queried about..</param>
        internal HierarchyNode GetParentFolderForPath(string path) {
            var parentDir = CommonUtils.GetParent(path);
            HierarchyNode parent;
            if (CommonUtils.IsSamePath(parentDir, ProjectHome)) {
                parent = this;
            } else {
                parent = FindNodeByFullPath(parentDir);
            }
            return parent;
        }

        #region IVsUIHierarchy methods

        public virtual int ExecCommand(uint itemId, ref Guid guidCmdGroup, uint nCmdId, uint nCmdExecOpt, IntPtr pvain, IntPtr p) {
            return this.InternalExecCommand(guidCmdGroup, nCmdId, nCmdExecOpt, pvain, p, CommandOrigin.UiHierarchy);
        }

        public virtual int QueryStatusCommand(uint itemId, ref Guid guidCmdGroup, uint cCmds, OLECMD[] cmds, IntPtr pCmdText) {
            return this.QueryStatusSelection(guidCmdGroup, cCmds, cmds, pCmdText, CommandOrigin.UiHierarchy);
        }

        int IVsUIHierarchy.Close() {
            return ((IVsHierarchy)this).Close();
        }

        #endregion

        #region IVsHierarchy methods

        public virtual int AdviseHierarchyEvents(IVsHierarchyEvents sink, out uint cookie) {
            cookie = this._hierarchyEventSinks.Add(sink) + 1;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Closes the project node.
        /// </summary>
        /// <returns>A success or failure value.</returns>
        int IVsHierarchy.Close() {
            int hr = VSConstants.S_OK;
            try {
                Close();
            } catch (COMException e) {
                hr = e.ErrorCode;
            }
            return hr;
        }

        /// <summary>
        /// Sets the service provider from which to access the services. 
        /// </summary>
        /// <param name="site">An instance to an Microsoft.VisualStudio.OLE.Interop object</param>
        /// <returns>A success or failure value.</returns>
        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider site) {
            return VSConstants.S_OK;
        }

        public virtual int GetCanonicalName(uint itemId, out string name) {
            HierarchyNode n = NodeFromItemId(itemId);
            name = (n != null) ? n.GetCanonicalName() : null;
            return VSConstants.S_OK;
        }

        public virtual int GetGuidProperty(uint itemId, int propid, out Guid guid) {
            guid = Guid.Empty;
            HierarchyNode n = NodeFromItemId(itemId);
            if (n != null) {
                int hr = n.GetGuidProperty(propid, out guid);
                __VSHPROPID vspropId = (__VSHPROPID)propid;
                return hr;
            }
            if (guid == Guid.Empty) {
                return VSConstants.DISP_E_MEMBERNOTFOUND;
            }
            return VSConstants.S_OK;
        }


        public virtual int GetProperty(uint itemId, int propId, out object propVal) {
            propVal = null;
            if (itemId != VSConstants.VSITEMID_ROOT && propId == (int)__VSHPROPID.VSHPROPID_IconImgList) {
                return VSConstants.DISP_E_MEMBERNOTFOUND;
            }

            HierarchyNode n = NodeFromItemId(itemId);
            if (n != null) {
                propVal = n.GetProperty(propId);
            }
            if (propVal == null) {
                return VSConstants.DISP_E_MEMBERNOTFOUND;
            }
            return VSConstants.S_OK;
        }

        public virtual int GetNestedHierarchy(uint itemId, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pItemId) {
            ppHierarchyNested = IntPtr.Zero;
            pItemId = 0;
            // If itemid is not a nested hierarchy we must return E_FAIL.
            return VSConstants.E_FAIL;
        }

        public virtual int GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider site) {
            site = Site.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// the canonicalName of an item is it's URL, or better phrased,
        /// the persistence data we put into @RelPath, which is a relative URL
        /// to the root project
        /// returning the itemID from this means scanning the list
        /// </summary>
        /// <param name="name"></param>
        /// <param name="itemId"></param>
        public virtual int ParseCanonicalName(string name, out uint itemId) {
            // we always start at the current node and go it's children down, so 
            //  if you want to scan the whole tree, better call 
            // the root
            name = EnsureRootedPath(name);
            itemId = 0;
            var child = FindNodeByFullPath(name);
            if (child != null) {
                itemId = child.HierarchyId;
                return VSConstants.S_OK;
            }

            return VSConstants.E_FAIL;
        }

        private string EnsureRootedPath(string name) {
            if (!Path.IsPathRooted(name)) {
                name = CommonUtils.GetAbsoluteFilePath(
                    ProjectHome,
                    name
                );
            }
            return name;
        }

        public virtual int QueryClose(out int fCanClose) {
            fCanClose = 1;
            return VSConstants.S_OK;
        }

        public virtual int SetGuidProperty(uint itemId, int propid, ref Guid guid) {
            HierarchyNode n = NodeFromItemId(itemId);
            int rc = VSConstants.E_INVALIDARG;
            if (n != null) {
                rc = n.SetGuidProperty(propid, ref guid);
            }
            return rc;
        }

        public virtual int SetProperty(uint itemId, int propid, object value) {
            HierarchyNode n = NodeFromItemId(itemId);
            if (n != null) {
                return n.SetProperty(propid, value);
            } else {
                return VSConstants.DISP_E_MEMBERNOTFOUND;
            }
        }

        public virtual int UnadviseHierarchyEvents(uint cookie) {
            this._hierarchyEventSinks.RemoveAt(cookie - 1);
            return VSConstants.S_OK;
        }

        public int Unused0() {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused1() {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused2() {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused3() {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused4() {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region Hierarchy change notification

        internal void OnItemAdded(HierarchyNode parent, HierarchyNode child, HierarchyNode previousVisible = null) {
            Utilities.ArgumentNotNull("parent", parent);
            Utilities.ArgumentNotNull("child", child);

            Site.GetUIThread().MustBeCalledFromUIThread();

            IDiskBasedNode diskNode = child as IDiskBasedNode;
            if (diskNode != null) {
                _diskNodes[diskNode.Url] = child;
            }

            if ((EventTriggeringFlag & ProjectNode.EventTriggering.DoNotTriggerHierarchyEvents) != 0) {
                return;
            }

            ExtensibilityEventsDispatcher.FireItemAdded(child);

            HierarchyNode prev = previousVisible ?? child.PreviousVisibleSibling;
            uint prevId = (prev != null) ? prev.HierarchyId : VSConstants.VSITEMID_NIL;
            foreach (IVsHierarchyEvents sink in _hierarchyEventSinks) {
                int result = sink.OnItemAdded(parent.HierarchyId, prevId, child.HierarchyId);
                if (ErrorHandler.Failed(result) && result != VSConstants.E_NOTIMPL) {
                    ErrorHandler.ThrowOnFailure(result);
                }
            }
        }

        internal void OnItemDeleted(HierarchyNode deletedItem) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            IDiskBasedNode diskNode = deletedItem as IDiskBasedNode;
            if (diskNode != null) {
                _diskNodes.Remove(diskNode.Url);
            }

            RaiseItemDeleted(deletedItem);
        }

        internal void RaiseItemDeleted(HierarchyNode deletedItem) {
            if ((EventTriggeringFlag & ProjectNode.EventTriggering.DoNotTriggerHierarchyEvents) != 0) {
                return;
            }

            ExtensibilityEventsDispatcher.FireItemRemoved(deletedItem);

            if (_hierarchyEventSinks.Count > 0) {
                // Note that in some cases (deletion of project node for example), an Advise
                // may be removed while we are iterating over it. To get around this problem we
                // take a snapshot of the advise list and walk that.
                List<IVsHierarchyEvents> clonedSink = new List<IVsHierarchyEvents>();

                foreach (IVsHierarchyEvents anEvent in _hierarchyEventSinks) {
                    clonedSink.Add(anEvent);
                }

                foreach (IVsHierarchyEvents clonedEvent in clonedSink) {
                    int result = clonedEvent.OnItemDeleted(deletedItem.HierarchyId);
                    if (ErrorHandler.Failed(result) && result != VSConstants.E_NOTIMPL) {
                        ErrorHandler.ThrowOnFailure(result);
                    }
                }
            }
        }

        internal void OnItemsAppended(HierarchyNode parent) {
            Utilities.ArgumentNotNull("parent", parent);

            if ((EventTriggeringFlag & ProjectNode.EventTriggering.DoNotTriggerHierarchyEvents) != 0) {
                return;
            }

            foreach (IVsHierarchyEvents sink in _hierarchyEventSinks) {
                int result = sink.OnItemsAppended(parent.HierarchyId);

                if (ErrorHandler.Failed(result) && result != VSConstants.E_NOTIMPL) {
                    ErrorHandler.ThrowOnFailure(result);
                }
            }
        }

        internal void OnPropertyChanged(HierarchyNode node, int propid, uint flags) {
            Utilities.ArgumentNotNull("node", node);

            if ((EventTriggeringFlag & ProjectNode.EventTriggering.DoNotTriggerHierarchyEvents) != 0) {
                return;
            }

            foreach (IVsHierarchyEvents sink in _hierarchyEventSinks) {
                int result = sink.OnPropertyChanged(node.HierarchyId, propid, flags);

                if (ErrorHandler.Failed(result) && result != VSConstants.E_NOTIMPL) {
                    ErrorHandler.ThrowOnFailure(result);
                }
            }
        }

        internal void OnInvalidateItems(HierarchyNode parent) {
            Utilities.ArgumentNotNull("parent", parent);

            if ((EventTriggeringFlag & ProjectNode.EventTriggering.DoNotTriggerHierarchyEvents) != 0) {
                return;
            }

            bool wasExpanded = ParentHierarchy != null && parent.GetIsExpanded();

            foreach (IVsHierarchyEvents sink in _hierarchyEventSinks) {
                int result = sink.OnInvalidateItems(parent.HierarchyId);

                if (ErrorHandler.Failed(result) && result != VSConstants.E_NOTIMPL) {
                    ErrorHandler.ThrowOnFailure(result);
                }
            }

            if (wasExpanded) {
                parent.ExpandItem(EXPANDFLAGS.EXPF_ExpandFolder);
            }
        }

        /// <summary>
        /// Causes the hierarchy to be redrawn.
        /// </summary>
        /// <param name="element">Used by the hierarchy to decide which element to redraw</param>
        internal void ReDrawNode(HierarchyNode node, UIHierarchyElement element) {
            foreach (IVsHierarchyEvents sink in _hierarchyEventSinks) {
                int result;
                if ((element & UIHierarchyElement.Icon) != 0) {
                    result = sink.OnPropertyChanged(node.ID, (int)__VSHPROPID.VSHPROPID_IconIndex, 0);
                    Debug.Assert(ErrorHandler.Succeeded(result), "Redraw failed for node " + this.GetMkDocument());
                }

                if ((element & UIHierarchyElement.Caption) != 0) {
                    result = sink.OnPropertyChanged(node.ID, (int)__VSHPROPID.VSHPROPID_Caption, 0);
                    Debug.Assert(ErrorHandler.Succeeded(result), "Redraw failed for node " + this.GetMkDocument());
                }

                if ((element & UIHierarchyElement.SccState) != 0) {
                    result = sink.OnPropertyChanged(node.ID, (int)__VSHPROPID.VSHPROPID_StateIconIndex, 0);
                    Debug.Assert(ErrorHandler.Succeeded(result), "Redraw failed for node " + this.GetMkDocument());
                }
            }
        }

        #endregion

        #region IVsHierarchyDeleteHandler methods

        public virtual int DeleteItem(uint delItemOp, uint itemId) {
            if (itemId == VSConstants.VSITEMID_SELECTION) {
                return VSConstants.E_INVALIDARG;
            }

            HierarchyNode node = NodeFromItemId(itemId);
            if (node != null) {
                node.Remove((delItemOp & (uint)__VSDELETEITEMOPERATION.DELITEMOP_DeleteFromStorage) != 0);
                return VSConstants.S_OK;
            }

            return VSConstants.E_FAIL;
        }

        public virtual int QueryDeleteItem(uint delItemOp, uint itemId, out int candelete) {
            candelete = 0;
            if (itemId == VSConstants.VSITEMID_SELECTION) {
                return VSConstants.E_INVALIDARG;
            }

            // We ask the project what state it is. If he is a state that should not allow delete then we return.
            if (IsCurrentStateASuppressCommandsMode()) {
                return VSConstants.S_OK;
            }

            HierarchyNode node = NodeFromItemId(itemId);

            if (node == null) {
                return VSConstants.E_FAIL;
            }

            // Ask the nodes if they can remove the item.
            bool canDeleteItem = node.CanDeleteItem((__VSDELETEITEMOPERATION)delItemOp);
            if (canDeleteItem) {
                candelete = 1;
            }

            return VSConstants.S_OK;
        }

        #endregion

        #region IVsHierarchyDeleteHandler2 methods

        public int ShowMultiSelDeleteOrRemoveMessage(uint dwDelItemOp, uint cDelItems, uint[] rgDelItems, out int pfCancelOperation) {
            pfCancelOperation = 0;
            return VSConstants.S_OK;
        }

        public int ShowSpecificDeleteRemoveMessage(uint dwDelItemOps, uint cDelItems, uint[] rgDelItems, out int pfShowStandardMessage, out uint pdwDelItemOp) {
            pfShowStandardMessage = 1;
            pdwDelItemOp = dwDelItemOps;

            var items = rgDelItems.Select(id => NodeFromItemId(id)).Where(n => n != null).ToArray();
            if (items.Length == 0) {
                return VSConstants.S_OK;
            } else {
                bool cancel, showStandardDialog;
                items[0].ShowDeleteMessage(items, (__VSDELETEITEMOPERATION)dwDelItemOps, out cancel, out showStandardDialog);

                if (showStandardDialog || cancel) {
                    pdwDelItemOp = 0;
                }
                if (!showStandardDialog) {
                    pfShowStandardMessage = 0;
                }
            }
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsPersistHierarchyItem2 methods

        /// <summary>
        /// Saves the hierarchy item to disk. 
        /// </summary>
        /// <param name="saveFlag">Flags whose values are taken from the VSSAVEFLAGS enumeration.</param>
        /// <param name="silentSaveAsName">New filename when doing silent save as</param>
        /// <param name="itemid">Item identifier of the hierarchy item saved from VSITEMID.</param>
        /// <param name="docData">Item identifier of the hierarchy item saved from VSITEMID.</param>
        /// <param name="cancelled">[out] true if the save action was canceled.</param>
        /// <returns>[out] true if the save action was canceled.</returns>
        public virtual int SaveItem(VSSAVEFLAGS saveFlag, string silentSaveAsName, uint itemid, IntPtr docData, out int cancelled) {
            cancelled = 0;

            // Validate itemid 
            if (itemid == VSConstants.VSITEMID_ROOT || itemid == VSConstants.VSITEMID_SELECTION) {
                return VSConstants.E_INVALIDARG;
            }

            HierarchyNode node = this.NodeFromItemId(itemid);
            if (node == null) {
                return VSConstants.E_FAIL;
            }

            string existingFileMoniker = node.GetMkDocument();

            // We can only perform save if the document is open
            if (docData == IntPtr.Zero) {
                throw new InvalidOperationException(SR.GetString(SR.CanNotSaveFileNotOpeneInEditor, node.Url));
            }

            string docNew = String.Empty;
            int returnCode = VSConstants.S_OK;
            IPersistFileFormat ff = null;
            IVsPersistDocData dd = null;
            IVsUIShell shell = Site.GetService(typeof(SVsUIShell)) as IVsUIShell;
            Utilities.CheckNotNull(shell);

            try {
                //Save docdata object. 
                //For the saveas action a dialog is show in order to enter new location of file.
                //In case of a save action and the file is readonly a dialog is also shown
                //with a couple of options, SaveAs, Overwrite or Cancel.
                ff = Marshal.GetObjectForIUnknown(docData) as IPersistFileFormat;
                Utilities.CheckNotNull(ff);

                if (VSSAVEFLAGS.VSSAVE_SilentSave == saveFlag) {
                    ErrorHandler.ThrowOnFailure(shell.SaveDocDataToFile(saveFlag, ff, silentSaveAsName, out docNew, out cancelled));
                } else {
                    dd = Marshal.GetObjectForIUnknown(docData) as IVsPersistDocData;
                    Utilities.CheckNotNull(dd);

                    ErrorHandler.ThrowOnFailure(dd.SaveDocData(saveFlag, out docNew, out cancelled));
                }

                // We can be unloaded after the SaveDocData() call if the save caused a designer to add a file and this caused
                // the project file to be reloaded (QEQS caused a newer version of the project file to be downloaded). So we check
                // here.
                if (IsClosed) {
                    cancelled = 1;
                    return (int)OleConstants.OLECMDERR_E_CANCELED;
                } else {
                    // if a SaveAs occurred we need to update to the fact our item's name has changed.
                    // this includes the following:
                    //    1. call RenameDocument on the RunningDocumentTable
                    //    2. update the full path name for the item in our hierarchy
                    //    3. a directory-based project may need to transfer the open editor to the
                    //       MiscFiles project if the new file is saved outside of the project directory.
                    //       This is accomplished by calling IVsExternalFilesManager::TransferDocument                    

                    // we have three options for a saveas action to be performed
                    // 1. the flag was set (the save as command was triggered)
                    // 2. a silent save specifying a new document name
                    // 3. a save command was triggered but was not possible because the file has a read only attrib. Therefore
                    //    the user has chosen to do a save as in the dialog that showed up
                    bool emptyOrSamePath = String.IsNullOrEmpty(docNew) || CommonUtils.IsSamePath(existingFileMoniker, docNew);
                    bool saveAs = ((saveFlag == VSSAVEFLAGS.VSSAVE_SaveAs)) ||
                        ((saveFlag == VSSAVEFLAGS.VSSAVE_SilentSave) && !emptyOrSamePath) ||
                        ((saveFlag == VSSAVEFLAGS.VSSAVE_Save) && !emptyOrSamePath);

                    if (saveAs) {
                        returnCode = node.AfterSaveItemAs(docData, docNew);

                        // If it has been cancelled recover the old name.
                        if ((returnCode == (int)OleConstants.OLECMDERR_E_CANCELED || returnCode == VSConstants.E_ABORT)) {
                            // Cleanup.
                            this.DeleteFromStorage(docNew);

                            if (ff != null) {
                                returnCode = shell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SilentSave, ff, existingFileMoniker, out docNew, out cancelled);
                            }
                        } else if (returnCode != VSConstants.S_OK) {
                            ErrorHandler.ThrowOnFailure(returnCode);
                        }
                    }
                }
            } catch (COMException e) {
                Trace.WriteLine("Exception :" + e.Message);
                returnCode = e.ErrorCode;

                // Try to recover
                // changed from MPFProj:
                // http://mpfproj10.codeplex.com/WorkItem/View.aspx?WorkItemId=6982
                if (ff != null && cancelled == 0) {
                    ErrorHandler.ThrowOnFailure(shell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SilentSave, ff, existingFileMoniker, out docNew, out cancelled));
                }
            }

            return returnCode;
        }

        /// <summary>
        /// Determines whether the hierarchy item changed. 
        /// </summary>
        /// <param name="itemId">Item identifier of the hierarchy item contained in VSITEMID.</param>
        /// <param name="docData">Pointer to the IUnknown interface of the hierarchy item.</param>
        /// <param name="isDirty">true if the hierarchy item changed.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int IsItemDirty(uint itemId, IntPtr docData, out int isDirty) {
            IVsPersistDocData pd = (IVsPersistDocData)Marshal.GetObjectForIUnknown(docData);
            return ErrorHandler.ThrowOnFailure(pd.IsDocDataDirty(out isDirty));
        }

        /// <summary>
        /// Flag indicating that changes to a file can be ignored when item is saved or reloaded. 
        /// </summary>
        /// <param name="itemId">Specifies the item id from VSITEMID.</param>
        /// <param name="ignoreFlag">Flag indicating whether or not to ignore changes (1 to ignore, 0 to stop ignoring).</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int IgnoreItemFileChanges(uint itemId, int ignoreFlag) {
            HierarchyNode n = this.NodeFromItemId(itemId);
            if (n != null) {
                n.IgnoreItemFileChanges(ignoreFlag == 0 ? false : true);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called to determine whether a project item is reloadable before calling ReloadItem. 
        /// </summary>
        /// <param name="itemId">Item identifier of an item in the hierarchy. Valid values are VSITEMID_NIL, VSITEMID_ROOT and VSITEMID_SELECTION.</param>
        /// <param name="isReloadable">A flag indicating that the project item is reloadable (1 for reloadable, 0 for non-reloadable).</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int IsItemReloadable(uint itemId, out int isReloadable) {
            isReloadable = 0;

            HierarchyNode n = this.NodeFromItemId(itemId);
            if (n != null) {
                isReloadable = (n.IsItemReloadable()) ? 1 : 0;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called to reload a project item. 
        /// </summary>
        /// <param name="itemId">Specifies itemid from VSITEMID.</param>
        /// <param name="reserved">Reserved.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int ReloadItem(uint itemId, uint reserved) {
            HierarchyNode n = this.NodeFromItemId(itemId);
            if (n != null) {
                n.ReloadItem(reserved);
            }

            return VSConstants.S_OK;
        }

        #endregion

        public void UpdatePathForDeferredSave(string oldPath, string newPath) {
            Site.GetUIThread().MustBeCalledFromUIThread();

            var existing = _diskNodes[oldPath];
            _diskNodes.Remove(oldPath);
            _diskNodes.Add(newPath, existing);
        }

        public IVsHierarchy ParentHierarchy {
            get {
                return parentHierarchy;
            }
        }

        [Conditional("DEBUG")]
        internal void AssertHasParentHierarchy() {
            // Calling into solution explorer before a parent hierarchy is assigned can
            // cause us to corrupt solution explorer if we're using flavored projects.  We
            // will call in with our inner project node and later we get wrapped in an
            // aggregate COM object which has different object identity.  At that point
            // solution explorer is confused because it uses object identity to track
            // the hierarchies.
            Debug.Assert(parentHierarchy != null, "dont call into the hierarchy before the project is loaded, it corrupts the hierarchy");
        }
    }
}
