// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Collections.List module

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections

open System
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework

(*
[Test Strategy]
Make sure each method works on:
* Integer List (value type)
* String List (reference type)
* Empty List (0 elements)
*)

[<TestFixture>]
type ListModule() =
    [<Test>]
    member this.Empty() =
        let emptyList = List.empty
        let resultEpt = List.length emptyList
        Assert.AreEqual(0, resultEpt)   
        
        let c : int list   = List.empty<int>
        let d : string list = List.empty<string>
        
        ()

    [<Test>]
    member this.Append() =
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

    [<Test>]
    member this.Avarage() =     
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
        
        
    [<Test>]
    member this.AverageBy() =  
        // empty double List   
        let emptyDouList = List.empty<System.Double>
        CheckThrowsArgumentException (fun () ->  List.averageBy (fun x -> x + 6.7) emptyDouList |> ignore    )
        
        // empty float32 List
        let emptyFloat32List: float32 list = []
        CheckThrowsArgumentException (fun () -> List.averageBy (fun x -> x + 9.8f ) emptyFloat32List |> ignore)
        
        // empty decimal List
        let emptyDecimalList = List.empty<System.Decimal>
        CheckThrowsArgumentException (fun () -> List.averageBy (fun x -> x + 9.8M) emptyDecimalList |>ignore )

        // float32 List
        let floatList: float32 list = [ 1.2f;3.5f;6.7f ]      
        let averageOfFloat = List.averageBy (fun x -> x + 9.8f ) floatList        
        Assert.AreEqual(averageOfFloat, 13.5999994f)
        
        // double List
        let doubleList: System.Double list = [ 1.0;8.0 ]
        let averageOfDouble = List.averageBy (fun x -> x + 6.7) doubleList        
        Assert.AreEqual(11.2, averageOfDouble)
        
        // decimal List
        let decimalList: decimal list = [ 0M;19M;19.03M ]
        let averageOfDecimal = List.averageBy (fun x -> x + 9.8M) decimalList        
        Assert.AreEqual(22.476666666666666666666666667M, averageOfDecimal)
            
        ()


      
    [<Test>]
    member this.Choose() = 
        // int List
        let intSrc:int list = [ 1..100 ]    
        let funcInt x = if (x%5=0) then Some x else None       
        let intChosen = List.choose funcInt intSrc        
        Assert.AreEqual(5, intChosen.[0])
        Assert.AreEqual(10, intChosen.[1])
        Assert.AreEqual(15, intChosen.[2])
        
        // string List
        let stringSrc: string list = [ "List"; "this"; "is" ;"str"; "list" ]
        let funcString x = match x with
                           | "list" -> Some x
                           | "List" -> Some x
                           | _ -> None
        let strChosen = List.choose funcString stringSrc           
        Assert.AreEqual("list", strChosen.[0].ToLower())
        Assert.AreEqual("list", strChosen.[1].ToLower())
        
        // empty List
        let emptySrc :int list = [ ]
        let emptyChosen = List.choose funcInt emptySrc        
        Assert.AreEqual(emptySrc, emptyChosen)

        
        () 
    [<Test>]
    member this.Concat() =
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
    [<Test>]
    member this.Exists() =
        // integer List
        let intArr = [ 2;4;6;8 ]
        let funcInt x = if (x%2 = 0) then true else false
        let resultInt = List.exists funcInt intArr        
        Assert.IsTrue(resultInt)
        
        // string List
        let strArr = ["."; ".."; "..."; "...."] 
        let funcStr (x:string) = if (x.Length >15) then true else false
        let resultStr = List.exists funcStr strArr        
        Assert.IsFalse(resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = List.exists funcInt emptyArr        
        Assert.IsFalse(resultEpt)
               
        ()
    [<Test>]
    member this.Exists2() =
        // integer List
        let intFir = [ 2;4;6;8 ]
        let intSec = [ 1;2;3;4 ]
        let funcInt x y = if (x%y = 0) then true else false
        let resultInt = List.exists2 funcInt intFir intSec        
        Assert.IsTrue(resultInt)
        
        // string List
        let strFir = ["Lists"; "are";  "commonly" ]
        let strSec = ["good"; "good";  "good"  ]
        let funcStr (x:string) (y:string) = if (x = y) then true else false
        let resultStr = List.exists2 funcStr strFir strSec        
        Assert.IsFalse(resultStr)
        
        // empty List
        let eptFir:int list = [ ]
        let eptSec:int list = [ ]
        let resultEpt = List.exists2 funcInt eptFir eptSec        
        Assert.IsFalse(resultEpt)
        
        ()

    [<Test>]
    member this.Filter() =
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

    [<Test>]
    member this.Where() =
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
        let emptyArr:int list = [ ]
        let resultEpt = List.where funcInt emptyArr        
        Assert.AreEqual(emptyArr, resultEpt)
            
        ()   

    [<Test>]
    member this.``where should work like filter``() =
        Assert.AreEqual([], List.where (fun x -> x % 2 = 0) [])
        Assert.AreEqual([0;2;4;6;8], List.where (fun x -> x % 2 = 0) [0..9])
        Assert.AreEqual(["a";"b";"c"], List.where (fun _ -> true) ["a";"b";"c"])

        ()

    [<Test>]
    member this.Find() =
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
        CheckThrowsKeyNotFoundException (fun () -> List.find (fun x -> true) emptyArr |> ignore) 
                
        () 

    [<Test>]
    member this.FindIndex() =
        // integer List
        let intArr = [ 1..20 ]
        let funcInt x = if (x%5 = 0) then true else false
        let resultInt = List.findIndex funcInt intArr        
        Assert.AreEqual(4, resultInt)
        
        // string List
        let strArr = ["."; ".."; "..."; "...."] 
        let funcStr (x:string) = if (x.Length >2) then true else false
        let resultStr = List.findIndex funcStr strArr        
        Assert.AreEqual(2, resultStr)
        
        // empty List
        let emptyArr:int list = [ ]  
        CheckThrowsKeyNotFoundException (fun () -> List.findIndex (fun x -> true) emptyArr |> ignore)   
                
        () 
        
    [<Test>]
    member this.TryPick() =
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

    [<Test>]
    member this.Fold() =
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

    [<Test>]
    member this.Fold2() =
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

    [<Test>]
    member this.FoldBack() =
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

    [<Test>]
    member this.FoldBack2() =
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

    [<Test>]
    member this.ForAll() =
        // integer List
        let resultInt = List.forall (fun x -> x > 2) [ 3..2..10 ]        
        Assert.IsTrue(resultInt)
        
        // string List
        let resultStr = List.forall (fun (x:string) -> x.Contains("a")) ["a";"b";"c";"d"]        
        Assert.IsFalse(resultStr)
        
        // empty List        
        let resultEpt = List.forall (fun (x:string) -> x.Contains("a")) []         
        Assert.IsTrue(resultEpt)
        
        ()
        
    [<Test>]
    member this.ForAll2() =
        // integer List
        let resultInt = List.forall2 (fun x y -> x < y) [ 1..10 ] [2..2..20]        
        Assert.IsTrue(resultInt)
        
        // string List
        let resultStr = List.forall2 (fun (x:string) (y:string) -> x.Length > y.Length) ["a";"b";"c";"d"] ["A";"B";"C";"D"]          
        Assert.IsFalse(resultStr)
        
        // empty List 
        let resultEpt = List.forall2 (fun x y -> x > y) [] []        
        Assert.IsTrue(resultEpt)
        
        ()

    [<Test>]
    member this.Hd() =
        // integer List
        let resultInt = List.head  [2..2..20]        
        Assert.AreEqual(2, resultInt)
        
        // string List
        let resultStr = List.head  ["a";"b";"c";"d"]         
        Assert.AreEqual("a", resultStr)
            
        CheckThrowsArgumentException(fun () -> List.head [] |> ignore)
        ()    
        
        
    [<Test>]
    member this.Init() = 
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

    [<Test>]
    member this.IsEmpty() =
        // integer List
        let intArr = [ 3;4;7;8;10 ]    
        let resultInt = List.isEmpty intArr         
        Assert.IsFalse(resultInt)
        
        // string List
        let strArr = ["a";"b";"c";"d"]    
        let resultStr = List.isEmpty strArr         
        Assert.IsFalse(resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let resultEpt = List.isEmpty emptyArr         
        Assert.IsTrue(resultEpt)
        ()

    [<Test>]
    member this.Iter() =
        // integer List
        let intArr = [ 1..10 ]  
        let resultInt = ref 0    
        let funInt (x:int) =   
            resultInt := !resultInt + x              
            () 
        List.iter funInt intArr         
        Assert.AreEqual(55, !resultInt)
        
        // string List
        let strArr = ["a";"b";"c";"d"]
        let resultStr = ref ""
        let funStr (x:string) =
            resultStr := (!resultStr) + x   
            ()
        List.iter funStr strArr          
        Assert.AreEqual("abcd", !resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let resultEpt = ref 0
        List.iter funInt emptyArr         
        Assert.AreEqual(0, !resultEpt)
        
        ()
       
    [<Test>]
    member this.Iter2() =
        // integer List
        let resultInt = ref 0    
        let funInt (x:int) (y:int) =   
            resultInt := !resultInt + x + y             
            () 
        List.iter2 funInt [ 1..10 ] [2..2..20]         
        Assert.AreEqual(165, !resultInt)
        
        // string List
        let resultStr = ref ""
        let funStr (x:string) (y:string) =
            resultStr := (!resultStr) + x  + y 
            ()
        List.iter2 funStr ["a";"b";"c";"d"] ["A";"B";"C";"D"]          
        Assert.AreEqual("aAbBcCdD", !resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let resultEpt = ref 0
        List.iter2 funInt emptyArr emptyArr         
        Assert.AreEqual(0, !resultEpt)
        
        ()
        
    [<Test>]
    member this.Iteri() =
        // integer List
        let intArr = [ 1..10 ]  
        let resultInt = ref 0    
        let funInt (x:int) y =   
            resultInt := !resultInt + x + y             
            () 
        List.iteri funInt intArr         
        Assert.AreEqual(100, !resultInt)
        
        // string List
        let strArr = ["a";"b";"c";"d"]
        let resultStr = ref 0
        let funStr (x:int) (y:string) =
            resultStr := (!resultStr) + x + y.Length
            ()
        List.iteri funStr strArr          
        Assert.AreEqual(10, !resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let resultEpt = ref 0
        List.iteri funInt emptyArr         
        Assert.AreEqual(0, !resultEpt)
        
        ()
        
    [<Test>]
    member this.Iteri2() =
        // integer List
        let resultInt = ref 0    
        let funInt (x:int) (y:int) (z:int) =   
            resultInt := !resultInt + x + y + z            
            () 
        List.iteri2 funInt [ 1..10 ] [2..2..20]         
        Assert.AreEqual(210, !resultInt)
        
        // string List
        let resultStr = ref ""
        let funStr (x:int) (y:string) (z:string) =
            resultStr := (!resultStr) + x.ToString()  + y + z
            ()
        List.iteri2 funStr ["a";"b";"c";"d"] ["A";"B";"C";"D"]          
        Assert.AreEqual("0aA1bB2cC3dD", !resultStr)
        
        // empty List    
        let emptyArr:int list = [ ]
        let resultEpt = ref 0
        List.iteri2 funInt emptyArr emptyArr         
        Assert.AreEqual(0, !resultEpt)
        
        ()        

    [<Test>]
    member this.Contains() =
        // integer List
        let intList = [ 2;4;6;8 ]
        let resultInt = List.contains 4 intList
        Assert.IsTrue(resultInt)

        // string List
        let strList = ["."; ".."; "..."; "...."]
        let resultStr = List.contains "....." strList
        Assert.IsFalse(resultStr)

        // empty List
        let emptyList:int list = [ ]
        let resultEpt = List.contains 4 emptyList
        Assert.IsFalse(resultEpt)
