namespace Test

open System.Runtime.CompilerServices
open CSharpLib

type MyTy() =
    static member GetCallerFilePath([<CallerFilePath>] ?path : string) =
        path

module Program =
    let matchesPath path (s : string) =
        s.EndsWith(path)
          && not (s.Contains("\\\\"))
          && not (path.Contains("\\.\\"))
          && not (path.Contains("\\..\\"))

        
    [<EntryPoint>]
    let main (_:string[]) =
    
        match MyTy.GetCallerFilePath() with
        | Some(path) when matchesPath "Conformance\\SpecialAttributesAndTypes\\Imported\\CallerInfo\\CallerFilePath.fs" path -> ()
        | x -> failwithf "Unexpected: %A" x
        
        match MyTy.GetCallerFilePath("42") with
        | Some("42") -> ()
        | x -> failwithf "Unexpected: %A" x
        
        match CallerInfoTest.FilePath() with
        | path when matchesPath "Conformance\\SpecialAttributesAndTypes\\Imported\\CallerInfo\\CallerFilePath.fs" path -> ()
        | x -> failwithf "Unexpected: %A" x
        
        match CallerInfoTest.FilePath("xyz") with
        | "xyz" -> ()
        | x -> failwithf "Unexpected: %A" x
        
        match CallerInfoTest.AllInfo(21) with
        | (path, _) when matchesPath "Conformance\\SpecialAttributesAndTypes\\Imported\\CallerInfo\\CallerFilePath.fs" path -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x
        
        0