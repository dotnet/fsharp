module Diagnostics.InlineArray

open Xunit
open FSharp.Test.Compiler

// [<InlineArray(N)>] structs: generated structural members enumerate declared fields only, so the
// elements the runtime replicates are invisible to them. .NET refuses such types outright
// (ValueType.Equals throws NotSupportedException), so the compiler refuses to generate the members.

[<Fact>]
let ``Structural members on an InlineArray struct are an error`` () =
    FSharp """
module M

open System.Runtime.CompilerServices

[<Struct; InlineArray(4)>]
type Buf4 =
    val mutable Element0: int
"""
    |> compile
    |> shouldFail
    |> withErrorCode 3926

[<Fact>]
let ``NoEquality and NoComparison on an InlineArray struct compiles`` () =
    FSharp """
module M

open System.Runtime.CompilerServices

[<Struct; InlineArray(4); NoEquality; NoComparison>]
type Buf4 =
    val mutable Element0: int
"""
    |> compile
    |> shouldSucceed

[<Fact>]
let ``CustomEquality and NoComparison on an InlineArray struct compiles`` () =
    FSharp """
module M

open System.Runtime.CompilerServices

[<Struct; InlineArray(4); CustomEquality; NoComparison>]
type Buf4 =
    val mutable Element0: int
    override this.Equals(o: obj) = true
    override this.GetHashCode() = 0
"""
    |> compile
    |> shouldSucceed

// Equality is still generated here, so the error must still fire: the suppression is per relation.
[<Fact>]
let ``NoComparison alone still generates equality and is an error`` () =
    FSharp """
module M

open System.Runtime.CompilerServices

[<Struct; InlineArray(4); NoComparison>]
type Buf4 =
    val mutable Element0: int
"""
    |> compile
    |> shouldFail
    |> withErrorCode 3926

// A field type without equality means F# generates no members, so there is nothing to diagnose.
[<Fact>]
let ``A field type without equality generates nothing and compiles`` () =
    FSharp """
module M

open System.Runtime.CompilerServices

[<Struct; InlineArray(4); NoEquality; NoComparison>]
type Row4 =
    val mutable Element0: float32

[<Struct; InlineArray(4)>]
type Buf4 =
    val mutable Element0: int -> int
"""
    |> withNoWarn 1178
    |> compile
    |> shouldSucceed

// Byref-like structs never get generated members, so no diagnostic and no impossible advice.
[<Fact>]
let ``A byref-like InlineArray struct compiles`` () =
    FSharp """
module M

open System.Runtime.CompilerServices

[<Struct; IsByRefLike; InlineArray(4)>]
type SpanBuf4 =
    val mutable Element0: int
"""
    |> compile
    |> shouldSucceed

[<Fact>]
let ``A signature-file InlineArray struct reports once`` () =
    Fsi """
module M

open System.Runtime.CompilerServices

[<Struct; InlineArray(4)>]
type Buf4 =
    val mutable Element0: int
"""
    |> withAdditionalSourceFile (FsSource "module M\n")
    |> compile
    |> shouldFail
    |> withErrorCode 3926

// A struct record reaches the generated members through the record path, not the val path.
[<Fact>]
let ``A struct record with the attribute is an error`` () =
    FSharp """
module M

open System.Runtime.CompilerServices

[<Struct; InlineArray(4)>]
type RecBuf4 = { mutable Element0: int }
"""
    |> compile
    |> shouldFail
    |> withErrorCode 3926

[<Fact>]
let ``A struct declared with one field per element is unaffected`` () =
    FSharp """
module M

[<Struct>]
type Four =
    val mutable E0: int
    val mutable E1: int
    val mutable E2: int
    val mutable E3: int
"""
    |> compile
    |> shouldSucceed
