// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices


module ``Warn Expression`` =

    [<Fact>]
    let ``Warn If Expression Result Unused``() =
        CompilerAssert.TypeCheckSingleError
            """
1 + 2
printfn "%d" 3
            """
            FSharpErrorSeverity.Warning
            20
            (2, 1, 2, 6)
            "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'."

    [<Fact>]
    let ``Warn If Possible Assignment``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 10
let y = "hello"

let changeX() =
    x = 20
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (6, 5, 6, 11)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to mutate a value, then mark the value 'mutable' and use the '<-' operator e.g. 'x <- expression'."

    [<Fact>]
    let ``Warn If Possible Assignment To Mutable``() =
        CompilerAssert.TypeCheckSingleError
            """
let mutable x = 10
let y = "hello"

let changeX() =
    x = 20
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (6, 5, 6, 11)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to mutate a value, then use the '<-' operator e.g. 'x <- expression'."

    [<Fact>]
    let ``Warn If Possible dotnet Property Setter``() =
        CompilerAssert.TypeCheckWithErrors
            """
open System

let z = System.Timers.Timer()
let y = "hello"

let changeProperty() =
    z.Enabled = true
    y = "test"
            """
            [|
                FSharpErrorSeverity.Warning, 760, (4, 9, 4, 30), "It is recommended that objects supporting the IDisposable interface are created using the syntax 'new Type(args)', rather than 'Type(args)' or 'Type' as a function value representing the constructor, to indicate that resources may be owned by the generated value"
                FSharpErrorSeverity.Warning, 20, (8, 5, 8, 21), "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to set a value to a property, then use the '<-' operator e.g. 'z.Enabled <- expression'."
            |]

    [<Fact>]
    let ``Don't Warn If Property Without Setter``() =
        CompilerAssert.TypeCheckSingleError
            """
type MyClass(property1 : int) =
    member val Property2 = "" with get

let x = MyClass(1)
let y = "hello"

let changeProperty() =
    x.Property2 = "22"
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (9, 5, 9, 23)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'."

    [<Fact>]
    let ``Warn If Implicitly Discarded``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 10
let y = 20

let changeX() =
    y * x = 20
    y = 30
            """
            FSharpErrorSeverity.Warning
            20
            (6, 5, 6, 15)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'."

    [<Fact>]
    let ``Warn If Discarded In List``() =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            """
let div _ _ = 1
let subView _ _ = [1; 2]

// elmish view
let view model dispatch =
   [
       yield! subView model dispatch
       div [] []
   ]
            """
            [|
                FSharpErrorSeverity.Warning,
                3221,
                (9, 8, 9, 17),
                "This expression returns a value of type 'int' but is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to use the expression as a value in the sequence then use an explicit 'yield'."
            |]

    [<Fact>]
    let ``Warn If Discarded In List 2``() =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            """
// stupid things to make the sample compile
let div _ _ = 1
let subView _ _ = [1; 2]
let y = 1

// elmish view
let view model dispatch =
   [
        div [] [
           match y with
           | 1 -> yield! subView model dispatch
           | _ -> subView model dispatch
        ]
   ]
            """
            [|
                FSharpErrorSeverity.Warning,
                3222,
                (13, 19, 13, 41),
                "This expression returns a value of type 'int list' but is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to use the expression as a value in the sequence then use an explicit 'yield!'."
            |]

    [<Fact>]
    let ``Warn If Discarded In List 3``() =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            """
// stupid things to make the sample compile
let div _ _ = 1
let subView _ _ = true
let y = 1

// elmish view
let view model dispatch =
   [
        div [] [
           match y with
           | 1 -> ()
           | _ -> subView model dispatch
        ]
   ]
            """
            [|
                FSharpErrorSeverity.Warning,
                20,
                (13, 19, 13, 41),
                "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'."
            |]

    [<Fact>]
    let ``Warn Only On Last Expression``() =
        CompilerAssert.TypeCheckSingleError
            """
let mutable x = 0
while x < 1 do
    printfn "unneeded"
    x <- x + 1
    true
            """
            FSharpErrorSeverity.Warning
            20
            (6, 5, 6, 9)
            "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'."

    [<Fact>]
    let ``Warn If Possible Property Setter``() =
        CompilerAssert.TypeCheckSingleError
            """
type MyClass(property1 : int) =
    member val Property1 = property1
    member val Property2 = "" with get, set

let x = MyClass(1)
let y = "hello"

let changeProperty() =
    x.Property2 = "20"
    y = "test"
            """
            FSharpErrorSeverity.Warning
            20
            (10, 5, 10, 23)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to set a value to a property, then use the '<-' operator e.g. 'x.Property2 <- expression'."


    [<Fact>]
    let ``Dont warn external function as unused``() =
        CompilerAssert.Pass
            """
open System
open System.Runtime.InteropServices

module Test =

    [<DllImport("shell32.dll", CharSet=CharSet.Auto)>]
    extern int32 ExtractIconEx(string szFileName, int nIconIndex,IntPtr[] phiconLarge, IntPtr[] phiconSmall,uint32 nIcons)

    [<DllImport("user32.dll", EntryPoint="DestroyIcon", SetLastError=true)>]
    extern int DestroyIcon(IntPtr hIcon)

[<EntryPoint>]
let main _argv =
    let _ = Test.DestroyIcon IntPtr.Zero

    let _ = Test.ExtractIconEx("", 0, [| |], [| |], 0u)

    0
            """
