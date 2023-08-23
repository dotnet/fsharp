module Language.NullableTests

open Xunit
open FSharp.Test.Compiler

let inline compile c =
    c
    |> withCheckNulls
    |> compile

[<Fact>]
let ``Nullable C# reference consumption`` () =
    let lib =
        CSharp """
    #nullable enable
    namespace Nullables {
        public class NullableClass {
            // Fields with nullable type
            public static string NullableField;

            // Fields with non-nullable type
            public static string? NonNullableField;

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
    
    let nullablestrNoParams : string = NullableClass.ReturnsNullableStringNoParams()
    let nonNullableStrNoParams : string __withnull = NullableClass.ReturnsNonNullableStringNoParams() // Here we don't expect any warning.

    let nullablestrNoParamsCorrectlyAnnotated : string __withnull = NullableClass.ReturnsNullableStringNoParams()
    let nonNullableStrNoParamsCorrectlyAnnotated : string = NullableClass.ReturnsNonNullableStringNoParams()

    let nullableField : string = NullableClass.NullableField
    let nonNullableField : string __withnull = NullableClass.NonNullableField

    """
    |> asLibrary
    |> withLangVersionPreview
    |> withReferences [lib]
    |> compile
    |> shouldFail
    |> withDiagnostics [
        // TODO NULLNESS: makes sure that both of these are expected.
        Warning 3261, Line 5, Col 40, Line 5, Col 85, "Nullness warning: The types 'string' and 'string __withnull' do not have compatible nullability.";
        Warning 3261, Line 5, Col 40, Line 5, Col 85, "Nullness warning: The types 'string' and 'string __withnull' do not have equivalent nullability."
    ]
