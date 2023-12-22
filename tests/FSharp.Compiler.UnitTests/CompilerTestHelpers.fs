namespace FSharp.Compiler.UnitTests

open System
open System.IO
open System.Threading.Tasks

open FSharp.Compiler.CodeAnalysis
open FSharp.Test.Utilities

[<AutoOpen>]
module CompilerTestHelpers =

    type Async with
        static member RunImmediate (computation: Async<'T>, ?cancellationToken ) =
            let cancellationToken = defaultArg cancellationToken Async.DefaultCancellationToken
            let ts = TaskCompletionSource<'T>()
            let task = ts.Task
            Async.StartWithContinuations(
                computation,
                (fun k -> ts.SetResult k),
                (fun exn -> ts.SetException exn),
                (fun _ -> ts.SetCanceled()),
                cancellationToken)
            task.Result

    let (|Warning|_|) (exn: Exception) =
        match exn with
        | :? FSharp.Compiler.DiagnosticsLogger.DiagnosticWithText as e -> Some (e.number, e.message)
        | _ -> None

    let sysLib nm =
#if !NETCOREAPP
        if Environment.OSVersion.Platform = PlatformID.Win32NT then // file references only valid on Windows
            let programFilesx86Folder = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)")
            $@"{programFilesx86Folder}\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\{nm}.dll"
        else
#endif
        let sysDir = AppContext.BaseDirectory
        let (++) a b = Path.Combine(a,b)
        sysDir ++ nm + ".dll"

    let PathRelativeToTestAssembly p = Path.Combine(Path.GetDirectoryName(Uri(typeof<FSharpChecker>.Assembly.Location).LocalPath), p)

    let fsCoreDefaultReference() =
        PathRelativeToTestAssembly "FSharp.Core.dll"

    let mkStandardProjectReferences () =
        TargetFrameworkUtil.currentReferences

    let mkProjectCommandLineArgsSilent (dllName, fileNames) =
      let args =
        [|  yield "--simpleresolution"
            yield "--noframework"
            yield "--debug:full"
            yield "--define:DEBUG"
    #if NETCOREAPP
            yield "--targetprofile:netcore"
            yield "--langversion:preview"
    #endif
            yield "--optimize-"
            yield "--out:" + dllName
            yield "--doc:test.xml"
            yield "--warn:3"
            yield "--fullpaths"
            yield "--flaterrors"
            yield "--target:library"
            for x in fileNames do
                yield x
            let references = mkStandardProjectReferences ()
            for r in references do
                yield "-r:" + r
         |]
      args

    let mkProjectCommandLineArgs (dllName, fileNames) =
        let args = mkProjectCommandLineArgsSilent (dllName, fileNames)
        printfn "dllName = %A, args = %A" dllName args
        args