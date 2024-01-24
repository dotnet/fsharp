(** ---
category: Release Notes
categoryindex: 600
index: 4
title: FSharp.Compiler.Service
---

# FSharp.Compiler.Service
*)
(*** hide ***)
#load "./.aux/Common.fsx"

open System.IO
open System.Xml.XPath
open Markdig
open Common

let path = Path.Combine(__SOURCE_DIRECTORY__, ".FSharp.Compiler.Service")
let fcsMajorVersion = versionPropsDoc.XPathSelectElement("//FCSMajorVersion").Value
let nugetPackage = "FSharp.Compiler.Service"
let availableNuGetVersions = getAvailableNuGetVersions nugetPackage

processFolder path (fun file ->
    let versionInFileName = Path.GetFileNameWithoutExtension(file)
    // Example: 8.0.200
    let versionParts = versionInFileName.Split '.'

    let version = $"%s{fcsMajorVersion}.%s{versionParts.[0]}.%s{versionParts.[2]}"
    // TODO: Can we determine if the current version is in code freeze based on the Version.props info?
    let title =
        if not (availableNuGetVersions.Contains version) then
            $"%s{version} - Unreleased"
        else
            match tryGetReleaseDate nugetPackage version with
            | None -> $"%s{version} - Unreleased"
            | Some d -> $"%s{version} - %s{d}"

    let nugetBadge =
        if not (availableNuGetVersions.Contains version) then
            System.String.Empty
        else
            $"<a href=\"https://www.nuget.org/packages/%s{nugetPackage}/%s{version}\" target=\"_blank\"><img alt=\"Nuget\" src=\"https://img.shields.io/badge/NuGet-%s{version}-blue\"></a>"

    let content = File.ReadAllText file |> Markdown.ToHtml |> transformH3 version

    $"""<h2><a name="%s{version}" class="anchor" href="#%s{version}">%s{title}</a></h2>%s{nugetBadge}%s{content}""")
(*** include-it-raw ***)
