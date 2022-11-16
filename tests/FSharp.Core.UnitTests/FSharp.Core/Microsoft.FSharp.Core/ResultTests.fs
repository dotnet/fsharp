// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for:
// Microsoft.FSharp.Core.Result

namespace FSharp.Core.UnitTests

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit

type EmailValidation=
    | Empty
    | NoAt

open Result

type ResultTests() =

    let assertWasNotCalledThunk () = raise (exn "Thunk should not have been called.")
    
    let fail_if_empty email=
        if String.IsNullOrEmpty(email) then Error Empty else Ok email

    let fail_if_not_at (email:string)=
        if (email.Contains("@")) then Ok email else Error NoAt

    let validate_email =
        fail_if_empty
        >> bind fail_if_not_at

    let test_validate_email email (expected:Result<string,EmailValidation>) =
        let actual = validate_email email
        Assert.AreEqual(expected, actual)

    let toUpper (v:string) = v.ToUpper()

    let shouldBeOkWithValue expected maybeOk = match maybeOk with | Error e-> failwith "Expected Ok, got Error!" | Ok v->Assert.AreEqual(expected, v)

    let shouldBeErrorWithValue expected maybeError = match maybeError with | Error e-> Assert.AreEqual(expected, e) | Ok v-> failwith "Expected Error, got Ok!"

    let addOneOk (v:int) = Ok (v+1)

    [<Fact>]
    member this.CanChainTogetherSuccessiveValidations() =
        test_validate_email "" (Error Empty)
        test_validate_email "something_else" (Error NoAt)
        test_validate_email "some@email.com" (Ok "some@email.com")

    [<Fact>]
    member this.MapWillTransformOkValues() =
        Ok "some@email.com" 
        |> map toUpper
        |> shouldBeOkWithValue "SOME@EMAIL.COM"

    [<Fact>]
    member this.MapWillNotTransformErrorValues() =
        Error "my error" 
        |> map toUpper
        |> shouldBeErrorWithValue "my error"

    [<Fact>]
    member this.MapErrorWillTransformErrorValues() =
        Error "my error" 
        |> mapError toUpper
        |> shouldBeErrorWithValue "MY ERROR"

    [<Fact>]
    member this.MapErrorWillNotTransformOkValues() =
        Ok "some@email.com" 
        |> mapError toUpper
        |> shouldBeOkWithValue "some@email.com"

    [<Fact>]
    member this.BindShouldModifyOkValue() =
        Ok 42
        |> bind addOneOk
        |> shouldBeOkWithValue 43

    [<Fact>]
    member this.BindErrorShouldNotModifyError() =
        Error "Error"
        |> bind addOneOk
        |> shouldBeErrorWithValue "Error"

    [<Fact>]
    member this.IsOk() =
        Assert.True(Result.isOk (Ok 1))
        Assert.False(Result.isOk (Error 1))

    [<Fact>]
    member this.IsError() =
        Assert.False(Result.isError (Ok 1))
        Assert.True(Result.isError (Error 1))
    
    [<Fact>]
    member this.DefaultValue() =
        Assert.AreEqual(Result.defaultValue 3 (Error 42), 3)
        Assert.AreEqual(Result.defaultValue 3 (Ok 42), 42)
        Assert.AreEqual(Result.defaultValue "1" (Error "x"), "1")
        Assert.AreEqual(Result.defaultValue "1" (Ok "x"), "x")

    [<Fact>]
    member this.DefaultWith() =
        Assert.AreEqual(Result.defaultWith (fun _ -> 3) (Error 42), 3)
        Assert.AreEqual(Result.defaultWith (fun err -> 3 + err) (Error 42), 45)
        Assert.AreEqual(Result.defaultWith (fun _ -> "1") (Error "42"), "1")
        Assert.AreEqual(Result.defaultWith (fun err -> "1" + err) (Error "42"), "142")

        Assert.AreEqual(Result.defaultWith assertWasNotCalledThunk (Ok 42), 42)
        Assert.AreEqual(Result.defaultWith assertWasNotCalledThunk (Ok "1"), "1")
        
    [<Fact>]
    member this.Count() =
        Assert.AreEqual(Result.count (Ok 1), 1)
        Assert.AreEqual(Result.count (Error 1), 0)
        
    [<Fact>]
    member this.Fold() =
        Assert.AreEqual(Result.fold (fun accum x -> accum + x * 2) 0 (Error 2), 0)
        Assert.AreEqual(Result.fold (fun accum x -> accum + x * 2) 0 (Ok 1), 2)
        Assert.AreEqual(Result.fold (fun accum x -> accum + x * 2) 10 (Ok 1), 12)
        
    [<Fact>]
    member this.FoldBack() =
        Assert.AreEqual(Result.foldBack (fun x accum -> accum + x * 2) (Error 2) 0, 0)
        Assert.AreEqual(Result.foldBack (fun x accum -> accum + x * 2) (Ok 1) 0, 2)
        Assert.AreEqual(Result.foldBack (fun x accum -> accum + x * 2) (Ok 1) 10, 12)
        
    [<Fact>]
    member this.Exists() =
        Assert.AreEqual(Result.exists (fun x -> x >= 5) (Error 6), false)
        Assert.AreEqual(Result.exists (fun x -> x >= 5) (Ok 42), true)
        Assert.AreEqual(Result.exists (fun x -> x >= 5) (Ok 4), false)
        
    [<Fact>]
    member this.ForAll() =
        Assert.AreEqual(Result.forall (fun x -> x >= 5) (Error 1), true)
        Assert.AreEqual(Result.forall (fun x -> x >= 5) (Ok 42), true)
        Assert.AreEqual(Result.forall (fun x -> x >= 5) (Ok 4), false)
    
    [<Fact>]
    member this.Contains() =    
        Assert.AreEqual(Result.contains 99 (Error 99), false)
        Assert.AreEqual(Result.contains 99 (Ok 99), true)
        Assert.AreEqual(Result.contains 99 (Ok 100), false)
            
    [<Fact>]
    member this.Iterate() =
        let mutable i = 0
        let increment _ =
            i <- i + 1
        
        Result.iter increment (Error "Hello world")
        Assert.AreEqual(0, i)
        Result.iter increment (Ok "Hello world")
        Assert.AreEqual(1, i)
        
    [<Fact>]
    member this.ToArray() =
        Assert.AreEqual(Result.toArray (Error 42), [| |])
        Assert.AreEqual(Result.toArray (Ok 42), [| 42 |])
        
    [<Fact>]
    member this.ToList() =
        Assert.AreEqual(Result.toList (Error 42), [ ])
        Assert.AreEqual(Result.toList (Ok 42), [ 42 ])
        
    [<Fact>]
    member this.ToOption() =
        Assert.AreEqual(Result.toOption (Error 42), None)
        Assert.AreEqual(Result.toOption (Ok 42), Some 42)
        
    [<Fact>]
    member this.ToValueOption() =
        Assert.AreEqual(Result.toValueOption (Error 42), ValueNone)
        Assert.AreEqual(Result.toValueOption (Ok 42), ValueSome 42)
