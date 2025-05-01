// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddOpenOnTopOnTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

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
