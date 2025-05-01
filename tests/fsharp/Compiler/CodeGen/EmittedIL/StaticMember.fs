// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test
open Xunit


module ``Static Member`` =

    [<Fact>]
    let ``Action on Static Member``() =
        CompilerAssert.CompileLibraryAndVerifyILRealSig(
            """
module StaticMember01

open System
type C =
    static member M() = ()
    static member CreateAction() =
        new Action(C.M)
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed StaticMember01
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname CreateAction@8
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .method assembly static void  Invoke() cil managed
      {
        
        .maxstack  8
        IL_0000:  ret
      } 

    } 

    .method public static void  M() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public static class [runtime]System.Action CreateAction() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldftn      void StaticMember01/C/CreateAction@8::Invoke()
      IL_0007:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                        native int)
      IL_000c:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$StaticMember01
       extends [runtime]System.Object
{
} 
            """
            ]))

    [<Fact>]
    let ``Action on Static Member with lambda``() =
        CompilerAssert.CompileLibraryAndVerifyILRealSig(
            """
module StaticMember02

open System
type C =
    static member M(x : int32) = ()
    static member CreateAction() =
        new Action<int32>(fun x -> C.M x)
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed StaticMember02
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname CreateAction@8
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .method assembly static void  Invoke(int32 x) cil managed
      {
        
        .maxstack  8
        IL_0000:  ret
      } 

    } 

    .method public static void  M(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public static class [runtime]System.Action`1<int32> CreateAction() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldftn      void StaticMember02/C/CreateAction@8::Invoke(int32)
      IL_0007:  newobj     instance void class [runtime]System.Action`1<int32>::.ctor(object,
                                                                                       native int)
      IL_000c:  ret
    } 

  } 

} 
            """
            ]))

    [<Fact>]
    let ``Action on Static Member with closure``() =
        CompilerAssert.CompileLibraryAndVerifyILRealSig(
            """
module StaticMember03

open System

type C =
    static member M(x: int32) = ()

type MyClass(x: int32) =
    member this.X = x

[<EntryPoint>]
let main _ =
    let input = new MyClass(7)
    let func = new Action<int32>(fun x -> C.M input.X)
    box func |> ignore // make sure 'func' is not optimized away
    0
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed StaticMember03
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public static void  M(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class auto ansi serializable nested public MyClass
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32 x
    .method public specialname rtspecialname instance void  .ctor(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      int32 StaticMember03/MyClass::x
      IL_000f:  ret
    } 

    .method public hidebysig specialname instance int32  get_X() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 StaticMember03/MyClass::x
      IL_0006:  ret
    } 

    .property instance int32 X()
    {
      .get instance int32 StaticMember03/MyClass::get_X()
    } 
  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname func@15
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static void  Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .method public static int32  main(string[] _arg1) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class StaticMember03/MyClass V_0,
             class [runtime]System.Action`1<int32> V_1,
             object V_2)
    IL_0000:  ldc.i4.7
    IL_0001:  newobj     instance void StaticMember03/MyClass::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  ldnull
    IL_0008:  ldftn      void StaticMember03/func@15::Invoke(int32)
    IL_000e:  newobj     instance void class [runtime]System.Action`1<int32>::.ctor(object,
                                                                                           native int)
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  box        class [runtime]System.Action`1<int32>
    IL_001a:  stloc.2
    IL_001b:  ldc.i4.0
    IL_001c:  ret
  } 

} 
            """
            ]))

    [<Fact>]
    let ``Func on Static Member``() =
        CompilerAssert.CompileLibraryAndVerifyILRealSig(
            """
module StaticMember04

open System
type C =
    static member M(x: int32) =
        x + 1
    static member CreateFunc() =
        new Func<int32, int32>(C.M)
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed StaticMember04
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname CreateFunc@9
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .method assembly static int32  Invoke(int32 x) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  add
        IL_0003:  ret
      } 

    } 

    .method public static int32  M(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  add
      IL_0003:  ret
    } 

    .method public static class [runtime]System.Func`2<int32,int32> CreateFunc() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldftn      int32 StaticMember04/C/CreateFunc@9::Invoke(int32)
      IL_0007:  newobj     instance void class [runtime]System.Func`2<int32,int32>::.ctor(object,
                                                                                           native int)
      IL_000c:  ret
    } 

  } 

} 
            """
            ]))

    [<Fact>]
    let ``Func on Static Member with lambda``() =
        CompilerAssert.CompileLibraryAndVerifyILRealSig(
            """
module StaticMember05

open System
type C =
    static member M(x: int32) =
        x + 43
    static member CreateFunc() =
        new Func<int32, int32>(fun x -> C.M x)
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed StaticMember05
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname CreateFunc@9
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .method assembly static int32  Invoke(int32 x) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.s   43
        IL_0003:  add
        IL_0004:  ret
      } 

    } 

    .method public static int32  M(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   43
      IL_0003:  add
      IL_0004:  ret
    } 

    .method public static class [runtime]System.Func`2<int32,int32> CreateFunc() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldftn      int32 StaticMember05/C/CreateFunc@9::Invoke(int32)
      IL_0007:  newobj     instance void class [runtime]System.Func`2<int32,int32>::.ctor(object,
                                                                                           native int)
      IL_000c:  ret
    } 

  } 

} 
            """
            ]))

    [<Fact>]
    let ``Func on Static Member with closure``() =
        CompilerAssert.CompileLibraryAndVerifyILRealSig(
            """
module StaticMember06

open System

type C =
    static member M(x: int32) =
        x + 43

type MyClass(x: int32) =
    member this.X = x

[<EntryPoint>]
let main _ =
    let input = new MyClass(7)
    let func = new Func<int32, int32>(fun x -> C.M input.X)
    box func |> ignore // dummy code to prevent func being optimized away
    0
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.method public static int32  main(string[] _arg1) cil managed
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
  
  .maxstack  4
  .locals init (class StaticMember06/MyClass V_0,
           class [runtime]System.Func`2<int32,int32> V_1,
           object V_2)
  IL_0000:  ldc.i4.7
  IL_0001:  newobj     instance void StaticMember06/MyClass::.ctor(int32)
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  newobj     instance void StaticMember06/func@16::.ctor(class StaticMember06/MyClass)
  IL_000d:  ldftn      instance int32 StaticMember06/func@16::Invoke(int32)
  IL_0013:  newobj     instance void class [runtime]System.Func`2<int32,int32>::.ctor(object,
                                                                                             native int)
  IL_0018:  stloc.1
  IL_0019:  ldloc.1
  IL_001a:  box        class [runtime]System.Func`2<int32,int32>
  IL_001f:  stloc.2
  IL_0020:  ldc.i4.0
  IL_0021:  ret
} 
            """
            """
.class auto autochar serializable sealed nested assembly beforefieldinit specialname func@16
       extends [runtime]System.Object
            """
            ]))

#if !FX_NO_WINFORMS
    [<Fact>]
    let ``EventHandler from Regression/83``() =
        CompilerAssert.CompileLibraryAndVerifyILRealSig(
            """
module StaticMember07

// #Regression 
let failures = ref false
let report_failure () = 
  System.Console.Error.WriteLine " NO"; failures := true

open System
open System.Windows.Forms

let form = new Form()

let lblHello = new Label()
let btnSay = new Button()

let btnSay_Click(sender, e) =
    lblHello.Text <- "Hello"

let form_Load(sender, e) =
    btnSay.add_Click(new EventHandler(fun sender e -> btnSay_Click(sender, e)))



let _ = lblHello.Location <- new System.Drawing.Point(16, 16)
let _ = lblHello.Name <- "lblHello"
let _ = lblHello.Size <- new System.Drawing.Size(72, 23)
let _ = lblHello.TabIndex <- 0

let _ = btnSay.Location <- new System.Drawing.Point(216, 16)
let _ = btnSay.Name <- "btnApply"
let _ = btnSay.TabIndex <- 1
let _ = btnSay.Text <- "Apply"

let _ = form.Text <- "1st F# App"
let _ = form.add_Load(new EventHandler(fun sender e -> form_Load(sender, e)))
let _ = form.Controls.AddRange(Array.ofList [(upcast (lblHello) : Control);
                                            (upcast (btnSay) : Control);
                                                        ])
 
(* let _ = Application.Run(form) *)

let _ = 
  if !failures then (System.Console.Out.WriteLine "Test Failed"; exit 1) 

do (System.Console.Out.WriteLine "Test Passed"; 
    printf "TEST PASSED OK" ; 
    exit 0)
            """,
            (fun verifier -> verifier.VerifyIL [
            """
IL_00bc:  ldnull
IL_00bd:  ldftn      void StaticMember07/clo@36::Invoke(object,
                                                        class [runtime]System.EventArgs)
IL_00c3:  newobj     instance void [runtime]System.EventHandler::.ctor(object,
                                                                       native int)
IL_00c8:  callvirt   instance void [System.Windows.Forms]System.Windows.Forms.Form::add_Load(class [runtime]System.EventHandler)
            """
            """
.class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname clo@36
       extends [runtime]System.Object
            """
            ]))
#endif
