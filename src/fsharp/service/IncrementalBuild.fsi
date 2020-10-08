// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open System

open FSharp.Compiler
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Range
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

open Microsoft.DotNet.DependencyManager

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

/// Accumulated results of type checking. The minimum amount of state in order to continue type-checking following files.
[<NoEquality; NoComparison>]
type internal TcInfo =
    {
        tcState: TcState
        tcEnvAtEndOfFile: TypeChecker.TcEnv

        /// Disambiguation table for module names
        moduleNamesDict: ModuleNamesDict

        topAttribs: TypeChecker.TopAttribs option

        latestCcuSigForFile: ModuleOrNamespaceType option

        /// Accumulated errors, last file first
        tcErrorsRev:(PhasedDiagnostic * FSharpErrorSeverity)[] list

        tcDependencyFiles: string list

        sigNameOpt: (string * SyntaxTree.QualifiedNameOfFile) option
    }

     member TcErrors: (PhasedDiagnostic * FSharpErrorSeverity)[]

/// Accumulated results of type checking. Optional data that isn't needed to type-check a file, but needed for more information for in tooling.
[<NoEquality; NoComparison>]
type internal TcInfoOptional =
    {
      /// Accumulated resolutions, last file first
      tcResolutionsRev: TcResolutions list

      /// Accumulated symbol uses, last file first
      tcSymbolUsesRev: TcSymbolUses list

      /// Accumulated 'open' declarations, last file first
      tcOpenDeclarationsRev: OpenDeclaration[] list

      /// Result of checking most recent file, if any
      latestImplFile: TypedImplFile option
      
      /// If enabled, stores a linear list of ranges and strings that identify an Item(symbol) in a file. Used for background find all references.
      itemKeyStore: ItemKeyStore option
      
      /// If enabled, holds semantic classification information for Item(symbol)s in a file.
      semanticClassification: struct (range * SemanticClassificationType) []
    }

    member TcSymbolUses: TcSymbolUses list

/// Represents the state in the incremental graph associated with checking a file
[<Sealed>]
type internal PartialCheckResults = 

    member TcImports: TcImports 

    member TcGlobals: TcGlobals 

    member TcConfig: TcConfig 

    member TimeStamp: DateTime 

    member TcInfo: CompilationThreadToken -> TcInfo

    /// Can cause a second type-check if `enablePartialTypeChecking` is true in the checker.
    /// Only use when it's absolutely necessary to get rich information on a file.
    member TcInfoWithOptional: CompilationThreadToken -> TcInfo * TcInfoOptional

    member TimeStamp: DateTime 

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

      /// Does the given file exist in the builder's pipeline?
      member ContainsFile: filename: string -> bool

      /// Await the untyped parse results for a particular slot in the vector of parse results.
      ///
      /// This may be a marginally long-running operation (parses are relatively quick, only one file needs to be parsed)
      member GetParseResultsForFile: CompilationThreadToken * filename:string -> Cancellable<ParsedInput option * Range.range * string * (PhasedDiagnostic * FSharpErrorSeverity)[]>

      /// Create the incremental builder
      static member TryCreateIncrementalBuilderForProjectOptions:
          CompilationThreadToken *
          ReferenceResolver.Resolver *
          defaultFSharpBinariesDir: string * 
          FrameworkImportsCache *
          scriptClosureOptions:LoadClosure option *
          sourceFiles:string list *
          commandLineArgs:string list *
          projectReferences: IProjectReference list *
          projectDirectory:string *
          useScriptResolutionRules:bool *
          keepAssemblyContents: bool *
          keepAllBackgroundResolutions: bool *
          maxTimeShareMilliseconds: int64 *
          tryGetMetadataSnapshot: ILBinaryReader.ILReaderTryGetMetadataSnapshot *
          suggestNamesForErrors: bool *
          keepAllBackgroundSymbolUses: bool *
          enableBackgroundItemKeyStoreAndSemanticClassification: bool *
          enablePartialTypeChecking: bool *
          dependencyProvider: DependencyProvider option
             -> Cancellable<IncrementalBuilder option * FSharpErrorInfo[]>

/// Generalized Incremental Builder. This is exposed only for unit testing purposes.
module internal IncrementalBuild =
    type INode = 
        abstract Name: string

    type ScalarBuildRule 
    type VectorBuildRule 

    [<Interface>]
    type IScalar = 
        inherit INode
        abstract Expr: ScalarBuildRule

    [<Interface>]
    type IVector =
        inherit INode
        abstract Expr: VectorBuildRule
            
    type Scalar<'T> =  interface inherit IScalar  end

    type Vector<'T> = interface inherit IVector end

    /// A set of build rules and the corresponding, possibly partial, results from building.
    type PartialBuild 

    /// Declares a vector build input.
    /// Only required for unit testing.
    val InputScalar : string -> Scalar<'T>

    /// Declares a scalar build input.
    /// Only required for unit testing.
    val InputVector : string -> Vector<'T>

    /// Methods for acting on build Vectors
    /// Only required for unit testing.
    module Vector = 
        /// Maps one vector to another using the given function.    
        val Map : string -> (CompilationThreadToken -> 'I -> 'O) -> Vector<'I> -> Vector<'O>
        /// Updates the creates a new vector with the same items but with 
        /// timestamp specified by the passed-in function.  
        val Stamp : string -> (TimeStampCache -> CompilationThreadToken -> 'I -> System.DateTime) -> Vector<'I> -> Vector<'I>
        /// Apply a function to each element of the vector, threading an accumulator argument
        /// through the computation. Returns intermediate results in a vector.
        val ScanLeft : string -> (CompilationThreadToken -> 'A -> 'I -> Eventually<'A>) -> Scalar<'A> -> Vector<'I> -> Vector<'A>
        /// Apply a function to a vector to get a scalar value.
        val Demultiplex : string -> (CompilationThreadToken -> 'I[] -> Cancellable<'O>)->Vector<'I> -> Scalar<'O>
        /// Convert a Vector into a Scalar.
        val AsScalar: string -> Vector<'I> -> Scalar<'I[]> 

    type Target = Target of INode * int  option

    /// Used for unit testing. Causes all steps of underlying incremental graph evaluation to cancel
    val LocallyInjectCancellationFault : unit -> IDisposable
    
    /// Evaluate a build. Only required for unit testing.
    val Eval : TimeStampCache -> CompilationThreadToken -> (CompilationThreadToken -> PartialBuild -> unit) -> INode -> PartialBuild -> Cancellable<PartialBuild>

    /// Evaluate a build for a vector up to a limit. Only required for unit testing.
    val EvalUpTo : TimeStampCache -> CompilationThreadToken -> (CompilationThreadToken -> PartialBuild -> unit) -> INode * int -> PartialBuild -> Cancellable<PartialBuild>

    /// Do one step in the build. Only required for unit testing.
    val Step : TimeStampCache -> CompilationThreadToken -> (CompilationThreadToken -> PartialBuild -> unit) -> Target -> PartialBuild -> Cancellable<PartialBuild option>

    /// Get a scalar vector. Result must be available. Only required for unit testing.
    val GetScalarResult : Scalar<'T> * PartialBuild -> ('T * System.DateTime) option

    /// Get a result vector. All results must be available or thrown an exception. Only required for unit testing.
    val GetVectorResult : Vector<'T> * PartialBuild -> 'T[]

    /// Get an element of vector result or None if there were no results. Only required for unit testing.
    val GetVectorResultBySlot<'T> : Vector<'T> * int * PartialBuild -> ('T * System.DateTime) option

    [<Sealed>]
    type BuildInput =
        /// Declare a named scalar output.
        static member ScalarInput: node:Scalar<'T> * value: 'T -> BuildInput
        static member VectorInput: node:Vector<'T> * value: 'T list -> BuildInput

    /// Declare build outputs and bind them to real values.
    /// Only required for unit testing.
    type BuildDescriptionScope = 
        new : unit -> BuildDescriptionScope

        /// Declare a named scalar output.
        member DeclareScalarOutput : output:Scalar<'T> -> unit

        /// Declare a named vector output.
        member DeclareVectorOutput : output:Vector<'T> -> unit

        /// Set the concrete inputs for this build. 
        member GetInitialPartialBuild : vectorinputs: BuildInput list -> PartialBuild

