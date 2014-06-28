// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

module internal GetVerifiedPublisherInfo =
/// Returns either the name of the verified publisher (e.g. Some("Microsoft") or Some("Contoso")), or None if there is no 
/// verified publisher info (or unverifiable) available.

    // can test e.g. with Office EXEs

    open System.Runtime.InteropServices
    open System

    // These aliases make transcribing types more straightforward
    type DWORD = int32
    type WCHAR = int16
    type PTR = System.IntPtr
    type HANDLE = System.IntPtr
    type BOOL = uint32

    ////////////////////////////////////////////////////////////////
    // Native Win32 API type transcriptions
    ////////////////////////////////////////////////////////////////

    // A hand-marshalled Win32 GUID struct
    type Win32Guid =
        struct
            val D1  : uint32
            val D2  : uint16
            val D3  : uint16
            val D4  : byte 
            val D5  : byte
            val D6  : byte
            val D7  : byte
            val D8  : byte
            val D9  : byte
            val D10 : byte
            val D11 : byte

            new(data1 : uint32, 
                data2 : uint16, 
                data3 : uint16, 
                data4 : byte, 
                data5 : byte, 
                data6 : byte, 
                data7 : byte, 
                data8 : byte, 
                data9 : byte, 
                data10 : byte, 
                data11 : byte) = 
                {   D1 = data1;
                    D2 = data2;
                    D3 = data3;
                    D4 = data4;
                    D5 = data5; 
                    D6 = data6; 
                    D7 = data7; 
                    D8 = data8; 
                    D9 = data9; 
                    D10 = data10; 
                    D11 = data11}
        end

    // from WinDef.h
    type FILETIME =
        struct
            val mutable dwLowDateTime : DWORD
            val mutable dwHighDateTime : DWORD
        end

    // from WinTrust.h
    type WINTRUST_DATA =
        struct
            val mutable cbStruct : DWORD
            val mutable pPolicyCallbackData : PTR
            val mutable pSIPClientData : PTR
            val mutable dwUIChoice : DWORD
            val mutable fdwRevocationChecks : DWORD
            val mutable dwUnionChoice : DWORD
            val mutable pFile : PTR // actually a union of choices
            val mutable dwStateAction : DWORD
            val mutable hWVTStateData : HANDLE
            val mutable pwszURLReference : PTR
            val mutable dwProvFlags : DWORD
            val mutable dwUIContext : DWORD
        end

    // from WinTrust.h
    type WINTRUST_FILE_INFO =
        struct
            val mutable cbStruct : DWORD
            val mutable pcwszFilePath : PTR
            val mutable hFile : HANDLE
            val mutable pgKnownSubject : PTR
        end

    // from WinTrust.h
    type CRYPT_PROVIDER_DATA =
        struct
            val mutable cbStruct : DWORD
            val mutable pWintrustData : PTR
            val mutable fOpenedFile : BOOL
            val mutable hWndParent : HANDLE
            val mutable pgActionID : PTR
            val mutable hProv : HANDLE
            val mutable dwError : DWORD
            val mutable dwRegSecuritySettings : DWORD
            val mutable dwRegPolicySettings : DWORD
            val mutable psPfns : PTR
            val mutable cdwTrustStepErrors : DWORD
            val mutable padwTrustStepErrors : PTR
            val mutable chStores : DWORD
            val mutable pahStores : PTR
            val mutable dwEncoding : PTR
            val mutable hMsg : HANDLE
            val mutable csSigners : DWORD
            val mutable pasSigners : PTR
            val mutable csProvPrivData : DWORD
            val mutable pasProvPrivDAta : PTR
            val mutable dwSubjectChoice : DWORD
            val mutable pPDSip : PTR
            val mutable pszUsageOID : PTR
            val mutable fRecallWithState : BOOL
            val mutable sftSystemTime : FILETIME
            val mutable pszCTLSignerUsageOID : PTR
            val mutable dwProvFlags : DWORD
            val mutable dwFinalError : DWORD
            val mutable pRequestUsage : PTR
            val mutable dwTrustPubSettings : DWORD
            val mutable dwUIStateFlags : DWORD
        end

    // from WinTrust.h
    type CRYPT_PROVIDER_SGNR =
        struct
            val mutable cbStruct : DWORD
            val mutable sftVerifyAsOf : FILETIME
            val mutable csCertChain : DWORD
            val mutable pasCertChain : PTR
            val mutable dwSignerType : DWORD
            val mutable psSigner : PTR
            val mutable dwError : DWORD
            val mutable csCounterSigners : DWORD
            val mutable pasCounterSigners : PTR
            val mutable pChainContext : PTR
        end

    // from WinCrypt.h
    type CERT_TRUST_STATUS =
        struct
            val mutable dwErrorStatus : DWORD
            val mutable dwInfoStatus : DWORD
        end

    // from WinCrypt.h
    type CERT_CHAIN_CONTEXT =
        struct
            val mutable cbSize : DWORD
            val mutable TrustStatus : CERT_TRUST_STATUS
            val mutable cChain : DWORD
            val mutable rgpChain : PTR // PCERT_SIMPLE_CHAIN*
        end

    // from WinCrypt.h
    type CERT_SIMPLE_CHAIN =
        struct
            val mutable cbSize : DWORD
            val mutable TrustStatus: CERT_TRUST_STATUS
            val mutable cElement : DWORD
            val mutable rgpElement : PTR // PCERT_CHAIN_ELEMENT*
            val mutable pTrustListInfo : PTR
            val mutable fHasRevocationFreshnessTime : BOOL
            val mutable dwRevocationFreshnessTime : DWORD
        end

    // from WinCrypt.h
    type CERT_CHAIN_ELEMENT =
        struct
            val mutable cbSize : DWORD
            val mutable pCertContext : PTR
            val mutable TrustStatus : CERT_TRUST_STATUS
            val mutable pRevocationInfo : PTR
            val mutable pIssuanceUsage : PTR
            val mutable pApplicationUsage : PTR
            val mutable pwszExtendedErrorInfo : PTR
        end

    ////////////////////////////////////////////////////////////////
    // Flags and constants
    ////////////////////////////////////////////////////////////////
    // from WinTrust.h
    let WTD_UI_NONE = 2
    let WTD_REVOKE_WHOLECHAIN = 1
    let WTD_CHOICE_FILE = 1
    let WTD_STATEACTION_VERIFY = 1
    let WTD_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT = 0x80
    let WTD_STATEACTION_CLOSE = 2
    let CERT_NAME_ATTR_TYPE = 3

    let INVALID_HANDLE_VALUE = System.IntPtr(-1)
    let nullPTR = System.IntPtr.Zero


    // GUID from SoftPub.h for pulling the correct authenticode certificate
    let WINTRUST_ACTION_GENERIC_VERIFY_V2 = Win32Guid(  uint32(0xaac56b),
                                                        uint16(0xcd44),
                                                        uint16(0x11d0),
                                                        byte(0x8c),
                                                        byte(0xc2),
                                                        byte(0x0),
                                                        byte(0xc0),
                                                        byte(0x4f),
                                                        byte(0xc2),
                                                        byte(0x95),
                                                        byte(0xee))


    ////////////////////////////////////////////////////////////////
    // P/Invokes
    ////////////////////////////////////////////////////////////////
    // see wintrust.h for details
    [<DllImport("WinTrust.dll", CallingConvention = CallingConvention.Winapi)>]
    extern int WinVerifyTrust (HANDLE _hwnd, PTR _pProvGuid, PTR _pwtd)

    // returns an unmarshalled pointer to a CRYPT_PROVIDER_DATA struct
    [<DllImport("WinTrust.dll", CallingConvention = CallingConvention.Winapi)>]
    extern PTR WTHelperProvDataFromStateData(HANDLE _hStateData)

    // returns an unmarshalled pointer to a CRYPT_PROVIDER_SGNR struct
    [<DllImport("WinTrust.dll", CallingConvention = CallingConvention.Winapi)>]
    extern PTR WTHelperGetProvSignerFromChain(PTR _pProvData, int32 _idxSigner, int32 _fCounterSigner, int32 _idxCounterSigner)

    [<DllImport("Crypt32.dll", CallingConvention = CallingConvention.Winapi)>]
    extern DWORD CertGetNameStringW(PTR _pCertContext, DWORD _dwType, DWORD _dwFlags, PTR _pvTypePara, PTR _pszNameString, DWORD _cchNameString)

    ////////////////////////////////////////////////////////////////
    // GetVerifiedPublisherInfo
    //  takes a path to a possibly signed binary, returns 
    //  Some(publisher_name) for verified binaries, None for 
    //  unverifed ones
    ////////////////////////////////////////////////////////////////
    let GetVerifiedPublisherInfo(path : string) =
        let mutable wtd = WINTRUST_DATA()
        let mutable wtfi = WINTRUST_FILE_INFO()

        // track GCHandle object for 
        let gcHandles = ref List.empty<GCHandle>

        let pin o =
            let handle = GCHandle.Alloc(o, GCHandleType.Pinned)
            let ptr = handle.AddrOfPinnedObject()
            gcHandles := handle :: !gcHandles
            handle,ptr

        let freeHandles() = for handle in !gcHandles do handle.Free()

        let _pathHandle, ppath = pin path 

        wtfi.cbStruct <- Marshal.SizeOf(wtfi)
        wtfi.pcwszFilePath <- ppath
        wtfi.hFile <- nullPTR
        wtfi.pgKnownSubject <- nullPTR

        let _pwtfiHandle, pwtfi = pin wtfi

        wtd.cbStruct <- Marshal.SizeOf(wtd)
        wtd.pPolicyCallbackData <- nullPTR
        wtd.pSIPClientData <- nullPTR
        wtd.dwUIChoice <- WTD_UI_NONE
        wtd.fdwRevocationChecks <- WTD_REVOKE_WHOLECHAIN
        wtd.dwUnionChoice <- WTD_CHOICE_FILE
        wtd.pFile <- pwtfi
        wtd.dwStateAction <- WTD_STATEACTION_VERIFY
        wtd.hWVTStateData <- nullPTR
        wtd.pwszURLReference <- nullPTR
        wtd.dwProvFlags <- WTD_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT

        let _guidHandle,pGuid = pin WINTRUST_ACTION_GENERIC_VERIFY_V2
        let _wtdHandle,pwtd = pin wtd

        let isAuthenticated = WinVerifyTrust(INVALID_HANDLE_VALUE,pGuid,pwtd) = 0  

        let publisherName =
            if isAuthenticated then
                // we need to remarshal because the call to WinVerifyTrust has changed the object's state
                wtd <- Marshal.PtrToStructure(pwtd,typeof<WINTRUST_DATA>) :?> WINTRUST_DATA

                let pUnmarshalledProvData = WTHelperProvDataFromStateData(wtd.hWVTStateData)
                let pUnmarshalledSGNRData = WTHelperGetProvSignerFromChain(pUnmarshalledProvData, 0, 0, 0)

                let pProvSigner = Marshal.PtrToStructure(pUnmarshalledSGNRData, typeof<CRYPT_PROVIDER_SGNR>) :?> CRYPT_PROVIDER_SGNR
                let pChainContext = Marshal.PtrToStructure(pProvSigner.pChainContext, typeof<CERT_CHAIN_CONTEXT>) :?> CERT_CHAIN_CONTEXT

                // WARNING: technically pChainContext->rgpChain[0]
                let ppChain = Marshal.PtrToStructure(pChainContext.rgpChain, typeof<HANDLE>) :?> HANDLE
                let pChain = Marshal.PtrToStructure(ppChain, typeof<CERT_SIMPLE_CHAIN>) :?> CERT_SIMPLE_CHAIN

                // WARNING: technically pChain->rgpElement[0]
                let ppElement = Marshal.PtrToStructure(pChain.rgpElement, typeof<HANDLE>) :?> HANDLE
                let pElement = Marshal.PtrToStructure(ppElement, typeof<CERT_CHAIN_ELEMENT>) :?> CERT_CHAIN_ELEMENT

                let pCertContext = pElement.pCertContext

                // get the length of the name string
                let attrStringLength = CertGetNameStringW(pCertContext,CERT_NAME_ATTR_TYPE,0,nullPTR,nullPTR,0)

                // allocate buffer for unicode publisher string
                let pNameBuffer = Marshal.AllocHGlobal(attrStringLength * 2)


                let _ = CertGetNameStringW(pCertContext,CERT_NAME_ATTR_TYPE,0,nullPTR,pNameBuffer,attrStringLength)
                let publisherNameString = Marshal.PtrToStringAuto(pNameBuffer)

                // free the unmanaged buffer
                Marshal.FreeHGlobal(pNameBuffer)

                Some(publisherNameString)
            else
                None

        // Clean up verification state
        wtd.dwStateAction <- WTD_STATEACTION_CLOSE
        WinVerifyTrust(INVALID_HANDLE_VALUE,pGuid,pwtd) |> ignore

        // free pinned data
        freeHandles()
    
        publisherName

    (* sample code
    let wordPath = @"c:\Program Files (x86)\Microsoft Office\Office14\WinWord.exe"
    let fooPath = @"c:\lab\fs\sltest.exe" // some random exe

    printfn "Is it authenticated?"
    printfn "%s: %A" wordPath (match GetVerifiedPublisherInfo wordPath with | Some(n) -> n | _ -> "Not authenticated")
    printfn "%s: %A" fooPath (match GetVerifiedPublisherInfo fooPath with | Some(n) -> n | _ -> "Not authenticated")
    *)