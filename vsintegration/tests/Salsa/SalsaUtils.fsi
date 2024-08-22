// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Salsa

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.ProjectSystem
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.TextManager.Interop
open FSharp.Compiler.SourceCodeServices
open Microsoft.Build.Framework

open Salsa


/// Functions for validating tests using VsOps. (Calls Assert behind the scenes.)
module internal VsOpsUtils =

    // ------------------------------------------------------------------------
    
    // Wrappers on top of VSOps functions, these make it easier to interact with
    // VSOps as well as enabling us to change VsOps slightly without breaking the world

    val CreateSolution                    : VisualStudio -> OpenSolution
    val GetOutputWindowPaneLines          : VisualStudio -> string list
    val CloseSolution                     : OpenSolution -> unit

    val CreateProject                     : OpenSolution * string -> OpenProject
    /// Add a new file not in any particular project.
    val NewFile                           : VisualStudio * string * string list -> File
    val DeleteFileFromDisk : VisualStudio * File -> unit
    val AddFileFromText                   : OpenProject * string * string list -> File
    val AddFileFromTextBlob               : OpenProject * string * string -> File
    val AddFileFromTextEx                 : OpenProject * string * string * Salsa.Salsa.BuildAction * string list -> File
    val AddLinkedFileFromTextEx           : OpenProject * string * string * string * string list -> File
    val AddAssemblyReference              : OpenProject * string -> unit 
    val AddAssemblyReferenceEx              : OpenProject * string * bool -> unit 
    val AddProjectReference               : OpenProject * OpenProject -> unit 
    val ProjectDirectory                  : OpenProject -> string
    val ProjectFile                       : OpenProject -> string
    val SetVersionFile                    : OpenProject * string -> unit
    val SetOtherFlags                     : OpenProject * string -> unit
    val SetConfigurationAndPlatform       : OpenProject * string -> unit
    val AddDisabledWarning                : OpenProject * string -> unit
    val Build                             : OpenProject -> BuildResult
    val BuildTarget                       : OpenProject * string -> BuildResult
    val GetMainOutputAssembly             : OpenProject -> string
    val Save                              : OpenProject -> unit
    val GetErrors                         : OpenProject -> Error list 
    /// Open a file outside of any project as if from File\Open\File... menu item. 
    val OpenFileViaOpenFile               : VisualStudio * string -> OpenFile
    val OpenFile                          : OpenProject * string -> OpenFile 
    val GetOpenFiles                      : OpenProject -> OpenFile list
    val SetProjectDefines                 : OpenProject * string list -> unit
    val PlaceIntoProjectFileBeforeImport  : OpenProject * string -> unit
    val OpenExistingProject               : VisualStudio * string * string -> OpenProject * OpenSolution
    val MoveCursorTo                      : OpenFile * int * int -> unit
    val GetCursorLocation                 : OpenFile -> int * int
    val GetLineNumber                     : OpenFile -> int -> string
    val GetAllLines                       : OpenFile -> string list
    val SwitchToFile                      : VisualStudio -> OpenFile -> unit
    val OnIdle                            : VisualStudio -> unit
    val ShiftKeyDown                      : VisualStudio -> unit
    val ShiftKeyUp                        : VisualStudio -> unit
    val ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients : VisualStudio -> unit
    val TakeCoffeeBreak                   : VisualStudio -> unit 
    val ReplaceFileInMemory               : OpenFile -> string list -> unit
    val ReplaceFileInMemoryWithoutCoffeeBreak   : OpenFile -> string list -> unit
    val SaveFileToDisk                    : OpenFile -> unit
    val GetSquiggleAtCursor               : OpenFile -> (Microsoft.VisualStudio.FSharp.LanguageService.Severity * string) option
    val GetSquigglesAtCursor               : OpenFile -> (Microsoft.VisualStudio.FSharp.LanguageService.Severity * string) list
    /// does a BackgroundRequestReason.MemberSelect at the cursor
    val AutoCompleteAtCursor              : OpenFile -> CompletionItem array
    /// does a BackgroundRequestReason.CompleteWord at the cursor
    val CtrlSpaceCompleteAtCursor         : OpenFile -> CompletionItem array
    /// like AutoCompleteAtCursor, but can pass e.g. BackgroundRequestReason.CompleteWord to do Ctrl-space rather than auto-dot-popup-completion
    val CompleteAtCursorForReason         : OpenFile * Microsoft.VisualStudio.FSharp.LanguageService.BackgroundRequestReason -> CompletionItem array
    val CompletionBestMatchAtCursorFor    : OpenFile * string * string option -> (string * bool * bool) option 
    val MoveCursorToEndOfMarker           : OpenFile * string -> unit
    val MoveCursorToStartOfMarker         : OpenFile * string -> unit
    val GetQuickInfoAtCursor              : OpenFile -> string   
    val GetQuickInfoAndSpanAtCursor       : OpenFile -> string*TextSpan
    val GetNameOfOpenFile                 : OpenFile -> string
    val GetProjectOptionsOfScript           : OpenFile -> FSharpProjectOptions
    val GetParameterInfoAtCursor          : OpenFile -> MethodListForAMethodTip_DEPRECATED option
    val GetTokenTypeAtCursor              : OpenFile -> Salsa.Salsa.TokenType
    val GetIdentifierAtCursor             : OpenFile -> (string * int) option
    val GetF1KeywordAtCursor              : OpenFile -> string option
    val GotoDefinitionAtCursor            : OpenFile -> GotoDefnResult
    val GotoDefinitionAtCursorForceGeneration : OpenFile -> GotoDefnResult
    val GetNavigationContentAtCursor      : OpenFile -> NavigationBarResult
    val GetHiddenRegionCommands           : OpenFile -> list<NewHiddenRegion> * Map<uint32, TextSpan>
    val Cleanup                           : VisualStudio -> unit

    val OutOfConeFilesAreAddedAsLinks     : VisualStudio -> bool
    val SupportsOutputWindowPane     : VisualStudio -> bool


    /// True if files outside of the project cone are added as links.
    val AutoCompleteMemberDataTipsThrowsScope : VisualStudio * string -> System.IDisposable

    
    val CreateSingleFileProject      : VisualStudio * string -> (OpenSolution * OpenProject * OpenFile)
    val CreateNamedSingleFileProject : VisualStudio * (string * string) -> (OpenSolution * OpenProject * OpenFile)
    val GetMatchingBracesForPositionAtCursor : OpenFile -> (TextSpan * TextSpan) array
    
    // ------------------------------------------------------------------------

    // Methods to simplify testing of specific features

    type SetMarkerPoint =
        | StartOfMarker
        | EndOfMarker

    // Navigation items & regions
    val AssertRegionListContains        : list<(int*int)*(int*int)> * list<NewHiddenRegion> -> unit
    val AssertNavigationContains        : DropDownMember[] * string -> unit
    val AssertNavigationContainsAll     : DropDownMember[] * seq<string> -> unit
    
    // Completion list
    val AssertCompListIsEmpty           : CompletionItem[] -> unit
    val AssertCompListContains          : CompletionItem[] * string -> unit
    val AssertCompListDoesNotContain    : CompletionItem[] * string -> unit
    val AssertCompListContainsAll       : CompletionItem[] * string list -> unit
    val AssertCompListContainsExactly   : CompletionItem[] * string list -> unit
    val AssertCompListDoesNotContainAny : CompletionItem[] * string list -> unit

    // Dot completion
    val DotCompletionAtMarker        : SetMarkerPoint -> OpenFile -> string -> CompletionItem[]
    val DotCompletionAtStartOfMarker : (OpenFile -> string -> CompletionItem[])
    val DotCompletionAtEndOfMarker   : (OpenFile -> string -> CompletionItem[])
    
    
    // Goto Definition
    val GotoDefnFailure : (string * string) option
    val GotoDefnSuccess : string -> string -> (string * string) option
    
    val CheckGotoDefnResult   : (string * string) option -> OpenFile -> GotoDefnResult -> unit
