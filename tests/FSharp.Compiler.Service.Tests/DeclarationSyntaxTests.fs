// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.DeclarationSyntaxTests

open System
open System.IO
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.Declarations
open FSharp.Compiler.Service.Tests.Common
open Xunit

let private mkOptions (sourceFiles: string[]) =
    let _, baseOptions = mkTestFileAndOptions Array.empty
    { baseOptions with SourceFiles = sourceFiles }

let private testPath (name: string) =
    Path.Combine(Path.GetTempPath(), "fcs-decl-syntax-tests", name)

[<Fact>]
let ``Split and count match top and nested declarations`` () =
    let file = testPath "SplitAndCount.fs"
    let ctx = DeclarationProjectCheck.createProject (mkOptions [| file |]) |> Async.RunSynchronously

    let source =
        """module Sample.Split

type Pair = { Left: int; Right: int }

let topValue = 1

module Nested =
    let inner x = x + 1
    let inner x y = x + y
"""

    let parse = DeclarationSyntax.parse ctx file source
    let decls = DeclarationSyntax.split parse

    Assert.Equal(DeclarationSyntax.countDeclarations parse, decls.Length)
    Assert.True(decls.Length >= 4)

    let pairDecl = decls |> Array.find (fun d -> d.Key.Name = "Pair")
    Assert.Equal(DeclarationKind.Type, pairDecl.Key.Kind)
    Assert.Equal<string list>([ "Sample"; "Split" ], pairDecl.Key.ModulePath)

    let innerDecls = decls |> Array.filter (fun d -> d.Key.Name = "inner")
    Assert.Equal(2, innerDecls.Length)
    Assert.Equal<int>([| 0; 1 |], innerDecls |> Array.map (fun d -> d.Key.Ordinal))

    let nestedHeader =
        decls
        |> Array.find (fun d -> d.Key.Name = "Nested" && d.Key.Kind = DeclarationKind.Module)

    Assert.Equal(Some nestedHeader.Key, innerDecls[0].Within)

[<Fact>]
let ``IsUnchangedFrom ignores leading XML doc comments and detects body edits`` () =
    let file = testPath "DocSensitivity.fs"
    let ctx = DeclarationProjectCheck.createProject (mkOptions [| file |]) |> Async.RunSynchronously

    let v1 =
        """module Sample.Docs

/// Original doc comment
let add x y = x + y
"""

    let v2 =
        """module Sample.Docs

/// Updated doc comment with more text
let add x y = x + y
"""

    let v3 =
        """module Sample.Docs

/// Updated doc comment with more text
let add x y = x + y + 1
"""

    let d1 = (DeclarationSyntax.split (DeclarationSyntax.parse ctx file v1))[0]
    let d2 = (DeclarationSyntax.split (DeclarationSyntax.parse ctx file v2))[0]
    let d3 = (DeclarationSyntax.split (DeclarationSyntax.parse ctx file v3))[0]

    Assert.True(d2.IsUnchangedFrom d1)
    Assert.False(d3.IsUnchangedFrom d1)

[<Fact>]
let ``Content tracks defines, uses, member uses, field or case uses, and open`` () =
    let file = testPath "ContentTracking.fs"
    let ctx = DeclarationProjectCheck.createProject (mkOptions [| file |]) |> Async.RunSynchronously

    let source =
        """module Sample.Content

open System

type Choice =
    | Yes of int
    | No

let inspect (s: string) c =
    match c with
    | Yes n -> s.Length + n
    | No -> 0
"""

    let decls = DeclarationSyntax.split (DeclarationSyntax.parse ctx file source)
    let openContent = DeclarationSyntax.content decls[0]
    Assert.True(openContent.AffectsAllLater)

    let typeContent = DeclarationSyntax.content decls[1]
    Assert.Contains("Choice", typeContent.Defines)
    Assert.Contains("Yes", typeContent.Defines)
    Assert.Contains("No", typeContent.Defines)

    let valContent = DeclarationSyntax.content decls[2]
    Assert.Contains("inspect", valContent.Defines)
    Assert.Contains("Length", valContent.MemberUses)
    Assert.Contains("Yes", valContent.FieldOrCaseUses)
    Assert.Contains("No", valContent.FieldOrCaseUses)

[<Fact>]
let ``FileDependencies computes intraproject dependencies`` () =
    let fileA = testPath "DepA.fs"
    let fileB = testPath "DepB.fs"
    let fileC = testPath "DepC.fs"
    let ctx = DeclarationProjectCheck.createProject (mkOptions [| fileA; fileB; fileC |]) |> Async.RunSynchronously

    let pA = DeclarationSyntax.parse ctx fileA "module Alpha\nlet x = 1\n"
    let pB = DeclarationSyntax.parse ctx fileB "module Beta\nopen Alpha\nlet y = x + 1\n"
    let pC = DeclarationSyntax.parse ctx fileC "module Gamma\nlet z = 42\n"

    let deps = DeclarationSyntax.fileDependencies ctx [| pA; pB; pC |]

    Assert.Equal(3, deps.Length)
    Assert.Empty(deps[0])
    Assert.Equal<int>([| 0 |], deps[1])
    Assert.Empty(deps[2])

[<Fact>]
let ``SymbolKeys resolve definitions across signature and implementation`` () =
    let sigFile = testPath "Paired.fsi"
    let implFile = testPath "Paired.fs"
    let ctx = DeclarationProjectCheck.createProject (mkOptions [| sigFile; implFile |]) |> Async.RunSynchronously

    let sigSource =
        """module Sample.Paired

val add: a: int -> b: int -> int
"""

    let implSource =
        """module Sample.Paired

let add a b = a + b
"""

    let pSig = DeclarationSyntax.parse ctx sigFile sigSource
    let pImpl = DeclarationSyntax.parse ctx implFile implSource

    let sigResult = DeclarationProjectCheck.checkSignatureFile ctx [] pSig
    let implResult = DeclarationProjectCheck.checkFile ctx [ sigResult ] pImpl None

    Assert.Empty(implResult.Diagnostics)

    let uses = implResult.CheckResults.GetAllUsesOfAllSymbolsInFile(CancellationToken.None)
    let addDef = uses |> Seq.find (fun u -> u.IsFromDefinition && u.Symbol.DisplayName = "add")

    let key =
        match DeclarationSymbolKeys.symbolKey addDef.Symbol with
        | Some k -> k
        | None -> failwith "Expected a symbol key for Paired.add"

    let defLoc = DeclarationSymbolKeys.definitionLocation implResult.CheckResults key CancellationToken.None
    let sigLoc = DeclarationSymbolKeys.signatureLocation ctx sigResult key true CancellationToken.None

    Assert.True(defLoc.IsSome)
    Assert.True(sigLoc.IsSome)
    Assert.Equal(sigFile, defLoc.Value.FileName)
    Assert.Equal(sigFile, sigLoc.Value.FileName)

    let unpairedFile = testPath "Unpaired.fs"
    let unpairedCtx = DeclarationProjectCheck.createProject (mkOptions [| unpairedFile |]) |> Async.RunSynchronously
    let unpairedParse = DeclarationSyntax.parse unpairedCtx unpairedFile "module Sample.Unpaired\nlet solo x = x + 1\n"
    let unpairedResult = DeclarationProjectCheck.checkFile unpairedCtx [] unpairedParse None
    let soloUse =
        unpairedResult.CheckResults.GetAllUsesOfAllSymbolsInFile(CancellationToken.None)
        |> Seq.find (fun u -> u.IsFromDefinition && u.Symbol.DisplayName = "solo")
    let soloKey = DeclarationSymbolKeys.symbolKey soloUse.Symbol |> Option.get
    let soloLoc = DeclarationSymbolKeys.definitionLocation unpairedResult.CheckResults soloKey CancellationToken.None
    Assert.True(soloLoc.IsSome)
    Assert.Equal(unpairedFile, soloLoc.Value.FileName)
