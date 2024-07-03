// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Collections.List module

namespace FSharp.Core.UnitTests.Collections

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit

(*
[Test Strategy]
Make sure each method works on:
* Integer List (value type)
* String List (reference type)
* Empty List (0 elements)
*)

type ListModule() =
    [<Fact>]
    member _.Empty() =
        let emptyList = List.empty
        let resultEpt = List.length emptyList
        Assert.AreEqual(0, resultEpt)   
        
        let c : int list   = List.empty<int>
        let d : string list = List.empty<string>
        
        ()

    [<Fact>]
    member _.AllPairs() =
        // integer List
        let resultInt =  List.allPairs [1..3] [2..2..6]
        Assert.AreEqual([(1,2);(1,4);(1,6)
                         (2,2);(2,4);(2,6)
                         (3,2);(3,4);(3,6)], resultInt)

        // string List
        let resultStr = List.allPairs [2;3;4;5] ["b";"c";"d";"e"]
        Assert.AreEqual([(2,"b");(2,"c");(2,"d");(2,"e")
                         (3,"b");(3,"c");(3,"d");(3,"e")
                         (4,"b");(4,"c");(4,"d");(4,"e")
                         (5,"b");(5,"c");(5,"d");(5,"e")] , resultStr)

        // empty List
        let resultEpt = List.allPairs [] []
        let empTuple:(obj*obj) list = []
        Assert.AreEqual(empTuple, resultEpt)

        ()

    [<Fact>]
    member _.Append() =
        // integer List
        let intList = List.append [ 1; 2 ] [ 3; 4 ]
        Assert.AreEqual([ 1; 2; 3; 4 ],intList)
        
        // string List
        let strList = List.append [ "a"; "b" ] [ "c"; "d" ]
        Assert.AreEqual([ "a"; "b" ;"c"; "d" ],strList)

        // empty List
        let emptyList = List.append [] []
        Assert.AreEqual([],emptyList)
        ()

    [<Fact>]
    member _.Average() =     
        // empty float32 List
        let emptyFloatList = List.empty<System.Single> 
        CheckThrowsArgumentException(fun () -> List.average emptyFloatList |> ignore)
        
        // empty double List
        let emptyDoubleList = List.empty<System.Double> 
        CheckThrowsArgumentException(fun () -> List.average emptyDoubleList |> ignore)
        
        // empty decimal List
        let emptyDecimalList = List.empty<System.Decimal> 
        CheckThrowsArgumentException (fun () -> List.average emptyDecimalList |>ignore )

        // float32 List
        let floatList: float32 list = [ 1.2f;3.5f;6.7f ]
        let averageOfFloat = List.average floatList        
        Assert.AreEqual(3.8000000000000003f, averageOfFloat)
        
        // double List
        let doubleList: List<System.Double> = [ 1.0;8.0 ]
        let averageOfDouble = List.average doubleList        
        Assert.AreEqual(4.5, averageOfDouble)
        
        // decimal List
        let decimalList: decimal list = [ 0M;19M;19.03M ]
        let averageOfDecimal = List.average decimalList        
        Assert.AreEqual(12.676666666666666666666666667M, averageOfDecimal)
        

        ()

    [<Fact>]
    member _.AverageBy() =
        // empty double List
        let emptyDouList = List.empty<System.Double>
        CheckThrowsArgumentException (fun () -> List.averageBy (fun x -> x + 6.7) emptyDouList |> ignore)

        // empty float32 List
        let emptyFloat32List: float32 list = []
        CheckThrowsArgumentException (fun () -> List.averageBy (fun x -> x + 9.8f) emptyFloat32List |> ignore)

        // empty decimal List
        let emptyDecimalList = List.empty<System.Decimal>
        CheckThrowsArgumentException (fun () -> List.averageBy (fun x -> x + 9.8M) emptyDecimalList |> ignore)

        // float32 List
        let floatList: float32 list = [ 1.5f; 2.5f; 3.5f; 4.5f ] // using values that behave nicely with IEEE floats
        let averageOfFloat = List.averageBy (fun x -> x + 1.0f) floatList
        Assert.AreEqual(4.0f, averageOfFloat)

        // double List
        let doubleList: System.Double list = [ 1.0; 8.0 ] // using values that behave nicely with IEEE doubles
        let averageOfDouble = List.averageBy (fun x -> x + 1.0) doubleList
        Assert.AreEqual(5.5, averageOfDouble)

        // decimal List
        let decimalList: decimal list = [ 0M;19M;19.03M ]
        let averageOfDecimal = List.averageBy (fun x -> x + 9.8M) decimalList
        Assert.AreEqual(22.476666666666666666666666667M, averageOfDecimal)

        ()

    [<Fact>]
    member _.ChunkBySize() =

        // int list
        Assert.True([ [1..4]; [5..8] ] = List.chunkBySize 4 [1..8])
        Assert.True([ [1..4]; [5..8]; [9..10] ] = List.chunkBySize 4 [1..10])
        Assert.True([ [1]; [2]; [3]; [4] ] = List.chunkBySize 1 [1..4])

        // string list
        Assert.True([ ["a"; "b"]; ["c";"d"]; ["e"] ] = List.chunkBySize 2 ["a";"b";"c";"d";"e"])

        // empty list
        Assert.True([] = List.chunkBySize 3 [])

        // invalidArg
        CheckThrowsArgumentException (fun () -> List.chunkBySize 0 [1..10] |> ignore)
        CheckThrowsArgumentException (fun () -> List.chunkBySize -1 [1..10] |> ignore)

        ()

    [<Fact>]
    member _.SplitInto() =

        // int list
        Assert.True([ [1..4]; [5..7]; [8..10] ] = List.splitInto 3 [1..10])
        Assert.True([ [1..4]; [5..8]; [9..11] ] = List.splitInto 3 [1..11])
        Assert.True([ [1..4]; [5..8]; [9..12] ] = List.splitInto 3 [1..12])

        Assert.True([ [1..2]; [3]; [4]; [5] ] = List.splitInto 4 [1..5])
        Assert.True([ [1]; [2]; [3]; [4] ] = List.splitInto 20 [1..4])

        // string list
        Assert.True([ ["a"; "b"]; ["c";"d"]; ["e"] ] = List.splitInto 3 ["a";"b";"c";"d";"e"])

        // empty list
        Assert.True([] = List.splitInto 3 [])

        // invalidArg
        CheckThrowsArgumentException (fun () -> List.splitInto 0 [1..10] |> ignore)
        CheckThrowsArgumentException (fun () -> List.splitInto -1 [1..10] |> ignore)

        ()

    [<Fact>]
    member _.distinct() = 
        // distinct should work on empty list
        Assert.AreEqual([], List.distinct [])

        // distinct should filter out simple duplicates
        Assert.AreEqual([1], List.distinct [1])
        Assert.AreEqual([1], List.distinct [1; 1])
        Assert.AreEqual([1; 2; 3], List.distinct [1; 2; 3; 1])
        Assert.AreEqual([[1;2]; [1;3]], List.distinct [[1;2]; [1;3]; [1;2]; [1;3]])        
        Assert.AreEqual([[1;1]; [1;2]; [1;3]; [1;4]], List.distinct [[1;1]; [1;2]; [1;3]; [1;4]])
        Assert.AreEqual([[1;1]; [1;4]], List.distinct [[1;1]; [1;1]; [1;1]; [1;4]])

        Assert.AreEqual([null], List.distinct [null])
        let list = new System.Collections.Generic.List<int>()
        Assert.True([null, list] = List.distinct [null, list])

    [<Fact>]
    member _.distinctBy() =
        // distinctBy should work on empty list
        Assert.AreEqual([], List.distinctBy (fun _ -> failwith "should not be executed") [])
        
        // distinctBy should filter out simple duplicates
        Assert.AreEqual([1], List.distinctBy id [1])
        Assert.AreEqual([1], List.distinctBy id [1; 1])
        Assert.AreEqual([1; 2; 3], List.distinctBy id [1; 2; 3; 1])

        // distinctBy should use the given projection to filter out simple duplicates
        Assert.AreEqual([1], List.distinctBy (fun x -> x / x) [1; 2])
        Assert.AreEqual([1; 2], List.distinctBy (fun x -> if x < 3 then x else 1) [1; 2; 3; 4])
        Assert.AreEqual([[1;2]; [1;3]], List.distinctBy (fun x -> List.sum x) [[1;2]; [1;3]; [2;1]])

        Assert.AreEqual([null], List.distinctBy id [null])
        let list = new System.Collections.Generic.List<int>()
        Assert.True([null, list] = List.distinctBy id [null, list])

    [<Fact>]
    member _.Take() =
        Assert.AreEqual(([] : int list) ,List.take 0 ([] : int list))
        Assert.AreEqual(([] : string list),List.take 0 ["str1";"str2";"str3";"str4"])
        Assert.AreEqual([1;2;4],List.take 3 [1;2;4;5;7])
        Assert.AreEqual(["str1";"str2"],List.take 2 ["str1";"str2";"str3";"str4"])

        CheckThrowsInvalidOperationExn (fun () -> List.take 1 [] |> ignore)
        CheckThrowsArgumentException (fun () -> List.take -1 [0;1] |> ignore)
        CheckThrowsInvalidOperationExn (fun () -> List.take 5 ["str1";"str2";"str3";"str4"] |> ignore)
              
    [<Fact>]
    member _.Choose() = 
        // int List
        let intSrc:int list = [ 1..100 ]    
        let funcInt x = if (x%5=0) then Some (x*x) else None       
        let intChosen = List.choose funcInt intSrc        
        Assert.AreEqual(25, intChosen.[0])
        Assert.AreEqual(100, intChosen.[1])
        Assert.AreEqual(225, intChosen.[2])
        
        // string List
        let stringSrc: string list = [ "List"; "this"; "is" ;"str"; "list" ]
        let funcString x = match x with
                           | "list" -> Some x
                           | "List" -> Some x
                           | _ -> None
        let strChosen = List.choose funcString stringSrc           
        Assert.AreEqual("list", strChosen.[0].ToLower())
        Assert.AreEqual("list", strChosen.[1].ToLower())
        
        // always None 
        let emptySrc :int list = [ ]
        let emptyChosen = List.choose (fun i -> Option<int>.None) intSrc        
        Assert.AreEqual(emptySrc, emptyChosen)

        // empty List
        let emptySrc :int list = [ ]
        let emptyChosen = List.choose funcInt emptySrc        
        Assert.AreEqual(emptySrc, emptyChosen)

        () 

    [<Fact>]
    member _.compareWith() =
        // compareWith should work on empty lists
        Assert.AreEqual(0,List.compareWith (fun _ -> failwith "should not be executed")  [] [])
        Assert.AreEqual(-1,List.compareWith (fun _ -> failwith "should not be executed") [] [1])
        Assert.AreEqual(1,List.compareWith (fun _ -> failwith "should not be executed")  ["1"] [])
    
        // compareWith should work on longer lists
        Assert.AreEqual(-1,List.compareWith compare ["1";"2"] ["1";"3"])
        Assert.AreEqual(1,List.compareWith compare [1;2;43] [1;2;1])
        Assert.AreEqual(1,List.compareWith compare [1;2;3;4] [1;2;3])
        Assert.AreEqual(0,List.compareWith compare [1;2;3;4] [1;2;3;4])
        Assert.AreEqual(-1,List.compareWith compare [1;2;3] [1;2;3;4])
        Assert.AreEqual(1,List.compareWith compare [1;2;3] [1;2;2;4])
        Assert.AreEqual(-1,List.compareWith compare [1;2;2] [1;2;3;4])

        // compareWith should use the comparer
        Assert.AreEqual(0,List.compareWith (fun x y -> 0) ["1";"2"] ["1";"3"])
        Assert.AreEqual(1,List.compareWith (fun x y -> 1) ["1";"2"] ["1";"3"])
        Assert.AreEqual(-1,List.compareWith (fun x y -> -1) ["1";"2"] ["1";"3"])
        
    [<Fact>]
    member _.takeWhile() =
        Assert.AreEqual(([] : int list),List.takeWhile (fun x -> failwith "should not be used") ([] : int list))
        Assert.AreEqual([1;2;4;5],List.takeWhile (fun x -> x < 6) [1;2;4;5;6;7])
        Assert.AreEqual(["a"; "ab"; "abc"],List.takeWhile (fun (x:string) -> x.Length < 4) ["a"; "ab"; "abc"; "abcd"; "abcde"])        
        Assert.AreEqual(["a"; "ab"; "abc"; "abcd"; "abcde"],List.takeWhile (fun _ -> true) ["a"; "ab"; "abc"; "abcd"; "abcde"])
        Assert.AreEqual(([] : string list),List.takeWhile (fun _ -> false) ["a"; "ab"; "abc"; "abcd"; "abcde"])
        Assert.AreEqual(([] : string list),List.takeWhile (fun _ -> false) ["a"])
        Assert.AreEqual(["a"],List.takeWhile (fun _ -> true) ["a"])
        Assert.AreEqual(["a"],List.takeWhile (fun x -> x <> "ab") ["a"; "ab"; "abc"; "abcd"; "abcde"])

    [<Fact>]
    member _.Concat() =
        // integer List
        let seqInt = 
            seq { for i in 1..10 do                
                    yield [i;i*10]}
        let conIntArr = List.concat seqInt        
        Assert.AreEqual(20, List.length conIntArr)
        
        // string List
        let strSeq = 
            seq { for a in 'a'..'c' do
                    for b in 'a'..'c' do
                        yield [a.ToString();b.ToString() ]}
     
        let conStrArr = List.concat strSeq        
        Assert.AreEqual(18, List.length conStrArr)
        
        // Empty List
        let emptyLists = [ [ ]; [ 0 ]; [ 1 ]; [ ]; [ ] ]
        let result2 = List.concat emptyLists
        Assert.AreEqual(2, result2.Length)   
        Assert.AreEqual(0, result2.[0])
        Assert.AreEqual(1, result2.[1])
        () 

    [<Fact>]
    member _.splitAt() =        
        Assert.AreEqual((([] : int list),([] : int list)), List.splitAt 0 ([] : int list))

        Assert.AreEqual([1..4], List.splitAt 4 [1..10] |> fst)       
        Assert.AreEqual([5..10], List.splitAt 4 [1..10] |> snd)      

        Assert.AreEqual(([]: int list), List.splitAt 0 [1..2] |> fst)
        Assert.AreEqual([1..2], List.splitAt 0 [1..2] |> snd)

        Assert.AreEqual([1], List.splitAt 1 [1..2] |> fst)
        Assert.AreEqual([2], List.splitAt 1 [1..2] |> snd)

        Assert.AreEqual([1..2], List.splitAt 2 [1..2] |> fst)
        Assert.AreEqual(([] : int list), List.splitAt 2 [1..2] |> snd)

        Assert.AreEqual(["a"], List.splitAt 1 ["a";"b";"c"] |> fst)
        Assert.AreEqual(["b";"c"], List.splitAt 1 ["a";"b";"c"] |> snd)

        // split should fail if index exceeds bounds
        CheckThrowsInvalidOperationExn (fun () -> List.splitAt 1 [] |> ignore)
        CheckThrowsArgumentException (fun () -> List.splitAt -1 [0;1] |> ignore)
        CheckThrowsInvalidOperationExn (fun () -> List.splitAt 5 ["str1";"str2";"str3";"str4"] |> ignore)
        ()

    [<Fact>]
    member _.countBy() =
        // countBy should work on empty list
        Assert.AreEqual(0,List.countBy (fun _ -> failwith "should not be executed") [] |> List.length)

        // countBy should count by the given key function
        Assert.AreEqual([5,1; 2,2; 3,2],List.countBy id [5;2;2;3;3])
        Assert.AreEqual([3,3; 2,2; 1,3],List.countBy (fun x -> if x < 3 then x else 3) [5;2;1;2;3;3;1;1])

    [<Fact>]
    member _.Except() =
        // integer list
        let intList1 = [ yield! {1..100}
                         yield! {1..100} ]
        let intList2 = [1..10]
        let expectedIntList = [11..100]

        Assert.AreEqual(expectedIntList, List.except intList2 intList1)

        // string list
        let strList1 = ["a"; "b"; "c"; "d"; "a"]
        let strList2 = ["b"; "c"]
        let expectedStrList = ["a"; "d"]

        Assert.AreEqual(expectedStrList, List.except strList2 strList1)

        // empty list
        let emptyIntList : int list = []
        Assert.AreEqual([1..100], List.except emptyIntList intList1)
        Assert.AreEqual(emptyIntList, List.except intList1 emptyIntList)
        Assert.AreEqual(emptyIntList, List.except emptyIntList emptyIntList)
        Assert.AreEqual(emptyIntList, List.except intList1 intList1)

        // null seq
        let nullSeq : int [] = null
        CheckThrowsArgumentNullException(fun () -> List.except nullSeq emptyIntList |> ignore)
        ()

    [<Fact>]
    member _.Exists() =
        // integer List
        let intArr = [ 2;4;6;8 ]
        let funcInt x = if (x%2 = 0) then true else false
        let resultInt = List.exists funcInt intArr        
        Assert.True(resultInt)
        
        // string List
        let strArr = ["."; ".."; "..."; "...."] 
        let funcStr (x:string) = if (x.Length >15) then true else false
        let resultStr = List.exists funcStr strArr        
        Assert.False(resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = List.exists funcInt emptyArr        
        Assert.False(resultEpt)
               
        ()
    [<Fact>]
    member _.Exists2() =
        // integer List
        let intFir = [ 2;4;6;8 ]
        let intSec = [ 1;2;3;4 ]
        let funcInt x y = if (x%y = 0) then true else false
        let resultInt = List.exists2 funcInt intFir intSec        
        Assert.True(resultInt)
        
        // string List
        let strFir = ["Lists"; "are";  "commonly" ]
        let strSec = ["good"; "good";  "good"  ]
        let funcStr (x:string) (y:string) = if (x = y) then true else false
        let resultStr = List.exists2 funcStr strFir strSec        
        Assert.False(resultStr)
        
        // empty List
        let eptFir:int list = [ ]
        let eptSec:int list = [ ]
        let resultEpt = List.exists2 funcInt eptFir eptSec        
        Assert.False(resultEpt)
        
        ()

    [<Fact>]
    member _.Filter() =
        // integer List
        let intArr = [ 1..20 ]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = List.filter funcInt intArr        
        Assert.AreEqual([5;10;15;20], resultInt)
        
        // string List
        let strArr = ["."; ".."; "..."; "...."] 
        let funcStr (x:string) = if (x.Length >2) then true else false
        let resultStr = List.filter funcStr strArr        
        Assert.AreEqual(["..."; "...."], resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = List.filter funcInt emptyArr        
        Assert.AreEqual(emptyArr, resultEpt)
            
        ()   

    [<Fact>]
    member _.Where() =
        // integer List
        let intArr = [ 1..20 ]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = List.where funcInt intArr        
        Assert.AreEqual([5;10;15;20], resultInt)
        
        // string List
        let strArr = ["."; ".."; "..."; "...."] 
        let funcStr (x:string) = if (x.Length >2) then true else false
        let resultStr = List.where funcStr strArr        
        Assert.AreEqual(["..."; "...."], resultStr)
        
        // empty List
        let emptyList:int list = [ ]
        let resultEpt = List.where funcInt emptyList
        Assert.AreEqual(emptyList, resultEpt)
            
        ()   

    [<Fact>]
    member _.``where should work like filter``() =
        Assert.AreEqual(([] : int list), List.where (fun x -> x % 2 = 0) [])
        Assert.AreEqual([0;2;4;6;8], List.where (fun x -> x % 2 = 0) [0..9])
        Assert.AreEqual(["a";"b";"c"], List.where (fun _ -> true) ["a";"b";"c"])

        ()

    [<Fact>]
    member _.Find() =
        // integer List
        let intArr = [ 1..20 ]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = List.find funcInt intArr        
        Assert.AreEqual(5, resultInt)
        
        // string List
        let strArr = ["."; ".."; "..."; "...."] 
        let funcStr (x:string) = if (x.Length >2) then true else false
        let resultStr = List.find funcStr strArr        
        Assert.AreEqual("...", resultStr)
        
        // empty List
        let emptyArr:int list = [ ]   
        CheckThrowsKeyNotFoundException (fun () -> List.find (fun _ -> true) emptyArr |> ignore)
                
        // not found
        CheckThrowsKeyNotFoundException (fun () -> List.find (fun _ -> false) intArr |> ignore)

        () 

    [<Fact>]
    member _.replicate() = 
        // replicate should create multiple copies of the given value
        Assert.AreEqual(0,List.replicate 0 null |> List.length)
        Assert.AreEqual(0,List.replicate 0 1 |> List.length)
        Assert.AreEqual([ (null : obj) ],(List.replicate 1 null : obj list))
        Assert.AreEqual(["1";"1"],List.replicate 2 "1")

        CheckThrowsArgumentException (fun () ->  List.replicate -1 null |> ignore)

    [<Fact>]
    member _.FindBack() =
        // integer List
        let funcInt x = if (x%5 = 0) then true else false
        Assert.AreEqual(20, List.findBack funcInt [ 1..20 ])
        Assert.AreEqual(15, List.findBack funcInt [ 1..19 ])
        Assert.AreEqual(5, List.findBack funcInt [ 5..9 ])

        // string List
        let strArr = ["."; ".."; "..."; "...."]
        let funcStr (x:string) = x.Length > 2
        let resultStr = List.findBack funcStr strArr
        Assert.AreEqual("....", resultStr)

        // empty List
        CheckThrowsKeyNotFoundException (fun () -> List.findBack (fun _ -> true) [] |> ignore)

        // not found
        CheckThrowsKeyNotFoundException (fun () -> List.findBack (fun _ -> false) [ 1..20 ] |> ignore)

        ()


    [<Fact>]
    member _.FindIndex() =
        // integer List
        let intArr = [ 1..20 ]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = List.findIndex funcInt intArr        
        Assert.AreEqual(4, resultInt)
        
        // string List
        let strArr = ["."; ".."; "..."; "...."] 
        let funcStr (x:string) = if (x.Length > 2) then true else false
        let resultStr = List.findIndex funcStr strArr        
        Assert.AreEqual(2, resultStr)
        
        // empty List
        let emptyArr:int list = [ ]  
        CheckThrowsKeyNotFoundException (fun () -> List.findIndex (fun _ -> true) emptyArr |> ignore)
                
        // not found
        CheckThrowsKeyNotFoundException (fun () -> List.findIndex (fun _ -> false) intArr |> ignore)

        () 
        
    [<Fact>]
    member _.FindIndexBack() =
        // integer List
        let funcInt x = if (x%5 = 0) then true else false
        Assert.AreEqual(19, List.findIndexBack funcInt [ 1..20 ])
        Assert.AreEqual(14, List.findIndexBack funcInt [ 1..19 ])
        Assert.AreEqual(0, List.findIndexBack funcInt [ 5..9 ])

        // string List
        let strArr = ["."; ".."; "..."; "...."]
        let funcStr (x:string) = x.Length > 2
        let resultStr = List.findIndexBack funcStr strArr
        Assert.AreEqual(3, resultStr)

        // empty List
        CheckThrowsKeyNotFoundException (fun () -> List.findIndexBack (fun _ -> true) [] |> ignore)

        // not found
        CheckThrowsKeyNotFoundException (fun () -> List.findIndexBack (fun _ -> false) [ 1..20 ] |> ignore)

        ()

    [<Fact>]
    member _.TryPick() =
        // integer List
        let intArr = [ 1..10 ]    
        let funcInt x = 
                match x with
                | _ when x % 3 = 0 -> Some (x.ToString())            
                | _ -> None
        let resultInt = List.tryPick funcInt intArr        
        Assert.AreEqual(Some "3", resultInt)
        
        // string List
        let strArr = ["a";"b";"c";"d"]
        let funcStr x = 
                match x with
                | "good" -> Some (x.ToString())            
                | _ -> None
        let resultStr = List.tryPick funcStr strArr        
        Assert.AreEqual(None, resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = List.tryPick funcInt emptyArr        
        Assert.AreEqual(None, resultEpt)
        
        ()

    [<Fact>]
    member _.Fold() =
        // integer List
        let intArr = [ 1..10 ]    
        let funcInt x y = x+y
        let resultInt = List.fold funcInt 9 intArr        
        Assert.AreEqual(64, resultInt)
        
        // string List
        let funcStr x y = x+y            
        let resultStr = List.fold funcStr "*" ["a";"b";"c";"d"]        
        Assert.AreEqual("*abcd", resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = List.fold funcInt 5 emptyArr        
        Assert.AreEqual(5, resultEpt)

           
        ()

    [<Fact>]
    member _.Fold2() =
        // integer List  
        let funcInt x y z = x + y + z
        let resultInt = List.fold2 funcInt 9 [ 1..10 ]  [1..2..20]        
        Assert.AreEqual(164, resultInt)
        
        // string List        
        let funcStr x y z= x + y + z        
        let resultStr = List.fold2 funcStr "*" ["a"; "b";  "c" ; "d" ] ["A"; "B";  "C" ; "D" ]        
        Assert.AreEqual("*aAbBcCdD", resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = List.fold2 funcInt 5 emptyArr emptyArr        
        Assert.AreEqual(5, resultEpt)
            
        ()

    [<Fact>]
    member _.FoldBack() =
        // integer List
        let intArr = [ 1..10 ]    
        let funcInt x y = x+y
        let resultInt = List.foldBack funcInt intArr 9        
        Assert.AreEqual(64, resultInt)
        
        // string List
        let strArr = ["a"; "b";  "c" ; "d" ]
        let funcStr x y = x+y
            
        let resultStr = List.foldBack funcStr strArr "*"         
        Assert.AreEqual("abcd*", resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = List.foldBack funcInt emptyArr 5         
        Assert.AreEqual(5, resultEpt)
        
        // 1 element
        let result1Element = List.foldBack funcInt [1] 0
        Assert.AreEqual(1, result1Element)
        
        // 2 elements
        let result2Element = List.foldBack funcInt [1;2] 0
        Assert.AreEqual(3, result2Element)
        
        // 3 elements
        let result3Element = List.foldBack funcInt [1;2;3] 0
        Assert.AreEqual(6, result3Element)
        
        // 4 elements
        let result4Element = List.foldBack funcInt [1;2;3;4] 0
        Assert.AreEqual(10, result4Element)

        ()

    [<Fact>]
    member _.FoldBack2() =
        // integer List  
        let funcInt x y z = x + y + z
        let resultInt = List.foldBack2 funcInt  [ 1..10 ]  [1..2..20] 9        
        Assert.AreEqual(164, resultInt)
        
        // string List
        let funcStr x y z= x + y + z        
        let resultStr = List.foldBack2 funcStr ["A";"B";"C";"D"] ["a";"b";"c";"d"] "*"        
        Assert.AreEqual("AaBbCcDd*", resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = List.foldBack2 funcInt emptyArr emptyArr 5        
        Assert.AreEqual(5, resultEpt)
        
        //1 element
        let result1Element = List.foldBack2 funcInt [1] [1] 0
        Assert.AreEqual(2, result1Element)
        
        //2 element
        let result2Element = List.foldBack2 funcInt [1;2] [1;2] 0
        Assert.AreEqual(6, result2Element)
        
        //3 element
        let result3Element = List.foldBack2 funcInt [1;2;3] [1;2;3] 0
        Assert.AreEqual(12, result3Element)
        
        //4 element
        let result4Element = List.foldBack2 funcInt [1;2;3;4] [1;2;3;4] 0
        Assert.AreEqual(20, result4Element)
        ()
        
        //unequal length list
        let funcUnequal x y () = ()
        CheckThrowsArgumentException( fun () -> (List.foldBack2 funcUnequal  [ 1..10 ]  [1..9] ()))

        ()

    [<Fact>]
    member _.ForAll() =
        // integer List
        let resultInt = List.forall (fun x -> x > 2) [ 3..2..10 ]        
        Assert.True(resultInt)
        
        // string List
        let resultStr = List.forall (fun (x:string) -> x.Contains("a")) ["a";"b";"c";"d"]        
        Assert.False(resultStr)
        
        // empty List        
        let resultEpt = List.forall (fun (x:string) -> x.Contains("a")) []         
        Assert.True(resultEpt)
        
        ()
        
    [<Fact>]
    member _.ForAll2() =
        // integer List
        let resultInt = List.forall2 (fun x y -> x < y) [ 1..10 ] [2..2..20]        
        Assert.True(resultInt)
        
        // string List
        let resultStr = List.forall2 (fun (x:string) (y:string) -> x.Length > y.Length) ["a";"b";"c";"d"] ["A";"B";"C";"D"]          
        Assert.False(resultStr)
        
        // empty List 
        let resultEpt = List.forall2 (fun x y -> x > y) [] []        
        Assert.True(resultEpt)
        
        ()

    [<Fact>]
    member _.GroupBy() =
        let funcInt x = x%5
             
        let IntList = [ 0 .. 9 ]
                    
        let group_byInt = List.groupBy funcInt IntList
        
        let expectedIntList = 
            [ for i in 0..4 -> i, [i; i+5] ]

        Assert.AreEqual(expectedIntList, group_byInt)
             
        // string list
        let funcStr (x:string) = x.Length
        let strList = ["l1ngth7"; "length 8";  "l2ngth7" ; "length  9"]
        
        let group_byStr = List.groupBy funcStr strList
        let expectedStrList = 
            [
                7, ["l1ngth7"; "l2ngth7"]
                8, ["length 8"]
                9, ["length  9"]
            ]
       
        Assert.AreEqual(expectedStrList, group_byStr)

        // Empty list
        let emptyList = []
        let group_byEmpty = List.groupBy funcInt emptyList
        let expectedEmptyList = []

        Assert.AreEqual(expectedEmptyList, group_byEmpty)

        ()

    [<Fact>]
    member _.Hd() =
        // integer List
        let resultInt = List.head  [2..2..20]        
        Assert.AreEqual(2, resultInt)
        
        // string List
        let resultStr = List.head  ["a";"b";"c";"d"]         
        Assert.AreEqual("a", resultStr)
            
        CheckThrowsArgumentException(fun () -> List.head [] |> ignore)
        ()    

    [<Fact>]
    member _.``exactlyOne should return the element from singleton lists``() =
        Assert.AreEqual(1, List.exactlyOne [1])
        Assert.AreEqual("2", List.exactlyOne ["2"])
        ()

    [<Fact>]
    member _.``exactlyOne should fail on empty list``() = 
        CheckThrowsArgumentException(fun () -> List.exactlyOne [] |> ignore)

    [<Fact>]
    member _.``exactlyOne should fail on lists with more than one element``() =
        CheckThrowsArgumentException(fun () -> List.exactlyOne ["1"; "2"] |> ignore)

    [<Fact>]
    member _.``tryExactlyOne should return the element from singleton lists``() =
        Assert.AreEqual(Some 1, List.tryExactlyOne [1])
        Assert.AreEqual(Some "2", List.tryExactlyOne ["2"])
        ()

    [<Fact>]
    member _.``tryExactlyOne should return None for empty list``() =
        Assert.AreEqual(None, List.tryExactlyOne [])

    [<Fact>]
    member _.``tryExactlyOne should return None for lists with more than one element``() =
        Assert.AreEqual(None, List.tryExactlyOne ["1"; "2"])

    [<Fact>]
    member _.TryHead() =
        // integer List
        let resultInt = List.tryHead  [2..2..20]        
        Assert.AreEqual(2, resultInt.Value)
        
        // string List
        let resultStr = List.tryHead  ["a";"b";"c";"d"]         
        Assert.AreEqual("a", resultStr.Value)
            
        let resultNone = List.tryHead []
        Assert.AreEqual(None, resultNone)

    [<Fact>]
    member _.TryLast() =
        // integer List
        let intResult = List.tryLast [1..9]
        Assert.AreEqual(9, intResult.Value)
                 
        // string List
        let strResult = List.tryLast (["first"; "second";  "third"])
        Assert.AreEqual("third", strResult.Value)
         
        // Empty List
        let emptyResult = List.tryLast List.empty
        Assert.True(emptyResult.IsNone)
        () 

    [<Fact>]
    member _.last() =
        // last should fail on empty list
        CheckThrowsArgumentException(fun () -> List.last [] |> ignore)

        // last should return the last element from lists
        Assert.AreEqual(1, List.last [1])
        Assert.AreEqual("2", List.last ["1"; "3"; "2"])
        Assert.AreEqual(["4"], List.last [["1"; "3"]; []; ["4"]])

    [<Fact>]
    member _.Init() = 
        // integer List
        let resultInt = List.init 3 (fun x -> x + 3)         
        Assert.AreEqual([3;4;5], resultInt)
        
        // string List
        let funStr (x:int) = 
            match x with
            | 0 -> "Lists"
            | 1 -> "are"
            | 2 -> "commonly"
            | _ -> "end"
            
        let resultStr = List.init 3 funStr        
        Assert.AreEqual(["Lists"; "are";  "commonly"  ], resultStr)
        
        // empty List  
        let resultEpt = List.init 0 (fun x -> x+1)        
        Assert.AreEqual(([] : int list), resultEpt)
        
        ()

    [<Fact>]
    member _.IsEmpty() =
        // integer List
        let intArr = [ 3;4;7;8;10 ]    
        let resultInt = List.isEmpty intArr         
        Assert.False(resultInt)
        
        // string List
        let strArr = ["a";"b";"c";"d"]    
        let resultStr = List.isEmpty strArr         
        Assert.False(resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let resultEpt = List.isEmpty emptyArr         
        Assert.True(resultEpt)
        ()

    [<Fact>]
    member _.Iter() =
        // integer List
        let intArr = [ 1..10 ]  
        let mutable resultInt = 0    
        let funInt (x:int) =   
            resultInt <- resultInt + x              
            () 
        List.iter funInt intArr         
        Assert.AreEqual(55, resultInt)
        
        // string List
        let strArr = ["a";"b";"c";"d"]
        let mutable resultStr = ""
        let funStr (x:string) =
            resultStr <- resultStr + x   
            ()
        List.iter funStr strArr          
        Assert.AreEqual("abcd", resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let mutable resultEpt = 0
        List.iter funInt emptyArr         
        Assert.AreEqual(0, resultEpt)
        
        ()
       
    [<Fact>]
    member _.Iter2() =
        // integer List
        let mutable resultInt = 0    
        let funInt (x:int) (y:int) =   
            resultInt <- resultInt + x + y             
            () 
        List.iter2 funInt [ 1..10 ] [2..2..20]         
        Assert.AreEqual(165, resultInt)
        
        // string List
        let mutable resultStr = ""
        let funStr (x:string) (y:string) =
            resultStr <- resultStr + x  + y 
            ()
        List.iter2 funStr ["a";"b";"c";"d"] ["A";"B";"C";"D"]          
        Assert.AreEqual("aAbBcCdD", resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let mutable resultEpt = 0
        List.iter2 funInt emptyArr emptyArr         
        Assert.AreEqual(0, resultEpt)
        
        ()
        
    [<Fact>]
    member _.Iteri() =
        // integer List
        let intArr = [ 1..10 ]  
        let mutable resultInt = 0    
        let funInt (x:int) y =   
            resultInt <- resultInt + x + y             
            () 
        List.iteri funInt intArr         
        Assert.AreEqual(100, resultInt)
        
        // string List
        let strArr = ["a";"b";"c";"d"]
        let mutable resultStr = 0
        let funStr (x:int) (y:string) =
            resultStr <- resultStr + x + y.Length
            ()
        List.iteri funStr strArr          
        Assert.AreEqual(10, resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let mutable resultEpt = 0
        List.iteri funInt emptyArr         
        Assert.AreEqual(0, resultEpt)
        
        ()
        
    [<Fact>]
    member _.Iteri2() =
        // integer List
        let mutable resultInt = 0    
        let funInt (x:int) (y:int) (z:int) =   
            resultInt <- resultInt + x + y + z            
            () 
        List.iteri2 funInt [ 1..10 ] [2..2..20]         
        Assert.AreEqual(210, resultInt)
        
        // string List
        let mutable resultStr = ""
        let funStr (x:int) (y:string) (z:string) =
            resultStr <- resultStr + x.ToString()  + y + z
            ()
        List.iteri2 funStr ["a";"b";"c";"d"] ["A";"B";"C";"D"]          
        Assert.AreEqual("0aA1bB2cC3dD", resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let mutable resultEpt = 0
        List.iteri2 funInt emptyArr emptyArr         
        Assert.AreEqual(0, resultEpt)
        
        ()        

    [<Fact>]
    member _.Contains() =
        // integer List
        let intList = [ 2;4;6;8 ]
        let resultInt = List.contains 4 intList
        Assert.True(resultInt)

        // string List
        let strList = ["."; ".."; "..."; "...."]
        let resultStr = List.contains "....." strList
        Assert.False(resultStr)

        // float List
        let flList = [nan;infinity;5.0;-0.]     
        Assert.False(List.contains nan flList)
        Assert.False(List.contains 4.99 flList)
        Assert.True(List.contains infinity flList)
        Assert.True(List.contains 0. flList)

        let flTupleList = [(nan,"text");(5.0,"wait")]
        Assert.False(List.contains (nan,"text") flTupleList)
        Assert.False(List.contains (5.0,"text") flTupleList)
        Assert.True(List.contains (5.0,"wait") flTupleList)

        // empty List
        let emptyList:int list = [ ]
        let resultEpt = List.contains 4 emptyList
        Assert.False(resultEpt)
        
    [<Fact>]
    member _.Singleton() =
        Assert.AreEqual([null],List.singleton null)
        Assert.AreEqual(["1"],List.singleton "1")   
        Assert.AreEqual([[]],List.singleton [])
        Assert.AreEqual([[||]],List.singleton [||])
        ()

    [<Fact>]
    member _.``pairwise should return pairs of the input list``() =
        Assert.AreEqual(([] : (int*int) list), List.pairwise [1])
        Assert.AreEqual([1,2], List.pairwise [1;2])
        Assert.AreEqual([1,2; 2,3], List.pairwise [1;2;3])
        Assert.AreEqual(["H","E"; "E","L"; "L","L"; "L","O"], List.pairwise ["H";"E";"L";"L";"O"])

    [<Fact>]
    member _.``Slicing with first index reverse behaves as expected``()  = 
        let list = [ 1;2;3;4;5 ]

        Assert.AreEqual(list.[^3..], list.[1..])

    [<Fact>]
    member _.``Slicing with second index reverse behaves as expected``()  = 
        let list = [ 1;2;3;4;5 ]

        Assert.AreEqual(list.[..^1], list.[..3])

    [<Fact>]
    member _.``Slicing with both index reverse behaves as expected``()  = 
        let list = [ 1;2;3;4;5 ]

        Assert.AreEqual(list.[^3..^1], list.[1..3])

    [<Fact>]
    member _.``Slicing with first index reverse and second index non reverse behaves as expected``()=
        let list = [1;2;3;4;5]

        Assert.AreEqual(list.[^3..4], list.[1..4])

    [<Fact>]
    member _.``Slicing with first index non reverse and second index reverse behaves as expected``()=
        let list = [1;2;3;4;5]

        Assert.AreEqual(list.[3..^0], list.[3..4])

    [<Fact>]
    member _.``Get item with reverse index behaves as expected``() = 
        let list = [1;2;3;4;5]

        Assert.AreEqual(list.[^1], 4)

    [<Fact>]
    member _.RandomShuffle() =
        let list = [ 1..20 ]

        let shuffled1 = list |> List.randomShuffle
        let shuffled2 = list |> List.randomShuffle

        Assert.AreNotEqual(shuffled1, list)
        Assert.AreNotEqual(shuffled1, shuffled2)

    [<Fact>]
    member _.RandomShuffleWith() =
        let arr = [ 1..20 ]

        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let shuffle1 = arr |> List.randomShuffleWith rand1
        let shuffle2 = arr |> List.randomShuffleWith rand2
        let shuffle3 = arr |> List.randomShuffleWith rand3

        Assert.AreEqual(shuffle1, shuffle2)
        Assert.AreNotEqual(arr, shuffle1)
        Assert.AreNotEqual(shuffle1, shuffle3)

    [<Fact>]
    member _.RandomShuffleWithWrongArg() =
        let list = [ 1..20 ]
        let nullRand = null

        CheckThrowsArgumentNullException (fun () -> List.randomShuffleWith nullRand list |> ignore)

    [<Fact>]
    member _.RandomShuffleBy() =
        let arr = [ 1..20 ]

        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let shuffle1 = arr |> List.randomShuffleBy rand1.NextDouble
        let shuffle2 = arr |> List.randomShuffleBy rand2.NextDouble
        let shuffle3 = arr |> List.randomShuffleBy rand3.NextDouble

        Assert.AreEqual(shuffle1, shuffle2)
        Assert.AreNotEqual(arr, shuffle1)
        Assert.AreNotEqual(shuffle1, shuffle3)

    [<Fact>]
    member _.RandomShuffleByWrongArg() =
        let list = [ 1..20 ]
        let wrongRandomizer = fun () -> 1.0

        CheckThrowsArgumentOutOfRangeException (fun () -> List.randomShuffleBy wrongRandomizer list |> ignore)

    [<Fact>]
    member _.RandomChoice() =
        let list = [ 1..5000 ]

        // try choice five times, if all are same, it must be broken
        let results = [
            List.randomChoice list
            List.randomChoice list
            List.randomChoice list
            List.randomChoice list
            List.randomChoice list
        ]
        let allSame = results |> List.forall (fun x -> x = results.Head)
        Assert.False(allSame)

    [<Fact>]
    member _.RandomChoiceWrongArg() =
        let emptyList = []

        CheckThrowsArgumentException (fun () -> List.randomChoice emptyList |> ignore)

    [<Fact>]
    member _.RandomChoiceWith() =
        let list = [ 1..5000 ]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choice1 = list |> List.randomChoiceWith rand1
        let choice2 = list |> List.randomChoiceWith rand2
        let choice3 = list |> List.randomChoiceWith rand3

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)

    [<Fact>]
    member _.RandomChoiceWithWrongArg() =
        let emptyList = []
        let list = [ 1..20 ]
        let nullRand = null
        let rand = Random(123)

        CheckThrowsArgumentNullException (fun () -> List.randomChoiceWith nullRand list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomChoiceWith rand emptyList |> ignore)

    [<Fact>]
    member _.RandomChoiceBy() =
        let list = [ 1..5000 ]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choice1 = list |> List.randomChoiceBy rand1.NextDouble
        let choice2 = list |> List.randomChoiceBy rand2.NextDouble
        let choice3 = list |> List.randomChoiceBy rand3.NextDouble

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)

    [<Fact>]
    member _.RandomChoiceByWrongArg() =
        let emptyList = []
        let list = [ 1..20 ]
        let wrongRandomizer = fun () -> 1.0
        let randomizer = Random(123).NextDouble

        CheckThrowsArgumentOutOfRangeException (fun () -> List.randomChoiceBy wrongRandomizer list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomChoiceBy randomizer emptyList |> ignore)

    [<Fact>]
    member _.RandomChoices() =
        let list = [ 1..50 ]

        let choicesLength = 20
        let choice1 = list |> List.randomChoices choicesLength
        let choice2 = list |> List.randomChoices choicesLength

        Assert.AreNotEqual(choice1, choice2)
        Assert.AreEqual(choicesLength, choice1.Length)
        Assert.AreEqual(choicesLength, choice2.Length)

        let list = [ 1; 2 ]
        let choices = list |> List.randomChoices choicesLength
        Assert.AreEqual(choicesLength, choices.Length)
        Assert.AreEqual(list, choices |> List.distinct |> List.sort)

    [<Fact>]
    member _.RandomChoicesWrongArg() =
        let emptyList = []
        let list = [ 1..50 ]
        let choicesLength = 20
        let negativeChoicesLength = -1

        CheckThrowsArgumentException (fun () -> List.randomChoices choicesLength emptyList |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomChoices negativeChoicesLength list |> ignore)

    [<Fact>]
    member _.RandomChoicesWith() =
        let list = [ 1..50 ]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choicesLength = 20
        let choice1 = list |> List.randomChoicesWith rand1 choicesLength
        let choice2 = list |> List.randomChoicesWith rand2 choicesLength
        let choice3 = list |> List.randomChoicesWith rand3 choicesLength

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)

    [<Fact>]
    member _.RandomChoicesWithWrongArg() =
        let emptyList = []
        let list = [ 1..50 ]
        let nullRand = null
        let rand = Random(123)
        let choicesLength = 20
        let negativeChoicesLength = -1

        CheckThrowsArgumentNullException (fun () -> List.randomChoicesWith nullRand choicesLength list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomChoicesWith rand choicesLength emptyList |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomChoicesWith rand negativeChoicesLength list |> ignore)

    [<Fact>]
    member _.RandomChoicesBy() =
        let list = [ 1..50 ]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choicesLength = 20
        let choice1 = list |> List.randomChoicesBy rand1.NextDouble choicesLength
        let choice2 = list |> List.randomChoicesBy rand2.NextDouble choicesLength
        let choice3 = list |> List.randomChoicesBy rand3.NextDouble choicesLength

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)

    [<Fact>]
    member _.RandomChoicesByWrongArg() =
        let emptyList = []
        let list = [ 1..50 ]
        let wrongRandomizer = fun () -> 1.0
        let randomizer = Random(123).NextDouble
        let choicesLength = 20
        let negativeChoicesLength = -1

        CheckThrowsArgumentOutOfRangeException (fun () -> List.randomChoicesBy wrongRandomizer choicesLength list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomChoicesBy randomizer choicesLength emptyList |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomChoicesBy randomizer negativeChoicesLength list |> ignore)

    [<Fact>]
    member _.RandomSample() =
        let arr = [ 1..50 ]

        let choicesLength = 20
        let choice1 = arr |> List.randomSample choicesLength
        let choice2 = arr |> List.randomSample choicesLength

        Assert.AreNotEqual(choice1, choice2)
        Assert.AreEqual(choicesLength, choice1.Length)
        Assert.AreEqual(choicesLength, choice2.Length)
        Assert.AreEqual(choice1, choice1 |> List.distinct)
        Assert.AreEqual(choice2, choice2 |> List.distinct)

    [<Fact>]
    member _.RandomSampleWrongArg() =
        let emptyList = []
        let list = [ 1..50 ]
        let tooBigSampleLength = 100
        let negativeSampleLength = -1
        let sampleLength = 20

        CheckThrowsArgumentException (fun () -> List.randomSample sampleLength emptyList |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomSample negativeSampleLength list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomSample tooBigSampleLength list |> ignore)
    
    [<Fact>]
    member _.RandomSampleWith() =
        let list = [ 1..50 ]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choicesLength = 20
        let choice1 = list |> List.randomSampleWith rand1 choicesLength
        let choice2 = list |> List.randomSampleWith rand2 choicesLength
        let choice3 = list |> List.randomSampleWith rand3 choicesLength

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)
        Assert.AreEqual(choicesLength, choice1.Length)
        Assert.AreEqual(choicesLength, choice3.Length)
        Assert.AreEqual(choice1, choice1 |> List.distinct)
        Assert.AreEqual(choice3, choice3 |> List.distinct)

    [<Fact>]
    member _.RandomSampleWithWrongArg() =
        let emptyArr = []
        let list = [ 1..50 ]
        let nullRand = null
        let rand = Random(123)
        let tooBigSampleLength = 100
        let negativeSampleLength = -1
        let sampleLength = 20

        CheckThrowsArgumentNullException (fun () -> List.randomSampleWith nullRand sampleLength list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomSampleWith rand sampleLength emptyArr |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomSampleWith rand negativeSampleLength list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomSampleWith rand tooBigSampleLength list |> ignore)

    [<Fact>]
    member _.RandomSampleBy() =
        let list = [ 1..50 ]
        let rand1 = Random(123)
        let rand2 = Random(123)
        let rand3 = Random(321)

        let choicesLength = 20
        let choice1 = list |> List.randomSampleBy rand1.NextDouble choicesLength
        let choice2 = list |> List.randomSampleBy rand2.NextDouble choicesLength
        let choice3 = list |> List.randomSampleBy rand3.NextDouble choicesLength

        Assert.AreEqual(choice1, choice2)
        Assert.AreNotEqual(choice1, choice3)
        Assert.AreEqual(choicesLength, choice1.Length)
        Assert.AreEqual(choicesLength, choice3.Length)
        Assert.AreEqual(choice1, choice1 |> List.distinct)
        Assert.AreEqual(choice3, choice3 |> List.distinct)

    [<Fact>]
    member _.RandomSampleByWrongArg() =
        let emptyArr = []
        let list = [ 1..50 ]
        let wrongRandomizer = fun () -> 1.0
        let randomizer = Random(123).NextDouble
        let tooBigSampleLength = 100
        let negativeSampleLength = -1
        let sampleLength = 20

        CheckThrowsArgumentOutOfRangeException (fun () -> List.randomSampleBy wrongRandomizer sampleLength list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomSampleBy randomizer sampleLength emptyArr |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomSampleBy randomizer negativeSampleLength list |> ignore)
        CheckThrowsArgumentException (fun () -> List.randomSampleBy randomizer tooBigSampleLength list |> ignore)