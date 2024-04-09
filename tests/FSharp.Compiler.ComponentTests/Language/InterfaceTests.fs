module Language.InterfaceTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Concrete instance method is not allowed in interfaces in lang version80``() =
    FSharp """
[<Interface>]
type I =
    member _.X () = 1
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 868, Line 4, Col 14, Line 4, Col 15, "Interfaces cannot contain definitions of concrete instance members. You may need to define a constructor on your type to indicate that the type is a class.")
    ]

[<Fact>]
let ``Concrete instance property is not allowed in interfaces in lang version80``() =
    FSharp """
[<Interface>]
type I =
    member _.Prop = "x"
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 868, Line 4, Col 14, Line 4, Col 18, "Interfaces cannot contain definitions of concrete instance members. You may need to define a constructor on your type to indicate that the type is a class.")
    ]

[<Fact>]
let ``Concrete static members are allowed in interfaces in lang version80``() =
    FSharp """
[<Interface>]
type I<'T> =
    static member Echo (x: 'T) = x
    static member Prop = Unchecked.defaultof<'T>

if I<int>.Echo 42 <> 42 || I<int>.Prop <> 0 || not (isNull I<string>.Prop) then
    failwith "failed"
    """
    |> withLangVersion80
    |> asExe
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``Concrete static members are not allowed in interfaces in lang version70``() =
    FSharp """
[<Interface>]
type I<'T> =
    static member Echo (x: 'T) = x
    static member Prop = Unchecked.defaultof<'T>
    """
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 3350, Line 4, Col 19, Line 4, Col 23, "Feature 'Static members in interfaces' is not available in F# 7.0. Please use language version 8.0 or greater.")
    ]

[<Fact>]
let ``Concrete static members are allowed in interfaces as intrinsics in lang version80``() =
    FSharp """
[<Interface>]
type I<'T> = 
    static member Prop = Unchecked.defaultof<'T>
type I<'T> with
    static member Echo (x: 'T) = x

if I<int>.Echo 42 <> 42 || I<int>.Prop <> 0 || not (isNull I<string>.Prop) then
    failwith "failed"
    """
    |> withLangVersion80
    |> asExe
    |> compileAndRun
    |> shouldSucceed


[<Fact>]
let ``Interface with concrete static members can be implemented in lang version80``() =
    FSharp """
[<Interface>]
type I =
    static member Echo (x: string) = x
    abstract member Blah: int

type Imp () =
    interface I with
        member _.Blah = 3

let o = { new I with member _.Blah = 4 }

if I.Echo "yup" <> "yup" || (Imp() :> I).Blah <> 3 || o.Blah <> 4 then
    failwith "failed"
    """
    |> withLangVersion80
    |> asExe
    |> compileAndRun
    |> shouldSucceed
