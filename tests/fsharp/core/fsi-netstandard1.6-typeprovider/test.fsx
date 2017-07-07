// #Conformance #Constants #Recursion #LetBindings #MemberDefinitions #Mutable 
#if TESTS_AS_APP
module Core_fsi_netstandard16
#endif

#light
let failures = ref false
let reportFailure (s) = 
  stderr.WriteLine ("NO: " + s); failures := true
let test s b = if b then () else reportFailure(s) 

let check s actual expected = 
    if actual = expected then printfn "%s: OK" s
    else reportFailure (sprintf "%s: FAILED, expected %A, got %A" s expected actual)


// This tests both referencing a .NET Standard 1.6 runtime library, and referencing a .NET Standard 2.0 design-time type provider component
#r @"FSharp.Data\netstandard1.6\FSharp.Data.dll"

open System
open FSharp.Data

type Simple = JsonProvider<""" { "name":"John", "age":94 } """>
let simple = Simple.Parse(""" { "name":"Tomas", "age":4 } """)
check "cewlkjc" simple.Age 4
check "cewlkjc2" simple.Name "Tomas"

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
        exit 0)

