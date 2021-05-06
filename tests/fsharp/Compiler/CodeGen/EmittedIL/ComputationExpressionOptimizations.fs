// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test.Utilities
open NUnit.Framework

open System

[<TestFixture>]
module ComputationExpressionOptimizations =

    [<Test>]
    // See https://github.com/fsharp/fslang-design/blob/master/tooling/FST-1034-lambda-optimizations.md
    //
    // This tests a number of code optimizations cooperating together.
    // - InlineIfLambda must be applied
    // - Computed functions must be reduced.
    //
    // This is for the "sync { ... }" builder that simply runs code synchronously - no
    // one uses this in practice but it's a good baseline test for the elimination and redution of constructs.
    //
    let ``check reduction of sample builder for synchronous code``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module Test

open System

type SyncCode<'T> = unit -> 'T

type SyncBuilder() =
    
    member inline _.Delay([<InlineIfLambda>] f: unit -> SyncCode<'T>) :  SyncCode<'T> =
        (fun () -> (f())())

    member inline _.Run([<InlineIfLambda>] code : SyncCode<'T>) : 'T = 
        code()

    [<DefaultValue>]
    member inline _.Zero() : SyncCode< unit> = 
        (fun () -> ())

    member inline _.Return (x: 'T) : SyncCode<'T> =
        (fun () -> x)

    member inline _.Combine([<InlineIfLambda>] code1: SyncCode<unit>, [<InlineIfLambda>] code2: SyncCode<'T>) : SyncCode<'T> =
        (fun () -> 
            code1()
            code2())

    member inline _.While([<InlineIfLambda>] condition: unit -> bool, [<InlineIfLambda>] body: SyncCode<unit>) : SyncCode<unit> =
       (fun () -> 
            while condition() do
                body())

    member inline _.TryWith([<InlineIfLambda>] body: SyncCode<'T>, [<InlineIfLambda>] catch: exn -> SyncCode<'T>) : SyncCode<'T> =
        (fun () -> 
            try
                body()
            with exn -> 
                (catch exn)())

    member inline _.TryFinally([<InlineIfLambda>] body: SyncCode<'T>, compensation: unit -> unit) : SyncCode<'T> =
        (fun () -> 
            let res = 
                try
                    body()
                with _ ->
                    compensation()
                    reraise()
            compensation()
            res)

    member inline this.Using(disp : #IDisposable, [<InlineIfLambda>] body: #IDisposable -> SyncCode<'T>) : SyncCode<'T> = 
        this.TryFinally(
            (fun () -> (body disp)()),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'T>, [<InlineIfLambda>] body : 'T -> SyncCode<unit>) : SyncCode<unit> =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> (body e.Current)()))))

    member inline _.ReturnFrom (value: 'T) : SyncCode<'T> =
        (fun () -> 
              value)

    member inline _.Bind (v: 'TResult1, [<InlineIfLambda>] continuation: 'TResult1 -> SyncCode<'TResult2>) : SyncCode<'TResult2> =
        (fun () -> 
             (continuation v)())

let sync = SyncBuilder()

module Examples =

     let t1 y = 
         sync {
            let x = 4 + 5 + y
            return x
         }

     let testFunctionWithBind y = 
         sync {
            printfn "step"
            let! x = t1 y
            return x + y
         }

     let testFunctionWithWhile y = 
         sync {
            printfn "step"
            while y < 0 do
                let! x = t1 y
                printfn $"step {x}"

            return y + y
         }
     let testFunctionWithTryCatch y = 
         sync {
            try 
               printfn "step"
               let! x = t1 y
               return x + y
            with _ -> 
               return 5
         }
     let testFunctionWithFinally y = 
         sync {
            try 
               printfn "step"
               let! x = t1 y
               return x + y
            finally
               printfn "step"
         }
            """
            (fun verifier -> verifier.VerifyIL [
            // Check testFunctionWithBind is flattened
            """
.method public static int32  testFunctionWithBind(int32 y) cil managed
{
  
  .maxstack  8
  IL_0000:  ldstr      "step"
  IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
  IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
  IL_000f:  pop
  IL_0010:  ldc.i4.s   9
  IL_0012:  ldarg.0
  IL_0013:  add
  IL_0014:  ldarg.0
  IL_0015:  add
  IL_0016:  ret
} 
            """

            // Check testFunctionWithWhile is flattened
            """
.method public static int32  testFunctionWithWhile(int32 y) cil managed
{
  
  .maxstack  7
  .locals init (int32 V_0)
  IL_0000:  ldstr      "step"
  IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
  IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
  IL_000f:  pop
  IL_0010:  ldarg.0
  IL_0011:  ldc.i4.0
  IL_0012:  bge.s      IL_003f
    
  IL_0014:  ldc.i4.s   9
  IL_0016:  ldarg.0
  IL_0017:  add
  IL_0018:  stloc.0
  IL_0019:  ldstr      "step %P()"
  IL_001e:  ldc.i4.1
  IL_001f:  newarr     [runtime]System.Object
  IL_0024:  dup
  IL_0025:  ldc.i4.0
  IL_0026:  ldloc.0
  IL_0027:  box        [runtime]System.Int32
  IL_002c:  stelem     [runtime]System.Object
  IL_0031:  ldnull
  IL_0032:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string,
                                                                                                                                                                                                                                                                                          object[],
                                                                                                                                                                                                                                                                                          class [runtime]System.Type[])
  IL_0037:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
  IL_003c:  pop
  IL_003d:  br.s       IL_0010
    
  IL_003f:  ldarg.0
  IL_0040:  ldarg.0
  IL_0041:  add
  IL_0042:  ret
} 
            """

            // Check testFunctionWithTryCatch is flattened
            """
.method public static int32  testFunctionWithTryCatch(int32 y) cil managed
{
  
  .maxstack  4
  .locals init (int32 V_0,
           class [runtime]System.Exception V_1)
  .try
  {
    IL_0000:  ldstr      "step"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  pop
    IL_0010:  ldc.i4.s   9
    IL_0012:  ldarg.0
    IL_0013:  add
    IL_0014:  ldarg.0
    IL_0015:  add
    IL_0016:  stloc.0
    IL_0017:  leave.s    IL_0023
    
  }  
  catch [runtime]System.Object 
  {
    IL_0019:  castclass  [runtime]System.Exception
    IL_001e:  stloc.1
    IL_001f:  ldc.i4.5
    IL_0020:  stloc.0
    IL_0021:  leave.s    IL_0023
    
  }  
  IL_0023:  ldloc.0
  IL_0024:  ret
} 
            """
            // Check testFunctionWithFinally is flattened
            """
.method public static int32  testFunctionWithFinally(int32 y) cil managed
{
  
  .maxstack  4
  .locals init (int32 V_0,
           int32 V_1,
           class [runtime]System.Exception V_2)
  .try
  {
    IL_0000:  ldstr      "step"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  pop
    IL_0010:  ldc.i4.s   9
    IL_0012:  ldarg.0
    IL_0013:  add
    IL_0014:  ldarg.0
    IL_0015:  add
    IL_0016:  stloc.1
    IL_0017:  leave.s    IL_003a
    
  }  
  catch [runtime]System.Object 
  {
    IL_0019:  castclass  [runtime]System.Exception
    IL_001e:  stloc.2
    IL_001f:  ldstr      "step"
    IL_0024:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0029:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002e:  pop
    IL_002f:  rethrow
    IL_0031:  ldnull
    IL_0032:  unbox.any  [runtime]System.Int32
    IL_0037:  stloc.1
    IL_0038:  leave.s    IL_003a
    
  }  
  IL_003a:  ldloc.1
  IL_003b:  stloc.0
  IL_003c:  ldstr      "step"
  IL_0041:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
  IL_0046:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
  IL_004b:  pop
  IL_004c:  ldloc.0
  IL_004d:  ret
} 
            """
            ])

