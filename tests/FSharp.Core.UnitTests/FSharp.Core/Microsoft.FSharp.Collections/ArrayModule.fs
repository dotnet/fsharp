// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Collections.Array module

namespace FSharp.Core.UnitTests.Collections

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit

(*
[Test Strategy]
Make sure each method works on:
* Integer array (value type)
* String  array (reference type)
* Empty   array (0 elements)
* Null    array (null)
*)

type ArrayModule() =

    [<Fact>]
    member _.Empty() =
        let emptyArray = Array.empty
        if Array.length emptyArray <> 0 then Assert.Fail()    
        
        let c : int[]   = Array.empty<int>
        Assert.True( (c = [| |]) )
        
        let d : string[] = Array.empty<string>
        Assert.True( (d = [| |]) )
        ()


    [<Fact>]
    member _.AllPairs() =
        // integer array
        let resultInt =  Array.allPairs [|1..3|] [|2..2..6|]
        if resultInt <> [|(1,2);(1,4);(1,6)
                          (2,2);(2,4);(2,6)
                          (3,2);(3,4);(3,6)|] then Assert.Fail()

        // string array
        let resultStr = Array.allPairs [|"A"; "B"; "C" ; "D" |] [|"a";"b";"c";"d"|]
        if resultStr <> [|("A","a");("A","b");("A","c");("A","d")
                          ("B","a");("B","b");("B","c");("B","d")
                          ("C","a");("C","b");("C","c");("C","d")
                          ("D","a");("D","b");("D","c");("D","d")|] then Assert.Fail()

        // empty array
        if Array.allPairs [||]     [||] <> [||]  then Assert.Fail()
        if Array.allPairs [|1..3|] [||] <> [||]  then Assert.Fail()
        if Array.allPairs [||] [|1..3|] <> [||]  then Assert.Fail()

        // null array
        let nullArr = null:string[]
        CheckThrowsArgumentNullException (fun () -> Array.allPairs nullArr nullArr  |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.allPairs [||]    nullArr  |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.allPairs nullArr [||]     |> ignore)

        ()

    [<Fact>]
    member _.Append() =
        // integer array
        let intArray = Array.append [| 1; 2 |] [| 3; 4 |]
        Assert.True( (intArray = [| 1; 2; 3; 4 |]) )

        // string array
        let strArray = Array.append [| "a"; "b" |] [| "C"; "D" |]
        Assert.True( (strArray = [| "a"; "b"; "C"; "D" |]) )

        // empty array
        let emptyArray : int[]  = [|   |]
        let singleArray : int[] = [| 1 |]
        
        let appEmptySingle = Array.append emptyArray singleArray
        let appSingleEmpty = Array.append singleArray emptyArray
        
        Assert.True( (appEmptySingle = [| 1 |]) )
        Assert.True( (appSingleEmpty = [| 1 |]) )
      
        // null array
        let nullArray = null:int[]
        let validArray = [| 1 |]
        CheckThrowsArgumentNullException (fun () -> Array.append validArray nullArray |> ignore)    
        CheckThrowsArgumentNullException (fun () -> Array.append nullArray validArray |> ignore)   

        ()

    [<Fact>]
    member _.Average() =   
      
        // empty float32 array
        let emptyFloatArray = Array.empty<float32> 
        CheckThrowsArgumentException(fun () -> Array.average emptyFloatArray |> ignore)
        
        // empty double array
        let emptyDoubleArray = Array.empty<System.Double> 
        CheckThrowsArgumentException(fun () -> Array.average emptyDoubleArray |> ignore)
        
        // empty decimal array
        let emptyDecimalArray = Array.empty<System.Decimal> 
        CheckThrowsArgumentException (fun () -> Array.average emptyDecimalArray |>ignore )

        // float32 array
        let floatArray: float32[] = [| 1.2f; 3.5f; 6.7f |]
        let averageOfFloat = Array.average floatArray
        if averageOfFloat <> 3.8000000000000003f then Assert.Fail()
        
        // double array
        let doubleArray: System.Double[] = [| 1.0;8.0 |]
        let averageOfDouble = Array.average doubleArray
        if averageOfDouble <> 4.5 then Assert.Fail()
        
        // decimal array
        let decimalArray: decimal[] = [| 0M; 19M; 19.03M |]
        let averageOfDecimal = Array.average decimalArray
        if averageOfDecimal <> 12.676666666666666666666666667M then Assert.Fail()      
        
        // null array
        let nullArr = null : double[]    
        CheckThrowsArgumentNullException (fun () -> Array.average nullArr |> ignore) 

        ()

    [<Fact>]
    member _.AverageBy() =

        // empty double array
        let emptyDouArray = Array.empty<System.Double>
        CheckThrowsArgumentException(fun () -> Array.averageBy (fun x -> x + 6.7) emptyDouArray |> ignore)

        // empty float32 array
        let emptyFloat32Array: float32[] = [||]
        CheckThrowsArgumentException(fun () -> Array.averageBy (fun x -> x + 9.8f) emptyFloat32Array |> ignore)

        // empty decimal array
        let emptyDecimalArray = Array.empty<System.Decimal>
        CheckThrowsArgumentException(fun () -> Array.averageBy (fun x -> x + 9.8M) emptyDecimalArray |> ignore)

        // float32 array
        let floatArray: float32[] = [| 1.5f; 2.5f; 3.5f; 4.5f |] // using values that behave nicely with IEEE floats
        let averageOfFloat = Array.averageBy (fun x -> x + 1.0f) floatArray
        Assert.AreEqual(4.0f, averageOfFloat)

        // double array
        let doubleArray: System.Double[] = [| 1.0; 8.0 |] // using values that behave nicely with IEEE doubles
        let averageOfDouble = Array.averageBy (fun x -> x + 1.0) doubleArray
        Assert.AreEqual(5.5, averageOfDouble)

        // decimal array
        let decimalArray: decimal[] = [| 0M;19M;19.03M |]
        let averageOfDecimal = Array.averageBy (fun x -> x + 9.8M) decimalArray
        Assert.AreEqual(22.476666666666666666666666667M, averageOfDecimal)

        // null array
        let nullArr : double[] = null
        CheckThrowsArgumentNullException (fun () -> Array.averageBy (fun x -> x + 6.7) nullArr |> ignore)

        ()

    [<Fact>]
    member _.ChunkBySize() =

        // int Seq
        Assert.True([| [|1..4|]; [|5..8|] |] = Array.chunkBySize 4 [|1..8|])
        Assert.True([| [|1..4|]; [|5..8|]; [|9..10|] |] = Array.chunkBySize 4 [|1..10|])
        Assert.True([| [|1|]; [|2|]; [|3|]; [|4|] |] = Array.chunkBySize 1 [|1..4|])
        Assert.True([| [|1..3|]; [|4|] |] = Array.chunkBySize 3 [|1..4|])
        Assert.True([| [|1..5|]; [|6..10|]; [|11..12|] |] = Array.chunkBySize 5 [|1..12|])

        // string Seq
        Assert.True([| [|"a"; "b"|]; [|"c";"d"|]; [|"e"|] |] = Array.chunkBySize 2 [|"a";"b";"c";"d";"e"|])

        // empty Seq
        Assert.True([||] = Array.chunkBySize 3 [||])

        // null Seq
        let nullArr:_[] = null
        CheckThrowsArgumentNullException (fun () -> Array.chunkBySize 3 nullArr |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> Array.chunkBySize 0 [|1..10|] |> ignore)
        CheckThrowsArgumentException (fun () -> Array.chunkBySize -1 [|1..10|] |> ignore)

        ()

    [<Fact>]
    member _.SplitInto() =

        // int array
        Assert.True([| [|1..4|]; [|5..7|]; [|8..10|] |] = Array.splitInto 3 [|1..10|])
        Assert.True([| [|1..4|]; [|5..8|]; [|9..11|] |] = Array.splitInto 3 [|1..11|])
        Assert.True([| [|1..4|]; [|5..8|]; [|9..12|] |] = Array.splitInto 3 [|1..12|])

        Assert.True([| [|1..2|]; [|3|]; [|4|]; [|5|] |] = Array.splitInto 4 [|1..5|])
        Assert.True([| [|1|]; [|2|]; [|3|]; [|4|] |] = Array.splitInto 20 [|1..4|])

        // string array
        Assert.True([| [|"a"; "b"|]; [|"c";"d"|]; [|"e"|] |] = Array.splitInto 3 [|"a";"b";"c";"d";"e"|])

        // empty array
        Assert.True([| |] = Array.splitInto 3 [| |])

        // null array
        let nullArr:_[] = null
        CheckThrowsArgumentNullException (fun () -> Array.splitInto 3 nullArr |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> Array.splitInto 0 [|1..10|] |> ignore)
        CheckThrowsArgumentException (fun () -> Array.splitInto -1 [|1..10|] |> ignore)

        ()

    [<Fact>]
    member _.distinct() =
        // distinct should work on empty array
        Assert.AreEqual([||], Array.distinct [||])

        // distinct not should work on null
        CheckThrowsArgumentNullException (fun () -> Array.distinct null |> ignore)

        // distinct should filter out simple duplicates
        Assert.AreEqual([|1|], Array.distinct [|1|])
        Assert.AreEqual([|1|], Array.distinct [|1; 1|])
        Assert.AreEqual([|1; 2; 3|], Array.distinct [|1; 2; 3; 1|])
        Assert.AreEqual([|[1;2]; [1;3]|], Array.distinct [|[1;2]; [1;3]; [1;2]; [1;3]|])
        Assert.AreEqual([|[1;1]; [1;2]; [1;3]; [1;4]|], Array.distinct [|[1;1]; [1;2]; [1;3]; [1;4]|])
        Assert.AreEqual([|[1;1]; [1;4]|], Array.distinct [|[1;1]; [1;1]; [1;1]; [1;4]|])

        Assert.AreEqual([|null|], Array.distinct [|null|])
        let list = new System.Collections.Generic.List<int>()
        Assert.AreEqual([|null, list|], Array.distinct [|null, list|])
        
    [<Fact>]
    member _.distinctBy() =
        // distinctBy should work on empty array
        Assert.AreEqual([||], Array.distinctBy (fun _ -> failwith "should not be executed") [||])

        // distinctBy should not work on null
        CheckThrowsArgumentNullException (fun () -> Array.distinctBy (fun _ -> failwith "should not be executed") null |> ignore)

        // distinctBy should filter out simple duplicates
        Assert.AreEqual([|1|], Array.distinctBy id [|1|])
        Assert.AreEqual([|1|], Array.distinctBy id [|1; 1|])
        Assert.AreEqual([|1; 2; 3|], Array.distinctBy id [|1; 2; 3; 1|])

        // distinctBy should use the given projection to filter out simple duplicates
        Assert.AreEqual([|1|], Array.distinctBy (fun x -> x / x) [|1; 2|])
        Assert.AreEqual([|1; 2|], Array.distinctBy (fun x -> if x < 3 then x else 1) [|1; 2; 3; 4|])
        Assert.AreEqual([|[1;2]; [1;3]|], Array.distinctBy (fun x -> List.sum x) [|[1;2]; [1;3]; [2;1]|])

        Assert.AreEqual([|null|], Array.distinctBy id [|null|])
        let list = new System.Collections.Generic.List<int>()
        Assert.AreEqual([|null, list|], Array.distinctBy id [|null, list|])

    [<Fact>]
    member _.Except() =
        // integer array
        let intArr1 = [| yield! {1..100}
                         yield! {1..100} |]
        let intArr2 = [| 1 .. 10 |]
        let expectedIntArr = [| 11 .. 100 |]

        Assert.AreEqual(expectedIntArr, Array.except intArr2 intArr1)

        // string array
        let strArr1 = [| "a"; "b"; "c"; "d"; "a" |]
        let strArr2 = [| "b"; "c" |]
        let expectedStrArr = [| "a"; "d" |]

        Assert.AreEqual(expectedStrArr, Array.except strArr2 strArr1)

        // empty array
        let emptyIntArr = [| |]
        Assert.AreEqual([|1..100|], Array.except emptyIntArr intArr1)
        Assert.AreEqual(emptyIntArr, Array.except intArr1 emptyIntArr)
        Assert.AreEqual(emptyIntArr, Array.except emptyIntArr emptyIntArr)
        Assert.AreEqual(emptyIntArr, Array.except intArr1 intArr1)

        // null array
        let nullArr : int [] = null
        CheckThrowsArgumentNullException(fun () -> Array.except nullArr emptyIntArr |> ignore)
        CheckThrowsArgumentNullException(fun () -> Array.except emptyIntArr nullArr |> ignore)
        CheckThrowsArgumentNullException(fun () -> Array.except nullArr nullArr |> ignore)

        ()

    [<Fact>]
    member _.Take() =
        Assert.AreEqual([||],Array.take 0 [||])
        Assert.AreEqual([||],Array.take 0 [|"str1";"str2";"str3";"str4"|])
        Assert.AreEqual([|1;2;4|],Array.take 3 [|1;2;4;5;7|])
        Assert.AreEqual([|"str1";"str2"|],Array.take 2 [|"str1";"str2";"str3";"str4"|])
        Assert.AreEqual( [|"str1";"str2";"str3";"str4"|],Array.take 4 [|"str1";"str2";"str3";"str4"|])

        CheckThrowsInvalidOperationExn (fun () -> Array.take 1 [||] |> ignore)
        CheckThrowsArgumentException (fun () -> Array.take -1 [|0;1|] |> ignore)
        CheckThrowsInvalidOperationExn (fun () -> Array.take 5 [|"str1";"str2";"str3";"str4"|] |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.take 5 null |> ignore)
        
    [<Fact>]
    member _.takeWhile() =
        Assert.AreEqual([||],Array.takeWhile (fun x -> failwith "should not be used") [||])
        Assert.AreEqual([|1;2;4;5|],Array.takeWhile (fun x -> x < 6) [|1;2;4;5;6;7|])
        Assert.AreEqual([|"a"; "ab"; "abc"|],Array.takeWhile (fun (x:string) -> x.Length < 4) [|"a"; "ab"; "abc"; "abcd"; "abcde"|])
        Assert.AreEqual([|"a"; "ab"; "abc"; "abcd"; "abcde"|],Array.takeWhile (fun _ -> true) [|"a"; "ab"; "abc"; "abcd"; "abcde"|])
        Assert.AreEqual([||],Array.takeWhile (fun _ -> false) [|"a"; "ab"; "abc"; "abcd"; "abcde"|])
        Assert.AreEqual([||],Array.takeWhile (fun _ -> false) [|"a"|])
        Assert.AreEqual([|"a"|],Array.takeWhile (fun _ -> true) [|"a"|])
        Assert.AreEqual([|"a"|],Array.takeWhile (fun x -> x <> "ab") [|"a"; "ab"; "abc"; "abcd"; "abcde"|])

        CheckThrowsArgumentNullException (fun () -> Array.takeWhile (fun _ -> failwith "should not be used") null |> ignore) 

    [<Fact>]
    member _.splitAt() =        
        Assert.AreEqual([||], Array.splitAt 0 [||] |> fst)  
        Assert.AreEqual([||], Array.splitAt 0 [||] |> snd)

        Assert.AreEqual([|1..4|], Array.splitAt 4 [|1..10|] |> fst)       
        Assert.AreEqual([|5..10|], Array.splitAt 4 [|1..10|] |> snd)      

        Assert.AreEqual([||], Array.splitAt 0 [|1..2|] |> fst)
        Assert.AreEqual([|1..2|], Array.splitAt 0 [|1..2|] |> snd)

        Assert.AreEqual([|1|], Array.splitAt 1 [|1..2|] |> fst)
        Assert.AreEqual([|2|], Array.splitAt 1 [|1..2|] |> snd)

        Assert.AreEqual([|1..2|], Array.splitAt 2 [|1..2|] |> fst)
        Assert.AreEqual([||], Array.splitAt 2 [|1..2|] |> snd)

        Assert.AreEqual([|"a"|], Array.splitAt 1 [|"a";"b";"c"|] |> fst)
        Assert.AreEqual([|"b";"c"|], Array.splitAt 1 [|"a";"b";"c"|] |> snd)

        // split should fail if index exceeds bounds
        CheckThrowsInvalidOperationExn (fun () -> Array.splitAt 1 [||] |> ignore)
        CheckThrowsArgumentException (fun () -> Array.splitAt -1 [|0;1|] |> ignore)
        CheckThrowsInvalidOperationExn (fun () -> Array.splitAt 5 [|"str1";"str2";"str3";"str4"|] |> ignore)
        
        CheckThrowsArgumentNullException (fun () -> Array.splitAt 0 null |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.splitAt 1 null |> ignore)

    [<Fact>]
    member _.replicate() =
        // replicate should create multiple copies of the given value
        Assert.AreEqual([||],Array.replicate 0 null)
        Assert.AreEqual([||],Array.replicate 0 1)
        Assert.AreEqual([|null|],Array.replicate 1 null)
        Assert.AreEqual([|"1";"1"|],Array.replicate 2 "1")

        CheckThrowsArgumentException (fun () ->  Array.replicate -1 null |> ignore)
        
    [<Fact>]
    member _.Blit() = 
        // int array   
        let intSrc = [| 1..10 |]
        let intDes:int[] = Array.zeroCreate 10 
        Array.blit intSrc 0 intDes 0 5
        if intDes.[4] <> 5 then Assert.Fail()
        if intDes.[5] <> 0 then Assert.Fail()
        
        // string array
        let strSrc = [| "a";"b";"c";"d";"e";"j"|]
        let strDes = Array.create 10 "w"
        Array.blit strSrc 1 strDes 2 3
        if strDes.[3] <> "c" || Array.get strDes 4 = "w" then Assert.Fail()
     
        // null array
        let nullArr = null:string[]
        CheckThrowsArgumentNullException (fun () -> Array.blit nullArr 1 strDes 2 3 |> ignore) 

        // bounds check
        CheckThrowsArgumentOutOfRangeException (fun () -> Array.blit intSrc -1 intDes 1 3 |> ignore)
        CheckThrowsArgumentOutOfRangeException (fun () -> Array.blit intSrc 1 intDes -1 3 |> ignore)
        CheckThrowsArgumentOutOfRangeException (fun () -> Array.blit intSrc 1 intDes 1 -3 |> ignore)
        CheckThrowsArgumentException (fun () -> Array.blit intSrc 1 intDes 1 300 |> ignore)
        CheckThrowsArgumentException (fun () -> Array.blit intSrc 1 intDes 5 8 |> ignore)
        
        ()

      
    member private this.ChooseTester chooseInt chooseString = 
        // int array
        let intSrc:int [] = [| 1..100 |]    
        let funcInt x = if (x%5=0) then Some x else None       
        let intChoosed : int[] = chooseInt funcInt intSrc
        if intChoosed.[1] <> 10 then Assert.Fail()
        
        // string array
        let stringSrc: string [] = "Lists are a commonly used data structure. They are not mutable, i.e., you can't delete an element of a list  instead you create a new list with the element deleted. List values often share storage under the hood, i.e., a list value only allocate more memory when you actually execute construction operations.".Split([|' '|], System.StringSplitOptions.RemoveEmptyEntries)
        let funcString x = match x with
                           | "list"-> Some x
                           | "List" -> Some x
                           | _ -> None
        let strChoosed : string[]  = chooseString funcString stringSrc   
        if strChoosed.[1].ToLower() <> "list" then Assert.Fail()
        
        // empty array
        let emptySrc :int[] = [| |]
        let emptyChoosed = chooseInt funcInt emptySrc
        Assert.True( (emptyChoosed = [| |]) )

        // null array
        let nullArr = null:int[]    
        CheckThrowsArgumentNullException (fun () -> chooseInt funcInt nullArr |> ignore) 
        
        () 
      
    [<Fact>]
    member this.Choose() = 
        this.ChooseTester Array.choose Array.choose

    [<Fact>]
    member this.``Parallel.Choose`` () = 
        this.ChooseTester Array.Parallel.choose Array.Parallel.choose

    member private this.CollectTester collectInt collectString =
    
        // int array - checking ordering
        let intSrc  = [| 1..3 |]
        let func = fun i -> [| 1..i |]
        let result : int[] = collectInt func intSrc
        Assert.AreEqual ([| 1; 1; 2; 1; 2; 3 |], result)
        
        // string array
        let stringSrc = [| "foo"; "bar" |]
        let func = fun s -> [| s |]
        let result : string[] = collectString func stringSrc
        Assert.AreEqual(stringSrc, result)
        
        // empty array
        let emptyArray : string [] = [| |]
        let result = collectString func emptyArray
        Assert.AreEqual(emptyArray,result)
        
        // null array
        let nullArr = null:int[]
        CheckThrowsArgumentNullException (fun () -> collectInt func nullArr |> ignore)
        
        ()

    [<Fact>]
    member this.Collect () =
        this.CollectTester Array.collect Array.collect
        
    [<Fact>]
    member _.CollectWithSideEffects () =
        let mutable stamp = 0
        let f x = stamp <- stamp + 1; [| x |]
        
        Array.collect f [| |] |> ignore
        Assert.AreEqual(0, stamp)
        
        stamp <- 0
        Array.collect f [|1;2;3|] |> ignore
        Assert.AreEqual(3, stamp)
        
    [<Fact>]
    member this.``Parallel.Collect`` () =
        this.CollectTester Array.Parallel.collect Array.Parallel.collect

    [<Fact>]
    member this.compareWith() =
        // compareWith should work on empty arrays
        Assert.AreEqual(0,Array.compareWith (fun _ -> failwith "should not be executed")  [||] [||])
        Assert.AreEqual(-1,Array.compareWith (fun _ -> failwith "should not be executed") [||] [|1|])
        Assert.AreEqual(1,Array.compareWith (fun _ -> failwith "should not be executed")  [|"1"|] [||])

        // compareWith should not work on null arrays          
        CheckThrowsArgumentNullException(fun () -> Array.compareWith (fun _ -> failwith "should not be executed") null [||] |> ignore)
        CheckThrowsArgumentNullException(fun () -> Array.compareWith (fun _ -> failwith "should not be executed") [||] null |> ignore)
    
        // compareWith should work on longer arrays
        Assert.AreEqual(-1,Array.compareWith compare [|"1";"2"|] [|"1";"3"|])
        Assert.AreEqual(1,Array.compareWith compare [|1;2;43|] [|1;2;1|])
        Assert.AreEqual(1,Array.compareWith compare [|1;2;3;4|] [|1;2;3|])
        Assert.AreEqual(0,Array.compareWith compare [|1;2;3;4|] [|1;2;3;4|])
        Assert.AreEqual(-1,Array.compareWith compare [|1;2;3|] [|1;2;3;4|])
        Assert.AreEqual(1,Array.compareWith compare [|1;2;3|] [|1;2;2;4|])
        Assert.AreEqual(-1,Array.compareWith compare [|1;2;2|] [|1;2;3;4|])

        // compareWith should use the comparer
        Assert.AreEqual(0,Array.compareWith (fun x y -> 0) [|"1";"2"|] [|"1";"3"|])
        Assert.AreEqual(1,Array.compareWith (fun x y -> 1) [|"1";"2"|] [|"1";"3"|])
        Assert.AreEqual(-1,Array.compareWith (fun x y -> -1) [|"1";"2"|] [|"1";"3"|])
        
    [<Fact>]
    member this.Concat() =
        // integer array
        let seqInt = 
            seq { for i in 1..10 do                
                    yield [|i; i*10|] }
                    
        let conIntArr = Array.concat seqInt
        if Array.length conIntArr <> 20 then Assert.Fail()
        
        // string array
        let strSeq = 
            seq { for a in 'a'..'c' do
                    for b in 'a'..'c' do
                        yield [|a.ToString();b.ToString() |]}
     
        let conStrArr = Array.concat strSeq
        if Array.length conStrArr <> 18 then Assert.Fail()
        
        // Empty array
        let emptyArrays = [| [| |]; [| 0 |]; [| 1 |]; [| |]; [| |] |]
        let result2 = Array.concat emptyArrays
        Assert.True(result2.[0] = 0 && result2.[1] = 1)
        if result2.[0] <> 0 && result2.[1] <> 1 then Assert.Fail()    

        // null array
        let nullArray = null:int[]
        let nullArrays = Array.create 2 nullArray
        CheckThrowsNullRefException (fun () -> Array.concat nullArrays |> ignore) 
                
        () 

    [<Fact>]
    member this.countBy() =
        // countBy should work on empty array
        Assert.AreEqual(0,Array.countBy (fun _ -> failwith "should not be executed") [||] |> Array.length)

        // countBy should not work on null
        CheckThrowsArgumentNullException(fun () -> Array.countBy (fun _ -> failwith "should not be executed") null |> ignore)

        // countBy should count by the given key function
        Assert.AreEqual([| 5,1; 2,2; 3,2 |],Array.countBy id [|5;2;2;3;3|])
        Assert.AreEqual([| 3,3; 2,2; 1,3 |],Array.countBy (fun x -> if x < 3 then x else 3) [|5;2;1;2;3;3;1;1|])

    [<Fact>]
    member this.Copy() =
        // int array
        let intSrc:int [] = [| 3;5;7 |]    
        let intCopyed = Array.copy  intSrc
        if intCopyed <> [| 3;5;7 |] then Assert.Fail()
        
        // string array
        let stringSrc: string [] = [|"Lists"; "are";  "commonly"  |]
        
        let strCopyed = Array.copy  stringSrc   
        if strCopyed <> [|"Lists"; "are";  "commonly"  |] then Assert.Fail()
        
        // empty array
        let emptySrc :int[] = [| |]
        let emptyCopyed = Array.copy emptySrc
        if emptyCopyed <> [| |] then Assert.Fail()

        // null array
        let nullArr = null:int[]    
        CheckThrowsArgumentNullException (fun () -> Array.copy nullArr |> ignore) 
        
        ()

    [<Fact>]
    member this.Create() =
        // int array
        let intArr = Array.create 3 8    
        if intArr <> [| 8;8;8 |] then Assert.Fail()
        
        // string array
        let strArr = Array.create 3 "good"
        Assert.True( (strArr = [|"good"; "good";  "good"|]) )
        
        // empty array
        let emptyArr = Array.create 0 "empty"    
        if emptyArr <> [| |] then Assert.Fail()

        // array with null elements
        let nullStr = null:string  
        let nullArr = Array.create 3 nullStr
        Assert.True( (nullArr = [|null; null; null|]) )
        
        ()

    
    [<Fact>]
    member this.TryHead() =
        // integer array
        let resultInt = Array.tryHead  [|2..2..20|]        
        Assert.AreEqual(2, resultInt.Value)
        
        // string array
        let resultStr = Array.tryHead  [|"a";"b";"c";"d"|]         
        Assert.AreEqual("a", resultStr.Value)

        // empty array   
        let resultNone = Array.tryHead [||]
        Assert.AreEqual(None, resultNone)

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.tryHead nullArr |> ignore) 
        ()
        
    [<Fact>]
    member this.Exists() =
        // integer array
        let intArr = [| 2;4;6;8 |]
        let funcInt x = if (x%2 = 0) then true else false
        let resultInt = Array.exists funcInt intArr
        if resultInt <> true then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" |]
        let funcStr (x:string) = if (x.Length >15) then true else false
        let resultStr = Array.exists funcStr strArr
        if resultStr <> false then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.exists funcInt emptyArr
        if resultEpt <> false then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.exists funcStr nullArr |> ignore) 
        
        ()
        
    [<Fact>]
    member this.Exists2() =
        // integer array
        let intFir = [| 2;4;6;8 |]
        let intSec = [| 1;2;3;4 |]
        let funcInt x y = if (x%y = 0) then true else false
        let resultInt = Array.exists2 funcInt intFir intSec
        if resultInt <> true then Assert.Fail()
        
        // string array
        let strFir = [|"Lists"; "are";  "commonly" |]
        let strSec = [|"good"; "good";  "good"  |]
        let funcStr (x:string) (y:string) = if (x = y) then true else false
        let resultStr = Array.exists2 funcStr strFir strSec
        if resultStr <> false then Assert.Fail()
        
        // empty array
        let eptFir:int[] = [| |]
        let eptSec:int[] = [| |]
        let resultEpt = Array.exists2 funcInt eptFir eptSec
        if resultEpt <> false then Assert.Fail()

        // null array
        let nullFir = null:string[] 
        let validArray = [| "a" |]      
        CheckThrowsArgumentNullException (fun () -> Array.exists2 funcStr nullFir validArray |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Array.exists2 funcStr validArray nullFir |> ignore) 
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.exists2 funcInt [|1..10|] [|2..20|] |> ignore)
        
        ()

    [<Fact>]
    member this.Fill() =
        // integer array
        let intArr = [|1..5|]
        Array.fill intArr 0 3 21
        if intArr <> [|21;21;21;4;5|] then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "data";"structor" |]
        Array.fill strArr 1 5 "a"
        
        if strArr <> [|"Lists"; "a"; "a"; "a"; "a";"a" |] then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        Array.fill emptyArr 0 0 8
        if emptyArr <> [| |] then Assert.Fail()

        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> Array.fill nullArr 0 1 "good" |> ignore)
        
        // start < 0
        CheckThrowsArgumentException(fun () -> Array.fill intArr -1 3 21)
        
        // len < 0        
        CheckThrowsArgumentException(fun () -> Array.fill intArr 1 -2 21)
        
         
        ()

    [<Fact>] 
    member this.Filter() =
        // integer array
        let intArr = [| 1..20 |]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = Array.filter funcInt intArr
        if resultInt <> [|5;10;15;20|] then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "data";"structor" |]
        let funcStr (x:string) = if (x.Length > 4) then true else false
        let resultStr = Array.filter funcStr strArr
        if resultStr <> [|"Lists";  "commonly"; "structor" |] then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.filter funcInt emptyArr
        if resultEpt <> [| |] then Assert.Fail()

        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () ->  Array.filter funcStr nullArr |> ignore) 
        
        ()

    [<Fact>]
    member this.ParallelFilter () = 
        let assertSameBehavior predicate arr =
            let sequentialZip = Array.filter predicate arr
            let paraZip = Array.Parallel.filter predicate arr
            Assert.AreEqual(sequentialZip, paraZip)

        [| 1..20 |] |> assertSameBehavior (fun x -> x%5 = 0)
        [|"Lists"; "are"; "a"; "commonly"; "data";"structor" |] |> assertSameBehavior  (fun x -> x.Length > 4) 
        [| |] |> assertSameBehavior  (fun x -> x%5 = 0)
        let nullArr = null:int[] 
        CheckThrowsArgumentNullException (fun () ->  Array.Parallel.filter (fun x -> x%5 = 0) nullArr |> ignore) 

        
    [<Fact>]
    member this.Filter2 () =
        // The Array.filter algorithm uses a bitmask as a temporary storage mechanism
        // for which elements to filter. This introduces some possible error conditions
        // around how the filter is filled and subsequently used, so filter test
        // does a pretty exhaustive test suite.
        // It works by first generating arrays which consist of sequences of unique
        // positive and negative numbers, as per arguments, it then filters for the
        // positive values, and then compares the results against the original array.

        let makeTestArray size posLength negLength startWithPos startFromEnd =
            let array = Array.zeroCreate size

            let mutable sign  = if startWithPos then 1         else -1
            let mutable count = if startWithPos then posLength else negLength
            for i = 1 to size do
                let idx = if startFromEnd then size-i else i-1
                array.[idx] <- (idx+1) * sign
                count <- count - 1
                if count <= 0 then
                    sign <- sign * -1
                    count <- if sign > 0 then posLength else negLength

            array

        let checkFilter filter (array: _ array) =
            let filtered = array |> filter (fun n -> n > 0)

            let mutable idx = 0
            for item in filtered do
                while array.[idx] < item do
                    idx <- idx + 1
                if item <> array.[idx] then
                    Assert.Fail ()
            idx <- idx + 1
            while idx < array.Length do
                if array.[idx] > 0 then
                    Assert.Fail ()
                idx <- idx + 1

        let checkCombinations filter maxSize =
            for size = 0 to maxSize do
                for posLength = 1 to size do
                    for negLength = 1 to size do
                        for startWithPos in [true; false] do
                            for startFromEnd in [true; false] do
                                let testArray = makeTestArray size posLength negLength startWithPos startFromEnd
                                checkFilter filter testArray

        // this could probably be a bit smaller, but needs to at least be > 64 to test chunk copying
        // of data, and > 96 gives a safer feel, so settle on a nice decimal rounding of one hundred
        // to appease those with digits.
        let suitableTestMaxLength = 100 

        checkCombinations Array.filter suitableTestMaxLength



    [<Fact>]
    member this.Where() =
        // integer array
        let intArr = [| 1..20 |]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = Array.where funcInt intArr
        if resultInt <> [|5;10;15;20|] then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "data";"structor" |]
        let funcStr (x:string) = if (x.Length > 4) then true else false
        let resultStr = Array.where funcStr strArr
        if resultStr <> [|"Lists";  "commonly"; "structor" |] then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.where funcInt emptyArr
        if resultEpt <> [| |] then Assert.Fail()

        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () ->  Array.where funcStr nullArr |> ignore) 
        
        ()   

    [<Fact>]
    member this.``where should work like filter``() =
        Assert.AreEqual([||], Array.where (fun x -> x % 2 = 0) [||])
        Assert.AreEqual([|0;2;4;6;8|], Array.where (fun x -> x % 2 = 0) [|0..9|])
        Assert.AreEqual([|"a";"b";"c"|], Array.where (fun _ -> true) [|"a";"b";"c"|])

    [<Fact>]
    member this.Find() =
        // integer array
        let intArr = [| 1..20 |]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = Array.find funcInt intArr
        if resultInt <> 5 then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "data";"structor" |]
        let funcStr (x:string) = if (x.Length >7) then true else false
        let resultStr = Array.find funcStr strArr
        if resultStr <> "commonly" then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |] 
        CheckThrowsKeyNotFoundException (fun () -> Array.find (fun _ -> true) emptyArr |> ignore)

        // not found
        CheckThrowsKeyNotFoundException (fun () -> Array.find (fun _ -> false) intArr |> ignore)

        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> Array.find funcStr nullArr |> ignore) 
        
        () 

    [<Fact>]
    member this.FindBack() =
        // integer array
        let funcInt x = if (x%5 = 0) then true else false
        Assert.AreEqual(20, Array.findBack funcInt [| 1..20 |])
        Assert.AreEqual(15, Array.findBack funcInt [| 1..19 |])
        Assert.AreEqual(5, Array.findBack funcInt [| 5..9 |])

        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "data";"structor" |]
        let funcStr (x:string) = x.Length > 7
        let resultStr = Array.findBack funcStr strArr
        Assert.AreEqual("structor", resultStr)

        // empty array
        CheckThrowsKeyNotFoundException (fun () -> Array.findBack (fun _ -> true) [| |] |> ignore)

        // not found
        CheckThrowsKeyNotFoundException (fun () -> Array.findBack (fun _ -> false) [| 1..20 |] |> ignore)

        // null array
        let nullArr = null:string[]
        CheckThrowsArgumentNullException (fun () -> Array.findBack funcStr nullArr |> ignore)

        ()

    [<Fact>]
    member this.FindIndex() =
        // integer array
        let intArr = [| 1..20 |]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = Array.findIndex funcInt intArr
        if resultInt <> 4 then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "data";"structor" |]
        let funcStr (x:string) = if (x.Length >7) then true else false
        let resultStr = Array.findIndex funcStr strArr
        if resultStr <> 3 then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]  
        CheckThrowsKeyNotFoundException(fun() -> Array.findIndex (fun _ -> true) emptyArr |> ignore)
        
        // not found
        CheckThrowsKeyNotFoundException(fun() -> Array.findIndex (fun _ -> false) intArr |> ignore)

        // null array
        let nullArr = null:string[]  
        CheckThrowsArgumentNullException (fun () -> Array.findIndex funcStr nullArr |> ignore) 
        
        () 
        
    [<Fact>]
    member this.FindIndexBack() =
        // integer array
        let funcInt x = if (x%5 = 0) then true else false
        Assert.AreEqual(19, Array.findIndexBack funcInt [| 1..20 |])
        Assert.AreEqual(14, Array.findIndexBack funcInt [| 1..19 |])
        Assert.AreEqual(0, Array.findIndexBack funcInt [| 5..9 |])

        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "data";"structor" |]
        let funcStr (x:string) = if (x.Length >7) then true else false
        let resultStr = Array.findIndexBack funcStr strArr
        Assert.AreEqual(5, resultStr)

        // empty array
        CheckThrowsKeyNotFoundException(fun() -> Array.findIndexBack (fun _ -> true) [| |] |> ignore)

        // not found
        CheckThrowsKeyNotFoundException(fun() -> Array.findIndexBack (fun _ -> false) [| 1..20 |] |> ignore)

        // null array
        let nullArr = null:string[]
        CheckThrowsArgumentNullException (fun () -> Array.findIndexBack funcStr nullArr |> ignore)

        ()

    [<Fact>]
    member this.Pick() =
        // integers
        let intArr = [| 1..10 |]
        let matchFunc n =
            if n = 3 then Some(n.ToString())
            else None
        let resultInt = Array.pick matchFunc intArr
        Assert.AreEqual("3", resultInt)
        
        // make it not found
        CheckThrowsKeyNotFoundException (fun () -> Array.pick (fun n -> None) intArr |> ignore)

    [<Fact>]
    member this.last() =
        // last should fail on empty array
        CheckThrowsArgumentException(fun () -> Array.last [||] |> ignore)

        // last should fail on null
        CheckThrowsArgumentNullException(fun () -> Array.last null |> ignore)

        // last should return the last element from arrays
        Assert.AreEqual(1, Array.last [|1|])
        Assert.AreEqual("2", Array.last [|"1"; "3"; "2"|])
        Assert.AreEqual(["4"], Array.last [|["1"; "3"]; []; ["4"]|])
    
    [<Fact>]
    member this.TryLast() =
        // integers array
        let IntSeq = [| 1..9 |]
        let intResult = Array.tryLast IntSeq
        Assert.AreEqual(9, intResult.Value)
                 
        // string array
        let strResult = Array.tryLast [|"first"; "second";  "third"|]
        Assert.AreEqual("third", strResult.Value)
         
        // Empty array
        let emptyResult = Array.tryLast Array.empty
        Assert.True(emptyResult.IsNone)
      
        // null array
        let nullArr = null:string[]  
        CheckThrowsArgumentNullException (fun () ->Array.tryLast nullArr |> ignore) 
        () 

    [<Fact>]
    member this.ToSeq() =
        let intArr = [| 1..10 |]
        let seq = Array.toSeq intArr
        let sum = Seq.sum seq
        Assert.AreEqual(55, sum)        

    member private _.TryPickTester tryPickInt tryPickString =
        // integer array
        let intArr = [| 1..10 |]    
        let funcInt x = 
                match x with
                | _ when x % 3 = 0 -> Some (x.ToString())            
                | _ -> None
        let resultInt = tryPickInt funcInt intArr
        if resultInt <> Some "3" then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" ; "list" |]
        let funcStr x = 
                match x with
                | "good" -> Some (x.ToString())            
                | _ -> None
        let resultStr = tryPickString funcStr strArr
        if resultStr <> None then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = tryPickInt funcInt emptyArr
        if resultEpt <> None then Assert.Fail()

        // null array
        let nullArr = null:string[]  
        CheckThrowsArgumentNullException (fun () -> tryPickString funcStr nullArr |> ignore)  
        
        ()

    [<Fact>]
    member this.TryPick() = this.TryPickTester Array.tryPick Array.tryPick

    [<Fact>]
    member this.ParallelTryPick() = this.TryPickTester Array.Parallel.tryPick Array.Parallel.tryPick

    [<Fact>]
    member this.Fold() =
        // integer array
        let intArr = [| 1..5 |]    
        let funcInt x y = x+"+"+y.ToString()
        let resultInt = Array.fold funcInt "x" intArr
        if resultInt <> "x+1+2+3+4+5" then Assert.Fail()
        
        // string array
        let strArr = [|"A"; "B";  "C" ; "D" |]
        let funcStr x y = x+y
            
        let resultStr = Array.fold funcStr "X" strArr
        if resultStr <> "XABCD" then Assert.Fail()
        
        // empty array
        let emptyArr : int[] = [| |]
        let resultEpt = Array.fold funcInt "x" emptyArr
        if resultEpt <> "x" then Assert.Fail()

        // null array
        let nullArr = null : string[] 
        CheckThrowsArgumentNullException (fun () -> Array.fold funcStr "begin" nullArr |> ignore)  
        
        ()

    [<Fact>]
    member this.Fold2() =
        // integer array  
        let funcInt x y z = x + y.ToString() + z.ToString()
        let resultInt = Array.fold2 funcInt "x" [| 1;3;5 |]  [|2;4;6|]
        if resultInt <> "x123456" then Assert.Fail()
        
        // string array
        let funcStr x y z= x + y + z        
        let resultStr = Array.fold2 funcStr "X" [|"A"; "B";  "C" ; "D" |] [|"H"; "I";  "J" ; "K" |]
        if resultStr <> "XAHBICJDK" then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.fold2 funcInt "x" emptyArr emptyArr
        if resultEpt <> "x" then Assert.Fail()

        // null array
        let nullArr = null:string[]
        let validArray = [| "a" |]
        CheckThrowsArgumentNullException (fun () -> Array.fold2 funcStr "begin" validArray nullArr |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Array.fold2 funcStr "begin" nullArr validArray |> ignore)  
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.fold2 funcInt "x" [| 1;3;5 |]  [|2;4;6;8|] |> ignore)
                
        ()

    [<Fact>]
    member this.FoldBack() =
        // integer array
        let intArr = [| 1..5 |]    
        let funcInt x y = x.ToString()+y
        let resultInt = Array.foldBack funcInt intArr "x"
        if resultInt <> "12345x" then Assert.Fail()
        
        // string array
        let strArr = [|"A"; "B";  "C" ; "D" |]
        let funcStr x y = x+y
            
        let resultStr = Array.foldBack funcStr strArr "X" 
        if resultStr <> "ABCDX" then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.foldBack funcInt emptyArr "x" 
        if resultEpt <> "x" then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.foldBack funcStr nullArr "begin" |> ignore)  
        
        ()

    [<Fact>]
    member this.FoldBack2() =
        // integer array  
        let funcInt x y z = x.ToString() + y.ToString() + z
        let resultInt = Array.foldBack2 funcInt  [| 1;3;5 |]  [|2;4;6|] "x"
        if resultInt <> "123456x" then Assert.Fail()
        
        // string array
        let funcStr x y z= x + y + z        
        let resultStr = Array.foldBack2 funcStr [|"A"; "B";  "C" ; "D" |] [|"H"; "I";  "J" ; "K" |] "X"
        if resultStr <> "AHBICJDKX" then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.foldBack2 funcInt emptyArr emptyArr "x"
        if resultEpt <> "x" then Assert.Fail()

        // null array
        let nullArr = null : string[] 
        let validArray = [| "a" |] 
        CheckThrowsArgumentNullException (fun () -> Array.foldBack2 funcStr nullArr validArray "begin" |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Array.foldBack2 funcStr validArray nullArr "begin" |> ignore)  
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.foldBack2 funcInt [|1..10|] [|2..20|] "x" |> ignore)
        
        ()

    [<Fact>]
    member this.ForAll() =
        // integer array
        let resultInt = Array.forall (fun x -> x > 2) [| 3..2..10 |]
        if resultInt <> true then Assert.Fail()
        
        // string array
        let resultStr = Array.forall (fun (x:string) -> x.Contains("a")) [|"Lists"; "are";  "commonly" ; "list" |]
        if resultStr <> false then Assert.Fail()
        
        // empty array 
        let resultEpt = Array.forall (fun (x:string) -> x.Contains("a")) [||] 
        if resultEpt <> true then Assert.Fail()

        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> Array.forall (fun x -> true) nullArr |> ignore)  
        
        ()

    [<Fact>]
    member this.ParallelForAll() =
        let inline assertSame predicate array =
            let seq = Array.forall predicate array
            let para = Array.Parallel.forall predicate array    
            Assert.AreEqual(seq, para, sprintf "%A" array)

        [| 3..2..10 |] |> assertSame (fun x -> x > 15)
        [| 3..2..10 |] |> assertSame (fun x -> x < 15)
        [|"Lists"; "are";  "commonly" ; "list" |] |> assertSame (fun (x:string) -> x.Contains("a")) 
        [||] |> assertSame (fun (x:string) -> x.Contains("a"))
        [||] |> assertSame (fun (x:string) -> x.Contains("a") |> not)
       
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> Array.Parallel.forall (fun x -> true) nullArr |> ignore)  
        
        ()

    [<Fact>]
    member this.ParallelExists() =
        let inline assertSame predicate array =
            let seq = Array.exists predicate array
            let para = Array.Parallel.exists predicate array    
            Assert.AreEqual(seq, para, sprintf "%A" array)

        [| 3..2..10 |] |> assertSame (fun x -> x > 2)
        [|"Lists"; "are";  "commonly" ; "list" |] |> assertSame (fun (x:string) -> x.Contains("a")) 
        [||] |> assertSame (fun (x:string) -> x.Contains("a"))
        [||] |> assertSame (fun (x:string) -> x.Contains("a") |> not)
       
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> Array.Parallel.exists (fun x -> true) nullArr |> ignore)  
        
        ()
        
    [<Fact>]
    member this.ForAll2() =
        // integer array
        let resultInt = Array.forall2 (fun x y -> x < y) [| 1..10 |] [|2..2..20|]
        if resultInt <> true then Assert.Fail()
        
        // string array
        let resultStr = Array.forall2 (fun (x:string) (y:string) -> x.Length < y.Length) [|"Lists"; "are";  "commonly" ; "list" |] [|"Listslong"; "arelong";  "commonlylong" ; "listlong" |]
        if resultStr <> true then Assert.Fail()
        
        // empty array 
        let resultEpt = Array.forall2 (fun x y -> x>y) [||] [||]
        if resultEpt <> true then Assert.Fail()

        // null array
        let nullArr = null:string[]
        let validArray = [| "a" |] 
        CheckThrowsArgumentNullException (fun () -> Array.forall2 (fun x y-> true) nullArr validArray |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Array.forall2 (fun x y-> true) validArray nullArr |> ignore)  
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.forall2 (fun x y -> x < y) [|1..10|] [|2..20|] |> ignore)
        
        ()
        
    [<Fact>]
    member this.Get() =
        // integer array
        let intArr = [| 3;4;7;8;10 |]    
        let resultInt = Array.get intArr 3
        if resultInt <> 8 then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" ; "list" |]
        
        let resultStr = Array.get strArr 2
        if resultStr <> "commonly" then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        CheckThrowsIndexOutRangException (fun () -> Array.get emptyArr -1 |> ignore)

        // null array
        let nullArr = null:string[] 
        CheckThrowsNullRefException (fun () -> Array.get nullArr 0 |> ignore)  
        
        ()

    [<Fact>]
    member this.``exactlyOne should return the element from singleton arrays``() =
        Assert.AreEqual(1, Array.exactlyOne [|1|])
        Assert.AreEqual("2", Array.exactlyOne [|"2"|])
        ()

    [<Fact>]
    member this.``exactlyOne should fail on empty array``() =
        CheckThrowsArgumentException(fun () -> Array.exactlyOne [||] |> ignore)

    [<Fact>]
    member this.``exactlyOne should fail on null array``() =
        CheckThrowsArgumentNullException(fun () -> Array.exactlyOne null |> ignore)

    [<Fact>]
    member this.``exactlyOne should fail on arrays with more than one element``() =
        CheckThrowsArgumentException(fun () -> Array.exactlyOne [|"1"; "2"|] |> ignore)

    [<Fact>]
    member this.``tryExactlyOne should return the element from singleton arrays``() =
        Assert.AreEqual(Some 1, Array.tryExactlyOne [|1|])
        Assert.AreEqual(Some "2", Array.tryExactlyOne [|"2"|])
        ()

    [<Fact>]
    member this.``tryExactlyOne should return None on empty array``() =
        Assert.AreEqual(None, Array.tryExactlyOne [||])

    [<Fact>]
    member this.``tryExactlyOne should return None for arrays with more than one element``() =
        Assert.AreEqual(None, Array.tryExactlyOne [|"1"; "2"|])

    [<Fact>]
    member this.``tryExactlyOne should fail on null array``() =
        CheckThrowsArgumentNullException(fun () -> Array.tryExactlyOne null |> ignore)

    [<Fact>]
    member this.GroupBy() =
        let funcInt x = x%5
             
        let IntArray = [| 0 .. 9 |]
                    
        let group_byInt = Array.groupBy funcInt IntArray
        
        let expectedIntArray = 
            [| for i in 0..4 -> i, [|i; i+5|] |]

        if group_byInt <> expectedIntArray then Assert.Fail()
             
        // string array
        let funcStr (x:string) = x.Length
        let strArray = [|"l1ngth7"; "length 8";  "l2ngth7" ; "length  9"|]
        
        let group_byStr = Array.groupBy funcStr strArray
        let expectedStrArray = 
            [|
                7, [|"l1ngth7"; "l2ngth7"|]
                8, [|"length 8"|]
                9, [|"length  9"|]
            |]
       
        if group_byStr <> expectedStrArray then Assert.Fail()

        // Empty array
        let emptyArray = [||]
        let group_byEmpty = Array.groupBy funcInt emptyArray
        let expectedEmptyArray = [||]

        if emptyArray <> expectedEmptyArray then Assert.Fail()

        CheckThrowsArgumentNullException(fun () -> Array.groupBy funcInt (null : int array) |> ignore)
        ()

    [<Fact>]
    member _.ParallelGroupBy() =

        let assertEqualityOfGroupings opName (seqGroup: ('TKey * 'TVal[])[]) (paraGroup: ('TKey * 'TVal[])[]) =
            seqGroup |> Array.sortInPlaceBy fst
            paraGroup |> Array.sortInPlaceBy fst

            seqGroup |> Array.iter (snd >> Array.sortInPlace)
            paraGroup |> Array.iter (snd >> Array.sortInPlace)

            if seqGroup.Length <> paraGroup.Length then                
                Assert.Fail($"{opName} produced different lengths of results. Seq={seqGroup.Length};Para={paraGroup.Length}.")
            
            let seqKeys = seqGroup |> Array.map fst
            let paraKeys = paraGroup |> Array.map fst
            if(seqKeys <> paraKeys) then
                Assert.Fail($"{opName} produced different keys. Seq=%A{seqKeys};Para=%A{paraKeys}.")

            Array.zip seqGroup paraGroup
            |> Array.iter (fun ((seqKey,seqGroup), (paraKey,paraGroup)) ->
                Assert.AreEqual(seqKey,paraKey,opName)
                if seqGroup <> paraGroup then
                    Assert.Fail($"{opName} produced different results for key={seqKey}. Seq=%A{seqGroup};Para=%A{paraGroup}."))

            Assert.True((seqGroup=paraGroup), $"{opName} produced different results. Seq=%A{seqGroup};Para=%A{paraGroup}.")
            

        let compareAndAssert opName array projection =
            let seqGroup = array |> Array.groupBy projection
            let paraGroup = array |> Array.Parallel.groupBy projection
            assertEqualityOfGroupings opName seqGroup paraGroup

        // int array
        let funcInt x = x%5             
        let IntArray = [| 0 .. 250 |]
        compareAndAssert "Int grouping" IntArray funcInt  

             
        // string array
        let funcStr (x:string) = x.Length
        let strArray = Array.init 177 (fun i -> string i)     
        compareAndAssert "String grouping" strArray funcStr        


        // Empty array
        compareAndAssert "Empty group" [||] funcInt

        // Reference key which can be null
        let sampleStringsCanBeNull = [|"a";null;"abc";String.Empty|]
        let pickStringByIdx idx = sampleStringsCanBeNull[idx % sampleStringsCanBeNull.Length]
        compareAndAssert "Key can be null" IntArray pickStringByIdx

        //String array w/ null keys and values
        let strArray = Array.init 222 (fun i -> if i%3=0 then String.Empty else null )
        compareAndAssert "String grouping w/ nulls" strArray id

        // Keys being special floats. Array.groupBy does not work here, we test results manually
        let specialFloats = [|infinity; -infinity;-0.0; 0.0; 1.0; -1.0; -0.0/0.0; -nan|]
        let pickSpecialFloatByIdx idx = specialFloats[idx % specialFloats.Length]

        let paraGroup = IntArray |> Array.Parallel.groupBy pickSpecialFloatByIdx
        Assert.AreEqual(6, paraGroup.Length, "There should be 6 special floats!")
        let (nan,nansGroup) = paraGroup |> Array.find (fun (k,_) -> Double.IsNaN(k))
        // Both -0.0/0.0; -nan are a Nan. => 2/8 => every 4th elements goes to the NaN bucket
        Assert.AreEqual((IntArray.Length / 4), nansGroup.Length, $"There should be {(IntArray.Length / 4)} NaNs!")



        CheckThrowsArgumentNullException(fun () -> Array.Parallel.groupBy funcInt (null : int array) |> ignore)
        ()

    member private this.InitTester initInt initString = 
        // integer array
        let resultInt : int[] = initInt 3 (fun x -> x + 3) 
        if resultInt <> [|3;4;5|] then Assert.Fail()
        
        // string array
        let funStr (x:int) = 
            match x with
            | 0 -> "Lists"
            | 1 -> "are"
            | 2 -> "commonly"
            | _ -> "end"    
        let resultStr = initString 3 funStr
        if resultStr <> [|"Lists"; "are";  "commonly"  |] then Assert.Fail()
        
        // empty array  
        let resultEpt = initInt 0 (fun x -> x+1)
        if resultEpt <> [| |] then Assert.Fail()
        
        ()

    [<Fact>]
    member this.Hd() =
        // integer array
        let resultInt = Array.head [|2..2..20|]
        Assert.AreEqual(2, resultInt)
        
        // string array
        let resultStr = Array.head [|"a";"b";"c";"d"|] 
        Assert.AreEqual("a", resultStr)
            
        CheckThrowsArgumentException(fun () -> Array.head [||] |> ignore)        
        CheckThrowsArgumentNullException(fun () -> Array.head null |> ignore)
        ()    

    [<Fact>]
    member this.Init() = 
        this.InitTester Array.init Array.init
        
    [<Fact>]
    member this.InitWithSideEffects () =
        let mutable stamp = 0
        let f i = 
            stamp <- stamp + 1
            i 
        Array.init 0 f |> ignore
        Assert.AreEqual (0, stamp)
        
        stamp <- 0
        Array.init 10 f |> ignore
        Assert.AreEqual (10, stamp)
        
    [<Fact>]
    member this.``Parallel.Init``() = 
        this.InitTester Array.Parallel.init Array.Parallel.init

    [<Fact>]
    member this.IsEmpty() =
        // integer array
        let intArr = [| 3;4;7;8;10 |]    
        let resultInt = Array.isEmpty intArr 
        if resultInt <> false then Assert.Fail()
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" ; "list" |]    
        let resultStr = Array.isEmpty strArr 
        if resultStr <> false then Assert.Fail()
        
        // empty array    
        let emptyArr:int[] = [| |]
        let resultEpt = Array.isEmpty emptyArr 
        if resultEpt <> true then Assert.Fail()

        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> Array.isEmpty nullArr |> ignore)  
        
        ()

    [<Fact>]
    member this.Iter() =
        // integer array
        let intArr = [| 1..10 |]  
        let mutable resultInt = 0    
        let funInt (x:int) =   
            resultInt <- resultInt + x              
            () 
        Array.iter funInt intArr 
        if resultInt <> 55 then Assert.Fail()    
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" ; "list" |]
        let mutable resultStr = ""
        let funStr (x: string) =
            resultStr <- resultStr + x   
            ()
        Array.iter funStr strArr  
        if resultStr <> "Listsarecommonlylist" then Assert.Fail()   
        
        // empty array    
        let emptyArr : int[] = [| |]
        let mutable resultEpt = 0
        Array.iter funInt emptyArr 
        if resultEpt <> 0 then Assert.Fail()    

        // null array
        let nullArr = null : string[]  
        CheckThrowsArgumentNullException (fun () -> Array.iter funStr nullArr |> ignore)  
        
        ()
       
    [<Fact>]
    member this.Iter2() =
        // integer array
        let mutable resultInt = 0    
        let funInt (x:int) (y:int) =   
            resultInt <- resultInt + x + y             
            () 
        Array.iter2 funInt [| 1..10 |] [|2..2..20|] 
        if resultInt <> 165 then Assert.Fail()    
        
        // string array
        let mutable resultStr = ""
        let funStr (x:string) (y:string) =
            resultStr <- resultStr + x  + y 
            ()
        Array.iter2 funStr [|"A"; "B";  "C" ; "D" |] [|"a"; "b"; "c"; "d"|]  
        if resultStr <> "AaBbCcDd" then Assert.Fail()   
        
        // empty array    
        let emptyArr:int[] = [| |]
        let mutable resultEpt = 0
        Array.iter2 funInt emptyArr emptyArr 
        if resultEpt <> 0 then Assert.Fail()    

        // null array
        let nullArr = null:string[]  
        let validArray = [| "a" |]     
        CheckThrowsArgumentNullException (fun () -> Array.iter2 funStr nullArr validArray |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Array.iter2 funStr validArray nullArr |> ignore)  
        
        // len1 <> len2        
        CheckThrowsArgumentException(fun () -> Array.iter2 funInt [| 1..10 |] [|2..20|])
  
        ()
        
        
    [<Fact>]
    member this.Iteri() =
        // integer array
        let intArr = [| 1..10 |]  
        let mutable resultInt = 0    
        let funInt (x:int) y =   
            resultInt <- resultInt + x + y             
            () 
        Array.iteri funInt intArr 
        if resultInt <> 100 then Assert.Fail()    
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" ; "list" |]
        let mutable resultStr = 0
        let funStr (x:int) (y:string) =
            resultStr <- resultStr + x + y.Length
            ()
        Array.iteri funStr strArr  
        if resultStr <> 26 then Assert.Fail()   
        
        // empty array    
        let emptyArr:int[] = [| |]
        let mutable resultEpt = 0
        Array.iteri funInt emptyArr 
        if resultEpt <> 0 then Assert.Fail()    

        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> Array.iteri funStr nullArr |> ignore)  
        
        ()
        
    [<Fact>]
    member this.Iteri2() =
        // integer array
        let mutable resultInt = 0    
        let funInt (x:int) (y:int) (z:int) =   
            resultInt <- resultInt + x + y + z            
            () 
        Array.iteri2 funInt [| 1..10 |] [|2..2..20|] 
        if resultInt <> 210 then Assert.Fail()    
        
        // string array
        let mutable resultStr = ""
        let funStr (x:int) (y:string) (z:string) =
            resultStr <- resultStr + x.ToString()  + y + z
            ()
        Array.iteri2 funStr [|"A"; "B";  "C" ; "D" |] [|"a"; "b"; "c"; "d"|]  
        if resultStr <> "0Aa1Bb2Cc3Dd" then Assert.Fail()   
        
        // empty array    
        let emptyArr:int[] = [| |]
        let mutable resultEpt = 0
        Array.iteri2 funInt emptyArr emptyArr 
        if resultEpt <> 0 then Assert.Fail()    

        // null array
        let nullArr = null:string[]
        let validArray = [| "a" |] 
        CheckThrowsArgumentNullException (fun () -> Array.iteri2 funStr nullArr validArray |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Array.iteri2 funStr validArray nullArr |> ignore)  
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.iteri2 funInt [| 1..10 |] [|2..20|]  |> ignore)
        
        ()                

    [<Fact>]
    member this.``pairwise should return pairs of the input array``() =
        Assert.AreEqual([||],Array.pairwise [||])
        Assert.AreEqual([||],Array.pairwise [|1|])
        Assert.AreEqual([|1,2|],Array.pairwise [|1;2|])
        Assert.AreEqual([|1,2; 2,3|],Array.pairwise [|1;2;3|])
        Assert.AreEqual([|"H","E"; "E","L"; "L","L"; "L","O"|],Array.pairwise [|"H";"E";"L";"L";"O"|])

    [<Fact>]
    member this.``pairwise should not work on null``() =
        CheckThrowsArgumentNullException(fun () -> Array.pairwise null |> ignore)

    member private this.MapTester mapInt (mapString : (string -> int) -> string array -> int array) =
        // empty array 
        let f x = x + 1
        let result = mapInt f [| |]
        if result <> [| |] then Assert.Fail ()
        
        // int array
        let result = mapInt f [| 1..100 |]
        if result <> [| 2..101 |] then Assert.Fail ()
        
        // string array
        let result = [| "a"; "aa"; "aaa" |] |> mapString (fun s -> s.Length) 
        if result <> [| 1..3 |] then Assert.Fail ()
        
        // null array
        let nullArg : int [] = null
        CheckThrowsArgumentNullException (fun () -> mapInt f nullArg |> ignore)
        
        ()
        
    [<Fact>]  
    member this.Map () =
        this.MapTester Array.map Array.map
        
    [<Fact>]
    member _.MapWithSideEffects () =
        let mutable stamp = 0
        let f x = stamp <- stamp + 1; x + 1
        
        Array.map f [| |] |> ignore
        Assert.AreEqual(0,stamp)
        
        stamp <- 0
        Array.map f [| 1..100 |] |> ignore
        Assert.AreEqual(100,stamp)
        
    [<Fact>]
    member this.``Parallel.Map`` () =
        this.MapTester Array.Parallel.map Array.Parallel.map

    member private _.MapiTester mapiInt mapiString =
        // empty array 
        let f i x = (i, x + 1)
        let result = mapiInt f [| |]
        if result <> [| |] then Assert.Fail ()
        
        // int array
        let result : (int*int) array = mapiInt f [| 1..2 |]
        if result <> [| (0,2); (1,3) |] then Assert.Fail ()
        
        // string array
        let result : (int*int) array = [| "a"; "aa"; "aaa" |] |> mapiString (fun i (s:string) -> i, s.Length) 
        if result <> [| (0,1); (1,2); (2,3) |] then Assert.Fail ()
        
        // null array
        let nullArg : int [] = null
        CheckThrowsArgumentNullException (fun () -> mapiInt f nullArg |> ignore)        
        ()

    [<Fact>]
    member this.Mapi () = this.MapiTester Array.mapi Array.mapi
        

    [<Fact>]
    member _.MapiWithSideEffects () =
        let mutable stamp = 0
        let f i x = stamp <- stamp + 1; (i, x + 1)
       
        Array.mapi f [| |] |> ignore
        Assert.AreEqual(0,stamp)
       
        stamp <- 0
        Array.mapi f [| 1..100 |] |> ignore
        Assert.AreEqual(100,stamp)
        ()
        
    [<Fact>]
    member this.``Parallel.Mapi`` () =
        this.MapiTester Array.Parallel.mapi Array.Parallel.mapi
        ()
        
    [<Fact>]
    member _.``Parallel.Iter``() =
        // integer array
        let intArr = [| 1..10 |]  
        let lockObj = obj()
        let mutable resultInt = 0    
        let funInt (x:int) =   
            lock lockObj (fun () -> resultInt <- resultInt + x)
            () 
        Array.Parallel.iter funInt intArr 
        if resultInt <> 55 then Assert.Fail()    
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" ; "list" |]
        let mutable resultStr = 0
        let funStr (x : string) =
            lock lockObj (fun () -> resultStr <- resultStr + x.Length)
            ()
        Array.Parallel.iter funStr strArr  
        if resultStr <> 20 then Assert.Fail()   
        
        // empty array    
        let emptyArr : int[] = [| |]
        let mutable resultEpt = 0
        Array.Parallel.iter funInt emptyArr 
        if resultEpt <> 0 then Assert.Fail()    

        // null array
        let nullArr = null : string[]  
        CheckThrowsArgumentNullException (fun () -> Array.Parallel.iter funStr nullArr |> ignore)  
        
        ()
        
    [<Fact>]
    member _.``Parallel.Iteri``() =   
        // integer array
        let intArr = [| 1..10 |] 
                 
        let lockObj = obj()
        let mutable resultInt = 0    
        let funInt (x:int) y =   
            lock lockObj (fun () -> resultInt <- resultInt + x + y)
            () 
        Array.Parallel.iteri funInt intArr 
        if resultInt <> 100 then Assert.Fail()    
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" ; "list" |]
        let mutable resultStr = 0
        let funStr (x:int) (y:string) =
            lock lockObj (fun () -> resultStr <- resultStr + x + y.Length)
            ()
        Array.Parallel.iteri funStr strArr  
        if resultStr <> 26 then Assert.Fail()   
        
        // empty array    
        let emptyArr:int[] = [| |]
        let mutable resultEpt = 0
        Array.Parallel.iteri funInt emptyArr 
        if resultEpt <> 0 then Assert.Fail()    

        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> Array.Parallel.iteri funStr nullArr |> ignore)  
        
        ()
    
    member private this.PartitionTester partInt partString =
        // int array
        let intSrc:int [] = [| 1..100 |]    
        let funcInt x = if (x%2=1) then true else false
        let intPartitioned : int[] * int[] = partInt funcInt intSrc
        if ([|1..2..100|],[|2..2..100|]) <> intPartitioned then Assert.Fail ()
        
        let allLeft = partInt (fun _ -> true) intSrc
        if (intSrc, [||]) <> allLeft then Assert.Fail()
        let allRight = partInt (fun _ -> false) intSrc
        if ([||], intSrc) <> allRight then Assert.Fail()

        
        // string array
        let stringSrc: string [] = "List 1 list 2 3 4 5".Split([|' '|], System.StringSplitOptions.RemoveEmptyEntries)
        let funcString x = match x with
                           | "list"-> true
                           | "List" -> true
                           | _ -> false
        let strPartitioned : string[] * string[]  = partString funcString stringSrc   
        if strPartitioned <> ([|"List";"list"|], [| "1";"2"; "3"; "4"; "5"|]) then Assert.Fail ()
        
        // empty array
        let emptySrc :int[] = [| |]
        let emptyPartitioned = partInt funcInt emptySrc
        if emptyPartitioned <> ([| |], [| |]) then Assert.Fail()
        
        // null array
        let nullArr = null:string[] 
        CheckThrowsArgumentNullException (fun () -> partString funcString nullArr |> ignore)
        
        
    [<Fact>]
    member this.Partition () =
        this.PartitionTester Array.partition Array.partition    

    [<Fact>]
    member _.Singleton() =
        Assert.AreEqual([|null|],Array.singleton null)
        Assert.AreEqual([|"1"|],Array.singleton "1")
        Assert.AreEqual([|[]|], Array.singleton [])
        Assert.True([|[||]|] = Array.singleton [||])

    [<Fact>]
    member this.``Parallel.Partition`` () =
        this.PartitionTester Array.Parallel.partition Array.Parallel.partition    

    [<Fact>]
    member _.Contains() =
        // integer array
        let intArr = [| 2;4;6;8 |]
        let resultInt = Array.contains 6 intArr
        Assert.True(resultInt)

        // string array
        let strArr = [|"Lists"; "are"; "commonly"|]
        let resultStr = Array.contains "not" strArr
        Assert.False(resultStr)

        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.contains 4 emptyArr
        Assert.False(resultEpt)

        // null array
        let nullArr = null:string[]
        CheckThrowsArgumentNullException (fun () -> Array.contains "empty" nullArr |> ignore)

    [<Fact>]
    member _.``Slicing with first index reverse behaves as expected``()  = 
        let arr = [| 1;2;3;4;5 |]

        Assert.AreEqual(arr.[^3..], arr.[1..])

    
    [<Fact>]
    member _.``Slicing with second index reverse behaves as expected``()  = 
        let arr = [| 1;2;3;4;5 |]

        Assert.AreEqual(arr.[..^1], arr.[..3])

    
    [<Fact>]
    member _.``Slicing with both index reverse behaves as expected``()  = 
        let arr = [| 1;2;3;4;5 |]

        Assert.AreEqual(arr.[^3..^1], arr.[1..3])

    [<Fact>]
    member _.``Slicing with first index reverse and second index non reverse behaves as expected``()=
        let arr = [|1;2;3;4;5|]

        Assert.AreEqual(arr.[^3..4], arr.[1..4])

    [<Fact>]
    member _.``Slicing with first index non reverse and second index reverse behaves as expected``()=
        let arr = [|1;2;3;4;5|]

        Assert.AreEqual(arr.[3..^0], arr.[3..4])

    [<Fact>]
    member _.``Set slice with first index reverse behaves as expected``()  = 
        let arr1 = [| 1;2;3;4;5 |]
        let arr2 = [| 1;2;3;4;5 |]

        arr1.[^3..] <- [| 9;8;7;6 |]
        arr2.[1..] <- [| 9;8;7;6 |]

        Assert.AreEqual(arr1, arr2)

    [<Fact>]
    member _.``Set slice with second index reverse behaves as expected``()  = 
        let arr1 = [| 1;2;3;4;5 |]
        let arr2 = [| 1;2;3;4;5 |]

        arr1.[..^1] <- [| 9;8;7;6 |]
        arr2.[..3] <- [| 9;8;7;6 |]

        Assert.AreEqual(arr1, arr2)

    [<Fact>]
    member _.``Set slice with both index reverse behaves as expected``()  = 
        let arr1 = [| 1;2;3;4;5 |]
        let arr2 = [| 1;2;3;4;5 |]

        arr1.[^3..^1] <- [| 8;7;6 |]
        arr2.[1..3] <- [| 8;7;6 |]

        Assert.AreEqual(arr1, arr2)

    [<Fact>]
    member _.``Get item with reverse index behaves as expected``() = 
        let arr = [|1;2;3;4;5|]
        Assert.AreEqual(arr.[^1], 4)

    [<Fact>]
    member _.``Set item with reverse index behaves as expected``() = 
        let arr = [|1;2;3;4;5|]

        arr.[^0] <- 9
        Assert.AreEqual(arr.[4], 9)

    [<Fact>] 
    member _.SlicingUnboundedEnd() = 
        let arr = [|1;2;3;4;5;6|]

        Assert.AreEqual(arr.[-1..], arr)
        Assert.AreEqual(arr.[0..], arr)
        Assert.AreEqual(arr.[1..], [2;3;4;5;6])
        Assert.AreEqual(arr.[2..], [3;4;5;6])
        Assert.AreEqual(arr.[5..], [6])
        Assert.AreEqual(arr.[6..], ([||]: int array))
        Assert.AreEqual(arr.[7..], ([||]: int array))

    
    [<Fact>] 
    member _.SlicingUnboundedStart() = 
        let arr = [|1;2;3;4;5;6|]

        Assert.AreEqual(arr.[..(-1)], ([||]: int array))
        Assert.AreEqual(arr.[..0], [|1|])
        Assert.AreEqual(arr.[..1], [|1;2|])
        Assert.AreEqual(arr.[..2], [|1;2;3|])
        Assert.AreEqual(arr.[..3], [|1;2;3;4|])
        Assert.AreEqual(arr.[..4], [|1;2;3;4;5|])
        Assert.AreEqual(arr.[..5], [|1;2;3;4;5;6|])
        Assert.AreEqual(arr.[..6], [|1;2;3;4;5;6|])
        Assert.AreEqual(arr.[..7], [|1;2;3;4;5;6|])


    [<Fact>]
    member _.SlicingBoundedStartEnd() =
        let arr = [|1;2;3;4;5;6|]

        Assert.AreEqual(arr.[*], arr)

        Assert.AreEqual(arr.[0..0], [|1|])
        Assert.AreEqual(arr.[0..1], [|1;2|])
        Assert.AreEqual(arr.[0..2], [|1;2;3|])
        Assert.AreEqual(arr.[0..3], [|1;2;3;4|])
        Assert.AreEqual(arr.[0..4], [|1;2;3;4;5|])
        Assert.AreEqual(arr.[0..5], [|1;2;3;4;5;6|])

        Assert.AreEqual(arr.[1..1], [|2|])
        Assert.AreEqual(arr.[1..2], [|2;3|])
        Assert.AreEqual(arr.[1..3], [|2;3;4|])
        Assert.AreEqual(arr.[1..4], [|2;3;4;5|])
        Assert.AreEqual(arr.[1..5], [|2;3;4;5;6|])

        Assert.AreEqual(arr.[0..1], [|1;2|])
        Assert.AreEqual(arr.[1..1], [|2|])
        Assert.AreEqual(arr.[2..1], ([||]: int array))
        Assert.AreEqual(arr.[3..1], ([||]: int array))
        Assert.AreEqual(arr.[4..1], ([||]: int array))


    [<Fact>]
    member _.SlicingEmptyArray() = 

        let empty : obj array = Array.empty
        Assert.AreEqual(empty.[*], ([||]: obj array))
        Assert.AreEqual(empty.[5..3], ([||]: obj array))
        Assert.AreEqual(empty.[0..], ([||]: obj array))
        Assert.AreEqual(empty.[0..0], ([||]: obj array))
        Assert.AreEqual(empty.[0..1], ([||]: obj array))
        Assert.AreEqual(empty.[3..5], ([||]: obj array))


    [<Fact>]
    member _.SlicingOutOfBounds() = 
        let arr = [|1;2;3;4;5;6|]
       
        Assert.AreEqual(arr.[..6], [|1;2;3;4;5;6|])
        Assert.AreEqual(arr.[6..], ([||]: int array))

        Assert.AreEqual(arr.[0..(-1)], ([||]: int array))
        Assert.AreEqual(arr.[1..(-1)], ([||]: int array))
        Assert.AreEqual(arr.[1..0], ([||]: int array))
        Assert.AreEqual(arr.[0..6], [|1;2;3;4;5;6|])
        Assert.AreEqual(arr.[1..6], [|2;3;4;5;6|])

        Assert.AreEqual(arr.[-1..1], [|1;2|])
        Assert.AreEqual(arr.[-3..(-4)], ([||]: int array))
        Assert.AreEqual(arr.[-4..(-3)], ([||]: int array))


    [<Fact>]
    member _.RandomShuffle() =
        let arr = [| 1..20 |]

        let shuffled1 = arr |> Array.randomShuffle
        let shuffled2 = arr |> Array.randomShuffle

        Assert.AreNotEqual(shuffled1, arr)
        Assert.AreNotEqual(shuffled1, shuffled2)

    [<Fact>]
    member _.RandomShuffleWrongArg() =
        let nullArr = null
        CheckThrowsArgumentNullException (fun () -> Array.randomShuffle nullArr |> ignore)

    [<Fact>]
    member _.RandomShuffleWith() =
        let arr = [| 1..20 |]

        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let shuffle1 = arr |> Array.randomShuffleWith rand1
        let shuffle2 = arr |> Array.randomShuffleWith rand2
        let shuffle3 = arr |> Array.randomShuffleWith rand3

        Assert.AreEqual(shuffle1, shuffle2)
        Assert.AreNotEqual(arr, shuffle1)
        Assert.AreNotEqual(shuffle1, shuffle3)

    [<Fact>]
    member _.RandomShuffleWithWrongArg() =
        let nullArr = null
        let arr = [| 1..20 |]
        let nullRand = null
        let rand = Random(123)

        CheckThrowsArgumentNullException (fun () -> Array.randomShuffleWith rand nullArr |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.randomShuffleWith nullRand arr |> ignore)

    [<Fact>]
    member _.RandomShuffleBy() =
        let arr = [| 1..20 |]

        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let shuffle1 = arr |> Array.randomShuffleBy rand1.NextDouble
        let shuffle2 = arr |> Array.randomShuffleBy rand2.NextDouble
        let shuffle3 = arr |> Array.randomShuffleBy rand3.NextDouble

        Assert.AreEqual(shuffle1, shuffle2)
        Assert.AreNotEqual(arr, shuffle1)
        Assert.AreNotEqual(shuffle1, shuffle3)

    [<Fact>]
    member _.RandomShuffleByWrongArg() =
        let nullArr = null
        let arr = [| 1..20 |]
        let wrongRandomizers = [
            fun () -> nan
            fun () -> 1.0
            fun () -> infinity
        ]
        let randomizer = Random(123).NextDouble

        CheckThrowsArgumentNullException (fun () -> Array.randomShuffleBy randomizer nullArr |> ignore)
        wrongRandomizers |> List.iter (fun wrongRandomizer ->
            CheckThrowsArgumentOutOfRangeException (fun () -> Array.randomShuffleBy wrongRandomizer arr |> ignore))

    [<Fact>]
    member _.RandomShuffleInPlace() =
        let arr = [| 1..20 |]
        let shuffled1 = [| 1..20 |]
        let shuffled2 = [| 1..20 |]

        shuffled1 |> Array.randomShuffleInPlace
        shuffled2 |> Array.randomShuffleInPlace

        Assert.AreNotEqual(shuffled1, arr)
        Assert.AreNotEqual(shuffled1, shuffled2)

    [<Fact>]
    member _.RandomShuffleInPlaceWrongArg() =
        let nullArr = null

        CheckThrowsArgumentNullException (fun () -> Array.randomShuffleInPlace nullArr |> ignore)

    [<Fact>]
    member _.RandomShuffleInPlaceWith() =
        let arr = [| 1..20 |]

        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let shuffle1 = [| 1..20 |]
        let shuffle2 = [| 1..20 |]
        let shuffle3 = [| 1..20 |]

        shuffle1 |> Array.randomShuffleInPlaceWith rand1
        shuffle2 |> Array.randomShuffleInPlaceWith rand2
        shuffle3 |> Array.randomShuffleInPlaceWith rand3

        Assert.AreEqual(shuffle1, shuffle2)
        Assert.AreNotEqual(arr, shuffle1)
        Assert.AreNotEqual(shuffle1, shuffle3)

    [<Fact>]
    member _.RandomShuffleInPlaceWithWrongArg() =
        let nullArr = null
        let arr = [| 1..20 |]
        let rand = Random(123)
        let nullRand = null

        CheckThrowsArgumentNullException (fun () -> Array.randomShuffleInPlaceWith rand nullArr |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.randomShuffleInPlaceWith nullRand arr |> ignore)

    [<Fact>]
    member _.RandomShuffleInPlaceBy() =
        let arr = [| 1..20 |]

        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let shuffle1 = [| 1..20 |]
        let shuffle2 = [| 1..20 |]
        let shuffle3 = [| 1..20 |]

        shuffle1 |> Array.randomShuffleInPlaceBy rand1.NextDouble
        shuffle2 |> Array.randomShuffleInPlaceBy rand2.NextDouble
        shuffle3 |> Array.randomShuffleInPlaceBy rand3.NextDouble

        Assert.AreEqual(shuffle1, shuffle2)
        Assert.AreNotEqual(arr, shuffle1)
        Assert.AreNotEqual(shuffle1, shuffle3)

    [<Fact>]
    member _.RandomShuffleInPlaceByWrongArg() =
        let nullArr = null
        let arr = [| 1..20 |]
        let wrongRandomizers = [
            fun () -> nan
            fun () -> 1.0
            fun () -> infinity
        ]
        let randomizer = Random(123).NextDouble

        CheckThrowsArgumentNullException (fun () -> Array.randomShuffleInPlaceBy randomizer nullArr |> ignore)
        wrongRandomizers |> List.iter (fun wrongRandomizer ->
            CheckThrowsArgumentOutOfRangeException (fun () -> Array.randomShuffleInPlaceBy wrongRandomizer arr |> ignore))

    [<Fact>]
    member _.RandomChoice() =
        let arr = [| 1..5000 |]

        // try choice five times, if all are same, it must be broken
        let results = [|
            Array.randomChoice arr
            Array.randomChoice arr
            Array.randomChoice arr
            Array.randomChoice arr
            Array.randomChoice arr
        |]
        let allSame = results |> Array.forall (fun x -> x = results.[0])
        Assert.False(allSame)

    [<Fact>]
    member _.RandomChoiceWrongArg() =
        let nullArr = null
        let emptyArr = [||]

        CheckThrowsArgumentNullException (fun () -> Array.randomChoice nullArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomChoice emptyArr |> ignore)

    [<Fact>]
    member _.RandomChoiceWith() =
        let arr = [| 1..5000 |]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choice1 = arr |> Array.randomChoiceWith rand1
        let choice2 = arr |> Array.randomChoiceWith rand2
        let choice3 = arr |> Array.randomChoiceWith rand3

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)

    [<Fact>]
    member _.RandomChoiceWithWrongArg() =
        let nullArr = null
        let emptyArr = [||]
        let arr = [| 1..20 |]
        let nullRand = null
        let rand = Random(123)

        CheckThrowsArgumentNullException (fun () -> Array.randomChoiceWith rand nullArr |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.randomChoiceWith nullRand arr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomChoiceWith rand emptyArr |> ignore)

    [<Fact>]
    member _.RandomChoiceBy() =
        let arr = [| 1..5000 |]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choice1 = arr |> Array.randomChoiceBy rand1.NextDouble
        let choice2 = arr |> Array.randomChoiceBy rand2.NextDouble
        let choice3 = arr |> Array.randomChoiceBy rand3.NextDouble

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)

    [<Fact>]
    member _.RandomChoiceByWrongArg() =
        let nullArr = null
        let emptyArr = [||]
        let arr = [| 1..20 |]
        let wrongRandomizers = [
            fun () -> nan
            fun () -> 1.0
            fun () -> infinity
        ]
        let randomizer = Random(123).NextDouble

        CheckThrowsArgumentNullException (fun () -> Array.randomChoiceBy randomizer nullArr |> ignore)
        wrongRandomizers |> List.iter (fun wrongRandomizer ->
            CheckThrowsArgumentOutOfRangeException (fun () -> Array.randomChoiceBy wrongRandomizer arr |> ignore))
        CheckThrowsArgumentException (fun () -> Array.randomChoiceBy randomizer emptyArr |> ignore)

    [<Fact>]
    member _.RandomChoices() =
        let arr = [| 1..50 |]

        let choicesLength = 20
        let choice1 = arr |> Array.randomChoices choicesLength
        let choice2 = arr |> Array.randomChoices choicesLength

        Assert.AreNotEqual(choice1, choice2)
        Assert.AreEqual(choicesLength, choice1.Length)
        Assert.AreEqual(choicesLength, choice2.Length)

        let arr = [| 1; 2 |]
        let choices = arr |> Array.randomChoices choicesLength
        Assert.AreEqual(choicesLength, choices.Length)
        Assert.AreEqual(arr, choices |> Array.distinct |> Array.sort)

    [<Fact>]
    member _.RandomChoicesWrongArg() =
        let nullArr = null
        let emptyArr = [||]
        let arr = [| 1..50 |]
        let choicesLength = 20
        let negativeChoicesLength = -1

        CheckThrowsArgumentNullException (fun () -> Array.randomChoices choicesLength nullArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomChoices choicesLength emptyArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomChoices negativeChoicesLength arr |> ignore)

    [<Fact>]
    member _.RandomChoicesWith() =
        let arr = [| 1..50 |]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choicesLength = 20
        let choice1 = arr |> Array.randomChoicesWith rand1 choicesLength
        let choice2 = arr |> Array.randomChoicesWith rand2 choicesLength
        let choice3 = arr |> Array.randomChoicesWith rand3 choicesLength

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)

    [<Fact>]
    member _.RandomChoicesWithWrongArg() =
        let nullArr = null
        let emptyArr = [||]
        let arr = [| 1..50 |]
        let nullRand = null
        let rand = Random(123)
        let choicesLength = 20
        let negativeChoicesLength = -1

        CheckThrowsArgumentNullException (fun () -> Array.randomChoicesWith rand choicesLength nullArr |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.randomChoicesWith nullRand choicesLength arr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomChoicesWith rand choicesLength emptyArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomChoicesWith rand negativeChoicesLength arr |> ignore)

    [<Fact>]
    member _.RandomChoicesBy() =
        let arr = [| 1..50 |]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choicesLength = 20
        let choice1 = arr |> Array.randomChoicesBy rand1.NextDouble choicesLength
        let choice2 = arr |> Array.randomChoicesBy rand2.NextDouble choicesLength
        let choice3 = arr |> Array.randomChoicesBy rand3.NextDouble choicesLength

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)

    [<Fact>]
    member _.RandomChoicesByWrongArg() =
        let nullArr = null
        let emptyArr = [||]
        let arr = [| 1..50 |]
        let wrongRandomizers = [
            fun () -> nan
            fun () -> 1.0
            fun () -> infinity
        ]
        let randomizer = Random(123).NextDouble
        let choicesLength = 20
        let negativeChoicesLength = -1

        CheckThrowsArgumentNullException (fun () -> Array.randomChoicesBy randomizer choicesLength nullArr |> ignore)
        wrongRandomizers |> List.iter (fun wrongRandomizer ->
            CheckThrowsArgumentOutOfRangeException (fun () -> Array.randomChoicesBy wrongRandomizer choicesLength arr |> ignore))
        CheckThrowsArgumentException (fun () -> Array.randomChoicesBy randomizer choicesLength emptyArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomChoicesBy randomizer negativeChoicesLength arr |> ignore)

    [<Fact>]
    member _.RandomSample() =
        let arr = [| 1..50 |]

        let choicesLength = 20
        let choice1 = arr |> Array.randomSample choicesLength
        let choice2 = arr |> Array.randomSample choicesLength

        Assert.AreNotEqual(choice1, choice2)
        Assert.AreEqual(choicesLength, choice1.Length)
        Assert.AreEqual(choicesLength, choice2.Length)
        Assert.AreEqual(choice1, choice1 |> Array.distinct)
        Assert.AreEqual(choice2, choice2 |> Array.distinct)

    [<Fact>]
    member _.RandomSampleWrongArg() =
        let nullArr = null
        let emptyArr = [||]
        let arr = [| 1..50 |]
        let tooBigSampleLength = 100
        let negativeSampleLength = -1
        let sampleLength = 20

        CheckThrowsArgumentNullException (fun () -> Array.randomSample sampleLength nullArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomSample sampleLength emptyArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomSample negativeSampleLength arr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomSample tooBigSampleLength arr |> ignore)

    [<Fact>]
    member _.RandomSampleWith() =
        let arr = [| 1..50 |]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choicesLength = 20
        let choice1 = arr |> Array.randomSampleWith rand1 choicesLength
        let choice2 = arr |> Array.randomSampleWith rand2 choicesLength
        let choice3 = arr |> Array.randomSampleWith rand3 choicesLength

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)
        Assert.AreEqual(choicesLength, choice1.Length)
        Assert.AreEqual(choicesLength, choice3.Length)
        Assert.AreEqual(choice1, choice1 |> Array.distinct)
        Assert.AreEqual(choice3, choice3 |> Array.distinct)

    [<Fact>]
    member _.RandomSampleWithWrongArg() =
        let nullArr = null
        let emptyArr = [||]
        let arr = [| 1..50 |]
        let nullRand = null
        let rand = Random(123)
        let tooBigSampleLength = 100
        let negativeSampleLength = -1
        let sampleLength = 20

        CheckThrowsArgumentNullException (fun () -> Array.randomSampleWith rand sampleLength nullArr |> ignore)
        CheckThrowsArgumentNullException (fun () -> Array.randomSampleWith nullRand sampleLength arr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomSampleWith rand sampleLength emptyArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomSampleWith rand negativeSampleLength arr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomSampleWith rand tooBigSampleLength arr |> ignore)

    [<Fact>]
    member _.RandomSampleBy() =
        let arr = [| 1..50 |]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choicesLength = 20
        let choice1 = arr |> Array.randomSampleBy rand1.NextDouble choicesLength
        let choice2 = arr |> Array.randomSampleBy rand2.NextDouble choicesLength
        let choice3 = arr |> Array.randomSampleBy rand3.NextDouble choicesLength

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)
        Assert.AreEqual(choicesLength, choice1.Length)
        Assert.AreEqual(choicesLength, choice3.Length)
        Assert.AreEqual(choice1, choice1 |> Array.distinct)
        Assert.AreEqual(choice3, choice3 |> Array.distinct)

    [<Fact>]
    member _.RandomSampleByWrongArg() =
        let nullArr = null
        let emptyArr = [||]
        let arr = [| 1..50 |]
        let wrongRandomizers = [
            fun () -> nan
            fun () -> 1.0
            fun () -> infinity
        ]
        let randomizer = Random(123).NextDouble
        let tooBigSampleLength = 100
        let negativeSampleLength = -1
        let sampleLength = 20

        CheckThrowsArgumentNullException (fun () -> Array.randomSampleBy randomizer sampleLength nullArr |> ignore)
        wrongRandomizers |> List.iter (fun wrongRandomizer ->
            CheckThrowsArgumentOutOfRangeException (fun () -> Array.randomSampleBy wrongRandomizer sampleLength arr |> ignore))
        CheckThrowsArgumentException (fun () -> Array.randomSampleBy randomizer sampleLength emptyArr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomSampleBy randomizer negativeSampleLength arr |> ignore)
        CheckThrowsArgumentException (fun () -> Array.randomSampleBy randomizer tooBigSampleLength arr |> ignore)