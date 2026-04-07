namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module DebugInlineAsCall =

    [<Fact>]
    let ``Call 01 - Release`` () =
        FSharp """
let inline f (x: int) =
    x + x

let i = f 5
"""
        |> asExe
        |> compile
        |> verifyILContains ["ldc.i4.s   10"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 02 - Debug`` () =
        FSharp """
let inline f (x: int) =
    x + x

[<EntryPoint>]
let main _ =
    let i = f 5
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::f(int32)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 03 - Two args`` () =
        FSharp """

let inline add a b =
    a + b

[<EntryPoint>]
let main _ =
    let i = add 1 2
    if i = 3 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<add>__debug@8'(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 04 - Function arg`` () =
        FSharp """
let inline apply (f: 'a -> 'b -> 'c) (x: 'a) (y: 'b) : 'c =
    f x y

[<EntryPoint>]
let main _ =
    let i = apply (+) 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       !!2 Test::apply<int32,int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!2>>,"]
        |> shouldSucceed


    [<Fact>]
    let ``Call 05 - Nested inline`` () =
        FSharp """
let inline double (x: int) =
    x + x

let inline quadruple (x: int) =
    double (double x)

[<EntryPoint>]
let main _ =
    let i = quadruple 3
    if i = 12 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            ["call       int32 Test::double(int32)"
             "call       int32 Test::quadruple(int32)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 06 - Multiple calls`` () =
        FSharp """
let inline double (x: int) =
    x + x

[<EntryPoint>]
let main _ =
    let i = double 1 + double 2
    if i = 6 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains [ "call       int32 Test::double(int32)" ]

    [<Fact>]
    let ``Call 07 - Local function`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline double (x: int) = x + x
    let i = double 5
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 08 - Local generic function`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline apply (f: 'a -> 'b) (x: 'a) : 'b = f x
    let i = apply (fun x -> x + 1) 5
    if i = 6 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>,int32>::InvokeFast<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 09 - FSharp.Core not`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let b = not true
    if b = false then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       bool [FSharp.Core]Microsoft.FSharp.Core.Operators::Not(bool)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 10 - Different assembly`` () =
        let library =
            FSharp """
module MyLib

let inline triple (x: int) = x + x + x
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary
            |> withName "mylib"

        FSharp """
open MyLib

[<EntryPoint>]
let main _ =
    let i = triple 3
    if i = 9 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 [mylib]MyLib::triple(int32)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 11 - Measure`` () =
        FSharp """
[<Measure>] type cm

let inline scale (x: float<'u>) = x * 2.0

[<EntryPoint>]
let main _ =
    let v = scale 5.0<cm>
    if v = 10.0<cm> then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       float64 Test::scale(float64)"]
        |> shouldSucceed

    [<Fact>]
    let ``Call 12 - No inner optimization`` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline f (x: int) =
        let i = 5 + 10
        x + i

    let i = f 20
    if i = 35 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "ldc.i4.5"
              "ldc.i4.s   10"
              "callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)" ]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 01`` () =
        FSharp """
let inline add (x: ^T) (y: ^T) =
    x + y

[<EntryPoint>]
let main _ =
    let i = add 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<add>__debug@7'(int32,"]
        |> shouldSucceed
    
    [<Fact>]
    let ``SRTP 02 - Local `` () =
        FSharp """
[<EntryPoint>]
let main _ =
    let inline add (x: ^T) (y: ^T) = x + y
    let i = add 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<add>__debug@5'(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 03 - Different type arguments`` () =
        FSharp """
let inline getLength (x: ^T) =
    (^T : (member Length : int) x)

[<EntryPoint>]
let main _ =
    let i = getLength "hello"
    let j = getLength [1; 2; 3]
    if i = 5 && j = 3 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<getLength>__debug@7'(string)"
              "call       int32 Test::'<getLength>__debug@8-1'(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)" ]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 04 - Multiple calls`` () =
        FSharp """
let inline add (x: ^T) (y: ^T) = x + y

[<EntryPoint>]
let main _ =
    let i = add 1 2 + add 3 4
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<add>__debug@6'(int32,"
              "call       int32 Test::'<add>__debug@6-1'(int32," ]
        |> shouldSucceed
    

    [<Fact>]
    let ``SRTP 05 - Different assembly`` () =
        let library =
            FSharp """
module MyLib

let inline add (x: ^T) (y: ^T) = x + y
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary
            |> withName "mylib2"

        FSharp """
open MyLib

[<EntryPoint>]
let main _ =
    let i = add 3 4
    if i = 7 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> verifyILContains ["call       int32 Test::'<add>__debug@6'(int32,"]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 06 - Different assembly`` () =
        let library =
            FSharp """
module MyLib

let inline add (x: ^T) (y: ^T) = x + y
"""
            |> withDebug
            |> withNoOptimize
            |> asLibrary
            |> withName "mylib3"

        FSharp """
open MyLib

let inline double (x: ^T) = add x x

[<EntryPoint>]
let main _ =
    let i = double 5
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> withReferences [library]
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<double>__debug@8'(int32)"
              "call       int32 Test::'<add>__debug@4'(int32," ]
        |> shouldSucceed

    [<Fact>]
    let ``SRTP 07 - Nested - Same project`` () =
        FSharp """
let inline add (x: ^T) (y: ^T) = x + y

let inline double (x: ^T) = add x x

[<EntryPoint>]
let main _ =
    let i = double 5
    if i = 10 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<double>__debug@8'(int32)"
              "call       int32 Test::'<add>__debug@4'(int32," ]
        |> shouldSucceed

    

    [<Fact>]
    let ``SRTP 08 - Nested - Different type arguments`` () =
        FSharp """
let inline add (x: ^T) (y: ^T) = x + y

let inline addBoth (x: ^A) (y: ^B) =
    let a = add x x
    let b = add y y
    (a, b)

[<EntryPoint>]
let main _ =
    let (a, b) = addBoth 2 3.0
    if a = 4 && b = 6.0 then 0 else 1
"""
        |> withDebug
        |> withNoOptimize
        |> asExe
        |> compileAndRun
        |> verifyILContains
            [ "call       int32 Test::'<add>__debug@5'(int32,"
              "call       float64 Test::'<add>__debug@6-1'(float64," ]
        |> shouldSucceed
