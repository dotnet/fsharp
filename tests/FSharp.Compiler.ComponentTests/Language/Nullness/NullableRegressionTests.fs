module Language.NullableRegressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let withVersionAndCheckNulls (version,checknulls) cu =
    cu
    |> withLangVersion version
    |> withWarnOn 3261
    |> withOptions ["--warnaserror+"]
    |> if checknulls then withCheckNulls else id

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Issue 20211 - deferred union attributes preserve overload selection`` checknulls =
    FSharp """
module Probe

type Methods =
    static member Choose<'T when 'T : not null>(_: 'T, _: System.IComparable) = 1
    static member Choose<'T>(_: 'T, _: System.IFormattable) = 2

[<Rep(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type U = Nil | Node of int
and [<System.ComponentModel.Description(nameof (Methods.Choose : U * int -> int))>]
    Consumer = class end
and RepAttribute = CompilationRepresentationAttribute
"""
    |> asLibrary
    |> withVersionAndCheckNulls ("preview", checknulls)
    |> withOptions [if checknulls then "--checknulls+" else "--checknulls-"; "--warnaserror-"]
    |> typecheck
    |> shouldSucceed
    |> withDiagnostics []

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``Issue 20211 - union annotation warns once after attributes resolve`` compileSource multipleAnnotations =
    let extraAnnotation = if multipleAnnotations then "let other : NN<U> = Unchecked.defaultof<_>" else ""
    let message = "Nullness warning: The type 'U' uses 'null' as a representation value but a non-null type is expected."
    FSharp $"""
module rec Duplicate

type NN<'T when 'T : not null> = {{ Value: 'T }}

[<Repr(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type U = Nil | Node of int

type Repr = CompilationRepresentationAttribute
let value : NN<U> = Unchecked.defaultof<_>
{extraAnnotation}
"""
    |> asLibrary
    |> withLangVersionPreview
    |> withCheckNulls
    |> (if compileSource then compile else typecheck)
    |> shouldFail
    |> withDiagnostics [
        Warning 3261, Line 10, Col 13, Line 10, Col 18, message
        if multipleAnnotations then
            Warning 3261, Line 11, Col 13, Line 11, Col 18, message
    ]
    |> fun result -> Assert.Equal((if multipleAnnotations then 2 else 1), result.Output.Diagnostics.Length)


[<Theory>]
[<InlineData("""
module rec M

open System.Collections.Generic

[<Struct>]
type Hole = Hole of string with
    member this.Value =
        let (Hole value) = this in value

type Substitution = Dictionary<Hole,obj>
""", true)>]
[<InlineData("""
module M

open System.Collections.Generic

[<Struct>]
type Hole = Hole of string with
    member this.Value =
        let (Hole value) = this in value

and Substitution = Dictionary<Hole,obj>
""", true)>]
[<InlineData("""
module rec M
open System.Collections.Generic
type Hole = Hole of string
type Substitution = Dictionary<Hole,obj>
""", true)>]
[<InlineData("""
module rec M
open System.Collections.Generic
[<Struct>]
type Hole<'T> = Hole of 'T
type Substitution = Dictionary<Hole<string | null>,obj>
""", true)>]
[<InlineData("""
module rec M
open System.Collections.Generic
[<NoComparison>]
type Container = { Values: Dictionary<Hole,obj> }
[<Struct>]
type Hole = Hole of string
""", true)>]
[<InlineData("""
module rec M
type Keyed<'T when 'T : not null>() = class end
[<Struct>]
type Hole = Hole of string
type Substitution = Keyed<Hole>
""", false)>]
[<InlineData("""
module M

open System.Collections.Generic

[<Struct>]
type Hole = Hole of string with
    member this.Value =
        let (Hole value) = this in value

type Substitution = Dictionary<Hole,obj>
""", true)>]
[<InlineData("""
module M
open System.Collections.Generic
type Substitution = Dictionary<Choice<string,int>,obj>
""", true)>]
let ``Issue 20211 - ordinary union constraints during declaration checking`` source checknulls =
    FSharp source
    |> asLibrary
    |> withVersionAndCheckNulls ("preview", checknulls)
    |> (if checknulls then id else withOptions ["--checknulls-"])
    |> typecheck
    |> shouldSucceed
    |> withDiagnostics []

[<TheoryForNETCOREAPP>]
[<InlineData("""
module M
open System.Collections.Generic
type Substitution = Dictionary<Maybe,obj>
and [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>] Maybe =
    | Missing
    | Present of string
""", 42, "Nullness warning: The type 'Maybe' uses 'null' as a representation value but a non-null type is expected.")>]
[<InlineData("""
module rec M
open System.Collections.Generic
type Substitution = Dictionary<Maybe,obj>
[<Rep(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type Maybe = Missing | Present of string
type RepAttribute = CompilationRepresentationAttribute
""", 42, "Nullness warning: The type 'Maybe' uses 'null' as a representation value but a non-null type is expected.")>]
[<InlineData("""
module M
open System.Collections.Generic
type Substitution = Dictionary<string option,obj>
""", 50, "Nullness warning: The type 'string option' uses 'null' as a representation value but a non-null type is expected.")>]
[<InlineData("""
module M
open System.Collections.Generic
type Substitution = Dictionary<(string | null),obj>
""", 52, "Nullness warning: The type 'string | null' supports 'null' but a non-null type is expected.")>]
let ``Issue 20211 - nullable constrained keys still warn`` source endColumn message =
    FSharp source
    |> asLibrary
    |> withVersionAndCheckNulls ("preview", true)
    |> typecheck
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 4, Col 21, Line 4, Col endColumn, message]

[<TheoryForNETCOREAPP>]
[<InlineData("module rec M", "", false)>]
[<InlineData("module rec M", "[<Struct>]", false)>]
[<InlineData("module rec M", "[<CompilationRepresentation(CompilationRepresentationFlags.None)>]", false)>]
[<InlineData("module rec M", "[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]", true)>]
[<InlineData("namespace rec M", "[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]", true)>]
[<InlineData("module M", "[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]", true)>]
let ``Issue 20211 - union constraints in early attribute arguments`` (scope: string) (attributes: string) expectWarning =
    let result =
        FSharp $"""
{scope}
open System.Collections.Generic
open System.ComponentModel
{attributes}
type U = N | S of string
[<TypeConverter(typeof<Dictionary<U,obj>>)>]
type C = class end
"""
        |> asLibrary
        |> withVersionAndCheckNulls ("preview", true)
        |> typecheck

    if expectWarning then
        result
        |> shouldFail
        |> withDiagnostics [
            Error 3261, Line 7, Col 24, Line 7, Col 41, "Nullness warning: The type 'U' uses 'null' as a representation value but a non-null type is expected."
        ]
    else
        result |> shouldSucceed |> withDiagnostics []

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``Issue 20211 - repeated nullable fields of finalized ordinary unions`` isSignature hasNullaryCase =
    let cases =
        [ if hasNullaryCase then yield "| Missing"
          for i in 1..64 -> $"| Case{i} of int" ]
        |> String.concat "\n"
    let fields =
        [ for i in 1..64 -> $"Field{i}: Token | null" ]
        |> String.concat "; "

    $"""
module M
type Token =
{cases}
type Envelope = {{ {fields} }}
"""
    |> (if isSignature then Fsi else FSharp)
    |> withName (if isSignature then "test.fsi" else "test.fs")
    |> asLibrary
    |> withVersionAndCheckNulls ("preview", true)
    |> typecheck
    |> shouldSucceed
    |> withDiagnostics []

[<Theory>]
[<InlineData(false, false, "Repr", "UseNullAsTrueValue")>]
[<InlineData(false, true, "Repr", "UseNullAsTrueValue")>]
[<InlineData(true, false, "Repr", "UseNullAsTrueValue")>]
[<InlineData(true, true, "Repr", "UseNullAsTrueValue")>]
[<InlineData(false, false, "Repr", "None")>]
[<InlineData(false, true, "Repr", "None")>]
[<InlineData(true, false, "Repr", "None")>]
[<InlineData(true, true, "Repr", "None")>]
[<InlineData(false, false, "CompilationRepresentation", "UseNullAsTrueValue")>]
[<InlineData(false, true, "CompilationRepresentation", "UseNullAsTrueValue")>]
[<InlineData(true, false, "CompilationRepresentation", "UseNullAsTrueValue")>]
[<InlineData(true, true, "CompilationRepresentation", "UseNullAsTrueValue")>]
let ``Issue 20211 - record constraints before representation attributes resolve`` isSignature unionFirst (attribute: string) (flags: string) =
    let union = $"[<{attribute}(CompilationRepresentationFlags.{flags})>]"
    let declarations =
        if unionFirst then
            $"{union}\ntype U = Nil | Node of int\nand R = {{ Item: NN<U> }}"
        else
            $"type R = {{ Item: NN<U> }}\nand {union} U = Nil | Node of int"

    let result =
        $"""
module M

type NN<'T when 'T : not null> = {{ Value: 'T }}
{declarations}
and Repr = CompilationRepresentationAttribute
"""
        |> (if isSignature then Fsi else FSharp)
        |> withName (if isSignature then "test.fsi" else "test.fs")
        |> asLibrary
        |> withVersionAndCheckNulls ("preview", true)
        |> typecheck

    if flags = "UseNullAsTrueValue" then
        let line, column = if unionFirst then 7, 17 else 5, 18
        result
        |> shouldFail
        |> withDiagnostics [
            Error 3261, Line line, Col column, Line line, Col (column + 5), "Nullness warning: The type 'U' uses 'null' as a representation value but a non-null type is expected."
        ]
    else
        result |> shouldSucceed |> withDiagnostics []

[<TheoryForNETCOREAPP>]
[<InlineData("module rec M", "UseNullAsTrueValue", false)>]
[<InlineData("namespace rec M", "UseNullAsTrueValue", false)>]
[<InlineData("module M", "UseNullAsTrueValue", false)>]
[<InlineData("module rec M", "None", false)>]
[<InlineData("module rec M", "UseNullAsTrueValue", true)>]
[<InlineData("namespace rec M", "UseNullAsTrueValue", true)>]
[<InlineData("module M", "UseNullAsTrueValue", true)>]
[<InlineData("module rec M", "None", true)>]
let ``Issue 20211 - union constraints with deferred representation attributes`` (scope: string) (flags: string) isSignature =
    let result =
        $"""
{scope}

type RepAttribute = CompilationRepresentationAttribute

[<Rep(CompilationRepresentationFlags.{flags})>]
type U = A | B of int

[<System.ComponentModel.TypeConverter(
    typeof<System.Collections.Generic.Dictionary<U, obj>>)>]
type T = T
"""
        |> (if isSignature then Fsi else FSharp)
        |> withName (if isSignature then "test.fsi" else "test.fs")
        |> asLibrary
        |> withVersionAndCheckNulls ("preview", true)
        |> typecheck

    if flags = "UseNullAsTrueValue" then
        result
        |> shouldFail
        |> withDiagnostics [
            Error 3261, Line 10, Col 12, Line 10, Col 57, "Nullness warning: The type 'U' uses 'null' as a representation value but a non-null type is expected."
        ]
    else
        result |> shouldSucceed |> withDiagnostics []

[<Theory>]
[<InlineData("preview",true)>]
[<InlineData("preview",false)>]
[<InlineData("8.0",false)>]
let ``Micro compilation`` langVersion checknulls =

    FsFromPath (__SOURCE_DIRECTORY__ ++ "micro.fsi")
    |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "micro.fs"))
    |> withLangVersion langVersion
    |> fun x ->
        if checknulls then
            x |> withCheckNulls |> withDefines ["CHECKNULLS"]
        else x
    |> compile
    |> shouldSucceed

[<Theory>]
[<InlineData("preview",true)>]
let ``Signature conformance`` langVersion checknulls =

    FsFromPath (__SOURCE_DIRECTORY__ ++ "signatures.fsi")
    |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "signatures.fs"))
    |> withLangVersion langVersion
    |> fun x ->
        if checknulls then
            x |> withCheckNulls |> withDefines ["CHECKNULLS"]
        else x
    |> compile
    |> shouldFail
    |> withDiagnostics
        [Warning 3262, Line 18, Col 48, Line 18, Col 60, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."
         (Warning 3261, Line 4, Col 5, Line 4, Col 10, "Nullness warning: Module 'M' contains
            val test2: x: (string | null) -> unit    
        but its signature specifies
            val test2: string -> unit    
        The types differ in their nullness annotations");
        (Warning 3261, Line 3, Col 5, Line 3, Col 10, "Nullness warning: Module 'M' contains
            val test1: x: string -> unit    
        but its signature specifies
            val test1: string | null -> unit    
        The types differ in their nullness annotations");
        (Warning 3261, Line 6, Col 5, Line 6, Col 17, "Nullness warning: Module 'M' contains
            val iRejectNulls: x: (string | null) -> string    
        but its signature specifies
            val iRejectNulls: string -> string    
        The types differ in their nullness annotations");
        (Warning 3261, Line 14, Col 14, Line 14, Col 21, "Nullness warning: Module 'M' contains
            member GenericContainer.GetNull: unit -> 'T    
        but its signature specifies
            member GenericContainer.GetNull: unit -> 'T | null    
        The types differ in their nullness annotations");
        (Warning 3261, Line 15, Col 14, Line 15, Col 24, "Nullness warning: Module 'M' contains
            member GenericContainer.GetNotNull: unit -> 'T | null    
        but its signature specifies
            member GenericContainer.GetNotNull: unit -> 'T    
        The types differ in their nullness annotations")]

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive v8 disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("8.0",false)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive vPreview disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive vPreview enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative v8 disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("8.0",false)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative vPreview disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative vPreview enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"library-functions.fs"|])>]
let ``Library functions nullness disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"library-functions.fs"|])>]
let ``Library functions nullness enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"using-nullness-syntax-positive.fs"|])>]
let ``With new nullness syntax nullness disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"positive-defaultValue-bug.fs"|])>]
let ``DefaultValueBug when checknulls is disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)
    |> typecheck
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"using-nullness-syntax-positive.fs"|])>]
let ``With new nullness syntax nullness enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)
    |> typecheck
    |> verifyBaseline

// https://github.com/dotnet/fsharp/issues/18288
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"inference-problem-size-explosion.fs"|])>]
let ``Inference problem limit regression previewNullness`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)
    |> withNoWarn 475 // The constraints 'struct' and 'null' are inconsistent
    |> typecheck
    |> shouldSucceed

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"inference-problem-size-explosion.fs"|])>]
let ``Inference problem limit regression previewNoNullness`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)
    |> withNoWarn 475 // The constraints 'struct' and 'null' are inconsistent
    |> typecheck
    |> shouldSucceed

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"inference-problem-size-explosion.fs"|])>]
let ``Inference problem limit regression v8`` compilation =
    compilation
    |> withVersionAndCheckNulls ("8.0",false)
    |> withNoWarn 475 // The constraints 'struct' and 'null' are inconsistent
    |> typecheck
    |> shouldSucceed

[<Theory>]
[<InlineData("preview",true,true)>]
[<InlineData("preview",true,false)>]
[<InlineData("preview",false,true)>]
[<InlineData("preview",false,false)>]
[<InlineData("8.0",false,false)>]
[<InlineData("8.0",false,true)>]
let ``DefaultValue regression`` (version,checknulls,fullCompile) =
    FSharp $"""
module MyLib

[<Struct;NoComparison;NoEquality>]
type C7 =
    [<DefaultValue>]
    val mutable Whoops : (int -> int) {if version="preview" then " | null" else ""} // no warnings in checknulls+
    """
    |> asLibrary
    |> withVersionAndCheckNulls (version,checknulls)
    |> (if fullCompile then compile else typecheck)
    |> fun x ->
        if checknulls then
            x |> shouldSucceed
        else
            x
            |> shouldFail
            |> withDiagnostics
                [(Error 444, Line 7, Col 17, Line 7, Col 23, "The type of a field using the 'DefaultValue' attribute must admit default initialization, i.e. have 'null' as a proper value or be a struct type whose fields all admit default initialization. You can use 'DefaultValue(false)' to disable this check")]

[<Fact>]
let ``Issue 19644 - match-null narrowing inside list comprehension`` () =
    FSharp """
module M

let lengths (xs: (string | null)[]) = [
    for x in xs do
        match x with
        | null -> ()
        | s -> yield s.Length
]
"""
    |> withVersionAndCheckNulls ("preview", true)
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Issue 19644 - match-null narrowing inside seq expression`` () =
    FSharp """
module M
let v (xs: (string | null) seq) =
    seq {
        for x in xs do
            match x with
            | null -> ()
            | y -> yield y.Length
    }
"""
    |> withVersionAndCheckNulls ("preview", true)
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Issue 19644 - match-null narrowing inside array comprehension`` () =
    FSharp """
module M
let v (xs: (string | null)[]) = [|
    for x in xs do
        match x with
        | null -> ()
        | y -> yield y.Length
|]
"""
    |> withVersionAndCheckNulls ("preview", true)
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Issue 19644 - match-null narrowing inside list comprehension (no for)`` () =
    FSharp """
module M

let v (s: string | null) = [
    match s with
    | null -> ()
    | x -> yield x.Length
]
"""
    |> withVersionAndCheckNulls ("preview", true)
    |> compile
    |> shouldSucceed
