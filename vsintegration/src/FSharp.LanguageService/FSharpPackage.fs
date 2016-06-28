// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Configuration
open System.ComponentModel.Design
open System.Runtime.InteropServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.OLE.Interop


// Workaround to access non-public settings persistence type.
// GetService( ) with this will work as long as the GUID matches the real type.
[<Guid("9B164E40-C3A2-4363-9BC5-EB4039DEF653")>]
type internal SVsSettingsPersistenceManager = class end

[<Guid(FSharpConstants.packageGuidString)>]
[<ProvideLanguageService(languageService = typeof<FSharpLanguageService>,
                         strLanguageName = FSharpConstants.fsharpLanguageName,
                         languageResourceID = 100,
                         MatchBraces = true,
                         MatchBracesAtCaret = true,
                         ShowCompletion = true,
                         ShowMatchingBrace = true,
                         ShowSmartIndent = true,
                         EnableAsyncCompletion = true,
                         QuickInfo = true,
                         DefaultToInsertSpaces  = true,
                         CodeSense = true,
                         DefaultToNonHotURLs = true,
                         EnableCommenting = true,
                         CodeSenseDelay = 100)>]
type internal FSharpPackage() as self =
    inherit Package()
    
    // In case the config file is incorrect, we silently recover and leave the feature enabled
    let enableLanguageService = 
        try 
            "false" <> ConfigurationManager.AppSettings.[FSharpConstants.enableLanguageService]
        with e -> 
            System.Diagnostics.Debug.Assert
              (false, sprintf "Error while loading 'devenv.exe.config' configuration: %A" e)
            true         

    let mutable componentID = 0u
    
    let CreateIfEnabled container serviceType = 
        if enableLanguageService then 
            self.CreateService(container,serviceType) 
        else 
            null
            
    let callback = new ServiceCreatorCallback(CreateIfEnabled)
    
    let mutable mgr : IOleComponentManager = null

#if !VS_VERSION_DEV12
    let fsharpSpecificProfileSettings =
        [| "TextEditor.F#.Insert Tabs", box false
           "TextEditor.F#.Brace Completion", box true
           "TextEditor.F#.Make URLs Hot", box false
           "TextEditor.F#.Indent Style", box 1u |]
#endif

    override self.Initialize() =
        UIThread.CaptureSynchronizationContext()
        self.EstablishDefaultSettingsIfMissing()
        (self :> IServiceContainer).AddService(typeof<FSharpLanguageService>, callback, true)
        base.Initialize()

    /// In case custom VS profile settings for F# are not applied, explicitly set them here.
    /// e.g. 'keep tabs' is the text editor default, but F# requires 'insert spaces'.
    /// We specify our customizations in the General profile for VS, but we have found that in some cases
    /// those customizations are incorrectly ignored.
    member private this.EstablishDefaultSettingsIfMissing() =
#if VS_VERSION_DEV12
        ()  // ISettingsManager only implemented for VS 14.0+
#else
        match this.GetService(typeof<SVsSettingsPersistenceManager>) with
        | :? Microsoft.VisualStudio.Settings.ISettingsManager as settingsManager ->
            for settingName,defaultValue in fsharpSpecificProfileSettings do
                // Only take action if the setting has no current custom value
                // If cloud-synced settings have already been applied or the user has made an explicit change, do nothing
                match settingsManager.TryGetValue(settingName) with
                | Microsoft.VisualStudio.Settings.GetValueResult.Missing, _ ->
                    settingsManager.SetValueAsync(settingName, defaultValue, false) |> ignore
                | _ -> ()
        | _ -> ()
#endif

    member self.RegisterForIdleTime() =
        mgr <- (self.GetService(typeof<SOleComponentManager>) :?> IOleComponentManager)
        if (componentID = 0u && mgr <> null) then
            let crinfo = Array.zeroCreate<OLECRINFO>(1)
            let mutable crinfo0 = crinfo.[0]
            crinfo0.cbSize <- Marshal.SizeOf(typeof<OLECRINFO>) |> uint32
            crinfo0.grfcrf <- uint32 (_OLECRF.olecrfNeedIdleTime ||| _OLECRF.olecrfNeedPeriodicIdleTime)
            crinfo0.grfcadvf <- uint32 (_OLECADVF.olecadvfModal ||| _OLECADVF.olecadvfRedrawOff ||| _OLECADVF.olecadvfWarningsOff)
            crinfo0.uIdleTimeInterval <- 1000u
            crinfo.[0] <- crinfo0 
            let componentID_out = ref componentID
            let _hr = mgr.FRegisterComponent(self, crinfo, componentID_out)
            componentID <- componentID_out.Value
            ()

    member self.CreateService(_container:IServiceContainer, serviceType:Type) =
        match serviceType with 
        | x when x = typeof<FSharpLanguageService> -> 
            let language = new FSharpLanguageService()
            language.SetSite(self)
            language.Initialize()
            self.RegisterForIdleTime()
            box language
        | _ -> null

    override self.Dispose(disposing) =
        try 
            if (componentID <> 0u) then
                begin match self.GetService(typeof<SOleComponentManager>) with 
                | :? IOleComponentManager as mgr -> 
                    mgr.FRevokeComponent(componentID) |> ignore
                | _ -> ()
                end
                componentID <- 0u
        finally
            base.Dispose(disposing)

    interface IOleComponent with

        override x.FContinueMessageLoop(_uReason:uint32, _pvLoopData:IntPtr, _pMsgPeeked:MSG[]) = 
            1

        override x.FDoIdle(grfidlef:uint32) =
            // see e.g "C:\Program Files\Microsoft Visual Studio 2008 SDK\VisualStudioIntegration\Common\IDL\olecm.idl" for details
            //Trace.Print("CurrentDirectoryDebug", (fun () -> sprintf "curdir='%s'\n" (System.IO.Directory.GetCurrentDirectory())))  // can be useful for watching how GetCurrentDirectory changes
            match x.GetService(typeof<FSharpLanguageService>) with 
            | :? FSharpLanguageService as pl -> 
                let periodic = (grfidlef &&& (uint32 _OLEIDLEF.oleidlefPeriodic)) <> 0u
                let mutable r = pl.OnIdle(periodic, mgr)
                if r = 0 && periodic && mgr.FContinueIdle() <> 0 then
                    r <- TaskReporterIdleRegistration.DoIdle(mgr)
                r
            | _ -> 0

        override x.FPreTranslateMessage(_pMsg) = 0

        override x.FQueryTerminate(_fPromptUser) = 1

        override x.FReserved1(_dwReserved, _message, _wParam, _lParam) = 1

        override x.HwndGetWindow(_dwWhich, _dwReserved) = 0n

        override x.OnActivationChange(_pic, _fSameComponent, _pcrinfo, _fHostIsActivating, _pchostinfo, _dwReserved) = ()

        override x.OnAppActivate(_fActive, _dwOtherThreadID) = ()

        override x.OnEnterState(_uStateID, _fEnter)  = ()
        
        override x.OnLoseActivation() = ()

        override x.Terminate() = ()

     
