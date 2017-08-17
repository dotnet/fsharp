// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections

open System
open NUnit.Framework

open FSharp.Core.Unittests.LibraryTestFx

// Various tests for the:
// Microsoft.FSharp.Collections.iseq type

(*
[Test Strategy]
Make sure each method works on:
* Integer ISeq (value type)
* String ISeq  (reference type)
* Empty ISeq   (0 elements)
* Null ISeq    (null)
*)

type iseq<'a> = FSharp.Collections.SeqComposition.ISeq<'a>

[<TestFixture>]
type ISeqModule() =
    let iseq (x:seq<_>) = x |> ISeq.ofSeq

    [<Test>]
    member this.AllPairs() =

        // integer ISeq
        let resultInt = ISeq.allPairs (iseq [1..7]) (iseq [11..17])
        let expectedInt =
            iseq <| seq { for i in 1..7 do
                            for j in 11..17 do
                                yield i, j }
        VerifySeqsEqual expectedInt resultInt

        // string ISeq
        let resultStr = ISeq.allPairs (iseq ["str3";"str4"]) (iseq ["str1";"str2"])
        let expectedStr = iseq ["str3","str1";"str3","str2";"str4","str1";"str4","str2"]
        VerifySeqsEqual expectedStr resultStr

        // empty ISeq
        VerifySeqsEqual ISeq.empty <| ISeq.allPairs ISeq.empty ISeq.empty
        VerifySeqsEqual ISeq.empty <| ISeq.allPairs (iseq { 1..7 }) ISeq.empty
        VerifySeqsEqual ISeq.empty <| ISeq.allPairs ISeq.empty (iseq { 1..7 })

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.allPairs null null |> ignore)
        //CheckThrowsArgumentNullException(fun() -> ISeq.allPairs null (iseq [1..7]) |> ignore)
        //CheckThrowsArgumentNullException(fun() -> ISeq.allPairs (iseq [1..7]) null |> ignore)
        ()

    [<Test>]
    member this.CachedSeq_Clear() =
        
        let evaluatedItems : int list ref = ref []
        let cachedSeq = 
            ISeq.initInfinite (fun i -> evaluatedItems := i :: !evaluatedItems; i)
            |> ISeq.cache
        
        // Verify no items have been evaluated from the ISeq yet
        Assert.AreEqual(List.length !evaluatedItems, 0)
        
        // Force evaluation of 10 elements
        ISeq.take 10 cachedSeq
        |> ISeq.toList
        |> ignore
        
        // verify ref clear switch length
        Assert.AreEqual(List.length !evaluatedItems, 10)

        // Force evaluation of 10 elements
        ISeq.take 10 cachedSeq
        |> ISeq.toList
        |> ignore
        
        // Verify ref clear switch length (should be cached)
        Assert.AreEqual(List.length !evaluatedItems, 10)

        
        // Clear
        (box cachedSeq :?> System.IDisposable) .Dispose()
        
        // Force evaluation of 10 elements
        ISeq.take 10 cachedSeq
        |> ISeq.toList
        |> ignore
        
        // Verify length of evaluatedItemList is 20
        Assert.AreEqual(List.length !evaluatedItems, 20)
        ()
        
    [<Test>]
    member this.Append() =

        // empty ISeq 
        let emptySeq1 = ISeq.empty
        let emptySeq2 = ISeq.empty
        let appendEmptySeq = ISeq.append emptySeq1 emptySeq2
        let expectResultEmpty = ISeq.empty
           
        VerifySeqsEqual expectResultEmpty appendEmptySeq
          
        // Integer ISeq  
        let integerSeq1:iseq<int> = iseq [0..4]
        let integerSeq2:iseq<int> = iseq [5..9]
         
        let appendIntergerSeq = ISeq.append integerSeq1 integerSeq2
       
        let expectResultInteger = iseq <| seq { for i in 0..9 -> i}
        
        VerifySeqsEqual expectResultInteger appendIntergerSeq
        
        
        // String ISeq
        let stringSeq1:iseq<string> = iseq ["1";"2"]
        let stringSeq2:iseq<string> = iseq ["3";"4"]
        
        let appendStringSeq = ISeq.append stringSeq1 stringSeq2
        
        let expectedResultString = iseq ["1";"2";"3";"4"]
        
        VerifySeqsEqual expectedResultString appendStringSeq
        
        // null ISeq
        let nullSeq1 = iseq [null;null]

        let nullSeq2 =iseq [null;null]

        let appendNullSeq = ISeq.append nullSeq1 nullSeq2
        
        let expectedResultNull = iseq [ null;null;null;null]
        
        VerifySeqsEqual expectedResultNull appendNullSeq
         
        ()

    [<Test>]
    member this.replicate() =
        // replicate should create multiple copies of the given value
        Assert.IsTrue(ISeq.isEmpty <| ISeq.replicate 0 null)
        Assert.IsTrue(ISeq.isEmpty <| ISeq.replicate 0 1)
        Assert.AreEqual(null, ISeq.head <| ISeq.replicate 1 null)
        Assert.AreEqual(["1";"1"],ISeq.replicate 2 "1" |> ISeq.toList)

        CheckThrowsArgumentException (fun () ->  ISeq.replicate -1 null |> ignore)
        
        
    [<Test>]
    member this.Average() =
        // empty ISeq 
        let emptySeq:iseq<double> = ISeq.empty<double>
        
        CheckThrowsArgumentException (fun () ->  ISeq.average emptySeq |> ignore)
        
            
        // double ISeq
        let doubleSeq:iseq<double> = iseq [1.0;2.2;2.5;4.3]
        
        let averageDouble = ISeq.average doubleSeq
        
        Assert.IsFalse( averageDouble <> 2.5)
        
        // float32 ISeq
        let floatSeq:iseq<float32> = iseq [ 2.0f;4.4f;5.0f;8.6f]
        
        let averageFloat = ISeq.average floatSeq
        
        Assert.IsFalse( averageFloat <> 5.0f)
        
        // decimal ISeq
        let decimalSeq:iseq<decimal> = iseq [ 0M;19M;19.03M]
        
        let averageDecimal = ISeq.average decimalSeq
        
        Assert.IsFalse( averageDecimal <> 12.676666666666666666666666667M )
        
        //// null ISeq
        //let nullSeq:iseq<double> = null
            
        //CheckThrowsArgumentNullException (fun () -> ISeq.average nullSeq |> ignore) 
        ()
        
        
    [<Test>]
    member this.AverageBy() =
        // empty ISeq 
        let emptySeq:iseq<double> = ISeq.empty<double>
        
        CheckThrowsArgumentException (fun () ->  ISeq.averageBy (fun x -> x+1.0) emptySeq |> ignore)
        
        // double ISeq
        let doubleSeq:iseq<double> = iseq [1.0;2.2;2.5;4.3]
        
        let averageDouble = ISeq.averageBy (fun x -> x-2.0) doubleSeq
        
        Assert.IsFalse( averageDouble <> 0.5 )
        
        // float32 ISeq
        let floatSeq:iseq<float32> = iseq [ 2.0f;4.4f;5.0f;8.6f]
        
        let averageFloat = ISeq.averageBy (fun x -> x*3.3f)  floatSeq
        
        Assert.IsFalse( averageFloat <> 16.5f )
        
        // decimal ISeq
        let decimalSeq:iseq<decimal> = iseq [ 0M;19M;19.03M]
        
        let averageDecimal = ISeq.averageBy (fun x -> x/10.7M) decimalSeq
        
        Assert.IsFalse( averageDecimal <> 1.1847352024922118380062305296M )
        
        //// null ISeq
        //let nullSeq:iseq<double> = null
            
        //CheckThrowsArgumentNullException (fun () -> ISeq.averageBy (fun (x:double)->x+4.0) nullSeq |> ignore) 
        ()
        
    [<Test>]
    member this.Cache() =
        // empty ISeq 
        let emptySeq:iseq<double> = ISeq.empty<double>
        
        let cacheEmpty = ISeq.cache emptySeq
        
        let expectedResultEmpty = ISeq.empty
        
        VerifySeqsEqual expectedResultEmpty cacheEmpty
               
        // double ISeq
        let doubleSeq:iseq<double> = iseq [1.0;2.2;2.5;4.3]
        
        let cacheDouble = ISeq.cache doubleSeq
        
        VerifySeqsEqual doubleSeq cacheDouble
        
            
        // float32 ISeq
        let floatSeq:iseq<float32> = iseq [ 2.0f;4.4f;5.0f;8.6f]
        
        let cacheFloat = ISeq.cache floatSeq
        
        VerifySeqsEqual floatSeq cacheFloat
        
        // decimal ISeq
        let decimalSeq:iseq<decimal> = iseq [ 0M; 19M; 19.03M]
        
        let cacheDecimal = ISeq.cache decimalSeq
        
        VerifySeqsEqual decimalSeq cacheDecimal
        
        // null ISeq
        let nullSeq = iseq [null]
        
        let cacheNull = ISeq.cache nullSeq
        
        VerifySeqsEqual nullSeq cacheNull
        ()

    [<Test>]
    member this.Case() =

        // integer ISeq
        let integerArray = [|1;2|]
        let integerSeq = ISeq.cast integerArray
        
        let expectedIntegerSeq = iseq [1;2]
        
        VerifySeqsEqual expectedIntegerSeq integerSeq
        
        // string ISeq
        let stringArray = [|"a";"b"|]
        let stringSeq = ISeq.cast stringArray
        
        let expectedStringSeq = iseq["a";"b"]
        
        VerifySeqsEqual expectedStringSeq stringSeq
        
        // empty ISeq
        let emptySeq = ISeq.cast ISeq.empty
        let expectedEmptySeq = ISeq.empty
        
        VerifySeqsEqual expectedEmptySeq ISeq.empty
        
        // null ISeq
        let nullArray = [|null;null|]
        let NullSeq = ISeq.cast nullArray
        let expectedNullSeq = iseq [null;null]
        
        VerifySeqsEqual expectedNullSeq NullSeq

        CheckThrowsExn<System.InvalidCastException>(fun () -> 
            let strings = 
                integerArray 
                |> ISeq.cast<string>               
            for o in strings do ()) 
        
        CheckThrowsExn<System.InvalidCastException>(fun () -> 
            let strings = 
                integerArray 
                |> ISeq.cast<string>
                :> System.Collections.IEnumerable // without this upcast the for loop throws, so it should with this upcast too
            for o in strings do ()) 
        
        ()
        
    [<Test>]
    member this.Choose() =
        
        // int ISeq
        let intSeq = iseq [1..20]    
        let funcInt x = if (x%5=0) then Some x else None       
        let intChoosed = ISeq.choose funcInt intSeq
        let expectedIntChoosed = iseq <| seq { for i = 1 to 4 do yield i*5}
        
        
       
        VerifySeqsEqual expectedIntChoosed intChoosed
        
        // string ISeq
        let stringSrc = iseq ["list";"List"]
        let funcString x = match x with
                           | "list"-> Some x
                           | "List" -> Some x
                           | _ -> None
        let strChoosed = ISeq.choose funcString stringSrc   
        let expectedStrChoose = iseq ["list";"List"]
      
        VerifySeqsEqual expectedStrChoose strChoosed
        
        // empty ISeq
        let emptySeq = ISeq.empty
        let emptyChoosed = ISeq.choose funcInt emptySeq
        
        let expectedEmptyChoose = ISeq.empty
        
        VerifySeqsEqual expectedEmptyChoose emptySeq
        

        //// null ISeq
        //let nullSeq:iseq<'a> = null    
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.choose funcInt nullSeq |> ignore) 
        ()

    [<Test>]
    member this.ChunkBySize() =

        let verify expected actual =
            ISeq.zip expected actual
            |> ISeq.iter ((<||) VerifySeqsEqual)

        // int ISeq
        verify (iseq [[1..4];[5..8]]) <| ISeq.chunkBySize 4 (iseq {1..8})
        verify (iseq [[1..4];[5..8];[9..10]]) <| ISeq.chunkBySize 4 (iseq {1..10})
        verify (iseq [[1]; [2]; [3]; [4]]) <| ISeq.chunkBySize 1 (iseq {1..4})

        ISeq.chunkBySize 2 (ISeq.initInfinite id)
        |> ISeq.take 3
        |> verify (iseq [[0;1];[2;3];[4;5]])

        ISeq.chunkBySize 1 (ISeq.initInfinite id)
        |> ISeq.take 5
        |> verify (iseq [[0];[1];[2];[3];[4]])

        // string ISeq
        verify (iseq [["a"; "b"];["c";"d"];["e"]]) <| ISeq.chunkBySize 2 (iseq ["a";"b";"c";"d";"e"])

        // empty ISeq
        verify ISeq.empty <| ISeq.chunkBySize 3 ISeq.empty

        //// null ISeq
        //let nullSeq:iseq<_> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.chunkBySize 3 nullSeq |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> ISeq.chunkBySize 0 (iseq {1..10}) |> ignore)
        CheckThrowsArgumentException (fun () -> ISeq.chunkBySize -1 (iseq {1..10}) |> ignore)

        ()

    [<Test>]
    member this.SplitInto() =

        let verify expected actual =
            ISeq.zip expected actual
            |> ISeq.iter ((<||) VerifySeqsEqual)

        // int ISeq
        ISeq.splitInto 3 (iseq {1..10}) |> verify (iseq [ {1..4}; {5..7}; {8..10} ])
        ISeq.splitInto 3 (iseq {1..11}) |> verify (iseq [ {1..4}; {5..8}; {9..11} ])
        ISeq.splitInto 3 (iseq {1..12}) |> verify (iseq [ {1..4}; {5..8}; {9..12} ])

        ISeq.splitInto 4 (iseq {1..5}) |> verify (iseq [ [1..2]; [3]; [4]; [5] ])
        ISeq.splitInto 20 (iseq {1..4}) |> verify (iseq [ [1]; [2]; [3]; [4] ])

        // string ISeq
        ISeq.splitInto 3 (iseq ["a";"b";"c";"d";"e"]) |> verify (iseq [ ["a"; "b"]; ["c";"d"]; ["e"] ])

        // empty ISeq
        VerifySeqsEqual [] <| ISeq.splitInto 3 (iseq [])

        //// null ISeq
        //let nullSeq:iseq<_> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.splitInto 3 nullSeq |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> ISeq.splitInto 0 (iseq [1..10]) |> ignore)
        CheckThrowsArgumentException (fun () -> ISeq.splitInto -1 (iseq [1..10]) |> ignore)

        ()

    [<Test>]
    member this.Compare() =
    
        // int ISeq
        let intSeq1 = iseq [1;3;7;9]    
        let intSeq2 = iseq [2;4;6;8] 
        let funcInt x y = if (x>y) then x else 0
        let intcompared = ISeq.compareWith funcInt intSeq1 intSeq2
       
        Assert.IsFalse( intcompared <> 7 )
        
        // string ISeq
        let stringSeq1 = iseq ["a"; "b"]
        let stringSeq2 = iseq ["c"; "d"]
        let funcString x y = match (x,y) with
                             | "a", "c" -> 0
                             | "b", "d" -> 1
                             |_         -> -1
        let strcompared = ISeq.compareWith funcString stringSeq1 stringSeq2  
        Assert.IsFalse( strcompared <> 1 )
         
        // empty ISeq
        let emptySeq = ISeq.empty
        let emptycompared = ISeq.compareWith funcInt emptySeq emptySeq
        
        Assert.IsFalse( emptycompared <> 0 )
       
        //// null ISeq
        //let nullSeq:iseq<int> = null    
         
        //CheckThrowsArgumentNullException (fun () -> ISeq.compareWith funcInt nullSeq emptySeq |> ignore)  
        //CheckThrowsArgumentNullException (fun () -> ISeq.compareWith funcInt emptySeq nullSeq |> ignore)  
        //CheckThrowsArgumentNullException (fun () -> ISeq.compareWith funcInt nullSeq nullSeq |> ignore)  

        ()
        
    [<Test>]
    member this.Concat() =
         // integer ISeq
        let seqInt = 
            iseq <| seq { for i in 0..9 do                
                            yield iseq <| seq {for j in 0..9 do
                                                  yield i*10+j}}
        let conIntSeq = ISeq.concat seqInt
        let expectedIntSeq = iseq <| seq { for i in 0..99 do yield i}
        
        VerifySeqsEqual expectedIntSeq conIntSeq
         
        // string ISeq
        let strSeq = 
            iseq <| seq { for a in 'a' .. 'b' do
                            for b in 'a' .. 'b' do
                                yield iseq [a; b] }
     
        let conStrSeq = ISeq.concat strSeq
        let expectedStrSeq = iseq ['a';'a';'a';'b';'b';'a';'b';'b';]
        VerifySeqsEqual expectedStrSeq conStrSeq
        
        // Empty ISeq
        let emptySeqs = iseq [iseq[ ISeq.empty;ISeq.empty];iseq[ ISeq.empty;ISeq.empty]]
        let conEmptySeq = ISeq.concat emptySeqs
        let expectedEmptySeq =iseq <| seq { for i in 1..4 do yield ISeq.empty}
        
        VerifySeqsEqual expectedEmptySeq conEmptySeq   

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.concat nullSeq  |> ignore) 
 
        () 
        
    [<Test>]
    member this.CountBy() =
        // integer ISeq
        let funcIntCount_by (x:int) = x%3 
        let seqInt = 
            iseq <| seq { for i in 0..9 do                
                           yield i}
        let countIntSeq = ISeq.countByVal funcIntCount_by seqInt
         
        let expectedIntSeq = iseq [0,4;1,3;2,3]
        
        VerifySeqsEqual expectedIntSeq countIntSeq
         
        // string ISeq
        let funcStrCount_by (s:string) = s.IndexOf("key")
        let strSeq = iseq [ "key";"blank key";"key";"blank blank key"]
       
        let countStrSeq = ISeq.countByVal funcStrCount_by strSeq
        let expectedStrSeq = iseq [0,2;6,1;12,1]
        VerifySeqsEqual expectedStrSeq countStrSeq
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let countEmptySeq = ISeq.countByVal funcIntCount_by emptySeq
        let expectedEmptySeq =iseq []
        
        VerifySeqsEqual expectedEmptySeq countEmptySeq  

        //// null ISeq
        //let nullSeq:iseq<'a> = null
       
        //CheckThrowsArgumentNullException (fun () -> ISeq.countBy funcIntCount_by nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.Distinct() =
        
        // integer ISeq
        let IntDistinctSeq =  
            iseq <| seq { for i in 0..9 do                
                            yield i % 3 }
       
        let DistinctIntSeq = ISeq.distinct IntDistinctSeq
       
        let expectedIntSeq = iseq [0;1;2]
        
        VerifySeqsEqual expectedIntSeq DistinctIntSeq
     
        // string ISeq
        let strDistinctSeq = iseq ["elementDup"; "ele1"; "ele2"; "elementDup"]
       
        let DistnctStrSeq = ISeq.distinct strDistinctSeq
        let expectedStrSeq = iseq ["elementDup"; "ele1"; "ele2"]
        VerifySeqsEqual expectedStrSeq DistnctStrSeq
        
        // Empty ISeq
        let emptySeq : iseq<decimal * unit>         = ISeq.empty
        let distinctEmptySeq : iseq<decimal * unit> = ISeq.distinct emptySeq
        let expectedEmptySeq : iseq<decimal * unit> = iseq []
       
        VerifySeqsEqual expectedEmptySeq distinctEmptySeq

        //// null ISeq
        //let nullSeq:iseq<unit> = null
       
        //CheckThrowsArgumentNullException(fun () -> ISeq.distinct nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.DistinctBy () =
        // integer ISeq
        let funcInt x = x % 3 
        let IntDistinct_bySeq =  
            iseq <| seq { for i in 0..9 do                
                            yield i }
       
        let distinct_byIntSeq = ISeq.distinctBy funcInt IntDistinct_bySeq
        
        let expectedIntSeq = iseq [0;1;2]
        
        VerifySeqsEqual expectedIntSeq distinct_byIntSeq
             
        // string ISeq
        let funcStrDistinct (s:string) = s.IndexOf("key")
        let strSeq = iseq [ "key"; "blank key"; "key dup"; "blank key dup"]
       
        let DistnctStrSeq = ISeq.distinctBy funcStrDistinct strSeq
        let expectedStrSeq = iseq ["key"; "blank key"]
        VerifySeqsEqual expectedStrSeq DistnctStrSeq
        
        // Empty ISeq
        let emptySeq            : iseq<int> = ISeq.empty
        let distinct_byEmptySeq : iseq<int> = ISeq.distinctBy funcInt emptySeq
        let expectedEmptySeq    : iseq<int> = iseq []
       
        VerifySeqsEqual expectedEmptySeq distinct_byEmptySeq

        //// null ISeq
        //let nullSeq : iseq<'a> = null
       
        //CheckThrowsArgumentNullException(fun () -> ISeq.distinctBy funcInt nullSeq  |> ignore) 
        () 

    [<Test>]
    member this.Except() =
        // integer ISeq
        let intSeq1 = iseq <| seq { yield! {1..100}
                                    yield! {1..100} }
        let intSeq2 = {1..10}
        let expectedIntSeq = {11..100}

        VerifySeqsEqual expectedIntSeq <| ISeq.except intSeq2 intSeq1

        // string ISeq
        let strSeq1 = iseq ["a"; "b"; "c"; "d"; "a"]
        let strSeq2 = iseq ["b"; "c"]
        let expectedStrSeq = iseq ["a"; "d"]

        VerifySeqsEqual expectedStrSeq <| ISeq.except strSeq2 strSeq1

        // double ISeq
        // Sequences with nan do not behave, due to the F# generic equality comparisons
//        let floatSeq1 = iseq [1.0; 1.0; System.Double.MaxValue; nan; nan]
//
//        VerifySeqsEqual [1.0; System.Double.MaxValue; nan; nan] <| ISeq.except [] floatSeq1
//        VerifySeqsEqual [1.0; System.Double.MaxValue] <| ISeq.except [nan] floatSeq1

        // empty ISeq
        let emptyIntSeq = ISeq.empty<int>
        VerifySeqsEqual {1..100} <| ISeq.except emptyIntSeq intSeq1
        VerifySeqsEqual emptyIntSeq <| ISeq.except intSeq1 emptyIntSeq
        VerifySeqsEqual emptyIntSeq <| ISeq.except emptyIntSeq emptyIntSeq
        VerifySeqsEqual emptyIntSeq <| ISeq.except intSeq1 intSeq1

        //// null ISeq
        //let nullSeq : iseq<int> = null
        //CheckThrowsArgumentNullException(fun () -> ISeq.except nullSeq emptyIntSeq |> ignore)
        //CheckThrowsArgumentNullException(fun () -> ISeq.except emptyIntSeq nullSeq |> ignore)
        //CheckThrowsArgumentNullException(fun () -> ISeq.except nullSeq nullSeq |> ignore)

        ()

    [<Test>]
    member this.Exists() =

        // Integer ISeq
        let funcInt x = (x % 2 = 0) 
        let IntexistsSeq =  
            iseq <| seq { for i in 0..9 do                
                            yield i}
       
        let ifExistInt = ISeq.exists funcInt IntexistsSeq
        
        Assert.IsTrue( ifExistInt) 
            
        // String ISeq
        let funcStr (s:string) = s.Contains("key")
        let strSeq = iseq ["key"; "blank key"]
       
        let ifExistStr = ISeq.exists funcStr strSeq
        
        Assert.IsTrue( ifExistStr)
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let ifExistsEmpty = ISeq.exists funcInt emptySeq
        
        Assert.IsFalse( ifExistsEmpty)
       
        

        //// null ISeq
        //let nullSeq:iseq<'a> = null
           
        //CheckThrowsArgumentNullException (fun () -> ISeq.exists funcInt nullSeq |> ignore) 
        () 
    
    [<Test>]
    member this.Exists2() =
        // Integer ISeq
        let funcInt x y = (x+y)%3=0 
        let Intexists2Seq1 =  iseq [1;3;7]
        let Intexists2Seq2 = iseq [1;6;3]
            
        let ifExist2Int = ISeq.exists2 funcInt Intexists2Seq1 Intexists2Seq2
        Assert.IsTrue( ifExist2Int)
             
        // String ISeq
        let funcStr s1 s2 = ((s1 + s2) = "CombinedString")
        let strSeq1 = iseq [ "Combined"; "Not Combined"]
        let strSeq2 = iseq ["String";    "Other String"]
        let ifexists2Str = ISeq.exists2 funcStr strSeq1 strSeq2
        Assert.IsTrue(ifexists2Str)
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let ifexists2Empty = ISeq.exists2 funcInt emptySeq emptySeq
        Assert.IsFalse( ifexists2Empty)
       
        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.exists2 funcInt nullSeq nullSeq |> ignore) 
        () 
    
    
    [<Test>]
    member this.Filter() =
        // integer ISeq
        let funcInt x = if (x % 5 = 0) then true else false
        let IntSeq =
            iseq <| seq { for i in 1..20 do
                            yield i }
                    
        let filterIntSeq = ISeq.filter funcInt IntSeq
          
        let expectedfilterInt = iseq [ 5;10;15;20]
        
        VerifySeqsEqual expectedfilterInt filterIntSeq
        
        // string ISeq
        let funcStr (s:string) = s.Contains("Expected Content")
        let strSeq = iseq [ "Expected Content"; "Not Expected"; "Expected Content"; "Not Expected"]
        
        let filterStrSeq = ISeq.filter funcStr strSeq
        
        let expectedfilterStr = iseq ["Expected Content"; "Expected Content"]
        
        VerifySeqsEqual expectedfilterStr filterStrSeq    
        // Empty ISeq
        let emptySeq = ISeq.empty
        let filterEmptySeq = ISeq.filter funcInt emptySeq
        
        let expectedEmptySeq =iseq []
       
        VerifySeqsEqual expectedEmptySeq filterEmptySeq
       
        

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.filter funcInt nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.Find() =
        
        // integer ISeq
        let funcInt x = if (x % 5 = 0) then true else false
        let IntSeq =
            iseq <| seq { for i in 1..20 do
                            yield i }
                    
        let findInt = ISeq.find funcInt IntSeq
        Assert.AreEqual(findInt, 5)  
             
        // string ISeq
        let funcStr (s:string) = s.Contains("Expected Content")
        let strSeq = iseq [ "Expected Content";"Not Expected"]
        
        let findStr = ISeq.find funcStr strSeq
        Assert.AreEqual(findStr, "Expected Content")
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        
        CheckThrowsKeyNotFoundException(fun () -> ISeq.find funcInt emptySeq |> ignore)
       
        // null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.find funcInt nullSeq |> ignore) 
        ()
    
    [<Test>]
    member this.FindBack() =
        // integer ISeq
        let funcInt x = x % 5 = 0
        Assert.AreEqual(20, ISeq.findBack funcInt <| (iseq <| seq { 1..20 }))
        Assert.AreEqual(15, ISeq.findBack funcInt <| (iseq <| seq { 1..19 }))
        Assert.AreEqual(5, ISeq.findBack funcInt <| (iseq <| seq { 5..9 }))

        // string ISeq
        let funcStr (s:string) = s.Contains("Expected")
        let strSeq = iseq [ "Not Expected"; "Expected Content"]
        let findStr = ISeq.findBack funcStr strSeq
        Assert.AreEqual("Expected Content", findStr)

        // Empty ISeq
        let emptySeq = ISeq.empty
        CheckThrowsKeyNotFoundException(fun () -> ISeq.findBack funcInt emptySeq |> ignore)

        // Not found
        let emptySeq = ISeq.empty
        CheckThrowsKeyNotFoundException(fun () -> iseq <| seq { 1..20 } |> ISeq.findBack (fun _ -> false) |> ignore)

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.findBack funcInt nullSeq |> ignore)
        ()

    [<Test>]
    member this.FindIndex() =
        
        // integer ISeq
        let digits = [1 .. 100] |> ISeq.ofList
        let idx = digits |> ISeq.findIndex (fun i -> i.ToString().Length > 1)
        Assert.AreEqual(idx, 9)

        // empty ISeq 
        CheckThrowsKeyNotFoundException(fun () -> ISeq.findIndex (fun i -> true) ISeq.empty |> ignore)
         
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.findIndex (fun i -> true) null |> ignore)
        ()
    
    [<Test>]
    member this.Permute() =
        let mapIndex i = (i + 1) % 4

        // integer iseq
        let intSeq = iseq <| seq { 1..4 }
        let resultInt = ISeq.permute mapIndex intSeq
        VerifySeqsEqual (iseq [4;1;2;3]) resultInt

        // string iseq
        let resultStr = ISeq.permute mapIndex (iseq [|"Lists"; "are";  "commonly"; "list" |])
        VerifySeqsEqual (iseq ["list"; "Lists"; "are";  "commonly" ]) resultStr

        // empty iseq
        let resultEpt = ISeq.permute mapIndex (iseq [||])
        VerifySeqsEqual ISeq.empty resultEpt

        //// null iseq
        //let nullSeq = null:string[]
        //CheckThrowsArgumentNullException (fun () -> ISeq.permute mapIndex nullSeq |> ignore)

        // argument exceptions
        CheckThrowsArgumentException (fun () -> ISeq.permute (fun _ -> 10) (iseq [0..9]) |> ISeq.iter ignore)
        CheckThrowsArgumentException (fun () -> ISeq.permute (fun _ -> 0) (iseq [0..9]) |> ISeq.iter ignore)
        ()

    [<Test>]
    member this.FindIndexBack() =
        // integer ISeq
        let digits = iseq <| seq { 1..100 }
        let idx = digits |> ISeq.findIndexBack (fun i -> i.ToString().Length = 1)
        Assert.AreEqual(idx, 8)

        // string ISeq
        let funcStr (s:string) = s.Contains("Expected")
        let strSeq = iseq [ "Not Expected"; "Expected Content" ]
        let findStr = ISeq.findIndexBack funcStr strSeq
        Assert.AreEqual(1, findStr)

        // empty ISeq
        CheckThrowsKeyNotFoundException(fun () -> ISeq.findIndexBack (fun i -> true) ISeq.empty |> ignore)

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.findIndexBack (fun i -> true) null |> ignore)
        ()

    [<Test>]
    member this.Pick() =
    
        let digits = [| 1 .. 10 |] |> ISeq.ofArray
        let result = ISeq.pick (fun i -> if i > 5 then Some(i.ToString()) else None) digits
        Assert.AreEqual(result, "6")
        
        // Empty iseq (Bugged, 4173)
        CheckThrowsKeyNotFoundException (fun () -> ISeq.pick (fun i -> Some('a')) (iseq ([| |] : int[])) |> ignore)

        //// Null
        //CheckThrowsArgumentNullException (fun () -> ISeq.pick (fun i -> Some(i + 0)) null |> ignore)
        ()
        
    [<Test>]
    member this.Fold() =
        let funcInt x y = x+y
             
        let IntSeq =
            iseq <| seq { for i in 1..10 do
                            yield i}
                    
        let foldInt = ISeq.fold funcInt 1 IntSeq
        if foldInt <> 56 then Assert.Fail()
        
        // string ISeq
        let funcStr (x:string) (y:string) = x+y
        let strSeq = iseq ["B"; "C";  "D" ; "E"]
        let foldStr = ISeq.fold  funcStr "A" strSeq
      
        if foldStr <> "ABCDE" then Assert.Fail()
        
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let foldEmpty = ISeq.fold funcInt 1 emptySeq
        if foldEmpty <> 1 then Assert.Fail()

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.fold funcInt 1 nullSeq |> ignore) 
        () 



    [<Test>]
    member this.Fold2() =
        Assert.AreEqual([(3,5); (2,3); (1,1)],ISeq.fold2 (fun acc x y -> (x,y)::acc) [] (iseq [ 1..3 ])  (iseq [1..2..6]))

        // integer List  
        let funcInt x y z = x + y + z
        let resultInt = ISeq.fold2 funcInt 9 (iseq [ 1..10 ]) (iseq [1..2..20])
        Assert.AreEqual(164, resultInt)
        
        // string List        
        let funcStr x y z = x + y + z        
        let resultStr = ISeq.fold2 funcStr "*" (iseq ["a"; "b";  "c" ; "d" ]) (iseq ["A"; "B";  "C" ; "D" ]        )
        Assert.AreEqual("*aAbBcCdD", resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = ISeq.fold2 funcInt 5 (iseq emptyArr) (iseq emptyArr)
        Assert.AreEqual(5, resultEpt)

        Assert.AreEqual(0,ISeq.fold2 funcInt 0 ISeq.empty (iseq [1]))
        Assert.AreEqual(-1,ISeq.fold2 funcInt -1 (iseq [1]) ISeq.empty)
            
        Assert.AreEqual(2,ISeq.fold2 funcInt 0 (iseq [1;2]) (iseq [1]))
        Assert.AreEqual(4,ISeq.fold2 funcInt 0 (iseq [1]) (iseq [3;6]))

        //// null ISeq
        //let nullSeq:iseq<'a> = null     
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.fold2 funcInt 0 nullSeq (iseq [1])  |> ignore) 
        //CheckThrowsArgumentNullException (fun () -> ISeq.fold2 funcInt 0 (iseq [1]) nullSeq |> ignore) 
        ()
        
    [<Test>]
    member this.FoldBack() =
        // int ISeq
        let funcInt x y = x-y
        let IntSeq = iseq <| seq { 1..4 }
        let foldInt = ISeq.foldBack funcInt IntSeq 6
        Assert.AreEqual((1-(2-(3-(4-6)))), foldInt)

        // string ISeq
        let funcStr (x:string) (y:string) = y.Remove(0,x.Length)
        let strSeq = iseq [ "A"; "B"; "C"; "D" ]
        let foldStr = ISeq.foldBack  funcStr strSeq "ABCDE"
        Assert.AreEqual("E", foldStr)
        
        // single element
        let funcStr2 elem acc = sprintf "%s%s" elem acc
        let strSeq2 = iseq [ "A" ]
        let foldStr2 = ISeq.foldBack funcStr2 strSeq2 "X"
        Assert.AreEqual("AX", foldStr2)

        // Empty ISeq
        let emptySeq = ISeq.empty
        let foldEmpty = ISeq.foldBack funcInt emptySeq 1
        Assert.AreEqual(1, foldEmpty)

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.foldBack funcInt nullSeq 1 |> ignore)

        // Validate that foldBack with the cons operator and the empty list returns a copy of the sequence
        let cons x y = x :: y
        let identityFoldr = ISeq.foldBack cons IntSeq []
        Assert.AreEqual([1;2;3;4], identityFoldr)

        ()

    [<Test>]
    member this.foldBack2() =
        // int ISeq
        let funcInt x y z = x + y + z
        let intSeq = iseq <| seq { 1..10 }
        let resultInt = ISeq.foldBack2 funcInt intSeq (iseq <| seq { 1..2..20 }) 9
        Assert.AreEqual(164, resultInt)

        // string ISeq
        let funcStr = sprintf "%s%s%s"
        let strSeq = iseq [ "A"; "B"; "C"; "D" ]
        let resultStr = ISeq.foldBack2  funcStr strSeq (iseq [ "a"; "b"; "c"; "d"]) "*"
        Assert.AreEqual("AaBbCcDd*", resultStr)

        // single element
        let strSeqSingle = iseq [ "X" ]
        Assert.AreEqual("XAZ", ISeq.foldBack2 funcStr strSeqSingle strSeq "Z")
        Assert.AreEqual("AXZ", ISeq.foldBack2 funcStr strSeq strSeqSingle "Z")
        Assert.AreEqual("XYZ", ISeq.foldBack2 funcStr strSeqSingle (iseq [ "Y" ]) "Z")

        // empty ISeq
        let emptySeq = ISeq.empty
        Assert.AreEqual(1, ISeq.foldBack2 funcInt emptySeq emptySeq 1)
        Assert.AreEqual(1, ISeq.foldBack2 funcInt emptySeq intSeq 1)
        Assert.AreEqual(1, ISeq.foldBack2 funcInt intSeq emptySeq 1)

        // infinite ISeq
        let infiniteSeq = ISeq.initInfinite (fun i -> 2 * i + 1)
        Assert.AreEqual(164, ISeq.foldBack2 funcInt intSeq infiniteSeq 9)
        Assert.AreEqual(164, ISeq.foldBack2 funcInt infiniteSeq intSeq 9)

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.foldBack2 funcInt nullSeq intSeq 1 |> ignore)
        //CheckThrowsArgumentNullException (fun () -> ISeq.foldBack2 funcInt intSeq nullSeq 1 |> ignore)
        //CheckThrowsArgumentNullException (fun () -> ISeq.foldBack2 funcInt nullSeq nullSeq 1 |> ignore)

        ()

    [<Test>]
    member this.ForAll() =

        let funcInt x  = if x%2 = 0 then true else false
        let IntSeq =
            iseq <| seq { for i in 1..10 do
                            yield i*2}
        let for_allInt = ISeq.forall funcInt  IntSeq
           
        if for_allInt <> true then Assert.Fail()
        
             
        // string ISeq
        let funcStr (x:string)  = x.Contains("a")
        let strSeq = iseq ["a"; "ab";  "abc" ; "abcd"]
        let for_allStr = ISeq.forall  funcStr strSeq
       
        if for_allStr <> true then Assert.Fail()
        
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let for_allEmpty = ISeq.forall funcInt emptySeq
        
        if for_allEmpty <> true then Assert.Fail()
        
        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.forall funcInt  nullSeq |> ignore) 
        () 
        
    [<Test>]
    member this.ForAll2() =

        let funcInt x y = if (x+y)%2 = 0 then true else false
        let IntSeq =
            iseq <| seq { for i in 1..10 do
                            yield i}
                    
        let for_all2Int = ISeq.forall2 funcInt  IntSeq IntSeq
           
        if for_all2Int <> true then Assert.Fail()
        
        // string ISeq
        let funcStr (x:string) (y:string)  = (x+y).Length = 5
        let strSeq1 = iseq ["a"; "ab";  "abc" ; "abcd"]
        let strSeq2 = iseq ["abcd"; "abc";  "ab" ; "a"]
        let for_all2Str = ISeq.forall2  funcStr strSeq1 strSeq2
       
        if for_all2Str <> true then Assert.Fail()
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let for_all2Empty = ISeq.forall2 funcInt emptySeq emptySeq
        
        if for_all2Empty <> true then Assert.Fail()

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.forall2 funcInt  nullSeq nullSeq |> ignore) 
        
    [<Test>]
    member this.GroupBy() =
        
        let funcInt x = x%5
             
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 do
                            yield i }
                    
        let group_byInt = ISeq.groupByVal funcInt IntSeq |> ISeq.map (fun (i, v) -> i, ISeq.toList v)
        
        let expectedIntSeq = 
            iseq <| seq { for i in 0..4 do
                            yield i, [i; i+5] }
                   
        VerifySeqsEqual group_byInt expectedIntSeq
             
        // string ISeq
        let funcStr (x:string) = x.Length
        let strSeq = iseq ["length7"; "length 8";  "length7" ; "length  9"]
        
        let group_byStr = ISeq.groupByVal  funcStr strSeq |> ISeq.map (fun (i, v) -> i, ISeq.toList v)
        let expectedStrSeq = 
            iseq <| seq {
                yield 7, ["length7"; "length7"]
                yield 8, ["length 8"]
                yield 9, ["length  9"] }
       
        VerifySeqsEqual expectedStrSeq group_byStr
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let group_byEmpty = ISeq.groupByVal funcInt emptySeq
        let expectedEmptySeq = iseq []

        VerifySeqsEqual expectedEmptySeq group_byEmpty
        
        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //let group_byNull = ISeq.groupBy funcInt nullSeq
        //CheckThrowsArgumentNullException (fun () -> ISeq.iter (fun _ -> ()) group_byNull) 
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
  
        let l = f 3 |> ISeq.toList
        Assert.AreEqual([3;4;5], l)

        let f i = iseq <| seq {                
                let pc = i*2
                yield pc
                yield (pc+1)
                yield (pc+2)
              }
        let l = f 3 |> ISeq.toList
        Assert.AreEqual([6;7;8], l)

    [<Test>]
    member this.Contains() =

        // Integer ISeq
        let intSeq = iseq <| seq { 0..9 }

        let ifContainsInt = ISeq.contains 5 intSeq

        Assert.IsTrue(ifContainsInt)

        // String ISeq
        let strSeq = iseq ["key"; "blank key"]

        let ifContainsStr = ISeq.contains "key" strSeq

        Assert.IsTrue(ifContainsStr)

        // Empty ISeq
        let emptySeq = ISeq.empty
        let ifContainsEmpty = ISeq.contains 5 emptySeq

        Assert.IsFalse(ifContainsEmpty)

        //// null ISeq
        //let nullSeq:iseq<'a> = null

        //CheckThrowsArgumentNullException (fun () -> ISeq.contains 5 nullSeq |> ignore)
