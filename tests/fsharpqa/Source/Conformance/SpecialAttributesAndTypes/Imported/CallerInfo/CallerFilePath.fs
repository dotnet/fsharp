namespace Test

open System.Runtime.CompilerServices
open CSharpLib

type MyTy([<CallerFilePath>] ?p0 : string) =
    let mutable p = p0

    member x.Path with get() = p

    static member GetCallerFilePath([<CallerFilePath>] ?path : string) =
        path

module Program =
    let doubleSeparator = "##".Replace('#', System.IO.Path.DirectorySeparatorChar)
    let sameDirectory = "#.#".Replace('#', System.IO.Path.DirectorySeparatorChar)
    let parentDirectory = ".."
    let matchesPath (path : string) (s : string) =
        s.EndsWith(path.Replace('#', System.IO.Path.DirectorySeparatorChar))
          && not (s.Contains(doubleSeparator))
          && not (s.Contains(sameDirectory))
          && not (s.Contains(parentDirectory))

        
    [<EntryPoint>]
    let main (_:string[]) =
        let o = MyTy()
        let o1 = MyTy("42")

        match o.Path with
        | Some(path) when matchesPath "Conformance#SpecialAttributesAndTypes#Imported#CallerInfo#CallerFilePath.fs" path -> ()
        | x -> failwithf "Unexpected: %A" x

        match o1.Path with
        | Some(path) when matchesPath "42" path -> ()
        | x -> failwithf "Unexpected: %A" x

        match MyTy.GetCallerFilePath() with
        | Some(path) when matchesPath "Conformance#SpecialAttributesAndTypes#Imported#CallerInfo#CallerFilePath.fs" path -> ()
        | x -> failwithf "Unexpected: %A" x
        
        match MyTy.GetCallerFilePath("42") with
        | Some("42") -> ()
        | x -> failwithf "Unexpected: %A" x
        
        match CallerInfoTest.FilePath() with
        | path when matchesPath "Conformance#SpecialAttributesAndTypes#Imported#CallerInfo#CallerFilePath.fs" path -> ()
        | x -> failwithf "Unexpected: %A" x
        
        match CallerInfoTest.FilePath("xyz") with
        | "xyz" -> ()
        | x -> failwithf "Unexpected: %A" x
        
        match CallerInfoTest.AllInfo(21) with
        | (path, _, _) when matchesPath "Conformance#SpecialAttributesAndTypes#Imported#CallerInfo#CallerFilePath.fs" path -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x

# 345 "qwerty1"
        match CallerInfoTest.AllInfo(123) with
        | (path, _, _) when matchesPath "Conformance#SpecialAttributesAndTypes#Imported#CallerInfo#qwerty1" path -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x

# 456 "qwerty2"
        match CallerInfoTest.AllInfo(123) with
        | (path, _, _) when matchesPath "Conformance#SpecialAttributesAndTypes#Imported#CallerInfo#qwerty2" path -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x

        0