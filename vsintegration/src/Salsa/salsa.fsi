// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Salsa

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.ProjectSystem
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.TextManager.Interop

open Microsoft.Build.Framework
open Microsoft.VisualStudio.FSharp.LanguageService.Implementation

module internal Salsa = 
    
    [<Class>]
    type HostCompile = 
        interface ITaskHost 
        member Compile : System.Converter<int,int> * string[] * string[] -> int
        
    
    type TokenType = Text | Keyword | Comment | Identifier | String | Number | InactiveCode | PreprocessorKeyword | Operator
    
    /// Declaration types.
    type DeclarationType = 
        | Class         = 0
        | Constant      = 6
        | FunctionType  = 12            // Like 'type FunctionType=unit->unit' 
        | Enum          = 18
        | EnumMember    = 24
        | Event         = 30
        | Exception     = 36
        | Interface     = 48
        | Method        = 72
        | FunctionValue = 74            // Like 'type Function x = 0'
        | Module        = 84
        | Namespace     = 90
        | Property      = 102
        | ValueType     = 108           // Like 'type ValueType=int*int' 
        | RareType      = 120           // Bucket for unusual types like 'type AsmType = (# "!0[]" #)'
        | Record        = 126
        | DiscriminatedUnion = 132 
        
    type BuildAction =
        | Compile = 0
        | EmbeddedResource = 1
        | None = 2        
        
    type BuildResult = {
        ExecutableOutput : string
        BuildSucceeded : bool
    }
    
    /// An error
    [<Sealed>]
    type Error  = 
        member Path : string
        member Message : string
        member Context : TextSpan
        member Severity : Severity
        override ToString : unit -> string
    


    type ChangeCallBack = IVsHierarchy * string -> unit

    /// Hooks for controlling behaviors
    
    
    /// Thrown when a marker is not found when placing the cursor via VsOps
    exception MarkerNotFoundException of string
    
    /// Representation of an item from the completion list
    type CompletionItem = string * string * (unit -> string) * DeclarationType
    
    type GotoDefnResult = Microsoft.VisualStudio.FSharp.LanguageService.GotoDefinitionResult
    [<AutoOpen>]
    module GotoDefnResultExtensions = 
        type Microsoft.VisualStudio.FSharp.LanguageService.GotoDefinitionResult with
            member ToOption : unit -> (TextSpan * string) option

    /// Representes the information that is displayed in the navigation bar
    type NavigationBarResult = 
      { TypesAndModules : DropDownMember[]
        Members : DropDownMember[]
        SelectedType : int
        SelectedMember : int }
    
    /// Methods for simulating VisualStudio
    [<NoEquality; NoComparison>]
    type ProjectBehaviorHooks = {
        CreateProjectHook:  (*projectFilename:*)string ->
                            (*files:*)(string*BuildAction*string option) list ->
                            (*references:*)(string*bool) list ->
                            (*projReferences:*)string list ->
                            (*disabledWarnings:*)string list ->
                            (*defines*)string list ->
                            (*versionFile:*)string ->
                            (*otherFlags:*)string ->
                            (*preImportXml:*)string ->
                            (*targetFrameworkVersion:*)string -> unit
        InitializeProjectHook : OpenProject -> unit
        MakeHierarchyHook : string->string->string->ChangeCallBack->OleServiceProvider->IVsHierarchy
        AddFileToHierarchyHook : string -> IVsHierarchy -> unit
        BuildHook : (*basename:*)string -> (*target:*)string -> IVsOutputWindowPane -> BuildResult
        GetMainOutputAssemblyHook : string -> string
        SaveHook : unit -> unit
        DestroyHook : unit->unit
        ModifyConfigurationAndPlatformHook : string->unit
    }   
    and [<NoEquality; NoComparison>] VsOps = {
        CreateVisualStudio                : unit -> VisualStudio
        CreateSolution                    : VisualStudio -> OpenSolution
        GetOutputWindowPaneLines          : VisualStudio -> string list
        CloseSolution                     : OpenSolution ->unit
        CreateProject                     : OpenSolution * string -> OpenProject
        CreateProjectWithHooks            : OpenSolution * ProjectBehaviorHooks * string -> OpenProject
        NewFile                           : VisualStudio * string * BuildAction * string list -> File
        DeleteFileFromDisk : File -> unit
        AddFileFromText                   : OpenProject * string * string * BuildAction * string list -> File
        AddLinkedFileFromText             : OpenProject*string*string*string*BuildAction*string list->File
        AddAssemblyReference              : OpenProject * string * bool -> unit 
        AddProjectReference               : OpenProject * OpenProject -> unit 
        ProjectDirectory                  : OpenProject -> string
        ProjectFile                       : OpenProject -> string
        SetVersionFile                    : OpenProject * string -> unit
        SetOtherFlags                     : OpenProject * string -> unit
        SetConfigurationAndPlatform       : OpenProject * string -> unit
        AddDisabledWarning                : OpenProject * string -> unit
        GetErrors                         : OpenProject -> Error list 
        BuildProject                      : OpenProject * string -> BuildResult 
        GetMainOutputAssembly             : OpenProject -> string
        SaveProject                       : OpenProject -> unit        
        OpenFileViaOpenFile               : VisualStudio * string -> OpenFile
        OpenFile                          : OpenProject * string -> OpenFile 
        GetOpenFiles                      : OpenProject -> OpenFile list
        SetProjectDefines                 : OpenProject * string list -> unit
        PlaceIntoProjectFileBeforeImport  : OpenProject * string -> unit
        OpenExistingProject               : VisualStudio * string * string -> OpenProject * OpenSolution
        MoveCursorTo                      : OpenFile * int * int -> unit
        GetCursorLocation                 : OpenFile -> int * int
        GetLineNumber                     : OpenFile -> int -> string
        GetAllLines                       : OpenFile -> string list
        SwitchToFile                      : VisualStudio * OpenFile -> unit
        OnIdle                            : VisualStudio -> unit
        ShiftKeyDown                      : VisualStudio -> unit
        ShiftKeyUp                        : VisualStudio -> unit
        TakeCoffeeBreak                   : VisualStudio -> unit 
        ReplaceFileInMemory               : OpenFile * string list * bool -> unit
        SaveFileToDisk                    : OpenFile -> unit
        CleanUp                           : VisualStudio -> unit
        CleanInvisibleProject             : VisualStudio -> unit
        ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients : VisualStudio -> unit
        GetSquiggleAtCursor               : OpenFile -> (Microsoft.VisualStudio.FSharp.LanguageService.Severity * string) option
        GetSquigglesAtCursor              : OpenFile -> (Microsoft.VisualStudio.FSharp.LanguageService.Severity * string) list
        /// does a BackgroundRequestReason.MemberSelect at the cursor
        AutoCompleteAtCursor              : OpenFile -> CompletionItem array
        /// like AutoCompleteAtCursor, but can pass e.g. BackgroundRequestReason.CompleteWord to do Ctrl-space rather than auto-dot-popup-completion
        CompleteAtCursorForReason         : OpenFile * Microsoft.VisualStudio.FSharp.LanguageService.BackgroundRequestReason -> CompletionItem array
        CompletionBestMatchAtCursorFor    : OpenFile * string * string option -> (string * bool * bool) option
        MoveCursorToEndOfMarker           : OpenFile * string -> unit
        MoveCursorToStartOfMarker         : OpenFile * string -> unit
        GetQuickInfoAtCursor              : OpenFile -> string  
        GetQuickInfoAndSpanAtCursor       : OpenFile -> string*TextSpan
        GetMatchingBracesForPositionAtCursor : OpenFile -> (TextSpan * TextSpan) array
        GetNameOfOpenFile                 : OpenFile -> string
        GetCheckOptionsOfScript           : OpenFile -> Microsoft.FSharp.Compiler.SourceCodeServices.CheckOptions
        GetParameterInfoAtCursor          : OpenFile -> MethodListForAMethodTip
        GetParameterInfoAtCursorNoFallback: OpenFile -> MethodListForAMethodTip
        GetTokenTypeAtCursor              : OpenFile -> TokenType
        GetIdentifierAtCursor             : OpenFile -> (string * int) option
        GetF1KeywordAtCursor              : OpenFile -> string option
        GotoDefinitionAtCursor            : OpenFile -> bool -> GotoDefnResult
        GetNavigationContentAtCursor      : OpenFile -> NavigationBarResult
        GetHiddenRegionCommands           : OpenFile -> list<NewHiddenRegion> * Map<uint32, TextSpan>
        CreatePhysicalProjectFileInMemory : ((*files:*)(string*BuildAction*string option) list) ->
                                            ((*references:*)(string*bool) list) ->
                                            ((*projectReferences:*)string list) ->
                                            ((*disabledWarnings:*)string list) ->
                                            ((*defines:*)string list) ->
                                            (*versionFile*) string ->
                                            ((*otherFlags:*)string) ->
                                            ((*otherProjMisc:*)string) -> 
                                            ((*targetFrameworkVersion:*)string) -> string
                
        /// True if files outside of the project cone are added as links.
        AutoCompleteMemberDataTipsThrowsScope : string -> System.IDisposable
        
        // VsOps capabilities.
        OutOfConeFilesAreAddedAsLinks : bool
        SupportsOutputWindowPane : bool   
    }

    // Opaque handles to vs objects
    /// Simulate a VisualStudio instance
    and VisualStudio = interface 
        abstract VsOps : VsOps
    end
    /// The solution opened in VS
    and OpenSolution = interface 
        abstract VS : VisualStudio
    end
    /// A project opened in VS
    and OpenProject = interface 
        abstract VS : VisualStudio
    end
    /// A file opened in VS
    and OpenFile = interface 
        abstract VS : VisualStudio
    end
    /// A file on disk
    and File = interface end    
    
    val CreateFSharpManifestResourceName : projectFileName:string -> configuration:string -> platform:string -> (string * string) list

    /// The different variations of of Salsa tests    
    module Models = 
        /// Salsa tests which create .fsproj files for projects.
        val MSBuild : unit -> VsOps * ProjectBehaviorHooks
        /// Salsa tests which create .fsproj files for projects using the installed
        /// FSharp.targets file.
        val InstalledMSBuild : unit -> VsOps * ProjectBehaviorHooks
