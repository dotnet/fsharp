namespace FSharp.Compiler.ComponentTests.EmittedIL

module StructDefensiveCopy = 

    open Xunit
    open System.IO
    open FSharp.Test
    open FSharp.Test.Compiler

    let verifyKeyValuePairInstanceMethodCall expectedIl =
        FSharp """
    module StructUnion01
    open System.Runtime.CompilerServices
    open System.Collections.Generic

    let doWork(kvp1:inref<KeyValuePair<int,int>>) =
        kvp1.ToString()
        """
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> verifyIL expectedIl

    #if NETSTANDARD 
    // KeyValuePair defined as a readonly struct (in C#)
    [<Fact>]
    let ``Defensive copy can be skipped on read-only structs``() =
        verifyKeyValuePairInstanceMethodCall ["""      .method public static string  doWork([in] valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>& kvp1) cil managed
      {
        .param [1]
        .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
    
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  constrained. valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>
        IL_0007:  callvirt   instance string [runtime]System.Object::ToString()
        IL_000c:  ret
      } 

    } """]

    #else 
    // KeyValuePair just a regular struct. Notice the "ldobj" instruction
    [<Fact>]
    let ``Non readonly struct needs a defensive copy``() =
        verifyKeyValuePairInstanceMethodCall ["""      .method public static string  doWork([in] valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>& kvp1) cil managed
          {
            .param [1]
            .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        
            .maxstack  3
            .locals init (valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32> V_0)
            IL_0000:  ldarg.0
            IL_0001:  ldobj      valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>
            IL_0006:  stloc.0
            IL_0007:  ldloca.s   V_0
            IL_0009:  constrained. valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>
            IL_000f:  callvirt   instance string [runtime]System.Object::ToString()
            IL_0014:  ret
          } """]
    #endif

    let verifyDateTimeExtensionMethodCall expectedIl =
        FSharp """
    module DateTimeExtensionMethod

    open System
    open System.Collections.Generic
    open System.Runtime.CompilerServices

    [<Extension>]
    type DateTimeExtensions =
        [<Extension>]
        static member PrintDate(d: inref<DateTime>) = d.ToString()
    
    let doWork(dt:inref<DateTime>) =
        dt.PrintDate()
        """
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> verifyIL expectedIl

    #if NETSTANDARD 
    // DateTime defined as a readonly struct (in C#)
    [<Fact>]
    let ``Defensive copy can be skipped for extension methods on read-only structs``() =
        verifyDateTimeExtensionMethodCall ["""      .method public static string  doWork([in] valuetype [runtime]System.DateTime& dt) cil managed
          {
            .param [1]
            .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        
            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  constrained. [runtime]System.DateTime
            IL_0007:  callvirt   instance string [runtime]System.Object::ToString()
            IL_000c:  ret
          } """]

    #else 
    // DateTime just a regular struct. Notice the "ldobj" instruction
    [<Fact>]
    let ``Non readonly struct needs a defensive copy when its extension method is called``() =
        verifyDateTimeExtensionMethodCall ["""      .method public static string  doWork([in] valuetype [runtime]System.DateTime& dt) cil managed
          {
            .param [1]
            .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        
            .maxstack  3
            .locals init (valuetype [runtime]System.DateTime& V_0,
                     valuetype [runtime]System.DateTime V_1)
            IL_0000:  ldarg.0
            IL_0001:  stloc.0
            IL_0002:  ldloc.0
            IL_0003:  ldobj      [runtime]System.DateTime
            IL_0008:  stloc.1
            IL_0009:  ldloca.s   V_1
            IL_000b:  constrained. [runtime]System.DateTime
            IL_0011:  callvirt   instance string [runtime]System.Object::ToString()
            IL_0016:  ret
          } """]
    #endif


    #if NETSTANDARD 
    [<Fact>]
    #endif
    let ``Csharp extension method on a readonly struct does not need defensive copy``() =
        let csLib = 
            CSharp """
    using System;
    public static class DateTimeExtensionMethod
    {
            public static string CustomPrintDate(this in DateTime d)
            {
                return d.Date.ToShortDateString();
            }
    }"""    |> withName "CsLib"

        FSharp """
    module DateTimeDefinedInCsharpUsage
    open System
    let doWork(dt:inref<DateTime>) =
        dt.CustomPrintDate()
        """
        |> withReferences [csLib]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> verifyIL ["""      .method public static string  doWork([in] valuetype [runtime]System.DateTime& dt) cil managed
          {
            .param [1]
            .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        
            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  call       string [CsLib]DateTimeExtensionMethod::CustomPrintDate(valuetype [runtime]System.DateTime&)
            IL_0006:  ret
          } """]

