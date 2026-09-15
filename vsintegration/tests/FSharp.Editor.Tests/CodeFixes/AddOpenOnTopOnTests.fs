// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddOpenOnTopOnTests

open System

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = AddOpenCodeFixProvider(AssemblyContentProvider())

/// Everything the fix offers, in the order the lightbulb lists it: the opens, then the qualifications.
let private allFixes code mode =
    codeFix |> multiFix code mode |> Seq.toList

/// Just the `open` suggestions, for the tests that are about where the declaration lands.
let private openFixes code mode =
    allFixes code mode
    |> List.filter (fun fix -> fix.Message.StartsWith("open ", StringComparison.Ordinal))

[<Fact>]
let ``Fixes FS0039 for missing opens - basic`` () =
    let code =
        """Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - first line is empty`` () =
    let code =
        """
Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """
open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - multiple first lines are empty`` () =
    let code =
        """

Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """

open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>] // The first declaration follows a block comment closing on its line
let ``Fixes FS0039 for missing opens - declaration shares its line with the end of a comment`` () =
    let code =
        """(* header
*) Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """(* header
*)
   open System

   Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - there is already an open directive`` () =
    let code =
        """open System.IO

Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """open System.IO
open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - top level module is explicit`` () =
    let code =
        """module Module1

Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """module Module1

open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - module has an attribute on the same line`` () =
    let code =
        """[<AutoOpen>] module Module1

Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """[<AutoOpen>] module Module1

open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - explicit top level module without a blank line`` () =
    let code =
        """module Module1
Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """module Module1

open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - namespace without a blank line`` () =
    let code =
        """namespace N1
module M1 =
    Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """namespace N1

open System

module M1 =
    Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - nested module`` () =
    let code =
        """module Module1 =

    Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """open System

module Module1 =

    Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - explicit module has attributes`` () =
    let code =
        """
[<AutoOpen>]
module Module1

Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """
[<AutoOpen>]
module Module1

open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - implicit module has attributes`` () =
    let code =
        """
[<Obsolete>]
type MyType() =
    let now = DateTime.Now
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """
open System

[<Obsolete>]
type MyType() =
    let now = DateTime.Now
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - nested module has attributes`` () =
    let code =
        """
[<AutoOpen>]
module Module1 =

    Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """
open System

[<AutoOpen>]
module Module1 =

    Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - module has multiple attributes`` () =
    let code =
        """
[<AutoOpen>]
[<AutoOpen>]
module Module1

Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """
[<AutoOpen>]
[<AutoOpen>]
module Module1

open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - attributes are mixed with empty lines`` () =
    let code =
        """
[<AutoOpen>]

[<AutoOpen>]
module Module1

Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """
[<AutoOpen>]

[<AutoOpen>]
module Module1

open System

Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - multiple modules in one file`` () =
    let code =
        """
module Module1 =

    let x = 42

module Module2 =

    Console.WriteLine(42)
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """
open System

module Module1 =

    let x = 42

module Module2 =

    Console.WriteLine(42)
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - explicit namespace`` () =
    let code =
        """
namespace N1

module M1 =

    Console.WriteLine 42
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """
namespace N1

open System

module M1 =

    Console.WriteLine 42
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>] // A plain `open` only reaches namespaces and modules; a type nested in a type needs `open type`
let ``Fixes FS0039 with open type for a type nested in a type`` () =
    let code =
        """module Module1

let folder () = SpecialFolder.Desktop
"""

    let expected =
        [
            {
                Message = "open type System.Environment"
                FixedCode =
                    """module Module1

open type System.Environment

let folder () = SpecialFolder.Desktop
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>] // `File` is both `System.IO.File` and the nested `System.Net.WebRequestMethods.File`
let ``Offers every namespace a name can be resolved from`` () =
    let code =
        """module Module1

let readFile () = File.ReadAllText "example.txt"
"""

    let expected =
        [
            {
                Message = "open System.IO"
                FixedCode =
                    """module Module1

open System.IO

let readFile () = File.ReadAllText "example.txt"
"""
            }
            {
                Message = "open type System.Net.WebRequestMethods"
                FixedCode =
                    """module Module1

open type System.Net.WebRequestMethods

let readFile () = File.ReadAllText "example.txt"
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>] // Qualifying the name in place is offered alongside opening what holds it
let ``Offers qualifying the name after the opens`` () =
    let code =
        """module Module1

let readFile () = File.ReadAllText "example.txt"
"""

    let expected =
        [
            "open System.IO"
            "open type System.Net.WebRequestMethods"
            // Qualifications, three of them at most, `System.IO.File` twice over because the type and
            // the member being reached through it are both candidates.
            "System.IO.File"
            "System.IO.File.ReadAllText"
            "System.Net.WebRequestMethods.File"
        ]

    let actual = allFixes code Auto |> List.map _.Message

    Assert.Equal<string list>(expected, actual)

[<Fact>] // `WriteLine` is a static member of four different types, `System` ones offered first
let ``Offers every type a static member can be resolved from`` () =
    let code =
        """module Module1

let write () = WriteLine "hi"
"""

    let expected =
        [
            {
                Message = "open type System.Console"
                FixedCode =
                    """module Module1

open type System.Console

let write () = WriteLine "hi"
"""
            }
            {
                Message = "open type System.Diagnostics.Debug"
                FixedCode =
                    """module Module1

open type System.Diagnostics.Debug

let write () = WriteLine "hi"
"""
            }
            {
                Message = "open type System.Diagnostics.Trace"
                FixedCode =
                    """module Module1

open type System.Diagnostics.Trace

let write () = WriteLine "hi"
"""
            }
            {
                Message = "open type Microsoft.VisualBasic.FileSystem"
                FixedCode =
                    """module Module1

open type Microsoft.VisualBasic.FileSystem

let write () = WriteLine "hi"
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>] // NEGATIVE: a type sitting directly in a namespace is reached by a plain open
let ``Fixes FS0039 with a plain open for a type in a namespace`` () =
    let code =
        """module Module1

let write () = Console.WriteLine "hi"
"""

    let expected =
        [
            {
                Message = "open System"
                FixedCode =
                    """module Module1

open System

let write () = Console.WriteLine "hi"
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Doesn't fix FS0039 for random undefined symbols`` () =
    let code =
        """
let f = g
"""

    let expected = []

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0043 for missing opens`` () =
    let code =
        """
module M =
    let (++) x y = 10 * x + y

module N =
    let theAnswer = 4 ++ 2
"""

    let expected =
        [
            {
                Message = "open M"
                FixedCode =
                    """
module M =
    let (++) x y = 10 * x + y

open M

module N =
    let theAnswer = 4 ++ 2
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Doesn't fix FS0043 for random unsupported values`` () =
    let code =
        """
type RecordType = { X : int }

let x : RecordType = null
"""

    let expected = []

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - module has multiline attributes`` () =
    let code =
        """
namespace X

open System

[<RequireQualifiedAccess;
  CompiledName((nameof System.Collections.Immutable.ImmutableArray)
               + "Module")>]
module FlatList =

    let a : KeyValuePair<string, int> = KeyValuePair<string, int>("key", 1)
"""

    let expected =
        [
            {
                Message = "open System.Collections.Generic"
                FixedCode =
                    """
namespace X

open System
open System.Collections.Generic

[<RequireQualifiedAccess;
  CompiledName((nameof System.Collections.Immutable.ImmutableArray)
               + "Module")>]
module FlatList =

    let a : KeyValuePair<string, int> = KeyValuePair<string, int>("key", 1)
"""
            }
        ]

    let actual = openFixes code Auto

    Assert.Equal<TestCodeFix list>(expected, actual)
