[<NUnit.Framework.TestFixture>][<NUnit.Framework.Parallelizable>]
module Tests.OpenDeclarationsGetter

open NUnit.Framework
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.VisualStudio.FSharp.Editor


let openDeclWithAutoOpens decls =
    { Declarations = 
        decls |> List.map (fun (decl: string) -> decl.Split '.')
      Parent = None
      IsUsed = false }

(*** OpenDeclWithAutoOpens ***)

[<Test>]
let ``OpenDeclWithAutoOpens becomes used after updated with a matching symbol prefix``() =
    let decl = openDeclWithAutoOpens ["System.IO"]
    let matched, updatedDecl = OpenDeclWithAutoOpens.updateBySymbolPrefix [|"System"; "IO"|] decl
    assertTrue matched
    assertTrue updatedDecl.IsUsed

[<Test>]
let ``OpenDeclWithAutoOpens does not become used after updated with a not matching symbol prefix``() =
    let decl = openDeclWithAutoOpens ["System.IO"]
    let matched, updatedDecl = OpenDeclWithAutoOpens.updateBySymbolPrefix [|"System"|] decl
    assertFalse matched
    assertFalse updatedDecl.IsUsed

[<Test>]
let ``OpenDeclWithAutoOpens stays used after updated with a not matching symbol prefix``() =
    let decl = { openDeclWithAutoOpens ["System.IO"] with IsUsed = true }
    let matched, updatedDecl = OpenDeclWithAutoOpens.updateBySymbolPrefix [|"System"|] decl
    assertFalse matched
    assertTrue updatedDecl.IsUsed

[<Test>]
let ``OpenDeclWithAutoOpens with many declarations becomes used after updated with a matching symbol prefix``() =
    let decl = openDeclWithAutoOpens 
                 ["System.IO"
                  "System.IO.AutoOpenModule1"
                  "System.IO.AutoOpenModule2"]
    let matched, updatedDecl = OpenDeclWithAutoOpens.updateBySymbolPrefix [|"System"; "IO"; "AutoOpenModule1"|] decl
    assertTrue matched
    assertTrue updatedDecl.IsUsed

(*** OpenDeclaration ***)

let internal range (startLine, startCol) (endLine, endCol) =
    Range.mkRange "range" (Range.mkPos startLine startCol) (Range.mkPos endLine endCol)

let internal openDecl decls =
    { Declarations = decls
      DeclarationRange = range (0, 0) (1, 1)
      ScopeRange = range (0, 0) (100, 100)
      IsUsed = false }

[<Test>]
let ``OpenDecl becomes used if any of its declarations is used``() =
    let decl = openDecl
                 [ openDeclWithAutoOpens ["System.IO"]
                   openDeclWithAutoOpens ["Top.Module"]]
    let matched, updatedDecl = OpenDeclaration.updateBySymbolPrefix [|"Top"; "Module"|] decl
    assertTrue matched
    assertTrue updatedDecl.IsUsed
    assertEqual [false; true] (updatedDecl.Declarations |> List.map (fun decl -> decl.IsUsed))

[<Test>]
let ``OpenDecl stays used if any of its declarations is used``() =
    let decl = { openDecl
                  [ { openDeclWithAutoOpens ["System.IO"] with IsUsed = true }
                    openDeclWithAutoOpens ["Top.Module"]] 
                 with IsUsed = true }

    let matched, updatedDecl = OpenDeclaration.updateBySymbolPrefix [|"Not"; "Matching"|] decl
    assertFalse matched
    assertTrue updatedDecl.IsUsed
    assertEqual [true; false] (updatedDecl.Declarations |> List.map (fun decl -> decl.IsUsed))

[<Test>]
let ``OpenDecl marks matching child decl even though another one is already marked as used``() =
    let decl = { openDecl
                  [ { openDeclWithAutoOpens ["System.IO"] with IsUsed = true }
                    openDeclWithAutoOpens ["Top.Module"]] 
                 with IsUsed = true }

    let matched, updatedDecl = OpenDeclaration.updateBySymbolPrefix [|"Top"; "Module"|] decl
    assertTrue matched
    assertTrue updatedDecl.IsUsed
    assertEqual [true; true] (updatedDecl.Declarations |> List.map (fun decl -> decl.IsUsed))

(*** OpenDeclarationGetter ***)
 
[<Test>]
let ``first matched decl become Used, the rest decls - do not``() =
    // declaration here and in the rest of the tests are in REVERSE ORDER
    let decls =
        [ openDecl [ openDeclWithAutoOpens ["Not.Matching"]]
          openDecl [ openDeclWithAutoOpens ["System.IO.AutoOpenModule"]]
          openDecl [ openDeclWithAutoOpens ["System.IO"; "System.IO.AutoOpenModule"]]
        ]

    let updatedDecls = 
        OpenDeclarationGetter.updateOpenDeclsWithSymbolPrefix 
            [|"System"; "IO"; "AutoOpenModule"|] (range (1, 1) (2, 2)) decls

    CollectionAssert.AreEqual ([false; true; false], updatedDecls |> List.map (fun decl -> decl.IsUsed))

[<Test>]
let ``matched decl become Used, there is another already matched one below it``() =
    // declaration here and in the rest of the tests are in REVERSE ORDER
    let decls =
        [ { openDecl [ { openDeclWithAutoOpens ["Other.Already.Matched"] with IsUsed = true }] with IsUsed = true }
          openDecl [ openDeclWithAutoOpens ["System.IO"]]
        ]

    let updatedDecls = 
        OpenDeclarationGetter.updateOpenDeclsWithSymbolPrefix 
            [|"System"; "IO"|] (range (1, 1) (2, 2)) decls

    CollectionAssert.AreEqual ([true; true], updatedDecls |> List.map (fun decl -> decl.IsUsed))

(*** setOpenDeclsIsUsedFlag ***)

[<Test>]
let ``set IsUsed flag to all declarations which have a given parent``() =
    let decls =
        [ { openDecl [ { openDeclWithAutoOpens ["Top.Module.AlreadyUsed"] with IsUsed = true }] with IsUsed = true }
          openDecl [ openDeclWithAutoOpens ["Top.Module.NotUsed"]]
          openDecl [ openDeclWithAutoOpens ["System.IO"]]
          openDecl [ openDeclWithAutoOpens ["System.IO.Module"]]
        ]
    let updatedDecls =  OpenDeclarationGetter.setOpenDeclsIsUsedFlag [|"System"; "IO"|] decls
    CollectionAssert.AreEqual ([true; false; true; false], (updatedDecls |> List.map (fun d -> d.IsUsed)))

(*** spreadIsUsedFlagToParents ***)

[<Test>]
let ``spread IsUsed flag up to parents``() =
    let decls =
        [ { openDecl [ { openDeclWithAutoOpens ["System.IO.Module"] with Parent = Some [|"System"; "IO"|]; IsUsed = true }]
            with IsUsed = true }
          openDecl [ openDeclWithAutoOpens ["System.IO"]]
        ]
    let updatedDecls =  OpenDeclarationGetter.spreadIsUsedFlagToParents decls
    CollectionAssert.AreEqual ([true; true], (updatedDecls |> List.map (fun d -> d.IsUsed)))

[<Test>]
let ``spread IsUsed flag up to parents /two levels/``() =
    let decls =
        [ { openDecl [ { openDeclWithAutoOpens ["System.IO.Module"] with Parent = Some [|"System"; "IO"|]; IsUsed = true }]
            with IsUsed = true }
          openDecl [ { openDeclWithAutoOpens ["System.IO"] with Parent = Some [|"System"|] } ]
          openDecl [ openDeclWithAutoOpens ["System.IO"]]
        ]
    let updatedDecls =  OpenDeclarationGetter.spreadIsUsedFlagToParents decls
    CollectionAssert.AreEqual ([true; true; true], (updatedDecls |> List.map (fun d -> d.IsUsed)))