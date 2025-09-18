namespace CompilerDirectives

open Microsoft.FSharp.Control
open Xunit
open Internal.Utilities
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.UnicodeLexing
open FSharp.Test.Compiler
open System.IO

module Line =

    let parse (source: string) sourceFileName =
        let checker = FSharpChecker.Create()
        let langVersion = "preview"
        let parsingOptions =
            { FSharpParsingOptions.Default with
                SourceFiles = [| sourceFileName |]
                LangVersionText = langVersion
                ApplyLineDirectives = true
                }
        checker.ParseFile(sourceFileName, SourceText.ofString source, parsingOptions) |> Async.RunSynchronously


    [<Literal>]
    let private case1 = """module A
#line 1 "xyz1.fs"
(
printfn ""
)
    """
    
    [<Literal>]
    let private case2 = """module A
(
#line 1 "xyz2.fs"
printfn ""
)
    """
    
    [<Literal>]
    let private case3 = """module A
(
#line 1 "xyz3.fs"
)
    """

    [<Theory>]
    [<InlineData(1, case1, "xyz1.fs:(1,0--3,1)")>]
    [<InlineData(2, case2, "Line2.fs:(2,0--5,1)")>]
    [<InlineData(3, case3, "Line3.fs:(2,0--4,1)")>]
    let ``check expr range interacting with line directive`` (case, source, expectedRange) =
        let parseResults = parse source $"Line{case}.fs"
        if parseResults.ParseHadErrors then failwith "unexpected: parse error"
        let exprRange =
            match parseResults.ParseTree with
            | ParsedInput.ImplFile(ParsedImplFileInput(contents = contents)) ->
                let (SynModuleOrNamespace(decls = decls)) = List.exactlyOne contents
                match List.exactlyOne decls with
                | SynModuleDecl.Expr(_, range) ->
                    let range = range.ApplyLineDirectives()
                    $"{range.FileName}:{range}"
                | _ -> failwith $"unexpected: not an expr"
            | ParsedInput.SigFile _ -> failwith "unexpected: sig file"
        if exprRange <> expectedRange then
            failwith $"case{case}: expected: {expectedRange}, found {exprRange}"


    [<Fact>]
    let TestLineDirectives () =
        let source =
            """
    module A
    3
    4
    #line 5 "test.fsl"
    6
    7
    #line 10 "test.fsy"
    9
    10
    """
        let errors =
            source
            |> FSharp
            |> withFileName "test.fs"
            |> compile
            |> shouldFail
            |> fun cr -> cr.Output.PerFileErrors |> List.map (fun (fn, d) -> fn, d.Range.StartLine)
        let expectedErrors =
            [("test.fs", 3); ("test.fs", 4); ("test.fsl", 5); ("test.fsl", 6); ("test.fsy", 10); ("test.fsy", 11)]
        Assert.True(
            List.forall2 (fun (e_fn, e_line) (fn, line) -> e_fn = fn && e_line = line) expectedErrors errors,
            sprintf "Expected: %A, Found: %A" expectedErrors errors
        )

    [<Fact>]
    let TestNoLineDirectives () =
        let source =
            """
    module A
    3
    4
    """
        let errors =
            source
            |> FSharp
            |> withFileName "testn.fs"
            |> compile
            |> shouldFail
            |> fun cr -> cr.Output.PerFileErrors |> List.map (fun (fn, d) -> fn, d.Range.StartLine)
        let expectedErrors =
            [("testn.fs", 3); ("testn.fs", 4)]
        Assert.True(
            List.forall2 (fun (e_fn, e_line) (fn, line) -> e_fn = fn && e_line = line) expectedErrors errors,
            sprintf "Expected: %A, Found: %A" expectedErrors errors
        )



    let private getTokens sourceText =
        let langVersion = LanguageVersion.Default
        let lexargs =
            mkLexargs (
                [],
                IndentationAwareSyntaxStatus(true, false),
                LexResourceManager(),
                [],
                DiscardErrorsLogger,
                PathMap.empty,
                true
            )
        let lexbuf = StringAsLexbuf(true, langVersion, None, sourceText)
        resetLexbufPos "testt.fs" lexbuf
        let tokenizer _ =
            let t = Lexer.token lexargs true lexbuf
            let p = lexbuf.StartPos
            t, FileIndex.fileOfFileIndex p.FileIndex, p.Line
        let isNotEof(t,_,_) = match t with Parser.EOF _ -> false | _ -> true
        Seq.initInfinite tokenizer |> Seq.takeWhile isNotEof |> Seq.toList

    let private code = """
1
#line 5 "other.fs"
2
#line 10 "testt.fs"
3
"""

    let private expected = [
        "testt.fs", 2
        "testt.fs", 4
        "testt.fs", 6
    ]

    [<Fact>]
    let checkOriginalLineNumbers() =
        let tokens = getTokens code
        Assert.Equal(expected.Length, tokens.Length)
        for ((e_idx, e_line), (_, idx, line)) in List.zip expected tokens do
            Assert.Equal(e_idx, idx)
            Assert.Equal(e_line, line)

    let callerInfoSource = """
        open System.Runtime.CompilerServices
        open System.Runtime.InteropServices

        type C() =
            static member M (
                [<CallerLineNumber; Optional; DefaultParameterValue 0>]c: int, 
                [<CallerFilePath; Optional; DefaultParameterValue "no value">]d: string) = 
                c, d

        #line 1 "file1.fs"
        let line1, file1 = C.M()
        #line 551 "file2.fs"
        let line2, file2 = C.M()
        
        printfn $"{file1} {line1} {file2} {line2}"
"""

    [<Fact>]
    let ``CallerLineNumber and CallerFilePath work with line directives`` () =
        let results =
            callerInfoSource
            |> FSharp
            |> withFileName "CallerInfo.fs"
            |> withLangVersion "preview"
            |> compileExeAndRun
        match results.RunOutput with
        | Some (ExecutionOutput output) ->
            let words = output.StdOut.Trim().Split(' ')
            let file1 = Path.GetFileName(words.[0])
            let line1 = int words.[1]
            let file2 = Path.GetFileName(words.[2])
            let line2 = int words.[3]
            Assert.Equal("file1.fs", file1)
            Assert.Equal(1, line1)
            Assert.Equal("file2.fs", file2)
            Assert.Equal(551, line2)
        | _ -> failwith "Unexpected: no run output"



    let csharpLibSource = """
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CSharpLib
{
    public class CallerInfoTest
    {
        public static int LineNumber([CallerLineNumber] int line = 777)
        {
            return line;
        }
        
        public static string FilePath([CallerFilePath] string filePath = "dummy1")
        {
            return filePath;
        }
		
		public static string MemberName([CallerMemberName] string memberName = "dummy1")
        {
            return memberName;
        }
        
        public static Tuple<string, int, string> AllInfo(int normalArg, [CallerFilePath] string filePath = "dummy2", [CallerLineNumber] int line = 778, [CallerMemberName] string memberName = "dummy3")
        {
            return new Tuple<string, int, string>(filePath, line, memberName);
        }
    }

    public class MyCallerInfoAttribute : Attribute
    {
        public int LineNumber { get; set; }
        
        public MyCallerInfoAttribute([CallerLineNumber] int lineNumber = -1)
        {
            LineNumber = lineNumber;
        }
    }

    public class MyCallerMemberNameAttribute : Attribute
    {
        public string MemberName { get; set; }

        public MyCallerMemberNameAttribute([CallerMemberName] string member = "dflt")
        {
            MemberName = member;
        }
    }
}
"""

    let fsharpSource = """

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
                printfn "starting main"
                let o = MyTy()
                let o1 = MyTy("42")

                match o.Path with
                | Some(path) when matchesPath "CallerInfo.fs" path -> ()
                | Some(path) -> failwithf "Unexpected (1): %s" path
                | None -> failwith "Unexpected (1): None"

                match o1.Path with
                | Some(path) when matchesPath "42" path -> ()
                | Some(path) -> failwithf "Unexpected (2): %s" path
                | None -> failwith "Unexpected (2): None"

                match MyTy.GetCallerFilePath() with
                | Some(path) when matchesPath "CallerInfo.fs" path -> ()
                | Some(path) -> failwithf "Unexpected (3): %s" path
                | None -> failwith "Unexpected (3): None"
                
                match MyTy.GetCallerFilePath("42") with
                | Some("42") -> ()
                | Some(path) -> failwithf "Unexpected (4): %s" path
                | None -> failwith "Unexpected (4): None"
                
                match CallerInfoTest.FilePath() with
                | path when matchesPath "CallerInfo.fs" path -> ()
                | path -> failwithf "Unexpected (5): %s" path
                
                match CallerInfoTest.FilePath("xyz") with
                | "xyz" -> ()
                | path -> failwithf "Unexpected (6): %s" path
                
                match CallerInfoTest.AllInfo(21) with
                | (path, _, _) when matchesPath "CallerInfo.fs" path -> ()
                | (path, _, _) -> failwithf "Unexpected (7): %s" path

        # 345 "qwerty.fsy"
                match CallerInfoTest.AllInfo(123) with
                | (path, _, _) when matchesPath "qwerty.fsy" path -> ()
                | (path, _, _) -> failwithf "Unexpected (8): %s" path

        # 456 "qwerty.fsl"
                match CallerInfoTest.AllInfo(123) with
                | (path, _, _) when matchesPath "qwerty.fsl" path -> ()
                | (path, _, _) -> failwithf "Unexpected (9): %s" path

                0
        """

    [<Fact>]
    let ``C# CallerLineNumber and CallerFilePath work with line directives`` () =
        let csharp =
            csharpLibSource
            |> CSharp
            |> withFileName "CallerInfoLib.cs"
        
        fsharpSource
        |> FSharp
        |> withFileName "CallerInfo.fs"
        |> withLangVersion "preview"
        |> withReferences [csharp]
        |> compileExeAndRun
        |> shouldSucceed
    
