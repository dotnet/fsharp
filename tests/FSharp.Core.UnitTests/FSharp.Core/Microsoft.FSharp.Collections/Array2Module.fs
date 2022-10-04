// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Collections.Array2D module

namespace FSharp.Core.UnitTests.Collections

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit
open Utils

(*
[Test Strategy]
Make sure each method works on:
* Integer array (value type)
* String  array (reference type)
* Empty   array (0 elements)
* Null    array (null)
*)

type Array2Module() =

    let shouldBeEmpty arr = 
        if Array2D.length2 arr <> 0 
        && Array2D.length1 arr <> 0 then 
            Assert.Fail("Array2D is not empty.")

    [<Fact>]
    member this.Base1() =
        // integer array  
        let intArr = 
            Array2D.init 2 10
                (fun i j -> 
                    let arg = (System.Math.PI / 10.0) * float j 
                    if i = 0 then System.Math.Sin(arg) else System.Math.Cos(arg))
        let resultInt = Array2D.base1 intArr
        if resultInt <> 0 then Assert.Fail()

        // string array 
        let strArr = Array2D.createBased 0 0 2 3 "goodboy" 
        let resultStr = Array2D.base1 strArr
        if resultStr <> 0 then Assert.Fail()

        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.base1  eptArr
        if resultEpt <> 0  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsNullRefException (fun () -> Array2D.base1  nullArr |> ignore)   
        
        ()

    [<Fact>]
    member this.Base2() =
        // integer array  
        let intArr = 
            Array2D.init 2 10
                (fun i j -> 
                    let arg = (System.Math.PI / 10.0) * float j 
                    if i = 0 then System.Math.Sin(arg) else System.Math.Cos(arg))
        let resultInt = Array2D.base2 intArr
        if resultInt <> 0 then Assert.Fail()
        
        // string array 
        let strArr = Array2D.createBased 0 0 2 3 "goodboy" 
        let resultStr = Array2D.base2 strArr
        if resultStr <> 0 then Assert.Fail()

        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.base2  eptArr
        if resultEpt <> 0  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsNullRefException (fun () -> Array2D.base2  nullArr |> ignore)   

        // Verify printing format of non-zero based arrays
        let v : int[,] = Array2D.createBased 10 1 3 4 2
        let actual = (sprintf "%A" v).Replace("\r","").Replace("\n","")
        let expected = "[bound1=10 bound2=1 [2; 2; 2; 2] [2; 2; 2; 2] [2; 2; 2; 2]]"
        Assert.AreEqual(expected, actual)
        ()

    [<Fact>]
    member this.Blit() =
        // integer array  
        let intArr = 
            Array2D.init 2 3
                (fun i j ->
                    let arg = (System.Math.PI / 10.0) * float j 
                    if i = 0 then System.Math.Sin(arg) else System.Math.Cos(arg))
        let intArr2 = Array2D.create 2 3 8.8
        let resultInt = Array2D.blit intArr 0 0 intArr2 0 0 2 2
        if intArr2.[1,1] <> 0.95105651629515353 then Assert.Fail()

        
        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> i.ToString() + "-" + j.ToString())
        let strArr2 = Array2D.create 2 3 ""    
        let resultStr = Array2D.blit strArr 0 0 strArr2 0 0 2 3
        if strArr2.[1,1] <> "1-1" then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.create 0 0 1
        let eptArr2 = Array2D.create 0 0 1
        let resultEpt = Array2D.blit eptArr 0 0 eptArr2 0 0 0 0 
        if eptArr2   <> eptArr  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsArgumentNullException (fun () -> Array2D.blit nullArr 0 0 nullArr 0 0 0 0  |> ignore) 
        // src1 < 0
        CheckThrowsArgumentException(fun () -> Array2D.blit intArr -1 1 intArr2 1 1 2 2 |> ignore) 
        // src2 < 0
        CheckThrowsArgumentException(fun () -> Array2D.blit intArr 1 -1 intArr2 1 1 2 2 |> ignore) 
        // dest1 < 0
        CheckThrowsArgumentException(fun () -> Array2D.blit intArr 1 1 intArr2 -1 1 2 2 |> ignore) 
        // dest2 < 0
        CheckThrowsArgumentException(fun () -> Array2D.blit intArr 1 1 intArr2 1 -1 2 2 |> ignore)
        // src1 + len1 > Length1 src
        CheckThrowsArgumentException(fun () -> Array2D.blit intArr 10 0 intArr2 0 0 2 2 |> ignore) 
        // src2 + len2 > Length2 src
        CheckThrowsArgumentException(fun () ->Array2D.blit intArr 0 10 intArr2 0 0 2 2  |> ignore)  
        // dest1 + len1 > Length1 dest
        CheckThrowsArgumentException(fun () -> Array2D.blit  intArr 0 0 intArr2 10 0 2 2 |> ignore) 
        // dest2 + len2 > Length2 dest
        CheckThrowsArgumentException(fun () -> Array2D.blit  intArr 0 0 intArr2 0 10 2 2 |> ignore)  
        ()

    [<Fact>]
    member this.BlitWithNonZeroBase() =
        let a = Array2D.createBased 1 1 3 3 0
        a.[1,1] <- 11
        a.[2,2] <- 22
        a.[3,3] <- 33

        let b = Array2D.createBased 1 1 3 3 0
        Array2D.blit a 1 1 b 2 2 2 2
        let res = Array2D.createBased 1 1 3 3 0
        res.[2,2] <- 11
        res.[3,3] <- 22
        if b <> res then Assert.Fail()

        let b = Array2D.createBased 1 1 3 3 0
        Array2D.blit a 1 1 b 1 1 2 2
        let res = Array2D.createBased 1 1 3 3 0
        res.[1,1] <- 11
        res.[2,2] <- 22
        if b <> res then Assert.Fail()

        let b = Array2D.createBased 1 1 3 3 0
        Array2D.blit a 2 2 b 1 1 2 2
        let res = Array2D.createBased 1 1 3 3 0
        res.[1,1] <- 22
        res.[2,2] <- 33
        if b <> res then Assert.Fail()

        let b = Array2D.createBased 1 1 3 3 0
        Array2D.blit a 1 1 b 1 1 3 3
        let res = Array2D.createBased 1 1 3 3 0
        res.[1,1] <- 11
        res.[2,2] <- 22
        res.[3,3] <- 33
        if b <> res then Assert.Fail()

        let b = Array2D.createBased 1 1 3 3 0
        CheckThrowsArgumentException(fun () -> Array2D.blit a 1 1 b 3 3 2 2 |> ignore)

        let b = Array2D.createBased 1 1 3 3 0
        CheckThrowsArgumentException(fun () -> Array2D.blit a 1 1 b 0 0 2 2 |> ignore)

        let b = Array2D.createBased 1 1 3 3 0
        CheckThrowsArgumentException(fun () -> Array2D.blit a 0 0 b 1 1 2 2 |> ignore)

        let b = Array2D.createBased 1 1 3 3 0
        CheckThrowsArgumentException(fun () -> Array2D.blit a 3 3 b 1 1 2 2 |> ignore)

        let b = Array2D.createBased 1 1 3 3 0
        CheckThrowsArgumentException(fun () -> Array2D.blit a 1 1 b 1 1 4 4 |> ignore)

        ()

    [<Fact>]
    member this.Copy() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let resultInt = Array2D.copy intArr 
        if resultInt <> intArr then Assert.Fail()

        
        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> i.ToString() + "-" + j.ToString())
        let resultStr = Array2D.copy strArr
        if resultStr <> strArr then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.copy eptArr
        if resultEpt   <> eptArr  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsArgumentNullException (fun () -> Array2D.copy nullArr |> ignore)   
        
        ()          
    
    [<Fact>]
    member this.Create() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> 100)
        let resultInt = Array2D.create 2 3 100 
        if resultInt <> intArr then Assert.Fail()

        
        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> "goodboy")
        let resultStr = Array2D.create 2 3 "goodboy"
        if resultStr <> strArr then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.create 0 0 1
        if resultEpt   <> eptArr  then Assert.Fail()
  
        ()  

    [<Fact>]
    member this.createBased() =
        // integer array  
        let intArr = Array2D.create 2 3 100
        let resultInt = Array2D.createBased 0 0 2 3 100
        if resultInt <> intArr then Assert.Fail()

        
        // string array 
        let strArr = Array2D.create 2 3 "goodboy"
        let resultStr = Array2D.createBased 0 0 2 3 "goodboy"
        if resultStr <> strArr then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.createBased 0 0 0 0 1
        if resultEpt   <> eptArr  then Assert.Fail()
        () 

    [<Fact>]
    member this.Get() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let resultInt = intArr.[1,1]
        if resultInt <> 101 then Assert.Fail()

        
        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> i.ToString() + "-" + j.ToString())
        let resultStr = strArr.[1,1]
        if resultStr <> "1-1" then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsNullRefException (fun () -> nullArr.[2,2] |> ignore)
        ()
        
    [<Fact>]
    member this.GetAndSetAPI() =
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let resultInt = Array2D.get intArr 1 1
        Assert.AreEqual(101, resultInt)
        Array2D.set intArr 1 1 1
        let resultInt = Array2D.get intArr 1 1
        Assert.AreEqual(1, resultInt)
        ()
        
    [<Fact>]
    member this.Init() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        if intArr.[1,1] <> 101 then Assert.Fail()

        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> i.ToString() + "-" + j.ToString())
        if strArr.[1,1] <> "1-1" then Assert.Fail()
        () 

    [<Fact>]
    member this.Init_Based() =
        // integer array  
        let intArr = Array2D.initBased 1 1 2 3 (fun i j -> i*100 + j)
        if intArr.[2,2] <> 202 then Assert.Fail()

        // string array 
        let strArr = Array2D.initBased 1 1 2 3 (fun i j -> i.ToString() + "-" + j.ToString())
        if strArr.[2,2] <> "2-2" then Assert.Fail()        
        () 

    [<Fact>]
    member this.Iter() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let mutable resultInt = 0 
        let funInt (x:int) =   
            resultInt <- resultInt + x              
            () 
        Array2D.iter funInt intArr 
        if resultInt <> 306 then Assert.Fail()
        
        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> i.ToString() + "-" + j.ToString())
        let mutable resultStr = ""
        let funStr (x:string) =
            resultStr <- resultStr + x + ","  
            ()
        Array2D.iter funStr strArr  
        if resultStr <> "0-0,0-1,0-2,1-0,1-1,1-2," then Assert.Fail()
        
        // null array
        let nullArr = null:string[,]    
        CheckThrowsArgumentNullException (fun () -> Array2D.iter funStr nullArr |> ignore)   
        ()

    [<Fact>]
    member this.IterNonZeroBased() =
        let a = Array2D.createBased 1 5 10 10 1
        let mutable result = 0
        a |> Array2D.iter (fun n -> result <- result + n)
        if result <> 100 then Assert.Fail()
        result <- 0
        a |> Array2D.iteri (fun i j x -> result <- result + i + j + x)
        if result <> 1600 then Assert.Fail()
        ()

    [<Fact>]
    member this.Iteri() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let mutable resultInt = 0 
        let funInt (x:int) (y:int) (z:int) =   
            resultInt <- resultInt + x  + y  + z         
            () 
        Array2D.iteri funInt intArr 
        if resultInt <> 315 then Assert.Fail()
        
        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> i.ToString() + "-" + j.ToString())
        let mutable resultStr = ""
        let funStr (x:int) (y:int) (z:string) =
            resultStr <- resultStr + "[" + x.ToString() + "," + y.ToString() + "]" + "=" + z + "; "  
            ()
        Array2D.iteri funStr strArr  
        if resultStr <> "[0,0]=0-0; [0,1]=0-1; [0,2]=0-2; [1,0]=1-0; [1,1]=1-1; [1,2]=1-2; " then Assert.Fail()
            
        // null array
        let nullArr = null:string[,]    
        CheckThrowsArgumentNullException (fun () -> Array2D.iteri funStr nullArr |> ignore)   
        
        ()  

    [<Fact>]
    member this.Length1() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let resultInt = Array2D.length1 intArr 
        if resultInt <> 2 then Assert.Fail()
        
        // string array 
        let strArr = Array2D.init 10 3 (fun i j -> i.ToString() + "-" + j.ToString())
        let resultStr = Array2D.length1 strArr
        if resultStr <> 10 then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.length1 eptArr
        if resultEpt   <> 0  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsNullRefException (fun () -> Array2D.length1 nullArr |> ignore)   
        
        ()  

    [<Fact>]
    member this.Length2() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let resultInt = Array2D.length2 intArr 
        if resultInt <> 3 then Assert.Fail()

        
        // string array 
        let strArr = Array2D.init 2 8 (fun i j -> i.ToString() + "-" + j.ToString())
        let resultStr = Array2D.length2 strArr
        if resultStr <> 8 then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.length2 eptArr
        if resultEpt   <> 0  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsNullRefException (fun () -> Array2D.length2 nullArr |> ignore)   
        
        () 

    [<Fact>]
    member this.Map() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let funInt x = x.ToString()
        let resultInt = Array2D.map funInt intArr 
        if resultInt <> (Array2D.init 2 3 (fun i j -> (i*100 + j).ToString())) then Assert.Fail()

        
        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> "goodboy")
        let funStr (x:string) = x.ToUpper()
        let resultStr = Array2D.map funStr strArr
        if resultStr <> Array2D.create 2 3 "GOODBOY" then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.map funInt eptArr
        if resultEpt   <> Array2D.create 0 0 ""  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsArgumentNullException (fun () -> Array2D.map funStr nullArr |> ignore)   
        
        ()   

    [<Fact>]
    member this.Mapi() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        let funInt x y z = x+y+z
        let resultInt = Array2D.mapi funInt intArr 
        if resultInt <> (Array2D.init 2 3 (fun i j -> i*100 + j + i + j)) then Assert.Fail()

        
        // string array 
        let strArr = Array2D.init 2 3 (fun i j -> "goodboy")
        let funStr (x:int) (y:int) (z:string) = x.ToString() + y.ToString() + z.ToUpper()
        let resultStr = Array2D.mapi funStr strArr
        if resultStr <> Array2D.init 2 3 (fun i j -> i.ToString() + j.ToString() + "GOODBOY")  then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.create 0 0 1
        let resultEpt = Array2D.mapi funInt eptArr
        if resultEpt   <> Array2D.create 0 0 1  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsArgumentNullException (fun () -> Array2D.mapi funStr nullArr |> ignore)   
        
        () 

    [<Fact>]
    member this.Rebase() =
        // integer array  
        let intArr = Array2D.createBased 2 3 2 3 168
        let resultInt = Array2D.rebase  intArr 
        if resultInt <> Array2D.createBased 0 0 2 3 168 then Assert.Fail()

        
        // string array 
        let strArr = Array2D.createBased 2 3 2 3 "gorilla"
        let resultStr = Array2D.rebase  strArr
        if resultStr <> Array2D.createBased 0 0 2 3 "gorilla" then Assert.Fail()
        
        // empty array     
        let eptArr = Array2D.createBased 2 3 0 0 1
        let resultEpt = Array2D.rebase eptArr
        if resultEpt   <> Array2D.createBased 0 0 0 0 1  then Assert.Fail()

        // null array
        let nullArr = null:string[,]    
        CheckThrowsArgumentNullException (fun () -> Array2D.rebase  nullArr |> ignore)   
        
        ()
        
    [<Fact>]
    member this.Set() =
        // integer array  
        let intArr = Array2D.init 2 3 (fun i j -> i*100 + j)
        intArr.[1,1] <- 8888
        if intArr.[1,1] <> 8888 then Assert.Fail()

        
        // string array 
        let strArr = Array2D.init 2 8 (fun i j -> i.ToString() + "-" + j.ToString())
        strArr.[1,1] <- "grape"
        if strArr.[1,1] <> "grape" then Assert.Fail()
    
        // null array
        let nullArr = null:string[,]    
        CheckThrowsNullRefException (fun () -> (nullArr.[0,0] <- "") |> ignore)  
        
        () 
        
    [<Fact>]
    member this.ZeroCreate() =
        // integer array  
        let intArr = Array2D.zeroCreate 2 3 
        if intArr <> Array2D.create 2 3 0 then Assert.Fail()

        // string array 
        let strArr = Array2D.zeroCreate 2 3
        if strArr <> Array2D.create 2 3 null then Assert.Fail()
        
        // invalid arguments
        CheckThrowsArgumentException (fun () -> Array2D.zeroCreate -1 2 |> ignore)
        CheckThrowsArgumentException (fun () -> Array2D.zeroCreate 1 -2 |> ignore)
       
        () 

    // Note: This is a top level primitive, not in the Array2D module
    [<Fact>]
    member this.array2D() = 

        let m1 : int[,] = array2D []
        if m1.GetLength(0) <> 0 then Assert.Fail()
        if m1.GetLength(1) <> 0 then Assert.Fail()

        let m1arr :int[,] = array2D [||]
        if m1 <> m1arr then Assert.Fail()
        
        let m2 : int[,] = array2D [[]]
        if m2.GetLength(0) <> 1 then Assert.Fail()
        if m2.GetLength(1) <> 0 then Assert.Fail()

        let m2arr :int[,] = array2D [|[||]|]
        if m2 <> m2arr then Assert.Fail()
        
        
        let m3 = array2D [[1]]
        if m3.GetLength(0) <> 1 then Assert.Fail()
        if m3.GetLength(1) <> 1 then Assert.Fail()
        if m3.[0,0] <> 1 then Assert.Fail()

        let m3arr = array2D [[1]]
        if m3 <> m3arr then Assert.Fail()
        
        let m6lislis = array2D [[1;2]; 
                                [3;4]]
        let m6arrarr = array2D [|[|1;2|]; 
                                 [|3;4|]|]
        let m6arrlis = array2D [|[1;2]; 
                                 [3;4]|]
        let m6lisarr = array2D [[|1;2|]; 
                                 [|3;4|]]
        if m6lislis <> m6arrarr then Assert.Fail()
        if m6lislis <> m6arrlis then Assert.Fail()
        if m6lislis <> m6lisarr then Assert.Fail()
        
        let m7 = array2D [for i in 0..1000 do
                            yield [for j in 0..1000 do
                                       yield i*j] ]


        let matrix :int[,] = array2D [[1;2;3]; 
                                      [2;3;4]]
        if matrix.GetLength(0) <> 2 then Assert.Fail()
        if matrix.GetLength(1) <> 3 then Assert.Fail()
        if matrix.[0,0] <> 1 then Assert.Fail()
        if matrix.[1,2] <> 4 then Assert.Fail()
        
        CheckThrowsArgumentException( fun () -> ignore (array2D [[1;2]; [2]]))
        CheckThrowsArgumentException( fun () -> ignore (array2D [[1;2]; [2;3]; [4]]))

        CheckThrowsArgumentNullException( fun () -> ignore (array2D null))
        CheckThrowsArgumentException( fun () -> ignore (array2D [null]))
        CheckThrowsArgumentException( fun () -> ignore (array2D [[|1|];null]))
       
        let m16 :string[,] = array2D [[null]]
        if m16.[0,0] <> null then Assert.Fail()

    [<Fact>]
    member this.``Slicing with reverse index in one slice expr behaves as expected``()  = 
        let arr = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]

        Assert.Equal(arr.[*, ^1..^0], arr.[*, 3..4])

    [<Fact>]
    member this.``Slicing with reverse index in both slice expr behaves as expected``()  = 
        let arr = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]

        Assert.Equal(arr.[..^1, ^1..^0], arr.[..0, 3..4])

    [<Fact>]
    member this.``Slicing with reverse index in fixed index behaves as expected``()  = 
        let arr = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]

        Assert.AreEqual(arr.[^1, ^1..^0], arr.[0, 3..4])

    [<Fact>]
    member this.``Slicing with reverse index and non reverse fixed index behaves as expected``()  = 
        let arr = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]

        Assert.AreEqual(arr.[1, ^1..^0], arr.[1, 3..4])

    [<Fact>]
    member this.``Set slice with reverse index in one slice expr behaves as expected``()  = 
        let arr1 = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]
        let arr2 = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]

        let setArray = array2D [[10;11]; [12;13]]
        arr1.[*, ^1..^0] <- setArray
        arr2.[*, ^1..^0] <- setArray

        Assert.Equal(arr1, arr2)

    [<Fact>]
    member this.``Set slice with reverse index in both slice expr behaves as expected``()  = 
        let arr1 = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]
        let arr2 = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]

        let setArray = array2D [[10;11]; [12;13]]
        arr1.[0..^0, ^1..^0] <- setArray
        arr2.[0..^0, ^1..^0] <- setArray

        Assert.Equal(arr1, arr2)

    [<Fact>]
    member this.``Set slice with reverse index in fixed index behaves as expected``()  = 
        let arr1 = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]
        let arr2 = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]

        let setArray = [|12;13|]
        arr1.[^1, ^1..^0] <- setArray
        arr2.[^1, ^1..^0] <- setArray

        Assert.Equal(arr1, arr2)

    [<Fact>]
    member this.``Set slice with reverse index in and non reverse fixed index behaves as expected``()  = 
        let arr1 = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]
        let arr2 = array2D [[ 1;2;3;4;5 ]; [ 5;4;3;2;1 ]]

        let setArray = [|12;13|]
        arr1.[1, ^1..^0] <- setArray
        arr2.[1, ^1..^0] <- setArray

        Assert.Equal(arr1, arr2)

    [<Fact>]
    member this.``Set item with reverse index in one dim behaves as expected``() =
        let arr = array2D [[1;2;3]; [3;2;1]]

        arr.[^1, 0] <- 9
        Assert.AreEqual(arr.[0, 0], 9)

    [<Fact>]
    member this.``Set item with reverse index in all dims behaves as expected``()=
        let arr = array2D [[1;2;3]; [3;2;1]]

        arr.[^0, ^0] <- 9
        Assert.AreEqual(arr.[1,2], 9)

    [<Fact>]
    member this.``Get item with reverse index in one dim behaves as expected``() =
        let arr = array2D [[1;2;3]; [4;5;6]]

        Assert.AreEqual(arr.[^0, 0], 4)

    [<Fact>]
    member this.``Get item with reverse index in all dims behaves as expected``()=
        let arr = array2D [[1;2;3]; [4;5;6]]

        Assert.AreEqual(arr.[^1, ^1], 2)

    [<Fact>]
    member this.SlicingBoundedStartEnd() =
        let m1 = array2D [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                            [| 10.0;20.0;30.0;40.0;50.0;60.0 |]  |]

        shouldEqual m1.[*,*] m1
        shouldEqual m1.[0..,*]  (array2D [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                                [| 10.0;20.0;30.0;40.0;50.0;60.0 |]  |])
        shouldEqual m1.[1..,*]  (array2D [| 
                                [| 10.0;20.0;30.0;40.0;50.0;60.0 |]  |])
        shouldEqual m1.[..0,*]  (array2D [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                                |])
        shouldEqual m1.[*,0..]  (array2D [| [| 1.0;2.0;3.0;4.0;5.0;6.0 |];
                                            [| 10.0;20.0;30.0;40.0;50.0;60.0 |]  
                                         |])
        shouldEqual m1.[*,1..]  (array2D [| [| 2.0;3.0;4.0;5.0;6.0 |];
                                            [| 20.0;30.0;40.0;50.0;60.0 |]  
                                         |])
        shouldEqual m1.[*,2..]  (array2D [| [| 3.0;4.0;5.0;6.0 |];
                                            [| 30.0;40.0;50.0;60.0 |]  
                                         |])
        shouldEqual m1.[*,3..]  (array2D [| [| 4.0;5.0;6.0 |];
                                            [| 40.0;50.0;60.0 |]  
                                         |])
        shouldEqual m1.[*,4..]  (array2D [| [| 5.0;6.0 |];
                                            [| 50.0;60.0 |]  
                                         |])
        shouldEqual m1.[*,5..]  (array2D [| [| 6.0 |];
                                            [| 60.0 |]  
                                |])
        shouldEqual m1.[*, 0]  [| 1.0; 10.0 |]
        shouldEqual m1.[1.., 3]  [| 40.0 |]
        shouldEqual m1.[1, *]  [| 10.0;20.0;30.0;40.0;50.0;60.0 |]
        shouldEqual m1.[0, ..3]  [| 1.0;2.0;3.0;4.0 |]
        

    [<Fact>]
    member this.SlicingUnboundedEnd() = 
        let arr = array2D [ [1;2;3;4;5;6]; [6;5;4;3;2;1]]

        shouldEqual arr.[(-1).., *] arr
        shouldEqual arr.[0.., 1..] (array2D [ [2;3;4;5;6]; [5;4;3;2;1] ])
        shouldEqual arr.[1.., ..3] (array2D [ [6;5;4;3] ])
        shouldBeEmpty arr.[2.., 6..] 


    [<Fact>]
    member this.SlicingUnboundedStart() = 
        let arr = array2D [ [1;2;3;4;5;6]; [6;5;4;3;2;1]]

        shouldBeEmpty arr.[..(-1), *] 
        shouldEqual arr.[..0, ..4] (array2D [ [1;2;3;4;5] ])
        shouldEqual arr.[..1, ..3] (array2D [ [1;2;3;4]; [6;5;4;3] ])
        shouldEqual arr.[..2, ..6] arr


    [<Fact>]
    member this.SlicingOutOfBounds() = 
        let arr = array2D [ [1;2;3;4;5;6]; [6;5;4;3;2;1]]
       
        shouldEqual arr.[*, ..6] arr
        shouldBeEmpty arr.[*, 6..] 
        shouldEqual arr.[..2, *] arr
        shouldBeEmpty arr.[2.., *] 

        shouldBeEmpty arr.[1..0, *] 
        shouldBeEmpty arr.[1..0, (-1)..0] 

        shouldBeEmpty arr.[0..(-1), *] 
        shouldBeEmpty arr.[1..(-1), *] 
        shouldBeEmpty arr.[1..0, *] 
        shouldEqual arr.[0..6, (-1)..9] arr
        shouldEqual arr.[*, 1..6] (array2D [ [2;3;4;5;6]; [5;4;3;2;1] ])

        shouldEqual arr.[1, 3..1] [| |] 
        shouldEqual arr.[3..1, 1] [| |] 
        shouldEqual arr.[10.., 1] [| |] 
        shouldEqual arr.[1, 10..] [| |] 
        shouldEqual arr.[1, .. -1] [| |] 
        shouldEqual arr.[.. -1, 1] [| |] 

    [<Fact>]
    member this.SlicingMutation() = 
        let arr2D1 = array2D [| [| 1.; 2.; 3.; 4. |];
           [| 5.; 6.; 7.; 8. |];
           [| 9.; 10.; 11.; 12. |] |]

        arr2D1.[0, *] <- [|0.; 0.; 0.; 0.|]
        shouldEqual arr2D1.[0,*]  [|0.; 0.; 0.; 0.|]
        arr2D1.[*, 1] <- [|100.; 100.; 100.|]
        shouldEqual arr2D1.[*,1]  [|100.; 100.; 100.|]
        shouldEqual arr2D1.[*,*]  (array2D [| [| 0.; 100.; 0.; 0. |];
                                 [| 5.; 100.; 7.; 8. |];
                                 [| 9.; 100.; 11.; 12. |] |])

