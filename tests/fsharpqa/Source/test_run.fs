namespace FSharpQA.Tests

open System
open System.IO
open NUnit.Framework

module ParseXmlConfig =

    let permutations () =
        [ TestCaseData(@"<Expects status=""success"">val e : seq<int></Expects>", "val e : seq<int>")
          TestCaseData(@"<Expect status=""warning"" span=""(8,6-8,7)"" id=""FS0086"">The '<' operator should not normally be redefined\. To</Expect>", "The '<' operator should not normally be redefined\. To")
          TestCaseData(@"<CmdLine>Missing 'do' in 'while' expression\. Expected 'while <expr> do <expr>'\.$</CmdLine>", "Missing 'do' in 'while' expression\. Expected 'while <expr> do <expr>'\.$") ]

    [<Test; TestCaseSource("permutations")>]
    let ``parse should fix malformed xml`` line message =
        match line |> RunPl.parseMalformedXml with
        | Choice1Of2 xml -> Assert.AreEqual(message, xml.Value)
        | Choice2Of2 exn -> Assert.Fail(sprintf "expected valid xml, but got error '%s'" exn.Message)
