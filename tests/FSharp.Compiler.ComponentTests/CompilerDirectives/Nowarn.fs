// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler
open FSharp.Test
open System
open System.Text

module Nowarn =

    let private intro = "module A"
    let private nowarn n = $"#nowarn {n}"
    let private warnon n = $"#warnon {n}"
    let private line1 = """#line 1 "some.fsy" """
    let private line10 = """#line 10 "some.fsy" """
    let private make20 = "1"
    let private make25 = "match None with None -> ()"
    let private subIntro = ["namespace A"; "module B ="]
    let private vp = "PREVIEW"
    let private v9 = "9.0"
    let private fs = String.concat Environment.NewLine >> FsSource
    let private fsSubModule lines = subIntro @ (lines |> List.map (fun s -> "  " + s)) |> fs
    let private fsi = String.concat System.Environment.NewLine >> FsiSource
    let private fsx = String.concat System.Environment.NewLine >> FsxSourceCode
    
    let private fsiSource44 = [
        "namespace A"
        "[<System.Obsolete>]"
        "type T = class end"
        "type T2 = T"
        "#nowarn 44"
        "type T3 = T"
        "#warnon 44"
        "type T4 = T"
        "#nowarn 44"
        "type T5 = T"
    ]
    
    let private fsSource44 = [
        "namespace A"
        "#nowarn 44"
        "[<System.Obsolete>]"
        "type T = class end"
        "type T2 = T"
        "type T3 = T"
        "type T4 = T"
        "type T5 = T"
    ]
    
    let private testData = [
        vp, [], [fs [intro; make20]], [Warning 20, 2]
        vp, [], [fs [intro; nowarn 20; make20]], []
        vp, [], [fs [intro; "#nowarn 20;;"; make20]], []
        vp, [], [fs [intro; make20; nowarn 20; make20; warnon 20; make20]], [Warning 20, 2; Warning 20, 6]
        v9, [], [fs [intro; make20; nowarn 20; make20; warnon 20; make20]], [Error 3350, 5]
        vp, [], [fs [intro; nowarn 20; line1; make20]], []
        v9, [], [fs [intro; nowarn 20; line1; make20]], []    // warning in real v9
        vp, [], [fs [intro; nowarn 20; line10; make20]], []
        v9, [], [fs [intro; nowarn 20; line10; make20]], []
        vp, [], [fs [intro; nowarn 20; line1; make20; warnon 20; make20]], []  // this will change if we go for the proposed RFC
        v9, [], [fs [intro; nowarn 20; line1; make20; warnon 20; make20]], [Error 3350, 2]
        vp, ["--nowarn:20"], [fs [intro; make20]], []
        v9, ["--nowarn:20"], [fs [intro; make20]], []
        vp, ["--nowarn:20"], [fs [intro; warnon 20; make20]], [Warning 20, 3]
        v9, ["--nowarn:20"], [fs [intro; warnon 20; make20]], [Error 3350, 2]
        vp, ["--warnon:3579"], [fs [intro; """ignore $"{1}" """]], [Warning 3579, 2]
        v9, ["--warnon:3579"], [fs [intro; """ignore $"{1}" """]], [Warning 3579, 2]
        vp, [], [fs [intro; "#warnon 3579"; """ignore $"{1}" """]], [Warning 3579, 3]
        v9, [], [fs [intro; "#warnon 3579"; """ignore $"{1}" """]], [Error 3350, 2]
        vp, ["--warnaserror"], [fs [intro; make20]], [Error 20, 2]
        vp, ["--warnaserror"; "--nowarn:20"], [fs [intro; make20]], []
        vp, ["--warnaserror"; "--nowarn:20"], [fs [intro; warnon 20; make20]], [Error 20, 3]
        v9, ["--warnaserror"; "--nowarn:20"], [fs [intro; warnon 20; make20]], [Error 3350, 2]
        vp, ["--warnaserror"], [fs [intro; nowarn 20; make20]], []
        vp, ["--warnaserror"; "--warnaserror-:20"], [fs [intro; make20]], [Warning 20, 2]
        vp, ["--warnaserror:20"], [fs [intro; nowarn 20; make20]], []
        vp, [], [fsSubModule [nowarn 20; make20]], []
        v9, [], [fsSubModule [nowarn 20; make20]], [Warning 236, 3]
        vp, [], [fsSubModule [make20; nowarn 20; make20; warnon 20; make20]], [Warning 20, 3; Warning 20, 7]
        v9, [], [fsSubModule [make20; nowarn 20; make20; warnon 20; make20]], [Error 3350, 6]
        vp, [], [fsi fsiSource44; fs fsSource44], [Warning 44, 4; Warning 44, 8]
        v9, [], [fsi fsiSource44; fs fsSource44], [Error 3350, 7]
        vp, [], [fsx [intro; make20; nowarn 20; make20; warnon 20; make20]], []  // 20 is not checked in scripts
        vp, [], [fsx [intro; make25; nowarn 25; make25; warnon 25; make25]], [Warning 25, 2; Warning 25, 6]
        v9, [], [fsx [intro; make25; nowarn 25; make25; warnon 25; make25]], [Error 3350, 5]
        vp, [], [fs [intro; "let x ="; nowarn 20; "    1"; warnon 20; "    2"; "    3"; "4"]], [Warning 20, 6; Warning 20, 8]
        vp, [], [fs [intro; nowarn 20; nowarn 20; warnon 20; make20]], [Warning 3875, 3; Warning 20, 5]
        v9, [], [fs [intro; nowarn 20; nowarn 20; warnon 20; make20]], [Error 3350, 4]
        vp, [], [fs [intro; "#nowarn \"\"\"20\"\"\" "; make20]], []
        ]

    let testMemberData =
        testData |> List.mapi (fun i (v, fl, sources, diags) -> [|
            box (i + 1)
            box v
            box fl
            box (List.toArray sources)
            box (List.toArray diags) |])

    let private testFailed (expected: (ErrorType * int) list) (actual: ErrorInfo list) =
        expected.Length <> actual.Length
        || (List.zip expected actual |> List.exists(fun((error, line), d) -> error <> d.Error || line <> d.Range.StartLine))

    let withDiags testId langVersion flags (sources: SourceCodeFileKind list) (expected: (ErrorType * int) list) (result: CompilationResult) =
        let actual = result.Output.Diagnostics
        if testFailed expected actual then
            let sb = new StringBuilder()
            let print (s: string) = sb.AppendLine s |> ignore
            print ""
            print $"test {testId} of {testData.Length}"
            print "  language version:"
            print $"    {langVersion}"
            print "  added compiler options:"
            for flag in flags do print $"    {flag}"
            for source in sources do
                print $"  source code %s{source.GetSourceFileName}:"
                let text = source.GetSourceText |> Option.defaultValue ""
                let lines = text.Split(Environment.NewLine) |> Array.toList
                for line in lines do print $"    {line}"
            print $"  expected diagnostics:"
            for (error, line) in expected do print $"    {error} in line {line}"
            print $"  actual diagnostics:"
            for d in actual do print $"    {d.Error} in line {d.Range.StartLine}"
            Assert.Fail(string sb)
            
    [<MemberData(nameof testMemberData)>]
    [<Theory>]
    let testWarnScopes testId langVersion flags (sourceArray: SourceCodeFileKind array) expectedDiags =
        let sources = Array.toList sourceArray
        sources.Head
        |> fsFromString
        |> FS
        |> withAdditionalSourceFiles sources.Tail
        |> withLangVersion langVersion
        |> withOptions flags
        |> compile
        |> withDiags testId langVersion flags sources (Array.toList expectedDiags)
