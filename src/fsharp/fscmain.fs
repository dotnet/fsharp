// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.CommandLineMain

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Runtime.CompilerServices

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.AbstractIL.IL // runningOnMono 
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
        if argv |> Array.exists  (fun x -> x = "/pause" || x = "--pause") then 
            System.Console.WriteLine("Press return to continue...")
            System.Console.ReadLine() |> ignore

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

        mainCompile (ctok, argv, legacyReferenceResolver, (*bannerAlreadyPrinted*)false, (*openBinariesInMemory*)false, (*defaultCopyFSharpCore*)true, quitProcessExiter, ConsoleLoggerProvider(), None, None)
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
