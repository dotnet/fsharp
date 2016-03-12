// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService {

    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;
    using System.Runtime.InteropServices;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Permissions;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Globalization;

    // This class is shared between assemblies
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal static class NativeMethods {

        public static IntPtr InvalidIntPtr = ((IntPtr)((int)(-1)));

        // IIDS
        public static readonly Guid IID_IServiceProvider = typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider).GUID;
        public static readonly Guid IID_IObjectWithSite = typeof(IObjectWithSite).GUID;
        public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");                    

        public static readonly Guid GUID_PropertyBrowserToolWindow = new Guid(unchecked((int)0xeefa5220), unchecked((short)0xe298), (short)0x11d0, new byte[]{ 0x8f, 0x78, 0x0, 0xa0, 0xc9, 0x11, 0x0, 0x57 });
        public static readonly Guid GUID_VSStandardCommandSet97 = new Guid("{5efc7975-14bc-11cf-9b2b-00aa00573819}");

        public static readonly Guid CLSID_HtmDocData = new Guid(unchecked((int)0x62C81794), unchecked((short)0xA9EC), (short)0x11D0, new byte[] {0x81, 0x98, 0x0, 0xa0, 0xc9, 0x1b, 0xbe, 0xe3});
        public static readonly Guid CLSID_HtmedPackage = new Guid(unchecked((int)0x1B437D20), unchecked((short)0xF8FE), (short)0x11D2, new byte[] {0xA6, 0xAE, 0x00, 0x10, 0x4B, 0xCC, 0x72, 0x69});
        public static readonly Guid CLSID_HtmlLanguageService = new Guid(unchecked((int)0x58E975A0), unchecked((short)0xF8FE), (short)0x11D2, new byte[] {0xA6, 0xAE, 0x00, 0x10, 0x4B, 0xCC, 0x72, 0x69});
        public static readonly Guid GUID_HtmlEditorFactory = new Guid("{C76D83F8-A489-11D0-8195-00A0C91BBEE3}");
        public static readonly Guid GUID_TextEditorFactory = new Guid("{8B382828-6202-11d1-8870-0000F87579D2}");
        public static readonly Guid GUID_HTMEDAllowExistingDocData = new Guid(unchecked((int)0x5742d216), unchecked((short)0x8071), (short)0x4779, new byte[] {0xbf, 0x5f, 0xa2, 0x4d, 0x5f, 0x31, 0x42, 0xba});
        
        /// <summary>GUID for the environment package.</summary>
        public static readonly Guid CLSID_VsEnvironmentPackage = new Guid("{DA9FB551-C724-11d0-AE1F-00A0C90FFFC3}");
        /// <summary>GUID for the "Visual Studio" pseudo folder in the registry.</summary>
        public static readonly Guid GUID_VsNewProjectPseudoFolder = new Guid("{DCF2A94A-45B0-11d1-ADBF-00C04FB6BE4C}");
        /// <summary>GUID for the "Miscellaneous Files" project.</summary>
        public static readonly Guid CLSID_MiscellaneousFilesProject = new Guid("{A2FE74E1-B743-11d0-AE1A-00A0C90FFFC3}");
        /// <summary>GUID for Solution Items project.</summary>
        public static readonly Guid CLSID_SolutionItemsProject = new Guid("{D1DCDB85-C5E8-11d2-BFCA-00C04F990235}");
        /// <summary>Pseudo service that returns a IID_IVsOutputWindowPane interface of the General output pane in the VS environment.
        /// Querying for this service will cause the General output pane to be created if it hasn't yet been created.
        /// </summary>
        public static readonly Guid SID_SVsGeneralOutputWindowPane = new Guid("{65482c72-defa-41b7-902c-11c091889c83}");
        /// <summary>
        /// SUIHostCommandDispatcher service returns an object that implements IOleCommandTarget.
        /// This object handles command routing for the Environment. Use this service if you need to
        /// route a command based on the current selection/state of the Environment.
        /// </summary>
        public static readonly Guid SID_SUIHostCommandDispatcher = new Guid("{e69cd190-1276-11d1-9f64-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid CLSID_VsUIHierarchyWindow = new Guid("{7D960B07-7AF8-11D0-8E5E-00A0C911005A}");

        /// <summary></summary>
        public static readonly Guid GUID_DefaultEditor = new Guid("{6AC5EF80-12BF-11D1-8E9B-00A0C911005A}");
        /// <summary></summary>
        public static readonly Guid GUID_ExternalEditor = new Guid("{8137C9E8-35FE-4AF2-87B0-DE3C45F395FD}");

        //--------------------------------------------------------------------
        // GUIDs for some panes of the Output Window
        //--------------------------------------------------------------------
        /// <summary>GUID of the general output pane inside the output window.</summary>
        public static readonly Guid GUID_OutWindowGeneralPane = new Guid("{3c24d581-5591-4884-a571-9fe89915cd64}");
        // Guids for GetOutputPane.
        public static readonly Guid BuildOrder = new Guid("2032b126-7c8d-48ad-8026-0e0348004fc0");
        public static readonly Guid BuildOutput = new Guid("1BD8A850-02D1-11d1-BEE7-00A0C913D1F8");
        public static readonly Guid DebugOutput = new Guid("FC076020-078A-11D1-A7DF-00A0C9110051");
        
        //--------------------------------------------------------------------
        // standard item types, to be returned from VSHPROPID_TypeGuid
        //--------------------------------------------------------------------
        /// <summary>Physical file on disk or web (IVsProject::GetMkDocument returns a file path).</summary>
        public static readonly Guid GUID_ItemType_PhysicalFile = new Guid("{6bb5f8ee-4483-11d3-8bcf-00c04f8ec28c}");
        /// <summary>Physical folder on disk or web (IVsProject::GetMkDocument returns a directory path).</summary>
        public static readonly Guid GUID_ItemType_PhysicalFolder = new Guid("{6bb5f8ef-4483-11d3-8bcf-00c04f8ec28c}");
        /// <summary>Non-physical folder (folder is logical and not a physical file system directory).</summary>
        public static readonly Guid GUID_ItemType_VirtualFolder = new Guid("{6bb5f8f0-4483-11d3-8bcf-00c04f8ec28c}");
        /// <summary>A nested hierarchy project.</summary>
        public static readonly Guid GUID_ItemType_SubProject = new Guid("{EA6618E8-6E24-4528-94BE-6889FE16485C}");

        //--------------------------------------------------------------------
        // GUIDs used in calling IVsMonitorSelection::GetCmdUIContextCookie()
        //--------------------------------------------------------------------
        /// <summary></summary>
        public static readonly Guid UICONTEXT_SolutionBuilding            = new Guid("{adfc4e60-0397-11d1-9f4e-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_Debugging                   = new Guid("{adfc4e61-0397-11d1-9f4e-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_Dragging                    = new Guid("{b706f393-2e5b-49e7-9e2e-b1825f639b63}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_FullScreenMode              = new Guid("{adfc4e62-0397-11d1-9f4e-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_DesignMode                  = new Guid("{adfc4e63-0397-11d1-9f4e-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_NoSolution                  = new Guid("{adfc4e64-0397-11d1-9f4e-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_SolutionExists              = new Guid("{f1536ef8-92ec-443c-9ed7-fdadf150da82}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_EmptySolution               = new Guid("{adfc4e65-0397-11d1-9f4e-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_SolutionHasSingleProject    = new Guid("{adfc4e66-0397-11d1-9f4e-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_SolutionHasMultipleProjects = new Guid("{93694fa0-0397-11d1-9f4e-00a0c911004f}");
        /// <summary></summary>
        public static readonly Guid UICONTEXT_CodeWindow                  = new Guid("{8fe2df1d-e0da-4ebe-9d5c-415d40e487b5}");

        //--------------------------------------------------------------------
        // GUIDS for built in task list views
        //--------------------------------------------------------------------
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewAll              = new Guid("{1880202e-fc20-11d2-8bb1-00c04f8ec28c}");
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewUserTasks        = new Guid("{1880202f-fc20-11d2-8bb1-00c04f8ec28c}");
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewShortcutTasks    = new Guid("{18802030-fc20-11d2-8bb1-00c04f8ec28c}");
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewHTMLTasks        = new Guid("{36ac1c0d-fe86-11d2-8bb1-00c04f8ec28c}");
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewCompilerTasks    = new Guid("{18802033-fc20-11d2-8bb1-00c04f8ec28c}");
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewCommentTasks     = new Guid("{18802034-fc20-11d2-8bb1-00c04f8ec28c}");
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewCurrentFileTasks = new Guid("{18802035-fc20-11d2-8bb1-00c04f8ec28c}");
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewCheckedTasks     = new Guid("{18802036-fc20-11d2-8bb1-00c04f8ec28c}");
        /// <summary></summary>
        public static readonly Guid GUID_VsTaskListViewUncheckedTasks   = new Guid("{18802037-fc20-11d2-8bb1-00c04f8ec28c}");

        /// <summary></summary>
        public static readonly Guid CLSID_VsTaskList            = new Guid("{BC5955D5-aa0d-11d0-a8c5-00a0c921a4d2}");
        /// <summary></summary>
        public static readonly Guid CLSID_VsTaskListPackage     = new Guid("{4A9B7E50-aa16-11d0-a8c5-00a0c921a4d2}");


        /// <summary></summary>
        public static readonly Guid SID_SVsToolboxActiveXDataProvider = new Guid("{35222106-bb44-11d0-8c46-00c04fc2aae2}");
        /// <summary></summary>
        public static readonly Guid CLSID_VsDocOutlinePackage         = new Guid("{21af45b0-ffa5-11d0-b63f-00a0c922e851}");
        /// <summary></summary>
        public static readonly Guid CLSID_VsCfgProviderEventsHelper   = new Guid("{99913f1f-1ee3-11d1-8a6e-00c04f682e21}");


        //--------------------------------------------------------------------
        // Component Selector page GUIDs
        //--------------------------------------------------------------------
        /// <summary></summary>
        public static readonly Guid GUID_COMPlusPage    = new Guid("{9A341D95-5A64-11d3-BFF9-00C04F990235}");
        /// <summary></summary>
        public static readonly Guid GUID_COMClassicPage = new Guid("{9A341D96-5A64-11d3-BFF9-00C04F990235}");
        /// <summary></summary>
        public static readonly Guid GUID_SolutionPage   = new Guid("{9A341D97-5A64-11d3-BFF9-00C04F990235}");
 
        [ComImport,System.Runtime.InteropServices.Guid("5EFC7974-14BC-11CF-9B2B-00AA00573819")]
        public class OleComponentUIManager {
        }

        [ComImport,System.Runtime.InteropServices.Guid("8E7B96A8-E33D-11D0-A6D5-00C04FB67F6A")]
        public class VsTextBuffer {
        }

        // HRESULTS
        public static bool Succeeded(int hr) {
            return(hr >= 0);
        }

        public static bool Failed(int hr) {
            return(hr < 0);
        }

        public static int ThrowOnFailure(int hr)
        {
            return ThrowOnFailure(hr, null);
        }

        public static int ThrowOnFailure(int hr, params int[] expectedHRFailure)
        {
            if (Failed(hr))
            {
                if ((null == expectedHRFailure) || (Array.IndexOf(expectedHRFailure, hr) < 0))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }

            return hr;
        }

        // packing
        public static int SignedHIWORD(int n) {
            return (int)(short)((n >> 16) & 0xffff);
        }

        public static int SignedLOWORD(int n) {
            return (int)(short)(n & 0xFFFF);
        }

        public const int
        CLSCTX_INPROC_SERVER  = 0x1;

        public const int
        S_FALSE =   0x00000001,
        S_OK =      0x00000000,

        IDOK =               1,
        IDCANCEL =           2,
        IDABORT =            3,
        IDRETRY =            4,
        IDIGNORE =           5,
        IDYES =              6,
        IDNO =               7,
        IDCLOSE =            8,
        IDHELP =             9,
        IDTRYAGAIN =        10,
        IDCONTINUE =        11,

        ILD_NORMAL =      0x0000,
        ILD_TRANSPARENT = 0x0001,
        ILD_MASK =        0x0010,
        ILD_ROP =         0x0040,

        OLECMDERR_E_NOTSUPPORTED = unchecked((int)0x80040100),
        OLECMDERR_E_UNKNOWNGROUP  = unchecked((int)0x80040104),
        
        UNDO_E_CLIENTABORT = unchecked((int)0x80044001),
        E_OUTOFMEMORY = unchecked((int)0x8007000E),
        E_INVALIDARG = unchecked((int)0x80070057),
        E_FAIL = unchecked((int)0x80004005),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_POINTER = unchecked((int)0x80004003),
        E_NOTIMPL = unchecked((int)0x80004001),   
        E_UNEXPECTED = unchecked((int)0x8000FFFF),
        E_HANDLE = unchecked((int)0x80070006),
        E_ABORT = unchecked((int)0x80004004),
        E_ACCESSDENIED = unchecked((int)0x80070005),
        E_PENDING = unchecked((int)0x8000000A); 

        public const int
        VS_E_UNSUPPORTEDFORMAT = unchecked((int)0x80041FEB),
        VS_E_INCOMPATIBLEDOCDATA = unchecked((int)0x80041FEA),
        VS_E_PACKAGENOTLOADED = unchecked((int)0x80041fe1),
        VS_E_PROJECTNOTLOADED = unchecked((int)0x80041fe2),
        VS_E_SOLUTIONNOTOPEN = unchecked((int)0x80041fe3),
        VS_E_SOLUTIONALREADYOPEN = unchecked((int)0x80041fe4),
        VS_E_PROJECTMIGRATIONFAILED = unchecked((int)0x80041fe5),
        VS_E_WIZARDBACKBUTTONPRESS = unchecked((int)0x80041fff),
        VS_S_PROJECTFORWARDED = unchecked((int)0x41ff0),
        VS_S_TBXMARKER = unchecked((int)0x41ff1);

        public const ushort CF_HDROP = 15; // winuser.h
        public const uint MK_CONTROL = 0x0008; //winuser.h
        public const uint MK_SHIFT = 0x0004;
        public const int MAX_PATH = 260; // windef.h    

        /// <summary>
        /// Specifies options for a bitmap image associated with a task item.
        /// </summary>
        public enum VSTASKBITMAP
        {
            BMP_COMPILE = -1,
            BMP_SQUIGGLE = -2,
            BMP_COMMENT = -3,
            BMP_SHORTCUT = -4,
            BMP_USER = -5
        };

        public const int
        OLECLOSE_SAVEIFDIRTY = 0,
        OLECLOSE_NOSAVE = 1,
        OLECLOSE_PROMPTSAVE = 2;

        public const int
        OLEIVERB_PRIMARY = 0,
        OLEIVERB_SHOW = -1,
        OLEIVERB_OPEN = -2,
        OLEIVERB_HIDE = -3,
        OLEIVERB_UIACTIVATE = -4,
        OLEIVERB_INPLACEACTIVATE = -5,
        OLEIVERB_DISCARDUNDOSTATE = -6,
        OLEIVERB_PROPERTIES = -7;                

        public const int 
        OLE_E_OLEVERB = unchecked((int)0x80040000),
        OLE_E_ADVF = unchecked((int)0x80040001),
        OLE_E_ENUM_NOMORE = unchecked((int)0x80040002),
        OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003),
        OLE_E_NOCONNECTION = unchecked((int)0x80040004),
        OLE_E_NOTRUNNING = unchecked((int)0x80040005),
        OLE_E_NOCACHE = unchecked((int)0x80040006),
        OLE_E_BLANK = unchecked((int)0x80040007),
        OLE_E_CLASSDIFF = unchecked((int)0x80040008),
        OLE_E_CANT_GETMONIKER = unchecked((int)0x80040009),
        OLE_E_CANT_BINDTOSOURCE = unchecked((int)0x8004000A),
        OLE_E_STATIC = unchecked((int)0x8004000B),
        OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C),
        OLE_E_INVALIDRECT = unchecked((int)0x8004000D),
        OLE_E_WRONGCOMPOBJ = unchecked((int)0x8004000E),
        OLE_E_INVALIDHWND = unchecked((int)0x8004000F),
        OLE_E_NOT_INPLACEACTIVE = unchecked((int)0x80040010),
        OLE_E_CANTCONVERT = unchecked((int)0x80040011),
        OLE_E_NOSTORAGE = unchecked((int)0x80040012);

        public const int 
        DISP_E_UNKNOWNINTERFACE = unchecked((int)0x80020001),
        DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003),
        DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004),
        DISP_E_TYPEMISMATCH = unchecked((int)0x80020005),
        DISP_E_UNKNOWNNAME = unchecked((int)0x80020006),
        DISP_E_NONAMEDARGS = unchecked((int)0x80020007),
        DISP_E_BADVARTYPE = unchecked((int)0x80020008),
        DISP_E_EXCEPTION = unchecked((int)0x80020009),
        DISP_E_OVERFLOW = unchecked((int)0x8002000A),
        DISP_E_BADINDEX = unchecked((int)0x8002000B),
        DISP_E_UNKNOWNLCID = unchecked((int)0x8002000C),
        DISP_E_ARRAYISLOCKED = unchecked((int)0x8002000D),
        DISP_E_BADPARAMCOUNT = unchecked((int)0x8002000E),
        DISP_E_PARAMNOTOPTIONAL = unchecked((int)0x8002000F),
        DISP_E_BADCALLEE = unchecked((int)0x80020010),
        DISP_E_NOTACOLLECTION = unchecked((int)0x80020011),
        DISP_E_DIVBYZERO = unchecked((int)0x80020012),
        DISP_E_BUFFERTOOSMALL = unchecked((int)0x80020013);

                public const int
                OFN_READONLY =             unchecked((int)0x00000001),
                OFN_OVERWRITEPROMPT =      unchecked((int)0x00000002),
                OFN_HIDEREADONLY =         unchecked((int)0x00000004),
                OFN_NOCHANGEDIR =          unchecked((int)0x00000008),
                OFN_SHOWHELP =             unchecked((int)0x00000010),
                OFN_ENABLEHOOK =           unchecked((int)0x00000020),
                OFN_ENABLETEMPLATE =       unchecked((int)0x00000040),
                OFN_ENABLETEMPLATEHANDLE = unchecked((int)0x00000080),
                OFN_NOVALIDATE =           unchecked((int)0x00000100),
                OFN_ALLOWMULTISELECT =     unchecked((int)0x00000200),
                OFN_EXTENSIONDIFFERENT =   unchecked((int)0x00000400),
                OFN_PATHMUSTEXIST =        unchecked((int)0x00000800),
                OFN_FILEMUSTEXIST =        unchecked((int)0x00001000),
                OFN_CREATEPROMPT =         unchecked((int)0x00002000),
                OFN_SHAREAWARE =           unchecked((int)0x00004000),
                OFN_NOREADONLYRETURN =     unchecked((int)0x00008000),
                OFN_NOTESTFILECREATE =     unchecked((int)0x00010000),
                OFN_NONETWORKBUTTON =      unchecked((int)0x00020000),
                OFN_NOLONGNAMES =          unchecked((int)0x00040000),
                OFN_EXPLORER =             unchecked((int)0x00080000),
                OFN_NODEREFERENCELINKS =   unchecked((int)0x00100000),
                OFN_LONGNAMES =            unchecked((int)0x00200000),
                OFN_ENABLEINCLUDENOTIFY =  unchecked((int)0x00400000),
                OFN_ENABLESIZING =         unchecked((int)0x00800000),
                OFN_USESHELLITEM =         unchecked((int)0x01000000),
                OFN_DONTADDTORECENT =      unchecked((int)0x02000000),
                OFN_FORCESHOWHIDDEN =      unchecked((int)0x10000000);

        public const uint
        VSITEMID_NIL               = unchecked((uint)-1),
        VSITEMID_ROOT              = unchecked((uint)-2), 
        VSITEMID_SELECTION         = unchecked((uint)-3);

        public const uint VSCOOKIE_NIL               = 0;

        // for ISelectionContainer flags
        public const uint
        ALL = 0x1,
        SELECTED = 0x2;

        // for IVsSelectionEvents flags
        public const uint
        UndoManager = 0x0,
        WindowFrame = 0x1,
        DocumentFrame = 0x2,
        StartupProject = 0x3,
        PropertyBrowserSID = 0x4,
        UserContext = 0x5;

        // for READONLYSTATUS
        public const int
        ROSTATUS_NotReadOnly = 0x0,
        ROSTATUS_ReadOnly = 0x1,
        ROSTATUS_Unknown = unchecked((int)0xFFFFFFFF);

        public const int
        IEI_DoNotLoadDocData = 0x10000000;

        public static readonly Guid LOGVIEWID_Any             = new Guid(0xffffffff, 0xffff, 0xffff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff );
        public static readonly Guid LOGVIEWID_Primary         = Guid.Empty;
        public static readonly Guid LOGVIEWID_Debugging       = new Guid("{7651a700-06e5-11d1-8ebd-00a0c90f26ea}");
        public static readonly Guid LOGVIEWID_Code            = new Guid("{7651a701-06e5-11d1-8ebd-00a0c90f26ea}");
        public static readonly Guid LOGVIEWID_Designer        = new Guid("{7651a702-06e5-11d1-8ebd-00a0c90f26ea}");
        public static readonly Guid LOGVIEWID_TextView        = new Guid("{7651a703-06e5-11d1-8ebd-00a0c90f26ea}");
        public static readonly Guid LOGVIEWID_UserChooseView  = new Guid("{7651a704-06e5-11d1-8ebd-00a0c90f26ea}");

        /// <summary>Command Group GUID for commands that only apply to the UIHierarchyWindow.</summary>
        public static readonly Guid GUID_VsUIHierarchyWindowCmds = new Guid("{60481700-078b-11d1-aaf8-00a0c9055a90}");
        /// <summary>
        /// The following commands are special commands that only apply to the UIHierarchyWindow.
        /// They are defined as part of the command group GUID: GUID_VsUIHierarchyWindowCmds.
        /// </summary>
        public enum VsUIHierarchyWindowCmdIds
        {
            /// <summary></summary>
            UIHWCMDID_RightClick        = 1,
            /// <summary></summary>
            UIHWCMDID_DoubleClick       = 2,
            /// <summary></summary>
            UIHWCMDID_EnterKey          = 3,
            /// <summary></summary>
            UIHWCMDID_StartLabelEdit    = 4,
            /// <summary></summary>
            UIHWCMDID_CommitLabelEdit   = 5,
            /// <summary></summary>
            UIHWCMDID_CancelLabelEdit   = 6
        }

        /// <summary>
        /// These element IDs are the only element IDs that can be used with the selection service.
        /// </summary>
        public enum VSSELELEMID
        {
            /// <summary></summary>
            SEID_UndoManager        = 0,
            /// <summary></summary>
            SEID_WindowFrame        = 1,
            /// <summary></summary>
            SEID_DocumentFrame      = 2,
            /// <summary></summary>
            SEID_StartupProject     = 3,
            /// <summary></summary>
            SEID_PropertyBrowserSID = 4,
            /// <summary></summary>
            SEID_UserContext        = 5,
            /// <summary></summary>
            SEID_ResultList         = 6,
            /// <summary></summary>
            SEID_LastWindowFrame    = 7
        }

                public const int
                CB_SETDROPPEDWIDTH = 0x0160,

                GWL_STYLE = (-16),
                GWL_EXSTYLE = (-20),

                DWL_MSGRESULT = 0,

                SW_SHOWNORMAL = 1,

                HTMENU = 5,

                WS_POPUP = unchecked((int)0x80000000),
                WS_CHILD = 0x40000000,
                WS_MINIMIZE = 0x20000000,
                WS_VISIBLE = 0x10000000,
                WS_DISABLED = 0x08000000,
                WS_CLIPSIBLINGS = 0x04000000,
                WS_CLIPCHILDREN = 0x02000000,
                WS_MAXIMIZE = 0x01000000,
                WS_CAPTION = 0x00C00000,
                WS_BORDER = 0x00800000,
                WS_DLGFRAME = 0x00400000,
                WS_VSCROLL = 0x00200000,
                WS_HSCROLL = 0x00100000,
                WS_SYSMENU = 0x00080000,
                WS_THICKFRAME = 0x00040000,
                WS_TABSTOP = 0x00010000,
                WS_MINIMIZEBOX = 0x00020000,
                WS_MAXIMIZEBOX = 0x00010000,
                WS_EX_DLGMODALFRAME = 0x00000001,
                WS_EX_MDICHILD = 0x00000040,
                WS_EX_TOOLWINDOW = 0x00000080,
                WS_EX_CLIENTEDGE = 0x00000200,
                WS_EX_CONTEXTHELP = 0x00000400,
                WS_EX_RIGHT = 0x00001000,
                WS_EX_LEFT = 0x00000000,
                WS_EX_RTLREADING = 0x00002000,
                WS_EX_LEFTSCROLLBAR = 0x00004000,
                WS_EX_CONTROLPARENT = 0x00010000,
                WS_EX_STATICEDGE = 0x00020000,
                WS_EX_APPWINDOW = 0x00040000,
                WS_EX_LAYERED = 0x00080000,
                WS_EX_TOPMOST = 0x00000008,
                WS_EX_NOPARENTNOTIFY = 0x00000004,

                LVM_SETEXTENDEDLISTVIEWSTYLE = (0x1000 + 54),

                LVS_EX_LABELTIP = 0x00004000,

                // winuser.h
                        WH_JOURNALPLAYBACK = 1,
                WH_GETMESSAGE = 3,
                WH_MOUSE = 7,
                WSF_VISIBLE = 0x0001,
                WM_NULL = 0x0000,
                WM_CREATE = 0x0001,
                WM_DELETEITEM = 0x002D,
                WM_DESTROY = 0x0002,
                WM_MOVE = 0x0003,
                WM_SIZE = 0x0005,
                WM_ACTIVATE = 0x0006,
                WA_INACTIVE = 0,
                WA_ACTIVE = 1,
                WA_CLICKACTIVE = 2,
                WM_SETFOCUS = 0x0007,
                WM_KILLFOCUS = 0x0008,
                WM_ENABLE = 0x000A,
                WM_SETREDRAW = 0x000B,
                WM_SETTEXT = 0x000C,
                WM_GETTEXT = 0x000D,
                WM_GETTEXTLENGTH = 0x000E,
                WM_PAINT = 0x000F,
                WM_CLOSE = 0x0010,
                WM_QUERYENDSESSION = 0x0011,
                WM_QUIT = 0x0012,
                WM_QUERYOPEN = 0x0013,
                WM_ERASEBKGND = 0x0014,
                WM_SYSCOLORCHANGE = 0x0015,
                WM_ENDSESSION = 0x0016,
                WM_SHOWWINDOW = 0x0018,
                WM_WININICHANGE = 0x001A,
                WM_SETTINGCHANGE = 0x001A,
                WM_DEVMODECHANGE = 0x001B,
                WM_ACTIVATEAPP = 0x001C,
                WM_FONTCHANGE = 0x001D,
                WM_TIMECHANGE = 0x001E,
                WM_CANCELMODE = 0x001F,
                WM_SETCURSOR = 0x0020,
                WM_MOUSEACTIVATE = 0x0021,
                WM_CHILDACTIVATE = 0x0022,
                WM_QUEUESYNC = 0x0023,
                WM_GETMINMAXINFO = 0x0024,
                WM_PAINTICON = 0x0026,
                WM_ICONERASEBKGND = 0x0027,
                WM_NEXTDLGCTL = 0x0028,
                WM_SPOOLERSTATUS = 0x002A,
                WM_DRAWITEM = 0x002B,
                WM_MEASUREITEM = 0x002C,
                WM_VKEYTOITEM = 0x002E,
                WM_CHARTOITEM = 0x002F,
                WM_SETFONT = 0x0030,
                WM_GETFONT = 0x0031,
                WM_SETHOTKEY = 0x0032,
                WM_GETHOTKEY = 0x0033,
                WM_QUERYDRAGICON = 0x0037,
                WM_COMPAREITEM = 0x0039,
                WM_GETOBJECT = 0x003D,
                WM_COMPACTING = 0x0041,
                WM_COMMNOTIFY = 0x0044,
                WM_WINDOWPOSCHANGING = 0x0046,
                WM_WINDOWPOSCHANGED = 0x0047,
                WM_POWER = 0x0048,
                WM_COPYDATA = 0x004A,
                WM_CANCELJOURNAL = 0x004B,
                WM_NOTIFY = 0x004E,
                WM_INPUTLANGCHANGEREQUEST = 0x0050,
                WM_INPUTLANGCHANGE = 0x0051,
                WM_TCARD = 0x0052,
                WM_HELP = 0x0053,
                WM_USERCHANGED = 0x0054,
                WM_NOTIFYFORMAT = 0x0055,
                WM_CONTEXTMENU = 0x007B,
                WM_STYLECHANGING = 0x007C,
                WM_STYLECHANGED = 0x007D,
                WM_DISPLAYCHANGE = 0x007E,
                WM_GETICON = 0x007F,
                WM_SETICON = 0x0080,
                WM_NCCREATE = 0x0081,
                WM_NCDESTROY = 0x0082,
                WM_NCCALCSIZE = 0x0083,
                WM_NCHITTEST = 0x0084,
                WM_NCPAINT = 0x0085,
                WM_NCACTIVATE = 0x0086,
                WM_GETDLGCODE = 0x0087,
                WM_NCMOUSEMOVE = 0x00A0,
                WM_NCLBUTTONDOWN = 0x00A1,
                WM_NCLBUTTONUP = 0x00A2,
                WM_NCLBUTTONDBLCLK = 0x00A3,
                WM_NCRBUTTONDOWN = 0x00A4,
                WM_NCRBUTTONUP = 0x00A5,
                WM_NCRBUTTONDBLCLK = 0x00A6,
                WM_NCMBUTTONDOWN = 0x00A7,
                WM_NCMBUTTONUP = 0x00A8,
                WM_NCMBUTTONDBLCLK = 0x00A9,
                WM_NCXBUTTONDOWN = 0x00AB,
                WM_NCXBUTTONUP = 0x00AC,
                WM_NCXBUTTONDBLCLK = 0x00AD,
                WM_KEYFIRST = 0x0100,
                WM_KEYDOWN = 0x0100,
                WM_KEYUP = 0x0101,
                WM_CHAR = 0x0102,
                WM_DEADCHAR = 0x0103,
                WM_CTLCOLOR = 0x0019,
                WM_SYSKEYDOWN = 0x0104,
                WM_SYSKEYUP = 0x0105,
                WM_SYSCHAR = 0x0106,
                WM_SYSDEADCHAR = 0x0107,
                WM_KEYLAST = 0x0108,
                WM_IME_STARTCOMPOSITION = 0x010D,
                WM_IME_ENDCOMPOSITION = 0x010E,
                WM_IME_COMPOSITION = 0x010F,
                WM_IME_KEYLAST = 0x010F,
                WM_INITDIALOG = 0x0110,
                WM_COMMAND = 0x0111,
                WM_SYSCOMMAND = 0x0112,
                WM_TIMER = 0x0113,
                WM_HSCROLL = 0x0114,
                WM_VSCROLL = 0x0115,
                WM_INITMENU = 0x0116,
                WM_INITMENUPOPUP = 0x0117,
                WM_MENUSELECT = 0x011F,
                WM_MENUCHAR = 0x0120,
                WM_ENTERIDLE = 0x0121,
                WM_CHANGEUISTATE = 0x0127,
                WM_UPDATEUISTATE = 0x0128,
                WM_QUERYUISTATE = 0x0129,
                WM_CTLCOLORMSGBOX = 0x0132,
                WM_CTLCOLOREDIT = 0x0133,
                WM_CTLCOLORLISTBOX = 0x0134,
                WM_CTLCOLORBTN = 0x0135,
                WM_CTLCOLORDLG = 0x0136,
                WM_CTLCOLORSCROLLBAR = 0x0137,
                WM_CTLCOLORSTATIC = 0x0138,
                WM_MOUSEFIRST = 0x0200,
                WM_MOUSEMOVE = 0x0200,
                WM_LBUTTONDOWN = 0x0201,
                WM_LBUTTONUP = 0x0202,
                WM_LBUTTONDBLCLK = 0x0203,
                WM_RBUTTONDOWN = 0x0204,
                WM_RBUTTONUP = 0x0205,
                WM_RBUTTONDBLCLK = 0x0206,
                WM_MBUTTONDOWN = 0x0207,
                WM_MBUTTONUP = 0x0208,
                WM_MBUTTONDBLCLK = 0x0209,
                WM_XBUTTONDOWN = 0x020B,
                WM_XBUTTONUP = 0x020C,
                WM_XBUTTONDBLCLK = 0x020D,
                WM_MOUSEWHEEL = 0x020A,
                WM_MOUSELAST = 0x020A,
                WM_PARENTNOTIFY = 0x0210,
                WM_ENTERMENULOOP = 0x0211,
                WM_EXITMENULOOP = 0x0212,
                WM_NEXTMENU = 0x0213,
                WM_SIZING = 0x0214,
                WM_CAPTURECHANGED = 0x0215,
                WM_MOVING = 0x0216,
                WM_POWERBROADCAST = 0x0218,
                WM_DEVICECHANGE = 0x0219,
                WM_IME_SETCONTEXT = 0x0281,
                WM_IME_NOTIFY = 0x0282,
                WM_IME_CONTROL = 0x0283,
                WM_IME_COMPOSITIONFULL = 0x0284,
                WM_IME_SELECT = 0x0285,
                WM_IME_CHAR = 0x0286,
                WM_IME_KEYDOWN = 0x0290,
                WM_IME_KEYUP = 0x0291,
                WM_MDICREATE = 0x0220,
                WM_MDIDESTROY = 0x0221,
                WM_MDIACTIVATE = 0x0222,
                WM_MDIRESTORE = 0x0223,
                WM_MDINEXT = 0x0224,
                WM_MDIMAXIMIZE = 0x0225,
                WM_MDITILE = 0x0226,
                WM_MDICASCADE = 0x0227,
                WM_MDIICONARRANGE = 0x0228,
                WM_MDIGETACTIVE = 0x0229,
                WM_MDISETMENU = 0x0230,
                WM_ENTERSIZEMOVE = 0x0231,
                WM_EXITSIZEMOVE = 0x0232,
                WM_DROPFILES = 0x0233,
                WM_MDIREFRESHMENU = 0x0234,
                WM_MOUSEHOVER = 0x02A1,
                WM_MOUSELEAVE = 0x02A3,
                WM_CUT = 0x0300,
                WM_COPY = 0x0301,
                WM_PASTE = 0x0302,
                WM_CLEAR = 0x0303,
                WM_UNDO = 0x0304,
                WM_RENDERFORMAT = 0x0305,
                WM_RENDERALLFORMATS = 0x0306,
                WM_DESTROYCLIPBOARD = 0x0307,
                WM_DRAWCLIPBOARD = 0x0308,
                WM_PAINTCLIPBOARD = 0x0309,
                WM_VSCROLLCLIPBOARD = 0x030A,
                WM_SIZECLIPBOARD = 0x030B,
                WM_ASKCBFORMATNAME = 0x030C,
                WM_CHANGECBCHAIN = 0x030D,
                WM_HSCROLLCLIPBOARD = 0x030E,
                WM_QUERYNEWPALETTE = 0x030F,
                WM_PALETTEISCHANGING = 0x0310,
                WM_PALETTECHANGED = 0x0311,
                WM_HOTKEY = 0x0312,
                WM_PRINT = 0x0317,
                WM_PRINTCLIENT = 0x0318,
                WM_HANDHELDFIRST = 0x0358,
                WM_HANDHELDLAST = 0x035F,
                WM_AFXFIRST = 0x0360,
                WM_AFXLAST = 0x037F,
                WM_PENWINFIRST = 0x0380,
                WM_PENWINLAST = 0x038F,
                WM_APP = unchecked((int)0x8000),
                WM_USER = 0x0400,
                WM_REFLECT =
                WM_USER + 0x1C00,
                WS_OVERLAPPED = 0x00000000,
                WPF_SETMINPOSITION = 0x0001,
                WM_CHOOSEFONT_GETLOGFONT = (0x0400 + 1),

                WHEEL_DELTA = 120,
                DWLP_MSGRESULT = 0,
                PSNRET_NOERROR = 0,
                PSNRET_INVALID = 1,
                PSNRET_INVALID_NOCHANGEPAGE = 2;

        public const int 
        PSN_APPLY = ((0-200)-2),
        PSN_KILLACTIVE = ((0-200)-1),
        PSN_RESET = ((0-200)-3),
        PSN_SETACTIVE = ((0-200)-0);

        public const int 
        GMEM_MOVEABLE = 0x0002,
        GMEM_ZEROINIT = 0x0040,
        GMEM_DDESHARE = 0x2000;

        public const int
        SWP_NOACTIVATE = 0x0010,
        SWP_NOZORDER = 0x0004,
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_FRAMECHANGED = 0x0020;

        public const int
        TVM_SETINSERTMARK = (0x1100 + 26),
        TVM_GETEDITCONTROL = (0x1100 + 15);

        public const int
        FILE_ATTRIBUTE_READONLY = 0x00000001;

        public const uint
        CEF_CLONEFILE   = 0x00000001,   // Mutually exclusive w/_OPENFILE
        CEF_OPENFILE    = 0x00000002,   // Mutually exclusive w/_CLONEFILE
        CEF_SILENT      = 0x00000004,   // Editor factory should create editor silently
        CEF_OPENASNEW   = 0x00000008;   // Editor factory should perform necessary fixups

        public const int cmdidToolsOptions    = 264;

        public const int 
            PSP_DEFAULT                = 0x00000000,
            PSP_DLGINDIRECT            = 0x00000001,
            PSP_USEHICON               = 0x00000002,
            PSP_USEICONID              = 0x00000004,
            PSP_USETITLE               = 0x00000008,
            PSP_RTLREADING             = 0x00000010,
            PSP_HASHELP                = 0x00000020,
            PSP_USEREFPARENT           = 0x00000040,
            PSP_USECALLBACK            = 0x00000080,
            PSP_PREMATURE              = 0x00000400,
            PSP_HIDEHEADER             = 0x00000800,
            PSP_USEHEADERTITLE         = 0x00001000,
            PSP_USEHEADERSUBTITLE      = 0x00002000;

        public const int
            PSH_DEFAULT             = 0x00000000,
            PSH_PROPTITLE           = 0x00000001,
            PSH_USEHICON            = 0x00000002,
            PSH_USEICONID           = 0x00000004,
            PSH_PROPSHEETPAGE       = 0x00000008,
            PSH_WIZARDHASFINISH     = 0x00000010,
            PSH_WIZARD              = 0x00000020,
            PSH_USEPSTARTPAGE       = 0x00000040,
            PSH_NOAPPLYNOW          = 0x00000080,
            PSH_USECALLBACK         = 0x00000100,
            PSH_HASHELP             = 0x00000200,
            PSH_MODELESS            = 0x00000400,
            PSH_RTLREADING          = 0x00000800,
            PSH_WIZARDCONTEXTHELP   = 0x00001000,
            PSH_WATERMARK           = 0x00008000,
            PSH_USEHBMWATERMARK     = 0x00010000,  // user pass in a hbmWatermark instead of pszbmWatermark
            PSH_USEHPLWATERMARK     = 0x00020000,  //
            PSH_STRETCHWATERMARK    = 0x00040000,  // stretchwatermark also applies for the header
            PSH_HEADER              = 0x00080000,
            PSH_USEHBMHEADER        = 0x00100000,
            PSH_USEPAGELANG         = 0x00200000,  // use frame dialog template matched to page
            PSH_WIZARD_LITE         = 0x00400000,
            PSH_NOCONTEXTHELP       = 0x02000000;

        public const int
            PSBTN_BACK              = 0,
            PSBTN_NEXT              = 1,
            PSBTN_FINISH            = 2,
            PSBTN_OK                = 3,
            PSBTN_APPLYNOW          = 4,
            PSBTN_CANCEL            = 5,
            PSBTN_HELP              = 6,
            PSBTN_MAX               = 6;

        public const int
            TRANSPARENT = 1,
            OPAQUE = 2,
            FW_BOLD = 700;

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        public class LOGFONT {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
            public string   lfFaceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public int idFrom;
            public int code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r)
            {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT {
            public int x;
            public int y;

            public POINT() {
            }

            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }
        
        /// <devdoc>
        /// Helper class for setting the text parameters to OLECMDTEXT structures.
        /// </devdoc>
        public sealed class OLECMDTEXT {

            // Private constructor to avoid creation of objects from this class.
            private OLECMDTEXT() {}

            /// <summary>
            /// Flags for the OLE command text
            /// </summary>
            public enum OLECMDTEXTF
            {
                /// <summary>No flag</summary>
                OLECMDTEXTF_NONE        = 0,
                /// <summary>The name of the command is required.</summary>
                OLECMDTEXTF_NAME = 1,
                /// <summary>A description of the status is required.</summary>
                OLECMDTEXTF_STATUS = 2
            }

            /// <summary>
            /// Gets the flags of the OLECMDTEXT structure
            /// </summary>
            /// <param name="pCmdTextInt">The structure to read.</param>
            /// <returns>The value of the flags.</returns>
            public static OLECMDTEXTF GetFlags(IntPtr pCmdTextInt)
            {
                Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));

                if ( (pCmdText.cmdtextf & (int)OLECMDTEXTF.OLECMDTEXTF_NAME) != 0 )
                    return OLECMDTEXTF.OLECMDTEXTF_NAME;

                if ( (pCmdText.cmdtextf & (int)OLECMDTEXTF.OLECMDTEXTF_STATUS) != 0 )
                    return OLECMDTEXTF.OLECMDTEXTF_STATUS;

                return OLECMDTEXTF.OLECMDTEXTF_NONE;
            }

            /// <devdoc>
            /// Accessing the text of this structure is very cumbersome.  Instead, you may
            /// use this method to access an integer pointer of the structure.
            /// Passing integer versions of this structure is needed because there is no
            /// way to tell the common language runtime that there is extra data at the end of the structure.
            /// </devdoc>
            public static string GetText(IntPtr pCmdTextInt) {
                Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT  pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));

                // Get the offset to the rgsz param.
                //
                IntPtr offset = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");

                // Punt early if there is no text in the structure.
                //
                if (pCmdText.cwActual == 0) {
                    return String.Empty;
                }

                char[] text = new char[pCmdText.cwActual - 1];

                Marshal.Copy((IntPtr)((long)pCmdTextInt + (long)offset), text, 0, text.Length);

                StringBuilder s = new StringBuilder(text.Length);
                s.Append(text);
                return s.ToString();
            }

            /// <devdoc>
            /// Accessing the text of this structure is very cumbersome.  Instead, you may
            /// use this method to access an integer pointer of the structure.
            /// Passing integer versions of this structure is needed because there is no
            /// way to tell the common language runtime that there is extra data at the end of the structure.
            /// </devdoc>
            /// <summary>
            /// Sets the text inside the structure starting from an integer pointer.
            /// </summary>
            /// <param name="pCmdTextInt">The integer pointer to the position where to set the text.</param>
            /// <param name="text">The text to set.</param>
            public static void SetText(IntPtr pCmdTextInt, string text) {
                Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT  pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT) Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));
                char[]          menuText = text.ToCharArray();

                // Get the offset to the rgsz param.  This is where we will stuff our text
                //
                IntPtr offset = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");
                IntPtr offsetToCwActual = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "cwActual");

                // The max chars we copy is our string, or one less than the buffer size,
                // since we need a null at the end.
                //
                int maxChars = Math.Min((int)pCmdText.cwBuf - 1, menuText.Length);

                Marshal.Copy(menuText, 0, (IntPtr)((long)pCmdTextInt + (long)offset), maxChars);

                // append a null character
                Marshal.WriteInt16((IntPtr)((long)pCmdTextInt + (long)offset + maxChars * 2), 0);

                // write out the length
                // +1 for the null char
                Marshal.WriteInt32((IntPtr)((long)pCmdTextInt + (long)offsetToCwActual), maxChars + 1);
            }
        }

        /// <devdoc>
        /// OLECMDF enums for IOleCommandTarget
        /// </devdoc>
        public enum tagOLECMDF {
            OLECMDF_SUPPORTED    = 1, 
            OLECMDF_ENABLED      = 2, 
            OLECMDF_LATCHED      = 4, 
            OLECMDF_NINCHED      = 8,
            OLECMDF_INVISIBLE    = 16
        }

        /// <devdoc>
        /// Constants for stream usage.
        /// </devdoc>
        public sealed class StreamConsts {
            public const   int LOCK_WRITE = 0x1;
            public const   int LOCK_EXCLUSIVE = 0x2;
            public const   int LOCK_ONLYONCE = 0x4;
            public const   int STATFLAG_DEFAULT = 0x0;
            public const   int STATFLAG_NONAME = 0x1;
            public const   int STATFLAG_NOOPEN = 0x2;
            public const   int STGC_DEFAULT = 0x0;
            public const   int STGC_OVERWRITE = 0x1;
            public const   int STGC_ONLYIFCURRENT = 0x2;
            public const   int STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 0x4;
            public const   int STREAM_SEEK_SET = 0x0;
            public const   int STREAM_SEEK_CUR = 0x1;
            public const   int STREAM_SEEK_END = 0x2;
        }

        /// <devdoc>
        /// This class implements a managed Stream object on top
        /// of a COM IStream
        /// </devdoc>
        internal sealed class DataStreamFromComStream : Stream, IDisposable {

            private Microsoft.VisualStudio.OLE.Interop.IStream comStream;

            #if DEBUG
            private string creatingStack;
            #endif

            public DataStreamFromComStream(Microsoft.VisualStudio.OLE.Interop.IStream comStream) : base() {
                this.comStream = comStream;

                #if DEBUG
                creatingStack = Environment.StackTrace;
                #endif
            }

            public override long Position {
                get {
                    return Seek(0, SeekOrigin.Current);
                }

                set {
                    Seek(value, SeekOrigin.Begin);
                }
            }

            public override bool CanWrite {
                get {
                    return true;
                }
            }

            public override bool CanSeek {
                get {
                    return true;
                }
            }

            public override bool CanRead {
                get {
                    return true;
                }
            }

            public override long Length {
                get {
                    long curPos = this.Position;
                    long endPos = Seek(0, SeekOrigin.End);
                    this.Position = curPos;
                    return endPos - curPos;
                }
            }

            private void _NotImpl(string message) {
                NotSupportedException ex = new NotSupportedException(message, new ExternalException(String.Empty, NativeMethods.E_NOTIMPL));
                throw ex;
            }

            protected override void Dispose(bool disposing) {
                try {
                    if (disposing) {
                        if (comStream != null) {
                            Flush();
                        }
                    }
                    // Cannot close COM stream from finalizer thread.
                    comStream = null;
                }
                finally {
                    base.Dispose(disposing);
                }
            }

            public override void Flush() {
                if (comStream != null) {
                    try {
                        comStream.Commit(StreamConsts.STGC_DEFAULT);
                    }
                    catch {
                    }
                }
            }

            public override int Read(byte[] buffer, int index, int count) {
                uint bytesRead;
                byte[] b = buffer;

                if (index != 0) {
                    b = new byte[buffer.Length - index];
                    buffer.CopyTo(b, 0);
                }

                comStream.Read(b, (uint)count, out bytesRead);

                if (index != 0) {
                    b.CopyTo(buffer, index);
                }

                return (int)bytesRead;
            }

            public override void SetLength(long value) {
                ULARGE_INTEGER ul = new ULARGE_INTEGER();
                ul.QuadPart = (ulong)value;
                comStream.SetSize(ul);
            }

            public override long Seek(long offset, SeekOrigin origin) {
                LARGE_INTEGER l = new LARGE_INTEGER();
                ULARGE_INTEGER[] ul = new ULARGE_INTEGER[1];
                ul[0] = new ULARGE_INTEGER();
                l.QuadPart = offset;
                comStream.Seek(l, (uint)origin, ul);
                return (long)ul[0].QuadPart;
            }

            public override void Write(byte[] buffer, int index, int count) {
                uint bytesWritten;

                if (count > 0) {

                    byte[] b = buffer;

                    if (index != 0) {
                        b = new byte[buffer.Length - index];
                        buffer.CopyTo(b, 0);
                    }

                    comStream.Write(b, (uint)count, out bytesWritten);
                    if (bytesWritten != count)
                        // Didn't write enough bytes to IStream!
                        throw new IOException();

                    if (index != 0) {
                        b.CopyTo(buffer, index);
                    }
                }
            }

            ~DataStreamFromComStream() {
                #if DEBUG
                if (comStream != null) {
                    Debug.Fail("DataStreamFromComStream not closed.  Creating stack: " + creatingStack);
                }
                #endif
                // CANNOT CLOSE NATIVE STREAMS IN FINALIZER THREAD
                // Close();
            }
        }

        /// <devdoc>
        /// Class that encapsulates a connection point cookie for COM event handling.
        /// </devdoc>
        public sealed class ConnectionPointCookie : IDisposable {
            private Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer cpc;
            private Microsoft.VisualStudio.OLE.Interop.IConnectionPoint connectionPoint;
            private uint cookie;
            #if DEBUG
            private string callStack ="(none)";
            private Type   eventInterface;
            #endif

            /// <devdoc>
            /// Creates a connection point to of the given interface type.
            /// which will call on a managed code sink that implements that interface.
            /// </devdoc>
            public ConnectionPointCookie(object source, object sink, Type eventInterface) : this(source, sink, eventInterface, true){
            }

            ~ConnectionPointCookie(){
                #if DEBUG
                System.Diagnostics.Debug.Assert(connectionPoint == null || cookie == 0, "We should never finalize an active connection point. (Interface = " + eventInterface.FullName + "), allocating code (see stack) is responsible for unhooking the ConnectionPoint by calling Disconnect.  Hookup Stack =\r\n" +  callStack);
                #endif

                // We must pass false here because chances are that if someone
                // forgot to Dispose this object, the IConnectionPoint is likely to be 
                // a disconnected RCW at this point (for example, we are far along in a 
                // VS shutdown scenario).  The result will be a memory leak, which is the 
                // expected result for an undisposed IDisposable object.
                Dispose(false);
            }

            public void Dispose() {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing) {
                if (disposing) {
                    try {
                        if (connectionPoint != null && cookie != 0) {
                            connectionPoint.Unadvise(cookie);
                        }
                    }
                    finally {
                        this.cookie = 0;
                        this.connectionPoint = null;
                        this.cpc = null;
                    }
                }
            }

            /// <devdoc>
            /// Creates a connection point to of the given interface type.
            /// which will call on a managed code sink that implements that interface.
            /// </devdoc>
            public ConnectionPointCookie(object source, object sink, Type eventInterface, bool throwException){
                Exception ex = null;
                if (source is Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer) {
                    this.cpc = (Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer)source;

                    try {
                        Guid tmp = eventInterface.GUID;
                        cpc.FindConnectionPoint(ref tmp, out connectionPoint);
                    }
                    catch {
                        connectionPoint = null;
                    }

                    if (connectionPoint == null) {
                        ex = new ArgumentException(/* SR.GetString(SR.ConnectionPoint_SourceIF, eventInterface.Name)*/);
                    }
                    else if (sink == null || !eventInterface.IsInstanceOfType(sink)) {
                        ex = new InvalidCastException(/* SR.GetString(SR.ConnectionPoint_SinkIF)*/);
                    }
                    else {
                        try {
                            connectionPoint.Advise(sink, out cookie);
                        }
                        catch {
                            cookie = 0;
                            connectionPoint = null;
                            ex = new Exception(/*SR.GetString(SR.ConnectionPoint_AdviseFailed, eventInterface.Name)*/);
                        }
                    }
                }
                else {
                    ex = new InvalidCastException(/*SR.ConnectionPoint_SourceNotICP)*/);
                }


                if (throwException && (connectionPoint == null || cookie == 0)) {
                    if (ex == null) {
                        throw new ArgumentException(/*SR.GetString(SR.ConnectionPoint_CouldNotCreate, eventInterface.Name)*/);
                    }
                    else {
                        throw ex;
                    }
                }

                #if DEBUG
                callStack = Environment.StackTrace;
                this.eventInterface = eventInterface;
                #endif
            }
        }

        /// <devdoc>
        /// This method takes a file URL and converts it to an absolute path.  The trick here is that
        /// if there is a '#' in the path, everything after this is treated as a fragment.  So
        /// we need to append the fragment to the end of the path.
        /// </devdoc>
        public static string GetAbsolutePath(string fileName) {
            System.Diagnostics.Debug.Assert(fileName != null && fileName.Length > 0, "Cannot get absolute path, fileName is not valid");

            Uri uri = new Uri(fileName);
            return uri.LocalPath + uri.Fragment;
        }

        /// <devdoc>
        /// This method takes a file URL and converts it to a local path.  The trick here is that
        /// if there is a '#' in the path, everything after this is treated as a fragment.  So
        /// we need to append the fragment to the end of the path.
        /// </devdoc>
        public static string GetLocalPath(string fileName) {
            System.Diagnostics.Debug.Assert(fileName != null && fileName.Length > 0, "Cannot get local path, fileName is not valid");

            Uri uri = new Uri(fileName);
            return uri.LocalPath + uri.Fragment;
        }

        // VSWhidbey 460000
        // VSCore does not properly escape paths with the certain characters (for example, #) in 
        // the URIs they provide.  Instead, if the path starts with file://, we will assume
        // the rest of the string is the non-escaped path.
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string GetLocalPathUnescaped(string url) {
            string filePrefix = "file:///";
            if (url.StartsWith(filePrefix, StringComparison.OrdinalIgnoreCase) ) {
                return url.Substring(filePrefix.Length);
            }
            else {
                return GetLocalPath(url);
            }
        }

        /// <devdoc>
        /// Please use this "approved" method to compare file names.
        /// </devdoc>
        public static bool IsSamePath(string file1, string file2) {
            if (file1 == null || file1.Length == 0) {
                return (file2 == null || file2.Length == 0);
            }
            Uri uri1 = new Uri(file1);
            Uri uri2 = new Uri(file2);
            if (uri1.IsFile && uri2.IsFile) {
                return 0 == String.Compare(uri1.LocalPath, uri2.LocalPath, StringComparison.OrdinalIgnoreCase);
            }
            return file1 == file2;
        }

        [ComImport(),Guid("9BDA66AE-CA28-4e22-AA27-8A7218A0E3FA"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEventHandler {

            // converts the underlying codefunction into an event handler for the given event
            // if the given event is NULL, then the function will handle no events
            [PreserveSig]
            int AddHandler(string bstrEventName); 

            [PreserveSig]
            int RemoveHandler(string bstrEventName);

            IVsEnumBSTR GetHandledEvents();

            bool HandlesEvent(string bstrEventName);
        }

        [ComImport(),Guid("A55CCBCC-7031-432d-B30A-A68DE7BDAD75"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IParameterKind {

            void SetParameterPassingMode(PARAMETER_PASSING_MODE ParamPassingMode);
            void SetParameterArrayDimensions(int uDimensions);
            int GetParameterArrayCount();
            int GetParameterArrayDimensions(int uIndex);
            int GetParameterPassingMode();
        }

        public enum PARAMETER_PASSING_MODE
        {
            cmParameterTypeIn = 1,
            cmParameterTypeOut = 2,
            cmParameterTypeInOut = 3
        } 

        [
        ComImport, Guid("3E596484-D2E4-461a-A876-254C4F097EBB"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IMethodXML
        {
            // Generate XML describing the contents of this function's body.
            void GetXML (ref string pbstrXML);
            
            // Parse the incoming XML with respect to the CodeModel XML schema and
            // use the result to regenerate the body of the function.
            [PreserveSig]
            int SetXML (string pszXML);
            
            // This is really a textpoint
            [PreserveSig]
            int GetBodyPoint([MarshalAs(UnmanagedType.Interface)]out object bodyPoint);
        }

        [ComImport(),Guid("EA1A87AD-7BC5-4349-B3BE-CADC301F17A3"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IVBFileCodeModelEvents {
            
            [PreserveSig]
            int StartEdit(); 
            
            [PreserveSig]
            int EndEdit();
        }

        ///--------------------------------------------------------------------------
        /// ICodeClassBase:
        ///--------------------------------------------------------------------------
        [GuidAttribute("23BBD58A-7C59-449b-A93C-43E59EFC080C")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport()]
        public interface ICodeClassBase {
            [PreserveSig()]
            int GetBaseName(out string pBaseName);
        }

        // APIS

        /// <summary>
        /// The kernel32 GetFileAttributes function.
        /// </summary>
        /// <param name="name">The name of the file to get the attributes for.</param>
        /// <returns>The attributes of the file.</returns>
        [DllImport("Kernel32", CharSet=CharSet.Unicode, SetLastError=true)]   
        public static extern int GetFileAttributes(String name);

        /// <summary>
        /// Places a message in the message queue associated with the thread that created the 
        /// specified window and then returns without waiting for the thread to process the message. 
        /// </summary>
        /// <param name="hwnd">Handle to the window whose window procedure is to receive the message.</param>
        /// <param name="msg">Specifies the message to be posted.</param>
        /// <param name="wparam">Specifies additional message-specific information.</param>
        /// <param name="lparam">Specifies additional message-specific information.</param>
        /// <returns>Nonzero indicates success. Zero indicates failure.</returns>
        [DllImport("User32", CharSet=CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        public static IntPtr GetNativeWndProc(System.Windows.Forms.Control control) {
            IntPtr handle = control.Handle;
            return GetWindowLong(new HandleRef(control, handle), GWL_WNDPROC);
        }
        public const int GWL_WNDPROC = (-4);
        //GetWindowLong won't work correctly for 64-bit: we should use GetWindowLongPtr instead.  On
        //32-bit, GetWindowLongPtr is just #defined as GetWindowLong.  GetWindowLong really should 
        //take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        //it'll be OK.
        public static IntPtr GetWindowLong(HandleRef hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLong32(HandleRef hWnd, int nIndex);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr64(HandleRef hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, short nIndex, int value);

        /// <summary>
        /// Changes the parent window of the specified child window.
        /// </summary>
        /// <param name="hWnd">Handle to the child window.</param>
        /// <param name="hWndParent">Handle to the new parent window. If this parameter is NULL, the desktop window becomes the new parent window.</param>
        /// <returns>A handle to the previous parent window indicates success. NULL indicates failure.</returns>
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        /// <summary>
        /// Changes the size, position, and z-order of a child, pop-up, or top-level window.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="hWndInsertAfter">Handle to the window to precede the positioned window in the z-order.</param>
        /// <param name="x">Specifies the new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">Specifies the new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">Specifies the new width of the window, in pixels.</param>
        /// <param name="cy">Specifies the new height of the window, in pixels.</param>
        /// <param name="flags">Specifies the window sizing and positioning flags.</param>
        /// <returns>Nonzero indicates success. Zero indicates failure.</returns>
        [DllImport("User32", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, int flags);

        /// <summary>
        /// Sets the specified window’s show state.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">Specifies how the window is to be shown.</param>
        /// <returns>Nonzero indicates that the window was previously visible. Zero indicates that the window was previously hidden.</returns>
        [DllImport("User32", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // IDataObject stuff
        /// <summary>
        /// Allocates the specified number of bytes from the heap.
        /// </summary>
        /// <param name="uFlags">Memory allocation attributes.</param>
        /// <param name="dwBytes">Number of bytes to allocate.</param>
        /// <returns>If the function succeeds, the return value is a handle to the newly allocated memory object, NULL otherwise.</returns>
        [DllImport("Kernel32", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

        /// <summary>
        /// Changes the size or attributes of a specified global memory object.
        /// </summary>
        /// <param name="handle">Handle to the global memory object to be reallocated.</param>
        /// <param name="bytes">New size of the memory block, in bytes.</param>
        /// <param name="flags">Reallocation options.</param>
        /// <returns>If the function succeeds, the return value is a handle to the reallocated memory object, NULL otherwise.</returns>
        [DllImport("Kernel32", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern IntPtr GlobalReAlloc(HandleRef handle, int bytes, int flags);

        /// <summary>
        /// Locks a global memory object and returns a pointer to the first byte of the object's memory block.
        /// </summary>
        /// <param name="handle">Handle to the global memory object.</param>
        /// <returns>If the function succeeds, the return value is a pointer to the first byte of the memory block, NULL otherwise.</returns>
        [DllImport("Kernel32", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern IntPtr GlobalLock(HandleRef handle);

        /// <summary>
        /// Decrements the lock count associated with a memory object.
        /// </summary>
        /// <param name="handle">Handle to the global memory object.</param>
        /// <returns>If the memory object is still locked after decrementing the lock count, the return value is a nonzero value.</returns>
        [DllImport("Kernel32", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern bool GlobalUnlock(HandleRef handle);

        /// <summary>
        /// Frees the specified global memory object and invalidates its handle.
        /// </summary>
        /// <param name="handle">Handle to the global memory object.</param>
        /// <returns>If the function succeeds, the return value is NULL; if it fails the return value is equal to a handle to the global memory object.</returns>
        [DllImport("Kernel32", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern IntPtr GlobalFree(HandleRef handle);

        /// <summary>
        /// Retrieves the current size of the specified global memory object, in bytes.
        /// </summary>
        /// <param name="handle">Handle to the global memory object.</param>
        /// <returns>If the function succeeds, the return value is the size of the specified global memory object, in bytes; it is zero otherwise.</returns>
        [DllImport("Kernel32", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern int GlobalSize(HandleRef handle);

        /// <summary>
        /// Moves a string to a location in memory.
        /// </summary>
        /// <param name="pdst">Pointer to the starting address of the move destination.</param>
        /// <param name="psrc">The string to be moved</param>
        /// <param name="cb">Size of the block of memory to move, in bytes.</param>
        [DllImport("Kernel32", ExactSpelling=true, EntryPoint="RtlMoveMemory", CharSet=CharSet.Unicode)]
        public static extern void CopyMemoryW(IntPtr pdst, string psrc, int cb);

        /// <summary>
        /// Moves an array of char to a location in memory.
        /// </summary>
        /// <param name="pdst">Pointer to the starting address of the move destination.</param>
        /// <param name="psrc">The array to be moved</param>
        /// <param name="cb">Size of the block of memory to move, in bytes.</param>
        [DllImport("Kernel32", ExactSpelling=true, EntryPoint="RtlMoveMemory", CharSet=CharSet.Unicode)]
        public static extern void CopyMemoryW(IntPtr pdst, char[] psrc, int cb);

        /// <summary></summary>
        [DllImport("Kernel32", ExactSpelling=true, EntryPoint="RtlMoveMemory", CharSet=CharSet.Unicode)]
        public static extern void CopyMemoryW(StringBuilder pdst, HandleRef psrc, int cb);

        /// <summary></summary>
        [DllImport("Kernel32", ExactSpelling=true, EntryPoint="RtlMoveMemory", CharSet=CharSet.Unicode)]
        public static extern void CopyMemoryW(char[] pdst, HandleRef psrc, int cb);

        /// <summary>
        /// Moves an array of bytes to a location in memory.
        /// </summary>
        /// <param name="pdst">Pointer to the starting address of the move destination.</param>
        /// <param name="psrc">The array to be moved</param>
        /// <param name="cb">Size of the block of memory to move, in bytes.</param>
        [DllImport("Kernel32", ExactSpelling=true, EntryPoint="RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr pdst, byte[] psrc, int cb);

        /// <summary></summary>
        [DllImport("Kernel32", ExactSpelling=true, EntryPoint="RtlMoveMemory")]
        public static extern void CopyMemory(byte[] pdst, HandleRef psrc, int cb);

        /// <summary></summary>
        [DllImport("Kernel32", ExactSpelling=true, EntryPoint="RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr pdst, HandleRef psrc, int cb);

        /// <summary></summary>
        [DllImport("Kernel32", ExactSpelling=true, EntryPoint="RtlMoveMemory", CharSet=CharSet.Unicode)]
        public static extern void CopyMemory(IntPtr pdst, string psrc, int cb);

        /// <summary>
        /// Maps a wide-character string to a new character string.
        /// The new character string is not necessarily from a multibyte character set.
        /// </summary>
        /// <param name="codePage">Specifies the code page used to perform the conversion.</param>
        /// <param name="flags">Specifies the handling of unmapped characters.</param>
        /// <param name="wideStr">The wide-character string to be converted.</param>
        /// <param name="chars">The number of wide characters in the string pointed to by the wideStr parameter.</param>
        /// <param name="pOutBytes">Points to the buffer to receive the translated string.</param>
        /// <param name="bufferBytes">the size, in bytes, of the buffer pointed to by the pOutBytes parameter.</param>
        /// <param name="defaultChar">The character used if a wide character cannot be represented in the specified code page.</param>
        /// <param name="pDefaultUsed">Flag that indicates whether a default character was used.</param>
        /// <returns>If the function succeeded, the return value is the number of bytes written to the buffer pointed to by pOutBytes; zero otherwise.</returns>
        [DllImport("Kernel32", ExactSpelling=true, CharSet=CharSet.Unicode)]
        public static extern int WideCharToMultiByte(int codePage, int flags, [MarshalAs(UnmanagedType.LPWStr)]string wideStr, int chars, [In,Out]byte[] pOutBytes, int bufferBytes, IntPtr defaultChar, IntPtr pDefaultUsed);

        [DllImport("user32.dll", EntryPoint = "IsDialogMessageA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool IsDialogMessageA(IntPtr hDlg, ref MSG msg);
        [DllImport("user32.dll")]
        public static extern void SetFocus(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();
        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hwnd, IntPtr rect, bool erase);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClientRect")]
        public static extern int GetClientRect(IntPtr hWnd, ref RECT rect);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);


        [DllImport("oleaut32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SafeArrayCreate(VarEnum vt, UInt32 cDims, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] SAFEARRAYBOUND[] rgsabound);
        [DllImport("oleaut32.dll", CharSet = CharSet.Auto, PreserveSig = false)]
        public static extern void SafeArrayPutElement(IntPtr psa, [MarshalAs(UnmanagedType.LPArray)] long[] rgIndices, IntPtr pv);

        /// <summary>
        /// Compares two COM objects to see if they represent the same underlying object.
        /// </summary>
        public static bool IsSameComObject(object obj1, object obj2) {
            IntPtr iunknown1 = IntPtr.Zero;
            IntPtr iunknown2 = IntPtr.Zero;

            try {
                if (obj1 != null)
                    iunknown1 = Marshal.GetIUnknownForObject(obj1);

                if (obj2 != null)
                    iunknown2 = Marshal.GetIUnknownForObject(obj2);

                // This may not work if one of the objects is managed and aggregated. In that case
                // Marshal.GetIUnknownForObject returns the inner IUnknown, and you have to call
                // Marshal.QueryInterface with IID_IUnknown to obtain the controlling IUnknown
                // suitable for identity verification.
                return iunknown1 == iunknown2;
            } finally {
                if (iunknown1 != IntPtr.Zero)
                    Marshal.Release(iunknown1);

                if (iunknown2 != IntPtr.Zero)
                    Marshal.Release(iunknown2);
            }
        }

    }



}

