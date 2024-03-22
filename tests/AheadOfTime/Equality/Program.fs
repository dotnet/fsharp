open System.Collections.Generic
open NonStructuralComparison

let failures = HashSet<string>()

let reportFailure (s: string) = 
    stderr.Write " NO: "
    stderr.WriteLine s
    failures.Add s |> ignore

let test testName x y =
    let result = HashIdentity.Structural.Equals(x, y)
    if result = false
    then
        stderr.WriteLine($"\n***** {testName}: Expected: 'true' Result: '{result}' = FAIL\n");
        reportFailure testName

module BasicTypes =
    test "test000" true true
    test "test001" 1y 1y
    test "test002" 1uy 1uy
    test "test003" 1s 1s
    test "test004" 1us 1us
    test "test005" 1 1
    test "test006" 1u 1u
    test "test007" 1L 1L
    test "test008" 1UL 1UL
    test "test009" (nativeint 1) (nativeint 1)
    test "test010" (unativeint 1) (unativeint 1)
    test "test011" 'a' 'a'
    test "test012" "a" "a"
    test "test013" 1m 1m
    test "test014" 1.0 1.0
    test "test015" 1.0f 1.0f

module Arrays =
    test "test100" [|1|] [|1|]
    test "test101" [|1L|] [|1L|]
    test "test102" [|1uy|] [|1uy|]
    test "test103" [|box 1|] [|box 1|]

module Structs = 
    test "test200" struct (1, 1) struct (1, 1)
    test "test201" struct (1, 1, 1) struct (1, 1, 1)
    test "test202" struct (1, 1, 1, 1) struct (1, 1, 1, 1)
    test "test203" struct (1, 1, 1, 1, 1) struct (1, 1, 1, 1, 1)
    test "test204" struct (1, 1, 1, 1, 1, 1) struct (1, 1, 1, 1, 1, 1)
    test "test205" struct (1, 1, 1, 1, 1, 1, 1) struct (1, 1, 1, 1, 1, 1, 1)
    test "test206" struct (1, 1, 1, 1, 1, 1, 1, 1) struct (1, 1, 1, 1, 1, 1, 1, 1)

module OptionsAndCo = 
    open System

    test "test301" (Some 1) (Some 1)
    test "test302" (ValueSome 1) (ValueSome 1)
    test "test303" (Ok 1) (Ok 1)
    test "test304" (Nullable 1) (Nullable 1)

module Enums =
    open System

    type SomeEnum = 
        | Case0 = 0
        | Case1 = 1

    test "test401" (enum<SomeEnum>(1)) (enum<SomeEnum>(1)) 
    test "test402" (enum<DayOfWeek>(1)) (enum<DayOfWeek>(1)) 

[<EntryPoint>]
let main _ =
    match failures with 
    | set when set.Count = 0 -> 
        stdout.WriteLine "All tests passed"
        exit 0
    | _ -> 
        stdout.WriteLine "Some tests failed"
        exit 1
