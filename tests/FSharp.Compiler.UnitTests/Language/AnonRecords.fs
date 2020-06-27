// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module AnonRecords =

    [<Test>]
    let NotStructConstraintPass() =
        Compiler.AssertPass
            """
type RefClass<'a when 'a : not struct>() = class end
let rAnon = RefClass<{| R: int |}>()
            """

    [<Test>]
    let StructConstraintPass() =
        Compiler.AssertPass
            """
type StructClass<'a when 'a : struct>() = class end
let sAnon = StructClass<struct {| S: int |}>()
            """

    [<Test>]
    let NotStructConstraintFail() =
        Compiler.AssertSingleErrorTypeCheck 
            """
    type RefClass<'a when 'a : not struct>() = class end
    let rAnon = RefClass<struct {| R: int |}>()
            """ 
            1
            (3, 16, 3, 45)
            "A generic construct requires that the type 'struct {|R : int|}' have reference semantics, but it does not, i.e. it is a struct"

    [<Test>]
    let StructConstraintFail() =
        Compiler.AssertSingleErrorTypeCheck 
            """
type StructClass<'a when 'a : struct>() = class end
let sAnon = StructClass<{| S: int |}>()
            """ 
            1
            (3, 12, 3, 37)
            "A generic construct requires that the type '{|S : int|}' is a CLI or F# struct type"
        
