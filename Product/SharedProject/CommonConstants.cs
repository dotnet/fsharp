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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace Microsoft.VisualStudioTools {
    internal static class CommonConstants {
        /// <summary>
        /// <see cref="VsConstants.UICONTEXT_NoSolution"/>.
        /// </summary>
        public const string UIContextNoSolution = "ADFC4E64-0397-11D1-9F4E-00A0C911004F";

        /// <summary>
        /// <see cref="VsConstants.UICONTEXT_SolutionExists"/>.
        /// </summary>
        public const string UIContextSolutionExists = "f1536ef8-92ec-443c-9ed7-fdadf150da82";

        /// <summary>
        /// Do not change this constant. It is a guid of the Visual Studio "Recent" page in
        /// the "Add Reference" dialog.
        /// </summary>
        internal const string AddReferenceMRUPageGuid = "{19B97F03-9594-4c1c-BE28-25FF030113B3}";

        internal const string TextEditorPackage = "{F5E7E720-1401-11D1-883B-0000F87579D2}";

        /// <summary>
        /// Do not change this constant. This is Visual Studio core text editor GUID.
        /// </summary>
        public static readonly Guid CMDUIGUID_TextEditor = new Guid("{8B382828-6202-11d1-8870-0000F87579D2}");

        internal const string LibraryGuid = "c0000061-2c33-4277-bf44-7e5f2677d6b8";
        internal const string FileNodePropertiesGuid = "c000008b-a973-4d60-9518-5ea1411ccd67";
        internal const string SearchPathsPropertiesGuid = "D94E2410-B416-4CB2-B892-AE83D7BF7356";
        internal const string FolderNodePropertiesGuid = "c0000081-fb55-4d5d-901b-ee624db34961";
        internal const string ProjectNodePropertiesGuid = "c0000016-9ab0-4d58-80e6-54f29e8d3144";
        internal static Guid NoSharedCommandsGuid = new Guid("{C4EBB0A2-969D-49D9-B87A-DCA1D3DF3A5B}");
        internal static Guid SearchPathItemTypeGuid = new Guid("{245F8B38-B204-4676-8F59-7415C34C06EA}");

        //"Set As StartUp File" command
        public const string StartupFile = "StartupFile";
        public const uint SetAsStartupFileCmdId = 0x3001;

        // Constants to handle the scrollbars.
        public const int ConsoleHorizontalScrollbar = 0;
        public const int ConsoleVerticalScrollbar = 1;

        //Start Without Debugging command
        public const int StartWithoutDebuggingCmdId = 0x4004;

        //Start Debugging command
        public const int StartDebuggingCmdId = 0x4005;

        //Working Directory project property
        public const string WorkingDirectory = "WorkingDirectory";

        //"Open Folder in Windows Explorer" command ID.
        //Don't change this! This is Visual Studio constant.
        public const VsCommands2K OpenFolderInExplorerCmdId = (VsCommands2K)1635;

        //Sort priority for the Working Directory node
        //We want this node to be the first node in the Search Path subtree
        public const int WorkingDirectorySortPriority = 100;

        //Project Home project property
        public const string ProjectHome = "ProjectHome";

        //TODO: Is there a constant in the SDK for this?
        public const int SizeOfVariant = 16;

        //"Command line arguments" project property
        public const string CommandLineArguments = "CommandLineArguments";

        public const string IsWindowsApplication = "IsWindowsApplication";

        public const string PublishUrl = "PublishUrl";

        //These are VS internal constants - don't change them
        public static Guid Std97CmdGroupGuid = typeof(VSConstants.VSStd97CmdID).GUID;
        public static Guid Std2KCmdGroupGuid = typeof(VSConstants.VSStd2KCmdID).GUID;

        //Command statuses
        public const int NotSupportedInvisibleCmdStatus = (int)OleConstants.OLECMDERR_E_NOTSUPPORTED |
                            (int)OleConstants.OLECMDSTATE_INVISIBLE;
        public const int SupportedEnabledCmdStatus = (int)(OLECMDF.OLECMDF_SUPPORTED |
                            OLECMDF.OLECMDF_ENABLED);
        public const int SupportedCmdStatus = (int)OLECMDF.OLECMDF_SUPPORTED;

        /// <summary>
        /// Show all files is enabled, we show the merged view of project + files
        /// </summary>
        public const string ShowAllFiles = "ShowAllFiles";
        /// <summary>
        /// Show all files is disabled, we show the project
        /// </summary>
        public const string ProjectFiles = "ProjectFiles";
        /// <summary>
        /// ProjectView property name for project file to enable / disable show all files setting
        /// </summary>
        public const string ProjectView = "ProjectView";

        /// <summary>
        /// Item meta data for whether or not a item in msbuild is visible in the project
        /// </summary>
        public const string Visible = "Visible";
    }
}
