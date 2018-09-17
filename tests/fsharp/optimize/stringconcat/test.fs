module Test.Test

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open FSharp.NativeInterop

[<AutoOpen>]
module Helpers = 
    let failures = ref false
    let report_failure (s) = 
        stderr.WriteLine ("NO: " + s); 
        failures := true
    let test s b = if b then () else report_failure(s) 

    (* TEST SUITE FOR Int32 *)

    let out r (s:string) = r := !r @ [s]

    let check s actual expected = 
        if actual = expected then printfn "%s: OK" s
        else report_failure (sprintf "%s: FAILED, expected %A, got %A" s expected actual)

    let check2 s expected actual = check s actual expected 

let arr = ResizeArray()

let inline ss (x: int) =
    arr.Add(x)
    "_" + x.ToString() + "_"

let test1 () =
    ss 1 + ss 2 + ss 3

let test2 () =
    ss 1 + ss 2 + ss 3 + ss 4

let test3 () =
    ss 1 + ss 2 + ss 3 + ss 4 + ss 5

let test4 () =
    ss 5 + ss 6 + ss 7 + String.Concat(ss 8, ss 9) + ss 10 + "_50_" + "_60_" + String.Concat(ss 100, String.Concat(ss 101, ss 102), ss 103) + String.Concat([|"_104_";"_105_"|]) + ss 106

let test5 () =
    ss 5 + ss 6 + ss 7 + String.Concat(ss 8, ss 9) + ss 10 + "_50_" + "_60_" + String.Concat(ss 100, (let x = String.Concat(ss 101, ss 102) in Console.WriteLine(x);x), ss 103) + String.Concat([|"_104_";"_105_"|]) + ss 106

let inline inlineStringConcat str1 str2 = str1 + str2

let test6 () =
    inlineStringConcat (inlineStringConcat (ss 1) (ss 2)) (ss 3) + ss 4

[<EntryPoint>]
let main argv =
    let r1 = test1 ()
    let r2 = test2 ()
    let r3 = test3 ()
    let r4 = test4 ()
    let r5 = test5 ()
    let r6 = test6 ()

    check "Test1" r1 "_1__2__3_"
    check "Test2" r2 "_1__2__3__4_"
    check "Test3" r3 "_1__2__3__4__5_"
    check "Test4" r4 "_5__6__7__8__9__10__50__60__100__101__102__103__104__105__106_"
    check "Test5" r5 "_5__6__7__8__9__10__50__60__100__101__102__103__104__105__106_"
    check "Test6" r6 "_1__2__3__4_"

    let expected =
        [
            1;2;3
            1;2;3;4
            1;2;3;4;5
            5;6;7;8;9;10;100;101;102;103;106
            5;6;7;8;9;10;100;101;102;103;106
            1;2;3;4
        ]

    check "Valid List" (arr |> Seq.toList) expected

    if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
    else (stdout.WriteLine "Test Passed"; 
            System.IO.File.WriteAllText("test.ok","ok"))

    0
