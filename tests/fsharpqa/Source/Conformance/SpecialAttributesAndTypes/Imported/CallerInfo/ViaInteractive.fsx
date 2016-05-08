open System.Runtime.CompilerServices
open CSharpLib

type MyTy() =
    static member GetCallerFilePath([<CallerFilePath>] ?path : string) =
        path
    static member GetCallerLineNumber([<CallerLineNumber>] ?line : int) =
        line

let matchesPath path (s : string) =
    s.EndsWith(path)
     && not (s.Contains("\\\\"))
     && not (path.Contains("\\.\\"))
     && not (path.Contains("\\..\\"))

let scriptName = if Array.contains "--exec" (System.Environment.GetCommandLineArgs()) then "ViaInteractive.fsx" else "stdin"
let checkPath = sprintf "Conformance\\SpecialAttributesAndTypes\\Imported\\CallerInfo\\%s" scriptName

match MyTy.GetCallerFilePath() with
| Some(path) when matchesPath checkPath path -> ()
| x -> failwithf "Unexpected: %A" x

if MyTy.GetCallerLineNumber() <> Some(23) then
    failwith "Unexpected F# CallerLineNumber"

match CallerInfoTest.AllInfo(21) with
| (path, 26, ".cctor") when matchesPath checkPath path -> ()
| x -> failwithf "Unexpected C# result with multiple parameter types: %A" x

#q