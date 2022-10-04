// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Core.Operators module

namespace FSharp.Core.UnitTests.Operators

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit

type OperatorsModule1() =

    [<Fact>]
    member _.KeyValue() =

        let funcKeyValue x =
            match x with
            | Operators.KeyValue(a) -> a
        
        // string int
        let stringint = funcKeyValue ( new System.Collections.Generic.KeyValuePair<string,int>("string",1))
        Assert.AreEqual(("string",1), stringint)
        
        // float char
        let floatchar = funcKeyValue ( new System.Collections.Generic.KeyValuePair<float,char>(1.0,'a'))
        Assert.AreEqual((1.0,'a'), floatchar)
        
        // null
        let nullresult = funcKeyValue ( new System.Collections.Generic.KeyValuePair<string,char>(null,' '))
        let (nullstring:string,blankchar:char) = nullresult
        
        CheckThrowsNullRefException(fun () -> nullstring.ToString() |> ignore)

    [<Fact>]
    member _.OptimizedRangesGetArraySlice() =

        let param1 = Some(1)
        let param2 = Some(2)
            
        // int
        let intslice = Operators.OperatorIntrinsics.GetArraySlice [|1;2;3;4;5;6|] param1 param2
        Assert.AreEqual([|2;3|], intslice)
        
        // string
        let stringslice = Operators.OperatorIntrinsics.GetArraySlice [|"1";"2";"3"|] param1 param2
        Assert.AreEqual([|"2";"3"|], stringslice)
        
        // null
        let stringslice = Operators.OperatorIntrinsics.GetArraySlice [|null;null;null|] param1 param2
        Assert.AreEqual([|null;null|], stringslice)

    [<Fact>]
    member _.OptimizedRangesGetArraySlice2D() =

        let param1D1 = Some(0)
        let param1D2 = Some(1)
        let param2D1 = Some(0)
        let param2D2 = Some(1)
            
        // int
        let intArray2D = Array2D.init 2 3 (fun i j -> i*100+j)
        let intslice = Operators.OperatorIntrinsics.GetArraySlice2D intArray2D param1D1 param1D2 param2D1 param2D2
        
        Assert.AreEqual(101, intslice.[1,1])
         
        // string
        let stringArray2D = Array2D.init 2 3 (fun i j -> (i*100+j).ToString())
        let stringslice = Operators.OperatorIntrinsics.GetArraySlice2D stringArray2D param1D1 param1D2 param2D1 param2D2
        Assert.AreEqual((101).ToString(), stringslice.[1,1])
        
        // null
        let nullArray2D = Array2D.init 2 3 (fun i j -> null)
        let nullslice = Operators.OperatorIntrinsics.GetArraySlice2D nullArray2D param1D1 param1D2 param2D1 param2D2
        Assert.AreEqual(null, nullslice.[1,1])

    [<Fact>]
    member _.OptimizedRangesGetStringSlice() =
        let param1 = Some(4)
        let param2 = Some(6)
            
        // string
        let stringslice = Operators.OperatorIntrinsics.GetStringSlice "abcdefg" param1 param2
        Assert.AreEqual("efg", stringslice)
        
        // null
        CheckThrowsNullRefException(fun () -> Operators.OperatorIntrinsics.GetStringSlice null param1 param2 |> ignore)

    [<Fact>]
    member _.OptimizedRangesSetArraySlice() =
        let param1 = Some(1)
        let param2 = Some(2)
            
        // int
        let intArray1 = [|1;2;3|]
        let intArray2 = [|4;5;6|]
        Operators.OperatorIntrinsics.SetArraySlice intArray1 param1 param2 intArray2
        Assert.AreEqual([|1;4;5|], intArray1)
        
        // string
        let stringArray1 = [|"1";"2";"3"|]
        let stringArray2 = [|"4";"5";"6"|]
        Operators.OperatorIntrinsics.SetArraySlice stringArray1 param1 param2 stringArray2
        Assert.AreEqual([|"1";"4";"5"|], stringArray1)
        
        // null
        let nullArray1 = [|null;null;null|]
        let nullArray2 = [|null;null;null|]
        Operators.OperatorIntrinsics.SetArraySlice nullArray1  param1 param2 nullArray2
        CheckThrowsNullRefException(fun () -> nullArray1.[0].ToString() |> ignore)

    [<Fact>]
    member _.OptimizedRangesSetArraySlice2D() =
        let param1D1 = Some(0)
        let param1D2 = Some(1)
        let param2D1 = Some(0)
        let param2D2 = Some(1)
            
        // int
        let intArray1 = Array2D.init 2 3 (fun i j -> i*10+j)
        let intArray2 = Array2D.init 2 3 (fun i j -> i*100+j)
        Operators.OperatorIntrinsics.SetArraySlice2D intArray1 param1D1 param1D2 param2D1 param2D2 intArray2
        Assert.AreEqual(101, intArray1.[1,1])
        
        // string
        let stringArray2D1 = Array2D.init 2 3 (fun i j -> (i*10+j).ToString())
        let stringArray2D2 = Array2D.init 2 3 (fun i j -> (i*100+j).ToString())
        Operators.OperatorIntrinsics.SetArraySlice2D stringArray2D1 param1D1 param1D2 param2D1 param2D2 stringArray2D2
        Assert.AreEqual((101).ToString(), stringArray2D1.[1,1])
        
        // null
        let nullArray2D1 = Array2D.init 2 3 (fun i j -> null)
        let nullArray2D2 = Array2D.init 2 3 (fun i j -> null)
        Operators.OperatorIntrinsics.SetArraySlice2D nullArray2D1 param1D1 param1D2 param2D1 param2D2 nullArray2D2
        CheckThrowsNullRefException(fun () -> nullArray2D1.[0,0].ToString()  |> ignore)

    [<Fact>]
    member _.OptimizedRangesSetArraySlice3D() =
        let intArray1 = Array3D.init 2 3 4 (fun i j k -> i*10+j)
        let intArray2 = Array3D.init 2 3 4 (fun i j k -> i*100+j)
        Operators.OperatorIntrinsics.SetArraySlice3D intArray1 (Some 0) (Some 1) (Some 0) (Some 1) (Some 0) (Some 1) intArray2
        Assert.AreEqual(101, intArray1.[1,1,1])

    [<Fact>]
    member _.OptimizedRangesSetArraySlice4D() =
        let intArray1 = Array4D.init 2 3 4 5 (fun i j k l -> i*10+j)
        let intArray2 = Array4D.init 2 3 4 5 (fun i j k l -> i*100+j)
        Operators.OperatorIntrinsics.SetArraySlice4D intArray1 (Some 0) (Some 1) (Some 0) (Some 1) (Some 0) (Some 1) (Some 0) (Some 1) intArray2
        Assert.AreEqual(101, intArray1.[1,1,1,1])

    [<Fact>]
    member _.Uncheckeddefaultof () =
        
        // int
        let intdefault = Operators.Unchecked.defaultof<int>
        Assert.AreEqual(0, intdefault)
      
        // string
        let stringdefault = Operators.Unchecked.defaultof<string>
        CheckThrowsNullRefException(fun () -> stringdefault.ToString() |> ignore)
        
        // null
        let structdefault = Operators.Unchecked.defaultof<DateTime>
        Assert.AreEqual(1,  structdefault.Day)

    [<Fact>]
    member _.abs () =
        
        // int
        let intabs = Operators.abs (-7)
        Assert.AreEqual(7, intabs)
      
        // float
        let floatabs = Operators.abs (-100.0)
        Assert.AreEqual(100.0, floatabs)
        
        // decimal
        let decimalabs = Operators.abs (-1000M)
        Assert.AreEqual(1000M, decimalabs)

    [<Fact>]
    member _.acos () =
        
        // min value
        let minacos = Operators.acos (0.0)
        Assert.AreNearEqual(1.5707963267948966, minacos)
      
        // normal value
        let normalacos = Operators.acos (0.3)
        Assert.AreNearEqual(1.2661036727794992, normalacos)
      
        // max value
        let maxacos = Operators.acos (1.0)
        Assert.AreEqual(0.0, maxacos)

    [<Fact>]
    member _.asin () =
        
        // min value
        let minasin = Operators.asin (0.0)
        Assert.AreEqual(0.0, minasin)
      
        // normal value
        let normalasin = Operators.asin (0.5)
        Assert.AreNearEqual(0.52359877559829893, normalasin)
      
        // max value
        let maxasin = Operators.asin (1.0)
        Assert.AreNearEqual(1.5707963267948966, maxasin)

    [<Fact>]
    member _.atan () =
        
        // min value
        let minatan = Operators.atan (0.0)
        Assert.AreNearEqual(0.0, minatan)
      
        // normal value
        let normalatan = Operators.atan (1.0)
        Assert.AreNearEqual(0.78539816339744828, normalatan)
      
        // biggish  value
        let maxatan = Operators.atan (infinity)
        Assert.AreNearEqual(1.5707963267948966, maxatan)

    [<Fact>]
    member _.atan2 () =
        
        // min value
        let minatan2 = Operators.atan2 (0.0) (1.0)
        Assert.AreNearEqual(0.0, minatan2)
      
        // normal value
        let normalatan2 = Operators.atan2 (1.0) (1.0)
        Assert.AreNearEqual(0.78539816339744828, normalatan2)
      
        // biggish  value
        let maxatan2 = Operators.atan2 (1.0) (0.0)
        Assert.AreNearEqual(1.5707963267948966, maxatan2)

    [<Fact>]
    member _.box () =
        
        // int value
        let intbox = Operators.box 1
        Assert.AreEqual(1, intbox)
      
        // string value
        let stringbox = Operators.box "string"
        Assert.AreEqual("string", stringbox)
      
        // null value
        let nullbox = Operators.box null
        CheckThrowsNullRefException(fun () -> nullbox.ToString()  |> ignore)

    [<Fact>]
    member _.byte() =
        // int type
        let intByte = Operators.byte 100
        Assert.AreEqual(100uy, intByte)
        
        // char type
        let charByte = Operators.byte '0'
        Assert.AreEqual(48uy, charByte)
        
        // boundary value
        let boundByte = Operators.byte 255.0
        Assert.AreEqual(255uy, boundByte)
        
        // Overflow
        let result = Operators.byte Int64.MaxValue
        Assert.AreEqual(Byte.MaxValue, result)
        
        // Overflow
        let result = Operators.byte Int64.MinValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte Single.MinValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte Single.MaxValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte Double.MinValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte Double.MaxValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte (Int64.MaxValue * 8L)
        Assert.AreEqual(248uy, result)      // bit-complement

        // Overflow
        let result = 255uy + 5uy
        Assert.AreEqual(4uy, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.byte Decimal.MinValue |> ignore)
        
    [<Fact>]
    member _.ceil() =
        // min value
        let minceil = Operators.ceil 0.1
        Assert.AreEqual(1.0, minceil)
        
        // normal value
        let normalceil = Operators.ceil 100.1
        Assert.AreEqual(101.0, normalceil)
        
        // max value
        let maxceil = Operators.ceil 1.7E+308
        Assert.AreEqual(1.7E+308, maxceil)
        
        // float32 value
        let float32ceil = Operators.ceil 100.1f
        Assert.AreEqual(101f, float32ceil)
        
        // decimal value
        let decimalceil = Operators.ceil 100.1m
        Assert.AreEqual(101m, decimalceil)
        
    [<Fact>]
    member _.char() =
        // int type
        Assert.AreEqual('0', Operators.char 48)
        Assert.AreEqual('0', Operators.char 48u)
        Assert.AreEqual('0', Operators.char 48s)
        Assert.AreEqual('0', Operators.char 48us)
        Assert.AreEqual('0', Operators.char 48y)
        Assert.AreEqual('0', Operators.char 48uy)
        Assert.AreEqual('0', Operators.char 48L)
        Assert.AreEqual('0', Operators.char 48uL)
        Assert.AreEqual('0', Operators.char 48n)
        Assert.AreEqual('0', Operators.char 48un)
        Assert.AreEqual('0', Operators.char 48f)
        Assert.AreEqual('0', Operators.char 48.)
        Assert.AreEqual('0', Operators.char 48m)

        // Overflow
        Assert.AreEqual('\000', Operators.char Single.MinValue)
        Assert.AreEqual('\000', Operators.char Double.MinValue)
        Assert.AreEqual('\000', Operators.char Single.MaxValue)
        Assert.AreEqual('\000', Operators.char Double.MaxValue)
        CheckThrowsOverflowException(fun () -> Operators.char Decimal.MinValue |> ignore)
        
        // string type
        let stringchar = Operators.char " "
        Assert.AreEqual(' ', stringchar)
       
    [<Fact>]
    member _.compare() =
        // int type
        let intcompare = Operators.compare 100 101
        Assert.AreEqual(-1, intcompare)
        
        // char type
        let charcompare = Operators.compare '0' '1'
        Assert.AreEqual(-1, charcompare)
        
        // null value
        let boundcompare = Operators.compare null null
        Assert.AreEqual(0, boundcompare)

    [<Fact>]
    member _.cos () =
        
        // min value
        let mincos = Operators.cos (0.0)
        Assert.AreEqual(1.0, mincos)
      
        // normal value
        let normalcos = Operators.cos (1.0)
        Assert.AreNearEqual(0.54030230586813977, normalcos)
        
        // biggish  value
        let maxcos = Operators.cos (1.57)
        Assert.AreNearEqual(0.00079632671073326335, maxcos)

    [<Fact>]
    member _.cosh () =

        // min value
        let mincosh = Operators.cosh (0.0)
        Assert.AreEqual(1.0, mincosh)
      
        // normal value
        let normalcosh = Operators.cosh (1.0)
        Assert.AreNearEqual(1.5430806348152437, normalcosh)
        
        // biggish  value
        let maxcosh = Operators.cosh (1.57)
        Assert.AreNearEqual(2.5073466880660993, maxcosh)

    [<Fact>]
    member _.decimal () =
        
        // int value
        let intdecimal = Operators.decimal (1)
        Assert.AreEqual(1M, intdecimal)
        
        // nativeint value
        let nativeintdecimal = Operators.decimal 1n
        Assert.AreEqual(1M, nativeintdecimal)
        
        // unativeint value
        let unativeintdecimal = Operators.decimal 1un
        Assert.AreEqual(1M, unativeintdecimal)
        
        // char value
        let chardecimal = Operators.decimal '\001'
        Assert.AreEqual(1M, chardecimal)
       
        // float value
        let floatdecimal = Operators.decimal (1.0)
        Assert.AreEqual(1M, floatdecimal)

    [<Fact>]
    member _.decr() =
        // zero
        let zeroref = ref 0
        Operators.decr zeroref
        Assert.AreEqual((ref -1), zeroref)
        
        //  big number
        let bigref = ref 32767
        Operators.decr bigref
        Assert.AreEqual((ref 32766), bigref)
        
        // normal value
        let normalref = ref 100
        Operators.decr (normalref)
        Assert.AreEqual((ref 99), normalref)
        
    [<Fact>]
    member _.defaultArg() =
        // zero
        let zeroOption = Some(0)
        let intdefaultArg = Operators.defaultArg zeroOption 2
        Assert.AreEqual(0, intdefaultArg)
        
        //  big number
        let bigOption = Some(32767)
        let bigdefaultArg = Operators.defaultArg bigOption 32766
        Assert.AreEqual(32767, bigdefaultArg)
        
        // normal value
        let normalOption = Some(100)
        let normalfaultArg = Operators.defaultArg normalOption 100
        Assert.AreEqual(100, normalfaultArg)
        
    [<Fact>]
    member _.double() =
        // int type
        let intdouble = Operators.float 100
        Assert.AreEqual(100.0, intdouble)
        
        // char type
        let chardouble = Operators.float '0'
        Assert.AreEqual(48.0, chardouble)
        
        // decimal type
        let decimaldouble = Operators.float 100m
        Assert.AreEqual(100.0, decimaldouble)

    [<Fact>]
    member _.enum() =
        // zero
        let intarg : int32 = 0
        let intenum = Operators.enum<System.ConsoleColor> intarg
        Assert.AreEqual(System.ConsoleColor.Black, intenum)
        
        // big number
        let bigarg : int32 = 15
        let charenum = Operators.enum<System.ConsoleColor> bigarg
        Assert.AreEqual(System.ConsoleColor.White, charenum)
        
        // normal value
        let normalarg : int32 = 9
        let boundenum = Operators.enum<System.ConsoleColor> normalarg
        Assert.AreEqual(System.ConsoleColor.Blue, boundenum)
        
#if IGNORED
    [<Test;Ignore("See FSB #3826 ? Need way to validate Operators.exit function.")>]
    member _.exit() =
        // zero
        try
            let intexit = Operators.exit 1

        with
            | _ -> ()
        //Assert.AreEqual(-1, intexit)
        
        //  big number
        let charexit = Operators.exit 32767
        //Assert.AreEqual(-1, charexit)
        
        // normal value
        let boundexit = Operators.exit 100
        Assert.AreEqual(0, boundexit)
#endif

    [<Fact>]
    member _.exp() =
        // zero
        let zeroexp = Operators.exp 0.0
        Assert.AreEqual(1.0, zeroexp)
        
        //  big number
        let bigexp = Operators.exp 32767.0
        Assert.AreEqual(infinity, bigexp)
        
        // normal value
        let normalexp = Operators.exp 100.0
        Assert.AreEqual(2.6881171418161356E+43, normalexp)
        
    [<Fact>]
    member _.failwith() =
        try
            let _ = Operators.failwith "failwith"
            Assert.Fail("Expect fail but not.")

        with
            | Failure("failwith") -> ()
            |_ -> Assert.Fail("Throw unexpected exception")

    [<Fact>]
    member _.float() =
        // int type
        let intfloat = Operators.float 100
        Assert.AreEqual((float)100, intfloat)
        
        // char type
        let charfloat = Operators.float '0'
        Assert.AreEqual((float)48, charfloat)

        // decimal type
        let intfloat = Operators.float 100m
        Assert.AreEqual((float)100, intfloat)

    [<Fact>]
    member _.float32() =
        // int type
        let intfloat32 = Operators.float32 100
        Assert.AreEqual((float32)100, intfloat32)
        
        // char type
        let charfloat32 = Operators.float32 '0'
        Assert.AreEqual((float32)48, charfloat32)
        
        // decimal type
        let intfloat32 = Operators.float32 100m
        Assert.AreEqual((float32)100, intfloat32)

    [<Fact>]
    member _.floor() =
        // float type
        let floatfloor = Operators.floor 100.9
        Assert.AreEqual(100.0, floatfloor)
        
        // float32 type
        let float32floor = Operators.floor 100.9f
        Assert.AreEqual(100.0f, float32floor)
        
        // decimal type
        let decimalfloor = Operators.floor 100.9m
        Assert.AreEqual(100m, decimalfloor)
    
    [<Fact>]
    member _.fst() =
        // int type
        let intfst = Operators.fst (100,101)
        Assert.AreEqual(100, intfst)
        
        // char type
        let charfst = Operators.fst ('0','1')
        Assert.AreEqual('0', charfst)
        
        // null value
        let boundfst = Operators.fst (null,null)
        Assert.AreEqual(null, boundfst)
        
    [<Fact>]
    member _.hash() =
        // int type (stable between JIT versions)
        let inthash = Operators.hash 100
        Assert.AreEqual(100, inthash)
        
        // char type (stable between JIT versions)
        let charhash = Operators.hash '0'
        Assert.AreEqual(3145776, charhash)
        
        // string value (test disabled, each JIT and each x86 vs x64 creates a different hash here)
        //let boundhash = Operators.hash "A"
        //Assert.AreEqual(-842352673, boundhash)
        
    [<Fact>]
    member _.id() =
        // int type
        let intid = Operators.id 100
        Assert.AreEqual(100, intid)
        
        // char type
        let charid = Operators.id '0'
        Assert.AreEqual('0', charid)
        
        // string value
        let boundid = Operators.id "A"
        Assert.AreEqual("A", boundid)

    [<Fact>]
    member _.ignore() =
        // value type
        let result = Operators.ignore 10
        Assert.AreEqual(null, result)
        
        // reference type
        let result = Operators.ignore "A"
        Assert.AreEqual(null, result)

    [<Fact>]
    member _.incr() =
        // legit value
        let result = ref 10
        Operators.incr result
        Assert.AreEqual(11, !result)
        
        // Overflow.
        let result = ref (Operators.Checked.int System.Int32.MaxValue)
        Operators.incr result
        Assert.AreEqual(System.Int32.MinValue, !result)

    [<Fact>]
    member _.infinity() =
        
        let inf = Operators.infinity
        let result = inf > System.Double.MaxValue
        Assert.True(result)
        
        // arithmetic operation
        let result = infinity + 3.0
        Assert.AreEqual(Double.PositiveInfinity, result)
        let result = infinity - 3.0
        Assert.AreEqual(Double.PositiveInfinity, result)
        let result = infinity * 3.0
        Assert.AreEqual(Double.PositiveInfinity, result)
        let result = infinity / 3.0
        Assert.AreEqual(Double.PositiveInfinity, result)
        let result = infinity / 3.0
        Assert.AreEqual(Double.PositiveInfinity, result)

    [<Fact>]
    member _.infinityf() =
        
        let inf = Operators.infinityf
        let result = inf > System.Single.MaxValue
        Assert.True(result)
        
        // arithmetic operation
        let result = infinityf + 3.0f
        Assert.AreEqual(Single.PositiveInfinity, result)
        let result = infinityf - 3.0f
        Assert.AreEqual(Single.PositiveInfinity, result)
        let result = infinityf * 3.0f
        Assert.AreEqual(Single.PositiveInfinity, result)
        let result = infinityf / 3.0f
        Assert.AreEqual(Single.PositiveInfinity, result)
        let result = infinityf / 3.0f
        Assert.AreEqual(Single.PositiveInfinity, result)
