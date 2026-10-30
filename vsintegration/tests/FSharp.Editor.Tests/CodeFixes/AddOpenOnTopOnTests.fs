// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddOpenOnTopOnTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework
open AddOpenOnTopSharedTests

let private codeFix = AddOpenCodeFixProvider(AssemblyContentProvider())

[<Fact>]
let ``Fixes FS0039 for missing opens - basic`` () =
    let code =
        """Console.WriteLine 42
"""

    let expected =
        Some
            {
                Message = "open System"
                FixedCode =
                    """open System

Console.WriteLine 42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - first line is empty`` () =
    let code =
        """
Console.WriteLine 42
"""

    let expected =
        Some
            {
                Message = "open System"
                FixedCode =
                    """
open System

Console.WriteLine 42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - multiple first lines are empty`` () =
    let code =
        """

Console.WriteLine 42
"""

    let expected =
        Some
            {
                Message = "open System"
                FixedCode =
                    """

open System

Console.WriteLine 42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

let placements =
    scenarios
        [
            ("declaration shares its line with the end of a comment",
             "(* header\n*) Console.WriteLine 42\n",
             "(* header\n*)\n   open System\n\n   Console.WriteLine 42\n")

            ("explicit top level module without a blank line",
             "module Module1\nConsole.WriteLine 42\n",
             "module Module1\n\nopen System\n\nConsole.WriteLine 42\n")

            ("last line with no line break", "module Module1\nConsole.WriteLine 42", "module Module1\n\nopen System\n\nConsole.WriteLine 42")

            ("namespace without a blank line",
             "namespace N1\nmodule M1 =\n    Console.WriteLine 42\n",
             "namespace N1\n\nopen System\n\nmodule M1 =\n    Console.WriteLine 42\n")
        ]

[<Theory; MemberData(nameof placements)>]
let ``Fixes FS0039 for missing opens - placement`` (_scenario: string, code: string, fixedCode: string) =
    expectOpenSystem Auto code fixedCode

[<Fact>]
let ``Fixes FS0039 for missing opens - there is already an open directive`` () =
    let code =
        """open System.IO

Console.WriteLine 42
"""

    let expected =
        Some
            {
                Message = "open System"
                FixedCode =
                    """open System.IO
open System

Console.WriteLine 42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - top level module is explicit`` () =
    let code =
        """module Module1

Console.WriteLine 42
"""

    let expected =
        Some
            {
                Message = "open System"
                FixedCode =
                    """module Module1

open System

Console.WriteLine 42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - nested module`` () =
    let code =
        """module Module1 =

    Console.WriteLine 42
"""

    let expected =
        Some
            {
                Message = "open System"
                FixedCode =
                    """open System

module Module1 =

    Console.WriteLine 42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - explicit module has attributes`` () =
    let code =
        """
[<AutoOpen>]
module Module1

Console.WriteLine 42
"""

    let expected =
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - implicit module has attributes`` () =
    let code =
        """
[<Obsolete>]
type MyType() =
    let now = DateTime.Now
"""

    let expected =
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - nested module has attributes`` () =
    let code =
        """
[<AutoOpen>]
module Module1 =

    Console.WriteLine 42
"""

    let expected =
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

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
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

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
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

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
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for missing opens - explicit namespace`` () =
    let code =
        """
namespace N1

module M1 =

    Console.WriteLine 42
"""

    let expected =
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

let openSuggestions =
    scenarios
        [
            ("a type nested in a type is opened with open type",
             "let folder () = SpecialFolder.Desktop",
             [| "open type System.Environment" |])

            ("every namespace the name resolves from",
             "let readFile () = File.ReadAllText \"example.txt\"",
             [| "open System.IO"; "open type System.Net.WebRequestMethods" |])

            ("every type a static member resolves from",
             "let write () = WriteLine \"hi\"",
             [|
                 "open type System.Console"
                 "open type System.Diagnostics.Debug"
                 "open type System.Diagnostics.Trace"
                 "open type Microsoft.VisualBasic.FileSystem"
             |])

            ("a type in a namespace is opened with a plain open", "let write () = Console.WriteLine \"hi\"", [| "open System" |])

            ("a generic type is not opened", "let keys (collection: KeyCollection) = collection", [||])
        ]

[<Theory; MemberData(nameof openSuggestions)>]
let ``Offers the opens that resolve the name`` (_scenario: string, usage: string, declarations: string[]) =
    let expected =
        [
            for declaration in declarations do
                {
                    Message = declaration
                    FixedCode = $"module Module1\n\n{declaration}\n\n{usage}\n"
                }
        ]

    let actual = codeFix |> openFixes $"module Module1\n\n{usage}\n" Auto

    Assert.Equal<TestCodeFix list>(expected, actual)

let suggestionMessages =
    scenarios
        [
            ("the qualifications follow the opens",
             "let readFile () = File.ReadAllText \"example.txt\"",
             [|
                 "open System.IO"
                 "open type System.Net.WebRequestMethods"
                 "System.IO.File"
                 "System.IO.File.ReadAllText"
                 "System.Net.WebRequestMethods.File"
             |])

            ("the System qualifications come first",
             "let write () = WriteLine \"hi\"",
             [|
                 "open type System.Console"
                 "open type System.Diagnostics.Debug"
                 "open type System.Diagnostics.Trace"
                 "open type Microsoft.VisualBasic.FileSystem"
                 "System.Console.WriteLine"
                 "System.Diagnostics.Debug.WriteLine"
                 "System.Diagnostics.Trace.WriteLine"
             |])
        ]

[<Theory; MemberData(nameof suggestionMessages)>]
let ``Lists the suggestions in the order of the lightbulb`` (_scenario: string, usage: string, messages: string[]) =
    let actual =
        codeFix |> allFixes $"module Module1\n\n{usage}\n" Auto |> List.map _.Message

    Assert.Equal<string list>(List.ofArray messages, actual)

[<Fact>]
let ``Qualifying a partly qualified name replaces what is written before the unresolved part`` () =
    let code =
        "module Review\nmodule M = let marker = ()\nmodule N =\n    module M =\n        type FixTarget817() = class end\nlet x = M.FixTarget817()\n"

    let actual =
        codeFix
        |> allFixes code Auto
        |> List.filter (fun fix -> fix.Message = "N.M.FixTarget817")
        |> List.map _.FixedCode

    Assert.Equal<string list>([ code.Replace("x = M.", "x = N.M.") ], actual)

[<Fact>]
let ``Doesn't fix FS0039 for random undefined symbols`` () =
    let code =
        """
let f = g
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

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
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0043 for random unsupported values`` () =
    let code =
        """
type RecordType = { X : int }

let x : RecordType = null
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

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
        Some
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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
