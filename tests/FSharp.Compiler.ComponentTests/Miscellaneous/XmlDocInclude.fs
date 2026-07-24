// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Miscellaneous

open System
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

    [<Fact>]
    let ``Inline include inside summary expands`` () =
        let res =
            runInclude (
                scenario
                    (Snippets.memberInlineInclude "d.xml" "/data/remarks")
                    [ "d.xml", Snippets.dataSummaryRemarks ]
            )

        res.Compilation |> shouldSucceed |> ignore

        res.Xml
        |> memberXmlEquals
            "M:Test.inlineIncluded(System.Int32)"
            "<summary>Inline before <remarks>Included remarks text.</remarks> inline after.</summary>"

    [<Fact>]
    let ``Inline include with XPath selecting multiple elements expands all inline`` () =
        let res =
            runInclude (
                scenario
                    (Snippets.memberInlineInclude "d.xml" "/data/*")
                    [ "d.xml", Snippets.dataSummaryRemarks ]
            )

        res.Compilation |> shouldSucceed |> ignore

        res.Xml
        |> memberXmlEquals
            "M:Test.inlineIncluded(System.Int32)"
            "<summary>Inline before <summary>Included summary text.</summary><remarks>Included remarks text.</remarks> inline after.</summary>"

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

        res.Compilation |> shouldSucceed |> withWarningCode 3887 |> ignore

    [<Fact>]
    let ``Non-element xpath result warns`` () =
        // An XPath that selects non-element nodes (here a text node) must warn, not crash XML doc writing.
        let res =
            runInclude (scenario (Snippets.memberWithInclude "d.xml" "/data/summary/text()") [ "d.xml", Snippets.dataSummaryRemarks ])

        res.Compilation |> shouldSucceed |> withWarningCode 3887 |> ignore

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
        |> withWarningCode 3887
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
    let ``Self include cycle is detected and terminates`` () =
        let res =
            runInclude (
                scenario
                    (Snippets.memberWithInclude "self.xml" "/data/summary")
                    [ "self.xml", Snippets.selfCycle "self.xml" ]
            )

        // Genuine self-reference (/data/summary includes /data/summary) must warn and terminate (test finishing = termination).
        res.Compilation |> shouldSucceed |> withWarningCode 3887 |> ignore

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
        res.Compilation |> shouldSucceed |> withWarningCode 3887 |> ignore

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
        |> withWarningCode 3887
        |> withDiagnosticMessageMatches "XPath expression is empty"
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

    [<Fact>]
    let ``Included param for a non-existent parameter warns`` () =
        // The include brings <param name="Q"> but f has no parameter Q -> unknown-parameter warning.
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <param name="x">Inline x doc.</param>
/// <include file="p.xml" path="/docs/param"/>
let f (x: int) = x
"""
                    [ "p.xml", """<?xml version="1.0"?><docs><param name="Q">Doc for a non-existent param.</param></docs>""" ]
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3390
        |> withDiagnosticMessageMatches "unknown parameter 'Q'"
        |> ignore

    [<Fact>]
    let ``Included paramref for a non-existent parameter warns`` () =
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <param name="x">Inline x doc.</param>
/// <include file="p.xml" path="/docs/paramref"/>
let f (x: int) = x
"""
                    [ "p.xml", """<?xml version="1.0"?><docs><paramref name="Q"/></docs>""" ]
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3390
        |> withDiagnosticMessageMatches "This XML comment is invalid: unknown parameter 'Q'"
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
    let ``Included duplicate param documentation warns`` () =
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <param name="x">Inline x doc.</param>
/// <include file="p.xml" path="/docs/param"/>
let f (x: int) = x
"""
                    [ "p.xml", """<?xml version="1.0"?><docs><param name="x">Included duplicate x doc.</param></docs>""" ]
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3390
        |> withDiagnosticMessageMatches "This XML comment is invalid: multiple documentation entries for parameter 'x'"
        |> ignore

    [<Fact>]
    let ``Included param without name warns`` () =
        let res =
            runInclude
                { scenario
                    """module Test

/// <summary>S</summary>
/// <param name="x">Inline x doc.</param>
/// <include file="p.xml" path="/docs/param"/>
let f (x: int) = x
"""
                    [ "p.xml", """<?xml version="1.0"?><docs><param>Included param without a name.</param></docs>""" ]
                  with
                    WarnOn = [ 3390 ] }

        res.Compilation
        |> shouldSucceed
        |> withWarningCode 3390
        |> withDiagnosticMessageMatches "This XML comment is invalid: missing 'name' attribute for parameter or parameter reference"
        |> ignore

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
    let ``Include error is reported once when doc checking and doc generation are both on`` () =
        // --warnon:3390 makes Check run (emit=false, quiet); --doc makes the writer run (emit=true).
        // A missing include file must yield EXACTLY ONE 3887, not two.
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

        let includeWarnings =
            res.Compilation.Output.Diagnostics
            |> List.filter (fun diagnostic -> diagnostic.Error = Warning 3887)

        Assert.Equal(1, includeWarnings.Length)

    [<Fact>]
    let ``Whitespace-only doc with a non-XML whitespace char does not warn under param checking`` () =
        // Regression: IsEmpty docs must short-circuit to "" (parity with GetXmlText); otherwise a
        // non-XML whitespace char (form feed) makes XDocument.Parse throw -> spurious FS3390.
        let res =
            runInclude { scenario "module Test\n\n///\u000C\nlet f (x: int) = x\n" [] with WarnOn = [ 3390 ] }

        res.Compilation |> shouldSucceed |> withDiagnostics [] |> ignore
