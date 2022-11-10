namespace ParallelTypeCheckingTests

open FSharp.Compiler.Service.Tests.Common
open NUnit.Framework
open ParallelTypeCheckingTests.ASTVisit
open ParallelTypeCheckingTests.TopModulesExtraction

[<AutoOpen>]
module private Helpers =
    let parse name code = parseSourceCode (name, code)

module TestRefs =
    let makeModuleRef ids =
        ReferenceOrAbbreviation.Reference
            {
                Ident = ids |> Seq.toArray
                Kind = ReferenceKind.ModuleOrNamespace
            }

    let makeTypeRef ids =
        ReferenceOrAbbreviation.Reference
            {
                Ident = ids |> Seq.toArray
                Kind = ReferenceKind.Type
            }

    let typeAbbr = ReferenceOrAbbreviation.Abbreviation Abbreviation.TypeAbbreviation

    let moduleAbbr =
        ReferenceOrAbbreviation.Abbreviation Abbreviation.ModuleAbbreviation

    [<Test>]
    let ``Simple`` () =

        let A =
            """
module A
open B
let x = C.x
"""

        let parsed = parseSourceCode ("A.fs", A)
        let refs = findModuleAndTypeRefs parsed
        let expected = [| makeModuleRef [ "B" ]; makeTypeRef [ "C"; "x" ] |]
        Assert.That(refs, Is.EqualTo expected)

    [<Test>]
    let ``No duplicate refs`` () =
        let A =
            """
module A
open B
open B
"""
            |> parse "A.fs"

        let refs = findModuleAndTypeRefs A
        Assert.That(refs, Is.EqualTo([| makeModuleRef [ "B" ] |]))

    [<Test>]
    let ``Big example`` () =
        let parseResults =
            parse
                "A.fs"
                """
module A1 = let a = 3
module A2 = let a = 3
module A3 = let a = 3
module A4 =
    
    type AAttribute(name : string) =
        inherit System.Attribute()
    
    let a = 3
    module A1 =
        let a = 3
    type X = int * int
    type Y = Y of int

module B =
    open A2
    let b = [|
        A1.a
        A2.a
        A3.a
    |]
    let c : A4.X = 2,2
    [<A4.A("name")>]
    let d : A4.Y = A4.Y 2
    type Z =
        {
            X : A4.X
            Y : A4.Y
        }

module LetBindings =
    let c = A4.a
    let d = A4.A1.a
    open A4
    let e = A1.a
    open A1
    let f = a
"""

        let refs = findModuleAndTypeRefs parseResults

        let expected =
            [|
                makeTypeRef [ "string" ]
                makeTypeRef [ "System"; "Attribute" ]
                makeTypeRef [ "int" ]
                typeAbbr // type X = ...
                makeModuleRef [ "A2" ]
                makeTypeRef [ "A1"; "a" ]
                makeTypeRef [ "A2"; "a" ]
                makeTypeRef [ "A3"; "a" ]
                makeTypeRef [ "A4"; "X" ]
                makeTypeRef [ "A4"; "A" ]
                makeTypeRef [ "A4"; "Y" ]
                makeTypeRef [ "A4"; "a" ]
                makeTypeRef [ "A4"; "A1"; "a" ]
                makeModuleRef [ "A4" ]
                makeModuleRef [ "A1" ]
            |]

        Assert.That(refs, Is.EqualTo expected)

module TestTopItems =

    [<Test>]
    let ``No duplicates returned`` () =
        let A =
            parse
                "A.fs"
                """
namespace A
let x = 3
namespace A
let y = 4
"""

        let tops = topModuleOrNamespaces A
        Assert.That(tops, Is.EqualTo [| [| "A" |] |])

    [<Test>]
    let ``Global namespace is equivalent to a namespace with a root ID`` () =
        let A =
            parse
                "A.fs"
                """
namespace global
"""

        let tops = topModuleOrNamespaces A
        Assert.That(tops, Is.EqualTo [| ([||]: SimpleId) |])

    [<Test>]
    let ``Top-level namespaces and modules are treated the same way`` () =
        let A =
            parse
                "A.fs"
                """
module A
let x = 3
"""

        let B =
            parse
                "A.fs"
                """
namespace A
let x = 3
"""

        let topA, topB = topModuleOrNamespaces A, topModuleOrNamespaces B
        Assert.That(topA, Is.EqualTo [| [| "A" |] |])
        Assert.That(topB, Is.EqualTo [| [| "A" |] |])

    [<Test>]
    let ``Nested modules/namespaces are not considered and all top-level items are returned`` () =
        let A =
            parse
                "A.fs"
                """
namespace A
module B =
    let x = 3
"""

        let top = topModuleOrNamespaces A
        Assert.That(top, Is.EqualTo [| [| "A" |] |])

    [<Test>]
    let ``Big example`` () =
        let A =
            parse
                "A.fs"
                """
namespace A
let x = 3

namespace B
type X = int * int

namespace C
module A =
    let x = 3

namespace D
[<AutoOpen>]
module D1 =
    module D2 =
        let x = 3
    
namespace D
module D1 =
    module D2 =
        let x = 3
"""

        let top = topModuleOrNamespaces A
        let expected = [| [| "A" |]; [| "B" |]; [| "C" |]; [| "D" |] |]
        Assert.That(top, Is.EqualTo expected)
