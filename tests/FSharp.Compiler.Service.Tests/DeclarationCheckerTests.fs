// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.DeclarationCheckerTests

open System
open System.IO
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.Declarations
open FSharp.Compiler.Service.Tests.Common
open Xunit

let private mkOptions (extraArgs: string[]) (sourceFiles: string[]) =
    let _, baseOptions = mkTestFileAndOptions extraArgs
    { baseOptions with SourceFiles = sourceFiles }

let private testPath (name: string) =
    Path.Combine(Path.GetTempPath(), "fcs-decl-checker-tests", name)

[<Fact>]
let ``Incremental declaration checking matches whole-file CheckFile signature`` () =
    let file = testPath "Parity.fs"
    let ctx = DeclarationChecker.CreateProject(mkOptions Array.empty [| file |]) |> Async.RunSynchronously

    let source =
        """module Sample.Parity

type Counter = { Value: int }

let increment (c: Counter) = { Value = c.Value + 1 }

let answer = (increment { Value = 41 }).Value
"""

    let parse = DeclarationChecker.Parse(ctx, file, source)
    let start, env0 = DeclarationChecker.BeginFile(ctx, [], parse)
    let decls = DeclarationChecker.Split parse

    let results, _ =
        ((ResizeArray(), env0), decls)
        ||> Array.fold (fun (acc, env) decl ->
            let r = DeclarationChecker.CheckDeclaration(start, env, decl, CancellationToken.None)
            acc.Add r
            acc, DeclarationChecker.ExtendEnv(env, r))

    let incFile = DeclarationChecker.FinishFile(start, results.ToArray(), parse)
    let wholeFile = DeclarationChecker.CheckFile(ctx, [], parse)

    Assert.Empty(incFile.Diagnostics)
    Assert.Empty(wholeFile.Diagnostics)
    Assert.Equal(wholeFile.Signature.Fingerprint, incFile.Signature.Fingerprint)

[<Fact>]
let ``Declaration fingerprint cuts off body edits and detects signature changes`` () =
    let file = testPath "FingerprintCutoff.fs"

    let checkSingleBody source =
        let ctx = DeclarationChecker.CreateProject(mkOptions Array.empty [| file |]) |> Async.RunSynchronously
        let parse = DeclarationChecker.Parse(ctx, file, source)
        let start, env0 = DeclarationChecker.BeginFile(ctx, [], parse)
        let decl = (DeclarationChecker.Split parse)[0]
        DeclarationChecker.CheckDeclaration(start, env0, decl, CancellationToken.None)

    let r1 = checkSingleBody "module Sample.Cutoff\nlet compute x = x + 1\n"
    let r2 = checkSingleBody "module Sample.Cutoff\nlet compute x = x + 2\n"
    let r3 = checkSingleBody "module Sample.Cutoff\nlet compute (x: float) = x + 2.0\n"

    Assert.Equal(r1.Fingerprint, r2.Fingerprint)
    Assert.NotEqual<string>(r1.Fingerprint, r3.Fingerprint)

[<Fact>]
let ``Shift moves checked declaration ranges without rechecking`` () =
    let file = testPath "ShiftTest.fs"
    let ctx = DeclarationChecker.CreateProject(mkOptions Array.empty [| file |]) |> Async.RunSynchronously

    let v1 =
        """module Sample.Shift

let first = 10

let second x = x + first
"""

    let p1 = DeclarationChecker.Parse(ctx, file, v1)
    let start, env0 = DeclarationChecker.BeginFile(ctx, [], p1)
    let d1 = DeclarationChecker.Split p1
    let rFirst = DeclarationChecker.CheckDeclaration(start, env0, d1[0], CancellationToken.None)
    let env1 = DeclarationChecker.ExtendEnv(env0, rFirst)
    let rSecond = DeclarationChecker.CheckDeclaration(start, env1, d1[1], CancellationToken.None)

    let v2 =
        """module Sample.Shift

let first = 10


let second x = x + first
"""

    let p2 = DeclarationChecker.Parse(ctx, file, v2)
    let d2 = DeclarationChecker.Split p2

    match DeclarationChecker.Shift(rSecond, d2[1]) with
    | ValueSome shifted ->
        Assert.Equal(rSecond.Declaration.Range.StartLine + 1, shifted.Declaration.Range.StartLine)
        Assert.True(shifted.IsSameDelta rSecond)
    | ValueNone -> failwith "Expected Shift to succeed for an unchanged line-shifted declaration."

[<Fact>]
let ``ProjectAssembly feeds referencing project in memory without disk output`` () =
    let fileA = testPath "LibA.fs"
    let optionsA = mkOptions Array.empty [| fileA |]

    let outDllA =
        optionsA.OtherOptions
        |> Array.pick (fun arg ->
            if arg.StartsWith("--out:", StringComparison.OrdinalIgnoreCase) then
                Some(arg.Substring("--out:".Length))
            else
                None)

    let ctxA = DeclarationChecker.CreateProject optionsA |> Async.RunSynchronously
    let pA = DeclarationChecker.Parse(ctxA, fileA, "module Sample.LibA\nlet baseValue = 21\n")
    let resA = DeclarationChecker.CheckFile(ctxA, [], pA)
    Assert.Empty(resA.Diagnostics)

    let asmA = DeclarationChecker.ProjectAssembly(ctxA, [ resA ], [ resA.Signature ])
    Assert.False(asmA.ReadsFileOnDisk)
    Assert.False(File.Exists outDllA)

    let fileB = testPath "AppB.fs"
    let optionsB = mkOptions [| "-r:" + outDllA |] [| fileB |]
    let refs = readOnlyDict [ outDllA, asmA ]
    let ctxB = DeclarationChecker.CreateProject(optionsB, refs) |> Async.RunSynchronously
    let pB = DeclarationChecker.Parse(ctxB, fileB, "module Sample.AppB\nopen Sample.LibA\nlet answer = baseValue * 2\n")
    let resB = DeclarationChecker.CheckFile(ctxB, [], pB)

    Assert.Empty(resB.Diagnostics)
    Assert.False(File.Exists outDllA)

[<Fact>]
let ``AnsweringDeclarations and ViewResults support queries over checked declarations`` () =
    let file = testPath "ViewResultsTest.fs"
    let ctx = DeclarationChecker.CreateProject(mkOptions Array.empty [| file |]) |> Async.RunSynchronously

    let source =
        """module Sample.Queries

let greet name = "Hello, " + name
"""

    let parse = DeclarationChecker.Parse(ctx, file, source)
    let start, env0 = DeclarationChecker.BeginFile(ctx, [], parse)
    let decls = DeclarationChecker.Split parse
    let res0 = DeclarationChecker.CheckDeclaration(start, env0, decls[0], CancellationToken.None)
    let results = [| res0 |]

    let struct (rows, _exit, isEmpty) = DeclarationChecker.AnsweringDeclarations(start, results, parse, 3, 29)
    let view = DeclarationChecker.ViewResults(start, results, parse)

    Assert.False(isEmpty)
    Assert.Contains(0, rows)
    Assert.True(view.HasFullTypeCheckInfo)
