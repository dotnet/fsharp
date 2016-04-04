// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler

open System
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.NameResolution


[<RequireQualifiedAccess>]
type internal FSharpErrorSeverity = 
    | Warning 
    | Error

[<Class>]
type internal FSharpErrorInfo = 
    member FileName: string
    member StartLineAlternate:int
    member EndLineAlternate:int
    [<Obsolete("This member has been replaced by StartLineAlternate, which produces 1-based line numbers rather than a 0-based line numbers. See https://github.com/fsharp/FSharp.Compiler.Service/issues/64")>]
    member StartLine:Line0
    [<Obsolete("This member has been replaced by EndLineAlternate, which produces 1-based line numbers rather than a 0-based line numbers. See https://github.com/fsharp/FSharp.Compiler.Service/issues/64")>]
    member EndLine:Line0
    member StartColumn:int
    member EndColumn:int
    member Severity:FSharpErrorSeverity
    member Message:string
    member Subcategory:string
    member ErrorNumber:int
    static member internal CreateFromExceptionAndAdjustEof : PhasedError * bool * bool * range * lastPosInFile:(int*int) -> FSharpErrorInfo
    static member internal CreateFromException : PhasedError * bool * bool * range -> FSharpErrorInfo

// Implementation details used by other code in the compiler    
[<Sealed>]
type internal ErrorScope = 
    interface IDisposable
    new : unit -> ErrorScope
    member ErrorsAndWarnings : FSharpErrorInfo list
    static member Protect<'a> : range -> (unit->'a) -> (string->'a) -> 'a
    static member ProtectWithDefault<'a> : range -> (unit -> 'a) -> 'a -> 'a
    static member ProtectAndDiscard : range -> (unit -> unit) -> unit

/// Lookup the global static cache for building the FrameworkTcImports
type internal FrameworkImportsCache = 
    new : size: int -> FrameworkImportsCache
    member Get : TcConfig -> TcGlobals * TcImports * AssemblyResolution list * UnresolvedAssemblyReference list
    member Clear: unit -> unit
    member Downsize: unit -> unit
  
/// Used for unit testing
module internal IncrementalBuilderEventTesting =

  type IBEvent =
        | IBEParsed of string // filename
        | IBETypechecked of string // filename
        | IBECreated

  val GetMostRecentIncrementalBuildEvents : int -> IBEvent list
  val GetCurrentIncrementalBuildEventNum : unit -> int

/// An error logger that capture errors, filtering them according to warning levels etc.
type internal CompilationErrorLogger = 
    inherit ErrorLogger

    /// Create the error logger
    new : debugName:string * tcConfig:TcConfig ->  CompilationErrorLogger
            
    /// Get the captured errors
    member GetErrors : unit -> (PhasedError * FSharpErrorSeverity) list

/// Represents the state in the incremental graph assocaited with checking a file
type internal PartialCheckResults = 
    { TcState : TcState 
      TcImports: TcImports 
      TcGlobals: TcGlobals 
      TcConfig: TcConfig 
      TcEnvAtEnd : TypeChecker.TcEnv 
      Errors : (PhasedError * FSharpErrorSeverity) list 
      TcResolutions: TcResolutions list 
      TcSymbolUses: TcSymbolUses list 
      TopAttribs: TypeChecker.TopAttribs option
      TimeStamp: DateTime }

/// Manages an incremental build graph for the build of an F# project
[<Class>]
type internal IncrementalBuilder = 

      /// Increment the usage count on the IncrementalBuilder by 1. Ths initial usage count is 0. The returns an IDisposable which will 
      /// decrement the usage count on the entire build by 1 and dispose if it is no longer used by anyone.
      member IncrementUsageCount : unit -> IDisposable
     
      /// Check if the builder is not disposed
      member IsAlive : bool

      /// The TcConfig passed in to the builder creation.
      member TcConfig : TcConfig

      /// The full set of source files including those from options
      member ProjectFileNames : string list

      /// Raised just before a file is type-checked, to invalidate the state of the file in VS and force VS to request a new direct typecheck of the file.
      /// The incremental builder also typechecks the file (error and intellisense results from the backgroud builder are not
      /// used by VS). 
      member BeforeTypeCheckFile : IEvent<string>

      /// Raised just after a file is parsed
      member FileParsed : IEvent<string>

      /// Raised just after a file is checked
      member FileChecked : IEvent<string>

      /// Raised just after the whole project has finished type checking. At this point, accessing the
      /// overall analysis results for the project will be quick.
      member ProjectChecked : IEvent<unit>

      /// Raised when a type provider invalidates the build.
      member ImportedCcusInvalidated : IEvent<string>

      /// The list of files the build depends on
      member Dependencies : string list
#if EXTENSIONTYPING
      /// Whether there are any 'live' type providers that may need a refresh when a project is Cleaned
      member ThereAreLiveTypeProviders : bool
#endif
      /// Perform one step in the F# build. Return true if the background work is finished.
      member Step : unit -> bool

      /// Get the preceding typecheck state of a slot, without checking if it is up-to-date w.r.t.
      /// the timestamps on files and referenced DLLs prior to this one. Return None if the result is not available.
      /// This is a very quick operation.
      member GetCheckResultsBeforeFileInProjectIfReady: filename:string -> PartialCheckResults option

      /// Get the preceding typecheck state of a slot, but only if it is up-to-date w.r.t.
      /// the timestamps on files and referenced DLLs prior to this one. Return None if the result is not available.
      /// This is a relatively quick operation.
      member AreCheckResultsBeforeFileInProjectReady: filename:string -> bool

      /// Get the preceding typecheck state of a slot. Compute the entire type check of the project up
      /// to the necessary point if the result is not available. This may be a long-running operation.
      ///
      // TODO: make this an Eventually (which can be scheduled) or an Async (which can be cancelled)
      member GetCheckResultsBeforeFileInProject : filename:string -> PartialCheckResults 

      /// Get the typecheck state after checking a file. Compute the entire type check of the project up
      /// to the necessary point if the result is not available. This may be a long-running operation.
      ///
      // TODO: make this an Eventually (which can be scheduled) or an Async (which can be cancelled)
      member GetCheckResultsAfterFileInProject : filename:string -> PartialCheckResults 

      /// Get the typecheck result after the end of the last file. The typecheck of the project is not 'completed'.
      /// This may be a long-running operation.
      ///
      // TODO: make this an Eventually (which can be scheduled) or an Async (which can be cancelled)
      member GetCheckResultsAfterLastFileInProject : unit -> PartialCheckResults 

      /// Get the final typecheck result. If 'generateTypedImplFiles' was set on Create then the TypedAssembly will contain implementations.
      /// This may be a long-running operation.
      ///
      // TODO: make this an Eventually (which can be scheduled) or an Async (which can be cancelled)
      member GetCheckResultsAndImplementationsForProject : unit -> PartialCheckResults * IL.ILAssemblyRef * IRawFSharpAssemblyData option * Tast.TypedAssembly option

      /// Get the logical time stamp that is associated with the output of the project if it were gully built immediately
      member GetLogicalTimeStampForProject: unit -> DateTime

      /// Await the untyped parse results for a particular slot in the vector of parse results.
      ///
      /// This may be a marginally long-running operation (parses are relatively quick, only one file needs to be parsed)
      member GetParseResultsForFile : filename:string -> Ast.ParsedInput option * Range.range * string * (PhasedError * FSharpErrorSeverity) list

      static member TryCreateBackgroundBuilderForProjectOptions : FrameworkImportsCache * scriptClosureOptions:LoadClosure option * sourceFiles:string list * commandLineArgs:string list * projectReferences: IProjectReference list * projectDirectory:string * useScriptResolutionRules:bool * isIncompleteTypeCheckEnvironment : bool * keepAssemblyContents: bool * keepAllBackgroundResolutions: bool -> IncrementalBuilder option * FSharpErrorInfo list 

      static member KeepBuilderAlive : IncrementalBuilder option -> IDisposable
      member IsBeingKeptAliveApartFromCacheEntry : bool

/// Generalized Incremental Builder. This is exposed only for unittesting purposes.
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
        val Map : string -> ('I -> 'O) -> Vector<'I> -> Vector<'O>
        /// Updates the creates a new vector with the same items but with 
        /// timestamp specified by the passed-in function.  
        val Stamp : string -> ('I -> System.DateTime) -> Vector<'I> -> Vector<'I>
        /// Apply a function to each element of the vector, threading an accumulator argument
        /// through the computation. Returns intermediate results in a vector.
        val ScanLeft : string -> ('A -> 'I -> Eventually<'A>) -> Scalar<'A> -> Vector<'I> -> Vector<'A>
        /// Apply a function to a vector to get a scalar value.
        val Demultiplex : string -> ('I[] -> 'O)->Vector<'I> -> Scalar<'O>
        /// Convert a Vector into a Scalar.
        val AsScalar: string -> Vector<'I> -> Scalar<'I[]> 

    type Target = Target of INode * int  option

    /// Evaluate a build. Only required for unit testing.
    val Eval : INode -> PartialBuild -> PartialBuild

    /// Evaluate a build for a vector up to a limit. Only required for unit testing.
    val EvalUpTo : INode * int -> PartialBuild -> PartialBuild

    /// Do one step in the build. Only required for unit testing.
    val Step : Target -> PartialBuild -> PartialBuild option
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
        /// Set the conrete inputs for this build. 
        member GetInitialPartialBuild : vectorinputs: BuildInput list -> PartialBuild

