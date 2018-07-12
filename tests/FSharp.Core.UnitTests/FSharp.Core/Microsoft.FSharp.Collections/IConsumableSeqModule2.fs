// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections

open System
open NUnit.Framework

open FSharp.Core.UnitTests.LibraryTestFx

#if IConsumableSeqIsPublic

//type iseq<'a> = IConsumableSeq.Core.IConsumableSeq<'a>

type IConsumableSeqWindowedTestInput<'t> =
    {
        InputSeq : iseq<'t>
        WindowSize : int
        ExpectedSeq : iseq<'t[]>
        Exception : Type option
    }

[<TestFixture>]
type IConsumableSeqModule2() =
    let iseq (x:seq<_>) = x |> IConsumableSeq.ofSeq

    [<Test>]
    member this.Hd() =
             
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 do
                            yield i }
                    
        if IConsumableSeq.head IntSeq <> 0 then Assert.Fail()
                 
        // string IConsumableSeq
        let strSeq = iseq ["first"; "second";  "third"]
        if IConsumableSeq.head strSeq <> "first" then Assert.Fail()
         
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        CheckThrowsArgumentException ( fun() -> IConsumableSeq.head emptySeq)
      
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->IConsumableSeq.head nullSeq) 
        () 

    [<Test>]
    member this.TryHead() =
        // int IConsumableSeq     
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 -> i }
                    
        let intResult = IConsumableSeq.tryHead IntSeq

        // string IConsumableSeq
        let strResult = IConsumableSeq.tryHead (iseq ["first"; "second";  "third"])
        Assert.AreEqual("first", strResult.Value)
         
        // Empty IConsumableSeq
        let emptyResult = IConsumableSeq.tryHead IConsumableSeq.empty
        Assert.AreEqual(None, emptyResult)
      
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->IConsumableSeq.head nullSeq) 
        () 
        
    [<Test>]
    member this.Tl() =
        // integer iseq  
        let resultInt = IConsumableSeq.tail <| (iseq <| seq { 1..10 } )
        Assert.AreEqual(Array.ofSeq (iseq <| seq { 2..10 }), Array.ofSeq resultInt)
        
        // string iseq
        let resultStr = IConsumableSeq.tail <| (iseq <| seq { yield "a"; yield "b"; yield "c"; yield "d" })
        Assert.AreEqual(Array.ofSeq (iseq <| seq { yield "b";  yield "c" ; yield "d" }), Array.ofSeq resultStr)
        
        // 1-element iseq
        let resultStr2 = IConsumableSeq.tail <| (iseq <| seq { yield "a" })
        Assert.AreEqual(Array.ofSeq (IConsumableSeq.empty : iseq<string>), Array.ofSeq resultStr2)

        //CheckThrowsArgumentNullException(fun () -> IConsumableSeq.tail null |> ignore)
        CheckThrowsArgumentException(fun () -> IConsumableSeq.tail IConsumableSeq.empty |> IConsumableSeq.iter (fun _ -> failwith "Should not be reached"))
        ()

    [<Test>]
    member this.Last() =
             
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 do
                            yield i }
                    
        if IConsumableSeq.last IntSeq <> 9 then Assert.Fail()
                 
        // string IConsumableSeq
        let strSeq = iseq ["first"; "second";  "third"]
        if IConsumableSeq.last strSeq <> "third" then Assert.Fail()
         
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        CheckThrowsArgumentException ( fun() -> IConsumableSeq.last emptySeq)
      
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->IConsumableSeq.last nullSeq) 
        () 

    [<Test>]
    member this.TryLast() =
             
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 -> i }
                    
        let intResult = IConsumableSeq.tryLast IntSeq
        Assert.AreEqual(9, intResult.Value)
                 
        // string IConsumableSeq
        let strResult = IConsumableSeq.tryLast (iseq ["first"; "second";  "third"])
        Assert.AreEqual("third", strResult.Value)
         
        // Empty IConsumableSeq
        let emptyResult = IConsumableSeq.tryLast IConsumableSeq.empty
        Assert.IsTrue(emptyResult.IsNone)
      
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->IConsumableSeq.tryLast nullSeq |> ignore) 
        () 
        
    [<Test>]
    member this.ExactlyOne() =
             
        let IntSeq =
            iseq <| seq { for i in 7 .. 7 do
                            yield i }
                    
        if IConsumableSeq.exactlyOne IntSeq <> 7 then Assert.Fail()
                 
        // string IConsumableSeq
        let strSeq = iseq ["second"]
        if IConsumableSeq.exactlyOne strSeq <> "second" then Assert.Fail()
         
        // Empty IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        CheckThrowsArgumentException ( fun() -> IConsumableSeq.exactlyOne emptySeq)
      
        // non-singleton IConsumableSeq
        let emptySeq = IConsumableSeq.empty
        CheckThrowsArgumentException ( fun() -> IConsumableSeq.exactlyOne (iseq [ 0 .. 1 ]) |> ignore )
      
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->IConsumableSeq.exactlyOne nullSeq) 
        () 
        
                
    [<Test>]
    member this.Init() =

        let funcInt x = x
        let init_finiteInt = IConsumableSeq.init 9 funcInt
        let expectedIntSeq = iseq [ 0..8]
      
        VerifySeqsEqual expectedIntSeq  init_finiteInt
        
             
        // string IConsumableSeq
        let funcStr x = x.ToString()
        let init_finiteStr = IConsumableSeq.init 5  funcStr
        let expectedStrSeq = iseq ["0";"1";"2";"3";"4"]

        VerifySeqsEqual expectedStrSeq init_finiteStr
        
        // null IConsumableSeq
        let funcNull x = null
        let init_finiteNull = IConsumableSeq.init 3 funcNull
        let expectedNullSeq = iseq [ null;null;null]
        
        VerifySeqsEqual expectedNullSeq init_finiteNull
        () 
        
    [<Test>]
    member this.InitInfinite() =

        let funcInt x = x
        let init_infiniteInt = IConsumableSeq.initInfinite funcInt
        let resultint = IConsumableSeq.find (fun x -> x =100) init_infiniteInt
        
        Assert.AreEqual(100,resultint)
        
             
        // string IConsumableSeq
        let funcStr x = x.ToString()
        let init_infiniteStr = IConsumableSeq.initInfinite  funcStr
        let resultstr = IConsumableSeq.find (fun x -> x = "100") init_infiniteStr
        
        Assert.AreEqual("100",resultstr)
       
       
    [<Test>]
    member this.IsEmpty() =
        
        //iseq int
        let seqint = iseq [1;2;3]
        let is_emptyInt = IConsumableSeq.isEmpty seqint
        
        Assert.IsFalse(is_emptyInt)
              
        //iseq str
        let seqStr = iseq["first";"second"]
        let is_emptyStr = IConsumableSeq.isEmpty  seqStr

        Assert.IsFalse(is_emptyInt)
        
        //iseq empty
        let seqEmpty = IConsumableSeq.empty
        let is_emptyEmpty = IConsumableSeq.isEmpty  seqEmpty
        Assert.IsTrue(is_emptyEmpty) 
        
        ////iseq null
        //let seqnull:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.isEmpty seqnull |> ignore)
        ()
        
    [<Test>]
    member this.Iter() =
        //iseq int
        let seqint =  iseq [ 1..3]
        let cacheint = ref 0
       
        let funcint x = cacheint := !cacheint + x
        IConsumableSeq.iter funcint seqint
        Assert.AreEqual(6,!cacheint)
              
        //iseq str
        let seqStr = iseq ["first";"second"]
        let cachestr =ref ""
        let funcstr x = cachestr := !cachestr+x
        IConsumableSeq.iter funcstr seqStr
         
        Assert.AreEqual("firstsecond",!cachestr)
        
         // empty array    
        let emptyseq = IConsumableSeq.empty
        let resultEpt = ref 0
        IConsumableSeq.iter (fun x -> Assert.Fail()) emptyseq   

        //// null seqay
        //let nullseq:iseq<'a> =  null
        
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.iter funcint nullseq |> ignore)  
        ()
        
    [<Test>]
    member this.Iter2() =
    
        //iseq int
        let seqint =  iseq [ 1..3]
        let cacheint = ref 0
       
        let funcint x y = cacheint := !cacheint + x+y
        IConsumableSeq.iter2 funcint seqint seqint
        Assert.AreEqual(12,!cacheint)
              
        //iseq str
        let seqStr = iseq ["first";"second"]
        let cachestr =ref ""
        let funcstr x y = cachestr := !cachestr+x+y
        IConsumableSeq.iter2 funcstr seqStr seqStr
         
        Assert.AreEqual("firstfirstsecondsecond",!cachestr)
        
         // empty array    
        let emptyseq = IConsumableSeq.empty
        let resultEpt = ref 0
        IConsumableSeq.iter2 (fun x y-> Assert.Fail()) emptyseq  emptyseq 

        //// null seqay
        //let nullseq:iseq<'a> =  null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.iter2 funcint nullseq nullseq |> ignore)  
        
        ()
        
    [<Test>]
    member this.Iteri() =
    
        // iseq int
        let seqint =  iseq [ 1..10]
        let cacheint = ref 0
       
        let funcint x y = cacheint := !cacheint + x+y
        IConsumableSeq.iteri funcint seqint
        Assert.AreEqual(100,!cacheint)
              
        // iseq str
        let seqStr = iseq ["first";"second"]
        let cachestr =ref 0
        let funcstr (x:int) (y:string) = cachestr := !cachestr+ x + y.Length
        IConsumableSeq.iteri funcstr seqStr
         
        Assert.AreEqual(12,!cachestr)
        
         // empty array    
        let emptyseq = IConsumableSeq.empty
        let resultEpt = ref 0
        IConsumableSeq.iteri funcint emptyseq
        Assert.AreEqual(0,!resultEpt)

        //// null seqay
        //let nullseq:iseq<'a> =  null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.iteri funcint nullseq |> ignore)  
        ()

    [<Test>]
    member this.Iteri2() =

        //iseq int
        let seqint = iseq [ 1..3]
        let cacheint = ref 0
       
        let funcint x y z = cacheint := !cacheint + x + y + z
        IConsumableSeq.iteri2 funcint seqint seqint
        Assert.AreEqual(15,!cacheint)
              
        //iseq str
        let seqStr = iseq ["first";"second"]
        let cachestr = ref 0
        let funcstr (x:int) (y:string) (z:string) = cachestr := !cachestr + x + y.Length + z.Length
        IConsumableSeq.iteri2 funcstr seqStr seqStr
         
        Assert.AreEqual(23,!cachestr)
        
        // empty iseq
        let emptyseq = IConsumableSeq.empty
        let resultEpt = ref 0
        IConsumableSeq.iteri2 (fun x y z -> Assert.Fail()) emptyseq emptyseq 

        //// null iseq
        //let nullseq:iseq<'a> =  null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.iteri2 funcint nullseq nullseq |> ignore)  
        
        // len1 <> len2
        let shorterSeq = iseq <| seq { 1..3 }
        let longerSeq = iseq <| seq { 2..2..100 }

        let testSeqLengths seq1 seq2 =
            let cache = ref 0
            let f x y z = cache := !cache + x + y + z
            IConsumableSeq.iteri2 f seq1 seq2
            !cache

        Assert.AreEqual(21, testSeqLengths shorterSeq longerSeq)
        Assert.AreEqual(21, testSeqLengths longerSeq shorterSeq)

        ()
        
    [<Test>]
    member this.Length() =

         // integer iseq  
        let resultInt = IConsumableSeq.length (iseq {1..8})
        if resultInt <> 8 then Assert.Fail()
        
        // string IConsumableSeq    
        let resultStr = IConsumableSeq.length (iseq ["Lists"; "are";  "commonly" ; "list" ])
        if resultStr <> 4 then Assert.Fail()
        
        // empty IConsumableSeq     
        let resultEpt = IConsumableSeq.length IConsumableSeq.empty
        if resultEpt <> 0 then Assert.Fail()

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null     
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.length  nullSeq |> ignore)  
        
        ()
        
    [<Test>]
    member this.Map() =

         // integer IConsumableSeq
        let funcInt x = 
                match x with
                | _ when x % 2 = 0 -> 10*x            
                | _ -> x
       
        let resultInt = IConsumableSeq.map funcInt (iseq { 1..10 })
        let expectedint = iseq [1;20;3;40;5;60;7;80;9;100]
        
        VerifySeqsEqual expectedint resultInt
        
        // string IConsumableSeq
        let funcStr (x:string) = x.ToLower()
        let resultStr = IConsumableSeq.map funcStr (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = iseq ["lists"; "are";  "commonly" ; "list"]
        
        VerifySeqsEqual expectedSeq resultStr
        
        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.map funcInt IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.map funcStr nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.Map2() =
         // integer IConsumableSeq
        let funcInt x y = x+y
        let resultInt = IConsumableSeq.map2 funcInt (iseq { 1..10 }) (iseq {2..2..20})
        let expectedint = iseq [3;6;9;12;15;18;21;24;27;30]
        
        VerifySeqsEqual expectedint resultInt
        
        // string IConsumableSeq
        let funcStr (x:int) (y:string) = x+y.Length
        let resultStr = IConsumableSeq.map2 funcStr (iseq[3;6;9;11]) (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = iseq [8;9;17;15]
        
        VerifySeqsEqual expectedSeq resultStr
        
        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.map2 funcInt IConsumableSeq.empty IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null 
        //let validSeq = iseq [1]
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.map2 funcInt nullSeq validSeq |> ignore)
        
        ()

    [<Test>]
    member this.Map3() = 
        // Integer iseq
        let funcInt a b c = (a + b) * c
        let resultInt = IConsumableSeq.map3 funcInt (iseq { 1..8 }) (iseq { 2..9 }) (iseq { 3..10 })
        let expectedInt = iseq [9; 20; 35; 54; 77; 104; 135; 170]
        VerifySeqsEqual expectedInt resultInt

        // First iseq is shorter
        VerifySeqsEqual (iseq [9; 20]) (IConsumableSeq.map3 funcInt (iseq { 1..2 }) (iseq { 2..9 }) (iseq { 3..10 }))
        // Second iseq is shorter
        VerifySeqsEqual (iseq [9; 20; 35]) (IConsumableSeq.map3 funcInt (iseq { 1..8 }) (iseq { 2..4 }) (iseq { 3..10 }))
        // Third iseq is shorter
        VerifySeqsEqual (iseq [9; 20; 35; 54]) (IConsumableSeq.map3 funcInt (iseq { 1..8 }) (iseq { 2..6 }) (iseq { 3..6 }))

        // String iseq
        let funcStr a b c = a + b + c
        let resultStr = IConsumableSeq.map3 funcStr (iseq ["A";"B";"C";"D"]) (iseq ["a";"b";"c";"d"]) (iseq ["1";"2";"3";"4"])
        let expectedStr = iseq ["Aa1";"Bb2";"Cc3";"Dd4"]
        VerifySeqsEqual expectedStr resultStr

        // Empty iseq
        let resultEmpty = IConsumableSeq.map3 funcStr IConsumableSeq.empty IConsumableSeq.empty IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEmpty

        //// Null iseq
        //let nullSeq = null : iseq<_>
        //let nonNullSeq = iseq [1]
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.map3 funcInt nullSeq nonNullSeq nullSeq |> ignore)

        ()

    [<Test>]
    member this.MapFold() =
        // integer IConsumableSeq
        let funcInt acc x = if x % 2 = 0 then 10*x, acc + 1 else x, acc
        let resultInt,resultIntAcc = IConsumableSeq.mapFold funcInt 100 <| (iseq <| seq { 1..10 })
        VerifySeqsEqual (iseq [ 1;20;3;40;5;60;7;80;9;100 ]) resultInt
        Assert.AreEqual(105, resultIntAcc)

        // string IConsumableSeq
        let funcStr acc (x:string) = match x.Length with 0 -> "empty", acc | _ -> x.ToLower(), sprintf "%s%s" acc x
        let resultStr,resultStrAcc = IConsumableSeq.mapFold funcStr "" <| iseq [ "";"BB";"C";"" ]
        VerifySeqsEqual (iseq [ "empty";"bb";"c";"empty" ]) resultStr
        Assert.AreEqual("BBC", resultStrAcc)

        // empty IConsumableSeq
        let resultEpt,resultEptAcc = IConsumableSeq.mapFold funcInt 100 IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt
        Assert.AreEqual(100, resultEptAcc)

        //// null IConsumableSeq
        //let nullArr = null:iseq<string>
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.mapFold funcStr "" nullArr |> ignore)

        ()

    [<Test>]
    member this.MapFoldBack() =
        // integer IConsumableSeq
        let funcInt x acc = if acc < 105 then 10*x, acc + 2 else x, acc
        let resultInt,resultIntAcc = IConsumableSeq.mapFoldBack funcInt (iseq <| seq { 1..10 }) 100
        VerifySeqsEqual (iseq [ 1;2;3;4;5;6;7;80;90;100 ]) resultInt
        Assert.AreEqual(106, resultIntAcc)

        // string IConsumableSeq
        let funcStr (x:string) acc = match x.Length with 0 -> "empty", acc | _ -> x.ToLower(), sprintf "%s%s" acc x
        let resultStr,resultStrAcc = IConsumableSeq.mapFoldBack funcStr (iseq [ "";"BB";"C";"" ]) ""
        VerifySeqsEqual (iseq [ "empty";"bb";"c";"empty" ]) resultStr
        Assert.AreEqual("CBB", resultStrAcc)

        // empty IConsumableSeq
        let resultEpt,resultEptAcc = IConsumableSeq.mapFoldBack funcInt IConsumableSeq.empty 100
        VerifySeqsEqual IConsumableSeq.empty resultEpt
        Assert.AreEqual(100, resultEptAcc)

        //// null IConsumableSeq
        //let nullArr = null:iseq<string>
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.mapFoldBack funcStr nullArr "" |> ignore)

        ()

    member private this.MapWithSideEffectsTester (map : (int -> int) -> iseq<int> -> iseq<int>) expectExceptions =
        let i = ref 0
        let f x = i := !i + 1; x*x
        let e = ((iseq [1;2]) |> map f).GetEnumerator()
        
        if expectExceptions then
            CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
            Assert.AreEqual(0, !i)
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)

        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(2, !i)
        if expectExceptions then
            CheckThrowsInvalidOperationExn (fun _ -> e.Current |> ignore)
            Assert.AreEqual(2, !i)

        
        i := 0
        let e = ((iseq []) |> map f).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        
        
    member private this.MapWithExceptionTester (map : (int -> int) -> iseq<int> -> iseq<int>) =
        let raiser x = if x > 0 then raise(NotSupportedException()) else x
        let e = (map raiser (iseq [0; 1])).GetEnumerator()
        Assert.IsTrue(e.MoveNext()) // should not throw
        Assert.AreEqual(0, e.Current)
        CheckThrowsNotSupportedException(fun _ -> e.MoveNext() |> ignore)
        Assert.AreEqual(0, e.Current) // should not throw

    [<Test>]
    member this.MapWithSideEffects () =
        this.MapWithSideEffectsTester IConsumableSeq.map true
        
    [<Test>]
    member this.MapWithException () =
        this.MapWithExceptionTester IConsumableSeq.map

        
    [<Test>]
    member this.SingletonCollectWithSideEffects () =
        this.MapWithSideEffectsTester (fun f-> IConsumableSeq.collect (f >> IConsumableSeq.singleton)) true
        
    [<Test>]
    member this.SingletonCollectWithException () =
        this.MapWithExceptionTester (fun f-> IConsumableSeq.collect (f >> IConsumableSeq.singleton))

#if !FX_NO_LINQ
    [<Test>]
    member this.SystemLinqSelectWithSideEffects () =
        this.MapWithSideEffectsTester (fun f s -> iseq <| System.Linq.Enumerable.Select(s, Func<_,_>(f))) false
        
    [<Test>]
    member this.SystemLinqSelectWithException () =
        this.MapWithExceptionTester (fun f s -> iseq <| System.Linq.Enumerable.Select(s, Func<_,_>(f)))
#endif
        
    [<Test>]
    member this.MapiWithSideEffects () =
        let i = ref 0
        let f _ x = i := !i + 1; x*x
        let e = ((iseq [1;2]) |> IConsumableSeq.mapi f).GetEnumerator()
        
        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(0, !i)
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(2, !i)
        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(2, !i)
        
        i := 0
        let e = ((iseq []) |> IConsumableSeq.mapi f).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        
    [<Test>]
    member this.Map2WithSideEffects () =
        let i = ref 0
        let f x y = i := !i + 1; x*x
        let e = (IConsumableSeq.map2 f (iseq [1;2]) (iseq [1;2])).GetEnumerator()
        
        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(0, !i)
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)

        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(2,!i)
        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(2, !i)
        
        i := 0
        let e = (IConsumableSeq.map2 f (iseq []) (iseq [])).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        
    [<Test>]
    member this.Mapi2WithSideEffects () =
        let i = ref 0
        let f _ x y = i := !i + 1; x*x
        let e = (IConsumableSeq.mapi2 f (iseq [1;2]) (iseq [1;2])).GetEnumerator()

        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(0, !i)
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)

        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)

        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(2,!i)
        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(2, !i)

        i := 0
        let e = (IConsumableSeq.mapi2 f (iseq []) (iseq [])).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)

    [<Test>]
    member this.Collect() =
         // integer IConsumableSeq
        let funcInt x = iseq [x+1]
        let resultInt = IConsumableSeq.collect funcInt (iseq { 1..10 })
       
        let expectedint = iseq <| seq {2..11}
        
        VerifySeqsEqual expectedint resultInt

//#if !FX_NO_CHAR_PARSE
//        // string IConsumableSeq
//        let funcStr (y:string) = y+"ist"
       
//        let resultStr = IConsumableSeq.collect funcStr (iseq ["L"])
        
        
//        let expectedSeq = iseq ['L';'i';'s';'t']
        
//        VerifySeqsEqual expectedSeq resultStr
//#endif        
        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.collect funcInt IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null 
       
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.collect funcInt nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.Mapi() =

         // integer IConsumableSeq
        let funcInt x y = x+y
        let resultInt = IConsumableSeq.mapi funcInt (iseq { 10..2..20 } )
        let expectedint = iseq [10;13;16;19;22;25]
        
        VerifySeqsEqual expectedint resultInt
        
        // string IConsumableSeq
        let funcStr (x:int) (y:string) =x+y.Length
       
        let resultStr = IConsumableSeq.mapi funcStr (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedStr = iseq [5;4;10;7]
         
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.mapi funcInt IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null 
       
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.mapi funcInt nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.Mapi2() =
         // integer IConsumableSeq
        let funcInt x y z = x+y+z
        let resultInt = IConsumableSeq.mapi2 funcInt (iseq { 1..10 }) (iseq {2..2..20})
        let expectedint = iseq [3;7;11;15;19;23;27;31;35;39]

        VerifySeqsEqual expectedint resultInt

        // string IConsumableSeq
        let funcStr (x:int) (y:int) (z:string) = x+y+z.Length
        let resultStr = IConsumableSeq.mapi2 funcStr (iseq[3;6;9;11]) (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = iseq [8;10;19;18]

        VerifySeqsEqual expectedSeq resultStr

        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.mapi2 funcInt IConsumableSeq.empty IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //let validSeq = iseq [1]
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.mapi2 funcInt nullSeq validSeq |> ignore)

        // len1 <> len2
        let shorterSeq = iseq <| seq { 1..10 }
        let longerSeq = iseq <| seq { 2..20 }

        let testSeqLengths seq1 seq2 =
            let f x y z = x + y + z
            IConsumableSeq.mapi2 f seq1 seq2

        VerifySeqsEqual (iseq [3;6;9;12;15;18;21;24;27;30]) (testSeqLengths shorterSeq longerSeq)
        VerifySeqsEqual (iseq [3;6;9;12;15;18;21;24;27;30]) (testSeqLengths longerSeq shorterSeq)

    [<Test>]
    member this.Indexed() =

         // integer IConsumableSeq
        let resultInt = IConsumableSeq.indexed (iseq { 10..2..20 })
        let expectedint = iseq [(0,10);(1,12);(2,14);(3,16);(4,18);(5,20)]

        VerifySeqsEqual expectedint resultInt

        // string IConsumableSeq
        let resultStr = IConsumableSeq.indexed (iseq ["Lists"; "Are"; "Commonly"; "List" ])
        let expectedStr = iseq [(0,"Lists");(1,"Are");(2,"Commonly");(3,"List")]

        VerifySeqsEqual expectedStr resultStr

        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.indexed IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.indexed nullSeq |> ignore)

        ()

    [<Test>]
    member this.Max() =
         // integer IConsumableSeq
        let resultInt = IConsumableSeq.max (iseq { 10..20 } )
        
        Assert.AreEqual(20,resultInt)
        
        // string IConsumableSeq
       
        let resultStr = IConsumableSeq.max (iseq ["Lists"; "Are";  "MaxString" ; "List" ])
        Assert.AreEqual("MaxString",resultStr)
          
        // empty IConsumableSeq
        CheckThrowsArgumentException(fun () -> IConsumableSeq.max ( IConsumableSeq.empty : iseq<float>) |> ignore)
        
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.max nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.MaxBy() =
    
        // integer IConsumableSeq
        let funcInt x = x % 8
        let resultInt = IConsumableSeq.maxBy funcInt (iseq { 2..2..20 } )
        Assert.AreEqual(6,resultInt)
        
        // string IConsumableSeq
        let funcStr (x:string)  =x.Length 
        let resultStr = IConsumableSeq.maxBy funcStr (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        Assert.AreEqual("Commonly",resultStr)
          
        // empty IConsumableSeq
        CheckThrowsArgumentException (fun () -> IConsumableSeq.maxBy funcInt (IConsumableSeq.empty : iseq<int>) |> ignore)
        
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () ->IConsumableSeq.maxBy funcInt nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.MinBy() =
    
        // integer IConsumableSeq
        let funcInt x = x % 8
        let resultInt = IConsumableSeq.minBy funcInt (iseq { 2..2..20 } )
        Assert.AreEqual(8,resultInt)
        
        // string IConsumableSeq
        let funcStr (x:string)  =x.Length 
        let resultStr = IConsumableSeq.minBy funcStr (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        Assert.AreEqual("Are",resultStr)
          
        // empty IConsumableSeq
        CheckThrowsArgumentException (fun () -> IConsumableSeq.minBy funcInt (IConsumableSeq.empty : iseq<int>) |> ignore) 
        
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () ->IConsumableSeq.minBy funcInt nullSeq |> ignore)
        
        ()
        
          
    [<Test>]
    member this.Min() =

         // integer IConsumableSeq
        let resultInt = IConsumableSeq.min (iseq { 10..20 } )
        Assert.AreEqual(10,resultInt)
        
        // string IConsumableSeq
        let resultStr = IConsumableSeq.min (iseq ["Lists"; "Are";  "minString" ; "List" ])
        Assert.AreEqual("Are",resultStr)
          
        // empty IConsumableSeq
        CheckThrowsArgumentException (fun () -> IConsumableSeq.min (IConsumableSeq.empty : iseq<int>) |> ignore) 
        
        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.min nullSeq |> ignore)
        
        ()

    [<Test>]
    member this.Item() =
         // integer IConsumableSeq
        let resultInt = IConsumableSeq.item 3 (iseq { 10..20 })
        Assert.AreEqual(13, resultInt)

        // string IConsumableSeq
        let resultStr = IConsumableSeq.item 2 (iseq ["Lists"; "Are"; "Cool" ; "List" ])
        Assert.AreEqual("Cool", resultStr)

        // empty IConsumableSeq
        CheckThrowsArgumentException(fun () -> IConsumableSeq.item 0 (IConsumableSeq.empty : iseq<decimal>) |> ignore)

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->IConsumableSeq.item 3 nullSeq |> ignore)

        // Negative index
        for i = -1 downto -10 do
           CheckThrowsArgumentException (fun () -> IConsumableSeq.item i (iseq { 10 .. 20 }) |> ignore)

        // Out of range
        for i = 11 to 20 do
           CheckThrowsArgumentException (fun () -> IConsumableSeq.item i (iseq { 10 .. 20 }) |> ignore)

    [<Test>]
    member this.``item should fail with correct number of missing elements``() =
        try
            IConsumableSeq.item 0 (iseq (Array.zeroCreate<int> 0)) |> ignore
            failwith "error expected"
        with
        | exn when exn.Message.Contains("seq was short by 1 element") -> ()

        try
            IConsumableSeq.item 2 (iseq (Array.zeroCreate<int> 0)) |> ignore
            failwith "error expected"
        with
        | exn when exn.Message.Contains("seq was short by 3 elements") -> ()

    [<Test>]
    member this.Of_Array() =
         // integer IConsumableSeq
        let resultInt = IConsumableSeq.ofArray [|1..10|]
        let expectedInt = {1..10}
         
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let resultStr = IConsumableSeq.ofArray [|"Lists"; "Are";  "ofArrayString" ; "List" |]
        let expectedStr = iseq ["Lists"; "Are";  "ofArrayString" ; "List" ]
        VerifySeqsEqual expectedStr resultStr
          
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.ofArray [| |] 
        VerifySeqsEqual resultEpt IConsumableSeq.empty
       
        ()
        
    [<Test>]
    member this.Of_List() =
         // integer IConsumableSeq
        let resultInt = IConsumableSeq.ofList [1..10]
        let expectedInt = {1..10}
         
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
       
        let resultStr =IConsumableSeq.ofList ["Lists"; "Are";  "ofListString" ; "List" ]
        let expectedStr = iseq ["Lists"; "Are";  "ofListString" ; "List" ]
        VerifySeqsEqual expectedStr resultStr
          
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.ofList [] 
        VerifySeqsEqual resultEpt IConsumableSeq.empty
        ()
        
          
    [<Test>]
    member this.Pairwise() =
         // integer IConsumableSeq
        let resultInt = IConsumableSeq.pairwise (iseq {1..3})
       
        let expectedInt = iseq [1,2;2,3]
         
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let resultStr =IConsumableSeq.pairwise (iseq ["str1"; "str2";"str3" ])
        let expectedStr = iseq ["str1","str2";"str2","str3"]
        VerifySeqsEqual expectedStr resultStr
          
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.pairwise (iseq [] )
        VerifySeqsEqual resultEpt IConsumableSeq.empty
       
        ()
        
    [<Test>]
    member this.Reduce() =
         
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.reduce (fun x y -> x/y) (iseq [5*4*3*2; 4;3;2;1])
        Assert.AreEqual(5,resultInt)
        
        // string IConsumableSeq
        let resultStr = IConsumableSeq.reduce (fun (x:string) (y:string) -> x.Remove(0,y.Length)) (iseq ["ABCDE";"A"; "B";  "C" ; "D" ])
        Assert.AreEqual("E",resultStr) 
       
        // empty IConsumableSeq 
        CheckThrowsArgumentException (fun () -> IConsumableSeq.reduce (fun x y -> x/y)  IConsumableSeq.empty |> ignore)
        
        //// null IConsumableSeq
        //let nullSeq : iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.reduce (fun (x:string) (y:string) -> x.Remove(0,y.Length))  nullSeq  |> ignore)   
        ()

    [<Test>]
    member this.ReduceBack() =
        // int IConsumableSeq
        let funcInt x y = x - y
        let IntSeq = iseq <| seq { 1..4 }
        let reduceInt = IConsumableSeq.reduceBack funcInt IntSeq
        Assert.AreEqual((1-(2-(3-4))), reduceInt)

        // string IConsumableSeq
        let funcStr (x:string) (y:string) = y.Remove(0,x.Length)
        let strSeq = iseq [ "A"; "B"; "C"; "D" ; "ABCDE" ]
        let reduceStr = IConsumableSeq.reduceBack  funcStr strSeq
        Assert.AreEqual("E", reduceStr)
        
        // string IConsumableSeq
        let funcStr2 elem acc = sprintf "%s%s" elem acc
        let strSeq2 = iseq [ "A" ]
        let reduceStr2 = IConsumableSeq.reduceBack  funcStr2 strSeq2
        Assert.AreEqual("A", reduceStr2)

        // Empty IConsumableSeq
        CheckThrowsArgumentException (fun () -> IConsumableSeq.reduceBack funcInt IConsumableSeq.empty |> ignore)

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.reduceBack funcInt nullSeq |> ignore)

        ()

    [<Test>]
    member this.Rev() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.rev (iseq [5;4;3;2;1])
        VerifySeqsEqual (iseq[1;2;3;4;5]) resultInt

        // string IConsumableSeq
        let resultStr = IConsumableSeq.rev (iseq ["A"; "B";  "C" ; "D" ])
        VerifySeqsEqual (iseq["D";"C";"B";"A"]) resultStr

        // empty IConsumableSeq
        VerifySeqsEqual IConsumableSeq.empty (IConsumableSeq.rev IConsumableSeq.empty)

        //// null IConsumableSeq
        //let nullSeq : iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.rev nullSeq  |> ignore)
        ()

    [<Test>]
    member this.Scan() =
        // integer IConsumableSeq
        let funcInt x y = x+y
        let resultInt = IConsumableSeq.scan funcInt 9 (iseq {1..10})
        let expectedInt = iseq [9;10;12;15;19;24;30;37;45;54;64]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let funcStr x y = x+y
        let resultStr =IConsumableSeq.scan funcStr "x" (iseq ["str1"; "str2";"str3" ])
        
        let expectedStr = iseq ["x";"xstr1"; "xstr1str2";"xstr1str2str3"]
        VerifySeqsEqual expectedStr resultStr
          
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.scan funcInt 5 IConsumableSeq.empty 
       
        VerifySeqsEqual resultEpt (iseq [ 5])
       
        //// null IConsumableSeq
        //let seqNull:iseq<'a> = null
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.scan funcInt 5 seqNull |> ignore)
        ()
        
    [<Test>]
    member this.ScanBack() =
        // integer IConsumableSeq
        let funcInt x y = x+y
        let resultInt = IConsumableSeq.scanBack funcInt (iseq { 1..10 }) 9
        let expectedInt = iseq [64;63;61;58;54;49;43;36;28;19;9]
        VerifySeqsEqual expectedInt resultInt

        // string IConsumableSeq
        let funcStr x y = x+y
        let resultStr = IConsumableSeq.scanBack funcStr (iseq ["A";"B";"C";"D"]) "X"
        let expectedStr = iseq ["ABCDX";"BCDX";"CDX";"DX";"X"]
        VerifySeqsEqual expectedStr resultStr

        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.scanBack funcInt IConsumableSeq.empty 5
        let expectedEpt = iseq [5]
        VerifySeqsEqual expectedEpt resultEpt

        //// null IConsumableSeq
        //let seqNull:iseq<'a> = null
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.scanBack funcInt seqNull 5 |> ignore)

        // exception cases
        let funcEx x (s:'State) = raise <| new System.FormatException() : 'State
        // calling scanBack with funcEx does not throw
        let resultEx = IConsumableSeq.scanBack funcEx (iseq <| seq {1..10}) 0
        // reading from resultEx throws
        CheckThrowsFormatException(fun() -> IConsumableSeq.head resultEx |> ignore)

        // Result consumes entire input sequence as soon as it is accesses an element
        let i = ref 0
        let funcState x s = (i := !i + x); x+s
        let resultState = IConsumableSeq.scanBack funcState (iseq <| seq {1..3}) 0
        Assert.AreEqual(0, !i)
        use e = resultState.GetEnumerator()
        Assert.AreEqual(6, !i)

        ()

    [<Test>]
    member this.Singleton() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.singleton 1
       
        let expectedInt = iseq [1]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let resultStr =IConsumableSeq.singleton "str1"
        let expectedStr = iseq ["str1"]
        VerifySeqsEqual expectedStr resultStr
         
        // null IConsumableSeq
        let resultNull = IConsumableSeq.singleton null
        let expectedNull = iseq [null]
        VerifySeqsEqual expectedNull resultNull
        ()
    
        
    [<Test>]
    member this.Skip() =
    
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.skip 2 (iseq [1;2;3;4])
        let expectedInt = iseq [3;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let resultStr =IConsumableSeq.skip 2 (iseq ["str1";"str2";"str3";"str4"])
        let expectedStr = iseq ["str3";"str4"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.skip 0 IConsumableSeq.empty 
        VerifySeqsEqual resultEpt IConsumableSeq.empty
        
         
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.skip 1 null |> ignore)
        ()
       
    [<Test>]
    member this.Skip_While() =
    
        // integer IConsumableSeq
        let funcInt x = (x < 3)
        let resultInt = IConsumableSeq.skipWhile funcInt (iseq [1;2;3;4;5;6])
        let expectedInt = iseq [3;4;5;6]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let funcStr (x:string) = x.Contains(".")
        let resultStr =IConsumableSeq.skipWhile funcStr (iseq [".";"asdfasdf.asdfasdf";"";"";"";"";"";"";"";"";""])
        let expectedStr = iseq ["";"";"";"";"";"";"";"";""]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.skipWhile funcInt IConsumableSeq.empty 
        VerifySeqsEqual resultEpt IConsumableSeq.empty
        
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.skipWhile funcInt null |> ignore)
        ()
       
    [<Test>]
    member this.Sort() =

        // integer IConsumableSeq
        let resultInt = IConsumableSeq.sort (iseq [1;3;2;4;6;5;7])
        let expectedInt = {1..7}
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
       
        let resultStr =IConsumableSeq.sort (iseq ["str1";"str3";"str2";"str4"])
        let expectedStr = iseq ["str1";"str2";"str3";"str4"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.sort IConsumableSeq.empty 
        VerifySeqsEqual resultEpt IConsumableSeq.empty
         
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.sort null  |> ignore)
        ()
        
    [<Test>]
    member this.SortBy() =

        // integer IConsumableSeq
        let funcInt x = Math.Abs(x-5)
        let resultInt = IConsumableSeq.sortBy funcInt (iseq [1;2;4;5;7])
        let expectedInt = iseq [5;4;7;2;1]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let funcStr (x:string) = x.IndexOf("key")
        let resultStr =IConsumableSeq.sortBy funcStr (iseq ["st(key)r";"str(key)";"s(key)tr";"(key)str"])
        
        let expectedStr = iseq ["(key)str";"s(key)tr";"st(key)r";"str(key)"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.sortBy funcInt IConsumableSeq.empty 
        VerifySeqsEqual resultEpt IConsumableSeq.empty
         
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.sortBy funcInt null  |> ignore)
        ()

    [<Test>]
    member this.SortDescending() =

        // integer IConsumableSeq
        let resultInt = IConsumableSeq.sortDescending (iseq [1;3;2;Int32.MaxValue;4;6;Int32.MinValue;5;7;0])
        let expectedInt = iseq <| seq {
            yield Int32.MaxValue;
            yield! iseq{ 7..-1..0 }
            yield Int32.MinValue
        }
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
       
        let resultStr = IConsumableSeq.sortDescending (iseq ["str1";null;"str3";"";"Str1";"str2";"str4"])
        let expectedStr = iseq ["str4";"str3";"str2";"str1";"Str1";"";null]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.sortDescending IConsumableSeq.empty 
        VerifySeqsEqual resultEpt IConsumableSeq.empty

        // tuple IConsumableSeq
        let tupSeq = (iseq[(2,"a");(1,"d");(1,"b");(1,"a");(2,"x");(2,"b");(1,"x")])
        let resultTup = IConsumableSeq.sortDescending tupSeq
        let expectedTup = (iseq[(2,"x");(2,"b");(2,"a");(1,"x");(1,"d");(1,"b");(1,"a")])   
        VerifySeqsEqual  expectedTup resultTup
         
        // float IConsumableSeq
        let minFloat,maxFloat,epsilon = System.Double.MinValue,System.Double.MaxValue,System.Double.Epsilon
        let floatSeq = iseq [0.0; 0.5; 2.0; 1.5; 1.0; minFloat;maxFloat;epsilon;-epsilon]
        let resultFloat = IConsumableSeq.sortDescending floatSeq
        let expectedFloat = iseq [maxFloat; 2.0; 1.5; 1.0; 0.5; epsilon; 0.0; -epsilon; minFloat; ]
        VerifySeqsEqual expectedFloat resultFloat

        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.sort null  |> ignore)
        ()
        
    [<Test>]
    member this.SortByDescending() =

        // integer IConsumableSeq
        let funcInt x = Math.Abs(x-5)
        let resultInt = IConsumableSeq.sortByDescending funcInt (iseq [1;2;4;5;7])
        let expectedInt = iseq [1;2;7;4;5]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let funcStr (x:string) = x.IndexOf("key")
        let resultStr =IConsumableSeq.sortByDescending funcStr (iseq ["st(key)r";"str(key)";"s(key)tr";"(key)str"])
        
        let expectedStr = iseq ["str(key)";"st(key)r";"s(key)tr";"(key)str"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.sortByDescending funcInt IConsumableSeq.empty 
        VerifySeqsEqual resultEpt IConsumableSeq.empty

        // tuple IConsumableSeq
        let tupSeq = (iseq[(2,"a");(1,"d");(1,"b");(1,"a");(2,"x");(2,"b");(1,"x")])
        let resultTup = IConsumableSeq.sortByDescending snd tupSeq         
        let expectedTup = (iseq[(2,"x");(1,"x");(1,"d");(1,"b");(2,"b");(2,"a");(1,"a")])
        VerifySeqsEqual  expectedTup resultTup
         
        // float IConsumableSeq
        let minFloat,maxFloat,epsilon = System.Double.MinValue,System.Double.MaxValue,System.Double.Epsilon
        let floatSeq = iseq [0.0; 0.5; 2.0; 1.5; 1.0; minFloat;maxFloat;epsilon;-epsilon]
        let resultFloat = IConsumableSeq.sortByDescending id floatSeq
        let expectedFloat = iseq [maxFloat; 2.0; 1.5; 1.0; 0.5; epsilon; 0.0; -epsilon; minFloat; ]
        VerifySeqsEqual expectedFloat resultFloat

        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.sortByDescending funcInt null  |> ignore)
        ()
        
    member this.SortWith() =

        // integer IConsumableSeq
        let intComparer a b = compare (a%3) (b%3)
        let resultInt = IConsumableSeq.sortWith intComparer (iseq <| seq {0..10})
        let expectedInt = iseq [0;3;6;9;1;4;7;10;2;5;8]
        VerifySeqsEqual expectedInt resultInt

        // string IConsumableSeq
        let resultStr = IConsumableSeq.sortWith compare (iseq ["str1";"str3";"str2";"str4"])
        let expectedStr = iseq ["str1";"str2";"str3";"str4"]
        VerifySeqsEqual expectedStr resultStr

        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.sortWith intComparer IConsumableSeq.empty
        VerifySeqsEqual resultEpt IConsumableSeq.empty

        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.sortWith intComparer null  |> ignore)

        ()

    [<Test>]
    member this.Sum() =
    
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.sum (iseq [1..10])
        Assert.AreEqual(55,resultInt)
        
        // float32 IConsumableSeq
        let floatSeq = (iseq [ 1.2f;3.5f;6.7f ])
        let resultFloat = IConsumableSeq.sum floatSeq
        if resultFloat <> 11.4f then Assert.Fail()
        
        // double IConsumableSeq
        let doubleSeq = (iseq [ 1.0;8.0 ])
        let resultDouble = IConsumableSeq.sum doubleSeq
        if resultDouble <> 9.0 then Assert.Fail()
        
        // decimal IConsumableSeq
        let decimalSeq = (iseq [ 0M;19M;19.03M ])
        let resultDecimal = IConsumableSeq.sum decimalSeq
        if resultDecimal <> 38.03M then Assert.Fail()      
          
      
        // empty float32 IConsumableSeq
        let emptyFloatSeq = IConsumableSeq.empty<System.Single> 
        let resultEptFloat = IConsumableSeq.sum emptyFloatSeq 
        if resultEptFloat <> 0.0f then Assert.Fail()
        
        // empty double IConsumableSeq
        let emptyDoubleSeq = IConsumableSeq.empty<System.Double> 
        let resultDouEmp = IConsumableSeq.sum emptyDoubleSeq 
        if resultDouEmp <> 0.0 then Assert.Fail()
        
        // empty decimal IConsumableSeq
        let emptyDecimalSeq = IConsumableSeq.empty<System.Decimal> 
        let resultDecEmp = IConsumableSeq.sum emptyDecimalSeq 
        if resultDecEmp <> 0M then Assert.Fail()
       
        ()
        
    [<Test>]
    member this.SumBy() =

        // integer IConsumableSeq
        let resultInt = IConsumableSeq.sumBy int (iseq [1..10])
        Assert.AreEqual(55,resultInt)
        
        // float32 IConsumableSeq
        let floatSeq = (iseq [ 1.2f;3.5f;6.7f ])
        let resultFloat = IConsumableSeq.sumBy float32 floatSeq
        if resultFloat <> 11.4f then Assert.Fail()
        
        // double IConsumableSeq
        let doubleSeq = (iseq [ 1.0;8.0 ])
        let resultDouble = IConsumableSeq.sumBy double doubleSeq
        if resultDouble <> 9.0 then Assert.Fail()
        
        // decimal IConsumableSeq
        let decimalSeq = (iseq [ 0M;19M;19.03M ])
        let resultDecimal = IConsumableSeq.sumBy decimal decimalSeq
        if resultDecimal <> 38.03M then Assert.Fail()      

        // empty float32 IConsumableSeq
        let emptyFloatSeq = IConsumableSeq.empty<System.Single> 
        let resultEptFloat = IConsumableSeq.sumBy float32 emptyFloatSeq 
        if resultEptFloat <> 0.0f then Assert.Fail()
        
        // empty double IConsumableSeq
        let emptyDoubleSeq = IConsumableSeq.empty<System.Double> 
        let resultDouEmp = IConsumableSeq.sumBy double emptyDoubleSeq 
        if resultDouEmp <> 0.0 then Assert.Fail()
        
        // empty decimal IConsumableSeq
        let emptyDecimalSeq = IConsumableSeq.empty<System.Decimal> 
        let resultDecEmp = IConsumableSeq.sumBy decimal emptyDecimalSeq 
        if resultDecEmp <> 0M then Assert.Fail()
       
        ()
        
    [<Test>]
    member this.Take() =
        // integer IConsumableSeq
        
        let resultInt = IConsumableSeq.take 3 (iseq [1;2;4;5;7])
       
        let expectedInt = iseq [1;2;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
       
        let resultStr =IConsumableSeq.take 2(iseq ["str1";"str2";"str3";"str4"])
     
        let expectedStr = iseq ["str1";"str2"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.take 0 IConsumableSeq.empty 
      
        VerifySeqsEqual resultEpt IConsumableSeq.empty
        
         
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.take 1 null |> ignore)
        ()
        
    [<Test>]
    member this.takeWhile() =
        // integer IConsumableSeq
        let funcInt x = (x < 6)
        let resultInt = IConsumableSeq.takeWhile funcInt (iseq [1;2;4;5;6;7])
      
        let expectedInt = iseq [1;2;4;5]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let funcStr (x:string) = (x.Length < 4)
        let resultStr =IConsumableSeq.takeWhile funcStr (iseq ["a"; "ab"; "abc"; "abcd"; "abcde"])
      
        let expectedStr = iseq ["a"; "ab"; "abc"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.takeWhile funcInt IConsumableSeq.empty 
        VerifySeqsEqual resultEpt IConsumableSeq.empty
        
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.takeWhile funcInt null |> ignore)
        ()
        
    [<Test>]
    member this.ToArray() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.toArray(iseq [1;2;4;5;7])
     
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)

        // string IConsumableSeq
        let resultStr =IConsumableSeq.toArray (iseq ["str1";"str2";"str3"])
    
        let expectedStr =  [|"str1";"str2";"str3"|]
        Assert.AreEqual(expectedStr,resultStr)
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.toArray IConsumableSeq.empty 
        Assert.AreEqual([||],resultEpt)
        
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.toArray null |> ignore)
        ()
        
    [<Test>]    
    member this.ToArrayFromICollection() =
        let inputCollection = ResizeArray(iseq [1;2;4;5;7])
        let resultInt = IConsumableSeq.toArray((iseq inputCollection))
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        
    
    [<Test>]    
    member this.ToArrayEmptyInput() =
        let resultInt = IConsumableSeq.toArray(IConsumableSeq.empty<int>)
        let expectedInt = Array.empty<int>
        Assert.AreEqual(expectedInt,resultInt)        

    [<Test>]    
    member this.ToArrayFromArray() =
        let resultInt = IConsumableSeq.toArray((iseq [|1;2;4;5;7|]))
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        
    
    [<Test>]    
    member this.ToArrayFromList() =
        let resultInt = IConsumableSeq.toArray((iseq [1;2;4;5;7]))
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        

    [<Test>]
    member this.ToList() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.toList (iseq [1;2;4;5;7])
        let expectedInt = [1;2;4;5;7]
        Assert.AreEqual(expectedInt,resultInt)
        
        // string IConsumableSeq
        let resultStr =IConsumableSeq.toList (iseq ["str1";"str2";"str3"])
        let expectedStr =  ["str1";"str2";"str3"]
        Assert.AreEqual(expectedStr,resultStr)
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.toList IConsumableSeq.empty 
        Assert.AreEqual([],resultEpt)
         
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.toList null |> ignore)
        ()
        
    [<Test>]
    member this.Truncate() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.truncate 3 (iseq [1;2;4;5;7])
        let expectedInt = [1;2;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let resultStr =IConsumableSeq.truncate 2 (iseq ["str1";"str2";"str3"])
        let expectedStr =  ["str1";"str2"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.truncate 0 IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt
        
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.truncate 1 null |> ignore)

        // negative count
        VerifySeqsEqual IConsumableSeq.empty <| IConsumableSeq.truncate -1 (iseq [1;2;4;5;7])
        VerifySeqsEqual IConsumableSeq.empty <| IConsumableSeq.truncate System.Int32.MinValue (iseq [1;2;4;5;7])

        ()
        
    [<Test>]
    member this.tryFind() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.tryFind (fun x -> (x%2=0)) (iseq [1;2;4;5;7])
        Assert.AreEqual(Some(2), resultInt)
        
         // integer IConsumableSeq - None
        let resultInt = IConsumableSeq.tryFind (fun x -> (x%2=0)) (iseq [1;3;5;7])
        Assert.AreEqual(None, resultInt)
        
        // string IConsumableSeq
        let resultStr = IConsumableSeq.tryFind (fun (x:string) -> x.Contains("2")) (iseq ["str1";"str2";"str3"])
        Assert.AreEqual(Some("str2"),resultStr)
        
         // string IConsumableSeq - None
        let resultStr = IConsumableSeq.tryFind (fun (x:string) -> x.Contains("2")) (iseq ["str1";"str4";"str3"])
        Assert.AreEqual(None,resultStr)
       
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.tryFind (fun x -> (x%2=0)) IConsumableSeq.empty
        Assert.AreEqual(None,resultEpt)

        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.tryFind (fun x -> (x%2=0))  null |> ignore)
        ()
        
    [<Test>]
    member this.TryFindBack() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.tryFindBack (fun x -> (x%2=0)) (iseq [1;2;4;5;7])
        Assert.AreEqual(Some 4, resultInt)

        // integer IConsumableSeq - None
        let resultInt = IConsumableSeq.tryFindBack (fun x -> (x%2=0)) (iseq [1;3;5;7])
        Assert.AreEqual(None, resultInt)

        // string IConsumableSeq
        let resultStr = IConsumableSeq.tryFindBack (fun (x:string) -> x.Contains("2")) (iseq ["str1";"str2";"str2x";"str3"])
        Assert.AreEqual(Some "str2x", resultStr)

        // string IConsumableSeq - None
        let resultStr = IConsumableSeq.tryFindBack (fun (x:string) -> x.Contains("2")) (iseq ["str1";"str4";"str3"])
        Assert.AreEqual(None, resultStr)

        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.tryFindBack (fun x -> (x%2=0)) IConsumableSeq.empty
        Assert.AreEqual(None, resultEpt)

        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.tryFindBack (fun x -> (x%2=0))  null |> ignore)
        ()

    [<Test>]
    member this.TryFindIndex() =

        // integer IConsumableSeq
        let resultInt = IConsumableSeq.tryFindIndex (fun x -> (x % 5 = 0)) (iseq [8; 9; 10])
        Assert.AreEqual(Some(2), resultInt)
        
         // integer IConsumableSeq - None
        let resultInt = IConsumableSeq.tryFindIndex (fun x -> (x % 5 = 0)) (iseq [9;3;11])
        Assert.AreEqual(None, resultInt)
        
        // string IConsumableSeq
        let resultStr = IConsumableSeq.tryFindIndex (fun (x:string) -> x.Contains("2")) (iseq ["str1"; "str2"; "str3"])
        Assert.AreEqual(Some(1),resultStr)
        
         // string IConsumableSeq - None
        let resultStr = IConsumableSeq.tryFindIndex (fun (x:string) -> x.Contains("2")) (iseq ["str1"; "str4"; "str3"])
        Assert.AreEqual(None,resultStr)
       
        
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.tryFindIndex (fun x -> (x%2=0)) IConsumableSeq.empty
        Assert.AreEqual(None, resultEpt)
        
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.tryFindIndex (fun x -> (x % 2 = 0))  null |> ignore)
        ()
        
    [<Test>]
    member this.TryFindIndexBack() =

        // integer IConsumableSeq
        let resultInt = IConsumableSeq.tryFindIndexBack (fun x -> (x % 5 = 0)) (iseq [5; 9; 10; 12])
        Assert.AreEqual(Some(2), resultInt)

        // integer IConsumableSeq - None
        let resultInt = IConsumableSeq.tryFindIndexBack (fun x -> (x % 5 = 0)) (iseq [9;3;11])
        Assert.AreEqual(None, resultInt)

        // string IConsumableSeq
        let resultStr = IConsumableSeq.tryFindIndexBack (fun (x:string) -> x.Contains("2")) (iseq ["str1"; "str2"; "str2x"; "str3"])
        Assert.AreEqual(Some(2), resultStr)

        // string IConsumableSeq - None
        let resultStr = IConsumableSeq.tryFindIndexBack (fun (x:string) -> x.Contains("2")) (iseq ["str1"; "str4"; "str3"])
        Assert.AreEqual(None, resultStr)

        // empty IConsumableSeq
        let resultEpt = IConsumableSeq.tryFindIndexBack (fun x -> (x%2=0)) IConsumableSeq.empty
        Assert.AreEqual(None, resultEpt)

        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.tryFindIndexBack (fun x -> (x % 2 = 0))  null |> ignore)
        ()

    [<Test>]
    member this.Unfold() =
        // integer IConsumableSeq
        
        let resultInt = IConsumableSeq.unfold (fun x -> if x = 1 then Some(7,2) else  None) 1
        
        VerifySeqsEqual (iseq [7]) resultInt
          
        // string IConsumableSeq
        let resultStr =IConsumableSeq.unfold (fun (x:string) -> if x.Contains("unfold") then Some("a","b") else None) "unfold"
        VerifySeqsEqual (iseq ["a"]) resultStr
        ()
        
        
    [<Test>]
    member this.Windowed() =

        let testWindowed config =
            try
                config.InputSeq
                |> IConsumableSeq.windowed config.WindowSize
                |> VerifySeqsEqual config.ExpectedSeq 
            with
            | _ when Option.isNone config.Exception -> Assert.Fail()
            | e when e.GetType() = (Option.get config.Exception) -> ()
            | _ -> Assert.Fail()

        {
          InputSeq = iseq [1..10]
          WindowSize = 1
          ExpectedSeq =  iseq <| seq { for i in 1..10 do yield [| i |] }
          Exception = None
        } |> testWindowed
        {
          InputSeq = iseq [1..10]
          WindowSize = 5
          ExpectedSeq =  iseq <| seq { for i in 1..6 do yield [| i; i+1; i+2; i+3; i+4 |] }
          Exception = None
        } |> testWindowed
        {
          InputSeq = iseq [1..10]
          WindowSize = 10
          ExpectedSeq =  iseq <| seq { yield [| 1 .. 10 |] }
          Exception = None
        } |> testWindowed
        {
          InputSeq = iseq [1..10]
          WindowSize = 25
          ExpectedSeq =  IConsumableSeq.empty
          Exception = None
        } |> testWindowed
        {
          InputSeq = iseq ["str1";"str2";"str3";"str4"]
          WindowSize = 2
          ExpectedSeq =  iseq [ [|"str1";"str2"|];[|"str2";"str3"|];[|"str3";"str4"|]]
          Exception = None
        } |> testWindowed
        {
          InputSeq = IConsumableSeq.empty
          WindowSize = 2
          ExpectedSeq = IConsumableSeq.empty
          Exception = None
        } |> testWindowed
        //{
        //  InputSeq = null
        //  WindowSize = 2
        //  ExpectedSeq = IConsumableSeq.empty
        //  Exception = Some typeof<ArgumentNullException>
        //} |> testWindowed
        {
          InputSeq = iseq [1..10]
          WindowSize = 0
          ExpectedSeq =  IConsumableSeq.empty
          Exception = Some typeof<ArgumentException>
        } |> testWindowed

        ()
        
    [<Test>]
    member this.Zip() =
    
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.zip (iseq [1..7]) (iseq [11..17])
        let expectedInt = 
            iseq <| seq { for i in 1..7 do
                            yield i, i+10 }
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let resultStr =IConsumableSeq.zip (iseq ["str3";"str4"]) (iseq ["str1";"str2"])
        let expectedStr = iseq ["str3","str1";"str4","str2"]
        VerifySeqsEqual expectedStr resultStr
      
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.zip IConsumableSeq.empty IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt
          
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.zip null null |> ignore)
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.zip null (iseq [1..7]) |> ignore)
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.zip (iseq [1..7]) null |> ignore)
        ()
        
    [<Test>]
    member this.Zip3() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.zip3 (iseq [1..7]) (iseq [11..17]) (iseq [21..27])
        let expectedInt = 
            iseq <| seq { for i in 1..7 do
                            yield i, (i + 10), (i + 20) }
        VerifySeqsEqual expectedInt resultInt
        
        // string IConsumableSeq
        let resultStr =IConsumableSeq.zip3 (iseq ["str1";"str2"]) (iseq ["str11";"str12"]) (iseq ["str21";"str22"])
        let expectedStr = iseq ["str1","str11","str21";"str2","str12","str22" ]
        VerifySeqsEqual expectedStr resultStr
      
        // empty IConsumableSeq 
        let resultEpt = IConsumableSeq.zip3 IConsumableSeq.empty IConsumableSeq.empty IConsumableSeq.empty
        VerifySeqsEqual IConsumableSeq.empty resultEpt
          
        //// null IConsumableSeq
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.zip3 null null null |> ignore)
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.zip3 null (iseq [1..7]) (iseq [1..7]) |> ignore)
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.zip3 (iseq [1..7]) null (iseq [1..7]) |> ignore)
        //CheckThrowsArgumentNullException(fun() -> IConsumableSeq.zip3 (iseq [1..7]) (iseq [1..7]) null |> ignore)
        ()
        
    [<Test>]
    member this.tryPick() =
         // integer IConsumableSeq
        let resultInt = IConsumableSeq.tryPick (fun x-> if x = 1 then Some("got") else None) (iseq [1..5])
         
        Assert.AreEqual(Some("got"),resultInt)
        
        // string IConsumableSeq
        let resultStr = IConsumableSeq.tryPick (fun x-> if x = "Are" then Some("got") else None) (iseq ["Lists"; "Are"])
        Assert.AreEqual(Some("got"),resultStr)
        
        // empty IConsumableSeq   
        let resultEpt = IConsumableSeq.tryPick (fun x-> if x = 1 then Some("got") else None) IConsumableSeq.empty
        Assert.IsNull(resultEpt)
       
        //// null IConsumableSeq
        //let nullSeq : iseq<'a> = null 
        //let funcNull x = Some(1)
        
        //CheckThrowsArgumentNullException(fun () -> IConsumableSeq.tryPick funcNull nullSeq |> ignore)
   
        ()

    [<Test>]
    member this.tryItem() =
        // integer IConsumableSeq
        let resultInt = IConsumableSeq.tryItem 3 (iseq { 10..20 })
        Assert.AreEqual(Some(13), resultInt)

        // string IConsumableSeq
        let resultStr = IConsumableSeq.tryItem 2 (iseq ["Lists"; "Are"; "Cool"; "List" ])
        Assert.AreEqual(Some("Cool"), resultStr)

        // empty IConsumableSeq
        let resultEmpty = IConsumableSeq.tryItem 0 IConsumableSeq.empty
        Assert.AreEqual(None, resultEmpty)

        //// null IConsumableSeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> IConsumableSeq.tryItem 3 nullSeq |> ignore)

        // Negative index
        let resultNegativeIndex = IConsumableSeq.tryItem -1 (iseq { 10..20 })
        Assert.AreEqual(None, resultNegativeIndex)

        // Index greater than length
        let resultIndexGreater = IConsumableSeq.tryItem 31 (iseq { 10..20 })
        Assert.AreEqual(None, resultIndexGreater)

#endif