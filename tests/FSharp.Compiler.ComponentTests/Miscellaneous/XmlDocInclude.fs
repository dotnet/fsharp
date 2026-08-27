// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Miscellaneous

open System
open System.Collections.Generic
open System.IO
open Xunit
open TestFramework
open FSharp.Test.Compiler
open FSharp.Test.XmlDocIncludeTestFramework

module XmlDocInclude =

    // Test helper: create temp directory with files
    let private setupDir (files: (string * string) list) =
        let dir = (createTemporaryDirectory ()).FullName

        for name, content in files do
            let p = Path.Combine(dir, name)
            Directory.CreateDirectory(Path.GetDirectoryName(p)) |> ignore
            File.WriteAllText(p, content)

        dir

    let private cleanup dir =
        try
            Directory.Delete(dir, true)
        with _ ->
            ()

    let private readEmittedXml (result: CompilationResult) : string =
        match result with
        | CompilationResult.Failure _ -> failwith "Cannot verify XML doc on failed compilation"
        | CompilationResult.Success output ->
            match output.OutputPath with
            | None -> failwith "No output path available"
            | Some dllPath ->
                let dir = Path.GetDirectoryName dllPath
                let byName = Path.Combine(dir, Path.GetFileNameWithoutExtension dllPath + ".xml")
                let fallback = Path.Combine(dir, "output.xml")

                if File.Exists byName then File.ReadAllText byName
                elif File.Exists fallback then File.ReadAllText fallback
                else failwith $"XML doc file not found: tried {byName} and {fallback}"

    let private verifyXmlDocContains (expected: string list) (result: CompilationResult) : CompilationResult =
        let content = readEmittedXml result

        for text in expected do
            if not (content.Contains text) then
                failwith $"XML doc missing: '{text}'\n\nActual:\n{content}"

        result

    let private verifyXmlDocNotContains (unexpected: string list) (result: CompilationResult) : CompilationResult =
        let content = readEmittedXml result

        for text in unexpected do
            if content.Contains text then
                failwith $"XML doc should not contain: '{text}'"

        result

    let private countSubstring (needle: string) (text: string) =
        text.Split([| needle |], StringSplitOptions.None).Length - 1

    let private includeWarnings res =
        res.Compilation.Output.Diagnostics
        |> List.filter (fun diagnostic -> diagnostic.Error = Warning 3908)

    let private includeWarningCount res = includeWarnings res |> List.length

    let private assertSingleIncludeWarningMatches expectedMessage res =
        let warnings = includeWarnings res
        Assert.Equal(1, warnings.Length)
        Assert.Contains(expectedMessage, warnings.Head.Message)

    let private fileSystemSupportsCaseDistinctFiles () =
        let directory = createTemporaryDirectory ()
        let upperPath = Path.Combine(directory.FullName, "Data.xml")
        let lowerPath = Path.Combine(directory.FullName, "data.xml")

        try
            File.WriteAllText(upperPath, "upper")
            File.WriteAllText(lowerPath, "lower")
            File.Exists upperPath
            && File.Exists lowerPath
            && File.ReadAllText upperPath = "upper"
            && File.ReadAllText lowerPath = "lower"
        finally
            Directory.Delete(directory.FullName, true)

    let private makeIncludeChainFiles prefix includeCount =
        [
            for i in 0 .. includeCount - 1 ->
                let content =
                    if i = includeCount - 1 then
                        $"""<?xml version="1.0"?><data><summary>{prefix} leaf.</summary></data>"""
                    else
                        $"""<?xml version="1.0"?><data><summary>{prefix} depth {i}. {Snippets.includeElement $"{prefix}{i + 1}.xml" "/data/summary"}</summary></data>"""

                $"{prefix}{i}.xml", content
        ]

    // Test data
    let private simpleData =
        """<?xml version="1.0"?>
<data>
  <summary>Included summary text.</summary>
</data>"""

    [<Fact>]
    let ``Include with absolute path expands`` () =
        let dir = setupDir [ "data/simple.data.xml", simpleData ]
        let dataPath = Path.Combine(dir, "data/simple.data.xml") |> normalizePathSeparator

        try
            Fs
                $"""
module Test
/// <include file="{dataPath}" path="/data/summary"/>
let f x = x
"""
            |> withXmlDoc
            |> compile
            |> shouldSucceed
            |> verifyXmlDocContains [ "Included summary text." ]
            |> ignore
        finally
            cleanup dir

    [<Fact>]
    let ``Include with XPath selecting specific element expands`` () =
        let dir =
            setupDir [
                "data.xml",
                """<?xml version="1.0"?>
<data>
  <summary>The summary text.</summary>
  <remarks>The remarks text.</remarks>
</data>"""
            ]

        let dataPath = Path.Combine(dir, "data.xml") |> normalizePathSeparator

        try
            Fs
                $"""
module Test
/// <include file="{dataPath}" path="/data/remarks"/>
let f x = x
"""
            |> withXmlDoc
            |> compile
            |> shouldSucceed
            |> verifyXmlDocContains [ "The remarks text." ]
            |> verifyXmlDocNotContains [ "The summary text." ]
            |> ignore
        finally
            cleanup dir

    [<Theory>]
    [<InlineData("/data/remarks",
                 "<summary>Inline before <remarks>Included remarks text.</remarks> inline after.</summary>")>]
    [<InlineData("/data/*",
                 "<summary>Inline before <summary>Included summary text.</summary><remarks>Included remarks text.</remarks> inline after.</summary>")>]
    let ``Inline include expands selected elements in place`` (xpath: string) (expectedInner: string) =
        let res =
            runInclude (scenario (Snippets.memberInlineInclude "d.xml" xpath) [ "d.xml", Snippets.dataSummaryRemarks ])

        res.Compilation |> shouldSucceed |> ignore

        res.Xml
        |> memberXmlEquals "M:Test.inlineIncluded(System.Int32)" expectedInner

    [<Fact>]
    let ``Inline include preserves sibling XML elements`` () =
        let source =
            $"""module Test

/// <summary>See {Snippets.includeElement "d.xml" "/data/remarks"} and <see cref="T:System.Int32"/> here.</summary>
let inlineWithSibling (x: int) = x
"""

        let res = runInclude (scenario source [ "d.xml", Snippets.dataSummaryRemarks ])

        res.Compilation |> shouldSucceed |> ignore

        res.Xml
        |> memberXmlEquals
            "M:Test.inlineWithSibling(System.Int32)"
            "<summary>See <remarks>Included remarks text.</remarks> and <see cref=\"T:System.Int32\"/> here.</summary>"

    [<Fact>]
    let ``Nested includes in external file expand`` () =
        let dir =
            setupDir [
                "outer.xml",
                """<?xml version="1.0"?>
<data>
  <summary>Outer start. <include file="inner.xml" path="/inner/detail"/> Outer end.</summary>
</data>"""
                "inner.xml",
                """<?xml version="1.0"?>
<inner>
  <detail>Inner detail text.</detail>
</inner>"""
            ]

        let outerPath = Path.Combine(dir, "outer.xml") |> normalizePathSeparator

        try
            Fs
                $"""
module Test
/// <include file="{outerPath}" path="/data/summary"/>
let f x = x
"""
            |> withXmlDoc
            |> compile
            |> shouldSucceed
            |> verifyXmlDocContains [ "Inner detail text." ]
            |> ignore
        finally
            cleanup dir

    [<Fact>]
    let ``Zero xpath matches emits no warning and inserts comment`` () =
        let res =
            runInclude (scenario (Snippets.memberWithInclude "d.xml" "/data/nope") [ "d.xml", Snippets.dataSummaryRemarks ])

        // Roslyn parity: a valid XPath that matches nothing must NOT emit any diagnostic.
        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

        // Roslyn parity: comment FIRST, then the original include tag is kept verbatim.
        res.Xml
        |> memberXmlEquals
            "M:Test.included(System.Int32,System.Int32)"
            """<!-- No matching elements were found for the following include tag --><include file="d.xml" path="/data/nope"/>"""

    [<Fact>]
    let ``Zero xpath matches inline preserves sibling text`` () =
        let res =
            runInclude (scenario (Snippets.memberInlineInclude "d.xml" "/data/nope") [ "d.xml", Snippets.dataSummaryRemarks ])

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

        // The comment + kept tag are spliced in place; surrounding text survives.
        res.Xml
        |> memberXmlEquals
            "M:Test.inlineIncluded(System.Int32)"
            """<summary>Inline before <!-- No matching elements were found for the following include tag --><include file="d.xml" path="/data/nope"/> inline after.</summary>"""

    [<Fact>]
    let ``Invalid xpath still warns`` () =
        let res =
            runInclude (scenario (Snippets.memberWithInclude "d.xml" "/data/[bad") [ "d.xml", Snippets.dataSummaryRemarks ])

        res.Compilation |> shouldSucceed |> withWarningCode 3908 |> ignore

    [<Fact>]
    let ``Include error names both the file and the xpath`` () =
        // Missing file: the FS3908 message must still name BOTH the file and the xpath.
        let res = runInclude (scenario (Snippets.memberWithInclude "missing-doc.xml" "/data/summary") [])
        res.Compilation |> shouldSucceed |> ignore
        assertSingleIncludeWarningMatches "missing-doc.xml" res
        assertSingleIncludeWarningMatches "/data/summary" res

    [<Fact>]
    let ``Invalid xpath error names both the file and the xpath`` () =
        let res = runInclude (scenario (Snippets.memberWithInclude "d.xml" "bad[[[") [ "d.xml", Snippets.dataSummaryRemarks ])
        res.Compilation |> shouldSucceed |> ignore
        assertSingleIncludeWarningMatches "d.xml" res
        assertSingleIncludeWarningMatches "bad[[[" res

    [<Theory>]
    [<InlineData("internal entity",
                 "<?xml version=\"1.0\"?>\n<!DOCTYPE data [ <!ENTITY lol \"lol\"> <!ENTITY lol2 \"&lol;&lol;&lol;\"> ]>\n<data><summary>&lol2;</summary></data>",
                 null, "lollol", "&lol2;")>]
    [<InlineData("external general entity",
                 "<?xml version=\"1.0\"?>\n<!DOCTYPE data [ <!ENTITY xxe SYSTEM \"file:///etc/hostname\"> ]>\n<data><summary>&xxe;</summary></data>",
                 null, "&xxe;", "hostname")>]
    [<InlineData("external DTD subset",
                 "<?xml version=\"1.0\"?>\n<!DOCTYPE data SYSTEM \"evil.dtd\">\n<data><summary>Should not expand.</summary></data>",
                 "<!ENTITY secret 'DTD SECRET'>", "Should not expand", "DTD SECRET")>]
    [<InlineData("public external DTD subset",
                 "<?xml version=\"1.0\"?>\n<!DOCTYPE data PUBLIC \"-//example//DTD DATA//EN\" \"evil.dtd\">\n<data><summary>Should not expand.</summary></data>",
                 "<!ENTITY secret 'PUBLIC DTD SECRET'>", "Should not expand", "PUBLIC DTD SECRET")>]
    let ``Included file with a DTD is rejected without entity expansion``
        (_case: string)
        (maliciousXml: string)
        (extraDtd: string)
        (forbidden1: string)
        (forbidden2: string)
        =
        let files =
            [ "d.xml", maliciousXml ]
            @ (if isNull extraDtd then [] else [ "evil.dtd", extraDtd ])

        let res =
            runInclude { scenario (Snippets.memberWithInclude "d.xml" "/data/summary") files with WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> ignore
        assertSingleIncludeWarningMatches "DTD is prohibited" res
        assertSingleIncludeWarningMatches "d.xml" res
        assertSingleIncludeWarningMatches "/data/summary" res
        let inner = memberInner "M:Test.included(System.Int32,System.Int32)" res.Xml
        Assert.Contains("<include file=\"d.xml\" path=\"/data/summary\"", inner)
        Assert.DoesNotContain(forbidden1, inner)
        Assert.DoesNotContain(forbidden2, inner)

    [<Fact>]
    let ``Included file that is not well-formed XML warns and keeps the tag`` () =
        // A syntactically broken external file (unclosed <summary>) must not crash the compiler:
        // it warns once via FS3908 (naming both the file and the xpath) and keeps the unexpanded tag.
        let malformed = "<?xml version=\"1.0\"?>\n<data><summary>Unclosed summary</data>"

        let res =
            runInclude (scenario (Snippets.memberWithInclude "broken.xml" "/data/summary") [ "broken.xml", malformed ])

        res.Compilation |> shouldSucceed |> ignore
        assertSingleIncludeWarningMatches "broken.xml" res
        assertSingleIncludeWarningMatches "/data/summary" res
        let inner = memberInner "M:Test.included(System.Int32,System.Int32)" res.Xml
        Assert.Contains("<include file=\"broken.xml\" path=\"/data/summary\"", inner)
        Assert.DoesNotContain("Unclosed summary", inner)

    [<Fact>]
    let ``Namespaced include element is not treated as an include`` () =
        // An element named 'include' but in a foreign XML namespace is ordinary XML, not the
        // documentation include tag (Roslyn parity). It must be preserved and never expanded,
        // and no FS3908 must be emitted even though a matching file and xpath exist.
        let source =
            "module Test\n\n/// <summary><include xmlns=\"urn:not-doc\" file=\"d.xml\" path=\"/data/summary\"/></summary>\nlet included (x: int) (y: int) = x + y\n"

        let res = runInclude (scenario source [ "d.xml", Snippets.dataSummaryRemarks ])

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

        // The foreign-namespace element is kept verbatim; the included text must NOT appear.
        let inner = memberInner "M:Test.included(System.Int32,System.Int32)" res.Xml
        Assert.Contains("urn:not-doc", inner)
        Assert.DoesNotContain("Included summary text.", inner)

    [<Fact>]
    let ``Included code block preserves inter-element whitespace`` () =
        let externalDoc =
            """<?xml version="1.0"?>
<data><summary><code><see cref="T:System.String"/>
    <see cref="T:System.Int32"/></code></summary></data>"""

        let res =
            runInclude (scenario (Snippets.memberWithInclude "d.xml" "/data/summary") [ "d.xml", externalDoc ])

        res.Compilation |> shouldSucceed |> ignore

        let inner = memberInner "M:Test.included(System.Int32,System.Int32)" res.Xml
        Assert.Contains("\n    <see cref=\"T:System.Int32\"", inner)

    [<Fact>]
    let ``Included multiline code block preserves exact whitespace`` () =
        let externalDoc =
            """<?xml version="1.0"?>
<data><summary><code>
    let x = 1

    let y = x + 1
</code></summary></data>"""

        let res =
            runInclude (scenario (Snippets.memberWithInclude "d.xml" "/data/summary") [ "d.xml", externalDoc ])

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

        let expected =
            """
 <summary><code>
    let x = 1

    let y = x + 1
</code></summary>
"""

        Assert.Equal(expected, memberInner "M:Test.included(System.Int32,System.Int32)" res.Xml)

    [<Fact>]
    let ``Non-element xpath result warns`` () =
        // An XPath that selects non-element nodes (here a text node) must warn, not crash XML doc writing.
        let res =
            runInclude (scenario (Snippets.memberWithInclude "d.xml" "/data/summary/text()") [ "d.xml", Snippets.dataSummaryRemarks ])

        res.Compilation |> shouldSucceed |> withWarningCode 3908 |> ignore

    [<Fact>]
    let ``Recursive include chain of depth three fully expands`` () =
        let res =
            runInclude (
                scenario
                    """module Test

/// <include file="a.xml" path="/data/summary"/>
let f (x: int) = x
"""
                    [ "a.xml", Snippets.chainA "b.xml"
                      "b.xml", Snippets.chainB "c.xml"
                      "c.xml", Snippets.chainC "C" ]
            )

        res.Compilation |> shouldSucceed |> ignore
        res.Xml |> memberXmlEquals "M:Test.f(System.Int32)" "<summary>A(<part>B(<leaf>C</leaf>)B</part>)A</summary>"

    [<Fact>]
    let ``Include chain at maximum depth expands fully`` () =
        let includeCount = 64
        let res = runInclude (scenario (Snippets.memberWithInclude "boundary0.xml" "/data/summary") (makeIncludeChainFiles "boundary" includeCount))

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

        let inner = memberInner "M:Test.included(System.Int32,System.Int32)" res.Xml
        Assert.Contains("boundary leaf.", inner)
        Assert.DoesNotContain("<include", inner)

    [<Fact>]
    let ``Include chain over maximum depth warns once and keeps failing include`` () =
        let includeCount = 65
        let res = runInclude (scenario (Snippets.memberWithInclude "overdepth0.xml" "/data/summary") (makeIncludeChainFiles "overdepth" includeCount))

        res.Compilation |> shouldSucceed |> ignore
        assertSingleIncludeWarningMatches "maximum include nesting depth of 64" res
        // The framed message must also name both the file and the xpath.
        assertSingleIncludeWarningMatches "overdepth64.xml" res
        assertSingleIncludeWarningMatches "/data/summary" res

        let inner = memberInner "M:Test.included(System.Int32,System.Int32)" res.Xml
        Assert.Contains("<include file=\"overdepth64.xml\" path=\"/data/summary\"", inner)
        Assert.DoesNotContain("overdepth leaf.", inner)

    [<Fact>]
    let ``Deep include chain stops with expansion limit warning`` () =
        let chainLength = 200

        let files =
            [
                for i in 0 .. chainLength - 1 ->
                    let content =
                        if i = chainLength - 1 then
                            """<?xml version="1.0"?><data><summary>Deep leaf.</summary></data>"""
                        else
                            $"""<?xml version="1.0"?><data><summary>Depth {i}. {Snippets.includeElement $"deep{i + 1}.xml" "/data/summary"}</summary></data>"""

                    $"deep{i}.xml", content
            ]

        let res = runInclude (scenario (Snippets.memberWithInclude "deep0.xml" "/data/summary") files)

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3908
        |> withDiagnosticMessageMatches "maximum include nesting depth of 64"
        |> ignore

    [<Fact>]
    let ``Diamond include DAG expands shared fragments correctly`` () =
        let levels = 8

        let files =
            [
                for i in 0 .. levels do
                    if i = levels then
                        yield $"d{i}.xml", """<?xml version="1.0"?><data><summary>Leaf.</summary></data>"""
                    else
                        yield
                            $"d{i}.xml",
                            $"""<?xml version="1.0"?><data><summary>D{i}[{Snippets.includeElement $"a{i}.xml" "/data/part"}{Snippets.includeElement $"b{i}.xml" "/data/part"}]</summary></data>"""

                        yield
                            $"a{i}.xml",
                            $"""<?xml version="1.0"?><data><part>A{i}{Snippets.includeElement $"d{i + 1}.xml" "/data/summary"}</part></data>"""

                        yield
                            $"b{i}.xml",
                            $"""<?xml version="1.0"?><data><part>B{i}{Snippets.includeElement $"d{i + 1}.xml" "/data/summary"}</part></data>"""
            ]

        let rec expected level =
            if level = levels then
                "<summary>Leaf.</summary>"
            else
                $"<summary>D{level}[<part>A{level}{expected (level + 1)}</part><part>B{level}{expected (level + 1)}</part>]</summary>"

        let res = runInclude (scenario (Snippets.memberWithInclude "d0.xml" "/data/summary") files)

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore
        res.Xml |> memberXmlEquals "M:Test.included(System.Int32,System.Int32)" (expected 0)

    [<Fact>]
    let ``Reused include deeper still respects depth limit`` () =
        let suffixLength = 60
        let prefixLength = 10

        let suffixFiles =
            [
                for i in 0 .. suffixLength - 1 ->
                    let content =
                        if i = suffixLength - 1 then
                            """<?xml version="1.0"?><data><summary>Suffix leaf.</summary></data>"""
                        else
                            $"""<?xml version="1.0"?><data><summary>S{i}. {Snippets.includeElement $"suffix{i + 1}.xml" "/data/summary"}</summary></data>"""

                    $"suffix{i}.xml", content
            ]

        let prefixFiles =
            [
                for i in 0 .. prefixLength - 1 ->
                    let nextInclude =
                        if i = prefixLength - 1 then
                            Snippets.includeElement "suffix0.xml" "/data/summary"
                        else
                            Snippets.includeElement $"prefix{i + 1}.xml" "/data/summary"

                    $"prefix{i}.xml", $"""<?xml version="1.0"?><data><summary>P{i}. {nextInclude}</summary></data>"""
            ]

        let source =
            $"""module Test

/// <summary>{Snippets.includeElement "suffix0.xml" "/data/summary"} {Snippets.includeElement "prefix0.xml" "/data/summary"}</summary>
let f (x: int) = x
"""

        let res = runInclude (scenario source (suffixFiles @ prefixFiles))

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3908
        |> withDiagnosticMessageMatches "maximum include nesting depth of 64"
        |> ignore

    [<Fact>]
    let ``Relative include inside external file resolves relative to that file`` () =
        // b.xml lives in d1/ and includes a BARE relative "c.xml": it must resolve to d1/c.xml
        // (b's directory), NOT the source directory. A decoy c.xml in the source dir must be ignored.
        let res =
            runInclude (
                scenario
                    """module Test

/// <include file="d1/b.xml" path="/data/part"/>
let f (x: int) = x
"""
                    [ "d1/b.xml", Snippets.chainB "c.xml"
                      "d1/c.xml", Snippets.chainC "Relative C"
                      "c.xml", Snippets.chainC "Root decoy C" ]
            )

        res.Compilation |> shouldSucceed |> ignore
        res.Xml |> memberXmlEquals "M:Test.f(System.Int32)" "<part>B(<leaf>Relative C</leaf>)B</part>"
        Assert.DoesNotContain("Root decoy C", memberInner "M:Test.f(System.Int32)" res.Xml)

    [<Fact>]
    let ``External xpath selecting two siblings inserts both in order`` () =
        let res =
            runInclude (
                scenario
                    """module Test

/// <include file="sib.xml" path="/data/item"/>
let f (x: int) = x
"""
                    [ "sib.xml", Snippets.twoSiblings ]
            )

        res.Compilation |> shouldSucceed |> ignore
        res.Xml |> memberXmlEquals "M:Test.f(System.Int32)" "<item>One</item><item>Two</item>"

    [<Fact>]
    let ``Missing include file does not fail compilation`` () =
        Fs
            """
module Test
/// <include file="/nonexistent/file.xml" path="/data/summary"/>
let f x = x
"""
        |> withXmlDoc
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Missing include file warns by default`` () =
        let res =
            runInclude (scenario (Snippets.memberWithInclude "does-not-exist.xml" "/data/summary") [])

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3908
        |> withDiagnosticMessageMatches "include"
        |> ignore

    [<Fact>]
    let ``Regular doc without include works`` () =
        Fs
            """
module Test
/// <summary>Regular summary</summary>
let f x = x
"""
        |> withXmlDoc
        |> compile
        |> shouldSucceed
        |> verifyXmlDocContains [ "Regular summary" ]
        |> ignore

    [<Fact>]
    let ``Circular include does not hang`` () =
        let dir =
            setupDir [
                "a.xml",
                """<?xml version="1.0"?>
<data>
  <summary>A <include file="b.xml" path="/data/inner"/> end.</summary>
</data>"""
                "b.xml",
                """<?xml version="1.0"?>
<data>
  <inner>B <include file="a.xml" path="/data/summary"/> end.</inner>
</data>"""
            ]

        let aPath = Path.Combine(dir, "a.xml") |> normalizePathSeparator

        try
            Fs
                $"""
module Test
/// <include file="{aPath}" path="/data/summary"/>
let f x = x
"""
            |> withXmlDoc
            |> ignoreWarnings
            |> compile
            |> shouldSucceed
            |> ignore
        finally
            cleanup dir

    [<Fact>]
    let ``Same file different xpath is not a cycle`` () =
        // The member includes /data/summary of self.xml; that <summary> in turn includes
        // /data/remarks of the SAME file. Different sections => must NOT be a false cycle.
        let selfData =
            """<?xml version="1.0"?>
<data>
  <summary>S: <include file="self.xml" path="/data/remarks"/></summary>
  <remarks>Shared remarks.</remarks>
</data>"""

        let res =
            runInclude (scenario (Snippets.memberWithInclude "self.xml" "/data/summary") [ "self.xml", selfData ])

        res.Compilation |> shouldSucceed |> ignore

        // If a false cycle fired, the inner <include> would survive unexpanded and this would NOT match.
        res.Xml
        |> memberXmlEquals
            "M:Test.included(System.Int32,System.Int32)"
            "<summary>S: <remarks>Shared remarks.</remarks></summary>"

    [<Fact>]
    let ``Case-distinct include paths are ordinal cycle keys`` () =
        let keys = HashSet<struct (string * string)>()
        keys.Add(struct ("Data.xml", "/data/summary")) |> ignore
        Assert.False(keys.Contains(struct ("data.xml", "/data/summary")))

        if fileSystemSupportsCaseDistinctFiles () then
            let source = Snippets.memberWithInclude "Data.xml" "/data/summary"

            let dataUpper =
                """<?xml version="1.0"?>
<data>
  <summary>Upper start. <include file="data.xml" path="/data/summary"/> Upper end.</summary>
</data>"""

            let dataLower =
                """<?xml version="1.0"?>
<data>
  <summary>Lower summary.</summary>
</data>"""

            let res =
                runInclude (scenario source [ "Data.xml", dataUpper; "data.xml", dataLower ])

            res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

            res.Xml
            |> memberXmlEquals
                "M:Test.included(System.Int32,System.Int32)"
                "<summary>Upper start. <summary>Lower summary.</summary> Upper end.</summary>"

    [<Fact>]
    let ``Self include cycle is detected and terminates`` () =
        let res =
            runInclude (
                scenario
                    (Snippets.memberWithInclude "self.xml" "/data/summary")
                    [ "self.xml", Snippets.selfCycle "self.xml" ]
            )

        // Genuine self-reference (/data/summary includes /data/summary) must warn and terminate (test finishing = termination).
        res.Compilation |> shouldSucceed |> ignore
        assertSingleIncludeWarningMatches "a circular include was detected" res
        // The framed message must also name both the file and the xpath.
        assertSingleIncludeWarningMatches "self.xml" res
        assertSingleIncludeWarningMatches "/data/summary" res

    [<Fact>]
    let ``Mutual include cycle between two files is detected and warns`` () =
        let res =
            runInclude (
                scenario
                    (Snippets.memberWithInclude "a.xml" "/data/summary")
                    [
                        "a.xml",
                        """<?xml version="1.0"?><data><summary>A: <include file="b.xml" path="/data/inner"/> end.</summary></data>"""
                        "b.xml",
                        """<?xml version="1.0"?><data><inner>B: <include file="a.xml" path="/data/summary"/> end.</inner></data>"""
                    ]
            )

        // A(/data/summary) -> B(/data/inner) -> A(/data/summary): genuine cycle must warn and terminate.
        res.Compilation |> shouldSucceed |> withWarningCode 3908 |> ignore

    [<Fact>]
    let ``Same file and xpath from sibling positions both expand`` () =
        // The same (file, xpath) appears at two NON-nested sibling sites; per-branch visited-set
        // copying must let both expand without a false circular-include warning.
        let source =
            $"""module Test

/// <summary>First {Snippets.includeElement "shared.xml" "/data/item"} and second {Snippets.includeElement "shared.xml" "/data/item"}</summary>
let siblingIncludes (x: int) = x
"""

        let res =
            runInclude (scenario source [ "shared.xml", """<?xml version="1.0"?><data><item>Shared.</item></data>""" ])

        res.Compilation |> shouldSucceed |> ignore

        res.Xml
        |> memberXmlEquals
            "M:Test.siblingIncludes(System.Int32)"
            "<summary>First <item>Shared.</item> and second <item>Shared.</item></summary>"

    [<Fact>]
    let ``Same include file used by two members expands for both`` () =
        let source =
            """module Test

/// <include file="shared.xml" path="/data/summary"/>
let first (x: int) = x

/// <include file="shared.xml" path="/data/summary"/>
let second (x: int) = x
"""

        let res = runInclude (scenario source [ "shared.xml", Snippets.dataSummaryRemarks ])

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

        res.Xml |> memberXmlEquals "M:Test.first(System.Int32)" "<summary>Included summary text.</summary>"
        res.Xml |> memberXmlEquals "M:Test.second(System.Int32)" "<summary>Included summary text.</summary>"

    [<Fact>]
    let ``Include budget is per documented member`` () =
        let includeCountPerMember = 6000
        let includes = String.replicate includeCountPerMember (Snippets.includeElement "leaf.xml" "/data/leaf")

        let source =
            $"""module Test

/// <summary>{includes}</summary>
let first (x: int) = x

/// <summary>{includes}</summary>
let second (x: int) = x
"""

        let res =
            runInclude (scenario source [ "leaf.xml", """<?xml version="1.0"?><data><leaf>L</leaf></data>""" ])

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

        let firstInner = memberInner "M:Test.first(System.Int32)" res.Xml
        let secondInner = memberInner "M:Test.second(System.Int32)" res.Xml

        Assert.Equal(includeCountPerMember, countSubstring "<leaf>L</leaf>" firstInner)
        Assert.Equal(includeCountPerMember, countSubstring "<leaf>L</leaf>" secondInner)
        Assert.DoesNotContain("<include", firstInner)
        Assert.DoesNotContain("<include", secondInner)

    [<Fact>]
    let ``Document with exactly maximum include budget expands all siblings`` () =
        let includeCount = 10000
        let includes = String.replicate includeCount (Snippets.includeElement "leaf.xml" "/data/leaf")

        let source =
            $"""module Test

/// <summary>{includes}</summary>
let f (x: int) = x
"""

        let res =
            runInclude (scenario source [ "leaf.xml", """<?xml version="1.0"?><data><leaf>L</leaf></data>""" ])

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

        let inner = memberInner "M:Test.f(System.Int32)" res.Xml
        Assert.Equal(includeCount, countSubstring "<leaf>L</leaf>" inner)
        Assert.DoesNotContain("<include", inner)

    [<Fact>]
    let ``Document over maximum include budget warns once and keeps failing includes`` () =
        // Several excess includes: the budget limit must be reported exactly once per document,
        // not once per over-budget include (no warning spam), while every unexpanded tag is kept.
        let excessCount = 5
        let includeCount = 10000 + excessCount
        let includes = String.replicate includeCount (Snippets.includeElement "leaf.xml" "/data/leaf")

        let source =
            $"""module Test

/// <summary>{includes}</summary>
let f (x: int) = x
"""

        let res =
            runInclude (scenario source [ "leaf.xml", """<?xml version="1.0"?><data><leaf>L</leaf></data>""" ])

        res.Compilation |> shouldSucceed |> ignore
        assertSingleIncludeWarningMatches "maximum of 10000 include expansions" res
        // The framed message must also name both the file and the xpath.
        assertSingleIncludeWarningMatches "leaf.xml" res
        assertSingleIncludeWarningMatches "/data/leaf" res

        let inner = memberInner "M:Test.f(System.Int32)" res.Xml
        Assert.Equal(10000, countSubstring "<leaf>L</leaf>" inner)
        Assert.Equal(excessCount, countSubstring "<include file=\"leaf.xml\" path=\"/data/leaf\"" inner)

    [<Fact>]
    let ``Include with rich XML content preserves structure`` () =
        let dir =
            setupDir [
                "data.xml",
                """<?xml version="1.0"?>
<data>
  <summary>Text with <b>bold</b> and <c>code</c> content.</summary>
</data>"""
            ]

        let dataPath = Path.Combine(dir, "data.xml") |> normalizePathSeparator

        try
            Fs
                $"""
module Test
/// <include file="{dataPath}" path="/data/summary"/>
let f x = x
"""
            |> withXmlDoc
            |> compile
            |> shouldSucceed
            |> verifyXmlDocContains [ "<b>bold</b>"; "<c>code</c>" ]
            |> ignore
        finally
            cleanup dir

    [<Fact>]
    let ``Include tag is not present in output`` () =
        let dir = setupDir [ "data/simple.data.xml", simpleData ]
        let dataPath = Path.Combine(dir, "data/simple.data.xml") |> normalizePathSeparator

        try
            Fs
                $"""
module Test
/// <include file="{dataPath}" path="/data/summary"/>
let f x = x
"""
            |> withXmlDoc
            |> compile
            |> shouldSucceed
            |> verifyXmlDocNotContains [ "<include" ]
            |> ignore
        finally
            cleanup dir

    [<Fact>]
    let ``Multiple includes in same doc expand`` () =
        let dir =
            setupDir [
                "data1.xml",
                """<?xml version="1.0"?>
<data>
  <part1>First part.</part1>
</data>"""
                "data2.xml",
                """<?xml version="1.0"?>
<data>
  <part2>Second part.</part2>
</data>"""
            ]

        let path1 = Path.Combine(dir, "data1.xml") |> normalizePathSeparator
        let path2 = Path.Combine(dir, "data2.xml") |> normalizePathSeparator

        try
            Fs
                $"""
module Test
/// <summary>
/// <include file="{path1}" path="/data/part1"/>
/// <include file="{path2}" path="/data/part2"/>
/// </summary>
let f x = x
"""
            |> withXmlDoc
            |> compile
            |> shouldSucceed
            |> verifyXmlDocContains [ "First part."; "Second part." ]
            |> ignore
        finally
            cleanup dir

    [<Fact>]
    let ``Include with empty path attribute generates warning`` () =
        let res =
            runInclude (scenario (Snippets.memberWithInclude "data/simple.data.xml" "") [ "data/simple.data.xml", simpleData ])

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3908
        |> withDiagnosticMessageMatches "XPath expression is empty"
        // Even with an empty xpath, the framed message still names the file.
        |> withDiagnosticMessageMatches "data/simple.data.xml"
        |> ignore

        Assert.True(res.XmlExists, $"XML doc file should exist: {res.XmlPath}")
        Assert.DoesNotContain("Included summary text.", res.Xml)

    [<Fact>]
    let ``Include missing file attribute does not fail compilation`` () =
        Fs
            """
module Test
/// <include path="/data/summary"/>
let f x = x
"""
        |> withXmlDoc
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Include missing path attribute does not fail compilation`` () =
        let dir = setupDir [ "data/simple.data.xml", simpleData ]
        let dataPath = Path.Combine(dir, "data/simple.data.xml") |> normalizePathSeparator

        try
            Fs
                $"""
module Test
/// <include file="{dataPath}"/>
let f x = x
"""
            |> withXmlDoc
            |> ignoreWarnings
            |> compile
            |> shouldSucceed
            |> ignore
        finally
            cleanup dir

    [<Fact>]
    let ``Included param documentation satisfies all-params-documented rule`` () =
        // x is documented inline, y ONLY via include. Without expansion in Check, the
        // "document all params" rule fires for y (3390). With expansion, both count.
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <param name="x">Inline x doc.</param>
/// <include file="p.xml" path="/docs/param"/>
let f (x: int) (y: int) = x + y
"""
                    [ "p.xml", """<?xml version="1.0"?><docs><param name="y">Included y doc.</param></docs>""" ]
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

    [<Theory>]
    [<InlineData("param",
                 """<param name="Q">Doc for a non-existent param.</param>""",
                 "unknown parameter 'Q'")>]
    [<InlineData("paramref",
                 """<paramref name="Q"/>""",
                 "This XML comment is invalid: unknown parameter 'Q'")>]
    [<InlineData("param",
                 """<param name="x">Included duplicate x doc.</param>""",
                 "This XML comment is invalid: multiple documentation entries for parameter 'x'")>]
    [<InlineData("param",
                 """<param>Included param without a name.</param>""",
                 "This XML comment is invalid: missing 'name' attribute for parameter or parameter reference")>]
    let ``Included param or paramref that fails validation warns`` (pathTag: string) (fragment: string) (message: string) =
        let source =
            $"""module Test

/// <summary>S</summary>
/// <param name="x">Inline x doc.</param>
/// <include file="p.xml" path="/docs/{pathTag}"/>
let f (x: int) = x
"""

        let res =
            runInclude
                { scenario source [ "p.xml", $"""<?xml version="1.0"?><docs>{fragment}</docs>""" ] with
                    WarnOn = [ 3390 ] }

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3390
        |> withDiagnosticMessageMatches message
        |> ignore

    [<Fact>]
    let ``Included paramref for an existing parameter is accepted`` () =
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <param name="x">Inline x doc.</param>
/// <include file="p.xml" path="/docs/paramref"/>
let f (x: int) = x
"""
                    [ "p.xml", """<?xml version="1.0"?><docs><paramref name="x"/></docs>""" ]
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

    [<Fact>]
    let ``Included XPath matching multiple params satisfies param validation`` () =
        let res =
            runInclude { scenario (Snippets.memberWithInclude "params.xml" "/data/param") [ "params.xml", Snippets.dataTwoParams ] with WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

    [<Fact>]
    let ``Nested include param documentation satisfies param validation`` () =
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <param name="x">Inline x doc.</param>
/// <include file="a.xml" path="/docs/include"/>
let f (x: int) (y: int) = x + y
"""
                    [
                        "a.xml",
                        """<?xml version="1.0"?><docs><include file="b.xml" path="/docs/param"/></docs>"""
                        "b.xml",
                        """<?xml version="1.0"?><docs><param name="y">Included y doc.</param></docs>"""
                    ]
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

    [<Fact>]
    let ``Included param before inline param satisfies param validation`` () =
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <include file="p.xml" path="/docs/param"/>
/// <param name="y">Inline y doc.</param>
let f (x: int) (y: int) = x + y
"""
                    [ "p.xml", """<?xml version="1.0"?><docs><param name="x">Included x doc.</param></docs>""" ]
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

    [<Fact>]
    let ``Quiet doc checking expands recursive includes under limit without include warnings`` () =
        let res =
            runInclude
                { scenario
                    (Snippets.memberWithInclude "quiet0.xml" "/data/summary")
                    (makeIncludeChainFiles "quiet" 10)
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore
        Assert.Equal(0, includeWarningCount res)
        let inner = memberInner "M:Test.included(System.Int32,System.Int32)" res.Xml
        Assert.Contains("quiet leaf.", inner)
        Assert.DoesNotContain("<include", inner)

    [<Fact>]
    let ``Quiet doc checking does not duplicate include expansion limit warning`` () =
        let res =
            runInclude
                { scenario
                    (Snippets.memberWithInclude "quietover0.xml" "/data/summary")
                    (makeIncludeChainFiles "quietover" 65)
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> ignore
        assertSingleIncludeWarningMatches "maximum include nesting depth of 64" res

    [<Fact>]
    let ``Include error is reported once when doc checking and doc generation are both on`` () =
        // --warnon:3390 makes Check run (emit=false, quiet); --doc makes the writer run (emit=true).
        // A missing include file must yield EXACTLY ONE 3908, not two.
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <include file="does-not-exist.xml" path="/data/summary"/>
let f (x: int) = x
"""
                    []
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> ignore
        Assert.Equal(1, includeWarningCount res)

    [<Fact>]
    let ``Whitespace-only doc with a non-XML whitespace char does not warn under param checking`` () =
        // Regression: IsEmpty docs must short-circuit to "" (parity with GetXmlText); otherwise a
        // non-XML whitespace char (form feed) makes XDocument.Parse throw -> spurious FS3390.
        let res =
            runInclude { scenario "module Test\n\n///\u000C\nlet f (x: int) = x\n" [] with WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore

    [<Fact>]
    let ``Include file resolves against the working directory when absent next to the source`` () =
        // RFC FS-1341 / C# XmlFileResolver parity: a relative file="" is resolved next to the
        // including source file first, then falls back to the compiler's working directory.
        let sourceDir = (createTemporaryDirectory ()).FullName
        let subdir = "xmlinc_" + Guid.NewGuid().ToString("N")
        let workingDirRelativeDir = Path.Combine(Directory.GetCurrentDirectory(), subdir)
        Directory.CreateDirectory workingDirRelativeDir |> ignore
        File.WriteAllText(Path.Combine(workingDirRelativeDir, "data.xml"), simpleData)

        // Bare relative path: absent next to the source (sourceDir/subdir/data.xml),
        // present under the working directory (cwd/subdir/data.xml).
        let includeRef = subdir + "/data.xml"

        try
            Fs
                $"""module Test

/// {Snippets.includeElement includeRef "/data/summary"}
let f (x: int) = x
"""
            |> withFileName (Path.Combine(sourceDir, "Library.fs"))
            |> withName "Library"
            |> withOutputDirectory (Some(DirectoryInfo sourceDir))
            |> withXmlDoc
            |> ignoreWarnings
            |> compile
            |> shouldSucceed
            |> verifyXmlDocContains [ "Included summary text." ]
            |> verifyXmlDocNotContains [ "<include" ]
            |> ignore
        finally
            cleanup sourceDir
            cleanup workingDirRelativeDir

    [<Fact>]
    let ``Include in a signature file resolves relative to the signature file`` () =
        // RFC FS-1341: for a member declared in a signature file, the .fsi documentation is
        // authoritative, and its <include> resolves relative to the .fsi (not the implementation).
        let dir = (createTemporaryDirectory ()).FullName
        File.WriteAllText(Path.Combine(dir, "data.xml"), simpleData)

        try
            Fsi
                $"""module Test

/// {Snippets.includeElement "data.xml" "/data/summary"}
val f: x: int -> int
"""
            |> withFileName (Path.Combine(dir, "Library.fsi"))
            |> withName "Library"
            |> withAdditionalSourceFile (FsSourceWithFileName (Path.Combine(dir, "Library.fs")) "module Test\n\nlet f (x: int) = x\n")
            |> withOutputDirectory (Some(DirectoryInfo dir))
            |> withXmlDoc
            |> ignoreWarnings
            |> compile
            |> shouldSucceed
            |> verifyXmlDocContains [ "Included summary text." ]
            |> verifyXmlDocNotContains [ "<include" ]
            |> ignore
        finally
            cleanup dir
