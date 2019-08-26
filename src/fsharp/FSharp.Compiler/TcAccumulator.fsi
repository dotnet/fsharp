namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Tast
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TcGlobals
open FSharp.Compiler

/// Accumulated results of type checking.
[<NoEquality; NoComparison>]
type internal TcAccumulator =
    { tcState: TcState
      tcEnvAtEndOfFile: TcEnv

      topAttribs: TopAttribs option

      /// Result of checking most recent file, if any
      latestImplFile: TypedImplFile option

      latestCcuSigForFile: ModuleOrNamespaceType option

      /// Disambiguation table for module names
      tcModuleNamesDict: ModuleNamesDict

      /// Accumulated errors, last file first
      tcErrorsRev: FSharp.Compiler.SourceCodeServices.FSharpErrorInfo [] list }

[<RequireQualifiedAccess>]
module internal TcAccumulator =

    val create: assemblyName: string -> tcConfig: TcConfig -> tcGlobals: TcGlobals -> tcImports: TcImports -> niceNameGen: NiceNameGenerator -> loadClosureOpt: LoadClosure option -> TcAccumulator
