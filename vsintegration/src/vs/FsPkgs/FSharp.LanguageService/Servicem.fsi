// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService
  open Microsoft.VisualStudio.TextManager.Interop
  open Microsoft.VisualStudio.Text
  open Microsoft.VisualStudio.FSharp.LanguageService
  open Microsoft.VisualStudio.Shell.Interop
  open Microsoft.FSharp.Compiler.SourceCodeServices
  open Microsoft.VisualStudio.OLE.Interop
  open Microsoft.VisualStudio.Shell
  open System.Collections


  module internal Implementation = 
      [<Class>]
      type internal FSharpScope =
          inherit AuthoringScope
          member GotoDefinition : textView:IVsTextView * row:int * column:int -> GotoDefinitionResult

      [<Class>]
      type UntypedFSharpScope =
          member GetHiddenRegions : file:string -> NewHiddenRegion list * Map<uint32,TextSpan>
          member SynchronizeNavigationDropDown : file:string * line:int * col:int * dropDownTypes:ArrayList * dropDownMembers:ArrayList * selectedType:int byref * selectedMember:int byref -> bool


  [<Class>]
  type internal LanguageServiceState =
      member InteractiveChecker : InteractiveChecker
      member Artifacts : Artifacts
      member GetColorizer : IVsTextLines -> FSharpColorizer
      member Unhook : unit -> unit
      member ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients : unit -> unit
      member WaitForBackgroundCompile : unit -> unit
      member ServiceProvider : Microsoft.VisualStudio.FSharp.LanguageService.ServiceProvider
      member CreateSource : buffer:IVsTextLines -> IdealSource
      member ExecuteBackgroundRequest : req:BackgroundRequest * source:IdealSource -> unit
      member UntypedParseScope : Implementation.UntypedFSharpScope option
      member Initialize : sp:Microsoft.VisualStudio.FSharp.LanguageService.ServiceProvider * dp:IdealDocumentationProvider * prefs:LanguagePreferences * enableStandaloneFileIntellisenseFlag:bool * createSource:(IVsTextLines -> IdealSource) -> unit
      member CreateBackgroundRequest : line : int * col : int * info : TokenInfo * sourceText : string * snapshot : ITextSnapshot * methodTipMiscellany : MethodTipMiscellany * fname : string *
                                         reason : BackgroundRequestReason * view : IVsTextView *
                                         sink : AuthoringSink * source : ISource * timestamp : int * synchronous : bool -> BackgroundRequest
      member OnIdle : unit -> unit
      static member Create : unit -> LanguageServiceState



  [<Class>]
  type internal FSharpLanguageService =
      inherit LanguageService
      interface IVsRunningDocTableEvents
      interface IVsProvideColorableItems     
      member LanguageServiceState : LanguageServiceState

  [<Class>]
  type internal FSharpPackage =
      inherit Package
      interface IOleComponent
      new : unit -> FSharpPackage

