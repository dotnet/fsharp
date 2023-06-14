// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ChangeToUpcastTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = FSharpChangeToUpcastCodeFixProvider()
let private diagnostic = 3198 // The conversion is an upcast, not a downcast...

// Test cases are taken from the original PR:
// https://github.com/dotnet/fsharp/pull/10463

[<Fact>]
let ``Fixes FS3198 - operator`` () =
    let code =
        """
type IFoo = abstract member Bar : unit -> unit
type Foo() = interface IFoo with member __.Bar () = ()

let Thing : IFoo = Foo() :?> IFoo
"""

    let expected =
        {
            Title = "Use ':>' operator"
            FixedCode =
                """
type IFoo = abstract member Bar : unit -> unit
type Foo() = interface IFoo with member __.Bar () = ()

let Thing : IFoo = Foo() :> IFoo
"""
        }

    let actual = codeFix |> fix code diagnostic

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS3198 - keyword`` () =
    let code =
        """
type IFoo = abstract member Bar : unit -> unit
type Foo() = interface IFoo with member __.Bar () = ()

let Thing : IFoo = downcast Foo()
"""

    let expected =
        {
            Title = "Use 'upcast'"
            FixedCode =
                """
type IFoo = abstract member Bar : unit -> unit
type Foo() = interface IFoo with member __.Bar () = ()

let Thing : IFoo = upcast Foo()
"""
        }

    let actual = codeFix |> fix code diagnostic

    Assert.Equal(expected, actual)
