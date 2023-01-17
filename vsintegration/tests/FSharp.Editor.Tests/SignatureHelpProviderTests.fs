// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.IO
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open Microsoft.CodeAnalysis.Text
open FSharp.Editor.Tests.Helpers

module SignatureHelpProvider =
    let private DefaultDocumentationProvider =
        { new IDocumentationBuilder with
            override doc.AppendDocumentationFromProcessedXML(_, _, _, _, _, _) = ()
            override doc.AppendDocumentation(_, _, _, _, _, _, _) = ()
        }

    let checker = FSharpChecker.Create()

    let filePath = "C:\\test.fs"

    let PathRelativeToTestAssembly p =
        Path.Combine(Path.GetDirectoryName(Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), p)

    let internal projectOptions =
        {
            ProjectFileName = "C:\\test.fsproj"
            ProjectId = None
            SourceFiles = [| filePath |]
            ReferencedProjects = [||]
            OtherOptions =
                [|
                    "-r:"
                    + PathRelativeToTestAssembly(@"DummyProviderForLanguageServiceTesting.dll")
                |]
            IsIncompleteTypeCheckEnvironment = true
            UseScriptResolutionRules = false
            LoadTime = DateTime.MaxValue
            OriginalLoadReferences = []
            UnresolvedReferences = None
            Stamp = None
        }

    let internal parsingOptions =
        { FSharpParsingOptions.Default with
            SourceFiles = [| filePath |]
        }

    let GetSignatureHelp (project: FSharpProject) (fileName: string) (caretPosition: int) =
        async {
            let triggerChar = None
            let fileContents = File.ReadAllText(fileName)
            let sourceText = SourceText.From(fileContents)
            let textLines = sourceText.Lines
            let caretLinePos = textLines.GetLinePosition(caretPosition)
            let caretLineColumn = caretLinePos.Character

            let document =
                RoslynTestHelpers.CreateSingleDocumentSolution(fileName, fileContents)

            let parseResults, checkFileResults =
                document.GetFSharpParseAndCheckResultsAsync("GetSignatureHelp")
                |> Async.RunSynchronously

            let paramInfoLocations =
                parseResults
                    .FindParameterLocations(
                        Position.fromZ caretLinePos.Line caretLineColumn
                    )
                    .Value

            let triggered =
                FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(
                    caretLinePos,
                    caretLineColumn,
                    paramInfoLocations,
                    checkFileResults,
                    DefaultDocumentationProvider,
                    sourceText,
                    caretPosition,
                    triggerChar
                )
                |> Async.RunSynchronously

            return triggered
        }
        |> Async.RunSynchronously

    let GetCompletionTypeNames (project: FSharpProject) (fileName: string) (caretPosition: int) =
        let sigHelp = GetSignatureHelp project fileName caretPosition

        match sigHelp with
        | None -> [||]
        | Some data ->
            let completionTypeNames =
                data.SignatureHelpItems
                |> Array.map (fun r -> r.Parameters |> Array.map (fun p -> p.CanonicalTypeTextForSorting))

            completionTypeNames

    let GetCompletionTypeNamesFromXmlString (xml: string) =
        use project = CreateProject xml
        let fileName, caretPosition = project.GetCaretPosition()
        let completionNames = GetCompletionTypeNames project fileName caretPosition
        completionNames

    let assertSignatureHelpForMethodCalls (fileContents: string) (marker: string) (expected: (string * int * int * string option) option) =
        let caretPosition = fileContents.IndexOf(marker) + marker.Length

        let triggerChar =
            if marker = "," then Some ','
            elif marker = "(" then Some '('
            elif marker = "<" then Some '<'
            else None

        let sourceText = SourceText.From(fileContents)
        let textLines = sourceText.Lines
        let caretLinePos = textLines.GetLinePosition(caretPosition)
        let caretLineColumn = caretLinePos.Character

        let document =
            RoslynTestHelpers.CreateSingleDocumentSolution(filePath, fileContents)

        let parseResults, checkFileResults =
            document.GetFSharpParseAndCheckResultsAsync("assertSignatureHelpForMethodCalls")
            |> Async.RunSynchronously

        let actual =
            let paramInfoLocations =
                parseResults.FindParameterLocations(Position.fromZ caretLinePos.Line caretLineColumn)

            match paramInfoLocations with
            | None -> None
            | Some paramInfoLocations ->
                let triggered =
                    FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(
                        caretLinePos,
                        caretLineColumn,
                        paramInfoLocations,
                        checkFileResults,
                        DefaultDocumentationProvider,
                        sourceText,
                        caretPosition,
                        triggerChar
                    )
                    |> Async.RunSynchronously

                checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

                match triggered with
                | None -> None
                | Some data -> Some(data.ApplicableSpan.ToString(), data.ArgumentIndex, data.ArgumentCount, data.ArgumentName)

        Assert.Equal(expected, actual)

    let assertSignatureHelpForFunctionApplication
        (fileContents: string)
        (marker: string)
        expectedArgumentCount
        expectedArgumentIndex
        expectedArgumentName
        =
        let caretPosition = fileContents.LastIndexOf(marker) + marker.Length

        let sourceText = SourceText.From(fileContents)
        let document =
            RoslynTestHelpers.CreateSingleDocumentSolution(filePath, fileContents)

        let parseResults, checkFileResults =
            document.GetFSharpParseAndCheckResultsAsync("assertSignatureHelpForFunctionApplication")
            |> Async.RunSynchronously

        let adjustedColumnInSource =
            let rec loop ch pos =
                if Char.IsWhiteSpace(ch) then
                    loop sourceText.[pos - 1] (pos - 1)
                else
                    pos

            loop sourceText.[caretPosition - 1] (caretPosition - 1)

        let sigHelp =
            FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
                parseResults,
                checkFileResults,
                document.Id,
                [],
                DefaultDocumentationProvider,
                sourceText,
                caretPosition,
                adjustedColumnInSource,
                filePath
            )
            |> Async.RunSynchronously

        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

        match sigHelp with
        | None -> failwith "Expected signature help"
        | Some sigHelp ->
            Assert.Equal(expectedArgumentCount, sigHelp.ArgumentCount)
            Assert.Equal(expectedArgumentIndex, sigHelp.ArgumentIndex)
            Assert.Equal(1, sigHelp.SignatureHelpItems.Length)
            Assert.True(expectedArgumentIndex < sigHelp.SignatureHelpItems[0].Parameters.Length)
            Assert.Equal(expectedArgumentName, sigHelp.SignatureHelpItems[0].Parameters[expectedArgumentIndex].ParameterName)

open SignatureHelpProvider

module ``Gives signature help in method calls`` =

    [<Fact>]
    let ``dot`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = "."
        assertSignatureHelpForMethodCalls fileContents marker None

    [<Fact>]
    let ``System`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = "System"
        assertSignatureHelpForMethodCalls fileContents marker None

    [<Fact>]
    let ``WriteLine`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = "WriteLine"
        assertSignatureHelpForMethodCalls fileContents marker None

    [<Fact>]
    let ``open paren`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = "("
        assertSignatureHelpForMethodCalls fileContents marker (Some("[7..64)", 0, 2, Some "format"))

    [<Fact>]
    let ``named arg`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = "format"
        assertSignatureHelpForMethodCalls fileContents marker (Some("[7..64)", 0, 2, Some "format"))

    [<Fact>]
    let ``comma`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = ","
        assertSignatureHelpForMethodCalls fileContents marker None

    [<Fact>]
    let ``second comma`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        assertSignatureHelpForMethodCalls fileContents """",""" (Some("[7..64)", 1, 2, Some "arg0"))

    [<Fact>]
    let ``second named arg`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = "arg0"
        assertSignatureHelpForMethodCalls fileContents marker (Some("[7..64)", 1, 2, Some "arg0"))

    [<Fact>]
    let ``second named arg equals`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = "arg0="
        assertSignatureHelpForMethodCalls fileContents marker (Some("[7..64)", 1, 2, Some "arg0"))

    [<Fact>]
    let ``World`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
"""

        let marker = "World"
        assertSignatureHelpForMethodCalls fileContents marker (Some("[7..64)", 1, 2, Some "arg0"))

    [<Fact>]
    let ``end paren`` () : unit =
        let fileContents =
            """
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
    """

        let marker = ")"
        assertSignatureHelpForMethodCalls fileContents marker (Some("[7..64)", 0, 2, Some "format"))

module ``Signature help with list literals, parens, etc`` =
    [<Fact>]
    let ``Open paren`` () : unit =
        let fileContents =
            """
//2
open System
Console.WriteLine([(1,2)])
"""

        let marker = "WriteLine("
        assertSignatureHelpForMethodCalls fileContents marker (Some("[20..45)", 0, 0, None))

    [<Fact>]
    let ``comma`` () : unit =
        let fileContents =
            """
//2
open System
Console.WriteLine([(1,2)])
"""

        let marker = ","
        assertSignatureHelpForMethodCalls fileContents marker None

    [<Fact>]
    let ``list and tuple bracket pair start`` () : unit =
        let fileContents =
            """
//2
open System
Console.WriteLine([(1,2)])
"""

        let marker = "[("
        assertSignatureHelpForMethodCalls fileContents marker (Some("[20..45)", 0, 1, None))

module ``Unfinished parentheses`` =
    [<Fact>]
    let ``Unfinished parentheses`` () : unit =
        let fileContents =
            """
let _ = System.DateTime(
"""

        let marker = "let _ = System.DateTime("
        assertSignatureHelpForMethodCalls fileContents marker (Some("[10..26)", 0, 0, None))

    [<Fact>]
    let ``Unfinished parentheses with comma`` () : unit =
        let fileContents =
            """
let _ = System.DateTime(1L,
"""

        let marker = "let _ = System.DateTime(1L,"
        assertSignatureHelpForMethodCalls fileContents marker (Some("[10..31)", 1, 2, None))

#if RELEASE
    [<Fact(Skip="Fails in some CI, reproduces locally in Release mode, needs investigation")>]
#else
    [<Fact>]
#endif
    let ``type provider static parameter tests`` () : unit =
        // This is old code and I'm too lazy to move it all out. - Phillip Carter
        let manyTestCases =
            [
                ("""
//3
type foo = N1.T< 
type foo2 = N1.T<Param1=
type foo3 = N1.T<ParamIgnored=
type foo4 = N1.T<Param1=1,
type foo5 = N1.T<Param1=1,ParamIgnored=
    """,
                 [
                     ("type foo = N1.T<", Some("[18..26)", 0, 0, None))
                     ("type foo2 = N1.T<", Some("[38..52)", 0, 0, Some "Param1"))
                     ("type foo2 = N1.T<Param1", Some("[38..52)", 0, 1, Some "Param1"))
                     ("type foo2 = N1.T<Param1=", Some("[38..52)", 0, 1, Some "Param1"))
                     ("type foo3 = N1.T<", Some("[64..84)", 0, 0, Some "ParamIgnored"))
                     ("type foo3 = N1.T<ParamIgnored=", Some("[64..84)", 0, 1, Some "ParamIgnored"))
                     ("type foo4 = N1.T<Param1", Some("[96..112)", 0, 2, Some "Param1"))
                     ("type foo4 = N1.T<Param1=", Some("[96..112)", 0, 2, Some "Param1"))
                     ("type foo4 = N1.T<Param1=1", Some("[96..112)", 0, 2, Some "Param1"))
                     ("type foo5 = N1.T<Param1", Some("[124..153)", 0, 2, Some "Param1"))
                     ("type foo5 = N1.T<Param1=", Some("[124..153)", 0, 2, Some "Param1"))
                     ("type foo5 = N1.T<Param1=1", Some("[124..153)", 0, 2, Some "Param1"))
                     ("type foo5 = N1.T<Param1=1,", Some("[124..153)", 1, 2, Some "ParamIgnored"))
                     ("type foo5 = N1.T<Param1=1,ParamIgnored", Some("[124..153)", 1, 2, Some "ParamIgnored"))
                     ("type foo5 = N1.T<Param1=1,ParamIgnored=", Some("[124..153)", 1, 2, Some "ParamIgnored"))
                 ])
                ("""
//4
type foo = N1.T< > 
type foo2 = N1.T<Param1= >
type foo3 = N1.T<ParamIgnored= >
type foo4 = N1.T<Param1=1, >
type foo5 = N1.T<Param1=1,ParamIgnored= >
""",
                 [
                     ("type foo = N1.T<", Some("[18..24)", 0, 0, None))
                     ("type foo2 = N1.T<", Some("[40..53)", 0, 0, Some "Param1"))
                     ("type foo2 = N1.T<Param1", Some("[40..53)", 0, 1, Some "Param1"))
                     ("type foo2 = N1.T<Param1=", Some("[40..53)", 0, 1, Some "Param1"))
                     ("type foo3 = N1.T<", Some("[68..87)", 0, 0, Some "ParamIgnored"))
                     ("type foo3 = N1.T<ParamIgnored=", Some("[68..87)", 0, 1, Some "ParamIgnored"))
                     ("type foo4 = N1.T<Param1", Some("[102..117)", 0, 2, Some "Param1"))
                     ("type foo4 = N1.T<Param1=", Some("[102..117)", 0, 2, Some "Param1"))
                     ("type foo4 = N1.T<Param1=1", Some("[102..117)", 0, 2, Some "Param1"))
                     ("type foo5 = N1.T<Param1", Some("[132..160)", 0, 2, Some "Param1"))
                     ("type foo5 = N1.T<Param1=", Some("[132..160)", 0, 2, Some "Param1"))
                     ("type foo5 = N1.T<Param1=1", Some("[132..160)", 0, 2, Some "Param1"))
                     ("type foo5 = N1.T<Param1=1,", Some("[132..160)", 1, 2, Some "ParamIgnored"))
                     ("type foo5 = N1.T<Param1=1,ParamIgnored", Some("[132..160)", 1, 2, Some "ParamIgnored"))
                     ("type foo5 = N1.T<Param1=1,ParamIgnored=", Some("[132..160)", 1, 2, Some "ParamIgnored"))
                 ])
            ]

        for (fileContents, testCases) in manyTestCases do
            for (marker, expected) in testCases do
                assertSignatureHelpForMethodCalls fileContents marker expected

module ``Function argument applications`` =
    let private DefaultDocumentationProvider =
        { new IDocumentationBuilder with
            override doc.AppendDocumentationFromProcessedXML(_, _, _, _, _, _) = ()
            override doc.AppendDocumentation(_, _, _, _, _, _, _) = ()
        }

    [<Fact>]
    let ``single argument function application`` () : unit =
        let fileContents =
            """
sqrt 
    """

        let marker = "sqrt "
        assertSignatureHelpForFunctionApplication fileContents marker 1 0 "value"

    [<Fact>]
    let ``multi-argument function application`` () : unit =
        let fileContents =
            """
let add2 x y = x + y
add2 1 
    """

        let marker = "add2 1 "
        assertSignatureHelpForFunctionApplication fileContents marker 2 1 "y"

    [<Fact>]
    let ``qualified function application`` () : unit =
        let fileContents =
            """
module M =
    let f x = x
M.f 
    """

        let marker = "M.f "
        assertSignatureHelpForFunctionApplication fileContents marker 1 0 "x"

    [<Fact>]
    let ``function application in single pipeline with no additional args`` () : unit =
        let fileContents =
            """
[1..10] |> id 
    """

        let marker = "id "
        let caretPosition = fileContents.IndexOf(marker) + marker.Length

        let sourceText = SourceText.From(fileContents)
        let document =
            RoslynTestHelpers.CreateSingleDocumentSolution(filePath, fileContents)

        let parseResults, checkFileResults =
            document.GetFSharpParseAndCheckResultsAsync("function application in single pipeline with no additional args")
            |> Async.RunSynchronously

        let adjustedColumnInSource =
            let rec loop ch pos =
                if Char.IsWhiteSpace(ch) then
                    loop sourceText.[pos - 1] (pos - 1)
                else
                    pos

            loop sourceText.[caretPosition - 1] (caretPosition - 1)

        let sigHelp =
            FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
                parseResults,
                checkFileResults,
                document.Id,
                [],
                DefaultDocumentationProvider,
                sourceText,
                caretPosition,
                adjustedColumnInSource,
                filePath
            )
            |> Async.RunSynchronously

        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

        Assert.True(sigHelp.IsNone, "No signature help is expected because there are no additional args to apply.")

    [<Fact>]
    let ``function application in single pipeline with an additional argument`` () : unit =
        let fileContents =
            """
[1..10] |> List.map  
    """

        let marker = "List.map "
        assertSignatureHelpForFunctionApplication fileContents marker 1 0 "mapping"

    [<Fact>]
    let ``function application in middle of pipeline with an additional argument`` () : unit =
        let fileContents =
            """
[1..10]
|> List.map 
|> List.filer (fun x -> x > 3)
    """

        let marker = "List.map "
        assertSignatureHelpForFunctionApplication fileContents marker 1 0 "mapping"

    [<Fact>]
    let ``function application with function as parameter`` () : unit =
        let fileContents =
            """
let derp (f: int -> int -> int) x = f x 1
derp 
"""

        let marker = "derp "
        assertSignatureHelpForFunctionApplication fileContents marker 2 0 "f"

    [<Fact>]
    let ``function application with function as second parameter 1`` () : unit =
        let fileContents =
            """
let derp (f: int -> int -> int) x = f x 1
let add x y = x + y
derp add 
"""

        let marker = "derp add "
        assertSignatureHelpForFunctionApplication fileContents marker 2 1 "x"

    [<Fact>]
    let ``function application with function as second parameter 2`` () : unit =
        let fileContents =
            """
let f (derp: int -> int) x = derp x
"""

        let marker = "derp "
        assertSignatureHelpForFunctionApplication fileContents marker 1 0 "arg0"

    [<Fact>]
    let ``function application with curried function as parameter`` () : unit =
        let fileContents =
            """
let f (derp: int -> int -> int) x = derp x 
"""

        let marker = "derp x "
        assertSignatureHelpForFunctionApplication fileContents marker 2 1 "arg1"

    // migrated from legacy test
    [<Fact>]
    let ``Multi.ReferenceToProjectLibrary`` () : unit =
        let completionNames =
            GetCompletionTypeNamesFromXmlString
                @"
    <Projects>

      <Project Name=""TestLibrary.fsproj"">
        <Reference>HelperLibrary.fsproj</Reference>
        <File Name=""test.fs"">
          <![CDATA[
    open Test
    Foo.Sum(12, $$
          ]]>
        </File>
      </Project>

      <Project Name=""HelperLibrary.fsproj"">
        <File Name=""Helper.fs"">
          <![CDATA[
    namespace Test

    type public Foo() =
        static member Sum(x:int, y:int) = x + y
          ]]>
        </File>
      </Project>

    </Projects>
    "

        let expected = [| [| "System.Int32"; "System.Int32" |] |]
        Assert.Equal<string array array>(expected, completionNames)
