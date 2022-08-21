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
    
#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``Regression: typechecker does not fail when attribute is on type variable (https://github.com/dotnet/fsharp/issues/13525)`` () =
        let csharpBaseClass = 
            CSharp """
        using System.Diagnostics.CodeAnalysis;
        
        namespace CSharp
        {
        
            public interface ITreeNode
            {
            }
        
            public static class Extensions
            {
                public static TNode Copy<TNode>([NotNull] this TNode node, ITreeNode context1 = null) where TNode : ITreeNode =>
                    node;
            }
        }""" |> withName "csLib"
        
        let fsharpSource =
            """
    module FooBar
    open type CSharp.Extensions
    
    let replaceWithCopy oldChild newChild =
        let newChildCopy = newChild.Copy()
        ignore newChildCopy
    """
        FSharp fsharpSource
        |> withLangVersion70
        |> withReferences [csharpBaseClass]
        |> compile
        |> shouldSucceed
