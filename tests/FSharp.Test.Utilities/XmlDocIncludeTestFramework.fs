// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open System
open System.IO
open System.Security
open System.Xml.Linq
open TestFramework
open FSharp.Test.Compiler

module XmlDocIncludeTestFramework =

    type IncludeScenario =
        {
            Source: string
            Files: (string * string) list
            WarnOn: int list
        }

    let scenario source files =
        {
            Source = source
            Files = files
            WarnOn = []
        }

    let withParamChecking includeScenario =
        if includeScenario.WarnOn |> List.contains 3390 then
            includeScenario
        else
            { includeScenario with WarnOn = includeScenario.WarnOn @ [ 3390 ] }

    let private fullPathForRelativeFile (directory: DirectoryInfo) (relativePath: string) =
        if String.IsNullOrWhiteSpace relativePath then
            invalidArg (nameof relativePath) "Include test file paths must be non-empty relative paths."

        if Path.IsPathRooted relativePath then
            invalidArg (nameof relativePath) $"Include test file path must be relative: {relativePath}"

        let basePath = Path.GetFullPath(directory.FullName)
        let fullPath = Path.GetFullPath(Path.Combine(basePath, relativePath))
        let basePathWithSeparator =
            basePath.TrimEnd([| Path.DirectorySeparatorChar; Path.AltDirectorySeparatorChar |]) + string Path.DirectorySeparatorChar

        if not (fullPath.StartsWith(basePathWithSeparator, StringComparison.Ordinal)) then
            invalidArg (nameof relativePath) $"Include test file path must stay under the scenario directory: {relativePath}"

        fullPath

    let private writeScenarioFile directory (relativePath, contents: string) =
        let path = fullPathForRelativeFile directory relativePath
        let parent = Path.GetDirectoryName path

        if not (String.IsNullOrEmpty parent) then
            Directory.CreateDirectory parent |> ignore

        File.WriteAllText(path, contents)

    let private addWarnOnCodes warnOnCodes compilationUnit =
        (compilationUnit, warnOnCodes)
        ||> List.fold (fun current warning -> current |> withWarnOn warning)

    let runInclude includeScenario =
        let directory = createTemporaryDirectory ()

        for file in includeScenario.Files do
            writeScenarioFile directory file

        let sourcePath = Path.Combine(directory.FullName, "Library.fs")
        let xmlPath = Path.Combine(directory.FullName, "Library.xml")
        File.WriteAllText(sourcePath, includeScenario.Source)

        let result =
            FsFromPath sourcePath
            |> withOutputDirectory (Some directory)
            |> withOptions [ $"--doc:{xmlPath}" ]
            |> addWarnOnCodes includeScenario.WarnOn
            |> compile

        let xml =
            if File.Exists xmlPath then
                File.ReadAllText xmlPath
            else
                ""

        xml, result

    let private memberElementName = XName.Get "member"
    let private nameAttributeName = XName.Get "name"

    let memberInner memberName xml =
        let document =
            try
                XDocument.Parse xml
            with ex ->
                failwith $"Could not parse XML documentation output: {ex.Message}\nFull XML:\n{xml}"

        let matchingMember =
            document.Descendants memberElementName
            |> Seq.tryFind (fun element ->
                let nameAttribute = element.Attribute nameAttributeName
                not (isNull nameAttribute) && nameAttribute.Value = memberName)

        match matchingMember with
        | Some element ->
            element.Nodes()
            |> Seq.map (fun node -> node.ToString(SaveOptions.DisableFormatting))
            |> String.concat ""
        | None -> failwith $"Could not find XML documentation member '{memberName}'.\nFull XML:\n{xml}"

    let private canonicalizeInnerXml fragment =
        try
            XElement.Parse("<r>" + fragment + "</r>").ToString(SaveOptions.DisableFormatting)
        with ex ->
            failwith $"Could not parse XML documentation fragment: {ex.Message}\nFragment:\n{fragment}"

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

        let private xmlAttribute value =
            SecurityElement.Escape value

        let private includeFileAttribute (file: string) =
            file.Replace("\\", "/") |> xmlAttribute

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

        let nestedA fileB =
            $"""<?xml version="1.0"?>
<data>
  <summary>Nested A start. <include file="{includeFileAttribute fileB}" path="/data/part"/> Nested A end.</summary>
</data>"""

        let nestedB =
            """<?xml version="1.0"?>
<data>
  <part>Nested B content.</part>
</data>"""

        let selfCycle selfFile =
            $"""<?xml version="1.0"?>
<data>
  <summary>Self cycle start. <include file="{includeFileAttribute selfFile}" path="/data/summary"/> Self cycle end.</summary>
</data>"""

        let memberWithInclude file path =
            $"""module Test

/// <include file="{includeFileAttribute file}" path="{xmlAttribute path}"/>
let included (x: int) (y: int) = x + y
"""

        let memberInlineInclude file path =
            $"""module Test

/// <summary>Inline before <include file="{includeFileAttribute file}" path="{xmlAttribute path}"/> inline after.</summary>
let inlineIncluded (x: int) = x
"""
