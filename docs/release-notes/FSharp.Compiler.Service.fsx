(** ---
category: Release Notes
categoryindex: 600
index: 4
title: FSharp.Compiler.Service
---

# FSharp.Compiler.Service
*)
(*** hide ***)
#r "nuget: Markdig, 0.33.0"
#r "nuget: FsHttp, 12.1.0"

open System.IO
open System.Xml.Linq
open System.Xml.XPath
open System.Text.RegularExpressions
open Markdig
open FsHttp

let path = Path.Combine(__SOURCE_DIRECTORY__, ".FSharp.Compiler.Service")
let versionProps = Path.Combine(__SOURCE_DIRECTORY__, "../../eng/Versions.props")
let versionPropsDoc = XDocument.Load(versionProps)
let fcsMajorVersion = versionPropsDoc.XPathSelectElement("//FCSMajorVersion").Value

// Find all published version on NuGet
let availableNugetVersions: Set<string> =
    http { GET $"https://api.nuget.org/v3-flatcontainer/fsharp.compiler.service/index.json" }
    |> Request.send
    |> Response.deserializeJson<{| versions: string array |}>
    |> fun json -> Set.ofArray json.versions

/// Try and find the publish date on NuGet
let tryGetReleaseDate version =
    http { GET $"https://api.nuget.org/v3/registration5-gz-semver2/fsharp.compiler.service/%s{version}.json" }
    |> Request.send
    |> Response.deserializeJson<{| published: string |}>
    |> fun json ->
        if System.String.IsNullOrWhiteSpace json.published then
            None
        else
            Some(json.published.Split('T').[0])

/// In order for the heading to appear in the page content menu in fsdocs,
/// they need to follow a specific HTML structure.
let transformH3 (version: string) (input: string) =
    let pattern = "<h3>(.*?)</h3>"

    let replacement =
        $"<h3><a name=\"%s{version}-$1\" class=\"anchor\" href=\"#%s{version}-$1\">$1</a></h3>"

    Regex.Replace(input, pattern, replacement)

Directory.EnumerateFiles(path, "*.md")
|> Seq.sortByDescending Path.GetFileNameWithoutExtension
|> Seq.map (fun file ->
    let versionInFileName = Path.GetFileNameWithoutExtension(file)
    // Example: 8.0.200
    let versionParts = versionInFileName.Split '.'

    let version = $"%s{fcsMajorVersion}.%s{versionParts.[0]}.%s{versionParts.[2]}"
    // TODO: Can we determine if the current version is in code freeze based on the Version.props info?
    let title =
        if not (availableNugetVersions.Contains version) then
            $"%s{version} - Unreleased"
        else
            match tryGetReleaseDate version with
            | None -> $"%s{version} - Unreleased"
            | Some d -> $"%s{version} - %s{d}"

    let nugetBadge =
        if not (availableNugetVersions.Contains version) then
            System.String.Empty
        else
            $"<a href=\"https://www.nuget.org/packages/FSharp.Compiler.Service/%s{version}\" target=\"_blank\"><img alt=\"Nuget\" src=\"https://img.shields.io/badge/NuGet-%s{version}-blue\"></a>"

    let content = File.ReadAllText file |> Markdown.ToHtml |> transformH3 version

    $"""<h2><a name="%s{version}" class="anchor" href="#%s{version}">%s{title}</a></h2>%s{nugetBadge}%s{content}""")
|> String.concat "\n"
(*** include-it-raw ***)
