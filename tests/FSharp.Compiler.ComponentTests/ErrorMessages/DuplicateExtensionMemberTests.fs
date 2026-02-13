// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Duplicate Extension Members`` =

    [<Fact>]
    let ``Same type name from different namespaces should error for static members``() =
        // Static extension members on types with same simple name but different namespaces
        // produce duplicate IL method signatures because the extended type's namespace is not
        // encoded in the IL method name/signature for static extensions.
        FSharp """
namespace NS1

type Task = class end

namespace NS2

type Task = class end

namespace NS3

module Extensions =
    type NS1.Task with
        static member Foo() = 1
    
    type NS2.Task with
        static member Bar() = 2
        """
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Extension members extending types with the same simple name 'Task'"

    [<Fact>]
    let ``Same type name from different namespaces should be allowed for instance members``() =
        // Instance extension members are safe because the extended type becomes the first
        // parameter in IL, differentiating the signatures.
        FSharp """
namespace NS1

type Task = class end

namespace NS2

type Task = class end

namespace NS3

module Extensions =
    type NS1.Task with
        member x.Foo() = 1
    
    type NS2.Task with
        member x.Bar() = 2
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Generic and non-generic types with same base name should be allowed``() =
        // This tests that Expr and Expr<'T> are allowed since they have different LogicalNames
        // Expr -> LogicalName = "Expr"
        // Expr<'T> -> LogicalName = "Expr`1"
        FSharp """
namespace TestNS

type Expr = class end
type Expr<'T> = class end

module Extensions =
    type Expr with
        member x.Foo() = 1
    
    type Expr<'T> with
        member x.Bar() = 2
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Same generic type name from different namespaces should error for static members``() =
        // Same IL collision issue as non-generic, but with generic types.
        FSharp """
namespace NS1

type Container<'T> = class end

namespace NS2

type Container<'T> = class end

namespace NS3

module Extensions =
    type NS1.Container<'T> with
        static member Foo() = 1
    
    type NS2.Container<'T> with
        static member Bar() = 2
        """
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Extension members extending types with the same simple name 'Container`1'"

    [<Fact>]
    let ``Same generic type name from different namespaces should be allowed for instance members``() =
        FSharp """
namespace NS1

type Container<'T> = class end

namespace NS2

type Container<'T> = class end

namespace NS3

module Extensions =
    type NS1.Container<'T> with
        member x.Foo() = 1
    
    type NS2.Container<'T> with
        member x.Bar() = 2
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Extensions on same type should be allowed``() =
        FSharp """
namespace TestNS

type MyType = class end

module Extensions =
    type MyType with
        member x.Foo() = 1
        member x.Bar() = 2
        """
        |> typecheck
        |> shouldSucceed
