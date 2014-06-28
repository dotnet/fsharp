// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.VisualStudio
open System.Runtime.InteropServices
open Internal.Utilities.Debug
open System

/// Helper methods for interoperating with COM                
module internal Com = 
   /// Execute managed code and return COM error information
    let Method methodDescription f = 
        Trace.Print("LanguageService", fun () -> sprintf "Enter %s\n" methodDescription)
        try 
            f() 
            Trace.Print("LanguageService", fun () -> sprintf "Exit %s normally\n" methodDescription)
            VSConstants.S_OK
        with e -> 
            Trace.Print("LanguageService", fun () -> sprintf "Exit %s with exception %A\n" methodDescription e)
            VSConstants.E_FAIL

    let ThrowOnFailure0(hr) = 
        ErrorHandler.ThrowOnFailure(hr)  |> ignore
        
    let ThrowOnFailure1(hr,res) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res
        
    let ThrowOnFailure2(hr,res1,res2) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res1,res2
        
    let ThrowOnFailure3(hr,res1,res2,res3) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res1,res2,res3

    let ThrowOnFailure4(hr,res1,res2,res3,res4) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res1,res2,res3,res4
        
    let ThrowOnFailure7(hr,res1,res2,res3,res4,res5,res6,res7) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res1,res2,res3,res4,res5,res6,res7
        
    let Succeeded hr = 
        // REVIEW: Not the correct check for succeeded
        hr = VSConstants.S_OK
        
    let BoolToHResult = function
          true -> VSConstants.S_OK 
        | false -> VSConstants.S_FALSE;            

#if DEBUG
    [<ComImport; InterfaceType(ComInterfaceType.InterfaceIsIUnknown); Guid("00020401-0000-0000-C000-000000000046")>]
    type ITypeInfo = 
      interface 
//        [<PreserveSig>]
//        int GetTypeAttr(out IntPtr pTypeAttr);
//        [<PreserveSig>]
//        int GetTypeComp(out UnsafeNativeMethods.ITypeComp pTComp);
//        [<PreserveSig>]
//        int GetFuncDesc([<In; MarshalAs<UnmanagedType.U4>>] int index, out IntPtr pFuncDesc);
//        [<PreserveSig>]
//        int GetVarDesc([<In; MarshalAs<UnmanagedType.U4>>] int index, out IntPtr pVarDesc);
//        [<PreserveSig>]
//        int GetNames([<In>] int memid, [<Out; MarshalAs<UnmanagedType.LPArray>>] string[] rgBstrNames, [<In; MarshalAs<UnmanagedType.U4>>] int cMaxNames, [<MarshalAs<UnmanagedType.U4>>] out int cNames);
//        [<PreserveSig>]
//        int GetIDsOfNames([<In>] IntPtr rgszNames, [<In, MarshalAs<UnmanagedType.U4>>] int cNames, out IntPtr pMemId);
        [<PreserveSig>]
        abstract GetDocumentation : [<In>] memid : int * [<MarshalAs(UnmanagedType.BStr)>] pBstrName: string byref * [<MarshalAs(UnmanagedType.BStr)>] pBstrDocString : string byref * [<MarshalAs(UnmanagedType.U4)>] pdwHelpContext : int byref * [<MarshalAs(UnmanagedType.BStr)>] pBstrHelpFile : string byref -> int
//        [<PreserveSig>]
//        int GetRefTypeInfo([<In>] IntPtr hreftype, out ITypeInfo pTypeInfo);
//        [<PreserveSig>]
//        int GetContainingTypeLib([<Out; MarshalAs(UnmanagedType.LPArray)] ITypeLib[] ppTLib, [<Out; MarshalAs<UnmanagedType.LPArray>>] int[] pIndex);
//        [<PreserveSig>]
//        void ReleaseTypeAttr(IntPtr typeAttr);
//        [<PreserveSig>]
//        void ReleaseFuncDesc(IntPtr funcDesc);
//        [<PreserveSig>]
//        void ReleaseVarDesc(IntPtr varDesc);
      end

    [<ComImport; InterfaceType(ComInterfaceType.InterfaceIsIUnknown); Guid("00020400-0000-0000-C000-000000000046")>]
    type IDispatch =
      interface
        [<PreserveSig>]
        abstract GetTypeInfo: [<In>] index : int * [<In>] lcid : int * [<MarshalAs(UnmanagedType.Interface)>] pTypeInfo:ITypeInfo byref -> int
//        [PreserveSig]
//        int GetIDsOfNames();
//        [PreserveSig]
//        int Invoke();
    end

    let TrueTypeName(o) = 
        let o = box o
        let simpletypename = (o.GetType()).FullName
        match o with
        | :? IDispatch as dispatch -> 
            let pTypeInfo : ITypeInfo = unbox null
            let pBstrName = ""
            let pBstrDocString = ""
            let pBstrHelpFile = ""
            let num = 0
            if Succeeded(dispatch.GetTypeInfo(0,0x409,ref pTypeInfo)) then
                if Succeeded(pTypeInfo.GetDocumentation(-1, ref pBstrName, ref pBstrDocString, ref num, ref pBstrHelpFile)) then
                    pBstrName
                else simpletypename
            else simpletypename
        | _ -> simpletypename

    let DiscoverKnownInterfaces o = 
        let CanQueryInterface iunk (iid:string) = 
            let mutable iid = Guid(iid)
            let hr,i = Marshal.QueryInterface(iunk,ref iid)
            try 
                Succeeded hr
            finally
                if IntPtr.Zero <> i then
                    Marshal.Release(i) |> ignore
            
        let knownInterfaces = 
          [ ("C7FEDB89-B36D-4a62-93F4-DC7A95999921","ICSharpProjectRoot") 
            ("1F3B9583-A66A-4be1-A15B-901DA4DB4ACF","ICSharpProjectHost")
            ("D4F3F4B1-E900-4e51-ADB3-D532348F83CB","new IVsPackage")
            ("7d960b00-7af8-11d0-8e5e-00a0c911005a","old IVsPackage")
            ("A7A1C907-C3D2-4acb-9114-4EE23B6FCF7E","new IVsToolWindowFactory")
            ("94E6E7DE-F418-11d2-B6FA-00C04F9901D1","old IVsToolWindowFactory")
            ("53BA0F89-24DD-46e1-A7D6-ED24C039FBC4","new IVsPersistSolutionOpts")
            ("45CF6805-93EB-11D0-AF4C-00A0C90F9DE6","old IVsPersistSolutionOpts")
            ("0D0E68EA-C910-45a7-8C24-7BBFA7D2D201","new IVsPersistSolutionProps")
            ("45CF6806-93EB-11D0-AF4C-00A0C90F9DE6","old IVsPersistSolutionProps")
            ("8D2EC486-8098-4afa-AB94-D270A5EF08CE","new IVsPersistSolutionProps2")
            ("8D2EC486-8098-4afa-AB94-D270A5EF08CE","old IVsPersistSolutionProps2")
            ("67A65088-52F3-4c47-B829-1B53A112E8DC","new IVsSolutionPersistence")
            ("45CF6807-93EB-11D0-AF4C-00A0C90F9DE6","old IVsSolutionPersistence")
            ("33FCD00A-BD45-403c-9C66-07BA9A923501","new IVsProjectFactory")
            ("7d960b05-7af8-11d0-8e5e-00a0c911005a","old IVsProjectFactory")
            ("8CBFFBBE-241E-4b9c-9926-C06F7374386C","new IVsNonSolutionProjectFactory")
            ("BC798C3A-4EB9-11d3-B2BF-00C04F688E57","old IVsNonSolutionProjectFactory")
            ("F08400BB-0960-47f4-9E12-591DBF370546","new IVsRegisterProjectTypes")
            ("7d960b06-7af8-11d0-8e5e-00a0c911005a","old IVsRegisterProjectTypes")
            ("E4197123-1086-4d51-B2D5-903F4D61C5AA","new IVsOwnedProjectFactory")
            ("70F026F7-E043-4634-9DFF-C1ED96C264D6","old IVsOwnedProjectFactory")
            ("59B2D1D0-5DB0-4f9f-9609-13F0168516D6","new IVsHierarchy")
            ("7d960b01-7af8-11d0-8e5e-00a0c911005a","old IVsHierarchy")
            ("E82609EA-5169-47f4-91D0-6957272CBE9F","new IVsUIHierarchy")
            ("7d960b02-7af8-11d0-8e5e-00a0c911005a","old IVsUIHierarchy")
            ("6DDD8DC3-32B2-4bf1-A1E1-B6DA40526D1E","new IVsHierarchyEvents")
            ("7d960b03-7af8-11d0-8e5e-00a0c911005a","old IVsHierarchyEvents")
            ("8FE0E50A-785A-4a50-8EDB-1D054D68EF87","new IVsParentHierarchy")
            ("92D73940-C541-11d2-8598-006097C68E81","old IVsParentHierarchy")
            ("7F7CD0DB-91EF-49dc-9FA9-02D128515DD4","new IVsSolution")
            ("054AECC1-AC4D-11d0-AF54-00A0C90F9DE6","old IVsSolution")
            ("95C6A090-BB9E-4bf2-B0BE-F1D04F0ECEA3","new IVsSolution2")
            ("95C6A090-BB9E-4bf2-B0BE-F1D04F0ECEA3","old IVsSolution2")
            ("58DCF7BF-F14E-43ec-A7B2-9F78EDD06418","new IVsSolution3")
            ("58DCF7BF-F14E-43ec-A7B2-9F78EDD06418","old IVsSolution3")
            ("A8516B56-7421-4dbd-AB87-57AF7A2E85DE","new IVsSolutionEvents")
            ("054AECC2-AC4D-11d0-AF54-00A0C90F9DE6","old IVsSolutionEvents")
            ("A711DF67-B00A-4e82-A990-51B2B450EA0F","new IVsSolutionEvents2")
            ("710932AF-2116-4cbd-8E48-0C5944EF0C6A","old IVsSolutionEvents2")
            ("F1DE2D75-3B95-4510-9B2B-565BC0E38877","new IVsSolutionEvents3")
            ("F1DE2D75-3B95-4510-9B2B-565BC0E38877","old IVsSolutionEvents3")
            ("23EC4D20-54A9-4365-82C8-ABDFBA686ECF","new IVsSolutionEvents4")
            ("23EC4D20-54A9-4365-82C8-ABDFBA686ECF","old IVsSolutionEvents4")
            ("A4662D0F-FA14-48ac-8E68-D481EF200627","new IVsFireSolutionEvents")
            ("054AECC2-AC4D-11d0-AF54-00A0C90F9DE6","old IVsFireSolutionEvents")
            ("ED6AAB26-108F-4b4f-A57B-14D20982713D","new IVsFireSolutionEvents2")
            ("ED6AAB26-108F-4b4f-A57B-14D20982713D","old IVsFireSolutionEvents2")
            ("925E8559-17DF-494c-87DA-BBEE251702DE","new IVsPrioritizedSolutionEvents")
            ("925E8559-17DF-494c-87DA-BBEE251702DE","old IVsPrioritizedSolutionEvents")
            ("7B1D55C6-4F6A-4865-B9B3-1A696E233065","new IVsSolutionEventsProjectUpgrade")
            ("7B1D55C6-4F6A-4865-B9B3-1A696E233065","old IVsSolutionEventsProjectUpgrade")
            ("CD4028ED-C4D8-44ba-890F-E7FB02A380C6","new IVsProject")
            ("625911f3-af99-11d0-8e69-00a0c911005a","old IVsProject")
            ("4AF886C3-7796-4c81-A174-4A87080DEE58","new IVsProject2")
            ("669B7232-890C-11d1-BC18-0000F87552E7","old IVsProject2")
            ("1C11116E-4FF2-4a80-82DC-69F95042E0A4","new IVsProject3")
            ("36201871-BC59-11d2-BFC9-00C04F990235","old IVsProject3")
            ("79001CD1-69C6-45b8-8F7A-DCCCE0469E8D","new IVsParentProject")
            ("79001CD1-69C6-45b8-8F7A-DCCCE0469E8D","old IVsParentProject")
            ("D63BB7D7-D7A0-4c02-AA85-7E9233797CDB","new IVsParentProject2")
            ("D63BB7D7-D7A0-4c02-AA85-7E9233797CDB","old IVsParentProject2")
            ("75661D39-F5DA-41b9-ABDA-9CF54C6B1AC9","new IVsProjectUpgrade")
            ("75661D39-F5DA-41b9-ABDA-9CF54C6B1AC9","old IVsProjectUpgrade")
            ("83B2961F-AC2B-409b-89BD-DCF698E3C402","new IVsDeferredSaveProject")
            ("83B2961F-AC2B-409b-89BD-DCF698E3C402","old IVsDeferredSaveProject")
            ("4B2BEBAA-BA1E-4479-8720-8CE19D276098","new IVsProjectSpecificEditorMap")
            ("21f29401-a80b-4a7f-b5c4-a9f9ca849447","old IVsProjectSpecificEditorMap")
            ("F84A6D1D-F305-4055-A02C-A642B871BB20","new IVsProjectSpecificEditorMap2")
            ("F84A6D1D-F305-4055-A02C-A642B871BB20","old IVsProjectSpecificEditorMap2")
            ("3F819030-50CF-4b72-B3FC-B3B9BFBBEE69","new IVsProjectResources")
            ("9c68abb3-d1e5-4986-a501-e1f446005a43","old IVsProjectResources")
            ("E09C9DCF-D4B7-4d6e-A676-1FC64B4BF6EB","new IVsSupportItemHandoff")
            ("3E7CBE01-C114-4291-80DE-7DCDE3AB0032","old IVsSupportItemHandoff")
            ("2AFA4F74-7A1A-4dee-8F99-46E74E5A3C0F","new IVsSupportItemHandoff2")
            ("2AFA4F74-7A1A-4dee-8F99-46E74E5A3C0F","old IVsSupportItemHandoff2")
            ("11DFCCEB-D935-4a9f-9796-5BA433C5AF8E","new IVsAddProjectItemDlg")
            ("a448e7a0-b830-11d0-9ffd-00a0c911e8e9","old IVsAddProjectItemDlg")
            ("6B90D260-E363-4e8a-AE51-BD19C493416D","new IVsAddProjectItemDlg2")
            ("8C73614F-7E67-11d2-BFB9-00C04F990235","old IVsAddProjectItemDlg2")
            ("D93A191C-525A-43bc-ACFD-7EF494143CF4","new IVsFilterAddProjectItemDlg")
            ("75437597-FE86-11d2-BECE-00C04F682A08","old IVsFilterAddProjectItemDlg")
            ("61116CFF-5319-440a-81CE-5D9F54A610DE","new IVsFilterAddProjectItemDlg2")
            ("61116CFF-5319-440a-81CE-5D9F54A610DE","old IVsFilterAddProjectItemDlg2")
            ("82A40D77-D2D4-4c93-AB11-8D50ADF02B1E","new IVsProjectTextImageProvider")
            ("6EF99245-719D-4d55-8955-7F9E9A1ADFD1","old IVsProjectTextImageProvider")
            ("C3E2ED14-4E64-4c26-84D7-68CCD071A0C8","new IVsSaveOptionsDlg")
            ("9B550A73-1215-11d3-BED1-00C04F682A08","old IVsSaveOptionsDlg")
            ("D5C658C5-59A1-414f-AF5E-E72E83377EAE","new IEnumRunningDocuments")
            ("6b60be84-7b47-11d2-b2c2-00c04fb17608","old IEnumRunningDocuments")
            ("A928AA21-EA77-47ac-8A07-355206C94BDD","new IVsRunningDocumentTable")
            ("625911f2-af99-11d0-8e69-00a0c911005a","old IVsRunningDocumentTable")
            ("CD68D3CF-7124-4d3a-AFED-3E305C2B7D0B","new IVsRunningDocumentTable2")
            ("625911f2-af99-11d0-8e69-00a0c911005a","old IVsRunningDocumentTable2")
            ("BEA6BB4F-A905-49ca-A216-202DF370E07E","new IVsRunningDocTableEvents")
            ("5579c9f0-d09d-11d0-8e75-00a0c911005a","old IVsRunningDocTableEvents")
            ("15C7826F-443C-406d-98F8-55F6260669EC","new IVsRunningDocTableEvents2")
            ("e38a9670-8e0b-11d1-b278-00c04fb17608","old IVsRunningDocTableEvents2")
            ("376ED667-F576-458f-B991-2CFD3EBC7B08","new IVsRunningDocTableEvents3")
            ("e69a388a-fce0-11d2-8a60-00c04f682e21","old IVsRunningDocTableEvents3")
            ("685933F2-C1AD-4540-A15A-D3F977A81AF7","new IVsDocumentLockHnewer")
            ("e2334ed2-43b5-11d3-8a7c-00c04f682e21","old IVsDocumentLockHnewer")
            ("11138F8A-38C0-4436-B5A6-2F5EF2C3E242","new IVsWindowFrame")
            ("7d960b0d-7af8-11d0-8e5e-00a0c911005a","old IVsWindowFrame")
            ("FE46E1DF-E8A8-48d3-932E-B61BC092E681","new IVsWindowFrameNotify")
            ("A31CAE00-0AB0-11d1-B646-00A0C922E851","old IVsWindowFrameNotify")
            ("F4DE74E7-078A-430E-B0E1-8B131BDEF335","new IVsWindowFrameNotify2")
            ("f4de74e7-078a-430e-b0e1-8b131bdef335","old IVsWindowFrameNotify2")
            ("B7EE8DBA-E930-4c5d-984D-B07F8EB60977","new IVsBackForwardNavigation")
            ("349D5D4E-5811-11d3-B741-00C04F9901D1","old IVsBackForwardNavigation")
            ("7E7C4B21-079F-4830-9ED5-E0CB0BF281F1","new IVsWindowView")
            ("C562FF5A-FE57-11d2-B709-00C04F9901D1","old IVsWindowView")
            ("CF7549A9-7A2A-4a6e-ACF4-05452C98CF7E","new IVsToolWindowToolbarHost")
            ("34ECEDC1-06FE-11d1-AED2-549FFB000000","old IVsToolWindowToolbarHost")
            ("4544D333-8D5F-4517-9113-3550D618F2AD","new IVsToolWindowToolbar")
            ("34ECEDC2-06FE-11d1-AED2-549FFB000000","old IVsToolWindowToolbar")
            ("B61FC35B-EEBF-4dec-BFF1-28A2DD43C38F","new IVsUIShell")
            ("7d960b1d-7af8-11d0-8e5e-00a0c911005a","old IVsUIShell")
            ("4E6B6EF9-8E3D-4756-99E9-1192BAAD5496","new IVsUIShell2")
            ("4E6B6EF9-8E3D-4756-99E9-1192BAAD5496","old IVsUIShell2")
            ("686C2AF1-A2E5-4f6c-B660-B63FD4F70C18","new IVsUIShellDocumentWindowMgr")
            ("A33B889B-18C2-4c4e-B561-4D95F0C3DD40","old IVsUIShellDocumentWindowMgr")
            ("7DB81657-7722-4407-B675-9F4A6FEEEA15","new IVsPackageDynamicToolOwner")
            ("914C74A0-8F69-11d1-BC27-0000F87552E7","old IVsPackageDynamicToolOwner")
            ("E36756DE-BB4F-4900-A7F0-E827BDBD2092","new IVsExternalFilesManager")
            ("2FC2CA21-B6AA-11d0-AE1A-00A0C90FFFC3","old IVsExternalFilesManager")
            ("999B1784-A1EE-42eb-B4B6-E928008FFB5D","new IVsExternalFilesManager2")
            ("74C9E366-2BCD-11D2-B2B4-00C04FB17608","old IVsExternalFilesManager2")
            ("F04C8816-7F77-450d-9527-14D0B93DA159","new IVsFileChangeEvents")
            ("b4e98631-f322-11d0-8e89-00a0c911005a","old IVsFileChangeEvents")
            ("250E1E9A-D2FC-410f-99E4-1ABA5B390A9A","new IVsFileChange")
            ("b4e98630-f322-11d0-8e89-00a0c911005a","old IVsFileChange")
            ("9bc72973-194a-4ea8-b4d5-afb0b0d0dcb1","new IVsFileChangeEx")
            ("9bc72973-194a-4ea8-b4d5-afb0b0d0dcb1","old IVsFileChangeEx")
            ("DC7EDE19-3DD1-4e20-A7F1-110883ED996F","new IVsIME")
            ("632f13be-b1cc-11d0-ae4c-00c04fb68006","old IVsIME")
            ("26831FB7-7C55-4ab1-B4AD-E37783F2D4A8","new IVsRelativePathResolver")
            ("702312F2-461F-45b2-8EEA-DA1D566115DF","old IVsRelativePathResolver")
            ("35299EEC-11EE-4518-9F08-401638D1D3BC","new IVsUIShellOpenDocument")
            ("7d960b17-7af8-11d0-8e5e-00a0c911005a","old IVsUIShellOpenDocument")
            ("0F4B629E-8C34-4b5e-A450-F9F8DCFE3009","new IVsMultiViewDocumentView")
            ("46ca0880-0ed7-11d1-8ebd-00a0c90f26ea","old IVsMultiViewDocumentView")
            ("D5D49C61-1C0B-4ea1-9ADB-A79FB1DBC7B5","new IVsPersistDocData")
            ("7d960b1c-7af8-11d0-8e5e-00a0c911005a","old IVsPersistDocData")
            ("9D71890D-090C-4b67-80C3-4CB55C600B60","new IVsPersistDocData2")
            ("1E3A7DC6-800A-11d2-ADD5-00C04F7971C3","old IVsPersistDocData2")
            ("BF955013-A875-439d-A4E7-A3BBDF12AA4F","new IVsDocDataFileChangeControl")
            ("04F29FC4-CE44-11d1-88B1-0000F87579D2","old IVsDocDataFileChangeControl")
            ("18933F6E-0937-4888-8C77-EC8A393B21EA","new IVsPersistHierarchyItem")
            ("a12946d0-bcf3-11d0-8e69-00a0c911005a","old IVsPersistHierarchyItem")
            ("5A494367-DF56-4062-8EDA-54E2C1FF42BC","new IVsPersistHierarchyItem2")
            ("EEFEA81A-8949-4f04-A089-CFBF9BC414C5","old IVsPersistHierarchyItem2")
            ("A2DD88C9-D878-4323-95A3-77DAF258E5D0","new IVsUIHierarchyWindow")
            ("7d960b0e-7af8-11d0-8e5e-00a0c911005a","old IVsUIHierarchyWindow")
            ("B0834D0F-ACFF-4ea5-809B-97CBB5D3D26B","new IVsWindowPane")
            ("7d960b08-7af8-11d0-8e5e-00a0c911005a","old IVsWindowPane")
            ("9673A35F-C03A-438d-BD7E-27D9E28AC184","new IEnumPackages")
            ("7d960b0c-7af8-11d0-8e5e-00a0c911005a","old IEnumPackages")
            ("BEC77711-2DF9-44d7-B478-A453C2E8A134","new IEnumHierarchies")
            ("A2C2BCF9-AC4D-11d0-AF54-00A0C90F9DE6","old IEnumHierarchies")
            ("8C453B03-8907-435b-96D7-573C40948F5C","new IEnumWindowFrames")
            ("46052C70-DCFB-11d0-9404-00A0C90F2734","old IEnumWindowFrames")
            ("FD9DC8E3-2FFC-446D-8C50-99CA4A3D2D1C","new IVsShell")
            ("7d960b09-7af8-11d0-8e5e-00a0c911005a","old IVsShell")
            ("F3519E2D-D5D2-4455-B9F4-5F61F993BD66","new IVsShell2")
            ("F3519E2D-D5D2-4455-B9F4-5F61F993BD66","old IVsShell2")
            ("FC5EF273-DCE3-4DBB-AEE3-F54F91F00286","new IVsBroadcastMessageEvents")
            ("9A726311-D779-11d0-AE21-00A0C90FFFC3","old IVsBroadcastMessageEvents")
            ("7A54FEA1-E786-4A30-8F38-45B3703E50DD","new IVsShellPropertyEvents")
            ("8C506C01-D7A0-11d0-AE21-00A0C90FFFC3","old IVsShellPropertyEvents")
            ("40FB079B-B62C-486F-9823-C9A2EAE8DBFD","new IVsEditorFactory")
            ("7d960b13-7af8-11d0-8e5e-00a0c911005a","old IVsEditorFactory")
            ("78036A8D-A04C-43E4-8BC0-846E63AFA9A2","new IVsRegisterEditors")
            ("7d960b14-7af8-11d0-8e5e-00a0c911005a","old IVsRegisterEditors")
            ("02AC210F-139B-4F8E-9159-501CF2A87D6E","new IVsEditorFactoryNotify")
            ("fa50ef7c-2b83-42c5-ab89-e9395e9731d8","old IVsEditorFactoryNotify")
            ("96973FC6-C2E4-4CB9-8BAF-7F7CD6DBC604","new IVsMultiItemSelect")
            ("7d960b0f-7af8-11d0-8e5e-00a0c911005a","old IVsMultiItemSelect")
            ("687396AE-252E-460F-8F54-EF2C521BB6D8","new IEnumHierarchyItems")
            ("1C97C7F5-8C7B-46a2-A84B-AB12A5833A45","old IEnumHierarchyItems")
            ("30E5C390-C3E6-40AC-BD1D-7015B1B5F541","new IVsEnumHierarchyItemsFactory")
            ("65C8CA4C-0871-48c5-A2E5-FB2F4DC4DB23","old IVsEnumHierarchyItemsFactory")
            ("E68652D0-396C-4937-95A3-F0AE7ACD0E15","new IVsSwatchClient")
            ("4C8F7500-5106-11d3-8821-00C04F7971A5","old IVsSwatchClient")
            ("18291FD1-A1DD-4264-AEAD-6AFD616BF15A","new IVsTrackSelectionEx")
            ("7d960b10-7af8-11d0-8e5e-00a0c911005a","old IVsTrackSelectionEx")
            ("82871589-D680-4D86-B969-9D1102B00F6F","new IVsSelectionEvents")
            ("7d960b11-7af8-11d0-8e5e-00a0c911005a","old IVsSelectionEvents")
            ("55AB9450-F9C7-4305-94E8-BEF12065338D","new IVsMonitorSelection")
            ("7d960b12-7af8-11d0-8e5e-00a0c911005a","old IVsMonitorSelection")
            ("9D21BCC5-2C63-4A61-B055-2F3DF78EB30A","new IVsTaskList")
            ("BC5955D1-AA0D-11d0-A8C5-00A0C921A4D2","old IVsTaskList")
            ("6909C6ED-2AF5-4A35-8EA7-E6095A3ECF9E","new IVsTaskProvider")
            ("BC5955D2-AA0D-11d0-A8C5-00A0C921A4D2","old IVsTaskProvider")
            ("A7E6B1F9-DFF1-4354-870F-196BE871F329","new IVsTaskProvider2")
            ("842BEEF8-B57A-11d2-8B97-00C04F8EC28C","old IVsTaskProvider2")
            ("0F6D7FB4-2649-4E51-BC20-3698F9F51358","new IVsTaskItem")
            ("BC5955D3-AA0D-11d0-A8C5-00A0C921A4D2","old IVsTaskItem")
            ("970A6925-5FFA-4A77-972F-7AB90C0130E5","new IVsTaskItem2")
            ("D30A201A-7837-11d2-8B81-00C04F8EC28C","old IVsTaskItem2")
            ("66638598-522B-4058-9E65-FAF237700E81","new IVsEnumTaskItems")
            ("BC5955D4-AA0D-11d0-A8C5-00A0C921A4D2","old IVsEnumTaskItems")
            ("327C43D7-CCB1-41D7-9A7B-CE87751201F7","new IVsCommentTaskToken")
            ("92ED80E0-144D-11d1-8F8B-00A0C91BBFA2","old IVsCommentTaskToken")
            ("EC47207E-5A2A-45D4-9FA4-F9AB94E380B4","new IVsEnumCommentTaskTokens")
            ("92ED80E1-144D-11d1-8F8B-00A0C91BBFA2","old IVsEnumCommentTaskTokens")
            ("D94C96DA-A6C4-4F52-84F6-52ECF05DEA3A","new IVsCommentTaskInfo")
            ("92ED80E2-144D-11d1-8F8B-00A0C91BBFA2","old IVsCommentTaskInfo")
            ("D529FAD1-4BE0-4BEA-92A3-A58A4B89D056","new IVsTaskListEvents")
            ("92ED80E3-144D-11d1-8F8B-00A0C91BBFA2","old IVsTaskListEvents")
            ("9B878A55-296A-404D-80C4-1468BB7CDC43","new IVsOutputWindowPane")
            ("B7886422-E776-11d0-AE28-00A0C90FFFC3","old IVsOutputWindowPane")
            ("533FAD11-FE7F-41EE-A381-8B67792CD692","new IVsOutputWindow")
            ("B7886421-E776-11d0-AE28-00A0C90FFFC3","old IVsOutputWindow")
            ("C734671A-9BB0-45C5-A08E-B9AB73CF5F47","new IVsAsyncEnum")
            ("d0b027c6-8c1f-11d0-8a34-00a0c91e2acd","old IVsAsyncEnum")
            ("EE559C3D-0189-4F81-B088-C6CC6A394CA1","new IVsAsyncEnumCallback")
            ("d0b027c7-8c1f-11d0-8a34-00a0c91e2acd","old IVsAsyncEnumCallback")
            ("AC7D8BE5-B7F5-400B-B02C-35207672F56B","new IVsHierarchyDropDataSource")
            ("7d960b1b-7af8-11d0-8e5e-00a0c911005a","old IVsHierarchyDropDataSource")
            ("D84D04B8-8E0D-4298-AD9C-27F8C0D5484A","new IVsHierarchyDropDataSource2")
            ("C43E5BC8-14FC-4b6d-9237-1ADD628D4899","old IVsHierarchyDropDataSource2")
            ("5AA5B118-B3D4-40C5-8739-231CE192850C","new IVsHierarchyDropDataTarget")
            ("7d960b1a-7af8-11d0-8e5e-00a0c911005a","old IVsHierarchyDropDataTarget")
            ("09b17094-f50c-40e0-8ab5-57c22a786596","new IVsOpenProjectOrSolutionDlg")
            ("09b17094-f50c-40e0-8ab5-57c22a786596","old IVsOpenProjectOrSolutionDlg")
            ("368FC032-AE91-44a2-BE6B-093A8A9E63CC","new IVsBrowseProjectLocation")
            ("368FC032-AE91-44a2-BE6B-093A8A9E63CC","old IVsBrowseProjectLocation")
            ("42085C99-3F5B-4b61-9737-592479718CEC","new IVsDetermineWizardTrust")
            ("42085C99-3F5B-4b61-9737-592479718CEC","old IVsDetermineWizardTrust")
            ("D6F79714-BFA9-4F00-98CF-E2FA31802694","new lib_VsShell")
            ("455AD7A0-8C58-11d0-A8AB-00A0C921A4D2","old lib_VsShell")
            ("22F1DA29-E27F-46c4-AAFE-F1DF81DCCD3E","new lib_VsShell2")
            ("22F1DA29-E27F-46c4-AAFE-F1DF81DCCD3E","old lib_VsShell2")
            ("F7C88E0E-A5C6-4E32-BD42-AFFCFB94A1D1","new IVsDebuggerEvents")
            ("7d960b15-7af8-11d0-8e5e-00a0c911005a","old IVsDebuggerEvents")
            ("E2E82904-6072-4F8E-A4F5-9AF15A98F444","new IVsDebugLaunch")
            ("A5412570-5FB9-11d1-A811-00A0C9110051","old IVsDebugLaunch")
            ("2E10DD68-AD50-4D3C-94F7-D6C165C7E25D","new IVsDebugger")
            ("7d960b16-7af8-11d0-8e5e-00a0c911005a","old IVsDebugger")
            ("B33300FB-FEFE-4E00-A74A-17A5EED1B1ED","new IVsDebugger2")
            ("B33300FB-FEFE-4E00-A74A-17A5EED1B1ED","old IVsDebugger2")
            ("D53BFAC7-AE4E-4500-AFB0-3925AE60B2BC","new IVsLaunchPad")
            ("EF16A8B0-41CF-11d1-84A4-00A0C9110055","old IVsLaunchPad")
            ("0DBD685A-0A10-4e25-B88E-02E58E60785E","new IVsLaunchPad2")
            ("0DBD685A-0A10-4e25-B88E-02E58E60785E","old IVsLaunchPad2")
            ("A9832932-5F3B-487d-A80D-95115EADDAC3","new IVsLaunchPadOutputParser")
            ("65BC5C20-41D1-11d1-84A4-00A0C9110055","old IVsLaunchPadOutputParser")
            ("A847B389-401A-4438-8A90-CA5BF2451E13","new IVsLaunchPadEvents")
            ("65BC5C20-41D1-11d1-84A4-00A0C9110055","old IVsLaunchPadEvents")
            ("6979C82C-21DB-4E5C-A225-C50A766AA5BA","new IVsLaunchPadFactory")
            ("c21c16a2-1612-4995-b445-f7b1c1657878","old IVsLaunchPadFactory")
            ("374FAF39-7EF3-4877-8667-7E96EC0C1771","new IVsJavaClassLocatorService")
            ("53525550-C745-11d0-A7A6-00A0C9110051","old IVsJavaClassLocatorService")
            ("A001CA6F-F6FF-4C98-873A-845B1C917B96","new IVsTextBufferProvider")
            ("76A3B2C0-C743-11d0-A7A6-00A0C9110051","old IVsTextBufferProvider")
            ("F925DA6B-3F43-4437-9E1E-4D4C1BBDAB3F","new IVsToolboxDataProvider")
            ("E370AEAA-AA14-11d0-8C46-00C04FC2AA89","old IVsToolboxDataProvider")
            ("B5E12E94-6653-4A0D-9C42-5357F2654360","new IVsToolboxUser")
            ("E370AEAB-AA14-11d0-8C46-00C04FC2AA89","old IVsToolboxUser")
            ("5303CCDE-D37A-445B-88A1-A71742F66345","new IEnumToolboxItems")
            ("ADB5A663-C641-11d0-8C54-00C04FC2AA89","old IEnumToolboxItems")
            ("CC81495D-3C2B-4B1E-82CE-965EA5FCA2A0","new IEnumToolboxTabs")
            ("ADB5A664-C641-11d0-8C54-00C04FC2AA89","old IEnumToolboxTabs")
            ("70E643E2-1673-4764-8A39-63CB1AEE0DC9","new IVsToolbox")
            ("E370AEA9-AA14-11d0-8C46-00C04FC2AA89","old IVsToolbox")
            ("08E728DC-9C45-4060-A243-B73443B7CA16","new IVsToolbox2")
            ("0F844C7D-5EF0-11d2-B213-0000F87570EE","old IVsToolbox2")
            ("6A2A1D82-C590-4AB1-8CC2-D95BACBBA9E0","new IVsToolboxClipboardCycler")
            ("E31E5D50-D8A6-11d2-AFBD-00105A9991EF","old IVsToolboxClipboardCycler")
            ("D388BD3B-4D50-4356-B09A-8917E706D196","new IVsStatusbarUser")
            ("DB5CFB59-FC95-11d0-8C7E-00C04FC2AA89","old IVsStatusbarUser")
            ("DC0AF70E-5097-4DD3-9983-5A98C3A19942","new IVsStatusbar")
            ("1F9C665D-F96A-11d0-8C7E-00C04FC2AA89","old IVsStatusbar")
            ("47B1D60A-4EB8-4723-B991-992E6393E392","new IVsDocOutlineProvider")
            ("81CD5C00-FFA1-11d0-B63F-00A0C922E851","old IVsDocOutlineProvider")
            ("1375E029-1FDD-47FF-A22C-6709242133E2","new IVsComponentSelectorProvider")
            ("040F3EE3-55D8-11d3-9ECE-00C04F682A08","old IVsComponentSelectorProvider")
            ("910035B1-D8BE-403A-975E-E4FB68CE40A1","new IVsComponentUser")
            ("0E3C4039-6639-11d3-BFFC-00C04F990235","old IVsComponentUser")
            ("66899421-F497-4503-8C9D-ADAE290F2F27","new IVsComponentSelectorDlg")
            ("2F952EED-564F-11d3-9ECE-00C04F682A08","old IVsComponentSelectorDlg")
            ("A4AAB3EC-A9BB-42E2-8FD4-B01FE406D3F1","new IVsObjectBrowser")
            ("970D9860-EE83-11d0-A778-00A0C91110C3","old IVsObjectBrowser")
            ("1E425321-94CB-448e-8E1E-E1EA2479E5E2","new IVsLiteTreeList")
            ("D1E5F1F2-66F8-4384-BB9E-38DA0DCCE632","old IVsLiteTreeList")
            ("C4158C7D-5052-48D9-8643-7A821BB0F50B","new IVsLiteTree")
            ("CC27B016-3D1E-469e-A0B6-9CFAB0E6DBF6","old IVsLiteTree")
            ("87066898-76AB-45E2-B33C-C5B6B99BB03E","new IVsLiteTreeEvents")
            ("E85449F3-F8BC-11d0-A77A-00A0C91110C3","old IVsLiteTreeEvents")
            ("E86128E4-3B1B-4BE9-BEB6-D30E5BF40850","new IVsLibrary")
            ("1FF9C984-5E75-47cd-B65F-FB63445BFCD7","old IVsLibrary")
            ("DC1B976F-4DC7-4B3D-9EC7-A0DE9D39BC13","new IVsLibraryMgr")
            ("7E547EFB-5DBB-4049-B039-86E416220E30","old IVsLibraryMgr")
            ("44CCEB38-619B-401C-9B48-B9E874FFEE21","new IVsObjectBrowserList")
            ("E85449F6-F8BC-11d0-A77A-00A0C91110C3","old IVsObjectBrowserList")
            ("C48F7AB9-8966-4138-B602-14C5EB8BD857","new IVsObjectList")
            ("07f5fbe1-1abb-11d3-85aa-006097c68e81","old IVsObjectList")
            ("5801DB45-16AA-4F08-BB57-82A070B79512","new IVsObjectListOwner")
            ("0e801c7a-479b-11d3-bdba-00c04f688e50","old IVsObjectListOwner")
            ("01E95D2E-2D20-4662-9DE7-4C1C35524260","new IVsObjectManager")
            ("07f5fbe0-1abb-11d3-85aa-006097c68e81","old IVsObjectManager")
            ("7C4C8065-FB7E-45D8-9B50-940A8FCB5876","new IVsObjectManagerEvents")
            ("07f5fbe2-1abb-11d3-85aa-006097c68e81","old IVsObjectManagerEvents")
            ("EA31732A-0A11-4E80-8DCC-9E6DB395BE59","new IVsObjectBrowserDescription")
            ("E85449F8-F8BC-11d0-A77A-00A0C91110C3","old IVsObjectBrowserDescription")
            ("0587FED2-8072-401F-9090-BCA98C44BBF7","new IVsObjectBrowserDescription2")
            ("7178484A-76B0-11d3-BDC7-00C04F688E50","old IVsObjectBrowserDescription2")
            ("D7ECCE71-9C14-49A9-A93D-A5ED6286AC46","new IVsClassView")
            ("C9C0AE26-AA77-11d2-B3F0-0000F87570EE","old IVsClassView")
            ("0DF98187-FD9A-4669-8A56-727910A4866C","new IVsObjBrowser")
            ("269A02DC-6AF8-11d3-BDC4-00C04F688E50","old IVsObjBrowser")
            ("46B4B7C2-11EB-4753-BE4B-0E0A16E9CE53","new IEnumComReferences")
            ("6114C8A0-0CE9-11d1-8BD9-00A0C90F26F7","old IEnumComReferences")
            ("66A77728-86E1-4D18-88C5-EE0D4FD4BF60","new IVsComReferenceDlgEvents")
            ("6114C8A1-0CE9-11d1-8BD9-00A0C90F26F7","old IVsComReferenceDlgEvents")
            ("CC05EE57-C6C0-4742-A469-0961E50B0049","new IVsComReferenceDlg")
            ("6114C8A2-0CE9-11d1-8BD9-00A0C90F26F7","old IVsComReferenceDlg")
            ("D2C45F92-23B5-408B-B41D-D4365FB7EDA8","new IVsExtensibleObject")
            ("94017641-2BA3-11d1-AE65-00A0C90F26F4","old IVsExtensibleObject")
            ("8C444EF9-5863-4AB1-A1D0-55CC60AC253A","new IVsLanguageInfoPackage")
            ("5E0EEA6C-2EBD-11d1-8CC5-00C04FC2AB22","old IVsLanguageInfoPackage")
            ("34DBAA55-2CA4-44EF-9F92-85435D3E4451","new IVsSwitchToolWindow")
            ("3d4683e0-313b-11d1-a04a-00a0c911e8e9","old IVsSwitchToolWindow")
            ("DC0A8728-F58B-4444-B9F0-32D6868BF399","new IVsMenuItem")
            ("F71AA513-9038-11d0-8C3C-00C04FC2AA89","old IVsMenuItem")
            ("6FBCB271-B391-4F80-B560-45E650DEF0A7","new IVsMenuEditor")
            ("559BAFB1-8396-11d0-B668-00AA00A3EE26","old IVsMenuEditor")
            ("61B34381-6D7C-461D-949A-1AE178CBA00D","new IVsMenuEditorSite")
            ("6A213651-8396-11d0-B668-00AA00A3EE26","old IVsMenuEditorSite")
            ("EAF61568-F99B-4BC2-83C4-1DAD8FFAE9E5","new IVsMenuEditorFactory")
            ("6513023F-94BD-11d0-8C3E-00C04FC2AA89","old IVsMenuEditorFactory")
            ("35A96FFB-7ED0-4D76-93CE-49BE83A9C91E","new IVsIntelliMouseHandler")
            ("B9C589F8-471B-11d1-8862-0000F87579D2","old IVsIntelliMouseHandler")
            ("F4936BE4-7AE0-4C97-9D82-51D219FC5D77","new IVsCodeShareHandler")
            ("16c5b4c1-03b3-11d1-a39a-006097df2373","old IVsCodeShareHandler")
            ("508ED8E9-923D-44ED-8165-5B96DA4E0829","new IVsWindowPaneCommit")
            ("AEC7E124-7662-11d1-9CF5-00C04FB17665","old IVsWindowPaneCommit")
            ("B1E402B6-D8E0-4422-9164-421FEE099F00","new IVsPropertyBrowser")
            ("AA71B5C0-CD90-11d1-B4D6-00A0C911E8B1","old IVsPropertyBrowser")
            ("DF29D855-D0EC-4DA1-BCC3-42FA3A09B1CB","new IVsUIHierWinClipboardHelper")
            ("7EEDD561-FC1E-11d2-BECD-00C04F682A08","old IVsUIHierWinClipboardHelper")
            ("4D25F3C7-3138-4AC6-91AF-D7FF6929DB9F","new IVsUIHierWinClipboardHelperEvents")
            ("7EEDD562-FC1E-11d2-BECD-00C04F682A08","old IVsUIHierWinClipboardHelperEvents")
            ("320E51F6-D238-4BD0-BA89-CCA91DBCF411","new IVsHierarchyDeleteHandler")
            ("8F97C0CD-2B64-11d3-BEDD-00C04F682A08","old IVsHierarchyDeleteHandler")
            ("78FD1CBD-387B-4262-BD7B-65C34F86356E","new IVsHierarchyDeleteHandler2")
            ("78FD1CBD-387B-4262-BD7B-65C34F86356E","old IVsHierarchyDeleteHandler2")
            ("6D10BA00-9465-4F93-8B1D-11E36EE1FF65","new IVsCmdNameMapping")
            ("D3EE8D38-78D7-11d2-8776-00C04F7971A5","old IVsCmdNameMapping")
            ("366704D5-85D0-4F7D-B267-90FA4DD37D5B","new IVsParseCommandLine")
            ("1B04D776-CAB7-11d2-A41B-00C04F72D18A","old IVsParseCommandLine")
            ("2BD8D42F-5BC5-4B7F-AB50-FE9310F2FE53","new IVsTextOut")
            ("2CAA1AB2-0261-11d3-BE8A-0080C747D9A0","old IVsTextOut")
            ("0660CD86-F3AB-4008-930D-BAE8B10FF8CA","new IVsCommandWindow")
            ("94964F2F-FF42-11d2-A434-00C04F72D18A","old IVsCommandWindow")
            ("811DEB01-C1B0-4172-9CA3-504C5095882E","new IVsThreadSafeCommandWindow")
            ("1D009554-87E2-11d3-A45A-00C04F72D18A","old IVsThreadSafeCommandWindow")
            ("575BC578-7562-44E7-986C-5B31398CF121","new IVsTestLog")
            ("9A90C18F-7F31-11d2-9BFC-00C04F9901D1","old IVsTestLog")
            ("CA3E5036-9567-407C-B464-5ECA98B533A0","new IVsTshell")
            ("F6A0FA50-B78B-11d0-A79E-00A0C9110051","old IVsTshell")
            ("BC039978-213E-4CA0-81C4-10EDF2AF2D66","new IVsPropertyPageFrame")
            ("41218D4C-AC2A-11d2-8B91-00C04F8EC28C","old IVsPropertyPageFrame")
            ("5FA2AC9A-3BEF-423A-8B5E-6645811BFB6B","new IVsFontAndColorDefaults")
            ("0514444A-F8DB-11d2-AE7D-00C04F7971C3","old IVsFontAndColorDefaults")
            ("1D42A4C9-57DE-4D3E-8010-485ADFC1E95B","new IVsFontAndColorGroup")
            ("A76B7F30-50CF-11d3-8E5C-00104BC90F0C","old IVsFontAndColorGroup")
            ("F73E1D1E-3D1B-44F0-B736-D59F960B7F9E","new IVsFontAndColorEvents")
            ("12F8E1DA-1EB3-11d3-AE9B-00C04F7971C3","old IVsFontAndColorEvents")
            ("3448FF72-B072-435E-9059-29D89C0A3CD0","new IVsFontAndColorDefaultsProvider")
            ("9B7C3392-145C-11d3-AE91-00C04F7971C3","old IVsFontAndColorDefaultsProvider")
            ("40BC7B1A-E625-4DA1-86B4-7660F3CCBB16","new IVsFontAndColorStorage")
            ("1369CBD4-0FBD-11d3-AE8E-00C04F7971C3","old IVsFontAndColorStorage")
            ("82780F79-A3ED-4B7F-90C0-5FEE14CBB53E","new IVsProjectStartupServices")
            ("30B4F22A-0EE3-11d3-9B52-00C04F68380C","old IVsProjectStartupServices")
            ("9E6F916A-3E8B-4741-8AFB-5187F82B699B","new IEnumProjectStartupServices")
            ("746797AC-0EE3-11d3-9B52-00C04F68380C","old IEnumProjectStartupServices")
            ("237ABD5F-9537-4AEE-A893-72AB9A0EA8E8","new IVsPropertyPage")
            ("6DD48D3C-1BD1-11d3-8BBD-00C04F8EC28C","old IVsPropertyPage")
            ("0FF510A3-5FA5-49F1-8CCC-190D71083F3E","new IVsPerPropertyBrowsing")
            ("0A55B998-D98E-11D2-91DF-00A0CC394083","old IVsPerPropertyBrowsing")
            ("ED77D5EC-B0DE-4721-BDC6-38DCBE589B4C","new IVsRegisterPriorityCommandTarget")
            ("C96FC5D4-DE9F-43bf-B197-03897D829800","old IVsRegisterPriorityCommandTarget")
            ("44A39218-81BD-4669-9DE0-F282A8BAEE34","new IVsObjectSearch")
            ("1AB09D1C-6A1F-410f-856D-7C35D386A068","old IVsObjectSearch")
            ("BED89B98-6EC9-43CB-B0A8-41D6E2D6669D","new IVsGeneratorProgress")
            ("bdb56d23-712a-43f0-a2d0-5cf06e71003d","old IVsGeneratorProgress")
            ("3634494C-492F-4F91-8009-4541234E4E99","new IVsSingleFileGenerator")
            ("edea12ea-3621-4a7e-ac32-8940f17453b7","old IVsSingleFileGenerator")
            ("B8F932A5-5037-48C9-AB3A-A4ABBA79358B","new IVsCfg")
            ("d0b027b1-8c1f-11d0-8a34-00a0c91e2acd","old IVsCfg")
            ("2BC88742-618D-46B2-B65D-67AC990E3215","new IVsDebuggableProjectCfg")
            ("d0b027dc-8c1f-11d0-8a34-00a0c91e2acd","old IVsDebuggableProjectCfg")
            ("A17326AD-C97B-4278-86E2-72163C4C6A8C","new IVsBuildStatusCallback")
            ("d0b027c3-8c1f-11d0-8a34-00a0c91e2acd","old IVsBuildStatusCallback")
            ("8588E475-BB33-4763-B4BA-0322F839AA3C","new IVsBuildableProjectCfg")
            ("d0b027c0-8c1f-11d0-8a34-00a0c91e2acd","old IVsBuildableProjectCfg")
            ("09857e8e-74cc-43a7-993d-3ec774dca298","new IVsBuildableProjectCfg2")
            ("09857e8e-74cc-43a7-993d-3ec774dca298","old IVsBuildableProjectCfg2")
            ("E9964F8D-5600-4623-B611-FF4007B22419","new IVsDeployStatusCallback")
            ("942DCAB5-BA5D-11d0-AB23-00A0C90F2713","old IVsDeployStatusCallback")
            ("358F6C9F-CD65-446A-B79A-30CEE094FDC1","new IVsDeployableProjectCfg")
            ("2bc4e9c7-66b3-11d1-b194-00a0c91e2acd","old IVsDeployableProjectCfg")
            ("A981529F-4D0D-46ee-A758-AC26E50E099D","new IVsDeployableProjectCfg2")
            ("a981529f-4d0d-46ee-a758-ac26e50e099d","old IVsDeployableProjectCfg2")
            ("2DBDF061-439B-4822-9727-CA3ED918B658","new IVsProjectCfg")
            ("d0b027b2-8c1f-11d0-8a34-00a0c91e2acd","old IVsProjectCfg")
            ("A7ADE7A0-F286-4C03-8137-D6D0EF3D6848","new IVsProjectCfg2")
            ("521F66DD-F1C1-11d2-B0AD-00A0C9CFCEE6","old IVsProjectCfg2")
            ("EEABD2BE-4F4F-4CCB-86AD-9F469C5C9686","new IVsCfgProvider")
            ("d0b027e0-8c1f-11d0-8a34-00a0c91e2acd","old IVsCfgProvider")
            ("E6D78900-BB40-4039-9C54-593A242B65DA","new IVsCfgProviderEvents")
            ("2bc4e9f0-66b3-11d1-b194-00a0c91e2acd","old IVsCfgProviderEvents")
            ("0D6D480C-894F-48E4-98D2-E0A7127750E4","new IVsCfgProviderEventsHelper")
            ("99913f1e-1ee3-11d3-8a6e-00c04f682e21","old IVsCfgProviderEventsHelper")
            ("623E34D5-82C1-42ED-A82C-6CA0478FFDDA","new IVsCfgProvider2")
            ("2bc4e9f1-66b3-11d1-b194-00a0c91e2acd","old IVsCfgProvider2")
            ("803E46E2-6A0D-4D5D-9F84-6CE1248B068D","new IVsProjectCfgProvider")
            ("e0b027b0-8c1f-11d0-8a34-00a0c91e2acd","old IVsProjectCfgProvider")
            ("509D0E4F-A770-44C3-9185-D4F1E4813AD6","new IVsGetCfgProvider")
            ("BFDCD88A-30CA-11d3-9B5F-00C04F68380C","old IVsGetCfgProvider")
            ("0A8AC2FB-87BC-4795-8C8B-47E877F48FE8","new IVsEnumOutputs")
            ("d0b027b3-8c1f-11d0-8a34-00a0c91e2acd","old IVsEnumOutputs")
            ("0238DCC5-62D6-4DAC-A977-2C6A36C502F4","new IVsOutput")
            ("d0b027b4-8c1f-11d0-8a34-00a0c91e2acd","old IVsOutput")
            ("2D39742A-C729-44C3-AC5B-85785D4C1C22","new IVsHierarchicalOutput")
            ("d0b027c8-8c1f-11d0-8a34-00a0c91e2acd","old IVsHierarchicalOutput")
            ("FCC03D95-7C2E-4398-AAAE-0F4B56104FC8","new IVsOutputGroup")
            ("521F66DE-F1C1-11d2-B0AD-00A0C9CFCEE6","old IVsOutputGroup")
            ("653BB330-1205-4CF8-8F88-723D6E199A01","new IVsOutput2")
            ("521F66DF-F1C1-11d2-B0AD-00A0C9CFCEE6","old IVsOutput2")
            ("A086E870-AA0B-4EF9-8CF3-4A38267B9C7D","new IVsDeployDependency")
            ("521F66E0-F1C1-11d2-B0AD-00A0C9CFCEE6","old IVsDeployDependency")
            ("B4D28A5B-063D-4622-B0C7-C3DDEBFCDCCF","new IVsProjectDeployDependency")
            ("06e2018b-568f-44e9-8af7-5d501cae6eb7","old IVsProjectDeployDependency")
            ("9DB6689F-3C5F-43ED-B0D5-54851A980B93","new IVsDependency")
            ("d0b027b6-8c1f-11d0-8a34-00a0c91e2acd","old IVsDependency")
            ("28D58EEE-EFFC-4B4D-834C-3A746FEAC7AE","new IVsBuildDependency")
            ("d0b027d6-8c1f-11d0-8a34-00a0c91e2acd","old IVsBuildDependency")
            ("0ED850AF-C30A-42BA-AA20-3436ADF24937","new IVsEnumDependencies")
            ("d0b027b5-8c1f-11d0-8a34-00a0c91e2acd","old IVsEnumDependencies")
            ("819CC554-C7BF-4965-A4D4-937B2B6CD2E1","new IVsDependencyProvider")
            ("d0b027c9-8c1f-11d0-8a34-00a0c91e2acd","old IVsDependencyProvider")
            ("EC9ABAFB-E744-44B5-8771-0B875EE6FC5C","new IVsPropertyStreamIn")
            ("d0b027cb-8c1f-11d0-8a34-00a0c91e2acd","old IVsPropertyStreamIn")
            ("805B0E0A-7122-4855-962F-887E46D2F112","new IVsPropertyFileIn")
            ("2bc4e9c0-66b3-11d1-b194-00a0c91e2acd","old IVsPropertyFileIn")
            ("BF283741-E0AD-49C0-BEA4-1E267E52208F","new IVsPropertyStreamOut")
            ("d0b027cc-8c1f-11d0-8a34-00a0c91e2acd","old IVsPropertyStreamOut")
            ("3018E511-6282-41FC-8E1F-77AB1BDDE523","new IVsPropertyFileOut")
            ("2bc4e9c1-66b3-11d1-b194-00a0c91e2acd","old IVsPropertyFileOut")
            ("0612FCA3-B60E-410B-BCCE-43953FF0763C","new IVsStructuredFileIOHelper")
            ("2bc4e9c2-66b3-11d1-b194-00a0c91e2acd","old IVsStructuredFileIOHelper")
            ("12B43F9F-8550-4FFA-850F-FE9D4D396C20","new IVsStructuredFileIO")
            ("d0b027e1-8c1f-11d0-8a34-00a0c91e2acd","old IVsStructuredFileIO")
            ("218D0424-9C53-4EA1-A679-A0AED59B0E4F","new IVsHTMLConverter")
            ("CB89733A-B2E0-11d1-981B-0000F8058E9D","old IVsHTMLConverter")
            ("05A323E9-5069-474E-9BCC-14F87302B213","new IVsSolutionSecurityOptions")
            ("96313150-6AA6-11d1-A202-0000F8026F55","old IVsSolutionSecurityOptions")
            ("A9F86308-5EA7-485D-BAB8-E8989C3CFBDC","new IVsUpdateSolutionEvents")
            ("d0b027da-8c1f-11d0-8a34-00a0c91e2acd","old IVsUpdateSolutionEvents")
            ("F59DBC1A-91C3-45C9-9796-1CAB558502DD","new IVsUpdateSolutionEvents2")
            ("868163bb-1da7-41df-87b8-ce64439a4093","old IVsUpdateSolutionEvents2")
            ("40025C28-3303-42CA-BED8-0F3BD856BD6D","new IVsUpdateSolutionEvents3")
            ("40025C28-3303-42CA-BED8-0F3BD856BD6D","old IVsUpdateSolutionEvents3")
            ("93E969D6-1AA0-455F-B208-6ED3C82B5C58","new IVsSolutionBuildManager")
            ("d0b027db-8c1f-11d0-8a34-00a0c91e2acd","old IVsSolutionBuildManager")
            ("80353F58-F2A3-47B8-B2DF-0475E07BB1C6","new IVsSolutionBuildManager2")
            ("e823a2b0-88ed-44c8-98de-73fd5d54b908","old IVsSolutionBuildManager2")
            ("B6EA87ED-C498-4484-81AC-0BED187E28E6","new IVsSolutionBuildManager3")
            ("B6EA87ED-C498-4484-81AC-0BED187E28E6","old IVsSolutionBuildManager3")
            ("910ACC85-ECD4-4CF8-85E0-EB105ABE8008","new IVsLibraryReferenceManager")
            ("699D5E17-9B22-466b-ACFA-2E12CD64E249","old IVsLibraryReferenceManager")
            ("0E7798AD-4000-48DF-AA1D-851425D45825","new IVsLangSpecificSyntax")
            ("9E9AB119-D2A1-4381-9020-6B771DC46AE9","old IVsLangSpecificSyntax")
            ("ad98f020-bad0-0000-0000-abc037459871","IProvideProjectSite")
            ]
            
        let iunk = Marshal.GetIUnknownForObject(o)
        try
            knownInterfaces 
            |> Seq.filter(fun (iid,_)->CanQueryInterface iunk iid) 
            |> Seq.map(snd)
        finally 
            let _ = Marshal.Release(iunk)
            ()

    let ReportKnownInterfaces o = 
        (DiscoverKnownInterfaces o) |> Seq.iter (fun s-> Trace.PrintLine("ReportKnownInterfaces", fun _ -> sprintf "  matched %s" s))
            
#endif            


