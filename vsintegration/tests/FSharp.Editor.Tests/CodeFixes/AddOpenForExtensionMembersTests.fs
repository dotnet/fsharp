// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddOpenForExtensionMembersTests

open System

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = AddOpenCodeFixProvider(AssemblyContentProvider())

/// Everything the fix offers, in the order the lightbulb lists it: the opens, then the qualifications.
let private allFixes code =
    codeFix |> multiFix code Auto |> Seq.toList

let private messages code = allFixes code |> List.map _.Message

let private openMessages code =
    messages code
    |> List.filter (fun message -> message.StartsWith("open ", StringComparison.Ordinal))

[<Fact>] // Only an open brings a C#-style extension method into scope; qualifying `xs.Where` makes no sense
let ``Offers the namespace of a C# extension method called on a value`` () =
    let code =
        """module Module1

let evens (xs: int list) = xs.Where(fun x -> x % 2 = 0)
"""

    let expected =
        [
            {
                Message = "open System.Linq"
                FixedCode =
                    """module Module1

open System.Linq

let evens (xs: int list) = xs.Where(fun x -> x % 2 = 0)
"""
            }
        ]

    let actual = allFixes code

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>] // The receiver is an expression whose type the fix does not know, so every `Where` counts - `System.Data` has one too
let ``Offers the namespace of a C# extension method called on an expression`` () =
    let code =
        """module Module1

let evens () = [ 1; 2; 3 ].Where(fun x -> x % 2 = 0)
"""

    Assert.Equal<string list>([ "open System.Data"; "open System.Linq" ], messages code)

[<Fact>] // `AsSpan` extends strings and arrays in different overloads of one class, and the content lists only one
let ``Offers a C# extension method whose other overload extends the receiver`` () =
    let code =
        """module Module1

let span (numbers: int[]) = numbers.AsSpan()
"""

    Assert.Equal<string list>([ "open System" ], messages code)

[<Fact>]
let ``Offers only the modules whose extension member extends the receiver`` () =
    let code =
        """module Module1

module StringExtensions =
    type System.String with
        member s.Shout = s.ToUpper()

module NumberExtensions =
    type System.Int32 with
        member n.Shout = string n

let greeting = "hi"
let loud = greeting.Shout
"""

    Assert.Equal<string list>([ "open StringExtensions" ], messages code)

[<Fact>]
let ``Offers the module of an F# extension property`` () =
    let code =
        """module Module1

module Extensions =
    type System.String with
        member s.Shout = s.ToUpper()

let greeting = "hi"
let loud = greeting.Shout
"""

    let expected =
        [
            {
                Message = "open Extensions"
                FixedCode =
                    """module Module1

module Extensions =
    type System.String with
        member s.Shout = s.ToUpper()

open Extensions

let greeting = "hi"
let loud = greeting.Shout
"""
            }
        ]

    let actual = allFixes code

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>] // Opening the namespace opens an [<AutoOpen>] module in it, and the module can still be opened on its own
let ``Offers both the namespace that opens an AutoOpen module and the module itself`` () =
    let code =
        """namespace Lib

[<AutoOpen>]
module StringExtensions =
    type System.String with
        member s.Shout() = s.ToUpper()

namespace App

module Use =
    let loud (s: string) = s.Shout()
"""

    Assert.Equal<string list>([ "open Lib"; "open Lib.StringExtensions" ], openMessages code)

[<Fact>]
let ``Offers the module of an F# static extension member`` () =
    let code =
        """module Module1

module Extensions =
    type System.String with
        static member Shout(s: string) = s.ToUpper()

let loud = System.String.Shout "hi"
"""

    Assert.Equal<string list>([ "open Extensions" ], openMessages code)

[<Fact>] // NEGATIVE: the extension is on `string`, and `n` is an `int`
let ``Doesn't offer an F# extension member of another type`` () =
    let code =
        """module Module1

module Extensions =
    type System.String with
        member s.Shout = s.ToUpper()

let n = 42
let loud = n.Shout
"""

    Assert.Equal<string list>([], messages code)

[<Fact>] // NEGATIVE: a module that requires qualified access cannot be opened
let ``Doesn't offer a module that requires qualified access`` () =
    let code =
        """module Module1

[<RequireQualifiedAccess>]
module Extensions =
    type System.String with
        member s.Shout = s.ToUpper()

let greeting = "hi"
let loud = greeting.Shout
"""

    Assert.Equal<string list>([], messages code)

[<Fact>] // NEGATIVE: opening its class does not make `Where` a name of its own; qualifying it does
let ``Doesn't offer to open the class of a C# extension method called by name`` () =
    let code =
        """module Module1

let evens (xs: int list) = Where(xs, fun x -> x % 2 = 0)
"""

    Assert.Equal<string list>([], openMessages code)
