// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module PropertySetterConstraints =

    let private csLib =
        CSharp
            """
namespace CsLib
{
    public class FieldHolder { public int X; }
    public class StaticHolder { public static int X { get; set; } }
}
"""
        |> withName "CsLib"

    let private typecheckPreview source =
        FSharp source
        |> asLibrary
        |> withLangVersionPreview
        |> typecheck

    [<Fact>]
    let ``Assignment solves setter constraints with properties, record fields and .NET fields`` () =
        FSharp
            """
open CsLib

type PropertyHolder() =
    member val X = 0 with get, set

type RecordHolder = { mutable X: int }

let inline setX (x: 'T when 'T: (member X: int with set)) v = x.X <- v
let inline bump (x: 'T when 'T: (member X: int) and 'T: (member X: int with set)) = x.X <- x.X + 1
let inline setStatic<'T when 'T: (static member X: int with set)> v = 'T.X <- v

let p = PropertyHolder()
let r = { X = 0 }
let f = FieldHolder()
setX p 1
setX r 2
setX f 3
bump p
bump r
bump f
setStatic<StaticHolder> 7
printf "%d %d %d %d" p.X r.X f.X StaticHolder.X
"""
        |> withLangVersionPreview
        |> withReferences [ csLib ]
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> verifyOutput "2 3 4 7"

    [<Fact>]
    let ``Assignment needs a setter constraint`` () =
        typecheckPreview
            """
module M

let inline setX (x: 'T when 'T: (member X: int)) v = x.X <- v
"""
        |> shouldFail
        |> withSingleDiagnostic (Error 810, Line 4, Col 54, Line 4, Col 57, "Property 'X' cannot be set")

    [<Fact>]
    let ``A setter constraint alone does not make the property readable`` () =
        typecheckPreview
            """
module M

let inline getX (x: 'T when 'T: (member X: int with set)) : int = x.X
"""
        |> shouldFail
        |> withSingleDiagnostic (Error 807, Line 4, Col 67, Line 4, Col 70, "Property 'X' is not readable")

    [<Fact>]
    let ``Assignment to a setter constraint needs the preview language version`` () =
        FSharp
            """
module M

let inline setX (x: 'T when 'T: (member X: int with set)) v = x.X <- v
"""
        |> asLibrary
        |> withLangVersion10
        |> typecheck
        |> shouldFail
        |> withErrorCode 72
