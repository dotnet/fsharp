// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    using System;
    using System.Reflection;
    using System.Globalization;
    using System.Resources;
    using System.Text;
    using System.Threading;
    using System.ComponentModel;
    using System.Security.Permissions;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class SRDescriptionAttribute : DescriptionAttribute
    {

        private bool replaced = false;

        public SRDescriptionAttribute(string description)
            : base(description)
        {
        }

        public override string Description
        {
            get
            {
                if (!replaced)
                {
                    replaced = true;
                    DescriptionValue = SR.GetString(base.Description, CultureInfo.CurrentUICulture);
                }
                return base.Description;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class SRCategoryAttribute : CategoryAttribute
    {

        public SRCategoryAttribute(string category)
            : base(category)
        {
        }

        protected override string GetLocalizedString(string value)
        {
            return SR.GetString(value, CultureInfo.CurrentUICulture);
        }
    }
    /*internal, but public for FSharp.Project.dll*/ public sealed class SR
    {
        /*internal, but public for FSharp.Project.dll*/ public const string AddToNullProjectError = "AddToNullProjectError";
        /*internal, but public for FSharp.Project.dll*/ public const string Advanced = "Advanced";
        /*internal, but public for FSharp.Project.dll*/ public const string AssemblyReferenceAlreadyExists = "AssemblyReferenceAlreadyExists";
        /*internal, but public for FSharp.Project.dll*/ public const string AttributeLoad = "AttributeLoad";
        /*internal, but public for FSharp.Project.dll*/ public const string BuildAction = "BuildAction";
        /*internal, but public for FSharp.Project.dll*/ public const string BuildActionDescription = "BuildActionDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string BuildCaption = "BuildCaption";
        /*internal, but public for FSharp.Project.dll*/ public const string BuildVerbosity = "BuildVerbosity";
        /*internal, but public for FSharp.Project.dll*/ public const string BuildVerbosityDescription = "BuildVerbosityDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string BuildEventError = "BuildEventError";
        /*internal, but public for FSharp.Project.dll*/ public const string CancelQueryEdit = "CancelQueryEdit";
        /*internal, but public for FSharp.Project.dll*/ public const string CannotAddFileThatIsOpenInEditor = "CannotAddFileThatIsOpenInEditor";
        /*internal, but public for FSharp.Project.dll*/ public const string CannotAddItemToProjectWithWildcards = "CannotAddItemToProjectWithWildcards";
        /*internal, but public for FSharp.Project.dll*/ public const string CanNotSaveFileNotOpeneInEditor = "CanNotSaveFileNotOpeneInEditor";
        /*internal, but public for FSharp.Project.dll*/ public const string CannotStartLibraries = "CannotStartLibraries";
        /*internal, but public for FSharp.Project.dll*/ public const string cli1 = "cli1";
        /*internal, but public for FSharp.Project.dll*/ public const string Compile = "Compile";
        /*internal, but public for FSharp.Project.dll*/ public const string ConfirmExtensionChange = "ConfirmExtensionChange";
        /*internal, but public for FSharp.Project.dll*/ public const string Content = "Content";
        /*internal, but public for FSharp.Project.dll*/ public const string CopyAlways = "CopyAlways";
        /*internal, but public for FSharp.Project.dll*/ public const string CopyIfNewer = "CopyIfNewer";
        /*internal, but public for FSharp.Project.dll*/ public const string CopyToLocal = "CopyToLocal";
        /*internal, but public for FSharp.Project.dll*/ public const string CopyToLocalDescription = "CopyToLocalDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string CopyToOutputDirectory = "CopyToOutputDirectory";
        /*internal, but public for FSharp.Project.dll*/ public const string CopyToOutputDirectoryDescription = "CopyToOutputDirectoryDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string CustomTool = "CustomTool";
        /*internal, but public for FSharp.Project.dll*/ public const string CustomToolDescription = "CustomToolDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string CustomToolNamespace = "CustomToolNamespace";
        /*internal, but public for FSharp.Project.dll*/ public const string CustomToolNamespaceDescription = "CustomToolNamespaceDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string DetailsImport = "DetailsImport";
        /*internal, but public for FSharp.Project.dll*/ public const string DetailsUserImport = "DetailsUserImport";
        /*internal, but public for FSharp.Project.dll*/ public const string DetailsItem = "DetailsItem";
        /*internal, but public for FSharp.Project.dll*/ public const string DetailsItemLocation = "DetailsItemLocation";
        /*internal, but public for FSharp.Project.dll*/ public const string DetailsProperty = "DetailsProperty";
        /*internal, but public for FSharp.Project.dll*/ public const string DetailsTarget = "DetailsTarget";
        /*internal, but public for FSharp.Project.dll*/ public const string DetailsUsingTask = "DetailsUsingTask";
        /*internal, but public for FSharp.Project.dll*/ public const string Detailed = "Detailed";
        /*internal, but public for FSharp.Project.dll*/ public const string Diagnostic = "Diagnostic";
        /*internal, but public for FSharp.Project.dll*/ public const string DirectoryExistError = "DirectoryExistError";
        /*internal, but public for FSharp.Project.dll*/ public const string DoNotCopy = "DoNotCopy";
        /*internal, but public for FSharp.Project.dll*/ public const string EditorViewError = "EditorViewError";
        /*internal, but public for FSharp.Project.dll*/ public const string EmbeddedResource = "EmbeddedResource";
        /*internal, but public for FSharp.Project.dll*/ public const string Error = "Error";
        /*internal, but public for FSharp.Project.dll*/ public const string ErrorInvalidFileName = "ErrorInvalidFileName";
        /*internal, but public for FSharp.Project.dll*/ public const string ErrorInvalidProjectName = "ErrorInvalidProjectName";
        /*internal, but public for FSharp.Project.dll*/ public const string ErrorReferenceCouldNotBeAdded = "ErrorReferenceCouldNotBeAdded";
        /*internal, but public for FSharp.Project.dll*/ public const string ErrorMsBuildRegistration = "ErrorMsBuildRegistration";
        /*internal, but public for FSharp.Project.dll*/ public const string ErrorSaving = "ErrorSaving";
        /*internal, but public for FSharp.Project.dll*/ public const string Exe = "Exe";
        /*internal, but public for FSharp.Project.dll*/ public const string ExpectedObjectOfType = "ExpectedObjectOfType";
        /*internal, but public for FSharp.Project.dll*/ public const string FailedToGetService = "FailedToGetService";
        /*internal, but public for FSharp.Project.dll*/ public const string FailedToRetrieveProperties = "FailedToRetrieveProperties";
        /*internal, but public for FSharp.Project.dll*/ public const string FileNameCannotContainALeadingPeriod = "FileNameCannotContainALeadingPeriod";
        /*internal, but public for FSharp.Project.dll*/ public const string FileCannotBeRenamedToAnExistingFile = "FileCannotBeRenamedToAnExistingFile";
        /*internal, but public for FSharp.Project.dll*/ public const string FileAlreadyExistsAndCannotBeRenamed = "FileAlreadyExistsAndCannotBeRenamed";
        /*internal, but public for FSharp.Project.dll*/ public const string FileAlreadyExists = "FileAlreadyExists";
        /*internal, but public for FSharp.Project.dll*/ public const string FileAlreadyExistsCaption = "FileAlreadyExistsCaption";
        /*internal, but public for FSharp.Project.dll*/ public const string FileAlreadyInProject = "FileAlreadyInProject";
        /*internal, but public for FSharp.Project.dll*/ public const string FileAlreadyInProjectCaption = "FileAlreadyInProjectCaption";
        /*internal, but public for FSharp.Project.dll*/ public const string FileCopyError = "FileCopyError";
        /*internal, but public for FSharp.Project.dll*/ public const string FileName = "FileName";
        /*internal, but public for FSharp.Project.dll*/ public const string FileNameDescription = "FileNameDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string FileOrFolderAlreadyExists = "FileOrFolderAlreadyExists";
        /*internal, but public for FSharp.Project.dll*/ public const string FileOrFolderCannotBeFound = "FileOrFolderCannotBeFound";
        /*internal, but public for FSharp.Project.dll*/ public const string FileProperties = "FileProperties";
        /*internal, but public for FSharp.Project.dll*/ public const string FolderName = "FolderName";
        /*internal, but public for FSharp.Project.dll*/ public const string FolderNameDescription = "FolderNameDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string FolderProperties = "FolderProperties";
        /*internal, but public for FSharp.Project.dll*/ public const string FullPath = "FullPath";
        /*internal, but public for FSharp.Project.dll*/ public const string FullPathDescription = "FullPathDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string ItemDoesNotExistInProjectDirectory = "ItemDoesNotExistInProjectDirectory";
        /*internal, but public for FSharp.Project.dll*/ public const string InvalidAutomationObject = "InvalidAutomationObject";
        /*internal, but public for FSharp.Project.dll*/ public const string InvalidLoggerType = "InvalidLoggerType";
        /*internal, but public for FSharp.Project.dll*/ public const string InvalidParameter = "InvalidParameter";
        /*internal, but public for FSharp.Project.dll*/ public const string Library = "Library";
        /*internal, but public for FSharp.Project.dll*/ public const string LinkedItemsAreNotSupported = "LinkedItemsAreNotSupported";
        /*internal, but public for FSharp.Project.dll*/ public const string Minimal = "Minimal";
        /*internal, but public for FSharp.Project.dll*/ public const string Misc = "Misc";
        /*internal, but public for FSharp.Project.dll*/ public const string None = "None";
        /*internal, but public for FSharp.Project.dll*/ public const string NoWildcardsInProject = "NoWildcardsInProject";
        /*internal, but public for FSharp.Project.dll*/ public const string NoZeroImpactProjects = "NoZeroImpactProjects";
        /*internal, but public for FSharp.Project.dll*/ public const string Normal = "Normal";
        /*internal, but public for FSharp.Project.dll*/ public const string NestedProjectFailedToReload = "NestedProjectFailedToReload";
        /*internal, but public for FSharp.Project.dll*/ public const string OutputPath = "OutputPath";
        /*internal, but public for FSharp.Project.dll*/ public const string OutputPathDescription = "OutputPathDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string PasteFailed = "PasteFailed";
        /*internal, but public for FSharp.Project.dll*/ public const string ParameterMustBeAValidGuid = "ParameterMustBeAValidGuid";
        /*internal, but public for FSharp.Project.dll*/ public const string ParameterMustBeAValidItemId = "ParameterMustBeAValidItemId";
        /*internal, but public for FSharp.Project.dll*/ public const string ParameterCannotBeNullOrEmpty = "ParameterCannotBeNullOrEmpty";
        /*internal, but public for FSharp.Project.dll*/ public const string PathTooLong = "PathTooLong";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectContainsCircularReferences = "ProjectContainsCircularReferences";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectReferencesDifferentFramework = "ProjectReferencesDifferentFramework";
        /*internal, but public for FSharp.Project.dll*/ public const string Program = "Program";
        /*internal, but public for FSharp.Project.dll*/ public const string Project = "Project";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectFile = "ProjectFile";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectFileDescription = "ProjectFileDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectFolder = "ProjectFolder";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectFolderDescription = "ProjectFolderDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectProperties = "ProjectProperties";
        /*internal, but public for FSharp.Project.dll*/ public const string Quiet = "Quiet";
        /*internal, but public for FSharp.Project.dll*/ public const string QueryReloadNestedProject = "QueryReloadNestedProject";
        /*internal, but public for FSharp.Project.dll*/ public const string ReferenceCouldNotBeAdded = "ReferenceCouldNotBeAdded";
        /*internal, but public for FSharp.Project.dll*/ public const string ReferenceAlreadyExists = "ReferenceAlreadyExists";
        /*internal, but public for FSharp.Project.dll*/ public const string ReferenceWithAssemblyNameAlreadyExists = "ReferenceWithAssemblyNameAlreadyExists";
        /*internal, but public for FSharp.Project.dll*/ public const string ReferencesNodeName = "ReferencesNodeName";
        /*internal, but public for FSharp.Project.dll*/ public const string ReferenceProperties = "ReferenceProperties";
        /*internal, but public for FSharp.Project.dll*/ public const string RefName = "RefName";
        /*internal, but public for FSharp.Project.dll*/ public const string RefNameDescription = "RefNameDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string RenameFolder = "RenameFolder";
        /*internal, but public for FSharp.Project.dll*/ public const string RTL = "RTL";
        /*internal, but public for FSharp.Project.dll*/ public const string SaveCaption = "SaveCaption";
        /*internal, but public for FSharp.Project.dll*/ public const string SaveModifiedDocuments = "SaveModifiedDocuments";
        /*internal, but public for FSharp.Project.dll*/ public const string SaveOfProjectFileOutsideCurrentDirectory = "SaveOfProjectFileOutsideCurrentDirectory";
        /*internal, but public for FSharp.Project.dll*/ public const string SpecificVersion = "SpecificVersion";
        /*internal, but public for FSharp.Project.dll*/ public const string SpecificVersionDescription = "SpecificVersionDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string StandardEditorViewError = "StandardEditorViewError";
        /*internal, but public for FSharp.Project.dll*/ public const string URL = "URL";
        /*internal, but public for FSharp.Project.dll*/ public const string UseOfDeletedItemError = "UseOfDeletedItemError";
        /*internal, but public for FSharp.Project.dll*/ public const string v1 = "v1";
        /*internal, but public for FSharp.Project.dll*/ public const string v11 = "v11";
        /*internal, but public for FSharp.Project.dll*/ public const string v2 = "v2";
        /*internal, but public for FSharp.Project.dll*/ public const string Warning = "Warning";
        /*internal, but public for FSharp.Project.dll*/ public const string WinExe = "WinExe";
        /*internal, but public for FSharp.Project.dll*/ public const string UpgradeCannotOpenProjectFileForEdit = "UpgradeCannotOpenProjectFileForEdit";
        /*internal, but public for FSharp.Project.dll*/ public const string UpgradeNoNeedToUpgradeAfterCheckout = "UpgradeNoNeedToUpgradeAfterCheckout";        
        /*internal, but public for FSharp.Project.dll*/ public const string InvalidOutputPath = "InvalidOutputPath";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectRefOnlyExeOrDll = "ProjectRefOnlyExeOrDll";
        /*internal, but public for FSharp.Project.dll*/ public const string CannotBuildWhenBuildInProgress = "CannotBuildWhenBuildInProgress";
        /*internal, but public for FSharp.Project.dll*/ public const string WorkingDirectoryNotExists = "WorkingDirectoryNotExists";
        /*internal, but public for FSharp.Project.dll*/ public const string CannotLoadUnknownTargetFrameworkProject = "CannotLoadUnknownTargetFrameworkProject";
        /*internal, but public for FSharp.Project.dll*/ public const string ProductName = "ProductName";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectReferencesHigherVersionWarning = "ProjectReferencesHigherVersionWarning";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectConversionNotRequired = "ProjectConversionNotRequired";
        /*internal, but public for FSharp.Project.dll*/ public const string ConversionNotRequired = "ConversionNotRequired";
        /*internal, but public for FSharp.Project.dll*/ public const string ErrorMakingProjectBackup = "ErrorMakingProjectBackup";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectBackupSuccessful = "ProjectBackupSuccessful";
        /*internal, but public for FSharp.Project.dll*/ public const string BackupNameConflict = "BackupNameConflict";
        /*internal, but public for FSharp.Project.dll*/ public const string ProjectContainsLinkedFile = "ProjectContainsLinkedFile";
        /*internal, but public for FSharp.Project.dll*/ public const string ErrorMakingBackup = "ErrorMakingBackup";
        /*internal, but public for FSharp.Project.dll*/ public const string BackupSuccessful = "BackupSuccessful";
        /*internal, but public for FSharp.Project.dll*/ public const string Identity = "Identity";
        /*internal, but public for FSharp.Project.dll*/ public const string IdentityDescription = "IdentityDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string Path = "Path";
        /*internal, but public for FSharp.Project.dll*/ public const string PathDescription = "PathDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string Version = "Version";
        /*internal, but public for FSharp.Project.dll*/ public const string VersionDescription = "VersionDescription";
        /*internal, but public for FSharp.Project.dll*/ public const string CopyOf = "CopyOf";
        /*internal, but public for FSharp.Project.dll*/ public const string CopyOf2 = "CopyOf2";

        static SR loader = null;
        ResourceManager resources;

        private static Object s_InternalSyncObject;
        private static Object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    Object o = new Object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, o, null);
                }
                return s_InternalSyncObject;
            }
        }

        /*internal, but public for FSharp.Project.dll*/ public SR()
        {
            resources = new System.Resources.ResourceManager("Microsoft.VisualStudio.Package.Project", this.GetType().Assembly);
        }

        private static SR GetLoader()
        {
            if (loader == null)
            {
                lock (InternalSyncObject)
                {
                    if (loader == null)
                    {
                        loader = new SR();
                    }
                }
            }

            return loader;
        }

        private static CultureInfo Culture
        {
            get { return null/*use ResourceManager default, CultureInfo.CurrentUICulture*/; }
        }

        public static ResourceManager Resources
        {
            get
            {
                return GetLoader().resources;
            }
        }

        public static string GetString(string name, params object[] args)
        {
            SR sys = GetLoader();
            if (sys == null)
                return null;
            string res = sys.resources.GetString(name, SR.Culture);

            if (args != null && args.Length > 0)
            {
                return String.Format(CultureInfo.CurrentCulture, res, args);
            }
            else
            {
                return res;
            }
        }

        public static string GetString(string name)
        {
            SR sys = GetLoader();
            if (sys == null)
                return null;
            return sys.resources.GetString(name, SR.Culture);
        }

        public static string GetString(string name, CultureInfo culture)
        {
            SR sys = GetLoader();
            if (sys == null)
                return null;
            return sys.resources.GetString(name, culture);
        }

        public static string GetStringWithCR(string name)
        {
            var s = GetString(name);
            return s.Replace(@"\n", Environment.NewLine);
        }

        public static object GetObject(string name)
        {
            SR sys = GetLoader();
            if (sys == null)
                return null;
            return sys.resources.GetObject(name, SR.Culture);
        }
    }
}
