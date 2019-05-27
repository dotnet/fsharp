// #NoMT #CompilerOptions #Deterministic
//<Expected status=success></Expects>
//
// IMPORTANT: PathMap1/pathmap.fs and PathMap2/pathmap.fs must be identical!
module TestPathMap

open System.Runtime.CompilerServices

type SomeType () =
    static member GetCallerFilePath ([<CallerFilePath>] ?filePath : string) =
        match filePath with
        | None -> "(Unknown)"
        | Some filePath -> filePath

[<EntryPoint>]
let main argv =
    let srcDir = __SOURCE_DIRECTORY__
    printfn "Current __SOURCE_DIRECTORY__ = %s" srcDir
    if srcDir <> "/src" then exit 1

    let srcFile = __SOURCE_FILE__
    printfn "Current __SOURCE_FILE__ = %s" srcDir
    if srcFile <> "pathmap.fs" then exit 1

    let callerFilePath = SomeType.GetCallerFilePath ()
    printfn "Current CallerFilePath = %s" srcDir
    if callerFilePath <> "/src/pathmap.fs" then exit 1

#line 2 @"F:/foo/SomethingElse.fs"

    let srcDir = __SOURCE_DIRECTORY__
    printfn "Changed __SOURCE_DIRECTORY__ = %s" srcDir
    if srcDir <> "/etc/foo" then exit 1

    let srcFile = __SOURCE_FILE__
    printfn "Changed __SOURCE_FILE__ = %s" srcDir
    if srcFile <> "SomethingElse.fs" then exit 1

    let callerFilePath = SomeType.GetCallerFilePath ()
    printfn "Changed CallerFilePath = %s" srcDir
    if callerFilePath <> "/etc/foo/SomethingElse.fs" then exit 1

    0
