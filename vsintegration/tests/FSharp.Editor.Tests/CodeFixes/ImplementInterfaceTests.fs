// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ImplementInterfaceTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ImplementInterfaceCodeFixProvider()

[<Theory>]
[<InlineData " with">]
[<InlineData "">]
let ``Fixes FS0366`` optionalWith =
    let code =
        $"""
type IMyInterface =
    abstract member MyMethod: unit -> unit

type MyType() =
    interface IMyInterface{optionalWith}
"""

    let expected =
        [
            {
                Message = "Implement interface"
                FixedCode =
                    """
type IMyInterface =
    abstract member MyMethod: unit -> unit

type MyType() =
    interface IMyInterface with
        member this.MyMethod(): unit = 
            raise (System.NotImplementedException())
"""
            }
            {
                Message = "Implement interface without type annotation"
                FixedCode =
                    """
type IMyInterface =
    abstract member MyMethod: unit -> unit

type MyType() =
    interface IMyInterface with
        member this.MyMethod() = raise (System.NotImplementedException())
"""
            }
        ]

    let actual = codeFix |> multiFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0366 for partially implemented interfaces`` () =
    let code =
        $"""
type IMyInterface =
    abstract member MyMethod1 : unit -> unit
    abstract member MyMethod2 : unit -> unit

type MyType() =
    interface IMyInterface with
        member this.MyMethod2(): unit = ()
"""

    let expected =
        [
            {
                Message = "Implement interface"
                FixedCode =
                    """
type IMyInterface =
    abstract member MyMethod1 : unit -> unit
    abstract member MyMethod2 : unit -> unit

type MyType() =
    interface IMyInterface with
        member this.MyMethod1(): unit = 
            raise (System.NotImplementedException())
        member this.MyMethod2(): unit = ()
"""
            }
            {
                Message = "Implement interface without type annotation"
                FixedCode =
                    """
type IMyInterface =
    abstract member MyMethod1 : unit -> unit
    abstract member MyMethod2 : unit -> unit

type MyType() =
    interface IMyInterface with
        member this.MyMethod1() = raise (System.NotImplementedException())
        member this.MyMethod2(): unit = ()
"""
            }
        ]

    let actual = codeFix |> multiFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't handle FS0036 for inherited interfaces`` () =
    let code =
        $"""
type IMyInterface1 =
    abstract member MyMethod1 : unit -> unit

type IMyInterface2 =
    inherit IMyInterface1
    abstract member MyMethod2 : unit -> unit

type MyType () =
    interface IMyInterface1 with
        member this.MyMethod1 () = ()
    interface IMyInterface2 with
"""

    let expected = []

    let actual = codeFix |> multiFix code Auto

    Assert.Equal(expected, actual)
