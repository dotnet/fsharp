module Tests.Service.HashDirectiveInfoTests

open System.IO
open Microsoft.FSharp.Compiler.Range
open NUnit.Framework
open Microsoft.FSharp.Compiler.SourceCodeServices.PathUtils
open FSharp.Compiler.Service.Tests.Common
open Microsoft.FSharp.Compiler.SourceCodeServices.HashDirectiveInfo

[<Literal>]
let dataFolderName = __SOURCE_DIRECTORY__ + "/data/"

let getTestFilename filename =
    dataFolderName </> "ParsedLoadDirectives" </> filename

let canonicalizeFilename filename = Path.GetFullPathSafe filename

let getAst filename = 
    let contents = File.ReadAllText(filename)
    match parseSourceCode (filename, contents) with
    | Some tree -> tree
    | None -> failwithf "Something went wrong during parsing %s!" filename

[<Test>]
let ``test1.fsx: verify parsed #load directives``() =
   let ast = getAst (getTestFilename "test1.fsx")
   let directives = getIncludeAndLoadDirectives ast
   
   let expectedMatches =
       [
       Some (FileInfo(getTestFilename "includes/a.fs").FullName)
       Some (FileInfo(getTestFilename "includes/b.fs").FullName)
       Some (FileInfo(getTestFilename "includes/b.fs").FullName)
       ]
   
   let results =
       directives
       |> Seq.map (function
          | Load(ExistingFile(filename), _) -> Some ((new FileInfo(filename)).FullName)
          | _ -> None
       )
       |> Seq.filter (Option.isSome)
       |> Seq.toList
   
   Assert.AreEqual(expectedMatches, results)

[<Test>]
let ``test1.fsx: verify parsed position lookup of individual #load directives``() =
    let ast = getAst (getTestFilename "test1.fsx")
    
    let expectations = [
      (mkPos 1 1,       Some (FileInfo(getTestFilename "includes/a.fs").FullName))
      (mkPos 1 5,       Some (FileInfo(getTestFilename "includes/a.fs").FullName))
      (mkPos 2 1,       Some (FileInfo(getTestFilename "includes/b.fs").FullName))
      (mkPos 2 5,       Some (FileInfo(getTestFilename "includes/b.fs").FullName))
      (mkPos 3 1000,    None)
      (mkPos 4 5,       Some (FileInfo(getTestFilename "includes/b.fs").FullName))
    ]
    
    let results =
        expectations
        |> Seq.map fst
        |> Seq.map (fun pos -> 
           let result = getHashLoadDirectiveResolvedPathAtPosition pos ast
           match result with
           | None      -> pos, None
           | Some path -> pos, Some (canonicalizeFilename path)
        )
        |> Seq.toList
    
    Assert.AreEqual(expectations, results)