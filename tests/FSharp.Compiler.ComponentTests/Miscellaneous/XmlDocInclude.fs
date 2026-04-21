// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Miscellaneous

open System
open System.IO
open Xunit
open FSharp.Test.Compiler

module XmlDocInclude =

    // Test helper: create temp directory with files
    let private setupDir (files: (string * string) list) =
        let dir = Path.Combine(Path.GetTempPath(), "XmlDocTest_" + Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(dir) |> ignore

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
        let dataPath = Path.Combine(dir, "data/simple.data.xml").Replace("\\", "/")

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

        let dataPath = Path.Combine(dir, "data.xml").Replace("\\", "/")

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

        let outerPath = Path.Combine(dir, "outer.xml").Replace("\\", "/")

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
    let ``Missing include file does not fail compilation`` () =
        Fs
            """
module Test
/// <include file="/nonexistent/file.xml" path="/data/summary"/>
let f x = x
"""
        |> withXmlDoc
        |> compile
        |> shouldSucceed
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

        let aPath = Path.Combine(dir, "a.xml").Replace("\\", "/")

        try
            Fs
                $"""
module Test
/// <include file="{aPath}" path="/data/summary"/>
let f x = x
"""
            |> withXmlDoc
            |> compile
            |> shouldSucceed
            |> ignore
        finally
            cleanup dir

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

        let dataPath = Path.Combine(dir, "data.xml").Replace("\\", "/")

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
        let dataPath = Path.Combine(dir, "data/simple.data.xml").Replace("\\", "/")

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

        let path1 = Path.Combine(dir, "data1.xml").Replace("\\", "/")
        let path2 = Path.Combine(dir, "data2.xml").Replace("\\", "/")

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
        let dir = setupDir [ "data/simple.data.xml", simpleData ]
        let dataPath = Path.Combine(dir, "data/simple.data.xml").Replace("\\", "/")

        try
            Fs
                $"""
module Test
/// <include file="{dataPath}" path=""/>
let f x = x
"""
            |> withXmlDoc
            |> compile
            |> shouldSucceed
            |> verifyXmlDocNotContains [ "Included summary text." ]
            |> ignore
        finally
            cleanup dir
