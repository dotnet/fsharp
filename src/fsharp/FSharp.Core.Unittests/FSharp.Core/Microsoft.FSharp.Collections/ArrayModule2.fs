// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Collections.Array module

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections

open System
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework

(*
[Test Strategy]
Make sure each method works on:
* Integer array (value type)
* String  array (reference type)
* Empty   array (0 elements)
* Null    array (null)
*)

[<TestFixture>]
type ArrayModule2() =

    [<Test>]
    member this.Length() =
        // integer array  
        let resultInt = Array.length [|1..8|]
        if resultInt <> 8 then Assert.Fail()
        
        // string array    
        let resultStr = Array.length [|"Lists"; "are";  "commonly" ; "list" |]
        if resultStr <> 4 then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.length [| |]
        if resultEpt <> 0 then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsNullRefException (fun () -> Array.length  nullArr  |> ignore)  
        
        ()

    [<Test>]
    member this.Map() = 
        // integer array
        let funcInt x = 
                match x with
                | _ when x % 2 = 0 -> 10*x            
                | _ -> x
        let resultInt = Array.map funcInt [| 1..10 |]
        if resultInt <> [|1;20;3;40;5;60;7;80;9;100|] then Assert.Fail()
        
        // string array
        let funcStr (x:string) = x.ToLower()
        let resultStr = Array.map funcStr [|"Lists"; "Are";  "Commonly" ; "List" |]
        if resultStr <> [|"lists"; "are";  "commonly" ; "list" |] then Assert.Fail()
        
        // empty array
        let resultEpt = Array.map funcInt [| |]
        if resultEpt <> [| |] then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.map funcStr nullArr |> ignore)
        
        ()

    [<Test>]
    member this.Map2() = 
        // integer array 
        let funcInt x y = x+y
        let resultInt = Array.map2 funcInt [|1..10|] [|2..2..20|]
        if resultInt <> [|3;6;9;12;15;18;21;24;27;30|] then Assert.Fail()
        
        // string array
        let funcStr (x:int) (y:string) =  x+ y.Length
        let resultStr = Array.map2 funcStr [|3;6;9;11|] [|"Lists"; "Are";  "Commonly" ; "List" |]
        if resultStr <> [|8;9;17;15|] then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.map2 funcInt emptyArr emptyArr
        if resultEpt <> [| |] then Assert.Fail()

        // null array
        let nullArr = null:int[]
        let validArray = [| 1 |]       
        CheckThrowsArgumentNullException (fun () -> Array.map2 funcInt nullArr validArray |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Array.map2 funcInt validArray nullArr |> ignore)  
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.map2 funcInt [|1..10|] [|2..20|] |> ignore)
        
        ()

    [<Test>]
    member this.Mapi() = 
        // integer array 
        let funcInt x y = x+y
        let resultInt = Array.mapi funcInt [|10..2..20|]
        if resultInt <> [|10;13;16;19;22;25|] then Assert.Fail()
        
        // string array
        let funcStr (x:int) (y:string) =  x+ y.Length
        let resultStr = Array.mapi funcStr  [|"Lists"; "Are";  "Commonly" ; "List" |]
        if resultStr <> [|5;4;10;7|] then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.mapi funcInt emptyArr 
        if resultEpt <> [| |] then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.mapi funcStr nullArr |> ignore)  
        
        ()

    [<Test>]
    member this.mapi2() = 
        // integer array 
        let funcInt x y z = x+y+z
        let resultInt = Array.mapi2 funcInt [|1..10|] [|2..2..20|]
        if resultInt <> [|3;7;11;15;19;23;27;31;35;39|] then Assert.Fail()
        
        // string array
        let funcStr  z (x:int) (y:string)  =z + x+ y.Length 
        let resultStr = Array.mapi2 funcStr [|3;6;9;11|] [|"Lists"; "Are";  "Commonly" ; "List" |]
        if resultStr <> [|8;10;19;18|] then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        let resultEpt = Array.mapi2 funcInt emptyArr emptyArr
        if resultEpt <> [| |] then Assert.Fail()

        // null array
        let nullArr = null:int[] 
        let validArray = [| 1 |]      
        CheckThrowsArgumentNullException (fun () -> Array.mapi2 funcInt validArray  nullArr  |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Array.mapi2 funcInt  nullArr validArray |> ignore)  
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.mapi2 funcInt [|1..10|] [|2..20|] |> ignore)
        
        ()

    [<Test>]
    member this.Max() = 
        // integer array 
        let resultInt = Array.max  [|2..2..20|]
        if resultInt <> 20 then Assert.Fail()
        
        // string array
        let resultStr = Array.max [|"t"; "ahe"; "Lists"; "Are";  "Commonly" ; "List";"a" |]
        if resultStr <> "t" then Assert.Fail()
        
        // empty array -- argumentexception   
        
        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.max   nullArr  |> ignore)  
        
        // len = 0
        CheckThrowsArgumentException(fun() -> Array.max  [||] |> ignore)
        
        ()

    [<Test>]
    member this.MaxBy()= 
        // integer array 
        let funcInt x = x%8
        let resultInt = Array.maxBy funcInt [|2..2..20|]
        if resultInt <> 6 then Assert.Fail()
        
        // string array
        let funcStr (x:string) = x.Length 
        let resultStr = Array.maxBy funcStr  [|"Lists"; "Are";  "Commonly" ; "List"|]
        if resultStr <> "Commonly" then Assert.Fail()    
        
        // empty array -- argumentexception    

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.maxBy funcStr   nullArr  |> ignore)  
        
        // len = 0
        CheckThrowsArgumentException(fun() -> Array.maxBy funcInt (Array.empty<int>) |> ignore)
        
        ()

    [<Test>]
    member this.Min() =
        // integer array 
        let resultInt = Array.min  [|3;7;8;9;4;1;1;2|]
        if resultInt <> 1 then Assert.Fail()
        
        // string array
        let resultStr = Array.min [|"a"; "Lists";  "Commonly" ; "List"  |] 
        if resultStr <> "Commonly" then Assert.Fail()
        
        // empty array -- argumentexception   
        
        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.min   nullArr  |> ignore)  
        
        // len = 0
        CheckThrowsArgumentException(fun () -> Array.min  [||] |> ignore)
        
        () 

    [<Test>]
    member this.MinBy()= 
        // integer array 
        let funcInt x = x%8
        let resultInt = Array.minBy funcInt [|3;7;9;4;8;1;1;2|]
        if resultInt <> 8 then Assert.Fail()
        
        // string array
        let funcStr (x:string) = x.Length 
        let resultStr = Array.minBy funcStr  [|"Lists"; "Are";  "Commonly" ; "List"|]
        if resultStr <> "Are" then Assert.Fail()    
        
        // empty array -- argumentexception    

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.minBy funcStr   nullArr  |> ignore)  
        
        // len = 0
        CheckThrowsArgumentException(fun () -> Array.minBy funcInt (Array.empty<int>) |> ignore)
        
        ()
        

    [<Test>]
    member this.Of_List() =
        // integer array  
        let resultInt = Array.ofList [1..10]
        if resultInt <> [|1..10|] then Assert.Fail()
        
        // string array    
        let resultStr = Array.ofList ["Lists"; "are";  "commonly" ; "list" ]
        if resultStr <> [| "Lists"; "are";  "commonly" ; "list" |] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.ofList []
        if resultEpt <> [||] then Assert.Fail()

        // null array
        
        ()

    [<Test>]
    member this.Of_Seq() =
        // integer array  
        let resultInt = Array.ofSeq {1..10}
        if resultInt <> [|1..10|] then Assert.Fail()
        
        // string array    
        let resultStr = Array.ofSeq (seq {for x in 'a'..'f' -> x.ToString()})
        if resultStr <> [| "a";"b";"c";"d";"e";"f" |] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.ofSeq []
        if resultEpt <> [| |] then Assert.Fail()

        // null array
        
        ()

    [<Test>]
    member this.Partition() =
        // integer array  
        let resultInt = Array.partition (fun x -> x%3 = 0) [|1..10|]
        if resultInt <> ([|3;6;9|], [|1;2;4;5;7;8;10|]) then Assert.Fail()
        
        // string array    
        let resultStr = Array.partition (fun (x:string) -> x.Length >4) [|"Lists"; "are";  "commonly" ; "list" |]
        if resultStr <> ([|"Lists";"commonly"|],[|"are"; "list"|]) then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.partition (fun x -> x%3 = 0) [||]
        if resultEpt <> ([||],[||]) then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.partition (fun (x:string) -> x.Length >4)  nullArr  |> ignore)  
        
        ()

    [<Test>]
    member this.Permute() =
        // integer array  
        let resultInt = Array.permute (fun i -> (i+1) % 4) [|1;2;3;4|]
        if resultInt <> [|4;1;2;3|] then Assert.Fail()
        
        // string array    
        let resultStr = Array.permute (fun i -> (i+1) % 4) [|"Lists"; "are";  "commonly" ; "list" |]
        if resultStr <> [|"list";"Lists"; "are";  "commonly" |] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.permute (fun i -> (i+1) % 4) [||]
        if resultEpt <> [||] then Assert.Fail()
    
        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.permute (fun i -> (i+1) % 4)  nullArr  |> ignore)   
        
        ()

    [<Test>]
    member this.Reduce() =
        // integer array  
        let resultInt = Array.reduce (fun x y -> x/y) [|5*4*3*2; 4;3;2;1|]
        if resultInt <> 5 then Assert.Fail()
        
        // string array    
        let resultStr = Array.reduce (fun (x:string) (y:string) -> x.Remove(0,y.Length)) [|"ABCDE";"A"; "B";  "C" ; "D" |]
        if resultStr <> "E" then  Assert.Fail()
        
        // empty array 
        CheckThrowsArgumentException (fun () -> Array.reduce (fun x y -> x/y)  [||] |> ignore)

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.reduce (fun (x:string) (y:string) -> x.Remove(0,y.Length))  nullArr  |> ignore)   
        
        ()

        
    [<Test>]
    member this.ReduceBack() =
        // integer array  
        let resultInt = Array.reduceBack (fun x y -> x/y) [|5*4*3*2; 4;3;2;1|]
        if resultInt <> 30 then Assert.Fail()
        
        // string array    
        let resultStr = Array.reduceBack (fun (x:string) (y:string) -> x.Remove(0,y.Length)) [|"ABCDE";"A"; "B";  "C" ; "D" |]
        if resultStr <> "ABCDE" then  Assert.Fail()
        
        // empty array 
        CheckThrowsArgumentException (fun () -> Array.reduceBack (fun x y -> x/y) [||] |> ignore)

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.reduceBack (fun (x:string) (y:string) -> x.Remove(0,y.Length))  nullArr  |> ignore)   
        
        ()
    

    [<Test>]
    member this.Rev() =
        // integer array  
        let resultInt = Array.rev  [|1..10|]
        if resultInt <> [|10;9;8;7;6;5;4;3;2;1|] then Assert.Fail()
        
        // string array    
        let resultStr = Array.rev  [|"Lists"; "are";  "commonly" ; "list" |]
        if resultStr <> [|"list"; "commonly"; "are"; "Lists" |] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.rev  [||]
        if resultEpt <> [||] then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.rev  nullArr  |> ignore) 
        ()

    [<Test>] 
    member this.Scan() =
        // integer array
        let funcInt x y = x+y
        let resultInt = Array.scan funcInt 9 [| 1..10 |]
        if resultInt <> [|9;10;12;15;19;24;30;37;45;54;64|] then Assert.Fail()
        
        // string array
        let funcStr x y = x+y        
        let resultStr = Array.scan funcStr "x" [|"A"; "B";  "C" ; "D" |]
        if resultStr <> [|"x";"xA";"xAB";"xABC";"xABCD"|] then Assert.Fail()
        
        // empty array
        let resultEpt = Array.scan funcInt 5 [| |]
        if resultEpt <> [|5|] then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.scan funcStr "begin"  nullArr  |> ignore)  
        
        ()   
    
    [<Test>]
    member this.ScanBack() =
        // integer array 
        let funcInt x y = x+y
        let resultInt = Array.scanBack funcInt [| 1..10 |] 9
        if resultInt <> [|64;63;61;58;54;49;43;36;28;19;9|] then Assert.Fail()
        
        // string array
        let funcStr x y = x+y        
        let resultStr = Array.scanBack funcStr [|"A"; "B";  "C" ; "D" |] "X" 
        if resultStr <> [|"ABCDX";"BCDX";"CDX";"DX";"X"|] then Assert.Fail()
        
        // empty array
        let resultEpt = Array.scanBack funcInt [| |] 5 
        if resultEpt <> [|5|] then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.scanBack funcStr nullArr "begin"  |> ignore) 
        
        ()

    [<Test>]
    member this.Set() =
        // integer array  
        let intArr = [|10;9;8;7|]
        Array.set intArr  3 600
        if intArr <> [|10;9;8;600|] then Assert.Fail()  
        
        // string array
        let strArr = [|"Lists"; "are";  "commonly" ; "list" |]    
        Array.set strArr 2 "always"
        if strArr <> [|"Lists"; "are";  "always" ; "list" |]     then Assert.Fail()
        
        // empty array -- outofbundaryexception
        
        // null array
        let nullArr = null:string[]      
        CheckThrowsNullRefException (fun () -> Array.set nullArr 0 "null"   |> ignore)
        
        ()    

    [<Test>]
    member this.sortInPlaceWith() =
        // integer array  
        let intArr = [|3;5;7;2;4;8|]
        Array.sortInPlaceWith compare intArr  
        if intArr <> [|2;3;4;5;7;8|] then Assert.Fail()  

        // Sort backwards
        let intArr = [|3;5;7;2;4;8|]
        Array.sortInPlaceWith (fun a b -> -1 * compare a b) intArr  
        if intArr <> [|8;7;5;4;3;2|] then Assert.Fail()  
        
        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "used"; "data"; "structure"|]    
        Array.sortInPlaceWith compare strArr 
        if strArr <> [| "Lists"; "a"; "are"; "commonly"; "data"; "structure"; "used"|]     then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        Array.sortInPlaceWith compare emptyArr
        if emptyArr <> [||] then Assert.Fail()
        
        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.sortInPlaceWith compare nullArr  |> ignore)  
        
        // len = 2  
        let len2Arr = [|8;3|]      
        Array.sortInPlaceWith compare len2Arr
        Assert.AreEqual([|3;8|], len2Arr)
        
        // Equal elements
        let eights = [|8; 8;8|]      
        Array.sortInPlaceWith compare eights
        Assert.AreEqual([|8;8;8|], eights)
        
        ()   
        

    [<Test>]
    member this.sortInPlaceBy() =
        // integer array  
        let intArr = [|3;5;7;2;4;8|]
        Array.sortInPlaceBy int intArr  
        if intArr <> [|2;3;4;5;7;8|] then Assert.Fail()  
        
        // string array
        let strArr = [|"Lists"; "are"; "a"; "commonly"; "used"; "data"; "structure"|]    
        Array.sortInPlaceBy (fun (x:string) -> x.Length)  strArr 
        // note: Array.sortInPlaceBy is not stable, so we allow 2 results.
        if strArr <> [| "a"; "are";"data"; "used";"Lists"; "commonly";"structure"|] && strArr <> [| "a"; "are"; "used"; "data"; "Lists"; "commonly";"structure"|]    then Assert.Fail()
        
        // empty array
        let emptyArr:int[] = [| |]
        Array.sortInPlaceBy int emptyArr
        if emptyArr <> [||] then Assert.Fail()
        
        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.sortInPlaceBy (fun (x:string) -> x.Length) nullArr |> ignore)  
        
        // len = 2  
        let len2Arr = [|8;3|]      
        Array.sortInPlaceBy int len2Arr
        if len2Arr <> [|3;8|] then Assert.Fail()  
        Assert.AreEqual([|3;8|],len2Arr)  
        
        ()  

    [<Test>]
    member this.Sub() =
        // integer array  
        let resultInt = Array.sub [|1..8|] 3 3
        if resultInt <> [|4;5;6|] then Assert.Fail()
        
        // string array    
        let resultStr = Array.sub [|"Lists"; "are";  "commonly" ; "list" |] 1 2
        if resultStr <> [|"are";  "commonly" |] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.sub [| |] 0 0
        if resultEpt <> [||] then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.sub nullArr 1 1 |> ignore)  
        
        // bounds
        CheckThrowsArgumentException (fun () -> Array.sub resultInt -1 2 |> ignore)
        CheckThrowsArgumentException (fun () -> Array.sub resultInt 1 -2 |> ignore)
        CheckThrowsArgumentException (fun () -> Array.sub resultInt 1 20 |> ignore)
        
        ()

    [<Test>]
    member this.Sum() =
        // empty integer array 
        let resultEptInt = Array.sum ([||]:int[]) 
        if resultEptInt <> 0 then Assert.Fail()    
        
        // empty float32 array
        let emptyFloatArray = Array.empty<System.Single> 
        let resultEptFloat = Array.sum emptyFloatArray 
        if resultEptFloat <> 0.0f then Assert.Fail()
        
        // empty double array
        let emptyDoubleArray = Array.empty<System.Double> 
        let resultDouEmp = Array.sum emptyDoubleArray 
        if resultDouEmp <> 0.0 then Assert.Fail()
        
        // empty decimal array
        let emptyDecimalArray = Array.empty<System.Decimal> 
        let resultDecEmp = Array.sum emptyDecimalArray 
        if resultDecEmp <> 0M then Assert.Fail()

        // integer array  
        let resultInt = Array.sum [|1..10|] 
        if resultInt <> 55 then Assert.Fail()  
        
        // float32 array
        let floatArray: float32[] = [| 1.1f; 1.1f; 1.1f |]
        let resultFloat = Array.sum floatArray
        if resultFloat < 3.3f - 0.001f || resultFloat > 3.3f + 0.001f then
            Assert.Fail()
        
        // double array
        let doubleArray: System.Double[] = [| 1.0; 8.0 |]
        let resultDouble = Array.sum doubleArray
        if resultDouble <> 9.0 then Assert.Fail()
        
        // decimal array
        let decimalArray: decimal[] = [| 0M; 19M; 19.03M |]
        let resultDecimal = Array.sum decimalArray
        if resultDecimal <> 38.03M then Assert.Fail()      
 
        // null array
        let nullArr = null:double[]    
        CheckThrowsArgumentNullException (fun () -> Array.sum  nullArr  |> ignore) 
        ()

    [<Test>]
    member this.SumBy() =
        // empty integer array         
        let resultEptInt = Array.sumBy int ([||]:int[]) 
        if resultEptInt <> 0 then Assert.Fail()    
        
        // empty float32 array
        let emptyFloatArray = Array.empty<System.Single> 
        let resultEptFloat = Array.sumBy float32 emptyFloatArray 
        if resultEptFloat <> 0.0f then Assert.Fail()
        
        // empty double array
        let emptyDoubleArray = Array.empty<System.Double> 
        let resultDouEmp = Array.sumBy float emptyDoubleArray 
        if resultDouEmp <> 0.0 then Assert.Fail()
        
        // empty decimal array
        let emptyDecimalArray = Array.empty<System.Decimal> 
        let resultDecEmp = Array.sumBy decimal emptyDecimalArray 
        if resultDecEmp <> 0M then Assert.Fail()

        // integer array  
        let resultInt = Array.sumBy int [|1..10|] 
        if resultInt <> 55 then Assert.Fail()  
        
        // float32 array
        let floatArray: string[] = [| "1.2";"3.5";"6.7" |]
        let resultFloat = Array.sumBy float32 floatArray
        if resultFloat <> 11.4f then Assert.Fail()
        
        // double array
        let doubleArray: System.Double[] = [| 1.0;8.0 |]
        let resultDouble = Array.sumBy float doubleArray
        if resultDouble <> 9.0 then Assert.Fail()
        
        // decimal array
        let decimalArray: decimal[] = [| 0M;19M;19.03M |]
        let resultDecimal = Array.sumBy decimal decimalArray
        if resultDecimal <> 38.03M then Assert.Fail()      
        
        // null array
        let nullArr = null:double[]    
        CheckThrowsArgumentNullException (fun () -> Array.sumBy float32  nullArr  |> ignore) 
        ()

    [<Test>]
    member this.To_List() =
        // integer array  
        let resultInt = Array.toList [|1..10|]
        if resultInt <> [1..10] then Assert.Fail()
        
        // string array    
        let resultStr = Array.toList [|"Lists"; "are";  "commonly" ; "list" |]
        if resultStr <> ["Lists"; "are";  "commonly" ; "list"] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.toList  [||]
        if resultEpt <> [] then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.toList   nullArr  |> ignore)  
        
        ()    
        
    [<Test>]
    member this.To_Seq() =
        // integer array  
        let resultInt = [|1..10|] |> Array.toSeq  |> Array.ofSeq
        if resultInt <> [|1..10|] then Assert.Fail()
        
        // string array    
        let resultStr = [|"Lists"; "are";  "commonly" ; "list" |] |> Array.toSeq |> Array.ofSeq
        if resultStr <> [|"Lists"; "are";  "commonly" ; "list" |] then Assert.Fail()
        
        // empty array     
        let resultEpt =[||] |> Array.toSeq  |> Array.ofSeq
        if resultEpt <> [||]  then Assert.Fail()

        // null array
        let nullArr = null:string[]  
        CheckThrowsArgumentNullException (fun () -> nullArr  |> Array.toSeq   |> ignore)  
        
        ()   

    [<Test>]
    member this.TryFind() =
        // integer array  
        let resultInt = [|1..10|] |> Array.tryFind (fun x -> x%7 = 0)  
        if resultInt <> Some 7 then Assert.Fail()
        
        // string array    
        let resultStr = [|"Lists"; "are";  "commonly" ; "list" |] |> Array.tryFind (fun (x:string) -> x.Length > 4)
        if resultStr <> Some "Lists" then Assert.Fail()
        
        // empty array     
        let resultEpt =[||] |> Array.tryFind  (fun x -> x%7 = 0)  
        if resultEpt <> None  then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.tryFind (fun (x:string) -> x.Length > 4)  nullArr  |> ignore)  
        
        ()
        
        
    [<Test>]
    member this.TryFindIndex() =
        // integer array  
        let resultInt = [|1..10|] |> Array.tryFindIndex (fun x -> x%7 = 0)  
        if resultInt <> Some 6 then Assert.Fail()
        
        // string array    
        let resultStr = [|"Lists"; "are";  "commonly" ; "list" |] |> Array.tryFindIndex (fun (x:string) -> x.Length > 4)
        if resultStr <> Some 0 then Assert.Fail()
        
        // empty array     
        let resultEpt =[||] |> Array.tryFindIndex  (fun x -> x % 7 = 0)  
        if resultEpt <> None  then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.tryFindIndex (fun (x:string) -> x.Length > 4)  nullArr  |> ignore)  
        
        ()

    [<Test>]
    member this.Unzip() =
        // integer array  
        let resultInt =  Array.unzip [|(1,2);(2,4);(3,6)|] 
        if resultInt <>  ([|1..3|], [|2..2..6|]) then Assert.Fail()
        
        // string array    
        let resultStr = Array.unzip [|("A","a");("B","b");("C","c");("D","d")|]
        let str = resultStr.ToString()
        if resultStr <> ([|"A"; "B";  "C" ; "D" |],[|"a";"b";"c";"d"|]) then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.unzip  [||]
        if resultEpt <> ([||],[||])  then Assert.Fail()

        // null array
        
        ()

    [<Test>]
    member this.Unzip3() =
        // integer array  
        let resultInt =  Array.unzip3 [|(1,2,3);(2,4,6);(3,6,9)|]
        if resultInt <> ([|1;2;3|], [|2;4;6|], [|3;6;9|]) then Assert.Fail()
        
        // string array    
        let resultStr = Array.unzip3 [|("A","1","a");("B","2","b");("C","3","c");("D","4","d")|]
        if resultStr <> ([|"A"; "B";  "C" ; "D" |], [|"1";"2";"3";"4"|], [|"a"; "b"; "c"; "d"|]) then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.unzip3  [||]
        if resultEpt <>  ([||], [||], [||]) then Assert.Fail()

        // null array
        
        ()

    [<Test>]
    member this.Zero_Create() =
        
        // Check for bogus input
        CheckThrowsArgumentException(fun () -> Array.zeroCreate -1 |> ignore)
        
        // integer array  
        let resultInt =  Array.zeroCreate 8 
        if resultInt <> [|0;0;0;0;0;0;0;0|] then Assert.Fail()
        
        // string array    
        let resultStr = Array.zeroCreate 3 
        if resultStr <> [|null;null;null|] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.zeroCreate  0
        if resultEpt <> [||]  then Assert.Fail()
        
        ()

    [<Test>]
    member this.BadCreateArguments() =
        // negative number
        CheckThrowsArgumentException (fun () -> Array.create -1 0 |> ignore)

    [<Test>]
    member this.Zip() =
        // integer array  
        let resultInt =  Array.zip [|1..3|] [|2..2..6|] 
        if resultInt <> [|(1,2);(2,4);(3,6)|] then Assert.Fail()
        
        // string array    
        let resultStr = Array.zip [|"A"; "B";  "C" ; "D" |] [|"a";"b";"c";"d"|]
        if resultStr <> [|("A","a");("B","b");("C","c");("D","d")|] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.zip  [||] [||]
        if resultEpt <> [||]  then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.zip nullArr   nullArr  |> ignore)  
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.zip [|1..10|] [|2..20|] |> ignore)
        
        ()

    [<Test>]
    member this.Zip3() =
        // integer array  
        let resultInt =  Array.zip3 [|1..3|] [|2..2..6|] [|3;6;9|]
        if resultInt <> [|(1,2,3);(2,4,6);(3,6,9)|] then Assert.Fail()
        
        // string array    
        let resultStr = Array.zip3 [|"A"; "B";  "C" ; "D" |]  [|"1";"2";"3";"4"|]  [|"a"; "b"; "c"; "d"|]
        let str = resultStr.ToString()
        if resultStr <> [|("A","1","a");("B","2","b");("C","3","c");("D","4","d")|] then Assert.Fail()
        
        // empty array     
        let resultEpt = Array.zip3  [||] [||] [||]
        if resultEpt <> [||]  then Assert.Fail()

        // null array
        let nullArr = null:string[]      
        CheckThrowsArgumentNullException (fun () -> Array.zip3 nullArr  nullArr  nullArr  |> ignore)  
        
        // len1 <> len2
        CheckThrowsArgumentException(fun () -> Array.zip3 [|1..10|] [|2..20|] [|1..10|] |> ignore)
        // len1 <> len3
        CheckThrowsArgumentException(fun () -> Array.zip3 [|1..10|] [|1..10|] [|2..20|] |> ignore)
        
        ()
