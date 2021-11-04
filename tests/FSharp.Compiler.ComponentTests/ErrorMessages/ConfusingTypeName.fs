// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Confusing Type Name`` =

    [<Fact (Skip = "Test platform C# build fails with latest fsharp.core")>]
    let ``Expected types with multiple references`` () =

        let csLibA =
            CSharp """
public class A { }
public class B<T> { }
        """ |> withName "libA"

        let csLibB =
            csLibA |> withName "libB"

        let fsLibC =
            FSharp """
module AMaker
let makeA () : A = A()
let makeB () = B<_>()
        """ |> withName "libC" |> withReferences [csLibA]

        let fsLibD =
            FSharp """
module OtherAMaker
let makeOtherA () : A = A()
let makeOtherB () = B<_>()
        """ |> withName "libD" |> withReferences [csLibB]

        let app =
            FSharp """
module ConfusingTypeName
let a = AMaker.makeA()
let otherA = OtherAMaker.makeOtherA()
printfn "%A %A" (a.GetType().AssemblyQualifiedName) (otherA.GetType().AssemblyQualifiedName)
printfn "%A" (a = otherA)

let b = AMaker.makeB<int>()
let otherB = OtherAMaker.makeOtherB<int>()
printfn "%A %A" (b.GetType().AssemblyQualifiedName) (otherB.GetType().AssemblyQualifiedName)
printfn "%A" (b = otherB)
        """ |> withReferences [csLibA; csLibB; fsLibC; fsLibD]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 686, Line 8, Col 9, Line 8, Col 21, "The method or function 'makeB' should not be given explicit type argument(s) because it does not declare its type parameters explicitly")
            (Warning 686, Line 9, Col 14, Line 9, Col 36, "The method or function 'makeOtherB' should not be given explicit type argument(s) because it does not declare its type parameters explicitly")
            (Error 1, Line 6, Col 19, Line 6, Col 25, "This expression was expected to have type\n    'A (libA, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'    \nbut here has type\n    'A (libB, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'    ")
            (Error 1, Line 11, Col 19, Line 11, Col 25, "This expression was expected to have type\n    'B<Microsoft.FSharp.Core.int> (libA, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'    \nbut here has type\n    'B<Microsoft.FSharp.Core.int> (libB, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'    ")
        ]
