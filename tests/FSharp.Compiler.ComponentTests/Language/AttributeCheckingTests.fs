// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.AttributeChecking

open Xunit
open FSharp.Test.Compiler

module AttributeCheckingTests =

    [<Fact>]
    let ``attributes check inherited AllowMultiple`` () =
        Fsx """
open System

[<AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)>]
type HttpMethodAttribute() = inherit Attribute()
type HttpGetAttribute() = inherit HttpMethodAttribute()

[<HttpGet; HttpGet>] // this shouldn't error like 
[<HttpMethod; HttpMethod>] // this doesn't
type C() =
    member _.M() = ()
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``AllowMultiple=false allows adding attribute to both property and getter/setter`` () =
        Fsx """
open System

[<AttributeUsage(AttributeTargets.Property ||| AttributeTargets.Method, AllowMultiple = false)>]
type FooAttribute() = inherit Attribute()

type C() =
    [<Foo>]
    member _.Foo
        with [<Foo>] get () = "bar"
         and [<Foo>] set (v: string) = ()
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed