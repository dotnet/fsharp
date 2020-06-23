// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

module ``Else branch is missing`` =

    [<Fact>]
    let ``Fail if else branch is missing``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 10
let y =
   if x > 10 then "test"
            """
            FSharpErrorSeverity.Error
            1
            (4, 19, 4, 25)
            "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'string'."

    [<Fact>]
    let ``Fail on type error in condition``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 10
let y =
   if x > 10 then 
     if x <> "test" then printfn "test"
     ()
            """
            FSharpErrorSeverity.Error
            1
            (5, 14, 5, 20)
            "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'    "

    [<Fact>]
    let ``Fail if else branch is missing in nesting``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 10
let y =
   if x > 10 then ("test")
            """
            FSharpErrorSeverity.Error
            1
            (4, 20, 4, 26)
            "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'string'."
