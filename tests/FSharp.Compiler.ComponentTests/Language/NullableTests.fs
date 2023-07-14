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
            public static string? ReturnsNullableString() { return null; }
            public static string ReturnsNonNullbleString() { return ""; }
        }
    }""" |> withName "csNullableLib"

    FSharp """
    module FSNullable
    open Nullables
    let nullablestr = NullableClass.ReturnsNullableString()
    let nonNullableStr = NullableClass.ReturnsNonNullbleString()
    ignore nullablestr
    ignore nonNullableStr
    """
    |> asLibrary
    |> withLangVersionPreview
    |> withReferences [lib]
    |> compile
    |> shouldSucceed
