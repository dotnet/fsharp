namespace FSharp.Compiler.ComponentTests.Attributes

open Xunit
open FSharp.Test.Compiler

module GenericAttributeAbbreviations =

    // Repro from https://github.com/dotnet/fsharp/issues/7877.
    // A type abbreviation of a generic attribute type must not crash with
    // FS0193 "The lists had different lengths" - it must report FS3891.
    [<Fact>]
    let ``Type abbreviation of generic attribute reports FS3891 instead of crashing`` () =
        Fsx """
type A<'T>() = inherit System.Attribute()
type B = A<int>
[<B>] type C = class end
"""
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 3891, Line 4, Col 3, Line 4, Col 4, "Generic attribute types are not supported in F#. The type 'A' has type parameters and cannot be used as an attribute.")
        |> ignore

    [<Theory>]
    [<InlineData("type B = A<int>")>]
    [<InlineData("type B = A<string>")>]
    [<InlineData("type B = A<int list>")>]
    [<InlineData("type B = A<System.Collections.Generic.List<int>>")>]
    let ``Generic attribute abbreviation variants all report FS3891`` (abbrev: string) =
        Fsx (sprintf """
type A<'T>() = inherit System.Attribute()
%s
[<B>] type C = class end
""" abbrev)
        |> compile
        |> shouldFail
        |> withErrorCode 3891
        |> ignore

    [<Fact>]
    let ``Two-parameter generic attribute abbreviation reports FS3891`` () =
        Fsx """
type A2<'T, 'U>() = inherit System.Attribute()
type B = A2<int, string>
[<B>] type C = class end
"""
        |> compile
        |> shouldFail
        |> withErrorCode 3891
        |> ignore

    [<Fact>]
    let ``Chained abbreviation through a generic attribute reports FS3891`` () =
        Fsx """
type A<'T>() = inherit System.Attribute()
type B = A<int>
type C2 = B
[<C2>] type D = class end
"""
        |> compile
        |> shouldFail
        |> withErrorCode 3891
        |> ignore

    // Non-regression: a non-generic attribute abbreviation must still compile.
    [<Fact>]
    let ``Non-generic attribute abbreviation is unchanged`` () =
        Fsx """
type A() = inherit System.Attribute()
type B = A
[<B>] type C = class end
"""
        |> compile
        |> shouldSucceed
        |> ignore

    // Non-regression: built-in attribute abbreviated and used should compile.
    [<Fact>]
    let ``Abbreviation of non-generic System attribute compiles`` () =
        Fsx """
type MyObsolete = System.ObsoleteAttribute
[<MyObsolete("use something else")>]
let foo () = ()
"""
        |> compile
        |> shouldSucceed
        |> ignore

    // Non-regression: the direct `[<A<int>>]` syntax is rejected by the parser,
    // not by the new check. Behavior here must not change.
    [<Fact>]
    let ``Direct generic attribute syntax remains a parse-level rejection`` () =
        Fsx """
type A<'T>() = inherit System.Attribute()
[<A<int>>] type C = class end
"""
        |> compile
        |> shouldFail
        |> ignore
