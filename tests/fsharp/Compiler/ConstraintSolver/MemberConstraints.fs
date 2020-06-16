// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module MemberConstraints =

    [<Test>]
    let ``we can overload operators on a type and not add all the extra jazz such as inlining and the ^ operator.``() = 
        CompilerAssert.CompileExeAndRun
            """
type Foo(x : int) = 
    member this.Val = x
    
    static member (-->) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (-->) ((src : Foo), (target : int)) = new Foo(src.Val + target)
    
    static member (+) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (+) ((src : Foo), (target : int)) = new Foo(src.Val + target)
    
let x = Foo(3) --> 4
let y = Foo(3) --> Foo(4)
let x2 = Foo(3) + 4
let y2 = Foo(3) + Foo(4)

if x.Val <> 7 then failwith "x.Val <> 7"
elif y.Val <> 7 then  failwith "y.Val <> 7"
elif x2.Val <> 7 then  failwith "x2.Val <> 7"
elif y2.Val <> 7 then  failwith "x.Val <> 7"
else ()
            """

    [<Test>]
    let ``Invalid member constraint with ErrorRanges``() = // Regression test for FSharp1.0:2262
        CompilerAssert.TypeCheckSingleErrorWithOptions
            [| "--test:ErrorRanges" |]
            """
let inline length (x: ^a) : int = (^a : (member Length : int with get, set) (x, ()))
            """
            FSharpErrorSeverity.Error
            697
            (2, 42, 2, 75)
            "Invalid constraint"
