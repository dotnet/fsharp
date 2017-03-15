// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections

open System
open NUnit.Framework

open FSharp.Core.Unittests.LibraryTestFx

// Various tests for the:
// Microsoft.FSharp.Collections.seq type

(*
[Test Strategy]
Make sure each method works on:
* Integer ISeq (value type)
* String ISeq  (reference type)
* Empty ISeq   (0 elements)
* Null ISeq    (null)
*)

[<TestFixture>]
type ISeqModule() =

    [<Test>]
    member this.AllPairs() =

        // integer ISeq
        let resultInt = Seq.allPairs (seq [1..7]) (seq [11..17])
        let expectedInt =
            seq { for i in 1..7 do
                    for j in 11..17 do
                        yield i, j }
        VerifySeqsEqual expectedInt resultInt

        // string ISeq
        let resultStr = Seq.allPairs (seq ["str3";"str4"]) (seq ["str1";"str2"])
        let expectedStr = seq ["str3","str1";"str3","str2";"str4","str1";"str4","str2"]
        VerifySeqsEqual expectedStr resultStr

        // empty ISeq
        VerifySeqsEqual ISeq.empty <| Seq.allPairs ISeq.empty ISeq.empty
        VerifySeqsEqual ISeq.empty <| Seq.allPairs { 1..7 } ISeq.empty
        VerifySeqsEqual ISeq.empty <| Seq.allPairs ISeq.empty { 1..7 }

        // null ISeq
        CheckThrowsArgumentNullException(fun() -> Seq.allPairs null null |> ignore)
        CheckThrowsArgumentNullException(fun() -> Seq.allPairs null (seq [1..7]) |> ignore)
        CheckThrowsArgumentNullException(fun() -> Seq.allPairs (seq [1..7]) null |> ignore)
        ()

    [<Test>]
    member this.CachedSeq_Clear() =
        
        let evaluatedItems : int list ref = ref []
        let cachedSeq = 
            ISeq.initInfinite (fun i -> evaluatedItems := i :: !evaluatedItems; i)
            |> Seq.cache
            |> ISeq.ofSeq
        
        // Verify no items have been evaluated from the ISeq yet
        Assert.AreEqual(List.length !evaluatedItems, 0)
        
        // Force evaluation of 10 elements
        ISeq.take 10 cachedSeq
        |> Seq.toList
        |> ignore
        
        // verify ref clear switch length
        Assert.AreEqual(List.length !evaluatedItems, 10)

        // Force evaluation of 10 elements
        ISeq.take 10 cachedSeq
        |> Seq.toList
        |> ignore
        
        // Verify ref clear switch length (should be cached)
        Assert.AreEqual(List.length !evaluatedItems, 10)

        
        // Clear
        (box cachedSeq :?> System.IDisposable) .Dispose()
        
        // Force evaluation of 10 elements
        ISeq.take 10 cachedSeq
        |> Seq.toList
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
        let integerSeq1 = seq [0..4] |> ISeq.ofSeq
        let integerSeq2 = seq [5..9] |> ISeq.ofSeq
         
        let appendIntergerSeq = ISeq.append integerSeq1 integerSeq2
       
        let expectResultInteger = seq { for i in 0..9 -> i}
        
        VerifySeqsEqual expectResultInteger appendIntergerSeq
        
        
        // String ISeq
        let stringSeq1 = seq ["1";"2"] |> ISeq.ofSeq
        let stringSeq2 = seq ["3";"4"] |> ISeq.ofSeq
        
        let appendStringSeq = ISeq.append stringSeq1 stringSeq2
        
        let expectedResultString = seq ["1";"2";"3";"4"]
        
        VerifySeqsEqual expectedResultString appendStringSeq
        
        // null ISeq
        let nullSeq1 = seq [null;null] |> ISeq.ofSeq
        let nullSeq2 = seq [null;null] |> ISeq.ofSeq

        let appendNullSeq = ISeq.append nullSeq1 nullSeq2
        
        let expectedResultNull = seq [ null;null;null;null]
        
        VerifySeqsEqual expectedResultNull appendNullSeq
         
        ()

    [<Test>]
    member this.replicate() =
        // replicate should create multiple copies of the given value
        Assert.IsTrue(Seq.isEmpty <| Seq.replicate 0 null)
        Assert.IsTrue(Seq.isEmpty <| Seq.replicate 0 1)
        Assert.AreEqual(null, ISeq.head <| (Seq.replicate 1 null |> ISeq.ofSeq))
        Assert.AreEqual(["1";"1"],Seq.replicate 2 "1" |> Seq.toList)

        CheckThrowsArgumentException (fun () ->  Seq.replicate -1 null |> ignore)
        
        
    [<Test>]
    member this.Average() =
        // empty ISeq 
        let emptySeq = ISeq.empty<double>
        
        CheckThrowsArgumentException (fun () ->  ISeq.average emptySeq |> ignore)
        
            
        // double ISeq
        let doubleSeq = seq [1.0;2.2;2.5;4.3] |> ISeq.ofSeq
        
        let averageDouble = ISeq.average doubleSeq
        
        Assert.IsFalse( averageDouble <> 2.5)
        
        // float32 ISeq
        let floatSeq = seq [ 2.0f;4.4f;5.0f;8.6f] |> ISeq.ofSeq
        
        let averageFloat = ISeq.average floatSeq
        
        Assert.IsFalse( averageFloat <> 5.0f)
        
        // decimal ISeq
        let decimalSeq = seq [ 0M;19M;19.03M] |> ISeq.ofSeq
        
        let averageDecimal = ISeq.average decimalSeq
        
        Assert.IsFalse( averageDecimal <> 12.676666666666666666666666667M )
        
        // null ISeq
        //let nullSeq : ISeq.Core.ISeq<double> = null
            
        //CheckThrowsArgumentNullException (fun () -> ISeq.average nullSeq |> ignore) 
        ()
        
        
    [<Test>]
    member this.AverageBy() =
        // empty ISeq 
        let emptySeq = ISeq.empty<double>
        
        CheckThrowsArgumentException (fun () ->  ISeq.averageBy (fun x -> x+1.0) emptySeq |> ignore)
        
        // double ISeq
        let doubleSeq = seq [1.0;2.2;2.5;4.3] |> ISeq.ofSeq
        
        let averageDouble = ISeq.averageBy (fun x -> x-2.0) doubleSeq
        
        Assert.IsFalse( averageDouble <> 0.5 )
        
        // float32 ISeq
        let floatSeq = seq [ 2.0f;4.4f;5.0f;8.6f] |> ISeq.ofSeq
        
        let averageFloat = ISeq.averageBy (fun x -> x*3.3f)  floatSeq
        
        Assert.IsFalse( averageFloat <> 16.5f )
        
        // decimal ISeq
        let decimalSeq = seq [ 0M;19M;19.03M] |> ISeq.ofSeq
        
        let averageDecimal = ISeq.averageBy (fun x -> x/10.7M) decimalSeq
        
        Assert.IsFalse( averageDecimal <> 1.1847352024922118380062305296M )
        
        //// null ISeq
        //let nullSeq = null
            
        //CheckThrowsArgumentNullException (fun () -> ISeq.averageBy (fun (x:double)->x+4.0) nullSeq |> ignore) 
        ()
        
    [<Test>]
    member this.Cache() =
        // empty ISeq 
        let emptySeq = ISeq.empty<double>
        
        let cacheEmpty = Seq.cache emptySeq
        
        let expectedResultEmpty = ISeq.empty
        
        VerifySeqsEqual expectedResultEmpty cacheEmpty
               
        // double ISeq
        let doubleSeq = seq [1.0;2.2;2.5;4.3] |> ISeq.ofSeq
        
        let cacheDouble = Seq.cache doubleSeq
        
        VerifySeqsEqual doubleSeq cacheDouble
        
            
        // float32 ISeq
        let floatSeq = seq [ 2.0f;4.4f;5.0f;8.6f]
        
        let cacheFloat = Seq.cache floatSeq
        
        VerifySeqsEqual floatSeq cacheFloat
        
        // decimal ISeq
        let decimalSeq = seq [ 0M; 19M; 19.03M]
        
        let cacheDecimal = Seq.cache decimalSeq
        
        VerifySeqsEqual decimalSeq cacheDecimal
        
        // null ISeq
        let nullSeq = seq [null]
        
        let cacheNull = Seq.cache nullSeq
        
        VerifySeqsEqual nullSeq cacheNull
        ()

    [<Test>]
    member this.Case() =

        // integer ISeq
        let integerArray = [|1;2|]
        let integerSeq = Seq.cast integerArray
        
        let expectedIntegerSeq = seq [1;2]
        
        VerifySeqsEqual expectedIntegerSeq integerSeq
        
        // string ISeq
        let stringArray = [|"a";"b"|]
        let stringSeq = Seq.cast stringArray
        
        let expectedStringSeq = seq["a";"b"]
        
        VerifySeqsEqual expectedStringSeq stringSeq
        
        // empty ISeq
        let emptySeq = Seq.cast ISeq.empty
        let expectedEmptySeq = ISeq.empty
        
        VerifySeqsEqual expectedEmptySeq ISeq.empty
        
        // null ISeq
        let nullArray = [|null;null|]
        let NullSeq = Seq.cast nullArray
        let expectedNullSeq = seq [null;null]
        
        VerifySeqsEqual expectedNullSeq NullSeq

        CheckThrowsExn<System.InvalidCastException>(fun () -> 
            let strings = 
                integerArray 
                |> Seq.cast<string>               
            for o in strings do ()) 
        
        CheckThrowsExn<System.InvalidCastException>(fun () -> 
            let strings = 
                integerArray 
                |> Seq.cast<string>
                :> System.Collections.IEnumerable // without this upcast the for loop throws, so it should with this upcast too
            for o in strings do ()) 
        
        ()
        
    [<Test>]
    member this.Choose() =
        
        // int ISeq
        let intSeq = seq [1..20] |> ISeq.ofSeq
        let funcInt x = if (x%5=0) then Some x else None       
        let intChoosed = ISeq.choose funcInt intSeq
        let expectedIntChoosed = seq { for i = 1 to 4 do yield i*5}
        
        
       
        VerifySeqsEqual expectedIntChoosed intChoosed
        
        // string ISeq
        let stringSrc = seq ["list";"List"] |> ISeq.ofSeq
        let funcString x = match x with
                           | "list"-> Some x
                           | "List" -> Some x
                           | _ -> None
        let strChoosed = ISeq.choose funcString stringSrc   
        let expectedStrChoose = seq ["list";"List"]
      
        VerifySeqsEqual expectedStrChoose strChoosed
        
        // empty ISeq
        let emptySeq = ISeq.empty
        let emptyChoosed = ISeq.choose funcInt emptySeq
        
        let expectedEmptyChoose = ISeq.empty
        
        VerifySeqsEqual expectedEmptyChoose emptySeq
        

        // null ISeq
        //let nullSeq = null    
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.choose funcInt nullSeq |> ignore) 
        ()

    [<Test>]
    member this.ChunkBySize() =

        let verify expected actual =
            ISeq.zip (expected |> ISeq.ofSeq) (actual |> ISeq.ofSeq)
            |> ISeq.iter ((<||) VerifySeqsEqual)

        // int ISeq
        verify [[1..4];[5..8]] <| Seq.chunkBySize 4 {1..8}
        verify [[1..4];[5..8];[9..10]] <| Seq.chunkBySize 4 {1..10}
        verify [[1]; [2]; [3]; [4]] <| Seq.chunkBySize 1 {1..4}

        Seq.chunkBySize 2 (ISeq.initInfinite id)
        |> ISeq.ofSeq
        |> ISeq.take 3
        |> verify [[0;1];[2;3];[4;5]]

        Seq.chunkBySize 1 (ISeq.initInfinite id)
        |> ISeq.ofSeq
        |> ISeq.take 5
        |> verify [[0];[1];[2];[3];[4]]

        // string ISeq
        verify [["a"; "b"];["c";"d"];["e"]] <| Seq.chunkBySize 2 ["a";"b";"c";"d";"e"]

        // empty ISeq
        verify ISeq.empty <| Seq.chunkBySize 3 ISeq.empty

        // null ISeq
        let nullSeq:seq<_> = null
        CheckThrowsArgumentNullException (fun () -> Seq.chunkBySize 3 nullSeq |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> Seq.chunkBySize 0 {1..10} |> ignore)
        CheckThrowsArgumentException (fun () -> Seq.chunkBySize -1 {1..10} |> ignore)

        ()

    [<Test>]
    member this.SplitInto() =

        let verify expected actual =
            ISeq.zip (expected |> ISeq.ofSeq) (actual |> ISeq.ofSeq)
            |> ISeq.iter ((<||) VerifySeqsEqual)

        // int ISeq
        Seq.splitInto 3 {1..10} |> verify (seq [ {1..4}; {5..7}; {8..10} ])
        Seq.splitInto 3 {1..11} |> verify (seq [ {1..4}; {5..8}; {9..11} ])
        Seq.splitInto 3 {1..12} |> verify (seq [ {1..4}; {5..8}; {9..12} ])

        Seq.splitInto 4 {1..5} |> verify (seq [ [1..2]; [3]; [4]; [5] ])
        Seq.splitInto 20 {1..4} |> verify (seq [ [1]; [2]; [3]; [4] ])

        // string ISeq
        Seq.splitInto 3 ["a";"b";"c";"d";"e"] |> verify ([ ["a"; "b"]; ["c";"d"]; ["e"] ])

        // empty ISeq
        VerifySeqsEqual [] <| Seq.splitInto 3 []

        // null ISeq
        let nullSeq:seq<_> = null
        CheckThrowsArgumentNullException (fun () -> Seq.splitInto 3 nullSeq |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> Seq.splitInto 0 [1..10] |> ignore)
        CheckThrowsArgumentException (fun () -> Seq.splitInto -1 [1..10] |> ignore)

        ()

    [<Test>]
    member this.Compare() =
    
        // int ISeq
        let intSeq1 = seq [1;3;7;9] |> ISeq.ofSeq
        let intSeq2 = seq [2;4;6;8] |> ISeq.ofSeq 
        let funcInt x y = if (x>y) then x else 0
        let intcompared = ISeq.compareWith funcInt intSeq1 intSeq2
       
        Assert.IsFalse( intcompared <> 7 )
        
        // string ISeq
        let stringSeq1 = seq ["a"; "b"] |> ISeq.ofSeq
        let stringSeq2 = seq ["c"; "d"] |> ISeq.ofSeq
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
       
        // null ISeq
        //let nullSeq = null    
         
        //CheckThrowsArgumentNullException (fun () -> ISeq.compareWith funcInt nullSeq emptySeq |> ignore)  
        //CheckThrowsArgumentNullException (fun () -> ISeq.compareWith funcInt emptySeq nullSeq |> ignore)  
        //CheckThrowsArgumentNullException (fun () -> ISeq.compareWith funcInt nullSeq nullSeq |> ignore)  

        ()
        
    [<Test>]
    member this.Concat() =
         // integer ISeq
        let seqInt = 
            seq { for i in 0..9 do                
                    yield seq {for j in 0..9 do
                                yield i*10+j} |> ISeq.ofSeq } |> ISeq.ofSeq
        let conIntSeq = ISeq.concat seqInt
        let expectedIntSeq = seq { for i in 0..99 do yield i}
        
        VerifySeqsEqual expectedIntSeq conIntSeq
         
        // string ISeq
        let strSeq = 
            seq { for a in 'a' .. 'b' do
                    for b in 'a' .. 'b' do
                        yield seq [a; b] |> ISeq.ofSeq }|> ISeq.ofSeq
     
        let conStrSeq = ISeq.concat strSeq
        let expectedStrSeq = seq ['a';'a';'a';'b';'b';'a';'b';'b';]
        VerifySeqsEqual expectedStrSeq conStrSeq
        
        // Empty ISeq
        let emptySeqs = seq [seq[ ISeq.empty;ISeq.empty]|> ISeq.ofSeq;seq[ ISeq.empty;ISeq.empty]|> ISeq.ofSeq]|> ISeq.ofSeq
        let conEmptySeq = ISeq.concat emptySeqs
        let expectedEmptySeq =seq { for i in 1..4 do yield ISeq.empty}
        
        VerifySeqsEqual expectedEmptySeq conEmptySeq   

        //// null ISeq
        //let nullSeq = null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.concat nullSeq  |> ignore) 
 
        () 
        
    [<Test>]
    member this.CountBy() =
        // integer ISeq
        let funcIntCount_by (x:int) = x%3 
        let seqInt = 
            seq { for i in 0..9 do                
                    yield i}
        let countIntSeq = Seq.countBy funcIntCount_by seqInt
         
        let expectedIntSeq = seq [0,4;1,3;2,3]
        
        VerifySeqsEqual expectedIntSeq countIntSeq
         
        // string ISeq
        let funcStrCount_by (s:string) = s.IndexOf("key")
        let strSeq = seq [ "key";"blank key";"key";"blank blank key"]
       
        let countStrSeq = Seq.countBy funcStrCount_by strSeq
        let expectedStrSeq = seq [0,2;6,1;12,1]
        VerifySeqsEqual expectedStrSeq countStrSeq
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let countEmptySeq = Seq.countBy funcIntCount_by emptySeq
        let expectedEmptySeq =seq []
        
        VerifySeqsEqual expectedEmptySeq countEmptySeq  

        // null ISeq
        let nullSeq = null
       
        CheckThrowsArgumentNullException (fun () -> Seq.countBy funcIntCount_by nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.Distinct() =
        
        // integer ISeq
        let IntDistinctSeq =  
            seq { for i in 0..9 do                
                    yield i % 3 } |> ISeq.ofSeq
       
        let DistinctIntSeq = ISeq.distinct IntDistinctSeq
       
        let expectedIntSeq = seq [0;1;2]
        
        VerifySeqsEqual expectedIntSeq DistinctIntSeq
     
        // string ISeq
        let strDistinctSeq = seq ["elementDup"; "ele1"; "ele2"; "elementDup"] |> ISeq.ofSeq
       
        let DistnctStrSeq = ISeq.distinct strDistinctSeq
        let expectedStrSeq = seq ["elementDup"; "ele1"; "ele2"] |> ISeq.ofSeq
        VerifySeqsEqual expectedStrSeq DistnctStrSeq
        
        // Empty ISeq
        let emptySeq          = ISeq.empty
        let distinctEmptySeq  = ISeq.distinct emptySeq
        let expectedEmptySeq  = seq []  |> ISeq.ofSeq
       
        VerifySeqsEqual expectedEmptySeq distinctEmptySeq

        //// null ISeq
        //let nullSeq = null
       
        //CheckThrowsArgumentNullException(fun () -> ISeq.distinct nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.DistinctBy () =
        // integer ISeq
        let funcInt x = x % 3 
        let IntDistinct_bySeq =  
            seq { for i in 0..9 do                
                    yield i } |> ISeq.ofSeq
       
        let distinct_byIntSeq = ISeq.distinctBy funcInt IntDistinct_bySeq
        
        let expectedIntSeq = seq [0;1;2] |> ISeq.ofSeq
        
        VerifySeqsEqual expectedIntSeq distinct_byIntSeq
             
        // string ISeq
        let funcStrDistinct (s:string) = s.IndexOf("key")
        let strSeq = seq [ "key"; "blank key"; "key dup"; "blank key dup"] |> ISeq.ofSeq
       
        let DistnctStrSeq = ISeq.distinctBy funcStrDistinct strSeq
        let expectedStrSeq = seq ["key"; "blank key"] |> ISeq.ofSeq
        VerifySeqsEqual expectedStrSeq DistnctStrSeq
        
        // Empty ISeq
        let emptySeq            = ISeq.empty
        let distinct_byEmptySeq = ISeq.distinctBy funcInt emptySeq
        let expectedEmptySeq    = seq [] |> ISeq.ofSeq
       
        VerifySeqsEqual expectedEmptySeq distinct_byEmptySeq

        //// null ISeq
        //let nullSeq = null
       
        //CheckThrowsArgumentNullException(fun () -> ISeq.distinctBy funcInt nullSeq  |> ignore) 
        () 

    [<Test>]
    member this.Except() =
        // integer ISeq
        let intSeq1 = seq { yield! {1..100}
                            yield! {1..100} } |> ISeq.ofSeq
        let intSeq2 = {1..10} |> ISeq.ofSeq
        let expectedIntSeq = {11..100} |> ISeq.ofSeq

        VerifySeqsEqual expectedIntSeq <| ISeq.except intSeq2 intSeq1

        // string ISeq
        let strSeq1 = seq ["a"; "b"; "c"; "d"; "a"] |> ISeq.ofSeq
        let strSeq2 = seq ["b"; "c"] |> ISeq.ofSeq
        let expectedStrSeq = seq ["a"; "d"] |> ISeq.ofSeq

        VerifySeqsEqual expectedStrSeq <| ISeq.except strSeq2 strSeq1

        // double ISeq
        // Sequences with nan do not behave, due to the F# generic equality comparisons
//        let floatSeq1 = seq [1.0; 1.0; System.Double.MaxValue; nan; nan]
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
        //let nullSeq  = null
        //CheckThrowsArgumentNullException(fun () -> ISeq.except nullSeq emptyIntSeq |> ignore)
        //CheckThrowsArgumentNullException(fun () -> ISeq.except emptyIntSeq nullSeq |> ignore)
        //CheckThrowsArgumentNullException(fun () -> ISeq.except nullSeq nullSeq |> ignore)

        ()

    [<Test>]
    member this.Exists() =

        // Integer ISeq
        let funcInt x = (x % 2 = 0) 
        let IntexistsSeq =  
            seq { for i in 0..9 do                
                    yield i} |> ISeq.ofSeq
       
        let ifExistInt = ISeq.exists funcInt IntexistsSeq
        
        Assert.IsTrue( ifExistInt) 
            
        // String ISeq
        let funcStr (s:string) = s.Contains("key")
        let strSeq = seq ["key"; "blank key"] |> ISeq.ofSeq
       
        let ifExistStr = ISeq.exists funcStr strSeq
        
        Assert.IsTrue( ifExistStr)
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let ifExistsEmpty = ISeq.exists funcInt emptySeq
        
        Assert.IsFalse( ifExistsEmpty)
       
        

        //// null ISeq
        //let nullSeq = null
           
        //CheckThrowsArgumentNullException (fun () -> ISeq.exists funcInt nullSeq |> ignore) 
        () 
    
    [<Test>]
    member this.Exists2() =
        // Integer ISeq
        let funcInt x y = (x+y)%3=0 
        let Intexists2Seq1 =  seq [1;3;7] |> ISeq.ofSeq
        let Intexists2Seq2 = seq [1;6;3] |> ISeq.ofSeq
            
        let ifExist2Int = ISeq.exists2 funcInt Intexists2Seq1 Intexists2Seq2
        Assert.IsTrue( ifExist2Int)
             
        // String ISeq
        let funcStr s1 s2 = ((s1 + s2) = "CombinedString")
        let strSeq1 = seq [ "Combined"; "Not Combined"] |> ISeq.ofSeq
        let strSeq2 = seq ["String";    "Other String"] |> ISeq.ofSeq
        let ifexists2Str = ISeq.exists2 funcStr strSeq1 strSeq2
        Assert.IsTrue(ifexists2Str)
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let ifexists2Empty = ISeq.exists2 funcInt emptySeq emptySeq
        Assert.IsFalse( ifexists2Empty)
       
        //// null ISeq
        //let nullSeq = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.exists2 funcInt nullSeq nullSeq |> ignore) 
        () 
    
    
    [<Test>]
    member this.Filter() =
        // integer ISeq
        let funcInt x = if (x % 5 = 0) then true else false
        let IntSeq =
            seq { for i in 1..20 do
                    yield i } |> ISeq.ofSeq
                    
        let filterIntSeq = ISeq.filter funcInt IntSeq
          
        let expectedfilterInt = seq [ 5;10;15;20]
        
        VerifySeqsEqual expectedfilterInt filterIntSeq
        
        // string ISeq
        let funcStr (s:string) = s.Contains("Expected Content")
        let strSeq = seq [ "Expected Content"; "Not Expected"; "Expected Content"; "Not Expected"] |> ISeq.ofSeq
        
        let filterStrSeq = ISeq.filter funcStr strSeq
        
        let expectedfilterStr = seq ["Expected Content"; "Expected Content"] |> ISeq.ofSeq
        
        VerifySeqsEqual expectedfilterStr filterStrSeq    
        // Empty ISeq
        let emptySeq = ISeq.empty
        let filterEmptySeq = ISeq.filter funcInt emptySeq
        
        let expectedEmptySeq =seq []
       
        VerifySeqsEqual expectedEmptySeq filterEmptySeq
       
        

        //// null ISeq
        //let nullSeq = null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.filter funcInt nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.Find() =
        
        // integer ISeq
        let funcInt x = if (x % 5 = 0) then true else false
        let IntSeq =
            seq { for i in 1..20 do
                    yield i } |> ISeq.ofSeq
                    
        let findInt = Seq.find funcInt IntSeq
        Assert.AreEqual(findInt, 5)  
             
        // string ISeq
        let funcStr (s:string) = s.Contains("Expected Content")
        let strSeq = seq [ "Expected Content";"Not Expected"] |> ISeq.ofSeq
        
        let findStr = Seq.find funcStr strSeq
        Assert.AreEqual(findStr, "Expected Content")
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        
        CheckThrowsKeyNotFoundException(fun () -> Seq.find funcInt emptySeq |> ignore)
       
        //// null ISeq
        //let nullSeq = null
        //CheckThrowsArgumentNullException (fun () -> Seq.find funcInt nullSeq |> ignore) 
        ()
    
    [<Test>]
    member this.FindBack() =
        // integer ISeq
        let funcInt x = x % 5 = 0
        Assert.AreEqual(20, Seq.findBack funcInt <| seq { 1..20 })
        Assert.AreEqual(15, Seq.findBack funcInt <| seq { 1..19 })
        Assert.AreEqual(5, Seq.findBack funcInt <| seq { 5..9 })

        // string ISeq
        let funcStr (s:string) = s.Contains("Expected")
        let strSeq = seq [ "Not Expected"; "Expected Content"] |> ISeq.ofSeq
        let findStr = Seq.findBack funcStr strSeq
        Assert.AreEqual("Expected Content", findStr)

        // Empty ISeq
        let emptySeq = ISeq.empty
        CheckThrowsKeyNotFoundException(fun () -> Seq.findBack funcInt emptySeq |> ignore)

        // Not found
        let emptySeq = ISeq.empty
        CheckThrowsKeyNotFoundException(fun () -> seq { 1..20 } |> Seq.findBack (fun _ -> false) |> ignore)

        //// null ISeq
        //let nullSeq = null
        //CheckThrowsArgumentNullException (fun () -> Seq.findBack funcInt nullSeq |> ignore)
        ()

    [<Test>]
    member this.FindIndex() =
        
        // integer ISeq
        let digits = [1 .. 100] |> ISeq.ofList
        let idx = digits |> Seq.findIndex (fun i -> i.ToString().Length > 1)
        Assert.AreEqual(idx, 9)

        // empty ISeq 
        CheckThrowsKeyNotFoundException(fun () -> Seq.findIndex (fun i -> true) ISeq.empty |> ignore)
         
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> Seq.findIndex (fun i -> true) null |> ignore)
        ()
    
    [<Test>]
    member this.Permute() =
        let mapIndex i = (i + 1) % 4

        // integer seq
        let intSeq = seq { 1..4 }|> ISeq.ofSeq
        let resultInt = ISeq.permute mapIndex intSeq
        VerifySeqsEqual (seq [4;1;2;3]) resultInt

        // string seq
        let resultStr = ISeq.permute mapIndex ([|"Lists"; "are";  "commonly"; "list" |] |> ISeq.ofSeq)
        VerifySeqsEqual (seq ["list"; "Lists"; "are";  "commonly" ]) resultStr

        // empty seq
        let resultEpt = ISeq.permute mapIndex ([||] |> ISeq.ofSeq)
        VerifySeqsEqual ISeq.empty resultEpt

        //// null seq
        //let nullSeq = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.permute mapIndex nullSeq |> ignore)

        // argument exceptions
        CheckThrowsArgumentException (fun () -> ISeq.permute (fun _ -> 10) ([0..9]|> ISeq.ofSeq) |> ISeq.iter ignore)
        CheckThrowsArgumentException (fun () -> ISeq.permute (fun _ -> 0) ([0..9]|> ISeq.ofSeq) |> ISeq.iter ignore)
        ()

    [<Test>]
    member this.FindIndexBack() =
        // integer ISeq
        let digits = seq { 1..100 }|> ISeq.ofSeq
        let idx = digits |> Seq.findIndexBack (fun i -> i.ToString().Length = 1)
        Assert.AreEqual(idx, 8)

        // string ISeq
        let funcStr (s:string) = s.Contains("Expected")
        let strSeq = seq [ "Not Expected"; "Expected Content" ] |> ISeq.ofSeq
        let findStr = Seq.findIndexBack funcStr strSeq
        Assert.AreEqual(1, findStr)

        // empty ISeq
        CheckThrowsKeyNotFoundException(fun () -> Seq.findIndexBack (fun i -> true) ISeq.empty |> ignore)

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> Seq.findIndexBack (fun i -> true) null |> ignore)
        ()

    [<Test>]
    member this.Pick() =
    
        let digits = [| 1 .. 10 |] |> ISeq.ofArray
        let result = Seq.pick (fun i -> if i > 5 then Some(i.ToString()) else None) digits
        Assert.AreEqual(result, "6")
        
        // Empty seq (Bugged, 4173)
        CheckThrowsKeyNotFoundException (fun () -> Seq.pick (fun i -> Some('a')) ([| |] : int[]) |> ignore)

        // Null
        CheckThrowsArgumentNullException (fun () -> Seq.pick (fun i -> Some(i + 0)) null |> ignore)
        ()
        
    [<Test>]
    member this.Fold() =
        let funcInt x y = x+y
             
        let IntSeq =
            seq { for i in 1..10 do
                    yield i} |> ISeq.ofSeq
                    
        let foldInt = ISeq.fold funcInt 1 IntSeq
        if foldInt <> 56 then Assert.Fail()
        
        // string ISeq
        let funcStr (x:string) (y:string) = x+y
        let strSeq = seq ["B"; "C";  "D" ; "E"] |> ISeq.ofSeq
        let foldStr = ISeq.fold  funcStr "A" strSeq
      
        if foldStr <> "ABCDE" then Assert.Fail()
        
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let foldEmpty = ISeq.fold funcInt 1 emptySeq
        if foldEmpty <> 1 then Assert.Fail()

        //// null ISeq
        //let nullSeq = null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.fold funcInt 1 nullSeq |> ignore) 
        () 



    [<Test>]
    member this.Fold2() =
        Assert.AreEqual([(3,5); (2,3); (1,1)],ISeq.fold2 (fun acc x y -> (x,y)::acc) [] (seq [ 1..3 ] |> ISeq.ofSeq)  (seq [1..2..6] |> ISeq.ofSeq))

        // integer List  
        let funcInt x y z = x + y + z
        let resultInt = ISeq.fold2 funcInt 9 (seq [ 1..10 ] |> ISeq.ofSeq) (seq [1..2..20] |> ISeq.ofSeq)
        Assert.AreEqual(164, resultInt)
        
        // string List        
        let funcStr x y z = x + y + z        
        let resultStr = ISeq.fold2 funcStr "*" (["a"; "b";  "c" ; "d" ] |> ISeq.ofSeq) (["A"; "B";  "C" ; "D" ] |> ISeq.ofSeq)
        Assert.AreEqual("*aAbBcCdD", resultStr)
        
        // empty List
        let emptyArr = [ ] |> ISeq.ofSeq
        let resultEpt = ISeq.fold2 funcInt 5 emptyArr emptyArr        
        Assert.AreEqual(5, resultEpt)

        Assert.AreEqual(0,ISeq.fold2 funcInt 0 ISeq.empty (seq [1] |> ISeq.ofSeq))
        Assert.AreEqual(-1,ISeq.fold2 funcInt -1 (seq [1] |> ISeq.ofSeq) ISeq.empty)
            
        Assert.AreEqual(2,ISeq.fold2 funcInt 0 (seq [1;2] |> ISeq.ofSeq) (seq [1] |> ISeq.ofSeq))
        Assert.AreEqual(4,ISeq.fold2 funcInt 0 (seq [1] |> ISeq.ofSeq) (seq [3;6] |> ISeq.ofSeq))

        //// null ISeq
        //let nullSeq = null     
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.fold2 funcInt 0 nullSeq (seq [1] |> ISeq.ofSeq)  |> ignore) 
        //CheckThrowsArgumentNullException (fun () -> ISeq.fold2 funcInt 0 (seq [1] |> ISeq.ofSeq) nullSeq |> ignore) 
        ()
        
    [<Test>]
    member this.FoldBack() =
        // int ISeq
        let funcInt x y = x-y
        let IntSeq = seq { 1..4 } |> ISeq.ofSeq
        let foldInt = ISeq.foldBack funcInt IntSeq 6
        Assert.AreEqual((1-(2-(3-(4-6)))), foldInt)

        // string ISeq
        let funcStr (x:string) (y:string) = y.Remove(0,x.Length)
        let strSeq = seq [ "A"; "B"; "C"; "D" ] |> ISeq.ofSeq
        let foldStr = ISeq.foldBack  funcStr strSeq "ABCDE"
        Assert.AreEqual("E", foldStr)
        
        // single element
        let funcStr2 elem acc = sprintf "%s%s" elem acc
        let strSeq2 = seq [ "A" ] |> ISeq.ofSeq
        let foldStr2 = ISeq.foldBack funcStr2 strSeq2 "X"
        Assert.AreEqual("AX", foldStr2)

        // Empty ISeq
        let emptySeq = ISeq.empty
        let foldEmpty = ISeq.foldBack funcInt emptySeq 1
        Assert.AreEqual(1, foldEmpty)

        //// null ISeq
        //let nullSeq = null
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
        let intSeq = seq { 1..10 } |> ISeq.ofSeq
        let resultInt = ISeq.foldBack2 funcInt intSeq (seq { 1..2..20 } |> ISeq.ofSeq) 9
        Assert.AreEqual(164, resultInt)

        // string ISeq
        let funcStr = sprintf "%s%s%s"
        let strSeq = seq [ "A"; "B"; "C"; "D" ] |> ISeq.ofSeq
        let resultStr = ISeq.foldBack2  funcStr strSeq (seq [ "a"; "b"; "c"; "d"] |> ISeq.ofSeq) "*"
        Assert.AreEqual("AaBbCcDd*", resultStr)

        // single element
        let strSeqSingle = seq [ "X" ] |> ISeq.ofSeq
        Assert.AreEqual("XAZ", ISeq.foldBack2 funcStr strSeqSingle strSeq "Z")
        Assert.AreEqual("AXZ", ISeq.foldBack2 funcStr strSeq strSeqSingle "Z")
        Assert.AreEqual("XYZ", ISeq.foldBack2 funcStr strSeqSingle (seq [ "Y" ] |> ISeq.ofSeq) "Z")

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
        //let nullSeq = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.foldBack2 funcInt nullSeq intSeq 1 |> ignore)
        //CheckThrowsArgumentNullException (fun () -> ISeq.foldBack2 funcInt intSeq nullSeq 1 |> ignore)
        //CheckThrowsArgumentNullException (fun () -> ISeq.foldBack2 funcInt nullSeq nullSeq 1 |> ignore)

        ()

    [<Test>]
    member this.ForAll() =

        let funcInt x  = if x%2 = 0 then true else false
        let IntSeq =
            seq { for i in 1..10 do
                    yield i*2} |> ISeq.ofSeq
        let for_allInt = ISeq.forall funcInt  IntSeq
           
        if for_allInt <> true then Assert.Fail()
        
             
        // string ISeq
        let funcStr (x:string)  = x.Contains("a")
        let strSeq = seq ["a"; "ab";  "abc" ; "abcd"] |> ISeq.ofSeq
        let for_allStr = ISeq.forall  funcStr strSeq
       
        if for_allStr <> true then Assert.Fail()
        
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let for_allEmpty = ISeq.forall funcInt emptySeq
        
        if for_allEmpty <> true then Assert.Fail()
        
        //// null ISeq
        //let nullSeq = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.forall funcInt  nullSeq |> ignore) 
        () 
        
    [<Test>]
    member this.ForAll2() =

        let funcInt x y = if (x+y)%2 = 0 then true else false
        let IntSeq =
            seq { for i in 1..10 do
                    yield i} |> ISeq.ofSeq
                    
        let for_all2Int = ISeq.forall2 funcInt  IntSeq IntSeq
           
        if for_all2Int <> true then Assert.Fail()
        
        // string ISeq
        let funcStr (x:string) (y:string)  = (x+y).Length = 5
        let strSeq1 = seq ["a"; "ab";  "abc" ; "abcd"] |> ISeq.ofSeq
        let strSeq2 = seq ["abcd"; "abc";  "ab" ; "a"] |> ISeq.ofSeq
        let for_all2Str = ISeq.forall2  funcStr strSeq1 strSeq2
       
        if for_all2Str <> true then Assert.Fail()
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let for_all2Empty = ISeq.forall2 funcInt emptySeq emptySeq
        
        if for_all2Empty <> true then Assert.Fail()

        //// null ISeq
        //let nullSeq = null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.forall2 funcInt  nullSeq nullSeq |> ignore) 
        
    [<Test>]
    member this.GroupBy() =
        
        let funcInt x = x%5
             
        let IntSeq =
            seq { for i in 0 .. 9 do
                    yield i } |> ISeq.ofSeq
                    
        let group_byInt = ISeq.groupByVal funcInt IntSeq |> ISeq.map (fun (i, v) -> i, Seq.toList v)
        
        let expectedIntSeq = 
            seq { for i in 0..4 do
                     yield i, [i; i+5] } |> ISeq.ofSeq
                   
        VerifySeqsEqual group_byInt expectedIntSeq
             
        // string ISeq
        let funcStr (x:string) = x.Length
        let strSeq = seq ["length7"; "length 8";  "length7" ; "length  9"] |> ISeq.ofSeq
        
        let group_byStr = ISeq.groupByVal  funcStr strSeq |> ISeq.map (fun (i, v) -> i, Seq.toList v)
        let expectedStrSeq = 
            seq {
                yield 7, ["length7"; "length7"]
                yield 8, ["length 8"]
                yield 9, ["length  9"] } |> ISeq.ofSeq
       
        VerifySeqsEqual expectedStrSeq group_byStr
        
        // Empty ISeq
        let emptySeq = ISeq.empty
        let group_byEmpty = ISeq.groupByVal funcInt emptySeq
        let expectedEmptySeq = seq []

        VerifySeqsEqual expectedEmptySeq group_byEmpty
        
        //// null ISeq
        //let nullSeq = null
        //let group_byNull = ISeq.groupByVal funcInt nullSeq
        //CheckThrowsArgumentNullException (fun () -> ISeq.iter (fun _ -> ()) group_byNull) 
        () 
    
    [<Test>]
    member this.DisposalOfUnstartedEnumerator() =
        let run = ref false
        let f() = seq {                
                try
                    ()
                finally 
                    run := true
              }
  
        f().GetEnumerator().Dispose() 
        Assert.IsFalse(!run)

    [<Test>]
    member this.WeirdLocalNames() =
       
        let f pc = seq {                
                yield pc
                yield (pc+1)
                yield (pc+2)
              }
  
        let l = f 3 |> Seq.toList
        Assert.AreEqual([3;4;5], l)

        let f i = seq {                
                let pc = i*2
                yield pc
                yield (pc+1)
                yield (pc+2)
              }
        let l = f 3 |> Seq.toList
        Assert.AreEqual([6;7;8], l)

    [<Test>]
    member this.Contains() =

        // Integer ISeq
        let intSeq = seq { 0..9 } |> ISeq.ofSeq

        let ifContainsInt = ISeq.contains 5 intSeq

        Assert.IsTrue(ifContainsInt)

        // String ISeq
        let strSeq = seq ["key"; "blank key"] |> ISeq.ofSeq

        let ifContainsStr = ISeq.contains "key" strSeq

        Assert.IsTrue(ifContainsStr)

        // Empty ISeq
        let emptySeq = ISeq.empty
        let ifContainsEmpty = ISeq.contains 5 emptySeq

        Assert.IsFalse(ifContainsEmpty)

        //// null ISeq
        //let nullSeq = null

        //CheckThrowsArgumentNullException (fun () -> ISeq.contains 5 nullSeq |> ignore)
