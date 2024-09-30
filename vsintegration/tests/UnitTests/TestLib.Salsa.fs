// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module UnitTests.TestLib.Salsa

open System
open System.IO
open Xunit
open Salsa.Salsa
open Salsa.VsOpsUtils
open System.Text.RegularExpressions

// Common asserts -------------------------------------------------------------

let AssertEqualWithMessage(expected, actual, message) =
    if expected <> actual then 
        printf "%s" message
        Assert.Fail(message)
        
let AssertEqual(expected,actual) =
    if expected<>actual then 
        let message = sprintf "Expected:\n%A\n\nbut got:\n%A." expected actual
        printf "%s" message
        Assert.Fail(message)
        
let AssertContainsInOrder(s:string,cs:string list) =
    let rec containsInOrderFrom fromIndex expects =
      match expects with
        | [] -> ()
        | expect :: expects ->
            let index = s.IndexOf((expect:string),(fromIndex:int))           
            if index = -1 then
               Assert.Fail(sprintf "Expected:\n%s\n\nto contain:\n%s\n\nafter index: %d." s expect fromIndex)
            else
               printfn "At index %d seen '%s'." index expect
            containsInOrderFrom index expects
    containsInOrderFrom 0 cs
    
let AssertContains(value: string, substring: string) =
    Assert.Contains(substring, value)
        
let AssertArrayContainsPartialMatchOf(a:string array,c) =
    let found = ref false
    a |> Array.iter(fun s -> found := s.Contains(c) || !found)
    if not(!found) then 
        printfn "Expected:\n%A" a            
        printfn "to contain\n%s" c
        Assert.Fail()        
            
let AssertNotContains(s:string,c) = 
    if (s.Contains(c)) then
        printf "Expected:\n%s\n\nnot to not contain:\n%s." s c
        Assert.Fail()   
        
let AssertMatches (r : Regex) (s:string) =
    if not (r.IsMatch(s)) then
        printfn "Expected regex '%s' to match '%s'." (r.ToString()) s
        Assert.Fail()
        
// Like AssertMatches, but runs for every prefix of regex up to each occurrence of 'c'
// Is helpful so that, if long regex match fails, you see first prefix that fails
let AssertMatchesRegex (c : char) (regexStr : string) (s:string) =
    let mutable i = regexStr.IndexOf(c, 0)
    while i <> -1 do
        let r = regexStr.Substring(0,i)
        let regex = new Regex(r)
        AssertMatches regex s
        i <- regexStr.IndexOf(c, i+1)
        
// Common TestFixture methods -------------------------------------------------

open System.IO
open UnitTests.TestLib.Utils

// adds qualifier to the global functions
// Non-controlled usage of these functions can easily lead to the violation of invariants in tests:
// - modification of shared VS\solution is prohibited and the only permitted operation is CreateSingleFileProject
// - modification of fresh VS instance is allowed, to denote that tests required fresh instance of VS add line 'use  _guard - this.WithNewVS()' at the beginning of the test
module internal GlobalFunctions = 
    let CreateSolution(vs) = CreateSolution(vs)
    let CloseSolution(sol) = CloseSolution(sol)
    let Cleanup(vs) = Cleanup(vs)
    let SetOtherFlags(proj, flags) = SetOtherFlags(proj, flags)
    let AddAssemblyReference(proj, ref) = AddAssemblyReference(proj, ref)
    let AddAssemblyReferenceEx(proj, ref, v) = AddAssemblyReferenceEx(proj, ref, v)
    let SetProjectDefines(proj, d) = SetProjectDefines(proj, d)
    let CreateSingleFileProject(vs, content) = CreateSingleFileProject(vs, content)
    let CreateNamedSingleFileProject(vs, content) = CreateNamedSingleFileProject(vs, content)
    let AddDisabledWarning(proj, warning) = AddDisabledWarning(proj, warning)

// hides existing global functions - so they can be accessed only from GlobalFunctions
[<AutoOpen>]
module HiddenFunctions = 
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.CreateSolution
    let CreateSolution() : unit = failwith "Should not be called"
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.CloseSolution
    let CloseSolution() : unit= failwith "Should not be called"
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.Cleanup
    let Cleanup() : unit= failwith "Should not be called"
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.AddAssemblyReference
    let AddAssemblyReference() : unit= failwith "Should not be called"
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.AddAssemblyReferenceEx
    let AddAssemblyReferenceEx() : unit= failwith "Should not be called"
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.SetProjectDefines
    let SetProjectDefines() : unit= failwith "Should not be called"
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.CreateSingleFileProject
    let CreateSingleFileProject() : unit = failwith "Should not be called"
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.CreateNamedSingleFileProject
    let CreateNamedSingleFileProject() : unit = failwith "Should not be called"
    /// this function should not be called from the global namespace
    /// if you really need it - use qualified form: GlobalFunctions.AddDisabledWarning
    let AddDisabledWarning() : unit = failwith "Should not be called"

