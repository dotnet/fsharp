// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Miscellaneous.XmlDoc

open System.IO
open Xunit
open FSharp.Compiler.Xml
open TestFramework


let memberDoc = "<summary>Summary</summary>"

let xmlFileContents signature = $"""<?xml version="1.0" encoding="utf-8"?>
<doc>
  <assembly>
    <name>FSharp.Core</name>
  </assembly>
  <members>
    <member name="T:Microsoft.FSharp.Collections.list`1">
      <summary>The type of immutable singly-linked lists. </summary>
  </member>
  <member name="{signature}">
    {memberDoc}
  </member>
</members>
</doc>
"""

[<Theory>]
[<InlineData("P:Microsoft.FSharp.Collections.FSharpList`1.Length")>]
[<InlineData("P:Microsoft.FSharp.Collections.FSharpList`1.Length'")>]
let ``Can extract XML docs from a file for a signature`` signature =
    let xmlFileName = tryCreateTemporaryFileName () + ".xml"

    try
        File.WriteAllText(xmlFileName, xmlFileContents signature)

        let docInfo =
            XmlDocumentationInfo.TryCreateFromFile(xmlFileName)
            |> Option.defaultWith (fun () -> failwith "Couldn't create XmlDoc from file")

        match docInfo.TryGetXmlDocBySig(signature) with
        | None -> failwith "Got no doc"
        | Some doc -> Assert.Equal(memberDoc, doc.UnprocessedLines |> String.concat "\n")

    finally
        File.Delete xmlFileName
