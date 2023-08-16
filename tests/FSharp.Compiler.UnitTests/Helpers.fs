[<AutoOpen>]
module FSharp.Compiler.UnitTests.Helpers

open System
open System.IO
open System.Threading.Tasks

open FSharp.Compiler.CodeAnalysis

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