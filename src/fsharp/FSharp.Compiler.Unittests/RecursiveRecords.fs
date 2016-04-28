// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Compiler.Unittests.RecursiveRecords

open System
open System.Text

open NUnit.Framework

open Microsoft.FSharp.Compiler

type TypeDef = {
    Name: string
    mutable Fields: FieldDef list
}
and FieldDef = {
    Name: string
    Type: TypeDef
}

let Int = { Name = "Int"; Fields = [] }
let Complex = { Name = "Complex"; Fields = [{ Name = "X"; Type = Int}] }
Complex.Fields <- { Name = "Deep"; Type = Complex } :: Complex.Fields

[<Test>]
let ``recursive structure with same record reference should not satck overflow`` () =
    Assert.AreEqual (Complex,Complex)
