// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open System
open System.IO
open System.Security
open System.Xml.Linq
open TestFramework
open FSharp.Test.Compiler

module XmlDocIncludeTestFramework =

    type IncludeScenario = { Source: string; Files: (string * string) list; WarnOn: int list }

    type IncludeResult = { Xml: string; XmlExists: bool; XmlPath: string; Compilation: CompilationResult }

    let scenario source files = { Source = source; Files = files; WarnOn = [] }

    let private fullPathForRelativeFile (directory: DirectoryInfo) (relativePath: string) =
        if String.IsNullOrWhiteSpace relativePath then
            invalidArg (nameof relativePath) "Include test file paths must be non-empty relative paths."

        if Path.IsPathRooted relativePath then
            invalidArg (nameof relativePath) $"Include test file path must be relative: {relativePath}"

        Path.GetFullPath(Path.Combine(directory.FullName, relativePath))

    let private writeScenarioFile directory (relativePath, contents: string) =
        let path = fullPathForRelativeFile directory relativePath

        match Path.GetDirectoryName path with
        | parent when not (String.IsNullOrEmpty parent) -> Directory.CreateDirectory parent |> ignore
        | _ -> ()

        File.WriteAllText(path, contents)

    let runInclude includeScenario =
        let directory = createTemporaryDirectory ()

        for file in includeScenario.Files do
            writeScenarioFile directory file

        let xmlPath = Path.Combine(directory.FullName, "Library.xml")

        let result =
            Fs includeScenario.Source
            |> withFileName (Path.Combine(directory.FullName, "Library.fs"))
            |> withName "Library"
            |> withOutputDirectory (Some directory)
            |> withXmlDoc
            |> ignoreWarnings
            |> fun compilationUnit ->
                (compilationUnit, includeScenario.WarnOn)
                ||> List.fold (fun current warning -> current |> withWarnOn warning)
            |> compile

        let xmlExists = File.Exists xmlPath

        {
            Xml = if xmlExists then File.ReadAllText xmlPath else ""
            XmlExists = xmlExists
            XmlPath = xmlPath
            Compilation = result
        }

    // Text-output verification reads emitted .xml directly, decoupled from the compiler doc reader under test.
    let private tryMemberInner memberName xml =
        if String.IsNullOrWhiteSpace xml then
            failwith "No XML documentation was emitted (did compilation succeed? check the CompilationResult)"

        let document =
            try
                XDocument.Parse(xml, LoadOptions.PreserveWhitespace)
            with ex ->
                failwith $"Could not parse XML documentation output: {ex.Message}\nFull XML:\n{xml}"

        let matchingMembers =
            document.Descendants(XName.Get "member")
            |> Seq.filter (fun element ->
                let nameAttribute = element.Attribute(XName.Get "name")
                not (isNull nameAttribute) && nameAttribute.Value = memberName)
            |> Seq.toList

        let matchingMember =
            match matchingMembers with
            | [] -> None
            | [ element ] -> Some element
            | members -> failwith $"Ambiguous: {members.Length} members named '{memberName}'"

        matchingMember
        |> Option.map (fun element ->
            element.Nodes()
            |> Seq.map (fun node -> node.ToString(SaveOptions.DisableFormatting))
            |> String.concat "")

    let memberInner memberName xml =
        tryMemberInner memberName xml
        |> Option.defaultWith (fun () -> failwith $"Could not find XML documentation member '{memberName}'.\nFull XML:\n{xml}")

    let private canonicalizeInnerXml fragment =
        let root =
            try
                XElement.Parse("<r>" + fragment + "</r>", LoadOptions.PreserveWhitespace)
            with ex ->
                failwith $"Could not parse XML documentation fragment: {ex.Message}\nFragment:\n{fragment}"

        root.DescendantNodes()
        |> Seq.choose (function :? XText as t -> Some t | _ -> None)
        |> Seq.filter (fun t -> String.IsNullOrWhiteSpace t.Value && (t.Value.Contains "\n" || t.Value.Contains "\r"))
        |> Seq.toList
        |> List.iter (fun t -> t.Remove())

        root.ToString(SaveOptions.DisableFormatting)

    let memberXmlEquals memberName expectedInner xml =
        let actualInner = memberInner memberName xml
        let expectedCanonical = canonicalizeInnerXml expectedInner
        let actualCanonical = canonicalizeInnerXml actualInner

        if expectedCanonical <> actualCanonical then
            failwith
                $"""XML documentation member '{memberName}' did not match.
Expected:
{expectedInner}

Actual:
{actualInner}

Expected canonical:
{expectedCanonical}

Actual canonical:
{actualCanonical}

Full XML:
{xml}"""

    module Snippets =

        let includeElement file path =
            $"""<include file="{normalizePathSeparator file |> SecurityElement.Escape}" path="{SecurityElement.Escape path}"/>"""

        let dataSummaryRemarks =
            """<?xml version="1.0"?>
<data>
  <summary>Included summary text.</summary>
  <remarks>Included remarks text.</remarks>
</data>"""

        let dataTwoParams =
            """<?xml version="1.0"?>
<data>
  <param name="x">Included x parameter.</param>
  <param name="y">Included y parameter.</param>
</data>"""

        let chainA fileB =
            $"""<?xml version="1.0"?><data><summary>A({includeElement fileB "/data/part"})A</summary></data>"""

        let chainB fileC =
            $"""<?xml version="1.0"?><data><part>B({includeElement fileC "/data/leaf"})B</part></data>"""

        let chainC leafText =
            $"""<?xml version="1.0"?><data><leaf>{leafText}</leaf></data>"""

        let twoSiblings =
            """<?xml version="1.0"?><data><item>One</item><item>Two</item></data>"""

        let selfCycle selfFile =
            $"""<?xml version="1.0"?><data><summary>Self cycle start. {includeElement selfFile "/data/summary"} Self cycle end.</summary></data>"""

        let memberWithInclude file path =
            $"""module Test

/// {includeElement file path}
let included (x: int) (y: int) = x + y
"""

        let memberInlineInclude file path =
            $"""module Test

/// <summary>Inline before {includeElement file path} inline after.</summary>
let inlineIncluded (x: int) = x
"""
