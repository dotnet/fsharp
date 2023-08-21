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
            public static string? ReturnsNullableStringNoParams() { return null; }
            public static string? ReturnsNullableString1NullableParam(string? _) { return null; }
            public static string? ReturnsNullableString1NonNullableParam(string _) { return null; }
            public static string? ReturnsNullableString2NullableParams(string? _, string? __) { return null; }
            public static string? ReturnsNullableString2NonNullableParams(string _, string __) { return null; }
            public static string? ReturnsNullableString1Nullable1NonNullableParam(string? _, string __) { return null; }
            
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
    let nonNullableStrNoParams : string __withnull = NullableClass.ReturnsNonNullableStringNoParams()

    let nullablestrNoParamsCorrectlyAnnotated : string __withnull = NullableClass.ReturnsNullableStringNoParams()
    let nonNullableStrNoParamsCorrectlyAnnotated : string = NullableClass.ReturnsNonNullableStringNoParams()

    ignore nullablestrNoParams
    ignore nonNullableStrNoParams
    """
    |> asLibrary
    |> withLangVersionPreview
    |> withReferences [lib]
    |> compile
    |> shouldFail
    |> withDiagnostics [
        Warning 3261, Line 4, Col 39, Line 4, Col 84, "Nullness warning: The types 'string' and 'string __withnull' do not have compatible nullability.";
        Warning 3261, Line 5, Col 40, Line 5, Col 85, "Nullness warning: The types 'string' and 'string __withnull' do not have equivalent nullability."
    ]
