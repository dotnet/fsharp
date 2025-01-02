module Language.NullableCSharpImport

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let withStrictNullness cu =
    cu
    |> withCheckNulls
    |> withWarnOn 3261
    |> withOptions ["--warnaserror+"]

let typeCheckWithStrictNullness cu =
    cu
    |> withStrictNullness
    |> typecheck


[<FactForNETCOREAPP>]
let ``Passing null to IlGenerator BeginCatchBlock is fine`` () = 
    FSharp """module MyLibrary
open System.Reflection.Emit
open System

let mutable logRefEmitCalls = true

type ILGenerator with
    member ilG.BeginCatchBlockAndLog (ty: Type | null) =
        if logRefEmitCalls then
            printfn "ilg%d.BeginCatchBlock(%A)" (abs <| hash ilG) ty

        ilG.BeginCatchBlock ty

let doSomethingAboutIt (ilg:ILGenerator) =
    ilg.BeginCatchBlockAndLog(null)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Consuming C# generic API which allows struct and yet uses question mark on the typar`` () = 
    FSharp """module MyLibrary
let ec = 
    { new System.Collections.Generic.IEqualityComparer<int> with
          member this.Equals(x, y) = (x+0) = (y+0)
          member this.GetHashCode(obj) = obj * 2}
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``TypeBuilder CreateTypeInfo with an upcast`` () = 
    FSharp """module MyLibrary
open System
open System.Reflection.Emit

let createType (typB:TypeBuilder) : Type=
    typB.CreateTypeInfo() :> Type
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``CurrentDomain ProcessExit add to event`` () = 
    FSharp """module MyLibrary
open System

do System.AppDomain.CurrentDomain.ProcessExit |> Event.add (fun args -> failwith $"{args.GetHashCode()}")
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Consuming C# extension methods which allow nullable this`` () = 
    FSharp """module MyLibrary

open System

let asMemoryOnNonNull : Memory<byte> = 
    let bytes = [|0uy..11uy|]
    let memory = bytes.AsMemory()
    memory

let asMemoryOnNull : Memory<byte> = 
    let bytes : (byte[]) | null = null
    let memory = bytes.AsMemory()
    memory
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Consuming LinkedList First and Last should warn about nullness`` () = 
    FSharp """module MyLibrary

let ll = new System.Collections.Generic.LinkedList<string>()
let x:System.Collections.Generic.LinkedListNode<string> = ll.Last
let y:System.Collections.Generic.LinkedListNode<string> = ll.First
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
         [ Error 3261, Line 4, Col 59, Line 4, Col 66, "Nullness warning: The types 'System.Collections.Generic.LinkedListNode<string>' and 'System.Collections.Generic.LinkedListNode<string> | null' do not have compatible nullability."
           Error 3261, Line 4, Col 59, Line 4, Col 66, "Nullness warning: A non-nullable 'System.Collections.Generic.LinkedListNode<string>' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."
           Error 3261, Line 5, Col 59, Line 5, Col 67, "Nullness warning: The types 'System.Collections.Generic.LinkedListNode<string>' and 'System.Collections.Generic.LinkedListNode<string> | null' do not have compatible nullability."
           Error 3261, Line 5, Col 59, Line 5, Col 67, "Nullness warning: A non-nullable 'System.Collections.Generic.LinkedListNode<string>' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression." ]

[<FactForNETCOREAPP>]
let ``Nullable directory info show warn on prop access`` () = 
    FSharp """module MyLibrary
open System.IO
open System

let d : DirectoryInfo | null = null
let s : string = d.Name // should warn here!!
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 6, Col 18, Line 6, Col 24, "Nullness warning: The types 'DirectoryInfo' and 'DirectoryInfo | null' do not have compatible nullability."]

[<Fact>]
let ``Consumption of netstandard2 BCL api which is not annotated`` () = 
    FSharp """module MyLibrary
open System.Reflection

[<StructuralEquality; StructuralComparison>]
type PublicKey =
    | PublicKey of byte[]
    | PublicKeyToken of byte[]

let FromAssemblyName (aname: AssemblyName) =
    match aname.GetPublicKey() with
    | Null
    | NonNull [||] ->
        match aname.GetPublicKeyToken() with
        | Null
        | NonNull [||] -> None
        | NonNull bytes -> Some(PublicKeyToken bytes)
    | NonNull bytes -> Some(PublicKey bytes)"""
    |> asLibrary
    |> asNetStandard20
    |> typeCheckWithStrictNullness
    |> shouldSucceed


[<FactForNETCOREAPP>]
let ``Consumption of nullable C# - no generics, just strings in methods and fields`` () =
    let csharpLib =
        CSharp """
    #nullable enable
    namespace Nullables {
        public class NullableClass {
            // Fields with nullable type
            public static string NotNullField;
            // Fields with non-nullable type
            public static string? MaybeNullField;
            // Methods which return nullable string
            public static string? ReturnsNullableStringNoParams() { return null; }
            public static string? ReturnsNullableString1NullableParam(string? _) { return null; }
            public static string? ReturnsNullableString1NonNullableParam(string _) { return null; }
            public static string? ReturnsNullableString2NullableParams(string? _, string? __) { return null; }
            public static string? ReturnsNullableString2NonNullableParams(string _, string __) { return null; }
            public static string? ReturnsNullableString1Nullable1NonNullableParam(string? _, string __) { return null; }
            
            // Methods which return non-nullable string
            public static string ReturnsNonNullableStringNoParams() { return ""; }
            public static string ReturnsNonNullableString1NullableParam(string? _) { return ""; }
            public static string ReturnsNonNullableString1NonNullableParam(string _) { return ""; }
            public static string ReturnsNonNullableString2NullableParams(string? _, string? __) { return ""; }
            public static string ReturnsNonNullableString2NonNullableParams(string _, string __) { return ""; }
            public static string ReturnsNonNullableString1Nullable1NonNullableParam(string? _, string __) { return ""; }
        }
    }""" |> withName "csNullableLib"

    FSharp """
    module FSNullable
    open Nullables
    
    let nullablestrNoParams : string = NullableClass.ReturnsNullableStringNoParams() // Error here, line 5
    let nonNullableStrNoParams : string | null = NullableClass.ReturnsNonNullableStringNoParams()
    let nullablestrNoParamsCorrectlyAnnotated : string | null = NullableClass.ReturnsNullableStringNoParams()
    let nonNullableStrNoParamsCorrectlyAnnotated : string = NullableClass.ReturnsNonNullableStringNoParams()
    let notNullField : string = NullableClass.NotNullField
    let maybeNullField : string | null = NullableClass.MaybeNullField
    let maybeNullField2 : string | null = NullableClass.NotNullField


    let notNullField2 : string = NullableClass.MaybeNullField  // Error here, line 14
    NullableClass.MaybeNullField <- null
    NullableClass.NotNullField <- null  // Error here, line 16

    let notNullArg : string = "hello"
    let maybeNullArg : string | null = "there"

    let nullableParamOk1 = NullableClass.ReturnsNullableString1NullableParam(notNullArg)
    let nullableParamOk2 = NullableClass.ReturnsNullableString1NullableParam(maybeNullArg) 

    let nonNullParamCallPass = NullableClass.ReturnsNullableString1NonNullableParam(notNullArg)
    let nonNullParamCallFail = NullableClass.ReturnsNullableString1NonNullableParam(maybeNullArg) // Error here, 25

    let mixedParams1 = NullableClass.ReturnsNullableString1Nullable1NonNullableParam(notNullArg,notNullArg)
    let mixedParams2 = NullableClass.ReturnsNullableString1Nullable1NonNullableParam(maybeNullArg,maybeNullArg) // Error here, 28
    let mixedParams3 = NullableClass.ReturnsNullableString1Nullable1NonNullableParam(maybeNullArg,notNullArg)
    let mixedParams4 = NullableClass.ReturnsNullableString1Nullable1NonNullableParam(notNullArg,maybeNullArg) // Error here, 30


    """
    |> asLibrary
    |> withReferences [csharpLib]
    |> withStrictNullness
    |> compile
    |> shouldFail
    |> withDiagnostics [
            Error 3261, Line 5, Col 40, Line 5, Col 85, "Nullness warning: The types 'string' and 'string | null' do not have compatible nullability."
            Error 3261, Line 5, Col 40, Line 5, Col 85, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."
            Error 3261, Line 14, Col 34, Line 14, Col 62, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."
            Error 3261, Line 16, Col 35, Line 16, Col 39, "Nullness warning: The type 'string' does not support 'null'."
            Error 3261, Line 25, Col 85, Line 25, Col 97, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."
            Error 3261, Line 28, Col 99, Line 28, Col 111, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."
            Error 3261, Line 30, Col 97, Line 30, Col 109, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]
    
    
[<FactForNETCOREAPP>]
let ``Regression 17701 - Nullable value type with nested generics`` () =
    let csharpLib =
        CSharp """
using System;
using System.Collections.Immutable;
#nullable enable
namespace Nullables;
public class NullableClass {
    public static ImmutableArray<string?>? nullableImmArrayOfStrings;
    public static ImmutableArray<string>? nullableImmArrayOfNotNullStrings;
}""" |> withName "csNullableLib"
     |> withCSharpLanguageVersionPreview

    FSharp """module FSNullable
open Nullables

let nullablestrNoParams = NullableClass.nullableImmArrayOfStrings
let toOption = NullableClass.nullableImmArrayOfStrings |> Option.ofNullable
let firstString = (toOption.Value |> Seq.head)
let lengthOfIt = firstString.Length

let theOtherOne = NullableClass.nullableImmArrayOfNotNullStrings
    """        
    |> asLibrary
    |> withReferences [csharpLib]
    |> withStrictNullness
    |> withLangVersionPreview
    |> compile
    |> shouldFail
    |> withDiagnostics 
                [Error 3261, Line 7, Col 18, Line 7, Col 36, "Nullness warning: The types 'string' and 'string | null' do not have compatible nullability."]
            
            
