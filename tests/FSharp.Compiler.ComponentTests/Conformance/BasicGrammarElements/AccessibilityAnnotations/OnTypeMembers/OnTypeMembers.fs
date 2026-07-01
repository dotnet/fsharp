// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AccessibilityAnnotations_OnTypeMembers =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    // SOURCE=AccessProtectedStatic01.fs SCFLAGS="-r:BaseClass.dll"    PRECMD="\$CSC_PIPE /target:library BaseClass.cs"                         # AccessProtectedStatic01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AccessProtectedStatic01.fs"|])>]
    let ``AccessProtectedStatic01_fs`` compilation =
        let lib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "BaseClass.cs")
            |> withName "ReadWriteLib"

        compilation
        |> withReferences [lib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_AccessProtectedInstance01.fs SCFLAGS="-r:BaseClass.dll --test:ErrorRanges"    PRECMD="\$CSC_PIPE /target:library BaseClass.cs"  # AccessProtectedInstance01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AccessProtectedInstance01.fs"|])>]
    let ``AccessProtectedInstance01_fs`` compilation =
        let lib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "BaseClass.cs")
            |> withName "ReadWriteLib"

        compilation
        |> withReferences [lib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_AccessPrivateMember01.fs SCFLAGS="--test:ErrorRanges"          # E_AccessPrivateMember01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AccessPrivateMember01.fs"|])>]
    let ``E_AccessPrivateMember01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 9, Col 10, Line 9, Col 17, "The struct, record or union type 'SpecSet' is not structurally comparable because the type 'SpecMulti' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'SpecSet' to clarify that the type is not comparable")
            (Error 491, Line 14, Col 13, Line 14, Col 28, "The member or object constructor 'Impl' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
        ]

    // SOURCE=E_OnProperty01.fs                                                # E_OnProperty01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnProperty01.fs"|])>]
    let ``E_OnProperty01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 491, Line 42, Col 1, Line 42, Col 6, "The member or object constructor 'Foo' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 42, Col 9, Line 42, Col 16, "The member or object constructor 'test1' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 42, Col 19, Line 42, Col 26, "The member or object constructor 'test2' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 42, Col 29, Line 42, Col 36, "The member or object constructor 'test5' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 42, Col 40, Line 42, Col 47, "The member or object constructor 'test6' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 807, Line 42, Col 62, Line 42, Col 67, "Property 'test8' is not readable")
            (Error 491, Line 43, Col 1, Line 43, Col 13, "The member or object constructor 'test3' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 44, Col 1, Line 44, Col 13, "The member or object constructor 'test4' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 45, Col 1, Line 45, Col 13, "The member or object constructor 'test5' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 46, Col 1, Line 46, Col 13, "The member or object constructor 'test6' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 810, Line 47, Col 3, Line 47, Col 8, "Property 'test7' cannot be set")
            (Error 257, Line 47, Col 1, Line 47, Col 8, "Invalid mutation of a constant expression. Consider copying the expression to a mutable local, e.g. 'let mutable x = ...'.")
        ]

    // SOURCE=E_OnProperty02.fs SCFLAGS="--test:ErrorRanges"                   # E_OnProperty02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnProperty02.fs"|])>]
    let ``E_OnProperty02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 15, Col 49, Line 15, Col 50, "Unexpected symbol ')' in pattern")
            (Error 1244, Line 15, Col 48, Line 15, Col 50, "Attempted to parse this as an operator name, but failed")
            (Error 558, Line 16, Col 36, Line 16, Col 50, "When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.")
            (Error 558, Line 19, Col 35, Line 19, Col 56, "When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.")
            (Error 10, Line 20, Col 49, Line 20, Col 50, "Unexpected identifier in pattern")
            (Error 1244, Line 20, Col 48, Line 20, Col 57, "Attempted to parse this as an operator name, but failed")
            (Error 10, Line 23, Col 36, Line 23, Col 42, "Unexpected keyword 'public' in member definition")
            (Error 3567, Line 23, Col 36, Line 23, Col 42, "Expecting member body")
        ]

    // SOURCE=OnProperty01.fs                                                  # OnProperty01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OnProperty01.fs"|])>]
    let ``OnProperty01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // C# base class with protected instance/static fields, shared by the issue 19963 regressions below.
    let baseClassLib =
        CSharpFromPath (__SOURCE_DIRECTORY__ ++ "BaseClass.cs") |> withName "BaseClassLib"

    // #19963: the optimizer must not relocate a protected base-field read out of its family
    // (it inlined a trivial member into startup code under --optimize+ → FieldAccessException).
    [<Fact>]
    let ``Protected base field read via optimized member does not crash (issue 19963)`` () =
        FSharp """
open TestBaseClass

type DerivedClass() =
    inherit BaseClass()
    member x.GetField() = x.ProtectedField

[<EntryPoint>]
let main _ =
    if DerivedClass().GetField() = "protected-field" then 0 else 1
"""
        |> withReferences [baseClassLib]
        |> withOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    // ── issue #5302: read a protected base field from a closure in a derived member ──
    // A `module Test` with a derived class whose body is `body`, plus an optional `tail` (e.g. an entry point).
    let private derivedSourceTemplate = """
module Test
open TestBaseClass
type DerivedClass() =
    inherit BaseClass()
    __BODY__
__TAIL__
"""

    let private derivedSource body tail =
        derivedSourceTemplate.Replace("__BODY__", body).Replace("__TAIL__", tail)

    // A runnable program whose DerivedClass.Run() body is `expr`, with an entry point asserting `expected`.
    let private derivedReturning (expr: string) (expected: string) =
        derivedSource ("member x.Run() = " + expr) ("[<EntryPoint>]\nlet main _ = if DerivedClass().Run() = \"" + expected + "\" then 0 else 1")

    // A compile-only program with an extra member/binding `body` (for rejection tests).
    let private derivedMember body = derivedSource body ""

    // Compile and run `src` under the feature (--langversion:preview, --realsig- so a surviving closure is
    // placed by L2 rather than left in the module), tuning the pipeline with `tune` (e.g. optimization level).
    let private runsTuned tune src =
        FSharp src |> withReferences [baseClassLib] |> withLangVersionPreview |> withRealInternalSignatureOff |> tune |> asExe |> compileAndRun |> shouldSucceed

    let private runsPreview src = runsTuned withOptimize src

    // Compile `src` at the language level set by `setLang`; assert it is rejected with a message matching `diag`.
    let private rejectedAt setLang diag src =
        FSharp src |> withReferences [baseClassLib] |> setLang |> compile |> shouldFail |> withDiagnosticMessageMatches diag

    let private rejectedPreview diag src = rejectedAt withLangVersionPreview diag src

    // Every ordinary closure shape can read a protected base field. Run under --realsig- (so the closure is
    // placed by L2, not left in the module) and --optimize+ (so a recursive closure exercises the L3 TLR-lift
    // refusal); easier realsig/optimize modes are strictly less demanding.
    [<Theory>]
    [<InlineData("(fun () -> x.ProtectedField) ()", "protected-field")>]
    [<InlineData("let rec f n = if n = 0 then x.ProtectedField else f (n - 1) in f 4", "protected-field")>]
    [<InlineData("System.Func<string>(fun () -> x.ProtectedField).Invoke()", "protected-field")>]
    [<InlineData("async { return x.ProtectedField } |> Async.RunSynchronously", "protected-field")>]
    [<InlineData("(task { return x.ProtectedField }).Result", "protected-field")>]
    [<InlineData("seq { yield x.ProtectedField } |> Seq.head", "protected-field")>]
    [<InlineData("(function true -> x.ProtectedField | _ -> \"\") true", "protected-field")>]
    [<InlineData("(lazy x.ProtectedField).Value", "protected-field")>]
    [<InlineData("[ x.ProtectedField ] |> List.head", "protected-field")>]
    [<InlineData("[| x.ProtectedField |] |> Array.head", "protected-field")>]
    [<InlineData("let f () = BaseClass.ProtectedStaticField in f ()", "protected-static-field")>]
    [<InlineData("let f () = x.ProtectedInternalField in f ()", "protected-internal-field")>]
    [<InlineData("let rec f n = if n = 0 then (x.ProtectedField <- \"written\"; x.ProtectedField) else f (n - 1) in f 2", "written")>]
    [<InlineData("let rec f n = if n = 0 then x.ProtectedIntField.ToString() else f (n - 1) in f 3", "42")>]
    let ``Protected base field read from a closure runs (issue 5302)`` (expr: string) (expected: string) =
        runsPreview (derivedReturning expr expected)

    // L2 in isolation: under --optimize- the closure survives un-inlined and must still nest in the declaring
    // type, or the relocated field load throws FieldAccessException at runtime.
    [<Fact>]
    let ``Protected base field from a closure nests in the declaring type under optimize- (issue 5302)`` () =
        derivedReturning "let f () = x.ProtectedField in f ()" "protected-field"
        |> runsTuned (withOptimization false)

    // The closure carries the type parameter of a generic derived type (the lift is refused, not re-homed
    // into the generic type — see issues #17607 / #14492).
    [<Fact>]
    let ``Protected base field from a closure in a generic derived type runs (issue 5302)`` () =
        """
open TestBaseClass
type DerivedClass<'T>(seed: 'T) =
    inherit BaseClass()
    member x.Run() = let rec f n = if n = 0 then x.ProtectedField + string (box seed) else f (n - 1) in f 3
[<EntryPoint>]
let main _ = if DerivedClass<int>(7).Run() = "protected-field7" then 0 else 1
"""
        |> runsPreview

    // A *direct* protected base field read in an object expression deriving from BaseClass is fine — the
    // override method is on the object-expression class, which has family access. Only closures are excluded.
    [<Fact>]
    let ``Protected base field read directly in an object expression runs (issue 5302)`` () =
        """
open TestBaseClass
let make () = { new BaseClass() with override this.ToString() = this.ProtectedField }
[<EntryPoint>]
let main _ = if (make ()).ToString() = "protected-field" then 0 else 1
"""
        |> runsPreview

    // Precision: the gate narrows to *protected* fields, not any IL field. A recursive closure reading a
    // PUBLIC field (System.String.Empty) must still be lifted to a module-level static under --optimize+, so
    // the lifted static `Test::f@` is present (a closure class would be `Test/f@`).
    [<Fact>]
    let ``Public field in a recursive closure is still lifted under the feature (issue 5302)`` () =
        FSharp """
module Test
type Holder() =
    member _.Run() =
        let rec f n = if n = 0 then System.String.Empty else f (n - 1)
        f 4
"""
        |> withLangVersionPreview
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyILPresent ["Test::f@"]

    // Fields only: a protected *method* call, and a closure inside an object expression, stay rejected even
    // with the feature on (methods could escape their object scope; objexpr closures are emitted beside the
    // object-expression class, not within it, so they cannot keep family access).
    [<Theory>]
    [<InlineData("member x.M() = let f () = x.ProtectedInstance() in f ()", "could escape their object scope")>]
    [<InlineData("member x.M() = { new System.IComparable with member _.CompareTo(o) = x.ProtectedField.Length }", "field 'ProtectedField' is not accessible")>]
    let ``Protected method or object-expression closure stays rejected with the feature (issue 5302)`` (body: string) (diag: string) =
        derivedMember body |> rejectedPreview diag

    // Regression for the object-expression soundness hole: an object expression deriving from BaseClass can
    // read its own protected base field directly, but reading it from a surviving closure (here `lazy`) must
    // stay rejected — that closure is emitted beside the object-expression class, so keeping the family region
    // would type-check then throw FieldAccessException at runtime.
    [<Fact>]
    let ``Protected base field from a closure inside an object expression stays rejected (issue 5302)`` () =
        """
module Test
open TestBaseClass
let make () =
    { new BaseClass() with
        override this.ToString() = (lazy this.ProtectedField).Value }
"""
        |> rejectedPreview "field 'ProtectedField' is not accessible"

    // Without the language feature the access stays rejected (FS1097), proving the gate.
    [<Fact>]
    let ``Protected base field from a closure is rejected without the language feature (issue 5302)`` () =
        derivedMember "member x.Run() = let f () = x.ProtectedField in f ()"
        |> rejectedAt withLangVersion80 "field 'ProtectedField' is not accessible"

    // #19963, static-field (I_ldsfld) variant of the above.
    [<Fact>]
    let ``Protected static base field read via optimized member does not crash (issue 19963)`` () =
        FSharp """
open TestBaseClass

type DerivedClass() =
    inherit BaseClass()
    member x.GetStaticField() = BaseClass.ProtectedStaticField

[<EntryPoint>]
let main _ =
    if DerivedClass().GetStaticField() = "protected-static-field" then 0 else 1
"""
        |> withReferences [baseClassLib]
        |> withOptimize
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    // #19963 positive: a PUBLIC IL field (String.Empty) in an inline value must still inline under
    // --optimize+; the over-broad form FS1118'd FSharp.Core's GetStringSlice. FS1118→error guards it.
    [<Fact>]
    let ``Public IL field access inside an inline value is still optimized away (issue 19963)`` () =
        FSharp """
module Test
let inline emptyOr (s: string) = if s.Length = 0 then System.String.Empty else s

[<EntryPoint>]
let main _ =
    if emptyOr "" = "" && emptyOr "x" = "x" then 0 else 1
"""
        |> withOptimize
        |> withOptions ["--warnaserror:1118"]
        |> asExe
        |> compileAndRun
        |> shouldSucceed
