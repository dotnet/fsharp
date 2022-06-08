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

    [<Fact>]
    let ``ObsoleteAttribute is not taken into account when used on a type.`` () =
        Fsx """
open System

[<Obsolete("Foo", true)>]
type Foo() =
  
    member _.Bar() = ()
        """
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ObsoleteAttribute is taken into account when used on member type.`` () =
        Fsx """
open System

type Foo() =
    [<Obsolete("Foo", true)>]
    member _.Bar() = ()

let foo = Foo()
foo.Bar()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withErrorCode 101
        |> withErrorMessage "This construct is deprecated. Foo"

    [<Fact>]
    let ``ObsoleteAttribute is taken into account when used on type when invoking member`` () =
        Fsx """
open System

[<Obsolete("Foo", true)>]
type Foo() =
    member _.Bar() = ()

let foo = Foo()
foo.Bar()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withErrorCodes [ 101; 101]
        |> withErrorMessages [ "This construct is deprecated. Foo"; "This construct is deprecated. Foo"]