namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Tast
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution

/// Accumulated results of type checking.
[<NoEquality; NoComparison>]
type internal TcAccumulator =
    { tcState: TcState
      tcEnvAtEndOfFile: TcEnv

      topAttribs: TopAttribs option

      /// Result of checking most recent file, if any
      latestImplFile: TypedImplFile option

      latestCcuSigForFile: ModuleOrNamespaceType option

      tcDependencyFiles: string list

      /// Disambiguation table for module names
      tcModuleNamesDict: ModuleNamesDict

      /// Accumulated errors, last file first
      tcErrorsRev:(PhasedDiagnostic * FSharpErrorSeverity)[] list }

[<RequireQualifiedAccess>]
module internal TcAccumulator =

    val createInitial: TcInitial -> CompilationThreadToken -> Cancellable<TcImports * TcAccumulator>
