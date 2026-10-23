(** ---
category: Release Notes
categoryindex: 600
index: 2
title: F# Language
---

# F\# Language
*)
(*** hide ***)
#load "./.aux/Common.fsx"

open System.IO
open Markdig
open Common

let path = Path.Combine(__SOURCE_DIRECTORY__, ".Language")

processFolder path (fun file ->
    let version = Path.GetFileNameWithoutExtension(file)
    let version = if version = "preview" then "Preview" else version
    let content = File.ReadAllText file |> Markdown.ToHtml |> transformH3 version
    $"""<h2><a name="%s{version}" class="anchor" href="#%s{version}">%s{version}</a></h2>%s{content}""")
(*** include-it-raw ***)
