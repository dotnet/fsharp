module Language.NullableTests

open Xunit
open FSharp.Test.Compiler

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
    let nullablestrNoParams = NullableClass.ReturnsNullableStringNoParams()
    let nonNullableStrNoParams = NullableClass.ReturnsNonNullableStringNoParams()
    ignore nullablestr
    ignore nonNullableStr
    """
    |> asLibrary
    |> withLangVersionPreview
    |> withReferences [lib]
    |> compile
    |> shouldSucceed
