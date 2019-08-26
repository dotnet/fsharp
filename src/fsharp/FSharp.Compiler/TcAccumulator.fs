namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution
open Internal.Utilities
open FSharp.Compiler.Compilation.Utilities

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
module TcAccumulator =

    let rangeStartup = FSharp.Compiler.Range.rangeN "startup" 1

    let create assemblyName (tcConfig: TcConfig) tcGlobals tcImports niceNameGen (loadClosureOpt: LoadClosure option) : TcAccumulator =
        let errorLogger = CompilationErrorLogger("TcAccumulator.create", tcConfig.errorSeverityOptions)
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter)

        let tcInitial = GetInitialTcEnv (assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)
        let tcState = GetInitialTcState (rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, niceNameGen, tcInitial)
        let loadClosureErrors = 
           [| match loadClosureOpt with 
              | None -> ()
              | Some loadClosure -> 
                for inp in loadClosure.Inputs do
                    for (err, isError) in inp.MetaCommandDiagnostics do 
                        yield err, (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning) |]

        let initialErrors = Array.append (loadClosureErrors.ToErrorInfos ()) (errorLogger.GetErrorInfos ())

        { tcState=tcState
          tcEnvAtEndOfFile = tcInitial
          topAttribs = None
          latestImplFile = None
          latestCcuSigForFile = None
          tcErrorsRev = [ initialErrors ] 
          tcModuleNamesDict = Map.empty }
