// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module PrimitiveConstraints =

    [<Test>]
    let ``Test primitive : constraints``() =
        CompilerAssert.CompileExeAndRun
            """
#light

type Foo(x : int) =
    member   this.Value      = x
    override this.ToString() = "Foo"

type Bar(x : int) =
    inherit Foo(-1)
    member   this.Value2     = x
    override this.ToString() = "Bar"

let test1 (x : Foo) = x.Value
let test2 (x : Bar) = (x.Value, x.Value2)

let f = new Foo(128)
let b = new Bar(256)

if test1 f <> 128       then failwith "test1 f <> 128"
elif test2 b <> (-1, 256) then failwith "test2 b <> (-1, 256)"
else ()
"""

    [<Test>]
    let ``Test primitive :> constraints``() =
        CompilerAssert.CompileExeAndRun
            """
#light
type Foo(x : int) =
    member   this.Value      = x
    override this.ToString() = "Foo"

type Bar(x : int) =
    inherit Foo(-1)
    member   this.Value2     = x
    override this.ToString() = "Bar"

type Ram(x : int) =
    inherit Foo(10)
    member   this.ValueA     = x
    override this.ToString() = "Ram"

let test (x : Foo) = (x.Value, x.ToString())

let f = new Foo(128)
let b = new Bar(256)
let r = new Ram(314)

if test f <> (128, "Foo") then failwith "test f <> (128, 'Foo')"
elif test b <> (-1, "Bar") then failwith "test b <> (-1, 'Bar')"
elif test r <> (10, "Ram") then failwith "test r <> (10, 'Ram')"
else ()
"""

    [<Test>]
    let ``Test primitive : null constraint``() =
        CompilerAssert.CompileExeAndRun
            """
let inline isNull<'a when 'a : null> (x : 'a) =
    match x with
    | null -> "is null"
    | _    -> (x :> obj).ToString()

let runTest =
    // Wrapping in try block to work around FSB 1989
    try
        if isNull null <> "is null" then failwith "isNull null <> is null"
        if isNull "F#" <> "F#"      then  failwith "isNull F# <> F#"
        ()
    with _ -> reraise()

runTest
"""

    [<Test>]
    /// Title: Type checking oddity
    ///
    /// This suggestion was resolved as by design,
    /// so the test makes sure, we're emitting error message about 'not being a valid object construction expression'
    let ``Invalid object constructor``() = // Regression test for FSharp1.0:4189
        CompilerAssert.TypeCheckWithErrorsAndOptionsAgainstBaseLine [| "--test:ErrorRanges" |] (__SOURCE_DIRECTORY__ ++ "../../") "typecheck/constructors/neg_invalid_constructor.fs"
