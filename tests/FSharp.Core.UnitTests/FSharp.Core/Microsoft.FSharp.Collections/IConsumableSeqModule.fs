// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections

open System
open NUnit.Framework

open FSharp.Core.UnitTests.LibraryTestFx

#if IConsumableSeqIsPublic

// Various tests for the:
// Microsoft.FSharp.Collections.iseq type

(*
[Test Strategy]
Make sure each method works on:
* Integer IConsumableSeq (value type)
* String IConsumableSeq  (reference type)
* Empty IConsumableSeq   (0 elements)
* Null IConsumableSeq    (null)
*)

type iseq<'a> = FSharp.Collections.SeqComposition.IConsumableSeq<'a>

[<TestFixture>]
type IConsumableSeqModule() =
    let iseq (x:seq<_>) = x |> IConsumableSeq.ofSeq

    [<Test>]
    member this.AllPairs() =

        // integer IConsumableSeq
        let resultInt = IConsumableSeq.allPairs (iseq [1..7]) (iseq [11..17])
        let expectedInt =
            iseq <| seq { for i in 1..7 do
                            for j in 11..17 do
                                yield i, j }
        VerifySeqsEqual expectedInt resultInt

        // string IConsumableSeq
        let resultStr = IConsumableSeq.allPairs (iseq ["str3";"str4"]) (iseq ["str1";"str2"])
        let expectedStr = iseq ["str3","str1";"str3","str2";"str4","str1";"str4","str2"]
        VerifySeqsEqual expectedStr resultStr

        // empty IConsumableSeq
        VerifySeqsEqual IConsumableSeq.empty <| IConsumableSeq.allPairs IConsumableSeq.empty IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty <| IConsumableSeq.allPairs (iseq { 1..7 }) IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty <| IConsumableSeq.allPairs IConsumableSeq.empty (iseq { 1..7 })

        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.allPairs null null |> ignore)
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.allPairs null (iseq [1..7]) |> ignore)
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.allPairs (iseq [1..7]) null |> ignore)
        ()

    [<Test>]
    member this.CachedSeq_Clear() =
        
        let evaluatedItems : int list ref = ref []
        let cachedSeq = 
            IConsumableSeq.initInfinite (fun i -> evaluatedItems := i :: !evaluatedItems; i)
            |> IConsumableSeq.cache
        
        // Verify no items have been evaluated from the IConsumableSeq yet
        Assert.AreEqual(List.length !evaluatedItems, 0)
        
        // Force evaluation of 10 elements
        IConsumableSeq.take 10 cachedSeq
        |> IConsumableSeq.toList
        |> ignore
        
        // verify ref clear switch length
        Assert.AreEqual(List.length !evaluatedItems, 10)

        // Force evaluation of 10 elements
        IConsumableSeq.take 10 cachedSeq
        |> IConsumableSeq.toList
        |> ignore
        
        // Verify ref clear switch length (should be cached)
        Assert.AreEqual(List.length !evaluatedItems, 10)

        
        // Clear
        (box cachedSeq :?> System.IDisposable) .Dispose()
        
        // Force evaluation of 10 elements
        IConsumableSeq.take 10 cachedSeq
        |> IConsumableSeq.toList
        |> ignore
        
        // Verify length of evaluatedItemList is 20
        Assert.AreEqual(List.length !evaluatedItems, 20)
        ()
        
    [<Test>]
    member this.Append() =

        // empty IConsumableSeq 
        let emptySeq1 = IConsumableSeq.empty
        let emptySeq2 = IConsumableSeq.empty
        let appendEmptySeq = IConsumableSeq.append emptySeq1 emptySeq2
        let expectResultEmpty = IConsumableSeq.empty
           
        VerifySeqsEqual expectResultEmpty appendEmptySeq
          
        // Integer IConsumableSeq  
        let integerSeq1:iseq<int> = iseq [0..4]
        let integerSeq2:iseq<int> = iseq [5..9]
         
        let appendIntergerSeq = IConsumableSeq.append integerSeq1 integerSeq2
       
        let expectResultInteger = iseq <| seq { for i in 0..9 -> i}
        
        VerifySeqsEqual expectResultInteger appendIntergerSeq
        
        
        // String IConsumableSeq
        let stringSeq1:iseq<string> = iseq ["1";"2"]
        let stringSeq2:iseq<string> = iseq ["3";"4"]
        
        let appendStringSeq = IConsumableSeq.append stringSeq1 stringSeq2
        
        let expectedResultString = iseq ["1";"2";"3";"4"]
        
        VerifySeqsEqual expectedResultString appendStringSeq
        
        // null IConsumableSeq
        let nullSeq1 = iseq [null;null]

        let nullSeq2 =iseq [null;null]

        let appendNullSeq = IConsumableSeq.append nullSeq1 nullSeq2
        
        let expectedResultNull = iseq [ null;null;null;null]
        
        VerifySeqsEqual expectedResultNull appendNullSeq
         
        ()

    [<Test>]
    member this.replicate() =
        // replicate should create multiple copies of the given value
        Assert.IsTrue(IConsumableSeq.isEmpty <| IConsumableSeq.replicate 0 null)
        Assert.IsTrue(IConsumableSeq.isEmpty <| IConsumableSeq.replicate 0 1)
        Assert.AreEqual(null, IConsumableSeq.head <| IConsumableSeq.replicate 1 null)
        Assert.AreEqual(["1";"1"],IConsumableSeq.replicate 2 "1" |> IConsumableSeq.toList)

        CheckThrowsArgumentException (fun () ->  IConsumableSeq.replicate -1 null |> ignore)
        
        
    [<Test>]
    member this.Average() =
        // empty IConsumableSeq 
        let emptySeq:iseq<double> = IConsumableSeq.empty<double>
        
        CheckThrowsArgumentException (fun () ->  IConsumableSeq.average emptySeq |> ignore)
        
            
        // double IConsumableSeq
        let doubleSeq:iseq<double> = iseq [1.0;2.2;2.5;4.3]
        
        let averageDouble = IConsumableSeq.average doubleSeq
        
        Assert.IsFalse( averageDouble <> 2.5)
        
        // float32 IConsumableSeq
        let floatSeq:iseq<float32> = iseq [ 2.0f;4.4f;5.0f;8.6f]
        
        let averageFloat = IConsumableSeq.average floatSeq
        
        Assert.IsFalse( averageFloat <> 5.0f)
        
        // decimal IConsumableSeq
        let decimalSeq:iseq<decimal> = iseq [ 0M;19M;19.03M]
        
        let averageDecimal = IConsumableSeq.average decimalSeq
        
        Assert.IsFalse( averageDecimal <> 12.676666666666666666666666667M )
        
        //// null IConsumableSeq
        //let nullSeq:iseq<double> = null
            
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.average nullSeq |> ignore) 
        ()
        
        
    [<Test>]
    member this.AverageBy() =
        // empty IConsumableSeq 
        let emptySeq:iseq<double> = IConsumableSeq.empty<double>
        
        CheckThrowsArgumentException (fun () ->  IConsumableSeq.averageBy (fun x -> x+1.0) emptySeq |> ignore)
        
        // double IConsumableSeq
        let doubleSeq:iseq<double> = iseq [1.0;2.2;2.5;4.3]
        
        let averageDouble = IConsumableSeq.averageBy (fun x -> x-2.0) doubleSeq
        
        Assert.IsFalse( averageDouble <> 0.5 )
        
        // float32 IConsumableSeq
        let floatSeq:iseq<float32> = iseq [ 2.0f;4.4f;5.0f;8.6f]
        
        let averageFloat = IConsumableSeq.averageBy (fun x -> x*3.3f)  floatSeq
        
        Assert.IsFalse( averageFloat <> 16.5f )
        
        // decimal IConsumableSeq
        let decimalSeq:iseq<decimal> = iseq [ 0M;19M;19.03M]
        
        let averageDecimal = IConsumableSeq.averageBy (fun x -> x/10.7M) decimalSeq
        
        Assert.IsFalse( averageDecimal <> 1.1847352024922118380062305296M )
        
        //// null IConsumableSeq
        //let nullSeq:iseq<double> = null
            
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.averageBy (fun (x:double)->x+4.0) nullSeq |> ignore) 
        ()
        
    [<Test>]
    member this.Cache() =
        // empty IConsumableSeq 
        let emptySeq:iseq<double> = IConsumableSeq.empty<double>
        
        let cacheEmpty = IConsumableSeq.cache emptySeq
        
        let expectedResultEmpty = IConsumableSeq.empty
        
        VerifySeqsEqual expectedResultEmpty cacheEmpty
               
        // double IConsumableSeq
        let doubleSeq:iseq<double> = iseq [1.0;2.2;2.5;4.3]
        
        let cacheDouble = IConsumableSeq.cache doubleSeq
        
        VerifySeqsEqual doubleSeq cacheDouble
        
            
        // float32 IConsumableSeq
        let floatSeq:iseq<float32> = iseq [ 2.0f;4.4f;5.0f;8.6f]
        
        let cacheFloat = IConsumableSeq.cache floatSeq
        
        VerifySeqsEqual floatSeq cacheFloat
        
        // decimal IConsumableSeq
        let decimalSeq:iseq<decimal> = iseq [ 0M; 19M; 19.03M]
        
        let cacheDecimal = IConsumableSeq.cache decimalSeq
        
        VerifySeqsEqual decimalSeq cacheDecimal
        
        // null IConsumableSeq
        let nullSeq = iseq [null]
        
        let cacheNull = IConsumableSeq.cache nullSeq
        
        VerifySeqsEqual nullSeq cacheNull
        ()

    [<Test>]
    member this.Case() =

        // integer IConsumableSeq
        let integerArray = [|1;2|]
        let integerSeq = IConsumableSeq.cast integerArray
        
        let expectedIntegerSeq = iseq [1;2]
        
        VerifySeqsEqual expectedIntegerSeq integerSeq
        
        // string IConsumableSeq
        let stringArray = [|"a";"b"|]
        let stringSeq = IConsumableSeq.cast stringArray
        
        let expectedStringSeq = iseq["a";"b"]
        
        VerifySeqsEqual expectedStringSeq stringSeq
        
        // empty IConsumableSeq
        let emptySeq = IConsumableSeq.cast IConsumableSeq.empty
        let expectedEmptySeq = IConsumableSeq.empty
        
        VerifySeqsEqual expectedEmptySeq IConsumableSeq.empty
        
        // null IConsumableSeq
        let nullArray = [|null;null|]
        let NullSeq = IConsumableSeq.cast nullArray
        let expectedNullSeq = iseq [null;null]
        
        VerifySeqsEqual expectedNullSeq NullSeq

        CheckThrowsExn<System.InvalidCastException>(fun () -> 
            let strings = 
                integerArray 
                |> IConsumableSeq.cast<string>               
            for o in strings do ()) 
        
        CheckThrowsExn<System.InvalidCastException>(fun () -> 
            let strings = 
                integerArray 
                |> IConsumableSeq.cast<string>
                :> System.Collections.IEnumerable // without this upcast the for loop throws, so it should with this upcast too
            for o in strings do ()) 
        
        ()
        
    [<Test>]
    member this.Choose() =
        
        // int IConsumableSeq
        let intSeq = iseq [1..20]    
        let funcInt x = if (x%5=0) then Some x else None       
        let intChoosed = IConsumableSeq.choose funcInt intSeq
        let expectedIntChoosed = iseq <| seq { for i = 1 to 4 do yield i*5}
        
        
       
        VerifySeqsEqual expectedIntChoosed intChoosed
        
        // string IConsumableSeq
        let stringSrc = iseq ["list";"List"]
        let funcString x = match x with
                           | "list"-> Some x
                           | "List" -> Some x
                           | _ -> None
        let strChoosed = IConsumableSeq.choose funcString stringSrc   
        let expectedStrChoose = iseq ["list";"List"]
      
        VerifySeqsEqual expectedStrChoose strChoosed
        
        // empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let emptyChoosed = IConsumableSeq.choose funcInt emptySeq
        
        let expectedEmptyChoose = IConsumableSeq.empty
        
        VerifySeqsEqual expectedEmptyChoose emptySeq
        

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null    
        
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.choose funcInt nullSeq |> ignore) 
        ()

    [<Test>]
    member this.ChunkBySize() =

        let verify expected actual =
            IConsumableSeq.zip expected actual
            |> IConsumableSeq.iter ((<||) VerifySeqsEqual)

        // int IConsumableSeq
        verify (iseq [[1..4];[5..8]]) <| IConsumableSeq.chunkBySize 4 (iseq {1..8})
        verify (iseq [[1..4];[5..8];[9..10]]) <| IConsumableSeq.chunkBySize 4 (iseq {1..10})
        verify (iseq [[1]; [2]; [3]; [4]]) <| IConsumableSeq.chunkBySize 1 (iseq {1..4})

        IConsumableSeq.chunkBySize 2 (IConsumableSeq.initInfinite id)
        |> IConsumableSeq.take 3
        |> verify (iseq [[0;1];[2;3];[4;5]])

        IConsumableSeq.chunkBySize 1 (IConsumableSeq.initInfinite id)
        |> IConsumableSeq.take 5
        |> verify (iseq [[0];[1];[2];[3];[4]])

        // string IConsumableSeq
        verify (iseq [["a"; "b"];["c";"d"];["e"]]) <| IConsumableSeq.chunkBySize 2 (iseq ["a";"b";"c";"d";"e"])

        // empty IConsumableSeq
        verify IConsumableSeq.empty <| IConsumableSeq.chunkBySize 3 IConsumableSeq.empty

        //// null IConsumableSeq
        //let nullSeq:iseq<_> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.chunkBySize 3 nullSeq |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> IConsumableSeq.chunkBySize 0 (iseq {1..10}) |> ignore)
        CheckThrowsArgumentException (fun () -> IConsumableSeq.chunkBySize -1 (iseq {1..10}) |> ignore)

        ()

    [<Test>]
    member this.SplitInto() =

        let verify expected actual =
            IConsumableSeq.zip expected actual
            |> IConsumableSeq.iter ((<||) VerifySeqsEqual)

        // int IConsumableSeq
        IConsumableSeq.splitInto 3 (iseq {1..10}) |> verify (iseq [ {1..4}; {5..7}; {8..10} ])
        IConsumableSeq.splitInto 3 (iseq {1..11}) |> verify (iseq [ {1..4}; {5..8}; {9..11} ])
        IConsumableSeq.splitInto 3 (iseq {1..12}) |> verify (iseq [ {1..4}; {5..8}; {9..12} ])

        IConsumableSeq.splitInto 4 (iseq {1..5}) |> verify (iseq [ [1..2]; [3]; [4]; [5] ])
        IConsumableSeq.splitInto 20 (iseq {1..4}) |> verify (iseq [ [1]; [2]; [3]; [4] ])

        // string IConsumableSeq
        IConsumableSeq.splitInto 3 (iseq ["a";"b";"c";"d";"e"]) |> verify (iseq [ ["a"; "b"]; ["c";"d"]; ["e"] ])

        // empty IConsumableSeq
        VerifySeqsEqual [] <| IConsumableSeq.splitInto 3 (iseq [])

        //// null IConsumableSeq
        //let nullSeq:iseq<_> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.splitInto 3 nullSeq |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> IConsumableSeq.splitInto 0 (iseq [1..10]) |> ignore)
        CheckThrowsArgumentException (fun () -> IConsumableSeq.splitInto -1 (iseq [1..10]) |> ignore)

        ()

    [<Test>]
    member this.Compare() =
    
        // int IConsumableSeq
        let intSeq1 = iseq [1;3;7;9]    
        let intSeq2 = iseq [2;4;6;8] 
        let funcInt x y = if (x>y) then x else 0
        let intcompared = IConsumableSeq.compareWith funcInt intSeq1 intSeq2
       
        Assert.IsFalse( intcompared <> 7 )
        
        // string IConsumableSeq
        let stringSeq1 = iseq ["a"; "b"]
        let stringSeq2 = iseq ["c"; "d"]
        let funcString x y = match (x,y) with
                             | "a", "c" -> 0
                             | "b", "d" -> 1
                             |_         -> -1
        let strcompared = IConsumableSeq.compareWith funcString stringSeq1 stringSeq2  
        Assert.IsFalse( strcompared <> 1 )
         
        // empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let emptycompared = IConsumableSeq.compareWith funcInt emptySeq emptySeq
        
        Assert.IsFalse( emptycompared <> 0 )
       
        //// null IConsumableSeq
        //let nullSeq:iseq<int> = null    
         
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.compareWith funcInt nullSeq emptySeq |> ignore)  
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.compareWith funcInt emptySeq nullSeq |> ignore)  
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.compareWith funcInt nullSeq nullSeq |> ignore)  

        ()
        
    [<Test>]
    member this.Concat() =
         // integer IConsumableSeq
        let seqInt = 
            iseq <| seq { for i in 0..9 do                
                            yield iseq <| seq {for j in 0..9 do
                                                  yield i*10+j}}
        let conIntSeq = IConsumableSeq.concat seqInt
        let expectedIntSeq = iseq <| seq { for i in 0..99 do yield i}
        
        VerifySeqsEqual expectedIntSeq conIntSeq
         
        // string IConsumableSeq
        let strSeq = 
            iseq <| seq { for a in 'a' .. 'b' do
                            for b in 'a' .. 'b' do
                                yield iseq [a; b] }
     
        let conStrSeq = IConsumableSeq.concat strSeq
        let expectedStrSeq = iseq ['a';'a';'a';'b';'b';'a';'b';'b';]
        VerifySeqsEqual expectedStrSeq conStrSeq
        
        // Empty IConsumableSeq
        let emptySeqs = iseq [iseq[ IConsumableSeq.empty;IConsumableSeq.empty];iseq[ IConsumableSeq.empty;IConsumableSeq.empty]]
        let conEmptySeq = IConsumableSeq.concat emptySeqs
        let expectedEmptySeq =iseq <| seq { for i in 1..4 do yield IConsumableSeq.empty}
        
        VerifySeqsEqual expectedEmptySeq conEmptySeq   

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.concat nullSeq  |> ignore) 
 
        () 
        
    [<Test>]
    member this.CountBy() =
        // integer IConsumableSeq
        let funcIntCount_by (x:int) = x%3 
        let seqInt = 
            iseq <| seq { for i in 0..9 do                
                           yield i}
        let countIntSeq = IConsumableSeq.countByVal funcIntCount_by seqInt
         
        let expectedIntSeq = iseq [0,4;1,3;2,3]
        
        VerifySeqsEqual expectedIntSeq countIntSeq
         
        // string IConsumableSeq
        let funcStrCount_by (s:string) = s.IndexOf("key")
        let strSeq = iseq [ "key";"blank key";"key";"blank blank key"]
       
        let countStrSeq = IConsumableSeq.countByVal funcStrCount_by strSeq
        let expectedStrSeq = iseq [0,2;6,1;12,1]
        VerifySeqsEqual expectedStrSeq countStrSeq
        
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let countEmptySeq = IConsumableSeq.countByVal funcIntCount_by emptySeq
        let expectedEmptySeq =iseq []
        
        VerifySeqsEqual expectedEmptySeq countEmptySeq  

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
       
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.countBy funcIntCount_by nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.Distinct() =
        
        // integer IConsumableSeq
        let IntDistinctSeq =  
            iseq <| seq { for i in 0..9 do                
                            yield i % 3 }
       
        let DistinctIntSeq = IConsumableSeq.distinct IntDistinctSeq
       
        let expectedIntSeq = iseq [0;1;2]
        
        VerifySeqsEqual expectedIntSeq DistinctIntSeq
     
        // string IConsumableSeq
        let strDistinctSeq = iseq ["elementDup"; "ele1"; "ele2"; "elementDup"]
       
        let DistnctStrSeq = IConsumableSeq.distinct strDistinctSeq
        let expectedStrSeq = iseq ["elementDup"; "ele1"; "ele2"]
        VerifySeqsEqual expectedStrSeq DistnctStrSeq
        
        // Empty IConsumableSeq
        let emptySeq : iseq<decimal * unit>         = IConsumableSeq.empty
        let distinctEmptySeq : iseq<decimal * unit> = IConsumableSeq.distinct emptySeq
        let expectedEmptySeq : iseq<decimal * unit> = iseq []
       
        VerifySeqsEqual expectedEmptySeq distinctEmptySeq

        //// null IConsumableSeq
        //let nullSeq:iseq<unit> = null
       
        //CheckThrowsArgumentNullException(fun () -> IConsumableSeq.distinct nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.DistinctBy () =
        // integer IConsumableSeq
        let funcInt x = x % 3 
        let IntDistinct_bySeq =  
            iseq <| seq { for i in 0..9 do                
                            yield i }
       
        let distinct_byIntSeq = IConsumableSeq.distinctBy funcInt IntDistinct_bySeq
        
        let expectedIntSeq = iseq [0;1;2]
        
        VerifySeqsEqual expectedIntSeq distinct_byIntSeq
             
        // string IConsumableSeq
        let funcStrDistinct (s:string) = s.IndexOf("key")
        let strSeq = iseq [ "key"; "blank key"; "key dup"; "blank key dup"]
       
        let DistnctStrSeq = IConsumableSeq.distinctBy funcStrDistinct strSeq
        let expectedStrSeq = iseq ["key"; "blank key"]
        VerifySeqsEqual expectedStrSeq DistnctStrSeq
        
        // Empty IConsumableSeq
        let emptySeq            : iseq<int> = IConsumableSeq.empty
        let distinct_byEmptySeq : iseq<int> = IConsumableSeq.distinctBy funcInt emptySeq
        let expectedEmptySeq    : iseq<int> = iseq []
       
        VerifySeqsEqual expectedEmptySeq distinct_byEmptySeq

        //// null IConsumableSeq
        //let nullSeq : iseq<'a> = null
       
        //CheckThrowsArgumentNullException(fun () -> IConsumableSeq.distinctBy funcInt nullSeq  |> ignore) 
        () 

    [<Test>]
    member this.Except() =
        // integer IConsumableSeq
        let intSeq1 = iseq <| seq { yield! {1..100}
                                    yield! {1..100} }
        let intSeq2 = {1..10}
        let expectedIntSeq = {11..100}

        VerifySeqsEqual expectedIntSeq <| IConsumableSeq.except intSeq2 intSeq1

        // string IConsumableSeq
        let strSeq1 = iseq ["a"; "b"; "c"; "d"; "a"]
        let strSeq2 = iseq ["b"; "c"]
        let expectedStrSeq = iseq ["a"; "d"]

        VerifySeqsEqual expectedStrSeq <| IConsumableSeq.except strSeq2 strSeq1

        // double IConsumableSeq
        // Sequences with nan do not behave, due to the F# generic equality comparisons
//        let floatSeq1 = iseq [1.0; 1.0; System.Double.MaxValue; nan; nan]
//
//        VerifySeqsEqual [1.0; System.Double.MaxValue; nan; nan] <| IConsumableSeq.except [] floatSeq1
//        VerifySeqsEqual [1.0; System.Double.MaxValue] <| IConsumableSeq.except [nan] floatSeq1

        // empty IConsumableSeq
        let emptyIntSeq = IConsumableSeq.empty<int>
        VerifySeqsEqual {1..100} <| IConsumableSeq.except emptyIntSeq intSeq1
        VerifySeqsEqual emptyIntSeq <| IConsumableSeq.except intSeq1 emptyIntSeq
        VerifySeqsEqual emptyIntSeq <| IConsumableSeq.except emptyIntSeq emptyIntSeq
        VerifySeqsEqual emptyIntSeq <| IConsumableSeq.except intSeq1 intSeq1

        //// null IConsumableSeq
        //let nullSeq : iseq<int> = null
        //CheckThrowsArgumentNullException(fun () -> IConsumableSeq.except nullSeq emptyIntSeq |> ignore)
        //CheckThrowsArgumentNullException(fun () -> IConsumableSeq.except emptyIntSeq nullSeq |> ignore)
        //CheckThrowsArgumentNullException(fun () -> IConsumableSeq.except nullSeq nullSeq |> ignore)

        ()

    [<Test>]
    member this.Exists() =

        // Integer IConsumableSeq
        let funcInt x = (x % 2 = 0) 
        let IntexistsSeq =  
            iseq <| seq { for i in 0..9 do                
                            yield i}
       
        let ifExistInt = IConsumableSeq.exists funcInt IntexistsSeq
        
        Assert.IsTrue( ifExistInt) 
            
        // String IConsumableSeq
        let funcStr (s:string) = s.Contains("key")
        let strSeq = iseq ["key"; "blank key"]
       
        let ifExistStr = IConsumableSeq.exists funcStr strSeq
        
        Assert.IsTrue( ifExistStr)
        
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let ifExistsEmpty = IConsumableSeq.exists funcInt emptySeq
        
        Assert.IsFalse( ifExistsEmpty)
       
        

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
           
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.exists funcInt nullSeq |> ignore) 
        () 
    
    [<Test>]
    member this.Exists2() =
        // Integer IConsumableSeq
        let funcInt x y = (x+y)%3=0 
        let Intexists2Seq1 =  iseq [1;3;7]
        let Intexists2Seq2 = iseq [1;6;3]
            
        let ifExist2Int = IConsumableSeq.exists2 funcInt Intexists2Seq1 Intexists2Seq2
        Assert.IsTrue( ifExist2Int)
             
        // String IConsumableSeq
        let funcStr s1 s2 = ((s1 + s2) = "CombinedString")
        let strSeq1 = iseq [ "Combined"; "Not Combined"]
        let strSeq2 = iseq ["String";    "Other String"]
        let ifexists2Str = IConsumableSeq.exists2 funcStr strSeq1 strSeq2
        Assert.IsTrue(ifexists2Str)
        
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let ifexists2Empty = IConsumableSeq.exists2 funcInt emptySeq emptySeq
        Assert.IsFalse( ifexists2Empty)
       
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.exists2 funcInt nullSeq nullSeq |> ignore) 
        () 
    
    
    [<Test>]
    member this.Filter() =
        // integer IConsumableSeq
        let funcInt x = if (x % 5 = 0) then true else false
        let IntSeq =
            iseq <| seq { for i in 1..20 do
                            yield i }
                    
        let filterIntSeq = IConsumableSeq.filter funcInt IntSeq
          
        let expectedfilterInt = iseq [ 5;10;15;20]
        
        VerifySeqsEqual expectedfilterInt filterIntSeq
        
        // string IConsumableSeq
        let funcStr (s:string) = s.Contains("Expected Content")
        let strSeq = iseq [ "Expected Content"; "Not Expected"; "Expected Content"; "Not Expected"]
        
        let filterStrSeq = IConsumableSeq.filter funcStr strSeq
        
        let expectedfilterStr = iseq ["Expected Content"; "Expected Content"]
        
        VerifySeqsEqual expectedfilterStr filterStrSeq    
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let filterEmptySeq = IConsumableSeq.filter funcInt emptySeq
        
        let expectedEmptySeq =iseq []
       
        VerifySeqsEqual expectedEmptySeq filterEmptySeq
       
        

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.filter funcInt nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.Find() =
        
        // integer IConsumableSeq
        let funcInt x = if (x % 5 = 0) then true else false
        let IntSeq =
            iseq <| seq { for i in 1..20 do
                            yield i }
                    
        let findInt = IConsumableSeq.find funcInt IntSeq
        Assert.AreEqual(findInt, 5)  
             
        // string IConsumableSeq
        let funcStr (s:string) = s.Contains("Expected Content")
        let strSeq = iseq [ "Expected Content";"Not Expected"]
        
        let findStr = IConsumableSeq.find funcStr strSeq
        Assert.AreEqual(findStr, "Expected Content")
        
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        
        CheckThrowsKeyNotFoundException(fun () -> IConsumableSeq.find funcInt emptySeq |> ignore)
       
        // null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.find funcInt nullSeq |> ignore) 
        ()
    
    [<Test>]
    member this.FindBack() =
        // integer IConsumableSeq
        let funcInt x = x % 5 = 0
        Assert.AreEqual(20, IConsumableSeq.findBack funcInt <| (iseq <| seq { 1..20 }))
        Assert.AreEqual(15, IConsumableSeq.findBack funcInt <| (iseq <| seq { 1..19 }))
        Assert.AreEqual(5, IConsumableSeq.findBack funcInt <| (iseq <| seq { 5..9 }))

        // string IConsumableSeq
        let funcStr (s:string) = s.Contains("Expected")
        let strSeq = iseq [ "Not Expected"; "Expected Content"]
        let findStr = IConsumableSeq.findBack funcStr strSeq
        Assert.AreEqual("Expected Content", findStr)

        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        CheckThrowsKeyNotFoundException(fun () -> IConsumableSeq.findBack funcInt emptySeq |> ignore)

        // Not found
        let emptySeq = IConsumableSeq.empty
        CheckThrowsKeyNotFoundException(fun () -> iseq <| seq { 1..20 } |> IConsumableSeq.findBack (fun _ -> false) |> ignore)

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.findBack funcInt nullSeq |> ignore)
        ()

    [<Test>]
    member this.FindIndex() =
        
        // integer IConsumableSeq
        let digits = [1 .. 100] |> IConsumableSeq.ofList
        let idx = digits |> IConsumableSeq.findIndex (fun i -> i.ToString().Length > 1)
        Assert.AreEqual(idx, 9)

        // empty IConsumableSeq 
        CheckThrowsKeyNotFoundException(fun () -> IConsumableSeq.findIndex (fun i -> true) IConsumableSeq.empty |> ignore)
         
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.findIndex (fun i -> true) null |> ignore)
        ()
    
    [<Test>]
    member this.Permute() =
        let mapIndex i = (i + 1) % 4

        // integer iseq
        let intSeq = iseq <| seq { 1..4 }
        let resultInt = IConsumableSeq.permute mapIndex intSeq
        VerifySeqsEqual (iseq [4;1;2;3]) resultInt

        // string iseq
        let resultStr = IConsumableSeq.permute mapIndex (iseq [|"Lists"; "are";  "commonly"; "list" |])
        VerifySeqsEqual (iseq ["list"; "Lists"; "are";  "commonly" ]) resultStr

        // empty iseq
        let resultEpt = IConsumableSeq.permute mapIndex (iseq [||])
        VerifySeqsEqual IConsumableSeq.empty resultEpt

        //// null iseq
        //let nullSeq = null:string[]
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.permute mapIndex nullSeq |> ignore)

        // argument exceptions
        CheckThrowsArgumentException (fun () -> IConsumableSeq.permute (fun _ -> 10) (iseq [0..9]) |> IConsumableSeq.iter ignore)
        CheckThrowsArgumentException (fun () -> IConsumableSeq.permute (fun _ -> 0) (iseq [0..9]) |> IConsumableSeq.iter ignore)
        ()

    [<Test>]
    member this.FindIndexBack() =
        // integer IConsumableSeq
        let digits = iseq <| seq { 1..100 }
        let idx = digits |> IConsumableSeq.findIndexBack (fun i -> i.ToString().Length = 1)
        Assert.AreEqual(idx, 8)

        // string IConsumableSeq
        let funcStr (s:string) = s.Contains("Expected")
        let strSeq = iseq [ "Not Expected"; "Expected Content" ]
        let findStr = IConsumableSeq.findIndexBack funcStr strSeq
        Assert.AreEqual(1, findStr)

        // empty IConsumableSeq
        CheckThrowsKeyNotFoundException(fun () -> IConsumableSeq.findIndexBack (fun i -> true) IConsumableSeq.empty |> ignore)

        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.findIndexBack (fun i -> true) null |> ignore)
        ()

    [<Test>]
    member this.Pick() =
    
        let digits = [| 1 .. 10 |] |> IConsumableSeq.ofArray
        let result = IConsumableSeq.pick (fun i -> if i > 5 then Some(i.ToString()) else None) digits
        Assert.AreEqual(result, "6")
        
        // Empty iseq (Bugged, 4173)
        CheckThrowsKeyNotFoundException (fun () -> IConsumableSeq.pick (fun i -> Some('a')) (iseq ([| |] : int[])) |> ignore)

        //// Null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.pick (fun i -> Some(i + 0)) null |> ignore)
        ()
        
    [<Test>]
    member this.Fold() =
        let funcInt x y = x+y
             
        let IntSeq =
            iseq <| seq { for i in 1..10 do
                            yield i}
                    
        let foldInt = IConsumableSeq.fold funcInt 1 IntSeq
        if foldInt <> 56 then Assert.Fail()
        
        // string IConsumableSeq
        let funcStr (x:string) (y:string) = x+y
        let strSeq = iseq ["B"; "C";  "D" ; "E"]
        let foldStr = IConsumableSeq.fold  funcStr "A" strSeq
      
        if foldStr <> "ABCDE" then Assert.Fail()
        
        
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let foldEmpty = IConsumableSeq.fold funcInt 1 emptySeq
        if foldEmpty <> 1 then Assert.Fail()

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.fold funcInt 1 nullSeq |> ignore) 
        () 



    [<Test>]
    member this.Fold2() =
        Assert.AreEqual([(3,5); (2,3); (1,1)],IConsumableSeq.fold2 (fun acc x y -> (x,y)::acc) [] (iseq [ 1..3 ])  (iseq [1..2..6]))

        // integer List  
        let funcInt x y z = x + y + z
        let resultInt = IConsumableSeq.fold2 funcInt 9 (iseq [ 1..10 ]) (iseq [1..2..20])
        Assert.AreEqual(164, resultInt)
        
        // string List        
        let funcStr x y z = x + y + z        
        let resultStr = IConsumableSeq.fold2 funcStr "*" (iseq ["a"; "b";  "c" ; "d" ]) (iseq ["A"; "B";  "C" ; "D" ]        )
        Assert.AreEqual("*aAbBcCdD", resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = IConsumableSeq.fold2 funcInt 5 (iseq emptyArr) (iseq emptyArr)
        Assert.AreEqual(5, resultEpt)

        Assert.AreEqual(0,IConsumableSeq.fold2 funcInt 0 IConsumableSeq.empty (iseq [1]))
        Assert.AreEqual(-1,IConsumableSeq.fold2 funcInt -1 (iseq [1]) IConsumableSeq.empty)
            
        Assert.AreEqual(2,IConsumableSeq.fold2 funcInt 0 (iseq [1;2]) (iseq [1]))
        Assert.AreEqual(4,IConsumableSeq.fold2 funcInt 0 (iseq [1]) (iseq [3;6]))

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null     
        
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.fold2 funcInt 0 nullSeq (iseq [1])  |> ignore) 
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.fold2 funcInt 0 (iseq [1]) nullSeq |> ignore) 
        ()
        
    [<Test>]
    member this.FoldBack() =
        // int IConsumableSeq
        let funcInt x y = x-y
        let IntSeq = iseq <| seq { 1..4 }
        let foldInt = IConsumableSeq.foldBack funcInt IntSeq 6
        Assert.AreEqual((1-(2-(3-(4-6)))), foldInt)

        // string IConsumableSeq
        let funcStr (x:string) (y:string) = y.Remove(0,x.Length)
        let strSeq = iseq [ "A"; "B"; "C"; "D" ]
        let foldStr = IConsumableSeq.foldBack  funcStr strSeq "ABCDE"
        Assert.AreEqual("E", foldStr)
        
        // single element
        let funcStr2 elem acc = sprintf "%s%s" elem acc
        let strSeq2 = iseq [ "A" ]
        let foldStr2 = IConsumableSeq.foldBack funcStr2 strSeq2 "X"
        Assert.AreEqual("AX", foldStr2)

        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let foldEmpty = IConsumableSeq.foldBack funcInt emptySeq 1
        Assert.AreEqual(1, foldEmpty)

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.foldBack funcInt nullSeq 1 |> ignore)

        // Validate that foldBack with the cons operator and the empty list returns a copy of the sequence
        let cons x y = x :: y
        let identityFoldr = IConsumableSeq.foldBack cons IntSeq []
        Assert.AreEqual([1;2;3;4], identityFoldr)

        ()

    [<Test>]
    member this.foldBack2() =
        // int IConsumableSeq
        let funcInt x y z = x + y + z
        let intSeq = iseq <| seq { 1..10 }
        let resultInt = IConsumableSeq.foldBack2 funcInt intSeq (iseq <| seq { 1..2..20 }) 9
        Assert.AreEqual(164, resultInt)

        // string IConsumableSeq
        let funcStr = sprintf "%s%s%s"
        let strSeq = iseq [ "A"; "B"; "C"; "D" ]
        let resultStr = IConsumableSeq.foldBack2  funcStr strSeq (iseq [ "a"; "b"; "c"; "d"]) "*"
        Assert.AreEqual("AaBbCcDd*", resultStr)

        // single element
        let strSeqSingle = iseq [ "X" ]
        Assert.AreEqual("XAZ", IConsumableSeq.foldBack2 funcStr strSeqSingle strSeq "Z")
        Assert.AreEqual("AXZ", IConsumableSeq.foldBack2 funcStr strSeq strSeqSingle "Z")
        Assert.AreEqual("XYZ", IConsumableSeq.foldBack2 funcStr strSeqSingle (iseq [ "Y" ]) "Z")

        // empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        Assert.AreEqual(1, IConsumableSeq.foldBack2 funcInt emptySeq emptySeq 1)
        Assert.AreEqual(1, IConsumableSeq.foldBack2 funcInt emptySeq intSeq 1)
        Assert.AreEqual(1, IConsumableSeq.foldBack2 funcInt intSeq emptySeq 1)

        // infinite IConsumableSeq
        let infiniteSeq = IConsumableSeq.initInfinite (fun i -> 2 * i + 1)
        Assert.AreEqual(164, IConsumableSeq.foldBack2 funcInt intSeq infiniteSeq 9)
        Assert.AreEqual(164, IConsumableSeq.foldBack2 funcInt infiniteSeq intSeq 9)

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.foldBack2 funcInt nullSeq intSeq 1 |> ignore)
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.foldBack2 funcInt intSeq nullSeq 1 |> ignore)
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.foldBack2 funcInt nullSeq nullSeq 1 |> ignore)

        ()

    [<Test>]
    member this.ForAll() =

        let funcInt x  = if x%2 = 0 then true else false
        let IntSeq =
            iseq <| seq { for i in 1..10 do
                            yield i*2}
        let for_allInt = IConsumableSeq.forall funcInt  IntSeq
           
        if for_allInt <> true then Assert.Fail()
        
             
        // string IConsumableSeq
        let funcStr (x:string)  = x.Contains("a")
        let strSeq = iseq ["a"; "ab";  "abc" ; "abcd"]
        let for_allStr = IConsumableSeq.forall  funcStr strSeq
       
        if for_allStr <> true then Assert.Fail()
        
        
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let for_allEmpty = IConsumableSeq.forall funcInt emptySeq
        
        if for_allEmpty <> true then Assert.Fail()
        
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.forall funcInt  nullSeq |> ignore) 
        () 
        
    [<Test>]
    member this.ForAll2() =

        let funcInt x y = if (x+y)%2 = 0 then true else false
        let IntSeq =
            iseq <| seq { for i in 1..10 do
                            yield i}
                    
        let for_all2Int = IConsumableSeq.forall2 funcInt  IntSeq IntSeq
           
        if for_all2Int <> true then Assert.Fail()
        
        // string IConsumableSeq
        let funcStr (x:string) (y:string)  = (x+y).Length = 5
        let strSeq1 = iseq ["a"; "ab";  "abc" ; "abcd"]
        let strSeq2 = iseq ["abcd"; "abc";  "ab" ; "a"]
        let for_all2Str = IConsumableSeq.forall2  funcStr strSeq1 strSeq2
       
        if for_all2Str <> true then Assert.Fail()
        
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let for_all2Empty = IConsumableSeq.forall2 funcInt emptySeq emptySeq
        
        if for_all2Empty <> true then Assert.Fail()

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.forall2 funcInt  nullSeq nullSeq |> ignore) 
        
    [<Test>]
    member this.GroupBy() =
        
        let funcInt x = x%5
             
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 do
                            yield i }
                    
        let group_byInt = IConsumableSeq.groupByVal funcInt IntSeq |> IConsumableSeq.map (fun (i, v) -> i, IConsumableSeq.toList v)
        
        let expectedIntSeq = 
            iseq <| seq { for i in 0..4 do
                            yield i, [i; i+5] }
                   
        VerifySeqsEqual group_byInt expectedIntSeq
             
        // string IConsumableSeq
        let funcStr (x:string) = x.Length
        let strSeq = iseq ["length7"; "length 8";  "length7" ; "length  9"]
        
        let group_byStr = IConsumableSeq.groupByVal  funcStr strSeq |> IConsumableSeq.map (fun (i, v) -> i, IConsumableSeq.toList v)
        let expectedStrSeq = 
            iseq <| seq {
                yield 7, ["length7"; "length7"]
                yield 8, ["length 8"]
                yield 9, ["length  9"] }
       
        VerifySeqsEqual expectedStrSeq group_byStr
        
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let group_byEmpty = IConsumableSeq.groupByVal funcInt emptySeq
        let expectedEmptySeq = iseq []

        VerifySeqsEqual expectedEmptySeq group_byEmpty
        
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //let group_byNull = IConsumableSeq.groupBy funcInt nullSeq
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.iter (fun _ -> ()) group_byNull) 
        () 
    
    [<Test>]
    member this.DisposalOfUnstartedEnumerator() =
        let run = ref false
        let f() = iseq <| seq {                
                try
                    ()
                finally 
                    run := true
              }
  
        f().GetEnumerator().Dispose() 
        Assert.IsFalse(!run)

    [<Test>]
    member this.WeirdLocalNames() =
       
        let f pc = iseq <| seq {                
                yield pc
                yield (pc+1)
                yield (pc+2)
              }
  
        let l = f 3 |> IConsumableSeq.toList
        Assert.AreEqual([3;4;5], l)

        let f i = iseq <| seq {                
                let pc = i*2
                yield pc
                yield (pc+1)
                yield (pc+2)
              }
        let l = f 3 |> IConsumableSeq.toList
        Assert.AreEqual([6;7;8], l)

    [<Test>]
    member this.Contains() =

        // Integer IConsumableSeq
        let intSeq = iseq <| seq { 0..9 }

        let ifContainsInt = IConsumableSeq.contains 5 intSeq

        Assert.IsTrue(ifContainsInt)

        // String IConsumableSeq
        let strSeq = iseq ["key"; "blank key"]

        let ifContainsStr = IConsumableSeq.contains "key" strSeq

        Assert.IsTrue(ifContainsStr)

        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        let ifContainsEmpty = IConsumableSeq.contains 5 emptySeq

        Assert.IsFalse(ifContainsEmpty)

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null

        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.contains 5 nullSeq |> ignore)

#endif