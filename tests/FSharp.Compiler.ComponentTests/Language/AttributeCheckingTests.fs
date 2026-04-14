// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

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
    
    [<FSharp.Test.FactForNETCOREAPP>]
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
        |> withLangVersionPreview
        |> withReferences [csharpBaseClass]
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``C# attribute subclass inherits AllowMultiple true from base`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class BaseAttribute : Attribute { }
            public class ChildAttribute : BaseAttribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

[<Child; Child>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``C# attribute subclass inherits AllowMultiple false from base`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class BaseAttribute : Attribute { }
            public class ChildAttribute : BaseAttribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

[<Child; Child>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 429, Line 4, Col 10, Line 4, Col 15, "The attribute type 'ChildAttribute' has 'AllowMultiple=false'. Multiple instances of this attribute cannot be attached to a single language element.")

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``C# attribute multi-level inheritance inherits AllowMultiple true`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class BaseAttribute : Attribute { }
            public class MiddleAttribute : BaseAttribute { }
            public class LeafAttribute : MiddleAttribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

[<Leaf; Leaf>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``C# attribute subclass with own AttributeUsage overrides base AllowMultiple`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class BaseAttribute : Attribute { }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class ChildAttribute : BaseAttribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

[<Child; Child>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 429, Line 4, Col 10, Line 4, Col 15, "The attribute type 'ChildAttribute' has 'AllowMultiple=false'. Multiple instances of this attribute cannot be attached to a single language element.")

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``F# attribute subclass of C# base inherits AllowMultiple true`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class BaseAttribute : Attribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

type ChildAttribute() = inherit BaseAttribute()

[<Child; Child>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed
