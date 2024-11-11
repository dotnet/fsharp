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

Directory.EnumerateFiles(path, "*.md")
|> Seq.sortWith (fun a b ->
    let a = Path.GetFileNameWithoutExtension a
    let b = Path.GetFileNameWithoutExtension b

    match a, b with
    | "preview", "preview" -> 0
    | "preview", _ -> -1
    | _, "preview" -> 1
    | _, _ -> 
        match System.Decimal.TryParse(b), System.Decimal.TryParse(b) with
        | (true, a) , ( true, b) -> compare (int b) (int a)
        | _ -> failwithf "Cannot compare %s with %s" b a
    )
|> Seq.map (fun file ->
    let version = Path.GetFileNameWithoutExtension(file)
    let version = if version = "preview" then "Preview" else version
    let content = File.ReadAllText file |> Markdown.ToHtml |> transformH3 version
    $"""<h2><a name="%s{version}" class="anchor" href="#%s{version}">%s{version}</a></h2>%s{content}""")
|> String.concat "\n"
(*** include-it-raw ***)
