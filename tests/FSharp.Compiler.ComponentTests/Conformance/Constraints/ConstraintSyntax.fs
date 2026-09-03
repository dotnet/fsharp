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
