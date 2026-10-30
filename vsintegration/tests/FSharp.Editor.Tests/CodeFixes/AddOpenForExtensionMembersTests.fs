// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddOpenForExtensionMembersTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework
open AddOpenOnTopSharedTests

let private codeFix = AddOpenCodeFixProvider(AssemblyContentProvider())

let private messages code =
    codeFix |> allFixes code Auto |> List.map _.Message

let private openMessages code =
    codeFix |> openFixes code Auto |> List.map _.Message

let private shoutOnSequenceOf elementType =
    $"""namespace Lib

open System.Collections.Generic
open System.Runtime.CompilerServices

[<Extension>]
type SequenceExtensions =
    [<Extension>]
    static member Shout(words: IEnumerable<string>) = words |> Seq.map (fun word -> word.ToUpper())

namespace App

module Use =
    let loud (xs: seq<{elementType}>) = xs.Shout()
"""

let private releaseOn receiverType =
    $"""namespace Lib

open System
open System.Runtime.CompilerServices

[<Extension>]
type DisposableExtensions =
    [<Extension>]
    static member Release<'T when 'T :> IDisposable>(resource: 'T) = resource.Dispose()

namespace App

module Use =
    let release (x: {receiverType}) = x.Release()
"""

let private constrainedOn constrainedTo receiverType =
    $"""namespace Lib

open System.Runtime.CompilerServices

[<Extension>]
type ConstrainedExtensions =
    [<Extension>]
    static member Constrained<'T when 'T : {constrainedTo}>(value: 'T) = value

namespace App

module Use =
    let constrained (x: {receiverType}) = x.Constrained()
"""

let extensionOpens =
    scenarios
        [
            ("a C# extension method called on an expression of unknown type: every namespace with one",
             "module Module1\n\nlet evens () = [ 1; 2; 3 ].Where(fun x -> x % 2 = 0)\n",
             [| "open System.Data"; "open System.Linq" |])

            ("a C# extension method whose other overload extends the receiver",
             "module Module1\n\nlet span (numbers: int[]) = numbers.AsSpan()\n",
             [| "open System" |])

            ("a C# extension on IEnumerable<string> extends a sequence of strings", shoutOnSequenceOf "string", [| "open Lib" |])

            ("a C# extension on IEnumerable<string> does not extend a sequence of numbers", shoutOnSequenceOf "int", [||])

            ("a C# extension on a disposable type parameter extends a stream", releaseOn "System.IO.MemoryStream", [| "open Lib" |])

            ("a C# extension on a disposable type parameter does not extend a number", releaseOn "int", [||])

            ("a C# extension on a struct type parameter extends a number", constrainedOn "struct" "int", [| "open Lib" |])

            ("a C# extension on a struct type parameter does not extend a string", constrainedOn "struct" "string", [||])

            ("a C# extension on a class type parameter extends a string", constrainedOn "not struct" "string", [| "open Lib" |])

            ("a C# extension on a class type parameter does not extend a number", constrainedOn "not struct" "int", [||])

            ("a C# extension on a type parameter with a default constructor extends a builder",
             constrainedOn "(new: unit -> 'T)" "System.Text.StringBuilder",
             [| "open Lib" |])

            ("a C# extension on a type parameter with a default constructor does not extend a string",
             constrainedOn "(new: unit -> 'T)" "string",
             [||])

            ("a C# extension on an enum type parameter does not extend a number", constrainedOn "enum<int>" "int", [||])

            ("a C# extension on a delegate type parameter does not extend a string", constrainedOn "delegate<unit, unit>" "string", [||])

            ("only the modules whose F# extension member extends the receiver",
             """module Module1

module StringExtensions =
    type System.String with
        member s.Shout = s.ToUpper()

module NumberExtensions =
    type System.Int32 with
        member n.Shout = string n

let greeting = "hi"
let loud = greeting.Shout
""",
             [| "open StringExtensions" |])

            ("an F# extension member of another type",
             """module Module1

module Extensions =
    type System.String with
        member s.Shout = s.ToUpper()

let n = 42
let loud = n.Shout
""",
             [||])

            ("a module that requires qualified access",
             """module Module1

[<RequireQualifiedAccess>]
module Extensions =
    type System.String with
        member s.Shout = s.ToUpper()

let greeting = "hi"
let loud = greeting.Shout
""",
             [||])
        ]

[<Theory; MemberData(nameof extensionOpens)>]
let ``Offers the opens of the extension members that apply`` (_scenario: string, code: string, expected: string[]) =
    Assert.Equal<string list>(List.ofArray expected, messages code)

let extensionOpensBesideQualifications =
    scenarios
        [
            ("the namespace that opens an AutoOpen module, and the module itself",
             """namespace Lib

[<AutoOpen>]
module StringExtensions =
    type System.String with
        member s.Shout() = s.ToUpper()

namespace App

module Use =
    let loud (s: string) = s.Shout()
""",
             [| "open Lib"; "open Lib.StringExtensions" |])

            ("the module of an F# static extension member",
             """module Module1

module Extensions =
    type System.String with
        static member Shout(s: string) = s.ToUpper()

let loud = System.String.Shout "hi"
""",
             [| "open Extensions" |])

            ("no open for the class of a C# extension method called by name",
             """module Module1

let evens (xs: int list) = Where(xs, fun x -> x % 2 = 0)
""",
             [||])
        ]

[<Theory; MemberData(nameof extensionOpensBesideQualifications)>]
let ``Offers the opens of the extension members, whatever else is offered`` (_scenario: string, code: string, expected: string[]) =
    Assert.Equal<string list>(List.ofArray expected, openMessages code)

[<Fact>]
let ``Offers the namespace of a C# extension method called on a value`` () =
    let code =
        "module Module1\n\nlet evens (xs: int list) = xs.Where(fun x -> x % 2 = 0)\n"

    let expected =
        [
            {
                Message = "open System.Linq"
                FixedCode = code.Replace("let evens", "open System.Linq\n\nlet evens")
            }
        ]

    Assert.Equal<TestCodeFix list>(expected, codeFix |> allFixes code Auto)

[<Fact>]
let ``Offers the module of an F# extension property`` () =
    let code =
        "module Module1\n\nmodule Extensions =\n    type System.String with\n        member s.Shout = s.ToUpper()\n\nlet greeting = \"hi\"\nlet loud = greeting.Shout\n"

    let expected =
        [
            {
                Message = "open Extensions"
                FixedCode = code.Replace("let greeting", "open Extensions\n\nlet greeting")
            }
        ]

    Assert.Equal<TestCodeFix list>(expected, codeFix |> allFixes code Auto)
