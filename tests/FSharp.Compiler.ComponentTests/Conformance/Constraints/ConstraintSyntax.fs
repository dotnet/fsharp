// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Constraints

open Xunit
open FSharp.Test.Compiler

module ConstraintSyntax =

    // https://github.com/dotnet/fsharp/issues/14580
    [<Fact>]
    let ``Bare 'enum' constraint reports the enum constraint form error`` () =
        Fsx """
type I<'T when 'T : enum> = interface end
        """
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 699, Line 2, Col 21, Line 2, Col 25, "An 'enum' constraint must be of the form 'enum<type>'")

    [<Fact>]
    let ``Unknown identifier constraint reports the identifier without internal markers`` () =
        Fsx """
type I<'T when 'T : notAConstraint> = interface end
        """
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 571, Line 2, Col 21, Line 2, Col 35, "Unexpected identifier: 'notAConstraint'")

    [<Fact>]
    let ``Unknown identifier constraint with type arguments reports the identifier without internal markers`` () =
        Fsx """
type I<'T when 'T : notAConstraint<int>> = interface end
        """
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 571, Line 2, Col 21, Line 2, Col 35, "Unexpected identifier: 'notAConstraint'")

    [<Fact>]
    let ``Unknown identifier before 'null' constraint reports the identifier without internal markers`` () =
        Fsx """
type I<'T when 'T : maybe null> = interface end
        """
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 571, Line 2, Col 21, Line 2, Col 26, "Unexpected identifier: 'maybe'")

    [<Fact>]
    let ``'enum' constraint with an underlying type is accepted`` () =
        Fsx """
type I<'T when 'T : enum<int>> = interface end
type E = A = 1
type Ok = I<E>
        """
        |> typecheck
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/14580
    [<Fact>]
    let ``'enum' constraint on a type of the same recursive group is accepted`` () =
        FSharp """
module rec MyModule

type MyEnum =
    | Alpha = 1
    | Beta = 2

type MyInter<'TEnum when 'TEnum : enum<int>> = interface end

type MyAlias = MyInter<MyEnum>
        """
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``'enum' constraint on a type of the same 'and' group is accepted`` () =
        FSharp """
module MyModule

type MyEnum =
    | Alpha = 1
    | Beta = 2

and MyInter<'TEnum when 'TEnum : enum<int>> = interface end

and MyAlias = MyInter<MyEnum>
        """
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``'enum' constraint on an inherited interface of the same recursive group is accepted`` () =
        FSharp """
module rec MyModule

type MyEnum =
    | Alpha = 1

type MyInter<'TEnum when 'TEnum : enum<int>> = interface end

type C() =
    interface MyInter<MyEnum>
        """
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Mismatched 'enum' constraint on a type of the same recursive group is reported`` () =
        FSharp """
module rec MyModule

type MyEnum =
    | Alpha = 1

type MyInter<'TEnum when 'TEnum : enum<int64>> = interface end

type MyAlias = MyInter<MyEnum>
        """
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "The type 'int64' does not match the type 'int'"

    [<Fact>]
    let ``Mismatched 'enum' constraint outside a recursive group is reported`` () =
        FSharp """
module MyModule

type MyEnum =
    | Alpha = 1

type MyInter<'TEnum when 'TEnum : enum<int64>> = interface end

type MyAlias = MyInter<MyEnum>
        """
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 9, Col 16, Line 9, Col 31, "The type 'int64' does not match the type 'int'")

    [<Fact>]
    let ``Generic code over an 'enum' constraint in a recursive module runs`` () =
        FSharp """
module rec MyModule

type Color =
    | Red = 1
    | Blue = 4

let combine<'T when 'T : enum<int>> (a: 'T) (b: 'T) : 'T =
    LanguagePrimitives.EnumOfValue (LanguagePrimitives.EnumToValue a ||| LanguagePrimitives.EnumToValue b)

[<EntryPoint>]
let main _ =
    if LanguagePrimitives.EnumToValue (combine Color.Red Color.Blue) <> 5 then
        failwith "expected Red ||| Blue to be 5"
    0
        """
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
