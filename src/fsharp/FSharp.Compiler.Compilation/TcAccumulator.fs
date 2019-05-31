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
      tcErrorsRev:(PhasedDiagnostic * FSharpErrorSeverity)[] list }

module TcAccumulator =

    let rangeStartup = FSharp.Compiler.Range.rangeN "startup" 1

    let createInitial tcInitial ctok =
      let tcConfig = tcInitial.tcConfig
      let importsInvalidated = tcInitial.importsInvalidated
      let assemblyName = tcInitial.assemblyName
      let niceNameGen = tcInitial.niceNameGen
      let loadClosureOpt = tcInitial.loadClosureOpt

      cancellable {
        let errorLogger = CompilationErrorLogger("CombineImportedAssembliesTask", tcConfig.errorSeverityOptions)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter)

        let tcConfigP = TcConfigProvider.Constant tcConfig
        let! (tcGlobals, tcImports) = TcImports.BuildResolvedTcImports (ctok, tcConfigP, [])

#if !NO_EXTENSIONTYPING
        tcImports.GetCcusExcludingBase() |> Seq.iter (fun ccu -> 
            // When a CCU reports an invalidation, merge them together and just report a 
            // general "imports invalidated". This triggers a rebuild.
            //
            // We are explicit about what the handler closure captures to help reason about the
            // lifetime of captured objects, especially in case the type provider instance gets leaked
            // or keeps itself alive mistakenly, e.g. via some global state in the type provider instance.
            //
            // The handler only captures
            //    1. a weak reference to the importsInvalidated event.
            //
            // In the invalidation handler we use a weak reference to allow the owner to 
            // be collected if, for some reason, a TP instance is not disposed or not GC'd.
            let capturedImportsInvalidated = WeakReference<_>(importsInvalidated)
            ccu.Deref.InvalidateEvent.Add(fun msg -> 
                match capturedImportsInvalidated.TryGetTarget() with 
                | true, tg -> tg.Trigger msg
                | _ -> ()))
#endif

        let tcInitial = GetInitialTcEnv (assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)
        let tcState = GetInitialTcState (rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, niceNameGen, tcInitial)
        let loadClosureErrors = 
           [ match loadClosureOpt with 
             | None -> ()
             | Some loadClosure -> 
                for inp in loadClosure.Inputs do
                    for (err, isError) in inp.MetaCommandDiagnostics do 
                        yield err, (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning) ]

        let initialErrors = Array.append (Array.ofList loadClosureErrors) (errorLogger.GetErrors())

        let tcAcc = 
            { tcState=tcState
              tcEnvAtEndOfFile=tcInitial
              topAttribs=None
              latestImplFile=None
              latestCcuSigForFile=None
              tcErrorsRev = [ initialErrors ] 
              tcModuleNamesDict = Map.empty }

        return (tcGlobals, tcImports, tcAcc)
        }