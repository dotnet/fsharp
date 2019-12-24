// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open System
open FSharp.Compiler
open FSharp.Compiler.Range
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.CompileOps
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Tast
open FSharp.Compiler.SourceCodeServices

/// Lookup the global static cache for building the FrameworkTcImports
type internal FrameworkImportsCache = 
    new : size: int -> FrameworkImportsCache
    member Get : CompilationThreadToken * TcConfig -> Cancellable<TcGlobals * TcImports * AssemblyResolution list * UnresolvedAssemblyReference list>
    member Clear: CompilationThreadToken -> unit
    member Downsize: CompilationThreadToken -> unit
  
/// Used for unit testing
module internal IncrementalBuilderEventTesting =

  type IBEvent =
        | IBEParsed of string // filename
        | IBETypechecked of string // filename
        | IBECreated

  val GetMostRecentIncrementalBuildEvents : int -> IBEvent list
  val GetCurrentIncrementalBuildEventNum : unit -> int

/// Represents the state in the incremental graph associated with checking a file
type internal PartialCheckResults = 
    { /// This field is None if a major unrecovered error occurred when preparing the initial state
      TcState : TcState
      TcImports: TcImports 
      TcGlobals: TcGlobals 
      TcConfig: TcConfig 

      /// This field is None if a major unrecovered error occurred when preparing the initial state
      TcEnvAtEnd : TypeChecker.TcEnv

      /// Represents the collected errors from type checking
      TcErrorsRev : (PhasedDiagnostic * FSharpErrorSeverity)[] list 

      /// Represents the collected name resolutions from type checking
      TcResolutionsRev: TcResolutions list 

      /// Represents the collected uses of symbols from type checking
      TcSymbolUsesRev: TcSymbolUses list 

      /// Represents open declarations
      TcOpenDeclarationsRev: OpenDeclaration[] list

      /// Disambiguation table for module names
      ModuleNamesDict: ModuleNamesDict

      TcDependencyFiles: string list

      /// Represents the collected attributes to apply to the module of assembly generates
      TopAttribs: TypeChecker.TopAttribs option

      TimeStamp: DateTime 
      
      /// Represents latest complete typechecked implementation file, including its typechecked signature if any.
      /// Empty for a signature file.
      LatestImplementationFile: TypedImplFile option 
      
      /// Represents latest inferred signature contents.
      LatestCcuSigForFile: ModuleOrNamespaceType option}

    member TcErrors: (PhasedDiagnostic * FSharpErrorSeverity)[]

    member TcSymbolUses: TcSymbolUses list

/// Manages an incremental build graph for the build of an F# project
[<Class>]
type internal IncrementalBuilder = 

      /// The TcConfig passed in to the builder creation.
      member TcConfig : TcConfig

      /// The full set of source files including those from options
      member SourceFiles : string list

      /// Raised just before a file is type-checked, to invalidate the state of the file in VS and force VS to request a new direct typecheck of the file.
      /// The incremental builder also typechecks the file (error and intellisense results from the background builder are not
      /// used by VS). 
      member BeforeFileChecked : IEvent<string>

      /// Raised just after a file is parsed
      member FileParsed : IEvent<string>

      /// Raised just after a file is checked
      member FileChecked : IEvent<string>

      /// Raised just after the whole project has finished type checking. At this point, accessing the
      /// overall analysis results for the project will be quick.
      member ProjectChecked : IEvent<unit>

#if !NO_EXTENSIONTYPING
      /// Raised when a type provider invalidates the build.
      member ImportsInvalidatedByTypeProvider : IEvent<string>
#endif

      /// Tries to get the current successful TcImports. This is only used in testing. Do not use it for other stuff.
      member TryGetCurrentTcImports : unit -> TcImports option

      /// The list of files the build depends on
      member AllDependenciesDeprecated : string[]

      /// Perform one step in the F# build. Return true if the background work is finished.
      member Step : CompilationThreadToken -> Cancellable<bool>

      /// Get the preceding typecheck state of a slot, without checking if it is up-to-date w.r.t.
      /// the timestamps on files and referenced DLLs prior to this one. Return None if the result is not available.
      /// This is a very quick operation.
      ///
      /// This is safe for use from non-compiler threads but the objects returned must in many cases be accessed only from the compiler thread.
      member GetCheckResultsBeforeFileInProjectEvenIfStale: filename:string -> PartialCheckResults option

      /// Get the preceding typecheck state of a slot, but only if it is up-to-date w.r.t.
      /// the timestamps on files and referenced DLLs prior to this one. Return None if the result is not available.
      /// This is a relatively quick operation.
      ///
      /// This is safe for use from non-compiler threads
      member AreCheckResultsBeforeFileInProjectReady: filename:string -> bool

      /// Get the preceding typecheck state of a slot. Compute the entire type check of the project up
      /// to the necessary point if the result is not available. This may be a long-running operation.
      ///
      // TODO: make this an Eventually (which can be scheduled) or an Async (which can be cancelled)
      member GetCheckResultsBeforeFileInProject : CompilationThreadToken * filename:string -> Cancellable<PartialCheckResults>

      /// Get the typecheck state after checking a file. Compute the entire type check of the project up
      /// to the necessary point if the result is not available. This may be a long-running operation.
      ///
      // TODO: make this an Eventually (which can be scheduled) or an Async (which can be cancelled)
      member GetCheckResultsAfterFileInProject : CompilationThreadToken * filename:string -> Cancellable<PartialCheckResults>

      /// Get the typecheck result after the end of the last file. The typecheck of the project is not 'completed'.
      /// This may be a long-running operation.
      ///
      // TODO: make this an Eventually (which can be scheduled) or an Async (which can be cancelled)
      member GetCheckResultsAfterLastFileInProject : CompilationThreadToken -> Cancellable<PartialCheckResults>

      /// Get the final typecheck result. If 'generateTypedImplFiles' was set on Create then the TypedAssemblyAfterOptimization will contain implementations.
      /// This may be a long-running operation.
      ///
      // TODO: make this an Eventually (which can be scheduled) or an Async (which can be cancelled)
      member GetCheckResultsAndImplementationsForProject : CompilationThreadToken -> Cancellable<PartialCheckResults * IL.ILAssemblyRef * IRawFSharpAssemblyData option * TypedImplFile list option>

      /// Get the logical time stamp that is associated with the output of the project if it were gully built immediately
      member GetLogicalTimeStampForProject: TimeStampCache * CompilationThreadToken -> DateTime

      /// Await the untyped parse results for a particular slot in the vector of parse results.
      ///
      /// This may be a marginally long-running operation (parses are relatively quick, only one file needs to be parsed)
      member GetParseResultsForFile : CompilationThreadToken * filename:string -> Cancellable<Ast.ParsedInput option * Range.range * string * (PhasedDiagnostic * FSharpErrorSeverity)[]>

      static member TryCreateBackgroundBuilderForProjectOptions : CompilationThreadToken * ReferenceResolver.Resolver * defaultFSharpBinariesDir: string * FrameworkImportsCache * scriptClosureOptions:LoadClosure option * sourceFiles:string list * commandLineArgs:string list * projectReferences: IProjectReference list * projectDirectory:string * useScriptResolutionRules:bool * keepAssemblyContents: bool * keepAllBackgroundResolutions: bool * maxTimeShareMilliseconds: int64 * tryGetMetadataSnapshot: ILBinaryReader.ILReaderTryGetMetadataSnapshot * suggestNamesForErrors: bool -> Cancellable<IncrementalBuilder option * FSharpErrorInfo[]>

module internal IncrementalBuild =

    /// Used for unit testing. Causes all steps of underlying incremental graph evaluation to cancel
    val LocallyInjectCancellationFault : unit -> IDisposable