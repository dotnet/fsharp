(** ---
category: Release Notes
categoryindex: 600
index: 3
title: FSharp.Core
---

# FSharp.Core
*)
(*** hide ***)
#load "./.aux/Common.fsx"

open System.IO
open Markdig
open Common

let path = Path.Combine(__SOURCE_DIRECTORY__, ".FSharp.Core")
let nugetPackage = "FSharp.Core"
let availableNuGetVersions = getAvailableNuGetVersions nugetPackage

processFolder path (fun file ->
    let version = Path.GetFileNameWithoutExtension(file)

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
