module Language.NullableCSharpImport

open FSharp.Test
open Xunit
open FSharp.Test.Compiler

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
    |> withLangVersionPreview
    |> withReferences [csharpLib]
    |> withCheckNulls
    |> withWarnOn 3261
    |> withOptions ["--warnaserror+"]
    |> compile
    |> shouldFail
    |> withDiagnostics [
            Error 3261, Line 5, Col 40, Line 5, Col 85, "Nullness warning: The types 'string' and 'string | null' do not have compatible nullability."
            Error 3261, Line 5, Col 40, Line 5, Col 85, "Nullness warning: The types 'string' and 'string | null' do not have equivalent nullability."
            Error 3261, Line 14, Col 34, Line 14, Col 62, "Nullness warning: The types 'string' and 'string | null' do not have equivalent nullability."
            Error 3261, Line 16, Col 35, Line 16, Col 39, "Nullness warning: The type 'string' does not support 'null'."
            Error 3261, Line 25, Col 85, Line 25, Col 97, "Nullness warning: The types 'string' and 'string | null' do not have equivalent nullability."
            Error 3261, Line 28, Col 99, Line 28, Col 111, "Nullness warning: The types 'string' and 'string | null' do not have equivalent nullability."
            Error 3261, Line 30, Col 97, Line 30, Col 109, "Nullness warning: The types 'string' and 'string | null' do not have equivalent nullability."]
            
