// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.CommandLineMain

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Runtime.CompilerServices

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.ILBinaryReader 
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Driver
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Internal.Utilities

#if FX_RESHAPED_REFLECTION
open Microsoft.FSharp.Core.ReflectionAdapters
#endif

#if !FX_NO_DEFAULT_DEPENDENCY_TYPE
[<Dependency("FSharp.Compiler.Private",LoadHint.Always)>] 
#endif
do ()


module Driver = 
    let main argv = 

        let ctok = AssumeCompilationThreadWithoutEvidence ()

        // Check for --pause as the very first step so that a compiler can be attached here.
        let pauseFlag = argv |> Array.exists  (fun x -> x = "/pause" || x = "--pause")
        if pauseFlag then 
            System.Console.WriteLine("Press return to continue...")
            System.Console.ReadLine() |> ignore

#if !FX_NO_APP_DOMAINS
        let timesFlag = argv |> Array.exists  (fun x -> x = "/times" || x = "--times")
        if timesFlag then 
            let stats = ILBinaryReader.GetStatistics()
            AppDomain.CurrentDomain.ProcessExit.Add(fun _ -> printfn "STATS: #ByteArrayFile = %d, #MemoryMappedFileOpen = %d, #MemoryMappedFileClosed = %d, #RawMemoryFile = %d, #WeakByteArrayFile = %d" stats.byteFileCount stats.memoryMapFileOpenedCount stats.memoryMapFileClosedCount stats.rawMemoryFileCount stats.weakByteFileCount)
#endif

        let quitProcessExiter = 
            { new Exiter with 
                member x.Exit(n) =                    
                    try 
                      exit n
                    with _ -> 
                      ()            
                    failwithf "%s" <| FSComp.SR.elSysEnvExitDidntExit() 
            }

        let legacyReferenceResolver = 
#if CROSS_PLATFORM_COMPILER
            SimulatedMSBuildReferenceResolver.SimulatedMSBuildResolver
#else
            MSBuildReferenceResolver.Resolver
#endif

        // This is the only place where ReduceMemoryFlag.No is set. This is because fsc.exe is not a long-running process and
        // thus we can use file-locking memory mapped files.
        //
        // This is also one of only two places where CopyFSharpCoreFlag.Yes is set.  The other is in LegacyHostedCompilerForTesting.
        mainCompile (ctok, argv, legacyReferenceResolver, (*bannerAlreadyPrinted*)false, ReduceMemoryFlag.No, CopyFSharpCoreFlag.Yes, quitProcessExiter, ConsoleLoggerProvider(), None, None)
        0 

[<EntryPoint>]
let main(argv) =
    System.Runtime.GCSettings.LatencyMode <- System.Runtime.GCLatencyMode.Batch
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter

#if !FX_NO_HEAPTERMINATION
    if not runningOnMono then Lib.UnmanagedProcessExecutionOptions.EnableHeapTerminationOnCorruption() (* SDL recommendation *)
    Lib.UnmanagedProcessExecutionOptions.EnableHeapTerminationOnCorruption() (* SDL recommendation *)
#endif

    try 
        Driver.main(Array.append [| "fsc.exe" |] argv)
    with e -> 
        errorRecovery e Microsoft.FSharp.Compiler.Range.range0
        1
