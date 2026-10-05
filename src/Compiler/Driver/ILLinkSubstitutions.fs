// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.ILLinkSubstitutions

open System.Xml.Linq

let names (assemblyName: string) =
    seq {
        for kind in [ "Signature"; "Optimization" ] do
            for suffix in [ "Data"; "CompressedData"; "DataB"; "CompressedDataB"; "Info" ] do
                yield $"FSharp{kind}{suffix}.{assemblyName}"
    }

let document (assemblyName: string) (names: seq<string>) =
    let x = XName.Get
    let assembly = XElement(x "assembly", XAttribute(x "fullname", assemblyName))

    for name in names do
        assembly.Add(XElement(x "resource", XAttribute(x "name", name), XAttribute(x "action", "remove"), ""))

    XElement(x "linker", assembly)
