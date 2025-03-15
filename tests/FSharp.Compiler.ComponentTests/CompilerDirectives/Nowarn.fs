// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler
open FSharp.Test
open System
open System.Text

module Nowarn =

    let private nowarn n = $"#nowarn {n}"
    let private warnon n = $"#warnon {n}"
    let private line1 = """#line 1 "some.fsy" """
    let private line10 = """#line 10 "some.fsy" """
    let private make20 = "1"
    let private make25 = "match None with None -> ()"
    let private W20 = Warning 20
    let private vp = "PREVIEW"
    let private v9 = "9.0"
    let private fs = String.concat Environment.NewLine >> FsSource
    let private fsMod lines = fs ("module A" :: lines)
    let private fsSub lines = fs ("namespace A" :: "module B =" :: (lines |> List.map (fun s -> "  " + s)))
    let private fsi = String.concat Environment.NewLine >> FsiSource
    let private fsx = String.concat Environment.NewLine >> FsxSourceCode
    
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
    
    let private testData =
        [
        vp, [], [fsMod [make20]], [W20, 2]
        vp, [], [fsMod [nowarn 20; make20]], []
        vp, [], [fsMod ["#nowarn 20;;"; make20]], []
        vp, [], [fsMod [make20; nowarn 20; make20; warnon 20; make20]], [W20, 2; W20, 6]
        v9, [], [fsMod [make20; nowarn 20; make20; warnon 20; make20]], [Error 3350, 5]
        vp, [], [fsMod [nowarn 20; line1; make20]], []
        v9, [], [fsMod [nowarn 20; line1; make20]], []     // real v9 shows a warning here
        vp, [], [fsMod [nowarn 20; line10; make20]], []
        v9, [], [fsMod [nowarn 20; line10; make20]], []
        vp, [], [fsMod [nowarn 20; line1; make20; warnon 20; make20]], [W20, 3]
        v9, [], [fsMod [nowarn 20; line1; make20; warnon 20; make20]], [Error 3350, 2]
        vp, ["--nowarn:20"], [fsMod [make20]], []
        v9, ["--nowarn:20"], [fsMod [make20]], []
        vp, ["--nowarn:20"], [fsMod [warnon 20; make20]], [W20, 3]
        v9, ["--nowarn:20"], [fsMod [warnon 20; make20]], [Error 3350, 2]
        vp, ["--warnon:3579"], [fsMod ["""ignore $"{1}" """]], [Warning 3579, 2]
        v9, ["--warnon:3579"], [fsMod ["""ignore $"{1}" """]], [Warning 3579, 2]
        vp, [], [fsMod ["#warnon 3579"; """ignore $"{1}" """]], [Warning 3579, 3]
        v9, [], [fsMod ["#warnon 3579"; """ignore $"{1}" """]], [Error 3350, 2]
        vp, ["--warnaserror"], [fsMod [make20]], [Error 20, 2]
        vp, ["--warnaserror"; "--nowarn:20"], [fsMod [make20]], []
        vp, ["--warnaserror"; "--nowarn:20"], [fsMod [warnon 20; make20]], [Error 20, 3]
        v9, ["--warnaserror"; "--nowarn:20"], [fsMod [warnon 20; make20]], [Error 3350, 2]
        vp, ["--warnaserror"], [fsMod [nowarn 20; make20]], []
        vp, ["--warnaserror"; "--warnaserror-:20"], [fsMod [make20]], [W20, 2]
        vp, ["--warnaserror:20"], [fsMod [nowarn 20; make20]], []
        vp, [], [fsSub [nowarn 20; make20]], []
        v9, [], [fsSub [nowarn 20; make20]], [Warning 236, 3; W20, 4]
        vp, [], [fsSub [make20; nowarn 20; make20; warnon 20; make20]], [W20, 3; W20, 7]
        v9, [], [fsSub [make20; nowarn 20; make20; warnon 20; make20]], [Warning 236, 4; Warning 236, 6; W20, 3; W20, 5; W20, 7]
        vp, [], [fsi fsiSource44; fs fsSource44], [Warning 44, 4; Warning 44, 8]
        v9, [], [fsi fsiSource44; fs fsSource44], [Error 3350, 7]
        vp, [], [fsx ["module A"; make20; nowarn 20; make20; warnon 20; make20]], []  // 20 is not checked in scripts
        vp, [], [fsx ["module A"; make25; nowarn 25; make25; warnon 25; make25]], [Warning 25, 2; Warning 25, 6]
        v9, [], [fsx ["module A"; make25; nowarn 25; make25; warnon 25; make25]], [Error 3350, 5]
        v9, [], [fsx ["module A"; make25; nowarn 25; make25]], []
        vp, [], [fsMod ["let x ="; nowarn 20; "    1"; warnon 20; "    2"; "    3"; "4"]], [W20, 6; W20, 8]
        vp, [], [fsMod [nowarn 20; nowarn 20; warnon 20; make20]], [Information 3876, 3; W20, 5]
        vp, [], [fsMod [nowarn 20; warnon 20; warnon 20; make20]], [Warning 3876, 4; W20, 5]
        vp, ["--warnon:3876"], [fsMod [nowarn 20; nowarn 20; warnon 20; make20]], [Warning 3876, 3; W20, 5]
        vp, [], [fsMod ["#nowarn \"\"\"20\"\"\" "; make20]], []
        vp, [], [fsMod ["#nowarnx 20"; make20]], [Error 3353, 2]
        vp, [], [fsMod ["#nowarn 20 // comment"; make20]], []
        vp, [], [fsMod ["#nowarn"; make20]], [Error 3875, 2]
        vp, [], [fsMod ["let a = 1; #nowarn 20"; make20]], [Error 3874, 2]
        ]
        |> List.mapi (fun i (v, fl, sources, diags) -> [|
            box (i + 1)
            box v
            box fl
            box (List.toArray sources)
            box (List.toArray diags) |])

    let testMemberData =
        match System.Int32.TryParse(System.Environment.GetEnvironmentVariable("NowarnSingleTest")) with
        | true, n when n > 0 && n <= testData.Length -> [testData[n-1]]
        | _ -> testData

    let private testFailed (expected: (ErrorType * int) list) (actual: ErrorInfo list) =
        expected.Length <> actual.Length
        || (List.zip expected actual |> List.exists(fun((error, line), d) -> error <> d.Error || line <> d.Range.StartLine))

    let private withDiags testId langVersion flags (sources: SourceCodeFileKind list) (expected: (ErrorType * int) list) (result: CompilationResult) =
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
                let lines = text.Split(Environment.NewLine |> Seq.toArray) |> Array.toList
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

    [<Fact>]
    let testBadLineDirectiveInteraction() =
        let sources =
            [
            "test1.fs", "module A1 \n#line 10 \"test.fsy\" \n()"
            "test2.fs", "module A2 \n#line 20 \"test.fsy\" \n()"
            ]
            |> List.map (fun (name, text) -> {FileName = name; SourceText = Some text})
            |> List.map SourceCodeFileKind.Fs
        let result =
            sources.Head
            |> fsFromString
            |> FS
            |> withAdditionalSourceFiles sources.Tail
            |> compile
        let actual = result.Output.Diagnostics
        if actual.Length <> 1 then Assert.Fail $"expected 1 warning, got {actual.Length}"
        let errorInfo = actual.Head
        if errorInfo.Error <> Warning 3877 then Assert.Fail $"expected Warning 3877, got {errorInfo.Error}"
        if errorInfo.Range.StartLine <> 3 then Assert.Fail $"expected warning in line 3, got line {errorInfo.Range.StartLine}"
        if not <| errorInfo.Message.StartsWith "The file 'test.fsy' was also pointed to" then Assert.Fail $"unexpected message {errorInfo.Message}"
 
    [<Fact>]
    let warnDirectiveArgRange() =
        FSharp """
module A
#nowarn xy "abx"
let a = 1; #nowarn 20
"""
        |> compile
        |> withDiagnostics [
            Error 3874, Line 4, Col 11, Line 4, Col 22, "#nowarn/#warnon directives must appear as the first non-whitespace characters on a line"
            Warning 203, Line 3, Col 9, Line 3, Col 11, "Invalid warning number 'xy'"
            Warning 203, Line 3, Col 12, Line 3, Col 17, "Invalid warning number 'abx'"
        ]
