// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Infrastructure for testing C# type forwarding with F#.
// Pattern: compile C# original lib → compile F# exe → swap lib with forwarder → run.

namespace Conformance.TypeForwarding

open System
open System.IO
open System.Reflection
open FSharp.Test.Compiler
open FSharp.Test

#if NETCOREAPP
open System.Runtime.Loader
#endif

type TypeForwardingResult =
    | TFSuccess of stdout: string
    | TFExecutionFailure of exn: exn
    | TFCompilationFailure of stage: string * errors: string

module TypeForwardingHelpers =

    let private getDiagnosticMessages (result: CompilationResult) =
        let diags =
            match result with
            | CompilationResult.Success output -> output.Diagnostics
            | CompilationResult.Failure output -> output.Diagnostics
        diags |> List.map (fun d -> d.Message) |> String.concat "\n"

    let private tryRunAssembly (dllPath: string) =
#if NETCOREAPP
        try
            let alc = new AssemblyLoadContext(Guid.NewGuid().ToString("N"), isCollectible = true)
            try
                let dllDir = Path.GetDirectoryName(dllPath)
                alc.add_Resolving(fun _ name ->
                    let candidate = Path.Combine(dllDir, name.Name + ".dll")
                    if File.Exists(candidate) then alc.LoadFromAssemblyPath(candidate) else null)
                let asm = alc.LoadFromAssemblyPath(dllPath)
                let entryPoint = asm.EntryPoint
                if isNull entryPoint then
                    TFCompilationFailure("NoEntryPoint", "No entry point found in F# assembly")
                else
                    entryPoint.Invoke(null, [| (Array.empty<string> :> obj) |]) |> ignore
                    TFSuccess ""
            finally
                alc.Unload()
        with
        | :? TargetInvocationException as tie -> TFExecutionFailure tie.InnerException
        | ex -> TFExecutionFailure ex
#else
        ignore dllPath
        TFExecutionFailure(System.NotSupportedException("TypeForwarding runtime tests require .NET Core"))
#endif

    /// Verify type forwarding: compile original C# lib, compile F# exe referencing it,
    /// compile target + forwarder, swap DLLs, run the F# exe via reflection.
    let verifyTypeForwarding (originalCSharp: string) (forwarderCSharp: string) (targetCSharp: string) (fsharpSource: string) : TypeForwardingResult =
        let outputDir = DirectoryInfo(Path.Combine(Path.GetTempPath(), "tf_" + Guid.NewGuid().ToString("N")))
        outputDir.Create()

        try
            // Step 1+2: Compile F# exe referencing original C# lib (framework compiles C# automatically)
            let originalLib = CSharp originalCSharp |> withName "Original"
            let fsharpExe =
                Fs fsharpSource
                |> withName "FSharpTest"
                |> asExe
                |> withReferences [originalLib]
                |> withOutputDirectory (Some outputDir)
                |> compile

            match fsharpExe with
            | CompilationResult.Failure _ ->
                TFCompilationFailure("FSharpCompilation", getDiagnosticMessages fsharpExe)
            | CompilationResult.Success fsharpOutput ->

            let fsharpDllPath =
                fsharpOutput.OutputPath
                |> Option.defaultWith (fun () -> Path.Combine(outputDir.FullName, "FSharpTest.dll"))

            let fsharpDir = Path.GetDirectoryName(fsharpDllPath)

            // Step 3: Compile target C# lib into a separate directory
            let targetDir = DirectoryInfo(Path.Combine(outputDir.FullName, "target"))
            targetDir.Create()

            let targetUnit =
                CSharp targetCSharp
                |> withName "Target"
                |> withOutputDirectory (Some targetDir)

            let targetResult = targetUnit |> compile

            match targetResult with
            | CompilationResult.Failure _ ->
                TFCompilationFailure("TargetCSharp", getDiagnosticMessages targetResult)
            | CompilationResult.Success _ ->

            // Step 4: Compile forwarder C# lib referencing target
            let forwarderDir = DirectoryInfo(Path.Combine(outputDir.FullName, "forwarder"))
            forwarderDir.Create()

            let forwarderResult =
                CSharp forwarderCSharp
                |> withName "Original"
                |> withReferences [targetUnit]
                |> withOutputDirectory (Some forwarderDir)
                |> compile

            match forwarderResult with
            | CompilationResult.Failure _ ->
                TFCompilationFailure("ForwarderCSharp", getDiagnosticMessages forwarderResult)
            | CompilationResult.Success _ ->

            // Step 5: Copy Target.dll to the F# exe directory
            let targetDllSrc = Path.Combine(targetDir.FullName, "Target.dll")
            let targetDllDest = Path.Combine(fsharpDir, "Target.dll")
            if File.Exists(targetDllSrc) then
                File.Copy(targetDllSrc, targetDllDest, true)

            // Step 6: Replace Original.dll with forwarder version
            let originalDllDest = Path.Combine(fsharpDir, "Original.dll")
            let forwarderDllSrc = Path.Combine(forwarderDir.FullName, "Original.dll")
            if File.Exists(forwarderDllSrc) then
                File.Copy(forwarderDllSrc, originalDllDest, true)

            // Step 7: Run the F# exe
            tryRunAssembly fsharpDllPath

        finally
            try outputDir.Delete(true) with _ -> ()

    let shouldSucceed (result: TypeForwardingResult) : unit =
        match result with
        | TFSuccess _ -> ()
        | TFExecutionFailure ex -> failwith $"Expected success but got execution failure: {ex.Message}"
        | TFCompilationFailure (stage, errors) -> failwith $"Expected success but got compilation failure at {stage}: {errors}"
