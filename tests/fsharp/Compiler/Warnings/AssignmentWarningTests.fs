// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Warnings assigning to mutable and immutable objects`` =

    [<Test>]
    let ``Unused compare with immutable when assignment might be intended``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 10
let y = "hello"

let changeX() =
    x = 20
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (6, 5, 6, 11)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to mutate a value, then mark the value 'mutable' and use the '<-' operator e.g. 'x <- expression'."

    [<Test>]
    let ``Unused compare with mutable when assignment might be intended``() =
        CompilerAssert.TypeCheckSingleError
            """
let mutable x = 10
let y = "hello"

let changeX() =
    x = 20
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (6, 5, 6, 11)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to mutate a value, then use the '<-' operator e.g. 'x <- expression'."

    [<Test>]
    let ``Unused comparison of property in dotnet object when assignment might be intended``() =
        CompilerAssert.TypeCheckSingleError
            """
open System

let z = new System.Timers.Timer()
let y = "hello"

let changeProperty() =
    z.Enabled = true
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (8, 5, 8, 21)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to set a value to a property, then use the '<-' operator e.g. 'z.Enabled <- expression'."

    [<Test>]
    let ``Unused comparison of property when assignment might be intended ``() =
        CompilerAssert.TypeCheckSingleError
            """
type MyClass(property1 : int) =
    member val Property1 = property1
    member val Property2 = "" with get, set
    
let x = MyClass(1)
let y = "hello"

let changeProperty() =
    x.Property2 = "20"
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (10, 5, 10, 23)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to set a value to a property, then use the '<-' operator e.g. 'x.Property2 <- expression'."

    [<Test>]
    let ``Don't warn if assignment to property without setter ``() =
        CompilerAssert.TypeCheckSingleError
            """
type MyClass(property1 : int) =
    member val Property2 = "" with get
    
let x = MyClass(1)
let y = "hello"

let changeProperty() =
    x.Property2 = "22"
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (9, 5, 9, 23)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'."
