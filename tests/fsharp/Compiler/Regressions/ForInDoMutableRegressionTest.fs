// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework
open FSharp.Test.Utilities

[<TestFixture()>]
module ForInDoMutableRegressionTest =

    /// This test is to ensure we initialize locals inside loops.
    [<Test>]
    let Script_ForInDoMutableRegressionTest() =
        let script = 
            """
open System.Collections.Generic

let bug() = 
    for a in [1;2;3;4] do
      let mutable x = null
      if x = null then
        x <- HashSet<int>()
      x.Add a |> ignore
      let expected = [a]
      let actual = List.ofSeq x
      if expected <> actual then
        failwith "Bug"

let not_a_bug() = 
  for a in [1;2;3;4] do
    let x = ref null
    if (!x) = null then
      x := HashSet<int>()
    (!x).Add a |> ignore
    let expected = [a]
    let actual = List.ofSeq (!x)
    if expected <> actual then
      failwith "Bug"

let rec test_rec xs =
    let mutable x = null
    match xs with
    | [] -> ()
    | a :: xs ->
        if x = null then
          x <- HashSet<int>()
        x.Add a |> ignore
        let expected = [a]
        let actual = List.ofSeq x
        if expected <> actual then
          failwith "Bug"
        test_rec xs

let test_for_loop () =
    let xs = [|1;2;3;4|]
    for i = 0 to xs.Length - 1 do
      let a = xs.[i]
      let mutable x = null
      if x = null then
        x <- HashSet<int>()
      x.Add a |> ignore
      let expected = [a]
      let actual = List.ofSeq x
      if expected <> actual then
        failwith "Bug"

bug ()
not_a_bug ()
test_rec [1;2;3;4]
test_for_loop ()
            """
        
        CompilerAssert.RunScript script []
