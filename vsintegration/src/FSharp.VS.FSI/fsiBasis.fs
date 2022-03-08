// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Text.RegularExpressions
open System.Runtime.InteropServices
open Microsoft.Win32
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.OLE.Interop

module internal AssemblyAttributes = 
    [<assembly:ComVisible(true)>]
    do()

module internal Guids =
    
    // FSI Session command set
    let guidInteractiveCommands         = Microsoft.VisualStudio.VSConstants.VsStd11 
    let cmdIDSessionInterrupt           = int Microsoft.VisualStudio.VSConstants.VSStd11CmdID.InteractiveSessionInterrupt
    let cmdIDSessionRestart             = int Microsoft.VisualStudio.VSConstants.VSStd11CmdID.InteractiveSessionRestart

    let guidFsiConsoleCmdSet            = Guid("0E455B35-F2EB-431b-A0BE-B268D8A7D17F")
    let cmdIDAttachDebugger             = 0x104
    let cmdIDDetachDebugger             = 0x105
    let cmdIDFsiConsoleContextMenu      = 0x2100 
   
    // Command set for SendToInteractive
    // some commands moved to VS Shell
    let guidInteractiveShell            = Microsoft.VisualStudio.VSConstants.VsStd11 
    let cmdIDSendSelection              = int Microsoft.VisualStudio.VSConstants.VSStd11CmdID.ExecuteSelectionInInteractive
    let cmdIDSendLine                   = int Microsoft.VisualStudio.VSConstants.VSStd11CmdID.ExecuteLineInInteractive

    // some commands not in VS Shell
    let guidInteractive                 = Guid("8B9BF77B-AF94-4588-8847-2EB2BFFD29EB")
    let cmdIDDebugSelection             = 0x01

    let guidFsiPackage                  = "eeeeeeee-9342-42f1-8ea9-42f0e8a6be55" // FSI-LINKAGE-POINT: when packaged here
    let guidFSharpProjectPkgString      = "91A04A73-4F2C-4E7C-AD38-C1A68E7DA05C" // FSI-LINKAGE-POINT: when packaged in project system

    [<Literal>]
    /// "35A5E6B8-4012-41fc-A652-2CDC56D74E9F"
    let guidFsiLanguageService          = "35A5E6B8-4012-41fc-A652-2CDC56D74E9F"        // The FSI lang service

    [<Literal>]
    /// "dee22b65-9761-4a26-8fb2-759b971d6dfc"
    let guidFsiSessionToolWindow        = "dee22b65-9761-4a26-8fb2-759b971d6dfc"

    // FSI Package command set
    let guidFsiPackageCmdSet            = Guid("0be3b0d7-4fc2-45bf-a168-957e8a8834d0")
    let cmdIDLaunchFsiToolWindow        = 0x101
    
    let nameFsiLanguageService          = "FSharpInteractive"                           // see Package registration attribute
    
    let guidFsharpLanguageService       = Guid("BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B")  // The F# source file lang service

    [<Literal>]
    let fsiContentTypeName              = "FSharpInteractive"

module internal Util =

#if NO_CHECKNULLS
    type MaybeNull<'T when 'T : null> = 'T

    // Shim to match nullness checking library support in preview
    let inline (|Null|NonNull|) (x: 'T) : Choice<unit,'T> = match x with null -> Null | v -> NonNull v
#else
    type MaybeNull<'T when 'T : not null> = 'T?
#endif

    /// Utility function to create an instance of a class from the local registry. [From Iron Python].
    let CreateObject (globalProvider:System.IServiceProvider) (classType:Type,interfaceType:Type) =
        // Follows IronPython sample. See ConsoleWindow.CreateObject
        let localRegistry = globalProvider.GetService(typeof<SLocalRegistry>) :?> ILocalRegistry   
        let mutable interfaceGuid = interfaceType.GUID    
        let res,interfacePointer = localRegistry.CreateInstance(classType.GUID, null, &interfaceGuid,uint32 CLSCTX.CLSCTX_INPROC_SERVER)
        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(res) |> ignore
        if interfacePointer = IntPtr.Zero then
            raise (new COMException("CanNotCreateObject"))
        else
            // Get a CLR object from the COM pointer
            let mutable obj = null
            try
                obj <- Marshal.GetObjectForIUnknown(interfacePointer)
            finally
                Marshal.Release(interfacePointer) |> ignore
            obj

    // CreateObject, using known type information.
    let CreateObjectT<'classT,'interfaceT> (provider:System.IServiceProvider) =
        let classType     = typeof<'classT>
        let interfaceType = typeof<'interfaceT>
        CreateObject provider (classType,interfaceType) :?> 'interfaceT

    let throwOnFailure0 (res)         = Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure res |> ignore; ()
    let throwOnFailure1 (res,a)       = Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure res |> ignore; a
    let throwOnFailure2 (res,a,b)     = Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure res |> ignore; (a,b)
    let throwOnFailure3 (res,a,b,c)   = Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure res |> ignore; (a,b,c)
    let throwOnFailure4 (res,a,b,c,d) = Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure res |> ignore; (a,b,c,d)

    module RegistryHelpers =
        /// returns Some(value) if key/value exists, None otherwise
        let tryReadHKCU<'t> subkey valueName =
            use key = Registry.CurrentUser.OpenSubKey(subkey)
            if isNull key then None else
            match key.GetValue(valueName) with
            | :? 't as valTyped -> Some(valTyped)
            | _ -> None

        /// write value to HKCU subkey, uses default .NET/registry type conversions
        let writeHKCU subkey valueName value =
            use key = Registry.CurrentUser.OpenSubKey(subkey, true)
            key.SetValue(valueName, value)

    module ArgParsing =
        let private lastIndexOfPattern s patt =
            let patt' = sprintf @"(\s|^)%s(\s|$)" patt
            match Regex.Matches(s, patt') |> Seq.cast<Match> |> Seq.tryLast with
            | None -> -1
            | Some(m) -> m.Index

        /// checks if combined arg string results in debug info on/off
        let debugInfoEnabled (args : string) =
            // FSI default is --debug:pdbonly, so disabling must be explicit
            match lastIndexOfPattern args @"(--|/)debug-" with
            | -1 -> true 
            | idxDisabled ->
                // check if it's enabled by later args
                let afterDisabled = args.Substring(idxDisabled + 5)
                let idxEnabled =
                    [lastIndexOfPattern afterDisabled @"(--|/)debug(\+|:full|:pdbonly)?"
                     lastIndexOfPattern afterDisabled @"(--|/)g"] |> List.max
                idxEnabled > idxDisabled

        /// checks if combined arg string results in optimizations on/off
        let optimizationsEnabled (args : string) =
            // FSI default is --optimize+, so disabling must be explicit
            match lastIndexOfPattern args @"(--|/)optimize-" with
            | -1 -> true 
            | idxDisabled ->
                // check if it's enabled by later args
                let afterDisabled = args.Substring(idxDisabled + 5)
                let idxEnabled = lastIndexOfPattern afterDisabled @"(--|/)optimize\+?"
                idxEnabled > idxDisabled

// History buffer.
// For now, follows the cmd.exe model.
type internal HistoryBuffer() =
    let lines   = new System.Collections.Generic.List<string>()    
    let mutable pointer = 0
    member this.Add(text:string)        = lines.Add(text); pointer <- lines.Count
    member this.CycleUp(text:string)    = if pointer-1 >= 0 then
                                            pointer <- pointer - 1
                                            Some lines.[pointer]
                                          else
                                            None
    member this.CycleDown(text:string)  = if pointer+1 < lines.Count then
                                            pointer <- pointer + 1
                                            Some lines.[pointer]
                                          else
                                            None



