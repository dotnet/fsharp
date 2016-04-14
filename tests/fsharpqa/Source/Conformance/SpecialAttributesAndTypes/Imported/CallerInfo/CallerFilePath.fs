namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerFilePath([<CallerFilePath>] ?path : string) =
        path

module Program =
    [<EntryPoint>]
    let main (_:string[]) =
        match MyTy.GetCallerFilePath() with
        | Some(path) when path.EndsWith("Conformance\SpecialAttributesAndTypes\Imported\CallerInfo\CallerFilePath.fs") -> 0
        | _ -> 1
