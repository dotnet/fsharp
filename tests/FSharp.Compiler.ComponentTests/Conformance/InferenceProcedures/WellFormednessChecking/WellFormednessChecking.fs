// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module WellFormednessChecking =

    let withCSharpBaseDerived compilation =
        let csharpLib =
            CSharp """
// Regression test for FSHARP1.0:6123
public abstract class Base
{
    public virtual int Foo { get { return 12; } }
}

public abstract class Derived : Base
{
    public abstract new int Foo { get; }
}
"""
            |> withName "CSharpBaseDerived"
        compilation
        |> withReferences [csharpLib]

    // SOURCE=E_NonAbstractClassNotImplAllInheritedAbstractSlots01.fs SCFLAGS="-r:CSharpBaseDerived.dll --test:ErrorRanges" PRECMD="\$CSC_PIPE /t:library CSharpBaseDerived.cs"
    // Skipped: C# interop test not working correctly in migrated test - needs investigation
    [<Theory(Skip = "C# interop test needs investigation"); FileInlineData("E_NonAbstractClassNotImplAllInheritedAbstractSlots01.fs")>]
    let ``E_NonAbstractClassNotImplAllInheritedAbstractSlots01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> withCSharpBaseDerived
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 365, Line 6, Col 6, Line 6, Col 15, "No implementation was given for 'Derived.get_Foo() : int'")
            (Error 54, Line 6, Col 6, Line 6, Col 15, "Non-abstract classes cannot contain abstract members. Either provide a default member implementation or add the '[<AbstractClass>]' attribute to your type.")
        ]

    // SOURCE=E_Clashing_Methods_in_Interface01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_Clashing_Methods_in_Interface01.fs")>]
    let ``E_Clashing_Methods_in_Interface01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 438
        |> ignore

    // SOURCE=E_Clashing_Methods_in_Interface02.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_Clashing_Methods_in_Interface02.fs")>]
    let ``E_Clashing_Methods_in_Interface02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 438
        |> ignore

    // SOURCE=E_Clashing_Record_Field_and_Member01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_Clashing_Record_Field_and_Member01.fs")>]
    let ``E_Clashing_Record_Field_and_Member01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 23
        |> ignore

    // SOURCE=E_Clashing_Values_in_AbstractClass02.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_Clashing_Values_in_AbstractClass02.fs")>]
    let ``E_Clashing_Values_in_AbstractClass02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 438
        |> ignore

    // SOURCE=E_Override_with_Incorrect_Type01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_Override_with_Incorrect_Type01.fs")>]
    let ``E_Override_with_Incorrect_Type01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 442
        |> ignore
