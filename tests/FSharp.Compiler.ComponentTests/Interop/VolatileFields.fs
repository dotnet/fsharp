// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Interop

open Xunit
open FSharp.Test.Compiler

module VolatileFields =

    let private csLib =
        CSharp
            """
namespace CsLib
{
    public class Holder
    {
        public volatile int Flag;
        public static volatile int SFlag;
    }
}
"""
        |> withName "CsLib"

    let private roundTrip =
        FSharp
            """
module M
open CsLib

let roundTrip (h: Holder) =
    h.Flag <- 1
    Holder.SFlag <- h.Flag
    Holder.SFlag
"""
        |> asLibrary
        |> withReferences [ csLib ]

    [<Fact>]
    let ``Volatile fields are read and written with the volatile prefix`` () =
        roundTrip
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyILContains
            [ """
      IL_0002:  volatile.
      IL_0004:  stfld      int32 modreq([runtime]System.Runtime.CompilerServices.IsVolatile) [CsLib]CsLib.Holder::Flag
      IL_0009:  ldarg.0
      IL_000a:  volatile.
      IL_000c:  ldfld      int32 modreq([runtime]System.Runtime.CompilerServices.IsVolatile) [CsLib]CsLib.Holder::Flag
      IL_0011:  volatile.
      IL_0013:  stsfld     int32 modreq([runtime]System.Runtime.CompilerServices.IsVolatile) [CsLib]CsLib.Holder::SFlag
      IL_0018:  volatile.
      IL_001a:  ldsfld     int32 modreq([runtime]System.Runtime.CompilerServices.IsVolatile) [CsLib]CsLib.Holder::SFlag
      IL_001f:  ret""" ]

    [<Fact>]
    let ``Volatile fields are accessed without the volatile prefix before the language feature`` () =
        roundTrip
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "volatile." ]
