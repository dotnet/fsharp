// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module AnonRecordsTests =

    [<Test>]
    let NotStructConstraintPass() =
        CompilerAssert.Pass
            """
type RefClass<'a when 'a : not struct>() = class end
let rAnon = RefClass<{| R: int |}>()
            """

    [<Test>]
    let StructConstraintPass() =
        CompilerAssert.Pass
            """
type StructClass<'a when 'a : struct>() = class end
let sAnon = StructClass<struct {| S: int |}>()
            """

    [<Test>]
    let NotStructConstraintFail() =
        CompilerAssert.TypeCheckSingleError
            """
type RefClass<'a when 'a : not struct>() = class end
let rAnon = RefClass<struct {| R: int |}>()
            """ 
            1
            (3, 12, 3, 41)
            "A generic construct requires that the type 'struct {|R : int|}' have reference semantics, but it does not, i.e. it is a struct"

    [<Test>]
    let StructConstraintFail() =
        CompilerAssert.TypeCheckSingleError
            """
type StructClass<'a when 'a : struct>() = class end
let sAnon = StructClass<{| S: int |}>()
            """ 
            1
            (3, 12, 3, 37)
            "A generic construct requires that the type '{|S : int|}' is a CLI or F# struct type"