// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// verifies that all translations in all .xlf files have `state="translated"`.

#r "System.Xml.Linq"

open System
open System.IO
open System.Xml.Linq

// usage: fsi VerifyAllTranslations.fsx -- baseDirectory
let baseDirectory =
    Environment.GetCommandLineArgs()
    |> Seq.skipWhile ((<>) "--")
    |> Seq.skip 1
    |> Seq.head

let hasUntranslatedStrings (xlfFile:string) =
    let doc = XDocument.Load(xlfFile)
    let untranslatedStates =
        doc.Root.Descendants()
        |> Seq.filter (fun (elem:XElement) -> elem.Name.LocalName = "target")
        |> Seq.map (fun (elem:XElement) -> elem.Attribute(XName.op_Implicit("state")))
        |> Seq.filter (isNull >> not)
        |> Seq.map (fun (attr:XAttribute) -> attr.Value)
        |> Seq.filter ((<>) "translated")
    Seq.length untranslatedStates > 0

let filesWithMissingTranslations =
    Directory.EnumerateFiles(baseDirectory, "*.xlf", SearchOption.AllDirectories)
    |> Seq.filter hasUntranslatedStrings
    |> Seq.toList

match filesWithMissingTranslations with
| [] -> printfn "All .xlf files have translations assigned."
| _ ->
    printfn "The following .xlf files have untranslated strings (state != 'translated'):\n\t%s" (String.Join("\n\t", filesWithMissingTranslations))
    Environment.Exit(1)
