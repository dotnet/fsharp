// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections

open System
open NUnit.Framework

open FSharp.Core.Unittests.LibraryTestFx

//type iseq<'a> = ISeq.Core.ISeq<'a>

type ISeqWindowedTestInput<'t> =
    {
        InputSeq : iseq<'t>
        WindowSize : int
        ExpectedSeq : iseq<'t[]>
        Exception : Type option
    }

[<TestFixture>]
type ISeqModule2() =
    let iseq (x:seq<_>) = x |> ISeq.ofSeq

    [<Test>]
    member this.Hd() =
             
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 do
                            yield i }
                    
        if ISeq.head IntSeq <> 0 then Assert.Fail()
                 
        // string ISeq
        let strSeq = iseq ["first"; "second";  "third"]
        if ISeq.head strSeq <> "first" then Assert.Fail()
         
        // Empty ISeq
        let emptySeq = ISeq.empty
        CheckThrowsArgumentException ( fun() -> ISeq.head emptySeq)
      
        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->ISeq.head nullSeq) 
        () 

    [<Test>]
    member this.TryHead() =
        // int ISeq     
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 -> i }
                    
        let intResult = ISeq.tryHead IntSeq

        // string ISeq
        let strResult = ISeq.tryHead (iseq ["first"; "second";  "third"])
        Assert.AreEqual("first", strResult.Value)
         
        // Empty ISeq
        let emptyResult = ISeq.tryHead ISeq.empty
        Assert.AreEqual(None, emptyResult)
      
        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->ISeq.head nullSeq) 
        () 
        
    [<Test>]
    member this.Tl() =
        // integer iseq  
        let resultInt = ISeq.tail <| (iseq <| seq { 1..10 } )
        Assert.AreEqual(Array.ofSeq (iseq <| seq { 2..10 }), Array.ofSeq resultInt)
        
        // string iseq
        let resultStr = ISeq.tail <| (iseq <| seq { yield "a"; yield "b"; yield "c"; yield "d" })
        Assert.AreEqual(Array.ofSeq (iseq <| seq { yield "b";  yield "c" ; yield "d" }), Array.ofSeq resultStr)
        
        // 1-element iseq
        let resultStr2 = ISeq.tail <| (iseq <| seq { yield "a" })
        Assert.AreEqual(Array.ofSeq (ISeq.empty : iseq<string>), Array.ofSeq resultStr2)

        //CheckThrowsArgumentNullException(fun () -> ISeq.tail null |> ignore)
        CheckThrowsArgumentException(fun () -> ISeq.tail ISeq.empty |> ISeq.iter (fun _ -> failwith "Should not be reached"))
        ()

    [<Test>]
    member this.Last() =
             
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 do
                            yield i }
                    
        if ISeq.last IntSeq <> 9 then Assert.Fail()
                 
        // string ISeq
        let strSeq = iseq ["first"; "second";  "third"]
        if ISeq.last strSeq <> "third" then Assert.Fail()
         
        // Empty ISeq
        let emptySeq = ISeq.empty
        CheckThrowsArgumentException ( fun() -> ISeq.last emptySeq)
      
        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->ISeq.last nullSeq) 
        () 

    [<Test>]
    member this.TryLast() =
             
        let IntSeq =
            iseq <| seq { for i in 0 .. 9 -> i }
                    
        let intResult = ISeq.tryLast IntSeq
        Assert.AreEqual(9, intResult.Value)
                 
        // string ISeq
        let strResult = ISeq.tryLast (iseq ["first"; "second";  "third"])
        Assert.AreEqual("third", strResult.Value)
         
        // Empty ISeq
        let emptyResult = ISeq.tryLast ISeq.empty
        Assert.IsTrue(emptyResult.IsNone)
      
        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->ISeq.tryLast nullSeq |> ignore) 
        () 
        
    [<Test>]
    member this.ExactlyOne() =
             
        let IntSeq =
            iseq <| seq { for i in 7 .. 7 do
                            yield i }
                    
        if ISeq.exactlyOne IntSeq <> 7 then Assert.Fail()
                 
        // string ISeq
        let strSeq = iseq ["second"]
        if ISeq.exactlyOne strSeq <> "second" then Assert.Fail()
         
        // Empty ISeq
        let emptySeq = ISeq.empty
        CheckThrowsArgumentException ( fun() -> ISeq.exactlyOne emptySeq)
      
        // non-singleton ISeq
        let emptySeq = ISeq.empty
        CheckThrowsArgumentException ( fun() -> ISeq.exactlyOne (iseq [ 0 .. 1 ]) |> ignore )
      
        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->ISeq.exactlyOne nullSeq) 
        () 
        
                
    [<Test>]
    member this.Init() =

        let funcInt x = x
        let init_finiteInt = ISeq.init 9 funcInt
        let expectedIntSeq = iseq [ 0..8]
      
        VerifySeqsEqual expectedIntSeq  init_finiteInt
        
             
        // string ISeq
        let funcStr x = x.ToString()
        let init_finiteStr = ISeq.init 5  funcStr
        let expectedStrSeq = iseq ["0";"1";"2";"3";"4"]

        VerifySeqsEqual expectedStrSeq init_finiteStr
        
        // null ISeq
        let funcNull x = null
        let init_finiteNull = ISeq.init 3 funcNull
        let expectedNullSeq = iseq [ null;null;null]
        
        VerifySeqsEqual expectedNullSeq init_finiteNull
        () 
        
    [<Test>]
    member this.InitInfinite() =

        let funcInt x = x
        let init_infiniteInt = ISeq.initInfinite funcInt
        let resultint = ISeq.find (fun x -> x =100) init_infiniteInt
        
        Assert.AreEqual(100,resultint)
        
             
        // string ISeq
        let funcStr x = x.ToString()
        let init_infiniteStr = ISeq.initInfinite  funcStr
        let resultstr = ISeq.find (fun x -> x = "100") init_infiniteStr
        
        Assert.AreEqual("100",resultstr)
       
       
    [<Test>]
    member this.IsEmpty() =
        
        //iseq int
        let seqint = iseq [1;2;3]
        let is_emptyInt = ISeq.isEmpty seqint
        
        Assert.IsFalse(is_emptyInt)
              
        //iseq str
        let seqStr = iseq["first";"second"]
        let is_emptyStr = ISeq.isEmpty  seqStr

        Assert.IsFalse(is_emptyInt)
        
        //iseq empty
        let seqEmpty = ISeq.empty
        let is_emptyEmpty = ISeq.isEmpty  seqEmpty
        Assert.IsTrue(is_emptyEmpty) 
        
        ////iseq null
        //let seqnull:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.isEmpty seqnull |> ignore)
        ()
        
    [<Test>]
    member this.Iter() =
        //iseq int
        let seqint =  iseq [ 1..3]
        let cacheint = ref 0
       
        let funcint x = cacheint := !cacheint + x
        ISeq.iter funcint seqint
        Assert.AreEqual(6,!cacheint)
              
        //iseq str
        let seqStr = iseq ["first";"second"]
        let cachestr =ref ""
        let funcstr x = cachestr := !cachestr+x
        ISeq.iter funcstr seqStr
         
        Assert.AreEqual("firstsecond",!cachestr)
        
         // empty array    
        let emptyseq = ISeq.empty
        let resultEpt = ref 0
        ISeq.iter (fun x -> Assert.Fail()) emptyseq   

        //// null seqay
        //let nullseq:iseq<'a> =  null
        
        //CheckThrowsArgumentNullException (fun () -> ISeq.iter funcint nullseq |> ignore)  
        ()
        
    [<Test>]
    member this.Iter2() =
    
        //iseq int
        let seqint =  iseq [ 1..3]
        let cacheint = ref 0
       
        let funcint x y = cacheint := !cacheint + x+y
        ISeq.iter2 funcint seqint seqint
        Assert.AreEqual(12,!cacheint)
              
        //iseq str
        let seqStr = iseq ["first";"second"]
        let cachestr =ref ""
        let funcstr x y = cachestr := !cachestr+x+y
        ISeq.iter2 funcstr seqStr seqStr
         
        Assert.AreEqual("firstfirstsecondsecond",!cachestr)
        
         // empty array    
        let emptyseq = ISeq.empty
        let resultEpt = ref 0
        ISeq.iter2 (fun x y-> Assert.Fail()) emptyseq  emptyseq 

        //// null seqay
        //let nullseq:iseq<'a> =  null
        //CheckThrowsArgumentNullException (fun () -> ISeq.iter2 funcint nullseq nullseq |> ignore)  
        
        ()
        
    [<Test>]
    member this.Iteri() =
    
        // iseq int
        let seqint =  iseq [ 1..10]
        let cacheint = ref 0
       
        let funcint x y = cacheint := !cacheint + x+y
        ISeq.iteri funcint seqint
        Assert.AreEqual(100,!cacheint)
              
        // iseq str
        let seqStr = iseq ["first";"second"]
        let cachestr =ref 0
        let funcstr (x:int) (y:string) = cachestr := !cachestr+ x + y.Length
        ISeq.iteri funcstr seqStr
         
        Assert.AreEqual(12,!cachestr)
        
         // empty array    
        let emptyseq = ISeq.empty
        let resultEpt = ref 0
        ISeq.iteri funcint emptyseq
        Assert.AreEqual(0,!resultEpt)

        //// null seqay
        //let nullseq:iseq<'a> =  null
        //CheckThrowsArgumentNullException (fun () -> ISeq.iteri funcint nullseq |> ignore)  
        ()

    [<Test>]
    member this.Iteri2() =

        //iseq int
        let seqint = iseq [ 1..3]
        let cacheint = ref 0
       
        let funcint x y z = cacheint := !cacheint + x + y + z
        ISeq.iteri2 funcint seqint seqint
        Assert.AreEqual(15,!cacheint)
              
        //iseq str
        let seqStr = iseq ["first";"second"]
        let cachestr = ref 0
        let funcstr (x:int) (y:string) (z:string) = cachestr := !cachestr + x + y.Length + z.Length
        ISeq.iteri2 funcstr seqStr seqStr
         
        Assert.AreEqual(23,!cachestr)
        
        // empty iseq
        let emptyseq = ISeq.empty
        let resultEpt = ref 0
        ISeq.iteri2 (fun x y z -> Assert.Fail()) emptyseq emptyseq 

        //// null iseq
        //let nullseq:iseq<'a> =  null
        //CheckThrowsArgumentNullException (fun () -> ISeq.iteri2 funcint nullseq nullseq |> ignore)  
        
        // len1 <> len2
        let shorterSeq = iseq <| seq { 1..3 }
        let longerSeq = iseq <| seq { 2..2..100 }

        let testSeqLengths seq1 seq2 =
            let cache = ref 0
            let f x y z = cache := !cache + x + y + z
            ISeq.iteri2 f seq1 seq2
            !cache

        Assert.AreEqual(21, testSeqLengths shorterSeq longerSeq)
        Assert.AreEqual(21, testSeqLengths longerSeq shorterSeq)

        ()
        
    [<Test>]
    member this.Length() =

         // integer iseq  
        let resultInt = ISeq.length (iseq {1..8})
        if resultInt <> 8 then Assert.Fail()
        
        // string ISeq    
        let resultStr = ISeq.length (iseq ["Lists"; "are";  "commonly" ; "list" ])
        if resultStr <> 4 then Assert.Fail()
        
        // empty ISeq     
        let resultEpt = ISeq.length ISeq.empty
        if resultEpt <> 0 then Assert.Fail()

        //// null ISeq
        //let nullSeq:iseq<'a> = null     
        //CheckThrowsArgumentNullException (fun () -> ISeq.length  nullSeq |> ignore)  
        
        ()
        
    [<Test>]
    member this.Map() =

         // integer ISeq
        let funcInt x = 
                match x with
                | _ when x % 2 = 0 -> 10*x            
                | _ -> x
       
        let resultInt = ISeq.map funcInt (iseq { 1..10 })
        let expectedint = iseq [1;20;3;40;5;60;7;80;9;100]
        
        VerifySeqsEqual expectedint resultInt
        
        // string ISeq
        let funcStr (x:string) = x.ToLower()
        let resultStr = ISeq.map funcStr (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = iseq ["lists"; "are";  "commonly" ; "list"]
        
        VerifySeqsEqual expectedSeq resultStr
        
        // empty ISeq
        let resultEpt = ISeq.map funcInt ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt

        //// null ISeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () -> ISeq.map funcStr nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.Map2() =
         // integer ISeq
        let funcInt x y = x+y
        let resultInt = ISeq.map2 funcInt (iseq { 1..10 }) (iseq {2..2..20})
        let expectedint = iseq [3;6;9;12;15;18;21;24;27;30]
        
        VerifySeqsEqual expectedint resultInt
        
        // string ISeq
        let funcStr (x:int) (y:string) = x+y.Length
        let resultStr = ISeq.map2 funcStr (iseq[3;6;9;11]) (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = iseq [8;9;17;15]
        
        VerifySeqsEqual expectedSeq resultStr
        
        // empty ISeq
        let resultEpt = ISeq.map2 funcInt ISeq.empty ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt

        //// null ISeq
        //let nullSeq:iseq<'a> = null 
        //let validSeq = iseq [1]
        //CheckThrowsArgumentNullException (fun () -> ISeq.map2 funcInt nullSeq validSeq |> ignore)
        
        ()

    [<Test>]
    member this.Map3() = 
        // Integer iseq
        let funcInt a b c = (a + b) * c
        let resultInt = ISeq.map3 funcInt (iseq { 1..8 }) (iseq { 2..9 }) (iseq { 3..10 })
        let expectedInt = iseq [9; 20; 35; 54; 77; 104; 135; 170]
        VerifySeqsEqual expectedInt resultInt

        // First iseq is shorter
        VerifySeqsEqual (iseq [9; 20]) (ISeq.map3 funcInt (iseq { 1..2 }) (iseq { 2..9 }) (iseq { 3..10 }))
        // Second iseq is shorter
        VerifySeqsEqual (iseq [9; 20; 35]) (ISeq.map3 funcInt (iseq { 1..8 }) (iseq { 2..4 }) (iseq { 3..10 }))
        // Third iseq is shorter
        VerifySeqsEqual (iseq [9; 20; 35; 54]) (ISeq.map3 funcInt (iseq { 1..8 }) (iseq { 2..6 }) (iseq { 3..6 }))

        // String iseq
        let funcStr a b c = a + b + c
        let resultStr = ISeq.map3 funcStr (iseq ["A";"B";"C";"D"]) (iseq ["a";"b";"c";"d"]) (iseq ["1";"2";"3";"4"])
        let expectedStr = iseq ["Aa1";"Bb2";"Cc3";"Dd4"]
        VerifySeqsEqual expectedStr resultStr

        // Empty iseq
        let resultEmpty = ISeq.map3 funcStr ISeq.empty ISeq.empty ISeq.empty
        VerifySeqsEqual ISeq.empty resultEmpty

        //// Null iseq
        //let nullSeq = null : iseq<_>
        //let nonNullSeq = iseq [1]
        //CheckThrowsArgumentNullException (fun () -> ISeq.map3 funcInt nullSeq nonNullSeq nullSeq |> ignore)

        ()

    [<Test>]
    member this.MapFold() =
        // integer ISeq
        let funcInt acc x = if x % 2 = 0 then 10*x, acc + 1 else x, acc
        let resultInt,resultIntAcc = ISeq.mapFold funcInt 100 <| (iseq <| seq { 1..10 })
        VerifySeqsEqual (iseq [ 1;20;3;40;5;60;7;80;9;100 ]) resultInt
        Assert.AreEqual(105, resultIntAcc)

        // string ISeq
        let funcStr acc (x:string) = match x.Length with 0 -> "empty", acc | _ -> x.ToLower(), sprintf "%s%s" acc x
        let resultStr,resultStrAcc = ISeq.mapFold funcStr "" <| iseq [ "";"BB";"C";"" ]
        VerifySeqsEqual (iseq [ "empty";"bb";"c";"empty" ]) resultStr
        Assert.AreEqual("BBC", resultStrAcc)

        // empty ISeq
        let resultEpt,resultEptAcc = ISeq.mapFold funcInt 100 ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt
        Assert.AreEqual(100, resultEptAcc)

        //// null ISeq
        //let nullArr = null:iseq<string>
        //CheckThrowsArgumentNullException (fun () -> ISeq.mapFold funcStr "" nullArr |> ignore)

        ()

    [<Test>]
    member this.MapFoldBack() =
        // integer ISeq
        let funcInt x acc = if acc < 105 then 10*x, acc + 2 else x, acc
        let resultInt,resultIntAcc = ISeq.mapFoldBack funcInt (iseq <| seq { 1..10 }) 100
        VerifySeqsEqual (iseq [ 1;2;3;4;5;6;7;80;90;100 ]) resultInt
        Assert.AreEqual(106, resultIntAcc)

        // string ISeq
        let funcStr (x:string) acc = match x.Length with 0 -> "empty", acc | _ -> x.ToLower(), sprintf "%s%s" acc x
        let resultStr,resultStrAcc = ISeq.mapFoldBack funcStr (iseq [ "";"BB";"C";"" ]) ""
        VerifySeqsEqual (iseq [ "empty";"bb";"c";"empty" ]) resultStr
        Assert.AreEqual("CBB", resultStrAcc)

        // empty ISeq
        let resultEpt,resultEptAcc = ISeq.mapFoldBack funcInt ISeq.empty 100
        VerifySeqsEqual ISeq.empty resultEpt
        Assert.AreEqual(100, resultEptAcc)

        //// null ISeq
        //let nullArr = null:iseq<string>
        //CheckThrowsArgumentNullException (fun () -> ISeq.mapFoldBack funcStr nullArr "" |> ignore)

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
        this.MapWithSideEffectsTester ISeq.map true
        
    [<Test>]
    member this.MapWithException () =
        this.MapWithExceptionTester ISeq.map

        
    [<Test>]
    member this.SingletonCollectWithSideEffects () =
        this.MapWithSideEffectsTester (fun f-> ISeq.collect (f >> ISeq.singleton)) true
        
    [<Test>]
    member this.SingletonCollectWithException () =
        this.MapWithExceptionTester (fun f-> ISeq.collect (f >> ISeq.singleton))

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
        let e = ((iseq [1;2]) |> ISeq.mapi f).GetEnumerator()
        
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
        let e = ((iseq []) |> ISeq.mapi f).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        
    [<Test>]
    member this.Map2WithSideEffects () =
        let i = ref 0
        let f x y = i := !i + 1; x*x
        let e = (ISeq.map2 f (iseq [1;2]) (iseq [1;2])).GetEnumerator()
        
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
        let e = (ISeq.map2 f (iseq []) (iseq [])).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        
    [<Test>]
    member this.Mapi2WithSideEffects () =
        let i = ref 0
        let f _ x y = i := !i + 1; x*x
        let e = (ISeq.mapi2 f (iseq [1;2]) (iseq [1;2])).GetEnumerator()

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
        let e = (ISeq.mapi2 f (iseq []) (iseq [])).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)

    [<Test>]
    member this.Collect() =
         // integer ISeq
        let funcInt x = iseq [x+1]
        let resultInt = ISeq.collect funcInt (iseq { 1..10 })
       
        let expectedint = iseq <| seq {2..11}
        
        VerifySeqsEqual expectedint resultInt

//#if !FX_NO_CHAR_PARSE
//        // string ISeq
//        let funcStr (y:string) = y+"ist"
       
//        let resultStr = ISeq.collect funcStr (iseq ["L"])
        
        
//        let expectedSeq = iseq ['L';'i';'s';'t']
        
//        VerifySeqsEqual expectedSeq resultStr
//#endif        
        // empty ISeq
        let resultEpt = ISeq.collect funcInt ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt

        //// null ISeq
        //let nullSeq:iseq<'a> = null 
       
        //CheckThrowsArgumentNullException (fun () -> ISeq.collect funcInt nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.Mapi() =

         // integer ISeq
        let funcInt x y = x+y
        let resultInt = ISeq.mapi funcInt (iseq { 10..2..20 } )
        let expectedint = iseq [10;13;16;19;22;25]
        
        VerifySeqsEqual expectedint resultInt
        
        // string ISeq
        let funcStr (x:int) (y:string) =x+y.Length
       
        let resultStr = ISeq.mapi funcStr (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedStr = iseq [5;4;10;7]
         
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq
        let resultEpt = ISeq.mapi funcInt ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt

        //// null ISeq
        //let nullSeq:iseq<'a> = null 
       
        //CheckThrowsArgumentNullException (fun () -> ISeq.mapi funcInt nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.Mapi2() =
         // integer ISeq
        let funcInt x y z = x+y+z
        let resultInt = ISeq.mapi2 funcInt (iseq { 1..10 }) (iseq {2..2..20})
        let expectedint = iseq [3;7;11;15;19;23;27;31;35;39]

        VerifySeqsEqual expectedint resultInt

        // string ISeq
        let funcStr (x:int) (y:int) (z:string) = x+y+z.Length
        let resultStr = ISeq.mapi2 funcStr (iseq[3;6;9;11]) (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = iseq [8;10;19;18]

        VerifySeqsEqual expectedSeq resultStr

        // empty ISeq
        let resultEpt = ISeq.mapi2 funcInt ISeq.empty ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //let validSeq = iseq [1]
        //CheckThrowsArgumentNullException (fun () -> ISeq.mapi2 funcInt nullSeq validSeq |> ignore)

        // len1 <> len2
        let shorterSeq = iseq <| seq { 1..10 }
        let longerSeq = iseq <| seq { 2..20 }

        let testSeqLengths seq1 seq2 =
            let f x y z = x + y + z
            ISeq.mapi2 f seq1 seq2

        VerifySeqsEqual (iseq [3;6;9;12;15;18;21;24;27;30]) (testSeqLengths shorterSeq longerSeq)
        VerifySeqsEqual (iseq [3;6;9;12;15;18;21;24;27;30]) (testSeqLengths longerSeq shorterSeq)

    [<Test>]
    member this.Indexed() =

         // integer ISeq
        let resultInt = ISeq.indexed (iseq { 10..2..20 })
        let expectedint = iseq [(0,10);(1,12);(2,14);(3,16);(4,18);(5,20)]

        VerifySeqsEqual expectedint resultInt

        // string ISeq
        let resultStr = ISeq.indexed (iseq ["Lists"; "Are"; "Commonly"; "List" ])
        let expectedStr = iseq [(0,"Lists");(1,"Are");(2,"Commonly");(3,"List")]

        VerifySeqsEqual expectedStr resultStr

        // empty ISeq
        let resultEpt = ISeq.indexed ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.indexed nullSeq |> ignore)

        ()

    [<Test>]
    member this.Max() =
         // integer ISeq
        let resultInt = ISeq.max (iseq { 10..20 } )
        
        Assert.AreEqual(20,resultInt)
        
        // string ISeq
       
        let resultStr = ISeq.max (iseq ["Lists"; "Are";  "MaxString" ; "List" ])
        Assert.AreEqual("MaxString",resultStr)
          
        // empty ISeq
        CheckThrowsArgumentException(fun () -> ISeq.max ( ISeq.empty : iseq<float>) |> ignore)
        
        //// null ISeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () -> ISeq.max nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.MaxBy() =
    
        // integer ISeq
        let funcInt x = x % 8
        let resultInt = ISeq.maxBy funcInt (iseq { 2..2..20 } )
        Assert.AreEqual(6,resultInt)
        
        // string ISeq
        let funcStr (x:string)  =x.Length 
        let resultStr = ISeq.maxBy funcStr (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        Assert.AreEqual("Commonly",resultStr)
          
        // empty ISeq
        CheckThrowsArgumentException (fun () -> ISeq.maxBy funcInt (ISeq.empty : iseq<int>) |> ignore)
        
        //// null ISeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () ->ISeq.maxBy funcInt nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.MinBy() =
    
        // integer ISeq
        let funcInt x = x % 8
        let resultInt = ISeq.minBy funcInt (iseq { 2..2..20 } )
        Assert.AreEqual(8,resultInt)
        
        // string ISeq
        let funcStr (x:string)  =x.Length 
        let resultStr = ISeq.minBy funcStr (iseq ["Lists"; "Are";  "Commonly" ; "List" ])
        Assert.AreEqual("Are",resultStr)
          
        // empty ISeq
        CheckThrowsArgumentException (fun () -> ISeq.minBy funcInt (ISeq.empty : iseq<int>) |> ignore) 
        
        //// null ISeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () ->ISeq.minBy funcInt nullSeq |> ignore)
        
        ()
        
          
    [<Test>]
    member this.Min() =

         // integer ISeq
        let resultInt = ISeq.min (iseq { 10..20 } )
        Assert.AreEqual(10,resultInt)
        
        // string ISeq
        let resultStr = ISeq.min (iseq ["Lists"; "Are";  "minString" ; "List" ])
        Assert.AreEqual("Are",resultStr)
          
        // empty ISeq
        CheckThrowsArgumentException (fun () -> ISeq.min (ISeq.empty : iseq<int>) |> ignore) 
        
        //// null ISeq
        //let nullSeq:iseq<'a> = null 
        //CheckThrowsArgumentNullException (fun () -> ISeq.min nullSeq |> ignore)
        
        ()

    [<Test>]
    member this.Item() =
         // integer ISeq
        let resultInt = ISeq.item 3 (iseq { 10..20 })
        Assert.AreEqual(13, resultInt)

        // string ISeq
        let resultStr = ISeq.item 2 (iseq ["Lists"; "Are"; "Cool" ; "List" ])
        Assert.AreEqual("Cool", resultStr)

        // empty ISeq
        CheckThrowsArgumentException(fun () -> ISeq.item 0 (ISeq.empty : iseq<decimal>) |> ignore)

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () ->ISeq.item 3 nullSeq |> ignore)

        // Negative index
        for i = -1 downto -10 do
           CheckThrowsArgumentException (fun () -> ISeq.item i (iseq { 10 .. 20 }) |> ignore)

        // Out of range
        for i = 11 to 20 do
           CheckThrowsArgumentException (fun () -> ISeq.item i (iseq { 10 .. 20 }) |> ignore)

    [<Test>]
    member this.``item should fail with correct number of missing elements``() =
        try
            ISeq.item 0 (iseq (Array.zeroCreate<int> 0)) |> ignore
            failwith "error expected"
        with
        | exn when exn.Message.Contains("seq was short by 1 element") -> ()

        try
            ISeq.item 2 (iseq (Array.zeroCreate<int> 0)) |> ignore
            failwith "error expected"
        with
        | exn when exn.Message.Contains("seq was short by 3 elements") -> ()

    [<Test>]
    member this.Of_Array() =
         // integer ISeq
        let resultInt = ISeq.ofArray [|1..10|]
        let expectedInt = {1..10}
         
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let resultStr = ISeq.ofArray [|"Lists"; "Are";  "ofArrayString" ; "List" |]
        let expectedStr = iseq ["Lists"; "Are";  "ofArrayString" ; "List" ]
        VerifySeqsEqual expectedStr resultStr
          
        // empty ISeq 
        let resultEpt = ISeq.ofArray [| |] 
        VerifySeqsEqual resultEpt ISeq.empty
       
        ()
        
    [<Test>]
    member this.Of_List() =
         // integer ISeq
        let resultInt = ISeq.ofList [1..10]
        let expectedInt = {1..10}
         
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
       
        let resultStr =ISeq.ofList ["Lists"; "Are";  "ofListString" ; "List" ]
        let expectedStr = iseq ["Lists"; "Are";  "ofListString" ; "List" ]
        VerifySeqsEqual expectedStr resultStr
          
        // empty ISeq 
        let resultEpt = ISeq.ofList [] 
        VerifySeqsEqual resultEpt ISeq.empty
        ()
        
          
    [<Test>]
    member this.Pairwise() =
         // integer ISeq
        let resultInt = ISeq.pairwise (iseq {1..3})
       
        let expectedInt = iseq [1,2;2,3]
         
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let resultStr =ISeq.pairwise (iseq ["str1"; "str2";"str3" ])
        let expectedStr = iseq ["str1","str2";"str2","str3"]
        VerifySeqsEqual expectedStr resultStr
          
        // empty ISeq 
        let resultEpt = ISeq.pairwise (iseq [] )
        VerifySeqsEqual resultEpt ISeq.empty
       
        ()
        
    [<Test>]
    member this.Reduce() =
         
        // integer ISeq
        let resultInt = ISeq.reduce (fun x y -> x/y) (iseq [5*4*3*2; 4;3;2;1])
        Assert.AreEqual(5,resultInt)
        
        // string ISeq
        let resultStr = ISeq.reduce (fun (x:string) (y:string) -> x.Remove(0,y.Length)) (iseq ["ABCDE";"A"; "B";  "C" ; "D" ])
        Assert.AreEqual("E",resultStr) 
       
        // empty ISeq 
        CheckThrowsArgumentException (fun () -> ISeq.reduce (fun x y -> x/y)  ISeq.empty |> ignore)
        
        //// null ISeq
        //let nullSeq : iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.reduce (fun (x:string) (y:string) -> x.Remove(0,y.Length))  nullSeq  |> ignore)   
        ()

    [<Test>]
    member this.ReduceBack() =
        // int ISeq
        let funcInt x y = x - y
        let IntSeq = iseq <| seq { 1..4 }
        let reduceInt = ISeq.reduceBack funcInt IntSeq
        Assert.AreEqual((1-(2-(3-4))), reduceInt)

        // string ISeq
        let funcStr (x:string) (y:string) = y.Remove(0,x.Length)
        let strSeq = iseq [ "A"; "B"; "C"; "D" ; "ABCDE" ]
        let reduceStr = ISeq.reduceBack  funcStr strSeq
        Assert.AreEqual("E", reduceStr)
        
        // string ISeq
        let funcStr2 elem acc = sprintf "%s%s" elem acc
        let strSeq2 = iseq [ "A" ]
        let reduceStr2 = ISeq.reduceBack  funcStr2 strSeq2
        Assert.AreEqual("A", reduceStr2)

        // Empty ISeq
        CheckThrowsArgumentException (fun () -> ISeq.reduceBack funcInt ISeq.empty |> ignore)

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.reduceBack funcInt nullSeq |> ignore)

        ()

    [<Test>]
    member this.Rev() =
        // integer ISeq
        let resultInt = ISeq.rev (iseq [5;4;3;2;1])
        VerifySeqsEqual (iseq[1;2;3;4;5]) resultInt

        // string ISeq
        let resultStr = ISeq.rev (iseq ["A"; "B";  "C" ; "D" ])
        VerifySeqsEqual (iseq["D";"C";"B";"A"]) resultStr

        // empty ISeq
        VerifySeqsEqual ISeq.empty (ISeq.rev ISeq.empty)

        //// null ISeq
        //let nullSeq : iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.rev nullSeq  |> ignore)
        ()

    [<Test>]
    member this.Scan() =
        // integer ISeq
        let funcInt x y = x+y
        let resultInt = ISeq.scan funcInt 9 (iseq {1..10})
        let expectedInt = iseq [9;10;12;15;19;24;30;37;45;54;64]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let funcStr x y = x+y
        let resultStr =ISeq.scan funcStr "x" (iseq ["str1"; "str2";"str3" ])
        
        let expectedStr = iseq ["x";"xstr1"; "xstr1str2";"xstr1str2str3"]
        VerifySeqsEqual expectedStr resultStr
          
        // empty ISeq 
        let resultEpt = ISeq.scan funcInt 5 ISeq.empty 
       
        VerifySeqsEqual resultEpt (iseq [ 5])
       
        //// null ISeq
        //let seqNull:iseq<'a> = null
        //CheckThrowsArgumentNullException(fun() -> ISeq.scan funcInt 5 seqNull |> ignore)
        ()
        
    [<Test>]
    member this.ScanBack() =
        // integer ISeq
        let funcInt x y = x+y
        let resultInt = ISeq.scanBack funcInt (iseq { 1..10 }) 9
        let expectedInt = iseq [64;63;61;58;54;49;43;36;28;19;9]
        VerifySeqsEqual expectedInt resultInt

        // string ISeq
        let funcStr x y = x+y
        let resultStr = ISeq.scanBack funcStr (iseq ["A";"B";"C";"D"]) "X"
        let expectedStr = iseq ["ABCDX";"BCDX";"CDX";"DX";"X"]
        VerifySeqsEqual expectedStr resultStr

        // empty ISeq
        let resultEpt = ISeq.scanBack funcInt ISeq.empty 5
        let expectedEpt = iseq [5]
        VerifySeqsEqual expectedEpt resultEpt

        //// null ISeq
        //let seqNull:iseq<'a> = null
        //CheckThrowsArgumentNullException(fun() -> ISeq.scanBack funcInt seqNull 5 |> ignore)

        // exception cases
        let funcEx x (s:'State) = raise <| new System.FormatException() : 'State
        // calling scanBack with funcEx does not throw
        let resultEx = ISeq.scanBack funcEx (iseq <| seq {1..10}) 0
        // reading from resultEx throws
        CheckThrowsFormatException(fun() -> ISeq.head resultEx |> ignore)

        // Result consumes entire input sequence as soon as it is accesses an element
        let i = ref 0
        let funcState x s = (i := !i + x); x+s
        let resultState = ISeq.scanBack funcState (iseq <| seq {1..3}) 0
        Assert.AreEqual(0, !i)
        use e = resultState.GetEnumerator()
        Assert.AreEqual(6, !i)

        ()

    [<Test>]
    member this.Singleton() =
        // integer ISeq
        let resultInt = ISeq.singleton 1
       
        let expectedInt = iseq [1]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let resultStr =ISeq.singleton "str1"
        let expectedStr = iseq ["str1"]
        VerifySeqsEqual expectedStr resultStr
         
        // null ISeq
        let resultNull = ISeq.singleton null
        let expectedNull = iseq [null]
        VerifySeqsEqual expectedNull resultNull
        ()
    
        
    [<Test>]
    member this.Skip() =
    
        // integer ISeq
        let resultInt = ISeq.skip 2 (iseq [1;2;3;4])
        let expectedInt = iseq [3;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let resultStr =ISeq.skip 2 (iseq ["str1";"str2";"str3";"str4"])
        let expectedStr = iseq ["str3";"str4"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.skip 0 ISeq.empty 
        VerifySeqsEqual resultEpt ISeq.empty
        
         
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.skip 1 null |> ignore)
        ()
       
    [<Test>]
    member this.Skip_While() =
    
        // integer ISeq
        let funcInt x = (x < 3)
        let resultInt = ISeq.skipWhile funcInt (iseq [1;2;3;4;5;6])
        let expectedInt = iseq [3;4;5;6]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let funcStr (x:string) = x.Contains(".")
        let resultStr =ISeq.skipWhile funcStr (iseq [".";"asdfasdf.asdfasdf";"";"";"";"";"";"";"";"";""])
        let expectedStr = iseq ["";"";"";"";"";"";"";"";""]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.skipWhile funcInt ISeq.empty 
        VerifySeqsEqual resultEpt ISeq.empty
        
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.skipWhile funcInt null |> ignore)
        ()
       
    [<Test>]
    member this.Sort() =

        // integer ISeq
        let resultInt = ISeq.sort (iseq [1;3;2;4;6;5;7])
        let expectedInt = {1..7}
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
       
        let resultStr =ISeq.sort (iseq ["str1";"str3";"str2";"str4"])
        let expectedStr = iseq ["str1";"str2";"str3";"str4"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.sort ISeq.empty 
        VerifySeqsEqual resultEpt ISeq.empty
         
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.sort null  |> ignore)
        ()
        
    [<Test>]
    member this.SortBy() =

        // integer ISeq
        let funcInt x = Math.Abs(x-5)
        let resultInt = ISeq.sortBy funcInt (iseq [1;2;4;5;7])
        let expectedInt = iseq [5;4;7;2;1]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let funcStr (x:string) = x.IndexOf("key")
        let resultStr =ISeq.sortBy funcStr (iseq ["st(key)r";"str(key)";"s(key)tr";"(key)str"])
        
        let expectedStr = iseq ["(key)str";"s(key)tr";"st(key)r";"str(key)"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.sortBy funcInt ISeq.empty 
        VerifySeqsEqual resultEpt ISeq.empty
         
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.sortBy funcInt null  |> ignore)
        ()

    [<Test>]
    member this.SortDescending() =

        // integer ISeq
        let resultInt = ISeq.sortDescending (iseq [1;3;2;Int32.MaxValue;4;6;Int32.MinValue;5;7;0])
        let expectedInt = iseq <| seq {
            yield Int32.MaxValue;
            yield! iseq{ 7..-1..0 }
            yield Int32.MinValue
        }
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
       
        let resultStr = ISeq.sortDescending (iseq ["str1";null;"str3";"";"Str1";"str2";"str4"])
        let expectedStr = iseq ["str4";"str3";"str2";"str1";"Str1";"";null]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.sortDescending ISeq.empty 
        VerifySeqsEqual resultEpt ISeq.empty

        // tuple ISeq
        let tupSeq = (iseq[(2,"a");(1,"d");(1,"b");(1,"a");(2,"x");(2,"b");(1,"x")])
        let resultTup = ISeq.sortDescending tupSeq
        let expectedTup = (iseq[(2,"x");(2,"b");(2,"a");(1,"x");(1,"d");(1,"b");(1,"a")])   
        VerifySeqsEqual  expectedTup resultTup
         
        // float ISeq
        let minFloat,maxFloat,epsilon = System.Double.MinValue,System.Double.MaxValue,System.Double.Epsilon
        let floatSeq = iseq [0.0; 0.5; 2.0; 1.5; 1.0; minFloat;maxFloat;epsilon;-epsilon]
        let resultFloat = ISeq.sortDescending floatSeq
        let expectedFloat = iseq [maxFloat; 2.0; 1.5; 1.0; 0.5; epsilon; 0.0; -epsilon; minFloat; ]
        VerifySeqsEqual expectedFloat resultFloat

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.sort null  |> ignore)
        ()
        
    [<Test>]
    member this.SortByDescending() =

        // integer ISeq
        let funcInt x = Math.Abs(x-5)
        let resultInt = ISeq.sortByDescending funcInt (iseq [1;2;4;5;7])
        let expectedInt = iseq [1;2;7;4;5]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let funcStr (x:string) = x.IndexOf("key")
        let resultStr =ISeq.sortByDescending funcStr (iseq ["st(key)r";"str(key)";"s(key)tr";"(key)str"])
        
        let expectedStr = iseq ["str(key)";"st(key)r";"s(key)tr";"(key)str"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.sortByDescending funcInt ISeq.empty 
        VerifySeqsEqual resultEpt ISeq.empty

        // tuple ISeq
        let tupSeq = (iseq[(2,"a");(1,"d");(1,"b");(1,"a");(2,"x");(2,"b");(1,"x")])
        let resultTup = ISeq.sortByDescending snd tupSeq         
        let expectedTup = (iseq[(2,"x");(1,"x");(1,"d");(1,"b");(2,"b");(2,"a");(1,"a")])
        VerifySeqsEqual  expectedTup resultTup
         
        // float ISeq
        let minFloat,maxFloat,epsilon = System.Double.MinValue,System.Double.MaxValue,System.Double.Epsilon
        let floatSeq = iseq [0.0; 0.5; 2.0; 1.5; 1.0; minFloat;maxFloat;epsilon;-epsilon]
        let resultFloat = ISeq.sortByDescending id floatSeq
        let expectedFloat = iseq [maxFloat; 2.0; 1.5; 1.0; 0.5; epsilon; 0.0; -epsilon; minFloat; ]
        VerifySeqsEqual expectedFloat resultFloat

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.sortByDescending funcInt null  |> ignore)
        ()
        
    member this.SortWith() =

        // integer ISeq
        let intComparer a b = compare (a%3) (b%3)
        let resultInt = ISeq.sortWith intComparer (iseq <| seq {0..10})
        let expectedInt = iseq [0;3;6;9;1;4;7;10;2;5;8]
        VerifySeqsEqual expectedInt resultInt

        // string ISeq
        let resultStr = ISeq.sortWith compare (iseq ["str1";"str3";"str2";"str4"])
        let expectedStr = iseq ["str1";"str2";"str3";"str4"]
        VerifySeqsEqual expectedStr resultStr

        // empty ISeq
        let resultEpt = ISeq.sortWith intComparer ISeq.empty
        VerifySeqsEqual resultEpt ISeq.empty

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.sortWith intComparer null  |> ignore)

        ()

    [<Test>]
    member this.Sum() =
    
        // integer ISeq
        let resultInt = ISeq.sum (iseq [1..10])
        Assert.AreEqual(55,resultInt)
        
        // float32 ISeq
        let floatSeq = (iseq [ 1.2f;3.5f;6.7f ])
        let resultFloat = ISeq.sum floatSeq
        if resultFloat <> 11.4f then Assert.Fail()
        
        // double ISeq
        let doubleSeq = (iseq [ 1.0;8.0 ])
        let resultDouble = ISeq.sum doubleSeq
        if resultDouble <> 9.0 then Assert.Fail()
        
        // decimal ISeq
        let decimalSeq = (iseq [ 0M;19M;19.03M ])
        let resultDecimal = ISeq.sum decimalSeq
        if resultDecimal <> 38.03M then Assert.Fail()      
          
      
        // empty float32 ISeq
        let emptyFloatSeq = ISeq.empty<System.Single> 
        let resultEptFloat = ISeq.sum emptyFloatSeq 
        if resultEptFloat <> 0.0f then Assert.Fail()
        
        // empty double ISeq
        let emptyDoubleSeq = ISeq.empty<System.Double> 
        let resultDouEmp = ISeq.sum emptyDoubleSeq 
        if resultDouEmp <> 0.0 then Assert.Fail()
        
        // empty decimal ISeq
        let emptyDecimalSeq = ISeq.empty<System.Decimal> 
        let resultDecEmp = ISeq.sum emptyDecimalSeq 
        if resultDecEmp <> 0M then Assert.Fail()
       
        ()
        
    [<Test>]
    member this.SumBy() =

        // integer ISeq
        let resultInt = ISeq.sumBy int (iseq [1..10])
        Assert.AreEqual(55,resultInt)
        
        // float32 ISeq
        let floatSeq = (iseq [ 1.2f;3.5f;6.7f ])
        let resultFloat = ISeq.sumBy float32 floatSeq
        if resultFloat <> 11.4f then Assert.Fail()
        
        // double ISeq
        let doubleSeq = (iseq [ 1.0;8.0 ])
        let resultDouble = ISeq.sumBy double doubleSeq
        if resultDouble <> 9.0 then Assert.Fail()
        
        // decimal ISeq
        let decimalSeq = (iseq [ 0M;19M;19.03M ])
        let resultDecimal = ISeq.sumBy decimal decimalSeq
        if resultDecimal <> 38.03M then Assert.Fail()      

        // empty float32 ISeq
        let emptyFloatSeq = ISeq.empty<System.Single> 
        let resultEptFloat = ISeq.sumBy float32 emptyFloatSeq 
        if resultEptFloat <> 0.0f then Assert.Fail()
        
        // empty double ISeq
        let emptyDoubleSeq = ISeq.empty<System.Double> 
        let resultDouEmp = ISeq.sumBy double emptyDoubleSeq 
        if resultDouEmp <> 0.0 then Assert.Fail()
        
        // empty decimal ISeq
        let emptyDecimalSeq = ISeq.empty<System.Decimal> 
        let resultDecEmp = ISeq.sumBy decimal emptyDecimalSeq 
        if resultDecEmp <> 0M then Assert.Fail()
       
        ()
        
    [<Test>]
    member this.Take() =
        // integer ISeq
        
        let resultInt = ISeq.take 3 (iseq [1;2;4;5;7])
       
        let expectedInt = iseq [1;2;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
       
        let resultStr =ISeq.take 2(iseq ["str1";"str2";"str3";"str4"])
     
        let expectedStr = iseq ["str1";"str2"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.take 0 ISeq.empty 
      
        VerifySeqsEqual resultEpt ISeq.empty
        
         
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.take 1 null |> ignore)
        ()
        
    [<Test>]
    member this.takeWhile() =
        // integer ISeq
        let funcInt x = (x < 6)
        let resultInt = ISeq.takeWhile funcInt (iseq [1;2;4;5;6;7])
      
        let expectedInt = iseq [1;2;4;5]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let funcStr (x:string) = (x.Length < 4)
        let resultStr =ISeq.takeWhile funcStr (iseq ["a"; "ab"; "abc"; "abcd"; "abcde"])
      
        let expectedStr = iseq ["a"; "ab"; "abc"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.takeWhile funcInt ISeq.empty 
        VerifySeqsEqual resultEpt ISeq.empty
        
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.takeWhile funcInt null |> ignore)
        ()
        
    [<Test>]
    member this.ToArray() =
        // integer ISeq
        let resultInt = ISeq.toArray(iseq [1;2;4;5;7])
     
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)

        // string ISeq
        let resultStr =ISeq.toArray (iseq ["str1";"str2";"str3"])
    
        let expectedStr =  [|"str1";"str2";"str3"|]
        Assert.AreEqual(expectedStr,resultStr)
        
        // empty ISeq 
        let resultEpt = ISeq.toArray ISeq.empty 
        Assert.AreEqual([||],resultEpt)
        
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.toArray null |> ignore)
        ()
        
    [<Test>]    
    member this.ToArrayFromICollection() =
        let inputCollection = ResizeArray(iseq [1;2;4;5;7])
        let resultInt = ISeq.toArray((iseq inputCollection))
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        
    
    [<Test>]    
    member this.ToArrayEmptyInput() =
        let resultInt = ISeq.toArray(ISeq.empty<int>)
        let expectedInt = Array.empty<int>
        Assert.AreEqual(expectedInt,resultInt)        

    [<Test>]    
    member this.ToArrayFromArray() =
        let resultInt = ISeq.toArray((iseq [|1;2;4;5;7|]))
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        
    
    [<Test>]    
    member this.ToArrayFromList() =
        let resultInt = ISeq.toArray((iseq [1;2;4;5;7]))
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        

    [<Test>]
    member this.ToList() =
        // integer ISeq
        let resultInt = ISeq.toList (iseq [1;2;4;5;7])
        let expectedInt = [1;2;4;5;7]
        Assert.AreEqual(expectedInt,resultInt)
        
        // string ISeq
        let resultStr =ISeq.toList (iseq ["str1";"str2";"str3"])
        let expectedStr =  ["str1";"str2";"str3"]
        Assert.AreEqual(expectedStr,resultStr)
        
        // empty ISeq 
        let resultEpt = ISeq.toList ISeq.empty 
        Assert.AreEqual([],resultEpt)
         
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.toList null |> ignore)
        ()
        
    [<Test>]
    member this.Truncate() =
        // integer ISeq
        let resultInt = ISeq.truncate 3 (iseq [1;2;4;5;7])
        let expectedInt = [1;2;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let resultStr =ISeq.truncate 2 (iseq ["str1";"str2";"str3"])
        let expectedStr =  ["str1";"str2"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty ISeq 
        let resultEpt = ISeq.truncate 0 ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt
        
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.truncate 1 null |> ignore)

        // negative count
        VerifySeqsEqual ISeq.empty <| ISeq.truncate -1 (iseq [1;2;4;5;7])
        VerifySeqsEqual ISeq.empty <| ISeq.truncate System.Int32.MinValue (iseq [1;2;4;5;7])

        ()
        
    [<Test>]
    member this.tryFind() =
        // integer ISeq
        let resultInt = ISeq.tryFind (fun x -> (x%2=0)) (iseq [1;2;4;5;7])
        Assert.AreEqual(Some(2), resultInt)
        
         // integer ISeq - None
        let resultInt = ISeq.tryFind (fun x -> (x%2=0)) (iseq [1;3;5;7])
        Assert.AreEqual(None, resultInt)
        
        // string ISeq
        let resultStr = ISeq.tryFind (fun (x:string) -> x.Contains("2")) (iseq ["str1";"str2";"str3"])
        Assert.AreEqual(Some("str2"),resultStr)
        
         // string ISeq - None
        let resultStr = ISeq.tryFind (fun (x:string) -> x.Contains("2")) (iseq ["str1";"str4";"str3"])
        Assert.AreEqual(None,resultStr)
       
        
        // empty ISeq 
        let resultEpt = ISeq.tryFind (fun x -> (x%2=0)) ISeq.empty
        Assert.AreEqual(None,resultEpt)

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.tryFind (fun x -> (x%2=0))  null |> ignore)
        ()
        
    [<Test>]
    member this.TryFindBack() =
        // integer ISeq
        let resultInt = ISeq.tryFindBack (fun x -> (x%2=0)) (iseq [1;2;4;5;7])
        Assert.AreEqual(Some 4, resultInt)

        // integer ISeq - None
        let resultInt = ISeq.tryFindBack (fun x -> (x%2=0)) (iseq [1;3;5;7])
        Assert.AreEqual(None, resultInt)

        // string ISeq
        let resultStr = ISeq.tryFindBack (fun (x:string) -> x.Contains("2")) (iseq ["str1";"str2";"str2x";"str3"])
        Assert.AreEqual(Some "str2x", resultStr)

        // string ISeq - None
        let resultStr = ISeq.tryFindBack (fun (x:string) -> x.Contains("2")) (iseq ["str1";"str4";"str3"])
        Assert.AreEqual(None, resultStr)

        // empty ISeq
        let resultEpt = ISeq.tryFindBack (fun x -> (x%2=0)) ISeq.empty
        Assert.AreEqual(None, resultEpt)

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.tryFindBack (fun x -> (x%2=0))  null |> ignore)
        ()

    [<Test>]
    member this.TryFindIndex() =

        // integer ISeq
        let resultInt = ISeq.tryFindIndex (fun x -> (x % 5 = 0)) (iseq [8; 9; 10])
        Assert.AreEqual(Some(2), resultInt)
        
         // integer ISeq - None
        let resultInt = ISeq.tryFindIndex (fun x -> (x % 5 = 0)) (iseq [9;3;11])
        Assert.AreEqual(None, resultInt)
        
        // string ISeq
        let resultStr = ISeq.tryFindIndex (fun (x:string) -> x.Contains("2")) (iseq ["str1"; "str2"; "str3"])
        Assert.AreEqual(Some(1),resultStr)
        
         // string ISeq - None
        let resultStr = ISeq.tryFindIndex (fun (x:string) -> x.Contains("2")) (iseq ["str1"; "str4"; "str3"])
        Assert.AreEqual(None,resultStr)
       
        
        // empty ISeq 
        let resultEpt = ISeq.tryFindIndex (fun x -> (x%2=0)) ISeq.empty
        Assert.AreEqual(None, resultEpt)
        
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.tryFindIndex (fun x -> (x % 2 = 0))  null |> ignore)
        ()
        
    [<Test>]
    member this.TryFindIndexBack() =

        // integer ISeq
        let resultInt = ISeq.tryFindIndexBack (fun x -> (x % 5 = 0)) (iseq [5; 9; 10; 12])
        Assert.AreEqual(Some(2), resultInt)

        // integer ISeq - None
        let resultInt = ISeq.tryFindIndexBack (fun x -> (x % 5 = 0)) (iseq [9;3;11])
        Assert.AreEqual(None, resultInt)

        // string ISeq
        let resultStr = ISeq.tryFindIndexBack (fun (x:string) -> x.Contains("2")) (iseq ["str1"; "str2"; "str2x"; "str3"])
        Assert.AreEqual(Some(2), resultStr)

        // string ISeq - None
        let resultStr = ISeq.tryFindIndexBack (fun (x:string) -> x.Contains("2")) (iseq ["str1"; "str4"; "str3"])
        Assert.AreEqual(None, resultStr)

        // empty ISeq
        let resultEpt = ISeq.tryFindIndexBack (fun x -> (x%2=0)) ISeq.empty
        Assert.AreEqual(None, resultEpt)

        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.tryFindIndexBack (fun x -> (x % 2 = 0))  null |> ignore)
        ()

    [<Test>]
    member this.Unfold() =
        // integer ISeq
        
        let resultInt = ISeq.unfold (fun x -> if x = 1 then Some(7,2) else  None) 1
        
        VerifySeqsEqual (iseq [7]) resultInt
          
        // string ISeq
        let resultStr =ISeq.unfold (fun (x:string) -> if x.Contains("unfold") then Some("a","b") else None) "unfold"
        VerifySeqsEqual (iseq ["a"]) resultStr
        ()
        
        
    [<Test>]
    member this.Windowed() =

        let testWindowed config =
            try
                config.InputSeq
                |> ISeq.windowed config.WindowSize
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
          ExpectedSeq =  ISeq.empty
          Exception = None
        } |> testWindowed
        {
          InputSeq = iseq ["str1";"str2";"str3";"str4"]
          WindowSize = 2
          ExpectedSeq =  iseq [ [|"str1";"str2"|];[|"str2";"str3"|];[|"str3";"str4"|]]
          Exception = None
        } |> testWindowed
        {
          InputSeq = ISeq.empty
          WindowSize = 2
          ExpectedSeq = ISeq.empty
          Exception = None
        } |> testWindowed
        //{
        //  InputSeq = null
        //  WindowSize = 2
        //  ExpectedSeq = ISeq.empty
        //  Exception = Some typeof<ArgumentNullException>
        //} |> testWindowed
        {
          InputSeq = iseq [1..10]
          WindowSize = 0
          ExpectedSeq =  ISeq.empty
          Exception = Some typeof<ArgumentException>
        } |> testWindowed

        ()
        
    [<Test>]
    member this.Zip() =
    
        // integer ISeq
        let resultInt = ISeq.zip (iseq [1..7]) (iseq [11..17])
        let expectedInt = 
            iseq <| seq { for i in 1..7 do
                            yield i, i+10 }
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let resultStr =ISeq.zip (iseq ["str3";"str4"]) (iseq ["str1";"str2"])
        let expectedStr = iseq ["str3","str1";"str4","str2"]
        VerifySeqsEqual expectedStr resultStr
      
        // empty ISeq 
        let resultEpt = ISeq.zip ISeq.empty ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt
          
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.zip null null |> ignore)
        //CheckThrowsArgumentNullException(fun() -> ISeq.zip null (iseq [1..7]) |> ignore)
        //CheckThrowsArgumentNullException(fun() -> ISeq.zip (iseq [1..7]) null |> ignore)
        ()
        
    [<Test>]
    member this.Zip3() =
        // integer ISeq
        let resultInt = ISeq.zip3 (iseq [1..7]) (iseq [11..17]) (iseq [21..27])
        let expectedInt = 
            iseq <| seq { for i in 1..7 do
                            yield i, (i + 10), (i + 20) }
        VerifySeqsEqual expectedInt resultInt
        
        // string ISeq
        let resultStr =ISeq.zip3 (iseq ["str1";"str2"]) (iseq ["str11";"str12"]) (iseq ["str21";"str22"])
        let expectedStr = iseq ["str1","str11","str21";"str2","str12","str22" ]
        VerifySeqsEqual expectedStr resultStr
      
        // empty ISeq 
        let resultEpt = ISeq.zip3 ISeq.empty ISeq.empty ISeq.empty
        VerifySeqsEqual ISeq.empty resultEpt
          
        //// null ISeq
        //CheckThrowsArgumentNullException(fun() -> ISeq.zip3 null null null |> ignore)
        //CheckThrowsArgumentNullException(fun() -> ISeq.zip3 null (iseq [1..7]) (iseq [1..7]) |> ignore)
        //CheckThrowsArgumentNullException(fun() -> ISeq.zip3 (iseq [1..7]) null (iseq [1..7]) |> ignore)
        //CheckThrowsArgumentNullException(fun() -> ISeq.zip3 (iseq [1..7]) (iseq [1..7]) null |> ignore)
        ()
        
    [<Test>]
    member this.tryPick() =
         // integer ISeq
        let resultInt = ISeq.tryPick (fun x-> if x = 1 then Some("got") else None) (iseq [1..5])
         
        Assert.AreEqual(Some("got"),resultInt)
        
        // string ISeq
        let resultStr = ISeq.tryPick (fun x-> if x = "Are" then Some("got") else None) (iseq ["Lists"; "Are"])
        Assert.AreEqual(Some("got"),resultStr)
        
        // empty ISeq   
        let resultEpt = ISeq.tryPick (fun x-> if x = 1 then Some("got") else None) ISeq.empty
        Assert.IsNull(resultEpt)
       
        //// null ISeq
        //let nullSeq : iseq<'a> = null 
        //let funcNull x = Some(1)
        
        //CheckThrowsArgumentNullException(fun () -> ISeq.tryPick funcNull nullSeq |> ignore)
   
        ()

    [<Test>]
    member this.tryItem() =
        // integer ISeq
        let resultInt = ISeq.tryItem 3 (iseq { 10..20 })
        Assert.AreEqual(Some(13), resultInt)

        // string ISeq
        let resultStr = ISeq.tryItem 2 (iseq ["Lists"; "Are"; "Cool"; "List" ])
        Assert.AreEqual(Some("Cool"), resultStr)

        // empty ISeq
        let resultEmpty = ISeq.tryItem 0 ISeq.empty
        Assert.AreEqual(None, resultEmpty)

        //// null ISeq
        //let nullSeq:iseq<'a> = null
        //CheckThrowsArgumentNullException (fun () -> ISeq.tryItem 3 nullSeq |> ignore)

        // Negative index
        let resultNegativeIndex = ISeq.tryItem -1 (iseq { 10..20 })
        Assert.AreEqual(None, resultNegativeIndex)

        // Index greater than length
        let resultIndexGreater = ISeq.tryItem 31 (iseq { 10..20 })
        Assert.AreEqual(None, resultIndexGreater)
